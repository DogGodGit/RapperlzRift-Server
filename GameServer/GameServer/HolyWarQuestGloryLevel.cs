using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class HolyWarQuestGloryLevel
{
	private int m_nLevel;

	private int m_nRequiredKillCount;

	private ExploitPointReward m_exploitPointReward;

	public int level => m_nLevel;

	public int requiredKillCount => m_nRequiredKillCount;

	public ExploitPointReward exploitPointReward => m_exploitPointReward;

	public int exploitPointRewardValue
	{
		get
		{
			if (m_exploitPointReward == null)
			{
				return 0;
			}
			return m_exploitPointReward.value;
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["gloryLevel"]);
		if (m_nLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "영광레벨이 유효하지 않습니다. m_nLevel = " + m_nLevel);
		}
		m_nRequiredKillCount = Convert.ToInt32(dr["requiredKillCount"]);
		if (m_nRequiredKillCount < 0)
		{
			SFLogUtil.Warn(GetType(), "필요킬수가 유효하지 않습니다. m_nRequiredKillCount = " + m_nRequiredKillCount);
		}
		long lnExploitPointRewardId = Convert.ToInt64(dr["exploitPointRewardId"]);
		m_exploitPointReward = Resource.instance.GetExploitPointReward(lnExploitPointRewardId);
		if (m_exploitPointReward == null)
		{
			SFLogUtil.Warn(GetType(), "공적포인트보상이 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnExploitPointRewardId = " + lnExploitPointRewardId);
		}
	}
}
