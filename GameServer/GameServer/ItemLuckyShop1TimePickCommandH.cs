using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ItemLuckyShop1TimePickCommandHandler : InGameCommandHandler<ItemLuckyShop1TimePickCommandBody, ItemLuckyShop1TimePickResponseBody>
{
	public const short kResult_NotEnoughDia = 101;

	public const short kResult_OverflowedPickCount = 102;

	public const short kResult_NotEnoughInventory = 103;

	private ItemReward m_itemReward;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		ItemLuckyShop itemLuckyShop = Resource.instance.itemLuckyShop;
		int nRequiredDia = itemLuckyShop.pick1TimeDia;
		if (m_myHero.dia < nRequiredDia)
		{
			throw new CommandHandleException(101, "다이아가 부족합니다.");
		}
		m_myHero.RefreshItemLuckyShopPickCount(currentDate);
		if (m_myHero.itemLuckyShopPick1TimeCount >= m_myHero.vipLevel.luckyShopPickMaxCount)
		{
			throw new CommandHandleException(102, "뽑기횟수가 최대횟수를 넘어갑니다.");
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
		m_myHero.UseDia(nRequiredDia, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
		if (m_itemReward != null)
		{
			m_myHero.AddItem(m_itemReward.item, m_itemReward.owned, m_itemReward.count, m_changedInventorySlots);
			pickResult = new PDItemLuckyShopPickResult(m_itemReward.item.id, m_itemReward.count);
		}
		m_myHero.AddGold(lnGoldReward);
		m_myHero.itemLuckyShopPick1TimeCount++;
		SaveToDB();
		SaveToLogDB(lnGoldReward);
		ItemLuckyShop1TimePickResponseBody resBody = new ItemLuckyShop1TimePickResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.pick1TimeCount = m_myHero.itemLuckyShopPick1TimeCount;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.pickResult = pickResult;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ItemLuckyShopPickCount(m_myHero.id, m_myHero.itemLuckyShopPickDate, m_myHero.itemLuckyShopFreePickCount, m_myHero.itemLuckyShopPick1TimeCount, m_myHero.itemLuckyShopPick5TimeCount));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(long lnRewardGold)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroItemLuckyShopPickLog(logId, m_myHero.id, 2, m_nUsedOwnDia, m_nUsedUnOwnDia, lnRewardGold, m_currentTime));
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
