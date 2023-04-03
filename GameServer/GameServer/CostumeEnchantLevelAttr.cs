using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CostumeEnchantLevelAttr
{
	private CostumeAttr m_attr;

	private CostumeEnchantLevel m_level;

	private AttrValue m_attrValue;

	public CostumeAttr attr => m_attr;

	public CostumeEnchantLevel level => m_level;

	public int attrValue
	{
		get
		{
			if (m_attrValue == null)
			{
				return 0;
			}
			return m_attrValue.value;
		}
	}

	public CostumeEnchantLevelAttr(CostumeAttr attr)
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
		int nLevel = Convert.ToInt32(dr["enchantLevel"]);
		m_level = Resource.instance.GetCostumeEnchantLevel(nLevel);
		if (m_level == null)
		{
			SFLogUtil.Warn(GetType(), "강화레벨이 존재하지 않습니다. nLevel = " + nLevel);
		}
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. lnAttrValueId = " + lnAttrValueId);
		}
	}
}
