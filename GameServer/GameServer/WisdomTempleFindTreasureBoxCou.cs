using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WisdomTempleFindTreasureBoxCount
{
	private WisdomTemple m_wisdomTemple;

	private int m_nCount;

	private int m_nRewardCount;

	public WisdomTemple wisdomTemple => m_wisdomTemple;

	public int count => m_nCount;

	public int rewardCount => m_nRewardCount;

	public WisdomTempleFindTreasureBoxCount(WisdomTemple wisdomTemple)
	{
		m_wisdomTemple = wisdomTemple;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nCount = Convert.ToInt32(dr["count"]);
		m_nRewardCount = Convert.ToInt32(dr["rewardCount"]);
		if (m_nRewardCount < 0)
		{
			SFLogUtil.Warn(GetType(), "보상갯수가 유효하지 않습니다. m_nCount = " + m_nCount + ", m_nRewardCount = " + m_nRewardCount);
		}
	}
}
