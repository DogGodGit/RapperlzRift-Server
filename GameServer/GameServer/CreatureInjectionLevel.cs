using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureInjectionLevel
{
	private int m_nLevel;

	private int m_nNextLevelUpRequiredExp;

	private int m_nRequiredItemCount;

	private long m_lnRequiredGold;

	public int level => m_nLevel;

	public int nextLevelUpRequiredExp => m_nNextLevelUpRequiredExp;

	public int requiredItemCount => m_nRequiredItemCount;

	public long requiredGold => m_lnRequiredGold;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["injectionLevel"]);
		if (m_nLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel);
		}
		m_nNextLevelUpRequiredExp = Convert.ToInt32(dr["nextLevelUpRequiredExp"]);
		if (m_nNextLevelUpRequiredExp < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업필요경험치가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nNextLevelUpRequiredExp = " + m_nNextLevelUpRequiredExp);
		}
		m_nRequiredItemCount = Convert.ToInt32(dr["requiredItemCount"]);
		if (m_nRequiredItemCount < 0)
		{
			SFLogUtil.Warn(GetType(), "필요아이템수량이 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nRequiredItemCount = " + m_nRequiredItemCount);
		}
		m_lnRequiredGold = Convert.ToInt64(dr["requiredGold"]);
	}
}
