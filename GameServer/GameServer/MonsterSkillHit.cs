using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class MonsterSkillHit : SkillHit
{
	private MonsterSkill m_skill;

	private int m_nId;

	private float m_fDamageFactor;

	public override int type => 3;

	public override Skill skill => m_skill;

	public MonsterSkill monsterSkill
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

	public override int id => m_nId;

	public override float damageFactor => m_fDamageFactor;

	public override int acquireLak => 0;

	public override List<JobSkillHitAbnormalState> abnormalStates => null;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["hitId"]);
		m_fDamageFactor = Convert.ToSingle(dr["damageFactor"]);
	}
}
