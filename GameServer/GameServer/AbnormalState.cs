using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AbnormalState
{
	public const int kId_Stun = 1;

	public const int kId_MoveRestriction = 2;

	public const int kId_Bleeding = 3;

	public const int kId_Flame = 4;

	public const int kId_OffenseDebuf = 5;

	public const int kId_DefenseDebuf = 6;

	public const int kId_ElementalDefenseDebuf = 7;

	public const int kId_DamageIncrease = 8;

	public const int kId_Evasion = 9;

	public const int kId_Shield = 10;

	public const int kId_WindWalk = 11;

	public const int kId_CurseDestruction = 12;

	public const int kId_AngerExplosion = 13;

	public const int kId_AbnormalStateImmune = 101;

	public const int kId_DamageAbsorbShield = 102;

	public const int kId_PhysicalOffenseBuff = 103;

	public const int kId_PhysicalOffenseBuff_HitByAddDuration = 104;

	public const int kId_PhysicalOffenseBuff_Penetration = 105;

	public const int kId_PhysicalOffenseBuff_Penetration_Immortality = 106;

	public const int kId_PhysicalOffenseBuff_CriticalBuff = 107;

	public const int kId_MagicalOffenseBuff_DefenseBuff = 108;

	public const int kId_MagicalOffenseBuff_DefenseBuff_DamageAbsorbShield = 109;

	public const int kId_MagicalOffenseBuff_DefenseBuff_ElementalOffenseBuff = 110;

	public const int kId_Suppression = 111;

	public const int kAbnormalStateHitInterval = 1;

	public const int kHitByAddSecond = 1;

	public const int kMaxHitByAddDuration = 2;

	public const int kImmortalityCount = 1;

	public const int kType_Disadvantage = 1;

	public const int kType_Advantage = 2;

	public const int kSourceType_HeroSkill = 1;

	public const int kSourceType_RankSkill = 2;

	private int m_nId;

	private string m_sNameKey;

	private string m_sDescriptionKey;

	private bool m_bIsOverlap;

	private int m_nType;

	private int m_nSourceType;

	private Dictionary<int, JobAbnormalState> m_jobAbnormalStates = new Dictionary<int, JobAbnormalState>();

	private List<AbnormalStateRankSkillLevel> m_abnormalStateRankSkillLevels = new List<AbnormalStateRankSkillLevel>();

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public string descriptionKey => m_sDescriptionKey;

	public bool isOverlap => m_bIsOverlap;

	public int type => m_nType;

	public int sourceType => m_nSourceType;

	public bool isTickPerSecondAbnormalState
	{
		get
		{
			if (m_nId != 3 && m_nId != 4)
			{
				return m_nId == 11;
			}
			return true;
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["abnormalStateId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_sDescriptionKey = Convert.ToString(dr["descriptionKey"]);
		m_bIsOverlap = Convert.ToBoolean(dr["isOverlap"]);
		m_nType = Convert.ToInt32(dr["type"]);
		if (!IsDefinedType(m_nType))
		{
			SFLogUtil.Warn(GetType(), "상태이상타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nType = " + m_nType);
		}
		m_nSourceType = Convert.ToInt32(dr["sourceType"]);
		if (!IsDefinedSourceType(m_nSourceType))
		{
			SFLogUtil.Warn(GetType(), "소스타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nSourceType = " + m_nSourceType);
		}
	}

	public JobAbnormalState GetJobAbnormalState(int nJobId)
	{
		if (!m_jobAbnormalStates.TryGetValue(nJobId, out var value))
		{
			return null;
		}
		return value;
	}

	public JobAbnormalState GetOrCreateJobAbnormalState(Job job)
	{
		int nJobId = job.id;
		JobAbnormalState jobAbnormalState = GetJobAbnormalState(nJobId);
		if (jobAbnormalState == null)
		{
			jobAbnormalState = new JobAbnormalState(job);
			jobAbnormalState.abnormalState = this;
			m_jobAbnormalStates.Add(nJobId, jobAbnormalState);
		}
		return jobAbnormalState;
	}

	public void AddAbnormalStateRankSkillLevel(AbnormalStateRankSkillLevel rankSkillLevel)
	{
		if (rankSkillLevel == null)
		{
			throw new ArgumentNullException("rankSkillLevel");
		}
		m_abnormalStateRankSkillLevels.Add(rankSkillLevel);
	}

	public AbnormalStateRankSkillLevel GetAbnormalStateRankSkillLevel(int nSkillLevel)
	{
		int nIndex = nSkillLevel - 1;
		if (nIndex < 0 || nIndex >= m_abnormalStateRankSkillLevels.Count)
		{
			return null;
		}
		return m_abnormalStateRankSkillLevels[nIndex];
	}

	public static bool IsDefinedType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}

	public static bool IsDefinedSourceType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
