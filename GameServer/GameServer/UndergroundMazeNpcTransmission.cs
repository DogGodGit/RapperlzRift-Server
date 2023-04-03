using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class UndergroundMazeNpcTransmissionEntry
{
	private UndergroundMazeNpc m_npc;

	private int m_nFloor;

	public UndergroundMaze undergrouneMaze => m_npc.undergroundMaze;

	public UndergroundMazeNpc npc => m_npc;

	public int floor => m_nFloor;

	public UndergroundMazeNpcTransmissionEntry(UndergroundMazeNpc npc)
	{
		m_npc = npc;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nFloor = Convert.ToInt32(dr["floor"]);
		if (undergrouneMaze.GetFloor(m_nFloor) == null)
		{
			SFLogUtil.Warn(GetType(), "지하미로층이 존재하지 않습니다. npcId = " + npc.id + ", m_nFloor = " + m_nFloor);
		}
	}
}
