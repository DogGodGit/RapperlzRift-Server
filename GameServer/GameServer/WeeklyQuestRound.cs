using System;
using System.Collections.Generic;

namespace GameServer;

public class WeeklyQuestRound
{
	private WeeklyQuest m_quest;

	private int m_nNo;

	private Dictionary<int, WeeklyQuestRoundReward> m_rewards = new Dictionary<int, WeeklyQuestRoundReward>();

	public WeeklyQuest quest => m_quest;

	public int no => m_nNo;

	public WeeklyQuestRound(WeeklyQuest quest, int nNo)
	{
		m_quest = quest;
		m_nNo = nNo;
	}

	public void AddReward(WeeklyQuestRoundReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward.level, reward);
	}

	public WeeklyQuestRoundReward GetReward(int nLevel)
	{
		if (!m_rewards.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}
}
