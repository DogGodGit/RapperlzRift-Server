using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class WarMemoryEnterCommandHandler : InGameCommandHandler<WarMemoryEnterCommandBody, WarMemoryEnterResponseBody>
{
    public const short kResult_EnterTimeout = 101;

    public const short kResult_NotEnoughItem = 102;

    private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

    private WarMemoryInstance m_warMemoryInst;

    private bool m_bIsFreeEnter;

    private int m_nEnterRequiredItemId;

    private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

    private DateValuePair<int> m_dailyWarMemoryFreePlayCount;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        if (m_myHero.currentPlace != null)
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        if (!(m_myHero.placeEntranceParam is WarMemoryEnterParam param))
        {
            throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
        }
        m_dungeonCreationTime = param.dungeonCreationTime;
        DateTime dungeonCreationDate = m_dungeonCreationTime.Date;
        Guid instanceId = param.warMemoryInstanceId;
        m_warMemoryInst = Cache.instance.GetPlace(instanceId) as WarMemoryInstance;
        if (m_warMemoryInst == null)
        {
            throw new CommandHandleException(1, "던전이 존재하지 않습니다.");
        }
        if (m_warMemoryInst.isFinished)
        {
            throw new CommandHandleException(101, "현재 던전에 입장할 수 없는 상태입니다.");
        }
        WarMemory warMemory = Resource.instance.warMemory;
        m_nEnterRequiredItemId = warMemory.enterRequiredItemId;
        if (m_myHero.GetWarMemoryAvailableFreeEnterCount(dungeonCreationDate) <= 0)
        {
            if (m_myHero.GetItemCount(m_nEnterRequiredItemId) <= 0)
            {
                throw new CommandHandleException(102, "입장에 필요한 아이템이 부족합니다.");
            }
        }
        else
        {
            m_bIsFreeEnter = true;
        }
        if (!m_bIsFreeEnter)
        {
            m_myHero.UseItem(m_nEnterRequiredItemId, bFirstUseOwn: true, 1, m_changedInventorySlots);
        }
        m_myHero.ClearPaidImmediateRevivalDailyCount(dungeonCreationDate);
        m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
        DateTimeOffset currentTime = DateTimeUtil.currentTime;
        lock (m_warMemoryInst.syncObject)
        {
            int nStartPositionIndex = (m_warMemoryInst.heroEnterNo - 1) % warMemory.startPositionCount;
            WarMemoryStartPosition startPosition = warMemory.GetStartPosition_ByIndex(nStartPositionIndex);
            m_myHero.SetPositionAndRotation(startPosition.SelectPosition(), startPosition.SelectRotationY());
            m_myHero.warMemoryStartPositionIndex = nStartPositionIndex;
            m_warMemoryInst.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
            m_myHero.RefreshDailyWarMemoryFreePlayCount(dungeonCreationDate);
            m_dailyWarMemoryFreePlayCount = m_myHero.dailyWarMemoryFreePlayCount;
            if (m_bIsFreeEnter)
            {
                m_dailyWarMemoryFreePlayCount.value++;
            }
            int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
            SaveToDB(nPaidImmediateRevivalDailyCount);
            SaveToDB_Log();
            WarMemoryEnterResponseBody resBody = new WarMemoryEnterResponseBody();
            resBody.date = dungeonCreationDate;
            resBody.placeInstanceId = m_warMemoryInst.instanceId;
            resBody.position = m_myHero.position;
            resBody.rotationY = m_myHero.rotationY;
            resBody.remainingStartTime = m_warMemoryInst.GetRemainingStartTime(currentTime);
            resBody.remainingLimitTime = m_warMemoryInst.GetRemainingLimitTime(currentTime);
            resBody.waveNo = m_warMemoryInst.waveNo;
            resBody.heroes = m_warMemoryInst.GetPDHeroes(m_myHero.id, currentTime).ToArray();
            resBody.monsterInsts = m_warMemoryInst.GetPDMonsterInstances(currentTime).ToArray();
            resBody.objectInsts = m_warMemoryInst.GetPDTransformationObjectInstances().ToArray();
            resBody.points = m_warMemoryInst.GetPDWarMemoryPoints().ToArray();
            resBody.hp = m_myHero.hp;
            resBody.freePlayCount = m_dailyWarMemoryFreePlayCount.value;
            resBody.changedInventorySlot = m_changedInventorySlots.Count > 0 ? m_changedInventorySlots[0].ToPDInventorySlot() : null;
            resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
            SendResponseOK(resBody);
            m_myHero.ProcessOrdealQuestMissions(OrdealQuestMissionType.WarMemoryDungeon, 1, currentTime);
            m_myHero.ProcessMainQuestForContent(21);
            m_myHero.ProcessSubQuestForContent(21);
        }
    }

    private void SaveToDB(int nPaidImmediateRevivalCount)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
        }
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, m_dungeonCreationTime.Date, nPaidImmediateRevivalCount));
        if (m_bIsFreeEnter)
        {
            dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_WarMemoryFreePlay(m_myHero.id, m_dailyWarMemoryFreePlayCount.date, m_dailyWarMemoryFreePlayCount.value));
        }
        dbWork.Schedule();
    }

    private void SaveToDB_Log()
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddWarMemoryMemberLog(m_warMemoryInst.instanceId, m_myHero.id, m_bIsFreeEnter ? 1 : 2, m_myHero.level));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex);
        }
    }
}
