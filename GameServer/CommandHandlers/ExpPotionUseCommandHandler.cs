using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class ExpPotionUseCommandHandler : InGameCommandHandler<ExpPotionUseCommandBody, ExpPotionUseResponseBody>
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
        int nUseCount = m_body.useCount;
        if (nSlotIndex < 0)
        {
            throw new CommandHandleException(1, "슬롯 인덱스가 유효하지 않습니다. nSlotIndex = " + nSlotIndex);
        }
        if (nUseCount <= 0)
        {
            throw new CommandHandleException(1, "사용 갯수가 유효하지 않습니다. nUseCount = " + nUseCount);
        }
        InventorySlot targetInventorySlot = m_myHero.GetInventorySlot(nSlotIndex);
        if (targetInventorySlot == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
        }
        if (targetInventorySlot.isEmpty)
        {
            throw new CommandHandleException(1, "빈 슬롯입니다. nSlotIndex = " + nSlotIndex);
        }
        if (targetInventorySlot.obj.inventoryObjectType != 3)
        {
            throw new CommandHandleException(1, "인벤토리 오브젝트 타입이 아이템이 아닙니다.");
        }
        ItemInventoryObject itemInventoryObj = (ItemInventoryObject)targetInventorySlot.obj;
        Item item = itemInventoryObj.item;
        int nItemCount = itemInventoryObj.count;
        bool bItemOwned = itemInventoryObj.owned;
        if (item.type.id != 6)
        {
            throw new CommandHandleException(1, "아이템 타입이 경험치 포션이 아닙니다.");
        }
        if (!item.IsUseableHeroLevel(m_myHero.level))
        {
            throw new CommandHandleException(1, "사용할 수 있는 영웅레벨이 아닙니다.");
        }
        if (nItemCount < nUseCount)
        {
            throw new CommandHandleException(101, "사용갯수가 아이템의 갯수를 넘어갑니다.");
        }
        m_myHero.RefreshExpPotionDailyUseCount(currentDate);
        int nExpPotionDailyUseCount = m_myHero.expPotionDailyUseCount.value;
        int nExpPotionUseMaxCount = m_myHero.vipLevel.expPotionUseMaxCount;
        int nRemainingDailyCount = nExpPotionUseMaxCount - nExpPotionDailyUseCount;
        if (nRemainingDailyCount <= 0)
        {
            throw new CommandHandleException(102, "일일사용횟수가 최대사용횟수를 넘어갑니다.");
        }
        nUseCount = nUseCount < nRemainingDailyCount ? nUseCount : nRemainingDailyCount;
        m_myHero.UseItem(nSlotIndex, nUseCount);
        long lnTotalExp = item.value1 * nUseCount;
        lnTotalExp = (long)Math.Floor(lnTotalExp * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
        m_myHero.AddExp(lnTotalExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
        m_myHero.expPotionDailyUseCount.value += nUseCount;
        int nUsedOwnCount = 0;
        int nUsedUnOwnCount = 0;
        if (bItemOwned)
        {
            nUsedOwnCount = nUseCount;
        }
        else
        {
            nUsedUnOwnCount = nUseCount;
        }
        SaveToDB(targetInventorySlot, item.id, nUsedOwnCount, nUsedUnOwnCount);
        ExpPotionUseResponseBody resBody = new ExpPotionUseResponseBody();
        resBody.date = currentDate;
        resBody.expPotinDailyUseCount = m_myHero.expPotionDailyUseCount.value;
        resBody.changedInventorySlot = targetInventorySlot.ToPDInventorySlot();
        resBody.acquiredExp = lnTotalExp;
        resBody.level = m_myHero.level;
        resBody.exp = m_myHero.exp;
        resBody.maxHp = m_myHero.realMaxHP;
        resBody.hp = m_myHero.hp;
        SendResponseOK(resBody);
    }

    private void SaveToDB(InventorySlot slot, int nItemId, int nItemOwnCount, int nItemUnOwnCount)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ExpPotionDateCount(m_myHero.id, m_myHero.expPotionDailyUseCount.date, m_myHero.expPotionDailyUseCount.value));
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
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
