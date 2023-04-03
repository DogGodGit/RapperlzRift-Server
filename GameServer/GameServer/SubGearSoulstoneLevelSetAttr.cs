using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SubGearSoulstoneLevelSetAttr
{
	private SubGearSoulstoneLevelSet m_soulstoneLevelSet;

	private int m_nAttrId;

	private AttrValue m_attrValue;

	public SubGearSoulstoneLevelSet soulstoneLevelSet
	{
		get
		{
			return m_soulstoneLevelSet;
		}
		set
		{
			m_soulstoneLevelSet = value;
		}
	}

	public int attrId => m_nAttrId;

	public AttrValue attrValue => m_attrValue;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		if (!Attr.IsDefined(m_nAttrId))
		{
			SFLogUtil.Warn(GetType(), "속성ID가 유효하지 않습니다. m_nAttrId = " + m_nAttrId);
		}
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성 값이 존재하지 않습니다. m_nAttrId = " + m_nAttrId + ", lnAttrValueId = " + lnAttrValueId);
		}
	}
}
