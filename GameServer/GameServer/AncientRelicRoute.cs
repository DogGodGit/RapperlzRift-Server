using System;
using System.Data;

namespace GameServer;

public class AncientRelicRoute : IPickEntry
{
	private AncientRelic m_ancientRelic;

	private int m_nId;

	private int m_nPoint;

	public AncientRelic ancientRelic => m_ancientRelic;

	public int id => m_nId;

	public int point => m_nPoint;

	int IPickEntry.point => m_nPoint;

	public AncientRelicRoute(AncientRelic ancientRelic)
	{
		m_ancientRelic = ancientRelic;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["routeId"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
	}
}
