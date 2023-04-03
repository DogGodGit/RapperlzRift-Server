using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WeeklyQuest
{
	public const int k10Round = 10;

	private int m_nRoundCount;

	private int m_nRequiredHeroLevel;

	private int m_nRoundRefreshRequiredGold;

	private int m_nRoundImmediateCompletionRequiredItemId;

	private int m_nTenRoundCompletionRequiredVipLevel;

	private float m_fTenRoundCompletionRewardFactor;

	private List<WeeklyQuestRound> m_rounds = new List<WeeklyQuestRound>();

	private List<WeeklyQuestTenRoundReward> m_tenRoundRewards = new List<WeeklyQuestTenRoundReward>();

	private Dictionary<int, WeeklyQuestMission> m_missions = new Dictionary<int, WeeklyQuestMission>();

	private WeeklyQuestMissionPool[] m_missionPools;

	public int roundCount => m_nRoundCount;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int roundRefreshRequiredGold => m_nRoundRefreshRequiredGold;

	public int roundImmediateCompletionRequiredItemId => m_nRoundImmediateCompletionRequiredItemId;

	public int tenRoundCompletionRequiredVipLevel => m_nTenRoundCompletionRequiredVipLevel;

	public float tenRoundCompletionRewardFactor => m_fTenRoundCompletionRewardFactor;

	public List<WeeklyQuestTenRoundReward> tenRoundRewards => m_tenRoundRewards;

	public WeeklyQuest()
	{
		m_missionPools = new WeeklyQuestMissionPool[Resource.instance.lastJobLevelMaster.level];
		for (int i = 0; i < m_missionPools.Length; i++)
		{
			m_missionPools[i] = new WeeklyQuestMissionPool(this, i + 1);
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRoundCount = Convert.ToInt32(dr["roundCount"]);
		if (m_nRoundCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "라운드횟수가 유효하지 않습니다. m_nRoundCount = " + m_nRoundCount);
		}
		for (int nRound = 1; nRound <= m_nRoundCount; nRound++)
		{
			WeeklyQuestRound round = new WeeklyQuestRound(this, nRound);
			AddRound(round);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "요구영웅레벨이 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		m_nRoundRefreshRequiredGold = Convert.ToInt32(dr["roundRefreshRequiredGold"]);
		if (m_nRoundRefreshRequiredGold < 0)
		{
			SFLogUtil.Warn(GetType(), "라운드갱신필요골드가 유효하지 않습니다. m_nRoundRefreshRequiredGold = " + m_nRoundRefreshRequiredGold);
		}
		m_nRoundImmediateCompletionRequiredItemId = Convert.ToInt32(dr["roundImmediateCompletionRequiredItemId"]);
		if (Resource.instance.GetItem(m_nRoundImmediateCompletionRequiredItemId) == null)
		{
			SFLogUtil.Warn(GetType(), "라운드즉시완성요구아이템이 유효하지 않습니다. m_nMissionImmediateCompletionRequiredItemId = " + m_nRoundImmediateCompletionRequiredItemId);
		}
		m_nTenRoundCompletionRequiredVipLevel = Convert.ToInt32(dr["tenRoundCompletionRequiredVipLevel"]);
		if (m_nTenRoundCompletionRequiredVipLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "10라운드전체완료요구VIP레벨이 유효하지 않습니다. m_nTenRoundCompletionRequiredVipLevel = " + m_nTenRoundCompletionRequiredVipLevel);
		}
		m_fTenRoundCompletionRewardFactor = Convert.ToInt32(dr["tenRoundCompletionRewardFactor"]);
		if (m_fTenRoundCompletionRewardFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "10라운드전체완료보상계수가 유효하지 않습니다. m_fTenRoundCompletionRewardFactor = " + m_fTenRoundCompletionRewardFactor);
		}
	}

	public void AddRound(WeeklyQuestRound round)
	{
		if (round == null)
		{
			throw new ArgumentNullException("round");
		}
		m_rounds.Add(round);
	}

	public WeeklyQuestRound GetRound(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_rounds.Count)
		{
			return null;
		}
		return m_rounds[nIndex];
	}

	public void AddTenRoundReward(WeeklyQuestTenRoundReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_tenRoundRewards.Add(reward);
	}

	public void AddMission(WeeklyQuestMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_missions.Add(mission.id, mission);
		for (int nHeroLevel = mission.minHeroLevel; nHeroLevel <= mission.maxHeroLevel; nHeroLevel++)
		{
			WeeklyQuestMissionPool pool = GetPool(nHeroLevel);
			if (pool == null)
			{
				SFLogUtil.Warn(GetType(), "미션풀이 존재하지 않습니다. nHeroLevel = " + nHeroLevel);
			}
			else
			{
				pool.AddMission(mission);
			}
		}
	}

	public WeeklyQuestMission GetMission(int nId)
	{
		if (!m_missions.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public WeeklyQuestMissionPool GetPool(int nHeroLevel)
	{
		int nIndex = nHeroLevel - 1;
		if (nIndex < 0 || nIndex >= m_missionPools.Length)
		{
			return null;
		}
		return m_missionPools[nIndex];
	}
}
