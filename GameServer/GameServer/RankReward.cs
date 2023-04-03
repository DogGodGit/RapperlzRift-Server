using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RankReward
{
	private Rank m_rank;

	private int m_nNo;

	private ItemReward m_itemReward;

	public Rank rank => m_rank;

	public int no => m_nNo;

	public ItemReward itemReward => m_itemReward;

	public RankReward(Rank rank)
	{
		if (rank == null)
		{
			throw new ArgumentNullException("rank");
		}
		m_rank = rank;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rewardNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "보상번호가 유효하지 않습니다. rankNo = " + m_rank.no + ", m_nNo = " + m_nNo);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId <= 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. rankNo = " + m_rank.no + ", m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
		}
		m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
		if (m_itemReward == null)
		{
			SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. rankNo = " + m_rank.no + ", m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
