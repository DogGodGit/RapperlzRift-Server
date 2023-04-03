using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountPotaionAttrValue
{
	private MountPotionAttrCount m_count;

	private int m_nAttrId;

	private AttrValue m_attrValue;

	public MountPotionAttrCount count => m_count;

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

	public MountPotaionAttrValue(MountPotionAttrCount count)
	{
		if (count == null)
		{
			throw new ArgumentNullException("count");
		}
		m_count = count;
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
		long lnAttrValue = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValue);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. m_nAttrId = " + m_nAttrId + ", lnAttrValue = " + lnAttrValue);
		}
	}
}
