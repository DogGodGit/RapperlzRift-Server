using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class UndergroundMazeFloor
{
	private UndergroundMaze m_undergroundMaze;

	private int m_nFloor;

	private string m_sNameKey;

	private Dictionary<int, UndergroundMazePortal> m_portals = new Dictionary<int, UndergroundMazePortal>();

	private List<UndergroundMazeMonsterArrange> m_arranges = new List<UndergroundMazeMonsterArrange>();

	private Dictionary<int, UndergroundMazeNpc> m_npcs = new Dictionary<int, UndergroundMazeNpc>();

	public UndergroundMaze undergroundMaze => m_undergroundMaze;

	public int floor => m_nFloor;

	public string nameKey => m_sNameKey;

	public List<UndergroundMazeMonsterArrange> arranges => m_arranges;

	public UndergroundMazeFloor(UndergroundMaze undergroundMaze)
	{
		m_undergroundMaze = undergroundMaze;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nFloor = Convert.ToInt32(dr["floor"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
	}

	public void AddPortal(UndergroundMazePortal portal)
	{
		if (portal == null)
		{
			throw new ArgumentNullException("portal");
		}
		m_portals.Add(portal.id, portal);
	}

	public UndergroundMazePortal GetPortal(int nId)
	{
		if (!m_portals.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddArrange(UndergroundMazeMonsterArrange arrange)
	{
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		m_arranges.Add(arrange);
	}

	public UndergroundMazeMonsterArrange GetArrange(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_arranges.Count)
		{
			return null;
		}
		return m_arranges[nIndex];
	}

	public void AddNpc(UndergroundMazeNpc npc)
	{
		if (npc == null)
		{
			throw new ArgumentNullException("npc");
		}
		m_npcs.Add(npc.id, npc);
	}

	public UndergroundMazeNpc GetNpc(int nId)
	{
		if (!m_npcs.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}
}
