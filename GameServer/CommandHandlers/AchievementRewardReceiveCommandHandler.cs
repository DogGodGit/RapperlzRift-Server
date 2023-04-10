using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class AchievementRewardReceiveCommandHandler : InGameCommandHandler<AchievementRewardReceiveCommandBody, AchievementRewardReceiveResponseBody>
{
    public const short kResult_DateNotEqualToCurrentTaskDate = 101;

    public const short kResult_NotEnoughAchievementPoint = 102;

    private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

    private Mail m_mail;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        DateTime currentDate = m_currentTime.Date;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        DateTime date = m_body.date;
        int nRewardNo = m_body.rewardNo;
        if (nRewardNo <= 0)
        {
            throw new CommandHandleException(1, "보상번호가 유효하지 않습니다. nRewardNo = " + nRewardNo);
        }
        m_myHero.RefreshTodayTaskCollection(currentDate);
        if (m_myHero.todayTaskCollection.date != date)
        {
            throw new CommandHandleException(101, "날짜가 오늘의할일 날짜와 다릅니다.");
        }
        int nNextRewardNo = m_myHero.receivedAchievementRewardNo + 1;
        AchievementReward achievementReward = Resource.instance.GetAchivementReward(nNextRewardNo);
        if (achievementReward == null)
        {
            throw new CommandHandleException(1, "달성보상이 존재하지 않습니다. nNextRewardNo = " + nNextRewardNo);
        }
        if (nRewardNo != nNextRewardNo)
        {
            throw new CommandHandleException(1, "받아야될 보상번호가 아닙니다. nRewardNo = " + nRewardNo);
        }
        m_myHero.RefreshAchievementDailyPoint(currentDate);
        if (m_myHero.achievementDailyPoint.value < achievementReward.requiredAchievementPoint)
        {
            throw new CommandHandleException(102, "달성점수가 부족합니다.");
        }
        ResultItemCollection resultItemCollection = new ResultItemCollection();
        foreach (AchievementRewardEntry entry in achievementReward.entries)
        {
            ItemReward itemReward = entry.itemReward;
            resultItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
        }
        foreach (ResultItem resultItem in resultItemCollection.resultItems)
        {
            int nRemainingCount = m_myHero.AddItem(resultItem.item, resultItem.owned, resultItem.count, m_changedInventorySlots);
            if (nRemainingCount > 0)
            {
                if (m_mail == null)
                {
                    m_mail = Mail.Create("MAIL_REWARD_N_11", "MAIL_REWARD_D_11", m_currentTime);
                }
                m_mail.AddAttachmentWithNo(new MailAttachment(resultItem.item, nRemainingCount, resultItem.owned));
            }
        }
        m_myHero.receivedAchievementRewardNo = nNextRewardNo;
        if (m_mail != null)
        {
            m_myHero.AddMail(m_mail, bSendEvent: true);
        }
        SaveToDB();
        SaveToDB_AddAchievementRewardLog(resultItemCollection);
        AchievementRewardReceiveResponseBody resBody = new AchievementRewardReceiveResponseBody();
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_AchievementReward(m_myHero.id, m_myHero.receivedAchievementRewardNo));
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
        }
        if (m_mail != null)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
        }
        dbWork.Schedule();
    }

    private void SaveToDB_AddAchievementRewardLog(ResultItemCollection resultItemCollection)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            Guid logId = Guid.NewGuid();
            logWork.AddSqlCommand(GameLogDac.CSC_AddAchievementRewardLog(logId, m_myHero.id, m_myHero.achievementDailyPoint.value, m_myHero.receivedAchievementRewardNo, m_currentTime));
            foreach (ResultItem result in resultItemCollection.resultItems)
            {
                logWork.AddSqlCommand(GameLogDac.CSC_AddAchievementRewardDetailLog(Guid.NewGuid(), logId, result.item.id, result.count, result.owned));
            }
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
