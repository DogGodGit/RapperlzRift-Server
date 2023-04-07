using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class SubQuestCompleteCommandHandler : InGameCommandHandler<SubQuestCompleteCommandBody, SubQuestCompleteResponseBody>
{
    public const short kResult_NotAcceptedSubQuest = 101;

    public const short kResult_NotCompletedQuestObjective = 102;

    public const short kResult_UnableInteractionPositionWithTargetNPC = 103;

    public const short kResult_NotEnoughInventory = 104;

    public const short kResult_NoGuildMember = 105;

    private HeroSubQuest m_heroSubQuest;

    private ResultItemCollection m_resultItemCollection;

    private HashSet<InventorySlot> m_changedInventorysSlots = new HashSet<InventorySlot>();

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nQuestId = m_body.questId;
        if (nQuestId <= 0)
        {
            throw new CommandHandleException(1, "퀘스트ID가 유효하지 않습니다. nQuestId = " + nQuestId);
        }
        if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
        {
            throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
        }
        m_heroSubQuest = m_myHero.GetSubQuest(nQuestId);
        if (m_heroSubQuest == null)
        {
            throw new CommandHandleException(1, "영웅서브퀘스트가 존재하지 않습니다. nQuestId = " + nQuestId);
        }
        if (!m_heroSubQuest.isAccepted)
        {
            throw new CommandHandleException(101, "영웅서브퀘스트가 수락한 상태가 아닙니다.");
        }
        SubQuest subQuest = m_heroSubQuest.quest;
        if (subQuest.targetContentId == 27)
        {
            if (m_myHero.guildMember == null)
            {
                throw new CommandHandleException(105, "길드에 가입되지 않았습니다.");
            }
        }
        else if (!m_heroSubQuest.isObjectiveCompleted)
        {
            throw new CommandHandleException(102, "영웅서브퀘스트의 목표가 완료되지 않았습니다.");
        }
        Npc completionNpc = subQuest.completionNpc;
        if (completionNpc != null)
        {
            if (!currentPlace.IsSame(completionNpc.continent.id, m_myHero.nationId))
            {
                throw new CommandHandleException(1, "퀘스트NPC가 있는 장소가 아닙니다.");
            }
            if (!completionNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.artifactRoomBestFloor))
            {
                throw new CommandHandleException(103, "해당 NPC와 상호작용할 수 없는 거리입니다.");
            }
        }
        m_resultItemCollection = new ResultItemCollection();
        foreach (SubQuestReward reward in subQuest.rewards)
        {
            ItemReward itemReward = reward.itemReward;
            if (itemReward != null)
            {
                m_resultItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
            }
        }
        if (!m_myHero.IsAvailableInventory(m_resultItemCollection))
        {
            throw new CommandHandleException(104, "인벤토리가 부족합니다.");
        }
        foreach (ResultItem resultItem in m_resultItemCollection.resultItems)
        {
            m_myHero.AddItem(resultItem.item, resultItem.owned, resultItem.count, m_changedInventorysSlots);
        }
        long lnExpReward = subQuest.expRewardValue;
        lnExpReward = (long)Math.Floor(lnExpReward * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
        m_myHero.AddExp(lnExpReward, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
        long lnGoldReward = subQuest.goldRewardValue;
        m_myHero.AddGold(lnGoldReward);
        m_heroSubQuest.Complete();
        SaveToDB();
        SaveToLogDB(lnExpReward, lnGoldReward);
        SubQuestCompleteResponseBody resBody = new SubQuestCompleteResponseBody();
        resBody.gold = m_myHero.gold;
        resBody.maxGold = m_myHero.maxGold;
        resBody.acquiredExp = lnExpReward;
        resBody.level = m_myHero.level;
        resBody.exp = m_myHero.exp;
        resBody.maxHP = m_myHero.realMaxHP;
        resBody.hp = m_myHero.hp;
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorysSlots).ToArray();
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
        foreach (InventorySlot slot in m_changedInventorysSlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
        }
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSubQuest_Complete(m_heroSubQuest.hero.id, m_heroSubQuest.quest.id, m_currentTime));
        dbWork.Schedule();
    }

    private void SaveToLogDB(long lnRewardExp, long lnRewardGold)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            Guid logId = Guid.NewGuid();
            logWork.AddSqlCommand(GameLogDac.CSC_AddHeroSubQuestRewardLog(logId, m_myHero.id, m_heroSubQuest.quest.id, lnRewardExp, lnRewardGold, m_currentTime));
            foreach (ResultItem resultItem in m_resultItemCollection.resultItems)
            {
                logWork.AddSqlCommand(GameLogDac.CSC_AddHeroSubQuestRewardDetailLog(Guid.NewGuid(), logId, resultItem.item.id, resultItem.owned, resultItem.count));
            }
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
