using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroTreatOfFarmQuestMission
{
	public const int kStatus_Start = 0;

	public const int kStatus_Complete = 1;

	public const int kStatus_Fail = 2;

	private Guid m_id = Guid.Empty;

	private HeroTreatOfFarmQuest m_quest;

	private TreatOfFarmQuestMission m_mission;

	private DateTimeOffset? m_monsterSpawnTime = null;

	private TreatOfFarmQuestMonsterInstance m_targetMonsterInst;

	public Guid id => m_id;

	public HeroTreatOfFarmQuest quest
	{
		get
		{
			return m_quest;
		}
		set
		{
			m_quest = value;
		}
	}

	public TreatOfFarmQuestMission mission => m_mission;

	public DateTimeOffset? monsterSpawnTime
	{
		get
		{
			return m_monsterSpawnTime;
		}
		set
		{
			m_monsterSpawnTime = value;
		}
	}

	public TreatOfFarmQuestMonsterArrange targetMonsterArrange => m_mission.GetMonsterArrange(m_quest.hero.level);

	public TreatOfFarmQuestMonsterInstance targetMonsterInst
	{
		get
		{
			return m_targetMonsterInst;
		}
		set
		{
			m_targetMonsterInst = value;
		}
	}

	public HeroTreatOfFarmQuestMission(HeroTreatOfFarmQuest quest)
		: this(quest, null)
	{
	}

	public HeroTreatOfFarmQuestMission(HeroTreatOfFarmQuest quest, TreatOfFarmQuestMission mission)
	{
		m_id = Guid.NewGuid();
		m_quest = quest;
		m_mission = mission;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["missionInstanceId"];
		int nMissionId = Convert.ToInt32(dr["missionId"]);
		m_mission = Resource.instance.treatOfFarmQuest.GetMission(nMissionId);
		if (m_mission == null)
		{
			throw new Exception("농장의위협퀘스트미션이 존재하지 않습니다. nMissionId = " + nMissionId);
		}
		m_monsterSpawnTime = SFDBUtil.ToNullableDateTimeOffset(dr["monsterSpawnTime"]);
	}

	public PDHeroTreatOfFarmQuestMission ToPDHeroTreatOfFarmMission(DateTimeOffset time)
	{
		PDHeroTreatOfFarmQuestMission inst = new PDHeroTreatOfFarmQuestMission();
		inst.missionId = m_mission.id;
		if (m_targetMonsterInst != null)
		{
			lock (m_targetMonsterInst.currentPlace.syncObject)
			{
				inst.monsterInstanceId = m_targetMonsterInst.instanceId;
				inst.monsterPosition = m_targetMonsterInst.position;
				inst.remainingMonsterLifetime = m_targetMonsterInst.remainingLifetime;
				return inst;
			}
		}
		return inst;
	}
}
