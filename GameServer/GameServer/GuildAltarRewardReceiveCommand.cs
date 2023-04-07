using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class GuildAltarRewardReceiveCommandHandler : InGameCommandHandler<GuildAltarRewardReceiveCommandBody, GuildAltarRewardReceiveResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_NotEnoughGuildMoralPoint = 102;

	public const short kResult_AlreadyReceived = 103;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTimeOffset.MinValue.Date;

	private Guild m_guild;

	private GuildLevel m_guildLevel;

	private ItemReward m_itemReward;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		GuildMember guildMember = m_myHero.guildMember;
		if (guildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		m_guild = guildMember.guild;
		m_guildLevel = Resource.instance.GetGuildLevel(m_guild.level);
		m_guild.RefreshMoralPoint(m_currentDate);
		if (!m_guild.isMoralPointMax)
		{
			throw new CommandHandleException(102, "길드의 모럴포인트가 부족합니다.");
		}
		if (m_myHero.guildAltarRewardReceivedDate == m_currentDate)
		{
			throw new CommandHandleException(103, "이미 보상을 받았습니다.");
		}
		m_itemReward = m_guildLevel.altarItemReawrd;
		if (m_itemReward != null)
		{
			int nRemainingCount = m_myHero.AddItem(m_itemReward.item, m_itemReward.owned, m_itemReward.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				m_mail = Mail.Create("MAIL_NAME_00006", "MAIL_DESC_00006", m_currentTime);
				m_mail.AddAttachmentWithNo(new MailAttachment(m_itemReward.item, nRemainingCount, m_itemReward.owned));
				m_myHero.AddMail(m_mail, bSendEvent: true);
			}
		}
		m_myHero.guildAltarRewardReceivedDate = m_currentDate;
		SaveToDB();
		SaveToGameLogDB();
		GuildAltarRewardReceiveResponseBody resBody = new GuildAltarRewardReceiveResponseBody();
		resBody.date = (DateTime)m_currentDate;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildAltarReward(m_myHero.id, m_currentDate));
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
			int nItemId = 0;
			int nItemCount = 0;
			bool bItemOwned = false;
			if (m_itemReward != null)
			{
				nItemId = m_itemReward.item.id;
				nItemCount = m_itemReward.count;
				bItemOwned = m_itemReward.owned;
			}
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildAltarRewardLog(Guid.NewGuid(), m_guild.id, m_myHero.id, m_guild.moralPoint, m_guild.level, nItemId, nItemCount, bItemOwned, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
