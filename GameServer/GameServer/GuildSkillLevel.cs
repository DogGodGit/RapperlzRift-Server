using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildSkillLevel
{
	private GuildSkill m_skill;

	private int m_nLevel;

	private int m_nRequiredGuildContributionPoint;

	private int m_nRequiredLaboratoryLevel;

	private Dictionary<int, GuildSkillLevelAttrValue> m_attrValues = new Dictionary<int, GuildSkillLevelAttrValue>();

	public GuildSkill skill => m_skill;

	public int level => m_nLevel;

	public int requiredGuildContributionPoint => m_nRequiredGuildContributionPoint;

	public int requiredLaboratoryLevel => m_nRequiredLaboratoryLevel;

	public Dictionary<int, GuildSkillLevelAttrValue> attrValues => m_attrValues;

	public GuildSkillLevel(GuildSkill skill)
	{
		m_skill = skill;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		if (m_nLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel);
		}
		m_nRequiredGuildContributionPoint = Convert.ToInt32(dr["requiredGuildContributionPoint"]);
		if (m_nRequiredGuildContributionPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업길드공헌도가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nRequiredGuildContributionPoint = " + m_nRequiredGuildContributionPoint);
		}
		m_nRequiredLaboratoryLevel = Convert.ToInt32(dr["requiredLaboratoryLevel"]);
		if (m_nRequiredLaboratoryLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업연구소레벨가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nRequiredLaboratoryLevel = " + m_nRequiredLaboratoryLevel);
		}
	}

	public void AddAttrValues(GuildSkillLevelAttrValue attrValue)
	{
		if (attrValue == null)
		{
			throw new ArgumentNullException("attrValue");
		}
		m_attrValues.Add(attrValue.attrId, attrValue);
	}
}
