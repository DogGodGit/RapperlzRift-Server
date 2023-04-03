using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RankActiveSkillLevel
{
	private RankActiveSkill m_skill;

	private int m_nLevel;

	private long m_lnNextLevelUpRequiredGold;

	private int m_nNextLevelUpRequiredItemId;

	private int m_nNextLevelUpRequiredItemCount;

	public RankActiveSkill skill => m_skill;

	public int level => m_nLevel;

	public long nextLevelUpRequiredGold => m_lnNextLevelUpRequiredGold;

	public int nextLevelUpRequiredItemId => m_nNextLevelUpRequiredItemId;

	public int nextLevelUpRequiredItemCount => m_nNextLevelUpRequiredItemCount;

	public RankActiveSkillLevel(RankActiveSkill skill)
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
		m_lnNextLevelUpRequiredGold = Convert.ToInt64(dr["nextLevelUpRequiredGold"]);
		if (m_lnNextLevelUpRequiredGold < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업필요골드가 유효하지 않습니다. skillId = " + m_skill.id + ", m_nLevel = " + m_nLevel + ", m_lnNextLevelUpRequiredGold = " + m_lnNextLevelUpRequiredGold);
		}
		m_nNextLevelUpRequiredItemId = Convert.ToInt32(dr["nextLevelUpRequiredItemId"]);
		if (m_nNextLevelUpRequiredItemCount > 0)
		{
			if (Resource.instance.GetItem(m_nNextLevelUpRequiredItemId) == null)
			{
				SFLogUtil.Warn(GetType(), "다음레벨업필요아이템ID가 존재하지 않습니다. skillId = " + m_skill.id + ", m_nLevel = " + m_nLevel + ", m_nNextLevelUpRequiredItemId = " + m_nNextLevelUpRequiredItemId);
			}
		}
		else if (m_nNextLevelUpRequiredItemCount < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업필요아이템ID가 유효하지 않습니다. skillId = " + m_skill.id + ", m_nLevel = " + m_nLevel + ", m_nNextLevelUpRequiredItemId = " + m_nNextLevelUpRequiredItemId);
		}
		m_nNextLevelUpRequiredItemCount = Convert.ToInt32(dr["nextLevelUpRequiredItemCount"]);
		if (m_nNextLevelUpRequiredItemCount < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업필요아이템수량이 유효하지 않습니다. skillId = " + m_skill.id + ", m_nLevel = " + m_nLevel + ", m_nNextLevelUpRequiredItemCount = " + m_nNextLevelUpRequiredItemCount);
		}
	}
}
