using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class DailyConsumeEvent
{
	private int m_nRequiredHeroLevel;

	private Dictionary<int, DailyConsumeEventMission> m_missions = new Dictionary<int, DailyConsumeEventMission>();

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
	}

	public void AddMission(DailyConsumeEventMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_missions.Add(mission.no, mission);
	}

	public DailyConsumeEventMission GetMission(int nMissionNo)
	{
		if (!m_missions.TryGetValue(nMissionNo, out var value))
		{
			return null;
		}
		return value;
	}
}
