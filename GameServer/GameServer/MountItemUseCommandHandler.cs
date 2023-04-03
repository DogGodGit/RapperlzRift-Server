using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MountItemUseCommandHandler : InGameCommandHandler<MountItemUseCommandBody, MountItemUseResponseBody>
{
	public const short kResult_NotEnooughItem = 101;

	public const short kResult_ExistHeroMount = 102;

	private InventorySlot m_targetInventorySlot;

	private int m_nMountItemId;

	private int m_nMountItemUsedOwnCount;

	private int m_nMountItemUsedUnOwnCount;

	private HeroMount m_newMount;

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
			throw new CommandHandleException(1, "인벤토리 슬롯이 존재하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		if (m_targetInventorySlot.isEmpty)
		{
			throw new CommandHandleException(1, "빈 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
		}
		if (m_targetInventorySlot.obj.inventoryObjectType != 3)
		{
			throw new CommandHandleException(1, "해당 인벤토리오브젝트타입은 아이템타입이 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		ItemInventoryObject itemInventoryObject = (ItemInventoryObject)m_targetInventorySlot.obj;
		Item targetItem = itemInventoryObject.item;
		bool bItemOwned = itemInventoryObject.owned;
		int nItemCount = itemInventoryObject.count;
		if (targetItem.type.id != 28)
		{
			throw new CommandHandleException(1, "해당 아이템은 탈것아이템타입이 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		if (!targetItem.IsUseableHeroLevel(m_myHero.level))
		{
			throw new CommandHandleException(1, "해당 아이템을 사용할 수 있는 레벨이 아닙니다.");
		}
		if (nItemCount < 1)
		{
			throw new CommandHandleException(101, "아이템이 부족합니다.");
		}
		if (m_myHero.ContainsMount(targetItem.value1))
		{
			throw new CommandHandleException(102, "이미 가지고 있는 탈것입니다.");
		}
		Mount mount = Resource.instance.GetMount(targetItem.value1);
		m_newMount = new HeroMount(m_myHero, mount, 1, 0);
		m_myHero.AddMount(m_newMount);
		m_myHero.UseItem(nSlotIndex, 1);
		m_nMountItemId = targetItem.id;
		if (bItemOwned)
		{
			m_nMountItemUsedOwnCount = 1;
		}
		else
		{
			m_nMountItemUsedUnOwnCount = 1;
		}
		m_newMount.RefreshAttrTotalValues();
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SaveToLogDB();
		MountItemUseResponseBody resBody = new MountItemUseResponseBody();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.addedMount = m_newMount.ToPDHeroMount();
		resBody.changedInventorySlot = m_targetInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroMount(m_newMount.hero.id, m_newMount.mount.id, m_newMount.level, m_newMount.satiety));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_targetInventorySlot));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, m_nMountItemId, m_nMountItemUsedOwnCount, m_nMountItemUsedUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
