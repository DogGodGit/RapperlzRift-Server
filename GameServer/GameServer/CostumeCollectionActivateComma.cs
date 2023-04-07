using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CostumeCollectionActivateCommandHandler : InGameCommandHandler<CostumeCollectionActivateCommandBody, CostumeCollectionActivateResponseBody>
{
	public const short kResult_NotEnoughItem = 101;

	public const short kResult_NotEnoughCostume = 102;

	private int m_nUsedItemId;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		CostumeCollection costumeCollection = m_myHero.costumeCollection;
		m_nUsedItemId = Resource.instance.costumeCollectionActivationItemId;
		int nRequiredItemCount = costumeCollection.activationItemCount;
		if (m_myHero.GetItemCount(m_nUsedItemId) < nRequiredItemCount)
		{
			throw new CommandHandleException(101, "아이템이 부족합니다.");
		}
		foreach (CostumeCollectionEntry entry in costumeCollection.entries.Values)
		{
			Costume costume = entry.costume;
			if (!m_myHero.ContainsCostume(costume.id))
			{
				throw new CommandHandleException(102, "필요 코스튬을 보유하고 있지 않습니다.");
			}
		}
		m_myHero.UseItem(m_nUsedItemId, bFisetUseOwn: true, nRequiredItemCount, m_changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		m_myHero.costumeCollectionActivated = true;
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SaveToLogDB();
		CostumeCollectionActivateResponseBody resBody = new CostumeCollectionActivateResponseBody();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CostumeCollectionActivation(m_myHero.id, m_myHero.costumeCollectionActivated));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCostumeCollectionActivationLog(Guid.NewGuid(), m_myHero.id, m_myHero.costumeCollectionId, m_nUsedItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
