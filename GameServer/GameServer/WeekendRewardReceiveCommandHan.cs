using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class WeekendRewardReceiveCommandHandler : InGameCommandHandler<WeekendRewardReceiveCommandBody, WeekendRewardReceiveResponseBody>
{
	public const short kResult_NotRewardDayOfWeek = 101;

	public const short kResult_AlreadyRewardReceived = 102;

	private HeroWeekendReward m_heroWeekendReward;

	private ItemReward m_itemReward;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		if (m_currentDate.DayOfWeek != DayOfWeek.Monday)
		{
			throw new CommandHandleException(101, "주말보상을 받을 수 있는 요일이 아닙니다.");
		}
		m_myHero.RefreshWeekendReward(m_currentDate);
		m_heroWeekendReward = m_myHero.weekendReward;
		if (!m_heroWeekendReward.isAnySelected)
		{
			throw new CommandHandleException(1, "보상을 받기위해선 하나라도 선택해야됩니다.");
		}
		if (m_heroWeekendReward.rewarded)
		{
			throw new CommandHandleException(102, "이미 주말보상을 받았습니다.");
		}
		int nRewardOwnDia = m_heroWeekendReward.CalculateResultValue();
		m_myHero.AddOwnDia(nRewardOwnDia);
		WeekendReward weekendReward = Resource.instance.weekendReward;
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
		m_heroWeekendReward.rewarded = true;
		SaveToDB();
		SaveToLogDB(nRewardOwnDia);
		WeekendRewardReceiveResponseBody resBody = new WeekendRewardReceiveResponseBody();
		resBody.ownDia = m_myHero.ownDia;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroWeekendReward_Rewarded(m_heroWeekendReward.hero.id));
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

	private void SaveToLogDB(int nRewardOwnDia)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeekendRewardLog(Guid.NewGuid(), m_myHero.id, m_heroWeekendReward.weekStartDate, m_heroWeekendReward.selection1, m_heroWeekendReward.selection2, m_heroWeekendReward.selection3, nRewardOwnDia, m_currentTime));
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
