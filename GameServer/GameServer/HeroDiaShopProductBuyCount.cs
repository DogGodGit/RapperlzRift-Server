using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroDiaShopProductBuyCount
{
	private Hero m_hero;

	private int m_nProductId;

	private int m_nBuyCount;

	public Hero hero => m_hero;

	public int productId => m_nProductId;

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

	public HeroDiaShopProductBuyCount(Hero hero)
		: this(hero, 0, 0)
	{
	}

	public HeroDiaShopProductBuyCount(Hero hero, int nProductId, int nBuyCount)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_nProductId = nProductId;
		m_nBuyCount = nBuyCount;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nProductId = Convert.ToInt32(dr["productId"]);
		m_nBuyCount = Convert.ToInt32(dr["buyCount"]);
	}

	public PDHeroDiaShopProductBuyCount ToPDHeroDiaShopProductBuyCount()
	{
		PDHeroDiaShopProductBuyCount inst = new PDHeroDiaShopProductBuyCount();
		inst.productId = m_nProductId;
		inst.buyCount = m_nBuyCount;
		return inst;
	}

	public static List<PDHeroDiaShopProductBuyCount> ToPDHeroDiaShopProductBuyCounts(IEnumerable<HeroDiaShopProductBuyCount> products)
	{
		List<PDHeroDiaShopProductBuyCount> insts = new List<PDHeroDiaShopProductBuyCount>();
		foreach (HeroDiaShopProductBuyCount product in products)
		{
			insts.Add(product.ToPDHeroDiaShopProductBuyCount());
		}
		return insts;
	}
}
