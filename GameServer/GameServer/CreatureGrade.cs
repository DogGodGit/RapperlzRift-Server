using System;
using System.Data;

namespace GameServer;

public class CreatureGrade
{
	public const int kId_Normal = 1;

	public const int kId_High = 2;

	public const int kId_Magic = 3;

	public const int kId_Rare = 4;

	public const int kId_Legend = 5;

	public const int kCount = 5;

	private int m_nGrade;

	public int grade => m_nGrade;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nGrade = Convert.ToInt32(dr["grade"]);
	}
}
