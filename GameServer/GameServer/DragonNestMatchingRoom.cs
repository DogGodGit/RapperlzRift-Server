using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer;

public class DragonNestMatchingRoom : MatchingRoom
{
	private DragonNest m_dragonNest;

	public override int enterMinMemberCount => m_dragonNest.enterMinMemberCount;

	public override int enterMaxMemberCount => m_dragonNest.enterMaxMemberCount;

	public override int matchingWaitingTime => m_dragonNest.matchingWaitingTime;

	public override int enterWaitingTime => m_dragonNest.enterWaitingTime;

	public void Init(DragonNestMatchingManager matchingManager)
	{
		if (matchingManager == null)
		{
			throw new ArgumentNullException("matchingManager");
		}
		m_dragonNest = Resource.instance.dragonNest;
		m_status = MatchingStatus.Matching;
		InitMatchingRoom(matchingManager);
	}

	protected override void OnEnteringDungeon()
	{
		int nTotalLevel = 0;
		int nEnterRequiredItemId = m_dragonNest.enterRequiredItemId;
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
				else if (hero.GetItemCount(nEnterRequiredItemId) <= 0)
				{
					BanishHero(hero, 4);
				}
				else
				{
					nTotalLevel += hero.level;
				}
			}
		}
		if (m_status == MatchingStatus.MatchingCompleted)
		{
			EnterDragonNest(nTotalLevel);
		}
	}

	private void EnterDragonNest(int nTotalLevel)
	{
		int nAverageHeroLevel = nTotalLevel / m_heroes.Count;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		_ = currentTime.Date;
		DragonNestInstance dragonNestInst = new DragonNestInstance();
		dragonNestInst.Init(m_dragonNest, currentTime, nAverageHeroLevel);
		Cache.instance.AddPlace(dragonNestInst);
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		int nEnterRequiredItemId = m_dragonNest.enterRequiredItemId;
		foreach (Hero hero in m_heroes.Values)
		{
			lock (hero.currentPlace.syncObject)
			{
				lock (hero.syncObject)
				{
					hero.matchingRoom = null;
					if (hero.isDead)
					{
						ServerEvent.SendDragonNestMatchingRoomBanished(hero.account.peer, 1);
						continue;
					}
					if (hero.GetItemCount(nEnterRequiredItemId) <= 0)
					{
						ServerEvent.SendDragonNestMatchingRoomBanished(hero.account.peer, 4);
						continue;
					}
					hero.currentPlace.Exit(hero, isLogOut: false, new DragonNestEnterParam(dragonNestInst.instanceId, currentTime));
					clientPeers.Add(hero.account.peer);
				}
			}
		}
		m_heroes.Clear();
		if (clientPeers.Count == 0)
		{
			dragonNestInst.Close();
		}
		else
		{
			ServerEvent.SendContinentExitForDragonNestEnter(clientPeers);
		}
		Close();
	}

	protected override void OnChangeStatus(float fRemainingTime, List<ClientPeer> peers)
	{
		ServerEvent.SendDragonNestMatchingStatusChanged(peers, (int)m_status, fRemainingTime);
	}

	protected override void OnBanishHero(Hero hero, int nBanishType)
	{
		ServerEvent.SendDragonNestMatchingRoomBanished(hero.account.peer, nBanishType);
	}
}
