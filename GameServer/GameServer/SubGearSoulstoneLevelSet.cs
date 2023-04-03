using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class SubGearSoulstoneLevelSet
{
	private int m_nSetNo;

	private string m_sNameKey;

	private int m_nRequiredTotalLevel;

	private List<SubGearSoulstoneLevelSetAttr> m_attrs = new List<SubGearSoulstoneLevelSetAttr>();

	public int setNo => m_nSetNo;

	public string nameKey => m_sNameKey;

	public int requiredTotalLevel => m_nRequiredTotalLevel;

	public List<SubGearSoulstoneLevelSetAttr> attrs => m_attrs;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nSetNo = Convert.ToInt32(dr["setNo"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_nRequiredTotalLevel = Convert.ToInt32(dr["requiredTotalLevel"]);
	}

	public void AddAttr(SubGearSoulstoneLevelSetAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_attrs.Add(attr);
		attr.soulstoneLevelSet = this;
	}
}
