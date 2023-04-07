using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class LevelUpRewardReceiveCommandHnadler : InGameCommandHandler<LevelUpRewardReceiveCommandBody, LevelUpRewardReceiveResponseBody>
{
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
		int nEntryId = m_body.entryId;
		if (nEntryId <= 0)
		{
			throw new CommandHandleException(1, "항목 ID가 유효하지 않습니다.");
		}
		LevelUpRewardEntry levelUpRewardEntry = Resource.instance.GetLevelUpRewardEntry(nEntryId);
		if (levelUpRewardEntry == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 레벨업 보상 항목입니다. nEntryId = " + nEntryId);
		}
		if (levelUpRewardEntry.level > m_myHero.level)
		{
			throw new CommandHandleException(1, "레벨업 보상을 받을 수 있는 영웅레벨이 아닙니다. nEntryId = " + nEntryId);
		}
		if (m_myHero.IsLevelUpRewardReceived(levelUpRewardEntry.id))
		{
			throw new CommandHandleException(1, "이미 받은 레벨업 보상입니다. nEntryId = " + nEntryId);
		}
		ResultItemCollection resultItemCollection = new ResultItemCollection();
		foreach (LevelUpRewardItem entryRewardItem in levelUpRewardEntry.rewardItems)
		{
			ItemReward rewardItem2 = entryRewardItem.itemReward;
			resultItemCollection.AddResultItemCount(rewardItem2.item, rewardItem2.owned, rewardItem2.count);
		}
		foreach (ResultItem rewardItem in resultItemCollection.resultItems)
		{
			int nRemainingCount = m_myHero.AddItem(rewardItem.item, rewardItem.owned, rewardItem.count, m_changedInventorySlots);
			if (nRemainingCount > 0)
			{
				if (m_mail == null)
				{
					m_mail = Mail.Create("MAIL_REWARD_N_10", "MAIL_REWARD_D_10", m_currentTime);
				}
				m_mail.AddAttachmentWithNo(new MailAttachment(rewardItem.item, nRemainingCount, rewardItem.owned));
			}
		}
		m_myHero.AddLevelUpReward(levelUpRewardEntry);
		if (m_mail != null)
		{
			m_myHero.AddMail(m_mail, bSendEvent: true);
		}
		SaveToDB(levelUpRewardEntry.id, resultItemCollection);
		LevelUpRewardReceiveResponseBody resBody = new LevelUpRewardReceiveResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nRewardEntryId, ResultItemCollection resultItemCollection)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroLevelUpReward(m_myHero.id, nRewardEntryId));
		if (m_mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(m_mail));
		}
		dbWork.Schedule();
		SaveToDB_AddLevelUpRewardLog(nRewardEntryId, resultItemCollection);
	}

	private void SaveToDB_AddLevelUpRewardLog(int nRewardEntryId, ResultItemCollection resultItemCollection)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddLevelUpRewardLog(logId, m_myHero.id, m_myHero.level, nRewardEntryId, m_currentTime));
			foreach (ResultItem result in resultItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddLevelUpRewardDetailLog(Guid.NewGuid(), logId, result.item.id, result.count, result.owned));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
