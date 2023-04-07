using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class DimensionRaidQuestAcceptCommandHandler : InGameCommandHandler<DimensionRaidQuestAcceptCommandBody, DimensionRaidQuestAcceptResponseBody>
{
	public const short kResult_NotEnoughHeroLevel = 101;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.dimensionRaidQuest != null)
		{
			throw new CommandHandleException(1, "현재 진행중인 퀘스트가 존재합니다.");
		}
		DimensionRaidQuest quest = Resource.instance.dimensionRaidQuest;
		if (m_myHero.level < quest.requiredHeroLevel)
		{
			throw new CommandHandleException(101, "영웅의 레벨이 부족합니다. heroLevel = " + m_myHero.level);
		}
		m_myHero.RefreshDailyDimensionRaidQuestStartCount(m_currentDate);
		DateValuePair<int> startCount = m_myHero.dailyDimensionRaidQuestStartCount;
		if (startCount.value >= quest.limitCount)
		{
			throw new CommandHandleException(1, "금일 시작횟수를 모두 사용했습니다.");
		}
		Npc questNpc = quest.questNpc;
		if (!currentPlace.IsSame(questNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "퀘스트 NPC가 있는 장소가 아닙니다.");
		}
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "퀘스트 NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		HeroDimensionRaidQuest heroQuest = new HeroDimensionRaidQuest(m_myHero, 1);
		m_myHero.dimensionRaidQuest = heroQuest;
		startCount.value++;
		SaveToDB();
		DimensionRaidQuestAcceptResponseBody resBody = new DimensionRaidQuestAcceptResponseBody();
		resBody.quest = heroQuest.ToPDHeroDimensionRaidQuest();
		resBody.date = (DateTime)startCount.date;
		resBody.dailyStartCount = startCount.value;
		SendResponseOK(resBody);
		m_myHero.ProcessSeriesMission(7);
		m_myHero.ProcessTodayMission(7, m_currentTime);
		m_myHero.ProcessTodayTask(3, m_currentDate);
		m_myHero.IncreaseOpen7DayEventProgressCount(15);
		m_myHero.ProcessRetrievalProgressCount(5, m_currentDate);
		m_myHero.ProcessOrdealQuestMissions(OrdealQuestMissionType.DimensionRaidQuest, 1, m_currentTime);
		m_myHero.ProcessMainQuestForContent(12);
		m_myHero.ProcessSubQuestForContent(12);
	}

	private void SaveToDB()
	{
		HeroDimensionRaidQuest heroQuest = m_myHero.dimensionRaidQuest;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddDimensionRaidQuest(heroQuest.id, m_myHero.id, heroQuest.step, m_currentTime));
		dbWork.Schedule();
	}
}
