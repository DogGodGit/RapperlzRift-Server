using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AnkouTomb : Location
{
	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

	public const int kStartYRotationType_Fixed = 1;

	public const int kStartYRotationType_Random = 2;

	public const int kAdditionalRewardExpType_2x = 1;

	public const int kAdditionalRewardExpType_3x = 2;

	public const float kSafetyRevivalWaitingTimeFactor = 0.9f;

	private int m_nLocationId;

	private int m_nRequiredConditionType;

	private int m_nRequiredHeroLevel;

	private int m_nRequiredMainQuestNo;

	private int m_nRequiredStamina;

	private int m_nEnterCount;

	private int m_nEnterMinMemberCount;

	private int m_nEnterMaxMemberCount;

	private int m_nWaveCount;

	private int m_nMatchingWaitingTime;

	private int m_nEnterWaitingTime;

	private int m_nStartDelayTime;

	private int m_nLimitTime;

	private int m_nExitDelayTime;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartRadius;

	private int m_nStartYRotationType;

	private float m_fStartYRotation;

	private Vector3 m_monsterSpawnPosition = Vector3.zero;

	private float m_fMonsterSpawnRadius;

	private int m_nBossMonsterSpawnRate;

	private int m_nMonsterPoint;

	private int m_nBossMonsterPoint;

	private int m_nClearPoint;

	private int m_nExp2xRewardRequiredUnOwnDia;

	private int m_nExp3xRewardRequiredUnOwnDia;

	private int m_nSafeRevivalWaitingTime;

	private Rect3D m_mapRect = Rect3D.zero;

	private List<AnkouTombSchedule> m_schedules = new List<AnkouTombSchedule>();

	private List<AnkouTombDifficulty> m_difficulties = new List<AnkouTombDifficulty>();

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.AnkouTomb;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public int requiredConditionType => m_nRequiredConditionType;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int requiredMainQuestNo => m_nRequiredMainQuestNo;

	public int requiredStamina => m_nRequiredStamina;

	public int enterCount => m_nEnterCount;

	public int enterMinMemberCount => m_nEnterMinMemberCount;

	public int enterMaxMemberCount => m_nEnterMaxMemberCount;

	public int waveCount => m_nWaveCount;

	public int matchingWaitingTime => m_nMatchingWaitingTime;

	public int enterWaitingTime => m_nEnterWaitingTime;

	public int startDelayTime => m_nStartDelayTime;

	public int limitTime => m_nLimitTime;

	public int exitDelayTime => m_nExitDelayTime;

	public Vector3 startPosition => m_startPosition;

	public float startRadius => m_fStartRadius;

	public int startYRotationType => m_nStartYRotationType;

	public float startYRotation => m_fStartYRotation;

	public Vector3 monsterSpawnPosition => m_monsterSpawnPosition;

	public float monsterSpawnRadius => m_fMonsterSpawnRadius;

	public int bossMonsterSpawnRate => m_nBossMonsterSpawnRate;

	public int monsterPoint => m_nMonsterPoint;

	public int bossMonsterPoint => m_nBossMonsterPoint;

	public int clearPoint => m_nClearPoint;

	public int exp2xRewardRequiredUnOwnDia => m_nExp2xRewardRequiredUnOwnDia;

	public int exp3xRewardRequiredUnOwnDia => m_nExp3xRewardRequiredUnOwnDia;

	public int safeRevivalWaitingTime => m_nSafeRevivalWaitingTime;

	public Rect3D mapRect => m_mapRect;

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
		m_nRequiredStamina = Convert.ToInt32(dr["requiredStamina"]);
		if (m_nRequiredStamina < 0)
		{
			SFLogUtil.Warn(GetType(), "필요스테미너가 유효하지 않습니다. m_nRequiredStamina = " + m_nRequiredStamina);
		}
		m_nEnterCount = Convert.ToInt32(dr["enterCount"]);
		if (m_nEnterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "입장횟수가 유효하지 않습니다. m_nEnterCount = " + m_nEnterCount);
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
		m_nWaveCount = Convert.ToInt32(dr["waveCount"]);
		if (m_nWaveCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "웨이브수가 유효하지 않습니다. m_nWaveCount = " + m_nWaveCount);
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
		if (m_nStartDelayTime < 0)
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
		m_startPosition.x = Convert.ToSingle(dr["startXPosition"]);
		m_startPosition.y = Convert.ToSingle(dr["startYPosition"]);
		m_startPosition.z = Convert.ToSingle(dr["startZPosition"]);
		m_fStartRadius = Convert.ToSingle(dr["startRadius"]);
		if (m_fStartRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "시작반지름이 유효하지 않습니다. m_fStartRadius = " + m_fStartRadius);
		}
		m_nStartYRotationType = Convert.ToInt32(dr["startYRotationType"]);
		if (!IsDefinedStartYRotationType(m_nStartYRotationType))
		{
			SFLogUtil.Warn(GetType(), "시작방향타입이 유효하지 않습니다. m_nStartYRotationType = " + m_nStartYRotationType);
		}
		m_fStartYRotation = Convert.ToSingle(dr["startYRotation"]);
		m_monsterSpawnPosition.x = Convert.ToSingle(dr["monsterSpawnXPosition"]);
		m_monsterSpawnPosition.y = Convert.ToSingle(dr["monsterSpawnYPosition"]);
		m_monsterSpawnPosition.z = Convert.ToSingle(dr["monsterSpawnZPosition"]);
		m_fMonsterSpawnRadius = Convert.ToSingle(dr["monsterSpawnRadius"]);
		if (m_fMonsterSpawnRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "몬스터출몰반지름이 유효하지 않습니다. m_fMonsterSpawnRadius = " + m_fMonsterSpawnRadius);
		}
		m_nBossMonsterSpawnRate = Convert.ToInt32(dr["bossMonsterSpawnRate"]);
		if (m_nBossMonsterSpawnRate < 0)
		{
			SFLogUtil.Warn(GetType(), "보스몬스터출몰확률이 유효하지 않습니다. m_nBossMonsterSpawnRate = " + m_nBossMonsterSpawnRate);
		}
		m_nMonsterPoint = Convert.ToInt32(dr["monsterPoint"]);
		if (m_nMonsterPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "일반몬스터점수가 유효하지 않습니다. m_nMonsterPoint = " + m_nMonsterPoint);
		}
		m_nBossMonsterPoint = Convert.ToInt32(dr["bossMonsterPoint"]);
		if (m_nBossMonsterPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "보스몬스터점수가 유효하지 않습니다. m_nBossMonsterPoint = " + m_nBossMonsterPoint);
		}
		m_nClearPoint = Convert.ToInt32(dr["clearPoint"]);
		if (m_nClearPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "클리어점수가 유효하지 않습니다. m_nClearPoint = " + m_nClearPoint);
		}
		m_nExp2xRewardRequiredUnOwnDia = Convert.ToInt32(dr["exp2xRewardRequiredUnOwnDia"]);
		if (m_nExp2xRewardRequiredUnOwnDia < 0)
		{
			SFLogUtil.Warn(GetType(), "경험치2배보상비귀속다이아가 유효하지 않습니다. m_nExp2xRewardRequiredUnOwnDia = " + m_nExp2xRewardRequiredUnOwnDia);
		}
		m_nExp3xRewardRequiredUnOwnDia = Convert.ToInt32(dr["exp3xRewardRequiredUnOwnDia"]);
		if (m_nExp3xRewardRequiredUnOwnDia < 0)
		{
			SFLogUtil.Warn(GetType(), "경험치3배보상비귀속다이아가 유효하지 않습니다. m_nExp3xRewardRequiredUnOwnDia = " + m_nExp3xRewardRequiredUnOwnDia);
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

	public Vector3 SelectStartPosition()
	{
		return Util.SelectPoint(m_startPosition, m_fStartRadius);
	}

	public float SelectStartRotationY()
	{
		if (m_nStartYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fStartYRotation);
		}
		return m_fStartYRotation;
	}

	public Vector3 SelectMonsterSpawnPosition()
	{
		return Util.SelectPoint(m_monsterSpawnPosition, m_fMonsterSpawnRadius);
	}

	public bool IsSafeRevivalWaitingTimeElapsed(float fElapsedTime)
	{
		return fElapsedTime >= (float)m_nSafeRevivalWaitingTime * 0.9f;
	}

	public void AddSchedule(AnkouTombSchedule schedule)
	{
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_schedules.Add(schedule);
	}

	public AnkouTombSchedule GetEnterableSchedule(DateTimeOffset time)
	{
		int nTime = (int)time.TimeOfDay.TotalSeconds;
		foreach (AnkouTombSchedule schedule in m_schedules)
		{
			if (schedule.IsEnterable(nTime))
			{
				return schedule;
			}
		}
		return null;
	}

	public void AddDifficulty(AnkouTombDifficulty difficulty)
	{
		if (difficulty == null)
		{
			throw new ArgumentNullException("difficulty");
		}
		m_difficulties.Add(difficulty);
	}

	public AnkouTombDifficulty GetDifficulty(int nDifficulty)
	{
		int nIndex = nDifficulty - 1;
		if (nIndex < 0 || nIndex >= m_difficulties.Count)
		{
			return null;
		}
		return m_difficulties[nIndex];
	}

	public static bool IsDefinedRequiredConditionType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}

	public static bool IsDefinedStartYRotationType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}

	public static bool IsDefinedAdditionalRewardExpType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
