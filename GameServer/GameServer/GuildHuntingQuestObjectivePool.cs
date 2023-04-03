using System;
using System.Collections.Generic;

namespace GameServer;

public class GuildHuntingQuestObjectivePool
{
	private GuildHuntingQuest m_quest;

	private int m_nMinHeroLevel;

	private Dictionary<int, GuildHuntingQuestObjective> m_objectives = new Dictionary<int, GuildHuntingQuestObjective>();

	private int m_nTotalPoint;

	public GuildHuntingQuest quest => m_quest;

	public int minHeroLevel => m_nMinHeroLevel;

	public Dictionary<int, GuildHuntingQuestObjective> objectives => m_objectives;

	public int totalPoint => m_nTotalPoint;

	public GuildHuntingQuestObjectivePool(GuildHuntingQuest quest, int nMinHeroLevel)
	{
		m_quest = quest;
		m_nMinHeroLevel = nMinHeroLevel;
	}

	public void AddObjective(GuildHuntingQuestObjective objective)
	{
		if (objective == null)
		{
			throw new ArgumentNullException("objective");
		}
		m_objectives.Add(objective.id, objective);
		m_nTotalPoint += objective.point;
		objective.pool = this;
	}

	public GuildHuntingQuestObjective SelectObjective()
	{
		return Util.SelectPickEntry(m_objectives.Values, m_nTotalPoint);
	}
}
