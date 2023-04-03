using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SupplySupportQuestOrder
{
	private SupplySupportQuest m_supplySupportQuest;

	private int m_nId;

	private int m_nOrderItemId;

	private GoldReward m_failRefundGoldReward;

	private List<SupplySupportQuestCartPoolEntry> m_cartPoolEntries = new List<SupplySupportQuestCartPoolEntry>();

	private int m_nCartPoolEntriesTotalPoint;

	public SupplySupportQuest supplySupportQuest => m_supplySupportQuest;

	public int id => m_nId;

	public int orderItemId => m_nOrderItemId;

	public GoldReward failRefundGoldReward => m_failRefundGoldReward;

	public long failRefundGoldRewardValue
	{
		get
		{
			if (m_failRefundGoldReward == null)
			{
				return 0L;
			}
			return m_failRefundGoldReward.value;
		}
	}

	public SupplySupportQuestOrder(SupplySupportQuest supplySupportQuest)
	{
		m_supplySupportQuest = supplySupportQuest;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["orderId"]);
		m_nOrderItemId = Convert.ToInt32(dr["orderItemId"]);
		if (m_nOrderItemId > 0)
		{
			if (Resource.instance.GetItem(m_nOrderItemId) == null)
			{
				SFLogUtil.Warn(GetType(), "지령서아이템이 존재하지 않습니니다. m_nId = " + m_nId + ", m_nOrderItemId = " + m_nOrderItemId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "지령서아이템ID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nOrderItemId = " + m_nOrderItemId);
		}
		long lnFailRefundGoldRewardId = Convert.ToInt64(dr["failRefundGoldRewardId"]);
		if (lnFailRefundGoldRewardId > 0)
		{
			m_failRefundGoldReward = Resource.instance.GetGoldReward(lnFailRefundGoldRewardId);
			if (m_failRefundGoldReward == null)
			{
				SFLogUtil.Warn(GetType(), "시패시골드보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnFailRefundGoldRewardId = " + lnFailRefundGoldRewardId);
			}
		}
		else if (lnFailRefundGoldRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "실패시골드보상ID가 유효하지 않습니다. m_nId = " + m_nId + ", lnFailRefundGoldRewardId = " + lnFailRefundGoldRewardId);
		}
	}

	public void AddCartPoolEntry(SupplySupportQuestCartPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_cartPoolEntries.Add(entry);
		m_nCartPoolEntriesTotalPoint += entry.point;
	}

	public SupplySupportQuestCart SelectCart()
	{
		return SelectCartPoolEntry().acquisitionCart;
	}

	private SupplySupportQuestCartPoolEntry SelectCartPoolEntry()
	{
		return Util.SelectPickEntry(m_cartPoolEntries, m_nCartPoolEntriesTotalPoint);
	}
}
