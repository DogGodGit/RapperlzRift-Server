using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountGearQuality
{
	public const int kId_Lowest = 1;

	public const int kId_Low = 2;

	public const int kId_Middle = 3;

	public const int kId_High = 4;

	public const int kId_Highest = 5;

	public const int kId_Beyond = 6;

	public const int kCount = 6;

	private int m_nId;

	private string m_sNameKey;

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["quality"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "품질이 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
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
