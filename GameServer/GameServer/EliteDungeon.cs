using System;
using System.Data;

namespace GameServer;

public class EliteDungeon : Location
{
	public const float kSafetyRevivalWaitingTimeFactor = 0.9f;

	private int m_nLocationId;

	private int m_nBaseEnterCount;

	private int m_nEnterCountAddInterval;

	private int m_nRequiredStamina;

	private int m_nStartDelayTime;

	private int m_nLimitTime;

	private int m_nExitDelayTime;

	private int m_nSafeRevivalWaitingTime;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartYRotation;

	private Vector3 m_monsterPosition = Vector3.zero;

	private float m_fMonsterYRotation;

	private Rect3D m_mapRect = Rect3D.zero;

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.EliteDungeon;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public int baseEnterCount => m_nBaseEnterCount;

	public int enterCountAddInterval => m_nEnterCountAddInterval;

	public int requiredStamina => m_nRequiredStamina;

	public int startDelayTime => m_nStartDelayTime;

	public int limitTime => m_nLimitTime;

	public int exitDelayTime => m_nExitDelayTime;

	public int safeRevivalWaitingTime => m_nSafeRevivalWaitingTime;

	public Vector3 startPosition => m_startPosition;

	public float startYRotation => m_fStartYRotation;

	public Vector3 monsterPosition => m_monsterPosition;

	public float monsterYRotation => m_fMonsterYRotation;

	public Rect3D mapRect => m_mapRect;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
		m_nBaseEnterCount = Convert.ToInt32(dr["baseEnterCount"]);
		m_nEnterCountAddInterval = Convert.ToInt32(dr["enterCountAddInterval"]);
		m_nRequiredStamina = Convert.ToInt32(dr["requiredStamina"]);
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		m_nExitDelayTime = Convert.ToInt32(dr["exitDelayTime"]);
		m_nSafeRevivalWaitingTime = Convert.ToInt32(dr["safeRevivalWaitingTime"]);
		m_startPosition.x = Convert.ToSingle(dr["startXPosition"]);
		m_startPosition.y = Convert.ToSingle(dr["startYPosition"]);
		m_startPosition.z = Convert.ToSingle(dr["startZPosition"]);
		m_fStartYRotation = Convert.ToSingle(dr["startYRotation"]);
		m_monsterPosition.x = Convert.ToSingle(dr["monsterXPosition"]);
		m_monsterPosition.y = Convert.ToSingle(dr["monsterYPosition"]);
		m_monsterPosition.z = Convert.ToSingle(dr["monsterZPosition"]);
		m_fMonsterYRotation = Convert.ToSingle(dr["monsterYRotation"]);
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public bool IsSafeRevivalWaitingTimeElapsed(float fElapsedTime)
	{
		return fElapsedTime >= (float)m_nSafeRevivalWaitingTime * 0.9f;
	}
}
