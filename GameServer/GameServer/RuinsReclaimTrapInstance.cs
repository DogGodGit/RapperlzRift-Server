using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimTrapInstance
{
	public const int kUpdateInterval = 500;

	public const int kHitInterval = 1;

	private RuinsReclaimTrap m_trap;

	private RuinsReclaimInstance m_currentPlace;

	private Timer m_updateTimer;

	private bool m_bReleased;

	public RuinsReclaimTrap trap => m_trap;

	public RuinsReclaimInstance currentPlace => m_currentPlace;

	public Vector3 position => m_trap.position;

	public float radius => m_trap.radius;

	public int trapDamage => m_trap.damage;

	public void Init(RuinsReclaimTrap trap, RuinsReclaimInstance ruinsReclaimInstance)
	{
		if (trap == null)
		{
			throw new ArgumentNullException("trap");
		}
		if (ruinsReclaimInstance == null)
		{
			throw new ArgumentNullException("ruinsReclaimInstance");
		}
		m_trap = trap;
		m_currentPlace = ruinsReclaimInstance;
	}

	public void Start()
	{
		m_updateTimer = new Timer(OnUpdateTimerTick);
		m_updateTimer.Change(500, 500);
	}

	private void OnUpdateTimerTick(object state)
	{
		m_currentPlace.AddWork(new SFAction(OnUpdate), bGlobalLockRequired: false);
	}

	private void OnUpdate()
	{
		OnUpdate_CheckHeroPosition();
	}

	private void OnUpdate_CheckHeroPosition()
	{
		try
		{
			CheckHeroPosition();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void CheckHeroPosition()
	{
		if (m_bReleased)
		{
			return;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (Sector sector in m_currentPlace.GetInterestSectorsOfPosition(position))
		{
			foreach (Hero hero in sector.heroes.Values)
			{
				lock (hero.syncObject)
				{
					if (InTrapPosition(hero.position))
					{
						hero.HitRuinsReclaimTrap(trapDamage, currentTime);
					}
				}
			}
		}
	}

	public bool InTrapPosition(Vector3 targetPosition)
	{
		return MathUtil.CircleContains(position, radius, targetPosition);
	}

	public void Release()
	{
		if (!m_bReleased)
		{
			m_bReleased = true;
			DisposeUpdateTimer();
		}
	}

	public void DisposeUpdateTimer()
	{
		if (m_updateTimer != null)
		{
			m_updateTimer.Dispose();
			m_updateTimer = null;
		}
	}
}
