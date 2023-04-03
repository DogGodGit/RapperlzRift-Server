using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NationWarPaidTransmission
{
	private NationWar m_nationWar;

	private int m_nTransmissionCount;

	private int m_nRequiredDia;

	public NationWar nationWar => m_nationWar;

	public int transmissionCount => m_nTransmissionCount;

	public int requiredDia => m_nRequiredDia;

	public NationWarPaidTransmission(NationWar nationWar)
	{
		m_nationWar = nationWar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nTransmissionCount = Convert.ToInt32(dr["transmissionCount"]);
		if (m_nTransmissionCount < 0)
		{
			SFLogUtil.Warn(GetType(), "전송횟수가 유효하지 않습니다. m_nTransmissionCount = " + m_nTransmissionCount);
		}
		m_nRequiredDia = Convert.ToInt32(dr["requiredDia"]);
		if (m_nRequiredDia < 0)
		{
			SFLogUtil.Warn(GetType(), "필요다이아가 유효하지 않습니다. m_nTransmissionCount = " + m_nTransmissionCount + ", m_nRequiredDia = " + m_nRequiredDia);
		}
	}
}
