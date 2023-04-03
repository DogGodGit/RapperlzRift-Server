using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WeeklyQuestRoundReward
{
	private int m_nRoundNo;

	private int m_nLevel;

	private ExpReward m_expReward;

	private GoldReward m_goldReward;

	public int roundNo => m_nRoundNo;

	public int level => m_nLevel;

	public ExpReward expReward => m_expReward;

	public long expRewardValue => m_expReward.value;

	public GoldReward goldReward => m_goldReward;

	public long goldRewardValue => m_goldReward.value;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nRoundNo = Convert.ToInt32(dr["roundNo"]);
		if (m_nRoundNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "라운드번호가 유효하지 않습니다. m_nRoundNo = " + m_nRoundNo);
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		if (m_nLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "레벨이 유효하지 않습니다. m_nRoundNo = " + m_nRoundNo + ", m_nLevel = " + m_nLevel);
		}
		long lnExpRewardId = Convert.ToInt64(dr["expRewardId"]);
		if (lnExpRewardId > 0)
		{
			m_expReward = Resource.instance.GetExpReward(lnExpRewardId);
			if (m_expReward == null)
			{
				SFLogUtil.Warn(GetType(), "보상경험치가 유효하지 않습니다. m_nRoundNo = " + m_nRoundNo + ", m_nLevel = " + m_nLevel + ", lnExpRewardId = " + lnExpRewardId);
			}
		}
		long lnGoldReward = Convert.ToInt64(dr["goldRewardId"]);
		if (lnGoldReward > 0)
		{
			m_goldReward = Resource.instance.GetGoldReward(lnGoldReward);
			if (m_goldReward == null)
			{
				SFLogUtil.Warn(GetType(), "보상골드가 유효하지 않습니다. m_nRoundNo = " + m_nRoundNo + ", m_nLevel = " + m_nLevel + ", lnGoldReward = " + lnGoldReward);
			}
		}
	}
}
