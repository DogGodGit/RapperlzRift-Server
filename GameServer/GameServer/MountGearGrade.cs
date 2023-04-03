using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MountGearGrade
{
	public const int kId_Normal = 1;

	public const int kId_High = 2;

	public const int kId_Magic = 3;

	public const int kId_Rare = 4;

	public const int kId_Legend = 5;

	public const int kCount = 5;

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
		m_nId = Convert.ToInt32(dr["grade"]);
		if (m_nId <= 0)
		{
			SFLogUtil.Warn(GetType(), "등급이 유효하지 않습니다. m_nId = " + m_nId);
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
	}

	public static bool IsDefined(int nValue)
	{
		if (nValue != 1 && nValue != 2 && nValue != 3 && nValue != 4)
		{
			return nValue == 5;
		}
		return true;
	}
}
