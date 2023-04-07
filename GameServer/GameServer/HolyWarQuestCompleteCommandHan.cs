using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class HolyWarQuestCompleteCommandHandler : InGameCommandHandler<HolyWarQuestCompleteCommandBody, HolyWarQuestCompleteResponseBody>
{
	public const short kResult_LimitTimeNotElapsed = 101;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	private HeroHolyWarQuest m_heroQuest;

	private long m_lnRewardExp;

	private int m_nRewardExploitPoint;

	private int m_nAcquiredExploitPoint;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
		}
		m_heroQuest = m_myHero.holyWarQuest;
		if (m_heroQuest == null)
		{
			throw new CommandHandleException(1, "현재 진행중인 퀘스트가 존재하지 않습니다.");
		}
		if (!m_heroQuest.IsLimitTimeElapsed(m_currentTime))
		{
			throw new CommandHandleException(101, "제한시간이 경과되지 않았습니다.");
		}
		HolyWarQuest quest = Resource.instance.holyWarQuest;
		Npc questNpc = quest.questNpc;
		if (!currentPlace.IsSame(questNpc.continent.id, m_myHero.nationId))
		{
			throw new CommandHandleException(1, "퀘스트 NPC가 있는 장소가 아닙니다.");
		}
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "퀘스트 NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		m_myHero.holyWarQuest = null;
		HolyWarQuestReward reward = quest.GetReward(m_myHero.level);
		if (reward != null)
		{
			m_lnRewardExp = reward.expRewardValue;
			m_lnRewardExp = (long)Math.Floor((float)m_lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
			m_myHero.AddExp(m_lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		}
		HolyWarQuestGloryLevel gloryLevel = m_heroQuest.gloryLevel;
		if (gloryLevel != null)
		{
			m_nRewardExploitPoint = gloryLevel.exploitPointRewardValue;
			m_nAcquiredExploitPoint = m_myHero.AddExploitPoint(m_nRewardExploitPoint, m_currentTime, bSaveToDB: false);
		}
		SaveToDB();
		SaveToGameLogDB();
		HolyWarQuestCompleteResponseBody resBody = new HolyWarQuestCompleteResponseBody();
		resBody.acquiredExp = m_lnRewardExp;
		resBody.acquiredExploitPoint = m_nAcquiredExploitPoint;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.exploitPoint = m_myHero.exploitPoint;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.date = (DateTime)m_currentDate;
		resBody.dailyExploitPoint = m_myHero.dailyExploitPoint.value;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHolyWarQuest_Completion(m_heroQuest.id, m_currentTime));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Exploit(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHolyWarQuestRewardLog(Guid.NewGuid(), m_myHero.id, m_heroQuest.id, m_heroQuest.killCount, m_nRewardExploitPoint, m_nAcquiredExploitPoint, m_lnRewardExp, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
