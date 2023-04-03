using System;
using System.Data;

namespace GameServer;

public class PaidImmediateRevival
{
	private int m_nRevivalCount;

	private int m_nRequiredDia;

	public int revivalCount => m_nRevivalCount;

	public int requiredDia => m_nRequiredDia;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRevivalCount = Convert.ToInt32(dr["revivalCount"]);
		m_nRequiredDia = Convert.ToInt32(dr["requiredDia"]);
	}
}
