using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class DimensionRaidQuest
{
	private int m_nRequiredHeroLevel;

	private Npc m_questNpc;

	private int m_nLimitCount;

	private int m_nTargetInteractionDuration;

	private List<DimensionRaidQuestStep> m_steps = new List<DimensionRaidQuestStep>();

	private List<DimensionRaidQuestReward> m_rewards = new List<DimensionRaidQuestReward>();

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public Npc questNpc => m_questNpc;

	public int limitCount => m_nLimitCount;

	public int targetInteractionDuration => m_nTargetInteractionDuration;

	public DimensionRaidQuestStep lastStep => m_steps.LastOrDefault();

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "요구영웅레벨이 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		int nQuestNpcId = Convert.ToInt32(dr["questNpcId"]);
		m_questNpc = Resource.instance.GetNpc(nQuestNpcId);
		if (m_questNpc == null)
		{
			SFLogUtil.Warn(GetType(), "퀘스트NPC가 존재하지 않습니다. nQuestNpcId = " + nQuestNpcId);
		}
		m_nLimitCount = Convert.ToInt32(dr["limitCount"]);
		if (m_nLimitCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한횟수가 유효하지 않습니다. m_nLimitCount = " + m_nLimitCount);
		}
		m_nTargetInteractionDuration = Convert.ToInt32(dr["targetInteractionDuration"]);
		if (m_nTargetInteractionDuration <= 0)
		{
			SFLogUtil.Warn(GetType(), "대상상호작용기간이 유효하지 않습니다. m_nTargetInteractionDuration = " + m_nTargetInteractionDuration);
		}
	}

	public void AddStep(DimensionRaidQuestStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_steps.Add(step);
	}

	public DimensionRaidQuestStep GetStep(int nStep)
	{
		int nIndex = nStep - 1;
		if (nIndex < 0 || nIndex >= m_steps.Count)
		{
			return null;
		}
		return m_steps[nIndex];
	}

	public void AddReward(DimensionRaidQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public DimensionRaidQuestReward GetReward(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}
}
