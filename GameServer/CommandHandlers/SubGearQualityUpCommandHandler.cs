using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class SubGearQualityUpCommandHandler : InGameCommandHandler<SubGearQualityUpCommandBody, SubGearQualityUpResponseBody>
{
    public const short kResult_AlreadyMaxQualityOfCurrentLevel = 101;

    public const short kResult_NotEnoughItem = 102;

    private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

    private HeroSubGear m_heroSubGear;

    private int m_nOldQuality;

    private int m_nMaterialItem1Id;

    private int m_nMaterialItem1OwnCount;

    private int m_nMaterialItem1UnOwnCount;

    private int m_nMaterialItem2Id;

    private int m_nMaterialItem2OwnCount;

    private int m_nMaterialItem2UnOwnCount;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nSubGearId = m_body.subGearId;
        if (nSubGearId <= 0)
        {
            throw new CommandHandleException(1, "유효하지 않은 보조장비ID 입니다. nSubGearId = " + nSubGearId);
        }
        m_heroSubGear = m_myHero.GetSubGear(nSubGearId);
        if (m_heroSubGear == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 영웅 보조장비 입니다. nSubGearId = " + nSubGearId);
        }
        if (!m_heroSubGear.equipped)
        {
            throw new CommandHandleException(1, "장착하지 않는 보조장비입니다.");
        }
        if (m_heroSubGear.isLastQualityOfCurrentLevel)
        {
            throw new CommandHandleException(101, "이미 현재 레벨의 마지막 품질입니다.");
        }
        m_nOldQuality = m_heroSubGear.quality;
        SubGearLevel currentLevel = m_heroSubGear.subGearLevel;
        SubGearLevelQuality currentQuality = currentLevel.GetQuality(m_heroSubGear.quality);
        int nQualityUpItem1Id = currentQuality.nextQualityUpItem1Id;
        int nQualityUpItem1Count = 0;
        if (nQualityUpItem1Id > 0)
        {
            nQualityUpItem1Count = currentQuality.nextQualityUpItem1Count;
            if (m_myHero.GetItemCount(nQualityUpItem1Id) < nQualityUpItem1Count)
            {
                throw new CommandHandleException(102, "아이템이 부족합니다.");
            }
        }
        int nQualityUpItem2Id = currentQuality.nextQualityUpItem2Id;
        int nQualityUpItem2Count = 0;
        if (nQualityUpItem2Id > 0)
        {
            nQualityUpItem2Count = currentQuality.nextQualityUpItem2Count;
            if (m_myHero.GetItemCount(nQualityUpItem2Id) < nQualityUpItem2Count)
            {
                throw new CommandHandleException(102, "아이템이 부족합니다.");
            }
        }
        bool bFisetUseOwn = true;
        if (nQualityUpItem1Id > 0)
        {
            m_myHero.UseItem(nQualityUpItem1Id, bFisetUseOwn, nQualityUpItem1Count, m_changedInventorySlots, out m_nMaterialItem1OwnCount, out m_nMaterialItem1UnOwnCount);
            m_nMaterialItem1Id = nQualityUpItem1Id;
        }
        if (nQualityUpItem2Id > 0)
        {
            m_myHero.UseItem(nQualityUpItem2Id, bFisetUseOwn, nQualityUpItem2Count, m_changedInventorySlots, out m_nMaterialItem2OwnCount, out m_nMaterialItem2UnOwnCount);
            m_nMaterialItem2Id = nQualityUpItem2Id;
        }
        m_heroSubGear.quality = currentQuality.quality + 1;
        m_heroSubGear.RefreshAttrTotalValues();
        m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
        SaveToDB();
        SubGearQualityUpResponseBody resBody = new SubGearQualityUpResponseBody();
        resBody.subGearQuality = m_heroSubGear.quality;
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
        resBody.maxHp = m_myHero.realMaxHP;
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
        }
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSubGear_LevelUp(m_heroSubGear.hero.id, m_heroSubGear.subGearId, m_heroSubGear.level, m_heroSubGear.quality));
        dbWork.Schedule();
        SaveToDB_AddSubGearQualityUpLog();
    }

    private void SaveToDB_AddSubGearQualityUpLog()
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddSubGearQualityUpLog(Guid.NewGuid(), m_heroSubGear.hero.id, m_heroSubGear.subGearId, m_heroSubGear.level, m_nOldQuality, m_heroSubGear.quality, m_nMaterialItem1Id, m_nMaterialItem1OwnCount, m_nMaterialItem1UnOwnCount, m_nMaterialItem2Id, m_nMaterialItem2OwnCount, m_nMaterialItem2UnOwnCount, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
