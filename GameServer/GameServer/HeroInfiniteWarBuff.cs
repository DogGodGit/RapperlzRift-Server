using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class HeroInfiniteWarBuff
{
	private Hero m_target;

	private InfiniteWarBuffBox m_buffBox;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private Timer m_timer;

	private bool m_bRunning;

	public Hero target => m_target;

	public InfiniteWarBuffBox buffBox => m_buffBox;

	public float offenseFactor => m_buffBox.offenseFactor;

	public float defenseFactor => m_buffBox.defenseFactor;

	public DateTimeOffset startTime => m_startTime;

	public void Init(Hero target, InfiniteWarBuffBox buffBox, DateTimeOffset startTime)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (buffBox == null)
		{
			throw new ArgumentNullException("buffBox");
		}
		m_target = target;
		m_buffBox = buffBox;
		m_startTime = startTime;
		int nBuffDuration = m_buffBox.infiniteWar.buffDuration * 1000;
		m_bRunning = true;
		m_timer = new Timer(OnTimerTick);
		m_timer.Change(nBuffDuration, -1);
	}

	private void OnTimerTick(object state)
	{
		m_target.AddWork(new SFAction(ProcessTimerTick), bGlobalLockRequired: false);
	}

	private void ProcessTimerTick()
	{
		if (m_bRunning)
		{
			DisposeTimer();
			m_bRunning = false;
			m_target.ProcessInfiniteWarBuffFinished();
		}
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
