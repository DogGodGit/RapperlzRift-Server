using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class OrdealQuestCompleteCommandHandler : InGameCommandHandler<OrdealQuestCompleteCommandBody, OrdealQuestCompleteResponseBody>
{
	public const short kResult_AlreadyCompltedQuest = 101;

	public const short kResult_NotCompletedQuestObjective = 102;

	private HeroOrdealQuest m_heroOrdealQuest;

	private HeroOrdealQuest m_newHeroOrdealQuest;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_heroOrdealQuest = m_myHero.ordealQuest;
		if (m_heroOrdealQuest == null)
		{
			throw new CommandHandleException(1, "영웅시련퀘스트가 존재하지 않습니다.");
		}
		if (m_heroOrdealQuest.completed)
		{
			throw new CommandHandleException(101, "이미 영웅시련퀘스트를 완료했습니다.");
		}
		if (!m_heroOrdealQuest.isObjectiveCompleted)
		{
			throw new CommandHandleException(102, "영웅시련퀘스트의 목표를 완료하지 않았습니다.");
		}
		OrdealQuest ordealQuest = Resource.instance.GetOrdealQuest(m_heroOrdealQuest.no);
		long lnExpReward = ordealQuest.expRewardValue;
		m_myHero.AddExp(lnExpReward, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		m_heroOrdealQuest.completed = true;
		OrdealQuest nextOrdealQuest = Resource.instance.GetOrdealQuest(m_heroOrdealQuest.no + 1);
		if (nextOrdealQuest != null && nextOrdealQuest.requiredHeroLevel <= m_myHero.level)
		{
			m_newHeroOrdealQuest = new HeroOrdealQuest(m_myHero);
			m_newHeroOrdealQuest.Start(nextOrdealQuest, m_currentTime);
			m_myHero.ordealQuest = m_newHeroOrdealQuest;
		}
		SaveToDB();
		SaveToLogDB(lnExpReward);
		OrdealQuestCompleteResponseBody resBody = new OrdealQuestCompleteResponseBody();
		resBody.nextQuest = ((m_newHeroOrdealQuest != null) ? m_newHeroOrdealQuest.ToPDHeroOrdealQuest(m_currentTime) : null);
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
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroOrdealQuest_Complete(m_heroOrdealQuest.hero.id, m_heroOrdealQuest.no, m_currentTime));
		if (m_newHeroOrdealQuest != null)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroOrdealQuest(m_newHeroOrdealQuest.hero.id, m_newHeroOrdealQuest.no, m_currentTime));
			HeroOrdealQuestSlot[] slots = m_newHeroOrdealQuest.slots;
			foreach (HeroOrdealQuestSlot slot in slots)
			{
				dbWork.AddSqlCommand(GameDac.CSC_AddHeroOrdealQuestMission(slot.quest.hero.id, slot.quest.no, slot.index, slot.mission.no, m_currentTime));
			}
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(long lnRewardExp)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroOrdealQuestRewardLog(Guid.NewGuid(), m_myHero.id, m_heroOrdealQuest.no, lnRewardExp, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
