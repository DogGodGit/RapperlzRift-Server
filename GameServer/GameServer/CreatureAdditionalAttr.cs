using System;
using System.Collections.Generic;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureAdditionalAttr : IPickEntry
{
	private int m_nAttrId;

	private List<CreatureAdditionalAttrValue> m_attrvalue = new List<CreatureAdditionalAttrValue>();

	public int attrId => m_nAttrId;

	public int point => 10;

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

	public void AddAttrValue(CreatureAdditionalAttrValue value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		m_attrvalue.Add(value);
	}

	public CreatureAdditionalAttrValue GetAttrValue(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_attrvalue.Count)
		{
			return null;
		}
		return m_attrvalue[nIndex];
	}
}
