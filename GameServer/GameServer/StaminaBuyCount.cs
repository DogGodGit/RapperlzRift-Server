using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class StaminaBuyCount
{
	private int m_nBuyCount;

	private int m_nStamina;

	private int m_nRequiredDia;

	public int buyCount => m_nBuyCount;

	public int stamina => m_nStamina;

	public int requiredDia => m_nRequiredDia;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nBuyCount = Convert.ToInt32(dr["buyCount"]);
		if (m_nBuyCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "구매횟수가 유효하지 않습니다. m_nBuyCount = " + m_nBuyCount);
		}
		m_nStamina = Convert.ToInt32(dr["stamina"]);
		if (m_nStamina <= 0)
		{
			SFLogUtil.Warn(GetType(), "스태미너가 유효하지 않습니다. m_nBuyCount = " + m_nBuyCount + ", m_nStamina = " + m_nStamina);
		}
		m_nRequiredDia = Convert.ToInt32(dr["requiredDia"]);
		if (m_nRequiredDia < 0)
		{
			SFLogUtil.Warn(GetType(), "필요다이아가 유효하지 않습니다. m_nBuyCount = " + m_nBuyCount + ", m_nRequiredDia = " + m_nRequiredDia);
		}
	}
}
