using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class StaminaRecoverySchedule
{
	private int m_nId;

	private int m_nRecoveryTime;

	private int m_nRecoveryStamina;

	public int id => m_nId;

	public int recoveryTime => m_nRecoveryTime;

	public int recoveryStamina => m_nRecoveryStamina;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["scheduleId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "스케줄ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nRecoveryTime = Convert.ToInt32(dr["recoveryTime"]);
		if (m_nRecoveryTime < 0)
		{
			SFLogUtil.Warn(GetType(), "회복시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nRecoveryTime = " + m_nRecoveryTime);
		}
		m_nRecoveryStamina = Convert.ToInt32(dr["recoveryStamina"]);
		if (m_nRecoveryStamina <= 0)
		{
			SFLogUtil.Warn(GetType(), "회복스태미너가 유효하지 않습니다. m_nId = " + m_nId + ", m_nRecoveryStamina = " + m_nRecoveryStamina);
		}
	}
}
