using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MountedSoulstoneComposeCommandHandler : InGameCommandHandler<MountedSoulstoneComposeCommandBody, MountedSoulstoneComposeResponseBody>
{
	public const short kResult_NotEnoughMaterialItem = 101;

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
		int nSubGearId = m_body.subGearId;
		int nSlotIndex = m_body.slotIndex;
		if (nSubGearId <= 0)
		{
			throw new CommandHandleException(1, "보조장비ID가 유효하지 않습니다. nSubGearId = " + nSubGearId);
		}
		if (nSlotIndex < 0)
		{
			throw new CommandHandleException(1, "슬롯인덱스가 유효하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		HeroSubGear heroSubGear = m_myHero.GetSubGear(nSubGearId);
		if (heroSubGear == null)
		{
			throw new CommandHandleException(1, "영웅보조장비가 존재하지 않습니다. nSubGearId = " + nSubGearId);
		}
		if (!heroSubGear.equipped)
		{
			throw new CommandHandleException(1, "영웅보조장비가 장착되어있지 않습니다.");
		}
		HeroSoulstoneSocket heroSoulstoneSocket = heroSubGear.GetSoulstoneSocket(nSlotIndex);
		if (heroSoulstoneSocket == null)
		{
			throw new CommandHandleException(1, "소울스톤소켓이 존재하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		if (!heroSoulstoneSocket.isOpened)
		{
			throw new CommandHandleException(1, "아직 개방되지 않은 소울스톤소켓입니다. nSlotIndex = " + nSlotIndex);
		}
		if (heroSoulstoneSocket.isEmpty)
		{
			throw new CommandHandleException(1, "빈 소켓입니다.");
		}
		int socketItemId = heroSoulstoneSocket.itemId;
		ItemCompositionRecipe recipe = Resource.instance.GetItemCompositionRecipe(socketItemId);
		if (recipe == null)
		{
			throw new CommandHandleException(1, "합성이 불가능한 아이템입니다.");
		}
		Item compositedItem = recipe.item;
		int materialItemCount = recipe.materialItemCount - 1;
		int nPriceGold = recipe.gold;
		int nResultCount = 1;
		bool bSubGearOwned = true;
		if (m_myHero.GetItemCount(socketItemId) < materialItemCount)
		{
			throw new CommandHandleException(101, "재료 아이템이 부족합니다.");
		}
		if (m_myHero.gold < nPriceGold)
		{
			throw new CommandHandleException(102, "골드가 부족합니다.");
		}
		int nUsedOwnCount = 0;
		int nUsedUnOwnCount = 0;
		m_myHero.UseItem(socketItemId, bSubGearOwned, materialItemCount, m_changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
		m_myHero.UseGold(nPriceGold);
		heroSoulstoneSocket.Mount(compositedItem);
		nUsedOwnCount++;
		heroSubGear.RefreshAttrTotalValues();
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB(heroSoulstoneSocket, compositedItem.id, nResultCount, bSubGearOwned, socketItemId, nUsedOwnCount, nUsedUnOwnCount, nPriceGold);
		MountedSoulstoneComposeResponseBody resBody = new MountedSoulstoneComposeResponseBody();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		resBody.gold = m_myHero.gold;
		resBody.maxHp = m_myHero.realMaxHP;
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroSoulstoneSocket socket, int nResultItemId, int nResultItemCount, bool bResultItemOwned, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, long lnUsedGold)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_myHero.id, m_myHero.gold));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSubGearSoulstoneSocket_Item(socket.subGear.hero.id, socket.subGear.subGearId, socket.index, socket.itemId));
		dbWork.Schedule();
		SaveToDB_AddItemCompositionLog(nResultItemId, nResultItemCount, bResultItemOwned, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, lnUsedGold);
	}

	private void SaveToDB_AddItemCompositionLog(int nResultItemId, int nResultItemCount, bool bResultItemOwned, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, long lnUsedGold)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemCompositionLog(Guid.NewGuid(), m_myHero.id, nResultItemId, nResultItemCount, bResultItemOwned, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, lnUsedGold, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
