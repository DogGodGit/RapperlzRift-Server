using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class Cart
{
	public const float kRidableRangeFactor = 1.1f;

	private int m_nId;

	private CartGrade m_grade;

	private float m_fRidableRange;

	private float m_fRadius;

	public int id => m_nId;

	public CartGrade grade => m_grade;

	public float ridableRange => m_fRidableRange;

	public float radius => m_fRadius;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["cartId"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "카트ID가 유효하지 않습니다. m_nId = " + m_nId);
		}
		int nGrade = Convert.ToInt32(dr["grade"]);
		m_grade = Resource.instance.GetCartGrade(nGrade);
		if (m_grade == null)
		{
			SFLogUtil.Warn(GetType(), "등급이 존재하지 않습니다. m_nId = " + m_nId + ", nGrade = " + nGrade);
		}
		m_fRidableRange = Convert.ToSingle(dr["ridableRange"]);
		if (m_fRidableRange <= 0f)
		{
			SFLogUtil.Warn(GetType(), "탑승가능거리가 유효하지 않습니다. m_nId = " + m_nId + ", m_fRidableRange = " + m_fRidableRange);
		}
		m_fRadius = Convert.ToSingle(dr["radius"]);
		if (m_fRadius <= 0f)
		{
			SFLogUtil.Warn(GetType(), "반경이 유효하지 않습니다. m_nId = " + m_nId + ", m_fRadius = " + m_fRadius);
		}
	}
}
