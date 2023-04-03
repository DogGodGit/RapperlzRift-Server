using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DimensionRaidQuestReward
{
	private int m_nLevel;

	private ExpReward m_expReward;

	private ExploitPointReward m_exploitPointReward;

	private ItemReward m_itemReward;

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

	public ItemReward itemReward => m_itemReward;

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
		m_expReward = Resource.instance.GetExpReward(lnExpRewardId);
		if (m_expReward == null)
		{
			SFLogUtil.Warn(GetType(), "경험치보상이 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnExpRewardId = " + lnExpRewardId);
		}
		long lnExploitPointRewardId = Convert.ToInt64(dr["exploitPointRewardId"]);
		m_exploitPointReward = Resource.instance.GetExploitPointReward(lnExploitPointRewardId);
		if (m_exploitPointReward == null)
		{
			SFLogUtil.Warn(GetType(), "공적포인트보상이 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnExploitPointRewardId = " + lnExploitPointRewardId);
		}
		long lnItemRewardId = Convert.ToInt64(dr["itemRewardId"]);
		m_itemReward = Resource.instance.GetItemReward(lnItemRewardId);
		if (m_itemReward == null)
		{
			SFLogUtil.Warn(GetType(), "아이템보상이 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnItemRewardId = " + lnItemRewardId);
		}
	}
}
