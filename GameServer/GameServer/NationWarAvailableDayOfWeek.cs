using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NationWarAvailableDayOfWeek
{
	private NationWar m_nationWar;

	private DayOfWeek m_dayOfWeek;

	public NationWar nationWar => m_nationWar;

	public DayOfWeek dayOfWeek => m_dayOfWeek;

	public NationWarAvailableDayOfWeek(NationWar nationWar)
	{
		m_nationWar = nationWar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_dayOfWeek = (DayOfWeek)Convert.ToInt32(dr["dayOfWeek"]);
		if (!IsDefinedDayOfWeek(m_dayOfWeek))
		{
			SFLogUtil.Warn(GetType(), "요일이 유효하지 않습니다. m_dayOfWeek = " + m_dayOfWeek);
		}
	}

	public static bool IsDefinedDayOfWeek(DayOfWeek dayOfWeek)
	{
		if (dayOfWeek != 0 && dayOfWeek != DayOfWeek.Monday && dayOfWeek != DayOfWeek.Tuesday && dayOfWeek != DayOfWeek.Wednesday && dayOfWeek != DayOfWeek.Thursday && dayOfWeek != DayOfWeek.Friday)
		{
			return dayOfWeek == DayOfWeek.Saturday;
		}
		return true;
	}
}
