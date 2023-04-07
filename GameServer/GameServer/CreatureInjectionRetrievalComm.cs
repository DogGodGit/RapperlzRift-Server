using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CreatureInjectionRetrievalCommandHandler : InGameCommandHandler<CreatureInjectionRetrievalCommandBody, CreatureInjectionRetrievalResponseBody>
{
	public const short kResult_NotEnoughInventory = 101;

	private HeroCreature m_heroCreature;

	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid instanceId = (Guid)m_body.instanceId;
		if (instanceId == Guid.Empty)
		{
			throw new CommandHandleException(1, "인스턴스ID가 유효하지 않습니다. instanceId = " + instanceId);
		}
		m_heroCreature = m_myHero.GetCreature(instanceId);
		if (m_heroCreature == null)
		{
			throw new CommandHandleException(1, "영웅크리처가 존재하지 않습니다. instanceId = " + instanceId);
		}
		int nOldInjectionLevel = m_heroCreature.injectionLevel;
		int nOldInjectionExp = m_heroCreature.injectionExp;
		m_heroCreature.GetAccumulationInjectionExp();
		Item rewardItem = Resource.instance.creatureInjectionExpItem;
		int nRewardItemCount = (int)((float)(m_heroCreature.injectionItemCount * Resource.instance.creatureInjectionExpRetrievalRate) / 10000f);
		if (rewardItem != null && m_myHero.GetInventoryAvailableSpace(rewardItem, bOwned: true) < nRewardItemCount)
		{
			throw new CommandHandleException(101, "인벤토리가 부족합니다.");
		}
		if (rewardItem != null)
		{
			m_myHero.AddItem(rewardItem, bOwned: true, nRewardItemCount, m_changedInventorySlots);
		}
		m_heroCreature.RetrievalInjecitonLevel();
		if (m_heroCreature.participated || m_heroCreature.cheered)
		{
			m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		SaveToDB();
		SaveToLogDB(nOldInjectionLevel, nOldInjectionExp, rewardItem, bRetrievalItemOwned: true, nRewardItemCount);
		CreatureInjectionRetrievalResponseBody resBody = new CreatureInjectionRetrievalResponseBody();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroCreature_InjectionLevel(m_heroCreature.instanceId, m_heroCreature.injectionLevel, m_heroCreature.injectionExp, m_heroCreature.injectionItemCount));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nOldInjectionLevel, int nOldInjectionExp, Item retrievalItem, bool bRetrievalItemOwned, int nRetrievalItemCount)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureInjectionRetrievalLog(Guid.NewGuid(), m_heroCreature.instanceId, m_myHero.id, nOldInjectionLevel, nOldInjectionExp, retrievalItem?.id ?? 0, bRetrievalItemOwned, nRetrievalItemCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
