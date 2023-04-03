using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ItemReward
{
	private long m_lnId;

	private Item m_item;

	private int m_nCount;

	private bool m_bOwned;

	public long id => m_lnId;

	public Item item => m_item;

	public int count => m_nCount;

	public bool owned => m_bOwned;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_lnId = Convert.ToInt64(dr["itemRewardId"]);
		int nItemId = Convert.ToInt32(dr["itemId"]);
		m_item = Resource.instance.GetItem(nItemId);
		if (m_item == null)
		{
			SFLogUtil.Warn(GetType(), "아이템이 존재하지 않습니다. m_lnId = " + m_lnId + ", nItemId = " + nItemId);
		}
		m_nCount = Convert.ToInt32(dr["itemCount"]);
		if (m_nCount < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템 수량이 유효하지 않습니다. m_lnId = " + m_lnId + ", m_nCount = " + m_nCount);
		}
		m_bOwned = Convert.ToBoolean(dr["itemOwned"]);
	}
}
