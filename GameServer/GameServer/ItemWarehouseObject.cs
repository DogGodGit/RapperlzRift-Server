using System;
using ClientCommon;

namespace GameServer;

public class ItemWarehouseObject : IWarehouseObject
{
	private WarehouseSlot m_warehouseSlot;

	private bool m_bOwned;

	private int m_nCount;

	private HeroWarehouseItem m_heroWarehouseItem;

	public HeroWarehouseItem heroWarehouseItem => m_heroWarehouseItem;

	public Item item => m_heroWarehouseItem.item;

	public int itemId => m_heroWarehouseItem.item.id;

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

	public int warehouseObjectType => 3;

	public WarehouseSlot warehouseSlot
	{
		get
		{
			return m_warehouseSlot;
		}
		set
		{
			m_warehouseSlot = value;
		}
	}

	public bool isFull => m_nCount >= item.type.maxCountPerInventorySlot;

	public bool isEmpty => m_nCount <= 0;

	public int availableSpace => Math.Max(0, item.type.maxCountPerInventorySlot - m_nCount);

	public ItemWarehouseObject(HeroWarehouseItem heroWarehouseItem, bool bOwned)
		: this(heroWarehouseItem, bOwned, 0)
	{
	}

	public ItemWarehouseObject(HeroWarehouseItem heroWarehouseItem, bool bOwned, int nCount)
	{
		if (heroWarehouseItem == null)
		{
			throw new ArgumentNullException("heroWarehouseItem");
		}
		m_heroWarehouseItem = heroWarehouseItem;
		m_bOwned = bOwned;
		m_nCount = nCount;
	}

	public PDWarehouseObject ToPDWarehouseObject()
	{
		PDItemWarehouseObject inst = new PDItemWarehouseObject();
		inst.itemId = m_heroWarehouseItem.item.id;
		inst.owned = m_bOwned;
		inst.count = m_nCount;
		return inst;
	}
}
