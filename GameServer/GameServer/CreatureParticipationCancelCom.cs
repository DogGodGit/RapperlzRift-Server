using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CreatureParticipationCancelCommandHandler : InGameCommandHandler<CreatureParticipationCancelCommandBody, CreatureParticipationCancelResponseBody>
{
	private HeroCreature m_heroCreature;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid instanceId = (Guid)m_body.instanceId;
		if (instanceId == Guid.Empty)
		{
			throw new CommandHandleException(1, "인스턴스ID가 유효하지 않습니다. instanceId = " + instanceId);
		}
		m_heroCreature = m_myHero.GetCreature(instanceId);
		if (m_heroCreature == null)
		{
			throw new CommandHandleException(1, "영웅크리처가 존재하지 않습니다. instanceId = " + instanceId);
		}
		if (!m_heroCreature.participated)
		{
			throw new CommandHandleException(1, "영웅크리처가 출전중이 아닙니다. instanceId = " + instanceId);
		}
		bool bOldParticipated = m_heroCreature.participated;
		m_myHero.CancelCreatureParticipation();
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SaveToLogDB(bOldParticipated);
		CreatureParticipationCancelResponseBody resBody = new CreatureParticipationCancelResponseBody();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CreatureParticipation(m_myHero.id, m_myHero.participationCreatureId));
		dbWork.Schedule();
	}

	private void SaveToLogDB(bool bOldParticipated)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureStatusUpdateLog(Guid.NewGuid(), m_heroCreature.instanceId, m_myHero.id, bOldParticipated, m_heroCreature.participated, bOldCheered: false, m_heroCreature.cheered, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
