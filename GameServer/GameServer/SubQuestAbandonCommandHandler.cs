using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SubQuestAbandonCommandHandler : InGameCommandHandler<SubQuestAbandonCommandBody, SubQuestAbandonResponseBody>
{
	public const short kResult_NotAcceptedSubQuest = 101;

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
			throw new CommandHandleException(1, "서브퀘스트ID가 유효하지 않습니다. nQuestId = " + nQuestId);
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
		if (!subQuest.abandonmentEnabled)
		{
			throw new CommandHandleException(1, "포기가 불가능한 서브퀘스트입니다.");
		}
		m_heroSubQuest.Abandon();
		SaveToDB();
		SaveToLogDB();
		SendResponseOK(null);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSubQuest_Abandon(m_heroSubQuest.hero.id, m_heroSubQuest.quest.id, m_currentTime));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroSubQuestAbandonmentLog(Guid.NewGuid(), m_heroSubQuest.hero.id, m_heroSubQuest.quest.id, m_heroSubQuest.progressCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
