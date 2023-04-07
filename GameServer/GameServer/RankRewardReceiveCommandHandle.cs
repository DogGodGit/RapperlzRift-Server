using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class RankRewardReceiveCommandHandler : InGameCommandHandler<RankRewardReceiveCommandBody, RankRewardReceiveResponseBody>
{
	public const short kResult_AlreadyReceived = 101;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	private Rank m_currentRank;

	private long m_lnRewardGold;

	private ResultItemCollection m_rewardItemCollection = new ResultItemCollection();

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private Mail m_mail;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		m_currentRank = m_myHero.rank;
		if (m_currentRank == null)
		{
			throw new CommandHandleException(1, "현재 계급이 없습니다.");
		}
		if (m_myHero.rankRewardReceivedDate == m_currentDate && m_myHero.rankRewardReceivedRankNo == m_currentRank.no)
		{
			throw new CommandHandleException(101, "이미 보상을 받았습니다.");
		}
		m_lnRewardGold = m_currentRank.goldRewardValue;
		m_myHero.AddGold(m_lnRewardGold);
		foreach (RankReward reward in m_currentRank.rewards.Values)
		{
			ItemReward itemReward = reward.itemReward;
			if (itemReward != null)
			{
				m_rewardItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
			}
		}
		foreach (ResultItem resultItem in m_rewardItemCollection.resultItems)
		{
			int nRemainingCount = m_myHero.AddItem(resultItem.item, resultItem.owned, resultItem.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_NAME_00013", "MAIL_DESC_00013", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(resultItem.item, nRemainingCount, resultItem.owned));
			}
		}
		m_myHero.rankRewardReceivedDate = m_currentDate;
		m_myHero.rankRewardReceivedRankNo = m_currentRank.no;
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB();
		SaveToGameLogDB();
		RankRewardReceiveResponseBody resBody = new RankRewardReceiveResponseBody();
		resBody.date = (DateTime)m_currentDate;
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_RankReward(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
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
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddRankRewardLog(logId, m_myHero.id, m_currentRank.no, m_lnRewardGold, m_currentTime));
			foreach (ResultItem result in m_rewardItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddRankRewardDetailLog(Guid.NewGuid(), logId, result.item.id, result.count, result.owned));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
