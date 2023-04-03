using System;

namespace GameServer;

public class WarMemoryEnterParam : PlaceEntranceParam
{
	private Guid m_warMemoryInstanceId = Guid.Empty;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	public Guid warMemoryInstanceId => m_warMemoryInstanceId;

	public DateTimeOffset dungeonCreationTime => m_dungeonCreationTime;

	public WarMemoryEnterParam(Guid warMemoryInstanceId, DateTimeOffset dungeonCreationTime)
	{
		m_warMemoryInstanceId = warMemoryInstanceId;
		m_dungeonCreationTime = dungeonCreationTime;
	}
}
