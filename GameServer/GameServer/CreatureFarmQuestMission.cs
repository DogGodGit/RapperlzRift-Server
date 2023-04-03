using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureFarmQuestMission
{
	public const float kInteractionMaxRangeFactor = 1.1f;

	private int m_nNo;

	private CreatureFarmQuestMissionTargetType m_targetType = CreatureFarmQuestMissionTargetType.Move;

	private Continent m_targetContinent;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private ContinentObject m_targetContinentObject;

	private int m_nTargetAutoCompletionTime;

	private int m_nTargetCount;

	private Dictionary<int, CreatureFarmQuestMissionMonsterArrange> m_monsterArranges = new Dictionary<int, CreatureFarmQuestMissionMonsterArrange>();

	private List<CreatureFarmQuestMissionReward> m_rewards = new List<CreatureFarmQuestMissionReward>();

	public int no => m_nNo;

	public CreatureFarmQuestMissionTargetType targetType => m_targetType;

	public Continent targetContinent => m_targetContinent;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public ContinentObject targetContinentObject => m_targetContinentObject;

	public int targetCount => m_nTargetCount;

	public int targetAutoCompletionTime => m_nTargetAutoCompletionTime;

	public Dictionary<int, CreatureFarmQuestMissionMonsterArrange> monsterArranges => m_monsterArranges;

	public List<CreatureFarmQuestMissionReward> rewards => m_rewards;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["missionNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "미션번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		int nTargetType = Convert.ToInt32(dr["targetType"]);
		if (!Enum.IsDefined(typeof(CreatureFarmQuestMissionTargetType), nTargetType))
		{
			SFLogUtil.Warn(GetType(), "목표타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", nTargetType = " + nTargetType);
		}
		m_targetType = (CreatureFarmQuestMissionTargetType)nTargetType;
		int nTargetContinentId = Convert.ToInt32(dr["targetContinentId"]);
		if (nTargetContinentId > 0)
		{
			m_targetContinent = Resource.instance.GetContinent(nTargetContinentId);
			if (m_targetContinent == null)
			{
				SFLogUtil.Warn(GetType(), "목표 대륙이 존재하지 않습니다. m_nNo = " + m_nNo + ", nTargetContinentId = " + nTargetContinentId);
			}
		}
		m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
		if (m_fTargetRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "목표 반경이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_fTargetRadius = " + m_fTargetRadius);
		}
		switch (m_targetType)
		{
		case CreatureFarmQuestMissionTargetType.Interaction:
		{
			int nTargetContinentObjectId = Convert.ToInt32(dr["targetContinentObjectId"]);
			m_targetContinentObject = Resource.instance.GetContinentObject(nTargetContinentObjectId);
			if (m_targetContinentObject == null)
			{
				SFLogUtil.Warn(GetType(), "목표 대륙오브젝트가 존재하지 않습니다. m_nNo = " + m_nNo + ", nTargetContinentObjectId = " + nTargetContinentObjectId);
			}
			break;
		}
		case CreatureFarmQuestMissionTargetType.ExclusiveMonsterHunt:
			m_nTargetAutoCompletionTime = Convert.ToInt32(dr["targetAutoCompletionTime"]);
			if (m_nTargetAutoCompletionTime <= 0)
			{
				SFLogUtil.Warn(GetType(), "목표자동완성시간이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetAutoCompletionTime = " + m_nTargetAutoCompletionTime);
			}
			break;
		}
		m_nTargetCount = Convert.ToInt32(dr["targetCount"]);
		if (m_nTargetCount < 0)
		{
			SFLogUtil.Warn(GetType(), "목표횟수가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nTargetCount = " + m_nTargetCount);
		}
	}

	public void AddMonsterArrange(CreatureFarmQuestMissionMonsterArrange monsterArrange)
	{
		if (monsterArrange == null)
		{
			throw new ArgumentNullException("monsterArrange");
		}
		m_monsterArranges.Add(monsterArrange.level, monsterArrange);
	}

	public CreatureFarmQuestMissionMonsterArrange GetMonsterArrange(int nLevel)
	{
		if (!m_monsterArranges.TryGetValue(nLevel, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddReward(CreatureFarmQuestMissionReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward);
	}

	public CreatureFarmQuestMissionReward GetReward(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_rewards.Count)
		{
			return null;
		}
		return m_rewards[nIndex];
	}

	public bool IsInteractionEnabledPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_targetPosition, m_fTargetRadius * 1.1f + fRadius * 2f, position);
	}
}
