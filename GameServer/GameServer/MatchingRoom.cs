using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ServerFramework;

namespace GameServer;

public abstract class MatchingRoom
{
	public const int kUpdateInterval = 1000;

	public const int kBanishType_Dead = 1;

	public const int kBanishType_RidingCart = 2;

	public const int kBanishType_OpenTimeout = 3;

	public const int kBanishType_NotEnoughItem = 4;

	public const int kBanishType_NotPlace = 5;

	public const int kBanishType_DungeonEnter = 6;

	protected MatchingManager m_matchingManager;

	protected Guid m_id = Guid.Empty;

	protected MatchingStatus m_status;

	private bool m_bReleased;

	protected Dictionary<Guid, Hero> m_heroes = new Dictionary<Guid, Hero>();

	private DateTimeOffset m_statusStartTime = DateTimeOffset.MinValue;

	private Timer m_updateTimer;

	public Guid id => m_id;

	public Dictionary<Guid, Hero> heroes => m_heroes;

	public MatchingStatus status => m_status;

	public bool released => m_bReleased;

	public bool isFull => m_heroes.Count >= enterMaxMemberCount;

	public abstract int enterMinMemberCount { get; }

	public abstract int enterMaxMemberCount { get; }

	public abstract int matchingWaitingTime { get; }

	public abstract int enterWaitingTime { get; }

	public MatchingRoom()
	{
		m_id = Guid.NewGuid();
	}

	protected void InitMatchingRoom(MatchingManager matchingManager)
	{
		m_matchingManager = matchingManager;
		m_updateTimer = new Timer(OnUpdateTick);
		m_updateTimer.Change(1000, 1000);
	}

	private void OnUpdateTick(object state)
	{
		Global.instance.AddWork(new SFAction(ProcessUpdateTimerTick));
	}

	private void ProcessUpdateTimerTick()
	{
		if (m_bReleased)
		{
			return;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		float fElapsedTime = (float)(currentTime - m_statusStartTime).TotalSeconds;
		if (m_status == MatchingStatus.MatchingTimeWaiting)
		{
			if (fElapsedTime >= (float)matchingWaitingTime)
			{
				List<ClientPeer> peers = GetClientPeers(Guid.Empty);
				ChangeStatus(MatchingStatus.MatchingCompleted, peers);
			}
		}
		else if (m_status == MatchingStatus.MatchingCompleted && fElapsedTime >= (float)enterWaitingTime)
		{
			OnEnteringDungeon();
		}
		OnUpdate(currentTime);
	}

	protected virtual void OnUpdate(DateTimeOffset time)
	{
	}

	protected void ChangeStatus(MatchingStatus status, List<ClientPeer> peers)
	{
		if (m_status != status)
		{
			m_status = status;
			m_statusStartTime = DateTimeUtil.currentTime;
			float fRemainingTime = 0f;
			switch (status)
			{
			case MatchingStatus.Matching:
				fRemainingTime = 0f;
				break;
			case MatchingStatus.MatchingTimeWaiting:
				fRemainingTime = matchingWaitingTime;
				break;
			case MatchingStatus.MatchingCompleted:
				fRemainingTime = enterWaitingTime;
				break;
			}
			OnChangeStatus(fRemainingTime, peers);
		}
	}

	protected abstract void OnChangeStatus(float fRemainingTime, List<ClientPeer> peers);

	protected abstract void OnEnteringDungeon();

	public void EnterHero(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_heroes.Add(hero.id, hero);
		hero.matchingRoom = this;
		if (m_status != MatchingStatus.MatchingCompleted)
		{
			List<ClientPeer> peers = GetClientPeers(hero.id);
			if (m_heroes.Count >= enterMaxMemberCount)
			{
				ChangeStatus(MatchingStatus.MatchingCompleted, peers);
			}
			else if (m_heroes.Count >= enterMinMemberCount)
			{
				ChangeStatus(MatchingStatus.MatchingTimeWaiting, peers);
			}
		}
	}

	public void EnterPartyMembers(IEnumerable<Hero> partyMembers)
	{
		if (partyMembers == null)
		{
			throw new ArgumentNullException("partyMembers");
		}
		if (partyMembers.Count() == 0)
		{
			return;
		}
		List<ClientPeer> peers = GetClientPeers(Guid.Empty);
		foreach (Hero partyMember in partyMembers)
		{
			lock (partyMember.syncObject)
			{
				m_heroes.Add(partyMember.id, partyMember);
				partyMember.matchingRoom = this;
			}
		}
		if (m_heroes.Count >= enterMaxMemberCount)
		{
			ChangeStatus(MatchingStatus.MatchingCompleted, peers);
		}
		else if (m_heroes.Count >= enterMinMemberCount)
		{
			ChangeStatus(MatchingStatus.MatchingTimeWaiting, peers);
		}
	}

	public void ExitHero(Hero hero)
	{
		m_heroes.Remove(hero.id);
		hero.matchingRoom = null;
		List<ClientPeer> peers = GetClientPeers(Guid.Empty);
		if (m_heroes.Count < enterMinMemberCount)
		{
			ChangeStatus(MatchingStatus.Matching, peers);
		}
		if (m_heroes.Count == 0)
		{
			Close();
		}
	}

	public void BanishHero(Hero hero, int nBanishType)
	{
		m_heroes.Remove(hero.id);
		hero.matchingRoom = null;
		OnBanishHero(hero, nBanishType);
		List<ClientPeer> peers = GetClientPeers(Guid.Empty);
		if (m_heroes.Count < enterMinMemberCount)
		{
			ChangeStatus(MatchingStatus.Matching, peers);
		}
		if (m_heroes.Count == 0)
		{
			Close();
		}
	}

	protected abstract void OnBanishHero(Hero hero, int nBanishType);

	public List<ClientPeer> GetClientPeers(Guid heroIdToExclude)
	{
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.id != heroIdToExclude)
			{
				clientPeers.Add(hero.account.peer);
			}
		}
		return clientPeers;
	}

	public List<ClientPeer> GetClientPeers(IEnumerable<Guid> heroIdsToExclude)
	{
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		foreach (Hero hero in m_heroes.Values)
		{
			bool bIsExclude = false;
			foreach (Guid heroId in heroIdsToExclude)
			{
				if (hero.id == heroId)
				{
					bIsExclude = true;
					break;
				}
			}
			if (!bIsExclude)
			{
				clientPeers.Add(hero.account.peer);
			}
		}
		return clientPeers;
	}

	public float GetRemainingTime(DateTimeOffset time)
	{
		if (m_status == MatchingStatus.MatchingTimeWaiting)
		{
			return (float)Math.Max((double)matchingWaitingTime - (time - m_statusStartTime).TotalSeconds, 0.0);
		}
		if (m_status == MatchingStatus.MatchingCompleted)
		{
			return (float)Math.Max((double)enterWaitingTime - (time - m_statusStartTime).TotalSeconds, 0.0);
		}
		return 0f;
	}

	public bool IsEnterableMemberCount(int nCount)
	{
		if (enterMaxMemberCount < m_heroes.Count + nCount)
		{
			return false;
		}
		return true;
	}

	protected void Close()
	{
		Release();
		m_matchingManager.RemoveRoom(m_id);
	}

	public void Release()
	{
		if (!m_bReleased)
		{
			DisposeUpdateTimer();
			m_bReleased = true;
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
