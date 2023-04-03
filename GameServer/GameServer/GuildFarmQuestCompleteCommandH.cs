using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildFarmQuestCompleteCommandHandler : InGameCommandHandler<GuildFarmQuestCompleteCommandBody, GuildFarmQuestCompleteResponseBody>
{
	public const short kResult_PerformingQuestNotExist = 101;

	public const short kResult_ObjectiveNotCompleted = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private Guild m_guild;

	private HeroGuildFarmQuest m_heroQuest;

	private long m_lnRewardExp;

	private ItemReward m_itemReward;

	private int m_nRewardGuildContributionPoint;

	private int m_nRewardGuildBuildingPoint;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (!(m_myHero.currentPlace is GuildTerritoryInstance currentPlace))
		{
			throw new CommandHandleException(1, "현재 장소가 길드영지가 아닙니다.");
		}
		m_guild = currentPlace.guild;
		GuildFarmQuest quest = Resource.instance.guildFarmQuest;
		GuildTerritoryNpc questNpc = quest.questNpc;
		if (!questNpc.IsInteractionEnabledPosition(m_myHero.position, m_myHero.radius))
		{
			throw new CommandHandleException(1, "퀘스트 NPC랑 상호작용할 수 있는 거리가 아닙니다.");
		}
		m_heroQuest = m_myHero.guildFarmQuest;
		if (m_heroQuest == null)
		{
			throw new CommandHandleException(101, "현재 진행중인 퀘스트가 존재하지 않습니다.");
		}
		if (!m_heroQuest.isObjectiveCompleted)
		{
			throw new CommandHandleException(102, "퀘스트 목표가 완료되지 않았습니다.");
		}
		m_myHero.RemoveGuildFarmQuest();
		GuildFarmQuestReward reward = quest.GetReward(m_myHero.level);
		if (reward != null)
		{
			m_lnRewardExp = (long)Math.Floor((float)m_lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
			m_lnRewardExp = reward.expRewardValue;
		}
		m_myHero.AddExp(m_lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		m_itemReward = quest.completionItemReward;
		if (m_itemReward != null)
		{
			int nRemainingCount = m_myHero.AddItem(m_itemReward.item, m_itemReward.owned, m_itemReward.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				m_mail = Mail.Create("MAIL_NAME_00007", "MAIL_DESC_00007", m_currentTime);
				m_mail.AddAttachmentWithNo(new MailAttachment(m_itemReward.item, nRemainingCount, m_itemReward.owned));
				m_myHero.AddMail(m_mail, bSendEvent: true);
			}
		}
		m_nRewardGuildContributionPoint = quest.completionGuildContributionPointRewardValue;
		m_myHero.AddGuildContributionPoint(m_nRewardGuildContributionPoint);
		m_nRewardGuildBuildingPoint = quest.completionGuildBuildingPointRewardValue;
		m_guild.AddBuildingPoint(m_nRewardGuildBuildingPoint, m_myHero.id);
		SaveToDB();
		SaveToGameLogDB();
		GuildFarmQuestCompleteResponseBody resBody = new GuildFarmQuestCompleteResponseBody();
		resBody.acquiredExp = m_lnRewardExp;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.guildContributionPoint = m_myHero.guildContributionPoint;
		resBody.totalGuildContributionPoint = m_myHero.totalGuildContributionPoint;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.giBuildingPoint = m_guild.buildingPoint;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_guild.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_myHero.id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildFarmQuest_Status(m_heroQuest.id, 1, m_currentTime));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_GuildContributionPoint(m_myHero));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_BuildingPoint(m_guild.id, m_guild.buildingPoint));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			int nItemId = 0;
			int nItemCount = 0;
			bool bItemOwned = false;
			if (m_itemReward != null)
			{
				nItemId = m_itemReward.item.id;
				nItemCount = m_itemReward.count;
				bItemOwned = m_itemReward.owned;
			}
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildFarmQuestRewardLog(Guid.NewGuid(), m_myHero.guildMember.guild.id, m_myHero.id, m_heroQuest.id, nItemId, nItemCount, bItemOwned, m_lnRewardExp, m_nRewardGuildContributionPoint, m_nRewardGuildBuildingPoint, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
