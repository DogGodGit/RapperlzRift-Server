namespace GameServer;

public class WarMemoryMatchingRoomEnterParam : MatchingRoomEntranceParam
{
	private WarMemorySchedule m_schedule;

	public WarMemorySchedule schedule => m_schedule;

	public WarMemoryMatchingRoomEnterParam(WarMemorySchedule schedule)
	{
		m_schedule = schedule;
	}
}
