using System;
using System.Data;

namespace GameServer;

public class HonorPointReward
{
	private long m_lnHonorPointRewardId;

	private int m_nValue;

	public long honorPointRewardId => m_lnHonorPointRewardId;

	public int value => m_nValue;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_lnHonorPointRewardId = Convert.ToInt64(dr["honorPointRewardId"]);
		m_nValue = Convert.ToInt32(dr["value"]);
	}
}
