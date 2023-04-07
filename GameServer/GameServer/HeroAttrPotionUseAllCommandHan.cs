using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class HeroAttrPotionUseAllCommandHandler : InGameCommandHandler<HeroAttrPotionUseAllCommandBody, HeroAttrPotionUseAllResponseBody>
{
	private class HeroPotionAttrLog
	{
		public int potionAttrId;

		public int count;

		public int usedItemId;

		public int usedItemOwnCount;

		public int usedItemUnOwnCount;

		public HeroPotionAttrLog(int nPotionAttrId, int nCount, int nUsedItemId, int nUsedItemOwnCount, int nUsedItemUnOwnCount)
		{
			potionAttrId = nPotionAttrId;
			count = nCount;
			usedItemId = nUsedItemId;
			usedItemOwnCount = nUsedItemOwnCount;
			usedItemUnOwnCount = nUsedItemUnOwnCount;
		}
	}

	private List<HeroPotionAttr> m_heroPotionAttrs = new List<HeroPotionAttr>();

	private HashSet<InventorySlot> m_changedInvetorySlots = new HashSet<InventorySlot>();

	private List<HeroPotionAttrLog> m_logs = new List<HeroPotionAttrLog>();

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		JobLevelMaster jobLevelMaster = m_myHero.job.GetLevel(m_myHero.level).master;
		foreach (PotionAttr potionAttr in Resource.instance.potionAttrs.Values)
		{
			HeroPotionAttr heroPotionAttr = m_myHero.GetOrCreatePotiaonAttr(potionAttr);
			int nRemainingCount = jobLevelMaster.potionAttrMaxCount - heroPotionAttr.count;
			int nRequiredItemId = potionAttr.requiredItemId;
			int nItemCount = m_myHero.GetItemCount(nRequiredItemId);
			int nUseCount = ((nRemainingCount < nItemCount) ? nRemainingCount : nItemCount);
			if (nUseCount > 0)
			{
				int nUsedItemOwnCount = 0;
				int nUsedItemUnOwnCount = 0;
				m_myHero.UseItem(nRequiredItemId, bFisetUseOwn: true, nUseCount, m_changedInvetorySlots, out nUsedItemOwnCount, out nUsedItemUnOwnCount);
				heroPotionAttr.count += nUseCount;
				m_heroPotionAttrs.Add(heroPotionAttr);
				m_logs.Add(new HeroPotionAttrLog(potionAttr.id, nUseCount, nRequiredItemId, nUsedItemOwnCount, nUsedItemUnOwnCount));
			}
		}
		if (m_heroPotionAttrs.Count > 0)
		{
			m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
			SaveToDB();
			SaveToLogDB();
		}
		HeroAttrPotionUseAllResponseBody resBody = new HeroAttrPotionUseAllResponseBody();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.changedPotionAttrs = HeroPotionAttr.ToPDHeroPotionAttrs(m_heroPotionAttrs).ToArray();
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInvetorySlots).ToArray();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		foreach (HeroPotionAttr attr in m_heroPotionAttrs)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroPotionAttr(attr.hero.id, attr.potionAttr.id, attr.count));
		}
		foreach (InventorySlot slot in m_changedInvetorySlots)
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
			foreach (HeroPotionAttrLog log in m_logs)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroPotionAttrLog(Guid.NewGuid(), m_myHero.id, log.potionAttrId, log.count, log.usedItemId, log.usedItemOwnCount, log.usedItemUnOwnCount, m_currentTime));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
