using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroMount
{
	private Hero m_hero;

	private Mount m_mount;

	private int m_nLevel;

	private int m_nSatiety;

	private int m_nAwakeningLevel;

	private int m_nAwakeningExp;

	private int m_nPotionAttrCount;

	private Dictionary<int, AttrValuePair> m_attrTotalValues = new Dictionary<int, AttrValuePair>();

	public Hero hero => m_hero;

	public Mount mount => m_mount;

	public int level => m_nLevel;

	public int satiety => m_nSatiety;

	public int awakeningLevel => m_nAwakeningLevel;

	public int awakeningExp => m_nAwakeningExp;

	public int potionAttrCount
	{
		get
		{
			return m_nPotionAttrCount;
		}
		set
		{
			m_nPotionAttrCount = value;
		}
	}

	public Dictionary<int, AttrValuePair> attrTotalValues => m_attrTotalValues;

	public bool isEquipped => m_mount.id == m_hero.equippedMountId;

	public bool isMaxLevel => m_nLevel >= m_mount.lastMountLevel.levelMaster.level;

	public bool isMaxAwakeningLevel => m_nAwakeningLevel >= m_mount.lastawakeningLevel.levelMaster.level;

	public HeroMount(Hero hero)
		: this(hero, null, 0, 0)
	{
	}

	public HeroMount(Hero hero, Mount mount, int nLevel, int nSatiety)
	{
		m_hero = hero;
		m_mount = mount;
		m_nLevel = nLevel;
		m_nSatiety = nSatiety;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nMountId = Convert.ToInt32(dr["mountId"]);
		m_mount = Resource.instance.GetMount(nMountId);
		if (m_mount == null)
		{
			throw new Exception("존재하지 않는 탈것입니다. nMountId = " + nMountId);
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		m_nSatiety = Convert.ToInt32(dr["satiety"]);
		m_nAwakeningLevel = Convert.ToInt32(dr["awakeningLevel"]);
		m_nAwakeningExp = Convert.ToInt32(dr["awakeningExp"]);
		m_nPotionAttrCount = Convert.ToInt32(dr["PotionAttrCount"]);
	}

	public void AddSatiety(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount == 0)
		{
			return;
		}
		MountLevelMaster lastLevelMaster = m_mount.lastMountLevel.levelMaster;
		if (m_nLevel >= lastLevelMaster.level)
		{
			return;
		}
		m_nSatiety += nAmount;
		do
		{
			MountLevelMaster levelMaster = m_mount.GetLevel(m_nLevel).levelMaster;
			int nNextLevelUpRequiredSatiety = levelMaster.nextLevelUpRequiredSatiety;
			if (m_nSatiety < nNextLevelUpRequiredSatiety)
			{
				break;
			}
			m_nLevel++;
			m_nSatiety -= nNextLevelUpRequiredSatiety;
		}
		while (m_nLevel < lastLevelMaster.level);
		if (m_nLevel >= lastLevelMaster.level)
		{
			m_nSatiety = 0;
		}
	}

	public void AddAwakeningExp(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount == 0)
		{
			return;
		}
		MountAwakeningLevelMaster lastLevelMaster = m_mount.lastawakeningLevel.levelMaster;
		if (m_nAwakeningLevel >= lastLevelMaster.level)
		{
			return;
		}
		m_nAwakeningExp += nAmount;
		do
		{
			MountAwakeningLevel level = m_mount.GetAwakeningLevel(m_nAwakeningLevel);
			int nNextLevelUpAwakeningExp = level.nextLevelUpAwakeningExp;
			if (m_nAwakeningExp < nNextLevelUpAwakeningExp)
			{
				break;
			}
			m_nAwakeningLevel++;
			m_nAwakeningExp -= nNextLevelUpAwakeningExp;
		}
		while (m_nAwakeningLevel < lastLevelMaster.level);
		if (m_nAwakeningLevel >= lastLevelMaster.level)
		{
			m_nAwakeningExp = 0;
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
	}

	private void RefreshAttrTotalValues_Sum()
	{
		MountLevel mountLevel = m_mount.GetLevel(m_nLevel);
		AddAttrTotalValue(1, mountLevel.maxHp);
		AddAttrTotalValue(2, mountLevel.physicalOffense);
		AddAttrTotalValue(3, mountLevel.magicalOffense);
		AddAttrTotalValue(4, mountLevel.phsicalDefense);
		AddAttrTotalValue(5, mountLevel.magicalDefense);
		MountPotionAttrCount potionAttr = Resource.instance.GetMountPotionAttrCount(m_nPotionAttrCount);
		if (potionAttr == null)
		{
			return;
		}
		foreach (MountPotaionAttrValue attrValue in potionAttr.attrValues)
		{
			AddAttrTotalValue(attrValue.attrId, attrValue.attrValue);
		}
	}

	public PDHeroMount ToPDHeroMount()
	{
		PDHeroMount inst = new PDHeroMount();
		inst.mountId = m_mount.id;
		inst.level = m_nLevel;
		inst.satiety = m_nSatiety;
		inst.awakeningLevel = m_nAwakeningLevel;
		inst.awakeningExp = m_nAwakeningExp;
		inst.potionAttrCount = m_nPotionAttrCount;
		return inst;
	}

	public static List<PDHeroMount> ToPDHeroMounts(IEnumerable<HeroMount> mounts)
	{
		List<PDHeroMount> insts = new List<PDHeroMount>();
		foreach (HeroMount mount in mounts)
		{
			insts.Add(mount.ToPDHeroMount());
		}
		return insts;
	}
}
