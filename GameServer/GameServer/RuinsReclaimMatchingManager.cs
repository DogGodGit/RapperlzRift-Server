using System;
using System.Collections.Generic;

namespace GameServer;

public class RuinsReclaimMatchingManager : MatchingManager
{
	private RuinsReclaim m_ruinsReclaim;

	public override bool isPartyMatchingEnabled => true;

	public RuinsReclaimMatchingManager(RuinsReclaim ruinsReclaim)
	{
		m_ruinsReclaim = ruinsReclaim;
	}

	protected override void OnEnterRoom_Party(IEnumerable<ClientPeer> peers, MatchingRoomEntranceParam param, MatchingStatus matchingStatus, float fRemainingTime)
	{
		ServerEvent.SendRuinsReclaimMatchingRoomPartyEnter(peers, (int)matchingStatus, fRemainingTime);
	}

	protected override bool IsEnterableMatchingRoom(Hero hero, DateTimeOffset time, MatchingRoomEntranceParam param)
	{
		if (!base.IsEnterableMatchingRoom(hero, time, param))
		{
			return false;
		}
		if (hero.GetRuinsReclaimAvailableFreeEnterCount(time.Date) <= 0 && hero.GetItemCount(m_ruinsReclaim.enterRequiredItemId) <= 0)
		{
			return false;
		}
		if (m_ruinsReclaim.requiredConditionType == 1)
		{
			if (hero.level < m_ruinsReclaim.requiredHeroLevel)
			{
				return false;
			}
		}
		else if (!hero.IsMainQuestCompleted(m_ruinsReclaim.requiredMainQuestNo))
		{
			return false;
		}
		return true;
	}

	protected override MatchingRoom FindRoom(int nEnterCount, MatchingRoomEntranceParam param)
	{
		MatchingRoom targetMatchingRoom = null;
		foreach (RuinsReclaimMatchingRoom room in m_rooms.Values)
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
		RuinsReclaimMatchingRoomEnterParam ruinsRelcaimMatchingRoomEnterParam = (RuinsReclaimMatchingRoomEnterParam)param;
		RuinsReclaimOpenSchedule openSchedule = ruinsRelcaimMatchingRoomEnterParam.openSchedule;
		RuinsReclaimMatchingRoom room = new RuinsReclaimMatchingRoom();
		room.Init(this, openSchedule);
		m_rooms.Add(room.id, room);
		return room;
	}
}
