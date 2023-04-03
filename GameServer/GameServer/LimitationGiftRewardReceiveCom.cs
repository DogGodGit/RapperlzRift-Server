using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class LimitationGiftRewardReceiveCommandHandler : InGameCommandHandler<LimitationGiftRewardReceiveCommandBody, LimitationGiftRewardReceiveResponseBody>
{
	public const short kResult_NotReceivableDay = 101;

	public const short kResult_NotEnoughHeroLevel = 102;

	public const short kResult_AlreadyReceivedGift = 103;

	public const short kResult_NotReceivableTime = 104;

	public const short kResult_NotEnoughInventory = 105;

	private ResultItemCollection m_resultItemCollection;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	private float m_fCurrentTimeOfDay;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		m_fCurrentTimeOfDay = (float)m_currentTime.TimeOfDay.TotalSeconds;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nScheduleId = m_body.scheduleId;
		if (nScheduleId <= 0)
		{
			throw new CommandHandleException(1, "스케쥴ID가 유효하지 않습니다. nScheduleId = " + nScheduleId);
		}
		LimitationGift limitationGift = Resource.instance.limitationGift;
		if (!limitationGift.ContainsDayOfWeek(m_currentDate.DayOfWeek))
		{
			throw new CommandHandleException(101, "한정선물을 받을 수 있는 요일이 아닙니다.");
		}
		m_myHero.RefreshLimitationGiftRewards(m_currentDate);
		if (m_myHero.level < limitationGift.requiredHeroLevel)
		{
			throw new CommandHandleException(102, "레벨이 부족합니다.");
		}
		if (m_myHero.IsRewardedLimitationGiftSchedule(nScheduleId))
		{
			throw new CommandHandleException(103, "이미 받은 한정선물보상입니다.");
		}
		LimitationGiftRewardSchedule schedule = Resource.instance.limitationGift.GetSchedule(nScheduleId);
		if (schedule == null)
		{
			throw new CommandHandleException(1, "스케쥴이 존재하지 않습니다. nScheduleId = " + nScheduleId);
		}
		if (!schedule.IsScheduleTime(m_fCurrentTimeOfDay))
		{
			throw new CommandHandleException(104, "받을 수 있는 시간이 아닙니다.");
		}
		m_resultItemCollection = new ResultItemCollection();
		foreach (LimitationGiftReward reward in schedule.rewards)
		{
			ItemReward itemReward = reward.itemReward;
			if (itemReward != null)
			{
				m_resultItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
			}
		}
		if (!m_myHero.IsAvailableInventory(m_resultItemCollection))
		{
			throw new CommandHandleException(105, "인벤토리가 부족합니다.");
		}
		foreach (ResultItem resultItem in m_resultItemCollection.resultItems)
		{
			m_myHero.AddItem(resultItem.item, resultItem.owned, resultItem.count, m_changedInventorySlots);
		}
		m_myHero.ReceiveLimitationGiftReward(nScheduleId);
		SaveToDB(nScheduleId);
		SaveToLogDB(nScheduleId);
		LimitationGiftRewardReceiveResponseBody resBody = new LimitationGiftRewardReceiveResponseBody();
		resBody.date = (DateTime)m_currentDate;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nScheduleId)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroLimitationGiftReawrd(m_myHero.id, m_currentDate, nScheduleId));
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nScheduleId)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroLimitationGiftRewardLog(logId, m_myHero.id, nScheduleId, m_currentTime));
			foreach (ResultItem resultItem in m_resultItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroLimitationGiftRewardDetailLog(Guid.NewGuid(), logId, resultItem.item.id, resultItem.owned, resultItem.count));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
