using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class DiaShopProductBuyCommandHandler : InGameCommandHandler<DiaShopProductBuyCommandBody, DiaShopProductBuyResponseBody>
{
	public const short kResult_NotEnoughVipLevel = 101;

	public const short kReuslt_NotSellable = 102;

	public const short kResult_NotAvailableTime = 103;

	public const short kResult_NotAvailableDay = 104;

	public const short kResult_BuyCountOverflowed = 105;

	public const short kResult_NotEnoughInventory = 106;

	public const short kResult_NotEnoughDia = 107;

	public const short kResult_NotEnoughUnOwnDia = 108;

	public const short kResult_NotEnoughItem = 109;

	private Item m_productItem;

	private bool m_bProductItemOwned;

	private int m_nUsedOwnDia;

	private int m_nUsedUnOwnDia;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private HeroDiaShopProductBuyCount m_dailyDiaShopProductBuyCount;

	private HeroDiaShopProductBuyCount m_totalDiaShopProductBuyCount;

	private HashSet<InventorySlot> m_chagedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nProductId = m_body.productId;
		int nCount = m_body.count;
		if (nProductId <= 0)
		{
			throw new CommandHandleException(1, "상품ID가 유효하지 않습니다. nProductId = " + nProductId);
		}
		if (nCount <= 0)
		{
			throw new CommandHandleException(1, "구매수량이 유효하지 않습니다. nCount = " + nCount);
		}
		DiaShopProduct diaShopProduct = Resource.instance.GetDiaShopProduct(nProductId);
		if (diaShopProduct == null)
		{
			throw new CommandHandleException(1, "다이아상점상품이 존재하지 않습니다. nProductId = " + nProductId);
		}
		if (!diaShopProduct.sellable)
		{
			throw new CommandHandleException(102, "판매중이 아닌 상품입니다.");
		}
		DiaShopCategory diaShopCategory = diaShopProduct.category;
		if (diaShopCategory != null && diaShopCategory.requiredVipLevel > m_myHero.vipLevel.level)
		{
			throw new CommandHandleException(101, "VIP레벨이 부족합니다.");
		}
		switch (diaShopProduct.periodType)
		{
		case DiaShopPeriodType.Period:
			if (!diaShopProduct.IsPeriodTime(m_currentTime.DateTime))
			{
				throw new CommandHandleException(103, "다이아상점상품 시간이 유효하지 않습니다.");
			}
			break;
		case DiaShopPeriodType.DayOfWeek:
			if (diaShopProduct.dayOfWeek != m_currentTime.DayOfWeek)
			{
				throw new CommandHandleException(104, "다이아상점상품 요일이 유효하지 않습니다.");
			}
			break;
		}
		m_myHero.RefreshDailyDiaShopProductBuyCounts(m_currentDate);
		m_dailyDiaShopProductBuyCount = m_myHero.GetOrCreateDailyDiaShopProductBuyCount(nProductId);
		m_totalDiaShopProductBuyCount = m_myHero.GetOrCreateTotalDiaShopProductBuyCount(nProductId);
		switch (diaShopProduct.buyLimitType)
		{
		case DiaShopBuyLimitType.DailyLimit:
			if (diaShopProduct.buyLimitCount > 0 && diaShopProduct.buyLimitCount < m_dailyDiaShopProductBuyCount.buyCount + nCount)
			{
				throw new CommandHandleException(105, "구매횟수가 최대횟수를 넘어갑니다.");
			}
			break;
		case DiaShopBuyLimitType.Accumulation:
			if (diaShopProduct.buyLimitCount > 0 && diaShopProduct.buyLimitCount < m_totalDiaShopProductBuyCount.buyCount + nCount)
			{
				throw new CommandHandleException(105, "구매횟수가 최대횟수를 넘어갑니다.");
			}
			break;
		}
		m_productItem = diaShopProduct.item;
		m_bProductItemOwned = diaShopProduct.itemOwned;
		if (m_myHero.GetInventoryAvailableSpace(m_productItem, m_bProductItemOwned) < nCount)
		{
			throw new CommandHandleException(106, "인벤토리가 부족합니다.");
		}
		int nTotalPrice = diaShopProduct.price * nCount;
		int nMoneyItemId = 0;
		switch (diaShopProduct.moneyType)
		{
		case DiaShopProductMoneyType.Dia:
			if (m_myHero.dia < nTotalPrice)
			{
				throw new CommandHandleException(107, "다이아가 부족합니다.");
			}
			m_myHero.UseDia(nTotalPrice, m_currentTime, out m_nUsedOwnDia, out m_nUsedUnOwnDia);
			break;
		case DiaShopProductMoneyType.UnOwnDia:
			if (m_myHero.unOwnDia < nTotalPrice)
			{
				throw new CommandHandleException(108, "비귀속다이아가 부족합니다.");
			}
			m_myAccount.UseUnOwnDia(nTotalPrice, m_currentTime);
			m_nUsedUnOwnDia = nTotalPrice;
			break;
		case DiaShopProductMoneyType.Item:
			nMoneyItemId = diaShopProduct.moneyItemId;
			if (m_myHero.GetItemCount(nMoneyItemId) < nTotalPrice)
			{
				throw new CommandHandleException(109, "아이템이 부족합니다.");
			}
			m_myHero.UseItem(nMoneyItemId, bFisetUseOwn: true, nTotalPrice, m_chagedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
			break;
		}
		m_myHero.AddItem(m_productItem, m_bProductItemOwned, nCount, m_chagedInventorySlots);
		m_dailyDiaShopProductBuyCount.buyCount += nCount;
		m_totalDiaShopProductBuyCount.buyCount += nCount;
		SaveToDB();
		SaveToLogDB(0, nProductId, nMoneyItemId, nCount);
		DiaShopProductBuyResponseBody resBody = new DiaShopProductBuyResponseBody();
		resBody.date = (DateTime)m_currentDate;
		resBody.dailyBuyCount = m_dailyDiaShopProductBuyCount.buyCount;
		resBody.totalBuyCount = m_totalDiaShopProductBuyCount.buyCount;
		resBody.ownDia = m_myHero.ownDia;
		resBody.unOwnDia = m_myHero.unOwnDia;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_chagedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateAccount_UnOwnDia(m_myAccount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		foreach (InventorySlot slot in m_chagedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroDiaShopProduct(m_dailyDiaShopProductBuyCount.hero.id, m_dailyDiaShopProductBuyCount.hero.dailyDiaShopProductBuyCountsDate, m_dailyDiaShopProductBuyCount.productId, m_dailyDiaShopProductBuyCount.buyCount));
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nCategoryId, int nProductId, int nUsedItemId, int nCount)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroDiaShopProductBuyLog(Guid.NewGuid(), m_myHero.id, nCategoryId, nProductId, m_nUsedOwnDia, m_nUsedUnOwnDia, nUsedItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, (m_productItem != null) ? m_productItem.id : 0, m_bProductItemOwned, nCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
