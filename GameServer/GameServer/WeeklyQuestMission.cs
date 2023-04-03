using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WeeklyQuestMission : IPickEntry
{
	public const float kAreaMaxRangeFactor = 1.1f;

	private int m_nId;

	private int m_nMinHeroLevel;

	private int m_nMaxHeroLevel;

	private WeeklyQuestMissionType m_type = WeeklyQuestMissionType.Move;

	private Continent m_targetContinent;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private Monster m_targetMonster;

	private ContinentObject m_targetContinentObject;

	private int m_nTargetCount;

	public int id => m_nId;

	public int minHeroLevel => m_nMinHeroLevel;

	public int maxHeroLevel => m_nMaxHeroLevel;

	public WeeklyQuestMissionType type => m_type;

	public Continent targetContinent => m_targetContinent;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public Monster targetMonster => m_targetMonster;

	public ContinentObject targetContinentObject => m_targetContinentObject;

	public int targetCount => m_nTargetCount;

	public int point => 10;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nMaxHeroLevel = Resource.instance.lastJobLevelMaster.level;
		m_nId = Convert.ToInt32(dr["missionId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "미션ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nMinHeroLevel = Convert.ToInt32(dr["minHeroLevel"]);
		if (m_nMinHeroLevel <= 0 || m_nMinHeroLevel > nMaxHeroLevel)
		{
			SFLogUtil.Warn(GetType(), "최소영웅레벨이 유효하지 않습니다. m_nId = " + m_nId + ", m_nMinHeroLevel = " + m_nMinHeroLevel);
		}
		m_nMaxHeroLevel = Convert.ToInt32(dr["maxHeroLevel"]);
		if (m_nMaxHeroLevel <= 0 || m_nMaxHeroLevel > nMaxHeroLevel)
		{
			SFLogUtil.Warn(GetType(), "최대영웅레벨이 유효하지 않습니다. m_nId = " + m_nId + ", m_nMaxHeroLevel = " + m_nMaxHeroLevel);
		}
		if (m_nMinHeroLevel > m_nMaxHeroLevel)
		{
			SFLogUtil.Warn(GetType(), "최대최소영웅레벨 비교값이 유효하지 않습니다. m_nId = " + m_nId + ", m_nMinHeroLevel = " + m_nMinHeroLevel + ", m_nMaxHeroLevel = " + m_nMaxHeroLevel);
		}
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(WeeklyQuestMissionType), nType))
		{
			SFLogUtil.Warn(GetType(), "미션타입이 유효하지 않습니다. m_nId = " + m_nId + ", nType = " + nType);
		}
		m_type = (WeeklyQuestMissionType)nType;
		int nTargetContinentId = Convert.ToInt32(dr["targetContinentId"]);
		m_targetContinent = Resource.instance.GetContinent(nTargetContinentId);
		if (m_targetContinent == null)
		{
			SFLogUtil.Warn(GetType(), "목표 대륙이 유효하지 않습니다. m_nId = " + m_nId + ", nTargetContinentId = " + nTargetContinentId);
		}
		m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
		switch (m_type)
		{
		case WeeklyQuestMissionType.Hunt:
		{
			int nTargetMonsterId = Convert.ToInt32(dr["targetMonsterId"]);
			m_targetMonster = Resource.instance.GetMonster(nTargetMonsterId);
			if (m_targetMonster == null)
			{
				SFLogUtil.Warn(GetType(), "목표몬스터가 존재하지 않습니다. m_nId = " + m_nId + ", nTargetMonsterId = " + nTargetMonsterId);
			}
			break;
		}
		case WeeklyQuestMissionType.Interaction:
		{
			int nTargetContinentObjectId = Convert.ToInt32(dr["targetContinentObjectId"]);
			m_targetContinentObject = Resource.instance.GetContinentObject(nTargetContinentObjectId);
			if (m_targetContinentObject == null)
			{
				SFLogUtil.Warn(GetType(), "목표대륙오브젝트가 존재하지 않습니다. m_nId = " + m_nId + ", nTargetContinentObjectId = " + nTargetContinentObjectId);
			}
			break;
		}
		}
		m_nTargetCount = Convert.ToInt32(dr["targetCount"]);
		if (m_nTargetCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표수량이 유효하지 않습니다. m_nId = " + m_nId + ", m_nTargetCount = " + m_nTargetCount);
		}
	}

	public bool TargetAreaContains(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_targetPosition, m_fTargetRadius * 1.1f + fRadius * 2f, position);
	}
}
