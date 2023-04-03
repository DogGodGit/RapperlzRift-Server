using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroHolyWarQuest
{
	public const float kLimitTimeFactor = 0.9f;

	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private HolyWarQuestSchedule m_schedule;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private int m_nKillCount;

	public Guid id => m_id;

	public Hero hero => m_hero;

	public HolyWarQuestSchedule schedule => m_schedule;

	public DateTimeOffset startTime => m_startTime;

	public int killCount => m_nKillCount;

	public HolyWarQuestGloryLevel gloryLevel => Resource.instance.holyWarQuest.GetGloryLevelByKillCount(m_nKillCount);

	public HeroHolyWarQuest(Hero hero)
		: this(hero, null, DateTimeOffset.MinValue)
	{
	}

	public HeroHolyWarQuest(Hero hero, HolyWarQuestSchedule schedule, DateTimeOffset startTime)
	{
		m_id = Guid.NewGuid();
		m_hero = hero;
		m_schedule = schedule;
		m_startTime = startTime;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["instanceId"];
		m_schedule = Resource.instance.holyWarQuest.GetSchedule(Convert.ToInt32(dr["scheduleId"]));
		if (m_schedule == null)
		{
			throw new Exception("스케쥴이 존재하지 않습니다. m_id = " + m_id);
		}
		m_startTime = (DateTimeOffset)dr["regTime"];
		m_nKillCount = Convert.ToInt32(dr["killCount"]);
	}

	private float GetElapsedTime(DateTimeOffset time)
	{
		return (float)(time - m_startTime).TotalSeconds;
	}

	public float GetRemainingTime(DateTimeOffset time)
	{
		return Math.Max(0f, (float)Resource.instance.holyWarQuest.limitTime - GetElapsedTime(time));
	}

	public bool IsLimitTimeElapsed(DateTimeOffset time)
	{
		return GetElapsedTime(time) >= (float)Resource.instance.holyWarQuest.limitTime * 0.9f;
	}

	public void IncreaseKillCount()
	{
		m_nKillCount++;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHolyWarQuest_KillCount(m_id, m_nKillCount));
		dbWork.Schedule();
		ServerEvent.SendHolyWarQuestUpdated(m_hero.account.peer, m_nKillCount);
	}

	public PDHeroHolyWarQuest ToPDHeroHolyWarQuest(DateTimeOffset time)
	{
		PDHeroHolyWarQuest inst = new PDHeroHolyWarQuest();
		inst.killCount = m_nKillCount;
		inst.remainingTime = GetRemainingTime(time);
		return inst;
	}
}
