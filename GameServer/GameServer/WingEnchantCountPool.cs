using System;
using System.Collections.Generic;

namespace GameServer;

public class WingEnchantCountPool
{
	private List<WingEnchantCountPoolEntry> m_entries = new List<WingEnchantCountPoolEntry>();

	private int m_nTotalPickPoint;

	public List<WingEnchantCountPoolEntry> entries => m_entries;

	public int totalPickPoint => m_nTotalPickPoint;

	public void AddEntry(WingEnchantCountPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		if (entry.pool != null)
		{
			throw new Exception("이미 날개강화횟수풀에 추가된 날개강화횟수풀항목 입니다.");
		}
		m_entries.Add(entry);
		entry.pool = this;
		m_nTotalPickPoint += entry.point;
	}

	public WingEnchantCountPoolEntry SelectEntry()
	{
		return Util.SelectPickEntry(m_entries, m_nTotalPickPoint);
	}
}
