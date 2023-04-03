using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class SubGearName
{
	private SubGear m_gear;

	private SubGearGrade m_grade;

	private string m_sNameKey;

	public SubGear gear
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

	public SubGearGrade grade => m_grade;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nGrade = Convert.ToInt32(dr["grade"]);
		m_grade = Resource.instance.GetSubGearGrade(nGrade);
		if (m_grade == null)
		{
			SFLogUtil.Warn(GetType(), "등급이 존재하지 않습니다. nGrade = " + nGrade);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
	}
}
