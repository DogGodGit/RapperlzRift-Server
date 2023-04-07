using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class DailyQuestRefreshCommandHandler : InGameCommandHandler<DailyQuestRefreshCommandBody, DailyQuestRefreshResponseBody>
{
	public const short kResult_NotEnoughGold = 101;

	private List<HeroDailyQuest> m_oldHeroDailyQuests = new List<HeroDailyQuest>();

	private List<HeroDailyQuest> m_newHeroDailyQuests = new List<HeroDailyQuest>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (!m_myHero.IsAvailableRefreshDailyQuest())
		{
			throw new CommandHandleException(1, "일일퀘스트를 갱신할 수 있는 상태가 아닙니다");
		}
		m_myHero.RefreshDailyQuestFreeRefreshCount(m_currentTime.Date);
		DateValuePair<int> dailyQuestFreeRefreshCount = m_myHero.dailyQuestFreeRefreshCount;
		DailyQuest dailyQuest = Resource.instance.dailyQuest;
		int nRefreshRequiredGold = 0;
		if (dailyQuestFreeRefreshCount.value >= dailyQuest.freeRefreshCount)
		{
			nRefreshRequiredGold = dailyQuest.refreshRequiredGold;
			if (m_myHero.gold < nRefreshRequiredGold)
			{
				throw new CommandHandleException(101, "골드가 부족합니다.");
			}
			m_myHero.UseGold(nRefreshRequiredGold);
		}
		else
		{
			dailyQuestFreeRefreshCount.value++;
		}
		for (int i = 0; i < m_myHero.dailyQuests.Length; i++)
		{
			HeroDailyQuest heroDailyQuest = m_myHero.dailyQuests[i];
			if (heroDailyQuest == null || heroDailyQuest.isCreated)
			{
				HeroDailyQuest newDailyQuest = m_myHero.CreateHeroDailyQuest(i);
				if (heroDailyQuest != null)
				{
					m_oldHeroDailyQuests.Add(heroDailyQuest);
				}
				m_myHero.SetDailyQuest(newDailyQuest);
				m_newHeroDailyQuests.Add(newDailyQuest);
			}
		}
		SaveToDB();
		SaveToLogDB(nRefreshRequiredGold);
		DailyQuestRefreshResponseBody resBody = new DailyQuestRefreshResponseBody();
		resBody.date = (DateTime)dailyQuestFreeRefreshCount.date;
		resBody.dailyQuestFreeRefreshCount = dailyQuestFreeRefreshCount.value;
		resBody.gold = m_myHero.gold;
		resBody.addedDailyQuests = HeroDailyQuest.ToPDHeroDailyQuests(m_newHeroDailyQuests, m_currentTime).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_DailyQuestFreeRefreshDateCount(m_myHero.id, m_myHero.dailyQuestFreeRefreshCount.date, m_myHero.dailyQuestFreeRefreshCount.value));
		foreach (HeroDailyQuest quest2 in m_oldHeroDailyQuests)
		{
			dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroDailyQuest(quest2.id));
		}
		foreach (HeroDailyQuest quest in m_newHeroDailyQuests)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroDailyQuest(quest.id, quest.hero.id, quest.slotIndex, quest.mission.id, m_currentTime));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nUsedGold)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			foreach (HeroDailyQuest quest2 in m_newHeroDailyQuests)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroDailyQuestCreationLog(quest2.id, quest2.hero.id, quest2.slotIndex, quest2.mission.id, m_currentTime));
			}
			Guid refreshLogId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroDailyQuestRefreshLog(refreshLogId, m_myHero.id, nUsedGold, m_currentTime));
			foreach (HeroDailyQuest quest in m_newHeroDailyQuests)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroDailyQuestRefreshDetailLog(Guid.NewGuid(), refreshLogId, quest.id));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
