using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WeeklyPresentPopularityPointRankingReward
{
	private WeeklyPresentPopularityPointRankingRewardGroup m_group;

	private int m_nNo;

	private ItemReward m_itemReward;

	public WeeklyPresentPopularityPointRankingRewardGroup group => m_group;

	public int no => m_nNo;

	public ItemReward itemReward => m_itemReward;

	public WeeklyPresentPopularityPointRankingReward(WeeklyPresentPopularityPointRankingRewardGroup group)
	{
		if (group == null)
		{
			throw new ArgumentNullException("group");
		}
		m_group = group;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId <= 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. groupNo = " + m_group.no + ", m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			return;
		}
		m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
		if (m_itemReward == null)
		{
			SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. groupNo = " + m_group.no + ", m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
public class WeeklyPresentPopularityPointRankingRewardGroup
{
	private int m_nNo;

	private int m_nHighRanking;

	private int m_nLowRanking;

	private Dictionary<int, WeeklyPresentPopularityPointRankingReward> m_rewards = new Dictionary<int, WeeklyPresentPopularityPointRankingReward>();

	public int no => m_nNo;

	public int highRanking => m_nHighRanking;

	public int lowRanking => m_nLowRanking;

	public Dictionary<int, WeeklyPresentPopularityPointRankingReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentException("dr");
		}
		m_nNo = Convert.ToInt32(dr["groupNo"]);
		m_nHighRanking = Convert.ToInt32(dr["highRanking"]);
		if (m_nHighRanking <= 0)
		{
			SFLogUtil.Warn(GetType(), "상위랭킹이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nHighRanking = " + m_nHighRanking);
		}
		m_nLowRanking = Convert.ToInt32(dr["lowRanking"]);
		if (m_nLowRanking <= 0)
		{
			SFLogUtil.Warn(GetType(), "하위랭킹이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nLowRanking = " + m_nLowRanking);
		}
		if (m_nHighRanking > m_nLowRanking)
		{
			SFLogUtil.Warn(GetType(), "상위랭킹이 하위랭킹보다 더 큽니다. m_nNo = " + m_nNo + ", m_nHighRanking = " + m_nHighRanking + ", m_nLowRanking = " + m_nLowRanking);
		}
	}

	public void AddReward(WeeklyPresentPopularityPointRankingReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward.no, reward);
	}
}
