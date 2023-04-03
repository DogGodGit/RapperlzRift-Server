using System;
using System.Collections.Generic;

namespace GameServer;

public class HeroHolyWarQuestStartScheduleCollection
{
	private DateTime m_date = DateTime.MinValue.Date;

	private HashSet<int> m_schedules = new HashSet<int>();

	public DateTime date
	{
		get
		{
			return m_date;
		}
		set
		{
			m_date = value;
		}
	}

	public HashSet<int> schedules => m_schedules;

	public void AddSchedule(int nScheduleId)
	{
		m_schedules.Add(nScheduleId);
	}

	public void ClearSchedules()
	{
		m_schedules.Clear();
	}

	public bool ContainsSchedule(int nScheduleId)
	{
		return m_schedules.Contains(nScheduleId);
	}
}
