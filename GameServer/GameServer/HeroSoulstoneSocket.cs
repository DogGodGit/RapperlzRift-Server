using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class HeroSoulstoneSocket
{
	private HeroSubGear m_subGear;

	private SubGearSoulstoneSocket m_socket;

	private Item m_item;

	public HeroSubGear subGear => m_subGear;

	public SubGearSoulstoneSocket socket => m_socket;

	public int index => m_socket.index;

	public Item item => m_item;

	public int itemId
	{
		get
		{
			if (m_item == null)
			{
				return 0;
			}
			return m_item.id;
		}
	}

	public bool isEmpty => m_item == null;

	public bool isOpened => m_subGear.grade >= m_socket.requiredGrade;

	public HeroSoulstoneSocket(HeroSubGear subGear, SubGearSoulstoneSocket socket)
	{
		m_subGear = subGear;
		m_socket = socket;
	}

	public void Mount(Item item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		m_item = item;
	}

	public void Unmount()
	{
		if (!isEmpty)
		{
			m_item = null;
		}
	}

	public PDHeroSubGearSoulstoneSocket ToPDHeroSubGearSoulstoneSocket()
	{
		PDHeroSubGearSoulstoneSocket inst = new PDHeroSubGearSoulstoneSocket();
		inst.index = m_socket.index;
		inst.itemId = itemId;
		return inst;
	}

	public FieldOfHonorHeroSubGearSoulstoneSocket ToFieldOfHonorHeroSubGearSoulstoneSocket(FieldOfHonorHeroEquippedSubGear subGear)
	{
		FieldOfHonorHeroSubGearSoulstoneSocket inst = new FieldOfHonorHeroSubGearSoulstoneSocket(subGear);
		inst.index = index;
		inst.item = m_item;
		return inst;
	}

	public static List<PDHeroSubGearSoulstoneSocket> ToPDHeroSubGearSoulstoneSockets(IEnumerable<HeroSoulstoneSocket> sockets)
	{
		List<PDHeroSubGearSoulstoneSocket> results = new List<PDHeroSubGearSoulstoneSocket>();
		foreach (HeroSoulstoneSocket socket in sockets)
		{
			if (!socket.isEmpty)
			{
				results.Add(socket.ToPDHeroSubGearSoulstoneSocket());
			}
		}
		return results;
	}
}
