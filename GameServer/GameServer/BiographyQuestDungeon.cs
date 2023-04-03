using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class BiographyQuestDungeon : Location
{
	public const float kSafetyRevivalWaitingTimeFactor = 0.9f;

	private int m_nId;

	private int m_nStartDelayTime;

	private int m_nLimitTime;

	private int m_nExitDelayTime;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartRadius;

	private float m_fStartRotationY;

	private int m_nSafeRevivalWaitingTime;

	private int m_nLocationId;

	private Rect3D m_mapRect = Rect3D.zero;

	private List<BiographyQuestDungeonWave> m_waves = new List<BiographyQuestDungeonWave>();

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.BiographyQuestDungeon;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public int id => m_nId;

	public int startDelayTime => m_nStartDelayTime;

	public int limitTime => m_nLimitTime;

	public int exitDelayTime => m_nExitDelayTime;

	public Vector3 startPosition => m_startPosition;

	public float startRadius => m_fStartRadius;

	public float startRotationY => m_fStartRotationY;

	public int safeRevivalWaitingTime => m_nSafeRevivalWaitingTime;

	public Rect3D mapRect => m_mapRect;

	public List<BiographyQuestDungeonWave> waves => m_waves;

	public int lastWaveNo => m_waves.Count;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nId = Convert.ToInt32(dr["dungeonId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "던전ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		if (m_nStartDelayTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "시작대기시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nStartDelayTime = " + m_nStartDelayTime);
		}
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		if (m_nLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "시작대기시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nStartDelayTime = " + m_nStartDelayTime);
		}
		m_nExitDelayTime = Convert.ToInt32(dr["exitDelayTime"]);
		if (m_nExitDelayTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "퇴장대기시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nExitDelayTime = " + m_nExitDelayTime);
		}
		m_startPosition.x = Convert.ToSingle(dr["startXPosition"]);
		m_startPosition.y = Convert.ToSingle(dr["startYPosition"]);
		m_startPosition.z = Convert.ToSingle(dr["startZPosition"]);
		m_fStartRadius = Convert.ToSingle(dr["startRadius"]);
		if (m_fStartRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "시작반경이 유효하지 않습니다. m_nId = " + m_nId + ", m_fStartRadius = " + m_fStartRadius);
		}
		m_fStartRotationY = Convert.ToSingle(dr["startYRotation"]);
		m_nSafeRevivalWaitingTime = Convert.ToInt32(dr["safeRevivalWaitingTime"]);
		if (m_nSafeRevivalWaitingTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "안전부활대기시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nSafeRevivalWaitingTime = " + m_nSafeRevivalWaitingTime);
		}
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
		if (m_nLocationId <= 0)
		{
			SFLogUtil.Warn(GetType(), "위치ID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nLocationId = " + m_nLocationId);
		}
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public void AddWave(BiographyQuestDungeonWave wave)
	{
		if (wave == null)
		{
			throw new ArgumentNullException("wave");
		}
		m_waves.Add(wave);
	}

	public BiographyQuestDungeonWave GetWave(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_waves.Count)
		{
			return null;
		}
		return m_waves[nIndex];
	}

	public Vector3 SelectStartPosition()
	{
		return Util.SelectPoint(m_startPosition, m_fStartRadius);
	}

	public bool IsSafeRevivalWaitingTimeElapsed(float fElapsedTime)
	{
		return fElapsedTime >= (float)m_nSafeRevivalWaitingTime * 0.9f;
	}
}
