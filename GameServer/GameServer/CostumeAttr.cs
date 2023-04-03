using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CostumeAttr
{
	private Costume m_costume;

	private int m_nAttrId;

	private List<CostumeEnchantLevelAttr> m_enchantLevelAttrs = new List<CostumeEnchantLevelAttr>();

	public Costume costume => m_costume;

	public int attrId => m_nAttrId;

	public List<CostumeEnchantLevelAttr> enchantLevelAttrs => m_enchantLevelAttrs;

	public CostumeAttr(Costume costume)
	{
		if (costume == null)
		{
			throw new ArgumentNullException("costume");
		}
		m_costume = costume;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		if (!Attr.IsDefined(m_nAttrId))
		{
			SFLogUtil.Warn(GetType(), "정의되지 않은 속성ID입니다. m_nAttrId = " + m_nAttrId);
		}
	}

	public void AddEnchantLevelAttr(CostumeEnchantLevelAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_enchantLevelAttrs.Add(attr);
	}

	public CostumeEnchantLevelAttr GetEnchantLevelAttr(int nLevel)
	{
		if (nLevel < 0 || nLevel >= m_enchantLevelAttrs.Count)
		{
			return null;
		}
		return m_enchantLevelAttrs[nLevel];
	}
}
