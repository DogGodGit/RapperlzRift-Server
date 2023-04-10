using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class AncientRelicEnterCommandHandler : InGameCommandHandler<AncientRelicEnterCommandBody, AncientRelicEnterResponseBody>
{
    public const short kResult_EnterTimeout = 101;

    private DateTimeOffset m_dungeonCreationTime = DateTimeOffset.MinValue;

    private AncientRelicInstance m_ancientRelicInstance;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        if (m_myHero.currentPlace != null)
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        if (!(m_myHero.placeEntranceParam is AncientRelicEnterParam param))
        {
            throw new CommandHandleException(1, "현재 사용할 수 없는 명령입니다.");
        }
        m_dungeonCreationTime = param.dungeonCreationTime;
        DateTime dungeonCreationDate = m_dungeonCreationTime.Date;
        Guid instanceId = param.ancientRelicInstanceId;
        m_ancientRelicInstance = Cache.instance.GetPlace(instanceId) as AncientRelicInstance;
        if (m_ancientRelicInstance == null)
        {
            throw new CommandHandleException(1, "던전이 존재하지 않습니다.");
        }
        if (m_ancientRelicInstance.isFinished)
        {
            throw new CommandHandleException(101, "현재 던전에 입장할 수 없는 상태입니다.");
        }
        AncientRelic ancientRelic = Resource.instance.ancientRelic;
        m_myHero.UseStamina(ancientRelic.requiredStamina, m_dungeonCreationTime);
        m_myHero.ClearPaidImmediateRevivalDailyCount(dungeonCreationDate);
        m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
        DateTimeOffset currentTime = DateTimeUtil.currentTime;
        lock (m_ancientRelicInstance.syncObject)
        {
            m_myHero.SetPositionAndRotation(ancientRelic.SelectPosition(), ancientRelic.SelectRotationY());
            m_ancientRelicInstance.Enter(m_myHero, currentTime, bIsRevivalEnter: false);
            m_myHero.RefreshDailyAncientRelicPlayCount(dungeonCreationDate);
            DateValuePair<int> dailyAncientRelicPlayCount = m_myHero.dailyAncientRelicPlayCount;
            dailyAncientRelicPlayCount.value++;
            int nPaidImmediateRevivalDailyCount = m_myHero.paidImmediateRevivalDailyCount.value;
            SaveToDB(nPaidImmediateRevivalDailyCount);
            AncientRelicEnterResponseBody resBody = new AncientRelicEnterResponseBody();
            resBody.date = dungeonCreationDate;
            resBody.placeInstanceId = m_ancientRelicInstance.instanceId;
            resBody.position = m_myHero.position;
            resBody.rotationY = m_myHero.rotationY;
            resBody.remainingStartTime = m_ancientRelicInstance.GetRemainingStartTime(currentTime);
            resBody.remainingLimitTime = m_ancientRelicInstance.GetRemainingLimitTime(currentTime);
            resBody.stepNo = m_ancientRelicInstance.stepNo;
            resBody.waveNo = m_ancientRelicInstance.waveNo;
            resBody.heroes = m_ancientRelicInstance.GetPDHeroes(m_myHero.id, currentTime).ToArray();
            resBody.monsterInsts = m_ancientRelicInstance.GetPDMonsterInstances(currentTime).ToArray();
            resBody.trapEffectHeroes = (Guid[])(object)m_ancientRelicInstance.GetTrapEffectHeroes().ToArray();
            resBody.hp = m_myHero.hp;
            resBody.stamina = m_myHero.stamina;
            resBody.playCount = dailyAncientRelicPlayCount.value;
            resBody.paidImmediateRevivalDailyCount = nPaidImmediateRevivalDailyCount;
            SendResponseOK(resBody);
            m_myHero.ProcessTodayTask(11, dungeonCreationDate);
            m_myHero.ProcessRetrievalProgressCount(11, dungeonCreationDate);
            m_myHero.ProcessMainQuestForContent(11);
            m_myHero.ProcessSubQuestForContent(11);
        }
    }

    private void SaveToDB(int nPaidImmediateRevivalCount)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_PaidImmediateRevivalCount(m_myHero.id, m_dungeonCreationTime.Date, nPaidImmediateRevivalCount));
        dbWork.AddSqlCommand(GameDac.CSC_AddAncientRelicInstanceMember(m_ancientRelicInstance.instanceId, m_myHero.id, m_myHero.level, 0, m_dungeonCreationTime));
        dbWork.Schedule();
    }
}
