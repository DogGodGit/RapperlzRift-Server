using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class HeroWarMemoryTransformationMonsterSkill
{
	private Hero m_hero;

	private MonsterSkill m_monsterSkill;

	private DateTimeOffset m_castTime = DateTimeOffset.MinValue;

	private int m_nCurrentHitId = -1;

	public Hero hero => m_hero;

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

	public HeroWarMemoryTransformationMonsterSkill(Hero hero, MonsterSkill monsterSkill)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		if (monsterSkill == null)
		{
			throw new ArgumentNullException("monsterSkill");
		}
		m_hero = hero;
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
		return new Offense(m_hero, m_monsterSkill, 1, m_monsterSkill.baseDamageType, m_monsterSkill.elementalId);
	}
}
public class HeroWarMemoryTransformationMonsterEffect
{
	private Hero m_hero;

	private WarMemoryTransformationObject m_transformationObject;

	private WarMemoryMonsterAttrFactor m_attrFactor;

	private DateTimeOffset m_effectStartTime = DateTimeOffset.MinValue;

	private bool m_bRunning;

	private Timer m_timer;

	public Hero hero => m_hero;

	public WarMemoryTransformationObject transformationObject => m_transformationObject;

	public WarMemoryMonsterAttrFactor attrFactor => m_attrFactor;

	public Monster transformationMonster => m_transformationObject.transformationMonster;

	public DateTimeOffset effectStartTime => m_effectStartTime;

	public bool running => m_bRunning;

	public void Init(Hero hero, WarMemoryTransformationObject transformationObject, WarMemoryMonsterAttrFactor attrFactor, DateTimeOffset time)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		if (transformationObject == null)
		{
			throw new ArgumentNullException("transformationObject");
		}
		if (attrFactor == null)
		{
			throw new ArgumentNullException("attrFactor");
		}
		m_hero = hero;
		m_transformationObject = transformationObject;
		m_attrFactor = attrFactor;
		m_effectStartTime = time;
		m_bRunning = true;
		int nDuration = transformationObject.transformationLifetime * 1000;
		m_timer = new Timer(OnTimerTick);
		m_timer.Change(nDuration, -1);
	}

	private void OnTimerTick(object state)
	{
		m_hero.AddWork(new SFAction(ProcessTimerTick), bGlobalLockRequired: false);
	}

	private void ProcessTimerTick()
	{
		if (m_bRunning)
		{
			Finish();
		}
	}

	private void Finish()
	{
		DisposeTimer();
		m_bRunning = false;
		m_hero.ProcessWarMemoryTransformationMonsterEffectFinished();
	}

	private void DisposeTimer()
	{
		if (m_timer != null)
		{
			m_timer.Dispose();
			m_timer = null;
		}
	}

	public void Stop()
	{
		if (m_bRunning)
		{
			DisposeTimer();
			m_bRunning = false;
		}
	}
}
