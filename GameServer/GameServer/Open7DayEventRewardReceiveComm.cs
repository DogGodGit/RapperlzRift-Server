using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class Open7DayEventRewardReceiveCommandHandler : InGameCommandHandler<Open7DayEventRewardReceiveCommandBody, Open7DayEventRewardReceiveResponseBody>
{
	public const short kResult_NotEnounghItem = 101;

	public const short kResult_NotEnounghInventory = 102;

	private int m_nRequiredItemId;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private ItemReward m_rewardItem;

	private HashSet<InventorySlot> m_changedInventSlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_myHero.open7DayEventRewarded)
		{
			throw new CommandHandleException(1, "이미 보상을 받았습니다.");
		}
		m_nRequiredItemId = Resource.instance.open7DayEventCostumeRewardRequiredItemId;
		int nRequiredItemCount = Resource.instance.open7DayEventCostumeRewardRequiredItemCount;
		if (m_myHero.GetItemCount(m_nRequiredItemId) < nRequiredItemCount)
		{
			throw new CommandHandleException(101, "아이템이 부족합니다.");
		}
		m_rewardItem = Resource.instance.open7DayEventCostumeItemReward;
		if (m_myHero.GetInventoryAvailableSpace(m_rewardItem.item, m_rewardItem.owned) < m_rewardItem.count)
		{
			throw new CommandHandleException(102, "인벤토리가 부족합니다.");
		}
		m_myHero.UseItem(m_nRequiredItemId, bFisetUseOwn: true, nRequiredItemCount, m_changedInventSlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		m_myHero.AddItem(m_rewardItem.item, m_rewardItem.owned, m_rewardItem.count, m_changedInventSlots);
		m_myHero.open7DayEventRewarded = true;
		SaveToDB();
		SaveToLogDB();
		Open7DayEventRewardReceiveResponseBody resBody = new Open7DayEventRewardReceiveResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventSlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Open7DayEventRewarded(m_myHero.id));
		foreach (InventorySlot slot in m_changedInventSlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroOpen7DayEventCostumeRewardLog(Guid.NewGuid(), m_myHero.id, m_nRequiredItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_rewardItem.item.id, m_rewardItem.owned, m_rewardItem.count, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
