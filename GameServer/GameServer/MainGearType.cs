using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainGearType
{
	public const int kType_Battleaxe = 1;

	public const int kType_Claw = 2;

	public const int kType_Wand = 3;

	public const int kType_Bow = 4;

	public const int kType_Armor = 5;

	public const int kCount = 5;

	private int m_nId;

	private MainGearCategory m_category;

	private string m_sNameKey;

	public int id => m_nId;

	public MainGearCategory category => m_category;

	public string nameKey => m_sNameKey;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["mainGearType"]);
		int nCategoryId = Convert.ToInt32(dr["categoryId"]);
		m_category = Resource.instance.GetMainGearCategory(nCategoryId);
		if (m_category == null)
		{
			SFLogUtil.Warn(GetType(), "카테고리가 존재하지 않습니다. m_nId = " + m_nId + ", nCategoryId = " + nCategoryId);
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
