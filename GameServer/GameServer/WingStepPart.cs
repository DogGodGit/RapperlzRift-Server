using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class WingStepPart
{
	private WingStep m_step;

	private WingPart m_part;

	private AttrValue m_increaseAttrValue;

	public WingStep step
	{
		get
		{
			return m_step;
		}
		set
		{
			m_step = value;
		}
	}

	public WingPart part => m_part;

	public int partId => m_part.id;

	public AttrValue increaseAttrValue => m_increaseAttrValue;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nPartId = Convert.ToInt32(dr["partId"]);
		m_part = Resource.instance.GetWingPart(nPartId);
		if (m_part == null)
		{
			SFLogUtil.Warn(GetType(), "날개파트가 존재하지 않습니다. nPartId = " + nPartId);
		}
		long lnIncreaseAttrValueId = Convert.ToInt64(dr["increaseAttrValueId"]);
		m_increaseAttrValue = Resource.instance.GetAttrValue(lnIncreaseAttrValueId);
		if (m_increaseAttrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. nPartId = " + nPartId + ",lnIncreaseAttrValueId = " + lnIncreaseAttrValueId);
		}
	}
}
