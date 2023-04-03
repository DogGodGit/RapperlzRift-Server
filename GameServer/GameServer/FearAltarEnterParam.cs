using System;

namespace GameServer;

public class FearAltarEnterParam : PlaceEntranceParam
{
	private Guid m_fearAltarInstanceId = Guid.Empty;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	public Guid fearAltarInstanceId => m_fearAltarInstanceId;

	public DateTimeOffset dungeonCreationTime => m_dungeonCreationTime;

	public FearAltarEnterParam(Guid fearAltarInstanceId, DateTimeOffset dungeonCreationTime)
	{
		m_fearAltarInstanceId = fearAltarInstanceId;
		m_dungeonCreationTime = dungeonCreationTime;
	}
}
