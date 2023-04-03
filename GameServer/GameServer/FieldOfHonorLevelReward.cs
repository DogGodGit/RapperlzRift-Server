using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class FieldOfHonorLevelReward
{
	private FieldOfHonor m_fieldOfHonor;

	private int m_nLevel;

	private ExpReward m_expReward;

	public FieldOfHonor fieldOfHonor => m_fieldOfHonor;

	public int level => m_nLevel;

	public ExpReward expReward => m_expReward;

	public FieldOfHonorLevelReward(FieldOfHonor fieldOfHonor)
	{
		m_fieldOfHonor = fieldOfHonor;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["level"]);
		long lnExpRewardId = Convert.ToInt64(dr["expRewardId"]);
		if (lnExpRewardId > 0)
		{
			m_expReward = Resource.instance.GetExpReward(lnExpRewardId);
			if (m_expReward == null)
			{
				SFLogUtil.Warn(GetType(), "경험치보상이 존재하지 않습니다. m_nLevel = " + m_nLevel + ", lnExpRewardId = " + lnExpRewardId);
			}
		}
		else if (lnExpRewardId < 0)
		{
			SFLogUtil.Warn(GetType(), "경험치보상ID가 유효하지 않습니다. m_nLevel = " + m_nLevel + ", lnExpRewardId = " + lnExpRewardId);
		}
	}
}
