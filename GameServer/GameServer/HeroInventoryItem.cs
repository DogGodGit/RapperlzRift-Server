using System;
using System.Collections.Generic;

namespace GameServer;

public class HeroInventoryItem
{
	private Hero m_hero;

	private Item m_item;

	private Dictionary<int, ItemInventoryObject> m_objects = new Dictionary<int, ItemInventoryObject>();

	public Item item => m_item;

	public int unOwnCount
	{
		get
		{
			int nValue = 0;
			foreach (ItemInventoryObject obj in m_objects.Values)
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
			foreach (ItemInventoryObject obj in m_objects.Values)
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
			foreach (ItemInventoryObject obj in m_objects.Values)
			{
				nValue += obj.count;
			}
			return nValue;
		}
	}

	public bool isEmpty => m_objects.Count == 0;

	public HeroInventoryItem(Hero hero, Item item)
	{
		m_hero = hero;
		m_item = item;
	}

	public void AddObject(ItemInventoryObject obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		m_objects.Add(obj.inventorySlot.index, obj);
	}

	public void RemoveObject(int nSlotIndex)
	{
		m_objects.Remove(nSlotIndex);
	}

	public int GetAvailableSpace(bool bOwned)
	{
		int nAvailableSpace = 0;
		foreach (ItemInventoryObject obj in m_objects.Values)
		{
			if (obj.owned == bOwned)
			{
				nAvailableSpace += obj.availableSpace;
			}
		}
		return nAvailableSpace;
	}

	public int AddItem(bool bOwned, int nCount, ICollection<InventorySlot> changedInventorySlots)
	{
		if (changedInventorySlots == null)
		{
			throw new ArgumentNullException("changedInventorySlots");
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
		foreach (ItemInventoryObject inventoryObject in m_objects.Values)
		{
			if (inventoryObject.owned == bOwned && !inventoryObject.isFull)
			{
				int nAddCount = Math.Min(inventoryObject.availableSpace, nRemainingCount);
				inventoryObject.count += nAddCount;
				nRemainingCount -= nAddCount;
				changedInventorySlots.Add(inventoryObject.inventorySlot);
				if (nRemainingCount <= 0)
				{
					return nRemainingCount;
				}
			}
		}
		return nRemainingCount;
	}

	public int UseOnly(bool bOwned, int nCount, ICollection<InventorySlot> changedInventorySlots)
	{
		if (nCount < 0)
		{
			throw new ArgumentOutOfRangeException("nCount");
		}
		if (changedInventorySlots == null)
		{
			throw new ArgumentNullException("changedInventorySlots");
		}
		if (nCount == 0)
		{
			return 0;
		}
		int nRemainingCount = nCount;
		List<ItemInventoryObject> emptyObjects = new List<ItemInventoryObject>();
		foreach (ItemInventoryObject obj in m_objects.Values)
		{
			if (obj.owned == bOwned)
			{
				int nValue = Math.Min(obj.count, nRemainingCount);
				obj.count -= nValue;
				nRemainingCount -= nValue;
				changedInventorySlots.Add(obj.inventorySlot);
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
		foreach (ItemInventoryObject emptyObj in emptyObjects)
		{
			InventorySlot slot = emptyObj.inventorySlot;
			slot.Clear();
			RemoveObject(slot.index);
		}
		return nCount - nRemainingCount;
	}

	public void Use(bool bFirstUseOwnItem, int nCount, ICollection<InventorySlot> changedInventorySlots, out int nUsedOwnCount, out int nUsedUnOwnCount)
	{
		if (nCount < 0)
		{
			throw new ArgumentOutOfRangeException("nCount");
		}
		if (changedInventorySlots == null)
		{
			throw new ArgumentNullException("changedInventorySlots");
		}
		nUsedOwnCount = 0;
		nUsedUnOwnCount = 0;
		if (nCount != 0)
		{
			int nFirsetUsedCount = UseOnly(bFirstUseOwnItem, nCount, changedInventorySlots);
			int nSecondUsedCount = 0;
			if (nFirsetUsedCount < nCount)
			{
				nSecondUsedCount = UseOnly(!bFirstUseOwnItem, nCount - nFirsetUsedCount, changedInventorySlots);
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
