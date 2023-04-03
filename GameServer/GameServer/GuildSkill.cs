using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class GuildSkill
{
	private int m_nId;

	private List<GuildSkillLevel> m_levels = new List<GuildSkillLevel>();

	private int[] m_maxLevelsOfLaboratoryLevel;

	public int id => m_nId;

	public GuildSkillLevel firstLevel => m_levels.FirstOrDefault();

	public GuildSkillLevel lastLevel => m_levels.LastOrDefault();

	public GuildSkill()
	{
		m_maxLevelsOfLaboratoryLevel = new int[Resource.instance.guildLaboratory.levels.Count];
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["guildSkillId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "길드스킬ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
	}

	public void AddLevel(GuildSkillLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_levels.Add(level);
		int nOldLevel = GetMaxLevelOfLaboratoryLevel(level.requiredLaboratoryLevel);
		if (level.level > nOldLevel)
		{
			SetMaxLevelOfLaboratoryLevel(level.requiredLaboratoryLevel, level.level);
		}
	}

	public GuildSkillLevel GetLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_levels.Count)
		{
			return null;
		}
		return m_levels[nIndex];
	}

	public int GetMaxLevelOfLaboratoryLevel(int nLaboratoryLevel)
	{
		int nIndex = nLaboratoryLevel - Resource.instance.guildLaboratory.firstLevel.level;
		if (nIndex < 0 || nIndex >= m_maxLevelsOfLaboratoryLevel.Length)
		{
			return -1;
		}
		return m_maxLevelsOfLaboratoryLevel[nIndex];
	}

	private void SetMaxLevelOfLaboratoryLevel(int nLaboratoryLevel, int nMaxLevel)
	{
		m_maxLevelsOfLaboratoryLevel[nLaboratoryLevel - Resource.instance.guildLaboratory.firstLevel.level] = nMaxLevel;
	}
}
