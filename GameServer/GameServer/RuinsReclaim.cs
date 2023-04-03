using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RuinsReclaim : Location
{
	public const int kStartYRotationType_Fixed = 1;

	public const int kStartYRotationTYpe_Random = 2;

	public const float kSafetyRevivalWaitingTimeFactor = 0.9f;

	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

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

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartRadius;

	private int m_nStartYRotationType;

	private float m_fStartYRotation;

	private int m_nDebuffAreaActivationStepNo;

	private int m_nDebuffAreaDeactivationStepNo;

	private Vector3 m_debuffAreaPosition = Vector3.zero;

	private float m_fDebuffAreaYRotation;

	private int m_nDebuffAreaWidth;

	private int m_nDebuffAreaHeight;

	private float m_fDebuffAreaOffenseFactor;

	private int m_nSummonMonsterHpRecoveryInterval;

	private int m_nSafeRevivalWaitingTime;

	private Rect3D m_mapRect = Rect3D.zero;

	private Dictionary<int, RuinsReclaimMonsterAttrFactor> m_monsterAttrFactors = new Dictionary<int, RuinsReclaimMonsterAttrFactor>();

	private Dictionary<int, RuinsReclaimRevivalPoint> m_revivalPoints = new Dictionary<int, RuinsReclaimRevivalPoint>();

	private List<RuinsReclaimTrap> m_traps = new List<RuinsReclaimTrap>();

	private Dictionary<int, RuinsReclaimPortal> m_portals = new Dictionary<int, RuinsReclaimPortal>();

	private List<RuinsReclaimOpenSchedule> m_openSchedules = new List<RuinsReclaimOpenSchedule>();

	private List<RuinsReclaimStep> m_steps = new List<RuinsReclaimStep>();

	private Dictionary<int, RuinsReclaimMonsterArrange> m_monsterArranges = new Dictionary<int, RuinsReclaimMonsterArrange>();

	private int m_nLastBossArrangeKey;

	private List<RuinsReclaimRandomRewardPoolEntry> m_randomRewardPoolEntries = new List<RuinsReclaimRandomRewardPoolEntry>();

	private int m_nRandomRewardTotalPoint;

	private List<RuinsReclaimReward> m_rewards = new List<RuinsReclaimReward>();

	private List<RuinsReclaimMonsterTerminatorRewardPoolEntry> m_monsterTerminatorRewardPoolEntries = new List<RuinsReclaimMonsterTerminatorRewardPoolEntry>();

	private int m_nMonsterTerminatorRewardTotalPoint;

	private List<RuinsReclaimUltimateAttackKingRewardPoolEntry> m_ultimateAttackKingRewardPoolEntries = new List<RuinsReclaimUltimateAttackKingRewardPoolEntry>();

	private int m_nUltimateAttackKingRewardTotalPoint;

	private List<RuinsReclaimPartyVolunteerRewardPoolEntry> m_partyVolunteerRewardPoolEntries = new List<RuinsReclaimPartyVolunteerRewardPoolEntry>();

	private int m_nPartyVolunteerRewardTotalPoint;

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.RuinsReclaim;

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

	public Vector3 startPosition => m_startPosition;

	public float startRadius => m_fStartRadius;

	public int startYRotationType => m_nStartYRotationType;

	public float startYRotation => m_fStartYRotation;

	public int debuffAreaActivationStepNo => m_nDebuffAreaActivationStepNo;

	public int debuffAreaDeactivationStepNo => m_nDebuffAreaDeactivationStepNo;

	public Vector3 debuffAreaPosition => m_debuffAreaPosition;

	public float debuffAreaYRotation => m_fDebuffAreaYRotation;

	public int debuffAreaWidth => m_nDebuffAreaWidth;

	public int debuffAreaHeight => m_nDebuffAreaHeight;

	public float debuffAreaOffenseFactor => m_fDebuffAreaOffenseFactor;

	public int summonMonsterHpRecoveryInterval => m_nSummonMonsterHpRecoveryInterval;

	public int safeRevivalWaitingTime => m_nSafeRevivalWaitingTime;

	public Rect3D mapRect => m_mapRect;

	public List<RuinsReclaimTrap> traps => m_traps;

	public int stepCount => m_steps.Count;

	public int lastBossArrangeKey => m_nLastBossArrangeKey;

	public List<RuinsReclaimReward> rewards => m_rewards;

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
		if (m_nExitDelayTime < 0)
		{
			SFLogUtil.Warn(GetType(), "퇴장대기시간이 유효하지 않습니다. m_nExitDelayTime = " + m_nExitDelayTime);
		}
		m_startPosition.x = Convert.ToSingle(dr["startXPosition"]);
		m_startPosition.y = Convert.ToSingle(dr["startYPosition"]);
		m_startPosition.z = Convert.ToSingle(dr["startZPosition"]);
		m_fStartRadius = Convert.ToSingle(dr["startRadius"]);
		m_nStartYRotationType = Convert.ToInt32(dr["startYRotationType"]);
		if (!IsDefinedStartYRotationType(m_nStartYRotationType))
		{
			SFLogUtil.Warn(GetType(), "시작방향타입이 유효하지 않습니다. m_nStartYRotationType = " + m_nStartYRotationType);
		}
		m_fStartYRotation = Convert.ToInt32(dr["startYRotation"]);
		m_nDebuffAreaActivationStepNo = Convert.ToInt32(dr["debuffAreaActivationStepNo"]);
		if (m_nDebuffAreaActivationStepNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "디버프지역활성화단계번호가 유효하지 않습니다. m_nDebuffAreaActivationStepNo = " + m_nDebuffAreaActivationStepNo);
		}
		m_nDebuffAreaDeactivationStepNo = Convert.ToInt32(dr["debuffAreaDeactivationStepNo"]);
		if (m_nDebuffAreaDeactivationStepNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "디버프지역비활성화단계번호가 유효하지 않습니다. m_nDebuffAreaDeactivationStepNo = " + m_nDebuffAreaDeactivationStepNo);
		}
		if (m_nDebuffAreaActivationStepNo >= m_nDebuffAreaDeactivationStepNo)
		{
			SFLogUtil.Warn(GetType(), "디버프지역활성화단계번호가 디버프지역비활성화단계번호보다 크거나 같습니다. m_nDebuffAreaActivationStepNo = " + m_nDebuffAreaActivationStepNo + ", m_nDebuffAreaDeactivationStepNo = " + m_nDebuffAreaDeactivationStepNo);
		}
		m_debuffAreaPosition.x = Convert.ToSingle(dr["debuffAreaXPosition"]);
		m_debuffAreaPosition.y = Convert.ToSingle(dr["debuffAreaYPosition"]);
		m_debuffAreaPosition.z = Convert.ToSingle(dr["debuffAreaZPosition"]);
		m_fDebuffAreaYRotation = Convert.ToSingle(dr["debuffAreaYRotation"]);
		m_nDebuffAreaWidth = Convert.ToInt32(dr["debuffAreaWidth"]);
		if (m_nDebuffAreaWidth <= 0)
		{
			SFLogUtil.Warn(GetType(), "디버프지역가로가 유효하지 않습니다. m_nDebuffAreaWidth = " + m_nDebuffAreaWidth);
		}
		m_nDebuffAreaHeight = Convert.ToInt32(dr["debuffAreaHeight"]);
		if (m_nDebuffAreaHeight <= 0)
		{
			SFLogUtil.Warn(GetType(), "디버프지역세로가 유효하지 않습니다. m_nDebuffAreaHeight = " + m_nDebuffAreaHeight);
		}
		m_fDebuffAreaOffenseFactor = Convert.ToSingle(dr["debuffAreaOffenseFactor"]);
		if (m_fDebuffAreaOffenseFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "디버프지역공격력계수가 유효하지 않습니다. m_fDebuffAreaOffenseFactor = " + m_fDebuffAreaOffenseFactor);
		}
		m_nSummonMonsterHpRecoveryInterval = Convert.ToInt32(dr["summonMonsterHpRecoveryInterval"]);
		if (m_nSummonMonsterHpRecoveryInterval <= 0)
		{
			SFLogUtil.Warn(GetType(), "소환몬스터HP회복간격이 유효하지 않습니다. m_nSummonMonsterHpRecoveryInterval = " + m_nSummonMonsterHpRecoveryInterval);
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

	public bool IsSafeRevivalWaitingTimeElapsed(float fElapsedTime)
	{
		return fElapsedTime >= (float)m_nSafeRevivalWaitingTime * 0.9f;
	}

	public void AddMonsterAttrFactor(RuinsReclaimMonsterAttrFactor monsterAttrFactor)
	{
		if (monsterAttrFactor == null)
		{
			throw new ArgumentNullException("monsterAttrFactor");
		}
		m_monsterAttrFactors.Add(monsterAttrFactor.level, monsterAttrFactor);
	}

	public RuinsReclaimMonsterAttrFactor GetMonsterAttrFactor(int nLevel)
	{
		if (!m_monsterAttrFactors.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddRevivalPoint(RuinsReclaimRevivalPoint revivalPoint)
	{
		if (revivalPoint == null)
		{
			throw new ArgumentNullException("revialPoint");
		}
		m_revivalPoints.Add(revivalPoint.id, revivalPoint);
	}

	public RuinsReclaimRevivalPoint GetRevivalPoint(int nId)
	{
		if (!m_revivalPoints.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddTrap(RuinsReclaimTrap trap)
	{
		if (trap == null)
		{
			throw new ArgumentNullException("trap");
		}
		m_traps.Add(trap);
	}

	public void AddPortal(RuinsReclaimPortal portal)
	{
		if (portal == null)
		{
			throw new ArgumentNullException("portal");
		}
		m_portals.Add(portal.id, portal);
	}

	public RuinsReclaimPortal GetPortal(int nId)
	{
		if (!m_portals.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddOpenSchedule(RuinsReclaimOpenSchedule openSchedule)
	{
		if (openSchedule == null)
		{
			throw new ArgumentNullException("openSchedule");
		}
		m_openSchedules.Add(openSchedule);
	}

	public RuinsReclaimOpenSchedule GetEnterableOpenSchedule(DateTimeOffset time)
	{
		int nTime = (int)time.TimeOfDay.TotalSeconds;
		foreach (RuinsReclaimOpenSchedule openSchedule in m_openSchedules)
		{
			if (openSchedule.IsEnterable(nTime))
			{
				return openSchedule;
			}
		}
		return null;
	}

	public void AddStep(RuinsReclaimStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_steps.Add(step);
	}

	public RuinsReclaimStep GetStep(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_steps.Count)
		{
			return null;
		}
		return m_steps[nIndex];
	}

	public void AddMonsterArrange(RuinsReclaimMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange.key, monsterArrange);
		if (monsterArrange.type == 2)
		{
			RuinsReclaimMonsterArrange lastBossArrange = GetMonsterArrange(m_nLastBossArrangeKey);
			if (lastBossArrange == null)
			{
				m_nLastBossArrangeKey = monsterArrange.key;
			}
			else if (monsterArrange.wave.step.no > lastBossArrange.wave.step.no)
			{
				m_nLastBossArrangeKey = monsterArrange.key;
			}
		}
	}

	public RuinsReclaimMonsterArrange GetMonsterArrange(int nKey)
	{
		if (!m_monsterArranges.TryGetValue(nKey, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddRandomRewardPoolEntry(RuinsReclaimRandomRewardPoolEntry randomRewardPoolEntry)
	{
		if (randomRewardPoolEntry == null)
		{
			throw new ArgumentNullException("randomRewardPoolEntry");
		}
		m_randomRewardPoolEntries.Add(randomRewardPoolEntry);
		m_nRandomRewardTotalPoint += randomRewardPoolEntry.point;
	}

	public RuinsReclaimRandomRewardPoolEntry SelectRandomRewardPoolEntry()
	{
		return Util.SelectPickEntry(m_randomRewardPoolEntries, m_nRandomRewardTotalPoint);
	}

	public void AddReward(RuinsReclaimReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public void AddMonsterTerminatorRewardPoolEntry(RuinsReclaimMonsterTerminatorRewardPoolEntry monsterTerminatorRewardPoolEntry)
	{
		if (monsterTerminatorRewardPoolEntry == null)
		{
			throw new ArgumentNullException("monsterTerminatorRewardPoolEntry");
		}
		m_monsterTerminatorRewardPoolEntries.Add(monsterTerminatorRewardPoolEntry);
		m_nMonsterTerminatorRewardTotalPoint += monsterTerminatorRewardPoolEntry.point;
	}

	public RuinsReclaimMonsterTerminatorRewardPoolEntry SelectMonsterTerminatorRewardPoolEntry()
	{
		return Util.SelectPickEntry(m_monsterTerminatorRewardPoolEntries, m_nMonsterTerminatorRewardTotalPoint);
	}

	public void AddUltimateAttackKingRewardPoolEntry(RuinsReclaimUltimateAttackKingRewardPoolEntry ultimateAttackKingRewardPoolEntry)
	{
		if (ultimateAttackKingRewardPoolEntry == null)
		{
			throw new ArgumentNullException("ultimateAttackKingRewardPoolEntry");
		}
		m_ultimateAttackKingRewardPoolEntries.Add(ultimateAttackKingRewardPoolEntry);
		m_nUltimateAttackKingRewardTotalPoint += ultimateAttackKingRewardPoolEntry.point;
	}

	public RuinsReclaimUltimateAttackKingRewardPoolEntry SelectUltimateAttackKingRewardPoolEntry()
	{
		return Util.SelectPickEntry(m_ultimateAttackKingRewardPoolEntries, m_nUltimateAttackKingRewardTotalPoint);
	}

	public void AddPartyVolunteerRewardPoolEntry(RuinsReclaimPartyVolunteerRewardPoolEntry partyVolunteerRewardPoolEntry)
	{
		if (partyVolunteerRewardPoolEntry == null)
		{
			throw new ArgumentNullException("partyVolunteerRewardPoolEntry");
		}
		m_partyVolunteerRewardPoolEntries.Add(partyVolunteerRewardPoolEntry);
		m_nPartyVolunteerRewardTotalPoint += partyVolunteerRewardPoolEntry.point;
	}

	public RuinsReclaimPartyVolunteerRewardPoolEntry SelectPartyVolunteerRewardPoolEntry()
	{
		return Util.SelectPickEntry(m_partyVolunteerRewardPoolEntries, m_nPartyVolunteerRewardTotalPoint);
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
