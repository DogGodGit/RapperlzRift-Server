using System;

namespace GameServer;

public class RuinsReclaimEnterParam : PlaceEntranceParam
{
	private Guid m_ruinsReclaimInstanceId = Guid.Empty;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	public Guid ruinsReclaimInstanceId => m_ruinsReclaimInstanceId;

	public DateTimeOffset dungeonCreationTime => m_dungeonCreationTime;

	public RuinsReclaimEnterParam(Guid ruinsReclaimInstanceId, DateTimeOffset dungeonCreationTime)
	{
		m_ruinsReclaimInstanceId = ruinsReclaimInstanceId;
		m_dungeonCreationTime = dungeonCreationTime;
	}
}
