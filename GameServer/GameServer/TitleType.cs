using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class TitleType
{
	private int m_nId;

	private int m_nCategoryId;

	public int id => m_nId;

	public int category => m_nCategoryId;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["type"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "타입이 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nCategoryId = Convert.ToInt32(dr["categoryId"]);
		if (m_nCategoryId <= 0)
		{
			SFLogUtil.Warn(GetType(), "카테고리가 유효하지 않습니다. m_nId = " + m_nId + ", m_nCategoryId = " + m_nCategoryId);
		}
	}
}
