using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class JobSkillLevel
{
	private JobSkill m_skill;

	private JobSkillLevelMaster m_levelMaster;

	private long m_lnBattlePower;

	private AttrValue m_physicalOffenseAmpAttrValue;

	private AttrValue m_magicalOffenseAmpAttrValue;

	private AttrValue m_offensePointAttrValue;

	public JobSkill skill
	{
		get
		{
			return m_skill;
		}
		set
		{
			m_skill = value;
		}
	}

	public int level => m_levelMaster.level;

	public JobSkillLevelMaster levelMaster
	{
		get
		{
			return m_levelMaster;
		}
		set
		{
			m_levelMaster = value;
		}
	}

	public long battlePower => m_lnBattlePower;

	public AttrValue physocalOffenseAmpAttrValue => m_physicalOffenseAmpAttrValue;

	public int physicalOffenseAmp
	{
		get
		{
			if (m_physicalOffenseAmpAttrValue == null)
			{
				return 0;
			}
			return m_physicalOffenseAmpAttrValue.value;
		}
	}

	public AttrValue magicalOffenseAmpAttrValue => m_magicalOffenseAmpAttrValue;

	public int magicalOffenseAmp
	{
		get
		{
			if (m_magicalOffenseAmpAttrValue == null)
			{
				return 0;
			}
			return m_magicalOffenseAmpAttrValue.value;
		}
	}

	public AttrValue offensePointAttrValue => m_offensePointAttrValue;

	public int offensePoint => m_offensePointAttrValue.value;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_lnBattlePower = Convert.ToInt64(dr["battlePower"]);
		long lnPhysicalOffeneseAmpAttrValueId = Convert.ToInt64(dr["physicalOffenseAmpAttrValueId"]);
		m_physicalOffenseAmpAttrValue = Resource.instance.GetAttrValue(lnPhysicalOffeneseAmpAttrValueId);
		if (m_physicalOffenseAmpAttrValue == null)
		{
			SFLogUtil.Warn(GetType(), "물리공격력증폭 속성값ID가 존재하지 않습니다. lnPhysicalOffeneseAmpAttrValueId = " + lnPhysicalOffeneseAmpAttrValueId);
		}
		long lnMagicalOffenseAmpAttrValueId = Convert.ToInt64(dr["magicalOffenseAmpAttrValueId"]);
		m_magicalOffenseAmpAttrValue = Resource.instance.GetAttrValue(lnMagicalOffenseAmpAttrValueId);
		if (m_magicalOffenseAmpAttrValue == null)
		{
			SFLogUtil.Warn(GetType(), "마법공격력증폭 속성값ID가 존재하지 않습니다. lnMagicalOffenseAmpAttrValueId = " + lnMagicalOffenseAmpAttrValueId);
		}
		long lnOffensePointAttrValueId = Convert.ToInt64(dr["offensePointAttrValueId"]);
		m_offensePointAttrValue = Resource.instance.GetAttrValue(lnOffensePointAttrValueId);
		if (m_offensePointAttrValue == null)
		{
			SFLogUtil.Warn(GetType(), "공격력 속성값ID가 존재하지 않습니다. lnOffensePointAttrValueId = " + lnOffensePointAttrValueId);
		}
	}

	public void AddLevelMaster(JobSkillLevelMaster levelMaster)
	{
		if (levelMaster == null)
		{
			throw new ArgumentNullException("levelMaster");
		}
		m_levelMaster = levelMaster;
	}
}
