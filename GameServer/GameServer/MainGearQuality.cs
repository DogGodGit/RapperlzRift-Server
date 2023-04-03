using System;
using System.Data;

namespace GameServer;

public class MainGearQuality
{
	public const int kId_Lowest = 1;

	public const int kId_Low = 2;

	public const int kId_Middle = 3;

	public const int kId_High = 4;

	public const int kId_Highest = 5;

	public const int kId_Beyond = 6;

	public const int kCount = 6;

	private int m_nId;

	public int id => m_nId;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["quality"]);
	}

	public static bool IsDefined(int nQuality)
	{
		if (nQuality != 1 && nQuality != 2 && nQuality != 3 && nQuality != 4 && nQuality != 5)
		{
			return nQuality == 6;
		}
		return true;
	}
}
