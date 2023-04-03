using System;
using System.Collections.Generic;

namespace GameServer;

public class MountPotionAttrCount
{
	private int m_nCount;

	private List<MountPotaionAttrValue> m_attrValues = new List<MountPotaionAttrValue>();

	public int count => m_nCount;

	public List<MountPotaionAttrValue> attrValues => m_attrValues;

	public MountPotionAttrCount(int nCount)
	{
		m_nCount = nCount;
	}

	public void AddAttrValue(MountPotaionAttrValue attrValue)
	{
		if (attrValue == null)
		{
			throw new ArgumentNullException("attrValue");
		}
		m_attrValues.Add(attrValue);
	}
}
