using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainQuest
{
	public const int kType_None = 0;

	public const int kType_Move = 1;

	public const int kType_Hunt = 2;

	public const int kType_Acquisition = 3;

	public const int kType_Interaction = 4;

	public const int kType_MainQuestDungeon = 5;

	public const int kType_Transport = 6;

	public const int kType_Content = 7;

	public const float kAreaMaxRangeFactor = 1.1f;

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

	private int m_nNo;

	private int m_nRequiredHeroLevel;

	private string m_sTitleKey;

	private int m_nType;

	private Npc m_startNpc;

	private string m_sStartTextKey;

	private string m_sTargetTextKey;

	private Continent m_targetContinent;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private Npc m_targetNpc;

	private int m_nTargetObjectId;

	private MainQuestDungeon m_targetDungeon;

	private Monster m_targetMonster;

	private int m_nTargetAcquisitionRate;

	private int m_nTargetContentId;

	private int m_nTargetCount;

	private Monster m_transformationMonster;

	private int m_nTransformationLifetime;

	private bool m_bTransformationRestored;

	private Cart m_cart;

	private Npc m_completionNpc;

	private string m_sCompletionTextKey;

	private ExpReward m_expReward;

	private GoldReward m_goldReward;

	private List<MainQuestReward> m_rewards = new List<MainQuestReward>();

	public int no => m_nNo;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public string titleKey => m_sTitleKey;

	public int type => m_nType;

	public Npc startNpc => m_startNpc;

	public string startTextKey => m_sStartTextKey;

	public string targetTextKey => m_sTargetTextKey;

	public Continent targetContinent => m_targetContinent;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public Npc targetNpc => m_targetNpc;

	public int targetObjectId => m_nTargetObjectId;

	public MainQuestDungeon targetDungeon => m_targetDungeon;

	public Monster targetMonster => m_targetMonster;

	public int targetAcquisitionRate => m_nTargetAcquisitionRate;

	public int targetContentId => m_nTargetContentId;

	public int targetCount => m_nTargetCount;

	public Monster transformationMonster => m_transformationMonster;

	public int transformationLifetime => m_nTransformationLifetime;

	public bool transformationRestored => m_bTransformationRestored;

	public Cart cart => m_cart;

	public Npc completionNpc => m_completionNpc;

	public string completionTextKey => m_sCompletionTextKey;

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

	public List<MainQuestReward> rewards => m_rewards;

	public bool isLast => m_nNo >= Resource.instance.lastMainQuest.no;

	public MainQuest nextMainQuest => Resource.instance.GetMainQuest(m_nNo + 1);

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["mainQuestNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "메인퀘스트번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "메인퀘스트 요구레벨이 유효하지 않습니다. m_nNo  = " + m_nNo + ", m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		m_sTitleKey = Convert.ToString(dr["titleKey"]);
		m_nType = Convert.ToInt32(dr["type"]);
		if (m_nType < 0)
		{
			SFLogUtil.Warn(GetType(), "메인퀘스트 타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nType = " + m_nType);
		}
		int nStartNpcId = Convert.ToInt32(dr["startNpcId"]);
		if (nStartNpcId > 0)
		{
			m_startNpc = Resource.instance.GetNpc(nStartNpcId);
			if (m_startNpc == null)
			{
				SFLogUtil.Warn(GetType(), "시작 NPC가 존재하지 않습니다. m_nNo = " + m_nNo + ", nStartNpcId = " + nStartNpcId);
			}
		}
		m_sStartTextKey = Convert.ToString(dr["startTextKey"]);
		m_sTargetTextKey = Convert.ToString(dr["targetTextKey"]);
		int nTargetContinentId = Convert.ToInt32(dr["targetContinentId"]);
		m_targetContinent = Resource.instance.GetContinent(nTargetContinentId);
		if (m_targetContinent == null)
		{
			SFLogUtil.Warn(GetType(), "목표 대륙이 존재하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetContinentId = " + nTargetContinentId);
		}
		m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
		if (m_fTargetRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "목표 반경범위가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_fTargetRadius = " + m_fTargetRadius);
		}
		int nTargetNpcId = Convert.ToInt32(dr["targetNpcId"]);
		if (nTargetNpcId > 0)
		{
			m_targetNpc = Resource.instance.GetNpc(nTargetNpcId);
			if (m_targetNpc == null)
			{
				SFLogUtil.Warn(GetType(), "목표 NPC가 존재하지 않습니다. m_nNo = " + m_nNo + ", nTargetNpcId = " + nTargetNpcId);
			}
		}
		m_nTargetObjectId = Convert.ToInt32(dr["targetContinentObjectId"]);
		if (m_nTargetObjectId > 0 && Resource.instance.GetContinentObject(m_nTargetObjectId) == null)
		{
			SFLogUtil.Warn(GetType(), "목표 오브젝트가 존재하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetObjectId = " + m_nTargetObjectId);
		}
		int nTargetDungeonId = Convert.ToInt32(dr["targetDungeonId"]);
		if (nTargetDungeonId > 0)
		{
			m_targetDungeon = Resource.instance.GetMainQuestDungeon(nTargetDungeonId);
			if (m_targetDungeon == null)
			{
				SFLogUtil.Warn(GetType(), "목표 던전이 존재하지 않습니다. m_nNo = " + m_nNo + ", nTargetDungeonId = " + nTargetDungeonId);
			}
		}
		int nTargetMonsterId = Convert.ToInt32(dr["targetMonsterId"]);
		if (nTargetMonsterId > 0)
		{
			m_targetMonster = Resource.instance.GetMonster(nTargetMonsterId);
			if (m_targetMonster == null)
			{
				SFLogUtil.Warn(GetType(), "목표 몬스터가 존재하지 않습니다. m_nNo = " + m_nNo + " , nTargetMonsterId = " + nTargetMonsterId);
			}
		}
		m_nTargetAcquisitionRate = Convert.ToInt32(dr["targetAcquisitionRate"]);
		if (m_nTargetAcquisitionRate < 0)
		{
			SFLogUtil.Warn(GetType(), "목표 획득 확률이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetAcquisitionRate = " + m_nTargetAcquisitionRate);
		}
		m_nTargetContentId = Convert.ToInt32(dr["targetContentId"]);
		if (m_nTargetContentId < 0)
		{
			SFLogUtil.Warn(GetType(), "목표 컨텐츠ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetContentId = " + m_nTargetContentId);
		}
		m_nTargetCount = Convert.ToInt32(dr["targetCount"]);
		if (m_nTargetCount < 0)
		{
			SFLogUtil.Warn(GetType(), "목표 수량이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetCount = " + m_nTargetCount);
		}
		int nTransformationMonsterId = Convert.ToInt32(dr["transformationMonsterId"]);
		if (nTransformationMonsterId > 0)
		{
			m_transformationMonster = Resource.instance.GetMonster(nTransformationMonsterId);
			if (m_transformationMonster == null)
			{
				SFLogUtil.Warn(GetType(), "변신몬스터가 존재하지 않습니다. m_nNo = " + m_nNo + ", nTransformationMonsterId = " + nTransformationMonsterId);
			}
		}
		else if (nTransformationMonsterId < 0)
		{
			SFLogUtil.Warn(GetType(), "변신몬스터ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", nTransformationMonsterId = " + nTransformationMonsterId);
		}
		m_nTransformationLifetime = Convert.ToInt32(dr["transformationLifetime"]);
		if (m_nTransformationLifetime < 0)
		{
			SFLogUtil.Warn(GetType(), "변신지속시간이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nTransformationLifetime = " + m_nTransformationLifetime);
		}
		m_bTransformationRestored = Convert.ToBoolean(dr["transformationRestored"]);
		if (m_nType == 6)
		{
			int nCartId = Convert.ToInt32(dr["cartId"]);
			m_cart = Resource.instance.GetCart(nCartId);
			if (m_cart == null)
			{
				SFLogUtil.Warn(GetType(), "카트가 존재하지 않습니다. m_nNo = " + m_nNo + ", nCartId = " + nCartId);
			}
		}
		int nCompletionNpcId = Convert.ToInt32(dr["completionNpcId"]);
		if (nCompletionNpcId > 0)
		{
			m_completionNpc = Resource.instance.GetNpc(nCompletionNpcId);
			if (m_completionNpc == null)
			{
				SFLogUtil.Warn(GetType(), "완료 NPC가 존재하지 않습니다. m_nNo = " + m_nNo + ", nCompletionNpcId = " + nCompletionNpcId);
			}
		}
		m_sCompletionTextKey = Convert.ToString(dr["completionTextKey"]);
		long lnExpRewardId = Convert.ToInt64(dr["expRewardId"]);
		if (lnExpRewardId > 0)
		{
			m_expReward = Resource.instance.GetExpReward(lnExpRewardId);
		}
		long lnGoldRewardId = Convert.ToInt64(dr["goldRewardId"]);
		if (lnGoldRewardId > 0)
		{
			m_goldReward = Resource.instance.GetGoldReward(lnGoldRewardId);
		}
	}

	public bool TargetAreaContains(Vector3 position)
	{
		return MathUtil.CircleContains(m_targetPosition, m_fTargetRadius * 1.1f, position);
	}

	public void AddReward(MainQuestReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		if (reward.mainQuest != null)
		{
			throw new Exception("해당 메인퀘스트보상아이템은 이미 메인퀘스트에 포함되어있습니다.");
		}
		m_rewards.Add(reward);
		reward.mainQuest = this;
	}
}
