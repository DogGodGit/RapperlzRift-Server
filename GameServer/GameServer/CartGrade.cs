using System;
using System.Data;

namespace GameServer;

public class CartGrade
{
	public const int kId_Normal = 1;

	public const int kId_High = 2;

	public const int kId_Magic = 3;

	public const int kId_Rare = 4;

	public const int kId_Legend = 5;

	public const int kCount = 5;

	private int m_nId;

	public int id => m_nId;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["grade"]);
	}

	public static bool IsDefined(int nGrade)
	{
		if (nGrade != 1 && nGrade != 2 && nGrade != 3 && nGrade != 4)
		{
			return nGrade == 5;
		}
		return true;
	}
}
