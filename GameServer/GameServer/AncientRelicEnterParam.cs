using System;

namespace GameServer;

public class AncientRelicEnterParam : PlaceEntranceParam
{
	private Guid m_ancientRelicInstanceId = Guid.Empty;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	public Guid ancientRelicInstanceId => m_ancientRelicInstanceId;

	public DateTimeOffset dungeonCreationTime => m_dungeonCreationTime;

	public AncientRelicEnterParam(Guid ancientRelicInstanceId, DateTimeOffset dungeonCreationTime)
	{
		m_ancientRelicInstanceId = ancientRelicInstanceId;
		m_dungeonCreationTime = dungeonCreationTime;
	}
}
