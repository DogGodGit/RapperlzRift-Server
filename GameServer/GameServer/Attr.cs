using System;
using System.Data;

namespace GameServer;

public class Attr
{
	public const int kId_MaxHP = 1;

	public const int kId_PhysicalOffense = 2;

	public const int kId_MagicalOffense = 3;

	public const int kId_PhysicalDefense = 4;

	public const int kId_MagicalDefense = 5;

	public const int kId_Critical = 6;

	public const int kId_CriticalResistance = 7;

	public const int kId_CriticalDamageIncRate = 8;

	public const int kId_CriticalDamageDecRate = 9;

	public const int kId_Penetration = 10;

	public const int kId_Block = 11;

	public const int kId_FireOffense = 12;

	public const int kId_FireDefense = 13;

	public const int kId_LightningOffense = 14;

	public const int kId_LightningDefense = 15;

	public const int kId_DarkOffense = 16;

	public const int kId_DarkDefense = 17;

	public const int kId_HolyOffense = 18;

	public const int kId_HolyDefense = 19;

	public const int kId_DamageIncRate = 20;

	public const int kId_DamageDecRate = 21;

	public const int kId_StunResistance = 22;

	public const int kId_SnareResistance = 23;

	public const int kId_SilenceResistance = 24;

	public const int kId_BaseMaxHPIncRate = 25;

	public const int kId_BaseOffenseIncRate = 26;

	public const int kId_BasePhysicalDefenseIncRate = 27;

	public const int kId_BaseMagicalDefenseIncRate = 28;

	public const int kId_Offense = 29;

	public const float kHeroElementalFactor = 1.8f;

	private int m_nId;

	private string m_sNameKey;

	private int m_nBattlePowerFactor;

	public int id => m_nId;

	public string nameKey => m_sNameKey;

	public int battlePowerFactor => m_nBattlePowerFactor;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["attrId"]);
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_nBattlePowerFactor = Convert.ToInt32(dr["battlePowerFactor"]);
	}

	public static bool IsDefined(int nId)
	{
		if (nId != 1 && nId != 2 && nId != 3 && nId != 4 && nId != 5 && nId != 6 && nId != 7 && nId != 8 && nId != 9 && nId != 10 && nId != 11 && nId != 12 && nId != 13 && nId != 14 && nId != 15 && nId != 16 && nId != 17 && nId != 18 && nId != 19 && nId != 20 && nId != 21 && nId != 22 && nId != 23 && nId != 24 && nId != 25 && nId != 26 && nId != 27 && nId != 28)
		{
			return nId == 29;
		}
		return true;
	}
}
