using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildHuntingDonationRewardReceiveCommandHandler : InGameCommandHandler<GuildHuntingDonationRewardReceiveCommandBody, GuildHuntingDonationRewardReceiveResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NotEnounghGuildDonationCount = 102;

	public const short kResult_AlreadReceivedReward = 103;

	private Guild m_myGuild;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		if (m_myHero.guildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		m_myGuild = m_myHero.guildMember.guild;
		m_myGuild.RefreshDailyHuntingDonationCount(currentDate);
		DateValuePair<int> dailyGuildHuntingDonationCount = m_myGuild.dailyHuntingDonationCount;
		if (dailyGuildHuntingDonationCount.value < Resource.instance.guildHuntingDonationMaxCount)
		{
			throw new CommandHandleException(102, "길드 기부횟수가 부족합니다.");
		}
		if (m_myHero.guildHuntingDonationCompletionRewardReceivedDate == currentDate)
		{
			throw new CommandHandleException(103, "이미 보상을 받았습니다.");
		}
		ItemReward itemReward = Resource.instance.guildHuntingDonationCompletionReward;
		int nRemainingCount = m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
		if (nRemainingCount > 0)
		{
			m_mail = Mail.Create("MAIL_NAME_00005", "MAIL_DESC_00005", m_currentTime);
			m_mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingCount, itemReward.owned));
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		m_myHero.guildHuntingDonationCompletionRewardReceivedDate = currentDate;
		SaveToDB();
		SaveToGameLogDB(itemReward);
		GuildHuntingDonationRewardReceiveResponseBody resBody = new GuildHuntingDonationRewardReceiveResponseBody();
		resBody.date = (DateTime)currentDate;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_HuntingDonationCompletionRewardRecivedDate(m_myHero.id, m_currentTime.Date));
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

	private void SaveToGameLogDB(ItemReward itemReward)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildHuntingDonationCompletionRewardLog(Guid.NewGuid(), m_myGuild.id, m_myHero.id, itemReward.item.id, itemReward.count, itemReward.owned, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
