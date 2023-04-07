using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class MainGearBoxUseCommandHandler : InGameCommandHandler<MainGearBoxUseCommandBody, MainGearBoxUseResponseBody>
{
    public const short kResult_NotEnoughItem = 101;

    private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

    private ResultItemCollection m_addedItems = new ResultItemCollection();

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nSlotIndex = m_body.slotIndex;
        int nUseCount = m_body.useCount;
        if (nSlotIndex < 0)
        {
            throw new CommandHandleException(1, "슬롯 인덱스가 유효하지 않습니다. nSlotIndex = " + nSlotIndex);
        }
        if (nUseCount <= 0)
        {
            throw new CommandHandleException(1, "사용 수량이 유효하지 않습니다. nUseCount = " + nUseCount);
        }
        InventorySlot targetInventorySlot = m_myHero.GetInventorySlot(nSlotIndex);
        if (targetInventorySlot == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
        }
        if (targetInventorySlot.isEmpty)
        {
            throw new CommandHandleException(1, "빈 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
        }
        if (targetInventorySlot.obj.inventoryObjectType != 3)
        {
            throw new CommandHandleException(1, "해당 인벤토리 슬롯의 오브젝트는 아이템이 아닙니다. nSlotIndex = " + nSlotIndex);
        }
        ItemInventoryObject itemObj = (ItemInventoryObject)targetInventorySlot.obj;
        Item targetItem = itemObj.item;
        int nItemCount = itemObj.count;
        bool bItemOwned = itemObj.owned;
        if (targetItem.type.id != 3)
        {
            throw new CommandHandleException(1, "해당 아이템은 메인장비상자가 아닙니다.");
        }
        if (!targetItem.IsUseableHeroLevel(m_myHero.level))
        {
            throw new CommandHandleException(1, "해당 아이템을 사용할 수 있는 레벨이 아닙니다.");
        }
        if (nItemCount < nUseCount)
        {
            throw new CommandHandleException(101, "아이템이 부족합니다.");
        }
        MainGearTier targetMainGearTier = Resource.instance.GetAvailableMaxMainGearTier(m_myHero.level);
        Item weaponBoxItem = targetMainGearTier.weaponBoxItem;
        Item armorBoxItem = targetMainGearTier.armorBoxItem;
        bool bPickBoxItemOwned = true;
        int nEmptyInventorySlotCount = m_myHero.emptyInventorySlotCount;
        int nProgressCount = 0;
        for (nProgressCount = 0; nProgressCount < nUseCount; nProgressCount++)
        {
            if (nItemCount == 1)
            {
                nEmptyInventorySlotCount++;
            }
            if (nEmptyInventorySlotCount <= 0)
            {
                break;
            }
            Item targetPickBox = null;
            targetPickBox = !Util.DrawLots(targetItem.value1) ? armorBoxItem : weaponBoxItem;
            int nAvailabelSpace = m_myHero.GetInventoryAvailableSpaceWithoutEmptySlots(targetPickBox, bPickBoxItemOwned);
            if (nAvailabelSpace < 1)
            {
                nEmptyInventorySlotCount--;
            }
            m_myHero.UseItem(nSlotIndex, 1);
            m_changedInventorySlots.Add(targetInventorySlot);
            m_myHero.AddItem(targetPickBox, bPickBoxItemOwned, 1, m_changedInventorySlots);
            nItemCount--;
            m_addedItems.AddResultItemCount(targetPickBox, bPickBoxItemOwned, 1);
        }
        int nUsedOwnCount = 0;
        int nUsedUnOwnCount = 0;
        if (bItemOwned)
        {
            nUsedOwnCount = nProgressCount;
        }
        else
        {
            nUsedUnOwnCount = nProgressCount;
        }
        if (nUsedOwnCount > 0 || nUsedUnOwnCount > 0)
        {
            SaveToDB(targetItem.id, nUsedOwnCount, nUsedUnOwnCount);
        }
        MainGearBoxUseResponseBody resBody = new MainGearBoxUseResponseBody();
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
        SendResponseOK(resBody);
    }

    private void SaveToDB(int nItemId, int nItemOwnCount, int nItemUnOwnCount)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
        }
        dbWork.Schedule();
        SaveToDB_AddMainGearBoxItemUseLog(nItemId, nItemOwnCount, nItemUnOwnCount);
    }

    private void SaveToDB_AddMainGearBoxItemUseLog(int nItemId, int nItemOwnCount, int nItemUnOwnCount)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            Guid logId = Guid.NewGuid();
            logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(logId, m_myHero.id, nItemId, nItemOwnCount, nItemUnOwnCount, m_currentTime));
            foreach (ResultItem result in m_addedItems.resultItems)
            {
                logWork.AddSqlCommand(GameLogDac.CSC_AddMainGearItemUseDetailLog(Guid.NewGuid(), logId, result.item.id, result.count, result.owned));
            }
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
