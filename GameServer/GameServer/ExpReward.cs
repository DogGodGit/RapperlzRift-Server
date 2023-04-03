using System;
using System.Data;

namespace GameServer;

public class ExpReward
{
	private long m_lnId;

	private long m_lnValue;

	public long id => m_lnId;

	public long value => m_lnValue;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_lnId = Convert.ToInt64(dr["expRewardId"]);
		m_lnValue = Convert.ToInt64(dr["value"]);
	}
}
