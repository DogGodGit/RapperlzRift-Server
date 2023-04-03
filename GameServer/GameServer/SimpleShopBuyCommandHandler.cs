using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SimpleShopBuyCommandHandler : InGameCommandHandler<SimpleShopBuyCommandBody, SimpleShopBuyResponseBody>
{
	public const short kResult_NotEnoughInventory = 101;

	public const short kResult_NotEnoughGold = 102;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nProductId = m_body.productId;
		int nCount = m_body.count;
		if (nProductId <= 0)
		{
			throw new CommandHandleException(1, "제품 ID가 유효하지 않습니다. nProductId = " + nProductId);
		}
		if (nCount <= 0)
		{
			throw new CommandHandleException(1, "제품 수량이 유효하지 않습니다. nCount = " + nCount);
		}
		SimpleShopProduct product = Resource.instance.GetSimpleShop(nProductId);
		if (product == null)
		{
			throw new CommandHandleException(1, "제품이 존재하지 않습니다. nProductId = " + nProductId);
		}
		Item productItem = product.item;
		long lnProductPrice = product.saleGold * nCount;
		bool bProductOwned = product.owned;
		if (m_myHero.GetInventoryAvailableSpace(productItem, bProductOwned) < nCount)
		{
			throw new CommandHandleException(101, "인벤토리가 부족합니다.");
		}
		if (m_myHero.gold < lnProductPrice)
		{
			throw new CommandHandleException(102, "골드가 부족합니다.");
		}
		m_myHero.AddItem(productItem, bProductOwned, nCount, m_changedInventorySlots);
		m_myHero.UseGold(lnProductPrice);
		int nTotalItemCount = m_myHero.GetItemCount(productItem.id, bProductOwned);
		SaveToDB(product.id, nCount, productItem.id, nTotalItemCount, bProductOwned, lnProductPrice);
		SimpleShopBuyResponseBody resBody = new SimpleShopBuyResponseBody();
		resBody.gold = m_myHero.gold;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nProductId, int nBuyCount, int nItemId, int nItemCount, bool bItemOwned, long lnUsedGold)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_myHero.id, m_myHero.gold));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.Schedule();
		SaveToDB_AddSimpleShopProductBuyLog(nProductId, nBuyCount, nItemId, nItemCount, bItemOwned, lnUsedGold);
	}

	private void SaveToDB_AddSimpleShopProductBuyLog(int nProductId, int nBuyCount, int nItemId, int nItemCount, bool bItemOwned, long lnUsedGold)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddSimpleShopProductBuyLog(Guid.NewGuid(), m_myHero.id, nProductId, nBuyCount, nItemId, nItemCount, bItemOwned, lnUsedGold, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
