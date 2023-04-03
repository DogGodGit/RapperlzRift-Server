using System;
using System.Collections.Generic;

namespace GameServer;

public class MainGearDisassembleResultPool
{
	private MainGearTier m_tier;

	private int m_nGrade;

	private List<MainGearDisassembleResultPoolEntry> m_entries = new List<MainGearDisassembleResultPoolEntry>();

	private int m_nTotalPickPoint;

	public MainGearTier tier => m_tier;

	public int grade => m_nGrade;

	public MainGearDisassembleResultPool(MainGearTier tier, int nGrade)
	{
		m_tier = tier;
		m_nGrade = nGrade;
	}

	public void AddEntry(MainGearDisassembleResultPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		if (entry.pool != null)
		{
			throw new Exception("이미 분해결과풀에 포함되어있는 분해결과풀항목입니다.");
		}
		m_entries.Add(entry);
		entry.pool = this;
		m_nTotalPickPoint += entry.point;
	}

	public List<MainGearDisassembleResultPoolEntry> SelectEntries(int nCount)
	{
		return Util.SelectPickEntries(m_entries, m_nTotalPickPoint, nCount, bDuplicated: true);
	}
}
