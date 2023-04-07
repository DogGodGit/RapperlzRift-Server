using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class CreatureSkillSlotOpenCommandHandler : InGameCommandHandler<CreatureSkillSlotOpenCommandBody, CreatureSkillSlotOpenResponseBody>
{
	public const short kResult_NotEnoughItem = 101;

	private HeroCreature m_heroCreature;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

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
		int nOldSlotCount = m_heroCreature.additionalOpenSkillSlotCount;
		CreatureSkillSlotOpenRecipe openRecipe = Resource.instance.GetCreatureSkillSlotOpenRecipe(nOldSlotCount + 1);
		if (openRecipe == null)
		{
			throw new CommandHandleException(1, "크리처스킬슬롯개방레시피가 존재하지 않습니다.");
		}
		int nRequiredItemId = openRecipe.requiredItemId;
		int nRequiredItemCount = openRecipe.requiredItemCount;
		if (m_myHero.GetItemCount(nRequiredItemId) < nRequiredItemCount)
		{
			throw new CommandHandleException(101, "아이템이 부족합니다.");
		}
		m_myHero.UseItem(nRequiredItemId, bFisetUseOwn: true, nRequiredItemCount, m_changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		m_heroCreature.additionalOpenSkillSlotCount++;
		SaveToDB();
		SaveToLogDB(nOldSlotCount, nRequiredItemId);
		CreatureSkillSlotOpenResponseBody resBody = new CreatureSkillSlotOpenResponseBody();
		resBody.additionalOpenSkillSlotCount = m_heroCreature.additionalOpenSkillSlotCount;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroCreature_AdditionalOpenSkillSlotCount(m_heroCreature.instanceId, m_heroCreature.additionalOpenSkillSlotCount));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nOldSlotCount, int nUsedItemId)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureSkilSlotOpenLog(Guid.NewGuid(), m_heroCreature.instanceId, m_myHero.id, nOldSlotCount, m_heroCreature.additionalOpenSkillSlotCount, nUsedItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
