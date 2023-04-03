using System;
using System.Collections.Generic;

namespace GameServer;

public class DropCountPool
{
	private int m_nId;

	private List<DropCountPoolEntry> m_entries = new List<DropCountPoolEntry>();

	private int m_nEntryTotalPoint;

	public int id => m_nId;

	public DropCountPool(int nId)
	{
		m_nId = nId;
	}

	public void AddEntry(DropCountPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_entries.Add(entry);
		m_nEntryTotalPoint += entry.point;
	}

	public int SelectDropCount()
	{
		return SelectEntry()?.count ?? 0;
	}

	private DropCountPoolEntry SelectEntry()
	{
		return Util.SelectPickEntry(m_entries, m_nEntryTotalPoint);
	}
}
