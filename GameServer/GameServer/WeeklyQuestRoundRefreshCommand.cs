using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class WeeklyQuestRoundRefreshCommandHandler : InGameCommandHandler<WeeklyQuestRoundRefreshCommandBody, WeeklyQuestRoundRefreshResponseBody>
{
	public const short kResult_NotExistWeeklyQuest = 101;

	public const short kResult_AlreadyWeeklyQuestCompleted = 102;

	public const short kResult_MismatchRoundId = 103;

	public const short kResult_InvalidateStatus = 104;

	public const short kResult_NotEnoughGold = 105;

	private HeroWeeklyQuest m_heroWeeklyQuest;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid roundId = (Guid)m_body.roundId;
		if (roundId == Guid.Empty)
		{
			throw new CommandHandleException(1, "라운드ID가 유효하지 않습니다. roundId = " + roundId);
		}
		m_heroWeeklyQuest = m_myHero.weeklyQuest;
		if (m_heroWeeklyQuest == null)
		{
			throw new CommandHandleException(101, "영웅주간퀘스트가 존재하지 않습니다.");
		}
		if (m_heroWeeklyQuest.isCompleted)
		{
			throw new CommandHandleException(102, "영웅주간퀘스트를 완료했습니다.");
		}
		if (m_heroWeeklyQuest.roundMission == null)
		{
			throw new CommandHandleException(1, "영웅주간퀘스트 미션이 존재하지 않습니다.");
		}
		if (m_heroWeeklyQuest.roundId != roundId)
		{
			throw new CommandHandleException(103, "현재진행중인 라운드ID가 아닙니다. roundId = " + roundId);
		}
		if (!m_heroWeeklyQuest.isRoundCreated)
		{
			throw new CommandHandleException(104, "영웅주간퀘스트가 생성상태가 아닙니다.");
		}
		int nRequiredGold = Resource.instance.weeklyQuest.roundRefreshRequiredGold;
		if (m_myHero.gold < nRequiredGold)
		{
			throw new CommandHandleException(105, "골드가 부족합니다.");
		}
		m_myHero.UseGold(nRequiredGold);
		_ = m_heroWeeklyQuest.roundMissionId;
		m_heroWeeklyQuest.RefreshCurrentRound();
		SaveToDB();
		SaveToLogDB();
		WeeklyQuestRoundRefreshResponseBody resBody = new WeeklyQuestRoundRefreshResponseBody();
		resBody.gold = m_myHero.gold;
		resBody.newRoundId = (Guid)m_heroWeeklyQuest.roundId;
		resBody.newRoundMissionId = m_heroWeeklyQuest.roundMissionId;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroWeeklyQuest(m_heroWeeklyQuest));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyQuestRoundCreationLog(m_heroWeeklyQuest.roundId, m_heroWeeklyQuest.hero.id, m_heroWeeklyQuest.weekStartDate, m_heroWeeklyQuest.roundNo, m_heroWeeklyQuest.roundMissionId, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
