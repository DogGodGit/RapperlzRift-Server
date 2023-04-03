using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class LimitationGift
{
	private int m_nRequiredHeroLevel;

	private HashSet<DayOfWeek> m_dayOfWeeks = new HashSet<DayOfWeek>();

	private Dictionary<int, LimitationGiftRewardSchedule> m_schedule = new Dictionary<int, LimitationGiftRewardSchedule>();

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요영웅레벨이 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
	}

	public void AddDayOfWeek(DayOfWeek dayOfWeek)
	{
		m_dayOfWeeks.Add(dayOfWeek);
	}

	public bool ContainsDayOfWeek(DayOfWeek dayOfWeek)
	{
		return m_dayOfWeeks.Contains(dayOfWeek);
	}

	public void AddSchedule(LimitationGiftRewardSchedule schedule)
	{
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_schedule.Add(schedule.id, schedule);
	}

	public LimitationGiftRewardSchedule GetSchedule(int nId)
	{
		if (!m_schedule.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}
}
