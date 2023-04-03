using System;
using System.Collections.Generic;

namespace GameServer;

public class BountyHunterQuestRewardCollection
{
	private int m_nQuestItemGrade;

	private List<BountyHunterQuestReward> m_rewards = new List<BountyHunterQuestReward>();

	public int questItemGrade => m_nQuestItemGrade;

	public List<BountyHunterQuestReward> rewards => m_rewards;

	public BountyHunterQuestRewardCollection(int nItemGrade)
	{
		m_nQuestItemGrade = nItemGrade;
	}

	public void AddReward(BountyHunterQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public BountyHunterQuestReward GetReward(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}
}
