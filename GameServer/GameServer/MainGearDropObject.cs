using System;
using ClientCommon;

namespace GameServer;

public class MainGearDropObject : DropObject
{
	private MainGear m_mainGear;

	private bool m_bOwned;

	private int m_nEnchantLevel;

	public override int type => 1;

	public MainGear mainGear => m_mainGear;

	public bool owned => m_bOwned;

	public int enchantLevel => m_nEnchantLevel;

	public MainGearDropObject(MainGear mainGear, bool bOwned, int nEnchantLevel)
	{
		if (mainGear == null)
		{
			throw new ArgumentNullException("mainGear");
		}
		m_mainGear = mainGear;
		m_bOwned = bOwned;
		m_nEnchantLevel = nEnchantLevel;
	}

	public override PDDropObject ToPDDropObject()
	{
		PDMainGearDropObject inst = new PDMainGearDropObject();
		inst.id = m_mainGear.id;
		inst.owned = m_bOwned;
		inst.enchantLevel = m_nEnchantLevel;
		return inst;
	}
}
