using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FearAltarStage : Location, IPickEntry
{
	public const int kStartYRotationType_Fixed = 1;

	public const int kStartYRotationType_Random = 2;

	private int m_nLocationId;

	private FearAltar m_fearAltar;

	private int m_nId;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartRadius;

	private int m_nStartYRotationType;

	private float m_fStartYRotation;

	private Rect3D m_mapRect = Rect3D.zero;

	private List<FearAltarStageWave> m_waves = new List<FearAltarStageWave>();

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.FearAltar;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public FearAltar fearAltar => m_fearAltar;

	public int id => m_nId;

	public Vector3 startPosition => m_startPosition;

	public float startRadius => m_fStartRadius;

	public int startYRotationType => m_nStartYRotationType;

	public float startYRotation => m_fStartYRotation;

	public Rect3D mapRect => m_mapRect;

	public int point => 1;

	int IPickEntry.point => point;

	public int waveCount => m_waves.Count;

	public FearAltarStage(FearAltar fearAltar)
	{
		m_fearAltar = fearAltar;
	}

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
		m_nId = Convert.ToInt32(dr["stageId"]);
		m_startPosition.x = Convert.ToSingle(dr["startXPosition"]);
		m_startPosition.y = Convert.ToSingle(dr["startYPosition"]);
		m_startPosition.z = Convert.ToSingle(dr["startZposition"]);
		m_fStartRadius = Convert.ToSingle(dr["startRadius"]);
		if (m_fStartYRotation < 0f)
		{
			SFLogUtil.Warn(GetType(), "시작반지름이 유효하지 않습니다. m_nId = " + m_nId + ", m_fStartYRotation = " + m_fStartYRotation);
		}
		m_nStartYRotationType = Convert.ToInt32(dr["startYRotationType"]);
		if (!isDefinedStartYRotationType(m_nStartYRotationType))
		{
			SFLogUtil.Warn(GetType(), "시작방향타입이 유효하지 않습니다. m_nId = " + m_nId + ", m_nStartYRotationType = " + m_nStartYRotationType);
		}
		m_fStartYRotation = Convert.ToSingle(dr["startYRotation"]);
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public Vector3 SelectStartPosition()
	{
		return Util.SelectPoint(m_startPosition, m_fStartRadius);
	}

	public float SelectStartRotationY()
	{
		if (m_nStartYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fStartYRotation);
		}
		return m_fStartYRotation;
	}

	public void AddWave(FearAltarStageWave wave)
	{
		if (wave == null)
		{
			throw new ArgumentNullException("wave");
		}
		m_waves.Add(wave);
	}

	public FearAltarStageWave GetWave(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_waves.Count)
		{
			return null;
		}
		return m_waves[nIndex];
	}

	public static bool isDefinedStartYRotationType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
