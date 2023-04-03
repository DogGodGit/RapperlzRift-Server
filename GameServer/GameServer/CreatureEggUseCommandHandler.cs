using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CreatureEggUseCommandHandler : InGameCommandHandler<CreatureEggUseCommandBody, CreatureEggUseResponseBody>
{
	public const short kResult_NotEnoughItem = 101;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private List<HeroCreature> m_addedHeroCreatures = new List<HeroCreature>();

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
		if (item.type.id != 39)
		{
			throw new CommandHandleException(1, "해당 아이템은 크리처알이 아닙니다.");
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
		int nHeroCreatureRemainingCapacity = Resource.instance.creatureMaxCount - m_myHero.creatureCount;
		int nProgressCount = 0;
		for (nProgressCount = 0; nProgressCount < nUseCount; nProgressCount++)
		{
			List<PickPoolEntry> pickEntries = pickPool.SelectEntries(m_myHero.baseJobId, m_myHero.level);
			foreach (PickPoolEntry entry2 in pickEntries)
			{
				switch (entry2.type)
				{
				case PickPoolEntryType.Creature:
					nHeroCreatureRemainingCapacity--;
					break;
				}
			}
			if (nHeroCreatureRemainingCapacity < 0)
			{
				break;
			}
			m_myHero.UseItem(nSlotIndex, 1);
			m_changedInventorySlots.Add(inventorySlot);
			foreach (PickPoolEntry entry in pickEntries)
			{
				switch (entry.type)
				{
				case PickPoolEntryType.Creature:
				{
					Creature creature = entry.creature;
					HeroCreature heroCreature = new HeroCreature(m_myHero);
					heroCreature.Init(creature);
					m_myHero.AddCreature(heroCreature, bInit: false);
					m_addedHeroCreatures.Add(heroCreature);
					break;
				}
				}
			}
			nItemCount--;
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
		CreatureEggUseResponseBody resBody = new CreatureEggUseResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.addedHeroCreatures = HeroCreature.ToPDHeroCreatures(m_addedHeroCreatures).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nItemId, int nItemOwnCount, int nItemUnOwnCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
		}
		foreach (HeroCreature creature in m_addedHeroCreatures)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddHeroCreature(creature));
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
			foreach (HeroCreature creature in m_addedHeroCreatures)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddPickBoxItemUseDetailLog(Guid.NewGuid(), logId, 5, null, null, null, null, null, null, creature.creature.id));
				logWork.AddSqlCommand(GameLogDaxEx.CSC_AddHeroCreatureCreationLog(creature, m_currentTime));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
