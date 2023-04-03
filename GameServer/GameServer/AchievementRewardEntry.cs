using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AchievementRewardEntry
{
	private AchievementReward m_reward;

	private int m_nEntryNo;

	private ItemReward m_itemReward;

	public AchievementReward reward => m_reward;

	public int entryNo => m_nEntryNo;

	public ItemReward itemReward => m_itemReward;

	public AchievementRewardEntry(AchievementReward reward)
	{
		m_reward = reward;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nEntryNo = Convert.ToInt32(dr["rewardEntryNo"]);
		if (m_nEntryNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "항목번호가 유효하지 않습니다. m_nEntryNo = " + m_nEntryNo);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
		if (m_itemReward == null)
		{
			SFLogUtil.Warn(GetType(), "아이템 보상이 존재하지 않습니다. m_nEntryNo = " + m_nEntryNo + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
