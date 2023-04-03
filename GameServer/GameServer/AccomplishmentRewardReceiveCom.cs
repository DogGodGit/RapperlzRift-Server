using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class AccomplishmentRewardReceiveCommandHandler : InGameCommandHandler<AccomplishmentRewardReceiveCommandBody, AccomplishmentRewardReceiveResponseBody>
{
	public const short kResult_ObectiveNotCompleted = 101;

	private HashSet<InventorySlot> m_changedInvetorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nAccomplishmentId = m_body.accomplishmentId;
		if (nAccomplishmentId <= 0)
		{
			throw new CommandHandleException(1, "업적ID가 유효하지 않습니다. nAccomplishmentId = " + nAccomplishmentId);
		}
		if (m_myHero.IsRewardedAccomplishment(nAccomplishmentId))
		{
			throw new CommandHandleException(1, "이미 보상받은 업적입니다. nAccomplishmentId = " + nAccomplishmentId);
		}
		Accomplishment accomplishment = Resource.instance.GetAccomplishment(nAccomplishmentId);
		if (accomplishment == null)
		{
			throw new CommandHandleException(1, "존재하지 않은 업적입니다. nAccomplishmentId = " + nAccomplishmentId);
		}
		if (!m_myHero.IsCompletedAccomplishmentObjective(accomplishment.type, accomplishment.objectiveValue))
		{
			throw new CommandHandleException(101, "업적의 목표가 완료되지 않았습니다.");
		}
		ItemReward rewardItem = accomplishment.rewardItem;
		int nRemainingCount = m_myHero.AddItem(rewardItem.item, rewardItem.owned, rewardItem.count, m_changedInvetorySlots);
		if (nRemainingCount > 0)
		{
			m_mail = Mail.Create("MAIL_NAME_00010", "MAIL_DESC_00010", m_currentTime);
			m_mail.AddAttachmentWithNo(new MailAttachment(rewardItem.item, nRemainingCount, rewardItem.owned));
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		m_myHero.AddRewardedAccomplishment(nAccomplishmentId);
		m_myHero.accomplishmentPoint += accomplishment.point;
		SaveToDB(nAccomplishmentId);
		SaveToGameLogDB(nAccomplishmentId, rewardItem);
		AccomplishmentRewardReceiveResponseBody resBody = new AccomplishmentRewardReceiveResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInvetorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nAccomplishmentId)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroAccomplishmentReward(m_myHero.id, nAccomplishmentId, m_currentTime));
		foreach (InventorySlot slot in m_changedInvetorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nAccomplishmentId, ItemReward itemReward)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroAccomplishmentRewardLog(Guid.NewGuid(), m_myHero.id, nAccomplishmentId, itemReward.item.id, itemReward.count, itemReward.owned, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
