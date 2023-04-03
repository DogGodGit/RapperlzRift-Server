using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroMainGearRefinementAttr : IAttrValuePair
{
	private HeroMainGearRefinement m_refinement;

	private int m_nIndex;

	private int m_nGrade;

	private int m_nAttrId;

	private AttrValue m_attrValue;

	private bool m_bIsProected;

	public HeroMainGearRefinement refinement
	{
		get
		{
			return m_refinement;
		}
		set
		{
			m_refinement = value;
		}
	}

	public int index => m_nIndex;

	public int grade => m_nGrade;

	public int attrId => m_nAttrId;

	public AttrValue attrValue => m_attrValue;

	public bool isProtected
	{
		get
		{
			return m_bIsProected;
		}
		set
		{
			m_bIsProected = value;
		}
	}

	int IAttrValuePair.id => m_nAttrId;

	int IAttrValuePair.value => m_attrValue.value;

	public HeroMainGearRefinementAttr()
		: this(0)
	{
	}

	public HeroMainGearRefinementAttr(int nIndex)
	{
		m_nIndex = nIndex;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nIndex = Convert.ToInt32(dr["index"]);
		m_nGrade = Convert.ToInt32(dr["grade"]);
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			throw new Exception("속성값이 존재하지 않습니다. m_nIndex = " + m_nIndex + ", lnAttrValueId = " + lnAttrValueId);
		}
	}

	public void SetAttrValue(int nGrade, int nAttrId, AttrValue attrValue)
	{
		m_nGrade = nGrade;
		m_nAttrId = nAttrId;
		m_attrValue = attrValue;
	}

	public PDHeroMainGearRefinementAttr ToHeroMainGearRefinementAttr()
	{
		PDHeroMainGearRefinementAttr inst = new PDHeroMainGearRefinementAttr();
		inst.index = m_nIndex;
		inst.grade = m_nGrade;
		inst.attrId = m_nAttrId;
		inst.attrValueId = attrValue.id;
		return inst;
	}

	public static List<PDHeroMainGearRefinementAttr> ToHeroMainGearRefinementAttrs(IEnumerable<HeroMainGearRefinementAttr> attrs)
	{
		List<PDHeroMainGearRefinementAttr> results = new List<PDHeroMainGearRefinementAttr>();
		foreach (HeroMainGearRefinementAttr attr in attrs)
		{
			results.Add(attr.ToHeroMainGearRefinementAttr());
		}
		return results;
	}
}
