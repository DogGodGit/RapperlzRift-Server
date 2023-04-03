using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class DailyAccessTimeRewardReceiveCommandHandler : InGameCommandHandler<DailyAccessTimeRewardReceiveCommandBody, DailyAccessTimeRewardReceiveResponseBody>
{
	public const short kResult_NotEnoughAccessTime = 101;

	private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nEntryId = m_body.entryId;
		if (nEntryId <= 0)
		{
			throw new CommandHandleException(1, "항목ID가 유효하지 않습니다. nEntryId = " + nEntryId);
		}
		AccessRewardEntry accessRewardEntry = Resource.instance.GetAccessRewardEntry(nEntryId);
		if (accessRewardEntry == null)
		{
			throw new CommandHandleException(1, "접속시간보상항목이 존재하지 않습니다. nEntryId = " + nEntryId);
		}
		m_myHero.RefreshDailyAccessTime(m_currentTime);
		float fDailyAccessTime = m_myHero.dailyAccessTime;
		if ((float)accessRewardEntry.accessTime > fDailyAccessTime)
		{
			throw new CommandHandleException(101, "아직 받을 수 있는 시간이 아닙니다.");
		}
		if (m_myHero.IsAccessRewardReceived(currentDate, accessRewardEntry.id))
		{
			throw new CommandHandleException(1, "이미 보상받은 접속시간보상항목입니다.");
		}
		ResultItemCollection resultItemCollection = new ResultItemCollection();
		foreach (AccessRewardItem entryReward in accessRewardEntry.rewardItems)
		{
			ItemReward rewardItem = entryReward.itemReward;
			resultItemCollection.AddResultItemCount(rewardItem.item, rewardItem.owned, rewardItem.count);
		}
		foreach (ResultItem result in resultItemCollection.resultItems)
		{
			int nRemainingItemCount = m_myHero.AddItem(result.item, result.owned, result.count, m_changedInventorySlots);
			if (nRemainingItemCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_8", "MAIL_REWARD_D_8", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(result.item, nRemainingItemCount, result.owned));
			}
		}
		HeroAccessReward heroAccessReward = new HeroAccessReward(m_myHero, currentDate, accessRewardEntry.id);
		m_myHero.AddAccessReward(heroAccessReward);
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB(heroAccessReward, fDailyAccessTime, resultItemCollection);
		DailyAccessTimeRewardReceiveResponseBody resBody = new DailyAccessTimeRewardReceiveResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroAccessReward heroAccessReward, float fDailyAccessTime, ResultItemCollection resultItemCollection)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroAccessReward(heroAccessReward.hero.id, heroAccessReward.date, heroAccessReward.id));
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
		SaveToDB_AddAccessRewardLog(fDailyAccessTime, heroAccessReward.id, resultItemCollection);
	}

	private void SaveToDB_AddAccessRewardLog(float fDailyAccessTime, int nEntryId, ResultItemCollection resultItemCollection)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddAccessRewardLog(logId, m_myHero.id, fDailyAccessTime, nEntryId, m_currentTime));
			foreach (ResultItem result in resultItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddAccessRewardDetailLog(Guid.NewGuid(), logId, result.item.id, result.count, result.owned));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
