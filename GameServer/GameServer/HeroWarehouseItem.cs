using System;
using System.Collections.Generic;

namespace GameServer;

public class HeroWarehouseItem
{
	private Hero m_hero;

	private Item m_item;

	private Dictionary<int, ItemWarehouseObject> m_objects = new Dictionary<int, ItemWarehouseObject>();

	public Hero hero => m_hero;

	public Item item => m_item;

	public int unOwnCount
	{
		get
		{
			int nValue = 0;
			foreach (ItemWarehouseObject obj in m_objects.Values)
			{
				if (!obj.owned)
				{
					nValue += obj.count;
				}
			}
			return nValue;
		}
	}

	public int ownCount
	{
		get
		{
			int nValue = 0;
			foreach (ItemWarehouseObject obj in m_objects.Values)
			{
				if (obj.owned)
				{
					nValue += obj.count;
				}
			}
			return nValue;
		}
	}

	public int count
	{
		get
		{
			int nValue = 0;
			foreach (ItemWarehouseObject obj in m_objects.Values)
			{
				nValue += obj.count;
			}
			return nValue;
		}
	}

	public bool isEmpty => m_objects.Count == 0;

	public HeroWarehouseItem(Hero hero, Item item)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_item = item;
	}

	public void AddObject(ItemWarehouseObject obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		m_objects.Add(obj.warehouseSlot.index, obj);
	}

	public void RemoveObject(int nSlotIndex)
	{
		m_objects.Remove(nSlotIndex);
	}

	public int GetAvailableSpace(bool bOwned)
	{
		int nAvailableSpace = 0;
		foreach (ItemWarehouseObject obj in m_objects.Values)
		{
			if (obj.owned == bOwned)
			{
				nAvailableSpace += obj.availableSpace;
			}
		}
		return nAvailableSpace;
	}

	public int AddItem(bool bOwned, int nCount, ICollection<WarehouseSlot> changedWarehouseSlots)
	{
		if (changedWarehouseSlots == null)
		{
			throw new ArgumentNullException("changedWarehouseSlots");
		}
		if (nCount < 0)
		{
			throw new ArgumentOutOfRangeException("nCount");
		}
		if (nCount == 0)
		{
			return 0;
		}
		int nRemainingCount = nCount;
		foreach (ItemWarehouseObject warehouseObject in m_objects.Values)
		{
			if (warehouseObject.owned == bOwned && !warehouseObject.isFull)
			{
				int nAddCount = Math.Min(warehouseObject.availableSpace, nRemainingCount);
				warehouseObject.count += nAddCount;
				nRemainingCount -= nAddCount;
				changedWarehouseSlots.Add(warehouseObject.warehouseSlot);
				if (nRemainingCount <= 0)
				{
					return nRemainingCount;
				}
			}
		}
		return nRemainingCount;
	}

	public int UseOnly(bool bOwned, int nCount, ICollection<WarehouseSlot> changedWarehouseSlots)
	{
		if (nCount < 0)
		{
			throw new ArgumentOutOfRangeException("nCount");
		}
		if (changedWarehouseSlots == null)
		{
			throw new ArgumentNullException("changedWarehouseSlots");
		}
		if (nCount == 0)
		{
			return 0;
		}
		int nRemainingCount = nCount;
		List<ItemWarehouseObject> emptyObjects = new List<ItemWarehouseObject>();
		foreach (ItemWarehouseObject obj in m_objects.Values)
		{
			if (obj.owned == bOwned)
			{
				int nValue = Math.Min(obj.count, nRemainingCount);
				obj.count -= nValue;
				nRemainingCount -= nValue;
				changedWarehouseSlots.Add(obj.warehouseSlot);
				if (obj.isEmpty)
				{
					emptyObjects.Add(obj);
				}
				if (nRemainingCount <= 0)
				{
					break;
				}
			}
		}
		foreach (ItemWarehouseObject emptyObj in emptyObjects)
		{
			WarehouseSlot slot = emptyObj.warehouseSlot;
			slot.Clear();
			RemoveObject(slot.index);
		}
		return nCount - nRemainingCount;
	}

	public void Use(bool bFirstUseOwnItem, int nCount, ICollection<WarehouseSlot> changedWarehouseSlots, out int nUsedOwnCount, out int nUsedUnOwnCount)
	{
		if (nCount < 0)
		{
			throw new ArgumentOutOfRangeException("nCount");
		}
		if (changedWarehouseSlots == null)
		{
			throw new ArgumentNullException("changedWarehouseSlots");
		}
		nUsedOwnCount = 0;
		nUsedUnOwnCount = 0;
		if (nCount != 0)
		{
			int nFirsetUsedCount = UseOnly(bFirstUseOwnItem, nCount, changedWarehouseSlots);
			int nSecondUsedCount = 0;
			if (nFirsetUsedCount < nCount)
			{
				nSecondUsedCount = UseOnly(!bFirstUseOwnItem, nCount - nFirsetUsedCount, changedWarehouseSlots);
			}
			if (bFirstUseOwnItem)
			{
				nUsedOwnCount = nFirsetUsedCount;
				nUsedUnOwnCount = nSecondUsedCount;
			}
			else
			{
				nUsedOwnCount = nSecondUsedCount;
				nUsedUnOwnCount = nFirsetUsedCount;
			}
		}
	}
}
