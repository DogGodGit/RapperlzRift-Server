using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class MainGearEnchantLevelSet
{
	private int m_nSetNo;

	private string m_sNameKey;

	private int m_nRequiredTotalEnchantLevel;

	private List<MainGearEnchantLevelSetAttr> m_attrs = new List<MainGearEnchantLevelSetAttr>();

	public int setNo => m_nSetNo;

	public string nameKey => m_sNameKey;

	public int requiredTotalEnchnatLevel => m_nRequiredTotalEnchantLevel;

	public List<MainGearEnchantLevelSetAttr> attrs => m_attrs;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nSetNo = Convert.ToInt32(dr["setNo"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_nRequiredTotalEnchantLevel = Convert.ToInt32(dr["requiredTotalEnchantLevel"]);
	}

	public void AddAttr(MainGearEnchantLevelSetAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attrs.Add(attr);
		attr.enchantLevelSet = this;
	}
}
