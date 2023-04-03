using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FishingBaitUseCommandHandler : InGameCommandHandler<FishingBaitUseCommandBody, FishingBaitUseResponseBody>
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
		if (item.type.id != 14)
		{
			throw new CommandHandleException(1, "해당 아이템은 낚시미끼가 아닙니다. nSlotIndex = " + nSlotIndex);
		}
		if (nItemCount < nUseCount)
		{
			throw new CommandHandleException(101, "아이템 수량이 부족합니다. nSlotIndex = " + nSlotIndex);
		}
		if (!item.IsUseableHeroLevel(m_myHero.level))
		{
			throw new CommandHandleException(1, "아이템을 사용할 수 있는 영웅레벨이 아닙니다.");
		}
		m_myHero.RefreshFishingQuestDailyStartCount(currentDate);
		DateValuePair<int> fishinQuestDailyStartCount = m_myHero.fishinQuestDailyStartCount;
		if (fishinQuestDailyStartCount.value >= Resource.instance.fishingQuest.limitCount)
		{
			throw new CommandHandleException(102, "일일사용횟수가 최대사용횟수를 넘어갑니다.");
		}
		HeroFishingQuest heroFishingQuest = m_myHero.fishingQuest;
		if (heroFishingQuest != null)
		{
			throw new CommandHandleException(1, "이미 낚시퀘스트를 진행중입니다.");
		}
		fishinQuestDailyStartCount.value += nUseCount;
		FishingQuest fishingQuest = Resource.instance.fishingQuest;
		heroFishingQuest = new HeroFishingQuest(m_myHero, fishinQuestDailyStartCount.date, fishinQuestDailyStartCount.value, fishingQuest.GetBait(item.id), 0);
		m_myHero.fishingQuest = heroFishingQuest;
		m_myHero.UseItem(targetInventorySlot.index, nUseCount);
		ProgressAccomplishment(item.grade);
		SaveToDB(heroFishingQuest, targetInventorySlot, item.grade);
		SaveToDB_AddItemUseLog(item.id, nUseCount, bItemOwned);
		FishingBaitUseResponseBody resBody = new FishingBaitUseResponseBody();
		resBody.quest = heroFishingQuest.ToPDHeroFishingQuest();
		resBody.date = (DateTime)currentDate;
		resBody.fishingQuestDailyStartCount = fishinQuestDailyStartCount.value;
		resBody.accEpicBaitItemUseCount = m_myHero.accEpicBaitItemUseCount;
		resBody.accLegendBaitItemUseCount = m_myHero.accLegendBaitItemUseCount;
		resBody.changedInventorySlot = targetInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
		m_myHero.ProcessSeriesMission(1);
		m_myHero.ProcessTodayMission(1, m_currentTime);
		m_myHero.ProcessTodayTask(6, currentDate);
		m_myHero.IncreaseOpen7DayEventProgressCount(11);
		m_myHero.ProcessRetrievalProgressCount(7, currentDate);
		if (item.grade == 5)
		{
			m_myHero.ProcessOrdealQuestMissions(OrdealQuestMissionType.LegendFishingQuest, 1, m_currentTime);
		}
		m_myHero.ProcessMainQuestForContent(7);
		m_myHero.ProcessSubQuestForContent(7);
	}

	private void SaveToDB(HeroFishingQuest heroFishingQuest, InventorySlot targetInventorySlot, int nItemGrade)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_UseFishingQuestItem(m_myHero.id, m_myHero.fishinQuestDailyStartCount.date, m_myHero.fishinQuestDailyStartCount.value, heroFishingQuest.bait.itemId, heroFishingQuest.castingCount));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(targetInventorySlot));
		if (nItemGrade == 4)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_EpicBaitItemUseCount(m_myHero.id, m_myHero.accEpicBaitItemUseCount));
		}
		if (nItemGrade == 5)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_LegendBaitItemUseCount(m_myHero.id, m_myHero.accLegendBaitItemUseCount));
		}
		dbWork.Schedule();
	}

	private void SaveToDB_AddItemUseLog(int nItemId, int nItemUseCount, bool bItemOwned)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			int nUsedOwnCount = 0;
			int nUsedUnOwnCount = 0;
			if (bItemOwned)
			{
				nUsedOwnCount = nItemUseCount;
			}
			else
			{
				nUsedUnOwnCount = nItemUseCount;
			}
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, nItemId, nUsedOwnCount, nUsedUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}

	private void ProgressAccomplishment(int nItemGrade)
	{
		switch (nItemGrade)
		{
		case 4:
			m_myHero.accEpicBaitItemUseCount++;
			break;
		case 5:
			m_myHero.accLegendBaitItemUseCount++;
			break;
		}
	}
}
