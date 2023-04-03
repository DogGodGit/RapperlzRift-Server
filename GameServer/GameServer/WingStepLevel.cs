using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WingStepLevel
{
	private WingStep m_step;

	private int m_nLevel;

	private int m_nNextLevelUpRequiredExp;

	private int m_nAccEnchantLimitCount;

	public WingStep step
	{
		get
		{
			return m_step;
		}
		set
		{
			m_step = value;
		}
	}

	public int level => m_nLevel;

	public int nextLevelUpRequiredExp => m_nNextLevelUpRequiredExp;

	public int accEnchantLimitCount => m_nAccEnchantLimitCount;

	public bool isLastLevel => m_nLevel == m_step.lastLevel.level;

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
		m_nAccEnchantLimitCount = Convert.ToInt32(dr["AccEnchantLimitCount"]);
		if (m_nAccEnchantLimitCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "누적강화제한횟수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nAccEnchantLimitCount = " + m_nAccEnchantLimitCount);
		}
	}
}
