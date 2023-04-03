using System;
using System.Data;

namespace GameServer;

public class WingEnchantCountPoolEntry : IPickEntry
{
	private WingEnchantCountPool m_pool;

	private int m_nId;

	private int m_nPoint;

	private int m_nCount;

	public WingEnchantCountPool pool
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

	public int id => m_nId;

	public int point => m_nPoint;

	public int count => m_nCount;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["entryId"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		m_nCount = Convert.ToInt32(dr["count"]);
	}
}
