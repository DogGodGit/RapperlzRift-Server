using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class UndergroundMazeNpc
{
	public const float kInteractionMaxRangeFactor = 1.1f;

	private int m_nId;

	private UndergroundMazeFloor m_floor;

	private string m_sNameKey;

	private string m_sDialogueKey;

	private Vector3 m_position = Vector3.zero;

	private float m_fYRotation;

	private float m_fInteractionMaxRange;

	private float m_fScale;

	private float m_fHeight;

	private float m_fRadius;

	private Dictionary<int, UndergroundMazeNpcTransmissionEntry> m_transmissionEntries = new Dictionary<int, UndergroundMazeNpcTransmissionEntry>();

	public int id => m_nId;

	public UndergroundMaze undergroundMaze => m_floor.undergroundMaze;

	public UndergroundMazeFloor floor => m_floor;

	public string nameKey => m_sNameKey;

	public string dialogueKey => m_sDialogueKey;

	public Vector3 position => m_position;

	public float yRotation => m_fYRotation;

	public float interactionMaxRange => m_fInteractionMaxRange;

	public float scale => m_fScale;

	public float height => m_fHeight;

	public float radius => m_fRadius;

	public UndergroundMazeNpc(UndergroundMazeFloor floor)
	{
		m_floor = floor;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["npcId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_sDialogueKey = Convert.ToString(dr["dialogueKey"]);
		m_position.x = Convert.ToSingle(dr["xPosition"]);
		m_position.y = Convert.ToSingle(dr["yPosition"]);
		m_position.z = Convert.ToSingle(dr["zPosition"]);
		m_fYRotation = Convert.ToSingle(dr["yRotation"]);
		m_fInteractionMaxRange = Convert.ToSingle(dr["interactionMaxRange"]);
		if (m_fInteractionMaxRange <= 0f)
		{
			SFLogUtil.Warn(GetType(), "상호작용최대범위가 유효하지 않습니다. floor = " + m_floor.floor + ", m_nId = " + m_nId + ", m_fInteractionMaxRange = " + m_fInteractionMaxRange);
		}
		m_fScale = Convert.ToSingle(dr["scale"]);
		m_fHeight = Convert.ToSingle(dr["height"]);
		m_fRadius = Convert.ToSingle(dr["radius"]);
	}

	public void AddTransmissionEntry(UndergroundMazeNpcTransmissionEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_transmissionEntries.Add(entry.floor, entry);
	}

	public UndergroundMazeNpcTransmissionEntry GetTransmissionEntry(int nFloor)
	{
		if (!m_transmissionEntries.TryGetValue(nFloor, out var value))
		{
			return null;
		}
		return value;
	}

	public bool IsInteractionEnabledPosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, m_fInteractionMaxRange * 1.1f + fRadius * 2f, position);
	}
}
