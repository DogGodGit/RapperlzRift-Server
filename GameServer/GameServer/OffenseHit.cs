namespace GameServer;

public class OffenseHit
{
	private Offense m_offense;

	private int m_nHitId;

	public Offense offense => m_offense;

	public int hitId => m_nHitId;

	public OffenseHit(Offense offense, int nHitId)
	{
		m_offense = offense;
		m_nHitId = nHitId;
	}
}
