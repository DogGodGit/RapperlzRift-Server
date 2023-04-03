using System;
using System.Collections.Generic;

namespace GameServer;

public class MainGearOptionAttrPool
{
	private MainGearTier m_tier;

	private int m_nGrade;

	private List<MainGearOptionAttrPoolEntry> m_entries = new List<MainGearOptionAttrPoolEntry>();

	private int m_nTotalPickPoint;

	public MainGearTier tier => m_tier;

	public int grade => m_nGrade;

	public MainGearOptionAttrPool(MainGearTier tier, int nGrade)
	{
		m_tier = tier;
		m_nGrade = nGrade;
	}

	public void AddEntry(MainGearOptionAttrPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		if (entry.pool != null)
		{
			throw new Exception("이미 메인장비옵션속성풀에 추가된 메인장비옵션속성풀항목입니다.");
		}
		m_entries.Add(entry);
		entry.pool = this;
		m_nTotalPickPoint += entry.point;
	}

	public MainGearOptionAttrPoolEntry SelectEntry()
	{
		return Util.SelectPickEntry(m_entries, m_nTotalPickPoint);
	}

	public List<MainGearOptionAttrPoolEntry> SelectEntries(int nCount)
	{
		return Util.SelectPickEntries(m_entries, m_nTotalPickPoint, nCount, bDuplicated: true);
	}
}
