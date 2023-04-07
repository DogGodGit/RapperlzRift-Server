using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class WarehouseDepositCommandHandler : InGameCommandHandler<WarehouseDepositCommandBody, WarehouseDepositResponseBody>
{
    public const short kResult_NotEnoughVipLevel = 101;

    private HashSet<WarehouseSlot> m_warehouseSlots = new HashSet<WarehouseSlot>();

    private HashSet<InventorySlot> m_inventorySlots = new HashSet<InventorySlot>();

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int[] inventorySlotIndices = m_body.inventorySlotIndices;
        if (inventorySlotIndices == null)
        {
            throw new CommandHandleException(1, "인벤토리슬롯인덱스목록이 존재하지 않습니다.");
        }
        if (inventorySlotIndices.Length == 0)
        {
            throw new CommandHandleException(1, "인벤토리슬롯인덱스의 값이 존재하지 않습니다.");
        }
        if (m_myHero.vipLevel.level < Resource.instance.warehouseRequiredVipLevel)
        {
            throw new CommandHandleException(101, "VIP레벨이 부족합니다.");
        }
        int[] array = inventorySlotIndices;
        foreach (int nInventorySlotIndex in array)
        {
            InventorySlot inventorySlot = m_myHero.GetInventorySlot(nInventorySlotIndex);
            if (inventorySlot == null || inventorySlot.isEmpty || m_inventorySlots.Contains(inventorySlot))
            {
                continue;
            }
            IInventoryObject inventoryObject = inventorySlot.obj;
            WarehouseSlot emptyWarehouseSlot = null;
            switch (inventoryObject.inventoryObjectType)
            {
                case 1:
                    {
                        emptyWarehouseSlot = m_myHero.GetEmptyWarehouseSlot();
                        if (emptyWarehouseSlot == null)
                        {
                            continue;
                        }
                        HeroMainGear heroMainGear = (HeroMainGear)inventoryObject;
                        inventorySlot.Clear();
                        emptyWarehouseSlot.Place(heroMainGear);
                        break;
                    }
                case 2:
                    {
                        emptyWarehouseSlot = m_myHero.GetEmptyWarehouseSlot();
                        if (emptyWarehouseSlot == null)
                        {
                            continue;
                        }
                        HeroSubGear heroSubGear = (HeroSubGear)inventoryObject;
                        inventorySlot.Clear();
                        emptyWarehouseSlot.Place(heroSubGear);
                        break;
                    }
                case 3:
                    {
                        ItemInventoryObject itemInventoryObject = (ItemInventoryObject)inventoryObject;
                        int nAvailableItemCount = m_myHero.GetWarehouseAvailableSpace(itemInventoryObject.item, itemInventoryObject.owned);
                        if (nAvailableItemCount < 1)
                        {
                            continue;
                        }
                        int nUseCount = Math.Min(itemInventoryObject.count, nAvailableItemCount);
                        m_myHero.UseItem(nInventorySlotIndex, nUseCount);
                        m_myHero.AddWarehosueItem(itemInventoryObject.item, itemInventoryObject.owned, nUseCount, m_warehouseSlots);
                        break;
                    }
                case 4:
                    {
                        emptyWarehouseSlot = m_myHero.GetEmptyWarehouseSlot();
                        if (emptyWarehouseSlot == null)
                        {
                            continue;
                        }
                        HeroMountGear heroMountGear = (HeroMountGear)inventoryObject;
                        inventorySlot.Clear();
                        emptyWarehouseSlot.Place(heroMountGear);
                        break;
                    }
            }
            if (emptyWarehouseSlot != null)
            {
                m_warehouseSlots.Add(emptyWarehouseSlot);
            }
            m_inventorySlots.Add(inventorySlot);
        }
        SaveToDB();
        WarehouseDepositResponseBody resBody = new WarehouseDepositResponseBody();
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_inventorySlots).ToArray();
        resBody.changedWarehouseSlots = WarehouseSlot.ToPDWarehouseSlots(m_warehouseSlots).ToArray();
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        foreach (InventorySlot slot2 in m_inventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot2));
        }
        foreach (WarehouseSlot slot in m_warehouseSlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateWarehouseSlot(slot));
        }
        dbWork.Schedule();
    }
}
