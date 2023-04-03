using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainGearDisassembleResultCountPool
{
	private MainGearTier m_tier;

	private int m_nGrade;

	private List<MainGearDisassembleResultCountPoolEntry> m_entries = new List<MainGearDisassembleResultCountPoolEntry>();

	private int m_nTotalPickPoint;

	public MainGearTier tier => m_tier;

	public int grade => m_nGrade;

	public MainGearDisassembleResultCountPool(MainGearTier tier, int nGrade)
	{
		m_tier = tier;
		m_nGrade = nGrade;
	}

	public void AddEntry(MainGearDisassembleResultCountPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		if (entry.pool != null)
		{
			throw new Exception("이미 분해결과개수풀에 추가되어있는 분해결과개수풀항목입니다.");
		}
		m_entries.Add(entry);
		entry.pool = this;
		m_nTotalPickPoint += entry.point;
	}

	public int SelectCount()
	{
		return Util.SelectPickEntry(m_entries, m_nTotalPickPoint).count;
	}
}
public class MainGearDisassembleResultCountPoolEntry : IPickEntry
{
	private MainGearDisassembleResultCountPool m_pool;

	private int m_nEntryNo;

	private int m_nPoint;

	private int m_nCount;

	public MainGearDisassembleResultCountPool pool
	{
		get
		{
			return m_pool;
		}
		set
		{
			m_pool = value;
		}
	}

	public int entryNo => m_nEntryNo;

	public int point => m_nPoint;

	public int count => m_nCount;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nEntryNo = Convert.ToInt32(dr["entryNo"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		m_nCount = Convert.ToInt32(dr["count"]);
		if (m_nCount < 0)
		{
			SFLogUtil.Warn(GetType(), "갯수가 유효하지 않습니다. m_nCount = " + m_nCount);
		}
	}
}
