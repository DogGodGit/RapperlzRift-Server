using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FieldOfHonorHeroSubGearSoulstoneSocket
{
	private FieldOfHonorHeroEquippedSubGear m_equippedSubGear;

	private int m_nIndex;

	private Item m_item;

	public FieldOfHonorHero hero => m_equippedSubGear.hero;

	public FieldOfHonorHeroEquippedSubGear equippedSubGear => m_equippedSubGear;

	public int index
	{
		get
		{
			return m_nIndex;
		}
		set
		{
			m_nIndex = value;
		}
	}

	public Item item
	{
		get
		{
			return m_item;
		}
		set
		{
			m_item = value;
		}
	}

	public FieldOfHonorHeroSubGearSoulstoneSocket(FieldOfHonorHeroEquippedSubGear subGear)
	{
		m_equippedSubGear = subGear;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nIndex = Convert.ToInt32(dr["socketIndex"]);
		int nItemId = Convert.ToInt32(dr["itemId"]);
		if (nItemId > 0)
		{
			m_item = Resource.instance.GetItem(nItemId);
			if (m_item == null)
			{
				SFLogUtil.Warn(GetType(), string.Concat("아이템이 존재하지 않습니다. heroId = ", hero.id, ", subGearId = ", m_equippedSubGear.subGear.id, ", nItemId = ", nItemId));
			}
		}
		else
		{
			SFLogUtil.Warn(GetType(), string.Concat("아이템ID가 유효하지 않습니다. heroId = ", hero.id, ", subGearId = ", m_equippedSubGear.subGear.id, ", nItemId = ", nItemId));
		}
	}

	public PDHeroSubGearSoulstoneSocket ToPDHeroSubGearSoulstoneSocket()
	{
		PDHeroSubGearSoulstoneSocket inst = new PDHeroSubGearSoulstoneSocket();
		inst.index = m_nIndex;
		inst.itemId = m_item.id;
		return inst;
	}
}
