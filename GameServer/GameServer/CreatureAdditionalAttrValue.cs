using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureAdditionalAttrValue
{
	private CreatureAdditionalAttr m_attr;

	private CreatureInjectionLevel m_injectionLevel;

	private AttrValue m_attrValue;

	public CreatureAdditionalAttr attr => m_attr;

	public CreatureInjectionLevel injectionLevel => m_injectionLevel;

	public AttrValue attrValue => m_attrValue;

	public CreatureAdditionalAttrValue(CreatureAdditionalAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attr = attr;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nAttrId = Convert.ToInt32(dr["attrId"]);
		m_attr = Resource.instance.GetCreatureAdditionalAttr(nAttrId);
		if (m_attr == null)
		{
			SFLogUtil.Warn(GetType(), "속성이 존재하지 않습니다. nAttrId = " + nAttrId);
		}
		int nInjectionLevel = Convert.ToInt32(dr["injectionLevel"]);
		m_injectionLevel = Resource.instance.GetCreatureInjectionLevel(nInjectionLevel);
		if (m_injectionLevel == null)
		{
			SFLogUtil.Warn(GetType(), "주입레벨이 존재하지 않습니다. nInjectionLevel = " + nInjectionLevel);
		}
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. lnAttrValueId = " + lnAttrValueId);
		}
	}
}
