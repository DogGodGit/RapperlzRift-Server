using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimMonsterAttrFactor
{
	private RuinsReclaim m_ruinsReclaim;

	private int m_nLevel;

	private float m_fMaxHpFactor;

	private float m_fOffenseFactor;

	public RuinsReclaim ruinsReclaim => m_ruinsReclaim;

	public int level => m_nLevel;

	public float maxHpFactor => m_fMaxHpFactor;

	public float offenseFactor => m_fOffenseFactor;

	public RuinsReclaimMonsterAttrFactor(RuinsReclaim ruinsReclaim)
	{
		m_ruinsReclaim = ruinsReclaim;
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
			SFLogUtil.Warn(GetType(), "최대HP계수가 유효하지 않습니다. m_fMaxHpFactor = " + m_fMaxHpFactor);
		}
		m_fOffenseFactor = Convert.ToSingle(dr["offenseFactor"]);
		if (m_fOffenseFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "공격력계수가 유효하지 않습니다. m_fOffenseFactor = " + m_fOffenseFactor);
		}
	}
}
