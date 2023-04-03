using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class ConstellationCycleBuff
{
	private ConstellationCycle m_cycle;

	private int m_nAttrId;

	private AttrValue m_attrValue;

	public ConstellationCycle cycle => m_cycle;

	public int attrId => m_nAttrId;

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

	public ConstellationCycleBuff(ConstellationCycle cycle)
	{
		if (cycle == null)
		{
			throw new ArgumentNullException("cycle");
		}
		m_cycle = cycle;
	}

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
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. m_nAttrId = " + m_nAttrId + ", lnAttrValueId = " + lnAttrValueId);
		}
	}
}
