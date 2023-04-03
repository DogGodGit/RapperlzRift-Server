using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroWingEnchant
{
	private HeroWingPart m_part;

	private WingStep m_step;

	private WingStepPart m_stepPart;

	private WingStepLevel m_stepLevel;

	private int m_nEnchantCount;

	public HeroWingPart part => m_part;

	public WingStep step => m_step;

	public WingStepPart stepPart => m_stepPart;

	public WingStepLevel stepLevel => m_stepLevel;

	public int enchantCount => m_nEnchantCount;

	public int attrValue => m_stepPart.increaseAttrValue.value * m_nEnchantCount;

	public HeroWingEnchant(HeroWingPart part)
		: this(part, null)
	{
	}

	public HeroWingEnchant(HeroWingPart part, WingStepLevel stepLevel)
	{
		if (part == null)
		{
			throw new ArgumentNullException("part");
		}
		m_part = part;
		if (stepLevel != null)
		{
			m_step = stepLevel.step;
			m_stepLevel = stepLevel;
			m_stepPart = m_step.GetPart(part.part.id);
		}
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nStep = Convert.ToInt32(dr["step"]);
		m_step = Resource.instance.GetWingStep(nStep);
		if (m_step == null)
		{
			throw new Exception("존재하지 않는 날개단계 입니다. nStep = " + nStep);
		}
		int nLevel = Convert.ToInt32(dr["level"]);
		m_stepLevel = m_step.GetLevel(nLevel);
		if (m_stepLevel == null)
		{
			throw new Exception("존재하지 않는 날개단계레벨입니다. nStep = " + nStep + ", nLevel = " + nLevel);
		}
		int nPartId = m_part.part.id;
		m_stepPart = m_step.GetPart(nPartId);
		if (m_stepPart == null)
		{
			throw new Exception("존재하지 않는 날개단계파트입니다. nStep = " + nStep + ", nPartId = " + nPartId);
		}
		m_nEnchantCount = Convert.ToInt32(dr["enchantCount"]);
	}

	public void AddEnchantCount(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nEnchantCount += nAmount;
			m_part.totalEnchantCount += nAmount;
		}
	}

	public PDHeroWingEnchant ToPDHeroWingEnchant()
	{
		PDHeroWingEnchant inst = new PDHeroWingEnchant();
		inst.step = m_step.step;
		inst.level = m_stepLevel.level;
		inst.enchantCount = m_nEnchantCount;
		return inst;
	}

	public FieldOfHonorHeroWingEnchant ToFieldOfHonorHeroWingEnchant(FieldOfHonorHeroWingPart fieldOfHonorHeroWingPart)
	{
		FieldOfHonorHeroWingEnchant inst = new FieldOfHonorHeroWingEnchant(fieldOfHonorHeroWingPart);
		inst.step = m_step.step;
		inst.level = m_stepLevel.level;
		inst.enchantCount = m_nEnchantCount;
		return inst;
	}

	public static List<PDHeroWingEnchant> ToPDHeroWingEnchants(IEnumerable<HeroWingEnchant> enchants)
	{
		List<PDHeroWingEnchant> insts = new List<PDHeroWingEnchant>();
		foreach (HeroWingEnchant enchant in enchants)
		{
			insts.Add(enchant.ToPDHeroWingEnchant());
		}
		return insts;
	}
}
