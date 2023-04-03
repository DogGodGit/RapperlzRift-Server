using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RankPassiveSkillLevel
{
	private RankPassiveSkill m_skill;

	private int m_nLevel;

	private long m_lnNextLevelUpRequiredGold;

	private int m_nNextLevelUpRequiredSpiritStone;

	private Dictionary<int, RankPassiveSkillAttrLevel> m_attrLevels = new Dictionary<int, RankPassiveSkillAttrLevel>();

	public RankPassiveSkill skill => m_skill;

	public int level => m_nLevel;

	public long nextLevelUpRequiredGold => m_lnNextLevelUpRequiredGold;

	public int nextLevelUpRequiredSpiritStone => m_nNextLevelUpRequiredSpiritStone;

	public Dictionary<int, RankPassiveSkillAttrLevel> attrLevels => m_attrLevels;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nSkillId = Convert.ToInt32(dr["skillId"]);
		if (nSkillId > 0)
		{
			m_skill = Resource.instance.GetRankPassiveSkill(nSkillId);
			if (m_skill == null)
			{
				SFLogUtil.Warn(GetType(), "스킬이 존재하지 않습니다. nSkillId = " + nSkillId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "스킬ID가 유효하지 않습니다. nSkillId = " + nSkillId);
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		m_lnNextLevelUpRequiredGold = Convert.ToInt64(dr["nextLevelUpRequiredGold"]);
		if (m_lnNextLevelUpRequiredGold < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업필요골드가 유효하지 않습니다. nSkillId = " + nSkillId + ", m_nLevel = " + m_nLevel + ", m_lnNextLevelUpRequiredGold = " + m_lnNextLevelUpRequiredGold);
		}
		m_nNextLevelUpRequiredSpiritStone = Convert.ToInt32(dr["nextLevelUpRequiredSpiritStone"]);
		if (m_nNextLevelUpRequiredSpiritStone < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업필요정령석이 유효하지 않습니다. nSkillId = " + nSkillId + ", m_nLevel = " + m_nLevel + ", m_nNextLevelUpRequiredSpiritStone = " + m_nNextLevelUpRequiredSpiritStone);
		}
	}

	public void AddAttrLevel(RankPassiveSkillAttrLevel attrLevel)
	{
		if (attrLevel == null)
		{
			throw new ArgumentNullException("attrLevel");
		}
		m_attrLevels.Add(attrLevel.attr.id, attrLevel);
	}

	public RankPassiveSkillAttrLevel GetAttrLevel(int nAttrId)
	{
		if (!m_attrLevels.TryGetValue(nAttrId, out var value))
		{
			return null;
		}
		return value;
	}
}
