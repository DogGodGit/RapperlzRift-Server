using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class UndergroundMazePortal
{
	public const int kExitYRotationType_Fixed = 1;

	public const int kExitYRotationType_Random = 2;

	public const float kRadiusFactor = 1.1f;

	private int m_nId;

	private UndergroundMazeFloor m_floor;

	private string m_sNameKey;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private float m_fYRotation;

	private Vector3 m_exitPosition = Vector3.zero;

	private float m_fExitRadius;

	private int m_nExitYRotationType;

	private float m_fExitYRotation;

	private UndergroundMazePortal m_linkedPortal;

	public int id => m_nId;

	public UndergroundMaze undergroundMaze => m_floor.undergroundMaze;

	public UndergroundMazeFloor floor => m_floor;

	public string nameKey => m_sNameKey;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public float yRotation => m_fYRotation;

	public Vector3 exitPosition => m_exitPosition;

	public float exitRadius => m_fExitRadius;

	public int exitYRotationType => m_nExitYRotationType;

	public float exitYRotation => m_fExitYRotation;

	public UndergroundMazePortal linkedPortal
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

	public UndergroundMazePortal(UndergroundMazeFloor floor)
	{
		m_floor = floor;
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
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_exitPosition.x = Convert.ToSingle(dr["exitXPosition"]);
		m_exitPosition.y = Convert.ToSingle(dr["exitYPosition"]);
		m_exitPosition.z = Convert.ToSingle(dr["exitZPosition"]);
		m_fExitRadius = Convert.ToSingle(dr["exitRadius"]);
		m_nExitYRotationType = Convert.ToInt32(dr["exitYRotationType"]);
		if (m_nExitYRotationType < 1 || m_nExitYRotationType > 2)
		{
			SFLogUtil.Warn(GetType(), "출구방향타입이 유효하지 않습니다. m_nExitYRotationType = " + m_nExitYRotationType);
		}
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
