using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class SubQuestAcceptCommandHandler : InGameCommandHandler<SubQuestAcceptCommandBody, SubQuestAcceptResponseBody>
{
    public const short kResult_UnableInteractionPositionWithTargetNPC = 101;

    public const short kResult_NotCompletedMainQuest = 102;

    public const short kResult_NotEnoughLevel = 103;

    public const short kResult_NotExistStartNpc = 104;

    public const short kResult_AlreadyAcceptedSubQuest = 105;

    public const short kResult_AlreadyCompletedSubQuest = 106;

    public const short kResult_NotAvailableReacceptance = 107;

    private HeroSubQuest m_heroSubQuest;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

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
        SubQuest subQuest = Resource.instance.GetSubQuest(nQuestId);
        if (subQuest == null)
        {
            throw new CommandHandleException(1, "서브퀘스트가 존재하지 않습니다. nQuestId = " + nQuestId);
        }
        Npc startNpc = subQuest.startNpc;
        if (startNpc == null)
        {
            throw new CommandHandleException(104, "시작NPC가 존재하지 않습니다.");
        }
        if (!currentPlace.IsSame(startNpc.continent.id, m_myHero.nationId))
        {
            throw new CommandHandleException(1, "퀘스트NPC가 있는 장소가 아닙니다.");
        }
        if (!startNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.artifactRoomBestFloor))
        {
            throw new CommandHandleException(101, "해당 NPC와 상호작용할 수 없는 거리입니다.");
        }
        switch (subQuest.requiredConditionType)
        {
            case SubQuestRequiredConditionType.MainQuestNo:
                if (!m_myHero.IsMainQuestCompleted(subQuest.requiredConditionValue))
                {
                    throw new CommandHandleException(102, "필요메인퀘스트를 완료하지 않았습니다.");
                }
                break;
            case SubQuestRequiredConditionType.HeroLevel:
                if (m_myHero.level < subQuest.requiredConditionValue)
                {
                    throw new CommandHandleException(103, "영웅레벨이 부족합니다.");
                }
                break;
        }
        HeroSubQuest heroSubQuest = m_myHero.GetSubQuest(nQuestId);
        if (heroSubQuest != null)
        {
            if (heroSubQuest.isAccepted)
            {
                throw new CommandHandleException(105, "이미 수락중인 서브퀘스트입니다.");
            }
            if (heroSubQuest.isCompleted)
            {
                throw new CommandHandleException(106, "이미 완료한 서브퀘스트입니다.");
            }
            if (!subQuest.reacceptanceEnabled)
            {
                throw new CommandHandleException(107, "재수락이 불가능한 서브퀘스트입니다.");
            }
            m_myHero.RemoveSubQuest(subQuest.id);
        }
        m_heroSubQuest = new HeroSubQuest(m_myHero, subQuest);
        m_myHero.AddSubQuest(m_heroSubQuest);
        SaveToDB();
        SaveToLogDB();
        SendResponseOK(null);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroSubQuest(m_heroSubQuest.hero.id, m_heroSubQuest.quest.id, m_heroSubQuest.progressCount, m_currentTime, (int)m_heroSubQuest.status, m_currentTime));
        dbWork.Schedule();
    }

    private void SaveToLogDB()
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddHeroSubQuestAcceptanceLog(Guid.NewGuid(), m_heroSubQuest.hero.id, m_heroSubQuest.quest.id, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
