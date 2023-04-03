using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class InfiniteWarBuffBox : IPickEntry
{
	private InfiniteWar m_infiniteWar;

	private int m_nId;

	private float m_fOffenseFactor;

	private float m_fDefenseFactor;

	private float m_fHPRecoveryFactor;

	public InfiniteWar infiniteWar => m_infiniteWar;

	public int id => m_nId;

	public float offenseFactor => m_fOffenseFactor;

	public float defenseFactor => m_fDefenseFactor;

	public float hpRecoveryFactor => m_fHPRecoveryFactor;

	public int point => 1;

	int IPickEntry.point => point;

	public InfiniteWarBuffBox(InfiniteWar infiniteWar)
	{
		m_infiniteWar = infiniteWar;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["buffBoxId"]);
		m_fOffenseFactor = Convert.ToSingle(dr["offenseFactor"]);
		if (m_fOffenseFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "공격력계수가 유효하지 않습니다. m_nId = " + m_nId + ", m_fOffenseFactor = " + m_fOffenseFactor);
		}
		m_fDefenseFactor = Convert.ToSingle(dr["defenseFactor"]);
		if (m_fDefenseFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "방어력계수가 유효하지 않습니다. m_nId = " + m_nId + ", m_fDefenseFactor = " + m_fDefenseFactor);
		}
		m_fHPRecoveryFactor = Convert.ToSingle(dr["hpRecoveryFactor"]);
		if (m_fHPRecoveryFactor < 0f)
		{
			SFLogUtil.Warn(GetType(), "HP회복계수가 유효하지 않습니다. m_nId = " + m_nId + ", m_fHPRecoveryFactor = " + m_fHPRecoveryFactor);
		}
	}
}
