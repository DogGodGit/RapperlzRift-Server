using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class CreatureSkillGrade : IPickEntry
{
	private int m_nGrade;

	private int m_nPoint;

	public int grade => m_nGrade;

	public int point => m_nPoint;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nGrade = Convert.ToInt32(dr["skillGrade"]);
		if (m_nGrade <= 0)
		{
			SFLogUtil.Warn(GetType(), "등급이 유효하지 않습니다. m_nGrade = " + m_nGrade);
		}
		m_nPoint = Convert.ToInt32(dr["point"]);
		if (m_nPoint <= 0)
		{
			SFLogUtil.Warn(GetType(), "가중치가 유효하지 않습니다. m_nGrade = " + m_nGrade + ", m_nPoint = " + m_nPoint);
		}
	}
}
