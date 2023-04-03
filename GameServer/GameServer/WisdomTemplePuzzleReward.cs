using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WisdomTemplePuzzleReward
{
	private WisdomTemple m_wisdomTemple;

	private int m_nLevel;

	private ExpReward m_expReward;

	public WisdomTemple wisdomTemple => m_wisdomTemple;

	public int level => m_nLevel;

	public ExpReward expReward => m_expReward;

	public WisdomTemplePuzzleReward(WisdomTemple wisdomTemple)
	{
		m_wisdomTemple = wisdomTemple;
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
