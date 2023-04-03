using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class HeroMainQuestTransformationMonsterEffect
{
	private Hero m_hero;

	private MainQuest m_mainQuest;

	private DateTimeOffset m_effectStartTime = DateTimeOffset.MinValue;

	private bool m_bRunning;

	private Timer m_timer;

	public Hero hero => m_hero;

	public MainQuest mainQuest => m_mainQuest;

	public Monster transformationMonster => m_mainQuest.transformationMonster;

	public DateTimeOffset effectStartTime => m_effectStartTime;

	public bool running => m_bRunning;

	public void Init(Hero hero, MainQuest mainQuest, DateTimeOffset time)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		if (mainQuest == null)
		{
			throw new ArgumentNullException("mainQuest");
		}
		m_hero = hero;
		m_mainQuest = mainQuest;
		m_effectStartTime = time;
		m_bRunning = true;
		int nDuration = m_mainQuest.transformationLifetime * 1000;
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
		m_hero.ProcessMainQuestTransformationMonsterEffectFinished();
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
public class HeroMainQuestTransformationMonsterSkill
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

	public HeroMainQuestTransformationMonsterSkill(Hero hero, MonsterSkill monsterSkill)
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
