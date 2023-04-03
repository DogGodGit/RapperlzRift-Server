using System;
using System.Collections.Generic;

namespace GameServer;

public class DailyQuestMissionPool
{
	private DailyQuest m_dailyQuest;

	private int m_nLevel;

	private List<DailyQuestMission> m_missions = new List<DailyQuestMission>();

	public DailyQuest dailyQuest => m_dailyQuest;

	public int level => m_nLevel;

	public List<DailyQuestMission> missions => m_missions;

	public DailyQuestMissionPool(DailyQuest quest, int nLevel)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		if (nLevel <= 0)
		{
			throw new ArgumentOutOfRangeException("level");
		}
		m_dailyQuest = quest;
		m_nLevel = nLevel;
	}

	public void AddMission(DailyQuestMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_missions.Add(mission);
	}

	public DailyQuestMission SelectMission(ICollection<int> exclusiveMissionIds)
	{
		List<DailyQuestMission> targetMissions = new List<DailyQuestMission>();
		int nTotalPickPoint = 0;
		foreach (DailyQuestMission mission in m_missions)
		{
			if (!exclusiveMissionIds.Contains(mission.id))
			{
				targetMissions.Add(mission);
				nTotalPickPoint += mission.point;
			}
		}
		return Util.SelectPickEntry(targetMissions, nTotalPickPoint);
	}
}
