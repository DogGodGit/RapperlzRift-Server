using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class MountLevelUpCommandHandler : InGameCommandHandler<MountLevelUpCommandBody, MountLevelUpResponseBody>
{
    public const short kResult_NotEnoughItem = 101;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nMountId = m_body.mountId;
        if (nMountId <= 0)
        {
            throw new CommandHandleException(1, "탈것ID가 유효하지 않습니다. nMountId = " + nMountId);
        }
        HeroMount targetHeroMount = m_myHero.GetMount(nMountId);
        if (targetHeroMount == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 영웅탈것입니다. nMountId = " + nMountId);
        }
        if (targetHeroMount.isMaxLevel)
        {
            throw new CommandHandleException(1, "이미 최대레벨에 도달한 탈것입니다.");
        }
        Item mountLevelUpItem = Resource.instance.mountLevelUpItem;
        bool bFirstUseOwned = true;
        if (m_myHero.GetItemCount(mountLevelUpItem.id) < 1)
        {
            throw new CommandHandleException(101, "탈것레벨업아이템 수량이 부족합니다.");
        }
        int nUsedOwnCount = 0;
        int nUsedUnOwnCount = 0;
        List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
        m_myHero.UseItem(mountLevelUpItem.id, bFirstUseOwned, 1, changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
        InventorySlot targetInventorySlot = changedInventorySlots[0];
        int nOldLevel = targetHeroMount.level;
        int nOldSatiety = targetHeroMount.satiety;
        int nSelectSatiety = SFRandom.Next(mountLevelUpItem.value1, mountLevelUpItem.value2 + 1);
        targetHeroMount.AddSatiety(nSelectSatiety);
        if (nOldLevel != targetHeroMount.level)
        {
            targetHeroMount.RefreshAttrTotalValues();
            m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
            if (m_myHero.isRiding)
            {
                Place currentPlace = m_myHero.currentPlace;
                if (currentPlace != null)
                {
                    ServerEvent.SendHeroMountLevelUp(currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.id, targetHeroMount.mount.id, targetHeroMount.level);
                }
            }
        }
        SaveToDB(targetInventorySlot, targetHeroMount, mountLevelUpItem.id, nUsedOwnCount, nUsedUnOwnCount, nOldLevel, nOldSatiety);
        MountLevelUpResponseBody resBody = new MountLevelUpResponseBody();
        resBody.changedInventorySlot = targetInventorySlot.ToPDInventorySlot();
        resBody.mountLevel = targetHeroMount.level;
        resBody.mountSatiety = targetHeroMount.satiety;
        resBody.maxHp = m_myHero.realMaxHP;
        SendResponseOK(resBody);
    }

    private void SaveToDB(InventorySlot targetInventorySlot, HeroMount heroMount, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nOldLevel, int nOldSatiety)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(targetInventorySlot));
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMount_Level(heroMount.hero.id, heroMount.mount.id, heroMount.level, heroMount.satiety));
        dbWork.Schedule();
        SaveToDB_AddHeroMountLevelUpLog(heroMount, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, nOldLevel, nOldSatiety);
    }

    private void SaveToDB_AddHeroMountLevelUpLog(HeroMount heroMount, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nOldLevel, int nOldSatiety)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddHeroMountLevelUpLog(Guid.NewGuid(), m_myHero.id, heroMount.mount.id, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, nOldLevel, heroMount.level, nOldSatiety, heroMount.satiety, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
