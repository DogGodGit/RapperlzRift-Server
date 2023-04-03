using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HolyWarQuestAcceptCommandHandler : InGameCommandHandler<HolyWarQuestAcceptCommandBody, HolyWarQuestAcceptResponseBody>
{
	public const short kResult_NotEnoughHeroLevel = 101;

	public const short kResult_NotQuestTime = 102;

	public const short kResult_AlreadyAccepted = 103;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	private float m_fTime;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		m_fTime = (float)m_currentTime.TimeOfDay.TotalSeconds;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		if (m_myHero.holyWarQuest != null)
		{
			throw new CommandHandleException(1, "현재 진행중인 퀘스트가 존재합니다.");
		}
		HolyWarQuest quest = Resource.instance.holyWarQuest;
		if (m_myHero.level < quest.requiredHeroLevel)
		{
			throw new CommandHandleException(101, "영웅의 레벨이 부족합니다. heroLevel = " + m_myHero.level);
		}
		HolyWarQuestSchedule schedule = quest.GetScheduleByTime(m_fTime);
		if (schedule == null)
		{
			throw new CommandHandleException(102, "현재 퀘스트시간이 아닙니다. m_currentTime = " + m_currentTime);
		}
		m_myHero.RefreshDailyHolyWarQuestStartScheduleCollection(m_currentDate);
		HeroHolyWarQuestStartScheduleCollection startScheduleCollection = m_myHero.dailyHolyWarQuestStartScheduleCollection;
		if (startScheduleCollection.ContainsSchedule(schedule.id))
		{
			throw new CommandHandleException(103, "이미 수락했습니다.");
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
		HeroHolyWarQuest heroQuest = new HeroHolyWarQuest(m_myHero, schedule, m_currentTime);
		m_myHero.holyWarQuest = heroQuest;
		startScheduleCollection.AddSchedule(schedule.id);
		SaveToDB();
		HolyWarQuestAcceptResponseBody resBody = new HolyWarQuestAcceptResponseBody();
		resBody.date = (DateTime)startScheduleCollection.date;
		resBody.quest = heroQuest.ToPDHeroHolyWarQuest(m_currentTime);
		SendResponseOK(resBody);
		m_myHero.ProcessTodayTask(18, m_currentDate);
		m_myHero.IncreaseOpen7DayEventProgressCount(16);
		m_myHero.ProcessOrdealQuestMissions(OrdealQuestMissionType.HolyWarQuest, 1, m_currentTime);
		m_myHero.ProcessMainQuestForContent(14);
		m_myHero.ProcessSubQuestForContent(14);
	}

	private void SaveToDB()
	{
		HeroHolyWarQuest heroQuest = m_myHero.holyWarQuest;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHolyWarQuest(heroQuest.id, m_myHero.id, heroQuest.schedule.id, heroQuest.startTime));
		dbWork.Schedule();
	}
}
