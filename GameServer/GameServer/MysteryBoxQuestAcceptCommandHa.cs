using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MysteryBoxQuestAcceptCommandHandler : InGameCommandHandler<MysteryBoxQuestAcceptCommandBody, MysteryBoxQuestAcceptResponseBody>
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
		if (m_myHero.mysteryBoxQuest != null)
		{
			throw new CommandHandleException(1, "현재 진행중인 퀘스트가 존재합니다.");
		}
		MysteryBoxQuest quest = Resource.instance.mysteryBoxQuest;
		if (m_myHero.level < quest.requiredHeroLevel)
		{
			throw new CommandHandleException(101, "영웅의 레벨이 부족합니다. heroLevel = " + m_myHero.level);
		}
		m_myHero.RefreshDailyMysteryBoxQuestStartCount(m_currentDate);
		DateValuePair<int> startCount = m_myHero.dailyMysteryBoxQuestStartCount;
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
		HeroMysteryBoxQuest heroQuest = new HeroMysteryBoxQuest(m_myHero);
		m_myHero.mysteryBoxQuest = heroQuest;
		startCount.value++;
		SaveToDB();
		MysteryBoxQuestAcceptResponseBody resBody = new MysteryBoxQuestAcceptResponseBody();
		resBody.quest = heroQuest.ToPDHeroMysteryBoxQuest();
		resBody.date = (DateTime)startCount.date;
		resBody.dailyStartCount = startCount.value;
		SendResponseOK(resBody);
		m_myHero.ProcessSeriesMission(8);
		m_myHero.ProcessTodayMission(8, m_currentTime);
		m_myHero.ProcessTodayTask(4, m_currentDate);
		m_myHero.IncreaseOpen7DayEventProgressCount(14);
		m_myHero.ProcessRetrievalProgressCount(8, m_currentDate);
		m_myHero.ProcessMainQuestForContent(9);
		m_myHero.ProcessSubQuestForContent(9);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddMysteryBoxQuest(m_myHero.mysteryBoxQuest.id, m_myHero.id, m_currentTime));
		dbWork.Schedule();
	}
}
