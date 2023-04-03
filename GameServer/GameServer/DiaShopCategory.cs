using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class DiaShopCategory
{
	private int m_nId;

	private int m_nRequiredVipLevel;

	public int id => m_nId;

	public int requiredVipLevel => m_nRequiredVipLevel;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["categoryId"]);
		if (m_nId < 0)
		{
			SFLogUtil.Warn(GetType(), "카테고리ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nRequiredVipLevel = Convert.ToInt32(dr["requiredVipLevel"]);
		if (m_nRequiredVipLevel < 0)
		{
			SFLogUtil.Warn(GetType(), "필요VIP레벨이 유효하지 않습니다 m_nId = " + m_nId + ", m_nRequiredVipLevel = " + m_nRequiredVipLevel);
		}
	}
}
