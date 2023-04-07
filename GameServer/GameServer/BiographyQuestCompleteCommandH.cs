using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class BiographyQuestCompleteCommandHandler : InGameCommandHandler<BiographyQuestCompleteCommandBody, BiographyQuestCompleteResponseBody>
{
	public const short kResult_AlreadyCompletedBiography = 101;

	public const short kResult_NotCompletedQuestObjective = 102;

	public const short kResult_AlreadyCompletedQuest = 103;

	private HeroBiography m_heroBiography;

	private BiographyQuest m_biographyQuest;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nBiographyId = m_body.biographyId;
		int nQuestNo = m_body.questNo;
		if (nBiographyId <= 0)
		{
			throw new CommandHandleException(1, "전기ID가 유효하지 않습니다. nBiographyId = " + nBiographyId);
		}
		if (nQuestNo <= 0)
		{
			throw new CommandHandleException(1, "퀘스트번호가 유효하지 않습니다. nQuestNo = " + nQuestNo);
		}
		m_heroBiography = m_myHero.GetBiography(nBiographyId);
		if (m_heroBiography == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 영웅전기입니다. nBiographyId = " + nBiographyId);
		}
		if (m_heroBiography.completed)
		{
			throw new CommandHandleException(101, "이미 완료한 영웅전기입니다. nBiographyId = " + nBiographyId);
		}
		HeroBiographyQuest heroBiographyQuest = m_heroBiography.quest;
		if (heroBiographyQuest == null)
		{
			throw new CommandHandleException(1, "퀘스트가 존재하지 않습니다.");
		}
		if (heroBiographyQuest.quest.no != nQuestNo)
		{
			throw new CommandHandleException(1, "진행 중인 퀘스트번호가 아닙니다. nQuestNo = " + nQuestNo);
		}
		if (!heroBiographyQuest.isObjectiveCompleted)
		{
			throw new CommandHandleException(102, "목표가 완료되지 않았습니다.");
		}
		if (heroBiographyQuest.completed)
		{
			throw new CommandHandleException(103, "이미 완료한 퀘스트입니다.");
		}
		m_biographyQuest = heroBiographyQuest.quest;
		long lnExpReward = m_biographyQuest.expRewardValue;
		lnExpReward = (long)Math.Floor((float)lnExpReward * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
		m_myHero.AddExp(lnExpReward, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		heroBiographyQuest.completed = true;
		SaveToDB();
		SaveToLogDB(lnExpReward);
		BiographyQuestCompleteResponseBody resBody = new BiographyQuestCompleteResponseBody();
		resBody.acquiredExp = lnExpReward;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroBiographyQuest_Complete(m_heroBiography.hero.id, m_heroBiography.biography.id, m_biographyQuest.no, m_currentTime));
		dbWork.Schedule();
	}

	private void SaveToLogDB(long lnReawrdExp)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroBiographyQuestRewardLog(Guid.NewGuid(), m_myHero.id, m_heroBiography.biography.id, m_biographyQuest.no, lnReawrdExp, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
