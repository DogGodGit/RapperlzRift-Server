using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SecretLetterGradePoolEntry : IPickEntry
{
	private SecretLetterGradePool m_pool;

	private int m_nGrade;

	private int m_nPoint;

	public SecretLetterGradePool pool => m_pool;

	public int grade => m_nGrade;

	public int point => m_nPoint;

	public SecretLetterGradePoolEntry(SecretLetterGradePool pool)
	{
		m_pool = pool;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nGrade = Convert.ToInt32(dr["grade"]);
		if (!SecretLetterGrade.IsDefined(m_nGrade))
		{
			SFLogUtil.Warn(GetType(), "등급이 유효하지 않습니다. m_pool.id = " + m_pool.id + ", m_nGrade = " + m_nGrade);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "포인트가 유효하지 않습니다. m_pool.id = " + m_pool.id + ", m_nGrade = " + m_nGrade + ", m_nPoint = " + m_nPoint);
		}
	}
}
