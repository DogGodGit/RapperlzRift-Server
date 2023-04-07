using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class OrdealQuestSlotCompleteCommandHandler : InGameCommandHandler<OrdealQuestSlotCompleteCommandBody, OrdealQuestSlotCompleteResponseBody>
{
	public const short kResult_CompletedAllMission = 101;

	public const short kResult_NotCompletedMissionObjective = 102;

	private HeroOrdealQuestSlot m_heroOrdealQuestSlot;

	private OrdealQuestMission m_mission;

	private OrdealQuestMission m_newMission;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nIndex = m_body.index;
		if (nIndex < 0)
		{
			throw new CommandHandleException(1, "인덱스가 유효하지 않습니다. nIndex = " + nIndex);
		}
		HeroOrdealQuest heroOrdealQuest = m_myHero.ordealQuest;
		if (heroOrdealQuest == null)
		{
			throw new CommandHandleException(1, "영웅시련퀘스트가 존재하지 않습니다.");
		}
		if (heroOrdealQuest.completed)
		{
			throw new CommandHandleException(1, "이미 영웅시련퀘스트를 완료했습니다.");
		}
		m_heroOrdealQuestSlot = heroOrdealQuest.GetSlot(nIndex);
		if (m_heroOrdealQuestSlot == null)
		{
			throw new CommandHandleException(1, "영웅시련퀘스트미션이 존재하지 않습니다. nIndex = " + nIndex);
		}
		if (m_heroOrdealQuestSlot.mission == null)
		{
			throw new CommandHandleException(101, "모든 미션을 완료했습니다. nIndex = " + nIndex);
		}
		if (!m_heroOrdealQuestSlot.IsObjectiveCompleted(m_currentTime))
		{
			throw new CommandHandleException(102, "영웅시련퀘스트미션의 목표가 완료되지 않았습니다.");
		}
		m_mission = m_heroOrdealQuestSlot.mission;
		long lnExpReward = m_mission.expRewardValue;
		m_myHero.AddExp(lnExpReward, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		m_heroOrdealQuestSlot.CompleteMission();
		OrdealQuest ordealQuest = Resource.instance.GetOrdealQuest(heroOrdealQuest.no);
		OrdealQuestSlot questSlot = ordealQuest.GetSlot(m_heroOrdealQuestSlot.index);
		m_newMission = questSlot.GetMission(m_mission.no + 1);
		if (m_newMission != null)
		{
			m_heroOrdealQuestSlot.StartMission(m_newMission, m_currentTime);
		}
		SaveToDB();
		SaveToLogDB(lnExpReward);
		OrdealQuestSlotCompleteResponseBody resBody = new OrdealQuestSlotCompleteResponseBody();
		resBody.nextMissionNo = ((m_newMission != null) ? m_newMission.no : 0);
		resBody.acquiredExp = lnExpReward;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroOrdealQuestMission_Complete(m_heroOrdealQuestSlot.quest.hero.id, m_heroOrdealQuestSlot.quest.no, m_heroOrdealQuestSlot.index, m_mission.no, m_currentTime));
		if (m_newMission != null)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroOrdealQuestMission(m_heroOrdealQuestSlot.quest.hero.id, m_heroOrdealQuestSlot.quest.no, m_heroOrdealQuestSlot.index, m_newMission.no, m_currentTime));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(long lnRewardExp)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroOrdealQuestMissionRewardLog(Guid.NewGuid(), m_myHero.id, m_heroOrdealQuestSlot.quest.no, m_heroOrdealQuestSlot.index, m_mission.no, lnRewardExp, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
