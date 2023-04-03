using System;
using System.Collections.Generic;
using System.Linq;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class OwnerProspectQuestRewardReceiveCommandHandler : InGameCommandHandler<OwnerProspectQuestRewardReceiveCommandBody, OwnerProspectQuestRewardReceiveResponseBody>
{
	public const short kResult_QuestNotExist = 101;

	public const short kResult_QuestNotCompleted = 102;

	private Guid m_instanceId = Guid.Empty;

	private HeroProspectQuest m_quest;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_instanceId = (Guid)m_body.instanceId;
		if (m_instanceId == Guid.Empty)
		{
			throw new CommandHandleException(1, "인스턴스ID가 유효하지 않습니다.");
		}
		m_currentTime = DateTimeUtil.currentTime;
		m_quest = m_myHero.GetOwnerProspectQuest(m_instanceId);
		if (m_quest == null)
		{
			throw new CommandHandleException(101, "퀘스트가 존재하지 않습니다. m_instanceId = " + m_instanceId);
		}
		if (!m_quest.isCompleted)
		{
			throw new CommandHandleException(102, "퀘스트가 완료되지 않았습니다. m_instanceId = " + m_instanceId);
		}
		foreach (ProspectQuestOwnerReward reward in m_quest.blessingTargetLevel.prospectQuestOwnerRewards.Values)
		{
			ItemReward itemReward = reward.itemReward;
			int nRemainingCount = m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_28", "MAIL_REWARD_D_28", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingCount, itemReward.owned));
			}
		}
		m_quest.ownerRewarded = true;
		m_myHero.RemoveOwnerProspectQuest(m_instanceId);
		if (m_quest.isAllRewarded)
		{
			Cache.instance.RemoveProspectQuest(m_instanceId);
		}
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB();
		SaveToGameLogDB();
		OwnerProspectQuestRewardReceiveResponseBody resBody = new OwnerProspectQuestRewardReceiveResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateContentWork(QueuingWorkContentId.GameDB_ProspectQuest));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroProspectQuest_OwnerRewarded(m_quest.instanceId, m_quest.ownerRewarded));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroProspectQuestRewardLog(logId, m_instanceId, m_myHero.id, bIsOwner: true, m_currentTime));
			foreach (ProspectQuestOwnerReward reward in m_quest.blessingTargetLevel.prospectQuestOwnerRewards.Values)
			{
				ItemReward itemReward = reward.itemReward;
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroProspectQuestRewardDetailLog(Guid.NewGuid(), logId, itemReward.item.id, itemReward.owned, itemReward.count));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
public class OwnerProspectQuestRewardReceiveAllCommandHandler : InGameCommandHandler<OwnerProspectQuestRewardReceiveAllCommandBody, OwnerProspectQuestRewardReceiveAllResponseBody>
{
	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private List<HeroProspectQuest> m_receivedQuests = new List<HeroProspectQuest>();

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		HeroProspectQuest[] array = m_myHero.ownerProspectQuests.Values.ToArray();
		foreach (HeroProspectQuest quest in array)
		{
			ReceiveReward(quest);
		}
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		if (m_receivedQuests.Count > 0)
		{
			SaveToDB();
			SaveToGameLogDB();
		}
		OwnerProspectQuestRewardReceiveAllResponseBody resBody = new OwnerProspectQuestRewardReceiveAllResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.receivedInstanceIds = (Guid[])(object)GetReceivedInstanceIds().ToArray();
		SendResponseOK(resBody);
	}

	private void ReceiveReward(HeroProspectQuest quest)
	{
		if (!quest.isCompleted)
		{
			return;
		}
		foreach (ProspectQuestOwnerReward reward in quest.blessingTargetLevel.prospectQuestOwnerRewards.Values)
		{
			ItemReward itemReward = reward.itemReward;
			int nRemainingCount = m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_28", "MAIL_REWARD_D_28", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingCount, itemReward.owned));
			}
		}
		quest.ownerRewarded = true;
		m_myHero.RemoveOwnerProspectQuest(quest.instanceId);
		if (quest.isAllRewarded)
		{
			Cache.instance.RemoveProspectQuest(quest.instanceId);
		}
		m_receivedQuests.Add(quest);
	}

	private List<Guid> GetReceivedInstanceIds()
	{
		List<Guid> results = new List<Guid>();
		foreach (HeroProspectQuest quest in m_receivedQuests)
		{
			results.Add(quest.instanceId);
		}
		return results;
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateContentWork(QueuingWorkContentId.GameDB_ProspectQuest));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		foreach (HeroProspectQuest quest in m_receivedQuests)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroProspectQuest_OwnerRewarded(quest.instanceId, quest.ownerRewarded));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			foreach (HeroProspectQuest quest in m_receivedQuests)
			{
				Guid logId = Guid.NewGuid();
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroProspectQuestRewardLog(logId, quest.instanceId, m_myHero.id, bIsOwner: true, m_currentTime));
				foreach (ProspectQuestOwnerReward reward in quest.blessingTargetLevel.prospectQuestOwnerRewards.Values)
				{
					ItemReward itemReward = reward.itemReward;
					logWork.AddSqlCommand(GameLogDac.CSC_AddHeroProspectQuestRewardDetailLog(Guid.NewGuid(), logId, itemReward.item.id, itemReward.owned, itemReward.count));
				}
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
