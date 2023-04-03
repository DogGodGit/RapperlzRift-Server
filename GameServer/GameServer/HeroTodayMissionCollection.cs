using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroTodayMissionCollection
{
	private Hero m_hero;

	private DateTime m_date = DateTime.MinValue.Date;

	private Dictionary<int, HeroTodayMission> m_missions = new Dictionary<int, HeroTodayMission>();

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

	public Dictionary<int, HeroTodayMission> missions => m_missions;

	public HeroTodayMissionCollection(Hero hero, DateTime date)
	{
		m_hero = hero;
		m_date = date;
	}

	public void AddMission(HeroTodayMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_missions.Add(mission.mission.id, mission);
	}

	public HeroTodayMission GetMission(int nMissionId)
	{
		if (!m_missions.TryGetValue(nMissionId, out var value))
		{
			return null;
		}
		return value;
	}

	public void InitializeMissions()
	{
		if (!m_hero.todayMissionTutorialStarted)
		{
			HeroTodayMission tutorialMissionId = new HeroTodayMission(this, Resource.instance.GetTodayMission(10));
			AddMission(tutorialMissionId);
		}
		TodayMissionPool pool = Resource.instance.GetTodayMissionPool(m_hero.level);
		List<TodayMission> missions = pool.Select(Resource.instance.todayMissionCount - m_missions.Count);
		foreach (TodayMission mission in missions)
		{
			HeroTodayMission heroMission2 = new HeroTodayMission(this, mission);
			AddMission(heroMission2);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		foreach (HeroTodayMission heroMission in m_missions.Values)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroTodayMission(m_hero.id, m_date, heroMission.mission.id));
		}
		dbWork.Schedule();
	}

	public void ClearMissions()
	{
		m_missions.Clear();
	}

	public List<PDHeroTodayMission> GetPDHeroTodayMissions()
	{
		List<PDHeroTodayMission> insts = new List<PDHeroTodayMission>();
		foreach (HeroTodayMission mission in m_missions.Values)
		{
			insts.Add(mission.ToPDHeroTodayMission());
		}
		return insts;
	}
}
