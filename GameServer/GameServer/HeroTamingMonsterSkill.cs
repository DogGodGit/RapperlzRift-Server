using System;

namespace GameServer;

public class HeroTamingMonsterSkill
{
	private Hero m_tamer;

	private MonsterSkill m_monsterSkill;

	private DateTimeOffset m_castTime = DateTimeOffset.MinValue;

	private int m_nCurrentHitId = -1;

	public Hero tamer => m_tamer;

	public MonsterSkill monsterSkill => m_monsterSkill;

	public int skillId => m_monsterSkill.skillId;

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

	public HeroTamingMonsterSkill(Hero tamer, MonsterSkill monsterSkill)
	{
		if (tamer == null)
		{
			throw new ArgumentNullException("tamer");
		}
		if (monsterSkill == null)
		{
			throw new ArgumentNullException("monsterSkill");
		}
		m_tamer = tamer;
		m_monsterSkill = monsterSkill;
	}

	public bool IsExpiredSkillCoolTime(DateTimeOffset time)
	{
		float fElapsedTime = DateTimeUtil.GetTimeSpanSeconds(m_castTime, time);
		if (fElapsedTime >= m_monsterSkill.coolTime)
		{
			return true;
		}
		return false;
	}

	public Offense MakeOffense()
	{
		Job job = tamer.job;
		return new Offense(m_tamer, m_monsterSkill, 1, job.offenseType, job.elementalId);
	}
}
