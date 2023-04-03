using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DailyQuestMission : IPickEntry
{
	private int m_nId;

	private int m_nRequiredHeroLevel;

	private DailyQuestGrade m_grade;

	private DailyQuestMissionType m_type;

	private Monster m_targetMonster;

	private ContinentObject m_targetContinentObject;

	private int m_nTargetCount;

	public int id => m_nId;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public DailyQuestGrade grade => m_grade;

	public DailyQuestMissionType type => m_type;

	public Monster targetMonster => m_targetMonster;

	public ContinentObject targetContinentObject => m_targetContinentObject;

	public int targetCount => m_nTargetCount;

	public int point => m_grade.point;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["missionId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "미션ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "요구영웅레벨이 유효하지 않습니다. m_nId = " + m_nId + ", m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		int nGrade = Convert.ToInt32(dr["grade"]);
		m_grade = Resource.instance.dailyQuest.GetGrade(nGrade);
		if (m_grade == null)
		{
			SFLogUtil.Warn(GetType(), "등급이 유효하지 않습니다. m_nId = " + m_nId + ", nGrade = " + nGrade);
		}
		int nType = Convert.ToInt32(dr["type"]);
		if (!Enum.IsDefined(typeof(DailyQuestMissionType), nType))
		{
			SFLogUtil.Warn(GetType(), "미션타입이 유효하지 않습니다. m_nId = " + m_nId + ", nType = " + nType);
		}
		m_type = (DailyQuestMissionType)nType;
		switch (m_type)
		{
		case DailyQuestMissionType.Hunt:
		{
			int nTargetMonsterId = Convert.ToInt32(dr["targetMonsterId"]);
			m_targetMonster = Resource.instance.GetMonster(nTargetMonsterId);
			if (m_targetMonster == null)
			{
				SFLogUtil.Warn(GetType(), "목표 몬스터가 존재하지 않습니다. m_nId = " + m_nId + ", nTargetMonsterId = " + nTargetMonsterId);
			}
			break;
		}
		case DailyQuestMissionType.Interaction:
		{
			int nTargetContinentObjectId = Convert.ToInt32(dr["targetContinentObjectId"]);
			m_targetContinentObject = Resource.instance.GetContinentObject(nTargetContinentObjectId);
			if (m_targetContinentObject == null)
			{
				SFLogUtil.Warn(GetType(), "목표 대륙오브젝트가 존재하지 않습니다. m_nId = " + m_nId + ", nTargetContinentObjectId = " + nTargetContinentObjectId);
			}
			break;
		}
		}
		m_nTargetCount = Convert.ToInt32(dr["targetCount"]);
		if (m_nTargetCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표횟수가 유효하지 않습니다. m_nId = " + m_nId + ", m_nTargetCount = " + m_nTargetCount);
		}
	}
}
