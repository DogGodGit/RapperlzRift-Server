using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class AncientRelic : Location
{
	public const int kStartYRotationType_Fixed = 1;

	public const int kStartYRotationType_Random = 2;

	public const float kSafetyRevivalWaitingTimeFactor = 0.9f;

	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

	private int m_nLocationId;

	private string m_sNameKey;

	private string m_sDescriptionKey;

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

	private int m_nWaveIntervalTime;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartRadius;

	private int m_nStartYRotationType;

	private float m_fStartYRotation;

	private int m_nSafeRevivalWaitingTime;

	private int m_nTrapActivateStartStep;

	private int m_nTrapPenaltyMoveSpeed;

	private int m_nTrapPenaltyDuration;

	private int m_nTrapDamage;

	private Rect3D m_mapRect = Rect3D.zero;

	private List<AncientRelicMonsterAttrFactor> m_monsterAttrFactors = new List<AncientRelicMonsterAttrFactor>();

	private List<AncientRelicRoute> m_routes = new List<AncientRelicRoute>();

	private int m_nRouteTotalPoint;

	private List<AncientRelicTrap> m_traps = new List<AncientRelicTrap>();

	private List<AncientRelicStep> m_steps = new List<AncientRelicStep>();

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.AncientRelic;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public string nameKey => m_sNameKey;

	public string descriptionKey => m_sDescriptionKey;

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

	public int waveIntervalTime => m_nWaveIntervalTime;

	public Vector3 startPosition => m_startPosition;

	public float startRadius => m_fStartRadius;

	public int startYRotationType => m_nStartYRotationType;

	public float startYRotation => m_fStartYRotation;

	public int safeRevivalWaitingTime => m_nSafeRevivalWaitingTime;

	public int trapActivateStartStep => m_nTrapActivateStartStep;

	public int trapPenaltyMoveSpeed => m_nTrapPenaltyMoveSpeed;

	public int trapPenaltyDuration => m_nTrapPenaltyDuration;

	public int trapDamage => m_nTrapDamage;

	public Rect3D mapRect => m_mapRect;

	public int stepCount => m_steps.Count;

	public List<AncientRelicTrap> traps => m_traps;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_sDescriptionKey = Convert.ToString(dr["descriptionKey"]);
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
		m_nWaveIntervalTime = Convert.ToInt32(dr["waveIntervalTime"]);
		m_startPosition.x = Convert.ToSingle(dr["startXPosition"]);
		m_startPosition.y = Convert.ToSingle(dr["startYPosition"]);
		m_startPosition.z = Convert.ToSingle(dr["startZPosition"]);
		m_fStartRadius = Convert.ToSingle(dr["startRadius"]);
		m_nStartYRotationType = Convert.ToInt32(dr["startYRotationType"]);
		if (!IsDefinedStartYRotationType(m_nStartYRotationType))
		{
			SFLogUtil.Warn(GetType(), "시작방향타입이 유효하지 않습니다. m_nStartYRotationType = " + m_nStartYRotationType);
		}
		m_fStartYRotation = Convert.ToSingle(dr["startYRotation"]);
		m_nSafeRevivalWaitingTime = Convert.ToInt32(dr["safeRevivalWaitingTime"]);
		m_nTrapActivateStartStep = Convert.ToInt32(dr["trapActivateStartStep"]);
		m_nTrapPenaltyMoveSpeed = Convert.ToInt32(dr["trapPenaltyMoveSpeed"]);
		m_nTrapPenaltyDuration = Convert.ToInt32(dr["trapPenaltyDuration"]);
		m_nTrapDamage = Convert.ToInt32(dr["trapDamage"]);
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public void AddMonsterAttrFactor(AncientRelicMonsterAttrFactor monsterAttrFactor)
	{
		if (monsterAttrFactor == null)
		{
			throw new ArgumentNullException("monsterAttrFactor");
		}
		m_monsterAttrFactors.Add(monsterAttrFactor);
	}

	public AncientRelicMonsterAttrFactor GetMonsterAttrFactor(int nAverageHeroLevel)
	{
		int nIndex = nAverageHeroLevel - 1;
		if (nIndex < 0 || nIndex >= m_monsterAttrFactors.Count)
		{
			return null;
		}
		return m_monsterAttrFactors[nIndex];
	}

	public void AddRoute(AncientRelicRoute route)
	{
		if (route == null)
		{
			throw new ArgumentNullException("route");
		}
		m_routes.Add(route);
		m_nRouteTotalPoint += route.point;
	}

	public AncientRelicRoute GetRoute(int nId)
	{
		int nIndex = nId - 1;
		if (nIndex < 0 || nIndex >= m_routes.Count)
		{
			return null;
		}
		return m_routes[nIndex];
	}

	public int SelectRouteId()
	{
		return SelectRoute().id;
	}

	private AncientRelicRoute SelectRoute()
	{
		return Util.SelectPickEntry(m_routes, m_nRouteTotalPoint);
	}

	public void AddTrap(AncientRelicTrap trap)
	{
		if (trap == null)
		{
			throw new ArgumentNullException("trap");
		}
		m_traps.Add(trap);
	}

	public AncientRelicTrap GetTrap(int nId)
	{
		int nIndex = nId - 1;
		if (nIndex < 0 || nIndex >= m_traps.Count)
		{
			return null;
		}
		return m_traps[nIndex];
	}

	public void AddStep(AncientRelicStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_steps.Add(step);
	}

	public AncientRelicStep GetStep(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_steps.Count)
		{
			return null;
		}
		return m_steps[nIndex];
	}

	public Vector3 SelectPosition()
	{
		return Util.SelectPoint(m_startPosition, m_fStartRadius);
	}

	public float SelectRotationY()
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
