using System;

namespace GameServer;

public class DragonNestEnterParam : PlaceEntranceParam
{
	private Guid m_dragonNestInstanceId = Guid.Empty;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	public Guid dragonNestInstanceId => m_dragonNestInstanceId;

	public DateTimeOffset dungeonCreationTime => m_dungeonCreationTime;

	public DragonNestEnterParam(Guid dragonNestInstanceId, DateTimeOffset dungeonCreationTime)
	{
		m_dragonNestInstanceId = dragonNestInstanceId;
		m_dungeonCreationTime = dungeonCreationTime;
	}
}
