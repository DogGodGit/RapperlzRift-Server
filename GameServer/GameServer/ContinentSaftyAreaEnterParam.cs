using System;

namespace GameServer;

public class ContinentSaftyAreaEnterParam : PlaceEntranceParam
{
	private Continent m_continent;

	private int m_nNationId;

	private Vector3 m_position = Vector3.zero;

	private float m_fRotationY;

	private DateTimeOffset m_enterTime = DateTimeOffset.MinValue;

	public Continent continent => m_continent;

	public int nationId => m_nNationId;

	public Vector3 position => m_position;

	public float rotationY => m_fRotationY;

	public DateTimeOffset enterTime => m_enterTime;

	public ContinentSaftyAreaEnterParam(Continent continent, int nNationId, Vector3 position, float fRotationY, DateTimeOffset enterTime)
	{
		m_continent = continent;
		m_nNationId = nNationId;
		m_position = position;
		m_fRotationY = fRotationY;
		m_enterTime = enterTime;
	}
}
