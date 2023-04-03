using System;
using System.Collections.Generic;

namespace GameServer;

public class DropObjectPool
{
	private int m_nId;

	private List<DropObjectPoolEntry> m_fixedEntries = new List<DropObjectPoolEntry>();

	private List<DropObjectPoolEntry> m_randomEntries = new List<DropObjectPoolEntry>();

	private int m_nEntryTotalPoint;

	public int id => m_nId;

	public List<DropObjectPoolEntry> fixedEntries => m_fixedEntries;

	public DropObjectPool(int nId)
	{
		m_nId = nId;
	}

	public void AddEntry(DropObjectPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		if (entry.point <= 0)
		{
			m_fixedEntries.Add(entry);
			return;
		}
		m_randomEntries.Add(entry);
		m_nEntryTotalPoint += entry.point;
	}

	public DropObjectPoolEntry SelectEntry()
	{
		return Util.SelectPickEntry(m_randomEntries, m_nEntryTotalPoint);
	}
}
