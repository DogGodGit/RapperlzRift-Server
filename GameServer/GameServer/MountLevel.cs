using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountLevel
{
	private Mount m_mount;

	private MountLevelMaster m_levelMaster;

	private AttrValue m_maxHpAttrValue;

	private AttrValue m_physicalOffenseAttrValue;

	private AttrValue m_magicalOffenseAttrValue;

	private AttrValue m_physicalDefenseAttrValue;

	private AttrValue m_magicalDefenseAttrValue;

	public Mount mount
	{
		get
		{
			return m_mount;
		}
		set
		{
			m_mount = value;
		}
	}

	public MountLevelMaster levelMaster => m_levelMaster;

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

	public int phsicalDefense
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
		int nLevel = Convert.ToInt32(dr["level"]);
		m_levelMaster = Resource.instance.GetMountLevelMaster(nLevel);
		if (m_levelMaster == null)
		{
			SFLogUtil.Warn(GetType(), "레벨마스터가 존재하지 않습니다. nLevel = " + nLevel);
		}
		long lnMaxHpAttrValueId = Convert.ToInt64(dr["maxHpAttrValueId"]);
		if (lnMaxHpAttrValueId > 0)
		{
			m_maxHpAttrValue = Resource.instance.GetAttrValue(lnMaxHpAttrValueId);
			if (m_maxHpAttrValue == null)
			{
				SFLogUtil.Warn(GetType(), "최대체력속성값이 존재하지 않습니다. nLevel = " + nLevel + ", lnMaxHpAttrValueId = " + lnMaxHpAttrValueId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "최대체력속성값ID가 유효하지 않습니다. nLevel = " + nLevel + ", lnMaxHpAttrValueId = " + lnMaxHpAttrValueId);
		}
		long lnPhysicalOffenseAttrValue = Convert.ToInt64(dr["physicalOffenseAttrValueId"]);
		if (lnPhysicalOffenseAttrValue > 0)
		{
			m_physicalOffenseAttrValue = Resource.instance.GetAttrValue(lnPhysicalOffenseAttrValue);
			if (m_maxHpAttrValue == null)
			{
				SFLogUtil.Warn(GetType(), "물리공격력속성값이 존재하지 않습니다. nLevel = " + nLevel + ", lnPhysicalOffenseAttrValue = " + lnPhysicalOffenseAttrValue);
			}
		}
		long lnMagicalOffenseAttrValue = Convert.ToInt64(dr["magicalOffenseAttrValueId"]);
		if (lnMagicalOffenseAttrValue > 0)
		{
			m_magicalOffenseAttrValue = Resource.instance.GetAttrValue(lnPhysicalOffenseAttrValue);
			if (m_maxHpAttrValue == null)
			{
				SFLogUtil.Warn(GetType(), "마법공격력속성값이 존재하지 않습니다. nLevel = " + nLevel + ", lnMagicalOffenseAttrValue = " + lnMagicalOffenseAttrValue);
			}
		}
		long lnPhysicalDefenseAttrValue = Convert.ToInt64(dr["physicalDefenseAttrValueId"]);
		if (lnPhysicalDefenseAttrValue > 0)
		{
			m_physicalDefenseAttrValue = Resource.instance.GetAttrValue(lnPhysicalDefenseAttrValue);
			if (m_maxHpAttrValue == null)
			{
				SFLogUtil.Warn(GetType(), "물리방어력속성값이 존재하지 않습니다. nLevel = " + nLevel + ", lnPhysicalDefenseAttrValue = " + lnPhysicalDefenseAttrValue);
			}
		}
		long lnMagicalDefenseAttrValue = Convert.ToInt64(dr["magicalDefenseAttrValueId"]);
		if (lnMagicalDefenseAttrValue > 0)
		{
			m_magicalDefenseAttrValue = Resource.instance.GetAttrValue(lnMagicalDefenseAttrValue);
			if (m_maxHpAttrValue == null)
			{
				SFLogUtil.Warn(GetType(), "마법방어력속성값이 존재하지 않습니다. nLevel = " + nLevel + ", lnMagicalDefenseAttrValue = " + lnMagicalDefenseAttrValue);
			}
		}
	}
}
