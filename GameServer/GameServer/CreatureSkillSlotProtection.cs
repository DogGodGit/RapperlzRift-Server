using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureSkillSlotProtection
{
	private int m_nCount;

	private int m_nRequiredSkillCount;

	private int m_nRequiredItemCount;

	public int count => m_nCount;

	public int requiredSkillCount => m_nRequiredSkillCount;

	public int requiredItemCount => m_nRequiredItemCount;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nCount = Convert.ToInt32(dr["protectionCount"]);
		if (m_nCount < 0)
		{
			SFLogUtil.Warn(GetType(), "보호개수가 유효하지 않습니다. m_nCount = " + m_nCount);
		}
		m_nRequiredSkillCount = Convert.ToInt32(dr["requiredSkillCount"]);
		if (m_nRequiredSkillCount < 0)
		{
			SFLogUtil.Warn(GetType(), "필요스킬개수가 유효하지 않습니다. m_nRequiredSkillCount = " + m_nRequiredSkillCount);
		}
		m_nRequiredItemCount = Convert.ToInt32(dr["requiredItemCount"]);
		if (m_nRequiredItemCount < 0)
		{
			SFLogUtil.Warn(GetType(), "필요아이템개수가 유효하지 않습니다. m_nRequiredItemCount = " + m_nRequiredItemCount);
		}
	}
}
