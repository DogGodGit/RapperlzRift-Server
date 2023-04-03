using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimStepWaveSkillEffect
{
	private RuinsReclaimStepWaveSkill m_skill;

	private RuinsReclaimInstance m_ruinsReclaimInst;

	private Timer m_castingIntervalTimer;

	private bool m_bRunning;

	public RuinsReclaimStepWaveSkill skill => m_skill;

	public RuinsReclaimInstance ruinsReclaimInst => m_ruinsReclaimInst;

	public Vector3 position => m_skill.position;

	public float radius => m_skill.radius;

	public bool running => m_bRunning;

	public void Init(RuinsReclaimStepWaveSkill skill, RuinsReclaimInstance ruinsReclaimInst)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		if (ruinsReclaimInst == null)
		{
			throw new ArgumentNullException("ruinsReclaimInst");
		}
		m_skill = skill;
		m_ruinsReclaimInst = ruinsReclaimInst;
		m_bRunning = true;
		int nInterval = skill.CastingInteraval * 1000;
		m_castingIntervalTimer = new Timer(OnCastingIntervalTimerTick);
		m_castingIntervalTimer.Change(nInterval, nInterval);
	}

	private void OnCastingIntervalTimerTick(object state)
	{
		m_ruinsReclaimInst.AddWork(new SFAction(ProcessTimerTick), bGlobalLockRequired: false);
	}

	private void ProcessTimerTick()
	{
		if (m_bRunning)
		{
			m_ruinsReclaimInst.OnCastWaveSkill(this);
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

	private void DisposeHitTimer()
	{
		if (m_castingIntervalTimer != null)
		{
			m_castingIntervalTimer.Dispose();
			m_castingIntervalTimer = null;
		}
	}

	public bool Contains(Vector3 targetPosition)
	{
		return MathUtil.CircleContains(position, radius, targetPosition);
	}
}
