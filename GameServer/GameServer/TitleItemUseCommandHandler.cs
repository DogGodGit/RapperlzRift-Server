using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class TitleItemUseCommandHandler : InGameCommandHandler<TitleItemUseCommandBody, TitleItemUseResponseBody>
{
	public const short kResult_NotEnoughItem = 101;

	public const short kResult_AlreadyAcquiredTitle = 102;

	private HeroTitle m_addedHeroTitle;

	private InventorySlot m_targetInventroySlot;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nSlotIndex = m_body.slotIndex;
		if (nSlotIndex < 0)
		{
			throw new CommandHandleException(1, "슬롯인덱스가 유효하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		m_targetInventroySlot = m_myHero.GetInventorySlot(nSlotIndex);
		if (m_targetInventroySlot == null)
		{
			throw new CommandHandleException(1, "인벤토리슬롯이 유효하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		if (m_targetInventroySlot.isEmpty)
		{
			throw new CommandHandleException(1, "빈 슬롯입니다. nSlotIndex = " + nSlotIndex);
		}
		if (m_targetInventroySlot.obj.inventoryObjectType != 3)
		{
			throw new CommandHandleException(1, "슬롯오브젝트타입이 아이템이 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		ItemInventoryObject itemInventoryObj = (ItemInventoryObject)m_targetInventroySlot.obj;
		Item targetItem = itemInventoryObj.item;
		bool bItemOwned = itemInventoryObj.owned;
		if (itemInventoryObj.count < 1)
		{
			throw new CommandHandleException(101, "아이템이 부족합니다.");
		}
		if (!targetItem.IsUseableHeroLevel(m_myHero.level))
		{
			throw new CommandHandleException(1, "레벨이 부족합니다.");
		}
		if (targetItem.type.id != 25)
		{
			throw new CommandHandleException(1, "해당 아이템은 칭호 아이템이 아닙니다.");
		}
		Title title = Resource.instance.GetTitle(targetItem.value1);
		if (m_myHero.ContainsTitle(title.id))
		{
			throw new CommandHandleException(102, "이미 추가된 칭호입니다. titleId = " + title.id);
		}
		m_addedHeroTitle = new HeroTitle(m_myHero, title, m_currentTime);
		m_myHero.AddTitle(m_addedHeroTitle);
		m_myHero.UseItem(nSlotIndex, 1);
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SaveToGameLogDB(targetItem.id, 1, bItemOwned);
		TitleItemUseResponseBody resBody = new TitleItemUseResponseBody();
		resBody.titleId = m_addedHeroTitle.title.id;
		resBody.remainingTime = m_addedHeroTitle.GetRemainingTime(m_currentTime);
		resBody.changedInventorySlot = m_targetInventroySlot.ToPDInventorySlot();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_targetInventroySlot));
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroTitle(m_addedHeroTitle.hero.id, m_addedHeroTitle.title.id, m_addedHeroTitle.startTime));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nItemId, int nItemUseCount, bool bItemOwned)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			int nItemOwnCount = 0;
			int nItemUnOwnCount = 0;
			if (bItemOwned)
			{
				nItemOwnCount = nItemUseCount;
			}
			else
			{
				nItemUnOwnCount = nItemUseCount;
			}
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, nItemId, nItemOwnCount, nItemUnOwnCount, m_currentTime));
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroTitleAcquiredLog(Guid.NewGuid(), m_myHero.id, m_addedHeroTitle.title.id, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
