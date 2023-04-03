using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class InfiniteWar : Location
{
	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

	public const float kSafetyRevivalWaitingTimeFactor = 0.9f;

	private int m_nLocation;

	private int m_nEnterCount;

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

	private int m_nMonsterSpawnDelayTime;

	private int m_nHeroKillPoint;

	private int m_nBuffBoxCreationInterval;

	private int m_nBuffBoxCreationCount;

	private Vector3 m_buffBoxPosition = Vector3.zero;

	private float m_fBuffBoxRadius;

	private int m_nBuffBoxLifetime;

	private int m_nBuffDuration;

	private float m_fBuffBoxAcquisitionRange;

	private int m_nSafeRevivalWaitingTime;

	private Rect3D m_mapRect = Rect3D.zero;

	private List<InfiniteWarBuffBox> m_buffBoxs = new List<InfiniteWarBuffBox>();

	private int m_nBuffBoxTotalPoint;

	private Dictionary<int, InfiniteWarMonsterAttrFactor> m_monsterAttrFactors = new Dictionary<int, InfiniteWarMonsterAttrFactor>();

	private List<InfiniteWarMonsterArrange> m_monsterArranges = new List<InfiniteWarMonsterArrange>();

	private List<InfiniteWarOpenSchedule> m_openSchedules = new List<InfiniteWarOpenSchedule>();

	private List<InfiniteWarStartPosition> m_startPositions = new List<InfiniteWarStartPosition>();

	private List<InfiniteWarReward> m_rewards = new List<InfiniteWarReward>();

	private Dictionary<int, List<InfiniteWarRankingReward>> m_rankingRewardsCollection = new Dictionary<int, List<InfiniteWarRankingReward>>();

	public override int locationId => m_nLocation;

	public override LocationType locationType => LocationType.InfiniteWar;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public int enterCount => m_nEnterCount;

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

	public int monsterSpawnDelayTime => m_nMonsterSpawnDelayTime;

	public int heroKillPoint => m_nHeroKillPoint;

	public int buffBoxCreationInterval => m_nBuffBoxCreationInterval;

	public int buffBoxCreationCount => m_nBuffBoxCreationCount;

	public Vector3 buffBoxPosition => m_buffBoxPosition;

	public float buffBoxRadius => m_fBuffBoxRadius;

	public int buffBoxLifetime => m_nBuffBoxLifetime;

	public int buffDuration => m_nBuffDuration;

	public float buffBoxAcquisitionRange => m_fBuffBoxAcquisitionRange;

	public int safeRevivalWaitingTime => m_nSafeRevivalWaitingTime;

	public Rect3D mapRect => m_mapRect;

	public List<InfiniteWarMonsterArrange> monsterArranges => m_monsterArranges;

	public List<InfiniteWarReward> rewards => m_rewards;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nLocation = Convert.ToInt32(dr["locationId"]);
		m_nEnterCount = Convert.ToInt32(dr["enterCount"]);
		if (m_nEnterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "입장횟수가 유효하지 않습니다. m_nEnterCount = " + m_nEnterCount);
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
		if (m_nRequiredStamina < 0)
		{
			SFLogUtil.Warn(GetType(), "필요스태미너가 유효하지 않습니다. m_nRequiredStamina = " + m_nRequiredStamina);
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
		if (m_nExitDelayTime < 0)
		{
			SFLogUtil.Warn(GetType(), "퇴장대기시간이 유효하지 않습니다. m_nExitDelayTime = " + m_nExitDelayTime);
		}
		m_nMonsterSpawnDelayTime = Convert.ToInt32(dr["monsterSpawnDelayTime"]);
		if (m_nMonsterSpawnDelayTime < 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터출몰지연시간이 유효하지 않습니다. m_nMonsterSpawnDelayTime = " + m_nMonsterSpawnDelayTime);
		}
		m_nHeroKillPoint = Convert.ToInt32(dr["heroKillPoint"]);
		if (m_nHeroKillPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "영웅처치점수가 유효하지 않습니다. m_nHeroKillPoint = " + m_nHeroKillPoint);
		}
		m_nBuffBoxCreationInterval = Convert.ToInt32(dr["buffBoxCreationInterval"]);
		if (m_nBuffBoxCreationInterval < 0)
		{
			SFLogUtil.Warn(GetType(), "버프상자생성간격이 유효하지 않습니다. m_nBuffBoxCreationInterval = " + m_nBuffBoxCreationInterval);
		}
		m_nBuffBoxCreationCount = Convert.ToInt32(dr["buffBoxCreationCount"]);
		if (m_nBuffBoxCreationCount < 0)
		{
			SFLogUtil.Warn(GetType(), "버프상자생성개수가 유효하지 않습니다. m_nBuffBoxCreationCount = " + m_nBuffBoxCreationCount);
		}
		m_buffBoxPosition.x = Convert.ToSingle(dr["buffBoxXPosition"]);
		m_buffBoxPosition.y = Convert.ToSingle(dr["buffBoxYPosition"]);
		m_buffBoxPosition.z = Convert.ToSingle(dr["buffBoxZPosition"]);
		m_fBuffBoxRadius = Convert.ToSingle(dr["buffBoxRadius"]);
		if (m_fBuffBoxRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "버프상자반지름이 유효하지 않습니다. m_fBuffBoxRadius = " + m_fBuffBoxRadius);
		}
		m_nBuffBoxLifetime = Convert.ToInt32(dr["buffBoxLifetime"]);
		if (m_nBuffBoxLifetime <= 0)
		{
			SFLogUtil.Warn(GetType(), "버프상자유지기간이 유효하지 않습니다. m_nBuffBoxLifetime = " + m_nBuffBoxLifetime);
		}
		m_nBuffDuration = Convert.ToInt32(dr["buffDuration"]);
		if (m_nBuffDuration <= 0)
		{
			SFLogUtil.Warn(GetType(), "버프유지기간이 유효하지 않습니다. m_nBuffDuration = " + m_nBuffDuration);
		}
		m_fBuffBoxAcquisitionRange = Convert.ToSingle(dr["buffBoxAcquisitionRange"]);
		if (m_fBuffBoxAcquisitionRange < 0f)
		{
			SFLogUtil.Warn(GetType(), "버프상자획득범위가 유효하지 않습니다. m_fBuffBoxAcquisitionRange = " + m_fBuffBoxAcquisitionRange);
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

	public void AddBuffBox(InfiniteWarBuffBox buffBox)
	{
		if (buffBox == null)
		{
			throw new ArgumentNullException("buffBox");
		}
		m_buffBoxs.Add(buffBox);
		m_nBuffBoxTotalPoint += buffBox.point;
	}

	public InfiniteWarBuffBox SelectBuffBox()
	{
		return Util.SelectPickEntry(m_buffBoxs, m_nBuffBoxTotalPoint);
	}

	public Vector3 SelectBuffBoxPosition()
	{
		return Util.SelectPoint(m_buffBoxPosition, m_fBuffBoxRadius);
	}

	public void AddMonsterAttrFactor(InfiniteWarMonsterAttrFactor monsterAttrFactor)
	{
		if (monsterAttrFactor == null)
		{
			throw new ArgumentNullException("monsterAttrFactor");
		}
		m_monsterAttrFactors.Add(monsterAttrFactor.level, monsterAttrFactor);
	}

	public InfiniteWarMonsterAttrFactor GetMonsterAttrFactor(int nLevel)
	{
		if (!m_monsterAttrFactors.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddMonsterArrange(InfiniteWarMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange);
	}

	public void AddOpenSchedule(InfiniteWarOpenSchedule openSchedule)
	{
		if (openSchedule == null)
		{
			throw new ArgumentNullException("openSchedule");
		}
		m_openSchedules.Add(openSchedule);
	}

	public InfiniteWarOpenSchedule GetEnterableOpenSchedule(DateTimeOffset time)
	{
		int nTime = (int)time.TimeOfDay.TotalSeconds;
		foreach (InfiniteWarOpenSchedule openSchedule in m_openSchedules)
		{
			if (openSchedule.IsEnterable(nTime))
			{
				return openSchedule;
			}
		}
		return null;
	}

	public void AddStartPosition(InfiniteWarStartPosition startPosition)
	{
		if (startPosition == null)
		{
			throw new ArgumentNullException("startPosition");
		}
		m_startPositions.Add(startPosition);
	}

	public InfiniteWarStartPosition GetStartPosition(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_startPositions.Count)
		{
			return null;
		}
		return m_startPositions[nIndex];
	}

	public void AddReward(InfiniteWarReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public void AddRankingReward(InfiniteWarRankingReward rankingReward)
	{
		if (rankingReward == null)
		{
			throw new ArgumentNullException("rankingReward");
		}
		GetOrCreateRankingRewards(rankingReward.ranking).Add(rankingReward);
	}

	public List<InfiniteWarRankingReward> GetRankingRewards(int nRanking)
	{
		if (!m_rankingRewardsCollection.TryGetValue(nRanking, out var value))
		{
			return null;
		}
		return value;
	}

	private List<InfiniteWarRankingReward> GetOrCreateRankingRewards(int nRanking)
	{
		List<InfiniteWarRankingReward> rankingRewards = GetRankingRewards(nRanking);
		if (rankingRewards == null)
		{
			rankingRewards = new List<InfiniteWarRankingReward>();
			m_rankingRewardsCollection.Add(nRanking, rankingRewards);
		}
		return rankingRewards;
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
