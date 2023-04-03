using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class InfiniteWarOpenSchedule
{
	private InfiniteWar m_infiniteWar;

	private int m_nId;

	private int m_nStartTime;

	private int m_nEndTime;

	public InfiniteWar infiniteWar => m_infiniteWar;

	public int id => m_nId;

	public int startTime => m_nStartTime;

	public int endTime => m_nEndTime;

	public InfiniteWarOpenSchedule(InfiniteWar infiniteWar)
	{
		m_infiniteWar = infiniteWar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["scheduleId"]);
		m_nStartTime = Convert.ToInt32(dr["startTime"]);
		if (m_nStartTime < 0 || m_nStartTime >= 86400)
		{
			SFLogUtil.Warn(GetType(), "시작시각이 유효하지 않습니다. m_nId = " + m_nId + ", m_nStartTime = " + m_nStartTime);
		}
		m_nEndTime = Convert.ToInt32(dr["endTime"]);
		if (m_nEndTime < 0 || m_nEndTime >= 86400)
		{
			SFLogUtil.Warn(GetType(), "종료시각이 유효하지 않습니다. m_nId = " + m_nId + ", m_nEndTime = " + m_nEndTime);
		}
		if (m_nStartTime >= m_nEndTime)
		{
			SFLogUtil.Warn(GetType(), "시작시각이 종료시각보다 늦거나 같습니다. m_nId = " + m_nId + ", m_nStartTime = " + m_nStartTime + ", m_nEndTime = " + m_nEndTime);
		}
	}

	public bool IsEnterable(int nTime)
	{
		if (nTime >= m_nStartTime)
		{
			return nTime < m_nEndTime;
		}
		return false;
	}
}
