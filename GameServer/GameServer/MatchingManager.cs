using System;
using System.Collections.Generic;

namespace GameServer;

public abstract class MatchingManager
{
	protected Dictionary<Guid, MatchingRoom> m_rooms = new Dictionary<Guid, MatchingRoom>();

	public abstract bool isPartyMatchingEnabled { get; }

	public void EnterRoom_Single(Hero hero, MatchingRoomEntranceParam param)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		if (param == null)
		{
			throw new ArgumentNullException("param");
		}
		MatchingRoom targetMatchinfRoom = MatchRoom(1, param);
		targetMatchinfRoom.EnterHero(hero);
	}

	public void EnterRoom_Party(Party party, DateTimeOffset time, MatchingRoomEntranceParam param)
	{
		if (party == null)
		{
			throw new ArgumentNullException("party");
		}
		if (param == null)
		{
			throw new ArgumentNullException("param");
		}
		if (!isPartyMatchingEnabled)
		{
			throw new Exception("Not PartyMatching Enabled Dungeon");
		}
		List<Hero> enterPartyMembers = new List<Hero>();
		List<ClientPeer> enterPartyMemberPeers = new List<ClientPeer>();
		foreach (PartyMember member in party.members)
		{
			if (!member.isLoggedIn)
			{
				continue;
			}
			Hero hero = member.hero;
			lock (hero.syncObject)
			{
				if (IsEnterableMatchingRoom(hero, time, param))
				{
					enterPartyMembers.Add(hero);
					if (hero.id != party.master.id)
					{
						enterPartyMemberPeers.Add(hero.account.peer);
					}
				}
			}
		}
		MatchingRoom targetMatchingRoom = MatchRoom(enterPartyMembers.Count, param);
		targetMatchingRoom.EnterPartyMembers(enterPartyMembers);
		OnEnterRoom_Party(enterPartyMemberPeers, param, targetMatchingRoom.status, targetMatchingRoom.GetRemainingTime(time));
	}

	protected abstract void OnEnterRoom_Party(IEnumerable<ClientPeer> peers, MatchingRoomEntranceParam param, MatchingStatus matchingStatus, float fRemainingTime);

	protected virtual bool IsEnterableMatchingRoom(Hero hero, DateTimeOffset time, MatchingRoomEntranceParam param)
	{
		if (!(hero.currentPlace is ContinentInstance))
		{
			return false;
		}
		if (hero.isMatching)
		{
			return false;
		}
		if (hero.isRidingCart)
		{
			return false;
		}
		return true;
	}

	private MatchingRoom MatchRoom(int nEnterCount, MatchingRoomEntranceParam param)
	{
		MatchingRoom targetMatchingRoom = FindRoom(nEnterCount, param);
		if (targetMatchingRoom == null)
		{
			targetMatchingRoom = CreateRoom(param);
		}
		return targetMatchingRoom;
	}

	protected abstract MatchingRoom FindRoom(int nEnterCount, MatchingRoomEntranceParam param);

	protected abstract MatchingRoom CreateRoom(MatchingRoomEntranceParam param);

	public void RemoveRoom(Guid id)
	{
		m_rooms.Remove(id);
	}

	public void Release()
	{
		foreach (MatchingRoom room in m_rooms.Values)
		{
			room.Release();
		}
		m_rooms.Clear();
	}
}
