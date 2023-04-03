using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CostumeEnchantLevel
{
	private int m_nLevel;

	private int m_nStep;

	private int m_nNextLevelUpSuccessRate;

	private int m_nNextLevelUpRequiredItemCount;

	private int m_nNextLevelUpMaxLuckyValue;

	public int level => m_nLevel;

	public int step => m_nStep;

	public int nextLevelUpSuccessRate => m_nNextLevelUpSuccessRate;

	public int nextLevelUpRequiredItemCount => m_nNextLevelUpRequiredItemCount;

	public int nextLevelUpMaxLuckyValue => m_nNextLevelUpMaxLuckyValue;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["enchantLevel"]);
		if (m_nLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "강화레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel);
		}
		m_nStep = Convert.ToInt32(dr["step"]);
		if (m_nStep < 0)
		{
			SFLogUtil.Warn(GetType(), "단계가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nStep = " + m_nStep);
		}
		m_nNextLevelUpSuccessRate = Convert.ToInt32(dr["nextLevelUpSuccessRate"]);
		if (m_nNextLevelUpSuccessRate < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업성공확률이 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nNextLevelUpSuccessRate = " + m_nNextLevelUpSuccessRate);
		}
		m_nNextLevelUpRequiredItemCount = Convert.ToInt32(dr["nextLevelUpRequiredItemCount"]);
		if (m_nNextLevelUpRequiredItemCount < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업필요아이템갯수가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nNextLevelUpRequiredItemCount = " + m_nNextLevelUpRequiredItemCount);
		}
		m_nNextLevelUpMaxLuckyValue = Convert.ToInt32(dr["nextLevelUpMaxLuckyValue"]);
		if (m_nNextLevelUpMaxLuckyValue < 0)
		{
			SFLogUtil.Warn(GetType(), "다음레벨업최대행운값이 유효하지 않습니다. m_nLevel = " + m_nLevel + ", m_nNextLevelUpMaxLuckyValue = " + m_nNextLevelUpMaxLuckyValue);
		}
	}
}
