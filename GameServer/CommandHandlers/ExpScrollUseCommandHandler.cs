using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class ExpScrollUseCommandHandler : InGameCommandHandler<ExpScrollUseCommandBody, ExpScrollUseResponseBody>
{
    public const short kResult_NotEnoughItem = 101;

    public const short kResult_DailyUseCountOverflowed = 102;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        DateTime currentDate = m_currentTime.Date;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nSlotIndex = m_body.slotIndex;
        int nUseCount = 1;
        if (nSlotIndex < 0)
        {
            throw new CommandHandleException(1, "슬롯인덱스가 유효하지 않습니다. nSlotIndex = " + nSlotIndex);
        }
        InventorySlot targetInventorySlot = m_myHero.GetInventorySlot(nSlotIndex);
        if (targetInventorySlot == null)
        {
            throw new CommandHandleException(1, "인벤토리슬롯이 존재하지 않습니다. nSlotIndex = " + nSlotIndex);
        }
        if (targetInventorySlot.isEmpty)
        {
            throw new CommandHandleException(1, "빈슬롯입니다. nSlotIndex = " + nSlotIndex);
        }
        if (targetInventorySlot.obj.inventoryObjectType != 3)
        {
            throw new CommandHandleException(1, "해당 인벤토리 오브젝트 타입은 아이템타입이 아닙니다. nSlotIndex = " + nSlotIndex);
        }
        ItemInventoryObject itemInventoryObj = (ItemInventoryObject)targetInventorySlot.obj;
        Item targetItem = itemInventoryObj.item;
        int nTargetItemCount = itemInventoryObj.count;
        if (targetItem.type.id != 11)
        {
            throw new CommandHandleException(1, "이 아이템은 경험치주문서가 아닙니다. nSlotIndex = " + nSlotIndex + ", itemType = " + targetItem.type.id);
        }
        if (nTargetItemCount < nUseCount)
        {
            throw new CommandHandleException(101, "아이템 수량이 부족합니다. nSlotIndex = " + nSlotIndex);
        }
        if (!targetItem.IsUseableHeroLevel(m_myHero.level))
        {
            throw new CommandHandleException(1, "사용할 수 있는 영웅레벨이 아닙니다. minLevel = " + targetItem.requiredMinHeroLevel + ", maxLevel = " + targetItem.requiredMaxHeroLevel);
        }
        m_myHero.RefreshExpScrollDailyUseCount(currentDate);
        int nExpScrollDailyUseCount = m_myHero.expScrollDailyUseCount.value + nUseCount;
        if (nExpScrollDailyUseCount > m_myHero.vipLevel.expScrollUseMaxCount)
        {
            throw new CommandHandleException(102, "일일사용횟수가 최대사용횟수를 넘어갑니다. expScrollUseMaxCount = " + m_myHero.vipLevel.expScrollUseMaxCount);
        }
        m_myHero.UseItem(targetInventorySlot.index, nUseCount);
        m_myHero.ApplyExpScroll(targetItem, m_currentTime);
        m_myHero.expScrollDailyUseCount.value = nExpScrollDailyUseCount;
        int nUsedOwnCount = 0;
        int nUsedUnOwnCount = 0;
        if (itemInventoryObj.owned)
        {
            nUsedOwnCount = nUseCount;
        }
        else
        {
            nUsedUnOwnCount = nUseCount;
        }
        SaveToDB(targetInventorySlot, nTargetItemCount, nUsedOwnCount, nUsedUnOwnCount);
        ExpScrollUseResponseBody resBody = new ExpScrollUseResponseBody();
        resBody.date = currentDate;
        resBody.expScrollDailyUseCount = nExpScrollDailyUseCount;
        resBody.expScrollRemainingTime = m_myHero.GetExpScrollRemainingTime(m_currentTime);
        resBody.changedInventorySlot = targetInventorySlot.ToPDInventorySlot();
        SendResponseOK(resBody);
    }

    private void SaveToDB(InventorySlot targetInventorySlot, int nItemId, int nItemOwnCount, int nItemUnOwnCount)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_UseExpScroll(m_myHero.id, m_myHero.expScrollDailyUseCount.date, m_myHero.expScrollDailyUseCount.value, m_myHero.expScrollStartTime, m_myHero.expScrollDuration, m_myHero.expScrollItemId));
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(targetInventorySlot));
        dbWork.Schedule();
        SaveToDB_AddItemUseLog(nItemId, nItemOwnCount, nItemUnOwnCount);
    }

    private void SaveToDB_AddItemUseLog(int nItemId, int nItemOwnCount, int nItemUnOwnCount)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, nItemId, nItemOwnCount, nItemUnOwnCount, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
