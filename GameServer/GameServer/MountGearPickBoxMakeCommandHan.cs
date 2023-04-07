using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class MountGearPickBoxMakeCommandHandler : InGameCommandHandler<MountGearPickBoxMakeCommandBody, MountGearPickBoxMakeResponseBody>
{
	public const short kResult_NotEnoughGold = 101;

	public const short kResult_NotEnoughMaterialItem1 = 102;

	public const short kResult_NotEnoughMaterialItem2 = 103;

	public const short kResult_NotEnoughMaterialItem3 = 104;

	public const short kResult_NotEnoughMaterialItem4 = 105;

	public const short kResult_NotEnoughInventory = 106;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nMountGearPickBoxItemId = m_body.mountGearPickBoxItemId;
		if (nMountGearPickBoxItemId <= 0)
		{
			throw new CommandHandleException(1, "탈것장비뽑기상자아이템ID가 유효하지 않습니다. nMountGearPickBoxItemId = " + nMountGearPickBoxItemId);
		}
		MountGearPickBoxRecipe recipe = Resource.instance.GetMountGearPickBoxRecipe(nMountGearPickBoxItemId);
		if (recipe == null)
		{
			throw new CommandHandleException(1, "탈것장비뽑기상자아이템이 아닙니다. nMountGearPickBoxItemId = " + nMountGearPickBoxItemId);
		}
		if (recipe.requiredHeroLevel > m_myHero.level)
		{
			throw new CommandHandleException(1, "레벨에 맞지 않은 탈것장비뽑기상자아이템레시피입니다.");
		}
		Item resultItem = recipe.resultItem;
		bool bResultItemOwned = recipe.resultItemOwned;
		int nResultItemCount = 1;
		int nRecipeGold = recipe.gold;
		Item materialItem1 = recipe.materialItem1;
		int nMaterialItem1Count = recipe.materialItem1Count;
		Item materialItem2 = recipe.materialItem2;
		int nMaterialItem2Count = recipe.materialItem2Count;
		Item materialItem3 = recipe.materialItem3;
		int nMaterialItem3Count = recipe.materialItem3Count;
		Item materialItem4 = recipe.materialItem4;
		int nMaterialItem4Count = recipe.materialItem4Count;
		if (m_myHero.gold < nRecipeGold)
		{
			throw new CommandHandleException(101, "골드가 부족합니다.");
		}
		if (m_myHero.GetItemCount(materialItem1.id) < nMaterialItem1Count)
		{
			throw new CommandHandleException(102, "재료아이템1 수량이 부족합니다.");
		}
		if (m_myHero.GetItemCount(materialItem2.id) < nMaterialItem2Count)
		{
			throw new CommandHandleException(103, "재료아이템2 수량이 부족합니다.");
		}
		if (m_myHero.GetItemCount(materialItem3.id) < nMaterialItem3Count)
		{
			throw new CommandHandleException(104, "재료아이템3 수량이 부족합니다.");
		}
		if (m_myHero.GetItemCount(materialItem4.id) < nMaterialItem4Count)
		{
			throw new CommandHandleException(105, "재료아이템4 수량이 부족합니다.");
		}
		if (m_myHero.GetInventoryAvailableSpace(resultItem, bResultItemOwned) < nResultItemCount)
		{
			throw new CommandHandleException(106, "인벤토리가 부족합니다.");
		}
		bool bFirstOwnUse = true;
		int nMaterialItem1UsedOwnCount = 0;
		int nMaterialItem1UsedUnOwnCount = 0;
		int nMaterialItem2UsedOwnCount = 0;
		int nMaterialItem2UsedUnOwnCount = 0;
		int nMaterialItem3UsedOwnCount = 0;
		int nMaterialItem3UsedUnOwnCount = 0;
		int nMaterialItem4UsedOwnCount = 0;
		int nMaterialItem4UsedUnOwnCount = 0;
		m_myHero.UseItem(materialItem1.id, bFirstOwnUse, nMaterialItem1Count, m_changedInventorySlots, out nMaterialItem1UsedOwnCount, out nMaterialItem1UsedUnOwnCount);
		m_myHero.UseItem(materialItem2.id, bFirstOwnUse, nMaterialItem2Count, m_changedInventorySlots, out nMaterialItem2UsedOwnCount, out nMaterialItem2UsedUnOwnCount);
		m_myHero.UseItem(materialItem3.id, bFirstOwnUse, nMaterialItem3Count, m_changedInventorySlots, out nMaterialItem3UsedOwnCount, out nMaterialItem3UsedUnOwnCount);
		m_myHero.UseItem(materialItem4.id, bFirstOwnUse, nMaterialItem4Count, m_changedInventorySlots, out nMaterialItem4UsedOwnCount, out nMaterialItem4UsedUnOwnCount);
		m_myHero.UseGold(nRecipeGold);
		m_myHero.AddItem(resultItem, bResultItemOwned, nResultItemCount, m_changedInventorySlots);
		SaveToDB(nRecipeGold, resultItem.id, nResultItemCount, bResultItemOwned, materialItem1.id, nMaterialItem1UsedOwnCount, nMaterialItem1UsedUnOwnCount, materialItem2.id, nMaterialItem2UsedOwnCount, nMaterialItem2UsedUnOwnCount, materialItem3.id, nMaterialItem3UsedOwnCount, nMaterialItem3UsedUnOwnCount, materialItem4.id, nMaterialItem4UsedOwnCount, nMaterialItem4UsedUnOwnCount);
		MountGearPickBoxMakeResponseBody resBody = new MountGearPickBoxMakeResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.gold = m_myHero.gold;
		SendResponseOK(resBody);
	}

	private void SaveToDB(long lnUsedGold, int nItemId, int nItemCount, bool bItemOwned, int nMaterialItem1Id, int nMaterialItem1OwnCount, int nMaterialItem1UnOwnCount, int nMaterialItem2Id, int nMaterialItem2OwnCount, int nMaterialItem2UnOwnCount, int nMaterialItem3Id, int nMaterialItem3OwnCount, int nMaterialItem3UnOwnCount, int nMaterialItem4Id, int nMaterialItem4OwnCount, int nMaterialItem4UnOwnCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_myHero.id, m_myHero.gold));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
		}
		dbWork.Schedule();
		SaveToDB_AddHeroMountPickBoxMakingLog(lnUsedGold, nItemId, nItemCount, bItemOwned, nMaterialItem1Id, nMaterialItem1OwnCount, nMaterialItem1UnOwnCount, nMaterialItem2Id, nMaterialItem2OwnCount, nMaterialItem2UnOwnCount, nMaterialItem3Id, nMaterialItem3OwnCount, nMaterialItem3UnOwnCount, nMaterialItem4Id, nMaterialItem4OwnCount, nMaterialItem4UnOwnCount);
	}

	private void SaveToDB_AddHeroMountPickBoxMakingLog(long lnUsedGold, int nItemId, int nItemCount, bool bItemOwned, int nMaterialItem1Id, int nMaterialItem1OwnCount, int nMaterialItem1UnOwnCount, int nMaterialItem2Id, int nMaterialItem2OwnCount, int nMaterialItem2UnOwnCount, int nMaterialItem3Id, int nMaterialItem3OwnCount, int nMaterialItem3UnOwnCount, int nMaterialItem4Id, int nMaterialItem4OwnCount, int nMaterialItem4UnOwnCount)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroMountGearPickBoxMakingLog(Guid.NewGuid(), m_myHero.id, lnUsedGold, nItemId, nItemCount, bItemOwned, nMaterialItem1Id, nMaterialItem1OwnCount, nMaterialItem1UnOwnCount, nMaterialItem2Id, nMaterialItem2OwnCount, nMaterialItem2UnOwnCount, nMaterialItem3Id, nMaterialItem3OwnCount, nMaterialItem3UnOwnCount, nMaterialItem4Id, nMaterialItem4OwnCount, nMaterialItem4UnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
