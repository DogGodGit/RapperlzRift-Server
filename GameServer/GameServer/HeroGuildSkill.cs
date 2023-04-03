using System;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroGuildSkill
{
	private Hero m_hero;

	private GuildSkill m_guildSkill;

	private int m_nLevel;

	private int m_nRealLevel;

	public Hero hero => m_hero;

	public GuildSkill skill => m_guildSkill;

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

	public int realLevel => m_nRealLevel;

	public bool isMaxLevel => m_nLevel >= m_guildSkill.lastLevel.level;

	public HeroGuildSkill(Hero hero)
		: this(hero, null)
	{
	}

	public HeroGuildSkill(Hero hero, GuildSkill skill)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_guildSkill = skill;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nGuildSkillId = Convert.ToInt32(dr["guildSkillId"]);
		m_guildSkill = Resource.instance.GetGuildSkill(nGuildSkillId);
		if (m_guildSkill == null)
		{
			throw new Exception("존재하지 않는 길드스킬입니다. nGuildSkillId = " + nGuildSkillId);
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
	}

	public bool RefreshRealLevel()
	{
		int nOldLevel = m_nRealLevel;
		if (m_hero.guildMember != null)
		{
			Guild guild = m_hero.guildMember.guild;
			int nMaxLevelOfLabortoryLevel = m_guildSkill.GetMaxLevelOfLaboratoryLevel(guild.laboratory.level.level);
			m_nRealLevel = Math.Min(m_nLevel, nMaxLevelOfLabortoryLevel);
		}
		else
		{
			m_nRealLevel = 0;
		}
		return nOldLevel != m_nRealLevel;
	}

	public PDHeroGuildSkill ToPDHeroGuildSkill()
	{
		PDHeroGuildSkill inst = new PDHeroGuildSkill();
		inst.id = m_guildSkill.id;
		inst.level = m_nLevel;
		return inst;
	}
}
