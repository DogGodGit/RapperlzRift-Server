using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WingMemoryPieceSuccessFactorPool
{
	private int m_nType;

	private List<WingMemoryPieceSuccessFactorPoolEntry> m_entries = new List<WingMemoryPieceSuccessFactorPoolEntry>();

	private int m_nTotalPickPoint;

	public int type => m_nType;

	public WingMemoryPieceSuccessFactorPool(int nType)
	{
		m_nType = nType;
	}

	public void AddEnty(WingMemoryPieceSuccessFactorPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_entries.Add(entry);
		m_nTotalPickPoint += entry.point;
	}

	public int SelectSuccessFactor()
	{
		return Util.SelectPickEntry(m_entries, m_nTotalPickPoint)?.count ?? 0;
	}
}
public class WingMemoryPieceSuccessFactorPoolEntry : IPickEntry
{
	private WingMemoryPieceStep m_step;

	private WingMemoryPieceType m_type;

	private int m_nNo;

	private int m_nPoint;

	private int m_nFactor;

	public WingMemoryPieceStep step => m_step;

	public WingMemoryPieceType type => m_type;

	public int no => m_nNo;

	public int point => m_nPoint;

	public int count => m_nFactor;

	public WingMemoryPieceSuccessFactorPoolEntry(WingMemoryPieceStep step)
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
		m_nFactor = Convert.ToInt32(dr["factor"]);
		if (m_nFactor <= 0)
		{
			SFLogUtil.Warn(GetType(), "개수가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nFactor = " + m_nFactor);
		}
	}
}
