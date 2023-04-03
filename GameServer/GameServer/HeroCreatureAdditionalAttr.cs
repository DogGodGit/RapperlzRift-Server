using System;
using System.Data;

namespace GameServer;

public class HeroCreatureAdditionalAttr
{
	private HeroCreature m_creature;

	private int m_nAttrNo;

	private CreatureAdditionalAttr m_attr;

	public HeroCreature creature => m_creature;

	public int no => m_nAttrNo;

	public CreatureAdditionalAttr attr
	{
		get
		{
			return m_attr;
		}
		set
		{
			m_attr = value;
		}
	}

	public HeroCreatureAdditionalAttr(HeroCreature creature)
		: this(creature, 0, null)
	{
	}

	public HeroCreatureAdditionalAttr(HeroCreature creature, int nAttrNo, CreatureAdditionalAttr attr)
	{
		if (creature == null)
		{
			throw new ArgumentNullException("creature");
		}
		m_creature = creature;
		m_nAttrNo = nAttrNo;
		m_attr = attr;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nAttrNo = Convert.ToInt32(dr["attrNo"]);
		int nAttrId = Convert.ToInt32(dr["attrId"]);
		m_attr = Resource.instance.GetCreatureAdditionalAttr(nAttrId);
		if (m_attr == null)
		{
			throw new Exception("존재하지 않는 추가속성입니다.");
		}
	}
}
