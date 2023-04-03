using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Rank
{
	private int m_nNo;

	private int m_nRequiredExploitPoint;

	private GoldReward m_goldReward;

	private Dictionary<int, RankAttr> m_attrs = new Dictionary<int, RankAttr>();

	private Dictionary<int, RankReward> m_rewards = new Dictionary<int, RankReward>();

	private Dictionary<int, RankActiveSkill> m_activeSkills = new Dictionary<int, RankActiveSkill>();

	private Dictionary<int, RankPassiveSkill> m_passiveSkills = new Dictionary<int, RankPassiveSkill>();

	public int no => m_nNo;

	public int requiredExploitPoint => m_nRequiredExploitPoint;

	public GoldReward goldReward => m_goldReward;

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

	public Dictionary<int, RankAttr> attrs => m_attrs;

	public Dictionary<int, RankReward> rewards => m_rewards;

	public Dictionary<int, RankActiveSkill> activeSkills => m_activeSkills;

	public Dictionary<int, RankPassiveSkill> passiveSkills => m_passiveSkills;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["rankNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "계급번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nRequiredExploitPoint = Convert.ToInt32(dr["requiredExploitPoint"]);
		if (m_nRequiredExploitPoint < 0)
		{
			SFLogUtil.Warn(GetType(), "요구공적포인트가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nRequiredExploitPoint = " + m_nRequiredExploitPoint);
		}
		long lnGoldRewardId = Convert.ToInt64(dr["goldRewardId"]);
		if (lnGoldRewardId <= 0)
		{
			SFLogUtil.Warn(GetType(), "골드보상ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", lnGoldRewardId = " + lnGoldRewardId);
		}
		m_goldReward = Resource.instance.GetGoldReward(lnGoldRewardId);
		if (m_goldReward == null)
		{
			SFLogUtil.Warn(GetType(), "골드보상이 존재하지 않습니다. m_nNo = " + m_nNo + ", lnGoldRewardId = " + lnGoldRewardId);
		}
	}

	public void AddAttr(RankAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attrs.Add(attr.id, attr);
	}

	public void AddReward(RankReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_rewards.Add(reward.no, reward);
	}

	public void AddActiveSkills(RankActiveSkill activeSkill)
	{
		if (activeSkill == null)
		{
			throw new ArgumentNullException("activeSkill");
		}
		m_activeSkills.Add(activeSkill.id, activeSkill);
	}

	public void AddPassiveSkill(RankPassiveSkill passiveSkill)
	{
		if (passiveSkill == null)
		{
			throw new ArgumentNullException("passiveSkill");
		}
		m_passiveSkills.Add(passiveSkill.id, passiveSkill);
	}
}
