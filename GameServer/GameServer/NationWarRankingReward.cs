using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NationWarRankingReward
{
	public const int kLogType_Ranking = 1;

	public const int kLogType_Lucky = 2;

	private NationWar m_nationWar;

	private int m_nRanking;

	private ItemReward m_itemReward;

	public NationWar nationWar => m_nationWar;

	public int ranking => m_nRanking;

	public ItemReward itemReward => m_itemReward;

	public NationWarRankingReward(NationWar nationWar)
	{
		m_nationWar = nationWar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRanking = Convert.ToInt32(dr["ranking"]);
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nRanking = " + m_nRanking + ", m_itemReward = " + m_itemReward);
			}
		}
		else if (lnItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. m_nRanking = " + m_nRanking + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
