using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class UndergroundMaze : Location
{
	public const int kStartYRotationType_Fixed = 1;

	public const int kStartYRotationType_Random = 2;

	public const int kRequiredConditionType_Level = 1;

	public const int kRequiredConditionType_MainQuest = 2;

	private int m_nLocationId;

	private string m_sNameKey;

	private string m_sDescriptionKey;

	private int m_nRequiredConditionType;

	private int m_nRequiredHeroLevel;

	private int m_nRequiredMainQuestNo;

	private int m_nLimitTime;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartRadius;

	private int m_nStartYRotationType;

	private float m_fStartYRotation;

	private Rect3D m_mapRect = Rect3D.zero;

	private List<UndergroundMazeFloor> m_floors = new List<UndergroundMazeFloor>();

	private Dictionary<int, UndergroundMazeEntrance> m_entrances = new Dictionary<int, UndergroundMazeEntrance>();

	public override int locationId => m_nLocationId;

	public override LocationType locationType => LocationType.UndergroundMaze;

	public override bool mountRidingEnabled => true;

	public override bool hpPotionUseEnabled => true;

	public override bool returnScrollUseEnabled => true;

	public override bool evasionCastEnabled => true;

	public string nameKey => m_sNameKey;

	public string descriptionKey => m_sDescriptionKey;

	public int requiredConditionType => m_nRequiredConditionType;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public int requiredMainQuestNo => m_nRequiredMainQuestNo;

	public int limitTime => m_nLimitTime;

	public Vector3 startPosition => m_startPosition;

	public float startRadius => m_fStartRadius;

	public int startYRotationType => m_nStartYRotationType;

	public float startYRotation => m_fStartYRotation;

	public Rect3D mapRect => m_mapRect;

	public List<UndergroundMazeFloor> floors => m_floors;

	public override void Set(DataRow dr)
	{
		base.Set(dr);
		m_nLocationId = Convert.ToInt32(dr["locationId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_sDescriptionKey = Convert.ToString(dr["descriptionKey"]);
		m_nRequiredConditionType = Convert.ToInt32(dr["requiredConditionType"]);
		if (!IsDefinedRequiredConditionType(m_nRequiredConditionType))
		{
			SFLogUtil.Warn(GetType(), "필요조건타입이 유효하지 않습니다. m_nRequiredConditionType = " + m_nRequiredConditionType);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		if (m_nRequiredHeroLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "필요영웅레벨이 유효하지 않습니다. m_nRequiredHeroLevel = " + m_nRequiredHeroLevel);
		}
		m_nRequiredMainQuestNo = Convert.ToInt32(dr["requiredMainQuestNo"]);
		if (m_nRequiredMainQuestNo < 0)
		{
			SFLogUtil.Warn(GetType(), "필요메인퀘스트번호가 유효하지 않습니다. m_nRequiredMainQuestNo = " + m_nRequiredMainQuestNo);
		}
		m_nLimitTime = Convert.ToInt32(dr["limitTime"]);
		if (m_nLimitTime <= 0)
		{
			SFLogUtil.Warn(GetType(), "제한시간이 유효하지 않습니다. m_nLimitTime = " + m_nLimitTime);
		}
		m_startPosition.x = Convert.ToSingle(dr["startXPosition"]);
		m_startPosition.y = Convert.ToSingle(dr["startYPosition"]);
		m_startPosition.z = Convert.ToSingle(dr["startZPosition"]);
		m_fStartRadius = Convert.ToSingle(dr["startRadius"]);
		m_nStartYRotationType = Convert.ToInt32(dr["startYRotationType"]);
		if (m_nStartYRotationType < 1 || m_nStartYRotationType > 2)
		{
			SFLogUtil.Warn(GetType(), "시작방향타입이 유효하지 않습니다. m_nStartYRotationType = " + m_nStartYRotationType);
		}
		m_fStartYRotation = Convert.ToSingle(dr["startYRotation"]);
		m_mapRect.x = Convert.ToSingle(dr["x"]);
		m_mapRect.y = Convert.ToSingle(dr["y"]);
		m_mapRect.z = Convert.ToSingle(dr["z"]);
		m_mapRect.sizeX = Convert.ToSingle(dr["xSize"]);
		m_mapRect.sizeY = Convert.ToSingle(dr["ySize"]);
		m_mapRect.sizeZ = Convert.ToSingle(dr["zSize"]);
	}

	public void AddFloor(UndergroundMazeFloor floor)
	{
		if (floor == null)
		{
			throw new ArgumentNullException("floor");
		}
		m_floors.Add(floor);
	}

	public UndergroundMazeFloor GetFloor(int nFloor)
	{
		int nIndex = nFloor - 1;
		if (nIndex < 0 || nIndex >= m_floors.Count)
		{
			return null;
		}
		return m_floors[nIndex];
	}

	public void AddEntrance(UndergroundMazeEntrance entrance)
	{
		if (entrance == null)
		{
			throw new ArgumentNullException("entrance");
		}
		m_entrances.Add(entrance.floor, entrance);
	}

	public UndergroundMazeEntrance GetEntrance(int nFloor)
	{
		if (!m_entrances.TryGetValue(nFloor, out var value))
		{
			return null;
		}
		return value;
	}

	public Vector3 SelectPosition()
	{
		return Util.SelectPoint(m_startPosition, m_fStartRadius);
	}

	public float SelectRotationY()
	{
		if (m_nStartYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fStartYRotation);
		}
		return m_fStartYRotation;
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
