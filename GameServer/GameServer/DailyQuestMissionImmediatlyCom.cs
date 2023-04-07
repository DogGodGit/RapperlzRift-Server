using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class DailyQuestMissionImmediatlyCompleteCommandHandler : InGameCommandHandler<DailyQuestMissionImmediatlyCompleteCommandBody, DailyQuestMissionImmediatlyCompleteResponseBody>
{
	public const short kResult_AlreadyMissionCompleted = 101;

	public const short kResult_NotEnoughGold = 102;

	private HeroDailyQuest m_heroDailyQuest;

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
			throw new CommandHandleException(1, "퀘스트ID가 유효하지 않습니다. questId = " + questId);
		}
		m_heroDailyQuest = m_myHero.GetDailyQuest(questId);
		if (m_heroDailyQuest == null)
		{
			throw new CommandHandleException(1, "영웅일일퀘스트가 존재하지 안습니다.");
		}
		if (!m_heroDailyQuest.isAccepted)
		{
			throw new CommandHandleException(1, "현재 일일퀘스트는 진행상태가 아닙니다.");
		}
		if (m_heroDailyQuest.IsMissionCompleted(m_currentTime))
		{
			throw new CommandHandleException(101, "이미 미션을 완료된 일일퀘스트입니다.");
		}
		DailyQuestMission dailyQuestMission = m_heroDailyQuest.mission;
		DailyQuestGrade dailyQuestGrade = dailyQuestMission.grade;
		int nRequiredGold = dailyQuestGrade.immediateCompletionRequiredGold;
		int nElaspedMinute = (int)(m_currentTime - m_heroDailyQuest.startTime.Value).TotalMinutes;
		nRequiredGold = (int)((float)nRequiredGold * (1f - (float)nElaspedMinute / (float)dailyQuestGrade.autoCompletionRequiredTime));
		if (m_myHero.gold < nRequiredGold)
		{
			throw new CommandHandleException(102, "골드가 부족합니다.");
		}
		m_myHero.UseGold(nRequiredGold);
		m_heroDailyQuest.missionImmediateCompleted = true;
		SaveToDB();
		SaveToLogDB(nRequiredGold);
		DailyQuestMissionImmediatlyCompleteResponseBody resBody = new DailyQuestMissionImmediatlyCompleteResponseBody();
		resBody.gold = m_myHero.gold;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroDailyQuest_MissionImmediateComplete(m_heroDailyQuest.id, m_currentTime));
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nUsedGold)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroDailyQuestImmediateCompletionLog(Guid.NewGuid(), m_myHero.id, m_heroDailyQuest.id, nUsedGold, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
