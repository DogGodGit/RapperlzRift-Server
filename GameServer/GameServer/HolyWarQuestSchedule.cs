using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class HolyWarQuestSchedule
{
	private int m_nId;

	private int m_nStartTime;

	private int m_nEndTime;

	public int id => m_nId;

	public int startTime => m_nStartTime;

	public int endTime => m_nEndTime;

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
		if (m_nStartTime < 0 || m_nStartTime >= 86400)
		{
			SFLogUtil.Warn(GetType(), "시작시간이 유효하지 않습니다. m_nStartTime = " + m_nStartTime);
		}
		m_nEndTime = Convert.ToInt32(dr["endTime"]);
		if (m_nEndTime < 0 || m_nEndTime > 86400)
		{
			SFLogUtil.Warn(GetType(), "종료시간이 유효하지 않습니다. m_nEndTime = " + m_nEndTime);
		}
		if (m_nStartTime >= m_nEndTime)
		{
			SFLogUtil.Warn(GetType(), "시작시간이 종료시간보다 크거나 같습니다. m_nStartTime = " + m_nStartTime + ", m_nEndTime = " + m_nEndTime);
		}
	}

	public bool Contains(float fTime)
	{
		if (fTime >= (float)m_nStartTime)
		{
			return fTime < (float)m_nEndTime;
		}
		return false;
	}
}
