using System;
using System.Data;

namespace GameServer;

public class MonsterLevelGap
{
	private int m_nLevelGap;

	private float m_fExpFactor;

	public int levelGap => m_nLevelGap;

	public float expFactor => m_fExpFactor;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevelGap = Convert.ToInt32(dr["levelGap"]);
		m_fExpFactor = Convert.ToSingle(dr["expFactor"]);
	}
}
