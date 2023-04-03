using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class StarEssenseItemUseCommandHandler : InGameCommandHandler<StarEssenseItemUseCommandBody, StarEssenseItemUseResponseBody>
{
	private const short kResult_NotEnoughItem = 101;

	private const short kResult_OverflowedUseCount = 102;

	private Item m_usedItem;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private InventorySlot m_targetInventorySlot;

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
			throw new CommandHandleException(1, "사용갯수가 유효하지 않습니다. nUseCount = " + nUseCount);
		}
		m_targetInventorySlot = m_myHero.GetInventorySlot(nSlotIndex);
		if (m_targetInventorySlot == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
		}
		if (m_targetInventorySlot.isEmpty)
		{
			throw new CommandHandleException(1, "빈 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
		}
		if (m_targetInventorySlot.obj.inventoryObjectType != 3)
		{
			throw new CommandHandleException(1, "인벤토리오브젝트타입이 아이템이 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		ItemInventoryObject itemInventoryObj = (ItemInventoryObject)m_targetInventorySlot.obj;
		m_usedItem = itemInventoryObj.item;
		bool bItemOwned = itemInventoryObj.owned;
		int nItemCount = itemInventoryObj.count;
		if (m_usedItem.type.id != 46)
		{
			throw new CommandHandleException(1, "해당 인벤토리슬롯의 아이템은 별의정수아이템타입이 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		if (!m_usedItem.IsUseableHeroLevel(m_myHero.level))
		{
			throw new CommandHandleException(1, "사용할 수 있는 레벨이 아닙니다.");
		}
		if (nItemCount < nUseCount)
		{
			throw new CommandHandleException(101, "아이템이 부족합니다.");
		}
		m_myHero.RefreshDailyStarEssenseItemUseCount(m_currentTime.Date);
		DateValuePair<int> dailyStarEssensItemUseCount = m_myHero.dailyStarEssensItemUseCount;
		if (dailyStarEssensItemUseCount.value + nUseCount > m_usedItem.value2)
		{
			throw new CommandHandleException(102, "사용횟수가 최대횟수를 넘어갑니다.");
		}
		m_myHero.UseItem(nSlotIndex, nUseCount);
		if (bItemOwned)
		{
			m_nUsedItemOwnCount = nUseCount;
		}
		else
		{
			m_nUsedItemUnOwnCount = nUseCount;
		}
		m_myHero.AddStarEssense(m_usedItem.value1 * nUseCount);
		dailyStarEssensItemUseCount.value += nUseCount;
		SaveToDB();
		SaveToLogDB();
		StarEssenseItemUseResponseBody resBody = new StarEssenseItemUseResponseBody();
		resBody.date = (DateTime)dailyStarEssensItemUseCount.date;
		resBody.dailyStarEssenseItemUseCount = dailyStarEssensItemUseCount.value;
		resBody.starEssesne = m_myHero.starEssense;
		resBody.changedInventorySlot = m_targetInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_StarEssenseItemUseDateCount(m_myHero.id, m_myHero.dailyStarEssensItemUseCount.date, m_myHero.dailyStarEssensItemUseCount.value));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_StarEssense(m_myHero.id, m_myHero.starEssense));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_targetInventorySlot));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, m_usedItem.id, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
