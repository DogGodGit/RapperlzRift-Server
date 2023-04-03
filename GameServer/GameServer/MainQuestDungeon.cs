using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainQuestDungeon : Location
{
	public const float kSafetyRevivalWaitingTimeFactor = 0.9f;

	private int m_nLocationId;

	private int m_nId;

	private string m_sNameKey;

	private string m_sDescriptionKey;

	private int m_nStartDelayTime;

	private int m_nLimitTime;

	private int m_nExitDelayTime;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartRadius;

	private float m_fStartYRotation;

	private int m_nSafeRevivalWaitingTime;

	private bool m_bCompletionExitPositionEnabled;

	private Vector3 m_completionExitPosition = Vector3.zero;

	private float m_fCompletionExitYRotation;

	private Rect3D m_mapRect = Rect3D.zero;

	private List<MainQuestDungeonStep> m_steps = new List<MainQuestDungeonStep>();

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.MainQuestDungeon;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public string descriptionKey => m_sDescriptionKey;

	public int startDelayTime => m_nStartDelayTime;

	public int limitTime => m_nLimitTime;

	public int exitDelayTime => m_nExitDelayTime;

	public Vector3 startPosition => m_startPosition;

	public float startRadius => m_fStartRadius;

	public float startYRotation => m_fStartYRotation;

	public int safeRevivalWaitingTime => m_nSafeRevivalWaitingTime;

	public bool completionExitPositionEnabled => m_bCompletionExitPositionEnabled;

	public Vector3 completionExitPosition => m_completionExitPosition;

	public float completionExitYRotation => m_fCompletionExitYRotation;

	public Rect3D mapRect => m_mapRect;

	public int stepCount => m_steps.Count;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
		m_nId = Convert.ToInt32(dr["dungeonId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_sDescriptionKey = Convert.ToString(dr["descriptionKey"]);
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		if (m_nStartDelayTime < 0)
		{
			SFLogUtil.Warn(GetType(), "시작대기시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nStartDelayTime = " + m_nStartDelayTime);
		}
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		if (m_nLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nLimitTime = " + m_nLimitTime);
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
		m_fStartYRotation = Convert.ToSingle(dr["startYRotation"]);
		m_nSafeRevivalWaitingTime = Convert.ToInt32(dr["safeRevivalWaitingTime"]);
		if (m_nSafeRevivalWaitingTime < 0)
		{
			SFLogUtil.Warn(GetType(), "안전부활대기시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nSafeRevivalWaitingTime = " + m_nSafeRevivalWaitingTime);
		}
		m_bCompletionExitPositionEnabled = Convert.ToBoolean(dr["completionExitPositionEnabled"]);
		m_completionExitPosition.x = Convert.ToSingle(dr["completionExitXPosition"]);
		m_completionExitPosition.y = Convert.ToSingle(dr["completionExitYPosition"]);
		m_completionExitPosition.z = Convert.ToSingle(dr["completionExitZPosition"]);
		m_fCompletionExitYRotation = Convert.ToSingle(dr["completionExitYRotation"]);
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public void AddStep(MainQuestDungeonStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_steps.Add(step);
	}

	public MainQuestDungeonStep GetStep(int nStep)
	{
		int nIndex = nStep - 1;
		if (nIndex < 0 || nIndex >= stepCount)
		{
			return null;
		}
		return m_steps[nIndex];
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
