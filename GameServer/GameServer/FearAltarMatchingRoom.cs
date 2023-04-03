using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer;

public class FearAltarMatchingRoom : MatchingRoom
{
	private FearAltar m_fearAltar;

	public override int enterMinMemberCount => m_fearAltar.enterMinMemberCount;

	public override int enterMaxMemberCount => m_fearAltar.enterMaxMemberCount;

	public override int matchingWaitingTime => m_fearAltar.matchingWaitingTime;

	public override int enterWaitingTime => m_fearAltar.enterWaitingTime;

	public void Init(FearAltarMatchingManager matchingManager)
	{
		if (matchingManager == null)
		{
			throw new ArgumentNullException("matchingManager");
		}
		m_fearAltar = Resource.instance.fearAltar;
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
			EnterFearAltar(nTotalLevel);
		}
	}

	private void EnterFearAltar(int nTotalLevel)
	{
		int nAverageHeroLevel = nTotalLevel / m_heroes.Count;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		FearAltarStage stage = m_fearAltar.SelectStage();
		FearAltarInstance fearAltarInst = new FearAltarInstance();
		fearAltarInst.Init(stage, currentTime, nAverageHeroLevel);
		Cache.instance.AddPlace(fearAltarInst);
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
						ServerEvent.SendFearAltarMatchingRoomBanished(hero.account.peer, 1);
						continue;
					}
					hero.currentPlace.Exit(hero, isLogOut: false, new FearAltarEnterParam(fearAltarInst.instanceId, currentTime));
					clientPeers.Add(hero.account.peer);
				}
			}
		}
		m_heroes.Clear();
		if (clientPeers.Count == 0)
		{
			fearAltarInst.Close();
		}
		else
		{
			ServerEvent.SendContinentExitForFearAltarEnter(clientPeers, stage.id);
		}
		Close();
	}

	protected override void OnChangeStatus(float fRemainingTime, List<ClientPeer> peers)
	{
		ServerEvent.SendFearAltarMatchingStatusChanged(peers, (int)m_status, fRemainingTime);
	}

	protected override void OnBanishHero(Hero hero, int nBanishType)
	{
		ServerEvent.SendFearAltarMatchingRoomBanished(hero.account.peer, nBanishType);
	}
}
