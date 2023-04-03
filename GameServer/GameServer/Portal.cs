using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Portal
{
	public const int kExitYRotationType_Fixed = 1;

	public const int kExitYRotationType_Random = 2;

	public const float kRadiusFactor = 1.1f;

	private int m_nId;

	private string m_sNameKey;

	private Continent m_continent;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private Vector3 m_exitPosition = Vector3.zero;

	private float m_fExitRadius;

	private int m_nExitYRotationType;

	private float m_fExitYRotation;

	private Portal m_linkedPortal;

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public Continent continent
	{
		get
		{
			return m_continent;
		}
		set
		{
			m_continent = value;
		}
	}

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public Vector3 exitPosition => m_exitPosition;

	public float exitRadius => m_fExitRadius;

	public int exitYRotationType => m_nExitYRotationType;

	public float exitYRotation => m_fExitYRotation;

	public Portal linkedPortal
	{
		get
		{
			return m_linkedPortal;
		}
		set
		{
			m_linkedPortal = value;
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["portalId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		m_exitPosition.x = Convert.ToSingle(dr["exitXPosition"]);
		m_exitPosition.y = Convert.ToSingle(dr["exitYPosition"]);
		m_exitPosition.z = Convert.ToSingle(dr["exitZPosition"]);
		m_fExitRadius = Convert.ToSingle(dr["exitRadius"]);
		m_nExitYRotationType = Convert.ToInt32(dr["exitYRotationType"]);
		m_fExitYRotation = Convert.ToSingle(dr["exitYRotation"]);
	}

	public bool IsEnterablePosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, m_fRadius * 1.1f + fRadius * 2f, position);
	}

	public Vector3 SelectExitPosition()
	{
		return Util.SelectPoint(m_exitPosition, m_fExitRadius);
	}

	public float SelectExitYRotation()
	{
		if (m_nExitYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fExitYRotation);
		}
		return m_fExitYRotation;
	}
}
