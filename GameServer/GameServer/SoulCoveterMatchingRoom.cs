using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer;

public class SoulCoveterMatchingRoom : MatchingRoom
{
	private SoulCoveter m_soulCoveter;

	private SoulCoveterDifficulty m_difficulty;

	public override int enterMinMemberCount => m_soulCoveter.enterMinMemberCount;

	public override int enterMaxMemberCount => m_soulCoveter.enterMaxMemberCount;

	public override int matchingWaitingTime => m_soulCoveter.matchingWaitingTime;

	public override int enterWaitingTime => m_soulCoveter.enterWaitingTime;

	public SoulCoveterDifficulty difficulty => m_difficulty;

	public void Init(SoulCoveterMatchingManager matchingManager, SoulCoveterDifficulty difficulty)
	{
		if (matchingManager == null)
		{
			throw new ArgumentNullException("matchingManager");
		}
		m_difficulty = difficulty;
		m_soulCoveter = Resource.instance.soulCoveter;
		m_status = MatchingStatus.Matching;
		InitMatchingRoom(matchingManager);
	}

	protected override void OnEnteringDungeon()
	{
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
			}
		}
		if (m_status == MatchingStatus.MatchingCompleted)
		{
			EnterSoulCoveter();
		}
	}

	private void EnterSoulCoveter()
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		SoulCoveterInstance soulCoveterInst = new SoulCoveterInstance();
		soulCoveterInst.Init(m_difficulty, currentTime);
		Cache.instance.AddPlace(soulCoveterInst);
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
						ServerEvent.SendSoulCoveterMatchingRoomBanished(hero.account.peer, 1);
						continue;
					}
					hero.currentPlace.Exit(hero, isLogOut: false, new SoulCoveterEnterParam(soulCoveterInst.instanceId, currentTime));
					clientPeers.Add(hero.account.peer);
				}
			}
		}
		m_heroes.Clear();
		if (clientPeers.Count == 0)
		{
			soulCoveterInst.Close();
		}
		else
		{
			ServerEvent.SendContinentExitForSoulCoveterEnter(clientPeers);
		}
		Close();
	}

	protected override void OnChangeStatus(float fRemainingTime, List<ClientPeer> peers)
	{
		ServerEvent.SendSoulCoveterMatchingStatusChanged(peers, (int)m_status, fRemainingTime);
	}

	protected override void OnBanishHero(Hero hero, int nBanishType)
	{
		ServerEvent.SendSoulCoveterMatchingRoomBanished(hero.account.peer, nBanishType);
	}
}
