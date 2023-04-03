using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AbnormalStateJobSkillLevel : AbnormalStateLevel
{
	private JobAbnormalState m_jobAbnormalState;

	public JobAbnormalState jobAbnormalState => m_jobAbnormalState;

	public override AbnormalState abnormalState => m_jobAbnormalState.abnormalState;

	public override int level => m_nLevel;

	public override int duration => m_nDuration;

	public override int value1 => m_nValue1;

	public override int value2 => m_nValue2;

	public override int value3 => m_nValue3;

	public override int value4 => m_nValue4;

	public override int value5 => m_nValue5;

	public override int value6 => m_nValue6;

	public AbnormalStateJobSkillLevel(JobAbnormalState jobAbnormalState)
	{
		m_jobAbnormalState = jobAbnormalState;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		m_nDuration = Convert.ToInt32(dr["duration"]);
		if (m_nDuration <= 0)
		{
			SFLogUtil.Warn(GetType(), "지속시간 유효하지 않습니다. abnormalStateId = " + abnormalState.id + ", jobId = " + jobAbnormalState.job.id + ", m_nLevel = " + m_nLevel + ", m_nDuration = " + m_nDuration);
		}
		m_nValue1 = Convert.ToInt32(dr["value1"]);
		m_nValue2 = Convert.ToInt32(dr["value2"]);
		m_nValue3 = Convert.ToInt32(dr["value3"]);
		m_nValue4 = Convert.ToInt32(dr["value4"]);
		m_nValue5 = Convert.ToInt32(dr["value5"]);
		m_nValue6 = Convert.ToInt32(dr["value6"]);
	}
}
