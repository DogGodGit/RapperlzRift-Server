using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NationFundReward
{
	private long m_lnId;

	private int m_nValue;

	public long id => m_lnId;

	public int value => m_nValue;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_lnId = Convert.ToInt64(dr["nationFundRewardId"]);
		if (m_lnId <= 0)
		{
			SFLogUtil.Warn(GetType(), "국고자금보상ID가 유효하지 않습니다. m_lnId = " + m_lnId);
		}
		m_nValue = Convert.ToInt32(dr["value"]);
		if (m_nValue < 0)
		{
			SFLogUtil.Warn(GetType(), "국고자금보상이 유효하지 않습니다. m_lnId = " + m_lnId + ", m_nValue = " + m_nValue);
		}
	}
}
