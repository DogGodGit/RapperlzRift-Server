using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CostumeCollectionAttr
{
	private CostumeCollection m_collection;

	private int m_nAttrId;

	private AttrValue m_attrValue;

	public CostumeCollection collection => m_collection;

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

	public CostumeCollectionAttr(CostumeCollection collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		m_collection = collection;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		if (m_nAttrId < 0)
		{
			SFLogUtil.Warn(GetType(), "속성ID가 유효하지 않습니다. m_nAttrId = " + m_nAttrId);
		}
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. lnAttrValueId = " + lnAttrValueId);
		}
	}
}
