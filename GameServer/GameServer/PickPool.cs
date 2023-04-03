using System;
using System.Collections.Generic;

namespace GameServer;

public class PickPool
{
	private int m_nId;

	private List<PickPoolEntry> m_fixedEntries = new List<PickPoolEntry>();

	private List<PickPoolEntry> m_randomEntries = new List<PickPoolEntry>();

	public int id => m_nId;

	public List<PickPoolEntry> fixedEntries => m_fixedEntries;

	public List<PickPoolEntry> randomEntries => m_randomEntries;

	public PickPool(int nId)
	{
		m_nId = nId;
	}

	public void AddEntry(PickPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		if (entry.pool != null)
		{
			throw new Exception("이미 뽑기풀에 추가된 뽑기풀항목입니다.");
		}
		if (entry.point > 0)
		{
			m_randomEntries.Add(entry);
		}
		else
		{
			m_fixedEntries.Add(entry);
		}
		entry.pool = this;
	}

	public List<PickPoolEntry> SelectEntries(int nJobId, int nLevel)
	{
		List<PickPoolEntry> resultEntries = new List<PickPoolEntry>();
		foreach (PickPoolEntry entry2 in m_fixedEntries)
		{
			if ((entry2.jobId <= 0 || entry2.jobId == nJobId) && entry2.IsAvaliableHeroLevel(nLevel))
			{
				resultEntries.Add(entry2);
			}
		}
		List<PickPoolEntry> randomPoolEntries = new List<PickPoolEntry>();
		int nTotalPoint = 0;
		foreach (PickPoolEntry entry in m_randomEntries)
		{
			if ((entry.jobId <= 0 || entry.jobId == nJobId) && entry.IsAvaliableHeroLevel(nLevel))
			{
				randomPoolEntries.Add(entry);
				nTotalPoint += entry.point;
			}
		}
		PickPoolEntry selectedEntry = Util.SelectPickEntry(randomPoolEntries, nTotalPoint);
		if (selectedEntry != null)
		{
			resultEntries.Add(selectedEntry);
		}
		return resultEntries;
	}
}
