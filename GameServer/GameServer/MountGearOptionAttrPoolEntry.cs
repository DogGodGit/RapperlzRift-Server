using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountGearOptionAttrPoolEntry : IPickEntry
{
	private MountGear m_gear;

	private int m_nNo;

	private int m_nPoint;

	private int m_nAttrGrade;

	private int m_nAttrId;

	private AttrValue m_attrValue;

	public MountGear gear
	{
		get
		{
			return m_gear;
		}
		set
		{
			m_gear = value;
		}
	}

	public int no => m_nNo;

	public int point => m_nPoint;

	public int attrGrade => m_nAttrGrade;

	public int attrId => m_nAttrId;

	public AttrValue attrValue => m_attrValue;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nNo = Convert.ToInt32(dr["entryNo"]);
		if (m_nNo <= 0)
		{
			SFLogUtil.Warn(GetType(), "항목번호가 유효하지 않습니다. m_nNo = " + m_nNo);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nPoint = " + m_nPoint);
		}
		m_nAttrGrade = Convert.ToInt32(dr["attrGrade"]);
		if (m_nAttrGrade <= 0)
		{
			SFLogUtil.Warn(GetType(), "속성등급이 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nAttrGrade = " + m_nAttrGrade);
		}
		m_nAttrId = Convert.ToInt32(dr["attrId"]);
		if (m_nAttrId <= 0)
		{
			SFLogUtil.Warn(GetType(), "속성ID가 유효하지 않습니다. m_nNo = " + m_nNo + ", m_nAttrId = " + m_nAttrId);
		}
		long lnAttrValueId = Convert.ToInt64(dr["attrValueId"]);
		m_attrValue = Resource.instance.GetAttrValue(lnAttrValueId);
		if (m_attrValue == null)
		{
			SFLogUtil.Warn(GetType(), "속성값이 유효하지 않습니다. m_nNo = " + m_nNo + ", lnAttrValueId = " + lnAttrValueId);
		}
	}
}
