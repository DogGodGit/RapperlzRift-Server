using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SubQuest
{
	public const int kContent_StoryDungeon = 1;

	public const int kContent_OsirisRoomDungeon = 2;

	public const int kContent_ExpDungeon = 3;

	public const int kContent_TreatOfFarmQuest = 4;

	public const int kContent_UndergroundMaze = 5;

	public const int kContent_BountyHunterQuest = 6;

	public const int kContent_FishingQuest = 7;

	public const int kContent_SecretLetterQuest = 8;

	public const int kContent_MysteryBoxQuest = 9;

	public const int kContent_ArtifactRoomDungeon = 10;

	public const int kContent_AncientRelicDungeon = 11;

	public const int kContent_DimensionRaidQuest = 12;

	public const int kContent_FieldOfHonor = 13;

	public const int kContent_HolyWarQuest = 14;

	public const int kContent_SoulCoverterDungeon = 15;

	public const int kContent_ProofOfValorDungeon = 16;

	public const int kContent_WisdomTempleDungeon = 17;

	public const int kContent_RuinsReclaimDungeon = 18;

	public const int kContent_TrueHeroQuest = 19;

	public const int kContent_FearAltarDungeon = 20;

	public const int kContent_WarMemoryDungeon = 21;

	public const int kContent_DragonNestDungeon = 22;

	public const int kContent_CreatureFarmQuest = 23;

	public const int kContent_DailyQuest = 24;

	public const int kContent_WeeklyQuest = 25;

	public const int kContent_AddFriend = 26;

	public const int kContent_JoinGuild = 27;

	private int m_nId;

	private SubQuestRequiredConditionType m_requiredConditionType = SubQuestRequiredConditionType.MainQuestNo;

	private int m_nRequiredConditionValue;

	private SubQuestObjectiveType m_type = SubQuestObjectiveType.Interaction;

	private Npc m_startNpc;

	private ContinentObject m_targetContinentObject;

	private Monster m_targetMonster;

	private int m_nTargetAcquisitionRate;

	private int m_nTargetContentId;

	private int m_nTargetCount;

	private Npc m_completionNpc;

	private bool m_bAbandonmentEnabled;

	private bool m_bReacceptanceEnabled;

	private ExpReward m_expReward;

	private GoldReward m_goldReward;

	private List<SubQuestReward> m_rewards = new List<SubQuestReward>();

	public int id => m_nId;

	public SubQuestRequiredConditionType requiredConditionType => m_requiredConditionType;

	public int requiredConditionValue => m_nRequiredConditionValue;

	public SubQuestObjectiveType type => m_type;

	public Npc startNpc => m_startNpc;

	public bool isAutoAccepted => m_startNpc == null;

	public ContinentObject targetContinentObject => m_targetContinentObject;

	public Monster targetMonster => m_targetMonster;

	public int targetAcquisitionRate => m_nTargetAcquisitionRate;

	public int targetContentId => m_nTargetContentId;

	public int targetCount => m_nTargetCount;

	public Npc completionNpc => m_completionNpc;

	public bool abandonmentEnabled => m_bAbandonmentEnabled;

	public bool reacceptanceEnabled => m_bReacceptanceEnabled;

	public ExpReward expReward => m_expReward;

	public long expRewardValue
	{
		get
		{
			if (m_expReward == null)
			{
				return 0L;
			}
			return m_expReward.value;
		}
	}

	public GoldReward goldReward => m_goldReward;

	public long goldRewardValue
	{
		get
		{
			if (m_goldReward == null)
			{
				return 0L;
			}
			return m_goldReward.value;
		}
	}

	public List<SubQuestReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["questId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "퀘스트ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nRequiredConditionType = Convert.ToInt32(dr["requiredConditionType"]);
		if (!Enum.IsDefined(typeof(SubQuestRequiredConditionType), nRequiredConditionType))
		{
			SFLogUtil.Warn(GetType(), "필요조건타입이 유효하지 않습니다. m_nId = " + m_nId + ", nRequiredConditionType =  " + nRequiredConditionType);
		}
		m_requiredConditionType = (SubQuestRequiredConditionType)nRequiredConditionType;
		m_nRequiredConditionValue = Convert.ToInt32(dr["requiredConditionValue"]);
		if (m_nRequiredConditionValue <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요조건값이 유효하지 않습니다. m_nId = " + m_nId + ", m_nRequiredConditionValue = " + m_nRequiredConditionValue);
		}
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(SubQuestObjectiveType), nType))
		{
			SFLogUtil.Warn(GetType(), "서브퀘스트타입이 유효하지 않습니다. m_nId = " + m_nId + ", nType = " + nType);
		}
		m_type = (SubQuestObjectiveType)nType;
		int nStartNpcId = Convert.ToInt32(dr["startNpcId"]);
		if (nStartNpcId > 0)
		{
			m_startNpc = Resource.instance.GetNpc(nStartNpcId);
			if (m_startNpc == null)
			{
				SFLogUtil.Warn(GetType(), "시작NPC가 존재하지 않습니다. m_nId = " + m_nId + ", nStartNpcId = " + nStartNpcId);
			}
		}
		switch (m_type)
		{
		case SubQuestObjectiveType.Interaction:
		{
			int nTargetContinentObjectId = Convert.ToInt32(dr["targetContinentObjectId"]);
			m_targetContinentObject = Resource.instance.GetContinentObject(nTargetContinentObjectId);
			if (m_targetContinentObject == null)
			{
				SFLogUtil.Warn(GetType(), "목표대륙오브젝트가 존재하지 않습니다. m_nId = " + m_nId + ", nTargetContinentObjectId = " + nTargetContinentObjectId);
			}
			break;
		}
		case SubQuestObjectiveType.Hunt:
		{
			int nTargetMonsterId = Convert.ToInt32(dr["targetMonsterId"]);
			m_targetMonster = Resource.instance.GetMonster(nTargetMonsterId);
			if (m_targetMonster == null)
			{
				SFLogUtil.Warn(GetType(), "목표몬스터가 존재하지 않습니다. m_nId = " + m_nId + ", nTargetMonsterId = " + nTargetMonsterId);
			}
			break;
		}
		case SubQuestObjectiveType.Acquisition:
		{
			int nTargetMonsterId2 = Convert.ToInt32(dr["targetMonsterId"]);
			m_targetMonster = Resource.instance.GetMonster(nTargetMonsterId2);
			if (m_targetMonster == null)
			{
				SFLogUtil.Warn(GetType(), "목표몬스터가 존재하지 않습니다. m_nId = " + m_nId + ", nTargetMonsterId = " + nTargetMonsterId2);
			}
			m_nTargetAcquisitionRate = Convert.ToInt32(dr["targetAcquisitionRate"]);
			if (m_nTargetAcquisitionRate <= 0)
			{
				SFLogUtil.Warn(GetType(), "목표획득확률이 유효하지 않습니다. m_nId = " + m_nId + ", m_nTargetAcquisitionRate = " + m_nTargetAcquisitionRate);
			}
			break;
		}
		case SubQuestObjectiveType.Content:
			m_nTargetContentId = Convert.ToInt32(dr["targetContentId"]);
			if (m_nTargetContentId <= 0)
			{
				SFLogUtil.Warn(GetType(), "컨텐츠ID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nTargetContentId = " + m_nTargetContentId);
			}
			break;
		}
		m_nTargetCount = Convert.ToInt32(dr["targetCount"]);
		if (m_nTargetCount < 0)
		{
			SFLogUtil.Warn(GetType(), "목표횟수가 유효하지 않습니다. m_nId = " + m_nId + ", m_nTargetCount = " + m_nTargetCount);
		}
		int nCompletionNpcId = Convert.ToInt32(dr["completionNpcId"]);
		if (nCompletionNpcId > 0)
		{
			m_completionNpc = Resource.instance.GetNpc(nCompletionNpcId);
			if (m_completionNpc == null)
			{
				SFLogUtil.Warn(GetType(), "완료NPC가 존재하지 않습니다. m_nId = " + m_nId + ", nCompletionNpcId = " + nCompletionNpcId);
			}
		}
		m_bAbandonmentEnabled = Convert.ToBoolean(dr["abandonmentEnabled"]);
		if (m_bAbandonmentEnabled)
		{
			m_bReacceptanceEnabled = Convert.ToBoolean(dr["reacceptanceEnabled"]);
		}
		long lnExpRewardId = Convert.ToInt64(dr["expRewardId"]);
		if (lnExpRewardId > 0)
		{
			m_expReward = Resource.instance.GetExpReward(lnExpRewardId);
			if (m_expReward == null)
			{
				SFLogUtil.Warn(GetType(), "경험치보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnExpRewardId = " + lnExpRewardId);
			}
		}
		long lnGoldReawrdId = Convert.ToInt64(dr["goldRewardId"]);
		if (lnGoldReawrdId > 0)
		{
			m_goldReward = Resource.instance.GetGoldReward(lnGoldReawrdId);
			if (m_goldReward == null)
			{
				SFLogUtil.Warn(GetType(), "골드보상이 존재하지 않습니다. m_nId = " + m_nId + ", lnGoldReawrdId = " + lnGoldReawrdId);
			}
		}
	}

	public void AddReward(SubQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public SubQuestReward GetReward(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}
}
