using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TreatOfFarmQuestMission : IPickEntry
{
	public const float kTargetRadiusFactor = 1.1f;

	private int m_nId;

	private int m_nPoint;

	private Continent m_targetContinent;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private Vector3 m_monsterSpawnPosition = Vector3.zero;

	private int m_nMonsterRotationType;

	private float m_fMonsterRotationY;

	public List<TreatOfFarmQuestMonsterArrange> m_monsterArranges = new List<TreatOfFarmQuestMonsterArrange>();

	public int id => m_nId;

	public int point => m_nPoint;

	public Continent targetContinent => m_targetContinent;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public Vector3 monsterSpawnPosition => m_monsterSpawnPosition;

	public int monsterRorationType => m_nMonsterRotationType;

	public float monsterRotationY => m_fMonsterRotationY;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["missionId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "항목번호가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nId = " + m_nId + ", m_nPoint = " + m_nPoint);
		}
		int nTargetContinentId = Convert.ToInt32(dr["targetContinentId"]);
		m_targetContinent = Resource.instance.GetContinent(nTargetContinentId);
		if (m_targetContinent == null)
		{
			SFLogUtil.Warn(GetType(), "목표대륙이 존재하지 않습니다. m_nId = " + m_nId + ", nTargetContinentId = " + nTargetContinentId);
		}
		m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
		if (m_fTargetRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "목표반경이 유효하지 않습니다. m_nId = " + m_nId + ", m_fTargetRadius = " + m_fTargetRadius);
		}
		m_monsterSpawnPosition.x = Convert.ToSingle(dr["monsterSpawnXPosition"]);
		m_monsterSpawnPosition.y = Convert.ToSingle(dr["monsterSpawnYPosition"]);
		m_monsterSpawnPosition.z = Convert.ToSingle(dr["monsterSpawnZPosition"]);
		m_nMonsterRotationType = Convert.ToInt32(dr["monsterYRotationType"]);
		if (m_nMonsterRotationType < 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터 방향 타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nMonsterRotationType = " + m_nMonsterRotationType);
		}
		m_fMonsterRotationY = Convert.ToSingle(dr["monsterYRotation"]);
	}

	public void AddMonsterArrange(TreatOfFarmQuestMonsterArrange arrange)
	{
		if (arrange == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_monsterArranges.Add(arrange);
		arrange.mission = this;
	}

	public TreatOfFarmQuestMonsterArrange GetMonsterArrange(int nHeroLevel)
	{
		TreatOfFarmQuestMonsterArrange monsterArrange = null;
		foreach (TreatOfFarmQuestMonsterArrange arrange in m_monsterArranges)
		{
			if (nHeroLevel >= arrange.heroMinLevel)
			{
				monsterArrange = arrange;
				continue;
			}
			return monsterArrange;
		}
		return monsterArrange;
	}

	public bool IsTargetAreaPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_targetPosition, m_fTargetRadius * 1.1f + fRadius * 2f, position);
	}
}
