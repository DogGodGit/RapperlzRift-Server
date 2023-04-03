using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer;

public class WarMemoryMatchingRoom : MatchingRoom
{
	private WarMemory m_warMemory;

	private WarMemorySchedule m_schedule;

	public override int enterMinMemberCount => m_warMemory.enterMinMemberCount;

	public override int enterMaxMemberCount => m_warMemory.enterMaxMemberCount;

	public override int matchingWaitingTime => m_warMemory.matchingWaitingTime;

	public override int enterWaitingTime => m_warMemory.enterWaitingTime;

	public void Init(WarMemoryMatchingManager matchingManager, WarMemorySchedule schedule)
	{
		if (matchingManager == null)
		{
			throw new ArgumentNullException("matchingManager");
		}
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_schedule = schedule;
		m_warMemory = schedule.warMemory;
		m_status = MatchingStatus.Matching;
		InitMatchingRoom(matchingManager);
	}

	protected override void OnUpdate(DateTimeOffset time)
	{
		base.OnUpdate(time);
		if (m_heroes.Count == 0)
		{
			return;
		}
		int nTime = (int)time.TimeOfDay.TotalSeconds;
		if (m_schedule.IsEnterable(nTime))
		{
			return;
		}
		foreach (Hero hero in m_heroes.Values)
		{
			hero.matchingRoom = null;
			ServerEvent.SendWarMemoryMatchingRoomBanished(hero.account.peer, 3);
		}
		m_heroes.Clear();
		Close();
	}

	protected override void OnEnteringDungeon()
	{
		int nTotalLevel = 0;
		DateTime currentDate = DateTimeUtil.currentTime.Date;
		int nEnterRequiredItemId = m_warMemory.enterRequiredItemId;
		Hero[] array = m_heroes.Values.ToArray();
		foreach (Hero hero in array)
		{
			lock (hero.syncObject)
			{
				if (hero.currentPlace == null)
				{
					BanishHero(hero, 5);
				}
				else if (hero.isDead)
				{
					BanishHero(hero, 1);
				}
				else if (hero.GetWarMemoryAvailableFreeEnterCount(currentDate) <= 0)
				{
					if (hero.GetItemCount(nEnterRequiredItemId) <= 0)
					{
						BanishHero(hero, 4);
					}
				}
				else
				{
					nTotalLevel += hero.level;
				}
			}
		}
		if (m_status == MatchingStatus.MatchingCompleted)
		{
			EnterWarMemory(nTotalLevel);
		}
	}

	private void EnterWarMemory(int nTotalLevel)
	{
		int nAverageHeroLevel = nTotalLevel / m_heroes.Count;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		WarMemoryInstance warMemoryInst = new WarMemoryInstance();
		warMemoryInst.Init(m_schedule, currentTime, nAverageHeroLevel);
		Cache.instance.AddPlace(warMemoryInst);
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		int nEnterRequiredItemId = m_warMemory.enterRequiredItemId;
		foreach (Hero hero in m_heroes.Values)
		{
			lock (hero.currentPlace.syncObject)
			{
				lock (hero.syncObject)
				{
					hero.matchingRoom = null;
					if (hero.isDead)
					{
						ServerEvent.SendWarMemoryMatchingRoomBanished(hero.account.peer, 1);
						continue;
					}
					if (hero.GetWarMemoryAvailableFreeEnterCount(currentDate) <= 0 && hero.GetItemCount(nEnterRequiredItemId) <= 0)
					{
						ServerEvent.SendWarMemoryMatchingRoomBanished(hero.account.peer, 3);
						continue;
					}
					hero.currentPlace.Exit(hero, isLogOut: false, new WarMemoryEnterParam(warMemoryInst.instanceId, currentTime));
					clientPeers.Add(hero.account.peer);
				}
			}
		}
		m_heroes.Clear();
		if (clientPeers.Count == 0)
		{
			warMemoryInst.Close();
		}
		else
		{
			ServerEvent.SendContinentExitForWarMemoryEnter(clientPeers);
		}
		Close();
	}

	protected override void OnChangeStatus(float fRemainingTime, List<ClientPeer> peers)
	{
		ServerEvent.SendWarMemoryMatchingStatusChanged(peers, (int)m_status, fRemainingTime);
	}

	protected override void OnBanishHero(Hero hero, int nBanishType)
	{
		ServerEvent.SendWarMemoryMatchingRoomBanished(hero.account.peer, nBanishType);
	}
}
