using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class DailyQuestAcceptCommandHandler : InGameCommandHandler<DailyQuestAcceptCommandBody, DailyQuestAcceptResponseBody>
{
    public const short kResult_kResult_DailyAcceptionCountOverflowed = 101;

    private HeroDailyQuest m_heroDailyQuest;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        DateTime currentDate = m_currentTime.Date;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        Guid questId = m_body.questId;
        if (questId == Guid.Empty)
        {
            throw new CommandHandleException(1, "유효하지 않은 퀘스트ID입니다. questId = " + questId);
        }
        m_heroDailyQuest = m_myHero.GetDailyQuest(questId);
        if (m_heroDailyQuest == null)
        {
            throw new CommandHandleException(1, "존재하지않는 일일퀘스트미션입니다. questId = " + questId);
        }
        if (!m_heroDailyQuest.isCreated)
        {
            throw new CommandHandleException(1, "현재 영웅일일퀘스트는 생성상태가 아닙니다.");
        }
        m_myHero.RefreshDailyQuestAcceptionCount(currentDate);
        DateValuePair<int> dailyQuestAcceptionCount = m_myHero.dailyQuestAcceptionCount;
        if (dailyQuestAcceptionCount.value >= Resource.instance.dailyQuest.playCount)
        {
            throw new CommandHandleException(101, "일일퀘스트수락횟수가 제한횟수를 넘어갑니다.");
        }
        m_heroDailyQuest.Accept(m_currentTime);
        dailyQuestAcceptionCount.value++;
        SaveToDB();
        DailyQuestAcceptResponseBody resBody = new DailyQuestAcceptResponseBody();
        resBody.date = dailyQuestAcceptionCount.date;
        resBody.dailyQuestAcceptionCount = dailyQuestAcceptionCount.value;
        SendResponseOK(resBody);
        m_myHero.ProcessTodayTask(26, currentDate);
        m_myHero.ProcessRetrievalProgressCount(13, currentDate);
        m_myHero.ProcessMainQuestForContent(24);
        m_myHero.ProcessSubQuestForContent(24);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroDailyQuest_Accept(m_heroDailyQuest.id, m_currentTime));
        dbWork.Schedule();
    }
}
