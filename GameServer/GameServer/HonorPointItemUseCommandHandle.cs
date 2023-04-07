using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class HonorPointItemUseCommandHandler : InGameCommandHandler<HonorPointItemUseCommandBody, HonorPointItemUseResponseBody>
{
	public const short kResult_NotEnoughItem = 101;

	private InventorySlot m_tagetInventorySlot;

	private int m_nResultHonorPoint;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nSlotIndex = m_body.slotIndex;
		int nUseCount = m_body.useCount;
		if (nSlotIndex < 0)
		{
			throw new CommandHandleException(1, "슬롯인덱스가 유효하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		if (nUseCount <= 0)
		{
			throw new CommandHandleException(1, "갯수가 유효하지 않습니다. nUseCount = " + nUseCount);
		}
		m_tagetInventorySlot = m_myHero.GetInventorySlot(nSlotIndex);
		if (m_tagetInventorySlot == null)
		{
			throw new CommandHandleException(1, "존재하지 않은 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
		}
		if (m_tagetInventorySlot.isEmpty)
		{
			throw new CommandHandleException(1, "빈 슬롯입니다. nSlotIndex = " + nSlotIndex);
		}
		if (m_tagetInventorySlot.obj.inventoryObjectType != 3)
		{
			throw new CommandHandleException(1, "해당 슬롯오브젝트타입은 아이템타입이 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		ItemInventoryObject itemInventoryObj = (ItemInventoryObject)m_tagetInventorySlot.obj;
		Item targetItem = itemInventoryObj.item;
		bool bOwned = itemInventoryObj.owned;
		if (targetItem.type.id != 20)
		{
			throw new CommandHandleException(1, "해당 아이템은 명예포인트아이템이 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		if (!targetItem.IsUseableHeroLevel(m_myHero.level))
		{
			throw new CommandHandleException(1, "사용할 수 있는 레벨이 아닙니다.");
		}
		if (itemInventoryObj.count < nUseCount)
		{
			throw new CommandHandleException(101, "아이템 수량이 부족합니다.");
		}
		m_myHero.UseItem(nSlotIndex, nUseCount);
		HonorPointReward honorPointReward = Resource.instance.GetHonorPointReward(targetItem.longValue1);
		if (honorPointReward != null)
		{
			m_nResultHonorPoint = honorPointReward.value * nUseCount;
		}
		m_myHero.AddHonorPoint(m_nResultHonorPoint);
		SaveToDB();
		SaveToGameLogDB(targetItem.id, bOwned, nUseCount);
		HonorPointItemUseResponseBody resBody = new HonorPointItemUseResponseBody();
		resBody.honorPoint = m_myHero.honorPoint;
		resBody.changedInventorySlot = m_tagetInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_HonorPoint(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_tagetInventorySlot));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nItemId, bool bOwned, int nUseCount)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			int nItemOwnCount = 0;
			int nItemUnOwnCount = 0;
			if (bOwned)
			{
				nItemOwnCount = nUseCount;
			}
			else
			{
				nItemUnOwnCount = nUseCount;
			}
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, nItemId, nItemOwnCount, nItemUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
