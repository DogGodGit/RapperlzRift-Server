using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SafeTimeEvent
{
	private int m_nRequiredAutoDuration;

	private int m_nStartTime;

	private int m_nEndTime;

	public int requiredAutoDuration => m_nRequiredAutoDuration;

	public int startTime => m_nStartTime;

	public int endTime => m_nEndTime;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRequiredAutoDuration = Convert.ToInt32(dr["requiredAutoDuration"]);
		if (m_nRequiredAutoDuration < 0)
		{
			SFLogUtil.Warn(GetType(), "필요자동사냥모드유지시간 m_nRequiredAutoDuration = " + m_nRequiredAutoDuration);
		}
		m_nStartTime = Convert.ToInt32(dr["startTime"]);
		if (m_nStartTime < 0 || m_nStartTime >= 86400)
		{
			SFLogUtil.Warn(GetType(), "시작시각이 유효하지 않습니다. m_nStartTime = " + m_nStartTime);
		}
		m_nEndTime = Convert.ToInt32(dr["endTime"]);
		if (m_nEndTime < 0 || m_nEndTime >= 86400)
		{
			SFLogUtil.Warn(GetType(), "종료시각이 유효하지 않습니다. m_nEndTime = " + m_nEndTime);
		}
		if (m_nStartTime >= m_nEndTime)
		{
			SFLogUtil.Warn(GetType(), "시작시각이 종료시각보다 크거나 같습니다. m_nStartTime = " + m_nStartTime + ", m_nEndTime = " + m_nEndTime);
		}
	}

	public bool IsEventTime(int nTime)
	{
		if (nTime >= m_nStartTime)
		{
			return nTime < m_nEndTime;
		}
		return false;
	}
}
