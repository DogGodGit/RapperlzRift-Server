using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ExpDungeon : Location
{
	public const float kSafetyRevivalWaitingTimeFactor = 0.9f;

	private int m_nLocationId;

	private string m_sNameKey;

	private string m_sDescriptionKey;

	private int m_nRequiredStamina;

	private int m_nLimitTime;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartYRotation;

	private int m_nStartDelayTime;

	private int m_nExitDelayTime;

	private int m_nWaveIntervalTime;

	private int m_nSafeRevivalWaitingTime;

	private float m_fSweepExpRewardFactor;

	private Rect3D m_mapRect = Rect3D.zero;

	private List<ExpDungeonDifficulty> m_difficulties = new List<ExpDungeonDifficulty>();

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.ExpDungeon;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public int requiredStamina => m_nRequiredStamina;

	public int limitTime => m_nLimitTime;

	public Vector3 startPosition => m_startPosition;

	public float startYRotation => m_fStartYRotation;

	public int startDelayTime => m_nStartDelayTime;

	public int exitDelayTime => m_nExitDelayTime;

	public int waveIntervalTime => m_nWaveIntervalTime;

	public int safeRevivalWaitingTime => m_nSafeRevivalWaitingTime;

	public float sweepExpRewardFactor => m_fSweepExpRewardFactor;

	public Rect3D mapRect => m_mapRect;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_sDescriptionKey = Convert.ToString(dr["descriptionKey"]);
		m_nRequiredStamina = Convert.ToInt32(dr["requiredStamina"]);
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		if (m_nLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한시간이 유효하지 않습니다. m_nLimitTime = " + m_nLimitTime);
		}
		m_startPosition.x = Convert.ToSingle(dr["startXPosition"]);
		m_startPosition.y = Convert.ToSingle(dr["startYPosition"]);
		m_startPosition.z = Convert.ToSingle(dr["startZPosition"]);
		m_fStartYRotation = Convert.ToSingle(dr["startYRotation"]);
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		m_nExitDelayTime = Convert.ToInt32(dr["exitDelayTime"]);
		m_nWaveIntervalTime = Convert.ToInt32(dr["waveIntervalTime"]);
		m_nSafeRevivalWaitingTime = Convert.ToInt32(dr["safeRevivalWaitingTime"]);
		m_fSweepExpRewardFactor = Convert.ToSingle(dr["sweepExpRewardFactor"]);
		if (m_fSweepExpRewardFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "소탕경험치보상계수가 유효하지 않습니다. m_fSweepExpRewardFactor = " + m_fSweepExpRewardFactor);
		}
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public void AddDifficulty(ExpDungeonDifficulty difficulty)
	{
		if (difficulty == null)
		{
			throw new ArgumentNullException("difficulty");
		}
		m_difficulties.Add(difficulty);
	}

	public ExpDungeonDifficulty GetDifficulty(int nDifficulty)
	{
		int nIndex = nDifficulty - 1;
		if (nIndex < 0 || nIndex >= m_difficulties.Count)
		{
			return null;
		}
		return m_difficulties[nIndex];
	}

	public bool IsSafeRevivalWaitingTimeElapsed(float fElapsedTime)
	{
		return fElapsedTime >= (float)m_nSafeRevivalWaitingTime * 0.9f;
	}
}
