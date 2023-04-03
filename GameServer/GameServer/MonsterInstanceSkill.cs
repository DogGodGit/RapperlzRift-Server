using System;

namespace GameServer;

public class MonsterInstanceSkill : IPickEntry
{
	private MonsterInstance m_monsterInst;

	private MonsterSkill m_monsterSkill;

	private DateTimeOffset m_castTime = DateTimeOffset.MinValue;

	public MonsterInstance monsterInst => m_monsterInst;

	public MonsterSkill monsterSkill => m_monsterSkill;

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

	int IPickEntry.point => m_monsterSkill.autoWeight;

	public MonsterInstanceSkill(MonsterInstance monsterInst, MonsterSkill monsterSkill)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		if (monsterSkill == null)
		{
			throw new ArgumentNullException("monsterSkill");
		}
		m_monsterInst = monsterInst;
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
		Skill skill = m_monsterSkill;
		return new Offense(m_monsterInst, skill, 1, m_monsterSkill.baseDamageType, m_monsterSkill.elementalId);
	}

	public static int Compare(MonsterInstanceSkill a, MonsterInstanceSkill b)
	{
		return a.monsterSkill.autoPriorityGroup.CompareTo(b.monsterSkill.autoPriorityGroup);
	}
}
