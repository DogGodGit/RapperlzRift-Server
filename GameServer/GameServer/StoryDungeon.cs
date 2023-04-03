using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class StoryDungeon : Location
{
	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

	public const float kSafetyRevivalWaitingTimeFactor = 0.9f;

	private int m_nLocationId;

	private int m_nNo;

	private string m_sNameKey;

	private int m_nEnterCount;

	private int m_nRequiredConditionType;

	private int m_nRequiredHeroMinLevel;

	private int m_nRequiredHeroMaxLevel;

	private int m_nRequiredMainQuestNo;

	private int m_nRequiredStamina;

	private int m_nLimitTime;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartRadius;

	private float m_fStartYRotation;

	private int m_nStartDelayTime;

	private int m_nExitDelayTime;

	private Vector3 m_tamingPosition = Vector3.zero;

	private float m_fTamingYRotation;

	private Vector3 m_clearPosition = Vector3.zero;

	private float m_fClearYRotation;

	private int m_nSafeRevivalWaitingTime;

	private Rect3D m_mapRect = Rect3D.zero;

	private List<StoryDungeonDifficulty> m_difficulties = new List<StoryDungeonDifficulty>();

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.StoryDungeon;

	public override bool mountRidingEnabled => false;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => false;

	public override bool evasionCastEnabled => true;

	public int no => m_nNo;

	public string nameKey => m_sNameKey;

	public int enterCount => m_nEnterCount;

	public int requiredConditionType => m_nRequiredConditionType;

	public int requiredHeroMinLevel => m_nRequiredHeroMinLevel;

	public int requiredHeroMaxLevel => m_nRequiredHeroMaxLevel;

	public int requiredMainQuestNo => m_nRequiredMainQuestNo;

	public int requiredStamina => m_nRequiredStamina;

	public int limitTime => m_nLimitTime;

	public Vector3 startPosition => m_startPosition;

	public float startRadius => m_fStartRadius;

	public float startYRotation => m_fStartYRotation;

	public int startDelayTime => m_nStartDelayTime;

	public int exitDelayTime => m_nExitDelayTime;

	public Vector3 tamingPosition => m_tamingPosition;

	public float tamingYRotation => m_fTamingYRotation;

	public Vector3 clearPosition => m_clearPosition;

	public float clearYRotation => m_fClearYRotation;

	public int safeRevivalWaitingTime => m_nSafeRevivalWaitingTime;

	public int topDifficulty => m_difficulties.Count;

	public Rect3D mapRect => m_mapRect;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
		m_nNo = Convert.ToInt32(dr["dungeonNo"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_nEnterCount = Convert.ToInt32(dr["enterCount"]);
		if (m_nEnterCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "입장횟수가 유효하지 않습니다. m_nNo = " + m_nNo + ", enterCount = " + enterCount);
		}
		m_nRequiredConditionType = Convert.ToInt32(dr["requiredConditionType"]);
		if (!IsDefinedRequiredConditionType(m_nRequiredConditionType))
		{
			SFLogUtil.Warn(GetType(), "필요조건타입이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nRequiredConditionType = " + m_nRequiredConditionType);
		}
		m_nRequiredHeroMinLevel = Convert.ToInt32(dr["requiredHeroMinLevel"]);
		m_nRequiredHeroMaxLevel = Convert.ToInt32(dr["requiredHeroMaxLevel"]);
		if (m_nRequiredHeroMinLevel > m_nRequiredHeroMaxLevel)
		{
			SFLogUtil.Warn(GetType(), "필요영웅최소레벨과 필요영웅최대레벨이 유효하지 않습니다. m_No = " + m_nNo + ", m_nRequiredHeroMinLevel = " + m_nRequiredHeroMinLevel + ", requiredHeroMaxLevel = " + requiredHeroMaxLevel);
		}
		m_nRequiredMainQuestNo = Convert.ToInt32(dr["requiredMainQuestNo"]);
		if (m_nRequiredMainQuestNo < 0)
		{
			SFLogUtil.Warn(GetType(), "필요메인퀘스트번호가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nRequiredMainQuestNo = " + m_nRequiredMainQuestNo);
		}
		m_nRequiredStamina = Convert.ToInt32(dr["requiredStamina"]);
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		if (m_nLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한시간이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nLimitTime = " + m_nLimitTime);
		}
		m_startPosition.x = Convert.ToSingle(dr["startXPosition"]);
		m_startPosition.y = Convert.ToSingle(dr["startYPosition"]);
		m_startPosition.z = Convert.ToSingle(dr["startZPosition"]);
		m_fStartRadius = Convert.ToSingle(dr["startRadius"]);
		m_fStartYRotation = Convert.ToSingle(dr["startYRotation"]);
		m_nStartDelayTime = Convert.ToInt32(dr["startDelayTime"]);
		m_nExitDelayTime = Convert.ToInt32(dr["exitDelayTime"]);
		m_tamingPosition.x = Convert.ToSingle(dr["tamingXPosition"]);
		m_tamingPosition.y = Convert.ToSingle(dr["tamingYPosition"]);
		m_tamingPosition.z = Convert.ToSingle(dr["tamingZPosition"]);
		m_fTamingYRotation = Convert.ToSingle(dr["tamingYRotation"]);
		m_clearPosition.x = Convert.ToSingle(dr["clearXPosition"]);
		m_clearPosition.y = Convert.ToSingle(dr["clearYPosition"]);
		m_clearPosition.z = Convert.ToSingle(dr["clearZPosition"]);
		m_fClearYRotation = Convert.ToSingle(dr["clearYRotation"]);
		m_nSafeRevivalWaitingTime = Convert.ToInt32(dr["safeRevivalWaitingTime"]);
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public void AddDifficulty(StoryDungeonDifficulty difficulty)
	{
		if (difficulty == null)
		{
			throw new ArgumentNullException("difficulty");
		}
		m_difficulties.Add(difficulty);
	}

	public StoryDungeonDifficulty GetDifficulty(int nDifficulty)
	{
		int nIndex = nDifficulty - 1;
		if (nIndex < 0 || nIndex >= m_difficulties.Count)
		{
			return null;
		}
		return m_difficulties[nIndex];
	}

	public Vector3 SelectPosition()
	{
		return Util.SelectPoint(m_startPosition, m_fStartRadius);
	}

	public bool IsSafeRevivalWaitingTimeElapsed(float fElapsedTime)
	{
		return fElapsedTime >= (float)m_nSafeRevivalWaitingTime * 0.9f;
	}

	public static bool IsDefinedRequiredConditionType(int nType)
	{
		if (nType != 1)
		{
			return nType == 2;
		}
		return true;
	}
}
