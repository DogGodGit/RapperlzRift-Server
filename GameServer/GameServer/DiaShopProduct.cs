using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DiaShopProduct
{
	private int m_nId;

	private DiaShopCategory m_category;

	private Item m_item;

	private bool m_bItemOwned;

	private DiaShopProductMoneyType m_moneyType = DiaShopProductMoneyType.Dia;

	private int m_nMoneyItemId;

	private int m_nOriginalPrice;

	private int m_nPrice;

	private DiaShopBuyLimitType m_buyLimitType = DiaShopBuyLimitType.DailyLimit;

	private int m_nBuyLimitCount;

	private DiaShopPeriodType m_periodType;

	private DateTime m_startTime = DateTime.MinValue;

	private DateTime m_endTime = DateTime.MinValue;

	private DayOfWeek m_dayOfWeek;

	private bool m_bSellable;

	public int id => m_nId;

	public DiaShopCategory category => m_category;

	public Item item => m_item;

	public bool itemOwned => m_bItemOwned;

	public DiaShopProductMoneyType moneyType => m_moneyType;

	public int moneyItemId => m_nMoneyItemId;

	public int originalPrice => m_nOriginalPrice;

	public int price => m_nPrice;

	public DiaShopBuyLimitType buyLimitType => m_buyLimitType;

	public int buyLimitCount => m_nBuyLimitCount;

	public DiaShopPeriodType periodType => m_periodType;

	public DateTime startTime => m_startTime;

	public DateTime endTime => m_endTime;

	public DayOfWeek dayOfWeek => m_dayOfWeek;

	public bool sellable => m_bSellable;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["productId"]);
		if (m_nId < 0)
		{
			SFLogUtil.Warn(GetType(), "상품ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nCategoryId = Convert.ToInt32(dr["categoryId"]);
		if (nCategoryId > 0)
		{
			m_category = Resource.instance.GetDiaShopCategory(nCategoryId);
			if (m_category == null)
			{
				SFLogUtil.Warn(GetType(), "카테고리가 존재하지 않습니다. m_nId = " + m_nId + ", nCategoryId = " + nCategoryId);
			}
		}
		int nItemId = Convert.ToInt32(dr["itemId"]);
		m_item = Resource.instance.GetItem(nItemId);
		if (m_item == null)
		{
			SFLogUtil.Warn(GetType(), "아이템이 존재하지 않습니다. m_nId = " + m_nId + ", nItemId = " + nItemId);
		}
		m_bItemOwned = Convert.ToBoolean(dr["itemOwned"]);
		int nMoneyType = Convert.ToInt32(dr["moneyType"]);
		if (!Enum.IsDefined(typeof(DiaShopProductMoneyType), nMoneyType))
		{
			SFLogUtil.Warn(GetType(), "재화타입이 유효하지 않습니다. m_nId = " + m_nId + ", nMoneyType = " + nMoneyType);
		}
		m_moneyType = (DiaShopProductMoneyType)nMoneyType;
		if (m_moneyType == DiaShopProductMoneyType.Item)
		{
			m_nMoneyItemId = Convert.ToInt32(dr["moneyItemId"]);
			if (Resource.instance.GetItem(m_nMoneyItemId) == null)
			{
				SFLogUtil.Warn(GetType(), "재화아이템이 존재하지 않습니다. m_nId = " + m_nId + ", m_nMoneyItemId = " + m_nMoneyItemId);
			}
		}
		m_nOriginalPrice = Convert.ToInt32(dr["originalPrice"]);
		if (m_nOriginalPrice < 0)
		{
			SFLogUtil.Warn(GetType(), "원가가 유효하지 않습니다. m_nId = " + m_nId + ", m_nOriginalPrice = " + m_nOriginalPrice);
		}
		m_nPrice = Convert.ToInt32(dr["price"]);
		if (m_nPrice < 0)
		{
			SFLogUtil.Warn(GetType(), "가격이 유효하지 않습니다. m_nId = " + m_nId + ", m_nPrice = " + m_nPrice);
		}
		int nBuyLimitType = Convert.ToInt32(dr["buyLimitType"]);
		if (!Enum.IsDefined(typeof(DiaShopBuyLimitType), nBuyLimitType))
		{
			SFLogUtil.Warn(GetType(), "구매횟수제한타입이 유효하지 않습니다. m_nId = " + m_nId + ", nBuyLimitType = " + nBuyLimitType);
		}
		m_buyLimitType = (DiaShopBuyLimitType)nBuyLimitType;
		m_nBuyLimitCount = Convert.ToInt32(dr["buyLimitCount"]);
		if (m_nBuyLimitCount < 0)
		{
			SFLogUtil.Warn(GetType(), "구매제한횟수가 유효하지 않습니다. m_nId = " + m_nId + ", m_nBuyLimitCount = " + m_nBuyLimitCount);
		}
		int nPeriodType = Convert.ToInt32(dr["periodType"]);
		if (!Enum.IsDefined(typeof(DiaShopPeriodType), nPeriodType))
		{
			SFLogUtil.Warn(GetType(), "기간한정타입이 유효하지 않습니다. m_nId = " + m_nId + ", nPeriodType = " + nPeriodType);
		}
		m_periodType = (DiaShopPeriodType)nPeriodType;
		switch (m_periodType)
		{
		case DiaShopPeriodType.Period:
			m_startTime = Convert.ToDateTime(dr["periodStartTime"]);
			m_endTime = Convert.ToDateTime(dr["periodEndTime"]);
			if (m_startTime > m_endTime)
			{
				SFLogUtil.Warn(GetType(), "시작시간이 종료시간보다 큽니다. m_nId = " + m_nId);
			}
			break;
		case DiaShopPeriodType.DayOfWeek:
		{
			int nDayOfWeek = Convert.ToInt32(dr["periodDayOfWeek"]);
			if (!Enum.IsDefined(typeof(DayOfWeek), nDayOfWeek))
			{
				SFLogUtil.Warn(GetType(), "요일이 유효하지 않습니다. m_nId = " + m_nId + ", nDayOfWeek = " + nDayOfWeek);
			}
			m_dayOfWeek = (DayOfWeek)nDayOfWeek;
			break;
		}
		}
		m_bSellable = Convert.ToBoolean(dr["sellable"]);
	}

	public bool IsPeriodTime(DateTime dt)
	{
		if (dt >= m_startTime)
		{
			return dt < m_endTime;
		}
		return false;
	}
}
