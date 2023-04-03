using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class DailyChargeEvent
{
	private int m_nRequiredHeroLevel;

	private Dictionary<int, DailyChargeEventMission> m_missions = new Dictionary<int, DailyChargeEventMission>();

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
	}

	public void AddMission(DailyChargeEventMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_missions.Add(mission.no, mission);
	}

	public DailyChargeEventMission GetMission(int nMissionNo)
	{
		if (!m_missions.TryGetValue(nMissionNo, out var value))
		{
			return null;
		}
		return value;
	}
}
