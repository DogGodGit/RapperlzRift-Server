using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildDailyRewardReceiveCommandHandler : InGameCommandHandler<GuildDailyRewardReceiveCommandBody, GuildDailyRewardReceiveResponseBody>
{
	public const short kResult_NoGuildMemeber = 101;

	public const short kResult_AlreadyReceived = 102;

	private GuildMember m_myGuildMember;

	private ItemReward m_reawrdItem;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime curretDate = m_currentTime.Date;
		m_myGuildMember = m_myHero.guildMember;
		if (m_myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		if (m_myHero.guildDailyRewardReceivedDate == curretDate)
		{
			throw new CommandHandleException(102, "이미 보상을 받았습니다.");
		}
		int nGuildLevel = m_myGuildMember.guild.level;
		GuildLevel guildLevel = Resource.instance.GetGuildLevel(nGuildLevel);
		m_reawrdItem = guildLevel.dailyItemReward;
		if (m_reawrdItem != null)
		{
			int nRemainingCount = m_myHero.AddItem(m_reawrdItem.item, m_reawrdItem.owned, m_reawrdItem.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				m_mail = Mail.Create("MAIL_NAME_00001", "MAIL_DESC_00001", m_currentTime);
				m_mail.AddAttachmentWithNo(new MailAttachment(m_reawrdItem.item, nRemainingCount, m_reawrdItem.owned));
				m_myHero.AddMail(m_mail, bSendEvent: true);
			}
		}
		m_myHero.guildDailyRewardReceivedDate = curretDate;
		SaveToDB();
		SaveToGameLogDB();
		GuildDailyRewardReceiveResponseBody resBody = new GuildDailyRewardReceiveResponseBody();
		resBody.date = (DateTime)m_myHero.guildDailyRewardReceivedDate;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildDailyRewardReceivedDate(m_myHero.id, m_myHero.guildDailyRewardReceivedDate));
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

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			int nRewardItemId = 0;
			int nRewardItemCount = 0;
			bool bRewardItemOwned = false;
			if (m_reawrdItem != null)
			{
				nRewardItemId = m_reawrdItem.item.id;
				nRewardItemCount = m_reawrdItem.count;
				bRewardItemOwned = m_reawrdItem.owned;
			}
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildDailyItemRewardLog(Guid.NewGuid(), m_myGuildMember.guild.id, m_myHero.id, nRewardItemId, nRewardItemCount, bRewardItemOwned, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
