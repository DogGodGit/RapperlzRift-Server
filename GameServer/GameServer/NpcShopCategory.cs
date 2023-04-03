using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NpcShopCategory
{
	private NpcShop m_shop;

	private int m_nId;

	private int m_requiredItemId;

	private Dictionary<int, NpcShopProduct> m_products = new Dictionary<int, NpcShopProduct>();

	public NpcShop shop => m_shop;

	public int id => m_nId;

	public int requiredItemId => m_requiredItemId;

	public Dictionary<int, NpcShopProduct> products => m_products;

	public NpcShopCategory(NpcShop shop)
	{
		m_shop = shop;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["categoryId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "카테고리ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_requiredItemId = Convert.ToInt32(dr["requiredItemId"]);
		if (Resource.instance.GetItem(m_requiredItemId) == null)
		{
			SFLogUtil.Warn(GetType(), "필요아이템이 존재하지 않습니다. m_nId = " + m_nId + ", itemId = " + m_requiredItemId);
		}
	}

	public void AddNpcShopProduct(NpcShopProduct product)
	{
		if (product == null)
		{
			throw new ArgumentNullException("product");
		}
		m_products.Add(product.id, product);
	}

	public NpcShopProduct GetNpcShopProduct(int nId)
	{
		if (!m_products.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}
}
