using System;
using System.Collections.Generic;

namespace GameServer;

public class FishingQuestBait
{
	private int m_nItemId;

	private List<FishingQuestCastingReward> m_rewards = new List<FishingQuestCastingReward>();

	public int itemId => m_nItemId;

	public List<FishingQuestCastingReward> rewards => m_rewards;

	public FishingQuestBait(int nItemId)
	{
		m_nItemId = nItemId;
	}

	public void AddReward(FishingQuestCastingReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public FishingQuestCastingReward GetReward(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}
}
