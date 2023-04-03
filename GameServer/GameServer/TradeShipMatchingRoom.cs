using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer;

public class TradeShipMatchingRoom : MatchingRoom
{
	private TradeShip m_tradeShip;

	private TradeShipDifficulty m_difficulty;

	private TradeShipSchedule m_schedule;

	public override int enterMinMemberCount => m_tradeShip.enterMinMemberCount;

	public override int enterMaxMemberCount => m_tradeShip.enterMaxMamberCount;

	public override int matchingWaitingTime => m_tradeShip.matchingWaitingTime;

	public override int enterWaitingTime => m_tradeShip.enterWaitingTime;

	public TradeShipDifficulty difficulty => m_difficulty;

	public void Init(TradeShipMatchingManager matchingManager, TradeShipDifficulty difficulty, TradeShipSchedule schedule)
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
		m_tradeShip = schedule.tradeShip;
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
			ServerEvent.SendTradeShipMatchingRoomBanished(hero.account.peer, 3);
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
			EnterTradeShip();
		}
	}

	private void EnterTradeShip()
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		TradeShipInstance tradeShipInst = new TradeShipInstance();
		tradeShipInst.Init(m_difficulty, m_schedule, currentTime);
		Cache.instance.AddPlace(tradeShipInst);
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
						ServerEvent.SendTradeShipMatchingRoomBanished(hero.account.peer, 1);
						continue;
					}
					hero.currentPlace.Exit(hero, isLogOut: false, new TradeShipEnterParam(tradeShipInst.instanceId, currentTime));
					clientPeers.Add(hero.account.peer);
				}
			}
		}
		m_heroes.Clear();
		if (clientPeers.Count == 0)
		{
			tradeShipInst.Close();
		}
		else
		{
			ServerEvent.SendContinentExitForTradeShipEnter(clientPeers);
		}
		Close();
	}

	protected override void OnChangeStatus(float fRemainingTime, List<ClientPeer> peers)
	{
		ServerEvent.SendTradeShipMatchingStatusChanged(peers, (int)m_status, fRemainingTime);
	}

	protected override void OnBanishHero(Hero hero, int nBanishType)
	{
		ServerEvent.SendTradeShipMatchingRoomBanished(hero.account.peer, nBanishType);
	}
}
