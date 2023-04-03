using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class VipLevelRewardReceiveCommandHandler : InGameCommandHandler<VipLevelRewardReceiveCommandBody, VipLevelRewardReceiveResponseBody>
{
	public const short kResult_NotEnoughVipLevel = 101;

	public const short kResult_AlreadyReceivedVipLevelReward = 102;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nVipLevel = m_body.vipLevel;
		if (nVipLevel <= 0)
		{
			throw new CommandHandleException(1, "유효하지 않는 vipLevel입니다. nVipLevel = " + nVipLevel);
		}
		VipLevel targetVipLevel = Resource.instance.GetVipLevel(nVipLevel);
		if (targetVipLevel == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 VIP레벨입니다. nVipLevel");
		}
		if (nVipLevel > m_myHero.vipLevel.level)
		{
			throw new CommandHandleException(101, "영웅 VIP레벨이 부족합니다. nVipLevel = " + nVipLevel);
		}
		if (m_myAccount.IsVipLevelRewardRecieved(nVipLevel))
		{
			throw new CommandHandleException(102, "이미 보상받은 VIP레벨보상입니다. nVipLevel = " + nVipLevel);
		}
		ResultItemCollection resultItemCollection = new ResultItemCollection();
		foreach (VipLevelReward reward in targetVipLevel.rewards)
		{
			ItemReward itemReward = reward.itemReward;
			resultItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
		}
		foreach (ResultItem result in resultItemCollection.resultItems)
		{
			int nRemainingCount = m_myHero.AddItem(result.item, result.owned, result.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_14", "MAIL_REWARD_D_14", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(result.item, nRemainingCount, result.owned));
			}
		}
		m_myAccount.AddReceivedVipLevelReward(targetVipLevel.level);
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB(targetVipLevel.level);
		SaveToDB_AddVipLevelRewardLog(targetVipLevel.level, resultItemCollection);
		VipLevelRewardReceiveResponseBody resBody = new VipLevelRewardReceiveResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nVipLevel)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		dbWork.AddSqlCommand(GameDac.CSC_AddVipLevelReward(m_myAccount.id, nVipLevel, m_myHero.id));
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

	private void SaveToDB_AddVipLevelRewardLog(int nVipLevel, ResultItemCollection resultItemCollection)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddVipLevelRewardLog(logId, m_myAccount.id, m_myHero.id, nVipLevel, m_currentTime));
			foreach (ResultItem result in resultItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddVipLevelRewardDetailLog(Guid.NewGuid(), logId, result.item.id, result.count, result.owned));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
