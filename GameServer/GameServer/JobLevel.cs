using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class JobLevel
{
	private Job m_job;

	private JobLevelMaster m_master;

	private AttrValue m_maxHpAttrValue;

	private AttrValue m_physicalOffenseAttrValue;

	private AttrValue m_magicalOffenseAttrValue;

	private AttrValue m_physicalDefenseAttrValue;

	private AttrValue m_magicalDefenseAttrValue;

	public Job job
	{
		get
		{
			return m_job;
		}
		set
		{
			m_job = value;
		}
	}

	public JobLevelMaster master => m_master;

	public int level => m_master.level;

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
		int nLevel = Convert.ToInt32(dr["level"]);
		m_master = Resource.instance.GetJobLevelMaster(nLevel);
		if (m_master == null)
		{
			SFLogUtil.Warn(GetType(), "직업레벨마스터가 존재하지 않습니다. nLevel = " + nLevel);
		}
		long lnMaxHpAttrValueId = Convert.ToInt64(dr["maxHpAttrValueId"]);
		m_maxHpAttrValue = Resource.instance.GetAttrValue(lnMaxHpAttrValueId);
		if (m_maxHpAttrValue == null)
		{
			SFLogUtil.Warn(GetType(), "최대체력속성값이 존재하지 않습니다. lnMaxHpAttrValueId = " + lnMaxHpAttrValueId);
		}
		long lnPhysicalOffenseAttrValueId = Convert.ToInt64(dr["physicalOffenseAttrValueId"]);
		m_physicalOffenseAttrValue = Resource.instance.GetAttrValue(lnPhysicalOffenseAttrValueId);
		if (m_physicalOffenseAttrValue == null)
		{
			SFLogUtil.Warn(GetType(), "물리공격력속성값이 존재하지 않습니다. lnPhysicalOffenseAttrValueId = " + lnPhysicalOffenseAttrValueId);
		}
		long lnMagicalOffenseAttrValueId = Convert.ToInt64(dr["magicalOffenseAttrValueId"]);
		m_magicalOffenseAttrValue = Resource.instance.GetAttrValue(lnMagicalOffenseAttrValueId);
		if (m_magicalOffenseAttrValue == null)
		{
			SFLogUtil.Warn(GetType(), "마법공격력속성값이 존재하지 않습니다. lnMagicalOffenseAttrValueId = " + lnMagicalOffenseAttrValueId);
		}
		long lnPhysicalDefenseAttrValueId = Convert.ToInt64(dr["physicalDefenseAttrValueId"]);
		m_physicalDefenseAttrValue = Resource.instance.GetAttrValue(lnPhysicalDefenseAttrValueId);
		if (m_physicalDefenseAttrValue == null)
		{
			SFLogUtil.Warn(GetType(), "물리공격력속성값이 존재하지 않습니다. lnPhysicalDefenseAttrValueId = " + lnPhysicalDefenseAttrValueId);
		}
		long lnMagicalDefenseAttrValueId = Convert.ToInt64(dr["magicalDefenseAttrValueId"]);
		m_magicalDefenseAttrValue = Resource.instance.GetAttrValue(lnMagicalDefenseAttrValueId);
		if (m_magicalDefenseAttrValue == null)
		{
			SFLogUtil.Warn(GetType(), "마법공격력속성값이 존재하지 않습니다. lnMagicalDefenseAttrValueId = " + lnMagicalDefenseAttrValueId);
		}
	}
}
