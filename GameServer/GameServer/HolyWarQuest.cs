using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class HolyWarQuest
{
	private int m_nRequiredHeroLevel;

	private Npc m_questNpc;

	private int m_nLimitTime;

	private Dictionary<int, HolyWarQuestSchedule> m_schedules = new Dictionary<int, HolyWarQuestSchedule>();

	private List<HolyWarQuestGloryLevel> m_gloryLevels = new List<HolyWarQuestGloryLevel>();

	private List<HolyWarQuestReward> m_rewards = new List<HolyWarQuestReward>();

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public Npc questNpc => m_questNpc;

	public int limitTime => m_nLimitTime;

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
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		if (m_nLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한시간이 유효하지 않습니다. m_nLimitTime = " + m_nLimitTime);
		}
	}

	public void AddSchedule(HolyWarQuestSchedule schedule)
	{
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_schedules.Add(schedule.id, schedule);
	}

	public HolyWarQuestSchedule GetSchedule(int nId)
	{
		if (!m_schedules.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public HolyWarQuestSchedule GetScheduleByTime(float fTime)
	{
		foreach (HolyWarQuestSchedule schedule in m_schedules.Values)
		{
			if (schedule.Contains(fTime))
			{
				return schedule;
			}
		}
		return null;
	}

	public void AddGloryLevel(HolyWarQuestGloryLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_gloryLevels.Add(level);
	}

	public HolyWarQuestGloryLevel GetGloryLevelByKillCount(int nKillCount)
	{
		HolyWarQuestGloryLevel result = null;
		foreach (HolyWarQuestGloryLevel level in m_gloryLevels)
		{
			if (nKillCount >= level.requiredKillCount)
			{
				result = level;
				continue;
			}
			return result;
		}
		return result;
	}

	public void AddReward(HolyWarQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public HolyWarQuestReward GetReward(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}
}
