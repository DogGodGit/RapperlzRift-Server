using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CreatureFarmQuestAcceptCommandHandler : InGameCommandHandler<CreatureFarmQuestAcceptCommandBody, CreatureFarmQuestAcceptResponseBody>
{
	public const short kResult_NotEnoughLevel = 101;

	public const short kResult_AlreadyConsigned = 102;

	public const short kResult_OverflowedAcceptionCount = 103;

	public const short kResult_UnableInteractionPositionWithStartNPC = 104;

	private HeroCreatureFarmQuest m_heroCreatureFarmQuest;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
		}
		CreatureFarmQuest creatureFarmQuest = Resource.instance.creatureFarmQuest;
		if (m_myHero.level < creatureFarmQuest.requiredHeroLevel)
		{
			throw new CommandHandleException(101, "레벨이 부족합니다.");
		}
		if (m_myHero.GetRemainingTaskConsignmentStartCount(2, currentDate) <= 0)
		{
			throw new CommandHandleException(102, "이미 위탁한 할일 입니다.");
		}
		m_myHero.RefreshDailyCreatureFarmAcceptionCount(currentDate);
		DateValuePair<int> dailyCreatureFarmQuestAcceptionCount = m_myHero.dailyCreatureFarmAcceptionCount;
		if (dailyCreatureFarmQuestAcceptionCount.value >= creatureFarmQuest.limitCount)
		{
			throw new CommandHandleException(103, "수락횟수가 최대횟수를 넘어갑니다.");
		}
		m_heroCreatureFarmQuest = m_myHero.creatureFarmQuest;
		if (m_heroCreatureFarmQuest != null)
		{
			throw new CommandHandleException(1, "영웅크리처농장퀘스트가 수락중입니다.");
		}
		Npc startNpc = creatureFarmQuest.startNpc;
		if (startNpc == null)
		{
			throw new CommandHandleException(1, "시작NPC가 존재하지 않습니다.");
		}
		if (!currentPlace.IsSame(startNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "현재 장소에 없는 시작NPC 입니다.");
		}
		if (!startNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(104, "시작NPC와 상호작용할 수 없는 위치입니다.");
		}
		CreatureFarmQuestMission mission = creatureFarmQuest.GetMission(1);
		m_heroCreatureFarmQuest = new HeroCreatureFarmQuest(m_myHero, mission, m_currentTime);
		m_myHero.AcceptCreatureFarmQuest(m_heroCreatureFarmQuest);
		dailyCreatureFarmQuestAcceptionCount.value++;
		if (mission.targetType == CreatureFarmQuestMissionTargetType.ExclusiveMonsterHunt)
		{
			m_myHero.ProcessCreatureFarmQuestMissionMonsterSpawn(m_currentTime);
		}
		SaveToDB();
		CreatureFarmQuestAcceptResponseBody resBody = new CreatureFarmQuestAcceptResponseBody();
		resBody.heroCreatureFarmQuest = m_heroCreatureFarmQuest.ToPDHeroCreatureFarmQuest();
		SendResponseOK(resBody);
		m_myHero.ProcessTodayTask(35, currentDate);
		m_myHero.ProcessMainQuestForContent(23);
		m_myHero.ProcessSubQuestForContent(23);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroCreatureFarmQuest(m_heroCreatureFarmQuest.instanceId, m_heroCreatureFarmQuest.hero.id, m_heroCreatureFarmQuest.missionNo, m_currentTime));
		dbWork.Schedule();
	}
}
