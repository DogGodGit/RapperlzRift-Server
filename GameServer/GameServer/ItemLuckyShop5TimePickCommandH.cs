using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class ItemLuckyShop5TimePickCommandHandler : InGameCommandHandler<ItemLuckyShop5TimePickCommandBody, ItemLuckyShop5TimePickResponseBody>
{
	public const short kResult_NotEnoughDia = 101;

	public const short kResult_OverflowedPickCount = 102;

	public const short kResult_NotEnoughInventory = 103;

	private ResultItemCollection m_resultItemCollection;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		ItemLuckyShop itemLuckyShop = Resource.instance.itemLuckyShop;
		int nRequiredDia = itemLuckyShop.pick5TimeDia;
		if (m_myHero.dia < nRequiredDia)
		{
			throw new CommandHandleException(1, "다이아가 부족합니다.");
		}
		m_myHero.RefreshItemLuckyShopPickCount(currentDate);
		if (m_myHero.itemLuckyShopPick5TimeCount >= m_myHero.vipLevel.luckyShopPickMaxCount)
		{
			throw new CommandHandleException(102, "뽑기횟수가 최대횟수를 넘어갑니다.");
		}
		if (m_myHero.emptyInventorySlotCount < 5)
		{
			throw new CommandHandleException(103, "인벤토리가 부족합니다.");
		}
		m_resultItemCollection = new ResultItemCollection();
		List<PDItemLuckyShopPickResult> pickResults = new List<PDItemLuckyShopPickResult>();
		List<ItemLuckyShopNormalPoolEntry> normalEntries = itemLuckyShop.SelectNormalEntries(5 - itemLuckyShop.pick5TimeSpecialPickCount);
		foreach (ItemLuckyShopNormalPoolEntry normalPoolEntry in normalEntries)
		{
			ItemReward itemReward2 = normalPoolEntry.itemReward;
			m_resultItemCollection.AddResultItemCount(itemReward2.item, itemReward2.owned, itemReward2.count);
			pickResults.Add(new PDItemLuckyShopPickResult(itemReward2.item.id, itemReward2.count));
		}
		List<ItemLuckyShopSpecialPoolEntry> specialEntries = itemLuckyShop.SelectSpecialEntries(itemLuckyShop.pick5TimeSpecialPickCount);
		foreach (ItemLuckyShopSpecialPoolEntry specialPoolEntry in specialEntries)
		{
			ItemReward itemReward = specialPoolEntry.itemReward;
			m_resultItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
			pickResults.Add(new PDItemLuckyShopPickResult(itemReward.item.id, itemReward.count));
		}
		if (!m_myHero.IsAvailableInventory(m_resultItemCollection))
		{
			throw new CommandHandleException(103, "인벤토리가 부족합니다.");
		}
		long lnGoldReward = itemLuckyShop.pick1TimeGoldRewardValue;
		m_myHero.UseDia(nRequiredDia, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
		foreach (ResultItem resultItem in m_resultItemCollection.resultItems)
		{
			m_myHero.AddItem(resultItem.item, resultItem.owned, resultItem.count, m_changedInventorySlots);
		}
		m_myHero.AddGold(lnGoldReward);
		m_myHero.itemLuckyShopPick5TimeCount++;
		SaveToDB();
		SaveToLogDB(lnGoldReward);
		ItemLuckyShop5TimePickResponseBody resBody = new ItemLuckyShop5TimePickResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.pick5TimeCount = m_myHero.itemLuckyShopPick5TimeCount;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.pickResults = pickResults.ToArray();
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
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroItemLuckyShopPickLog(logId, m_myHero.id, 3, m_nUsedOwnDia, m_nUsedUnOwnDia, lnRewardGold, m_currentTime));
			foreach (ResultItem resultItem in m_resultItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroItemLuckyShopPickDetailLog(Guid.NewGuid(), logId, resultItem.item.id, resultItem.owned, resultItem.count));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
