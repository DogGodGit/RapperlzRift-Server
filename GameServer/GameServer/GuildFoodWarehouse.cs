using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class GuildFoodWarehouse
{
	private int m_nLimitCount;

	private GuildTerritoryNpc m_npc;

	private int m_nLevelUpRequiredItemType;

	private ItemReward m_fullLevelItemReward;

	private List<GuildFoodWarehouseLevel> m_levels = new List<GuildFoodWarehouseLevel>();

	private Dictionary<GuildFoodWareshouseStockRewardKey, GuildFoodWareshouseStockReward> m_stockRewards = new Dictionary<GuildFoodWareshouseStockRewardKey, GuildFoodWareshouseStockReward>();

	public int limitCount => m_nLimitCount;

	public GuildTerritoryNpc npc => m_npc;

	public int levelUpRequiredItemType => m_nLevelUpRequiredItemType;

	public ItemReward fullLevelItemReward => m_fullLevelItemReward;

	public GuildFoodWarehouseLevel lastLevel => m_levels.LastOrDefault();

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLimitCount = Convert.ToInt32(dr["limitCount"]);
		if (m_nLimitCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한횟수가 유효하지 않습니다. m_nLimitCount = " + m_nLimitCount);
		}
		int nGuildTerritoryNpcId = Convert.ToInt32(dr["guildTerritoryNpcId"]);
		m_npc = Resource.instance.GetGuildTerritoryNpc(nGuildTerritoryNpcId);
		if (m_npc == null)
		{
			SFLogUtil.Warn(GetType(), "NPC가 존재하지 않습니다. nGuildTerritoryNpcId = " + nGuildTerritoryNpcId);
		}
		m_nLevelUpRequiredItemType = Convert.ToInt32(dr["levelUpRequiredItemType"]);
		if (!Resource.instance.ContainsItemType(m_nLevelUpRequiredItemType))
		{
			SFLogUtil.Warn(GetType(), "레벨업아이템타입이 존재하지 않습니다. m_nLevelUpRequiredItemType = " + m_nLevelUpRequiredItemType);
		}
		long lnFullLevelItemRewardId = Convert.ToInt64(dr["fullLevelItemRewardId"]);
		m_fullLevelItemReward = Resource.instance.GetItemReward(lnFullLevelItemRewardId);
		if (m_fullLevelItemReward == null)
		{
			SFLogUtil.Warn(GetType(), "만렙아이템보상이 존재하지 않습니다. lnFullLevelItemRewardId = " + lnFullLevelItemRewardId);
		}
	}

	public void AddLevel(GuildFoodWarehouseLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_levels.Add(level);
	}

	public GuildFoodWarehouseLevel GetLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_levels.Count)
		{
			return null;
		}
		return m_levels[nIndex];
	}

	public void AddStockReward(GuildFoodWareshouseStockReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_stockRewards.Add(reward.key, reward);
	}

	public GuildFoodWareshouseStockReward GetStockReward(GuildFoodWareshouseStockRewardKey key)
	{
		if (!m_stockRewards.TryGetValue(key, out var value))
		{
			return null;
		}
		return value;
	}

	public GuildFoodWareshouseStockReward GetStockReward(int nItemId, int nHeroLevel)
	{
		return GetStockReward(new GuildFoodWareshouseStockRewardKey(nItemId, nHeroLevel));
	}
}
