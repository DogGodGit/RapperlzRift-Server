using System;
using System.Collections.Generic;

namespace GameServer;

public class PlaceAlliance
{
	private Guid m_id = Guid.Empty;

	private List<int> m_nationIds = new List<int>();

	public Guid id => m_id;

	public PlaceAlliance(Guid id, List<int> nationIds)
	{
		m_id = id;
		m_nationIds = nationIds;
	}

	public bool IsAlliance(int nNationId1, int nNationId2)
	{
		if (m_nationIds.Contains(nNationId1))
		{
			return m_nationIds.Contains(nNationId2);
		}
		return false;
	}
}
