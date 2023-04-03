using System;

namespace GameServer;

public class UndergroundMazeEnterParam : PlaceEntranceParam
{
	private UndergroundMazeFloor m_floor;

	private DateTimeOffset m_enterTime = DateTimeOffset.MinValue;

	public UndergroundMazeFloor floor => m_floor;

	public DateTimeOffset enterTime => m_enterTime;

	public UndergroundMazeEnterParam(UndergroundMazeFloor floor, DateTimeOffset enterTime)
	{
		m_floor = floor;
		m_enterTime = enterTime;
	}
}
