using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class FirstChargeEvent
{
	private Dictionary<int, FirstChargeEventReward> m_rewards = new Dictionary<int, FirstChargeEventReward>();

	public Dictionary<int, FirstChargeEventReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
	}

	public void AddReward(FirstChargeEventReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward.no, reward);
	}
}
