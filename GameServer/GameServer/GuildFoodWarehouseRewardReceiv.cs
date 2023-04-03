using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GuildFoodWarehouseRewardReceiveCommandHandler : InGameCommandHandler<GuildFoodWarehouseRewardReceiveCommandBody, GuildFoodWarehouseRewardReceiveResponseBody>
{
	public const short kResult_NoGuildMember = 101;

	public const short kResult_CollectionNotExist = 102;

	public const short kResult_AlreadyReceived = 103;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private Guild m_guild;

	private Guid m_targetCollectionId = Guid.Empty;

	private ItemReward m_itemReward;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		GuildMember guildMember = m_myHero.guildMember;
		if (guildMember == null)
		{
			throw new CommandHandleException(101, "길드에 가입되어있지 않습니다.");
		}
		m_guild = guildMember.guild;
		m_targetCollectionId = m_guild.foodWarehouseCollectionId;
		if (m_targetCollectionId == Guid.Empty)
		{
			throw new CommandHandleException(102, "군량창고를 징수하지 않았습니다.");
		}
		if (m_myHero.receivedGuildFoodWarehouseCollectionId == m_targetCollectionId)
		{
			throw new CommandHandleException(103, "이미 보상을 받았습니다.");
		}
		GuildFoodWarehouse warehouse = Resource.instance.guildFoodWarehouse;
		m_itemReward = warehouse.fullLevelItemReward;
		if (m_itemReward != null)
		{
			int nRemainingCount = m_myHero.AddItem(m_itemReward.item, m_itemReward.owned, m_itemReward.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				m_mail = Mail.Create("MAIL_NAME_00008", "MAIL_DESC_00008", m_currentTime);
				m_mail.AddAttachmentWithNo(new MailAttachment(m_itemReward.item, nRemainingCount, m_itemReward.owned));
				m_myHero.AddMail(m_mail, bSendEvent: true);
			}
		}
		m_myHero.receivedGuildFoodWarehouseCollectionId = m_targetCollectionId;
		SaveToDB();
		SaveToGameLogDB();
		GuildFoodWarehouseRewardReceiveResponseBody resBody = new GuildFoodWarehouseRewardReceiveResponseBody();
		resBody.receivedCollectionId = (Guid)m_targetCollectionId;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_GuildFoodWarehouseReward(m_myHero.id, m_targetCollectionId));
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
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildFoodWarehouseRewardLog(Guid.NewGuid(), m_targetCollectionId, m_guild.id, m_myHero.id, nItemId, nItemCount, bItemOwned, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
