using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildHuntingQuestObjective : IPickEntry
{
	private GuildHuntingQuestObjectivePool m_pool;

	private int m_nId;

	private int m_nMinHeroLevel;

	private int m_nPoint;

	private GuildHuntingQuestObjectiveType m_type;

	private Continent m_targetContinent;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private ContinentObject m_targetContinentObject;

	private Monster m_targetMonster;

	private int m_nTargetCount;

	public GuildHuntingQuestObjectivePool pool
	{
		get
		{
			return m_pool;
		}
		set
		{
			m_pool = value;
		}
	}

	public int id => m_nId;

	public int minHeroLevel => m_nMinHeroLevel;

	public int point => m_nPoint;

	public GuildHuntingQuestObjectiveType type => m_type;

	public Continent targetContinent => m_targetContinent;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public ContinentObject targetContinentObject => m_targetContinentObject;

	public Monster targetMonster => m_targetMonster;

	public int targetCount => m_nTargetCount;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["objectiveId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nMinHeroLevel = Convert.ToInt32(dr["minHeroLevel"]);
		if (m_nMinHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "최소영웅레벨이 유효하지 않습니다. m_nMinHeroLevel = " + m_nMinHeroLevel);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nPoint = " + m_nPoint);
		}
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(GuildHuntingQuestObjectiveType), nType))
		{
			SFLogUtil.Warn(GetType(), "목표 타입이 유효하지 않습니다. nType = " + nType);
		}
		m_type = (GuildHuntingQuestObjectiveType)nType;
		int nTargetContinent = Convert.ToInt32(dr["targetContinentId"]);
		m_targetContinent = Resource.instance.GetContinent(nTargetContinent);
		if (m_targetContinent == null)
		{
			SFLogUtil.Warn(GetType(), "목표 대륙이 존재하지 않습니다. nTargetContinent = " + nTargetContinent);
		}
		m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
		if (m_fTargetRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "목표 반경이 유효하지 않습니다. m_fTargetRadius = " + m_fTargetRadius);
		}
		switch (m_type)
		{
		case GuildHuntingQuestObjectiveType.Hunt:
		{
			int nTargetMonsterId = Convert.ToInt32(dr["targetMonsterId"]);
			m_targetMonster = Resource.instance.GetMonster(nTargetMonsterId);
			if (m_targetMonster == null)
			{
				SFLogUtil.Warn(GetType(), "목표 몬스터가 존재하지 않습니다. nTargetMonsterId = " + nTargetMonsterId);
			}
			break;
		}
		case GuildHuntingQuestObjectiveType.Interact:
		{
			int nTargetContinentObjectId = Convert.ToInt32(dr["targetContinentObjectId"]);
			m_targetContinentObject = Resource.instance.GetContinentObject(nTargetContinentObjectId);
			if (m_targetContinentObject == null)
			{
				SFLogUtil.Warn(GetType(), "목표 대륙오브젝트가 존재하지 않습니다. nTargetContinentObjectId = " + nTargetContinentObjectId);
			}
			break;
		}
		}
		m_nTargetCount = Convert.ToInt32(dr["targetCount"]);
		if (m_nTargetCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표 횟수가 유효하지 않습니다. m_nTargetCount = " + m_nTargetCount);
		}
	}
}
