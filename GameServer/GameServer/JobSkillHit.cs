using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class JobSkillHit : SkillHit
{
	private JobSkill m_skill;

	private int m_nId;

	private float m_fDamageFactor;

	private int m_nAcquireLak;

	private List<JobSkillHitAbnormalState> m_abnormalStates = new List<JobSkillHitAbnormalState>();

	public override int type => 1;

	public JobSkill jobSkill
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

	public override List<JobSkillHitAbnormalState> abnormalStates => m_abnormalStates;

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

	public void AddAbnormalState(JobSkillHitAbnormalState abnormalState)
	{
		if (abnormalState == null)
		{
			throw new ArgumentNullException("abnormalState");
		}
		if (abnormalState.hit != null)
		{
			throw new Exception("이미 직업스킬적중에 추가된 직업스킬적중상태이상 입니다.");
		}
		m_abnormalStates.Add(abnormalState);
		abnormalState.hit = this;
	}
}
