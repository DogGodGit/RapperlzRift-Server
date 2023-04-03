using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroTreatOfFarmQuest
{
	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private bool m_bCompleted;

	private int m_nCompletedMissionCount;

	private HeroTreatOfFarmQuestMission m_currentMission;

	public Guid id => m_id;

	public Hero hero => m_hero;

	public DateTimeOffset regTime => m_regTime;

	public bool objectiveCompleted => m_nCompletedMissionCount >= Resource.instance.treatOfFarmQuest.limitCount;

	public bool completed
	{
		get
		{
			return m_bCompleted;
		}
		set
		{
			m_bCompleted = value;
		}
	}

	public int completedMissionCount
	{
		get
		{
			return m_nCompletedMissionCount;
		}
		set
		{
			m_nCompletedMissionCount = value;
		}
	}

	public HeroTreatOfFarmQuestMission currentMission
	{
		get
		{
			return m_currentMission;
		}
		set
		{
			m_currentMission = value;
		}
	}

	public HeroTreatOfFarmQuest(Hero hero)
	{
		m_hero = hero;
	}

	public HeroTreatOfFarmQuest(Hero hero, DateTimeOffset regTime)
	{
		m_id = Guid.NewGuid();
		m_hero = hero;
		m_regTime = regTime;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["instanceId"];
		m_regTime = (DateTimeOffset)dr["regTime"];
		m_bCompleted = Convert.ToBoolean(dr["completed"]);
	}

	public void CompleteCurrentMission(DateTimeOffset time)
	{
		if (m_currentMission != null)
		{
			m_currentMission = null;
			if (m_regTime.Date == time.Date)
			{
				m_nCompletedMissionCount++;
			}
		}
	}

	public void FailCurrentMission(DateTimeOffset time, bool bSendEvent)
	{
		if (m_currentMission != null)
		{
			HeroTreatOfFarmQuestMission mission = m_currentMission;
			m_currentMission = null;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateTreatOfFarmMission_Status(mission.id, 2, time));
			dbWork.Schedule();
			if (bSendEvent)
			{
				ServerEvent.SendTreatOfFarmQuestMissionFail(m_hero.account.peer);
			}
		}
	}

	public PDHeroTreatOfFarmQuest ToPDHeroTreatOfFarmQuest(DateTimeOffset time)
	{
		PDHeroTreatOfFarmQuest inst = new PDHeroTreatOfFarmQuest();
		inst.completed = m_bCompleted;
		inst.completedMissionCount = m_nCompletedMissionCount;
		inst.currentMission = ((m_currentMission != null) ? m_currentMission.ToPDHeroTreatOfFarmMission(time) : null);
		return inst;
	}
}
