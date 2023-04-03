using System;

namespace GameServer;

public class AnkouTombEnterParam : PlaceEntranceParam
{
	private Guid m_ankouTombInstanceId = Guid.Empty;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	public Guid ankouTombInstanceId => m_ankouTombInstanceId;

	public DateTimeOffset dungeonCreationTime => m_dungeonCreationTime;

	public AnkouTombEnterParam(Guid ankouTombInstanceId, DateTimeOffset dungeonCreationTime)
	{
		m_ankouTombInstanceId = ankouTombInstanceId;
		m_dungeonCreationTime = dungeonCreationTime;
	}
}
