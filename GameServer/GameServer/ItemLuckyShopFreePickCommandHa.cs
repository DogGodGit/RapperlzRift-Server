using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ItemLuckyShopFreePickCommandHandler : InGameCommandHandler<ItemLuckyShopFreePickCommandBody, ItemLuckyShopFreePickResponseBody>
{
	public const short kResult_NotElapsedTime = 101;

	public const short kResult_OverflowedFreePickCount = 102;

	public const short kResult_NotEnoughInventory = 103;

	private ItemReward m_itemReward;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		if (m_myHero.GetRemainingItemLuckyShopFreePickTime(m_currentTime) > 0f)
		{
			throw new CommandHandleException(101, "무료뽑기대기시간이 경과되지 않았습니다.");
		}
		ItemLuckyShop itemLuckyShop = Resource.instance.itemLuckyShop;
		m_myHero.RefreshItemLuckyShopPickCount(currentDate);
		if (m_myHero.itemLuckyShopFreePickCount >= itemLuckyShop.freePickCount)
		{
			throw new CommandHandleException(102, "무료뽑기횟수가 최대횟수를 넘어갑니다.");
		}
		if (m_myHero.emptyInventorySlotCount < 1)
		{
			throw new CommandHandleException(103, "인벤토리가 부족합니다.");
		}
		PDItemLuckyShopPickResult pickResult = null;
		ItemLuckyShopNormalPoolEntry normalPoolEntry = itemLuckyShop.SelectNormalEntry();
		if (normalPoolEntry != null)
		{
			m_itemReward = normalPoolEntry.itemReward;
		}
		long lnGoldReward = itemLuckyShop.pick1TimeGoldRewardValue;
		if (m_itemReward != null && m_myHero.GetInventoryAvailableSpace(m_itemReward.item, m_itemReward.owned) < m_itemReward.count)
		{
			throw new CommandHandleException(103, "인벤토리가 부족합니다.");
		}
		m_myHero.AddGold(lnGoldReward);
		if (m_itemReward != null)
		{
			m_myHero.AddItem(m_itemReward.item, m_itemReward.owned, m_itemReward.count, m_changedInventorySlots);
			pickResult = new PDItemLuckyShopPickResult(m_itemReward.item.id, m_itemReward.count);
		}
		m_myHero.itemLuckyShopFreePickTime = m_currentTime;
		m_myHero.itemLuckyShopFreePickCount++;
		SaveToDB();
		SaveToLogDB(lnGoldReward);
		ItemLuckyShopFreePickResponseBody resBody = new ItemLuckyShopFreePickResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.freePickCount = m_myHero.itemLuckyShopFreePickCount;
		resBody.freePickRemainingTime = m_myHero.GetRemainingItemLuckyShopFreePickTime(m_currentTime);
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.pickResult = pickResult;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ItemLuckyShopFreePickTime(m_myHero.id, m_myHero.itemLuckyShopFreePickTime));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ItemLuckyShopPickCount(m_myHero.id, m_myHero.itemLuckyShopPickDate, m_myHero.itemLuckyShopFreePickCount, m_myHero.itemLuckyShopPick1TimeCount, m_myHero.itemLuckyShopPick5TimeCount));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(long lnGoldReward)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroItemLuckyShopPickLog(logId, m_myHero.id, 1, 0, 0, lnGoldReward, m_currentTime));
			if (m_itemReward != null)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroItemLuckyShopPickDetailLog(Guid.NewGuid(), logId, m_itemReward.item.id, m_itemReward.owned, m_itemReward.count));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
