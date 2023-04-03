using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MysteryBoxQuest
{
	private int m_nRequiredHeroLevel;

	private Npc m_questNpc;

	private Npc m_targetNpc;

	private int m_nLimitCount;

	private int m_nInteractionDuration;

	private int m_nDefaultBoxGradePoolId;

	private MysteryBoxGradePool m_defaultBoxGradePool;

	private int m_nVipBoostMinPickCount;

	private MysteryBoxGrade[] m_boxGrades = new MysteryBoxGrade[5];

	private Dictionary<int, MysteryBoxGradePool> m_boxGradePools = new Dictionary<int, MysteryBoxGradePool>();

	private MysteryBoxQuestReward[,] m_rewards;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public Npc questNpc => m_questNpc;

	public Npc targetNpc => m_targetNpc;

	public int limitCount => m_nLimitCount;

	public int interactionDuration => m_nInteractionDuration;

	public int defaultBoxGradePoolId => m_nDefaultBoxGradePoolId;

	public MysteryBoxGradePool defaultBoxGradePool => m_defaultBoxGradePool;

	public int vipBoostMinPickCount => m_nVipBoostMinPickCount;

	public MysteryBoxQuest()
	{
		m_rewards = new MysteryBoxQuestReward[5, Resource.instance.lastJobLevelMaster.level];
	}

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
		int nTargetNpcId = Convert.ToInt32(dr["targetNpcId"]);
		m_targetNpc = Resource.instance.GetNpc(nTargetNpcId);
		if (m_targetNpc == null)
		{
			SFLogUtil.Warn(GetType(), "대상NPC가 존재하지 않습니다. nTargetNpcId = " + nTargetNpcId);
		}
		m_nLimitCount = Convert.ToInt32(dr["limitCount"]);
		if (m_nLimitCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한횟수가 유효하지 않습니다. m_nLimitCount = " + m_nLimitCount);
		}
		m_nInteractionDuration = Convert.ToInt32(dr["interactionDuration"]);
		if (m_nInteractionDuration <= 0)
		{
			SFLogUtil.Warn(GetType(), "상호작용기간이 유효하지 않습니다. m_nInteractionDuration = " + m_nInteractionDuration);
		}
		m_nDefaultBoxGradePoolId = Convert.ToInt32(dr["defaultBoxGradePoolId"]);
		if (m_nDefaultBoxGradePoolId <= 0)
		{
			SFLogUtil.Warn(GetType(), "기본상자등급풀ID가 유효하지 않습니다. m_nDefaultBoxGradePoolId = " + m_nDefaultBoxGradePoolId);
		}
		m_nVipBoostMinPickCount = Convert.ToInt32(dr["vipBoostMinPickCount"]);
		if (m_nVipBoostMinPickCount < 0)
		{
			SFLogUtil.Warn(GetType(), "vip부스트최소뽑기수가 유효하지 않습니다. m_nVipBoostMinPickCount = " + m_nVipBoostMinPickCount);
		}
	}

	public void SetLate()
	{
		m_defaultBoxGradePool = GetBoxGradePool(m_nDefaultBoxGradePoolId);
		if (m_defaultBoxGradePool == null)
		{
			SFLogUtil.Warn(GetType(), "기본상자등급풀이 존재하지 않습니다. m_nDefaultBoxGradePoolId = " + m_nDefaultBoxGradePoolId);
		}
	}

	public void AddReward(MysteryBoxQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards[reward.grade - 1, reward.level - 1] = reward;
	}

	public MysteryBoxQuestReward GetReward(int nGrade, int nLevel)
	{
		int nGradeIndex = nGrade - 1;
		if (nGradeIndex < 0 || nGradeIndex >= m_rewards.GetLength(0))
		{
			return null;
		}
		int nLevelIndex = nLevel - 1;
		if (nLevelIndex < 0 || nLevelIndex >= m_rewards.GetLength(1))
		{
			return null;
		}
		return m_rewards[nGradeIndex, nLevelIndex];
	}

	public long GetExpReward(int nGrade, int nLevel)
	{
		return GetReward(nGrade, nLevel)?.expRewardValue ?? 0;
	}

	public void AddBoxGrade(MysteryBoxGrade grade)
	{
		if (grade == null)
		{
			throw new ArgumentNullException("grade");
		}
		m_boxGrades[grade.id - 1] = grade;
	}

	public MysteryBoxGrade GetBoxGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_boxGrades.Length)
		{
			return null;
		}
		return m_boxGrades[nIndex];
	}

	public MysteryBoxGradePool GetBoxGradePool(int nId)
	{
		if (!m_boxGradePools.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public MysteryBoxGradePool GetOrCreateBoxGradePool(int nId)
	{
		MysteryBoxGradePool pool = GetBoxGradePool(nId);
		if (pool == null)
		{
			pool = new MysteryBoxGradePool(nId);
			m_boxGradePools.Add(nId, pool);
		}
		return pool;
	}
}
