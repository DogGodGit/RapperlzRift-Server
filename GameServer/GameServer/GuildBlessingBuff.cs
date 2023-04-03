using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class GuildBlessingBuff
{
	private int m_nId;

	private float m_fExpRewardFactor;

	private int m_nDuration;

	private int m_nDia;

	public int id => m_nId;

	public float expRewardFactor => m_fExpRewardFactor;

	public int duration => m_nDuration;

	public int dia => m_nDia;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["buffId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "버프ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_fExpRewardFactor = Convert.ToSingle(dr["expRewardFactor"]);
		if (m_fExpRewardFactor <= 0f)
		{
			SFLogUtil.Warn(GetType(), "경험치보상계수가 유효하지 않습니다. m_nId = " + m_nId + ", m_fExpRewardFactor = " + m_fExpRewardFactor);
		}
		m_nDuration = Convert.ToInt32(dr["duration"]);
		if (m_nDuration <= 0)
		{
			SFLogUtil.Warn(GetType(), "지속시간이 유효하지 않습니다. m_nId = " + m_nId + ", m_nDuration = " + m_nDuration);
		}
		m_nDia = Convert.ToInt32(dr["dia"]);
		if (m_nDia <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요다이아가 유효하지 않습니다. m_nId = " + m_nId + ", m_nDia = " + m_nDia);
		}
	}
}
