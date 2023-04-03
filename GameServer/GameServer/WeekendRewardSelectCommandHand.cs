using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class WeekendRewardSelectCommandHandler : InGameCommandHandler<WeekendRewardSelectCommandBody, WeekendRewardSelectResponseBody>
{
	public const short kResult_NotEnoughHeroLevel = 101;

	public const short kResult_NotSelectableSelectionNo = 102;

	public const short kResult_AlreadySelected = 103;

	private HeroWeekendReward m_heroWeekendReward;

	private ItemReward m_itemReward;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTimeOffset.MinValue.Date;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		DayOfWeek currentDayOfWeek = m_currentDate.DayOfWeek;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nSelectionNo = m_body.selectionNo;
		if (nSelectionNo < 0)
		{
			throw new CommandHandleException(1, "선택번호가 유효하지 않습니다. nSelectionNo = " + nSelectionNo);
		}
		WeekendReward weekendReward = Resource.instance.weekendReward;
		if (m_myHero.level < weekendReward.requiredHeroLevel)
		{
			throw new CommandHandleException(101, "레벨이 부족합니다.");
		}
		if (WeekendReward.GetSelectionNo(currentDayOfWeek) != nSelectionNo)
		{
			throw new CommandHandleException(102, "선택할 수 없는 선택번호입니다. nSelectionNo = " + nSelectionNo);
		}
		WeekendRewardNumberPool weekendRewardNumberPool = weekendReward.GetNumberPool(nSelectionNo);
		if (weekendRewardNumberPool == null)
		{
			throw new CommandHandleException(1, "넘버풀이 존재하지 않습니다. nSelectionNo = " + nSelectionNo);
		}
		m_myHero.RefreshWeekendReward(m_currentDate);
		m_heroWeekendReward = m_myHero.weekendReward;
		if (m_heroWeekendReward.IsSelected(nSelectionNo))
		{
			throw new CommandHandleException(1, "이미 선택을 했습니다.");
		}
		WeekendRewardNumberPoolEntry numberPoolEntry = weekendRewardNumberPool.SelectEntry();
		int nSelectedNumber = numberPoolEntry.number;
		m_heroWeekendReward.SetSelection(nSelectionNo, nSelectedNumber);
		if (Util.DrawLots(weekendReward.itemRewardRate))
		{
			m_itemReward = weekendReward.itemReward;
			if (m_itemReward != null)
			{
				int nRemainingCount = m_myHero.AddItem(m_itemReward.item, m_itemReward.owned, m_itemReward.count, m_changedInventorySlots);
				if (nRemainingCount > 0)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_23", "MAIL_REWARD_D_23", m_currentTime);
					m_mail.AddAttachmentWithNo(new MailAttachment(m_itemReward.item, nRemainingCount, m_itemReward.owned));
					m_myHero.AddMail(m_mail, bSendEvent: true);
				}
			}
		}
		SaveToDB();
		SaveToLogDB();
		WeekendRewardSelectResponseBody resBody = new WeekendRewardSelectResponseBody();
		resBody.selectedNumber = nSelectedNumber;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroWeekendReward(m_heroWeekendReward.hero.id, m_heroWeekendReward.weekStartDate, m_heroWeekendReward.selection1, m_heroWeekendReward.selection2, m_heroWeekendReward.selection3));
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

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			if (m_itemReward != null)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeekendRewardItemRewardLog(Guid.NewGuid(), m_myHero.id, m_itemReward.item.id, m_itemReward.owned, m_itemReward.count, m_currentTime));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
