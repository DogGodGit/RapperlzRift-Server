using System;
using System.Collections.Generic;

namespace GameServer;

public class WarMemoryMatchingManager : MatchingManager
{
	private WarMemory m_warMemory;

	public override bool isPartyMatchingEnabled => true;

	public WarMemoryMatchingManager(WarMemory warMemory)
	{
		m_warMemory = warMemory;
	}

	protected override void OnEnterRoom_Party(IEnumerable<ClientPeer> peers, MatchingRoomEntranceParam param, MatchingStatus matchingStatus, float fRemainingTime)
	{
		ServerEvent.SendWarMemoryMatchingRoomPartyEnter(peers, (int)matchingStatus, fRemainingTime);
	}

	protected override bool IsEnterableMatchingRoom(Hero hero, DateTimeOffset time, MatchingRoomEntranceParam param)
	{
		if (!base.IsEnterableMatchingRoom(hero, time, param))
		{
			return false;
		}
		if (hero.GetWarMemoryAvailableFreeEnterCount(time.Date) <= 0 && hero.GetItemCount(m_warMemory.enterRequiredItemId) <= 0)
		{
			return false;
		}
		if (m_warMemory.requiredConditionType == 1)
		{
			if (hero.level < m_warMemory.requiredHeroLevel)
			{
				return false;
			}
		}
		else if (!hero.IsMainQuestCompleted(m_warMemory.requiredMainQuestNo))
		{
			return false;
		}
		return true;
	}

	protected override MatchingRoom FindRoom(int nEnterCount, MatchingRoomEntranceParam param)
	{
		MatchingRoom targetMatchingRoom = null;
		foreach (WarMemoryMatchingRoom room in m_rooms.Values)
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
		WarMemoryMatchingRoomEnterParam warMemoryMatchingRoomEnterParam = (WarMemoryMatchingRoomEnterParam)param;
		WarMemorySchedule schedule = warMemoryMatchingRoomEnterParam.schedule;
		WarMemoryMatchingRoom room = new WarMemoryMatchingRoom();
		room.Init(this, schedule);
		m_rooms.Add(room.id, room);
		return room;
	}
}
