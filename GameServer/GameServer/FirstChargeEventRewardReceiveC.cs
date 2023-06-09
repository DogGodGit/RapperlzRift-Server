using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class FirstChargeEventRewardReceiveCommandHandler : InGameCommandHandler<FirstChargeEventRewardReceiveCommandBody, FirstChargeEventRewardReceiveResponseBody>
{
	public const short kResult_EventNotExist = 101;

	public const short kResult_ObjectiveNotCompleted = 102;

	public const short kResult_AlreadyReceived = 103;

	public const short kResult_NotEnoughInventory = 104;

	private FirstChargeEvent m_evt;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private ResultItemCollection m_rewardItemCollection = new ResultItemCollection();

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_evt = Resource.instance.firstChargeEvent;
		if (m_evt == null)
		{
			throw new CommandHandleException(101, "이벤트가 존재하지 않습니다.");
		}
		if (!m_myAccount.firstChargeEventObjectiveCompleted)
		{
			throw new CommandHandleException(102, "첫충전을 하지 않았습니다.");
		}
		if (m_myAccount.firstChargeEventRewarded)
		{
			throw new CommandHandleException(103, "이미 보상을 받았습니다.");
		}
		foreach (FirstChargeEventReward reward in m_evt.rewards.Values)
		{
			ItemReward itemReward = reward.itemReward;
			m_rewardItemCollection.AddResultItemCount(itemReward.item, itemReward.owned, itemReward.count);
		}
		if (!m_myHero.IsAvailableInventory(m_rewardItemCollection))
		{
			throw new CommandHandleException(104, "인벤토리가 부족합니다.");
		}
		foreach (ResultItem result in m_rewardItemCollection.resultItems)
		{
			m_myHero.AddItem(result.item, result.owned, result.count, m_changedInventorySlots);
		}
		m_myAccount.firstChargeEventRewarded = true;
		SaveToDB();
		SaveToGameLogDB();
		FirstChargeEventRewardReceiveResponseBody resBody = new FirstChargeEventRewardReceiveResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateAccountWork(m_myAccount.id));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateAccount_FirstChargeEventReward(m_myAccount.id));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroFirstChargeEventRewardLog(logId, m_myAccount.id, m_myHero.id, m_currentTime));
			foreach (ResultItem rewardItem in m_rewardItemCollection.resultItems)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroFirstChargeEventRewardDetailLog(Guid.NewGuid(), logId, rewardItem.item.id, rewardItem.owned, rewardItem.count));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
