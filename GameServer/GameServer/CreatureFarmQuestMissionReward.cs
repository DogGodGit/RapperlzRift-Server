using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureFarmQuestMissionReward
{
	private CreatureFarmQuestMission m_mission;

	private int m_nLevel;

	private ExpReward m_expReward;

	public CreatureFarmQuestMission mission => m_mission;

	public int level => m_nLevel;

	public ExpReward expReward => m_expReward;

	public long expRewardValue => m_expReward.value;

	public CreatureFarmQuestMissionReward(CreatureFarmQuestMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_mission = mission;
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
			SFLogUtil.Warn(GetType(), "레벨이 부족합니다. m_nLevel = " + m_nLevel);
		}
		long lnExpRewardId = Convert.ToInt64(dr["expRewardId"]);
		if (lnExpRewardId > 0)
		{
			m_expReward = Resource.instance.GetExpReward(lnExpRewardId);
			if (m_expReward == null)
			{
				SFLogUtil.Warn(GetType(), "경험치보상이 존재하지 않습니다. lnExpRewardId = " + lnExpRewardId);
			}
		}
	}
}
