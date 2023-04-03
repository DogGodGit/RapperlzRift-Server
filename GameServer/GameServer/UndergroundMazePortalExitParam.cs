namespace GameServer;

public class UndergroundMazePortalExitParam : PlaceEntranceParam
{
	private UndergroundMazePortal m_linkedPortal;

	public UndergroundMazePortal linkedPortal => m_linkedPortal;

	public UndergroundMazePortalExitParam(UndergroundMazePortal linkedPortal)
	{
		m_linkedPortal = linkedPortal;
	}
}
