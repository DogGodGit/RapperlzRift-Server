using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CreatureRearCommandHandler : InGameCommandHandler<CreatureRearCommandBody, CreatureRearResponseBody>
{
	public const short kResult_MaxCreatureLevel = 101;

	public const short kResult_NotEnoughItem = 102;

	private HeroCreature m_heroCreature;

	private int m_nUsedOwnCount;

	private int m_nUsedUnOwnCount;

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
		int nItemId = m_body.itemId;
		if (instanceId == Guid.Empty)
		{
			throw new CommandHandleException(1, "인스턴스ID가 유효하지 않습니다. instanceId = " + instanceId);
		}
		if (nItemId <= 0)
		{
			throw new CommandHandleException(1, "아이템ID가 유효하지 않습니다. nItemId = " + nItemId);
		}
		m_heroCreature = m_myHero.GetCreature(instanceId);
		if (m_heroCreature == null)
		{
			throw new CommandHandleException(1, "영웅크리처가 존재하지 않습니다. instanceId = " + instanceId);
		}
		if (m_heroCreature.level >= Resource.instance.lastCreatureLevel.level)
		{
			throw new CommandHandleException(101, "영웅크리처의 레벨이 최대입니다.");
		}
		Item item = Resource.instance.GetItem(nItemId);
		if (item == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 아이템입니다. nItemId = " + nItemId);
		}
		if (item.type.id != 37)
		{
			throw new CommandHandleException(1, "해당 아이템타입은 크리처먹이타입이 아닙니다. nItemId = " + nItemId);
		}
		if (m_myHero.GetItemCount(item.id) < 1)
		{
			throw new CommandHandleException(102, "아이템이 부족합니다.");
		}
		int nAcquisitionExp = item.value1;
		int nOldLevel = m_heroCreature.level;
		int nOldExp = m_heroCreature.exp;
		m_myHero.UseItem(item.id, bFisetUseOwn: true, 1, m_changedInventorySlots, out m_nUsedOwnCount, out m_nUsedUnOwnCount);
		m_heroCreature.AddExp(nAcquisitionExp);
		if (m_heroCreature.level != nOldLevel && (m_heroCreature.participated || m_heroCreature.cheered))
		{
			m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		SaveToDB();
		SaveToLogDB(item.id, nOldLevel, nOldExp, nAcquisitionExp);
		CreatureRearResponseBody resBody = new CreatureRearResponseBody();
		resBody.creatureLevel = m_heroCreature.level;
		resBody.creatureExp = m_heroCreature.exp;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroCreature_Level(m_heroCreature.instanceId, m_heroCreature.level, m_heroCreature.exp));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nUsedItemId, int nOldLevel, int nOldExp, int nAcquisitionExp)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureRearingLog(Guid.NewGuid(), m_heroCreature.instanceId, m_myHero.id, nUsedItemId, m_nUsedOwnCount, m_nUsedUnOwnCount, nOldLevel, m_heroCreature.level, nOldExp, m_heroCreature.exp, nAcquisitionExp, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
