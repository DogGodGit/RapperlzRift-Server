using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class DailyQuestAbandonCommandHandler : InGameCommandHandler<DailyQuestAbandonCommandBody, DailyQuestAbandonResponseBody>
{
	public const short kResult_AlreadyMissionCompleted = 101;

	private HeroDailyQuest m_heroDailyQuest;

	private HeroDailyQuest m_newHeroDailyQuest;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid questId = (Guid)m_body.questId;
		if (questId == Guid.Empty)
		{
			throw new CommandHandleException(1, "유효하지 않는 퀘스트ID입니다. questId = " + questId);
		}
		m_heroDailyQuest = m_myHero.GetDailyQuest(questId);
		if (m_heroDailyQuest == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 일일퀘스트입니다. questId = " + questId);
		}
		if (!m_heroDailyQuest.isAccepted)
		{
			throw new CommandHandleException(1, "일일퀘스트가 수락상태가 아닙니다. questId = " + questId);
		}
		if (m_heroDailyQuest.IsMissionCompleted(m_currentTime))
		{
			throw new CommandHandleException(101, "미션을 완료한 퀘스트입니다. questId = " + questId);
		}
		m_newHeroDailyQuest = m_myHero.CreateHeroDailyQuest(m_heroDailyQuest.slotIndex);
		m_myHero.SetDailyQuest(m_newHeroDailyQuest);
		SaveToDB();
		SaveToLogDB();
		DailyQuestAbandonResponseBody resBody = new DailyQuestAbandonResponseBody();
		resBody.addedDailyQuest = m_newHeroDailyQuest.ToPDHeroDailyQuest(m_currentTime);
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroDailyQuest_Abandon(m_heroDailyQuest.id, m_currentTime));
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroDailyQuest(m_newHeroDailyQuest.id, m_newHeroDailyQuest.hero.id, m_newHeroDailyQuest.slotIndex, m_newHeroDailyQuest.mission.id, m_currentTime));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroDailyQuestCreationLog(m_newHeroDailyQuest.id, m_newHeroDailyQuest.hero.id, m_newHeroDailyQuest.slotIndex, m_newHeroDailyQuest.mission.id, m_currentTime));
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroDailyQuestAbandonLog(Guid.NewGuid(), m_myHero.id, m_heroDailyQuest.id, m_newHeroDailyQuest.id, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
