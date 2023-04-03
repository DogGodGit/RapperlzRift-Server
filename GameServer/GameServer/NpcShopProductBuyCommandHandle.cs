using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class NpcShopProductBuyCommandHandler : InGameCommandHandler<NpcShopProductBuyCommandBody, NpcShopProductBuyResponseBody>
{
	public const short kResult_NotEnoughMeterialItem = 101;

	public const short kResult_NotEnoughInventory = 102;

	public const short kResult_BuyCountOverflowed = 103;

	private HeroNpcShopProduct m_heroNpcShopProudct;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private int m_nRequiredItemId;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nProductId = m_body.productId;
		if (nProductId <= 0)
		{
			throw new CommandHandleException(1, "상품ID가 유효하지 않습니다. nProductId = " + nProductId);
		}
		NpcShopProduct npcShopProduct = Resource.instance.GetNpcShopProduct(nProductId);
		if (npcShopProduct == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 NPC상점상품입니다. nProductId = " + nProductId);
		}
		NpcShopCategory npcShopCategory = npcShopProduct.category;
		m_nRequiredItemId = npcShopCategory.requiredItemId;
		int nRequiredItemCount = npcShopProduct.requiredItemCount;
		if (m_myHero.GetItemCount(m_nRequiredItemId) < nRequiredItemCount)
		{
			throw new CommandHandleException(101, "필요아이템수량이 부족합니다.");
		}
		if (m_myHero.GetInventoryAvailableSpace(npcShopProduct.item, npcShopProduct.itemOwned) < 1)
		{
			throw new CommandHandleException(102, "인벤토리가 부족합니다.");
		}
		m_heroNpcShopProudct = m_myHero.GetNpcShopProduct(nProductId);
		int nBuyCount = ((m_heroNpcShopProudct != null) ? m_heroNpcShopProudct.buyCount : 0) + 1;
		if (npcShopProduct.limitCount > 0 && npcShopProduct.limitCount < nBuyCount)
		{
			throw new CommandHandleException(103, "구매수량이 제한수량을 넘어갑니다.");
		}
		if (m_heroNpcShopProudct == null)
		{
			m_heroNpcShopProudct = new HeroNpcShopProduct(m_myHero, npcShopProduct);
			m_myHero.AddNpcShopProduct(m_heroNpcShopProudct);
		}
		m_myHero.UseItem(m_nRequiredItemId, bFisetUseOwn: true, nRequiredItemCount, m_changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		m_myHero.AddItem(npcShopProduct.item, npcShopProduct.itemOwned, 1, m_changedInventorySlots);
		m_heroNpcShopProudct.buyCount = nBuyCount;
		SaveToDB();
		SaveToGameLogDB(npcShopProduct, 1);
		NpcShopProductBuyResponseBody resBody = new NpcShopProductBuyResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroNpcShopProduct(m_heroNpcShopProudct.hero.id, m_heroNpcShopProudct.product.id, m_heroNpcShopProudct.buyCount));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(NpcShopProduct npcShopProduct, int nBuyCount)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroNpcShopProductBuyLog(Guid.NewGuid(), m_myHero.id, npcShopProduct.id, m_nRequiredItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, npcShopProduct.item.id, npcShopProduct.itemOwned, nBuyCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
