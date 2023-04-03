using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FieldOfHonorRankingReward
{
	private FieldOfHonor m_fieldOfHonor;

	private int m_nHighRanking;

	private int m_nLowRanking;

	private int m_nRewardNo;

	private ItemReward m_itemReward;

	public FieldOfHonor fieldOfHonor => m_fieldOfHonor;

	public int highRanking => m_nHighRanking;

	public int lowRanking => m_nLowRanking;

	public int rewardNo => m_nRewardNo;

	public ItemReward itemReward => m_itemReward;

	public FieldOfHonorRankingReward(FieldOfHonor fieldOfHonor)
	{
		m_fieldOfHonor = fieldOfHonor;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nHighRanking = Convert.ToInt32(dr["highRanking"]);
		m_nLowRanking = Convert.ToInt32(dr["lowRanking"]);
		if (m_nHighRanking > m_nLowRanking)
		{
			SFLogUtil.Warn(GetType(), "상위랭킹이 하위랭킹보다 순위가 낮습니다. m_nHighRanking = " + m_nHighRanking + ", m_nLowRanking = " + m_nLowRanking);
		}
		m_nRewardNo = Convert.ToInt32(dr["rewardNo"]);
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nHighRanking = " + m_nHighRanking + ", m_nLowRanking = " + m_nLowRanking + ", m_nRewardNo = " + m_nRewardNo + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. m_nHighRanking = " + m_nHighRanking + ", m_nLowRanking = " + m_nLowRanking + ", m_nRewardNo = " + m_nRewardNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
