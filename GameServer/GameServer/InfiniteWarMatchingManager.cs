using System;
using System.Collections.Generic;

namespace GameServer;

public class InfiniteWarMatchingManager : MatchingManager
{
	private InfiniteWar m_infiniteWar;

	public override bool isPartyMatchingEnabled => false;

	public InfiniteWarMatchingManager(InfiniteWar infiniteWar)
	{
		m_infiniteWar = infiniteWar;
	}

	protected override void OnEnterRoom_Party(IEnumerable<ClientPeer> peers, MatchingRoomEntranceParam param, MatchingStatus matchingStatus, float fRemainingTime)
	{
	}

	protected override bool IsEnterableMatchingRoom(Hero hero, DateTimeOffset time, MatchingRoomEntranceParam param)
	{
		if (!base.IsEnterableMatchingRoom(hero, time, param))
		{
			return false;
		}
		if (hero.GetInfiniteWarAvailableEnterCount(time.Date) <= 0)
		{
			return false;
		}
		if (m_infiniteWar.requiredConditionType == 1)
		{
			if (hero.level < m_infiniteWar.requiredHeroLevel)
			{
				return false;
			}
		}
		else if (!hero.IsMainQuestCompleted(m_infiniteWar.requiredMainQuestNo))
		{
			return false;
		}
		if (hero.stamina < m_infiniteWar.requiredStamina)
		{
			return false;
		}
		return true;
	}

	protected override MatchingRoom FindRoom(int nEnterCount, MatchingRoomEntranceParam param)
	{
		MatchingRoom targetMatchingRoom = null;
		foreach (InfiniteWarMatchingRoom room in m_rooms.Values)
		{
			if (room.IsEnterableMemberCount(nEnterCount))
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
		InfiniteWarMatchingRoomEnterParam infiniteWarMatchingRoomEnterParam = (InfiniteWarMatchingRoomEnterParam)param;
		InfiniteWarOpenSchedule openSchedule = infiniteWarMatchingRoomEnterParam.openSchedule;
		InfiniteWarMatchingRoom room = new InfiniteWarMatchingRoom();
		room.Init(this, openSchedule);
		m_rooms.Add(room.id, room);
		return room;
	}
}
