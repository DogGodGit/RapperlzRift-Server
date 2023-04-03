using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TrueHeroQuestReward
{
	private TrueHeroQuest m_quest;

	private int m_nLevel;

	private ExpReward m_expReward;

	private ExploitPointReward m_exploitPointReward;

	public TrueHeroQuest quest => m_quest;

	public int level => m_nLevel;

	public ExpReward expReward => m_expReward;

	public long expRewardValue
	{
		get
		{
			if (m_expReward == null)
			{
				return 0L;
			}
			return m_expReward.value;
		}
	}

	public ExploitPointReward exploitPointReward => m_exploitPointReward;

	public int m_exploitPointRewardValue
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

	public TrueHeroQuestReward(TrueHeroQuest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_quest = quest;
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
		if (lnExpRewardId > 0)
		{
			m_expReward = Resource.instance.GetExpReward(lnExpRewardId);
			if (m_expReward == null)
			{
				SFLogUtil.Warn(GetType(), "경험치보상이 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnExpRewardId = " + lnExpRewardId);
			}
		}
		long lnExploitPointReawrdId = Convert.ToInt64(dr["exploitPointRewardId"]);
		if (lnExploitPointReawrdId > 0)
		{
			m_exploitPointReward = Resource.instance.GetExploitPointReward(lnExploitPointReawrdId);
			if (m_exploitPointReward == null)
			{
				SFLogUtil.Warn(GetType(), "공적포인트보상이 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnExploitPointReawrdId = " + lnExploitPointReawrdId);
			}
		}
	}
}
