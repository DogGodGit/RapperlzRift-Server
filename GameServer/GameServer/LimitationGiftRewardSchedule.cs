using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class LimitationGiftRewardSchedule
{
	private int m_nId;

	private int m_nStartTime;

	private int m_nEndTime;

	private List<LimitationGiftReward> m_rewards = new List<LimitationGiftReward>();

	public int id => m_nId;

	public int startTime => m_nStartTime;

	public int endTime => m_nEndTime;

	public List<LimitationGiftReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["scheduleId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "스케쥴ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nStartTime = Convert.ToInt32(dr["startTime"]);
		if (m_nStartTime < 0)
		{
			SFLogUtil.Warn(GetType(), "시작시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nStartTime = " + m_nStartTime);
		}
		m_nEndTime = Convert.ToInt32(dr["endTime"]);
		if (m_nEndTime < 0)
		{
			SFLogUtil.Warn(GetType(), "종료시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nEndTime = " + m_nEndTime);
		}
		if (m_nStartTime >= m_nEndTime)
		{
			SFLogUtil.Warn(GetType(), "시작시간이 종료시간보다 큽니다. m_nId = " + m_nId + ", m_nStartTime = " + m_nStartTime + ", m_nEndTime = " + m_nEndTime);
		}
	}

	public void AddReward(LimitationGiftReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public bool IsScheduleTime(float fTime)
	{
		if (fTime >= (float)m_nStartTime)
		{
			return fTime < (float)m_nEndTime;
		}
		return false;
	}
}
