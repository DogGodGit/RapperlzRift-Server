using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class HonorShopProductBuyCommandHandler : InGameCommandHandler<HonorShopProductBuyCommandBody, HonorShopProductBuyResponseBody>
{
	public const short kResult_NotEnoughHonorPoint = 101;

	public const short kResult_NotEnoughInventory = 102;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private int m_nProductId;

	private int m_nCount;

	private HonorShopProduct m_product;

	private int m_nRequiredHonorPoint;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		m_nProductId = m_body.productId;
		m_nCount = m_body.count;
		if (m_nProductId <= 0)
		{
			throw new CommandHandleException(1, "상품ID가 유효하지 않습니다. m_nProductId = " + m_nProductId);
		}
		if (m_nCount <= 0)
		{
			throw new CommandHandleException(1, "상품수량이 유효하지 않습니다. m_nCount = " + m_nCount);
		}
		m_product = Resource.instance.GetHonorShopProduct(m_nProductId);
		if (m_product == null)
		{
			throw new CommandHandleException(1, "상품이 존재하지 않습니다. m_nProductId = " + m_nProductId);
		}
		Item item = m_product.item;
		bool bItemOwned = m_product.itemOwned;
		m_nRequiredHonorPoint = m_product.requiredHonorPoint * m_nCount;
		if (m_myHero.honorPoint < m_nRequiredHonorPoint)
		{
			throw new CommandHandleException(101, "명예포인트가 부족합니다.");
		}
		if (m_myHero.GetInventoryAvailableSpace(item, bItemOwned) < m_nCount)
		{
			throw new CommandHandleException(102, "인벤토리가 부족합니다.");
		}
		m_myHero.UseHonorPoint(m_nRequiredHonorPoint);
		m_myHero.AddItem(item, bItemOwned, m_nCount, m_changedInventorySlots);
		SaveToDB();
		SaveToGameLogDB();
		HonorShopProductBuyResponseBody resBody = new HonorShopProductBuyResponseBody();
		resBody.honorPoint = m_myHero.honorPoint;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_HonorPoint(m_myHero));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToGameLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHonorShopProductBuyLog(logId, m_myHero.id, m_nProductId, m_nCount, m_nRequiredHonorPoint, m_product.item.id, m_product.itemOwned, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
