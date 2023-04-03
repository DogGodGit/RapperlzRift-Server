using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SimpleShopSellCommandHandler : InGameCommandHandler<SimpleShopSellCommandBody, SimpleShopSellResponseBody>
{
	public class SimpleShopSellDetailLog
	{
		public int sellSlotIndex;

		public int inventorySlotIndex;

		public SimpleShopSellDetailLogType type;

		public Guid? heroMainGearId;

		public int itemId;

		public int itemCount;

		public bool itemOwned;

		public Guid? heroMountGearId;

		public long gainedGold;

		public SimpleShopSellDetailLog(int nSellSlotIndex, int nInventorySlotIndex, SimpleShopSellDetailLogType type, Guid? heroMainGearId, int nItemId, int nItemCount, bool bItemOwned, Guid? heroMountGearId, long lnGainedGold)
		{
			sellSlotIndex = nSellSlotIndex;
			inventorySlotIndex = nInventorySlotIndex;
			this.type = type;
			this.heroMainGearId = heroMainGearId;
			itemId = nItemId;
			itemCount = nItemCount;
			itemOwned = bItemOwned;
			this.heroMountGearId = heroMountGearId;
			gainedGold = lnGainedGold;
		}
	}

	public const short kResult_OverflowSellingCount = 101;

	private List<HeroMainGear> m_removedHeroMainGear = new List<HeroMainGear>();

	private List<HeroMountGear> m_removedHeroMountGear = new List<HeroMountGear>();

	private List<InventorySlot> m_targetInventorySlots = new List<InventorySlot>();

	private List<SimpleShopSellDetailLog> m_simpleShopSellDetailLogs = new List<SimpleShopSellDetailLog>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int[] slotIndices = m_body.slotIndices;
		if (slotIndices == null)
		{
			throw new CommandHandleException(1, "유효하지않는 인벤토리슬롯 인덱스 목록입니다.");
		}
		if (slotIndices.Length > Resource.instance.simpleShopSellSlotCount)
		{
			throw new CommandHandleException(101, "판매갯수를 넘어갑니다.");
		}
		int[] array = slotIndices;
		foreach (int nSlotIndex in array)
		{
			InventorySlot targetSlot = m_myHero.GetInventorySlot(nSlotIndex);
			if (targetSlot == null)
			{
				throw new CommandHandleException(1, "존재하지 않는 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
			}
			if (targetSlot.isEmpty)
			{
				throw new CommandHandleException(1, "빈 슬롯입니다. nSlotIndex = " + nSlotIndex);
			}
			if (m_targetInventorySlots.Contains(targetSlot))
			{
				throw new CommandHandleException(1, "중복된 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
			}
			IInventoryObject inventoryObj = targetSlot.obj;
			if (!inventoryObj.saleable)
			{
				throw new CommandHandleException(1, "판매할 수 없는 아이템이 존재합니다.");
			}
			m_targetInventorySlots.Add(targetSlot);
		}
		long lnTotalGold = 0L;
		foreach (InventorySlot slot in m_targetInventorySlots)
		{
			IInventoryObject inventoryObj2 = slot.obj;
			switch (inventoryObj2.inventoryObjectType)
			{
			case 1:
			{
				HeroMainGear mainGear = (HeroMainGear)inventoryObj2;
				int nSaleGold = mainGear.gear.saleGold;
				m_myHero.RemoveMainGear(mainGear.id);
				m_removedHeroMainGear.Add(mainGear);
				slot.Clear();
				lnTotalGold += nSaleGold;
				m_simpleShopSellDetailLogs.Add(new SimpleShopSellDetailLog(m_simpleShopSellDetailLogs.Count, slot.index, SimpleShopSellDetailLogType.MainGear, mainGear.id, 0, 0, bItemOwned: false, null, nSaleGold));
				break;
			}
			case 3:
			{
				ItemInventoryObject item = (ItemInventoryObject)inventoryObj2;
				int nSaleGold2 = item.item.saleGold;
				int nItemCount = item.count;
				m_myHero.UseItem(slot.index, nItemCount);
				lnTotalGold += nSaleGold2 * nItemCount;
				m_simpleShopSellDetailLogs.Add(new SimpleShopSellDetailLog(m_simpleShopSellDetailLogs.Count, slot.index, SimpleShopSellDetailLogType.Item, null, item.itemId, nItemCount, item.owned, null, nSaleGold2 * nItemCount));
				break;
			}
			case 4:
			{
				HeroMountGear mountGear = (HeroMountGear)inventoryObj2;
				int nSaleGold3 = mountGear.gear.saleGold;
				m_myHero.RemoveMountGear(mountGear.id);
				m_removedHeroMountGear.Add(mountGear);
				slot.Clear();
				lnTotalGold += nSaleGold3;
				m_simpleShopSellDetailLogs.Add(new SimpleShopSellDetailLog(m_simpleShopSellDetailLogs.Count, slot.index, SimpleShopSellDetailLogType.MountGear, null, 0, 0, bItemOwned: false, mountGear.id, nSaleGold3));
				break;
			}
			}
		}
		m_myHero.AddGold(lnTotalGold);
		SaveToDB();
		SimpleShopSellResponseBody resBody = new SimpleShopSellResponseBody();
		resBody.gold = m_myHero.gold;
		resBody.maxGold = m_myHero.maxGold;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_myHero.id, m_myHero.gold));
		foreach (HeroMainGear heroMainGear in m_removedHeroMainGear)
		{
			dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroMainGear(heroMainGear.id, m_currentTime));
		}
		foreach (HeroMountGear heroMountGear in m_removedHeroMountGear)
		{
			dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroMountGear(heroMountGear.id, m_currentTime));
		}
		foreach (InventorySlot slot in m_targetInventorySlots)
		{
			dbWork.AddSqlCommand(GameDac.CSC_DeleteInventorySlot(slot.hero.id, slot.index));
		}
		dbWork.Schedule();
		SaveToDB_AddSimpleShopSellLog();
	}

	private void SaveToDB_AddSimpleShopSellLog()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddSimpleShopSellLog(logId, m_myHero.id, m_currentTime));
			foreach (SimpleShopSellDetailLog log in m_simpleShopSellDetailLogs)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddSimpleSellDetailLog(Guid.NewGuid(), logId, log.sellSlotIndex, log.inventorySlotIndex, (int)log.type, log.heroMainGearId, log.itemId, log.itemCount, log.itemOwned, log.heroMountGearId, log.gainedGold));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
