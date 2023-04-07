using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class TrueHeroQuestAcceptCommandHandler : InGameCommandHandler<TrueHeroQuestAcceptCommandBody, TrueHeroQuestAcceptResponseBody>
{
	public const short kResult_UnableInteractionPositionWithQuestNPC = 101;

	public const short kResult_NotEnoughHeroLevel = 102;

	public const short kResult_NotEnoughVipLevel = 103;

	public const short kResult_ExistAcceptedQuest = 104;

	public const short kResult_AlreadyCompletedQuest = 105;

	public HeroTrueHeroQuest m_heroTrueHeroQuest;

	public DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
		}
		TrueHeroQuest trueHeroQuest = Resource.instance.trueHeroQuest;
		Npc questNpc = trueHeroQuest.questNpc;
		if (!currentPlace.IsSame(questNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "퀘스트NPC가 있는 장소가 아닙니다.");
		}
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(101, "상호작용할 수 있는 위치가 아닙니다.");
		}
		if (m_myHero.level < trueHeroQuest.reqruiedHeroLevel)
		{
			throw new CommandHandleException(102, "레벨이 부족합니다.");
		}
		if (m_myHero.vipLevel.level < trueHeroQuest.requiredVipLevel)
		{
			throw new CommandHandleException(103, "VIP레벨이 부족합니다.");
		}
		m_heroTrueHeroQuest = m_myHero.trueHeroQuest;
		if (m_heroTrueHeroQuest != null)
		{
			if (!m_heroTrueHeroQuest.completed)
			{
				throw new CommandHandleException(104, "진행중인 진정한영웅퀘스트가 존재합니다.");
			}
			if (m_heroTrueHeroQuest.regTime.Date == currentDate)
			{
				throw new CommandHandleException(105, "금일 진정한영웅퀘스트를 완료했습니다.");
			}
		}
		m_heroTrueHeroQuest = new HeroTrueHeroQuest(m_myHero, 1, m_myHero.vipLevel.level, m_currentTime);
		m_myHero.SetTrueHeroQuest(m_heroTrueHeroQuest);
		SaveToDB();
		SaveToLogDB();
		TrueHeroQuestAcceptResponseBody resBody = new TrueHeroQuestAcceptResponseBody();
		resBody.trueHeroQuest = m_heroTrueHeroQuest.ToPDHeroTrueHeroQuest();
		SendResponseOK(resBody);
		m_myHero.ProcessMainQuestForContent(19);
		m_myHero.ProcessSubQuestForContent(19);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroTrueHeroQuest(m_heroTrueHeroQuest.hero.id, m_heroTrueHeroQuest.id, m_heroTrueHeroQuest.stepNo, m_heroTrueHeroQuest.vipLevel, m_heroTrueHeroQuest.regTime));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroTrueHeroQuestStartLog(m_heroTrueHeroQuest.id, m_heroTrueHeroQuest.hero.id, m_heroTrueHeroQuest.vipLevel, m_heroTrueHeroQuest.regTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
