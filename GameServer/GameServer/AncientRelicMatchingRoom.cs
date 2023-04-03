using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer;

public class AncientRelicMatchingRoom : MatchingRoom
{
	private AncientRelic m_ancientRelic;

	public override int enterMinMemberCount => m_ancientRelic.enterMinMemberCount;

	public override int enterMaxMemberCount => m_ancientRelic.enterMaxMemberCount;

	public override int matchingWaitingTime => m_ancientRelic.matchingWaitingTime;

	public override int enterWaitingTime => m_ancientRelic.enterWaitingTime;

	public void Init(AncientRelicMatchingManager matchingManager)
	{
		if (matchingManager == null)
		{
			throw new ArgumentNullException("matchingManager");
		}
		m_ancientRelic = Resource.instance.ancientRelic;
		m_status = MatchingStatus.Matching;
		InitMatchingRoom(matchingManager);
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
			EnterAncientRelic(nTotalLevel);
		}
	}

	private void EnterAncientRelic(int nTotalLevel)
	{
		int nAverageHeroLevel = nTotalLevel / m_heroes.Count;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		AncientRelicInstance ancientRelicInst = new AncientRelicInstance();
		ancientRelicInst.Init(m_ancientRelic, currentTime, nAverageHeroLevel);
		Cache.instance.AddPlace(ancientRelicInst);
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
						ServerEvent.SendAncientRelicMatchingRoomBanished(hero.account.peer, 1);
						continue;
					}
					hero.currentPlace.Exit(hero, isLogOut: false, new AncientRelicEnterParam(ancientRelicInst.instanceId, currentTime));
					clientPeers.Add(hero.account.peer);
				}
			}
		}
		m_heroes.Clear();
		if (clientPeers.Count == 0)
		{
			ancientRelicInst.Close();
		}
		else
		{
			ServerEvent.SendContinentExitForAncientRelicEnter(clientPeers);
		}
		Close();
	}

	protected override void OnChangeStatus(float fRemainingTime, List<ClientPeer> peers)
	{
		ServerEvent.SendAncientRelicMatchingStatusChanged(peers, (int)m_status, fRemainingTime);
	}

	protected override void OnBanishHero(Hero hero, int nBanishType)
	{
		ServerEvent.SendAncientRelicMatchingRoomBanished(hero.account.peer, nBanishType);
	}
}
