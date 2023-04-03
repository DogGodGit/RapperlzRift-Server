using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class RankPassiveSkillAttr
{
	private RankPassiveSkill m_skill;

	private int m_nId;

	private List<RankPassiveSkillAttrLevel> m_attrLevels = new List<RankPassiveSkillAttrLevel>();

	public RankPassiveSkill skill => m_skill;

	public int id => m_nId;

	public RankPassiveSkillAttr(RankPassiveSkill skill)
	{
		m_skill = skill;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["attrId"]);
	}

	public void AddAttrLevel(RankPassiveSkillAttrLevel attrLevel)
	{
		if (attrLevel == null)
		{
			throw new ArgumentNullException("attrLevel");
		}
		m_attrLevels.Add(attrLevel);
	}

	public RankPassiveSkillAttrLevel GetAttrLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_attrLevels.Count)
		{
			return null;
		}
		return m_attrLevels[nIndex];
	}
}
