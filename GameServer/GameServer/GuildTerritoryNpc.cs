using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildTerritoryNpc
{
	public const float kInteractionMaxRangeFactor = 1.1f;

	private int m_nId;

	private Vector3 m_position = Vector3.zero;

	private float m_fRotationY;

	private float m_fInteractionMaxRange;

	public int id => m_nId;

	public Vector3 position => m_position;

	public float rotationY => m_fRotationY;

	public float interactionMaxRange => m_fInteractionMaxRange;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["npcId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRotationY = Convert.ToSingle(dr["yRotation"]);
		m_fInteractionMaxRange = Convert.ToSingle(dr["interactionMaxRange"]);
		if (m_fInteractionMaxRange < 0f)
		{
			SFLogUtil.Warn(GetType(), "상호작용최대거리가 유효하지 않습니다. m_nId = " + m_nId + ", m_fInteractionMaxRange = " + m_fInteractionMaxRange);
		}
	}

	public bool IsInteractionEnabledPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, m_fInteractionMaxRange * 1.1f + fRadius * 2f, position);
	}
}
