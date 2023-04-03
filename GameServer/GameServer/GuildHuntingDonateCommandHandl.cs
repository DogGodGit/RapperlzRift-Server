using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildHuntingDonateCommandHandler : InGameCommandHandler<GuildHuntingDonateCommandBody, GuildHuntingDonateResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_DailyDonationCountOverflowed = 102;

	public const short kResult_AlreadyDonated = 103;

	public const short kResult_NotEnounghItem = 104;

	private Guild m_myGuild;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private int m_nUsedOwnCount;

	private int m_nUsedUnOwnCount;

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currnetDate = m_currentTime.Date;
		GuildMember myGuildMember = m_myHero.guildMember;
		if (myGuildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		m_myGuild = myGuildMember.guild;
		m_myGuild.RefreshDailyHuntingDonationCount(currnetDate);
		DateValuePair<int> dailyGuildHuntingDonationCount = m_myGuild.dailyHuntingDonationCount;
		if (dailyGuildHuntingDonationCount.value >= Resource.instance.guildHuntingDonationMaxCount)
		{
			throw new CommandHandleException(102, "최대횟수가 일일횟수를 넘어갑니다.");
		}
		if (m_myHero.guildHuntingDonationDate == currnetDate)
		{
			throw new CommandHandleException(103, "이미 길드헌팅기부를 했습니다.");
		}
		Item guildHuntingDonationItem = Resource.instance.guildHuntingDonationItem;
		if (m_myHero.GetItemCount(guildHuntingDonationItem.id) < 1)
		{
			throw new CommandHandleException(104, "아이템이 부족합니다.");
		}
		m_myHero.UseItem(guildHuntingDonationItem.id, bFisetUseOwn: true, 1, m_changedInventorySlots, out m_nUsedOwnCount, out m_nUsedUnOwnCount);
		ItemReward itemReward = Resource.instance.guildHuntingDonationReward;
		int nRemainingCount = m_myHero.AddItem(itemReward.item, itemReward.owned, itemReward.count, m_changedInventorySlots);
		if (nRemainingCount > 0)
		{
			m_mail = Mail.Create("MAIL_NAME_00004", "MAIL_DESC_00004", m_currentTime);
			m_mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingCount, itemReward.owned));
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		dailyGuildHuntingDonationCount.value++;
		ServerEvent.SendGuildHuntingDonationCountUpdated(m_myGuild.GetClientPeers(m_myHero.id), dailyGuildHuntingDonationCount.value);
		m_myHero.guildHuntingDonationDate = currnetDate;
		SaveToDB(dailyGuildHuntingDonationCount);
		SaveToGameLogDB(guildHuntingDonationItem.id, itemReward);
		GuildHuntingDonateResponseBody resBody = new GuildHuntingDonateResponseBody();
		resBody.date = (DateTime)currnetDate;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.guildHuntingDonationCount = dailyGuildHuntingDonationCount.value;
		SendResponseOK(resBody);
	}

	private void SaveToDB(DateValuePair<int> dailyGuildHuntingDonationCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateGuildWork(m_myGuild.id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_HuntingDonationDateCount(m_myGuild.id, dailyGuildHuntingDonationCount.date, dailyGuildHuntingDonationCount.value));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_HuntingDonationDate(m_myHero.id, m_currentTime.Date));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nUsedItemId, ItemReward itemReward)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildHuntingDonationLog(Guid.NewGuid(), m_myGuild.id, m_myHero.id, nUsedItemId, m_nUsedOwnCount, m_nUsedUnOwnCount, itemReward.item.id, itemReward.count, itemReward.owned, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
