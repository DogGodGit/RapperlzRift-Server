using System;
using System.Collections.Generic;

namespace GameServer;

public class AncientRelicMatchingManager : MatchingManager
{
	private AncientRelic m_ancientRelic;

	public override bool isPartyMatchingEnabled => true;

	public AncientRelicMatchingManager(AncientRelic ancientRelic)
	{
		m_ancientRelic = ancientRelic;
	}

	protected override void OnEnterRoom_Party(IEnumerable<ClientPeer> peers, MatchingRoomEntranceParam param, MatchingStatus matchingStatus, float fRemainingTime)
	{
		ServerEvent.SendAncientRelicMatchingRoomPartyEnter(peers, (int)matchingStatus, fRemainingTime);
	}

	protected override bool IsEnterableMatchingRoom(Hero hero, DateTimeOffset time, MatchingRoomEntranceParam param)
	{
		if (!base.IsEnterableMatchingRoom(hero, time, param))
		{
			return false;
		}
		if (hero.GetAncientRelicAvailableEnterCount(time.Date) <= 0)
		{
			return false;
		}
		if (m_ancientRelic.requiredConditionType == 1)
		{
			if (hero.level < m_ancientRelic.requiredHeroLevel)
			{
				return false;
			}
		}
		else if (!hero.IsMainQuestCompleted(m_ancientRelic.requiredMainQuestNo))
		{
			return false;
		}
		if (hero.stamina < m_ancientRelic.requiredStamina)
		{
			return false;
		}
		return true;
	}

	protected override MatchingRoom FindRoom(int nEnterCount, MatchingRoomEntranceParam param)
	{
		MatchingRoom targetMatchingRoom = null;
		foreach (AncientRelicMatchingRoom room in m_rooms.Values)
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
		AncientRelicMatchingRoom room = new AncientRelicMatchingRoom();
		room.Init(this);
		m_rooms.Add(room.id, room);
		return room;
	}
}
