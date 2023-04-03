using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FearAltar
{
	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

	public const float kSafetyRevivalWaitingTimeFactor = 0.9f;

	private int m_nRequiredConditionType;

	private int m_nRequiredHeroLevel;

	private int m_nRequiredMainQuestNo;

	private int m_nRequiredStamina;

	private int m_nEnterMinMemberCount;

	private int m_nEnterMaxMemberCount;

	private int m_nMatchingWaitingTime;

	private int m_nEnterWaitingTime;

	private int m_nStartDelayTime;

	private int m_nLimitTime;

	private int m_nExitDelayTime;

	private int m_nSafeRevivalWaitingTime;

	private int m_nHalidomMonsterLifetime;

	private int m_nHalidomAcquisitionRate;

	private Dictionary<int, FearAltarMonsterAttrFactor> m_monsterAttrFactors = new Dictionary<int, FearAltarMonsterAttrFactor>();

	private Dictionary<int, FearAltarReward> m_rewards = new Dictionary<int, FearAltarReward>();

	private List<FearAltarHalidomCollectionReward> m_halidomCollectionRewards = new List<FearAltarHalidomCollectionReward>();

	private Dictionary<int, FearAltarHalidomElemental> m_halidomElementals = new Dictionary<int, FearAltarHalidomElemental>();

	private Dictionary<int, FearAltarHalidomLevel> m_halidomLevels = new Dictionary<int, FearAltarHalidomLevel>();

	private Dictionary<int, FearAltarHalidom> m_halidoms = new Dictionary<int, FearAltarHalidom>();

	private int m_nHalidomTotalPoint;

	private Dictionary<int, FearAltarStage> m_stages = new Dictionary<int, FearAltarStage>();

	private int m_nStageTotalPoint;

	private Dictionary<int, FearAltarStageWaveMonsterArrange> m_monsterArranges = new Dictionary<int, FearAltarStageWaveMonsterArrange>();

	public int requiredConditionType => m_nRequiredConditionType;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int requiredMainQuestNo => m_nRequiredMainQuestNo;

	public int requiredStamina => m_nRequiredStamina;

	public int enterMinMemberCount => m_nEnterMinMemberCount;

	public int enterMaxMemberCount => m_nEnterMaxMemberCount;

	public int matchingWaitingTime => m_nMatchingWaitingTime;

	public int enterWaitingTime => m_nEnterWaitingTime;

	public int startDelayTime => m_nStartDelayTime;

	public int limitTime => m_nLimitTime;

	public int exitDelayTime => m_nExitDelayTime;

	public int safeRevivalWaitingTime => m_nSafeRevivalWaitingTime;

	public int halidomMonsterLifetime => m_nHalidomMonsterLifetime;

	public int halidomAcquisitionRate => m_nHalidomAcquisitionRate;

	public Dictionary<int, FearAltarHalidomLevel> halidomLevels => m_halidomLevels;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRequiredConditionType = Convert.ToInt32(dr["requiredConditionType"]);
		if (!IsDefinedRequiredConditionType(m_nRequiredConditionType))
		{
			SFLogUtil.Warn(GetType(), "필요조건타입이 유효하지 않습니다. m_nRequiredConditionType = " + m_nRequiredConditionType);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "필요영웅레벨이 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		m_nRequiredMainQuestNo = Convert.ToInt32(dr["requiredMainQuestNo"]);
		if (m_nRequiredMainQuestNo < 0)
		{
			SFLogUtil.Warn(GetType(), "필요메인퀘스트번호가 유효하지 않습니다. m_nRequiredMainQuestNo = " + m_nRequiredMainQuestNo);
		}
		m_nRequiredStamina = Convert.ToInt32(dr["requiredStamina"]);
		m_nEnterMinMemberCount = Convert.ToInt32(dr["enterMinMemberCount"]);
		m_nEnterMaxMemberCount = Convert.ToInt32(dr["enterMaxMemberCount"]);
		m_nMatchingWaitingTime = Convert.ToInt32(dr["matchingWaitingTime"]);
		m_nEnterWaitingTime = Convert.ToInt32(dr["enterWaitingTime"]);
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		m_nExitDelayTime = Convert.ToInt32(dr["exitDelayTime"]);
		m_nSafeRevivalWaitingTime = Convert.ToInt32(dr["safeRevivalWaitingTime"]);
		m_nHalidomMonsterLifetime = Convert.ToInt32(dr["halidomMonsterLifetime"]);
		m_nHalidomAcquisitionRate = Convert.ToInt32(dr["halidomAcquisitionRate"]);
	}

	public bool IsSafeRevivalWaitingTimeElapsed(float fElapsedTime)
	{
		return fElapsedTime >= (float)m_nSafeRevivalWaitingTime * 0.9f;
	}

	public void AddMonsterAttrFactor(FearAltarMonsterAttrFactor monsterAttrFactor)
	{
		if (monsterAttrFactor == null)
		{
			throw new ArgumentNullException("monsterAttrFactor");
		}
		m_monsterAttrFactors.Add(monsterAttrFactor.level, monsterAttrFactor);
	}

	public FearAltarMonsterAttrFactor GetMonsterAttrFactor(int nLevel)
	{
		if (!m_monsterAttrFactors.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddReward(FearAltarReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward.level, reward);
	}

	public FearAltarReward GetReward(int nLevel)
	{
		if (!m_rewards.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddHalidomCollectionReward(FearAltarHalidomCollectionReward halidomCollectionReward)
	{
		if (halidomCollectionReward == null)
		{
			throw new ArgumentNullException("halidomCollectionReward");
		}
		m_halidomCollectionRewards.Add(halidomCollectionReward);
	}

	public FearAltarHalidomCollectionReward GetHalidomCollectionReward(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_halidomCollectionRewards.Count)
		{
			return null;
		}
		return m_halidomCollectionRewards[nIndex];
	}

	public void AddHalidomElemental(FearAltarHalidomElemental halidomElemental)
	{
		if (halidomElemental == null)
		{
			throw new ArgumentNullException("halidomElemental");
		}
		m_halidomElementals.Add(halidomElemental.id, halidomElemental);
	}

	public FearAltarHalidomElemental GetHalidomElemental(int nId)
	{
		if (!m_halidomElementals.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddHalidomLevel(FearAltarHalidomLevel halidomLevel)
	{
		if (halidomLevel == null)
		{
			throw new ArgumentNullException("halidomLevel");
		}
		m_halidomLevels.Add(halidomLevel.level, halidomLevel);
	}

	public FearAltarHalidomLevel GetHalidomLevel(int nLevel)
	{
		if (!m_halidomLevels.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddHalidom(FearAltarHalidom halidom)
	{
		if (halidom == null)
		{
			throw new ArgumentNullException("halidom");
		}
		m_halidoms.Add(halidom.id, halidom);
		m_nHalidomTotalPoint += halidom.point;
	}

	public FearAltarHalidom GetHalidom(int nId)
	{
		if (!m_halidoms.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public FearAltarHalidom SelectHalidom()
	{
		return Util.SelectPickEntry(m_halidoms.Values, m_nHalidomTotalPoint);
	}

	public void AddStage(FearAltarStage stage)
	{
		if (stage == null)
		{
			throw new ArgumentNullException("stage");
		}
		m_stages.Add(stage.id, stage);
		m_nStageTotalPoint += stage.point;
	}

	public void AddMonsterArrange(FearAltarStageWaveMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange.key, monsterArrange);
	}

	public FearAltarStage GetStage(int nId)
	{
		if (!m_stages.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public FearAltarStage SelectStage()
	{
		return Util.SelectPickEntry(m_stages.Values, m_nStageTotalPoint);
	}

	public static bool IsDefinedRequiredConditionType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
