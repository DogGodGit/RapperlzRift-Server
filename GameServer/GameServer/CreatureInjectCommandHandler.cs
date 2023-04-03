using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CreatureInjectCommandHandler : InGameCommandHandler<CreatureInjectCommandBody, CreatureInjectResponseBody>
{
	public const short kResult_MaxCreatureInjectionLevel = 101;

	public const short kResult_NotEnoughItem = 102;

	public const short kResult_NotEnoughGold = 103;

	private HeroCreature m_heroCreature;

	private int m_nUsedOwnCount;

	private int m_nUsedUnOwnCount;

	private long m_lnUsedGold;

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
		CreatureLevel creatureLevel = Resource.instance.GetCreatureLevel(m_heroCreature.level);
		if (m_heroCreature.injectionLevel >= creatureLevel.maxInjectionLevel)
		{
			throw new CommandHandleException(101, "영웅크리처의 주입레벨이 최대입니다.");
		}
		int nOldInjectionLevel = m_heroCreature.injectionLevel;
		int nOldInjectionExp = m_heroCreature.injectionExp;
		CreatureInjectionLevel injectionLevel = Resource.instance.GetCreatureInjectionLevel(nOldInjectionLevel);
		int nRequiredItemCount = injectionLevel.requiredItemCount;
		m_lnUsedGold = injectionLevel.requiredGold;
		Item requiredItem = Resource.instance.creatureInjectionExpItem;
		if (m_myHero.GetItemCount(requiredItem.id) < nRequiredItemCount)
		{
			throw new CommandHandleException(102, "아이템이 부족합니다.");
		}
		if (m_myHero.gold < m_lnUsedGold)
		{
			throw new CommandHandleException(103, "골드가 부족합니다.");
		}
		CreatureInjectionLevelUpEntry levelUpEntry = Resource.instance.SelectCreatureInjectionLevelUpEntry();
		bool bCritical = levelUpEntry.isCritical;
		int nInjectionExp = SFRandom.Next(levelUpEntry.minInjectionExp, levelUpEntry.maxInjectionExp + 1);
		m_heroCreature.AddInjectionExp(nInjectionExp);
		m_myHero.UseItem(requiredItem.id, bFisetUseOwn: true, nRequiredItemCount, m_changedInventorySlots, out m_nUsedOwnCount, out m_nUsedUnOwnCount);
		m_heroCreature.injectionItemCount += nRequiredItemCount;
		m_myHero.UseGold(m_lnUsedGold);
		if (m_heroCreature.level != injectionLevel.level && (m_heroCreature.participated || m_heroCreature.cheered))
		{
			m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		SaveToDB();
		SaveToLogDB(requiredItem.id, nOldInjectionLevel, nOldInjectionExp, nInjectionExp);
		CreatureInjectResponseBody resBody = new CreatureInjectResponseBody();
		resBody.isCritical = bCritical;
		resBody.injectionLevel = m_heroCreature.injectionLevel;
		resBody.injectionExp = m_heroCreature.injectionExp;
		resBody.injectionItemCount = m_heroCreature.injectionItemCount;
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.gold = m_myHero.gold;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		SendResponseOK(resBody);
		if (nOldInjectionLevel != m_heroCreature.injectionLevel && Resource.instance.CheckSystemMessageCondition(5, m_heroCreature.injectionLevel))
		{
			SystemMessage.SendCreatureInjection(m_myHero, m_heroCreature);
		}
	}

	public void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_myHero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroCreature_InjectionLevel(m_heroCreature.instanceId, m_heroCreature.injectionLevel, m_heroCreature.injectionExp, m_heroCreature.injectionItemCount));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.Schedule();
	}

	public void SaveToLogDB(int nUsedItemId, int nOldInjectionLevel, int nOldInjectionExp, int nAcquisitionInjectionExp)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureInjectionLog(Guid.NewGuid(), m_heroCreature.instanceId, m_myHero.id, nUsedItemId, m_nUsedOwnCount, m_nUsedUnOwnCount, m_lnUsedGold, nOldInjectionLevel, m_heroCreature.injectionLevel, nOldInjectionExp, m_heroCreature.injectionExp, nAcquisitionInjectionExp, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
