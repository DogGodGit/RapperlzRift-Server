using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class BountyHunterQuest
{
	private int m_nId;

	private string m_sTitleKey;

	private int m_nTargetMonsterMinLevel;

	private int m_nTargetCount;

	private Continent m_targetContienent;

	private Vector3 m_targetPosisition = Vector3.zero;

	private float m_fTargetRadius;

	public int id => m_nId;

	public string titleKey => m_sTitleKey;

	public int targetMonsterMinLevel => m_nTargetMonsterMinLevel;

	public int targetCount => m_nTargetCount;

	public Continent targetContinent => m_targetContienent;

	public Vector3 targetPosition => m_targetPosisition;

	public float targetRadius => m_fTargetRadius;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["questId"]);
		if (m_nId < 0)
		{
			SFLogUtil.Warn(GetType(), "퀘스트ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_sTitleKey = Convert.ToString(dr["titleKey"]);
		m_nTargetMonsterMinLevel = Convert.ToInt32(dr["targetMonsterMinLevel"]);
		if (m_nTargetMonsterMinLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표몬스터 최소레벨이 유효하지 않습니다. m_nId = " + m_nId + ", m_nTargetMonsterMinLevel = " + m_nTargetMonsterMinLevel);
		}
		m_nTargetCount = Convert.ToInt32(dr["targetCount"]);
		if (m_nTargetCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "목표횟수가 유효하지 않습니다. m_nId = " + m_nId + ", m_nTargetCount = " + m_nTargetCount);
		}
		int nTargetContinentId = Convert.ToInt32(dr["targetContinentId"]);
		m_targetContienent = Resource.instance.GetContinent(nTargetContinentId);
		if (m_targetContienent == null)
		{
			SFLogUtil.Warn(GetType(), "목표대륙이 존재하지 않습니다. m_nId = " + m_nId + ", m_targetContienent = " + m_targetContienent);
		}
		m_targetPosisition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosisition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosisition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
		if (m_fTargetRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "목표 반지름이 유효하지 않습니다. m_nId = " + m_nId + ", m_fTargetRadius = " + m_fTargetRadius);
		}
	}
}
