using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class TreatOfFarmQuestMissionAcceptCommandHandler : InGameCommandHandler<TreatOfFarmQuestMissionAcceptCommandBody, TreatOfFarmQuestMissionAcceptResponseBody>
{
	public const short kResult_CurrentDateNotEqualToQuestStartDate = 101;

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
		if (heroTreatOfFarmQuest == null)
		{
			throw new CommandHandleException(1, "농장의위협 퀘스트가 존재하지 않습니다.");
		}
		if (heroTreatOfFarmQuest.regTime.Date != currentDate)
		{
			throw new CommandHandleException(101, "현재 날짜가 퀘스트시작날짜와 다릅니다.");
		}
		if (heroTreatOfFarmQuest.currentMission != null)
		{
			throw new CommandHandleException(1, "이미 진행중인 농장의위협 미션이 존재합니다.");
		}
		if (heroTreatOfFarmQuest.objectiveCompleted)
		{
			throw new CommandHandleException(1, "이미 농장의위협 퀘스트 목표를 완료했습니다.");
		}
		TreatOfFarmQuest treatOfFarmQuest = Resource.instance.treatOfFarmQuest;
		Npc questNpc = treatOfFarmQuest.questNpc;
		if (!currentPlace.IsSame(questNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "퀘스트 NPC가 있는 장소가 아닙니다.");
		}
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "퀘스트 NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		TreatOfFarmQuestMission treatOfFarmQuestMission = treatOfFarmQuest.SelectMission();
		HeroTreatOfFarmQuestMission newHeroTreatOfFarmQuestMission = (heroTreatOfFarmQuest.currentMission = new HeroTreatOfFarmQuestMission(heroTreatOfFarmQuest, treatOfFarmQuestMission));
		SaveToDB(newHeroTreatOfFarmQuestMission);
		TreatOfFarmQuestMissionAcceptResponseBody resBody = new TreatOfFarmQuestMissionAcceptResponseBody();
		resBody.heroTreatOfFarmQuestMission = newHeroTreatOfFarmQuestMission.ToPDHeroTreatOfFarmMission(m_currentTime);
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroTreatOfFarmQuestMission newHeroTreatOfFarmQuestMission)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddTreatOfFarmQuestMission(newHeroTreatOfFarmQuestMission.id, newHeroTreatOfFarmQuestMission.quest.id, newHeroTreatOfFarmQuestMission.mission.id, m_currentTime));
		dbWork.Schedule();
	}
}
