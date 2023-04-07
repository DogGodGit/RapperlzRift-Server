using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class OpenGiftReceiveCommandHandler : InGameCommandHandler<OpenGiftReceiveCommandBody, OpenGiftReceiveResponseBody>
{
    public const short kResult_NotEnoughHeroLevel = 101;

    public const short kResult_NotElapsedDay = 102;

    public const short kResult_NotEnoughInventory = 103;

    private ResultItemCollection m_resultItemCollection;

    private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nDay = m_body.day;
        if (nDay <= 0)
        {
            throw new CommandHandleException(1, "오픈선물 일차가 유효하지 않습니다. nDay = " + nDay);
        }
        OpenGift openGift = Resource.instance.GetOpenGift(nDay);
        if (openGift == null)
        {
            throw new CommandHandleException(1, "오픈선물이 존재하지 않습니다. nDay = " + nDay);
        }
        if (m_myHero.IsReceivedOpenGift(nDay))
        {
            throw new CommandHandleException(1, "이미 받은 오픈선물입니다.");
        }
        if (m_myHero.level < Resource.instance.openGiftRequiredHeroLevel)
        {
            throw new CommandHandleException(101, "레벨이 부족합니다.");
        }
        if (m_myHero.GetElapsedDaysFromCreation(m_currentTime) < nDay)
        {
            throw new CommandHandleException(102, "받을 수 있는 출석일차가 아닙니다.");
        }
        m_resultItemCollection = new ResultItemCollection();
        foreach (OpenGiftReward reward in openGift.rewards)
        {
            ItemReward itemReward = reward.itemReward;
            m_resultItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
        }
        if (!m_myHero.IsAvailableInventory(m_resultItemCollection))
        {
            throw new CommandHandleException(103, "인벤토리가 부족합니다.");
        }
        foreach (ResultItem rewardItem in m_resultItemCollection.resultItems)
        {
            m_myHero.AddItem(rewardItem.item, rewardItem.owned, rewardItem.count, m_changedInventorySlots);
        }
        m_myHero.AddReceivedOpenGiftReward(nDay);
        SaveToDB(nDay);
        SaveToLogDB(nDay);
        OpenGiftReceiveResponseBody resBody = new OpenGiftReceiveResponseBody();
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
        SendResponseOK(resBody);
    }

    private void SaveToDB(int nDay)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
        }
        dbWork.AddSqlCommand(GameDac.CSC_AddHeroOpenGiftReward(m_myHero.id, nDay));
        dbWork.Schedule();
    }

    private void SaveToLogDB(int nDay)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            Guid logId = Guid.NewGuid();
            logWork.AddSqlCommand(GameLogDac.CSC_AddHeroOpenGiftRewardLog(logId, m_myHero.id, nDay, m_currentTime));
            foreach (ResultItem rewardItem in m_resultItemCollection.resultItems)
            {
                logWork.AddSqlCommand(GameLogDac.CSC_AddHeroOpenGiftRewardDetailLog(Guid.NewGuid(), logId, rewardItem.item.id, rewardItem.owned, rewardItem.count));
            }
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
