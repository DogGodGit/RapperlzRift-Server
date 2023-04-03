using System;
using System.Collections.Generic;
using System.Linq;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class CreatureVaryCommandHandler : InGameCommandHandler<CreatureVaryCommandBody, CreatureVaryResponseBody>
{
	public const short kResult_OverflowedVariationCount = 101;

	public const short kResult_NotEnoughItem = 102;

	private HeroCreature m_heroCreature;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private InventorySlot m_targetInventorySlot;

	private List<HeroCreatureBaseAttr> m_oldBaseAttrs = new List<HeroCreatureBaseAttr>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = m_currentTime.Date;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid instanceId = (Guid)m_body.instanceId;
		if (instanceId == Guid.Empty)
		{
			throw new CommandHandleException(1, "영웅 크리처ID가 유효하지 않습니다. instanceId = " + instanceId);
		}
		m_heroCreature = m_myHero.GetCreature(instanceId);
		if (m_heroCreature == null)
		{
			throw new CommandHandleException(1, "영웅크리처가 존재하지 않습니다.");
		}
		m_myHero.RefreshDailyCreatureVariationCount(currentDate);
		DateValuePair<int> dailyCrueatureVariationCount = m_myHero.dailyCreatureVariationCount;
		if (dailyCrueatureVariationCount.value > m_myHero.vipLevel.creatureVariationMaxCount)
		{
			throw new CommandHandleException(101, "변이횟수가 최대횟수를 넘어갑니다.");
		}
		int nRequiredItemId = Resource.instance.creatureVariationRequiredItemId;
		if (m_myHero.GetItemCount(nRequiredItemId) < 1)
		{
			throw new CommandHandleException(102, "아이템이 부족합니다.");
		}
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		m_myHero.UseItem(nRequiredItemId, bFisetUseOwn: true, 1, changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		m_targetInventorySlot = changedInventorySlots[0];
		int nOldQuality = m_heroCreature.quality;
		m_oldBaseAttrs = m_heroCreature.baseAttrs.Values.ToList();
		m_heroCreature.RemoveBaseAttrs();
		Creature creature = m_heroCreature.creature;
		foreach (CreatureBaseAttrValue attr in creature.baseAttrs.Values)
		{
			int nValue = SFRandom.Next(attr.minAttrValue, attr.maxAttrValue + 1);
			HeroCreatureBaseAttr newBaseAttr = new HeroCreatureBaseAttr(m_heroCreature, attr, nValue);
			m_heroCreature.AddBaseAttr(newBaseAttr);
		}
		m_heroCreature.quality = SFRandom.Next(creature.minQuality, creature.maxQuality + 1);
		if (m_heroCreature.participated || m_heroCreature.cheered)
		{
			m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		SaveToDB();
		SaveToLogDB(nOldQuality, nRequiredItemId);
		CreatureVaryResponseBody resBody = new CreatureVaryResponseBody();
		resBody.date = (DateTime)dailyCrueatureVariationCount.date;
		resBody.dailyCreatureVariationCount = dailyCrueatureVariationCount.value;
		resBody.quality = m_heroCreature.quality;
		resBody.baseAttrs = HeroCreatureBaseAttr.ToPDHeroCreatureBaseAttrs(m_heroCreature.baseAttrs.Values).ToArray();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.changedInventorySlot = m_targetInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CreatureVariationDateCount(m_myHero.id, m_myHero.dailyCreatureVariationCount.date, m_myHero.dailyCreatureVariationCount.value));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroCreatureBaseAttrs(m_heroCreature.instanceId));
		foreach (HeroCreatureBaseAttr baseAttr in m_heroCreature.baseAttrs.Values)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroCreatureBaseAttr(baseAttr.creature.instanceId, baseAttr.attr.attr.attrId, baseAttr.baseValue));
		}
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_targetInventorySlot));
		dbWork.Schedule();
	}

	private void SaveToLogDB(int nOldQuality, int nUsedItemId)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureVariationLog(logId, m_heroCreature.instanceId, m_myHero.id, nOldQuality, m_heroCreature.quality, nUsedItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_currentTime));
			foreach (HeroCreatureBaseAttr oldAttr in m_oldBaseAttrs)
			{
				int nAttrId = oldAttr.attr.attr.attrId;
				HeroCreatureBaseAttr newAttr = m_heroCreature.GetBaseAttr(nAttrId);
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureVariationDetailLog(Guid.NewGuid(), logId, nAttrId, oldAttr.baseValue, newAttr.baseValue));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
