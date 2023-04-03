using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RechargeEvent
{
	private int m_nRequiredUnOwnDia;

	private Dictionary<int, RechargeEventReward> m_rewards = new Dictionary<int, RechargeEventReward>();

	public int requiredUnOwnDia => m_nRequiredUnOwnDia;

	public Dictionary<int, RechargeEventReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRequiredUnOwnDia = Convert.ToInt32(dr["requiredUnOwnDia"]);
		if (m_nRequiredUnOwnDia <= 0)
		{
			SFLogUtil.Warn(GetType(), "요구비귀속다이아가 유효하지 않습니다.");
		}
	}

	public void AddReward(RechargeEventReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward.no, reward);
	}
}
