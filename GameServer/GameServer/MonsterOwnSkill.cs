using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MonsterOwnSkill
{
	private Monster m_monster;

	private MonsterSkill m_skill;

	public Monster monster
	{
		get
		{
			return m_monster;
		}
		set
		{
			m_monster = value;
		}
	}

	public MonsterSkill skill => m_skill;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nSkillId = Convert.ToInt32(dr["skillId"]);
		if (nSkillId > 0)
		{
			m_skill = Resource.instance.GetMonsterSkill(nSkillId);
			if (m_skill == null)
			{
				SFLogUtil.Warn(GetType(), "몬스터스킬이 존재하지 않습니다. nSkillId = " + nSkillId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "몬스터스킬ID가 유효하지 않습니다. nSkillId = " + nSkillId);
		}
	}
}
