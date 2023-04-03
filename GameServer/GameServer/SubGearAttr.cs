using System;
using System.Data;

namespace GameServer;

public class SubGearAttr
{
	private SubGear m_subGear;

	private int m_nAttrId;

	public SubGear subGear
	{
		get
		{
			return m_subGear;
		}
		set
		{
			m_subGear = value;
		}
	}

	public int attrId => m_nAttrId;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
	}
}
