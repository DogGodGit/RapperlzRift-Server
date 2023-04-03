using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class SkillEffect
{
	private Place m_place;

	private Offense m_offense;

	private Vector3 m_position = Vector3.zero;

	private Guid m_targetId = Guid.Empty;

	private bool m_bIsHeroHitEnabled;

	private int m_nAttackerLevel;

	private int m_nCurrentHitId;

	private bool m_bRunning;

	private Timer m_hitTimer;

	public Place place => m_place;

	public Offense offense => m_offense;

	public Vector3 position => m_position;

	public Guid targetId => m_targetId;

	public int attackerLevel => m_nAttackerLevel;

	public bool isHeroHitEnabled => m_bIsHeroHitEnabled;

	public int currentHitId => m_nCurrentHitId;

	public void Init(Place place, Offense offense, Vector3 position, Guid targetId, int nUnitLevel)
	{
		if (place == null)
		{
			throw new ArgumentNullException("place");
		}
		if (offense == null)
		{
			throw new ArgumentNullException("offense");
		}
		m_place = place;
		m_offense = offense;
		m_position = position;
		m_targetId = targetId;
		m_nAttackerLevel = nUnitLevel;
		if (offense.attacker is Hero heroAttacker)
		{
			m_bIsHeroHitEnabled = !heroAttacker.isSafeMode && !heroAttacker.isDistorting;
		}
		else
		{
			m_bIsHeroHitEnabled = true;
		}
		Skill skill = m_offense.skill;
		int nSkillHitCount = skill.hitCount;
		if (nSkillHitCount < 2)
		{
			throw new Exception("스킬적중횟수가 유효하지 않습니다. nSkillHitCount = " + nSkillHitCount);
		}
		float fDuration = skill.ssDuration;
		int nHitInterval = (int)Math.Floor(fDuration * 1000f / (float)(nSkillHitCount - 1));
		int nDelay = Math.Max((int)Math.Floor(skill.ssStartDelay * 1000f), 0);
		m_hitTimer = new Timer(OnHitTimerTick);
		m_hitTimer.Change(nDelay, nHitInterval);
		m_bRunning = true;
	}

	private void OnHitTimerTick(object state)
	{
		m_place.AddWork(new SFAction(ProcessTimerTick), bGlobalLockRequired: false);
	}

	private void ProcessTimerTick()
	{
		if (m_bRunning)
		{
			m_nCurrentHitId++;
			m_place.ProcessSkillEffectHitTick(this);
			if (m_nCurrentHitId == m_offense.skill.hitCount)
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
		m_place.ProcessSkillEffectFinished(this);
	}

	private void DisposeHitTimer()
	{
		if (m_hitTimer != null)
		{
			m_hitTimer.Dispose();
			m_hitTimer = null;
		}
	}

	public bool Contains(Vector3 point)
	{
		Skill skill = m_offense.skill;
		if (skill.hitAreaType != SkillHitAreaType.Circle)
		{
			return false;
		}
		return MathUtil.CircleContains(m_position, skill.hitAreaValue1, point);
	}
}
