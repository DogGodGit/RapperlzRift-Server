using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class UndergroundMazeEntrance
{
	private UndergroundMaze m_undergroundMaze;

	private int m_nFloor;

	private int m_nRequiredHeroLevel;

	public UndergroundMaze undergroundMaze => m_undergroundMaze;

	public int floor => m_nFloor;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public UndergroundMazeEntrance(UndergroundMaze undergroundMaze)
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
		if (m_undergroundMaze.GetFloor(m_nFloor) == null)
		{
			SFLogUtil.Warn(GetType(), "지하미궁층이 존재하지 않습니다. m_nFloor = " + m_nFloor);
		}
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
	}
}
