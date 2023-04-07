using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class BountyHunterQuestScrollUseCommandHandler : InGameCommandHandler<BountyHunterQuestScrollUseCommandBody, BountyHunterQuestScrollUseResponseBody>
{
	public const short kResult_NotEnoughItem = 101;

	public const short kResult_DailyUseCountOverflowed = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		int nSlotIndex = m_body.slotIndex;
		int nUseCount = 1;
		if (nSlotIndex < 0)
		{
			throw new CommandHandleException(1, "슬롯인덱스가 유효하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		InventorySlot targetInventorySlot = m_myHero.GetInventorySlot(nSlotIndex);
		if (targetInventorySlot == null)
		{
			throw new CommandHandleException(1, "인벤토리슬롯이 존재하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		if (targetInventorySlot.isEmpty)
		{
			throw new CommandHandleException(1, "빈슬롯 입니다. nSlotIndex = " + nSlotIndex);
		}
		if (targetInventorySlot.obj.inventoryObjectType != 3)
		{
			throw new CommandHandleException(1, "해당 인벤토리 슬롯의 오브젝트 타입이 아이템이 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		ItemInventoryObject inventoryItem = (ItemInventoryObject)targetInventorySlot.obj;
		Item item = inventoryItem.item;
		int nItemCount = inventoryItem.count;
		bool bItemOwned = inventoryItem.owned;
		if (item.type.id != 13)
		{
			throw new CommandHandleException(1, "해당 아이템은 현상금수배서가 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		if (nItemCount < nUseCount)
		{
			throw new CommandHandleException(101, "아이템 수량이 부족합니다. nSlotIndex = " + nSlotIndex);
		}
		if (!item.IsUseableHeroLevel(m_myHero.level))
		{
			throw new CommandHandleException(1, "아이템을 사용할 수 있는 영웅레벨이 아닙니다.");
		}
		m_myHero.RefreshBountyHunterQuestDailyStartCount(currentDate);
		DateValuePair<int> bountyHunterQuestDailyStartCount = m_myHero.bountyHunterQuestDailyStartCount;
		if (bountyHunterQuestDailyStartCount.value >= Resource.instance.bountyHunterMaxCount)
		{
			throw new CommandHandleException(102, "일일사용횟수가 최대사용횟수를 넘어갑니다.");
		}
		HeroBountyHunterQuest heroBountyHunterQuest = m_myHero.bountyHunterQuest;
		if (heroBountyHunterQuest != null)
		{
			throw new CommandHandleException(1, "아직 진행중인 현상금사냥꾼퀘스트가 존재합니다.");
		}
		BountyHunterQuest bountyHunterQuest = Resource.instance.GetBountyQuest(item.value1);
		heroBountyHunterQuest = new HeroBountyHunterQuest(m_myHero, bountyHunterQuest, item.grade);
		m_myHero.bountyHunterQuest = heroBountyHunterQuest;
		bountyHunterQuestDailyStartCount.value += nUseCount;
		m_myHero.UseItem(targetInventorySlot.index, nUseCount);
		int nUsedOwnCount = 0;
		int nUsedUnOwnCount = 0;
		if (bItemOwned)
		{
			nUsedOwnCount = nUseCount;
		}
		else
		{
			nUsedUnOwnCount = nUseCount;
		}
		SaveToDB(targetInventorySlot, heroBountyHunterQuest, item.id, nUsedOwnCount, nUsedUnOwnCount);
		BountyHunterQuestScrollUseResponseBody resBody = new BountyHunterQuestScrollUseResponseBody();
		resBody.quest = heroBountyHunterQuest.ToPDHeroBountyHunterQuest();
		resBody.date = (DateTime)currentDate;
		resBody.bountyHunterQuestDailyStartCount = bountyHunterQuestDailyStartCount.value;
		resBody.changedInventorySlot = targetInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
		m_myHero.ProcessSeriesMission(2);
		m_myHero.ProcessTodayMission(2, m_currentTime);
		m_myHero.ProcessTodayTask(7, currentDate);
		m_myHero.IncreaseOpen7DayEventProgressCount(5);
		m_myHero.ProcessRetrievalProgressCount(4, currentDate);
		m_myHero.ProcessMainQuestForContent(6);
		m_myHero.ProcessSubQuestForContent(6);
	}

	private void SaveToDB(InventorySlot inventorySlot, HeroBountyHunterQuest heroBountyHunterQuest, int nItemId, int nItemOwnCount, int nItemUnOwnCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(inventorySlot));
		dbWork.AddSqlCommand(GameDac.CSC_AddBountyHunterQuest(heroBountyHunterQuest.id, heroBountyHunterQuest.hero.id, heroBountyHunterQuest.quest.id, heroBountyHunterQuest.itemGrade, m_currentTime));
		dbWork.Schedule();
		SaveToDB_AddItemUseLog(nItemId, nItemOwnCount, nItemUnOwnCount);
	}

	private void SaveToDB_AddItemUseLog(int nItemId, int nItemOwnCount, int nItemUnOwnCount)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, nItemId, nItemOwnCount, nItemUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
