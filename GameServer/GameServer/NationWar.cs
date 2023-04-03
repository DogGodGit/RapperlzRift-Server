using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NationWar
{
	public const int kStartYRotationType_Fixed = 1;

	public const int kStartYRotationType_Random = 2;

	public const float kNationWarCallCoolTimeFactor = 0.9f;

	public const float kNationWarConvergingAttackFactor = 0.9f;

	public const float kNationWarConvergingAttackLifTime = 300f;

	public const int kNationWarMultiKillCountSection = 10;

	private int m_nDeclarationAvailableServerOpenDayCount;

	private int m_nDeclarationStartTime;

	private int m_nDeclarationEndTime;

	private int m_nDeclarationRequiredNationFund;

	private int m_nWeeklyDeclarationMaxCount;

	private int m_nStartTime;

	private int m_nEndTime;

	private int m_nResultDisplayEndTime;

	private int m_nJoinPopupDisplayDuration;

	private Continent m_offenseStartContinent;

	private Vector3 m_offenseStartPosition = Vector3.zero;

	private int m_nOffenseStartYRotationType;

	private float m_fOffenseStartYRotation;

	private float m_fOffenseStartRadius;

	private Continent m_defenseStartContinent;

	private Vector3 m_defenseStartPosition = Vector3.zero;

	private int m_nDefenseStartYRotationType;

	private float m_fDefenseStartYRotation;

	private float m_fDefenseStartRadius;

	private int m_nFreeTransmissionCount;

	private int m_nNationCallCount;

	private int m_nNationCallCoolTime;

	private int m_nNationCallLifetime;

	private float m_fNationCallRadius;

	private int m_nConvergingAttackCount;

	private int m_nConvergingAttackCoolTime;

	private int m_nConvergingAttackLifeTime;

	private ItemReward m_winNationItemReward1;

	private ItemReward m_winNationItemReward2;

	private ItemReward m_winNationAllianceItemReward;

	private ExploitPointReward m_winNationExploitPointReward;

	private ItemReward m_loseNationItemReward1;

	private ItemReward m_loseNationItemReward2;

	private ItemReward m_loseNationAllianceItemReward;

	private ExploitPointReward m_loseNationExploitPointReward;

	private int m_nLuckyRewardHighRanking;

	private int m_nLuckyRewardLowRanking;

	private int m_nLuckyRewardHeroCount;

	private ItemReward m_luckyItemReward;

	private Dictionary<int, NationWarNpc> m_npcs = new Dictionary<int, NationWarNpc>();

	private List<NationWarPaidTransmission> m_paidTransmissions = new List<NationWarPaidTransmission>();

	private List<NationWarHeroObjectiveEntry> m_heroObjectiveEntries = new List<NationWarHeroObjectiveEntry>();

	private List<NationWarExpReward> m_expRewards = new List<NationWarExpReward>();

	private Dictionary<DayOfWeek, NationWarAvailableDayOfWeek> m_availableDayOfWeeks = new Dictionary<DayOfWeek, NationWarAvailableDayOfWeek>();

	private List<SystemNationWarDeclaration> m_systemDeclarations = new List<SystemNationWarDeclaration>();

	private Dictionary<int, NationWarRevivalPoint> m_revivalPoints = new Dictionary<int, NationWarRevivalPoint>();

	private Dictionary<int, NationWarMonsterArrange> m_monsterArranges = new Dictionary<int, NationWarMonsterArrange>();

	private Dictionary<int, NationWarRankingReward> m_rankingRewards = new Dictionary<int, NationWarRankingReward>();

	private Dictionary<int, NationWarPointReward> m_pointRewards = new Dictionary<int, NationWarPointReward>();

	public int declarationAvailableServerOpenCount => m_nDeclarationAvailableServerOpenDayCount;

	public int declarationStartTime => m_nDeclarationStartTime;

	public int declarationEndTime => m_nDeclarationEndTime;

	public int declarationRequredNationFund => m_nDeclarationRequiredNationFund;

	public int weeklyDeclarationMaxCount => m_nWeeklyDeclarationMaxCount;

	public int startTime => m_nStartTime;

	public int endTime => m_nEndTime;

	public int resultDisplayEndTime => m_nResultDisplayEndTime;

	public int joinPopupDisplayDuration => m_nJoinPopupDisplayDuration;

	public Continent offenseStartContinent => m_offenseStartContinent;

	public int offenseStartContinentId => m_offenseStartContinent.id;

	public Vector3 offenseStartPosition => m_offenseStartPosition;

	public int offenseStartYRotationType => m_nOffenseStartYRotationType;

	public float offenseStartYRotation => m_fOffenseStartYRotation;

	public float offenseStartRadius => m_fOffenseStartRadius;

	public Continent defenseStartContinent => m_defenseStartContinent;

	public int defenseStartContinentId => m_defenseStartContinent.id;

	public Vector3 defenseStartPosition => m_defenseStartPosition;

	public int defenseStartYRotationType => m_nDefenseStartYRotationType;

	public float defenseStartYRotation => m_fDefenseStartYRotation;

	public float defenseStartRadius => m_fDefenseStartRadius;

	public int freeTransmissionCount => m_nFreeTransmissionCount;

	public int nationCallCount => m_nNationCallCount;

	public int nationCallCoolTime => m_nNationCallCoolTime;

	public int nationCallLifetime => m_nNationCallLifetime;

	public float nationCallRadius => m_fNationCallRadius;

	public int convergingAttackCount => m_nConvergingAttackCount;

	public int convergingAttackCoolTime => m_nConvergingAttackCoolTime;

	public int convergingAttackLifeTime => m_nConvergingAttackLifeTime;

	public ItemReward winNationItemReward1 => m_winNationItemReward1;

	public ItemReward winNationItemReward2 => m_winNationItemReward2;

	public ItemReward winNationAllianceItemReward => m_winNationAllianceItemReward;

	public ExploitPointReward winNationExploitPointReward => m_winNationExploitPointReward;

	public ItemReward loseNationItemReward1 => m_loseNationItemReward1;

	public ItemReward loseNationItemReward2 => m_loseNationItemReward2;

	public ItemReward loseNationAllianceItemReward => m_loseNationAllianceItemReward;

	public ExploitPointReward loseNationExploitPointReward => m_loseNationExploitPointReward;

	public int luckyRewardHighRanking => m_nLuckyRewardHighRanking;

	public int luckyRewardLowRanking => m_nLuckyRewardLowRanking;

	public int luckyRewardHeroCount => m_nLuckyRewardHeroCount;

	public ItemReward luckyItemReward => m_luckyItemReward;

	public List<NationWarHeroObjectiveEntry> heroObjectiveEntries => m_heroObjectiveEntries;

	public List<SystemNationWarDeclaration> systemDeclarations => m_systemDeclarations;

	public Dictionary<int, NationWarRevivalPoint> revivalPoints => m_revivalPoints;

	public Dictionary<int, NationWarMonsterArrange> monsterArranges => m_monsterArranges;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		Resource res = Resource.instance;
		m_nDeclarationAvailableServerOpenDayCount = Convert.ToInt32(dr["declarationAvailableServerOpenDaycount"]);
		if (m_nDeclarationAvailableServerOpenDayCount < 0)
		{
			SFLogUtil.Warn(GetType(), "선포가능서버오픈일수가 유효하지 않습니다. m_nDeclarationAvailableServerOpenDayCount = " + m_nDeclarationAvailableServerOpenDayCount);
		}
		m_nDeclarationStartTime = Convert.ToInt32(dr["declarationStartTime"]);
		if (m_nDeclarationStartTime < 0)
		{
			SFLogUtil.Warn(GetType(), "선포시작시각이 유효하지 않습니다. m_nDeclarationStartTime = " + m_nDeclarationStartTime);
		}
		m_nDeclarationEndTime = Convert.ToInt32(dr["declarationEndTime"]);
		if (m_nDeclarationEndTime < 0)
		{
			SFLogUtil.Warn(GetType(), "선포종료시각이 유효하지 않습니다. m_nDeclarationEndTime = " + m_nDeclarationEndTime);
		}
		if (m_nDeclarationStartTime > m_nDeclarationEndTime)
		{
			SFLogUtil.Warn(GetType(), "선포종료시각이 선포시작시각보다 작습니다. m_nDeclarationStartTime = " + m_nDeclarationStartTime + ", m_nDeclarationEndTime = " + m_nDeclarationEndTime);
		}
		m_nDeclarationRequiredNationFund = Convert.ToInt32(dr["declarationRequiredNationFund"]);
		if (m_nDeclarationRequiredNationFund < 0)
		{
			SFLogUtil.Warn(GetType(), "국가전선포필요국고자금이 유효하지 않습니다. m_nDeclarationRequiredNationFund = " + m_nDeclarationRequiredNationFund);
		}
		m_nWeeklyDeclarationMaxCount = Convert.ToInt32(dr["weeklyDeclarationMaxCount"]);
		if (m_nWeeklyDeclarationMaxCount < 0)
		{
			SFLogUtil.Warn(GetType(), "주간선포가능최대횟수가 유효하지 않습니다. m_nWeeklyDeclarationMaxCount = " + m_nWeeklyDeclarationMaxCount);
		}
		m_nStartTime = Convert.ToInt32(dr["startTime"]);
		if (m_nStartTime < 0)
		{
			SFLogUtil.Warn(GetType(), "시작시각이 유효하지 않습니다. m_nStartTime = " + m_nStartTime);
		}
		m_nEndTime = Convert.ToInt32(dr["endTime"]);
		if (m_nEndTime < 0)
		{
			SFLogUtil.Warn(GetType(), "종료시각이 유효하지 않습니다. m_nEndTime = " + m_nEndTime);
		}
		if (m_nStartTime > m_nEndTime)
		{
			SFLogUtil.Warn(GetType(), "종료시각이 시작시각보다 작습니다. m_nStartTime = " + m_nStartTime + ", m_nEndTime = " + m_nEndTime);
		}
		m_nResultDisplayEndTime = Convert.ToInt32(dr["resultDisplayEndTime"]);
		if (m_nResultDisplayEndTime < 0)
		{
			SFLogUtil.Warn(GetType(), "결과표시종료시각이 유효하지 않습니다. m_nResultDisplayEndTime = " + m_nResultDisplayEndTime);
		}
		m_nJoinPopupDisplayDuration = Convert.ToInt32(dr["joinPopupDisplayDuration"]);
		if (m_nJoinPopupDisplayDuration < 0)
		{
			SFLogUtil.Warn(GetType(), "참여표시기간이 유효하지 않습니다. m_nJoinPopupDisplayDuration = " + m_nJoinPopupDisplayDuration);
		}
		int nOffenseStartContinentId = Convert.ToInt32(dr["offenseStartContinentId"]);
		if (nOffenseStartContinentId > 0)
		{
			m_offenseStartContinent = res.GetContinent(nOffenseStartContinentId);
			if (m_offenseStartContinent == null)
			{
				SFLogUtil.Warn(GetType(), "공격시작대륙이 존재하지 않습니다. nOffenseStartContinentId = " + nOffenseStartContinentId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "공격시작대륙ID가 유효하지 않습니다. nOffenseStartContinentId = " + nOffenseStartContinentId);
		}
		m_offenseStartPosition.x = Convert.ToSingle(dr["offenseStartXPosition"]);
		m_offenseStartPosition.y = Convert.ToSingle(dr["offenseStartYPosition"]);
		m_offenseStartPosition.z = Convert.ToSingle(dr["offenseStartZPosition"]);
		m_nOffenseStartYRotationType = Convert.ToInt32(dr["offenseStartYRotationType"]);
		if (!IsDefinedStartYRotationType(m_nOffenseStartYRotationType))
		{
			SFLogUtil.Warn(GetType(), "공격시작방향타입이 유효하지 않습니다. m_nOffenseStartYRotationType = " + m_nOffenseStartYRotationType);
		}
		m_fOffenseStartYRotation = Convert.ToSingle(dr["offenseStartYRotation"]);
		m_fOffenseStartRadius = Convert.ToSingle(dr["offenseStartRadius"]);
		if (m_fOffenseStartRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "공격시작반지름이 유효하지 않습니다. m_fOffenseStartRadius = " + m_fOffenseStartRadius);
		}
		int nDefenseStartContinentId = Convert.ToInt32(dr["defenseStartContinentId"]);
		if (nDefenseStartContinentId > 0)
		{
			m_defenseStartContinent = res.GetContinent(nDefenseStartContinentId);
			if (m_defenseStartContinent == null)
			{
				SFLogUtil.Warn(GetType(), "방어시작대륙이 존재하지 않습니다. nDefenseStartContinentId = " + nDefenseStartContinentId);
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), "방어시작대륙ID가 유효하지 않습니다. nDefenseStartContinentId = " + nDefenseStartContinentId);
		}
		m_defenseStartPosition.x = Convert.ToSingle(dr["defenseStartXPosition"]);
		m_defenseStartPosition.y = Convert.ToSingle(dr["defenseStartYPosition"]);
		m_defenseStartPosition.z = Convert.ToSingle(dr["defenseStartZPosition"]);
		m_nDefenseStartYRotationType = Convert.ToInt32(dr["defenseStartYRotationType"]);
		if (!IsDefinedStartYRotationType(m_nDefenseStartYRotationType))
		{
			SFLogUtil.Warn(GetType(), "방어시작방향타입이 유효하지 않습니다. m_nDefenseStartYRotationType = " + m_nDefenseStartYRotationType);
		}
		m_fDefenseStartYRotation = Convert.ToSingle(dr["defenseStartYRotation"]);
		m_fDefenseStartRadius = Convert.ToSingle(dr["defenseStartRadius"]);
		if (m_fDefenseStartRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "방어시작반지름이 유효하지 않습니다. m_fDefenseStartRadius = " + m_fDefenseStartRadius);
		}
		m_nFreeTransmissionCount = Convert.ToInt32(dr["freeTransmissionCount"]);
		if (m_nFreeTransmissionCount < 0)
		{
			SFLogUtil.Warn(GetType(), "무료전송횟수가 유효하지 않습니다. m_nFreeTransmissionCount = " + m_nFreeTransmissionCount);
		}
		m_nNationCallCount = Convert.ToInt32(dr["nationCallCount"]);
		if (m_nNationCallCount < 0)
		{
			SFLogUtil.Warn(GetType(), "국가소집횟수가 유효하지 않습니다. m_nNationCallCount = " + m_nNationCallCount);
		}
		m_nNationCallCoolTime = Convert.ToInt32(dr["nationCallCoolTime"]);
		if (m_nNationCallCoolTime < 0)
		{
			SFLogUtil.Warn(GetType(), "국가소집쿨타임이 유효하지 않습니다. m_nNationCallCoolTime = " + m_nNationCallCoolTime);
		}
		m_nNationCallLifetime = Convert.ToInt32(dr["nationCallLifetime"]);
		if (m_nNationCallLifetime < 0)
		{
			SFLogUtil.Warn(GetType(), "국가소집유효기간이 유효하지 않습니다. m_nNationCallLifetime = " + m_nNationCallLifetime);
		}
		m_fNationCallRadius = Convert.ToSingle(dr["nationCallRadius"]);
		if (m_fNationCallRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "국가소집반경이 유효하지 않습니다. m_fNationCallRadius = " + m_fNationCallRadius);
		}
		m_nConvergingAttackCount = Convert.ToInt32(dr["convergingAttackCount"]);
		if (m_nConvergingAttackCount < 0)
		{
			SFLogUtil.Warn(GetType(), "집중공격횟수가 유효하지 않습니다. m_nConvergingAttackCount = " + m_nConvergingAttackCount);
		}
		m_nConvergingAttackLifeTime = Convert.ToInt32(dr["convergingAttackLifeTime"]);
		if (m_nConvergingAttackLifeTime < 0)
		{
			SFLogUtil.Warn(GetType(), "집중공격유효기간이 유효하지 않습니다. m_nConvergingAttackLifeTime = " + m_nConvergingAttackLifeTime);
		}
		m_nConvergingAttackCoolTime = Convert.ToInt32(dr["convergingAttackCoolTime"]);
		if (m_nConvergingAttackCoolTime < 0)
		{
			SFLogUtil.Warn(GetType(), "집중공격쿨타입이 유효하지 않습니다. m_nConvergingAttackCoolTime = " + m_nConvergingAttackCoolTime);
		}
		long lnWinNationItemRewardId1 = Convert.ToInt64(dr["winNationItemRewardId1"]);
		if (lnWinNationItemRewardId1 > 0)
		{
			m_winNationItemReward1 = res.GetItemReward(lnWinNationItemRewardId1);
			if (m_winNationItemReward1 == null)
			{
				SFLogUtil.Warn(GetType(), "승리국가아이템보상1이 존재하지 않습니다. lnWinNationItemRewardId1 = " + lnWinNationItemRewardId1);
			}
		}
		else if (lnWinNationItemRewardId1 < 0)
		{
			SFLogUtil.Warn(GetType(), "승리국가아이템보상1ID가 유효하지 않습니다. lnWinNationItemRewardId1 = " + lnWinNationItemRewardId1);
		}
		long lnWinNationItemRewardId2 = Convert.ToInt64(dr["winNationItemRewardId2"]);
		if (lnWinNationItemRewardId2 > 0)
		{
			m_winNationItemReward2 = res.GetItemReward(lnWinNationItemRewardId2);
			if (m_winNationItemReward2 == null)
			{
				SFLogUtil.Warn(GetType(), "승리국가아이템보상2가 존재하지 않습니다. lnWinNationItemRewardId2 = " + lnWinNationItemRewardId2);
			}
		}
		else if (lnWinNationItemRewardId2 < 0)
		{
			SFLogUtil.Warn(GetType(), "승리국가아이템보상2ID가 유효하지 않습니다. lnWinNationItemRewardId2 = " + lnWinNationItemRewardId2);
		}
		long lnWinNationAllianceItemRewardId = Convert.ToInt64(dr["winNationAllianceItemRewardId"]);
		if (lnWinNationAllianceItemRewardId > 0)
		{
			m_winNationAllianceItemReward = res.GetItemReward(lnWinNationAllianceItemRewardId);
			if (m_winNationAllianceItemReward == null)
			{
				SFLogUtil.Warn(GetType(), "승리국가동맹아이템보상이 존재하지 않습니다. lnWinNationAllianceItemRewardId = " + lnWinNationAllianceItemRewardId);
			}
		}
		else if (lnWinNationAllianceItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "승리국가동맹아이템보상ID가 유효하지 않습니다. lnWinNationAllianceItemRewardId = " + lnWinNationAllianceItemRewardId);
		}
		long lnWinNationExploitPointRewardId = Convert.ToInt64(dr["winNationExploitPointRewardId"]);
		if (lnWinNationExploitPointRewardId > 0)
		{
			m_winNationExploitPointReward = res.GetExploitPointReward(lnWinNationExploitPointRewardId);
			if (m_winNationExploitPointReward == null)
			{
				SFLogUtil.Warn(GetType(), "승리국가공적점수보상이 존재하지 않습니다. lnWinNationExploitPointRewardId = " + lnWinNationExploitPointRewardId);
			}
		}
		else if (lnWinNationExploitPointRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "승리국가공적점수보상ID가 유효하지 않습니다. lnWinNationExploitPointRewardId = " + lnWinNationExploitPointRewardId);
		}
		long lnLoseNationItemRewardId1 = Convert.ToInt64(dr["loseNationItemRewardId1"]);
		if (lnLoseNationItemRewardId1 > 0)
		{
			m_loseNationItemReward1 = res.GetItemReward(lnLoseNationItemRewardId1);
			if (m_loseNationItemReward1 == null)
			{
				SFLogUtil.Warn(GetType(), "패배국가아이템보상1이 존재하지 않습니다. lnLoseNationItemRewardId1 = " + lnLoseNationItemRewardId1);
			}
		}
		else if (lnLoseNationItemRewardId1 < 0)
		{
			SFLogUtil.Warn(GetType(), "패배국가아이템보상1ID가 유효하지 않습니다. lnLoseNationItemRewardId1 = " + lnLoseNationItemRewardId1);
		}
		long lnLoseNationItemRewardId2 = Convert.ToInt64(dr["loseNationItemRewardId2"]);
		if (lnLoseNationItemRewardId2 > 0)
		{
			m_loseNationItemReward2 = res.GetItemReward(lnLoseNationItemRewardId2);
			if (m_loseNationItemReward2 == null)
			{
				SFLogUtil.Warn(GetType(), "패배국가아이템보상2가 존재하지 않습니다. lnLoseNationItemRewardId2 = " + lnLoseNationItemRewardId2);
			}
		}
		else if (lnLoseNationItemRewardId2 < 0)
		{
			SFLogUtil.Warn(GetType(), "패배국가아이템보상2ID가 유효하지 않습니다. lnLoseNationItemRewardId2 = " + lnLoseNationItemRewardId2);
		}
		long lnLoseNationAllianceItemRewardId = Convert.ToInt64(dr["loseNationAllianceItemRewardId"]);
		if (lnLoseNationAllianceItemRewardId > 0)
		{
			m_loseNationAllianceItemReward = res.GetItemReward(lnLoseNationAllianceItemRewardId);
			if (m_loseNationAllianceItemReward == null)
			{
				SFLogUtil.Warn(GetType(), "패배국가동맹아이템보상이 존재하지 않습니다. lnLoseNationAllianceItemRewardId = " + lnLoseNationAllianceItemRewardId);
			}
		}
		else if (lnLoseNationAllianceItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "패배국가동맹아이템보상ID가 유효하지 않습니다. lnLoseNationAllianceItemRewardId = " + lnLoseNationAllianceItemRewardId);
		}
		long lnLoseNationExploitPointRewardId = Convert.ToInt64(dr["loseNationExploitPointRewardId"]);
		if (lnLoseNationExploitPointRewardId > 0)
		{
			m_loseNationExploitPointReward = res.GetExploitPointReward(lnLoseNationExploitPointRewardId);
			if (m_loseNationExploitPointReward == null)
			{
				SFLogUtil.Warn(GetType(), "패배국가공적점수보상이 존재하지 않습니다. m_loseNationExploitPointReward = " + m_loseNationExploitPointReward);
			}
		}
		else if (lnLoseNationExploitPointRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "패배국가공적점수보상ID가 유효하지 않습니다. lnLoseNationExploitPointRewardId = " + lnLoseNationExploitPointRewardId);
		}
		m_nLuckyRewardHighRanking = Convert.ToInt32(dr["luckyRewardHighRanking"]);
		if (m_nLuckyRewardHighRanking <= 0)
		{
			SFLogUtil.Warn(GetType(), "행운보상상위랭킹이 유효하지 않습니다. m_nLuckyRewardHighRanking = " + m_nLuckyRewardHighRanking);
		}
		m_nLuckyRewardLowRanking = Convert.ToInt32(dr["luckyRewardLowRanking"]);
		if (m_nLuckyRewardLowRanking <= 0)
		{
			SFLogUtil.Warn(GetType(), "행운보상하위랭킹이 유효하지 않습니다. m_nLuckyRewardLowRanking = " + m_nLuckyRewardLowRanking);
		}
		if (m_nLuckyRewardHighRanking > m_nLuckyRewardLowRanking)
		{
			SFLogUtil.Warn(GetType(), "행운보상상위랭킹이 행운보상하위랭킹보다 낮습니다. m_nLuckyRewardHighRanking = " + m_nLuckyRewardHighRanking + ", m_nLuckyRewardLowRanking = " + m_nLuckyRewardLowRanking);
		}
		m_nLuckyRewardHeroCount = Convert.ToInt32(dr["luckyRewardHeroCount"]);
		if (m_nLuckyRewardHeroCount < 0)
		{
			SFLogUtil.Warn(GetType(), "향운보상영웅수가 유효하지 않습니다. m_nLuckyRewardHeroCount = " + m_nLuckyRewardHeroCount);
		}
		long lnLuckyItemRewardId = Convert.ToInt64(dr["luckyItemRewardId"]);
		if (lnLuckyItemRewardId > 0)
		{
			m_luckyItemReward = res.GetItemReward(lnLuckyItemRewardId);
			if (m_luckyItemReward == null)
			{
				SFLogUtil.Warn(GetType(), "행운아이템보상이 존재하지 않습니다. lnLuckyItemRewardId = " + lnLuckyItemRewardId);
			}
		}
		else if (lnLuckyItemRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "행운아이템보상ID가 유효하지 않습니다. lnLuckyItemRewardId = " + lnLuckyItemRewardId);
		}
	}

	public void AddNpc(NationWarNpc npc)
	{
		if (npc == null)
		{
			throw new ArgumentNullException("npc");
		}
		m_npcs.Add(npc.id, npc);
	}

	public NationWarNpc GetNpc(int nNpcId)
	{
		if (!m_npcs.TryGetValue(nNpcId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddPaidTransmission(NationWarPaidTransmission paidTransmission)
	{
		if (paidTransmission == null)
		{
			throw new ArgumentNullException("paidTransmission");
		}
		m_paidTransmissions.Add(paidTransmission);
	}

	public NationWarPaidTransmission GetPaidTransmission(int nCount)
	{
		int nTransmissionCount = Math.Min(nCount, m_nFreeTransmissionCount);
		int nIndex = nTransmissionCount - 1;
		if (nIndex < 0)
		{
			return null;
		}
		return m_paidTransmissions[nIndex];
	}

	public void AddHeroObjectiveEntry(NationWarHeroObjectiveEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_heroObjectiveEntries.Add(entry);
	}

	public void AddExpReward(NationWarExpReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_expRewards.Add(reward);
	}

	public NationWarExpReward GetExpReward(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_expRewards.Count)
		{
			return null;
		}
		return m_expRewards[nIndex];
	}

	public void AddAvailableDayOfWeek(NationWarAvailableDayOfWeek availableDayOfWeek)
	{
		if (availableDayOfWeek == null)
		{
			throw new ArgumentNullException("availableDayOfWeek");
		}
		m_availableDayOfWeeks.Add(availableDayOfWeek.dayOfWeek, availableDayOfWeek);
	}

	public bool ContainsAvailableDayOfWeek(DayOfWeek dayOfWeek)
	{
		return m_availableDayOfWeeks.ContainsKey(dayOfWeek);
	}

	public void AddSystemDeclaration(SystemNationWarDeclaration systemDeclaration)
	{
		if (systemDeclaration == null)
		{
			throw new ArgumentNullException("systemDeclaration");
		}
		m_systemDeclarations.Add(systemDeclaration);
	}

	public void AddRevivalPoint(NationWarRevivalPoint revivalPoint)
	{
		if (revivalPoint == null)
		{
			throw new ArgumentNullException("revivalPoint");
		}
		m_revivalPoints.Add(revivalPoint.id, revivalPoint);
	}

	public NationWarRevivalPoint GetRevivalPoint(int nRevivalPointId)
	{
		if (!m_revivalPoints.TryGetValue(nRevivalPointId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddMonsterArrange(NationWarMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange.id, monsterArrange);
	}

	public NationWarMonsterArrange GetMonsterArrange(int nArrangeId)
	{
		if (!m_monsterArranges.TryGetValue(nArrangeId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddRankingReward(NationWarRankingReward rankingReward)
	{
		if (rankingReward == null)
		{
			throw new ArgumentNullException("rankingReward");
		}
		m_rankingRewards.Add(rankingReward.ranking, rankingReward);
	}

	public NationWarRankingReward GetRankingReward(int nRanking)
	{
		if (!m_rankingRewards.TryGetValue(nRanking, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddPointReward(NationWarPointReward pointReward)
	{
		if (pointReward == null)
		{
			throw new ArgumentNullException("pointReward");
		}
		m_pointRewards.Add(pointReward.ratingDifference, pointReward);
	}

	public NationWarPointReward GetPointReward(int nRatingDifference)
	{
		if (!m_pointRewards.TryGetValue(nRatingDifference, out var value))
		{
			return null;
		}
		return value;
	}

	public bool IsEnabledNationWarDeclaration(DateTimeOffset time)
	{
		int nTotalSecond = (int)Math.Floor(time.TimeOfDay.TotalSeconds);
		if (nTotalSecond >= m_nDeclarationStartTime)
		{
			return true;
		}
		return false;
	}

	public bool IsEnabledNationWarJoin(DateTimeOffset time)
	{
		int nTotalSecond = (int)Math.Floor(time.TimeOfDay.TotalSeconds);
		int nElapsedTime = nTotalSecond - m_nStartTime;
		if (nElapsedTime >= 0 && nElapsedTime < m_nJoinPopupDisplayDuration)
		{
			return true;
		}
		return false;
	}

	public Vector3 SelectOffenseStartPosition()
	{
		return Util.SelectPoint(m_offenseStartPosition, m_fOffenseStartRadius);
	}

	public float SelectOffenseStartRotationY()
	{
		if (m_nOffenseStartYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fOffenseStartYRotation);
		}
		return m_fOffenseStartYRotation;
	}

	public Vector3 SelectDefenseStartPosition()
	{
		return Util.SelectPoint(m_defenseStartPosition, m_fDefenseStartRadius);
	}

	public float SelectDefenseStartRotationY()
	{
		if (m_nDefenseStartYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fDefenseStartYRotation);
		}
		return m_fDefenseStartYRotation;
	}

	public static bool IsDefinedStartYRotationType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
