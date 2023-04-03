using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WarMemoryRankingReward
{
	private WarMemory m_warMemory;

	private int m_nHighRanking;

	private int m_nLowRanking;

	private int m_nRewardNo;

	private ItemReward m_itemReward;

	public WarMemory warMemory => m_warMemory;

	public int highRanking => m_nHighRanking;

	public int lowRanking => m_nLowRanking;

	public int rewardNo => m_nRewardNo;

	public ItemReward itemReward => m_itemReward;

	public WarMemoryRankingReward(WarMemory warMemory)
	{
		m_warMemory = warMemory;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nHighRanking = Convert.ToInt32(dr["highRanking"]);
		m_nLowRanking = Convert.ToInt32(dr["lowRanking"]);
		m_nRewardNo = Convert.ToInt32(dr["rewardNo"]);
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nHighRanking = " + m_nHighRanking + ", m_nLowRanking = " + m_nLowRanking + ", m_nRewardNo = " + m_nRewardNo);
			}
		}
		else if (lnItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. m_nHighRanking = " + m_nHighRanking + ", m_nLowRanking = " + m_nLowRanking + ", m_nRewardNo = " + m_nRewardNo);
		}
	}
}
