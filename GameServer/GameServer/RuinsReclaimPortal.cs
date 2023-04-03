using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimPortal
{
	public const int kExitYRotationType_Fixed = 1;

	public const int kExitYRotationType_Random = 2;

	public const float kRadiusFactor = 1.1f;

	private RuinsReclaim m_ruinsReclaim;

	private int m_nId;

	private Vector3 m_position = Vector3.zero;

	private float m_fRadius;

	private Vector3 m_exitPosition = Vector3.zero;

	private float m_fExitRadius;

	private int m_nExitYRotationType;

	private float m_fExitYRotation;

	public RuinsReclaim ruinsReclaim => m_ruinsReclaim;

	public int id => m_nId;

	public Vector3 position => m_position;

	public float radius => m_fRadius;

	public Vector3 exitPosition => m_exitPosition;

	public float exitRadius => m_fExitRadius;

	public int exitYRotationType => m_nExitYRotationType;

	public float exitYRotation => m_fExitYRotation;

	public RuinsReclaimPortal(RuinsReclaim ruinsReclaim)
	{
		m_ruinsReclaim = ruinsReclaim;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["portalId"]);
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "반지름이 유효하지 않습니다. m_nId = " + m_nId + ", m_fRadius = " + m_fRadius);
		}
		m_exitPosition.x = Convert.ToSingle(dr["exitXPosition"]);
		m_exitPosition.y = Convert.ToSingle(dr["exitYPosition"]);
		m_exitPosition.z = Convert.ToSingle(dr["exitZPosition"]);
		m_fExitRadius = Convert.ToSingle(dr["exitRadius"]);
		if (m_fExitRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "출구반지름이 유효하지 않습니다. m_nId = " + m_nId + ", m_fExitRadius = " + m_fExitRadius);
		}
		m_nExitYRotationType = Convert.ToInt32(dr["exitYRotationType"]);
		if (!IsDefinedExitYRotationType(m_nExitYRotationType))
		{
			SFLogUtil.Warn(GetType(), "출구방향타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nExitYRotationType = " + m_nExitYRotationType);
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

	public static bool IsDefinedExitYRotationType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
