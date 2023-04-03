using System;
using System.Data;
using ServerFramework;

namespace GameServer;

public class MainGearTier
{
	private int m_nId;

	private int m_nRequiredHeroLevel;

	private Item m_weaponBoxItem;

	private Item m_armorBoxItem;

	private MainGearOptionAttrPool[] m_optionAttrPools = new MainGearOptionAttrPool[5];

	private MainGearDisassembleResultCountPool[] m_disassembleResultCountPools = new MainGearDisassembleResultCountPool[5];

	private MainGearDisassembleResultPool[] m_disassembleResultPools = new MainGearDisassembleResultPool[5];

	public int id => m_nId;

	public int requiredHeroLevel => m_nRequiredHeroLevel;

	public Item weaponBoxItem => m_weaponBoxItem;

	public Item armorBoxItem => m_armorBoxItem;

	public MainGearTier()
	{
		for (int k = 0; k < m_optionAttrPools.Length; k++)
		{
			m_optionAttrPools[k] = new MainGearOptionAttrPool(this, k + 1);
		}
		for (int j = 0; j < m_disassembleResultCountPools.Length; j++)
		{
			m_disassembleResultCountPools[j] = new MainGearDisassembleResultCountPool(this, j + 1);
		}
		for (int i = 0; i < m_disassembleResultPools.Length; i++)
		{
			m_disassembleResultPools[i] = new MainGearDisassembleResultPool(this, i + 1);
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["tier"]);
		m_nRequiredHeroLevel = Convert.ToInt32(dr["requiredHeroLevel"]);
		int nWeaponBoxItemId = Convert.ToInt32(dr["weaponBoxItemId"]);
		m_weaponBoxItem = Resource.instance.GetItem(nWeaponBoxItemId);
		if (m_weaponBoxItem == null)
		{
			SFLogUtil.Warn(GetType(), "존재하지 않는 무기상자 아이템 입니다. tier = " + m_nId + ", nWeaponBoxItemId = " + nWeaponBoxItemId);
		}
		int nArmorBoxItemId = Convert.ToInt32(dr["armorBoxItemId"]);
		m_armorBoxItem = Resource.instance.GetItem(nArmorBoxItemId);
		if (m_armorBoxItem == null)
		{
			SFLogUtil.Warn(GetType(), "존재하지 않는 갑옷상자 아이템 입니다. tier = " + m_nId + ", nArmorBoxItemId = " + nArmorBoxItemId);
		}
	}

	public MainGearOptionAttrPool GetOptionAttrPool(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_optionAttrPools.Length)
		{
			return null;
		}
		return m_optionAttrPools[nIndex];
	}

	public MainGearDisassembleResultCountPool GetDisassembleResultCountPool(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_disassembleResultCountPools.Length)
		{
			return null;
		}
		return m_disassembleResultCountPools[nIndex];
	}

	public MainGearDisassembleResultPool GetDisassembleResultPool(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_disassembleResultPools.Length)
		{
			return null;
		}
		return m_disassembleResultPools[nIndex];
	}
}
