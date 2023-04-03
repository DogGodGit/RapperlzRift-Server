using System;
using System.Data;

namespace GameServer;

public class SubGearGrade
{
	public const int kId_Normal = 1;

	public const int kId_High = 2;

	public const int kId_Magic = 3;

	public const int kId_Rare = 4;

	public const int kId_Legend = 5;

	public const int kId_Mythic = 6;

	public const int kId_Beyond = 7;

	public const int kCount = 7;

	private int m_nId;

	private string m_sNameKey;

	private string m_sColoerCode;

	public int id => m_nId;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["grade"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_sColoerCode = Convert.ToString(dr["colorCode"]);
	}

	public static bool IsDefined(int nGrade)
	{
		if (nGrade != 1 && nGrade != 2 && nGrade != 3 && nGrade != 4 && nGrade != 5 && nGrade != 6)
		{
			return nGrade == 7;
		}
		return true;
	}
}
