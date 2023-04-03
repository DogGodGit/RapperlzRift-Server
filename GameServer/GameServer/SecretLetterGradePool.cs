using System;
using System.Collections.Generic;

namespace GameServer;

public class SecretLetterGradePool
{
	private int m_nId;

	private List<SecretLetterGradePoolEntry> m_entries = new List<SecretLetterGradePoolEntry>();

	private int m_nTotalPoint;

	public int id => m_nId;

	public SecretLetterGradePool(int nId)
	{
		m_nId = nId;
	}

	public void AddEntry(SecretLetterGradePoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_entries.Add(entry);
		m_nTotalPoint += entry.point;
	}

	public SecretLetterGradePoolEntry SelectEntry()
	{
		return Util.SelectPickEntry(m_entries, m_nTotalPoint);
	}
}
