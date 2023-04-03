using System;
using System.Data;

namespace GameServer;

public class AncientRelicStepRoute
{
	private AncientRelicStep m_step;

	private int m_nId;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private int m_nRemoveObstacleId;

	public AncientRelic ancientRelic => m_step.ancientRelic;

	public AncientRelicStep step => m_step;

	public int id => m_nId;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public int removeObstacleId => m_nRemoveObstacleId;

	public AncientRelicStepRoute(AncientRelicStep step)
	{
		m_step = step;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["routeId"]);
		m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
		m_nRemoveObstacleId = Convert.ToInt32(dr["removeObstacleId"]);
	}
}
