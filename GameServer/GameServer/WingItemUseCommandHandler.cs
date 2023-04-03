using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class WingItemUseCommandHandler : InGameCommandHandler<WingItemUseCommandBody, WingItemUseResponseBody>
{
	public const short kResult_NotEnoughItem = 101;

	private HeroWing m_addedHeroWing;

	private InventorySlot m_targetInventorySlot;

	private Item m_usedItem;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

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
		m_targetInventorySlot = m_myHero.GetInventorySlot(nSlotIndex);
		if (m_targetInventorySlot == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 인벤토리슬롯입니다. nSlotIndex = " + nSlotIndex);
		}
		if (m_targetInventorySlot.isEmpty)
		{
			throw new CommandHandleException(1, "빈 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
		}
		if (m_targetInventorySlot.obj.inventoryObjectType != 3)
		{
			throw new CommandHandleException(1, "해당 인벤토리오브젝트타입은 아이템이 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		ItemInventoryObject itemInventoryObject = (ItemInventoryObject)m_targetInventorySlot.obj;
		m_usedItem = itemInventoryObject.item;
		bool bItemOwned = itemInventoryObject.owned;
		int nItemCount = itemInventoryObject.count;
		if (m_usedItem.type.id != 29)
		{
			throw new CommandHandleException(1, "해당 아이템은 날개아이템이 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		if (!m_usedItem.IsUseableHeroLevel(m_myHero.level))
		{
			throw new CommandHandleException(1, "아이템을 사용할 수 없는 레벨입니다.");
		}
		if (nItemCount < 1)
		{
			throw new CommandHandleException(101, "아이템이 부족합니다.");
		}
		Wing targetWing = Resource.instance.GetWing(m_usedItem.value1);
		if (m_myHero.ContainsWing(targetWing.id))
		{
			throw new CommandHandleException(1, "이미 가지고 있는 날개입니다.");
		}
		m_addedHeroWing = new HeroWing(m_myHero, targetWing);
		m_addedHeroWing.Init();
		m_myHero.AddWing(m_addedHeroWing);
		m_myHero.UseItem(nSlotIndex, 1);
		if (bItemOwned)
		{
			m_nUsedItemOwnCount = 1;
		}
		else
		{
			m_nUsedItemUnOwnCount = 1;
		}
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SaveToLogDB();
		WingItemUseResponseBody resBody = new WingItemUseResponseBody();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.addedWing = m_addedHeroWing.ToPDHeroWing();
		resBody.changedInventorySlot = m_targetInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroWing(m_addedHeroWing.hero.id, m_addedHeroWing.wing.id, m_addedHeroWing.memoryPieceStep));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_targetInventorySlot));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, (m_usedItem != null) ? m_usedItem.id : 0, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
