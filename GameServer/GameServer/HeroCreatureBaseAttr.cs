using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroCreatureBaseAttr
{
	private HeroCreature m_creature;

	private CreatureBaseAttrValue m_attr;

	private int m_nBaseValue;

	public HeroCreature creature => m_creature;

	public CreatureBaseAttrValue attr => m_attr;

	public int baseValue
	{
		get
		{
			return m_nBaseValue;
		}
		set
		{
			m_nBaseValue = value;
		}
	}

	public HeroCreatureBaseAttr(HeroCreature creature)
		: this(creature, null, 0)
	{
	}

	public HeroCreatureBaseAttr(HeroCreature creature, CreatureBaseAttrValue attr, int nValue)
	{
		if (creature == null)
		{
			throw new ArgumentNullException("creature");
		}
		m_creature = creature;
		m_attr = attr;
		m_nBaseValue = nValue;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nAttrId = Convert.ToInt32(dr["attrId"]);
		Creature creature = m_creature.creature;
		m_attr = creature.GetBaseAttrValue(nAttrId);
		if (m_attr == null)
		{
			throw new Exception("크리처기본속성이 존재하지 않습니다. nAttrId = " + nAttrId);
		}
		m_nBaseValue = Convert.ToInt32(dr["baseValue"]);
	}

	public PDHeroCreatureBaseAttr ToPDHeroCreatureBaseAttr()
	{
		PDHeroCreatureBaseAttr inst = new PDHeroCreatureBaseAttr();
		inst.attrId = m_attr.attr.attrId;
		inst.baseValue = m_nBaseValue;
		return inst;
	}

	public static List<PDHeroCreatureBaseAttr> ToPDHeroCreatureBaseAttrs(IEnumerable<HeroCreatureBaseAttr> attrs)
	{
		List<PDHeroCreatureBaseAttr> insts = new List<PDHeroCreatureBaseAttr>();
		foreach (HeroCreatureBaseAttr attr in attrs)
		{
			insts.Add(attr.ToPDHeroCreatureBaseAttr());
		}
		return insts;
	}
}
