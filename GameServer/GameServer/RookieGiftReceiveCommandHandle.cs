using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class RookieGiftReceiveCommandHandler : InGameCommandHandler<RookieGiftReceiveCommandBody, RookieGiftReceiveResponseBody>
{
	public const short kResult_NotElapsedWaitingTime = 101;

	public const short kResult_NotEnoughInventory = 102;

	private ResultItemCollection m_resultItemCollection;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		int nRookieGiftNo = m_myHero.rookieGiftNo;
		RookieGift rookieGift = Resource.instance.GetRookieGift(nRookieGiftNo);
		if (rookieGift == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 신병선물입니다. nRookieGiftNo = " + nRookieGiftNo);
		}
		if (m_myHero.GetCurrentRookieGiftLoginDuration(m_currentTime) < (float)rookieGift.waitingTime)
		{
			throw new CommandHandleException(101, "아직 대기시간입니다.");
		}
		m_resultItemCollection = new ResultItemCollection();
		foreach (RookieGiftReward reward in rookieGift.rewards)
		{
			ItemReward itemReward = reward.itemReward;
			m_resultItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
		}
		if (!m_myHero.IsAvailableInventory(m_resultItemCollection))
		{
			throw new CommandHandleException(102, "인벤토리가 부족합니다.");
		}
		foreach (ResultItem rewardItem in m_resultItemCollection.resultItems)
		{
			m_myHero.AddItem(rewardItem.item, rewardItem.owned, rewardItem.count, m_changedInventorySlots);
		}
		m_myHero.rookieGiftNo = nRookieGiftNo + 1;
		m_myHero.rookieGiftLoginDuration = 0f;
		m_myHero.rookieGitfLoginStartTime = m_currentTime;
		SaveToDB();
		SaveToLogDB();
		RookieGiftReceiveResponseBody resBody = new RookieGiftReceiveResponseBody();
		resBody.rookieGiftNo = m_myHero.rookieGiftNo;
		resBody.rookieGiftRemainingTime = m_myHero.GetCurrentRookieGiftRemainingTime(m_currentTime);
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_RookieGift(m_myHero.id, m_myHero.rookieGiftNo, m_myHero.rookieGiftLoginDuration));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroRookieGiftRewardLog(logId, m_myHero.id, m_myHero.rookieGiftNo, m_currentTime));
			foreach (ResultItem resultItem in m_resultItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroRookieGiftRewardDetailLog(Guid.NewGuid(), logId, resultItem.item.id, resultItem.owned, resultItem.count));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
