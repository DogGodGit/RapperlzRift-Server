using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WisdomTempleMonsterAttrFactor
{
	private WisdomTemple m_wisdomTemple;

	private int m_nLevel;

	private float m_fMaxHpFactor;

	private float m_fOffenseFactor;

	public WisdomTemple wisdomTemple => m_wisdomTemple;

	public int level => m_nLevel;

	public float maxHpFactor => m_fMaxHpFactor;

	public float offenseFactor => m_fOffenseFactor;

	public WisdomTempleMonsterAttrFactor(WisdomTemple wisdomTemple)
	{
		m_wisdomTemple = wisdomTemple;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		m_fMaxHpFactor = Convert.ToSingle(dr["maxHpFactor"]);
		if (m_fMaxHpFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "최대HP계수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_fMaxHpFactor = " + m_fMaxHpFactor);
		}
		m_fOffenseFactor = Convert.ToSingle(dr["offenseFactor"]);
		if (m_fOffenseFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "공격력계수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_fOffenseFactor = " + m_fOffenseFactor);
		}
	}
}
