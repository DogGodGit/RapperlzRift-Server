using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class TreatOfFarmQuestAcceptCommandHandler : InGameCommandHandler<TreatOfFarmQuestAcceptCommandBody, TreatOfFarmQuestAcceptResponseBody>
{
	public const short kResult_NotEnoughHeroLevel = 101;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		HeroTreatOfFarmQuest heroTreatOfFarmQuest = m_myHero.treatOfFarmQuest;
		if (heroTreatOfFarmQuest != null && heroTreatOfFarmQuest.regTime.Date == currentDate)
		{
			throw new CommandHandleException(1, "이미 농장의위협퀘스트가 존재합니다.");
		}
		TreatOfFarmQuest treatOfFarmQuest = Resource.instance.treatOfFarmQuest;
		if (m_myHero.level < treatOfFarmQuest.requiredHeroLevel)
		{
			throw new CommandHandleException(101, "농장의위협퀘스트를 수락할 수 있는 레벨이 아닙니다. Level = " + treatOfFarmQuest.requiredHeroLevel);
		}
		Npc questNpc = treatOfFarmQuest.questNpc;
		if (!currentPlace.IsSame(questNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "퀘스트 NPC가 있는 장소가 아닙니다.");
		}
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "퀘스트 NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		HeroTreatOfFarmQuest newHeroTreatOfFarmQuest = new HeroTreatOfFarmQuest(m_myHero, m_currentTime);
		TreatOfFarmQuestMission questMission = treatOfFarmQuest.SelectMission();
		HeroTreatOfFarmQuestMission newHeroTreatOfFarmQuestMission = (newHeroTreatOfFarmQuest.currentMission = new HeroTreatOfFarmQuestMission(newHeroTreatOfFarmQuest, questMission));
		m_myHero.treatOfFarmQuest = newHeroTreatOfFarmQuest;
		SaveToDB(newHeroTreatOfFarmQuest, newHeroTreatOfFarmQuestMission);
		TreatOfFarmQuestAcceptResponseBody resBody = new TreatOfFarmQuestAcceptResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.heroTreatOfFarmQuest = newHeroTreatOfFarmQuest.ToPDHeroTreatOfFarmQuest(m_currentTime);
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroTreatOfFarmQuest treatOfFramQuest, HeroTreatOfFarmQuestMission treatOfFramQuestMission)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddTreatOfFarmQuest(treatOfFramQuest.id, treatOfFramQuest.hero.id, treatOfFramQuest.regTime));
		dbWork.AddSqlCommand(GameDac.CSC_AddTreatOfFarmQuestMission(treatOfFramQuestMission.id, treatOfFramQuestMission.quest.id, treatOfFramQuestMission.mission.id, m_currentTime));
		dbWork.Schedule();
	}
}
