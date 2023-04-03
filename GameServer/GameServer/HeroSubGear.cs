using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroSubGear : IInventoryObject, IWarehouseObject
{
	private Hero m_hero;

	private SubGear m_subGear;

	private SubGearLevel m_level;

	private int m_nQuality;

	private bool m_bEquipped;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private long m_lnBattlePower;

	private InventorySlot m_inventorySlot;

	private WarehouseSlot m_warehouseSlot;

	private HeroSoulstoneSocket[] m_soulstoneSockets;

	private HeroRuneSocket[] m_runeSockets;

	private Dictionary<int, AttrValuePair> m_attrTotalValues = new Dictionary<int, AttrValuePair>();

	public Hero hero => m_hero;

	public SubGear subGear => m_subGear;

	public int subGearId => m_subGear.id;

	public SubGearLevel subGearLevel
	{
		get
		{
			return m_level;
		}
		set
		{
			m_level = value;
		}
	}

	public int level => m_level.level;

	public int grade => m_level.grade;

	public int quality
	{
		get
		{
			return m_nQuality;
		}
		set
		{
			m_nQuality = value;
		}
	}

	public bool equipped
	{
		get
		{
			return m_bEquipped;
		}
		set
		{
			m_bEquipped = value;
		}
	}

	public DateTimeOffset regTime => m_regTime;

	public long battlePower => m_lnBattlePower;

	public int inventoryObjectType => 2;

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

	public bool saleable => false;

	public HeroSoulstoneSocket[] soulstoneSockets => m_soulstoneSockets;

	public int totalMountedSoulstoneLevel
	{
		get
		{
			int nTotalMountedSoulstoneLevel = 0;
			HeroSoulstoneSocket[] array = m_soulstoneSockets;
			foreach (HeroSoulstoneSocket socket in array)
			{
				if (!socket.isEmpty)
				{
					nTotalMountedSoulstoneLevel += socket.item.level;
				}
			}
			return nTotalMountedSoulstoneLevel;
		}
	}

	public HeroRuneSocket[] runeSockets => m_runeSockets;

	public Dictionary<int, AttrValuePair> attrTotalValues => m_attrTotalValues;

	public bool isLastLevel => m_level.level >= m_subGear.lastLevel.level;

	public bool isLastLevelOfCurrentGrade => m_level.level >= m_subGear.GetMaxLevelOfGrade(m_level.grade);

	public bool isLastQualityOfCurrentLevel => m_nQuality >= m_level.lastQuality.quality;

	public int warehouseObjectType => 2;

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

	public HeroSubGear()
	{
	}

	public HeroSubGear(Hero hero)
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
		int nSubGearId = Convert.ToInt32(dr["subGearId"]);
		m_subGear = Resource.instance.GetSubGear(nSubGearId);
		if (m_subGear == null)
		{
			throw new Exception("존재하지 않는 보조장비 입니다. nSubGearId = " + nSubGearId);
		}
		int nLevel = Convert.ToInt32(dr["level"]);
		m_level = m_subGear.GetLevel(nLevel);
		if (m_level == null)
		{
			throw new Exception("존재하지 않는 보조장비 레벨입니다. nSubGearId = " + m_subGear.id + ", nLevel = " + nLevel);
		}
		m_nQuality = Convert.ToInt32(dr["quality"]);
		m_bEquipped = Convert.ToBoolean(dr["equipped"]);
		m_regTime = (DateTimeOffset)dr["regTime"];
		InitSoulstoneSocket();
		InitRuneSocket();
		RefreshAttrTotalValues();
	}

	public void Init(SubGear subGear, int nLevel, DateTimeOffset regTime)
	{
		if (subGear == null)
		{
			throw new ArgumentNullException("subGear");
		}
		m_subGear = subGear;
		m_level = m_subGear.GetLevel(nLevel);
		m_nQuality = m_level.firstQuality.quality;
		m_bEquipped = false;
		m_regTime = regTime;
		InitSoulstoneSocket();
		InitRuneSocket();
		RefreshAttrTotalValues();
	}

	public void InitSoulstoneSocket()
	{
		m_soulstoneSockets = new HeroSoulstoneSocket[m_subGear.soulstoneSocketCount];
		foreach (SubGearSoulstoneSocket soulstoneSocket in m_subGear.soulstoneSockets)
		{
			m_soulstoneSockets[soulstoneSocket.index] = new HeroSoulstoneSocket(this, soulstoneSocket);
		}
	}

	public HeroSoulstoneSocket GetSoulstoneSocket(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_soulstoneSockets.Length)
		{
			return null;
		}
		return m_soulstoneSockets[nIndex];
	}

	public void InitRuneSocket()
	{
		m_runeSockets = new HeroRuneSocket[m_subGear.runeSocketCount];
		foreach (SubGearRuneSocket runeSocket in m_subGear.runeSockets)
		{
			m_runeSockets[runeSocket.index] = new HeroRuneSocket(this, runeSocket);
		}
	}

	public HeroRuneSocket GetRuneSocket(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_runeSockets.Length)
		{
			return null;
		}
		return m_runeSockets[nIndex];
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
		SubGearLevelQuality quality = m_level.GetQuality(m_nQuality);
		foreach (SubGearAttrValue levelAttr in quality.attrValues)
		{
			AddAttrTotalValue(levelAttr.id, levelAttr.value);
		}
		HeroSoulstoneSocket[] array = m_soulstoneSockets;
		foreach (HeroSoulstoneSocket socket in array)
		{
			if (!socket.isEmpty)
			{
				Item socketItem = socket.item;
				int nAttrId = socketItem.value1;
				AttrValue attrValue = Resource.instance.GetAttrValue(socketItem.longValue1);
				AddAttrTotalValue(nAttrId, attrValue.value);
			}
		}
		HeroRuneSocket[] array2 = m_runeSockets;
		foreach (HeroRuneSocket socket2 in array2)
		{
			if (!socket2.isEmpty)
			{
				Item socketItem2 = socket2.item;
				int nAttrId2 = socketItem2.value1;
				AttrValue attrValue2 = Resource.instance.GetAttrValue(socketItem2.longValue1);
				AddAttrTotalValue(nAttrId2, attrValue2.value);
			}
		}
	}

	private void RefreshBattlePower()
	{
		m_lnBattlePower = Util.CalcBattlePower(m_attrTotalValues.Values);
	}

	public void SetHeroSubGearByFieldOfHonorHeroEquippedSubGear(FieldOfHonorHeroEquippedSubGear fieldOfHonorHeroEquippedSubGear)
	{
		if (fieldOfHonorHeroEquippedSubGear == null)
		{
			throw new ArgumentNullException("fieldOfHonorHeroEquippedSubGear");
		}
		m_subGear = fieldOfHonorHeroEquippedSubGear.subGear;
		m_level = m_subGear.GetLevel(fieldOfHonorHeroEquippedSubGear.level);
		m_nQuality = fieldOfHonorHeroEquippedSubGear.quality;
		InitRuneSocket();
		foreach (FieldOfHonorHeroSubGearRuneSocket fieldOfHonorHeroSubGearRuneSocket in fieldOfHonorHeroEquippedSubGear.runeSockets.Values)
		{
			HeroRuneSocket heroRuneSocket = GetRuneSocket(fieldOfHonorHeroSubGearRuneSocket.index);
			heroRuneSocket.Mount(fieldOfHonorHeroSubGearRuneSocket.item);
		}
		InitSoulstoneSocket();
		foreach (FieldOfHonorHeroSubGearSoulstoneSocket fieldOfHonorHeroSubGearSoulstone in fieldOfHonorHeroEquippedSubGear.soulstoneSockets.Values)
		{
			HeroSoulstoneSocket heroSoulstoneSocket = GetSoulstoneSocket(fieldOfHonorHeroSubGearSoulstone.index);
			heroSoulstoneSocket.Mount(fieldOfHonorHeroSubGearSoulstone.item);
		}
	}

	public PDInventoryObject ToPDInventoryObject()
	{
		PDSubGearInventoryObject inst = new PDSubGearInventoryObject();
		inst.subGearId = m_subGear.id;
		return inst;
	}

	public PDWarehouseObject ToPDWarehouseObject()
	{
		PDSubGearWarehouseObject inst = new PDSubGearWarehouseObject();
		inst.subGearId = m_subGear.id;
		return inst;
	}

	public PDFullHeroSubGear ToPDFullHeroSubGear()
	{
		PDFullHeroSubGear inst = new PDFullHeroSubGear();
		inst.subGearId = m_subGear.id;
		inst.level = m_level.level;
		inst.quality = m_nQuality;
		inst.equipped = m_bEquipped;
		inst.equippedSoulstoneSockets = HeroSoulstoneSocket.ToPDHeroSubGearSoulstoneSockets(m_soulstoneSockets).ToArray();
		inst.equippedRuneSockets = HeroRuneSocket.ToPDHeroSubGearRuneSockets(m_runeSockets).ToArray();
		return inst;
	}

	public FieldOfHonorHeroEquippedSubGear ToFieldOfHonorHeroEquippedSubGear(FieldOfHonorHero fieldOfHonorHero)
	{
		FieldOfHonorHeroEquippedSubGear inst = new FieldOfHonorHeroEquippedSubGear(fieldOfHonorHero);
		inst.subGear = m_subGear;
		inst.level = m_level.level;
		inst.quality = m_nQuality;
		HeroRuneSocket[] array = m_runeSockets;
		foreach (HeroRuneSocket runeSocket in array)
		{
			if (runeSocket.item != null)
			{
				inst.AddRuneSocket(runeSocket.ToFieldOfHonorHeroSubGearRuneSocket(inst));
			}
		}
		HeroSoulstoneSocket[] array2 = m_soulstoneSockets;
		foreach (HeroSoulstoneSocket soulstoneSocket in array2)
		{
			if (soulstoneSocket.item != null)
			{
				inst.AddSoulstoneSocket(soulstoneSocket.ToFieldOfHonorHeroSubGearSoulstoneSocket(inst));
			}
		}
		return inst;
	}

	public static List<PDFullHeroSubGear> ToPDFullHeroSubGears(IEnumerable<HeroSubGear> gears)
	{
		List<PDFullHeroSubGear> results = new List<PDFullHeroSubGear>();
		foreach (HeroSubGear gear in gears)
		{
			results.Add(gear.ToPDFullHeroSubGear());
		}
		return results;
	}
}
