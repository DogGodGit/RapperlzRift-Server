using System;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroRankPassiveSkill
{
	private Hero m_hero;

	private RankPassiveSkill m_skill;

	private int m_nLevel;

	public Hero hero => m_hero;

	public RankPassiveSkill skill => m_skill;

	public int skillId => m_skill.id;

	public int level
	{
		get
		{
			return m_nLevel;
		}
		set
		{
			m_nLevel = value;
		}
	}

	public RankPassiveSkillLevel skillLevel => m_skill.GetLevel(m_nLevel);

	public bool isMaxLevel => m_nLevel >= m_skill.maxLevel.level;

	public HeroRankPassiveSkill(Hero hero, RankPassiveSkill skill)
	{
		m_hero = hero;
		m_skill = skill;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
	}

	public PDHeroRankPassiveSkill ToPDHeroPassiveSkill()
	{
		PDHeroRankPassiveSkill inst = new PDHeroRankPassiveSkill();
		inst.skillId = m_skill.id;
		inst.level = m_nLevel;
		return inst;
	}
}
