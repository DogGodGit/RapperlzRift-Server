using System;
using System.Data;

namespace GameServer;

public class WarMemoryMonsterAttrFactor
{
	private WarMemory m_warMemory;

	private int m_nLevel;

	private float m_fMaxHpFactor;

	private float m_fOffenseFactor;

	public WarMemory warMemory => m_warMemory;

	public int level => m_nLevel;

	public float maxHpFactor => m_fMaxHpFactor;

	public float offenseFactor => m_fOffenseFactor;

	public WarMemoryMonsterAttrFactor(WarMemory warMemory)
	{
		m_warMemory = warMemory;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		m_fMaxHpFactor = Convert.ToSingle(dr["maxHpFactor"]);
		m_fOffenseFactor = Convert.ToSingle(dr["offenseFactor"]);
	}
}
