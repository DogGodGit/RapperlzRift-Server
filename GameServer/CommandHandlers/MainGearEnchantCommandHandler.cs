using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class MainGearEnchantCommandHandler : InGameCommandHandler<MainGearEnchantCommandBody, MainGearEnchantResponseBody>
{
    public const short kResult_NotEnoughMaterialItem = 101;

    public const short kResult_NotEnoughPenaltyPreventItem = 102;

    public const short kResult_DailyEnchantCountOverflowed = 103;

    private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    private DateTime m_currentDate = DateTime.MinValue.Date;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        m_currentDate = m_currentTime.Date;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        Guid heroMainGearId = m_body.heroMainGearId;
        bool bUsePenaltyPreventItem = m_body.usePenaltyPreventItem;
        HeroMainGear heroMainGear = m_myHero.GetMainGear(heroMainGearId);
        if (heroMainGear == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 메인장비입니다. heroMainGearId = " + heroMainGearId);
        }
        if (heroMainGear.isMaxEnchantLevel)
        {
            throw new CommandHandleException(1, "최대 강화레벨입니다.");
        }
        MainGearEnchantLevel mainGearEnchantLevel = Resource.instance.GetMainGearEnchantLevel(heroMainGear.enchantLevel);
        if (mainGearEnchantLevel == null)
        {
            throw new CommandHandleException(1, string.Concat("존재하지 않는 강화레벨입니다. heroMainGearId = ", heroMainGearId, ", enchantLevel = ", heroMainGear.enchantLevel));
        }
        MainGearEnchantStep mainGearEnchantStep = mainGearEnchantLevel.step;
        int nOldEnchantLevel = heroMainGear.enchantLevel;
        bool bOldOwned = heroMainGear.owned;
        m_myHero.RefreshMainGearEnchantDailyCount(m_currentDate);
        int nMainGearEnchantDailyCount = m_myHero.mainGearEnchantDailyCount.value + 1;
        if (nMainGearEnchantDailyCount > m_myHero.vipLevel.mainGearEnchantMaxCount)
        {
            throw new CommandHandleException(103, "메인장비 일일최대강화횟수를 넘어갑니다.");
        }
        int nNextSuccesccRate = mainGearEnchantLevel.nextSusscessRate;
        bool bPenaltyPreventEnabled = mainGearEnchantLevel.penaltyPreventEnabled;
        int nNextMaterialItemId = mainGearEnchantStep.nextEnchantMaterialItemId;
        int nNextMaterialItemCount = 1;
        int nNextPenaltyPreventItemId = mainGearEnchantStep.nextEnchantPenaltyPreventItemId;
        int nNextPenaltyPreventItemCount = 1;
        if (m_myHero.GetItemCount(nNextMaterialItemId) < nNextMaterialItemCount)
        {
            throw new CommandHandleException(101, "다음강화 재료아이템이 부족합니다.");
        }
        if (bPenaltyPreventEnabled && bUsePenaltyPreventItem && m_myHero.GetItemCount(nNextPenaltyPreventItemId) < nNextPenaltyPreventItemCount)
        {
            throw new CommandHandleException(102, "다음강화 패널티방지아이템이 부족합니다.");
        }
        bool bMainGearOwned = heroMainGear.owned;
        int nNextMaterialItemOwnCount = 0;
        int nNextMaterialItemUnOwnCount = 0;
        int nNextPaneltyPreventItemOwnCount = 0;
        int nNextPaneltyPreventItemUnOwnCount = 0;
        m_myHero.UseItem(nNextMaterialItemId, bMainGearOwned, nNextMaterialItemCount, m_changedInventorySlots, out nNextMaterialItemOwnCount, out nNextMaterialItemUnOwnCount);
        if (bPenaltyPreventEnabled && bUsePenaltyPreventItem)
        {
            m_myHero.UseItem(nNextPenaltyPreventItemId, bMainGearOwned, nNextPenaltyPreventItemCount, m_changedInventorySlots, out nNextPaneltyPreventItemOwnCount, out nNextPaneltyPreventItemUnOwnCount);
        }
        if (nNextMaterialItemOwnCount > 0 || nNextPaneltyPreventItemOwnCount > 0)
        {
            heroMainGear.owned = true;
        }
        bool bSuccess = Util.DrawLots(nNextSuccesccRate);
        if (!bSuccess)
        {
            if (heroMainGear.enchantLevel > 0 && (!bPenaltyPreventEnabled || !bUsePenaltyPreventItem))
            {
                heroMainGear.enchantLevel--;
            }
        }
        else
        {
            heroMainGear.enchantLevel++;
        }
        if (nOldEnchantLevel != heroMainGear.enchantLevel)
        {
            heroMainGear.RefreshAttrTotalValues();
            if (heroMainGear.isEquipped)
            {
                m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
            }
        }
        m_myHero.mainGearEnchantDailyCount.value = nMainGearEnchantDailyCount;
        if (heroMainGear.isEquipped && heroMainGear.enchantLevel > m_myHero.maxEquippedMainGearEnchantLevel)
        {
            m_myHero.maxEquippedMainGearEnchantLevel = heroMainGear.enchantLevel;
        }
        SaveToDB(heroMainGear);
        SaveToDB_AddMainGearEnchantLog(heroMainGear, nOldEnchantLevel, bOldOwned, bSuccess, nNextMaterialItemId, nNextMaterialItemOwnCount, nNextMaterialItemUnOwnCount, nNextPenaltyPreventItemId, nNextPaneltyPreventItemOwnCount, nNextPaneltyPreventItemUnOwnCount);
        MainGearEnchantResponseBody resBody = new MainGearEnchantResponseBody();
        resBody.date = m_currentDate;
        resBody.isSuccess = bSuccess;
        resBody.enchantLevel = heroMainGear.enchantLevel;
        resBody.maxEquippedMainGearEnchantLevel = m_myHero.maxEquippedMainGearEnchantLevel;
        resBody.enchantDailyCount = m_myHero.mainGearEnchantDailyCount.value;
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
        resBody.maxHp = m_myHero.realMaxHP;
        resBody.hp = m_myHero.hp;
        SendResponseOK(resBody);
        if (bSuccess && Resource.instance.CheckSystemMessageCondition(2, heroMainGear.enchantLevel))
        {
            SystemMessage.SendMainGearEnchantment(m_myHero, heroMainGear);
        }
    }

    private void SaveToDB(HeroMainGear heroMainGear)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_MainGearEnchantDateCount(m_myHero.id, m_myHero.mainGearEnchantDailyCount.date, m_myHero.mainGearEnchantDailyCount.value));
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
        }
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMainGear_Enchant(heroMainGear.id, heroMainGear.enchantLevel, heroMainGear.owned));
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_MaxEquippedGearEnchantLevel(m_myHero.id, m_myHero.maxEquippedMainGearEnchantLevel));
        dbWork.Schedule();
    }

    private void SaveToDB_AddMainGearEnchantLog(HeroMainGear heroMainGear, int nOldEnchantLevel, bool bOldOwned, bool bIsSuccess, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nPenaltyPreventItemId, int nPenaltyPreventItemOwnCount, int nPenaltyPreventItemUnOwnCount)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddHeroMainGearEnchantLog(Guid.NewGuid(), heroMainGear.hero.id, heroMainGear.id, nOldEnchantLevel, heroMainGear.enchantLevel, bOldOwned, heroMainGear.owned, bIsSuccess, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, nPenaltyPreventItemId, nPenaltyPreventItemOwnCount, nPenaltyPreventItemUnOwnCount, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
