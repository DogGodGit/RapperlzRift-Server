using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ChargeEvent
{
	private int m_nId;

	private DateTime m_startTime = DateTime.MinValue;

	private DateTime m_endTime = DateTime.MinValue;

	private Dictionary<int, ChargeEventMission> m_missions = new Dictionary<int, ChargeEventMission>();

	public int id => m_nId;

	public DateTime startTime => m_startTime;

	public DateTime endTime => m_endTime;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["eventId"]);
		m_startTime = Convert.ToDateTime(dr["startTime"]);
		m_endTime = Convert.ToDateTime(dr["endTime"]);
		if (m_startTime >= m_endTime)
		{
			SFLogUtil.Warn(GetType(), "시작시각이 종료시각보다 크거나 같습니다. m_nId = " + m_nId);
		}
	}

	public bool IsEventTime(DateTime time)
	{
		if (time >= m_startTime)
		{
			return time < m_endTime;
		}
		return false;
	}

	public void AddMission(ChargeEventMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_missions.Add(mission.no, mission);
	}

	public ChargeEventMission GetMission(int nMissionNo)
	{
		if (!m_missions.TryGetValue(nMissionNo, out var value))
		{
			return null;
		}
		return value;
	}
}
