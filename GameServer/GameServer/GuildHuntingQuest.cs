using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildHuntingQuest
{
	private int m_nLimitCount;

	private Npc m_questNpc;

	private ItemReward m_itemReward;

	private Dictionary<int, GuildHuntingQuestObjective> m_objectives = new Dictionary<int, GuildHuntingQuestObjective>();

	private List<GuildHuntingQuestObjectivePool> m_objectivePools = new List<GuildHuntingQuestObjectivePool>();

	public int limitCount => m_nLimitCount;

	public Npc questNpc => m_questNpc;

	public ItemReward itemReward => m_itemReward;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLimitCount = Convert.ToInt32(dr["limitCount"]);
		if (m_nLimitCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한횟수가 유효하지 않습니다. m_nLimitCount = " + m_nLimitCount);
		}
		int nQuestNpcId = Convert.ToInt32(dr["questNpcId"]);
		m_questNpc = Resource.instance.GetNpc(nQuestNpcId);
		if (m_questNpc == null)
		{
			SFLogUtil.Warn(GetType(), "퀘스트NPC가 존재하지 않습니다. nQuestNpcId = " + nQuestNpcId);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
		if (m_itemReward == null)
		{
			SFLogUtil.Warn(GetType(), "보상아이템이 존재하지 않습니다. lnItemRewardId = " + lnItemRewardId);
		}
	}

	public void AddObjective(GuildHuntingQuestObjective objective)
	{
		if (objective == null)
		{
			throw new ArgumentNullException("objective");
		}
		m_objectives.Add(objective.id, objective);
		GuildHuntingQuestObjectivePool objectivePool = GetObjectivePool(objective.minHeroLevel);
		if (objectivePool == null)
		{
			objectivePool = new GuildHuntingQuestObjectivePool(this, objective.minHeroLevel);
			m_objectivePools.Add(objectivePool);
		}
		objectivePool.AddObjective(objective);
	}

	public GuildHuntingQuestObjective GetObjective(int nId)
	{
		if (!m_objectives.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public GuildHuntingQuestObjectivePool GetObjectivePool(int nMinHeroLevel)
	{
		foreach (GuildHuntingQuestObjectivePool pool in m_objectivePools)
		{
			if (nMinHeroLevel == pool.minHeroLevel)
			{
				return pool;
			}
		}
		return null;
	}

	public GuildHuntingQuestObjectivePool GetObjectivePoolOfHeroLevel(int nHeroLevel)
	{
		GuildHuntingQuestObjectivePool objectivePool = null;
		foreach (GuildHuntingQuestObjectivePool pool in m_objectivePools)
		{
			if (nHeroLevel >= pool.minHeroLevel)
			{
				objectivePool = pool;
				continue;
			}
			return objectivePool;
		}
		return objectivePool;
	}
}
