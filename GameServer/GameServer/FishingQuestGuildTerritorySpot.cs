using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FishingQuestGuildTerritorySpot
{
	public const float kTargetRadiusFactor = 1.1f;

	private int m_nId;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	public int id => m_nId;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["spotId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "퀘스트 번호가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "반지름이 유효하지 않습니다. m_nId = " + m_nId + ", m_fRadius = " + m_fRadius);
		}
	}

	public bool IsTargetAreaPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, m_fRadius * 1.1f + fRadius * 2f, position);
	}
}
