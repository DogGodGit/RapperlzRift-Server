using System;

namespace GameServer;

public class InfiniteWarEnterParam : PlaceEntranceParam
{
	private Guid m_infiniteWarInstanceId = Guid.Empty;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	public Guid infiniteWarInstanceId => m_infiniteWarInstanceId;

	public DateTimeOffset dungeonCreationTime => m_dungeonCreationTime;

	public InfiniteWarEnterParam(Guid infiniteWarInstanceId, DateTimeOffset dungeonCreationTime)
	{
		m_infiniteWarInstanceId = infiniteWarInstanceId;
		m_dungeonCreationTime = dungeonCreationTime;
	}
}
