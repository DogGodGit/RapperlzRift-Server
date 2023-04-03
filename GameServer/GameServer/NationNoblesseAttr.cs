using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class NationNoblesseAttr
{
	private NationNoblesse m_nationNoblesse;

	private int m_nId;

	private AttrValue m_attrValue;

	public NationNoblesse nationNoblesse => m_nationNoblesse;

	public int id => m_nId;

	public AttrValue attrValue => m_attrValue;

	public NationNoblesseAttr(NationNoblesse nationNoblesse)
	{
		m_nationNoblesse = nationNoblesse;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["attrId"]);
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		if (lnAttrValueId > 0)
		{
			m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
			if (m_attrValue == null)
			{
				SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. nationNoblesseId = " + m_nationNoblesse.id + ", m_nId = " + m_nId + ", lnAttrValueId = " + lnAttrValueId);
			}
		}
		else if (lnAttrValueId < 0)
		{
			SFLogUtil.Warn(GetType(), "속성값ID가 유효하지 않습니다. nationNoblesseId = " + m_nationNoblesse.id + ", m_nId = " + m_nId + ", lnAttrValueId = " + lnAttrValueId);
		}
	}
}
