using System;
using System.Data;

namespace GameServer;

public class MainGearCategory
{
	public const int kType_Weapon = 1;

	public const int kType_Armor = 2;

	public const int kCount = 2;

	private int m_nId;

	private string m_sNameKey;

	private int m_nSlotIndex;

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public int slotIndex => m_nSlotIndex;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["categoryId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_nSlotIndex = Convert.ToInt32(dr["slotIndex"]);
	}

	public static bool IsDefined(int nValue)
	{
		if (nValue != 1)
		{
			return nValue == 2;
		}
		return true;
	}
}
