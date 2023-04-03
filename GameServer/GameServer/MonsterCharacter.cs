using System;
using System.Data;

namespace GameServer;

public class MonsterCharacter
{
	private int m_nId;

	private string m_sNameKey;

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentException("dr");
		}
		m_nId = Convert.ToInt32(dr["monsterCharacterId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
	}
}
