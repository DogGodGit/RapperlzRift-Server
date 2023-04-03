using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildDailyObjectiveRewardReceiveCommandHandler : InGameCommandHandler<GuildDailyObjectiveRewardReceiveCommandBody, GuildDailyObjectiveRewardReceiveResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NotEnoughCompletionMemberCount = 102;

	private GuildMember m_myGuildMember;

	private Guild m_myGuild;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어 있지 않습니다.");
		}
		m_myGuild = m_myGuildMember.guild;
		m_myHero.RefreshGuildDailyObjectiveReceivedReward(currentDate);
		int nRewardNo = m_myHero.receivedGuildDailyObjectiveRewardNo.value + 1;
		GuildDailyObjectiveReward dailyObjectiveReward = Resource.instance.GetGuildDailyObjectiveReward(nRewardNo);
		if (dailyObjectiveReward == null)
		{
			throw new CommandHandleException(1, "오늘의목표일일보상이 존재하지 않습니다. nRewardNo = " + nRewardNo);
		}
		if (dailyObjectiveReward.completionMemberCount > m_myGuild.dailyObjectiveCompletionMembers.Count)
		{
			throw new CommandHandleException(102, "완료 멤버수가 부족합니다.");
		}
		ItemReward itemReward1 = dailyObjectiveReward.itemReward1;
		ItemReward itemReward2 = dailyObjectiveReward.itemReward2;
		ItemReward itemReward3 = dailyObjectiveReward.itemReward3;
		AddItem(itemReward1);
		AddItem(itemReward2);
		AddItem(itemReward3);
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		m_myHero.receivedGuildDailyObjectiveRewardNo.value = nRewardNo;
		SaveToDB();
		SaveToGameLogDB(nRewardNo, itemReward1, itemReward2, itemReward3);
		GuildDailyObjectiveRewardReceiveResponseBody resBody = new GuildDailyObjectiveRewardReceiveResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.rewardNo = nRewardNo;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void AddItem(ItemReward itemReward)
	{
		if (itemReward == null)
		{
			return;
		}
		int nRemainingCount = m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
		if (nRemainingCount > 0)
		{
			if (m_mail == null)
			{
				m_mail = Mail.Create("MAIL_NAME_00002", "MAIL_DESC_00002", m_currentTime);
			}
			m_mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingCount, itemReward.owned));
		}
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildDailyObjectiveReward(m_myHero.id, m_myHero.receivedGuildDailyObjectiveRewardNo.date, m_myHero.receivedGuildDailyObjectiveRewardNo.value));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nRewardNo, ItemReward itemReward1, ItemReward itemReward2, ItemReward itemReward3)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildDailyObjectiveRewardLog(logId, m_myGuild.id, m_currentTime.Date, m_myHero.id, nRewardNo, m_myGuild.dailyObjectiveCompletionMembers.Count, m_currentTime));
			if (itemReward1 != null)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddGuildDailyObjectiveRewardDetailLog(Guid.NewGuid(), logId, itemReward1.item.id, itemReward1.count, itemReward1.owned));
			}
			if (itemReward2 != null)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddGuildDailyObjectiveRewardDetailLog(Guid.NewGuid(), logId, itemReward2.item.id, itemReward2.count, itemReward2.owned));
			}
			if (itemReward3 != null)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddGuildDailyObjectiveRewardDetailLog(Guid.NewGuid(), logId, itemReward3.item.id, itemReward3.count, itemReward3.owned));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
