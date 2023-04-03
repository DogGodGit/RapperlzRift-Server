using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroMountGearOptionAttr : IAttrValuePair
{
	private HeroMountGear m_gear;

	private int m_nIndex;

	private int m_nGrade;

	private int m_nAttrId;

	private AttrValue m_attrValue;

	public HeroMountGear gear
	{
		get
		{
			return m_gear;
		}
		set
		{
			m_gear = value;
		}
	}

	public int index => m_nIndex;

	public int grade => m_nGrade;

	public int attrId => m_nAttrId;

	public AttrValue attrValue => m_attrValue;

	int IAttrValuePair.id => m_nAttrId;

	int IAttrValuePair.value => m_attrValue.value;

	public HeroMountGearOptionAttr()
		: this(null, 0, 0, 0, null)
	{
	}

	public HeroMountGearOptionAttr(HeroMountGear gear, int nIndex, int nGrade, int nAttrId, AttrValue attrValue)
	{
		m_gear = gear;
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
		long lnAttrValue = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValue);
		if (m_attrValue == null)
		{
			throw new Exception("속성값이 존재하지 않습니다. lnAttrValue = " + lnAttrValue);
		}
	}

	public void SetOptionAttr(int nGrade, int nAttrId, AttrValue attrValue)
	{
		m_nGrade = nGrade;
		m_nAttrId = nAttrId;
		m_attrValue = attrValue;
	}

	public PDHeroMountGearOptionAttr ToPDHeroMountGearOptionAttr()
	{
		PDHeroMountGearOptionAttr inst = new PDHeroMountGearOptionAttr();
		inst.index = m_nIndex;
		inst.grade = m_nGrade;
		inst.attrId = m_nAttrId;
		inst.attrValueId = m_attrValue.id;
		return inst;
	}

	public static List<PDHeroMountGearOptionAttr> ToPDHeroMountGearOptionAttrs(IEnumerable<HeroMountGearOptionAttr> optionAttrs)
	{
		List<PDHeroMountGearOptionAttr> insts = new List<PDHeroMountGearOptionAttr>();
		foreach (HeroMountGearOptionAttr attr in optionAttrs)
		{
			insts.Add(attr.ToPDHeroMountGearOptionAttr());
		}
		return insts;
	}
}
