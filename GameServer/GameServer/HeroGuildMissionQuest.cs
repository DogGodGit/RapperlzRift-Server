using System;
using System.Collections.Generic;
using System.Data;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroGuildMissionQuest
{
	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private Guid m_guildId = Guid.Empty;

	private DateTime m_date = DateTime.MinValue.Date;

	private bool m_bCompleted;

	private int m_nCompletionCount;

	private HeroGuildMissionQuestMission m_currentMission;

	public Guid id => m_id;

	public Hero hero => m_hero;

	public Guid guildId => m_guildId;

	public DateTime date => m_date;

	public int completionCount
	{
		get
		{
			return m_nCompletionCount;
		}
		set
		{
			m_nCompletionCount = value;
		}
	}

	public bool isObjectiveCompleted => m_nCompletionCount >= Resource.instance.guildMissionQuest.limitCount;

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

	public HeroGuildMissionQuestMission currentMission => m_currentMission;

	public HeroGuildMissionQuest(Hero hero, DateTime date)
		: this(hero, Guid.Empty, date)
	{
	}

	public HeroGuildMissionQuest(Hero hero, Guid guildId, DateTime date)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_id = Guid.NewGuid();
		m_hero = hero;
		m_guildId = guildId;
		m_date = date;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["instanceId"];
		m_guildId = (Guid)dr["guildId"];
		m_bCompleted = Convert.ToBoolean(dr["completed"]);
	}

	public void SetCurrentMission(HeroGuildMissionQuestMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_currentMission = mission;
	}

	public void AbandonCurrentMission(DateTimeOffset time)
	{
		if (m_currentMission != null)
		{
			HeroGuildMissionQuestMission mission = m_currentMission;
			m_currentMission = null;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildMissionQuestMission_Status(mission.id, 2, time));
			dbWork.Schedule();
		}
	}

	public void FailCurrentMission(DateTimeOffset time, bool bSendEvent)
	{
		if (m_currentMission == null)
		{
			return;
		}
		if (m_currentMission.mission.type == GuildMissionType.Summon && m_currentMission.monsterInst != null)
		{
			lock (m_currentMission.monsterInst.currentPlace.syncObject)
			{
				m_currentMission.monsterInst.currentPlace.RemoveMonster(m_currentMission.monsterInst, bSendEvent: true);
			}
		}
		HeroGuildMissionQuestMission mission = m_currentMission;
		m_currentMission = null;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildMissionQuestMission_Status(mission.id, 3, time));
		dbWork.Schedule();
		if (bSendEvent)
		{
			ServerEvent.SendGuildMissionFailed(m_hero.account.peer);
		}
	}

	public void ProgressCurrentMission(DateTimeOffset currentTime)
	{
		if (m_currentMission != null)
		{
			m_currentMission.progressCount++;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildMissionQuestMission_ProgressCount(m_currentMission.id, m_currentMission.progressCount));
			dbWork.Schedule();
			ServerEvent.SendGuildMissionUpdated(m_hero.account.peer, m_currentMission.progressCount);
			if (m_currentMission.isCompleted)
			{
				CompleteCurrentMission(currentTime);
			}
		}
	}

	public void CompleteCurrentMission(DateTimeOffset currentTime)
	{
		if (m_currentMission == null || !m_currentMission.isCompleted)
		{
			return;
		}
		m_nCompletionCount++;
		HeroGuildMissionQuestMission oldHeroGuildMission = m_currentMission;
		GuildMember guildMember = m_hero.guildMember;
		Guild guild = guildMember.guild;
		GuildMissionQuest guildMissionQuest = Resource.instance.guildMissionQuest;
		HeroGuildMissionQuestMission newHeroGuildMission = null;
		ItemReward itemReward = guildMissionQuest.completionItemReward;
		HashSet<InventorySlot> changedInventorySlots = new HashSet<InventorySlot>();
		Mail mail = null;
		if (!isObjectiveCompleted)
		{
			GuildMission newGuildMission = guildMissionQuest.SelectMission();
			newHeroGuildMission = new HeroGuildMissionQuestMission(this, newGuildMission, guild.id);
			if (newGuildMission.type == GuildMissionType.Summon)
			{
				ContinentInstance currentPlace = (ContinentInstance)m_hero.currentPlace;
				GuildMissionMonsterInstance monsterInst = new GuildMissionMonsterInstance();
				monsterInst.Init(currentPlace, newGuildMission.targetSummonMonsterArrange, newGuildMission.SelectSummonMonsterPosition(m_hero.position), newGuildMission.targetSummonMonsterKillLimitTime, m_hero.id, m_hero.name, newHeroGuildMission.id, currentTime);
				currentPlace.SpawnMonster(monsterInst, currentTime);
				newHeroGuildMission.monsterInst = monsterInst;
				newHeroGuildMission.spawnedMonsterContinentId = currentPlace.continent.id;
			}
			SetCurrentMission(newHeroGuildMission);
		}
		else
		{
			itemReward = guildMissionQuest.completionItemReward;
			int nRemainingCount = m_hero.AddItem(itemReward.item, itemReward.owned, itemReward.count, changedInventorySlots);
			if (nRemainingCount > 0)
			{
				mail = Mail.Create("MAIL_NAME_00009", "MAIL_DESC_00009", currentTime);
				mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingCount, itemReward.owned));
				m_hero.AddMail(mail, bSendEvent: true);
			}
			m_bCompleted = true;
		}
		GuildMissionQuestReward missionQuestReward = guildMissionQuest.GetReward(m_hero.level);
		long lnRewardExp = missionQuestReward.expRewardValue;
		lnRewardExp = (long)Math.Floor((float)lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_hero.level));
		m_hero.AddExp(lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		GuildMission guildMission = oldHeroGuildMission.mission;
		int nRewardContributionPoint = guildMission.contributionPointRewardValue;
		int nRewardFund = 0;
		int nRewardBuildingPoint = 0;
		nRewardFund = guildMission.fundRewardValue;
		nRewardBuildingPoint = guildMission.buildingPointRewardValue;
		guild.AddFund(nRewardFund, m_hero.id);
		guild.AddBuildingPoint(nRewardBuildingPoint, m_hero.id);
		m_hero.AddGuildContributionPoint(nRewardContributionPoint);
		ServerEvent.SendGuildMissionCompleted(m_hero.account.peer, currentTime.Date, newHeroGuildMission?.ToPDHeroGuildMission(), m_nCompletionCount, lnRewardExp, m_hero.realMaxHP, m_hero.hp, m_hero.exp, m_hero.level, m_hero.totalGuildContributionPoint, m_hero.guildContributionPoint, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray(), guild.fund, guild.buildingPoint);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateGuildWork(guild.id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_Fund(guild.id, guild.fund));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_BuildingPoint(guild.id, guild.buildingPoint));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_GuildContributionPoint(m_hero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_hero));
		if (m_bCompleted)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildMissionQuest_Complete(m_id, currentTime));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildMissionQuestMission_Status(oldHeroGuildMission.id, 1, currentTime));
		if (newHeroGuildMission != null)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroGuildMissionQuestMission(newHeroGuildMission.id, newHeroGuildMission.quest.id, newHeroGuildMission.guildId, newHeroGuildMission.mission.id, newHeroGuildMission.spawnedMonsterContinentId, currentTime));
		}
		foreach (InventorySlot slot in changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(mail));
		}
		dbWork.Schedule();
		m_hero.ProcessTodayTask(20, currentTime.Date);
		m_hero.IncreaseOpen7DayEventProgressCount(9);
		if (m_bCompleted)
		{
			guild.CompleteGuildDailyObjective(currentTime.Date, 1, guildMember);
		}
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildMissionRewardLog(logId, guild.id, m_hero.id, oldHeroGuildMission.id, oldHeroGuildMission.mission.id, nRewardContributionPoint, nRewardFund, nRewardBuildingPoint, lnRewardExp, itemReward?.item.id ?? 0, itemReward?.count ?? 0, itemReward?.owned ?? false, currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public PDHeroGuildMissionQuest ToPDHeroGuildMissionQuest()
	{
		PDHeroGuildMissionQuest inst = new PDHeroGuildMissionQuest();
		inst.completedMissionCount = m_nCompletionCount;
		inst.completed = m_bCompleted;
		inst.currentMission = ((m_currentMission != null) ? m_currentMission.ToPDHeroGuildMission() : null);
		return inst;
	}
}
