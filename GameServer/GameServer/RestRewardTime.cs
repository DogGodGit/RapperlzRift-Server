using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RestRewardTime
{
	private int m_nRestTime;

	private long m_lnRequiredGold;

	private int m_nRequiredDia;

	public int restTime => m_nRestTime;

	public long requiredGold => m_lnRequiredGold;

	public int requiredDia => m_nRequiredDia;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRestTime = Convert.ToInt32(dr["restTime"]);
		if (m_nRestTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "휴식시간이 유효하지 않습니다. m_nRestTime = " + m_nRestTime);
		}
		m_lnRequiredGold = Convert.ToInt64(dr["requiredGold"]);
		if (m_lnRequiredGold <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요골드가 유효하지 않습니다. m_nRestTime = " + m_nRestTime + ", m_lnRequiredGold = " + m_lnRequiredGold);
		}
		m_nRequiredDia = Convert.ToInt32(dr["requiredDia"]);
		if (m_nRequiredDia <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요다이아가 유효하지 않습니다. m_nRestTime = " + m_nRestTime + ", m_nRequiredDia = " + m_nRequiredDia);
		}
	}
}
