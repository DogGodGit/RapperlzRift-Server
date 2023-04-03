using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildWeeklyObjectiveRewardReceiveCommandHandler : InGameCommandHandler<GuildWeeklyObjectiveRewardReceiveCommandBody, GuildWeeklyObjectiveRewardReceiveResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_AlreadyReceivedReward = 102;

	public const short kResult_WeekyObjectiveNotSet = 103;

	public const short kResult_NotEnoughCompletedMemberCount = 104;

	private GuildMember m_myGuildMember;

	private Guild m_myGuild;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private List<Mail> m_mails = new List<Mail>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어 있지 않습니다.");
		}
		m_myGuild = m_myGuildMember.guild;
		DateTime currentDateOfMonday = DateTimeUtil.GetWeekStartDate(m_currentDate);
		if (currentDateOfMonday == m_myHero.guildWeeklyObjectiveRewardReceivedDate)
		{
			throw new CommandHandleException(102, "이미 받은 보상입니다.");
		}
		int nObjectiveId = m_myGuild.weeklyObjectiveId;
		if (nObjectiveId <= 0)
		{
			throw new CommandHandleException(103, "주간목표가 설정되지 않았습니다. nObjectiveId = " + nObjectiveId);
		}
		GuildWeeklyObjective weeklyObjective = Resource.instance.GetGuildWeeklyObjective(nObjectiveId);
		if (weeklyObjective.completionMemberCount > m_myGuild.weeklyObjectiveCompletionMemberCount)
		{
			throw new CommandHandleException(104, "완료 멤버수가 부족합니다.");
		}
		ItemReward itemReward1 = weeklyObjective.itemReward1;
		ItemReward itemReward2 = weeklyObjective.itemReward2;
		ItemReward itemReward3 = weeklyObjective.itemReward3;
		AddItem(itemReward1);
		AddItem(itemReward2);
		AddItem(itemReward3);
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		m_myHero.guildWeeklyObjectiveRewardReceivedDate = currentDateOfMonday;
		SaveToDB();
		SaveToGameLogDB(nObjectiveId, itemReward1, itemReward2, itemReward3);
		GuildWeeklyObjectiveRewardReceiveResponseBody resBody = new GuildWeeklyObjectiveRewardReceiveResponseBody();
		resBody.rewardReceivedDate = (DateTime)currentDateOfMonday;
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
				m_mail = Mail.Create("MAIL_NAME_00003", "MAIL_DESC_00003", m_currentTime);
			}
			m_mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingCount, itemReward.owned));
		}
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildWeeklyObjectiveRewardReceivedDate(m_myHero.id, m_myHero.guildWeeklyObjectiveRewardReceivedDate));
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

	private void SaveToGameLogDB(int nObjectiveId, ItemReward itemReward1, ItemReward itemReward2, ItemReward itemReward3)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildWeeklyObjectiveRewardLog(logId, m_myGuild.id, m_myHero.id, nObjectiveId, m_myGuild.dailyObjectiveCompletionMembers.Count, m_currentTime));
			if (itemReward1 != null)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddGuildWeeklyObjectiveRewardDetailLog(Guid.NewGuid(), logId, itemReward1.item.id, itemReward1.count, itemReward1.owned));
			}
			if (itemReward2 != null)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddGuildWeeklyObjectiveRewardDetailLog(Guid.NewGuid(), logId, itemReward2.item.id, itemReward2.count, itemReward2.owned));
			}
			if (itemReward3 != null)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddGuildWeeklyObjectiveRewardDetailLog(Guid.NewGuid(), logId, itemReward3.item.id, itemReward3.count, itemReward3.owned));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
