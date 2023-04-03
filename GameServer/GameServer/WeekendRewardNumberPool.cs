using System;
using System.Collections.Generic;

namespace GameServer;

public class WeekendRewardNumberPool
{
	private int m_nSelectionNo;

	private List<WeekendRewardNumberPoolEntry> m_entries = new List<WeekendRewardNumberPoolEntry>();

	private int m_nTotalPickPoint;

	public int selectionNo => m_nSelectionNo;

	public WeekendRewardNumberPool(int nSelectionNo)
	{
		m_nSelectionNo = nSelectionNo;
	}

	public void AddEntry(WeekendRewardNumberPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_entries.Add(entry);
		m_nTotalPickPoint += entry.point;
	}

	public WeekendRewardNumberPoolEntry SelectEntry()
	{
		return Util.SelectPickEntry(m_entries, m_nTotalPickPoint);
	}
}
