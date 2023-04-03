using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DailyChargeEventMission
{
	private DailyChargeEvent m_evt;

	private int m_nNo;

	private int m_nRequiredUnOwnDia;

	private Dictionary<int, DailyChargeEventMissionReward> m_rewards = new Dictionary<int, DailyChargeEventMissionReward>();

	public DailyChargeEvent evt => m_evt;

	public int no => m_nNo;

	public int requiredUnOwnDia => m_nRequiredUnOwnDia;

	public Dictionary<int, DailyChargeEventMissionReward> rewards => m_rewards;

	public DailyChargeEventMission(DailyChargeEvent evt)
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
		m_nRequiredUnOwnDia = Convert.ToInt32(dr["requiredUnOwnDia"]);
		if (m_nRequiredUnOwnDia <= 0)
		{
			SFLogUtil.Warn(GetType(), "요구비귀속다이아가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
	}

	public void AddReward(DailyChargeEventMissionReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward.no, reward);
	}
}
