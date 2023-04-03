using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroMainGearOptionAttr : IAttrValuePair
{
	private HeroMainGear m_heroMainGear;

	private int m_nIndex;

	private int m_nGrade;

	private int m_nAttrId;

	private AttrValue m_attrValue;

	public HeroMainGear heroMainGear
	{
		get
		{
			return m_heroMainGear;
		}
		set
		{
			m_heroMainGear = value;
		}
	}

	public int index => m_nIndex;

	public int attrGrade => m_nGrade;

	public int attrId => m_nAttrId;

	public AttrValue attrValue => m_attrValue;

	int IAttrValuePair.id => m_nAttrId;

	int IAttrValuePair.value => m_attrValue.value;

	public HeroMainGearOptionAttr()
		: this(null, 0, 0, 0, null)
	{
	}

	public HeroMainGearOptionAttr(HeroMainGear heroMainGear, int nIndex, int nGrade, int nAttrId, AttrValue attrValue)
	{
		m_heroMainGear = heroMainGear;
		m_nIndex = nIndex;
		m_nGrade = nGrade;
		m_nAttrId = nAttrId;
		m_attrValue = attrValue;
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

	public void SetAttrValue(MainGearOptionAttrPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_nGrade = entry.attrGrade;
		m_nAttrId = entry.attrId;
		m_attrValue = entry.attrValue;
	}

	public PDHeroMainGearOptionAttr ToPDHeroMainGearOptionAttr()
	{
		PDHeroMainGearOptionAttr inst = new PDHeroMainGearOptionAttr();
		inst.index = m_nIndex;
		inst.grade = m_nGrade;
		inst.attrId = m_nAttrId;
		inst.attrValueId = m_attrValue.id;
		return inst;
	}

	public FieldOfHonorHeroMainGearOptionAttr ToFieldOfHonorHeroMainGearOptionAttr(FieldOfHonorHeroEquippedMainGear heroMainGear)
	{
		FieldOfHonorHeroMainGearOptionAttr inst = new FieldOfHonorHeroMainGearOptionAttr(heroMainGear);
		inst.index = m_nIndex;
		inst.grade = m_nGrade;
		inst.attrId = m_nAttrId;
		inst.attrValue = m_attrValue;
		return inst;
	}

	public static List<PDHeroMainGearOptionAttr> ToPDHeroMainGearOptionAttrs(IEnumerable<HeroMainGearOptionAttr> attrs)
	{
		List<PDHeroMainGearOptionAttr> results = new List<PDHeroMainGearOptionAttr>();
		foreach (HeroMainGearOptionAttr attr in attrs)
		{
			results.Add(attr.ToPDHeroMainGearOptionAttr());
		}
		return results;
	}
}
