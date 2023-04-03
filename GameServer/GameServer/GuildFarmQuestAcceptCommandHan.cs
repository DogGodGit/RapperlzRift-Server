using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildFarmQuestAcceptCommandHandler : InGameCommandHandler<GuildFarmQuestAcceptCommandBody, GuildFarmQuestAcceptResponseBody>
{
	public const short kResult_DailyStartCountIsMax = 101;

	public const short kResult_NotQuestTime = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	private Guild m_guild;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		if (!(m_myHero.currentPlace is GuildTerritoryInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소가 길드영지가 아닙니다.");
		}
		m_guild = currentPlace.guild;
		GuildFarmQuest quest = Resource.instance.guildFarmQuest;
		if (!quest.IsQuestTime((float)m_currentTime.TimeOfDay.TotalSeconds))
		{
			throw new CommandHandleException(102, "퀘스트시간이 아닙니다.");
		}
		GuildTerritoryNpc questNpc = quest.questNpc;
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "퀘스트 NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		if (m_myHero.guildFarmQuest != null)
		{
			throw new CommandHandleException(1, "현재 진행중인 퀘스트가 존재합니다.");
		}
		m_myHero.RefreshDailyGuildFarmQuestStartCount(m_currentDate);
		DateValuePair<int> startCount = m_myHero.dailyGuildFarmQuestStartCount;
		if (startCount.value >= quest.limitCount)
		{
			throw new CommandHandleException(101, "일일시작횟수가 최대입니다.");
		}
		HeroGuildFarmQuest heroQuest = new HeroGuildFarmQuest(m_myHero);
		m_myHero.SetGuildFarmQuest(heroQuest);
		startCount.value++;
		SaveToDB();
		GuildFarmQuestAcceptResponseBody resBody = new GuildFarmQuestAcceptResponseBody();
		resBody.quest = heroQuest.ToPDHeroGuildFarmQuest();
		resBody.date = (DateTime)startCount.date;
		resBody.dailyStartCount = startCount.value;
		SendResponseOK(resBody);
		m_myHero.ProcessTodayTask(23, m_currentDate);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroGuildFarmQuest(m_myHero.guildFarmQuest.id, m_myHero.id, m_guild.id, m_currentTime));
		dbWork.Schedule();
	}
}
