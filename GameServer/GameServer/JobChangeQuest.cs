using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class JobChangeQuest
{
	public const float kTargetRadiusFactor = 1.1f;

	private int m_nNo;

	private Npc m_questNpc;

	private JobChangeQuestType m_type = JobChangeQuestType.MonsterHunt;

	private bool m_bIsTargetOwnNation;

	private Continent m_targetContinent;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private Monster m_targetMonster;

	private ContinentObject m_targetContinentObject;

	private int m_nTargetCount;

	private int m_nLimitTime;

	private MonsterArrange m_targetMonsterArrange;

	private Vector3 m_targetGuildTerritoryPosition = Vector3.zero;

	private float m_targetGuildTerritoryRadius;

	private MonsterArrange m_targetGuildMonsterArrange;

	private ItemReward m_completionItemReward;

	public Dictionary<int, JobChangeQuestDifficulty> m_difficulties = new Dictionary<int, JobChangeQuestDifficulty>();

	public int no => m_nNo;

	public Npc questNpc => m_questNpc;

	public JobChangeQuestType type => m_type;

	public bool isTargetOwnNation => m_bIsTargetOwnNation;

	public Continent targetContinent => m_targetContinent;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public Monster targetMonster => m_targetMonster;

	public ContinentObject targetContinentObject => m_targetContinentObject;

	public int targetCount => m_nTargetCount;

	public int limitTime => m_nLimitTime;

	public MonsterArrange targetMonsterArrange => m_targetMonsterArrange;

	public Vector3 targetGuildTerritoryPosition => m_targetGuildTerritoryPosition;

	public float targetGuildTerritoryRadius => m_targetGuildTerritoryRadius;

	public MonsterArrange targetGuildMonsterArrange => m_targetGuildMonsterArrange;

	public ItemReward completionItemReward => m_completionItemReward;

	public Dictionary<int, JobChangeQuestDifficulty> difficulties => m_difficulties;

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
		int nQuestNpcId = Convert.ToInt32(dr["questNpcId"]);
		m_questNpc = Resource.instance.GetNpc(nQuestNpcId);
		if (m_questNpc == null)
		{
			SFLogUtil.Warn(GetType(), "퀘스트NPC가 존재하지 않습니다. m_nNo = " + m_nNo + ", nQuestNpcId = " + nQuestNpcId);
		}
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(JobChangeQuestType), nType))
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", nType = " + nType);
		}
		m_type = (JobChangeQuestType)nType;
		m_bIsTargetOwnNation = Convert.ToBoolean(dr["isTargetOwnNation"]);
		int nTargetContinentId = Convert.ToInt32(dr["targetContinentId"]);
		m_targetContinent = Resource.instance.GetContinent(nTargetContinentId);
		if (m_targetContinent == null)
		{
			SFLogUtil.Warn(GetType(), "목표대륙이 존재하지 않습니다. m_nNo = " + m_nNo + ", nTargetContinentId = " + nTargetContinentId);
		}
		m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
		if (m_fTargetRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "목표반경이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_fTargetRadius = " + m_fTargetRadius);
		}
		switch (m_type)
		{
		case JobChangeQuestType.MonsterHunt:
		{
			int nTargetMonsterId = Convert.ToInt32(dr["targetMonsterId"]);
			m_targetMonster = Resource.instance.GetMonster(nTargetMonsterId);
			if (m_targetMonster == null)
			{
				SFLogUtil.Warn(GetType(), "목표몬스터가 존재하지 않습니다. m_nNo = " + m_nNo + ", nTargetMonsterId = " + nTargetMonsterId);
			}
			break;
		}
		case JobChangeQuestType.Interaction:
		{
			int nTargetContinentObjectId = Convert.ToInt32(dr["targetContinentObjectId"]);
			m_targetContinentObject = Resource.instance.GetContinentObject(nTargetContinentObjectId);
			if (m_targetContinentObject == null)
			{
				SFLogUtil.Warn(GetType(), "목표 대륙오브젝트가 존재하지 않습니다. m_nNo = " + m_nNo + ", nTargetContinentObjectId = " + nTargetContinentObjectId);
			}
			break;
		}
		case JobChangeQuestType.ExclusiveMonsterHunt:
		{
			long lnTargetMonsterArrangeId = Convert.ToInt64(dr["targetMonsterArrangeId"]);
			if (lnTargetMonsterArrangeId > 0)
			{
				m_targetMonsterArrange = Resource.instance.GetMonsterArrange(lnTargetMonsterArrangeId);
				if (m_targetMonsterArrange == null)
				{
					SFLogUtil.Warn(GetType(), "목표목스터배치가 존재하지 않습니다. m_nNo = " + m_nNo + ", lnTargetMonsterArrangeId = " + lnTargetMonsterArrangeId);
				}
			}
			m_targetGuildTerritoryPosition.x = Convert.ToSingle(dr["targetGuildTerritoryXPosition"]);
			m_targetGuildTerritoryPosition.y = Convert.ToSingle(dr["targetGuildTerritoryYPosition"]);
			m_targetGuildTerritoryPosition.z = Convert.ToSingle(dr["targetGuildTerritoryZPosition"]);
			m_targetGuildTerritoryRadius = Convert.ToSingle(dr["targetGuildTerritoryRadius"]);
			if (m_targetGuildTerritoryRadius < 0f)
			{
				SFLogUtil.Warn(GetType(), "목표 길드영토 반경이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_targetGuildTerritoryRadius = " + m_targetGuildTerritoryRadius);
			}
			long lnTargetGuildMonsterArrangeId = Convert.ToInt64(dr["targetGuildMonsterArrangeId"]);
			if (lnTargetGuildMonsterArrangeId > 0)
			{
				m_targetGuildMonsterArrange = Resource.instance.GetMonsterArrange(lnTargetGuildMonsterArrangeId);
				if (m_targetGuildMonsterArrange == null)
				{
					SFLogUtil.Warn(GetType(), "목표길드몬스터배치가 존재하지 않습니다. m_nNo = " + m_nNo + ", lnTargetGuildMonsterArrangeId = " + lnTargetGuildMonsterArrangeId);
				}
			}
			break;
		}
		}
		m_nTargetCount = Convert.ToInt32(dr["targetCount"]);
		if (m_nTargetCount < 0)
		{
			SFLogUtil.Warn(GetType(), "목표횟수가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetCount = " + m_nTargetCount);
		}
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		if (m_nLimitTime < 0)
		{
			SFLogUtil.Warn(GetType(), "제한시간이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nLimitTime = " + m_nLimitTime);
		}
		long lnCompletionItemRewardId = Convert.ToInt64(dr["completionItemRewardId"]);
		if (lnCompletionItemRewardId > 0)
		{
			m_completionItemReward = Resource.instance.GetItemReward(lnCompletionItemRewardId);
			if (m_completionItemReward == null)
			{
				SFLogUtil.Warn(GetType(), "완료아이템보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnCompletionItemRewardId = " + lnCompletionItemRewardId);
			}
		}
	}

	public Vector3 SelectPositionOnContinent()
	{
		return Util.SelectPoint(m_targetPosition, m_fTargetRadius);
	}

	public Vector3 SelectPositionOnGuildTerritory()
	{
		return Util.SelectPoint(m_targetGuildTerritoryPosition, m_targetGuildTerritoryRadius);
	}

	public float SelectRotationY()
	{
		return SFRandom.NextFloat(360f);
	}

	public bool IsTargetContinentAreaPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_targetPosition, m_fTargetRadius * 1.1f + fRadius * 2f, position);
	}

	public bool IsTargetGuildTerritoryAreaPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_targetGuildTerritoryPosition, m_fTargetRadius * 1.1f + fRadius * 2f, position);
	}

	public void AddDifficulty(JobChangeQuestDifficulty difficulty)
	{
		if (difficulty == null)
		{
			throw new ArgumentNullException("difficulty");
		}
		m_difficulties.Add(difficulty.difficulty, difficulty);
	}

	public JobChangeQuestDifficulty GetDifficulty(int nDifficulty)
	{
		if (!m_difficulties.TryGetValue(nDifficulty, out var value))
		{
			return null;
		}
		return value;
	}
}
