using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DragonNestMonsterAttrFactor
{
	private DragonNest m_dragonNest;

	private int m_nLevel;

	private float m_fMaxHpFactor;

	private float m_fOffenseFactor;

	private float m_fTrapDamageFactor;

	public DragonNest dragonNest => m_dragonNest;

	public int level => m_nLevel;

	public float maxHpFactor => m_fMaxHpFactor;

	public float offenseFactor => m_fOffenseFactor;

	public float trapDamageFactor => m_fTrapDamageFactor;

	public DragonNestMonsterAttrFactor(DragonNest dragonNest)
	{
		m_dragonNest = dragonNest;
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
		m_fTrapDamageFactor = Convert.ToSingle(dr["trapDamageFactor"]);
		if (m_fTrapDamageFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "함정데미지계수가 유효하지 않습니다. m_fTrapDamageFactor = " + m_fTrapDamageFactor);
		}
	}
}
