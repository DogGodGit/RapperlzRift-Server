using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class RetrieveGoldAllCommandHandler : InGameCommandHandler<RetrieveGoldAllCommandBody, RetrieveGoldAllResponseBody>
{
    public const short kResult_NotEnoughGold = 101;

    private List<HeroRetrievalLog> m_logs = new List<HeroRetrievalLog>();

    private long m_lnTotalExpReward;

    private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

    private Mail m_mail;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        DateTime currentDate = m_currentTime.Date;
        DateTime yesterday = currentDate.AddDays(-1.0);
        m_myHero.RefreshRetrievals(currentDate);
        long lnTotalRequiredGold = 0L;
        HeroRetrievalProgressCountCollection prevRetrivalProgressCountCollection = m_myHero.GetOrCreateRetrivalProgressCountCollection(yesterday);
        foreach (Retrieval retrieval2 in Resource.instance.retrievals.Values)
        {
            int nRetrievalId2 = retrieval2.id;
            if (m_myHero.GetAvailableRetrieval(nRetrievalId2))
            {
                HeroRetrievalProgressCount prevProgressCount2 = prevRetrivalProgressCountCollection.GetProgressCount(nRetrievalId2);
                HeroRetrieval heroRetrieval2 = m_myHero.GetRetrieval(nRetrievalId2);
                int nTargetCount2 = m_myHero.GetRetrievalTargetCount(nRetrievalId2);
                int nPrevProgressCount2 = prevProgressCount2?.progressCount ?? 0;
                int nRetrievalCount2 = heroRetrieval2?.count ?? 0;
                int nRemainingRetrievalCount2 = nTargetCount2 - nPrevProgressCount2 - nRetrievalCount2;
                if (nRemainingRetrievalCount2 > 0)
                {
                    lnTotalRequiredGold += retrieval2.goldRetrievalReqruiedGold * nRemainingRetrievalCount2;
                }
            }
        }
        if (m_myHero.gold < lnTotalRequiredGold)
        {
            throw new CommandHandleException(101, "골드가 부족합니다.");
        }
        int nOldLevel = m_myHero.level;
        foreach (Retrieval retrieval in Resource.instance.retrievals.Values)
        {
            int nRetrievalId = retrieval.id;
            RetrievalReward retrievalReward = retrieval.GetReward(nOldLevel);
            HeroRetrievalProgressCount prevProgressCount = prevRetrivalProgressCountCollection.GetProgressCount(nRetrievalId);
            HeroRetrieval heroRetrieval = m_myHero.GetRetrieval(nRetrievalId);
            int nTargetCount = m_myHero.GetRetrievalTargetCount(nRetrievalId);
            int nPrevProgressCount = prevProgressCount?.progressCount ?? 0;
            int nRetrievalCount = heroRetrieval?.count ?? 0;
            int nRemainingRetrievalCount = nTargetCount - nPrevProgressCount - nRetrievalCount;
            if (nRemainingRetrievalCount <= 0)
            {
                continue;
            }
            if (heroRetrieval == null)
            {
                heroRetrieval = new HeroRetrieval(m_myHero, nRetrievalId);
                m_myHero.AddRetrieval(heroRetrieval);
            }
            long lnRequiredGold = retrieval.goldRetrievalReqruiedGold * nRemainingRetrievalCount;
            long lnRewardExp = 0L;
            ItemReward rewardItem = null;
            if (retrievalReward != null)
            {
                lnRewardExp = retrievalReward.goldExpRewardValue * nRemainingRetrievalCount;
                rewardItem = retrievalReward.goldItemReward;
            }
            m_lnTotalExpReward = (long)Math.Floor(m_lnTotalExpReward * Cache.instance.GetWorldLevelExpFactor(nOldLevel));
            m_myHero.UseGold(lnRequiredGold);
            if (lnRewardExp > 0)
            {
                m_myHero.AddExp(lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
            }
            if (rewardItem != null)
            {
                int nRemainingCount = m_myHero.AddItem(rewardItem.item, rewardItem.owned, rewardItem.count * nRemainingRetrievalCount, m_changedInventorySlots);
                if (nRemainingCount > 0)
                {
                    if (m_mail == null)
                    {
                        m_mail = Mail.Create("MAIL_REWARD_N_22", "MAIL_REWARD_D_22", m_currentTime);
                    }
                    m_mail.AddAttachmentWithNo(new MailAttachment(rewardItem.item, nRemainingCount, rewardItem.owned));
                }
            }
            heroRetrieval.count += nRemainingRetrievalCount;
            m_lnTotalExpReward += lnRewardExp;
            m_logs.Add(new HeroRetrievalLog(Guid.NewGuid(), m_myHero.id, retrieval.id, nRemainingRetrievalCount, nOldLevel, m_myHero.vipLevel.level, 1, lnRequiredGold, 0, 0, m_currentTime, Guid.NewGuid(), lnRewardExp, rewardItem?.item.id ?? 0, rewardItem?.owned ?? false, rewardItem?.count ?? 0));
        }
        if (m_mail != null)
        {
            m_myHero.AddMail(m_mail, bSendEvent: true);
        }
        if (m_logs.Count > 0)
        {
            SaveToDB();
            SaveToLogDB();
        }
        RetrieveGoldAllResponseBody resBody = new RetrieveGoldAllResponseBody();
        resBody.date = currentDate;
        resBody.retrievals = HeroRetrieval.ToPDHeroRetrievals(m_myHero.retrievals.Values).ToArray();
        resBody.gold = m_myHero.gold;
        resBody.acquiredExp = m_lnTotalExpReward;
        resBody.level = m_myHero.level;
        resBody.exp = m_myHero.exp;
        resBody.maxHP = m_myHero.realMaxHP;
        resBody.hp = m_myHero.hp;
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
        }
        foreach (HeroRetrieval retrieval in m_myHero.retrievals.Values)
        {
            dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroRetrieval(retrieval.hero.id, retrieval.hero.retrievalDate, retrieval.id, retrieval.count));
        }
        if (m_mail != null)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
        }
        dbWork.Schedule();
    }

    private void SaveToLogDB()
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            foreach (HeroRetrievalLog log in m_logs)
            {
                logWork.AddSqlCommand(log.ToSqlcommands());
            }
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
