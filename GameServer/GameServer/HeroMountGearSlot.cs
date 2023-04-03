using System;

namespace GameServer;

public class HeroMountGearSlot
{
	public const int kIndex_Horsewhip = 0;

	public const int kIndex_Horsearmor = 1;

	public const int kIndex_Bridle = 2;

	public const int kIndex_Saddle = 3;

	public const int kIndex_Stirrups = 4;

	public const int kIndex_Horseshoe = 5;

	public const int kCount = 6;

	private Hero m_hero;

	private int m_nIndex = -1;

	private HeroMountGear m_heroMountGear;

	public Hero hero => m_hero;

	public int index => m_nIndex;

	public HeroMountGear heroMountGear => m_heroMountGear;

	public bool isEmpty => m_heroMountGear == null;

	public HeroMountGearSlot(Hero hero, int nIndex)
	{
		m_hero = hero;
		m_nIndex = nIndex;
	}

	public void Equip(HeroMountGear heroMountGear)
	{
		if (heroMountGear == null)
		{
			throw new ArgumentNullException("heroMountGear");
		}
		if (heroMountGear.isEquipped)
		{
			throw new ArgumentException("해당 영웅탈것장비는 이미 장착되어 있습니다. heroMountGear.id = " + heroMountGear.id);
		}
		if (m_heroMountGear != null)
		{
			throw new Exception("이 슬롯에는 장착되어 있는 장비가 존재합니다. m_nIndex = " + m_nIndex);
		}
		m_heroMountGear = heroMountGear;
		heroMountGear.gearSlot = this;
	}

	public void Unequip()
	{
		if (m_heroMountGear != null)
		{
			m_heroMountGear.gearSlot = null;
			m_heroMountGear = null;
		}
	}
}
