using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AnkouTombRewardPoolEntry : IPickEntry
{
	private AnkouTombDifficulty m_difficulty;

	private int m_nNo;

	private int m_nPoint;

	private ItemReward m_itemReward;

	public AnkouTombDifficulty difficulty => m_difficulty;

	public int no => m_nNo;

	public int point => m_nPoint;

	int IPickEntry.point => m_nPoint;

	public ItemReward itemReward => m_itemReward;

	public AnkouTombRewardPoolEntry(AnkouTombDifficulty difficulty)
	{
		m_difficulty = difficulty;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. difficulty = " + m_difficulty.difficulty + ", m_nNo = " + m_nNo + ", m_nPoint = " + m_nPoint);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. difficulty = " + m_difficulty.difficulty + ", m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
			}
		}
		else if (lnItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상ID가 유효하지 않습니다. difficulty = " + m_difficulty.difficulty + ", m_nNo = " + m_nNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
