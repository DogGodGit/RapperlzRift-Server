using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainGearRefinementRecipe
{
	private int m_nProtectionCount;

	private int m_nProtectionItemId;

	public int protectionCount => m_nProtectionCount;

	public int protectionItemId => m_nProtectionItemId;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nProtectionCount = Convert.ToInt32(dr["protectionCount"]);
		if (m_nProtectionCount <= 0)
		{
			SFLogUtil.Warn(GetType(), "보호갯수가 유효하지 않습니다. m_nProtectionCount = " + m_nProtectionCount);
		}
		m_nProtectionItemId = Convert.ToInt32(dr["protectionItemId"]);
		if (m_nProtectionItemId <= 0)
		{
			SFLogUtil.Warn(GetType(), "보호아이템ID가 유효하지 않습니다. m_nProtectionItemId = " + m_nProtectionItemId);
		}
	}
}
