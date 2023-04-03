using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class DragonNestTrapInstance
{
	public const int kUpdateInterval = 500;

	public const int kHitInterval = 3;

	private DragonNestTrap m_trap;

	private DragonNestMonsterAttrFactor m_attrFactor;

	private DragonNestInstance m_dragonNestInst;

	private Timer m_updateTimer;

	private bool m_bReleased;

	public DragonNestTrap trap => m_trap;

	public DragonNestMonsterAttrFactor attrFactor => m_attrFactor;

	public DragonNestInstance dragonNestInst => m_dragonNestInst;

	public Vector3 position => m_trap.position;

	public void Init(DragonNestInstance dragonNestInst, DragonNestTrap trap, DragonNestMonsterAttrFactor attrFactor)
	{
		if (dragonNestInst == null)
		{
			throw new ArgumentNullException("dragonNestInst");
		}
		if (trap == null)
		{
			throw new ArgumentNullException("trap");
		}
		if (attrFactor == null)
		{
			throw new ArgumentNullException("attrFactor");
		}
		m_dragonNestInst = dragonNestInst;
		m_trap = trap;
		m_attrFactor = attrFactor;
		m_updateTimer = new Timer(OnUpdateTimerTick);
		m_updateTimer.Change(500, 500);
	}

	private void OnUpdateTimerTick(object state)
	{
		m_dragonNestInst.AddWork(new SFAction(OnUpdate), bGlobalLockRequired: false);
	}

	private void OnUpdate()
	{
		if (m_bReleased)
		{
			return;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (Sector sector in m_dragonNestInst.GetInterestSectorsOfPosition(position))
		{
			foreach (Hero hero in sector.heroes.Values)
			{
				lock (hero.syncObject)
				{
					if (m_trap.ContainsPosition(hero.position))
					{
						hero.HitDragonNestTrap(this, currentTime);
					}
				}
			}
		}
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
