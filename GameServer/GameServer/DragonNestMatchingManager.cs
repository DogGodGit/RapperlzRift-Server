using System;
using System.Collections.Generic;

namespace GameServer;

public class DragonNestMatchingManager : MatchingManager
{
	private DragonNest m_dragonNest;

	public override bool isPartyMatchingEnabled => true;

	public DragonNestMatchingManager(DragonNest dragonNest)
	{
		m_dragonNest = dragonNest;
	}

	protected override void OnEnterRoom_Party(IEnumerable<ClientPeer> peers, MatchingRoomEntranceParam param, MatchingStatus matchingStatus, float fRemainingTime)
	{
		ServerEvent.SendDragonNestMatchingRoomPartyEnter(peers, (int)matchingStatus, fRemainingTime);
	}

	protected override bool IsEnterableMatchingRoom(Hero hero, DateTimeOffset time, MatchingRoomEntranceParam param)
	{
		if (!base.IsEnterableMatchingRoom(hero, time, param))
		{
			return false;
		}
		if (hero.GetItemCount(m_dragonNest.enterRequiredItemId) <= 0)
		{
			return false;
		}
		if (m_dragonNest.requiredConditionType == 1)
		{
			if (hero.level < m_dragonNest.requiredHeroLevel)
			{
				return false;
			}
		}
		else if (!hero.IsMainQuestCompleted(m_dragonNest.requiredMainQuestNo))
		{
			return false;
		}
		return true;
	}

	protected override MatchingRoom FindRoom(int nEnterCount, MatchingRoomEntranceParam param)
	{
		MatchingRoom targetMatchingRoom = null;
		foreach (DragonNestMatchingRoom room in m_rooms.Values)
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
		DragonNestMatchingRoom room = new DragonNestMatchingRoom();
		room.Init(this);
		m_rooms.Add(room.id, room);
		return room;
	}
}
