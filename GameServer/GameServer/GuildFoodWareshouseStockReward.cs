using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public struct GuildFoodWareshouseStockRewardKey
{
	public int itemId;

	public int heroLevel;

	public GuildFoodWareshouseStockRewardKey(int nItemId, int nHeroLevel)
	{
		itemId = nItemId;
		heroLevel = nHeroLevel;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is GuildFoodWareshouseStockRewardKey))
		{
			return false;
		}
		return Equals((GuildFoodWareshouseStockRewardKey)obj);
	}

	public bool Equals(GuildFoodWareshouseStockRewardKey other)
	{
		return this == other;
	}

	public override int GetHashCode()
	{
		return itemId.GetHashCode() ^ heroLevel.GetHashCode();
	}

	public override string ToString()
	{
		return $"({itemId},{heroLevel})";
	}

	public static bool operator ==(GuildFoodWareshouseStockRewardKey a, GuildFoodWareshouseStockRewardKey b)
	{
		if (a.itemId == b.itemId)
		{
			return a.heroLevel == b.heroLevel;
		}
		return false;
	}

	public static bool operator !=(GuildFoodWareshouseStockRewardKey a, GuildFoodWareshouseStockRewardKey b)
	{
		if (a.itemId == b.itemId)
		{
			return a.heroLevel != b.heroLevel;
		}
		return true;
	}
}
public class GuildFoodWareshouseStockReward
{
	private int m_nItemId;

	private int m_nHeroLevel;

	private ExpReward m_expReward;

	private int itemId => m_nItemId;

	public int heroLevel => m_nHeroLevel;

	public ExpReward expReward => m_expReward;

	public long expRewardValue
	{
		get
		{
			if (m_expReward == null)
			{
				return 0L;
			}
			return m_expReward.value;
		}
	}

	public GuildFoodWareshouseStockRewardKey key => new GuildFoodWareshouseStockRewardKey(m_nItemId, m_nHeroLevel);

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nItemId = Convert.ToInt32(dr["itemId"]);
		if (m_nItemId <= 0)
		{
			SFLogUtil.Warn(GetType(), "아이템ID가 유효하지 않습니다. m_nItemId = " + m_nItemId);
		}
		m_nHeroLevel = Convert.ToInt32(dr["heroLevel"]);
		if (m_nHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "영웅레벨이 유효하지 않습니다. m_nItemId = " + m_nItemId + ", m_nHeroLevel = " + m_nHeroLevel);
		}
		long lnExpRewardId = Convert.ToInt64(dr["expRewardId"]);
		m_expReward = Resource.instance.GetExpReward(lnExpRewardId);
		if (m_expReward == null)
		{
			SFLogUtil.Warn(GetType(), "경험치보상이 존재하지 않습니다. m_nItemId = " + m_nItemId + ", m_nHeroLevel = " + m_nHeroLevel + ", lnExpRewardId = " + lnExpRewardId);
		}
	}
}
