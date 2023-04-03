using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FieldOfHonorHeroMainGearOptionAttr
{
	private FieldOfHonorHeroEquippedMainGear m_heroMainGear;

	private int m_nIndex;

	private int m_nGrade;

	private int m_nAttrId;

	private AttrValue m_attrValue;

	public FieldOfHonorHeroEquippedMainGear heroMainGear => m_heroMainGear;

	public FieldOfHonorHero hero => m_heroMainGear.hero;

	public int index
	{
		get
		{
			return m_nIndex;
		}
		set
		{
			m_nIndex = value;
		}
	}

	public int grade
	{
		get
		{
			return m_nGrade;
		}
		set
		{
			m_nGrade = value;
		}
	}

	public int attrId
	{
		get
		{
			return m_nAttrId;
		}
		set
		{
			m_nAttrId = value;
		}
	}

	public AttrValue attrValue
	{
		get
		{
			return m_attrValue;
		}
		set
		{
			m_attrValue = value;
		}
	}

	public FieldOfHonorHeroMainGearOptionAttr(FieldOfHonorHeroEquippedMainGear heroMainGear)
	{
		m_heroMainGear = heroMainGear;
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
		if (lnAttrValueId > 0)
		{
			m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
			if (m_attrValue == null)
			{
				SFLogUtil.Warn(GetType(), string.Concat("속성값이 존재하지 않습니다. heroMainGearId = ", m_heroMainGear.heroMainGearId, ", m_nIndex = ", m_nIndex, ", lnAttrValueId = ", lnAttrValueId));
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), string.Concat("속성값ID가 유효하지 않습니다. heroMainGearId = ", m_heroMainGear.heroMainGearId, ", m_nIndex = ", m_nIndex, ", lnAttrValueId = ", lnAttrValueId));
		}
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
}
