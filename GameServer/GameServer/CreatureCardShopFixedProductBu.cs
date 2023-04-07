using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CreatureCardShopFixedProductBuyCommandHandler : InGameCommandHandler<CreatureCardShopFixedProductBuyCommandBody, CreatureCardShopFixedProductBuyResponseBody>
{
	public const short kResult_AlreadyBoughtProduct = 101;

	public const short kResult_NotEnoughSoulPowder = 102;

	public const short kResult_NotEnoughInventory = 103;

	private InventorySlot m_changedInventorySlot;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private int m_nProductId;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_nProductId = m_body.productId;
		if (m_nProductId <= 0)
		{
			throw new CommandHandleException(1, "상품ID가 유효하지 않습니다. productId = " + m_nProductId);
		}
		if (m_myHero.IsPurchasedCreatureCardShopFixedProduct(m_nProductId))
		{
			throw new CommandHandleException(101, "이미 구매한 상품입니다. productId = " + m_nProductId);
		}
		CreatureCardShopFixedProduct product = Resource.instance.GetCreatureCardShopFixedProduct(m_nProductId);
		if (product == null)
		{
			throw new CommandHandleException(1, "존재하지 않은 고정상품입니다. productId = " + m_nProductId);
		}
		int nSaleSoulPowder = product.saleSoulPowder;
		if (m_myHero.soulPowder < nSaleSoulPowder)
		{
			throw new CommandHandleException(102, "영혼가루가 부족합니다.");
		}
		Item productItem = product.item;
		bool bOwned = product.owned;
		if (m_myHero.GetInventoryAvailableSpace(productItem, bOwned) <= 0)
		{
			throw new CommandHandleException(103, "인벤토리가 부족합니다.");
		}
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		m_myHero.AddItem(productItem, bOwned, 1, changedInventorySlots);
		m_changedInventorySlot = changedInventorySlots[0];
		m_myHero.UseSoulPowder(nSaleSoulPowder);
		m_myHero.AddPurchasedCreatureCardShopFixedProduct(m_nProductId);
		SaveToDB();
		SaveToGameLogDB(productItem.id, bOwned, nSaleSoulPowder);
		CreatureCardShopFixedProductBuyResponseBody resBody = new CreatureCardShopFixedProductBuyResponseBody();
		resBody.soulPowder = m_myHero.soulPowder;
		resBody.changedInventorySlot = m_changedInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SoulPowder(m_myHero.id, m_myHero.soulPowder));
		dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(m_changedInventorySlot));
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroCreatureCardShopFixedProductBuy(m_myHero.id, m_nProductId));
		dbWork.Schedule();
	}

	private void SaveToGameLogDB(int nItemId, bool bItemOwned, int nUsedSoulPowder)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardShopFixedProductBuyLog(m_myHero.creatureCardShopId, m_nProductId, nItemId, bItemOwned, nUsedSoulPowder, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
