using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroMountGear : IInventoryObject, IWarehouseObject
{
	private Hero m_hero;

	private Guid m_id = Guid.Empty;

	private MountGear m_gear;

	private bool m_bOwned;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private long m_lnBattlePower;

	private InventorySlot m_inventorySlot;

	private WarehouseSlot m_warehouseSlot;

	private HeroMountGearSlot m_heroMountgearSlot;

	private List<HeroMountGearOptionAttr> m_optionAttrs = new List<HeroMountGearOptionAttr>();

	public Dictionary<int, AttrValuePair> m_attrTotalValues = new Dictionary<int, AttrValuePair>();

	public Hero hero => m_hero;

	public Guid id => m_id;

	public MountGear gear => m_gear;

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

	public int inventoryObjectType => 4;

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

	public HeroMountGearSlot gearSlot
	{
		get
		{
			return m_heroMountgearSlot;
		}
		set
		{
			m_heroMountgearSlot = value;
		}
	}

	public bool isEquipped => m_heroMountgearSlot != null;

	public List<HeroMountGearOptionAttr> optionAttrs => m_optionAttrs;

	public Dictionary<int, AttrValuePair> attrTotalValues => m_attrTotalValues;

	public int warehouseObjectType => 4;

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

	public HeroMountGear()
	{
	}

	public HeroMountGear(Hero hero)
	{
		m_hero = hero;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["heroMountGearId"];
		int nMountGearId = Convert.ToInt32(dr["mountGearId"]);
		m_gear = Resource.instance.GetMountGear(nMountGearId);
		if (m_gear == null)
		{
			throw new Exception("존재하지 않는 탈것장비입니다.");
		}
		m_bOwned = Convert.ToBoolean(dr["owned"]);
		m_regTime = (DateTimeOffset)dr["regTime"];
	}

	public void Init(MountGear gear, bool bOwned, DateTimeOffset regTime)
	{
		m_id = Guid.NewGuid();
		m_gear = gear;
		m_bOwned = bOwned;
		m_regTime = regTime;
		CreateOptionAttrs();
		RefreshAttrTotalValues();
	}

	public void AddOptionAttr(HeroMountGearOptionAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_optionAttrs.Add(attr);
		attr.gear = this;
	}

	public HeroMountGearOptionAttr GetOptionAttr(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_optionAttrs.Count)
		{
			return null;
		}
		return m_optionAttrs[nIndex];
	}

	public void CreateOptionAttrs()
	{
		CreateOptionAttrs(Resource.instance.mountGearOptionAttrCount);
	}

	public void CreateOptionAttrs(int nCount)
	{
		List<MountGearOptionAttrPoolEntry> entries = m_gear.SelectOptionAttrPoolEntries(nCount);
		foreach (MountGearOptionAttrPoolEntry entry in entries)
		{
			int nIndex = m_optionAttrs.Count;
			HeroMountGearOptionAttr attr = new HeroMountGearOptionAttr(this, nIndex, entry.attrGrade, entry.attrId, entry.attrValue);
			m_optionAttrs.Add(attr);
		}
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
		AddAttrTotalValue(1, m_gear.maxHp);
		AddAttrTotalValue(2, m_gear.physicalOffense);
		AddAttrTotalValue(3, m_gear.magicalOffense);
		AddAttrTotalValue(4, m_gear.physicalDefense);
		AddAttrTotalValue(5, m_gear.magicalDefense);
		foreach (HeroMountGearOptionAttr attr in m_optionAttrs)
		{
			AddAttrTotalValue(attr.attrId, attr.attrValue.value);
		}
	}

	private void RefreshBattlePower()
	{
		m_lnBattlePower = Util.CalcBattlePower(m_attrTotalValues.Values);
	}

	public PDInventoryObject ToPDInventoryObject()
	{
		PDMountGearInventoryObject inst = new PDMountGearInventoryObject();
		inst.heroMountGearId = (Guid)m_id;
		return inst;
	}

	public PDWarehouseObject ToPDWarehouseObject()
	{
		PDMountGearWarehouseObject inst = new PDMountGearWarehouseObject();
		inst.heroMountGearId = (Guid)m_id;
		return inst;
	}

	public PDHeroMountGear ToPDHeroMountGear()
	{
		PDHeroMountGear inst = new PDHeroMountGear();
		inst.id = (Guid)m_id;
		inst.mountGearId = m_gear.id;
		inst.owned = m_bOwned;
		inst.optionAttrs = HeroMountGearOptionAttr.ToPDHeroMountGearOptionAttrs(m_optionAttrs).ToArray();
		return inst;
	}

	public static List<PDHeroMountGear> ToPDHeroMountGears(IEnumerable<HeroMountGear> mountGears)
	{
		List<PDHeroMountGear> insts = new List<PDHeroMountGear>();
		foreach (HeroMountGear mountGear in mountGears)
		{
			insts.Add(mountGear.ToPDHeroMountGear());
		}
		return insts;
	}
}
