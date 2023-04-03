using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class WarehouseSlot
{
	private Hero m_hero;

	private int m_nSlotIndex = -1;

	private IWarehouseObject m_object;

	public Hero hero => m_hero;

	public int index => m_nSlotIndex;

	public IWarehouseObject obj => m_object;

	public bool isEmpty => m_object == null;

	public WarehouseSlot(Hero hero, int nSlotIndex)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_nSlotIndex = nSlotIndex;
	}

	public void Clear()
	{
		if (m_object != null)
		{
			m_object.warehouseSlot = null;
			m_object = null;
		}
	}

	public void Place(IWarehouseObject warehouseObj)
	{
		if (warehouseObj == null)
		{
			throw new ArgumentException("warehouseObj");
		}
		if (!isEmpty)
		{
			throw new Exception("이 슬롯에는 이미 창고객체가 배치되어있습니다.");
		}
		m_object = warehouseObj;
		warehouseObj.warehouseSlot = this;
	}

	public PDWarehouseSlot ToPDWarehouseSlot()
	{
		PDWarehouseSlot inst = new PDWarehouseSlot();
		inst.index = m_nSlotIndex;
		if (m_object != null)
		{
			inst.warehouseObject = m_object.ToPDWarehouseObject();
		}
		return inst;
	}

	public static List<PDWarehouseSlot> ToPDWarehouseSlots(IEnumerable<WarehouseSlot> slots)
	{
		List<PDWarehouseSlot> insts = new List<PDWarehouseSlot>();
		foreach (WarehouseSlot slot in slots)
		{
			insts.Add(slot.ToPDWarehouseSlot());
		}
		return insts;
	}
}
