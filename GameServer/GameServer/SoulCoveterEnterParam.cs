using System;

namespace GameServer;

public class SoulCoveterEnterParam : PlaceEntranceParam
{
	private Guid m_soulCoveterInstanceId = Guid.Empty;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	public Guid soulCoveterInstanceId => m_soulCoveterInstanceId;

	public DateTimeOffset dungeonCreationTime => m_dungeonCreationTime;

	public SoulCoveterEnterParam(Guid soulCoveterInstanceId, DateTimeOffset dungeonCreationTime)
	{
		m_soulCoveterInstanceId = soulCoveterInstanceId;
		m_dungeonCreationTime = dungeonCreationTime;
	}
}
