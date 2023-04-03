using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureCardShopRefreshSchedule
{
	private int m_nId;

	private int m_nRefreshTime;

	public int id => m_nId;

	public int refreshTime => m_nRefreshTime;

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
		m_nRefreshTime = Convert.ToInt32(dr["refreshTime"]);
		if (m_nRefreshTime < 0)
		{
			SFLogUtil.Warn(GetType(), "갱신시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nRefreshTime = " + m_nRefreshTime);
		}
	}
}
