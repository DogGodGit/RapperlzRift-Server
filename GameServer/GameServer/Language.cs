using System;
using System.Data;

namespace GameServer;

public class Language
{
	private int m_nId;

	private bool m_bSupported;

	public int id => m_nId;

	public bool supported
	{
		get
		{
			return m_bSupported;
		}
		set
		{
			m_bSupported = value;
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["languageId"]);
	}
}
