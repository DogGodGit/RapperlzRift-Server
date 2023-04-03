using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroTodayTask
{
	private HeroTodayTaskCollection m_collection;

	private TodayTask m_task;

	private int m_nProgressCount;

	public HeroTodayTaskCollection collection => m_collection;

	public TodayTask task => m_task;

	public int progressCount => m_nProgressCount;

	public HeroTodayTask(HeroTodayTaskCollection collection, TodayTask todayTask, int nProgressCount)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		if (todayTask == null)
		{
			throw new ArgumentNullException("todayTask");
		}
		m_collection = collection;
		m_task = todayTask;
		m_nProgressCount = nProgressCount;
	}

	public void IncreaseProgressCount()
	{
		m_nProgressCount++;
		DateValuePair<int> achievementDailyPoint = m_collection.hero.achievementDailyPoint;
		achievementDailyPoint.value += m_task.achievementPoint;
		ServerEvent.SendTodayTaskUpdated(m_collection.hero.account.peer, m_collection.date, m_task.id, m_nProgressCount, achievementDailyPoint.value);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_collection.hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_AchievementPoint(m_collection.hero.id, achievementDailyPoint.date, achievementDailyPoint.value, m_collection.hero.receivedAchievementRewardNo));
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroTodayTask(m_collection.hero.id, m_collection.date, m_task.id, m_nProgressCount));
		dbWork.Schedule();
	}

	public PDHeroTodayTask ToPDHeroTodayTask()
	{
		PDHeroTodayTask inst = new PDHeroTodayTask();
		inst.taskId = m_task.id;
		inst.progressCount = m_nProgressCount;
		return inst;
	}
}
