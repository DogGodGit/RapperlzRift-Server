using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class HeroOsirisRoomMoneyBuff
{
	private Hero m_target;

	private MoneyBuff m_moneyBuff;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private Timer m_timer;

	private bool m_bRunning;

	public Hero target => m_target;

	public MoneyBuff moneyBuff => m_moneyBuff;

	public DateTimeOffset startTime => m_startTime;

	public void Init(Hero target, MoneyBuff moneyBuff, DateTimeOffset startTime)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (moneyBuff == null)
		{
			throw new ArgumentNullException("moneyBuff");
		}
		m_target = target;
		m_moneyBuff = moneyBuff;
		m_startTime = startTime;
		int nBuffDuration = moneyBuff.lifetime * 1000;
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
			m_target.ProcessOsirisRoomMoneyBuffFinished();
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
