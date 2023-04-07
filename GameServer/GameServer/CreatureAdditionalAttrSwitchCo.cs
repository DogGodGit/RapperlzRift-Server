using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CreatureAdditionalAttrSwitchCommandHandler : InGameCommandHandler<CreatureAdditionalAttrSwitchCommandBody, CreatureAdditionalAttrSwitchResponseBody>
{
	private class CreatureAdditionalAttrSwitchDetailLog
	{
		public int oldAttrId;

		public int attrId;

		public CreatureAdditionalAttrSwitchDetailLog(int nOldAttrId, int nAttrId)
		{
			oldAttrId = nOldAttrId;
			attrId = nAttrId;
		}
	}

	public const short kResult_NotEnoughitem = 101;

	private HeroCreature m_heroCreature;

	private List<CreatureAdditionalAttrSwitchDetailLog> m_detailLogs = new List<CreatureAdditionalAttrSwitchDetailLog>();

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private InventorySlot m_targetInventorySlot;

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
		int nRequiredItemId = Resource.instance.creatureAdditionalAttrSwitchRequiredItemId;
		if (m_myHero.GetItemCount(nRequiredItemId) < 1)
		{
			throw new CommandHandleException(101, "아이템이 부족합니다.");
		}
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		m_myHero.UseItem(nRequiredItemId, bFisetUseOwn: true, 1, changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		m_targetInventorySlot = changedInventorySlots[0];
		foreach (HeroCreatureAdditionalAttr additionalAttr in m_heroCreature.additionalAttrs)
		{
			int nOldAttrId = additionalAttr.attr.attrId;
			CreatureAdditionalAttr attr = (additionalAttr.attr = Resource.instance.SelectCreatureAdditionalAttr());
			m_detailLogs.Add(new CreatureAdditionalAttrSwitchDetailLog(nOldAttrId, attr.attrId));
		}
		if (m_heroCreature.participated || m_heroCreature.cheered)
		{
			m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		SaveToDB();
		SaveToLogDB(nRequiredItemId);
		CreatureAdditionalAttrSwitchResponseBody resBody = new CreatureAdditionalAttrSwitchResponseBody();
		resBody.additionalAttrIds = m_heroCreature.GetAdditionalAttrIds().ToArray();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.changedInventorySlot = m_targetInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroCreatureAdditionalAttrs(m_heroCreature.instanceId));
		foreach (HeroCreatureAdditionalAttr attr in m_heroCreature.additionalAttrs)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroCreatureAdditionalAttr(attr.creature.instanceId, attr.no, attr.attr.attrId));
		}
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_targetInventorySlot));
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nUsedItemId)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureAdditionalAttrSwitchLog(logId, m_heroCreature.instanceId, m_myHero.id, nUsedItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_currentTime));
			foreach (CreatureAdditionalAttrSwitchDetailLog detailLog in m_detailLogs)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureAdditionalAttrSwitchDetailLog(Guid.NewGuid(), logId, detailLog.oldAttrId, detailLog.attrId));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
