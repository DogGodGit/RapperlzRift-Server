namespace GameServer;

public class InfiniteWarMatchingRoomEnterParam : MatchingRoomEntranceParam
{
	private InfiniteWarOpenSchedule m_openSchedule;

	public InfiniteWarOpenSchedule openSchedule => m_openSchedule;

	public InfiniteWarMatchingRoomEnterParam(InfiniteWarOpenSchedule openSchedule)
	{
		m_openSchedule = openSchedule;
	}
}
