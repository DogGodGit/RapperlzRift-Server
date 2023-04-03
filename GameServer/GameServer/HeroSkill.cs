using System;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroSkill
{
	private Hero m_hero;

	private JobSkill m_skill;

	private int m_nLevel;

	private DateTimeOffset m_castTime = DateTimeOffset.MinValue;

	private int m_nCurrentChainSkillId;

	private int m_nCurrentHitId = -1;

	private Guid m_targetHeroId = Guid.Empty;

	public Hero hero => m_hero;

	public JobSkill skill => m_skill;

	public int skillId => m_skill.skillId;

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

	public DateTimeOffset castTime
	{
		get
		{
			return m_castTime;
		}
		set
		{
			m_castTime = value;
		}
	}

	public int currentChainSkillId
	{
		get
		{
			return m_nCurrentChainSkillId;
		}
		set
		{
			m_nCurrentChainSkillId = value;
		}
	}

	public int currentHitId
	{
		get
		{
			return m_nCurrentHitId;
		}
		set
		{
			m_nCurrentHitId = value;
		}
	}

	public Guid targetHeroId
	{
		get
		{
			return m_targetHeroId;
		}
		set
		{
			m_targetHeroId = value;
		}
	}

	public bool isMaxLevel => m_nLevel >= m_skill.maxSkillLevel.level;

	public bool isOpened
	{
		get
		{
			if (!m_hero.IsMainQuestCompleted(m_skill.skillMaster.openRequiredMainQuestNo))
			{
				return !m_hero.isReal;
			}
			return true;
		}
	}

	public HeroSkill(Hero hero, JobSkill skill)
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

	public Offense MakeOffense()
	{
		Skill skill = ((m_skill.formType == 1) ? ((Skill)m_skill.GetChainSkill(m_nCurrentChainSkillId)) : ((Skill)m_skill));
		Job job = m_hero.job;
		JobSkillLevel skillLevel = m_skill.GetLevel(m_nLevel);
		int nOffenseAmp = ((job.offenseType == OffenseType.Physical) ? skillLevel.physicalOffenseAmp : skillLevel.magicalOffenseAmp);
		return new Offense(m_hero, skill, m_nLevel, job.offenseType, job.elementalId, nOffenseAmp, skillLevel.offensePoint);
	}

	public void SetHeroSkillByFieldOfHonorHeroSkill(FieldOfHonorHeroSkill fieldOfHonorHeroSkill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_nLevel = fieldOfHonorHeroSkill.level;
	}

	public PDHeroSkill ToPDHeroSkill()
	{
		PDHeroSkill inst = new PDHeroSkill();
		inst.skillId = m_skill.skillId;
		inst.level = m_nLevel;
		return inst;
	}

	public FieldOfHonorHeroSkill ToFieldOfHonorHeroSkill(FieldOfHonorHero fieldOfHonorHero)
	{
		FieldOfHonorHeroSkill inst = new FieldOfHonorHeroSkill(fieldOfHonorHero);
		inst.id = m_skill.skillId;
		inst.level = m_nLevel;
		return inst;
	}
}
