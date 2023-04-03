using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class AncientRelicTrapInstance
{
	public const int kUpdateInterval = 500;

	public const int kStatus_Init = 0;

	public const int kStatus_StartWaiting = 1;

	public const int kStatus_Activation = 2;

	public const int kStatus_Inactivation = 3;

	public const int kHitInterval = 3;

	private AncientRelicTrap m_trap;

	private AncientRelicInstance m_currentPlace;

	private float m_fTrapFactor;

	private int m_nStatus;

	private DateTimeOffset m_statusChangeTime = DateTimeOffset.MinValue;

	private Timer m_updateTimer;

	private bool m_bReleased;

	public AncientRelicTrap trap => m_trap;

	public AncientRelicInstance currentPlace => m_currentPlace;

	public Vector3 position => m_trap.position;

	public float width => m_trap.width;

	public float Height => m_trap.height;

	public int startDelayTime => m_trap.startDelayTime;

	public int regenInterval => m_trap.regenInterval;

	public int duration => m_trap.duration;

	public int trapPenaltyMoveSpeed => m_trap.ancientRelic.trapPenaltyMoveSpeed;

	public int trapDamage => (int)Math.Floor((float)m_trap.ancientRelic.trapDamage * m_fTrapFactor);

	public int status => m_nStatus;

	public void Init(AncientRelicTrap trap, AncientRelicInstance ancientRelicInstance, float fTrapFactor)
	{
		if (trap == null)
		{
			throw new ArgumentNullException("trap");
		}
		if (ancientRelicInstance == null)
		{
			throw new ArgumentNullException("ancientRelicInstance");
		}
		m_trap = trap;
		m_currentPlace = ancientRelicInstance;
		m_fTrapFactor = fTrapFactor;
	}

	public void Start(DateTimeOffset time)
	{
		m_nStatus = 1;
		m_statusChangeTime = time;
		m_updateTimer = new Timer(OnUpdateTimerTick);
		m_updateTimer.Change(500, 500);
	}

	private void OnUpdateTimerTick(object state)
	{
		m_currentPlace.AddWork(new SFAction(OnUpdate), bGlobalLockRequired: false);
	}

	private void OnUpdate()
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		OnUpdate_UpdateStatus(currentTime);
		OnUpdate_InHeroPositionCheck(currentTime);
	}

	private void OnUpdate_UpdateStatus(DateTimeOffset time)
	{
		try
		{
			UpdateStatus(time);
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void UpdateStatus(DateTimeOffset time)
	{
		float fElapsedTime = (float)(time - m_statusChangeTime).TotalSeconds;
		switch (m_nStatus)
		{
		case 1:
			if (fElapsedTime > (float)startDelayTime)
			{
				StatusChange(2, time);
			}
			break;
		case 2:
			if (fElapsedTime > (float)duration)
			{
				StatusChange(3, time);
			}
			break;
		case 3:
			if (fElapsedTime > (float)regenInterval)
			{
				StatusChange(2, time);
			}
			break;
		}
	}

	private void StatusChange(int nStatus, DateTimeOffset time)
	{
		if (!m_bReleased && m_nStatus != 0 && m_nStatus != nStatus)
		{
			m_nStatus = nStatus;
			m_statusChangeTime = time;
			if (m_nStatus == 2)
			{
				ServerEvent.SendAncientRelicTrapActivated(m_currentPlace.GetClientPeers(), m_trap.id);
			}
		}
	}

	private void OnUpdate_InHeroPositionCheck(DateTimeOffset time)
	{
		try
		{
			InHeroPositionCheck(time);
		}
		catch (Exception ex)
		{
			SFLogUtil.Warn(GetType(), null, ex);
		}
	}

	private void InHeroPositionCheck(DateTimeOffset time)
	{
		if (m_nStatus != 2)
		{
			return;
		}
		foreach (Sector sector in m_currentPlace.GetInterestSectorsOfPosition(position))
		{
			foreach (Hero hero in sector.heroes.Values)
			{
				lock (hero.syncObject)
				{
					if (InTrapHitPosition(hero.position))
					{
						hero.HitAncientRelicTrap(trapDamage, trapPenaltyMoveSpeed, duration, time);
					}
				}
			}
		}
	}

	public bool InTrapHitPosition(Vector3 targetPosition)
	{
		float fHalfWidth = width / 2f;
		float fHalfHeight = Height / 2f;
		if (targetPosition.x >= position.x - fHalfWidth && targetPosition.x <= position.x + fHalfWidth && targetPosition.z >= position.z - fHalfHeight)
		{
			return targetPosition.z <= position.z + fHalfHeight;
		}
		return false;
	}

	public void Release()
	{
		if (!m_bReleased)
		{
			m_bReleased = true;
			DisposeUpdateTimer();
		}
	}

	private void DisposeUpdateTimer()
	{
		if (m_updateTimer != null)
		{
			m_updateTimer.Dispose();
			m_updateTimer = null;
		}
	}
}
