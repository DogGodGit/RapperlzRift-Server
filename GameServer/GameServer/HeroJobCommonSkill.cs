using System;

namespace GameServer;

public class HeroJobCommonSkill
{
	private Hero m_hero;

	private JobCommonSkill m_skill;

	private DateTimeOffset m_castTime = DateTimeOffset.MinValue;

	private int m_nCurrentHitId = -1;

	public Hero hero => m_hero;

	public JobCommonSkill skill => m_skill;

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

	public bool isOpened => m_hero.IsMainQuestCompleted(m_skill.openRequiredMainQuestNo);

	public HeroJobCommonSkill(Hero hero, JobCommonSkill skill)
	{
		m_hero = hero;
		m_skill = skill;
	}

	public bool IsExpiredSkillCoolTime(DateTimeOffset time)
	{
		float fElapsedTime = DateTimeUtil.GetTimeSpanSeconds(m_castTime, time);
		if (fElapsedTime >= m_skill.coolTime)
		{
			return true;
		}
		return false;
	}
}
