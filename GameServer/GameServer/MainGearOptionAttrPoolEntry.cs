using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainGearOptionAttrPoolEntry : IPickEntry
{
	private MainGearOptionAttrPool m_pool;

	private int m_nEntryNo;

	private int m_nPoint;

	private int m_nAttrGrade;

	private int m_nAttrId;

	private AttrValue m_attrValue;

	public MainGearOptionAttrPool pool
	{
		get
		{
			return m_pool;
		}
		set
		{
			m_pool = value;
		}
	}

	public int entryNo => m_nEntryNo;

	public int point => m_nPoint;

	public int attrGrade => m_nAttrGrade;

	public int attrId => m_nAttrId;

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
		m_nEntryNo = Convert.ToInt32(dr["entryNo"]);
		m_nPoint = Convert.ToInt32(dr["point"]);
		m_nAttrGrade = Convert.ToInt32(dr["attrGrade"]);
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 존재하지 않습니다. lnAttrValueId = " + lnAttrValueId);
		}
	}
}
