using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroDailyQuest
{
	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private int m_nSlotIndex;

	private DailyQuestMission m_mission;

	private int m_nProgressCount;

	private HashSet<long> m_huntedMonsters = new HashSet<long>();

	private HeroDailyQuestMissionStatus m_status;

	private DateTimeOffset? m_startTime = null;

	private bool m_bMissionImmediateCompleted;

	public Guid id => m_id;

	public Hero hero => m_hero;

	public int slotIndex => m_nSlotIndex;

	public DailyQuestMission mission => m_mission;

	public int progressCount
	{
		get
		{
			return m_nProgressCount;
		}
		set
		{
			m_nProgressCount = value;
		}
	}

	public HeroDailyQuestMissionStatus status => m_status;

	public bool isCreated => m_status == HeroDailyQuestMissionStatus.Creation;

	public bool isAccepted => m_status == HeroDailyQuestMissionStatus.Progress;

	public DateTimeOffset? startTime => m_startTime;

	public bool missionImmediateCompleted
	{
		get
		{
			return m_bMissionImmediateCompleted;
		}
		set
		{
			m_bMissionImmediateCompleted = value;
		}
	}

	public HeroDailyQuest(Hero hero)
		: this(hero, 0, null)
	{
	}

	public HeroDailyQuest(Hero hero, int nSlotIndex, DailyQuestMission mission)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_id = Guid.NewGuid();
		m_hero = hero;
		m_nSlotIndex = nSlotIndex;
		m_mission = mission;
		m_status = HeroDailyQuestMissionStatus.Creation;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["questInstanceId"];
		m_nSlotIndex = Convert.ToInt32(dr["slotIndex"]);
		int nMissionId = Convert.ToInt32(dr["missionId"]);
		m_mission = Resource.instance.dailyQuest.GetMission(nMissionId);
		if (m_mission == null)
		{
			throw new Exception(string.Concat("일일퀘스트미션이 유효하지 않습니다. m_id = ", m_id, ", nMissionId = ", nMissionId));
		}
		m_nProgressCount = Convert.ToInt32(dr["progressCount"]);
		int nStatus = Convert.ToInt32(dr["status"]);
		if (!Enum.IsDefined(typeof(HeroDailyQuestMissionStatus), nStatus))
		{
			throw new Exception(string.Concat("상태가 유효하지 않습니다. m_id = ", m_id, ", nStatus = ", nStatus));
		}
		m_status = (HeroDailyQuestMissionStatus)nStatus;
		m_startTime = SFDBUtil.ToNullableDateTimeOffset(dr["startTime"]);
		m_bMissionImmediateCompleted = Convert.ToBoolean(dr["missionImmediateCompleted"]);
	}

	public void Accept(DateTimeOffset currentTime)
	{
		m_status = HeroDailyQuestMissionStatus.Progress;
		m_startTime = currentTime;
	}

	public bool IsMissionCompleted(DateTimeOffset currentTime)
	{
		if (m_bMissionImmediateCompleted)
		{
			return true;
		}
		if (GetAutoCompletionRemainingTime(currentTime) <= 0f)
		{
			return true;
		}
		if (m_nProgressCount >= m_mission.targetCount)
		{
			return true;
		}
		return false;
	}

	public float GetAutoCompletionRemainingTime(DateTimeOffset currentTime)
	{
		if (!isAccepted)
		{
			return 0f;
		}
		int nAutoCompletionRequiredTime = m_mission.grade.autoCompletionRequiredTime * 60;
		float fElaspsedTime = (int)(currentTime - m_startTime.Value).TotalSeconds;
		return Math.Max((float)nAutoCompletionRequiredTime - fElaspsedTime, 0f);
	}

	public bool IncreaseProgressCountByMonsterQuest(long lnMonsterInstanceId)
	{
		if (!m_huntedMonsters.Add(lnMonsterInstanceId))
		{
			return false;
		}
		IncreaseProgressCount();
		return true;
	}

	public void IncreaseProgressCount()
	{
		m_nProgressCount++;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroDailyQuest_ProgressCount(m_id, m_nProgressCount));
		dbWork.Schedule();
	}

	public PDHeroDailyQuest ToPDHeroDailyQuest(DateTimeOffset currentTime)
	{
		PDHeroDailyQuest inst = new PDHeroDailyQuest();
		inst.id = (Guid)m_id;
		inst.slotIndex = m_nSlotIndex;
		inst.missionId = m_mission.id;
		inst.isAccepted = isAccepted;
		inst.missionImmediateCompleted = m_bMissionImmediateCompleted;
		inst.progressCount = m_nProgressCount;
		inst.autoCompletionRemainingTime = GetAutoCompletionRemainingTime(currentTime);
		return inst;
	}

	public PDHeroDailyQuestProgressCount ToPDHeroDailyQuestProgressCount()
	{
		PDHeroDailyQuestProgressCount inst = new PDHeroDailyQuestProgressCount();
		inst.id = (Guid)m_id;
		inst.progressCount = m_nProgressCount;
		return inst;
	}

	public static List<PDHeroDailyQuest> ToPDHeroDailyQuests(IEnumerable<HeroDailyQuest> quests, DateTimeOffset currentTime)
	{
		List<PDHeroDailyQuest> insts = new List<PDHeroDailyQuest>();
		foreach (HeroDailyQuest quest in quests)
		{
			insts.Add(quest.ToPDHeroDailyQuest(currentTime));
		}
		return insts;
	}
}
