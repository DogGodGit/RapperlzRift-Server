using System;
using System.Data;

namespace GameServer;

public class Nation
{
	private int m_nId;

	private string m_sNameKey;

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["nationId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
	}
}
