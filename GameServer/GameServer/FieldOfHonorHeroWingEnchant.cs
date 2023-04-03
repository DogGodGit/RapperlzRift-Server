using System;
using System.Data;
using ClientCommon;

namespace GameServer;

public class FieldOfHonorHeroWingEnchant
{
	private FieldOfHonorHeroWingPart m_part;

	private int m_nStep;

	private int m_nLevel;

	private int m_nEnchantCount;

	public FieldOfHonorHero hero => m_part.hero;

	public FieldOfHonorHeroWingPart part => m_part;

	public int step
	{
		get
		{
			return m_nStep;
		}
		set
		{
			m_nStep = value;
		}
	}

	public int level
	{
		get
		{
			return m_nLevel;
		}
		set
		{
			m_nLevel = value;
		}
	}

	public int enchantCount
	{
		get
		{
			return m_nEnchantCount;
		}
		set
		{
			m_nEnchantCount = value;
		}
	}

	public int attrValue => Resource.instance.GetWingStep(m_nStep).GetPart(m_part.id).increaseAttrValue.value * m_nEnchantCount;

	public FieldOfHonorHeroWingEnchant(FieldOfHonorHeroWingPart part)
	{
		m_part = part;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nStep = Convert.ToInt32(dr["step"]);
		m_nLevel = Convert.ToInt32(dr["level"]);
		m_nEnchantCount = Convert.ToInt32(dr["enchantCount"]);
	}

	public PDHeroWingEnchant ToPDHeroWingEnchant()
	{
		PDHeroWingEnchant inst = new PDHeroWingEnchant();
		inst.step = m_nStep;
		inst.level = m_nLevel;
		inst.enchantCount = m_nEnchantCount;
		return inst;
	}
}
