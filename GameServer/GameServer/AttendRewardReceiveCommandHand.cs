using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class AttendRewardReceiveCommandHandler : InGameCommandHandler<AttendRewardReceiveCommandBody, AttendRewardReceiveResponseBody>
{
	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

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
		int nAttendDay = m_body.attendDay;
		if (nAttendDay <= 0)
		{
			throw new CommandHandleException(1, "출석 일차가 유효하지 않습니다. nAttendDay = " + nAttendDay);
		}
		DateValuePair<int> heroDailyAttendRewardDay = m_myHero.dailyAttendReawrdDay;
		if (heroDailyAttendRewardDay.date >= currentDate)
		{
			throw new CommandHandleException(1, "이미 출석보상을 받았습니다.");
		}
		int nDailyAttendRewardDay = heroDailyAttendRewardDay.value;
		nDailyAttendRewardDay = nDailyAttendRewardDay % Resource.instance.dailyAttendRewardDayCount + 1;
		if (nDailyAttendRewardDay != nAttendDay)
		{
			throw new CommandHandleException(1, "받아야될 출석 일차가 아닙니다.");
		}
		DailyAttendRewardEntry dailyattendRewardEntry = Resource.instance.GetDailyAttendRewardEntry(nDailyAttendRewardDay);
		if (dailyattendRewardEntry == null)
		{
			throw new CommandHandleException(1, "출석보상이 존재하지 않습니다. nDailyAttendRewardDay = " + nDailyAttendRewardDay);
		}
		ItemReward dailyRewardItem = dailyattendRewardEntry.itemReward;
		int nDailyAttendRewardItemRemainingCount = m_myHero.AddItem(dailyRewardItem.item, dailyRewardItem.owned, dailyRewardItem.count, m_changedInventorySlots);
		if (nDailyAttendRewardItemRemainingCount > 0)
		{
			m_mail = Mail.Create("MAIL_REWARD_N_9", "MAIL_REWARD_D_9", m_currentTime);
			m_mail.AddAttachmentWithNo(new MailAttachment(dailyRewardItem.item, nDailyAttendRewardItemRemainingCount, dailyRewardItem.owned));
		}
		int nWeekendRewardItemId = 0;
		int nWeekendRewardItemCount = 0;
		bool bWeekednRewardItemOwned = false;
		if (Resource.instance.IsWeekendAttendRewardDayOfWeek(currentDate.DayOfWeek))
		{
			long weekendRewardItemId = Resource.instance.weekendAttendItemRewardId;
			ItemReward weekendRewardItem = Resource.instance.GetItemReward(weekendRewardItemId);
			int nWeekenedRewardItemRemainingCount = m_myHero.AddItem(weekendRewardItem.item, weekendRewardItem.owned, weekendRewardItem.count, m_changedInventorySlots);
			if (nWeekenedRewardItemRemainingCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_9", "MAIL_REWARD_D_9", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(weekendRewardItem.item, nWeekenedRewardItemRemainingCount, weekendRewardItem.owned));
			}
			nWeekendRewardItemId = weekendRewardItem.item.id;
			nWeekendRewardItemCount = weekendRewardItem.count - nWeekenedRewardItemRemainingCount;
			bWeekednRewardItemOwned = weekendRewardItem.owned;
		}
		heroDailyAttendRewardDay.date = currentDate;
		heroDailyAttendRewardDay.value = nDailyAttendRewardDay;
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB();
		SaveToDB_AddDailyAttendRewardLog(nDailyAttendRewardDay, dailyRewardItem, nDailyAttendRewardItemRemainingCount, nWeekendRewardItemId, nWeekendRewardItemCount, bWeekednRewardItemOwned);
		AttendRewardReceiveResponseBody resBody = new AttendRewardReceiveResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_DailyAttendDateCount(m_myHero.id, m_myHero.dailyAttendReawrdDay.date, m_myHero.dailyAttendReawrdDay.value));
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

	private void SaveToDB_AddDailyAttendRewardLog(int nDay, ItemReward dailyAttendReawrdItem, int nDailyRewardItemRemainingCount, int nWeekendRewardItemId, int nWeekendRewardItemCount, bool bWeekendRewardItemOwned)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddDailyAttendRewardLog(Guid.NewGuid(), m_myHero.id, nDay, dailyAttendReawrdItem.item.id, dailyAttendReawrdItem.count - nDailyRewardItemRemainingCount, dailyAttendReawrdItem.owned, nWeekendRewardItemId, nWeekendRewardItemCount, bWeekendRewardItemOwned, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
