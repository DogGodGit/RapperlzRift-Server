using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureBaseAttr
{
	private int m_nAttrId;

	public int attrId => m_nAttrId;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		if (m_nAttrId <= 0)
		{
			SFLogUtil.Warn(GetType(), "속성ID가 유효하지 않습니다. m_nAttrId = " + m_nAttrId);
		}
	}
}
