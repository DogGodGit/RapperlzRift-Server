using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class HeroWingPart
{
	private Hero m_hero;

	private WingPart m_part;

	private List<HeroWingEnchant> m_enchants = new List<HeroWingEnchant>();

	private int m_nTotalEnchantCount;

	private Dictionary<int, AttrValuePair> m_attrTotalValues = new Dictionary<int, AttrValuePair>();

	private long m_lnBattlePower;

	public Hero hero => m_hero;

	public WingPart part => m_part;

	public List<HeroWingEnchant> enchants => m_enchants;

	public int attrId => m_part.attrId;

	public int totalEnchantCount
	{
		get
		{
			return m_nTotalEnchantCount;
		}
		set
		{
			m_nTotalEnchantCount = value;
		}
	}

	public Dictionary<int, AttrValuePair> attrTotalValues => m_attrTotalValues;

	public long battlePower => m_lnBattlePower;

	public HeroWingPart(WingPart part)
	{
		m_part = part;
	}

	public HeroWingPart(Hero hero, WingPart part)
	{
		m_hero = hero;
		m_part = part;
	}

	public void AddEnchant(HeroWingEnchant enchant)
	{
		if (enchant == null)
		{
			throw new ArgumentNullException("enchant");
		}
		enchants.Add(enchant);
		m_nTotalEnchantCount += enchant.enchantCount;
	}

	public HeroWingEnchant GetEnchant(int nStep, int nLevel)
	{
		foreach (HeroWingEnchant enchant in enchants)
		{
			if (nStep == enchant.step.step && nLevel == enchant.stepLevel.level)
			{
				return enchant;
			}
		}
		return null;
	}

	public HeroWingEnchant GetOrCreateEnchant(WingStepLevel stepLevel)
	{
		HeroWingEnchant enchant = GetEnchant(stepLevel.step.step, stepLevel.level);
		if (enchant == null)
		{
			enchant = new HeroWingEnchant(this, stepLevel);
			AddEnchant(enchant);
		}
		return enchant;
	}

	public int GetTotalAttrValue()
	{
		int nTotalAttrValue = 0;
		foreach (HeroWingEnchant enchant in enchants)
		{
			nTotalAttrValue += enchant.attrValue;
		}
		return nTotalAttrValue;
	}

	private AttrValuePair GetAttrTotalValue(int nAttrId)
	{
		if (!m_attrTotalValues.TryGetValue(nAttrId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddAttrTotalValue(int nAttrId, int nValue)
	{
		AttrValuePair attrValue = GetAttrTotalValue(nAttrId);
		if (attrValue == null)
		{
			attrValue = new AttrValuePair(nAttrId, 0);
			m_attrTotalValues.Add(nAttrId, attrValue);
		}
		attrValue.value += nValue;
	}

	private void ClearAttrTotalValues()
	{
		m_attrTotalValues.Clear();
	}

	public void RefreshAttrTotalValues()
	{
		ClearAttrTotalValues();
		RefreshAttrTotalValues_Sum();
		RefreshBattlePower();
	}

	private void RefreshAttrTotalValues_Sum()
	{
		foreach (HeroWingEnchant enchant in m_enchants)
		{
			AddAttrTotalValue(m_part.attrId, enchant.attrValue);
		}
	}

	private void RefreshBattlePower()
	{
		m_lnBattlePower = Util.CalcBattlePower(m_attrTotalValues.Values);
	}

	public void SetHeroWingPartByFieldOfHonorHeroWingPart(FieldOfHonorHeroWingPart fieldOfHonorHeroWingPart)
	{
		if (fieldOfHonorHeroWingPart == null)
		{
			throw new ArgumentNullException("fieldOfHonorHeroWingPart");
		}
		foreach (FieldOfHonorHeroWingEnchant fieldOfHonorHeroWingEnchant in fieldOfHonorHeroWingPart.enchants)
		{
			WingStep wingStep = Resource.instance.GetWingStep(fieldOfHonorHeroWingEnchant.step);
			WingStepLevel wingStepLevel = wingStep.GetLevel(fieldOfHonorHeroWingEnchant.level);
			HeroWingEnchant heroWingEnchant = new HeroWingEnchant(this, wingStepLevel);
			heroWingEnchant.AddEnchantCount(fieldOfHonorHeroWingEnchant.enchantCount);
			AddEnchant(heroWingEnchant);
		}
	}

	public PDHeroWingPart ToPDHeroWingPart()
	{
		PDHeroWingPart inst = new PDHeroWingPart();
		inst.partId = m_part.id;
		inst.enchants = HeroWingEnchant.ToPDHeroWingEnchants(enchants).ToArray();
		return inst;
	}

	public FieldOfHonorHeroWingPart ToFieldOfHonorHeroWingPart(FieldOfHonorHero fieldOfHonorHero)
	{
		FieldOfHonorHeroWingPart inst = new FieldOfHonorHeroWingPart(fieldOfHonorHero, m_part.id);
		foreach (HeroWingEnchant enchant in m_enchants)
		{
			inst.AddEnchant(enchant.ToFieldOfHonorHeroWingEnchant(inst));
		}
		return inst;
	}

	public static List<PDHeroWingPart> ToPDHeroWingParts(IEnumerable<HeroWingPart> wingParts)
	{
		List<PDHeroWingPart> insts = new List<PDHeroWingPart>();
		foreach (HeroWingPart wingPart in wingParts)
		{
			insts.Add(wingPart.ToPDHeroWingPart());
		}
		return insts;
	}
}
