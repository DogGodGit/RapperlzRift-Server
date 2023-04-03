using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CostumeCollectionShuffleCommandHandler : InGameCommandHandler<CostumeCollectionShuffleCommandBody, CostumeCollectionShuffleResponseBody>
{
	public const short kResult_NotEnoughItem = 101;

	private int m_nUsedItemId;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		m_nUsedItemId = Resource.instance.costumeCollectionShuffleItemId;
		int nRequiredItemCount = Resource.instance.costumeCollectionShuffleItemCount;
		if (m_myHero.GetItemCount(m_nUsedItemId) < nRequiredItemCount)
		{
			throw new CommandHandleException(101, "아이템이 부족합니다.");
		}
		m_myHero.UseItem(m_nUsedItemId, bFisetUseOwn: true, nRequiredItemCount, m_changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		int nOldCollectionId = m_myHero.costumeCollectionId;
		m_myHero.costumeCollection = Resource.instance.SelectCostumeCollection(nOldCollectionId);
		m_myHero.costumeCollectionActivated = false;
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SaveToLogDB(nOldCollectionId);
		CostumeCollectionShuffleResponseBody resBody = new CostumeCollectionShuffleResponseBody();
		resBody.collectionId = m_myHero.costumeCollectionId;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CostumeCollectionId(m_myHero.id, m_myHero.costumeCollection.id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CostumeCollectionActivation(m_myHero.id, m_myHero.costumeCollectionActivated));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nOldCollectionId)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCostumeCollectionSuffleLog(Guid.NewGuid(), m_myHero.id, nOldCollectionId, m_myHero.costumeCollectionId, m_nUsedItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}
}
