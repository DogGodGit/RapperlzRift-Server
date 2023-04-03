using System;
using System.Collections.Generic;

namespace GameServer;

public class TradeShipMatchingManager : MatchingManager
{
	private TradeShip m_tradeShip;

	public override bool isPartyMatchingEnabled => true;

	public TradeShipMatchingManager(TradeShip tradeShip)
	{
		m_tradeShip = tradeShip;
	}

	protected override void OnEnterRoom_Party(IEnumerable<ClientPeer> peers, MatchingRoomEntranceParam param, MatchingStatus matchingStatus, float fRemainingTime)
	{
		TradeShipMatchingRoomEnterParam tradeShipMatchingRoomEnterParam = (TradeShipMatchingRoomEnterParam)param;
		ServerEvent.SendTradeShipMatchingRoomPartyEnter(peers, tradeShipMatchingRoomEnterParam.difficulty.difficulty, (int)matchingStatus, fRemainingTime);
	}

	protected override bool IsEnterableMatchingRoom(Hero hero, DateTimeOffset time, MatchingRoomEntranceParam param)
	{
		if (!base.IsEnterableMatchingRoom(hero, time, param))
		{
			return false;
		}
		if (hero.GetTradeShipAvailableEnterCount(time.Date) <= 0)
		{
			return false;
		}
		if (m_tradeShip.requiredConditionType == 1)
		{
			if (hero.level < m_tradeShip.requiredHeroLevel)
			{
				return false;
			}
		}
		else if (!hero.IsMainQuestCompleted(m_tradeShip.requiredMainQuestNo))
		{
			return false;
		}
		TradeShipMatchingRoomEnterParam tradeShipMatchingRoomEnterParam = (TradeShipMatchingRoomEnterParam)param;
		TradeShipDifficulty difficulty = tradeShipMatchingRoomEnterParam.difficulty;
		if (hero.level < difficulty.minHeroLevel || hero.level > difficulty.maxHeroLevel)
		{
			return false;
		}
		if (hero.stamina < m_tradeShip.requiredStamina)
		{
			return false;
		}
		return true;
	}

	protected override MatchingRoom FindRoom(int nEnterCount, MatchingRoomEntranceParam param)
	{
		TradeShipMatchingRoomEnterParam tradeShipMatchingRoomEnterParam = (TradeShipMatchingRoomEnterParam)param;
		int nDifficulty = tradeShipMatchingRoomEnterParam.difficulty.difficulty;
		MatchingRoom targetMatchingRoom = null;
		foreach (TradeShipMatchingRoom room in m_rooms.Values)
		{
			if (room.difficulty.difficulty == nDifficulty && room.IsEnterableMemberCount(nEnterCount))
			{
				if (targetMatchingRoom == null)
				{
					targetMatchingRoom = room;
				}
				else if (room.heroes.Count > targetMatchingRoom.heroes.Count)
				{
					targetMatchingRoom = room;
				}
			}
		}
		return targetMatchingRoom;
	}

	protected override MatchingRoom CreateRoom(MatchingRoomEntranceParam param)
	{
		TradeShipMatchingRoomEnterParam tradeShipMatchingRoomEnterParam = (TradeShipMatchingRoomEnterParam)param;
		TradeShipDifficulty difficulty = tradeShipMatchingRoomEnterParam.difficulty;
		TradeShipSchedule schedule = tradeShipMatchingRoomEnterParam.schedule;
		TradeShipMatchingRoom room = new TradeShipMatchingRoom();
		room.Init(this, difficulty, schedule);
		m_rooms.Add(room.id, room);
		return room;
	}
}
