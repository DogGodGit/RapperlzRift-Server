using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class MountGearRefineCommandHandler : InGameCommandHandler<MountGearRefineCommandBody, MountGearRefineResponseBody>
{
    public const short kResult_NotEnoughItem = 101;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        DateTime currentDate = m_currentTime.Date;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        Guid heroMountGearId = m_body.heroMountGearId;
        int nOptionAttrIndex = m_body.optionAttrIndex;
        if (nOptionAttrIndex < 0)
        {
            throw new CommandHandleException(1, "옵션속성인덱스가 유효하지 않습니다. nOptionAttrIndex = " + nOptionAttrIndex);
        }
        HeroMountGear targetHeroMountGear = m_myHero.GetMountGear(heroMountGearId);
        if (targetHeroMountGear == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 영웅탈것장비입니다. heroMountGearId = " + heroMountGearId);
        }
        HeroMountGearOptionAttr targetHeroMountGearOptionAttr = targetHeroMountGear.GetOptionAttr(nOptionAttrIndex);
        if (targetHeroMountGearOptionAttr == null)
        {
            throw new CommandHandleException(1, string.Concat("존재하지 않는 영웅탈것장비옵션속성 입니다. heroMountGearId = ", heroMountGearId, ", nOptionAttrIndex= ", nOptionAttrIndex));
        }
        int nMountGearRefinementItemId = Resource.instance.mountGearRefinementItemId;
        bool bOldOwned = targetHeroMountGear.owned;
        if (m_myHero.GetItemCount(nMountGearRefinementItemId) < 1)
        {
            throw new CommandHandleException(101, "아이템이 부족합니다.");
        }
        m_myHero.RefreshMountGearDailyRefinementCount(currentDate);
        int nMountGearRefinementDailyCount = m_myHero.mountGearRefinementDailyCount.value + 1;
        if (nMountGearRefinementDailyCount > m_myHero.vipLevel.mountGearRefinementMaxCount)
        {
            throw new CommandHandleException(1, "탈것장비 일일재강화 최대횟수를 넘어갑니다.");
        }
        int nOldAttrGrade = targetHeroMountGearOptionAttr.grade;
        int nOldAttrId = targetHeroMountGearOptionAttr.attrId;
        long lnOldAttrValueId = targetHeroMountGearOptionAttr.attrValue.id;
        MountGear mountGear = targetHeroMountGear.gear;
        MountGearOptionAttrPoolEntry entry = mountGear.SelectOptionAttrPoolEntry();
        targetHeroMountGearOptionAttr.SetOptionAttr(entry.attrGrade, entry.attrId, entry.attrValue);
        int nUsedOwnCount = 0;
        int nUsedUnOwnCount = 0;
        List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
        m_myHero.UseItem(nMountGearRefinementItemId, bOldOwned, 1, changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
        InventorySlot targetInventorySlot = changedInventorySlots[0];
        if (nUsedOwnCount > 0)
        {
            targetHeroMountGear.owned = true;
        }
        targetHeroMountGear.RefreshAttrTotalValues();
        if (targetHeroMountGear.isEquipped)
        {
            m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
        }
        m_myHero.mountGearRefinementDailyCount.value = nMountGearRefinementDailyCount;
        SaveToDB(targetInventorySlot, targetHeroMountGearOptionAttr, bOldOwned, nMountGearRefinementItemId, nUsedOwnCount, nUsedUnOwnCount, nOldAttrGrade, nOldAttrId, lnOldAttrValueId);
        MountGearRefineResponseBody resBody = new MountGearRefineResponseBody();
        resBody.date = currentDate;
        resBody.mountGearRefinementDailyCount = m_myHero.mountGearRefinementDailyCount.value;
        resBody.changedHeroMountGearOptionAttr = targetHeroMountGearOptionAttr.ToPDHeroMountGearOptionAttr();
        resBody.changedInventorySlot = targetInventorySlot.ToPDInventorySlot();
        resBody.maxHp = m_myHero.realMaxHP;
        resBody.hp = m_myHero.hp;
        SendResponseOK(resBody);
    }

    private void SaveToDB(InventorySlot inventorySlot, HeroMountGearOptionAttr heroMountGearOptionAttr, bool bOldOwned, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nOldGrade, int nOldAttrId, long lnOldAttrValueId)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_MountGearRefinementDateCount(m_myHero.id, m_myHero.mountGearRefinementDailyCount.date, m_myHero.mountGearRefinementDailyCount.value));
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(inventorySlot));
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMountGearOptionAttr(heroMountGearOptionAttr.gear.id, heroMountGearOptionAttr.index, heroMountGearOptionAttr.grade, heroMountGearOptionAttr.attrId, heroMountGearOptionAttr.attrValue.id));
        dbWork.Schedule();
        SaveToDB_AddHeroMountGearRefinementLog(heroMountGearOptionAttr, bOldOwned, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, nOldGrade, nOldAttrId, lnOldAttrValueId);
    }

    private void SaveToDB_AddHeroMountGearRefinementLog(HeroMountGearOptionAttr heroMountGearOptionAttr, bool bOldOwned, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nOldGrade, int nOldAttrId, long lnOldAttrValueId)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddHeroMountGearRefinementLog(Guid.NewGuid(), heroMountGearOptionAttr.gear.hero.id, heroMountGearOptionAttr.gear.id, bOldOwned, heroMountGearOptionAttr.gear.owned, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, heroMountGearOptionAttr.index, nOldGrade, heroMountGearOptionAttr.grade, nOldAttrId, heroMountGearOptionAttr.attrId, lnOldAttrValueId, heroMountGearOptionAttr.attrValue.id, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
