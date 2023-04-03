using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WingMemoryPieceType
{
	private int m_nType;

	private int m_nRequiredItemId;

	private int m_nSuceessRate;

	private int m_nCriticalRate;

	private int m_nCriticalFactor;

	public int type => m_nType;

	public int requiredItemId => m_nRequiredItemId;

	public int suceessRate => m_nSuceessRate;

	public int criticalRate => m_nCriticalRate;

	public int criticalFactor => m_nCriticalFactor;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nType = Convert.ToInt32(dr["type"]);
		if (m_nType < 0)
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nType = " + m_nType);
		}
		m_nRequiredItemId = Convert.ToInt32(dr["requiredItemId"]);
		if (m_nRequiredItemId > 0 && Resource.instance.GetItem(m_nRequiredItemId) == null)
		{
			SFLogUtil.Warn(GetType(), "필요아이템ID가 유효하지 않습니다. m_nType = " + m_nType + ", m_nRequiredItemId = " + m_nRequiredItemId);
		}
		m_nSuceessRate = Convert.ToInt32(dr["successRate"]);
		if (m_nSuceessRate < 0)
		{
			SFLogUtil.Warn(GetType(), "성공확률이 유효하지 않습니다. m_nType = " + m_nType + ", m_nSuceessRate = " + m_nSuceessRate);
		}
		m_nCriticalRate = Convert.ToInt32(dr["criticalRate"]);
		if (m_nCriticalRate < 0)
		{
			SFLogUtil.Warn(GetType(), "크리티컬확률이 유효하지 않습니다. m_nType = " + m_nType + ", m_nCriticalRate = " + m_nCriticalRate);
		}
		m_nCriticalFactor = Convert.ToInt32(dr["criticalFactor"]);
		if (m_nCriticalFactor < 0)
		{
			SFLogUtil.Warn(GetType(), "크리티컬계수가 유효하지 않습니다. m_nType = " + m_nType + ", m_nCriticalFactor = " + m_nCriticalFactor);
		}
	}
}
