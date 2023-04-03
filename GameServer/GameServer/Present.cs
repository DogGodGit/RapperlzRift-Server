using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Present
{
	private int m_nId;

	private int m_nRequiredVipLevel;

	private int m_nRequiredDia;

	private int m_nPopularityPoint;

	private int m_nContributionPoint;

	private bool m_bIsMessageSend;

	private bool m_bIsEffectDisplay;

	public int id => m_nId;

	public int requiredVipLevel => m_nRequiredVipLevel;

	public int requiredDia => m_nRequiredDia;

	public int popularityPoint => m_nPopularityPoint;

	public int contributionPoint => m_nContributionPoint;

	public bool isMessageSend => m_bIsMessageSend;

	public bool isEffectDisplay => m_bIsEffectDisplay;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["presentId"]);
		m_nRequiredVipLevel = Convert.ToInt32(dr["requiredVipLevel"]);
		m_nRequiredDia = Convert.ToInt32(dr["requiredDia"]);
		if (m_nRequiredDia <= 0)
		{
			SFLogUtil.Warn(GetType(), "필요다이아가 유효하지 않습니다. m_nId = " + m_nId + ", m_nRequiredDia = " + m_nRequiredDia);
		}
		m_nPopularityPoint = Convert.ToInt32(dr["popularityPoint"]);
		if (m_nPopularityPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "인기점수가 유효하지 않습니다. m_nId = " + m_nId + ", m_nPopularityPoint = " + m_nPopularityPoint);
		}
		m_nContributionPoint = Convert.ToInt32(dr["contributionPoint"]);
		if (m_nContributionPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "공헌점수가 유효하지 않습니다. m_nId = " + m_nId + ", m_nContributionPoint = " + m_nContributionPoint);
		}
		m_bIsMessageSend = Convert.ToBoolean(dr["isMessageSend"]);
		m_bIsEffectDisplay = Convert.ToBoolean(dr["isEffectDisplay"]);
	}
}
