using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroCreatureFarmQuest
{
	private Guid m_instanceId = Guid.Empty;

	private Hero m_hero;

	private CreatureFarmQuestMission m_mission;

	private int m_nProgressCount;

	private DateTimeOffset m_missionStartTime = DateTimeOffset.MinValue;

	private CreatureFarmQuestMissionMonsterInstance m_monsterInst;

	public Guid instanceId => m_instanceId;

	public Hero hero => m_hero;

	public CreatureFarmQuestMission mission => m_mission;

	public int missionNo
	{
		get
		{
			if (m_mission == null)
			{
				return 0;
			}
			return m_mission.no;
		}
	}

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

	public bool isMissionObjectiveCompleted => m_nProgressCount >= m_mission.targetCount;

	public DateTimeOffset missionStartTime => m_missionStartTime;

	public CreatureFarmQuestMissionMonsterInstance targetMonsterInst
	{
		get
		{
			return m_monsterInst;
		}
		set
		{
			m_monsterInst = value;
		}
	}

	public HeroCreatureFarmQuest(Hero hero)
		: this(hero, null, DateTimeOffset.MinValue)
	{
	}

	public HeroCreatureFarmQuest(Hero hero, CreatureFarmQuestMission mission, DateTimeOffset missionStartTime)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_instanceId = Guid.NewGuid();
		m_hero = hero;
		m_mission = mission;
		m_missionStartTime = missionStartTime;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_instanceId = (Guid)dr["questInstanceId"];
		int nMissionId = Convert.ToInt32(dr["missionNo"]);
		m_mission = Resource.instance.creatureFarmQuest.GetMission(nMissionId);
		m_nProgressCount = Convert.ToInt32(dr["progressCount"]);
		m_missionStartTime = (DateTimeOffset)dr["missionStartTime"];
	}

	public float GetRemainingTime(DateTimeOffset currentTime)
	{
		if (m_mission == null)
		{
			return 0f;
		}
		if (m_mission.targetAutoCompletionTime <= 0)
		{
			return 0f;
		}
		float fElapsedTime = (float)(currentTime - m_missionStartTime).TotalSeconds;
		return Math.Max((float)m_mission.targetAutoCompletionTime - fElapsedTime, 0f);
	}

	public void IncreaseProgressCount(DateTimeOffset currentTime)
	{
		m_nProgressCount++;
		ServerEvent.SendCreatureFarmQuestProgressCountUpdated(m_hero.account.peer, m_mission.no, m_nProgressCount);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroCreatureFarmQuest_ProgressCount(m_instanceId, m_nProgressCount));
		dbWork.Schedule();
		if (isMissionObjectiveCompleted)
		{
			CompleteMission(currentTime, bSendEvent: true);
		}
	}

	public void CompleteMission(DateTimeOffset currentTime, bool bSendEvent)
	{
		long lnExpReward = 0L;
		CompleteMission(currentTime, bSendEvent, out lnExpReward);
	}

	public void CompleteMission(DateTimeOffset currentTime, bool bSendEvent, out long lnExpReward)
	{
		lnExpReward = 0L;
		CreatureFarmQuestMissionReward missionReward = m_mission.GetReward(m_hero.level);
		if (missionReward != null)
		{
			lnExpReward = missionReward.expRewardValue;
			lnExpReward += (long)Math.Floor((float)lnExpReward * Cache.instance.GetWorldLevelExpFactor(m_hero.level));
			m_hero.AddExp(lnExpReward, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		}
		if (m_monsterInst != null)
		{
			m_monsterInst = null;
		}
		int nOldMissionNo = m_mission.no;
		CreatureFarmQuestMission nextMission = (m_mission = Resource.instance.creatureFarmQuest.GetMission(nOldMissionNo + 1));
		m_missionStartTime = currentTime;
		m_nProgressCount = 0;
		if (m_mission != null && m_mission.targetType == CreatureFarmQuestMissionTargetType.ExclusiveMonsterHunt)
		{
			m_hero.ProcessCreatureFarmQuestMissionMonsterSpawn(currentTime);
		}
		if (bSendEvent)
		{
			ServerEvent.SendCreatureFarmQuestMissionCompleted(m_hero.account.peer, lnExpReward, m_hero.realMaxHP, m_hero.hp, m_hero.level, m_hero.exp, missionNo);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_hero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroCreatureFarmQuest_Mission(m_instanceId, missionNo, currentTime));
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureFarmQuestMissionRewardLog(Guid.NewGuid(), m_instanceId, m_hero.id, nOldMissionNo, lnExpReward, currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public PDHeroCreatureFarmQuest ToPDHeroCreatureFarmQuest()
	{
		PDHeroCreatureFarmQuest inst = new PDHeroCreatureFarmQuest();
		inst.instanceId = (Guid)m_instanceId;
		inst.missionNo = missionNo;
		inst.progressCount = m_nProgressCount;
		if (m_monsterInst != null)
		{
			lock (m_monsterInst.currentPlace.syncObject)
			{
				inst.monsterInstanceId = m_monsterInst.instanceId;
				inst.monsterPosition = m_monsterInst.position;
				inst.remainingMonsterLifetime = m_monsterInst.remainingLifetime;
				return inst;
			}
		}
		return inst;
	}
}
