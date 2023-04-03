using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer;

public class RuinsReclaimMatchingRoom : MatchingRoom
{
	private RuinsReclaim m_ruinsReclaim;

	private RuinsReclaimOpenSchedule m_openSchedule;

	public override int enterMinMemberCount => m_ruinsReclaim.enterMinMemberCount;

	public override int enterMaxMemberCount => m_ruinsReclaim.enterMaxMemberCount;

	public override int matchingWaitingTime => m_ruinsReclaim.matchingWaitingTime;

	public override int enterWaitingTime => m_ruinsReclaim.enterWaitingTime;

	public void Init(RuinsReclaimMatchingManager matchingManager, RuinsReclaimOpenSchedule openSchedule)
	{
		if (matchingManager == null)
		{
			throw new ArgumentNullException("matchingManager");
		}
		if (openSchedule == null)
		{
			throw new ArgumentNullException("openSchedule");
		}
		m_openSchedule = openSchedule;
		m_ruinsReclaim = Resource.instance.ruinsReclaim;
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
			ServerEvent.SendRuinsReclaimMatchingRoomBanished(hero.account.peer, 3);
		}
		m_heroes.Clear();
		Close();
	}

	protected override void OnEnteringDungeon()
	{
		int nTotalLevel = 0;
		DateTime currentDate = DateTimeUtil.currentTime.Date;
		int nEnterRequiredItemId = m_ruinsReclaim.enterRequiredItemId;
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
				else if (hero.GetRuinsReclaimAvailableFreeEnterCount(currentDate) <= 0)
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
			EnterRuinsReclaim(nTotalLevel);
		}
	}

	private void EnterRuinsReclaim(int nTotalLevel)
	{
		int nAverageHeroLevel = nTotalLevel / m_heroes.Count;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		RuinsReclaimInstance ruinsReclaimInst = new RuinsReclaimInstance();
		ruinsReclaimInst.Init(m_openSchedule, currentTime, nAverageHeroLevel);
		Cache.instance.AddPlace(ruinsReclaimInst);
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		int nEnterRequiredItemId = m_ruinsReclaim.enterRequiredItemId;
		foreach (Hero hero in m_heroes.Values)
		{
			lock (hero.currentPlace.syncObject)
			{
				lock (hero.syncObject)
				{
					hero.matchingRoom = null;
					if (hero.isDead)
					{
						ServerEvent.SendRuinsReclaimMatchingRoomBanished(hero.account.peer, 1);
						continue;
					}
					if (hero.GetRuinsReclaimAvailableFreeEnterCount(currentDate) <= 0 && hero.GetItemCount(nEnterRequiredItemId) <= 0)
					{
						ServerEvent.SendRuinsReclaimMatchingRoomBanished(hero.account.peer, 4);
						continue;
					}
					hero.currentPlace.Exit(hero, isLogOut: false, new RuinsReclaimEnterParam(ruinsReclaimInst.instanceId, currentTime));
					clientPeers.Add(hero.account.peer);
				}
			}
		}
		m_heroes.Clear();
		if (clientPeers.Count == 0)
		{
			ruinsReclaimInst.Close();
		}
		else
		{
			ServerEvent.SendContinentExitForRuinsReclaimEnter(clientPeers);
		}
		Close();
	}

	protected override void OnChangeStatus(float fRemainingTime, List<ClientPeer> peers)
	{
		ServerEvent.SendRuinsReclaimMatchingStatusChanged(peers, (int)m_status, fRemainingTime);
	}

	protected override void OnBanishHero(Hero hero, int nBanishType)
	{
		ServerEvent.SendRuinsReclaimMatchingRoomBanished(hero.account.peer, nBanishType);
	}
}
