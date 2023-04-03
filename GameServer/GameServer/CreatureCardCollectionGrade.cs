using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureCardCollectionGrade
{
	public const int kGrade_White = 1;

	public const int kGrade_Green = 2;

	public const int kGrade_Blue = 3;

	public const int kGrade_Purple = 4;

	public const int kGrade_Orange = 5;

	public const int kCount = 5;

	private int m_nId;

	private int m_nCollectionFamePoint;

	public int id => m_nId;

	public int collectionFamePoint => m_nCollectionFamePoint;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["grade"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "등급이 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_nCollectionFamePoint = Convert.ToInt32(dr["collectionFamePoint"]);
		if (m_nCollectionFamePoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "컬렉션명성점수가 유효하지 않습니다. m_nId = " + m_nId + ", m_nCollectionFamePoint = " + m_nCollectionFamePoint);
		}
	}
}
