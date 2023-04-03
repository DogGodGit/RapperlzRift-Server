using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroCreatureCardShopRandomProduct
{
	private Hero m_hero;

	private CreatureCardShopRandomProduct m_product;

	private bool m_bPurchased;

	public Hero hero => m_hero;

	public CreatureCardShopRandomProduct product => m_product;

	public bool purchased
	{
		get
		{
			return m_bPurchased;
		}
		set
		{
			m_bPurchased = true;
		}
	}

	public HeroCreatureCardShopRandomProduct(Hero hero)
		: this(hero, null)
	{
	}

	public HeroCreatureCardShopRandomProduct(Hero hero, CreatureCardShopRandomProduct product)
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
		m_product = Resource.instance.GetCreatureCardShopRandomProduct(nProductId);
		if (m_product == null)
		{
			throw new Exception("존재하지 않는 크리처카드상점랜덤상품입니다. nProductId = " + nProductId);
		}
		m_bPurchased = Convert.ToBoolean(dr["purchased"]);
	}

	public PDHeroCreatureCardShopRandomProduct ToPDHeroCreatureCardShopRandomProduct()
	{
		PDHeroCreatureCardShopRandomProduct inst = new PDHeroCreatureCardShopRandomProduct();
		inst.productId = m_product.id;
		inst.purchased = m_bPurchased;
		return inst;
	}

	public static List<PDHeroCreatureCardShopRandomProduct> ToPDHeroCreatureCardShopRandomProducts(IEnumerable<HeroCreatureCardShopRandomProduct> products)
	{
		List<PDHeroCreatureCardShopRandomProduct> insts = new List<PDHeroCreatureCardShopRandomProduct>();
		foreach (HeroCreatureCardShopRandomProduct product in products)
		{
			insts.Add(product.ToPDHeroCreatureCardShopRandomProduct());
		}
		return insts;
	}
}
