using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroMainGear : IInventoryObject, IWarehouseObject
{
	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private MainGear m_mainGear;

	private int m_nEnchantLevel;

	private bool m_bOwned;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private long m_lnBattlePower;

	private InventorySlot m_inventorySlot;

	private WarehouseSlot m_warehouseSlot;

	private List<HeroMainGearOptionAttr> m_optionAttrs = new List<HeroMainGearOptionAttr>();

	private List<HeroMainGearRefinement> m_refinements = new List<HeroMainGearRefinement>();

	private Dictionary<int, AttrValuePair> m_attrTotalValues = new Dictionary<int, AttrValuePair>();

	public Guid id => m_id;

	public Hero hero => m_hero;

	public MainGear gear => m_mainGear;

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

	public bool isMaxEnchantLevel => m_nEnchantLevel >= Resource.instance.maxMainGearEnchantLevel.enchantLevel;

	public bool owned
	{
		get
		{
			return m_bOwned;
		}
		set
		{
			m_bOwned = value;
		}
	}

	public DateTimeOffset regTime => m_regTime;

	public long battlePower => m_lnBattlePower;

	public int inventoryObjectType => 1;

	public InventorySlot inventorySlot
	{
		get
		{
			return m_inventorySlot;
		}
		set
		{
			m_inventorySlot = value;
		}
	}

	public bool saleable => true;

	public Dictionary<int, AttrValuePair> attrTotalValues => m_attrTotalValues;

	public List<HeroMainGearOptionAttr> optionAttrs => m_optionAttrs;

	public int optionAttrCount => m_optionAttrs.Count;

	public List<HeroMainGearRefinement> refinements => m_refinements;

	public bool isWeapon => m_mainGear.isWeapon;

	public bool isArmor => m_mainGear.isArmor;

	public bool isEquipped => m_id == (m_mainGear.isWeapon ? m_hero.equippedWeaponId : m_hero.equippedArmorId);

	public bool equipable
	{
		get
		{
			if (m_mainGear.jobId <= 0)
			{
				return true;
			}
			return m_mainGear.jobId == m_hero.baseJobId;
		}
	}

	public int warehouseObjectType => 1;

	public WarehouseSlot warehouseSlot
	{
		get
		{
			return m_warehouseSlot;
		}
		set
		{
			m_warehouseSlot = value;
		}
	}

	public HeroMainGear()
	{
	}

	public HeroMainGear(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["heroMainGearId"];
		int nMainGearId = Convert.ToInt32(dr["mainGearId"]);
		m_mainGear = Resource.instance.GetMainGear(nMainGearId);
		if (m_mainGear == null)
		{
			throw new Exception("존재하지 않는 메인장비 입니다. nMainGearId = " + nMainGearId);
		}
		m_nEnchantLevel = Convert.ToInt32(dr["enchantLevel"]);
		m_bOwned = Convert.ToBoolean(dr["owned"]);
		m_regTime = (DateTimeOffset)dr["regTime"];
	}

	public void Init(MainGear mainGear, int nEnchantLevel, bool bOwned, DateTimeOffset regTime)
	{
		if (mainGear == null)
		{
			throw new ArgumentNullException("mainGear");
		}
		m_id = Guid.NewGuid();
		m_mainGear = mainGear;
		m_nEnchantLevel = nEnchantLevel;
		m_bOwned = bOwned;
		m_regTime = regTime;
		CreateOptionAttrs();
		RefreshAttrTotalValues();
	}

	public void Unequip()
	{
		if (m_mainGear.isWeapon)
		{
			m_hero.equippedWeapon = null;
		}
		else
		{
			m_hero.equippedArmor = null;
		}
	}

	public void AddOptinAttr(HeroMainGearOptionAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		if (attr.heroMainGear != null)
		{
			throw new Exception("이미 영웅메인장비에 추가된 영웅메인장비옵션속성입니다.");
		}
		m_optionAttrs.Add(attr);
		attr.heroMainGear = this;
	}

	public HeroMainGearOptionAttr GetOptionAttr(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_optionAttrs.Count)
		{
			return null;
		}
		return m_optionAttrs[nIndex];
	}

	public void ClearOptionAttrs()
	{
		m_optionAttrs.Clear();
	}

	public void SetOptionAttr(int nIndex, int nGrade, int nAttrId, AttrValue attrValue)
	{
		HeroMainGearOptionAttr attr = GetOptionAttr(nIndex);
		if (attr == null)
		{
			throw new ArgumentOutOfRangeException("nIndex");
		}
		attr.SetAttrValue(nGrade, nAttrId, attrValue);
	}

	private void CreateOptionAttrs()
	{
		int nMinCount = Resource.instance.mainGearOptionAttrMinCount;
		int nMaxCount = Resource.instance.mainGearOptionAttrMaxCount;
		int nCount = SFRandom.Next(nMinCount, nMaxCount + 1);
		CreateOptionAttrs(nCount);
	}

	private void CreateOptionAttrs(int nOptionCount)
	{
		MainGearOptionAttrPool pool = m_mainGear.tier.GetOptionAttrPool(m_mainGear.grade.id);
		List<MainGearOptionAttrPoolEntry> entries = pool.SelectEntries(nOptionCount);
		foreach (MainGearOptionAttrPoolEntry entry in entries)
		{
			int nIndex = m_optionAttrs.Count;
			HeroMainGearOptionAttr attr = new HeroMainGearOptionAttr(this, nIndex, entry.attrGrade, entry.attrId, entry.attrValue);
			m_optionAttrs.Add(attr);
		}
	}

	public HeroMainGearRefinement GetRefinement(int nTurn)
	{
		int nIndex = nTurn - 1;
		if (nIndex < 0 || nIndex >= m_refinements.Count)
		{
			return null;
		}
		return m_refinements[nIndex];
	}

	public void AddRefinement(HeroMainGearRefinement refinement)
	{
		if (refinement == null)
		{
			throw new ArgumentNullException("refinement");
		}
		m_refinements.Add(refinement);
	}

	public void ClearRefinement()
	{
		m_refinements.Clear();
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
		foreach (HeroMainGearOptionAttr optionAttr in m_optionAttrs)
		{
			AddAttrTotalValue(optionAttr.attrId, optionAttr.attrValue.value);
		}
	}

	private void RefreshBattlePower()
	{
		m_lnBattlePower = Util.CalcBattlePower(m_attrTotalValues.Values);
	}

	public void SetHeroMainGearByFieldOfHonorHeroEquippedMainGear(FieldOfHonorHeroEquippedMainGear fieldOfHonorHeroEquippedMainGear)
	{
		if (fieldOfHonorHeroEquippedMainGear == null)
		{
			throw new ArgumentNullException("fieldOfHonorHeroEquippedMainGear");
		}
		m_id = fieldOfHonorHeroEquippedMainGear.heroMainGearId;
		m_mainGear = fieldOfHonorHeroEquippedMainGear.mainGear;
		m_nEnchantLevel = fieldOfHonorHeroEquippedMainGear.enchantLevel;
		foreach (FieldOfHonorHeroMainGearOptionAttr fieldOfHonorHeroMainGearOptionAttr in fieldOfHonorHeroEquippedMainGear.optionAttrs)
		{
			HeroMainGearOptionAttr optionAttr = new HeroMainGearOptionAttr(this, fieldOfHonorHeroMainGearOptionAttr.index, fieldOfHonorHeroMainGearOptionAttr.grade, fieldOfHonorHeroMainGearOptionAttr.attrId, fieldOfHonorHeroMainGearOptionAttr.attrValue);
			m_optionAttrs.Add(optionAttr);
		}
	}

	public PDInventoryObject ToPDInventoryObject()
	{
		PDMainGearInventoryObject inst = new PDMainGearInventoryObject();
		inst.heroMainGearId = (Guid)m_id;
		return inst;
	}

	public PDWarehouseObject ToPDWarehouseObject()
	{
		PDMainGearWarehouseObject inst = new PDMainGearWarehouseObject();
		inst.heroMainGearId = (Guid)m_id;
		return inst;
	}

	public PDFullHeroMainGear ToPDFullHeroMainGear(bool bIncludeRefinements)
	{
		PDFullHeroMainGear inst = new PDFullHeroMainGear();
		inst.id = (Guid)m_id;
		inst.mainGearId = m_mainGear.id;
		inst.enchantLevel = m_nEnchantLevel;
		inst.owned = m_bOwned;
		inst.optionAttrs = HeroMainGearOptionAttr.ToPDHeroMainGearOptionAttrs(m_optionAttrs).ToArray();
		if (bIncludeRefinements)
		{
			inst.refinements = HeroMainGearRefinement.ToPDHeroMainGearRefinements(m_refinements).ToArray();
		}
		else
		{
			inst.refinements = new PDHeroMainGearRefinement[0];
		}
		return inst;
	}

	public PDHeroMainGear ToPDHeroMainGear()
	{
		PDHeroMainGear inst = new PDHeroMainGear();
		inst.id = (Guid)m_id;
		inst.mainGearId = m_mainGear.id;
		inst.enchantLevel = m_nEnchantLevel;
		return inst;
	}

	public FieldOfHonorHeroEquippedMainGear ToFieldOfHonorHeroEquippedMainGear(FieldOfHonorHero fieldOfHonorHero)
	{
		FieldOfHonorHeroEquippedMainGear inst = new FieldOfHonorHeroEquippedMainGear(fieldOfHonorHero);
		inst.heroMainGearId = m_id;
		inst.mainGear = m_mainGear;
		inst.enchantLevel = m_nEnchantLevel;
		foreach (HeroMainGearOptionAttr optionAttr in m_optionAttrs)
		{
			inst.AddOptionAttr(optionAttr.ToFieldOfHonorHeroMainGearOptionAttr(inst));
		}
		return inst;
	}

	public static List<PDFullHeroMainGear> ToPDFullHeroMainGears(IEnumerable<HeroMainGear> gears)
	{
		List<PDFullHeroMainGear> results = new List<PDFullHeroMainGear>();
		foreach (HeroMainGear gear in gears)
		{
			results.Add(gear.ToPDFullHeroMainGear(bIncludeRefinements: true));
		}
		return results;
	}
}
