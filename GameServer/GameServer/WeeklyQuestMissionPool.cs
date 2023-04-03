using System;
using System.Collections.Generic;

namespace GameServer;

public class WeeklyQuestMissionPool
{
	private WeeklyQuest m_quest;

	private int m_nHeroLevel;

	private Dictionary<int, WeeklyQuestMission> m_missions = new Dictionary<int, WeeklyQuestMission>();

	private int m_nTotalPickPoint;

	public WeeklyQuest quest => m_quest;

	public int heroLevel => m_nHeroLevel;

	public Dictionary<int, WeeklyQuestMission> missions => m_missions;

	public WeeklyQuestMissionPool(WeeklyQuest quest, int nHeroLevel)
	{
		m_quest = quest;
		m_nHeroLevel = nHeroLevel;
	}

	public void AddMission(WeeklyQuestMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_missions.Add(mission.id, mission);
		m_nTotalPickPoint += mission.point;
	}

	public WeeklyQuestMission SelectMission()
	{
		return Util.SelectPickEntry(m_missions.Values, m_nTotalPickPoint);
	}
}
