using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class SubGearGradeUpCommandHandler : InGameCommandHandler<SubGearGradeUpCommandBody, SubGearGradeUpResponseBody>
{
    public const short kResult_LastLevel = 101;

    public const short kResult_QualityUpRequired = 102;

    public const short kResult_NotMaxLevelOfCurrentGrade = 103;

    public const short kResult_NotEnoughItem = 104;

    private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

    private HeroSubGear m_heroSubGear;

    private int m_nOldLevel;

    private int m_nOldGrade;

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
            throw new CommandHandleException(1, "보조장비ID가 유효하지 않습니다. nSubGearId = " + nSubGearId);
        }
        m_heroSubGear = m_myHero.GetSubGear(nSubGearId);
        if (m_heroSubGear == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 영웅보조장비입니다. nSubGearId = " + nSubGearId);
        }
        if (!m_heroSubGear.equipped)
        {
            throw new CommandHandleException(1, "장착중인 영웅보조장비가 아닙니다.");
        }
        if (m_heroSubGear.level >= m_myHero.level)
        {
            throw new CommandHandleException(1, "보조장비 레벨은 영웅 레벨을 넘어갈 수 없습니다.");
        }
        if (m_heroSubGear.isLastLevel)
        {
            throw new CommandHandleException(101, "최대레벨입니다.");
        }
        if (!m_heroSubGear.isLastQualityOfCurrentLevel)
        {
            throw new CommandHandleException(102, "품질을 올려야합니다.");
        }
        if (!m_heroSubGear.isLastLevelOfCurrentGrade)
        {
            throw new CommandHandleException(103, "현재 등급의 마지막 레벨이 아닙니다.");
        }
        m_nOldLevel = m_heroSubGear.level;
        m_nOldGrade = m_heroSubGear.grade;
        SubGearLevel currentSubGearLevel = m_heroSubGear.subGearLevel;
        int nGradeUpItem1Id = currentSubGearLevel.nextGradeUpItem1Id;
        int nGradeUpItem1Count = 0;
        if (nGradeUpItem1Id > 0)
        {
            nGradeUpItem1Count = currentSubGearLevel.nextGradeUpItem1Count;
            if (m_myHero.GetItemCount(nGradeUpItem1Id) < nGradeUpItem1Count)
            {
                throw new CommandHandleException(104, "아이템이 부족합니다.");
            }
        }
        int nGradeUpItem2Id = currentSubGearLevel.nextGradeUpItem2Id;
        int nGradeUpItem2Count = 0;
        if (nGradeUpItem2Id > 0)
        {
            nGradeUpItem2Count = currentSubGearLevel.nextGradeUpItem2Count;
            if (m_myHero.GetItemCount(nGradeUpItem2Id) < nGradeUpItem2Count)
            {
                throw new CommandHandleException(104, "아이템이 부족합니다.");
            }
        }
        bool bFisetUseOwn = true;
        if (nGradeUpItem1Id > 0)
        {
            m_myHero.UseItem(nGradeUpItem1Id, bFisetUseOwn, nGradeUpItem1Count, m_changedInventorySlots, out m_nMaterialItem1OwnCount, out m_nMaterialItem1UnOwnCount);
            m_nMaterialItem1Id = nGradeUpItem1Id;
        }
        if (nGradeUpItem2Id > 0)
        {
            m_myHero.UseItem(nGradeUpItem2Id, bFisetUseOwn, nGradeUpItem2Count, m_changedInventorySlots, out m_nMaterialItem2OwnCount, out m_nMaterialItem2UnOwnCount);
            m_nMaterialItem2Id = nGradeUpItem2Id;
        }
        SubGearLevel nextSubGearLevel = m_heroSubGear.subGear.GetLevel(currentSubGearLevel.level + 1);
        m_heroSubGear.subGearLevel = nextSubGearLevel;
        m_heroSubGear.quality = nextSubGearLevel.firstQuality.quality;
        m_heroSubGear.RefreshAttrTotalValues();
        m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
        SaveToDB();
        SubGearGradeUpResponseBody resBody = new SubGearGradeUpResponseBody();
        resBody.subGearLevel = m_heroSubGear.level;
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
        SaveToDB_AddSubGearGradeUpLog();
    }

    private void SaveToDB_AddSubGearGradeUpLog()
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddSubGearGradeUpLog(Guid.NewGuid(), m_heroSubGear.hero.id, m_heroSubGear.subGearId, m_nOldLevel, m_heroSubGear.level, m_nOldGrade, m_heroSubGear.grade, m_nMaterialItem1Id, m_nMaterialItem1OwnCount, m_nMaterialItem1UnOwnCount, m_nMaterialItem2Id, m_nMaterialItem2OwnCount, m_nMaterialItem2UnOwnCount, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
