using System;
using System.Collections.Generic;

namespace GameServer;

public class OpenGift
{
	private int m_nDay;

	private List<OpenGiftReward> m_rewards = new List<OpenGiftReward>();

	public int day => m_nDay;

	public List<OpenGiftReward> rewards => m_rewards;

	public OpenGift(int nDay)
	{
		m_nDay = nDay;
	}

	public void AddReward(OpenGiftReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public OpenGiftReward GetReward(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}
}
