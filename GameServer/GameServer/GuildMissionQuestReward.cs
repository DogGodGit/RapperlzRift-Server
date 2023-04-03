using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildMissionQuestReward
{
	private int m_nLevel;

	private ExpReward m_expReawrd;

	public int level => m_nLevel;

	public ExpReward expReward => m_expReawrd;

	public long expRewardValue
	{
		get
		{
			if (m_expReawrd == null)
			{
				return 0L;
			}
			return m_expReawrd.value;
		}
	}

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
		long lnExpRewardId = Convert.ToInt64(dr["expRewardId"]);
		m_expReawrd = Resource.instance.GetExpReward(lnExpRewardId);
		if (m_expReawrd == null)
		{
			SFLogUtil.Warn(GetType(), "보상경험치가 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnExpRewardId = " + lnExpRewardId);
		}
	}
}
