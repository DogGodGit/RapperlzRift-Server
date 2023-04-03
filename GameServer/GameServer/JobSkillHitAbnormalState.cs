using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class JobSkillHitAbnormalState
{
	private JobSkillHit m_hit;

	private AbnormalState m_abnormalState;

	private int m_nHitRate;

	public JobSkillHit hit
	{
		get
		{
			return m_hit;
		}
		set
		{
			m_hit = value;
		}
	}

	public AbnormalState abnormalState => m_abnormalState;

	public int hitRate => m_nHitRate;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nAbnormalStateId = Convert.ToInt32(dr["abnormalStateId"]);
		if (nAbnormalStateId > 0)
		{
			m_abnormalState = Resource.instance.GetAbnormalState(nAbnormalStateId);
			if (m_abnormalState == null)
			{
				SFLogUtil.Warn(GetType(), "상태이상이 존재하지 않습니다. nAbnormalStateId = " + nAbnormalStateId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "상태이상ID가 유효하지 않습니다. nAbnormalStateId = " + nAbnormalStateId);
		}
		m_nHitRate = Convert.ToInt32(dr["hitRate"]);
		if (m_nHitRate <= 0)
		{
			SFLogUtil.Warn(GetType(), "상태이상적중률이 유효하지 않습니다. m_nHitRate = " + m_nHitRate);
		}
	}
}
