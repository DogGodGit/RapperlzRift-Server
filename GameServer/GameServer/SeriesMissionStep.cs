using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SeriesMissionStep
{
	private SeriesMission m_mission;

	private int m_nStep;

	private int m_nTargetCount;

	private List<SeriesMissionStepReward> m_rewards = new List<SeriesMissionStepReward>();

	public SeriesMission mission
	{
		get
		{
			return m_mission;
		}
		set
		{
			m_mission = value;
		}
	}

	public int step => m_nStep;

	public int targetCount => m_nTargetCount;

	public List<SeriesMissionStepReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nStep = Convert.ToInt32(dr["step"]);
		if (m_nStep <= 0)
		{
			SFLogUtil.Warn(GetType(), "단계가 유효하지 않습니다. m_nStep = " + m_nStep);
		}
		m_nTargetCount = Convert.ToInt32(dr["targetCount"]);
		if (m_nTargetCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표 횟수가 유효하지 않습니다. m_nStep = " + m_nStep + ", m_nTargetCount = " + m_nTargetCount);
		}
	}

	public void AddReward(SeriesMissionStepReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
		reward.step = this;
	}
}
