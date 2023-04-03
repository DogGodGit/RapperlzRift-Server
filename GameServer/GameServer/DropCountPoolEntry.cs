using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DropCountPoolEntry : IPickEntry
{
	private DropCountPool m_pool;

	private int m_nNo;

	private int m_nPoint;

	private int m_nCount;

	public DropCountPool pool => m_pool;

	public int no => m_nNo;

	public int point => m_nPoint;

	public int count => m_nCount;

	int IPickEntry.point => m_nPoint;

	public DropCountPoolEntry(DropCountPool pool)
	{
		m_pool = pool;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "포인트가 유효하지 않습니다. poolId = " + m_pool.id + ", m_nNo = " + m_nNo);
		}
		m_nCount = Convert.ToInt32(dr["count"]);
		if (m_nCount < 0)
		{
			SFLogUtil.Warn(GetType(), "수량이 유효하지 않습니다. poolId = " + m_pool.id + ", m_nCount = " + m_nCount);
		}
	}
}
