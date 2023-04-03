using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureLevel
{
	private int m_nLevel;

	private int m_nNextLevelUpRequiredExp;

	private int m_nMaxInjectionLevel;

	public int level => m_nLevel;

	public int nextLevelUpRequiredExp => m_nNextLevelUpRequiredExp;

	public int maxInjectionLevel => m_nMaxInjectionLevel;

	public bool isLastLevel => m_nLevel >= Resource.instance.lastCreatureLevel.level;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		if (m_nLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel);
		}
		m_nNextLevelUpRequiredExp = Convert.ToInt32(dr["nextLevelUpRequiredExp"]);
		if (m_nNextLevelUpRequiredExp < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업필요경험치가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nNextLevelUpRequiredExp = " + m_nNextLevelUpRequiredExp);
		}
		m_nMaxInjectionLevel = Convert.ToInt32(dr["maxInjectionLevel"]);
		if (m_nMaxInjectionLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "최대주입레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nMaxInjectionLevel = " + m_nMaxInjectionLevel);
		}
	}
}
