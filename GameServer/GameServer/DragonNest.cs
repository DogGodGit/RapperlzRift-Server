using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DragonNest : Location
{
	public const int kStartYRotationType_Fixed = 1;

	public const int kStartYRotationType_Random = 2;

	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

	public const float kSafetyRevivalWaitingTimeFactor = 0.9f;

	private int m_nLocationId;

	private int m_nRequiredConditionType;

	private int m_nRequiredHeroLevel;

	private int m_nRequiredMainQuestNo;

	private int m_nEnterRequiredItemId;

	private int m_nEnterMinMemberCount;

	private int m_nEnterMaxMemberCount;

	private int m_nMatchingWaitingTime;

	private int m_nEnterWaitingTime;

	private int m_nStartDelayTime;

	private int m_nLimitTime;

	private int m_nExitDelayTime;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartRadius;

	private int m_nStartYRotationType;

	private float m_fStartYRotation;

	private int m_nSafeRevivalWaitingTime;

	private int m_nBaseMaxStep;

	private int m_nAdditionalStepOpenRate;

	private int m_nTrapPenaltyMoveSpeed;

	private int m_nTrapPenaltyDuration;

	private Rect3D m_mapRect = Rect3D.zero;

	private Dictionary<int, DragonNestMonsterAttrFactor> m_monsterAttrFactors = new Dictionary<int, DragonNestMonsterAttrFactor>();

	private List<DragonNestTrap> m_traps = new List<DragonNestTrap>();

	private List<DragonNestStep> m_steps = new List<DragonNestStep>();

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.DragonNest;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public int requiredConditionType => m_nRequiredConditionType;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int requiredMainQuestNo => m_nRequiredMainQuestNo;

	public int enterRequiredItemId => m_nEnterRequiredItemId;

	public int enterMinMemberCount => m_nEnterMinMemberCount;

	public int enterMaxMemberCount => m_nEnterMaxMemberCount;

	public int matchingWaitingTime => m_nMatchingWaitingTime;

	public int enterWaitingTime => m_nEnterWaitingTime;

	public int startDelayTime => m_nStartDelayTime;

	public int limitTime => m_nLimitTime;

	public int exitDelayTime => m_nExitDelayTime;

	public Vector3 startPosition => m_startPosition;

	public float startRadius => m_fStartRadius;

	public int startYRotationType => m_nStartYRotationType;

	public float startYRotation => m_fStartYRotation;

	public int safeRevivalWaitingTime => m_nSafeRevivalWaitingTime;

	public int baseMaxStep => m_nBaseMaxStep;

	public int additionalStepOpenRate => m_nAdditionalStepOpenRate;

	public int trapPenaltyMoveSpeed => m_nTrapPenaltyMoveSpeed;

	public int trapPenaltyDuration => m_nTrapPenaltyDuration;

	public Rect3D mapRect => m_mapRect;

	public List<DragonNestTrap> traps => m_traps;

	public int stepCount => m_steps.Count;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		Resource res = Resource.instance;
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
		m_nEnterRequiredItemId = Convert.ToInt32(dr["enterRequiredItemId"]);
		if (m_nEnterRequiredItemId > 0)
		{
			if (res.GetItem(m_nEnterRequiredItemId) == null)
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
		if (m_nExitDelayTime < 0)
		{
			SFLogUtil.Warn(GetType(), "퇴장대기시간이 유효하지 않습니다. m_nExitDelayTime = " + m_nExitDelayTime);
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
		m_nSafeRevivalWaitingTime = Convert.ToInt32(dr["safeRevivalWaitingTime"]);
		if (m_nSafeRevivalWaitingTime < 0)
		{
			SFLogUtil.Warn(GetType(), "안전부활대기시간이 유효하지 않습니다. m_nSafeRevivalWaitingTime = " + m_nSafeRevivalWaitingTime);
		}
		m_nBaseMaxStep = Convert.ToInt32(dr["baseMaxStep"]);
		if (m_nBaseMaxStep <= 0)
		{
			SFLogUtil.Warn(GetType(), "기본최대단계가 유효하지 않습니다. m_nBaseMaxStep = " + m_nBaseMaxStep);
		}
		m_nAdditionalStepOpenRate = Convert.ToInt32(dr["additionalStepOpenRate"]);
		if (m_nAdditionalStepOpenRate < 0)
		{
			SFLogUtil.Warn(GetType(), "추가단계개방확률이 유효하지 않습니다.");
		}
		m_nTrapPenaltyMoveSpeed = Convert.ToInt32(dr["trapPenaltyMoveSpeed"]);
		m_nTrapPenaltyDuration = Convert.ToInt32(dr["trapPenaltyDuration"]);
		if (m_nTrapPenaltyDuration <= 0)
		{
			SFLogUtil.Warn(GetType(), "함정패널티유지시간이 유효하지 않습니다. m_nTrapPenaltyDuration = " + m_nTrapPenaltyDuration);
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

	public bool IsSafeRevivalWaitingTimeElapsed(float fElapsedTime)
	{
		return fElapsedTime >= (float)m_nSafeRevivalWaitingTime * 0.9f;
	}

	public void AddMonsterAttrFactor(DragonNestMonsterAttrFactor monsterAttrFactor)
	{
		if (monsterAttrFactor == null)
		{
			throw new ArgumentNullException("monsterAttrFactor");
		}
		m_monsterAttrFactors.Add(monsterAttrFactor.level, monsterAttrFactor);
	}

	public DragonNestMonsterAttrFactor GetMonsterAttrFactor(int nLevel)
	{
		if (!m_monsterAttrFactors.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddTrap(DragonNestTrap trap)
	{
		if (trap == null)
		{
			throw new ArgumentNullException("trap");
		}
		m_traps.Add(trap);
	}

	public void AddStep(DragonNestStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_steps.Add(step);
	}

	public DragonNestStep GetStep(int nStepNo)
	{
		int nIndex = nStepNo - 1;
		if (nIndex < 0 || nIndex >= stepCount)
		{
			return null;
		}
		return m_steps[nIndex];
	}

	public static bool IsDefinedStartYRotationType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
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
