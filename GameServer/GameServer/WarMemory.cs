using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WarMemory : Location
{
	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

	public const float kSafetyRevivalWaitingTimeFactor = 0.9f;

	private int m_nLocationId;

	private int m_nRequiredConditionType;

	private int m_nRequiredHeroLevel;

	private int m_nRequiredMainQuestNo;

	private int m_nFreeEnterCount;

	private int m_nEnterRequiredItemId;

	private int m_nEnterMinMemberCount;

	private int m_nEnterMaxMemberCount;

	private int m_nMatchingWaitingTime;

	private int m_nEnterWaitingTime;

	private int m_nStartDelayTime;

	private int m_nLimitTime;

	private int m_nExitDelayTime;

	private int m_nSafeRevivalWaitingTime;

	private Rect3D m_mapRect = Rect3D.zero;

	private Dictionary<int, WarMemoryMonsterAttrFactor> m_monsterAttrFactors = new Dictionary<int, WarMemoryMonsterAttrFactor>();

	private List<WarMemoryStartPosition> m_startPositions = new List<WarMemoryStartPosition>();

	private List<WarMemorySchedule> m_schedules = new List<WarMemorySchedule>();

	private Dictionary<int, WarMemoryReward> m_rewards = new Dictionary<int, WarMemoryReward>();

	private List<WarMemoryRankingReward> m_rankingRewards = new List<WarMemoryRankingReward>();

	private List<WarMemoryWave> m_waves = new List<WarMemoryWave>();

	private Dictionary<int, WarMemoryMonsterArrange> m_monsterArranges = new Dictionary<int, WarMemoryMonsterArrange>();

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.WarMemory;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public int requiredConditionType => m_nRequiredConditionType;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int requiredMainQuestNo => m_nRequiredMainQuestNo;

	public int freeEnterCount => m_nFreeEnterCount;

	public int enterRequiredItemId => m_nEnterRequiredItemId;

	public int enterMinMemberCount => m_nEnterMinMemberCount;

	public int enterMaxMemberCount => m_nEnterMaxMemberCount;

	public int matchingWaitingTime => m_nMatchingWaitingTime;

	public int enterWaitingTime => m_nEnterWaitingTime;

	public int startDelayTime => m_nStartDelayTime;

	public int limitTime => m_nLimitTime;

	public int exitDelayTime => m_nExitDelayTime;

	public int safeRevivalWaitingTime => m_nSafeRevivalWaitingTime;

	public Rect3D mapRect => m_mapRect;

	public int startPositionCount => m_startPositions.Count;

	public int waveCount => m_waves.Count;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
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
		m_nFreeEnterCount = Convert.ToInt32(dr["freeEnterCount"]);
		if (m_nFreeEnterCount < 0)
		{
			SFLogUtil.Warn(GetType(), "무료입장횟수가 유효하지 않습니다. m_nFreeEnterCount = " + m_nFreeEnterCount);
		}
		m_nEnterRequiredItemId = Convert.ToInt32(dr["enterRequiredItemId"]);
		if (m_nEnterRequiredItemId > 0)
		{
			if (Resource.instance.GetItem(m_nEnterRequiredItemId) == null)
			{
				SFLogUtil.Warn(GetType(), "입장필요아이템이 존재하지 않습니다. m_nEnterRequiredItemId = " + m_nEnterRequiredItemId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "입장필요아이템ID가 유효하지 않습니다. m_nEnterRequiredItemId = " + m_nEnterRequiredItemId);
		}
		m_nEnterMinMemberCount = Convert.ToInt32(dr["enterMinMemberCount"]);
		if (m_nEnterMinMemberCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "입장최소인원이 유효하지 않습니다. m_nEnterMinMemberCount = " + m_nEnterMinMemberCount);
		}
		m_nEnterMaxMemberCount = Convert.ToInt32(dr["enterMaxMemberCount"]);
		if (m_nEnterMaxMemberCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "입장최대인원이 유효하지 않습니다. m_nEnterMaxMemberCount = " + m_nEnterMaxMemberCount);
		}
		if (m_nEnterMinMemberCount > m_nEnterMaxMemberCount)
		{
			SFLogUtil.Warn(GetType(), "입장최소인원이 입장최대인원보다 많습니다. m_nEnterMinMemberCount = " + m_nEnterMinMemberCount + ", m_nEnterMaxMemberCount = " + m_nEnterMaxMemberCount);
		}
		m_nMatchingWaitingTime = Convert.ToInt32(dr["matchingWaitingTime"]);
		if (m_nMatchingWaitingTime < 0)
		{
			SFLogUtil.Warn(GetType(), "매칭대기시간이 유효하지 않습니다. m_nMatchingWaitingTime = " + m_nMatchingWaitingTime);
		}
		m_nEnterWaitingTime = Convert.ToInt32(dr["enterWaitingTime"]);
		if (m_nEnterWaitingTime < 0)
		{
			SFLogUtil.Warn(GetType(), "입장대기시간이 유효하지 않습니다. m_nEnterWaitingTime = " + m_nEnterWaitingTime);
		}
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		if (m_nStartDelayTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "시작대기시간이 유효하지 않습니다. m_nStartDelayTime = " + m_nStartDelayTime);
		}
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		if (m_nLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한시간이 유효하지 않습니다. m_nLimitTime = " + m_nLimitTime);
		}
		m_nExitDelayTime = Convert.ToInt32(dr["exitDelayTime"]);
		if (m_nExitDelayTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "퇴장대기시간 유효하지 않습니다. m_nExitDelayTime = " + m_nExitDelayTime);
		}
		m_nSafeRevivalWaitingTime = Convert.ToInt32(dr["safeRevivalWaitingTime"]);
		if (m_nSafeRevivalWaitingTime < 0)
		{
			SFLogUtil.Warn(GetType(), "안전부활대기시간이 유효하지 않습니다. m_nSafeRevivalWaitingTime = " + m_nSafeRevivalWaitingTime);
		}
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public bool IsSafeRevivalWaitingTimeElapsed(float fElapsedTime)
	{
		return fElapsedTime >= (float)m_nSafeRevivalWaitingTime * 0.9f;
	}

	public void AddMonsterAttrFactor(WarMemoryMonsterAttrFactor monsterAttrFactor)
	{
		if (monsterAttrFactor == null)
		{
			throw new ArgumentNullException("monsterAttrFactor");
		}
		m_monsterAttrFactors.Add(monsterAttrFactor.level, monsterAttrFactor);
	}

	public WarMemoryMonsterAttrFactor GetMonsterAttrFactor(int nLevel)
	{
		if (!m_monsterAttrFactors.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddStartPosition(WarMemoryStartPosition startPosition)
	{
		if (startPosition == null)
		{
			throw new ArgumentNullException("startPosition");
		}
		m_startPositions.Add(startPosition);
	}

	public WarMemoryStartPosition GetStartPosition_ByIndex(int nIndex)
	{
		if (nIndex < 0 || nIndex >= startPositionCount)
		{
			return null;
		}
		return m_startPositions[nIndex];
	}

	public void AddSchedule(WarMemorySchedule schedule)
	{
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_schedules.Add(schedule);
	}

	public WarMemorySchedule GetEnterableSchedule(DateTimeOffset time)
	{
		int nTime = (int)time.TimeOfDay.TotalSeconds;
		foreach (WarMemorySchedule schedule in m_schedules)
		{
			if (schedule.IsEnterable(nTime))
			{
				return schedule;
			}
		}
		return null;
	}

	public void AddReward(WarMemoryReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward.level, reward);
	}

	public WarMemoryReward GetReward(int nLevel)
	{
		if (!m_rewards.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddRankingReward(WarMemoryRankingReward rankingReward)
	{
		if (rankingReward == null)
		{
			throw new ArgumentNullException("rankingReweard");
		}
		m_rankingRewards.Add(rankingReward);
	}

	public List<WarMemoryRankingReward> GetRankingRewards(int nRanking)
	{
		List<WarMemoryRankingReward> results = new List<WarMemoryRankingReward>();
		foreach (WarMemoryRankingReward rankingReward in m_rankingRewards)
		{
			if (nRanking >= rankingReward.highRanking && nRanking <= rankingReward.lowRanking)
			{
				results.Add(rankingReward);
			}
		}
		return results;
	}

	public void AddWave(WarMemoryWave wave)
	{
		if (wave == null)
		{
			throw new ArgumentNullException("wave");
		}
		m_waves.Add(wave);
	}

	public WarMemoryWave GetWave(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= waveCount)
		{
			return null;
		}
		return m_waves[nIndex];
	}

	public void AddMonsterArrange(WarMemoryMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange.key, monsterArrange);
	}

	public WarMemoryMonsterArrange GetMonsterArrange(int nKey)
	{
		if (!m_monsterArranges.TryGetValue(nKey, out var value))
		{
			return null;
		}
		return value;
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
