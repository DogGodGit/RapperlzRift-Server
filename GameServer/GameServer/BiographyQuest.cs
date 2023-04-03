using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class BiographyQuest
{
	public const float kInteractionMaxRangeFactor = 1.1f;

	private Biography m_biography;

	private int m_nNo;

	private BiographyQuestType m_type = BiographyQuestType.Move;

	private Npc m_startNpc;

	private Npc m_completionNpc;

	private Continent m_targetContinent;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private Monster m_targetMonster;

	private Npc m_targetNpc;

	private ContinentObject m_targetContinentObject;

	private int m_nTargetDungeonId;

	private int m_nTargetCount;

	private ExpReward m_expReawrd;

	public Biography biography => m_biography;

	public int no => m_nNo;

	public BiographyQuestType type => m_type;

	public Npc startNpc => m_startNpc;

	public Npc completionNpc => m_completionNpc;

	public Continent targetContinent => m_targetContinent;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public Monster targetMonster => m_targetMonster;

	public Npc targetNpc => m_targetNpc;

	public ContinentObject targetContinentObject => m_targetContinentObject;

	public int targetDungeonId => m_nTargetDungeonId;

	public int targetCount => m_nTargetCount;

	public ExpReward expReward => m_expReawrd;

	public long expRewardValue
	{
		get
		{
			if (m_expReawrd == null)
			{
				return 0L;
			}
			return m_expReawrd.value;
		}
	}

	public bool isLastQuest => m_nNo >= m_biography.lastQuest.no;

	public BiographyQuest(Biography biography)
	{
		if (biography == null)
		{
			throw new ArgumentNullException("biography");
		}
		m_biography = biography;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["questNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "퀘스트번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(BiographyQuestType), nType))
		{
			SFLogUtil.Warn(GetType(), "퀘스트타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", nType = " + nType);
		}
		m_type = (BiographyQuestType)nType;
		int nStartNpcId = Convert.ToInt32(dr["startNpcId"]);
		if (nStartNpcId > 0)
		{
			m_startNpc = Resource.instance.GetNpc(nStartNpcId);
			if (m_startNpc == null)
			{
				SFLogUtil.Warn(GetType(), "시작NPC가 유효하지 않습니다. m_nNo = " + m_nNo + ", nStartNpcId = " + nStartNpcId);
			}
		}
		int nCompletionNpcId = Convert.ToInt32(dr["completionNpcId"]);
		if (nCompletionNpcId > 0)
		{
			m_completionNpc = Resource.instance.GetNpc(nCompletionNpcId);
			if (m_completionNpc == null)
			{
				SFLogUtil.Warn(GetType(), "완료NPC가 유효하지 않습니다. m_nNo = " + m_nNo + ", nCompletionNpcId = " + nCompletionNpcId);
			}
		}
		switch (m_type)
		{
		case BiographyQuestType.Move:
		{
			int nTargetContinentId = Convert.ToInt32(dr["targetContinentId"]);
			m_targetContinent = Resource.instance.GetContinent(nTargetContinentId);
			if (m_targetContinent == null)
			{
				SFLogUtil.Warn(GetType(), "목표 대륙이 존재하지 않습니다. m_nNo = " + m_nNo + ", nTargetContinentId = " + nTargetContinentId);
			}
			m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
			m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
			m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
			m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
			if (m_fTargetRadius <= 0f)
			{
				SFLogUtil.Warn(GetType(), "목표 반경이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_fTargetRadius = " + m_fTargetRadius);
			}
			break;
		}
		case BiographyQuestType.Hunt:
		{
			int nTargetMonsterId = Convert.ToInt32(dr["targetMonsterId"]);
			m_targetMonster = Resource.instance.GetMonster(nTargetMonsterId);
			if (m_targetMonster == null)
			{
				SFLogUtil.Warn(GetType(), "목표 몬스터가 존재하지 않습니다. m_nNo = " + m_nNo + ", nTargetMonsterId = " + nTargetMonsterId);
			}
			break;
		}
		case BiographyQuestType.ObjectInteraction:
		{
			int nTargetContinentObjectId = Convert.ToInt32(dr["targetContinentObjectId"]);
			m_targetContinentObject = Resource.instance.GetContinentObject(nTargetContinentObjectId);
			if (m_targetContinentObject == null)
			{
				SFLogUtil.Warn(GetType(), "목표 대륙오브젝트가 존재하지 않습니다. m_nNo = " + m_nNo + ", nTargetContinentObjectId = " + nTargetContinentObjectId);
			}
			break;
		}
		case BiographyQuestType.NpcConversation:
		{
			int nTargetNpcId = Convert.ToInt32(dr["targetNpcId"]);
			m_targetNpc = Resource.instance.GetNpc(nTargetNpcId);
			if (m_targetNpc == null)
			{
				SFLogUtil.Warn(GetType(), "목표 NPC가 존재하지 않습니다. m_nNo = " + m_nNo + ", nTargetNpcId = " + nTargetNpcId);
			}
			break;
		}
		case BiographyQuestType.BiographyDungeon:
			m_nTargetDungeonId = Convert.ToInt32(dr["targetDungeonId"]);
			if (Resource.instance.GetBiographyQuestDungeon(m_nTargetDungeonId) == null)
			{
				SFLogUtil.Warn(GetType(), "목표 던전이 존재하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetDungeonId = " + m_nTargetDungeonId);
			}
			break;
		}
		m_nTargetCount = Convert.ToInt32(dr["targetCount"]);
		if (m_nTargetCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표 횟수가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetCount = " + m_nTargetCount);
		}
		long lnExpRewardId = Convert.ToInt32(dr["expRewardId"]);
		if (lnExpRewardId > 0)
		{
			m_expReawrd = Resource.instance.GetExpReward(lnExpRewardId);
			if (m_expReawrd == null)
			{
				SFLogUtil.Warn(GetType(), "경험치보상이 유효하지 않습니다. m_nNo = " + m_nNo + ", lnExpRewardId = " + lnExpRewardId);
			}
		}
	}

	public bool IsInteractionEnabledPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_targetPosition, m_fTargetRadius * 1.1f + fRadius * 2f, position);
	}
}
