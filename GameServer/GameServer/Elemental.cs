using System;
using System.Data;

namespace GameServer;

public class Elemental
{
	public const int kType_Fire = 1;

	public const int kType_Lightning = 2;

	public const int kType_Holy = 3;

	public const int kType_Dark = 4;

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
		m_nId = Convert.ToInt32(dr["elementalId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
	}
}
