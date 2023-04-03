using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class LevelRankingReward
{
	private int m_nHighRanking;

	private int m_nLowRanking;

	private ItemReward m_itemReward;

	public int highRanking => m_nHighRanking;

	public int lowRanking => m_nLowRanking;

	public ItemReward itemReward => m_itemReward;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nHighRanking = Convert.ToInt32(dr["highRanking"]);
		if (m_nHighRanking <= 0)
		{
			SFLogUtil.Warn(GetType(), "상위랭킹이 유효하지 않습니다. m_nHighRanking = " + m_nHighRanking);
		}
		m_nLowRanking = Convert.ToInt32(dr["lowRanking"]);
		if (m_nLowRanking <= 0)
		{
			SFLogUtil.Warn(GetType(), "하위랭킹이 유효하지 않습니다. m_nLowRanking = " + m_nLowRanking);
		}
		if (m_nHighRanking > m_nLowRanking)
		{
			SFLogUtil.Warn(GetType(), "상위랭킹이 하위랭킹보다 순위가 낮습니다. m_nHighRanking = " + m_nHighRanking + ", m_nLowRanking = " + m_nLowRanking);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
		if (m_itemReward == null)
		{
			SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nHighRanking = " + m_nHighRanking + ", m_nLowRanking = " + m_nLowRanking + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
