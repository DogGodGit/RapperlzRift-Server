using System;
using System.Data;
using ClientCommon;

namespace GameServer;

public class HeroGuildMissionQuestMission
{
	private Guid m_id = Guid.Empty;

	private HeroGuildMissionQuest m_quest;

	private Guid m_guildId = Guid.Empty;

	private GuildMission m_mission;

	private int m_nProgressCount;

	private int m_nSpawnedMonsterContinentId;

	private GuildMissionMonsterInstance m_monsterInst;

	public Guid id => m_id;

	public HeroGuildMissionQuest quest => m_quest;

	public Guid guildId => m_guildId;

	public GuildMission mission => m_mission;

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

	public bool isCompleted => m_nProgressCount >= m_mission.targetCount;

	public int spawnedMonsterContinentId
	{
		get
		{
			return m_nSpawnedMonsterContinentId;
		}
		set
		{
			m_nSpawnedMonsterContinentId = value;
		}
	}

	public GuildMissionMonsterInstance monsterInst
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

	public HeroGuildMissionQuestMission(HeroGuildMissionQuest quest)
		: this(quest, null, Guid.Empty)
	{
	}

	public HeroGuildMissionQuestMission(HeroGuildMissionQuest quest, GuildMission mission, Guid guildId)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_id = Guid.NewGuid();
		m_quest = quest;
		m_mission = mission;
		m_guildId = guildId;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["missionInstanceId"];
		m_guildId = (Guid)dr["guildId"];
		int nMissionId = Convert.ToInt32(dr["missionId"]);
		m_mission = Resource.instance.guildMissionQuest.GetMission(nMissionId);
		if (m_mission == null)
		{
			throw new Exception("미션이 존재하지 않습니다. nMissionId = " + nMissionId);
		}
		m_nProgressCount = Convert.ToInt32(dr["progressCount"]);
		if (m_mission.type == GuildMissionType.Summon)
		{
			m_nSpawnedMonsterContinentId = Convert.ToInt32(dr["spawnedMonsterContinentId"]);
		}
	}

	public PDHeroGuildMission ToPDHeroGuildMission()
	{
		PDHeroGuildMission inst = new PDHeroGuildMission();
		inst.missionId = mission.id;
		inst.progressCount = m_nProgressCount;
		if (m_monsterInst != null)
		{
			lock (m_monsterInst.currentPlace.syncObject)
			{
				inst.monsterInstanceId = m_monsterInst.instanceId;
				inst.monsterSpawnedContinentId = m_nSpawnedMonsterContinentId;
				inst.monsterPosition = m_monsterInst.position;
				inst.remainingMonsterLifetime = m_monsterInst.remainingLifetime;
				return inst;
			}
		}
		return inst;
	}
}
