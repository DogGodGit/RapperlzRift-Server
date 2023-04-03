using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MountGearPickBoxMakeTotallyCommandHandler : InGameCommandHandler<MountGearPickBoxMakeTotallyCommandBody, MountGearPickBoxMakeTotallyResponseBody>
{
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
		long lnTotalUsedGold = 0L;
		int nTotalMaterialItem1UsedOwnCount = 0;
		int nTotalMaterialItem1UsedUnOwnCount = 0;
		int nTotalMaterialItem2UsedOwnCount = 0;
		int nTotalMaterialItem2UsedUnOwnCount = 0;
		int nTotalMaterialItem3UsedOwnCount = 0;
		int nTotalMaterialItem3UsedUnOwnCount = 0;
		int nTotalMaterialItem4UsedOwnCount = 0;
		int nTotalMaterialItem4UsedUnOwnCount = 0;
		int nTotalResultItemCount = 0;
		while (m_myHero.gold >= nRecipeGold && m_myHero.GetItemCount(materialItem1.id) >= nMaterialItem1Count && m_myHero.GetItemCount(materialItem2.id) >= nMaterialItem2Count && m_myHero.GetItemCount(materialItem3.id) >= nMaterialItem3Count && m_myHero.GetItemCount(materialItem4.id) >= nMaterialItem4Count && m_myHero.GetInventoryAvailableSpace(resultItem, bResultItemOwned) >= nResultItemCount)
		{
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
			lnTotalUsedGold += nRecipeGold;
			nTotalMaterialItem1UsedOwnCount += nMaterialItem1UsedOwnCount;
			nTotalMaterialItem1UsedUnOwnCount += nMaterialItem1UsedUnOwnCount;
			nTotalMaterialItem2UsedOwnCount += nMaterialItem2UsedOwnCount;
			nTotalMaterialItem2UsedUnOwnCount += nMaterialItem2UsedUnOwnCount;
			nTotalMaterialItem3UsedOwnCount += nMaterialItem3UsedOwnCount;
			nTotalMaterialItem3UsedUnOwnCount += nMaterialItem3UsedUnOwnCount;
			nTotalMaterialItem4UsedOwnCount += nMaterialItem4UsedOwnCount;
			nTotalMaterialItem4UsedUnOwnCount += nMaterialItem4UsedUnOwnCount;
			nTotalResultItemCount += nResultItemCount;
		}
		if (nTotalResultItemCount > 0)
		{
			SaveToDB();
			SaveToDB_AddHeroMountPickBoxMakingLog(lnTotalUsedGold, resultItem.id, nTotalResultItemCount, bResultItemOwned, materialItem1.id, nTotalMaterialItem1UsedOwnCount, nTotalMaterialItem1UsedUnOwnCount, materialItem2.id, nTotalMaterialItem2UsedOwnCount, nTotalMaterialItem2UsedUnOwnCount, materialItem3.id, nTotalMaterialItem3UsedOwnCount, nTotalMaterialItem3UsedUnOwnCount, materialItem4.id, nTotalMaterialItem4UsedOwnCount, nTotalMaterialItem4UsedUnOwnCount);
		}
		MountGearPickBoxMakeTotallyResponseBody resBody = new MountGearPickBoxMakeTotallyResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.gold = m_myHero.gold;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_myHero.id, m_myHero.gold));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_ApplyChangedInventorySlots(slot));
		}
		dbWork.Schedule();
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
