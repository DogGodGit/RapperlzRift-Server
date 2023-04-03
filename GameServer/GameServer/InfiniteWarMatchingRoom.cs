using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer;

public class InfiniteWarMatchingRoom : MatchingRoom
{
	private InfiniteWar m_infiniteWar;

	private InfiniteWarOpenSchedule m_openSchedule;

	public override int enterMinMemberCount => m_infiniteWar.enterMinMemberCount;

	public override int enterMaxMemberCount => m_infiniteWar.enterMaxMemberCount;

	public override int matchingWaitingTime => m_infiniteWar.matchingWaitingTime;

	public override int enterWaitingTime => m_infiniteWar.enterWaitingTime;

	public void Init(InfiniteWarMatchingManager matchingManager, InfiniteWarOpenSchedule openSchedule)
	{
		if (matchingManager == null)
		{
			throw new ArgumentNullException("matchingManager");
		}
		if (openSchedule == null)
		{
			throw new ArgumentException("openSchedule");
		}
		m_infiniteWar = Resource.instance.infiniteWar;
		m_openSchedule = openSchedule;
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
		if (m_openSchedule.IsEnterable(nTime))
		{
			return;
		}
		foreach (Hero hero in m_heroes.Values)
		{
			hero.matchingRoom = null;
			ServerEvent.SendInfiniteWarMatchingRoomBanished(hero.account.peer, 3);
		}
		m_heroes.Clear();
		Close();
	}

	protected override void OnEnteringDungeon()
	{
		int nTotalLevel = 0;
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
				else
				{
					nTotalLevel += hero.level;
				}
			}
		}
		if (m_status == MatchingStatus.MatchingCompleted)
		{
			EnterInfiniteWar(nTotalLevel);
		}
	}

	private void EnterInfiniteWar(int nTotalLevel)
	{
		int nAverageHeroLevel = nTotalLevel / m_heroes.Count;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		InfiniteWarInstance infiniteWarInst = new InfiniteWarInstance();
		infiniteWarInst.Init(m_openSchedule, currentTime, nAverageHeroLevel);
		Cache.instance.AddPlace(infiniteWarInst);
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		foreach (Hero hero in m_heroes.Values)
		{
			lock (hero.currentPlace.syncObject)
			{
				lock (hero.syncObject)
				{
					hero.matchingRoom = null;
					if (hero.isDead)
					{
						ServerEvent.SendInfiniteWarMatchingRoomBanished(hero.account.peer, 1);
						continue;
					}
					hero.currentPlace.Exit(hero, isLogOut: false, new InfiniteWarEnterParam(infiniteWarInst.instanceId, currentTime));
					clientPeers.Add(hero.account.peer);
				}
			}
		}
		m_heroes.Clear();
		if (clientPeers.Count == 0)
		{
			infiniteWarInst.Close();
		}
		else
		{
			ServerEvent.SendContinentExitForInfiniteWarEnter(clientPeers);
		}
		Close();
	}

	protected override void OnChangeStatus(float fRemainingTime, List<ClientPeer> peers)
	{
		ServerEvent.SendInfiniteWarMatchingStatusChanged(peers, (int)m_status, fRemainingTime);
	}

	protected override void OnBanishHero(Hero hero, int nBanishType)
	{
		ServerEvent.SendInfiniteWarMatchingRoomBanished(hero.account.peer, nBanishType);
	}
}
