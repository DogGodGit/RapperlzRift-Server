using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class JobSkillLevelMaster
{
	private JobSkillMaster m_skillMaster;

	private int m_nLevel;

	private int m_nNextLevelUpRequiredHeroLevel;

	private long m_lnNextLevelUpGold;

	private int m_nNextLevelUpItemId;

	private int m_nNextLevelUpItemCount;

	public JobSkillMaster skillMaster
	{
		get
		{
			return m_skillMaster;
		}
		set
		{
			m_skillMaster = value;
		}
	}

	public int level => m_nLevel;

	public int nextLevelUpRequiredHeroLevel => m_nNextLevelUpRequiredHeroLevel;

	public long nextLevelUpGold => m_lnNextLevelUpGold;

	public int nextLevelUpItemId => m_nNextLevelUpItemId;

	public int nextLevelUpItemCount => m_nNextLevelUpItemCount;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		if (m_nLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel);
		}
		m_nNextLevelUpRequiredHeroLevel = Convert.ToInt32(dr["nextLevelUprequiredHeroLevel"]);
		if (m_nNextLevelUpRequiredHeroLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "영웅요구레벨이 유효하지 않습니다. m_nNextLevelUpRequiredHeroLevel = " + m_nNextLevelUpRequiredHeroLevel);
		}
		m_lnNextLevelUpGold = Convert.ToInt64(dr["nextLevelUpGold"]);
		if (m_lnNextLevelUpGold < 0)
		{
			SFLogUtil.Warn(GetType(), "다음 레벨업 골드가 유효하지 않습니다. m_lnNextLevelUpGold = " + m_lnNextLevelUpGold);
		}
		m_nNextLevelUpItemId = Convert.ToInt32(dr["nextLevelUpItemId"]);
		if (m_nNextLevelUpItemId < 0)
		{
			SFLogUtil.Warn(GetType(), "다음 레벨업 아이템 ID가 유효하지 않습니다. m_nNextLevelUpItemId = " + m_nNextLevelUpItemId);
		}
		if (m_nNextLevelUpItemId > 0)
		{
			m_nNextLevelUpItemCount = Convert.ToInt32(dr["nextLevelUpItemCount"]);
			if (m_nNextLevelUpItemCount <= 0)
			{
				SFLogUtil.Warn(GetType(), "다음 레벨업 아이템 수량이 유효하지 않습니다. m_nNextLevelUpItemCount = " + m_nNextLevelUpItemCount);
			}
		}
	}
}
