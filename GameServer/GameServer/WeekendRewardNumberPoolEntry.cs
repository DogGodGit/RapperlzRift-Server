using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WeekendRewardNumberPoolEntry : IPickEntry
{
	private WeekendRewardNumberPool m_pool;

	private int m_nNo;

	private int m_nPoint;

	private int m_nNumber;

	public WeekendRewardNumberPool pool => m_pool;

	public int no => m_nNo;

	public int point => m_nPoint;

	public int number => m_nNumber;

	public WeekendRewardNumberPoolEntry(WeekendRewardNumberPool pool)
	{
		if (pool == null)
		{
			throw new ArgumentNullException("pool");
		}
		m_pool = pool;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "항목번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nPoint = " + m_nPoint);
		}
		m_nNumber = Convert.ToInt32(dr["number"]);
		if (m_nNumber < 0)
		{
			SFLogUtil.Warn(GetType(), "넘버가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nNumber = " + m_nNumber);
		}
	}
}
