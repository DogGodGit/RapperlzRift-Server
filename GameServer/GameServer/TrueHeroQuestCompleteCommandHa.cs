using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class TrueHeroQuestCompleteCommandHandler : InGameCommandHandler<TrueHeroQuestCompleteCommandBody, TrueHeroQuestCompleteResponseBody>
{
	public const short kResult_UnableInteractionPositionWithQuestNPC = 101;

	public const short kResult_AlreadyCompletedQuest = 102;

	public const short kResult_NotObjectiveCompletion = 103;

	private HeroTrueHeroQuest m_heroTrueHeroQuest;

	private long m_lnExpReward;

	private int m_nExploitPointReward;

	private int m_nAcquriedExploitPoint;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
		}
		TrueHeroQuest trueHeroQuest = Resource.instance.trueHeroQuest;
		Npc questNpc = trueHeroQuest.questNpc;
		if (!currentPlace.IsSame(questNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "퀘스트NPC가 있는 장소가 아닙니다.");
		}
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(101, "상호작용할 수 있는 위치가 아닙니다.");
		}
		m_heroTrueHeroQuest = m_myHero.trueHeroQuest;
		if (m_heroTrueHeroQuest == null)
		{
			throw new CommandHandleException(1, "영웅진정한영웅퀘스트가 존재하지 않습니다.");
		}
		if (m_heroTrueHeroQuest.completed)
		{
			throw new CommandHandleException(102, "이미 퀘스트를 완료했습니다.");
		}
		if (!m_heroTrueHeroQuest.isObjectiveCompleted)
		{
			throw new CommandHandleException(103, "퀘스트 목표가 완료되지 않았습니다.");
		}
		TrueHeroQuestReward trueHeroQuestReward = trueHeroQuest.GetReward(m_myHero.level);
		if (trueHeroQuestReward != null)
		{
			m_lnExpReward = trueHeroQuestReward.expRewardValue;
			m_nExploitPointReward = trueHeroQuestReward.m_exploitPointRewardValue;
		}
		if (m_lnExpReward > 0)
		{
			m_lnExpReward = (long)Math.Floor((float)m_lnExpReward * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
			m_myHero.AddExp(m_lnExpReward, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		}
		if (m_nExploitPointReward > 0)
		{
			m_nAcquriedExploitPoint = m_myHero.AddExploitPoint(m_nExploitPointReward, m_currentTime, bSaveToDB: false);
		}
		m_heroTrueHeroQuest.completed = true;
		SaveToDB();
		SaveToLogDB();
		TrueHeroQuestCompleteResponseBody resBody = new TrueHeroQuestCompleteResponseBody();
		resBody.acquiredExp = m_lnExpReward;
		resBody.acquiredExploitPoint = m_nAcquriedExploitPoint;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.exploitPoint = m_myHero.exploitPoint;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.date = (DateTime)m_currentTime.Date;
		resBody.dailyExploitPoint = m_myHero.dailyExploitPoint.value;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Exploit(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroTrueHeroQuest_Completion(m_myHero.id));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroTrueHeroQuestRewardLog(Guid.NewGuid(), m_myHero.id, m_heroTrueHeroQuest.id, m_nExploitPointReward, m_nAcquriedExploitPoint, m_lnExpReward, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
