using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroNpcShopProduct
{
	private Hero m_hero;

	private NpcShopProduct m_product;

	private int m_nBuyCount;

	public Hero hero => m_hero;

	public NpcShopProduct product => m_product;

	public int buyCount
	{
		get
		{
			return m_nBuyCount;
		}
		set
		{
			m_nBuyCount = value;
		}
	}

	public bool buyable => m_nBuyCount < m_product.limitCount;

	public HeroNpcShopProduct(Hero hero)
		: this(hero, null)
	{
	}

	public HeroNpcShopProduct(Hero hero, NpcShopProduct product)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_product = product;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nProductId = Convert.ToInt32(dr["productId"]);
		m_product = Resource.instance.GetNpcShopProduct(nProductId);
		if (m_product == null)
		{
			throw new Exception(string.Concat("NPC상점상품이 존재하지 않습니다. heroId = ", m_hero.id, ", nProductId = ", nProductId));
		}
		m_nBuyCount = Convert.ToInt32(dr["buyCount"]);
	}

	public PDHeroNpcShopProduct ToPDHeroNpcShopProduct()
	{
		PDHeroNpcShopProduct inst = new PDHeroNpcShopProduct();
		inst.productId = m_product.id;
		inst.buyCount = m_nBuyCount;
		return inst;
	}

	public static List<PDHeroNpcShopProduct> ToPDHeroNpcShopProducts(IEnumerable<HeroNpcShopProduct> products)
	{
		List<PDHeroNpcShopProduct> insts = new List<PDHeroNpcShopProduct>();
		foreach (HeroNpcShopProduct product in products)
		{
			insts.Add(product.ToPDHeroNpcShopProduct());
		}
		return insts;
	}
}
