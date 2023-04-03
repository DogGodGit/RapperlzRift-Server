using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class HeroAncientRelicTrapEffect
{
	private Hero m_target;

	private int m_nMoveSpeed;

	private int m_nDuration;

	private DateTimeOffset m_effectStartTime = DateTimeOffset.MinValue;

	private bool m_bRunning;

	private Timer m_timer;

	public Hero target => m_target;

	public int moveSpeed => m_nMoveSpeed;

	public int duration => m_nDuration;

	public DateTimeOffset effectStartTime => m_effectStartTime;

	public bool running => m_bRunning;

	public void Init(Hero target, int nMoveSpeed, int nDuration, DateTimeOffset time)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		m_target = target;
		m_nMoveSpeed = nMoveSpeed;
		m_nDuration = nDuration;
		m_effectStartTime = time;
		m_bRunning = true;
		m_timer = new Timer(OnTimerTick);
		m_timer.Change(nDuration * 1000, -1);
	}

	private void OnTimerTick(object state)
	{
		m_target.AddWork(new SFAction(ProcessTimerTick), bGlobalLockRequired: false);
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
		m_target.ProcessAncientRelicTrapAbnormalStateEffectFinished();
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
