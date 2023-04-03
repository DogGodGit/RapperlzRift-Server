using System;
using System.Data;
using ClientCommon;

namespace GameServer;

public class FieldOfHonorHeroRealAttr
{
	private FieldOfHonorHero m_hero;

	private int m_nId;

	private int m_nValue;

	public FieldOfHonorHero hero => m_hero;

	public int id
	{
		get
		{
			return m_nId;
		}
		set
		{
			m_nId = value;
		}
	}

	public int value
	{
		get
		{
			return m_nValue;
		}
		set
		{
			m_nValue = value;
		}
	}

	public FieldOfHonorHeroRealAttr(FieldOfHonorHero hero)
	{
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["attrId"]);
		m_nValue = Convert.ToInt32(dr["value"]);
	}

	public PDAttrValuePair ToPDAttrValuePair()
	{
		PDAttrValuePair inst = new PDAttrValuePair();
		inst.id = m_nId;
		inst.value = m_nValue;
		return inst;
	}
}
