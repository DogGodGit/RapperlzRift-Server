using System;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroGuildHuntingQuest
{
	private Guid m_id = Guid.Empty;

	private Guid m_guildId = Guid.Empty;

	private Hero m_hero;

	private GuildHuntingQuestObjective m_objective;

	private int m_nProgressCount;

	public Guid id => m_id;

	public Guid guildId => m_guildId;

	public Hero hero => m_hero;

	public GuildHuntingQuestObjective objective => m_objective;

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

	public bool isObjectiveCompleted => m_nProgressCount >= m_objective.targetCount;

	public HeroGuildHuntingQuest(GuildMember guildMember)
		: this(guildMember, null)
	{
	}

	public HeroGuildHuntingQuest(GuildMember guildMember, GuildHuntingQuestObjective objective)
	{
		if (guildMember == null)
		{
			throw new ArgumentNullException("guildMember");
		}
		m_id = Guid.NewGuid();
		m_guildId = guildMember.guild.id;
		m_hero = guildMember.hero;
		m_objective = objective;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["questInstanceId"];
		m_guildId = (Guid)dr["guildId"];
		int nObjectiveId = Convert.ToInt32(dr["objectiveId"]);
		m_objective = Resource.instance.guildHuntingQuest.GetObjective(nObjectiveId);
		if (m_objective == null)
		{
			throw new Exception("목표가 존재하지 않습니다. nObjectiveId = " + nObjectiveId);
		}
		m_nProgressCount = Convert.ToInt32(dr["progressCount"]);
	}

	public void IncreaseProgressCount()
	{
		m_nProgressCount++;
		ServerEvent.SendGuildHuntingQuestUpdated(m_hero.account.peer, m_nProgressCount);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildHuntingQuest_ProgressCount(m_id, m_nProgressCount));
		dbWork.Schedule();
	}

	public PDHeroGuildHuntingQuest ToPDHeroGuildHuntingQuest()
	{
		PDHeroGuildHuntingQuest inst = new PDHeroGuildHuntingQuest();
		inst.id = (Guid)m_id;
		inst.objectiveId = m_objective.id;
		inst.progressCount = m_nProgressCount;
		return inst;
	}
}
