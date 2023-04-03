using System;
using ClientCommon;

namespace GameServer;

public class ItemInventoryObject : IInventoryObject
{
	private InventorySlot m_inventorySlot;

	private bool m_bOwned;

	private int m_nCount;

	private HeroInventoryItem m_heroInventoryItem;

	public HeroInventoryItem heroInventoryItem => m_heroInventoryItem;

	public Item item => m_heroInventoryItem.item;

	public int itemId => m_heroInventoryItem.item.id;

	public bool owned => m_bOwned;

	public int count
	{
		get
		{
			return m_nCount;
		}
		set
		{
			m_nCount = value;
		}
	}

	public int inventoryObjectType => 3;

	public InventorySlot inventorySlot
	{
		get
		{
			return m_inventorySlot;
		}
		set
		{
			m_inventorySlot = value;
		}
	}

	public bool saleable => m_heroInventoryItem.item.saleable;

	public bool isFull => m_nCount >= item.type.maxCountPerInventorySlot;

	public bool isEmpty => m_nCount <= 0;

	public int availableSpace => Math.Max(0, item.type.maxCountPerInventorySlot - m_nCount);

	public ItemInventoryObject(HeroInventoryItem heroInventoryItem, bool bOwned)
		: this(heroInventoryItem, bOwned, 0)
	{
	}

	public ItemInventoryObject(HeroInventoryItem heroInventoryItem, bool bOwned, int nCount)
	{
		m_heroInventoryItem = heroInventoryItem;
		m_bOwned = bOwned;
		m_nCount = nCount;
	}

	public PDInventoryObject ToPDInventoryObject()
	{
		PDItemInventoryObject inst = new PDItemInventoryObject();
		inst.itemId = m_heroInventoryItem.item.id;
		inst.owned = m_bOwned;
		inst.count = m_nCount;
		return inst;
	}
}
