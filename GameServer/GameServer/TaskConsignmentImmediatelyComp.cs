using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class TaskConsignmentImmediatelyCompleteCommandHandler : InGameCommandHandler<TaskConsignmentImmediatelyCompleteCommandBody, TaskConsignmentImmediatelyCompleteResponseBody>
{
	public const short kResult_AlreadyElapsedCompletionTime = 101;

	public const short kResult_NotEnoughGold = 102;

	public const short kResult_NotEnoughInventory = 103;

	private HeroTaskConsignment m_heroTaskConsignment;

	private long m_lnUsedGold;

	private long m_lnRewardExp;

	private ResultItemCollection m_resultItemCollection;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Guild m_myGuild;

	private int m_nGuildFund;

	private int m_nGuildBuildingPoint;

	private int m_nGuildContributionPoint;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid instanceId = (Guid)m_body.instanceId;
		if (instanceId == Guid.Empty)
		{
			throw new CommandHandleException(1, "인스턴스ID가 유효하지 않습니다.");
		}
		m_heroTaskConsignment = m_myHero.GetTaskConsignment(instanceId);
		if (m_heroTaskConsignment == null)
		{
			throw new CommandHandleException(1, "영웅위탁이 존재하지 않습니다.");
		}
		if (m_heroTaskConsignment.GetRemainingTime(m_currentTime) <= 0f)
		{
			throw new CommandHandleException(101, "완료시간이 경과되었습니다.");
		}
		int nUsedExpItemId = m_heroTaskConsignment.usedExpItemId;
		Item usedExpItem = null;
		if (nUsedExpItemId > 0)
		{
			usedExpItem = Resource.instance.GetItem(nUsedExpItemId);
			if (usedExpItem.type.id != 11)
			{
				usedExpItem = null;
			}
		}
		TaskConsignment consignment = m_heroTaskConsignment.consignment;
		m_lnUsedGold = consignment.immediateCompletionRequiredGold;
		if (usedExpItem == null)
		{
			float fElapsedTime = m_heroTaskConsignment.GetElapsedTime(m_currentTime);
			float fIntervalGold = (float)(int)Math.Floor(fElapsedTime / (float)consignment.immediateCompletionRequiredGoldReduceInterval) * consignment.immediateCompletionIntervalGold;
			m_lnUsedGold = (long)Math.Floor((float)m_lnUsedGold - fIntervalGold);
		}
		if (m_myHero.gold < m_lnUsedGold)
		{
			throw new CommandHandleException(102, "골드가 부족합니다.");
		}
		TaskConsignmentExpReward consignmentExpReward = consignment.GetExpReward(m_myHero.level);
		m_resultItemCollection = new ResultItemCollection();
		foreach (TaskConsignmentItemReward consignmentItemReward in consignment.itemRewards)
		{
			ItemReward itemReward = consignmentItemReward.itemReward;
			if (itemReward != null)
			{
				m_resultItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
			}
		}
		if (!m_myHero.IsAvailableInventory(m_resultItemCollection))
		{
			throw new CommandHandleException(103, "인벤토리가 부족합니다.");
		}
		m_myHero.UseGold(m_lnUsedGold);
		if (consignmentExpReward != null)
		{
			m_lnRewardExp = consignmentExpReward.expRewardValue;
			if (usedExpItem != null)
			{
				m_lnRewardExp = (long)((float)(m_lnRewardExp * usedExpItem.value2) / 10000f);
			}
			m_lnRewardExp = (long)Math.Floor((float)m_lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_myHero.level));
			m_myHero.AddExp(m_lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		}
		foreach (ResultItem resultItem in m_resultItemCollection.resultItems)
		{
			m_myHero.AddItem(resultItem.item, resultItem.owned, resultItem.count, m_changedInventorySlots);
		}
		GuildMember myGuildMember = m_myHero.guildMember;
		m_myGuild = null;
		if (consignment.isGuildContent && myGuildMember != null)
		{
			m_myGuild = myGuildMember.guild;
			m_nGuildFund = consignment.guildFundRewardValue;
			m_nGuildBuildingPoint = consignment.guildBuildingPointRewardValue;
			m_nGuildContributionPoint = consignment.guildContributionPointRewardValue;
			if (m_nGuildFund > 0)
			{
				m_myGuild.AddFund(m_nGuildFund, myGuildMember.id);
			}
			if (m_nGuildBuildingPoint > 0)
			{
				m_myGuild.AddBuildingPoint(m_nGuildBuildingPoint, myGuildMember.id);
			}
			if (m_nGuildContributionPoint > 0)
			{
				m_myHero.AddGuildContributionPoint(m_nGuildContributionPoint);
			}
		}
		m_myHero.CompleteTaskConsignment(m_heroTaskConsignment.instanceId);
		SaveToDB();
		SaveToLogDB();
		TaskConsignmentImmediatelyCompleteResponseBody resBody = new TaskConsignmentImmediatelyCompleteResponseBody();
		resBody.gold = m_myHero.gold;
		resBody.acquiredExp = m_lnRewardExp;
		resBody.level = m_myHero.level;
		resBody.exp = m_myHero.exp;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.totalGuildContributionPoint = m_myHero.totalGuildContributionPoint;
		resBody.guildContributionPoint = m_myHero.guildContributionPoint;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		if (m_myGuild != null)
		{
			resBody.giFund = m_myGuild.fund;
			resBody.giBuildingPoint = m_myGuild.buildingPoint;
		}
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		if (m_myGuild != null)
		{
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateGuildWork(m_myGuild.id));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_Fund(m_myGuild.id, m_myGuild.fund));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_BuildingPoint(m_myGuild.id, m_myGuild.buildingPoint));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildContributionPoint(m_myHero.id, m_myHero.totalGuildContributionPoint, m_myHero.guildContributionPoint));
		}
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_myHero));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroTaskConsignment_Status(m_heroTaskConsignment.instanceId, 2, m_currentTime));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroTaskConsignmentCompletionLog(logId, m_myHero.id, m_heroTaskConsignment.instanceId, (int)m_heroTaskConsignment.GetRemainingTime(m_currentTime), m_lnUsedGold, m_lnRewardExp, m_nGuildFund, m_nGuildBuildingPoint, m_nGuildContributionPoint, m_currentTime));
			foreach (ResultItem resultItem in m_resultItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroTaskConsignmentCompletionDetailLog(Guid.NewGuid(), logId, resultItem.item.id, resultItem.owned, resultItem.count));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
