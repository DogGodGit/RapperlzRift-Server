namespace GameServer;

public class TradeShipMatchingRoomEnterParam : MatchingRoomEntranceParam
{
	private TradeShipDifficulty m_difficulty;

	private TradeShipSchedule m_schedule;

	public TradeShipDifficulty difficulty => m_difficulty;

	public TradeShipSchedule schedule => m_schedule;

	public TradeShipMatchingRoomEnterParam(TradeShipDifficulty difficulty, TradeShipSchedule schedule)
	{
		m_difficulty = difficulty;
		m_schedule = schedule;
	}
}
