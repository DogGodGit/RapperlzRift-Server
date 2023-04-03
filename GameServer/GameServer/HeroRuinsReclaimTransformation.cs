using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class HeroRuinsReclaimTransformationMonsterEffect
{
	private Hero m_hero;

	private RuinsReclaimStepWaveSkill m_skill;

	private DateTimeOffset m_effectStartTime = DateTimeOffset.MinValue;

	private bool m_bRunning;

	private Timer m_timer;

	public Hero hero => m_hero;

	public RuinsReclaimStepWaveSkill skill => m_skill;

	public Monster transformationMonster => m_skill.transformationMonster;

	public DateTimeOffset effectStartTime => m_effectStartTime;

	public bool running => m_bRunning;

	public void Init(Hero hero, RuinsReclaimStepWaveSkill skill, DateTimeOffset time)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_hero = hero;
		m_skill = skill;
		m_effectStartTime = time;
		m_bRunning = true;
		int nDuration = skill.transformationLifetime * 1000;
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
		m_hero.ProcessRuinsReclaimTransformationMonsterEffectFinished();
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
