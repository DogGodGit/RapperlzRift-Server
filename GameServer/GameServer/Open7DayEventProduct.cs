using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Open7DayEventProduct
{
	private int m_nId;

	private Open7DayEventDay m_day;

	private Item m_item;

	private bool m_bItemOwned;

	private int m_nItemCount;

	private int m_nRequiredDia;

	public int id => m_nId;

	public Open7DayEventDay day => m_day;

	public Item item => m_item;

	public bool itemOwned => m_bItemOwned;

	public int itemCount => m_nItemCount;

	public int requiredDia => m_nRequiredDia;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["productId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "상품ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nDay = Convert.ToInt32(dr["day"]);
		m_day = Resource.instance.GetOpen7DayEventDay(nDay);
		if (m_day == null)
		{
			SFLogUtil.Warn(GetType(), "일차가 존재하지 않습니다. m_nId = " + m_nId + ", nDay = " + nDay);
		}
		int nItemId = Convert.ToInt32(dr["itemId"]);
		m_item = Resource.instance.GetItem(nItemId);
		if (m_item == null)
		{
			SFLogUtil.Warn(GetType(), "아이템이 존재하지 않습니다. m_nId = " + m_nId + ", nItemId = " + nItemId);
		}
		m_bItemOwned = Convert.ToBoolean(dr["itemOwned"]);
		m_nItemCount = Convert.ToInt32(dr["itemCount"]);
		if (m_nItemCount < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템수량이 유효하지 않습니다. m_nId = " + m_nId + ", m_nItemCount = " + m_nItemCount);
		}
		m_nRequiredDia = Convert.ToInt32(dr["requiredDia"]);
		if (m_nRequiredDia < 0)
		{
			SFLogUtil.Warn(GetType(), "필요다이아가 유효하지 않습니다.");
		}
	}
}
