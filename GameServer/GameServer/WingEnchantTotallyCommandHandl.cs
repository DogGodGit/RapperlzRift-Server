using System;
using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class WingEnchantTotallyCommandHandler : InGameCommandHandler<WingEnchantTotallyCommandBody, WingEnchantTotallyResponseBody>
{
	private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

	private HeroWing m_addedWing;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nWingPartId = m_body.wingPartId;
		if (nWingPartId <= 0)
		{
			throw new CommandHandleException(1, "날개파트ID가 유효하지 않습니다.");
		}
		HeroWingPart heroWingPart = m_myHero.GetWingPart(nWingPartId);
		if (heroWingPart == null)
		{
			throw new CommandHandleException(1, "영웅날개파트가 존재하지 않습니다. nWingPartId = " + nWingPartId);
		}
		WingStep wingStep = m_myHero.wingStep;
		WingStepLevel wingStepLevel = m_myHero.wingStepLevel;
		int nOldWingExp = m_myHero.wingExp;
		if (wingStep.isLastStep && wingStepLevel.isLastLevel)
		{
			throw new CommandHandleException(1, "날개 강화 단계 레벨이 이미 최대로 도달했습니다.");
		}
		int nWingEnchantItemId = Resource.instance.wingEnchantItemId;
		int nWingEnchantItemCount = wingStep.enchantMaterialItemCount;
		int nTotalUsedOwnCount = 0;
		int nTotalUsedUnOwnCount = 0;
		int nTotalPickEnchantCount = 0;
		HeroWingEnchant heroWingEnchant = null;
		WingStepLevel currentWingStepLevel;
		do
		{
			int nTotalEnchantLimitCount = wingStepLevel.accEnchantLimitCount;
			int nTotalEnchantCount = heroWingPart.totalEnchantCount;
			int nRemainingEnchantCount = nTotalEnchantLimitCount - nTotalEnchantCount;
			if (nRemainingEnchantCount <= 0 || m_myHero.GetItemCount(nWingEnchantItemId) < nWingEnchantItemCount)
			{
				break;
			}
			if (heroWingEnchant == null)
			{
				heroWingEnchant = heroWingPart.GetOrCreateEnchant(wingStepLevel);
			}
			int nUsedOwnCount = 0;
			int nUsedUnOwnCount = 0;
			m_myHero.UseItem(nWingEnchantItemId, bFisetUseOwn: true, nWingEnchantItemCount, m_changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
			WingEnchantCountPoolEntry wingEnchantCountPoolEntry = Resource.instance.wingEnchantCountPool.SelectEntry();
			int nSelectEnchantCount = wingEnchantCountPoolEntry.count;
			int nEnchantCount = ((nSelectEnchantCount < nRemainingEnchantCount) ? nSelectEnchantCount : nRemainingEnchantCount);
			int nRequiredNextLevelUpExp = m_myHero.wingStepLevel.nextLevelUpRequiredExp - m_myHero.wingExp;
			int nRequiredNextLevelUpCount = (nRequiredNextLevelUpExp - 1) / Resource.instance.wingEnchantExp + 1;
			if (nEnchantCount > nRequiredNextLevelUpCount)
			{
				nEnchantCount = nRequiredNextLevelUpCount;
			}
			heroWingEnchant.AddEnchantCount(nEnchantCount);
			int nWingExpAmount = nEnchantCount * Resource.instance.wingEnchantExp;
			m_myHero.AddWingEnchantExp(nWingExpAmount);
			nTotalUsedOwnCount += nUsedOwnCount;
			nTotalUsedUnOwnCount += nUsedUnOwnCount;
			nTotalPickEnchantCount += nEnchantCount;
			currentWingStepLevel = m_myHero.wingStepLevel;
			WingStep currentWingStep = currentWingStepLevel.step;
			if (currentWingStep.step > wingStep.step)
			{
				Wing rewardWing = currentWingStep.rewardWing;
				if (rewardWing != null && !m_myHero.ContainsWing(rewardWing.id))
				{
					m_addedWing = new HeroWing(m_myHero, rewardWing);
					m_addedWing.Init();
					m_myHero.AddWing(m_addedWing);
				}
				break;
			}
		}
		while (currentWingStepLevel.level == wingStepLevel.level);
		if (heroWingEnchant != null)
		{
			heroWingPart.RefreshAttrTotalValues();
			m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
			SaveToDB(heroWingEnchant, nTotalPickEnchantCount, nWingEnchantItemId, nTotalUsedOwnCount, nTotalUsedUnOwnCount, wingStep.step, wingStepLevel.level, nOldWingExp);
		}
		WingEnchantTotallyResponseBody resBody = new WingEnchantTotallyResponseBody();
		if (heroWingEnchant != null)
		{
			resBody.changedEnchant = heroWingEnchant.ToPDHeroWingEnchant();
		}
		resBody.wingStep = m_myHero.wingStep.step;
		resBody.wingLevel = m_myHero.wingStepLevel.level;
		resBody.wingExp = m_myHero.wingExp;
		resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
		if (m_addedWing != null)
		{
			resBody.addedWing = m_addedWing.ToPDHeroWing();
		}
		resBody.maxHp = m_myHero.realMaxHP;
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroWingEnchant heroWingEnchant, int nPickEnchantCount, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nOldStep, int nOldLevel, int nOldExp)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_EnchantWing(m_myHero.id, m_myHero.wingStep.step, m_myHero.wingStepLevel.level, m_myHero.wingExp));
		foreach (InventorySlot slot in m_changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		if (m_addedWing != null)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroWing(m_addedWing.hero.id, m_addedWing.wing.id, m_addedWing.memoryPieceStep));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroWingEnchant(heroWingEnchant.part.hero.id, heroWingEnchant.part.part.id, heroWingEnchant.step.step, heroWingEnchant.stepLevel.level, heroWingEnchant.enchantCount));
		dbWork.Schedule();
		SaveToDB_AddHeroWingEnchantLog(heroWingEnchant, nPickEnchantCount, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, nOldStep, nOldLevel, nOldExp);
	}

	private void SaveToDB_AddHeroWingEnchantLog(HeroWingEnchant heroWingEnchant, int nPickEnchantCount, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nOldStep, int nOldLevel, int nOldExp)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWingEnchantLog(Guid.NewGuid(), m_myHero.id, heroWingEnchant.part.part.id, nPickEnchantCount, heroWingEnchant.enchantCount, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, nOldStep, nOldLevel, nOldExp, m_myHero.wingStep.step, m_myHero.wingStepLevel.level, m_myHero.wingExp, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
