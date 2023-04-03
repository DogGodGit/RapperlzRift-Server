using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class FieldOfHonorHeroWingPart
{
	private FieldOfHonorHero m_hero;

	private int m_nId;

	private List<FieldOfHonorHeroWingEnchant> m_enchants = new List<FieldOfHonorHeroWingEnchant>();

	private Dictionary<int, AttrValuePair> m_attrTotalValues = new Dictionary<int, AttrValuePair>();

	private long m_lnBattlePower;

	public FieldOfHonorHero hero => m_hero;

	public int id => m_nId;

	public List<FieldOfHonorHeroWingEnchant> enchants => m_enchants;

	public Dictionary<int, AttrValuePair> attrTotalValues => m_attrTotalValues;

	public long battlePower => m_lnBattlePower;

	public FieldOfHonorHeroWingPart(FieldOfHonorHero hero, int nId)
	{
		m_hero = hero;
		m_nId = nId;
	}

	public void AddEnchant(FieldOfHonorHeroWingEnchant enchant)
	{
		if (enchant == null)
		{
			throw new ArgumentNullException("enchant");
		}
		m_enchants.Add(enchant);
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
		foreach (FieldOfHonorHeroWingEnchant enchant in m_enchants)
		{
			WingPart part = Resource.instance.GetWingPart(m_nId);
			AddAttrTotalValue(part.attrId, enchant.attrValue);
		}
	}

	public void RefreshBattlePower()
	{
		m_lnBattlePower = Util.CalcBattlePower(m_attrTotalValues.Values);
	}

	public PDHeroWingPart ToPDHeroWingPart()
	{
		PDHeroWingPart inst = new PDHeroWingPart();
		inst.partId = m_nId;
		List<PDHeroWingEnchant> pdEnchants = new List<PDHeroWingEnchant>();
		foreach (FieldOfHonorHeroWingEnchant enchant in m_enchants)
		{
			pdEnchants.Add(enchant.ToPDHeroWingEnchant());
		}
		inst.enchants = pdEnchants.ToArray();
		return inst;
	}
}
