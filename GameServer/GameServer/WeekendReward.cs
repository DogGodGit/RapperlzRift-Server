using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WeekendReward
{
	public const DayOfWeek kRewardDayOfWeek = DayOfWeek.Monday;

	private int m_nRequiredHeroLevel;

	private int m_nItemRewardRate;

	private ItemReward m_itemReward;

	private Dictionary<int, WeekendRewardNumberPool> m_numberPools = new Dictionary<int, WeekendRewardNumberPool>();

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int itemRewardRate => m_nItemRewardRate;

	public ItemReward itemReward => m_itemReward;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요영웅레벨이 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		m_nItemRewardRate = Convert.ToInt32(dr["itemRewardRate"]);
		if (m_nItemRewardRate < 0)
		{
			SFLogUtil.Warn(GetType(), "아이템보상확률이 유효하지 않습니다. m_nItemRewardRate = " + m_nItemRewardRate);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		if (lnItemRewardId > 0)
		{
			m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
			if (m_itemReward == null)
			{
				SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. lnItemRewardId = " + lnItemRewardId);
			}
		}
	}

	public void AddNumberPool(WeekendRewardNumberPool pool)
	{
		if (pool == null)
		{
			throw new ArgumentNullException("pool");
		}
		m_numberPools.Add(pool.selectionNo, pool);
	}

	public WeekendRewardNumberPool GetNumberPool(int nSelectionNo)
	{
		if (!m_numberPools.TryGetValue(nSelectionNo, out var value))
		{
			return null;
		}
		return value;
	}

	public static int GetSelectionNo(DayOfWeek dayOfWeek)
	{
		return dayOfWeek switch
		{
			DayOfWeek.Friday => 1, 
			DayOfWeek.Saturday => 2, 
			DayOfWeek.Sunday => 3, 
			_ => 0, 
		};
	}
}
