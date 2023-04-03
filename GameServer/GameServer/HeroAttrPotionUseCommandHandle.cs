using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroAttrPotionUseCommandHandler : InGameCommandHandler<HeroAttrPotionUseCommandBody, HeroAttrPotionUseResponseBody>
{
	public const short kResult_NotEnoughItem = 101;

	public const short kResult_OverflowedUseCount = 102;

	private HeroPotionAttr m_heroPotionAttr;

	private int m_nUsedItemId;

	private int m_nUsedItemOwnCount;

	private int m_nUsedItemUnOwnCount;

	private InventorySlot m_changedInventorySlot;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nPotionAttrId = m_body.potionAttrId;
		if (nPotionAttrId < 0)
		{
			throw new CommandHandleException(1, "물약속성ID가 유효하지 않습니다. nPotionAttrId = " + nPotionAttrId);
		}
		PotionAttr potionAttr = Resource.instance.GetPotionAttr(nPotionAttrId);
		if (potionAttr == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 물약속성입니다. nPotionAttrId = " + nPotionAttrId);
		}
		m_nUsedItemId = potionAttr.requiredItemId;
		if (m_myHero.GetItemCount(m_nUsedItemId) < 1)
		{
			throw new CommandHandleException(101, "아이템이 부족합니다.");
		}
		JobLevelMaster jobLevelMaster = m_myHero.job.GetLevel(m_myHero.level).master;
		m_heroPotionAttr = m_myHero.GetOrCreatePotiaonAttr(potionAttr);
		if (m_heroPotionAttr.count >= jobLevelMaster.potionAttrMaxCount)
		{
			throw new CommandHandleException(102, "사용횟수가 최대횟수를 넘어갑니다.");
		}
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		m_myHero.UseItem(m_nUsedItemId, bFisetUseOwn: true, 1, changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
		m_changedInventorySlot = changedInventorySlots[0];
		m_heroPotionAttr.count++;
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SaveToLogDB();
		HeroAttrPotionUseResponseBody resBody = new HeroAttrPotionUseResponseBody();
		resBody.maxHP = m_myHero.realMaxHP;
		resBody.potionAttrCount = m_heroPotionAttr.count;
		resBody.changedInventorySlot = m_changedInventorySlot.ToPDInventorySlot();
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroPotionAttr(m_heroPotionAttr.hero.id, m_heroPotionAttr.potionAttr.id, m_heroPotionAttr.count));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_changedInventorySlot));
		dbWork.Schedule();
	}

	private void SaveToLogDB()
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroPotionAttrLog(Guid.NewGuid(), m_myHero.id, m_heroPotionAttr.potionAttr.id, 1, m_nUsedItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
