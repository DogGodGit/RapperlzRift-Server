using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class WarehouseWithdrawCommandHandler : InGameCommandHandler<WarehouseWithdrawCommandBody, WarehouseWithdrawResponseBody>
{
	public const short kResult_NotEnoughInventory = 101;

	private HashSet<InventorySlot> m_inventorySlots = new HashSet<InventorySlot>();

	private WarehouseSlot m_warehouseSlot;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nWarehouseSlotIndex = m_body.warehouseSlotIndex;
		if (nWarehouseSlotIndex < 0)
		{
			throw new CommandHandleException(1, "창고슬롯인덱스가 유효하지 않습니다. nWarehouseSlotIndex = " + nWarehouseSlotIndex);
		}
		m_warehouseSlot = m_myHero.GetWarehouseSlot(nWarehouseSlotIndex);
		if (m_warehouseSlot == null)
		{
			throw new CommandHandleException(1, "창고슬롯이 존재하지 않습니다. nWarehouseSlotIndex = " + nWarehouseSlotIndex);
		}
		if (m_warehouseSlot.isEmpty)
		{
			throw new CommandHandleException(1, "빈 창고 슬롯입니다. nWarehouseSlotIndex = " + nWarehouseSlotIndex);
		}
		IWarehouseObject warehouseObject = m_warehouseSlot.obj;
		switch (warehouseObject.warehouseObjectType)
		{
		case 1:
		case 2:
		case 4:
			if (m_myHero.emptyInventorySlotCount < 1)
			{
				throw new CommandHandleException(101, "인벤토리가 부족합니다.");
			}
			break;
		case 3:
		{
			ItemWarehouseObject itemWarehouseObj = (ItemWarehouseObject)warehouseObject;
			if (m_myHero.GetInventoryAvailableSpace(itemWarehouseObj.item, itemWarehouseObj.owned) < 1)
			{
				throw new CommandHandleException(101, "인벤토리가 부족합니다.");
			}
			break;
		}
		}
		switch (warehouseObject.warehouseObjectType)
		{
		case 1:
		{
			HeroMainGear heroMainGear = (HeroMainGear)warehouseObject;
			InventorySlot emptyInventorySlot = m_myHero.GetEmptyInventorySlot();
			m_warehouseSlot.Clear();
			emptyInventorySlot.Place(heroMainGear);
			m_inventorySlots.Add(emptyInventorySlot);
			break;
		}
		case 2:
		{
			HeroSubGear heroSubGear = (HeroSubGear)warehouseObject;
			InventorySlot emptyInventorySlot2 = m_myHero.GetEmptyInventorySlot();
			m_warehouseSlot.Clear();
			emptyInventorySlot2.Place(heroSubGear);
			m_inventorySlots.Add(emptyInventorySlot2);
			break;
		}
		case 3:
		{
			ItemWarehouseObject itemWarehouseObject = (ItemWarehouseObject)warehouseObject;
			int nAvailableItemCount = m_myHero.GetInventoryAvailableSpace(itemWarehouseObject.item, itemWarehouseObject.owned);
			int nUseCount = Math.Min(itemWarehouseObject.count, nAvailableItemCount);
			m_myHero.UseWarehouseItem(nWarehouseSlotIndex, nUseCount);
			m_myHero.AddItem(itemWarehouseObject.item, itemWarehouseObject.owned, nUseCount, m_inventorySlots);
			break;
		}
		case 4:
		{
			HeroMountGear heroMountGear = (HeroMountGear)warehouseObject;
			InventorySlot emptyInventorySlot3 = m_myHero.GetEmptyInventorySlot();
			m_warehouseSlot.Clear();
			emptyInventorySlot3.Place(heroMountGear);
			m_inventorySlots.Add(emptyInventorySlot3);
			break;
		}
		}
		SaveToDB();
		WarehouseWithdrawResponseBody resBody = new WarehouseWithdrawResponseBody();
		resBody.cangedInvetorySlots = InventorySlot.ToPDInventorySlots(m_inventorySlots).ToArray();
		resBody.changedWarehouseSlot = m_warehouseSlot.ToPDWarehouseSlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (InventorySlot slot in m_inventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteWarehouseSlot(m_warehouseSlot));
		dbWork.Schedule();
	}
}
