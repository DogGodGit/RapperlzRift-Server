using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SecretLetterQuest
{
	private int m_nRequiredHeroLevel;

	private Npc m_questNpc;

	private Npc m_targetNpc;

	private int m_nLimitCount;

	private int m_nInteractionDuration;

	private int m_nDefaultLetterGradePoolId;

	private SecretLetterGradePool m_defaultLetterGradePool;

	private int m_nVipBoostMinPickCount;

	private SecretLetterGrade[] m_letterGrades = new SecretLetterGrade[5];

	private Dictionary<int, SecretLetterGradePool> m_letterGradePools = new Dictionary<int, SecretLetterGradePool>();

	private SecretLetterQuestReward[,] m_rewards;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public Npc questNpc => m_questNpc;

	public Npc targetNpc => m_targetNpc;

	public int limitCount => m_nLimitCount;

	public int interactionDuration => m_nInteractionDuration;

	public int defaultLetterGradePoolId => m_nDefaultLetterGradePoolId;

	public SecretLetterGradePool defaultLetterGradePool => m_defaultLetterGradePool;

	public int vipBoostMinPickCount => m_nVipBoostMinPickCount;

	public SecretLetterQuest()
	{
		m_rewards = new SecretLetterQuestReward[5, Resource.instance.lastJobLevelMaster.level];
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
		m_nDefaultLetterGradePoolId = Convert.ToInt32(dr["defaultLetterGradePoolId"]);
		if (m_nDefaultLetterGradePoolId <= 0)
		{
			SFLogUtil.Warn(GetType(), "기본밀서등급풀ID가 유효하지 않습니다. m_nDefaultLetterGradePoolId = " + m_nDefaultLetterGradePoolId);
		}
		m_nVipBoostMinPickCount = Convert.ToInt32(dr["vipBoostMinPickCount"]);
		if (m_nVipBoostMinPickCount < 0)
		{
			SFLogUtil.Warn(GetType(), "vip부스트최소뽑기수가 유효하지 않습니다. m_nVipBoostMinPickCount = " + m_nVipBoostMinPickCount);
		}
	}

	public void SetLate()
	{
		m_defaultLetterGradePool = GetLetterGradePool(m_nDefaultLetterGradePoolId);
		if (m_defaultLetterGradePool == null)
		{
			SFLogUtil.Warn(GetType(), "기본밀서등급풀이 존재하지 않습니다. m_nDefaultLetterGradePoolId = " + m_nDefaultLetterGradePoolId);
		}
	}

	public void AddReward(SecretLetterQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards[reward.grade - 1, reward.level - 1] = reward;
	}

	public SecretLetterQuestReward GetReward(int nGrade, int nLevel)
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

	public void AddLetterGrade(SecretLetterGrade grade)
	{
		if (grade == null)
		{
			throw new ArgumentNullException("grade");
		}
		m_letterGrades[grade.id - 1] = grade;
	}

	public SecretLetterGrade GetLetterGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_letterGrades.Length)
		{
			return null;
		}
		return m_letterGrades[nIndex];
	}

	public SecretLetterGradePool GetLetterGradePool(int nId)
	{
		if (!m_letterGradePools.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public SecretLetterGradePool GetOrCreateLetterGradePool(int nId)
	{
		SecretLetterGradePool pool = GetLetterGradePool(nId);
		if (pool == null)
		{
			pool = new SecretLetterGradePool(nId);
			m_letterGradePools.Add(nId, pool);
		}
		return pool;
	}
}
