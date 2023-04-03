using System;

namespace GameServer;

public class NationWarCall
{
	private NationWarInstance m_nationWarInst;

	private Guid m_callerId = Guid.Empty;

	private string m_sCallerName;

	private int m_nCallerNoblesseId;

	private Continent m_continent;

	private int m_nNationId;

	private Vector3 m_position = Vector3.zero;

	private float m_fRotationY;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	public NationWarInstance nationWarInst => m_nationWarInst;

	public Guid callerId => m_callerId;

	public string callerName => m_sCallerName;

	public int callerNoblesseId => m_nCallerNoblesseId;

	public Continent continent => m_continent;

	public int nationId => m_nNationId;

	public Vector3 position => m_position;

	public float rotationY => m_fRotationY;

	public DateTimeOffset regTime => m_regTime;

	public NationWarCall(NationWarInstance nationWarInst, Guid callerId, string sCallerName, int nCallerNoblesseId, Continent continent, int nNationId, Vector3 position, float fRotationY, DateTimeOffset regTime)
	{
		m_nationWarInst = nationWarInst;
		m_callerId = callerId;
		m_sCallerName = sCallerName;
		m_nCallerNoblesseId = nCallerNoblesseId;
		m_continent = continent;
		m_nNationId = nNationId;
		m_position = position;
		m_fRotationY = fRotationY;
		m_regTime = regTime;
	}

	public Vector3 SelectPosition()
	{
		float fRadius = Resource.instance.nationWar.nationCallRadius;
		return Util.SelectPoint(m_position, fRadius);
	}
}
