namespace GameServer;

public class PortalExitParam : PlaceEntranceParam
{
	private Portal m_portal;

	private int m_nNationId;

	public Portal portal => m_portal;

	public int nationId => m_nNationId;

	public PortalExitParam(Portal portal, int nNationId)
	{
		m_portal = portal;
		m_nNationId = nNationId;
	}
}
