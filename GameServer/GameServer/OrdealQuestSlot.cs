using System;
using System.Collections.Generic;

namespace GameServer;

public class OrdealQuestSlot
{
	private OrdealQuest m_quest;

	private int m_nIndex;

	private List<OrdealQuestMission> m_missions = new List<OrdealQuestMission>();

	public OrdealQuest quest => m_quest;

	public int index => m_nIndex;

	public int maxMissionNo => m_missions.Count;

	public List<OrdealQuestMission> missions => m_missions;

	public OrdealQuestSlot(OrdealQuest quest, int nIndex)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_quest = quest;
		m_nIndex = nIndex;
	}

	public void AddMission(OrdealQuestMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_missions.Add(mission);
	}

	public OrdealQuestMission GetMission(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_missions.Count)
		{
			return null;
		}
		return m_missions[nIndex];
	}
}
