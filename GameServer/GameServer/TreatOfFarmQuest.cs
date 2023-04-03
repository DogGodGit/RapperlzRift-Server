using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TreatOfFarmQuest
{
	private int m_nRequiredHeroLevel;

	private string m_sTitleKey;

	private int m_nLimitCount;

	private Npc m_questNpc;

	private int m_nMonsterKillLimitTime;

	private Dictionary<int, TreatOfFarmQuestMission> m_missions = new Dictionary<int, TreatOfFarmQuestMission>();

	private int nTotalPoint;

	private List<TreatOfFarmQuestReward> m_rewards = new List<TreatOfFarmQuestReward>();

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public string titleKey => m_sTitleKey;

	public int limitCount => m_nLimitCount;

	public Npc questNpc => m_questNpc;

	public int monsterKillLimitTime => m_nMonsterKillLimitTime;

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
		m_sTitleKey = Convert.ToString(dr["titleKey"]);
		m_nLimitCount = Convert.ToInt32(dr["limitCount"]);
		if (m_nLimitCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한횟수가 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel + ", m_nLimitCount = " + m_nLimitCount);
		}
		int nQuestNpcId = Convert.ToInt32(dr["questNpcId"]);
		m_questNpc = Resource.instance.GetNpc(nQuestNpcId);
		if (m_questNpc == null)
		{
			SFLogUtil.Warn(GetType(), "퀘스트NPC가 존재하지 않습니다.  m_nRequiredHeroLevel = " + m_nRequiredHeroLevel + ", nQuestNpcId = " + nQuestNpcId);
		}
		m_nMonsterKillLimitTime = Convert.ToInt32(dr["monsterKillLimitTime"]);
		if (m_nMonsterKillLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터처치제한시간이 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel + ", m_nMonsterKillLimitTime = " + m_nMonsterKillLimitTime);
		}
	}

	public void AddMission(TreatOfFarmQuestMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_missions.Add(mission.id, mission);
		nTotalPoint += mission.point;
	}

	public TreatOfFarmQuestMission GetMission(int nId)
	{
		if (!m_missions.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public TreatOfFarmQuestMission SelectMission()
	{
		return Util.SelectPickEntry(m_missions.Values, nTotalPoint);
	}

	public void AddReward(TreatOfFarmQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public TreatOfFarmQuestReward GetReward(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}
}
