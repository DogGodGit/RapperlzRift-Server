using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildTerritory : Location
{
	public const int kStartRotationYType_Fixed = 1;

	public const int kStartRotationYType_Random = 2;

	private int m_nLocationId;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartRadius;

	private int m_nStartRotationYType;

	private float m_fStartRotationY;

	private Rect3D m_mapRect = Rect3D.zero;

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.GuildTerritory;

	public override bool mountRidingEnabled => true;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => true;

	public override bool evasionCastEnabled => true;

	public Vector3 startPosition => m_startPosition;

	public float startRadius => m_fStartRadius;

	public int startRotationYType => m_nStartRotationYType;

	public float startRotationY => m_fStartRotationY;

	public Rect3D mapRect => m_mapRect;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
		m_startPosition.x = Convert.ToSingle(dr["startXPosition"]);
		m_startPosition.y = Convert.ToSingle(dr["startYPosition"]);
		m_startPosition.z = Convert.ToSingle(dr["startZPosition"]);
		m_fStartRadius = Convert.ToSingle(dr["startRadius"]);
		if (m_fStartRadius < 0f)
		{
			SFLogUtil.Warn(GetType(), "시작반경이 유효하지 않습니다. m_fStartRadius = " + m_fStartRadius);
		}
		m_nStartRotationYType = Convert.ToInt32(dr["startYRotationType"]);
		m_fStartRotationY = Convert.ToSingle(dr["startYRotation"]);
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
		if (m_mapRect.sizeX <= 0f)
		{
			SFLogUtil.Warn(GetType(), "맵크기가 유효하지 않습니다. m_mapRect.sizeX = " + m_mapRect.sizeX);
		}
		if (m_mapRect.sizeY <= 0f)
		{
			SFLogUtil.Warn(GetType(), "맵크기가 유효하지 않습니다. m_mapRect.sizeY = " + m_mapRect.sizeY);
		}
		if (m_mapRect.sizeZ <= 0f)
		{
			SFLogUtil.Warn(GetType(), "맵크기가 유효하지 않습니다. m_mapRect.sizeZ = " + m_mapRect.sizeZ);
		}
	}

	public Vector3 SelectStartPosition()
	{
		return Util.SelectPoint(m_startPosition, m_fStartRadius);
	}

	public float SelectStartRotationY()
	{
		if (m_nStartRotationYType != 1)
		{
			return SFRandom.NextFloat(0f, m_fStartRotationY);
		}
		return m_fStartRotationY;
	}
}
