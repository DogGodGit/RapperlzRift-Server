using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WingMemoryPieceCriticalCountPool
{
	private int m_nType;

	private List<WingMemoryPieceCriticalCountPoolEntry> m_entries = new List<WingMemoryPieceCriticalCountPoolEntry>();

	private int m_nTotalPickPoint;

	public int type => m_nType;

	public WingMemoryPieceCriticalCountPool(int nType)
	{
		m_nType = nType;
	}

	public void AddEnty(WingMemoryPieceCriticalCountPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_entries.Add(entry);
		m_nTotalPickPoint += entry.point;
	}

	public int SelectCriticalCount()
	{
		return Util.SelectPickEntry(m_entries, m_nTotalPickPoint)?.count ?? 0;
	}
}
public class WingMemoryPieceCriticalCountPoolEntry : IPickEntry
{
	private WingMemoryPieceStep m_step;

	private WingMemoryPieceType m_type;

	private int m_nNo;

	private int m_nPoint;

	private int m_nCount;

	public WingMemoryPieceStep step => m_step;

	public WingMemoryPieceType type => m_type;

	public int no => m_nNo;

	public int point => m_nPoint;

	public int count => m_nCount;

	public WingMemoryPieceCriticalCountPoolEntry(WingMemoryPieceStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_step = step;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nType = Convert.ToInt32(dr["type"]);
		m_type = Resource.instance.GetWingMemoryPieceType(nType);
		if (m_type == null)
		{
			SFLogUtil.Warn(GetType(), "타입이 존재하지 않습니다. nType = " + nType);
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "항목번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nPoint = " + m_nPoint);
		}
		m_nCount = Convert.ToInt32(dr["count"]);
		if (m_nCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "수량이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nCount = " + m_nCount);
		}
	}
}
