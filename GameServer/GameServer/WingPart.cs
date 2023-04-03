using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WingPart
{
	private int m_nId;

	private int m_nAttrId;

	public int id => m_nId;

	public int attrId => m_nAttrId;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["partId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "날개 파트 ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		if (m_nAttrId <= 0)
		{
			SFLogUtil.Warn(GetType(), "속성ID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nAttrId = " + m_nAttrId);
		}
	}
}
