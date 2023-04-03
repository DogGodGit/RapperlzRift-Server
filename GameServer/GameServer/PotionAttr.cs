using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class PotionAttr
{
	private int m_nId;

	private int m_nAttrId;

	private AttrValue m_incAttrValue;

	private int m_nRequiredItemId;

	public int id => m_nId;

	public int attrId => m_nAttrId;

	public int incAttrValue
	{
		get
		{
			if (m_incAttrValue == null)
			{
				return 0;
			}
			return m_incAttrValue.value;
		}
	}

	public int requiredItemId => m_nRequiredItemId;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["potionAttrId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "물약속성ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		if (!Attr.IsDefined(m_nAttrId))
		{
			SFLogUtil.Warn(GetType(), "속성ID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nAttrId = " + m_nAttrId);
		}
		long lnIncAttrValueId = Convert.ToInt64(dr["incAttrValueId"]);
		m_incAttrValue = Resource.instance.GetAttrValue(lnIncAttrValueId);
		if (m_incAttrValue == null)
		{
			SFLogUtil.Warn(GetType(), "증가속성값이 존재하지 않습니다. m_nId = " + m_nId + ", lnIncAttrValueId = " + lnIncAttrValueId);
		}
		m_nRequiredItemId = Convert.ToInt32(dr["requiredItemId"]);
		if (Resource.instance.GetItem(m_nRequiredItemId) == null)
		{
			SFLogUtil.Warn(GetType(), "필요아이템ID가 유효하지 않습니다. m_nId = " + m_nId + ", m_nRequiredItemId = " + m_nRequiredItemId);
		}
	}
}
