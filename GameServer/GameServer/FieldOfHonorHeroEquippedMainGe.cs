using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FieldOfHonorHeroEquippedMainGear
{
	private Guid m_heroMainGearId = Guid.Empty;

	private FieldOfHonorHero m_hero;

	private MainGear m_mainGear;

	private int m_nEnchantLevel;

	private List<FieldOfHonorHeroMainGearOptionAttr> m_optionAttrs = new List<FieldOfHonorHeroMainGearOptionAttr>();

	private Dictionary<int, AttrValuePair> m_attrTotalValues = new Dictionary<int, AttrValuePair>();

	private long m_lnBattlePower;

	public Guid heroMainGearId
	{
		get
		{
			return m_heroMainGearId;
		}
		set
		{
			m_heroMainGearId = value;
		}
	}

	public FieldOfHonorHero hero => m_hero;

	public MainGear mainGear
	{
		get
		{
			return m_mainGear;
		}
		set
		{
			m_mainGear = value;
		}
	}

	public int enchantLevel
	{
		get
		{
			return m_nEnchantLevel;
		}
		set
		{
			m_nEnchantLevel = value;
		}
	}

	public List<FieldOfHonorHeroMainGearOptionAttr> optionAttrs => m_optionAttrs;

	public Dictionary<int, AttrValuePair> attrTotalValues => m_attrTotalValues;

	public long battlePower => m_lnBattlePower;

	public FieldOfHonorHeroEquippedMainGear(FieldOfHonorHero hero)
	{
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_heroMainGearId = SFDBUtil.ToGuid(dr["heroMainGearId"]);
		int nMainGearId = Convert.ToInt32(dr["mainGearId"]);
		if (nMainGearId > 0)
		{
			m_mainGear = Resource.instance.GetMainGear(nMainGearId);
			if (m_mainGear == null)
			{
				SFLogUtil.Warn(GetType(), string.Concat("메인장비가 존재하지 않습니다. m_heroMainGearId = ", m_heroMainGearId, ", nMainGearId = ", nMainGearId));
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), string.Concat("메인장비ID가 유효하지 않습니다. m_heroMainGearId = ", m_heroMainGearId, ", nMainGearId = ", nMainGearId));
		}
		m_nEnchantLevel = Convert.ToInt32(dr["enchantLevel"]);
		RefreshAttrTotalValues();
	}

	public void AddOptionAttr(FieldOfHonorHeroMainGearOptionAttr optionAttr)
	{
		if (optionAttr == null)
		{
			throw new ArgumentNullException("optionAttr");
		}
		m_optionAttrs.Add(optionAttr);
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
		foreach (MainGearBaseAttr baseAttr in m_mainGear.baseAttrs)
		{
			MainGearBaseAttrEnchantLevel enchantBaseLevel = baseAttr.GetEnchantLevel(m_nEnchantLevel);
			AddAttrTotalValue(baseAttr.id, enchantBaseLevel.value);
		}
		foreach (FieldOfHonorHeroMainGearOptionAttr optionAttr in m_optionAttrs)
		{
			AddAttrTotalValue(optionAttr.attrId, optionAttr.attrValue.value);
		}
	}

	private void RefreshBattlePower()
	{
		m_lnBattlePower = Util.CalcBattlePower(m_attrTotalValues.Values);
	}

	public PDFullHeroMainGear ToPDFullHeroMainGear()
	{
		PDFullHeroMainGear inst = new PDFullHeroMainGear();
		inst.id = (Guid)m_heroMainGearId;
		inst.mainGearId = m_mainGear.id;
		inst.enchantLevel = m_nEnchantLevel;
		inst.owned = true;
		List<PDHeroMainGearOptionAttr> pdOptionAttrs = new List<PDHeroMainGearOptionAttr>();
		foreach (FieldOfHonorHeroMainGearOptionAttr optionAttr in m_optionAttrs)
		{
			pdOptionAttrs.Add(optionAttr.ToPDHeroMainGearOptionAttr());
		}
		inst.optionAttrs = pdOptionAttrs.ToArray();
		inst.refinements = new List<PDHeroMainGearRefinement>().ToArray();
		return inst;
	}
}
