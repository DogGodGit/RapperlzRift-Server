using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SupplySupportQuestReward
{
	private SupplySupportQuest m_supplySupportQuest;

	private int m_nCartId;

	private int m_nLevel;

	private ExpReward m_expReward;

	private GoldReward m_goldReward;

	private ExploitPointReward m_exploitPointReward;

	public SupplySupportQuest supplySupportQuest => m_supplySupportQuest;

	public int cartId => m_nCartId;

	public int level => m_nLevel;

	public ExpReward expReward => m_expReward;

	public GoldReward goldReward => m_goldReward;

	public ExploitPointReward exploitPointReward => m_exploitPointReward;

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

	public long goldRewardValue
	{
		get
		{
			if (m_goldReward == null)
			{
				return 0L;
			}
			return m_goldReward.value;
		}
	}

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

	public SupplySupportQuestReward(SupplySupportQuest supplySupportQuest)
	{
		m_supplySupportQuest = supplySupportQuest;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nCartId = Convert.ToInt32(dr["cartId"]);
		m_nLevel = Convert.ToInt32(dr["level"]);
		long lnExpRewardId = Convert.ToInt64(dr["expRewardId"]);
		if (lnExpRewardId > 0)
		{
			m_expReward = Resource.instance.GetExpReward(lnExpRewardId);
			if (m_expReward == null)
			{
				SFLogUtil.Warn(GetType(), "경험치보상이 존재하지 않습니다. m_nCartId = " + m_nCartId + ", m_nLevel = " + m_nLevel + ", lnExpRewardId = " + lnExpRewardId);
			}
		}
		else if (lnExpRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "경험치보상ID가 유효하지 않습니다. m_nCartId = " + m_nCartId + ", m_nLevel = " + m_nLevel + ", lnExpRewardId = " + lnExpRewardId);
		}
		long lnGoldRewardId = Convert.ToInt64(dr["goldRewardId"]);
		if (lnGoldRewardId > 0)
		{
			m_goldReward = Resource.instance.GetGoldReward(lnGoldRewardId);
			if (m_goldReward == null)
			{
				SFLogUtil.Warn(GetType(), "골드보상이 존재하지 않습니다. m_nCartId = " + m_nCartId + ", m_nLevel = " + m_nLevel + ", lnGoldRewardId = " + lnGoldRewardId);
			}
		}
		else if (lnGoldRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "골드보상ID가 유효하지 않습니다. m_nCartId = " + m_nCartId + ", m_nLevel = " + m_nLevel + ", lnGoldRewardId = " + lnGoldRewardId);
		}
		long lnExploitPointRewardId = Convert.ToInt64(dr["exploitPointRewardId"]);
		if (lnExploitPointRewardId > 0)
		{
			m_exploitPointReward = Resource.instance.GetExploitPointReward(lnExploitPointRewardId);
			if (m_exploitPointReward == null)
			{
				SFLogUtil.Warn(GetType(), "공적점수보상이 존재하지 않습니다. m_nCartId = " + m_nCartId + ", m_nLevel = " + m_nLevel + ", lnExploitPointRewardId = " + lnExploitPointRewardId);
			}
		}
		else if (lnExploitPointRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "공적점수보상ID가 유효하지 않습니다. m_nCartId = " + m_nCartId + ", m_nLevel = " + m_nLevel + ", lnExploitPointRewardId = " + lnExploitPointRewardId);
		}
	}
}
