namespace GameServer;

public class RuinsReclaimMatchingRoomEnterParam : MatchingRoomEntranceParam
{
	private RuinsReclaimOpenSchedule m_openSchedule;

	public RuinsReclaimOpenSchedule openSchedule => m_openSchedule;

	public RuinsReclaimMatchingRoomEnterParam(RuinsReclaimOpenSchedule openSchedule)
	{
		m_openSchedule = openSchedule;
	}
}
