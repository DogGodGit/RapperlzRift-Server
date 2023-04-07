using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class PickBoxUseCommandHandler : InGameCommandHandler<PickBoxUseCommandBody, PickBoxUseResponseBody>
{
    public const short kResult_NotEnoughItem = 101;

    private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

    private List<HeroMainGear> m_addedMainGears = new List<HeroMainGear>();

    private ResultItemCollection m_addedItems = new ResultItemCollection();

    private List<HeroMountGear> m_addedMountGears = new List<HeroMountGear>();

    private HashSet<HeroCreatureCard> m_changedHeroCreatureCards = new HashSet<HeroCreatureCard>();

    private List<int> m_addedCreatureCardIds = new List<int>();

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nSlotIndex = m_body.slotIndex;
        int nUseCount = m_body.useCount;
        if (nSlotIndex < 0)
        {
            throw new CommandHandleException(1, "슬롯 인덱스가 유효하지 않습니다. nSlotIndex = " + nSlotIndex);
        }
        if (nUseCount <= 0)
        {
            throw new CommandHandleException(1, "사용 수량이 유효하지 않습니다. nUseCount = " + nUseCount);
        }
        InventorySlot inventorySlot = m_myHero.GetInventorySlot(nSlotIndex);
        if (inventorySlot == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 인벤토리슬롯입니다. nSlotIndex = " + nSlotIndex);
        }
        if (inventorySlot.isEmpty)
        {
            throw new CommandHandleException(1, "빈 슬롯입니다. nSlotIndex = " + nSlotIndex);
        }
        if (inventorySlot.obj.inventoryObjectType != 3)
        {
            throw new CommandHandleException(1, "해당 슬롯오브젝트는 인벤토리아이템 타입이 아닙니다. nSlotIndex = " + nSlotIndex);
        }
        ItemInventoryObject itemObj = (ItemInventoryObject)inventorySlot.obj;
        Item item = itemObj.item;
        int nItemCount = itemObj.count;
        bool bItemOwned = itemObj.owned;
        if (item.type.id != 4)
        {
            throw new CommandHandleException(1, "해당 아이템은 뽑기 상자가 아닙니다.");
        }
        if (!item.IsUseableHeroLevel(m_myHero.level))
        {
            throw new CommandHandleException(1, "사용할 수 있는 영웅레벨이 아닙니다.");
        }
        if (nItemCount < nUseCount)
        {
            throw new CommandHandleException(101, "아이템이 부족합니다.");
        }
        PickPool pickPool = Resource.instance.GetPickPool(item.value1);
        int nEmptyInventorySlotCount = m_myHero.emptyInventorySlotCount;
        ResultItemCollection resultItemCollection = new ResultItemCollection();
        int nProgressCount = 0;
        for (nProgressCount = 0; nProgressCount < nUseCount; nProgressCount++)
        {
            if (nItemCount == 1)
            {
                nEmptyInventorySlotCount++;
            }
            if (pickPool.randomEntries.Count > 0 && nEmptyInventorySlotCount <= 0)
            {
                break;
            }
            List<PickPoolEntry> pickEntries = pickPool.SelectEntries(m_myHero.baseJobId, m_myHero.level);
            foreach (PickPoolEntry entry2 in pickEntries)
            {
                switch (entry2.type)
                {
                    case PickPoolEntryType.MainGear:
                    case PickPoolEntryType.MountGear:
                        if (nEmptyInventorySlotCount >= 0)
                        {
                            nEmptyInventorySlotCount--;
                        }
                        break;
                    case PickPoolEntryType.Item:
                        resultItemCollection.AddResultItemCount(entry2.item, entry2.itemOwned, entry2.itemCount);
                        break;
                }
            }
            if (!m_myHero.IsAvailableInventory(resultItemCollection, nEmptyInventorySlotCount) || nEmptyInventorySlotCount < 0)
            {
                break;
            }
            m_myHero.UseItem(nSlotIndex, 1);
            m_changedInventorySlots.Add(inventorySlot);
            foreach (PickPoolEntry entry in pickEntries)
            {
                switch (entry.type)
                {
                    case PickPoolEntryType.MainGear:
                        {
                            HeroMainGear heroMainGear = new HeroMainGear(m_myHero);
                            heroMainGear.Init(entry.mainGear, 0, entry.mainGearOwned, m_currentTime);
                            InventorySlot emptySlot = m_myHero.GetEmptyInventorySlot();
                            emptySlot.Place(heroMainGear);
                            m_changedInventorySlots.Add(emptySlot);
                            m_myHero.AddMainGear(heroMainGear, bInit: false, m_currentTime);
                            m_addedMainGears.Add(heroMainGear);
                            break;
                        }
                    case PickPoolEntryType.Item:
                        m_myHero.AddItem(entry.item, entry.itemOwned, entry.itemCount, m_changedInventorySlots);
                        m_addedItems.AddResultItemCount(entry.item, entry.itemOwned, entry.itemCount);
                        break;
                    case PickPoolEntryType.MountGear:
                        {
                            HeroMountGear heroMountGear = new HeroMountGear(m_myHero);
                            heroMountGear.Init(entry.mountGear, entry.mountGearOwned, m_currentTime);
                            InventorySlot emptySlot2 = m_myHero.GetEmptyInventorySlot();
                            emptySlot2.Place(heroMountGear);
                            m_changedInventorySlots.Add(emptySlot2);
                            m_myHero.AddMountGear(heroMountGear);
                            m_addedMountGears.Add(heroMountGear);
                            break;
                        }
                    case PickPoolEntryType.CreatureCard:
                        {
                            CreatureCard creatureCard = entry.creatureCard;
                            HeroCreatureCard heroCreatureCard = m_myHero.IncreaseCreatureCardCount(creatureCard);
                            m_changedHeroCreatureCards.Add(heroCreatureCard);
                            m_addedCreatureCardIds.Add(creatureCard.id);
                            break;
                        }
                }
            }
            nItemCount--;
            resultItemCollection.Clear();
        }
        int nUsedOwnCount = 0;
        int nUsedUnOwnCount = 0;
        if (bItemOwned)
        {
            nUsedOwnCount = nProgressCount;
        }
        else
        {
            nUsedUnOwnCount = nProgressCount;
        }
        if (nUsedOwnCount > 0 || nUsedUnOwnCount > 0)
        {
            SaveToDB(item.id, nUsedOwnCount, nUsedUnOwnCount);
        }
        PickBoxUseResponseBody resBody = new PickBoxUseResponseBody();
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
        resBody.addedHeroMainGears = HeroMainGear.ToPDFullHeroMainGears(m_addedMainGears).ToArray();
        resBody.maxAcquisitionMainGearGrade = m_myHero.maxAcquisitionMainGearGrade;
        resBody.addedHeroMountGears = HeroMountGear.ToPDHeroMountGears(m_addedMountGears).ToArray();
        resBody.changedHeroCreatureCards = HeroCreatureCard.ToPDHeroCreatureCards(m_changedHeroCreatureCards).ToArray();
        SendResponseOK(resBody);
    }

    private void SaveToDB(int nItemId, int nItemOwnCount, int nItemUnOwnCount)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        foreach (HeroMainGear mainGear in m_addedMainGears)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_AddHeroMainGear(mainGear));
        }
        foreach (HeroMountGear mountGear in m_addedMountGears)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_AddHeroMountGear(mountGear));
        }
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
        }
        foreach (HeroCreatureCard card in m_changedHeroCreatureCards)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroCreatureCard(card));
        }
        dbWork.Schedule();
        SaveToDB_AddPickBoxItemUseDetailLog(nItemId, nItemOwnCount, nItemUnOwnCount);
    }

    private void SaveToDB_AddPickBoxItemUseDetailLog(int nItemId, int nItemOwnCount, int nItemUnOwnCount)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            Guid logId = Guid.NewGuid();
            logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(logId, m_myHero.id, nItemId, nItemOwnCount, nItemUnOwnCount, m_currentTime));
            foreach (HeroMainGear addedMainGear in m_addedMainGears)
            {
                logWork.AddSqlCommand(GameLogDac.CSC_AddPickBoxItemUseDetailLog(Guid.NewGuid(), logId, 1, addedMainGear.id, null, null, null, null, null, null));
            }
            foreach (ResultItem result in m_addedItems.resultItems)
            {
                logWork.AddSqlCommand(GameLogDac.CSC_AddPickBoxItemUseDetailLog(Guid.NewGuid(), logId, 2, null, result.item.id, result.count, result.owned, null, null, null));
            }
            foreach (HeroMountGear addedMountGear in m_addedMountGears)
            {
                logWork.AddSqlCommand(GameLogDac.CSC_AddPickBoxItemUseDetailLog(Guid.NewGuid(), logId, 3, null, null, null, null, addedMountGear.id, null, null));
            }
            foreach (int nAddedCreatureCardId in m_addedCreatureCardIds)
            {
                logWork.AddSqlCommand(GameLogDac.CSC_AddPickBoxItemUseDetailLog(Guid.NewGuid(), logId, 4, null, null, null, null, null, nAddedCreatureCardId, null));
            }
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
