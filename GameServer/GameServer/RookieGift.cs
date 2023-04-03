using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RookieGift
{
	private int m_nNo;

	private int m_nWaitingTime;

	private List<RookieGiftReward> m_rewards = new List<RookieGiftReward>();

	public int no => m_nNo;

	public int waitingTime => m_nWaitingTime;

	public List<RookieGiftReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["giftNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "선물번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nWaitingTime = Convert.ToInt32(dr["waitingTime"]);
		if (m_nWaitingTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "대기시간이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nWaitingTime = " + m_nWaitingTime);
		}
	}

	public void AddReward(RookieGiftReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public RookieGiftReward GetReward(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}
}
