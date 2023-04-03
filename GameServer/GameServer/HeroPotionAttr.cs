using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroPotionAttr
{
	private Hero m_hero;

	private PotionAttr m_potionAttr;

	private int m_nCount;

	public Hero hero => m_hero;

	public PotionAttr potionAttr => m_potionAttr;

	public int count
	{
		get
		{
			return m_nCount;
		}
		set
		{
			m_nCount = value;
		}
	}

	public HeroPotionAttr(Hero hero)
		: this(hero, null)
	{
	}

	public HeroPotionAttr(Hero hero, PotionAttr attr)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_potionAttr = attr;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nPotionAttrId = Convert.ToInt32(dr["potionAttrId"]);
		m_potionAttr = Resource.instance.GetPotionAttr(nPotionAttrId);
		if (m_potionAttr == null)
		{
			throw new Exception("물약속성이 존재하지 않습니다. nPotionAttrId = " + nPotionAttrId);
		}
		m_nCount = Convert.ToInt32(dr["count"]);
	}

	public PDHeroPotionAttr ToPDHeroPotionAttr()
	{
		PDHeroPotionAttr inst = new PDHeroPotionAttr();
		inst.potionAttrId = m_potionAttr.id;
		inst.count = m_nCount;
		return inst;
	}

	public static List<PDHeroPotionAttr> ToPDHeroPotionAttrs(IEnumerable<HeroPotionAttr> attrs)
	{
		List<PDHeroPotionAttr> insts = new List<PDHeroPotionAttr>();
		foreach (HeroPotionAttr attr in attrs)
		{
			insts.Add(attr.ToPDHeroPotionAttr());
		}
		return insts;
	}
}
