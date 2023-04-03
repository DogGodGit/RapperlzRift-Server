namespace GameServer;

public class HeroInitEnterParam : PlaceEntranceParam
{
	private Location m_location;

	private int m_nLocationParam;

	private Vector3 m_position = Vector3.zero;

	private float m_fRotationY;

	public Location location => m_location;

	public int locationParam => m_nLocationParam;

	public Vector3 position => m_position;

	public float rotationY => m_fRotationY;

	public HeroInitEnterParam(Location location, int nLocationParam, Vector3 position, float fRotationY)
	{
		m_location = location;
		m_nLocationParam = nLocationParam;
		m_position = position;
		m_fRotationY = fRotationY;
	}
}
