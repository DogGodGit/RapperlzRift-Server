using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer;

public class AnkouTombMatchingRoom : MatchingRoom
{
	private AnkouTomb m_ankouTomb;

	private AnkouTombDifficulty m_difficulty;

	private AnkouTombSchedule m_schedule;

	public override int enterMinMemberCount => m_ankouTomb.enterMinMemberCount;

	public override int enterMaxMemberCount => m_ankouTomb.enterMaxMemberCount;

	public override int matchingWaitingTime => m_ankouTomb.matchingWaitingTime;

	public override int enterWaitingTime => m_ankouTomb.enterWaitingTime;

	public AnkouTombDifficulty difficulty => m_difficulty;

	public void Init(AnkouTombMatchingManager matchingManager, AnkouTombDifficulty difficulty, AnkouTombSchedule schedule)
	{
		if (matchingManager == null)
		{
			throw new ArgumentNullException("matchingManager");
		}
		if (difficulty == null)
		{
			throw new ArgumentNullException("difficulty");
		}
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_schedule = schedule;
		m_difficulty = difficulty;
		m_ankouTomb = schedule.ankouTomb;
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
			ServerEvent.SendAnkouTombMatchingRoomBanished(hero.account.peer, 3);
		}
		m_heroes.Clear();
		Close();
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
			EnterAnkouTomb();
		}
	}

	private void EnterAnkouTomb()
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		AnkouTombInstance ankouTombInst = new AnkouTombInstance();
		ankouTombInst.Init(m_difficulty, m_schedule, currentTime);
		Cache.instance.AddPlace(ankouTombInst);
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
						ServerEvent.SendAnkouTombMatchingRoomBanished(hero.account.peer, 1);
						continue;
					}
					hero.currentPlace.Exit(hero, isLogOut: false, new AnkouTombEnterParam(ankouTombInst.instanceId, currentTime));
					clientPeers.Add(hero.account.peer);
				}
			}
		}
		m_heroes.Clear();
		if (clientPeers.Count == 0)
		{
			ankouTombInst.Close();
		}
		else
		{
			ServerEvent.SendContinentExitForAnkouTombEnter(clientPeers);
		}
		Close();
	}

	protected override void OnChangeStatus(float fRemainingTime, List<ClientPeer> peers)
	{
		ServerEvent.SendAnkouTombMatchingStatusChanged(peers, (int)m_status, fRemainingTime);
	}

	protected override void OnBanishHero(Hero hero, int nBanishType)
	{
		ServerEvent.SendAnkouTombMatchingRoomBanished(hero.account.peer, nBanishType);
	}
}
