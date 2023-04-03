using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class StoryDungeonTrapEffect
{
	private StoryDungeonInstance m_storyDungeonInst;

	private StoryDungeonTrap m_trap;

	private int m_nCurrentHitId;

	private bool m_bRunning;

	private Timer m_hitTimer;

	public StoryDungeonInstance storyDungeonInst => m_storyDungeonInst;

	public Vector3 position => m_trap.position;

	public int hitCount => m_trap.hitCount;

	public int damage => (int)m_trap.damage;

	public int currentHitId => m_nCurrentHitId;

	public void Init(StoryDungeonInstance storyDungeonInst, StoryDungeonTrap trap)
	{
		if (storyDungeonInst == null)
		{
			throw new ArgumentNullException("storyDungeonInst");
		}
		if (trap == null)
		{
			throw new ArgumentNullException("trap");
		}
		m_storyDungeonInst = storyDungeonInst;
		m_trap = trap;
		int nHitInterval = 0;
		if (hitCount > 1)
		{
			nHitInterval = m_trap.castingDuration * 1000 / (hitCount - 1);
		}
		else if (hitCount == 1)
		{
			nHitInterval = -1;
		}
		if (nHitInterval == 0)
		{
			throw new Exception("적중간격이 유효하지 않습니다.");
		}
		int nCastingStartDelay = (int)Math.Floor(m_trap.castingStartDelay * 1000f);
		m_hitTimer = new Timer(OnHitTimerTick);
		m_hitTimer.Change(nCastingStartDelay, nHitInterval);
		m_bRunning = true;
	}

	private void OnHitTimerTick(object state)
	{
		m_storyDungeonInst.AddWork(new SFAction(ProcessTimerTick), bGlobalLockRequired: false);
	}

	private void ProcessTimerTick()
	{
		if (m_bRunning)
		{
			m_nCurrentHitId++;
			m_storyDungeonInst.ProcessTrapEffectHitTick(this);
			if (m_nCurrentHitId >= hitCount)
			{
				Finish();
			}
		}
	}

	public void Stop()
	{
		if (m_bRunning)
		{
			DisposeHitTimer();
			m_bRunning = false;
		}
	}

	private void Finish()
	{
		DisposeHitTimer();
		m_bRunning = false;
		m_storyDungeonInst.ProcessTrapEffectFinished(this);
	}

	private void DisposeHitTimer()
	{
		if (m_hitTimer != null)
		{
			m_hitTimer.Dispose();
			m_hitTimer = null;
		}
	}

	public bool Contains(Vector3 position)
	{
		return m_trap.ValidationAreaContains(position);
	}
}
