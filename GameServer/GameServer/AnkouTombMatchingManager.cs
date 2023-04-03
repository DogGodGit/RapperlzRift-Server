using System;
using System.Collections.Generic;

namespace GameServer;

public class AnkouTombMatchingManager : MatchingManager
{
	private AnkouTomb m_ankouTomb;

	public override bool isPartyMatchingEnabled => true;

	public AnkouTombMatchingManager(AnkouTomb ankouTomb)
	{
		m_ankouTomb = ankouTomb;
	}

	protected override void OnEnterRoom_Party(IEnumerable<ClientPeer> peers, MatchingRoomEntranceParam param, MatchingStatus matchingStatus, float fRemainingTime)
	{
		AnkouTombMatchingRoomEnterParam ankouTombMatchingRoomEnterParam = (AnkouTombMatchingRoomEnterParam)param;
		ServerEvent.SendAnkouTombMatchingRoomPartyEnter(peers, ankouTombMatchingRoomEnterParam.difficulty.difficulty, (int)matchingStatus, fRemainingTime);
	}

	protected override bool IsEnterableMatchingRoom(Hero hero, DateTimeOffset time, MatchingRoomEntranceParam param)
	{
		if (!base.IsEnterableMatchingRoom(hero, time, param))
		{
			return false;
		}
		if (hero.GetAnkouTombAvailableEnterCount(time.Date) <= 0)
		{
			return false;
		}
		if (m_ankouTomb.requiredConditionType == 1)
		{
			if (hero.level < m_ankouTomb.requiredHeroLevel)
			{
				return false;
			}
		}
		else if (!hero.IsMainQuestCompleted(m_ankouTomb.requiredMainQuestNo))
		{
			return false;
		}
		AnkouTombMatchingRoomEnterParam ankouTombMatchingRoomEnterParam = (AnkouTombMatchingRoomEnterParam)param;
		AnkouTombDifficulty difficulty = ankouTombMatchingRoomEnterParam.difficulty;
		if (hero.level < difficulty.minHeroLevel || hero.level > difficulty.maxHeroLevel)
		{
			return false;
		}
		if (hero.stamina < m_ankouTomb.requiredStamina)
		{
			return false;
		}
		return true;
	}

	protected override MatchingRoom FindRoom(int nEnterCount, MatchingRoomEntranceParam param)
	{
		AnkouTombMatchingRoomEnterParam ankouTombMatchingRoomEnterParam = (AnkouTombMatchingRoomEnterParam)param;
		int nDifficulty = ankouTombMatchingRoomEnterParam.difficulty.difficulty;
		MatchingRoom targetMatchingRoom = null;
		foreach (AnkouTombMatchingRoom room in m_rooms.Values)
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
		AnkouTombMatchingRoomEnterParam ankouTombMatchingRoomEnterParam = (AnkouTombMatchingRoomEnterParam)param;
		AnkouTombDifficulty difficulty = ankouTombMatchingRoomEnterParam.difficulty;
		AnkouTombSchedule schedule = ankouTombMatchingRoomEnterParam.schedule;
		AnkouTombMatchingRoom room = new AnkouTombMatchingRoom();
		room.Init(this, difficulty, schedule);
		m_rooms.Add(room.id, room);
		return room;
	}
}
