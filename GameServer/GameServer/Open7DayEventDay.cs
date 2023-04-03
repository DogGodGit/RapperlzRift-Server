using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Open7DayEventDay
{
	private int m_nDay;

	public int day => m_nDay;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nDay = Convert.ToInt32(dr["day"]);
		if (day <= 0)
		{
			SFLogUtil.Warn(GetType(), "일차가 유효하지 않습니다. m_nDay = " + m_nDay);
		}
	}
}
