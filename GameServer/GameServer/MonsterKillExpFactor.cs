using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MonsterKillExpFactor
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
		if (m_fExpFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "경험치계수가 유효하지 않습니다. m_nLevelGap = " + m_nLevelGap + ", m_fExpFactor = " + m_fExpFactor);
		}
	}
}
