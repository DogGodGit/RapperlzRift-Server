using System;
using System.Data;

namespace GameServer;

public class AttrValue
{
	private long m_lnId;

	private int m_nValue;

	public long id => m_lnId;

	public int value => m_nValue;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_lnId = Convert.ToInt64(dr["attrValueId"]);
		m_nValue = Convert.ToInt32(dr["value"]);
	}
}
