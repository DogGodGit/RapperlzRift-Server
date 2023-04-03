using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FishingQuest
{
	private Npc m_npc;

	private int m_nRequiredHeroLevel;

	private int m_nLimitCount;

	private int m_nCastingCount;

	private int m_nCastingInterval;

	private float m_fPartyRadius;

	private float m_fPartyExpRewardFactor;

	private float m_fGuildExpRewardFactor;

	private Dictionary<int, FishingQuestSpot> m_spots = new Dictionary<int, FishingQuestSpot>();

	private Dictionary<int, FishingQuestBait> m_baits = new Dictionary<int, FishingQuestBait>();

	private Dictionary<int, FishingQuestGuildTerritorySpot> m_guildTerritorySpots = new Dictionary<int, FishingQuestGuildTerritorySpot>();

	public Npc npc => m_npc;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int limitCount => m_nLimitCount;

	public int castingCount => m_nCastingCount;

	public int castingInterval => m_nCastingInterval;

	public float partyRadius => m_fPartyRadius;

	public float partyExpRewardFactor => m_fPartyExpRewardFactor;

	public float guildExpRewardFactor => m_fGuildExpRewardFactor;

	public Dictionary<int, FishingQuestSpot> spots => m_spots;

	public Dictionary<int, FishingQuestBait> baits => m_baits;

	private Dictionary<int, FishingQuestGuildTerritorySpot> guildTerritorySpots => m_guildTerritorySpots;

	public FishingQuest()
	{
		ItemType itemType = Resource.instance.GetItemType(14);
		if (itemType == null)
		{
			SFLogUtil.Warn(GetType(), "낚시미끼아이템타입이 존재하지 않습니다.");
			return;
		}
		foreach (Item item in itemType.items.Values)
		{
			m_baits.Add(item.id, new FishingQuestBait(item.id));
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nNpcId = Convert.ToInt32(dr["npcId"]);
		m_npc = Resource.instance.GetNpc(nNpcId);
		if (m_npc == null)
		{
			SFLogUtil.Warn(GetType(), "NPC가 존재하지 않습니다. nNpcId = " + nNpcId);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "필요영웅레벨이 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		m_nLimitCount = Convert.ToInt32(dr["limitCount"]);
		if (m_nLimitCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한횟수가 유효하지 않습니다. m_nLimitCount = " + m_nLimitCount);
		}
		m_nCastingCount = Convert.ToInt32(dr["castingCount"]);
		if (m_nCastingCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "낚시 캐스팅 횟수가 유효하지 않습니다. m_nCastingCount = " + m_nCastingCount);
		}
		m_nCastingInterval = Convert.ToInt32(dr["castingInterval"]);
		if (m_nCastingInterval <= 0)
		{
			SFLogUtil.Warn(GetType(), "낚시 캐스팅 간격이 유효하지 않습니다. m_nCastingInterval = " + m_nCastingInterval);
		}
		m_fPartyRadius = Convert.ToSingle(dr["partyRadius"]);
		if (m_fPartyRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "파티 반지름이 유효하지 않습니다. m_fPartyRadius = " + m_fPartyRadius);
		}
		m_fPartyExpRewardFactor = Convert.ToSingle(dr["partyExpRewardFactor"]);
		if (m_fPartyExpRewardFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "파티 경험치 보상계수가 유효하지 않습니다. m_fPartyExpRewardFactor = " + m_fPartyExpRewardFactor);
		}
		m_fGuildExpRewardFactor = Convert.ToSingle(dr["guildExpRewardFactor"]);
		if (m_fGuildExpRewardFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "길드경험치보상계수가 유효하지 않습니다. m_fGuildExpRewardFactor = " + m_fGuildExpRewardFactor);
		}
	}

	public void AddSpot(FishingQuestSpot spot)
	{
		if (spot == null)
		{
			throw new ArgumentNullException("spot");
		}
		m_spots.Add(spot.id, spot);
	}

	public FishingQuestSpot GetSpot(int nId)
	{
		if (!m_spots.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddBait(FishingQuestBait bait)
	{
		if (bait == null)
		{
			throw new ArgumentNullException("bait");
		}
		m_baits.Add(bait.itemId, bait);
	}

	public FishingQuestBait GetBait(int nId)
	{
		if (!m_baits.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddGuildTerritorySpot(FishingQuestGuildTerritorySpot spot)
	{
		if (spot == null)
		{
			throw new ArgumentNullException("spot");
		}
		m_guildTerritorySpots.Add(spot.id, spot);
	}

	public FishingQuestGuildTerritorySpot GetGuildTerritorySpot(int nId)
	{
		if (!m_guildTerritorySpots.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}
}
