using System;
using System.Collections.Generic;

namespace GameServer;

public class MysteryBoxGradePool
{
	private int m_nId;

	private List<MysteryBoxGradePoolEntry> m_entries = new List<MysteryBoxGradePoolEntry>();

	private int m_nTotalPoint;

	public int id => m_nId;

	public MysteryBoxGradePool(int nId)
	{
		m_nId = nId;
	}

	public void AddEntry(MysteryBoxGradePoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_entries.Add(entry);
		m_nTotalPoint += entry.point;
	}

	public MysteryBoxGradePoolEntry SelectEntry()
	{
		return Util.SelectPickEntry(m_entries, m_nTotalPoint);
	}
}
