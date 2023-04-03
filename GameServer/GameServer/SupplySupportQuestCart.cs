using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SupplySupportQuestCart
{
	private SupplySupportQuest m_supplySupportQuest;

	private Cart m_cart;

	private ItemReward m_destructionItemReward;

	private int m_nSortNo;

	private List<SupplySupportQuestChangeCartPoolEntry> m_changeCartPoolEntries = new List<SupplySupportQuestChangeCartPoolEntry>();

	private int m_nChangeCartPoolEntriesTotalPoint;

	public SupplySupportQuest supplySupportQuest => m_supplySupportQuest;

	public Cart cart => m_cart;

	public int id => m_cart.id;

	public ItemReward destructionItemReward => m_destructionItemReward;

	public int sortNo => m_nSortNo;

	public SupplySupportQuestCart(SupplySupportQuest supplySupportQuest)
	{
		m_supplySupportQuest = supplySupportQuest;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nCartId = Convert.ToInt32(dr["cartId"]);
		if (nCartId > 0)
		{
			m_cart = Resource.instance.GetCart(nCartId);
			if (m_cart == null)
			{
				SFLogUtil.Warn(GetType(), "카트가 존재하지 않습니다. nCardId = " + nCartId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "카트ID가 유효하지 않습니다. nCartId = " + nCartId);
		}
		long lnDestructionItemRewardId = Convert.ToInt64(dr["destructionItemRewardId"]);
		if (lnDestructionItemRewardId > 0)
		{
			m_destructionItemReward = Resource.instance.GetItemReward(lnDestructionItemRewardId);
			if (m_destructionItemReward == null)
			{
				SFLogUtil.Warn(GetType(), "파괴시획득아이템보상이 존재하지 않습니다. id = " + id + ", lnDestructionItemRewardId = " + lnDestructionItemRewardId);
			}
		}
		else if (lnDestructionItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "파괴시획득아이템보상ID가 유효하지 않습니다. id = " + id + ", lnDestructionItemRewardId = " + lnDestructionItemRewardId);
		}
		m_nSortNo = Convert.ToInt32(dr["sortNo"]);
	}

	public void AddChangeCartPoolEntry(SupplySupportQuestChangeCartPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_changeCartPoolEntries.Add(entry);
		m_nChangeCartPoolEntriesTotalPoint += entry.point;
	}

	public SupplySupportQuestCart SelectChangeCart()
	{
		return SelectChangeCartPoolEntry().acquisitionCart;
	}

	private SupplySupportQuestChangeCartPoolEntry SelectChangeCartPoolEntry()
	{
		return Util.SelectPickEntry(m_changeCartPoolEntries, m_nChangeCartPoolEntriesTotalPoint);
	}
}
