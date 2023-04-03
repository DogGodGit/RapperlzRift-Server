using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FieldOfHonor : Location
{
	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

	public const int kMaxRanking = 9999;

	public const int kOutOfRanking = 10000;

	public const int kTopRankingListCount = 50;

	private int m_nLocationId;

	private string m_sNameKey;

	private string m_sDescriptionKey;

	private string m_sTargetTitleKey;

	private string m_sTargetContentKey;

	private int m_nRequiredConditionType;

	private int m_nRequiredHeroLevel;

	private int m_nRequiredMainQuestNo;

	private int m_nStartDelayTime;

	private int m_nLimitTime;

	private int m_nExitDelayTime;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartYRotation;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetYRotation;

	private HonorPointReward m_winnerHonorPointReward;

	private HonorPointReward m_loserHonorPointReward;

	private Rect3D m_mapRect = Rect3D.zero;

	private Dictionary<int, FieldOfHonorLevelReward> m_levelRewards = new Dictionary<int, FieldOfHonorLevelReward>();

	private List<FieldOfHonorRankingReward> m_rankingRewards = new List<FieldOfHonorRankingReward>();

	private List<FieldOfHonorTarget> m_targets = new List<FieldOfHonorTarget>();

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.FieldOfHonor;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => false;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => false;

	public string namekey => m_sNameKey;

	public string descriptionKey => m_sDescriptionKey;

	public string targetTitlKey => m_sTargetTitleKey;

	public string targetContentKey => m_sTargetContentKey;

	public int requiredConditionType => m_nRequiredConditionType;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int requiredMainQuestNo => m_nRequiredMainQuestNo;

	public int startDelayTime => m_nStartDelayTime;

	public int limitTime => m_nLimitTime;

	public int exitDelayTime => m_nExitDelayTime;

	public Vector3 startPosition => m_startPosition;

	public float startYRotation => m_fStartYRotation;

	public Vector3 targetPosition => m_targetPosition;

	public float targetYRotation => m_fTargetYRotation;

	public HonorPointReward winnerHonorPointReward => m_winnerHonorPointReward;

	public HonorPointReward loserHonorPointReward => m_loserHonorPointReward;

	public Rect3D mapRect => m_mapRect;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_sDescriptionKey = Convert.ToString(dr["descriptionKey"]);
		m_sTargetTitleKey = Convert.ToString(dr["targetTitleKey"]);
		m_sTargetContentKey = Convert.ToString(dr["targetContentKey"]);
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
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		if (m_nLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한시간이 유효하지 않습니다. m_nLimitTime = " + m_nLimitTime);
		}
		m_nExitDelayTime = Convert.ToInt32(dr["exitDelayTime"]);
		m_startPosition.x = Convert.ToSingle(dr["startXPosition"]);
		m_startPosition.y = Convert.ToSingle(dr["startYPosition"]);
		m_startPosition.z = Convert.ToSingle(dr["startZPosition"]);
		m_fStartYRotation = Convert.ToSingle(dr["startYRotation"]);
		m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetYRotation = Convert.ToSingle(dr["targetYRotation"]);
		long lnWinnerHonorPointRewardId = Convert.ToInt64(dr["winnerHonorPointRewardId"]);
		if (lnWinnerHonorPointRewardId > 0)
		{
			m_winnerHonorPointReward = Resource.instance.GetHonorPointReward(lnWinnerHonorPointRewardId);
			if (m_winnerHonorPointReward == null)
			{
				SFLogUtil.Warn(GetType(), "승리명예점수보상이 존재하지 않습니다. lnWinnerHonorPointRewardId = " + lnWinnerHonorPointRewardId);
			}
		}
		else if (lnWinnerHonorPointRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "승리명예점수보상ID가 유효하지 않습니다. lnWinnerHonorPointRewardId = " + lnWinnerHonorPointRewardId);
		}
		long lnLoserHonorPointRewardId = Convert.ToInt64(dr["loserHonorPointRewardId"]);
		if (lnLoserHonorPointRewardId > 0)
		{
			m_loserHonorPointReward = Resource.instance.GetHonorPointReward(lnLoserHonorPointRewardId);
			if (m_loserHonorPointReward == null)
			{
				SFLogUtil.Warn(GetType(), "패배명예점수보상이 존재하지 않습니다. lnLoserHonorPointRewardId = " + lnLoserHonorPointRewardId);
			}
		}
		else if (lnLoserHonorPointRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "패배명예점수보상ID 가유효하지 않습니다. lnLoserHonorPointRewardId = " + lnLoserHonorPointRewardId);
		}
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public void AddLevelReward(FieldOfHonorLevelReward levelReward)
	{
		if (levelReward == null)
		{
			throw new ArgumentNullException("levelReward");
		}
		m_levelRewards.Add(levelReward.level, levelReward);
	}

	public FieldOfHonorLevelReward GetLevelReward(int nLevel)
	{
		if (!m_levelRewards.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddRankingReward(FieldOfHonorRankingReward rankingReward)
	{
		if (rankingReward == null)
		{
			throw new ArgumentNullException("rankingReward");
		}
		m_rankingRewards.Add(rankingReward);
	}

	public FieldOfHonorRankingReward GetRankingReward(int nHighRanking, int nLowRanking, int nRewardNo)
	{
		foreach (FieldOfHonorRankingReward rankingReward in m_rankingRewards)
		{
			if (rankingReward.highRanking == nHighRanking && rankingReward.lowRanking == nLowRanking && rankingReward.rewardNo == nRewardNo)
			{
				return rankingReward;
			}
		}
		return null;
	}

	public void AddTarget(FieldOfHonorTarget target)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		m_targets.Add(target);
	}

	public int SelectTarget(int nRanking, int nSlotIndex)
	{
		foreach (FieldOfHonorTarget target in m_targets)
		{
			if (nRanking <= target.lowRanking && nRanking >= target.highRanking && nSlotIndex == target.slotIndex)
			{
				return SFRandom.Next(nRanking + target.targetHighRankingGap, nRanking + target.targetLowRankingGap + 1);
			}
		}
		return 0;
	}

	public List<ItemReward> GetRankingReward(int nRanking)
	{
		List<ItemReward> rankingRewars = new List<ItemReward>();
		foreach (FieldOfHonorRankingReward rankingReward in m_rankingRewards)
		{
			if (nRanking <= rankingReward.lowRanking && nRanking >= rankingReward.highRanking)
			{
				rankingRewars.Add(rankingReward.itemReward);
			}
		}
		return rankingRewars;
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
