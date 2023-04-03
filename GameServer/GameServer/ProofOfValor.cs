using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class ProofOfValor : Location
{
	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

	private int m_nLocationId;

	private int m_nRequiredConditionType;

	private int m_nRequiredHeroLevel;

	private int m_nRequiredMainQuestNo;

	private int m_nRequiredStamina;

	private int m_nDailyFreeRefreshCount;

	private int m_nDailyPaidRefreshCount;

	private int m_nStartDelayTime;

	private int m_nLimitTime;

	private int m_nExitDelayTime;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartYRotation;

	private int m_nBuffBoxCreationTime;

	private int m_nBuffBoxCreationInterval;

	private int m_nBuffBoxLifeTime;

	private int m_nBuffDuration;

	private int m_nFailureRewardSoulPowder;

	private Rect3D m_mapRect = Rect3D.zero;

	private Dictionary<int, ProofOfValorBuffBox> m_buffBoxs = new Dictionary<int, ProofOfValorBuffBox>();

	private List<ProofOfValorBuffBoxArrange> m_buffBoxArranges = new List<ProofOfValorBuffBoxArrange>();

	private List<ProofOfValorMonsterAttrFactor> m_monsterAttrFactors = new List<ProofOfValorMonsterAttrFactor>();

	private List<ProofOfValorPaidRefresh> m_paidRefreshs = new List<ProofOfValorPaidRefresh>();

	private Dictionary<int, ProofOfValorBossMonsterArrange> m_bossMonsterArranges = new Dictionary<int, ProofOfValorBossMonsterArrange>();

	private int m_nBossMonsterArrangeTotalPoint;

	private List<ProofOfValorRefreshSchedule> m_refreshSchedules = new List<ProofOfValorRefreshSchedule>();

	private Dictionary<int, ProofOfValorCreatureCardPool> m_creatureCardPools = new Dictionary<int, ProofOfValorCreatureCardPool>();

	private List<ProofOfValorReward> m_rewards = new List<ProofOfValorReward>();

	private List<ProofOfValorClearGrade> m_clearGrades = new List<ProofOfValorClearGrade>();

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.ProofOfValor;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public int requiredConditionType => m_nRequiredConditionType;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int requiredMainQuestNo => m_nRequiredMainQuestNo;

	public int requiredStamina => m_nRequiredStamina;

	public int dailyFreeRefreshCount => m_nDailyFreeRefreshCount;

	public int dailyPaidRefreshCount => m_nDailyPaidRefreshCount;

	public int startDelayTime => m_nStartDelayTime;

	public int limitTime => m_nLimitTime;

	public int exitDelayTime => m_nExitDelayTime;

	public Vector3 startPosition => m_startPosition;

	public float startYRotation => m_fStartYRotation;

	public int buffBoxCreationTime => m_nBuffBoxCreationTime;

	public int buffBoxCreationInterval => m_nBuffBoxCreationInterval;

	public int buffBoxLifeTime => m_nBuffBoxLifeTime;

	public int buffDuration => m_nBuffDuration;

	public int failureRewardSoulPowder => m_nFailureRewardSoulPowder;

	public Rect3D mapRect => m_mapRect;

	public Dictionary<int, ProofOfValorBuffBox> buffBoxs => m_buffBoxs;

	public List<ProofOfValorBuffBoxArrange> buffBoxArranges => m_buffBoxArranges;

	public List<ProofOfValorMonsterAttrFactor> monsterAttrFactor => m_monsterAttrFactors;

	public List<ProofOfValorRefreshSchedule> refreshSchedules => m_refreshSchedules;

	public ProofOfValorRefreshSchedule lastRefreshSchedule => m_refreshSchedules.LastOrDefault();

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
			SFLogUtil.Warn(GetType(), "필요체력이 유효하지 않습니다. m_nRequiredStamina = " + m_nRequiredStamina);
		}
		m_nDailyFreeRefreshCount = Convert.ToInt32(dr["dailyFreeRefreshCount"]);
		if (m_nDailyFreeRefreshCount < 0)
		{
			SFLogUtil.Warn(GetType(), "일일무료갱신횟수가 유효하지 않습니다. m_nDailyFreeRefreshCount = " + m_nDailyFreeRefreshCount);
		}
		m_nDailyPaidRefreshCount = Convert.ToInt32(dr["dailyPaidRefreshCount"]);
		if (m_nDailyPaidRefreshCount < 0)
		{
			SFLogUtil.Warn(GetType(), "일일유료갱신횟수가 유효하지 않습니다. m_nDailyPaidRefreshCount = " + m_nDailyPaidRefreshCount);
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
		m_fStartYRotation = Convert.ToSingle(dr["startYRotation"]);
		m_nBuffBoxCreationTime = Convert.ToInt32(dr["buffBoxCreationTime"]);
		if (m_nBuffBoxCreationTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "버프상자생성시각이 유효하지 않습니다. m_nBuffBoxCreationTime = " + m_nBuffBoxCreationTime);
		}
		m_nBuffBoxCreationInterval = Convert.ToInt32(dr["buffBoxCreationInterval"]);
		if (m_nBuffBoxCreationInterval <= 0)
		{
			SFLogUtil.Warn(GetType(), "버프상자생성간격이 유효하지 않습니다. m_nBuffBoxCreationInterval = " + m_nBuffBoxCreationInterval);
		}
		m_nBuffBoxLifeTime = Convert.ToInt32(dr["buffBoxLifeTime"]);
		if (m_nBuffBoxLifeTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "버프상자유지기간이 유효하지 않습니다. m_nBuffBoxLifeTime = " + m_nBuffBoxLifeTime);
		}
		if (m_nBuffBoxLifeTime >= m_nBuffBoxCreationInterval)
		{
			SFLogUtil.Warn(GetType(), "버프상자유지기간이 버프상자생성간격보다 크거나 같습니다. m_nBuffBoxCreationInterval = " + m_nBuffBoxCreationInterval + ", m_nBuffBoxLifeTime = " + m_nBuffBoxLifeTime);
		}
		m_nBuffDuration = Convert.ToInt32(dr["buffDuration"]);
		if (m_nBuffDuration <= 0)
		{
			SFLogUtil.Warn(GetType(), "버프유지기간이 유효하지 않습니다. m_nBuffDuration = " + m_nBuffDuration);
		}
		m_nFailureRewardSoulPowder = Convert.ToInt32(dr["failureRewardSoulPowder"]);
		if (m_nFailureRewardSoulPowder < 0)
		{
			SFLogUtil.Warn(GetType(), "실패보상영혼가루가 유효하지 않습니다. m_nFailureRewardSoulPowder = " + m_nFailureRewardSoulPowder);
		}
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public void AddBuffBox(ProofOfValorBuffBox buffBox)
	{
		if (buffBox == null)
		{
			throw new ArgumentNullException("buffBox");
		}
		m_buffBoxs.Add(buffBox.id, buffBox);
	}

	public ProofOfValorBuffBox GetBuffBox(int nBuffBoxId)
	{
		if (!m_buffBoxs.TryGetValue(nBuffBoxId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddBuffBoxArrange(ProofOfValorBuffBoxArrange buffBoxArrange)
	{
		if (buffBoxArrange == null)
		{
			throw new ArgumentNullException("buffBoxArrange");
		}
		m_buffBoxArranges.Add(buffBoxArrange);
	}

	public void AddMonsterAttrFactor(ProofOfValorMonsterAttrFactor monsterAttrFactor)
	{
		if (monsterAttrFactor == null)
		{
			throw new ArgumentNullException("monsterAttrFactor");
		}
		m_monsterAttrFactors.Add(monsterAttrFactor);
	}

	public ProofOfValorMonsterAttrFactor GetMonsterAttrFactor(int nHeroLevel)
	{
		int nIndex = nHeroLevel - 1;
		if (nIndex < 0 || nIndex >= m_monsterAttrFactors.Count)
		{
			return null;
		}
		return m_monsterAttrFactors[nIndex];
	}

	public void AddPaidRefresh(ProofOfValorPaidRefresh paidRefresh)
	{
		if (paidRefresh == null)
		{
			throw new ArgumentNullException("paidRefresh");
		}
		m_paidRefreshs.Add(paidRefresh);
	}

	public ProofOfValorPaidRefresh GetPaidRefresh(int nCount)
	{
		int nRefreshCount = Math.Min(nCount, m_paidRefreshs.Count);
		int nIndex = nRefreshCount - 1;
		if (nIndex < 0)
		{
			return null;
		}
		return m_paidRefreshs[nIndex];
	}

	public void AddBossMonsterArrange(ProofOfValorBossMonsterArrange bossMonsterArrange)
	{
		if (bossMonsterArrange == null)
		{
			throw new ArgumentNullException("bossMonsterArrange");
		}
		m_bossMonsterArranges.Add(bossMonsterArrange.id, bossMonsterArrange);
		m_nBossMonsterArrangeTotalPoint += bossMonsterArrange.point;
	}

	public ProofOfValorBossMonsterArrange GetBossMonaterArrange(int nId)
	{
		if (!m_bossMonsterArranges.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public ProofOfValorBossMonsterArrange SelectBossMonsterArrange()
	{
		return Util.SelectPickEntry(m_bossMonsterArranges.Values, m_nBossMonsterArrangeTotalPoint);
	}

	public void AddRefreshSchedule(ProofOfValorRefreshSchedule refreshSchedule)
	{
		if (refreshSchedule == null)
		{
			throw new ArgumentNullException("refreshSchedule");
		}
		m_refreshSchedules.Add(refreshSchedule);
	}

	public ProofOfValorCreatureCardPool GetCreatureCardPool(int nPoolId)
	{
		if (!m_creatureCardPools.TryGetValue(nPoolId, out var value))
		{
			return null;
		}
		return value;
	}

	public ProofOfValorCreatureCardPool GetOrCreateCreatureCardPool(int nPoolId)
	{
		ProofOfValorCreatureCardPool creatureCardPool = GetCreatureCardPool(nPoolId);
		if (creatureCardPool == null)
		{
			creatureCardPool = new ProofOfValorCreatureCardPool(this, nPoolId);
			m_creatureCardPools.Add(creatureCardPool.id, creatureCardPool);
		}
		return creatureCardPool;
	}

	public void AddReward(ProofOfValorReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public ProofOfValorReward GetReward(int nHeroLevel)
	{
		int nIndex = nHeroLevel - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}

	public void AddClearGrade(ProofOfValorClearGrade clearGrade)
	{
		if (clearGrade == null)
		{
			throw new ArgumentNullException("clearGrade");
		}
		m_clearGrades.Add(clearGrade);
	}

	public ProofOfValorClearGrade GetClearGrade(float fRemainingTime)
	{
		foreach (ProofOfValorClearGrade clearGrade in m_clearGrades)
		{
			if (fRemainingTime >= (float)clearGrade.minRemainingTime)
			{
				return clearGrade;
			}
		}
		return m_clearGrades.LastOrDefault();
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
