using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountGear
{
	private int m_nId;

	private int m_nRequiredHeroLevel;

	private MountGearType m_type;

	private MountGearGrade m_grade;

	private MountGearQuality m_quality;

	private string m_sNameKey;

	private int m_nSaleGold;

	private AttrValue m_maxHpAttrValue;

	private AttrValue m_physicalOffenseAttrValue;

	private AttrValue m_magicalOffenseAttrValue;

	private AttrValue m_physicalDefenseAttrValue;

	private AttrValue m_magicalDefenseAttrValue;

	private List<MountGearOptionAttrPoolEntry> m_optionAttrPoolEntries = new List<MountGearOptionAttrPoolEntry>();

	private int m_nOptionAttrPoolEntryTotalPoint;

	public int id => m_nId;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public MountGearType type => m_type;

	public MountGearGrade grade => m_grade;

	public MountGearQuality quality => m_quality;

	public string nameKey => m_sNameKey;

	public int saleGold => m_nSaleGold;

	public AttrValue maxHpAttrValue => m_maxHpAttrValue;

	public int maxHp
	{
		get
		{
			if (m_maxHpAttrValue == null)
			{
				return 0;
			}
			return m_maxHpAttrValue.value;
		}
	}

	public AttrValue physicalOffenseAttrValue => m_physicalOffenseAttrValue;

	public int physicalOffense
	{
		get
		{
			if (m_physicalOffenseAttrValue == null)
			{
				return 0;
			}
			return m_physicalOffenseAttrValue.value;
		}
	}

	public AttrValue magicalOffenseAttrValue => m_magicalOffenseAttrValue;

	public int magicalOffense
	{
		get
		{
			if (m_magicalOffenseAttrValue == null)
			{
				return 0;
			}
			return m_magicalOffenseAttrValue.value;
		}
	}

	public AttrValue physicalDefenseAttrValue => m_physicalDefenseAttrValue;

	public int physicalDefense
	{
		get
		{
			if (m_physicalDefenseAttrValue == null)
			{
				return 0;
			}
			return m_physicalDefenseAttrValue.value;
		}
	}

	public AttrValue magicalDefenseAttrValue => m_magicalDefenseAttrValue;

	public int magicalDefense
	{
		get
		{
			if (m_magicalDefenseAttrValue == null)
			{
				return 0;
			}
			return m_magicalDefenseAttrValue.value;
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["mountGearId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "탈것장비 ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "요구영웅레벨가 유효하지 않습니다. m_nId = " + m_nId + ", m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		int nType = Convert.ToInt32(dr["type"]);
		m_type = Resource.instance.GetMountGearType(nType);
		if (m_type == null)
		{
			SFLogUtil.Warn(GetType(), "타입이 존재하지 않습니다. m_nId = " + m_nId + ", nType = " + nType);
		}
		int nGrade = Convert.ToInt32(dr["grade"]);
		m_grade = Resource.instance.GetMountGearGrade(nGrade);
		if (m_grade == null)
		{
			SFLogUtil.Warn(GetType(), "등급이 존재하지 않습니다. m_nId = " + m_nId + ", nGrade = " + nGrade);
		}
		int nQuality = Convert.ToInt32(dr["quality"]);
		m_quality = Resource.instance.GetMountGearQuality(nQuality);
		if (m_quality == null)
		{
			SFLogUtil.Warn(GetType(), "품질이 존재하지 않습니다. m_nId = " + m_nId + ", nQuality = " + nQuality);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_nSaleGold = Convert.ToInt32(dr["saleGold"]);
		if (m_nSaleGold < 0)
		{
			SFLogUtil.Warn(GetType(), "판매가격이 유효하지 않습니다.  m_nId = " + m_nId + ", m_nSaleGold = " + m_nSaleGold);
		}
		long lnMaxHpAttrValueId = Convert.ToInt64(dr["maxHpAttrValueId"]);
		if (lnMaxHpAttrValueId > 0)
		{
			m_maxHpAttrValue = Resource.instance.GetAttrValue(lnMaxHpAttrValueId);
			if (m_maxHpAttrValue == null)
			{
				SFLogUtil.Warn(GetType(), "최대체력속성값이 존재하지 않습니다. m_nId = " + m_nId + ", lnMaxHpAttrValueId = " + lnMaxHpAttrValueId);
			}
		}
		long lnPhysicalOffenseAttrValueId = Convert.ToInt64(dr["physicalOffenseAttrValueId"]);
		if (lnPhysicalOffenseAttrValueId > 0)
		{
			m_physicalOffenseAttrValue = Resource.instance.GetAttrValue(lnPhysicalOffenseAttrValueId);
			if (m_physicalOffenseAttrValue == null)
			{
				SFLogUtil.Warn(GetType(), "물리공격력속성값이 존재하지 않습니다. m_nId = " + m_nId + ", lnPhysicalOffenseAttrValueId = " + lnPhysicalOffenseAttrValueId);
			}
		}
		long lnMagicalOffenseAttrValueId = Convert.ToInt64(dr["magicalOffenseAttrValueId"]);
		if (lnMagicalOffenseAttrValueId > 0)
		{
			m_magicalOffenseAttrValue = Resource.instance.GetAttrValue(lnMagicalOffenseAttrValueId);
			if (m_magicalOffenseAttrValue == null)
			{
				SFLogUtil.Warn(GetType(), "마법공격력이 존재하지 않습니다. m_nId = " + m_nId + ", lnMagicalOffenseAttrValueId = " + lnMagicalOffenseAttrValueId);
			}
		}
		long lnPhysicalDefenseAttrValueId = Convert.ToInt64(dr["physicalDefenseAttrValueId"]);
		if (lnPhysicalDefenseAttrValueId > 0)
		{
			m_physicalDefenseAttrValue = Resource.instance.GetAttrValue(lnPhysicalDefenseAttrValueId);
			if (m_physicalDefenseAttrValue == null)
			{
				SFLogUtil.Warn(GetType(), "물리방어력이 존재하지 않습니다. m_nId = " + m_nId + ", lnPhysicalDefenseAttrValueId = " + lnPhysicalDefenseAttrValueId);
			}
		}
		long lnMagicalDefenseAttrValueId = Convert.ToInt64(dr["magicalDefenseAttrValueId"]);
		if (lnMagicalDefenseAttrValueId > 0)
		{
			m_magicalDefenseAttrValue = Resource.instance.GetAttrValue(lnMagicalDefenseAttrValueId);
			if (m_magicalDefenseAttrValue == null)
			{
				SFLogUtil.Warn(GetType(), "마법방어력이 존재하지 않습니다. m_nId = " + m_nId + ", lnMagicalDefenseAttrValueId = " + lnMagicalDefenseAttrValueId);
			}
		}
	}

	public void AddOptionAttrPoolEntry(MountGearOptionAttrPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_optionAttrPoolEntries.Add(entry);
		entry.gear = this;
		m_nOptionAttrPoolEntryTotalPoint += entry.point;
	}

	public MountGearOptionAttrPoolEntry SelectOptionAttrPoolEntry()
	{
		return Util.SelectPickEntry(m_optionAttrPoolEntries, m_nOptionAttrPoolEntryTotalPoint);
	}

	public List<MountGearOptionAttrPoolEntry> SelectOptionAttrPoolEntries(int nCount)
	{
		return Util.SelectPickEntries(m_optionAttrPoolEntries, m_nOptionAttrPoolEntryTotalPoint, nCount, bDuplicated: true);
	}
}
