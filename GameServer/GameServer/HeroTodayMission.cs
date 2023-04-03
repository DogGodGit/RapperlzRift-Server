using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroTodayMission
{
	private HeroTodayMissionCollection m_collection;

	private TodayMission m_mission;

	private int m_nProgressCount;

	private bool m_bRewardReceived;

	public HeroTodayMissionCollection collection => m_collection;

	public TodayMission mission => m_mission;

	public int progressCount => m_nProgressCount;

	public bool rewardReceived
	{
		get
		{
			return m_bRewardReceived;
		}
		set
		{
			m_bRewardReceived = value;
		}
	}

	public bool isObjectiveCompleted => m_nProgressCount >= m_mission.targetCount;

	public bool isTutorial => m_mission.id == 10;

	public HeroTodayMission(HeroTodayMissionCollection collection)
		: this(collection, null)
	{
	}

	public HeroTodayMission(HeroTodayMissionCollection collection, TodayMission mission)
	{
		m_collection = collection;
		m_mission = mission;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		int nMissionId = Convert.ToInt32(dr["missionId"]);
		m_mission = Resource.instance.GetTodayMission(nMissionId);
		if (m_mission == null)
		{
			throw new Exception("오늘의미션이 존재하지 않습니다. nMissionId = " + nMissionId);
		}
		m_nProgressCount = Convert.ToInt32(dr["progressCount"]);
		m_bRewardReceived = Convert.ToBoolean(dr["rewardReceived"]);
	}

	public void IncreaseProgressCount()
	{
		m_nProgressCount++;
		ServerEvent.SendTodayMissionUpdated(m_collection.hero.account.peer, m_mission.id, m_nProgressCount);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_collection.hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroTodayMission_ProgressCount(m_collection.hero.id, m_collection.date, m_mission.id, m_nProgressCount));
		dbWork.Schedule();
	}

	public PDHeroTodayMission ToPDHeroTodayMission()
	{
		PDHeroTodayMission inst = new PDHeroTodayMission();
		inst.missionId = m_mission.id;
		inst.progressCount = m_nProgressCount;
		inst.rewardReceived = m_bRewardReceived;
		return inst;
	}
}
