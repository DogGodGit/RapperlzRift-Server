using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SubGearAttrValue
{
	private SubGearLevelQuality m_quality;

	private int m_nAttrId;

	private AttrValue m_attrValue;

	public SubGearLevelQuality quality
	{
		get
		{
			return m_quality;
		}
		set
		{
			m_quality = value;
		}
	}

	public int id => m_nAttrId;

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
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. lnAttrValueId = " + lnAttrValueId);
		}
	}
}
