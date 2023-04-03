using System;
using System.Collections.Generic;

namespace GameServer;

public class SoulCoveterMatchingManager : MatchingManager
{
	private SoulCoveter m_soulCoveter;

	public override bool isPartyMatchingEnabled => true;

	public SoulCoveterMatchingManager(SoulCoveter soulCoveter)
	{
		m_soulCoveter = soulCoveter;
	}

	protected override void OnEnterRoom_Party(IEnumerable<ClientPeer> peers, MatchingRoomEntranceParam param, MatchingStatus matchingStatus, float fRemainingTime)
	{
		SoulCoveterMatchingRoomEnterParam soulCoveterMatchingRoomEnterParam = (SoulCoveterMatchingRoomEnterParam)param;
		ServerEvent.SendSoulCoveterMatchingRoomPartyEnter(peers, soulCoveterMatchingRoomEnterParam.difficulty.difficulty, (int)matchingStatus, fRemainingTime);
	}

	protected override bool IsEnterableMatchingRoom(Hero hero, DateTimeOffset time, MatchingRoomEntranceParam param)
	{
		if (!base.IsEnterableMatchingRoom(hero, time, param))
		{
			return false;
		}
		if (hero.GetSoulCoveterAvailableEnterCount(time.Date) <= 0)
		{
			return false;
		}
		if (m_soulCoveter.requiredConditionType == 1)
		{
			if (hero.level < m_soulCoveter.requiredHeroLevel)
			{
				return false;
			}
		}
		else if (!hero.IsMainQuestCompleted(m_soulCoveter.requiredMainQuestNo))
		{
			return false;
		}
		if (hero.stamina < m_soulCoveter.requiredStamina)
		{
			return false;
		}
		return true;
	}

	protected override MatchingRoom FindRoom(int nEnterCount, MatchingRoomEntranceParam param)
	{
		SoulCoveterMatchingRoomEnterParam soulCoveterMatchingRoomEnterParam = (SoulCoveterMatchingRoomEnterParam)param;
		int nDifficulty = soulCoveterMatchingRoomEnterParam.difficulty.difficulty;
		MatchingRoom targetMatchingRoom = null;
		foreach (SoulCoveterMatchingRoom room in m_rooms.Values)
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
		SoulCoveterMatchingRoomEnterParam soulCoveterMatchingRoomEnterParam = (SoulCoveterMatchingRoomEnterParam)param;
		SoulCoveterDifficulty difficulty = soulCoveterMatchingRoomEnterParam.difficulty;
		SoulCoveterMatchingRoom room = new SoulCoveterMatchingRoom();
		room.Init(this, difficulty);
		m_rooms.Add(room.id, room);
		return room;
	}
}
