using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DailyQuest
{
	private int m_nPlayCount;

	private int m_nRequiredHeroLevel;

	private int m_nFreeRefreshCount;

	private int m_nRefreshRequiredGold;

	private int m_nSlotCount;

	private List<DailyQuestReward> m_rewards = new List<DailyQuestReward>();

	private DailyQuestGrade[] m_grades = new DailyQuestGrade[5];

	private Dictionary<int, DailyQuestMission> m_missions = new Dictionary<int, DailyQuestMission>();

	private DailyQuestMissionPool[] m_missionPools;

	public int playCount => m_nPlayCount;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int freeRefreshCount => m_nFreeRefreshCount;

	public int refreshRequiredGold => m_nRefreshRequiredGold;

	public int slotCount => m_nSlotCount;

	private List<DailyQuestReward> rewards => m_rewards;

	public DailyQuestGrade[] grades => m_grades;

	public Dictionary<int, DailyQuestMission> missions => m_missions;

	public DailyQuestMissionPool[] missionPools => m_missionPools;

	public DailyQuest()
	{
		int nLastLevel = Resource.instance.lastJobLevelMaster.level;
		m_missionPools = new DailyQuestMissionPool[nLastLevel];
		for (int i = 0; i < nLastLevel; i++)
		{
			m_missionPools[i] = new DailyQuestMissionPool(this, i + 1);
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nPlayCount = Convert.ToInt32(dr["playCount"]);
		if (m_nPlayCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "실행횟수가 유효하지 않습니다. m_nPlayCount = " + m_nPlayCount);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "요구영웅레벨이 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		m_nFreeRefreshCount = Convert.ToInt32(dr["freeRefreshCount"]);
		if (m_nFreeRefreshCount < 0)
		{
			SFLogUtil.Warn(GetType(), "무료갱신횟수가 유효하지 않습니다. m_nFreeRefreshCount = " + m_nFreeRefreshCount);
		}
		m_nRefreshRequiredGold = Convert.ToInt32(dr["refreshRequiredGold"]);
		if (m_nRefreshRequiredGold < 0)
		{
			SFLogUtil.Warn(GetType(), "갱신필요골드가 유효하지 않습니다. m_nRefreshRequiredGold = " + m_nRefreshRequiredGold);
		}
		m_nSlotCount = Convert.ToInt32(dr["slotCount"]);
		if (m_nSlotCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "슬롯갯수가 유효하지 않습니다. m_nSlotCount = " + m_nSlotCount);
		}
	}

	public void AddReward(DailyQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public DailyQuestReward GetReward(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}

	public void AddGrade(DailyQuestGrade grade)
	{
		if (grade == null)
		{
			throw new ArgumentNullException("grade");
		}
		m_grades[grade.grade - 1] = grade;
	}

	public DailyQuestGrade GetGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_grades.Length)
		{
			return null;
		}
		return m_grades[nIndex];
	}

	public void AddMission(DailyQuestMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_missions.Add(mission.id, mission);
		for (int i = mission.requiredHeroLevel - 1; i < m_missionPools.Length; i++)
		{
			m_missionPools[i].AddMission(mission);
		}
	}

	public DailyQuestMission GetMission(int nId)
	{
		if (!m_missions.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public DailyQuestMissionPool GetMissionPool(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex > m_missionPools.Length)
		{
			return null;
		}
		return m_missionPools[nIndex];
	}
}
