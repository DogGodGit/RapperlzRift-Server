using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NpcShopProduct
{
	private int m_nId;

	private NpcShopCategory m_category;

	private Item m_item;

	private bool m_bItemOwned;

	private int m_nRequiredItemCount;

	private int m_nLimitCount;

	public int id => m_nId;

	public NpcShopCategory category => m_category;

	public Item item => m_item;

	public bool itemOwned => m_bItemOwned;

	public int requiredItemCount => m_nRequiredItemCount;

	public int limitCount => m_nLimitCount;

	public NpcShopProduct(NpcShopCategory category)
	{
		m_category = category;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["productId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "상품ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nItemId = Convert.ToInt32(dr["itemId"]);
		m_item = Resource.instance.GetItem(nItemId);
		if (m_item == null)
		{
			SFLogUtil.Warn(GetType(), "아이템이 존재하지 않습니다. m_nId = " + m_nId + ", nItemId = " + nItemId);
		}
		m_bItemOwned = Convert.ToBoolean(dr["itemOwned"]);
		m_nRequiredItemCount = Convert.ToInt32(dr["requiredItemCount"]);
		if (m_nRequiredItemCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요아이템수량이 유효하지 않습니다. m_nId = " + m_nId + ", m_nRequiredItemCount = " + m_nRequiredItemCount);
		}
		m_nLimitCount = Convert.ToInt32(dr["limitCount"]);
		if (m_nLimitCount < 0)
		{
			SFLogUtil.Warn(GetType(), "제한수량이 유효하지 않습니다. m_nId = " + m_nId + ", m_nLimitCount = " + m_nLimitCount);
		}
	}
}
