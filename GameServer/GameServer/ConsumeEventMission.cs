using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ConsumeEventMission
{
	private ConsumeEvent m_evt;

	private int m_nNo;

	private int m_nRequiredDia;

	private Dictionary<int, ConsumeEventMissionReward> m_rewards = new Dictionary<int, ConsumeEventMissionReward>();

	public ConsumeEvent evt => m_evt;

	public int no => m_nNo;

	public int requiredDia => m_nRequiredDia;

	public Dictionary<int, ConsumeEventMissionReward> rewards => m_rewards;

	public ConsumeEventMission(ConsumeEvent evt)
	{
		if (evt == null)
		{
			throw new ArgumentNullException("evt");
		}
		m_evt = evt;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["missionNo"]);
		m_nRequiredDia = Convert.ToInt32(dr["requiredDia"]);
		if (m_nRequiredDia <= 0)
		{
			SFLogUtil.Warn(GetType(), "요구다이아가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
	}

	public void AddReward(ConsumeEventMissionReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward.no, reward);
	}
}
