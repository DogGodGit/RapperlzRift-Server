using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CreatureFarmQuestMissionMoveObjectiveCompleteCommandHandler : InGameCommandHandler<CreatureFarmQuestMissionMoveObjectiveCompleteCommandBody, CreatureFarmQuestMissionMoveObjectiveCompleteResponseBody>
{
	public const short kResult_UnableInteractionPositionWithTargetPosition = 101;

	private HeroCreatureFarmQuest m_heroCreatureFarmQuest;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid instanceId = (Guid)m_body.instanceId;
		int nMissionNo = m_body.missionNo;
		if (instanceId == Guid.Empty)
		{
			throw new CommandHandleException(1, "인스턴스ID가 유효하지 않습니다. instanceId = " + instanceId);
		}
		if (nMissionNo <= 0)
		{
			throw new CommandHandleException(1, "미션번호가 유효하지 않습니다. nMissionNo = " + nMissionNo);
		}
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
		}
		m_heroCreatureFarmQuest = m_myHero.creatureFarmQuest;
		if (m_heroCreatureFarmQuest == null)
		{
			throw new CommandHandleException(1, "영웅크리처농장퀘스트가 존재하지 않습니다.");
		}
		if (m_heroCreatureFarmQuest.instanceId != instanceId)
		{
			throw new CommandHandleException(1, "진행중인 영웅크리처농장퀘스트ID가 아닙니다. instanceId = " + instanceId);
		}
		CreatureFarmQuestMission mission = m_heroCreatureFarmQuest.mission;
		if (mission == null)
		{
			throw new CommandHandleException(1, "영웅크리처농장퀘스트미션이 존재하지 않습니다.");
		}
		if (mission.no != nMissionNo)
		{
			throw new CommandHandleException(1, "진행중인 영웅크리처농장퀘스트 미션번호가 아닙니다. nMissionNo = " + nMissionNo);
		}
		if (mission.targetType != CreatureFarmQuestMissionTargetType.Move)
		{
			throw new CommandHandleException(1, "해당 미션의 목표는 이동타입이 아닙니다.");
		}
		if (!currentPlace.IsSame(mission.targetContinent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "현재 장소에 없는 지점입니다.");
		}
		if (!mission.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(101, "목표를 완료할 수 있는 위치가 아닙니다.");
		}
		m_heroCreatureFarmQuest.progressCount++;
		SaveToDB();
		long lnExpReward = 0L;
		m_heroCreatureFarmQuest.CompleteMission(m_currentTime, bSendEvent: false, out lnExpReward);
		CreatureFarmQuestMissionMoveObjectiveCompleteResponseBody resBody = new CreatureFarmQuestMissionMoveObjectiveCompleteResponseBody();
		resBody.acquiredExp = lnExpReward;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.exp = m_myHero.exp;
		resBody.level = m_myHero.level;
		resBody.nextMissionNo = m_heroCreatureFarmQuest.missionNo;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroCreatureFarmQuest_ProgressCount(m_heroCreatureFarmQuest.instanceId, m_heroCreatureFarmQuest.progressCount));
		dbWork.Schedule();
	}
}
