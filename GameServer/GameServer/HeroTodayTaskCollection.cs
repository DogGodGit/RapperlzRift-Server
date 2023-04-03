using System;
using System.Collections.Generic;
using ClientCommon;

namespace GameServer;

public class HeroTodayTaskCollection
{
	private Hero m_hero;

	private DateTime m_date = DateTime.MinValue.Date;

	private Dictionary<int, HeroTodayTask> m_tasks = new Dictionary<int, HeroTodayTask>();

	public Hero hero => m_hero;

	public DateTime date
	{
		get
		{
			return m_date;
		}
		set
		{
			m_date = value;
		}
	}

	public Dictionary<int, HeroTodayTask> tasks => m_tasks;

	public HeroTodayTaskCollection(Hero hero, DateTime date)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_hero = hero;
		m_date = date;
	}

	public void AddTodayTask(HeroTodayTask task)
	{
		if (task == null)
		{
			throw new ArgumentNullException("task");
		}
		m_tasks.Add(task.task.id, task);
	}

	public HeroTodayTask GetTodayTask(int nId)
	{
		if (!m_tasks.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private HeroTodayTask GetOrCreateTodayTask(int nId)
	{
		HeroTodayTask heroTask = GetTodayTask(nId);
		if (heroTask == null)
		{
			TodayTask todayTask = Resource.instance.GetTodayTask(nId);
			if (todayTask == null)
			{
				return null;
			}
			heroTask = new HeroTodayTask(this, todayTask, 0);
			m_tasks.Add(heroTask.task.id, heroTask);
		}
		return heroTask;
	}

	public void ProcessTask(int nId)
	{
		GetOrCreateTodayTask(nId)?.IncreaseProgressCount();
	}

	public void ClearTasks()
	{
		m_tasks.Clear();
	}

	public List<PDHeroTodayTask> GetPDHeroTodayTasks()
	{
		List<PDHeroTodayTask> insts = new List<PDHeroTodayTask>();
		foreach (HeroTodayTask task in m_tasks.Values)
		{
			insts.Add(task.ToPDHeroTodayTask());
		}
		return insts;
	}
}
