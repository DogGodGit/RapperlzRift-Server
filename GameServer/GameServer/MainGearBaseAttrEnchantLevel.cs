using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainGearBaseAttrEnchantLevel
{
	private MainGearBaseAttr m_attr;

	private int m_nLevel;

	private AttrValue m_attrValue;

	public MainGearBaseAttr attr
	{
		get
		{
			return m_attr;
		}
		set
		{
			m_attr = value;
		}
	}

	public int level => m_nLevel;

	public AttrValue attrValue => m_attrValue;

	public int value
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

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nLevel = Convert.ToInt32(dr["enchantLevel"]);
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. lnAttrValueId = " + lnAttrValueId);
		}
	}
}
