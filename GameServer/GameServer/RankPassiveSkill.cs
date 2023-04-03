using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class RankPassiveSkill
{
	private int m_nId;

	private Rank m_requiredRank;

	private Dictionary<int, RankPassiveSkillAttr> m_attrs = new Dictionary<int, RankPassiveSkillAttr>();

	private List<RankPassiveSkillLevel> m_levels = new List<RankPassiveSkillLevel>();

	public int id => m_nId;

	public Rank requiredRank => m_requiredRank;

	public RankPassiveSkillLevel maxLevel => m_levels.LastOrDefault();

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["skillId"]);
		int nRequiredRankNo = Convert.ToInt32(dr["requiredRankNo"]);
		if (nRequiredRankNo > 0)
		{
			m_requiredRank = Resource.instance.GetRank(nRequiredRankNo);
			if (m_requiredRank == null)
			{
				SFLogUtil.Warn(GetType(), "필요계급이 존재하지 않습니다. m_nId = " + m_nId + ", nRequiredRankNo = " + nRequiredRankNo);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "필요계급번호가 유효하지 않습니다. m_nId = " + m_nId + ", nRequiredRankNo = " + nRequiredRankNo);
		}
	}

	public void AddAttr(RankPassiveSkillAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attrs.Add(attr.id, attr);
	}

	public RankPassiveSkillAttr GetAttr(int nAttrId)
	{
		if (!m_attrs.TryGetValue(nAttrId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddLevel(RankPassiveSkillLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_levels.Add(level);
	}

	public RankPassiveSkillLevel GetLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_levels.Count)
		{
			return null;
		}
		return m_levels[nIndex];
	}
}
