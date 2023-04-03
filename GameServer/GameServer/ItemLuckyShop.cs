using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ItemLuckyShop
{
	private int m_nFreePickCount;

	private int m_nFreePickWaitingTime;

	private int m_nPick1TimeDia;

	private int m_nPick5TimeDia;

	private int m_nPick5TimeSpecialPickCount;

	private GoldReward m_pick1TimeGoldReward;

	private GoldReward m_pick5TimeGoldReward;

	private List<ItemLuckyShopNormalPoolEntry> m_normalPool = new List<ItemLuckyShopNormalPoolEntry>();

	private int m_nTotalNormalPickPoint;

	private List<ItemLuckyShopSpecialPoolEntry> m_specialPool = new List<ItemLuckyShopSpecialPoolEntry>();

	private int m_nTotalSpecialPickPoint;

	public int freePickCount => m_nFreePickCount;

	public int freePickWaitingTime => m_nFreePickWaitingTime;

	public int pick1TimeDia => m_nPick1TimeDia;

	public int pick5TimeDia => m_nPick5TimeDia;

	public int pick5TimeSpecialPickCount => m_nPick5TimeSpecialPickCount;

	public GoldReward pick1TimeGoldReward => m_pick1TimeGoldReward;

	public long pick1TimeGoldRewardValue
	{
		get
		{
			if (m_pick1TimeGoldReward == null)
			{
				return 0L;
			}
			return m_pick1TimeGoldReward.value;
		}
	}

	public GoldReward pick5TimeGoldReward => m_pick5TimeGoldReward;

	public long pick5TimeGoldRewardValue
	{
		get
		{
			if (m_pick5TimeGoldReward == null)
			{
				return 0L;
			}
			return m_pick5TimeGoldReward.value;
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nFreePickCount = Convert.ToInt32(dr["freePickCount"]);
		if (m_nFreePickCount < 0)
		{
			SFLogUtil.Warn(GetType(), "무료뽑기횟수가 유효하지 않습니다. m_nFreePickCount = " + m_nFreePickCount);
		}
		m_nFreePickWaitingTime = Convert.ToInt32(dr["freePickWaitingTime"]);
		if (m_nFreePickWaitingTime < 0)
		{
			SFLogUtil.Warn(GetType(), "무료뽑기대기시간이 유효하지 않습니다. m_nFreePickWaitingTime = " + m_nFreePickWaitingTime);
		}
		m_nPick1TimeDia = Convert.ToInt32(dr["pick1TimeDia"]);
		if (m_nPick1TimeDia < 0)
		{
			SFLogUtil.Warn(GetType(), "뽑기1회다이아가 유효하지 않습니다. m_nFreePickWaitingTime = " + m_nFreePickWaitingTime);
		}
		m_nPick5TimeDia = Convert.ToInt32(dr["pick5TimeDia"]);
		if (m_nPick1TimeDia < 0)
		{
			SFLogUtil.Warn(GetType(), "뽑기5회다이아가 유효하지 않습니다. m_nFreePickWaitingTime = " + m_nFreePickWaitingTime);
		}
		m_nPick5TimeSpecialPickCount = Convert.ToInt32(dr["pick5TimeSpecialPickCount"]);
		if (m_nPick5TimeSpecialPickCount < 0 || m_nPick5TimeSpecialPickCount > 5)
		{
			SFLogUtil.Warn(GetType(), "뽑기5회특수뽑기횟수가 유효하지 않습니다. m_nFreePickWaitingTime = " + m_nFreePickWaitingTime);
		}
		long lnPick1TimeGoldRewardId = Convert.ToInt64(dr["pick1TimeGoldRewardId"]);
		if (lnPick1TimeGoldRewardId > 0)
		{
			m_pick1TimeGoldReward = Resource.instance.GetGoldReward(lnPick1TimeGoldRewardId);
			if (m_pick1TimeGoldReward == null)
			{
				SFLogUtil.Warn(GetType(), "뽑기1회골드보상이 존재하지 않습니다. lnPick1TimeGoldRewardId = " + lnPick1TimeGoldRewardId);
			}
		}
		long lnPick5TimeGoldRewardId = Convert.ToInt64(dr["pick5TimeGoldRewardId"]);
		if (lnPick5TimeGoldRewardId > 0)
		{
			m_pick5TimeGoldReward = Resource.instance.GetGoldReward(lnPick5TimeGoldRewardId);
			if (m_pick5TimeGoldReward == null)
			{
				SFLogUtil.Warn(GetType(), "뽑기5회골드보상이 존재하지 않습니다. lnPick5TimeGoldRewardId = " + lnPick5TimeGoldRewardId);
			}
		}
	}

	public void AddNormalEntry(ItemLuckyShopNormalPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_normalPool.Add(entry);
		m_nTotalNormalPickPoint += entry.point;
	}

	public ItemLuckyShopNormalPoolEntry GetNormalEntry(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex > m_normalPool.Count)
		{
			return null;
		}
		return m_normalPool[nIndex];
	}

	public ItemLuckyShopNormalPoolEntry SelectNormalEntry()
	{
		return Util.SelectPickEntry(m_normalPool, m_nTotalNormalPickPoint);
	}

	public List<ItemLuckyShopNormalPoolEntry> SelectNormalEntries(int nCount)
	{
		return Util.SelectPickEntries(m_normalPool, m_nTotalNormalPickPoint, nCount, bDuplicated: true);
	}

	public void AddSpecialEntry(ItemLuckyShopSpecialPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_specialPool.Add(entry);
		m_nTotalSpecialPickPoint += entry.point;
	}

	public ItemLuckyShopSpecialPoolEntry GetSpecialEntry(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_specialPool.Count)
		{
			return null;
		}
		return m_specialPool[nNo];
	}

	public List<ItemLuckyShopSpecialPoolEntry> SelectSpecialEntries(int nCount)
	{
		return Util.SelectPickEntries(m_specialPool, m_nTotalSpecialPickPoint, nCount, bDuplicated: true);
	}
}
