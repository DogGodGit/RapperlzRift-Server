using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class InventorySlot
{
	private Hero m_hero;

	private int m_nIndex = -1;

	private IInventoryObject m_object;

	public Hero hero => m_hero;

	public int index => m_nIndex;

	public IInventoryObject obj => m_object;

	public bool isEmpty => m_object == null;

	public InventorySlot(Hero hero, int nIndex)
	{
		m_hero = hero;
		m_nIndex = nIndex;
	}

	public void Clear()
	{
		if (m_object != null)
		{
			m_object.inventorySlot = null;
			m_object = null;
		}
	}

	public void Place(IInventoryObject inventoryObj)
	{
		if (inventoryObj == null)
		{
			throw new ArgumentException("inventoryObj");
		}
		if (!isEmpty)
		{
			throw new Exception("이 슬롯에는 이미 인벤토리객체가 배치되어있습니다.");
		}
		m_object = inventoryObj;
		inventoryObj.inventorySlot = this;
	}

	public PDInventorySlot ToPDInventorySlot()
	{
		PDInventorySlot inst = new PDInventorySlot();
		inst.index = m_nIndex;
		if (m_object != null)
		{
			inst.inventoryObject = m_object.ToPDInventoryObject();
		}
		return inst;
	}

	public static List<PDInventorySlot> ToPDInventorySlots(IEnumerable<InventorySlot> slots)
	{
		List<PDInventorySlot> insts = new List<PDInventorySlot>();
		foreach (InventorySlot slot in slots)
		{
			insts.Add(slot.ToPDInventorySlot());
		}
		return insts;
	}
}
