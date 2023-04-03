using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class JobChainSkillHit : SkillHit
{
	private JobChainSkill m_skill;

	private int m_nId;

	private float m_fDamageFactor;

	private int m_nAcquireLak;

	public override int type => 2;

	public JobChainSkill jobChainSkill
	{
		get
		{
			return m_skill;
		}
		set
		{
			m_skill = value;
		}
	}

	public override Skill skill => m_skill;

	public override int id => m_nId;

	public override float damageFactor => m_fDamageFactor;

	public override int acquireLak => m_nAcquireLak;

	public override List<JobSkillHitAbnormalState> abnormalStates => null;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["hitId"]);
		m_fDamageFactor = Convert.ToSingle(dr["damageFactor"]);
		m_nAcquireLak = Convert.ToInt32(dr["acquireLak"]);
	}
}
