using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ItemLuckyShopNormalPoolEntry : IPickEntry
{
	private int m_nNo;

	private int m_nPoint;

	private ItemReward m_itemReward;

	public int no => m_nNo;

	public int point => m_nPoint;

	public ItemReward itemReward => m_itemReward;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "항목번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nNo = " + m_nNo + ",m_nPoint = " + m_nPoint);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템 보상이 존재하지 않습니다. m_nNo = " + m_nNo + " lnItemRewardId = " + lnItemRewardId);
			}
		}
	}
}
