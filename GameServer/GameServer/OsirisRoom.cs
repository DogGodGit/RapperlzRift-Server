using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class OsirisRoom : Location
{
	private int m_nLocationId;

	private int m_nRequiredStamina;

	private int m_nStartDelayTime;

	private int m_nLimitTime;

	private int m_nExitDelayTime;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartYRotation;

	private int m_nWaveInterval;

	private int m_nMonsterSpawnInterval;

	private Vector3 m_monsterSpawnPosition = Vector3.zero;

	private float m_fMonsterSpawnYRotation;

	private Vector3 m_targetPosition = Vector3.zero;

	private float m_fTargetRadius;

	private Rect3D m_mapRect = Rect3D.zero;

	private List<OsirisRoomDifficulty> m_difficulties = new List<OsirisRoomDifficulty>();

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.OsirisRoom;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public int requiredStamina => m_nRequiredStamina;

	public int startDelayTime => m_nStartDelayTime;

	public int limitTime => m_nLimitTime;

	public int exitDelayTime => m_nExitDelayTime;

	public Vector3 startPosition => m_startPosition;

	public float startYRotation => m_fStartYRotation;

	public int waveInterval => m_nWaveInterval;

	public int monsterSpawnInterval => m_nMonsterSpawnInterval;

	public Vector3 monsterSpawnPosition => m_monsterSpawnPosition;

	public float monsterSpawnYRotation => m_fMonsterSpawnYRotation;

	public Vector3 targetPosition => m_targetPosition;

	public float targetRadius => m_fTargetRadius;

	public Rect3D mapRect => m_mapRect;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
		m_nRequiredStamina = Convert.ToInt32(dr["requiredStamina"]);
		if (m_nRequiredStamina < 0)
		{
			SFLogUtil.Warn(GetType(), "필요체력이 유효하지 않습니다. m_nRequiredStamina = " + m_nRequiredStamina);
		}
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		if (m_nStartDelayTime < 0)
		{
			SFLogUtil.Warn(GetType(), "시작대기시간이 유효하지 않습니다. m_nStartDelayTime = " + m_nStartDelayTime);
		}
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		if (m_nLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한시간이 유효하지 않습니다. m_nLimitTime = " + m_nLimitTime);
		}
		m_nExitDelayTime = Convert.ToInt32(dr["exitDelayTime"]);
		if (m_nExitDelayTime < 0)
		{
			SFLogUtil.Warn(GetType(), "퇴장대기시간이 유효하지 않습니다. m_nExitDelayTime = " + m_nExitDelayTime);
		}
		m_startPosition.x = Convert.ToSingle(dr["startXPosition"]);
		m_startPosition.y = Convert.ToSingle(dr["startYPosition"]);
		m_startPosition.z = Convert.ToSingle(dr["startZPosition"]);
		m_fStartYRotation = Convert.ToSingle(dr["startYRotation"]);
		m_nWaveInterval = Convert.ToInt32(dr["waveInterval"]);
		if (m_nWaveInterval < 0)
		{
			SFLogUtil.Warn(GetType(), "웨이브간격이 유효하지 않습니다. m_nWaveInterval = " + m_nWaveInterval);
		}
		m_nMonsterSpawnInterval = Convert.ToInt32(dr["monsterSpawnInterval"]);
		if (m_nMonsterSpawnInterval < 0)
		{
			SFLogUtil.Warn(GetType(), "몬스터출몰간격이 유효하지 않습니다. m_nMonsterSpawnInterval = " + m_nMonsterSpawnInterval);
		}
		m_monsterSpawnPosition.x = Convert.ToSingle(dr["monsterSpawnXPosition"]);
		m_monsterSpawnPosition.y = Convert.ToSingle(dr["monsterSpawnYPosition"]);
		m_monsterSpawnPosition.z = Convert.ToSingle(dr["monsterSpawnZPosition"]);
		m_fMonsterSpawnYRotation = Convert.ToSingle(dr["monsterSpawnYRotation"]);
		m_targetPosition.x = Convert.ToSingle(dr["targetXPosition"]);
		m_targetPosition.y = Convert.ToSingle(dr["targetYPosition"]);
		m_targetPosition.z = Convert.ToSingle(dr["targetZPosition"]);
		m_fTargetRadius = Convert.ToSingle(dr["targetRadius"]);
		if (m_fTargetRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "목적지 반지름이 유효하지 않습니다. m_fTargetRadius = " + m_fTargetRadius);
		}
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public bool ContainsTargetPosition(Vector3 position)
	{
		return MathUtil.CircleContains(m_targetPosition, m_fTargetRadius, position);
	}

	public void AddDifficulty(OsirisRoomDifficulty difficulty)
	{
		if (difficulty == null)
		{
			throw new ArgumentNullException("difficulty");
		}
		m_difficulties.Add(difficulty);
	}

	public OsirisRoomDifficulty GetDifficulty(int nDifficulty)
	{
		int nIndex = nDifficulty - 1;
		if (nIndex < 0 || nIndex >= m_difficulties.Count)
		{
			return null;
		}
		return m_difficulties[nIndex];
	}
}
