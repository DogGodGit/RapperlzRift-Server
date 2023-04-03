using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class HeroRuneSocket
{
	private HeroSubGear m_subGear;

	private SubGearRuneSocket m_socket;

	private Item m_item;

	public HeroSubGear subGear => m_subGear;

	public SubGearRuneSocket socket => m_socket;

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

	public bool isOpened => m_subGear.level >= m_socket.requiredGearLevel;

	public HeroRuneSocket(HeroSubGear subGear, SubGearRuneSocket socket)
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

	public PDHeroSubGearRuneSocket ToPDHeroSubGearRuneSocket()
	{
		PDHeroSubGearRuneSocket inst = new PDHeroSubGearRuneSocket();
		inst.index = m_socket.index;
		inst.itemId = itemId;
		return inst;
	}

	public FieldOfHonorHeroSubGearRuneSocket ToFieldOfHonorHeroSubGearRuneSocket(FieldOfHonorHeroEquippedSubGear subGear)
	{
		FieldOfHonorHeroSubGearRuneSocket inst = new FieldOfHonorHeroSubGearRuneSocket(subGear);
		inst.index = index;
		inst.item = m_item;
		return inst;
	}

	public static List<PDHeroSubGearRuneSocket> ToPDHeroSubGearRuneSockets(IEnumerable<HeroRuneSocket> sockets)
	{
		List<PDHeroSubGearRuneSocket> results = new List<PDHeroSubGearRuneSocket>();
		foreach (HeroRuneSocket socket in sockets)
		{
			if (!socket.isEmpty)
			{
				results.Add(socket.ToPDHeroSubGearRuneSocket());
			}
		}
		return results;
	}
}
