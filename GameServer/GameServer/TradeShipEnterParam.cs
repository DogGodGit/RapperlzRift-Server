using System;

namespace GameServer;

public class TradeShipEnterParam : PlaceEntranceParam
{
	private Guid m_tradeShipInstanceId = Guid.Empty;

	private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

	public Guid tradeShipInstanceId => m_tradeShipInstanceId;

	public DateTimeOffset dungeonCreationTime => m_dungeonCreationTime;

	public TradeShipEnterParam(Guid tradeShipInstanceId, DateTimeOffset dungeonCreationTime)
	{
		m_tradeShipInstanceId = tradeShipInstanceId;
		m_dungeonCreationTime = dungeonCreationTime;
	}
}
