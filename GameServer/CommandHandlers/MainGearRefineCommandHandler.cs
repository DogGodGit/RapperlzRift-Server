using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class MainGearRefineCommandHandler : InGameCommandHandler<MainGearRefineCommandBody, MainGearRefineResponseBody>
{
    public const short kResult_NotEnoughMaterialItem = 101;

    public const short kResult_NotEnoughProtectionItem = 102;

    public const short kResult_DailyRefinementCountOverrflowed = 103;

    private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    private DateTime m_currentDate = DateTime.MinValue.Date;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        m_currentDate = m_currentTime.Date;
        Guid heromMainGearId = m_body.heroMainGearId;
        bool bIsSingleRefinement = m_body.isSingleRefinement;
        int[] protectedIndices = m_body.protectedIndices;
        if (protectedIndices == null)
        {
            throw new CommandHandleException(1, "보호인덱스 목록이 유효하지 않습니다.");
        }
        int nRefinementCount = bIsSingleRefinement ? 1 : 3;
        HeroMainGear heroMainGear = m_myHero.GetMainGear(heromMainGearId);
        if (heroMainGear == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 영웅메인장비입니다. heromMainGearId = " + heromMainGearId);
        }
        List<int> protectedOptionAttrIndices = new List<int>();
        int[] array = protectedIndices;
        foreach (int nIndex in array)
        {
            HeroMainGearOptionAttr attr = heroMainGear.GetOptionAttr(nIndex);
            if (attr == null)
            {
                throw new CommandHandleException(1, "속성이 존재하지 않습니다. nIndex = " + nIndex);
            }
            if (protectedOptionAttrIndices.Contains(attr.index))
            {
                throw new CommandHandleException(1, "중복된 인덱스입니다. nIndex = " + nIndex);
            }
            protectedOptionAttrIndices.Add(attr.index);
        }
        int nProtectionCount = protectedOptionAttrIndices.Count;
        m_myHero.RefreshMainGearRefinementDailyCount(m_currentDate);
        int nMainGearRefinementDailyCount = m_myHero.mainGearRefinementDailyCount.value + nRefinementCount;
        if (nMainGearRefinementDailyCount > m_myHero.vipLevel.mainGearRefinementMaxCount)
        {
            throw new CommandHandleException(103, "메인장비 일일최대세련횟수를 넘어갑니다.");
        }
        int nMaterialItemId = Resource.instance.mainGearRefinementItemId;
        int nProtectionItemId = 0;
        if (nProtectionCount > 0)
        {
            MainGearRefinementRecipe recipe = Resource.instance.GetMainGearRefinementRecipe(nProtectionCount);
            if (recipe == null)
            {
                throw new CommandHandleException(1, "존재하지않는 메인장비세련레시피입니다. nProtectionCount = " + nProtectionCount);
            }
            nProtectionItemId = recipe.protectionItemId;
        }
        if (m_myHero.GetItemCount(nMaterialItemId) < nRefinementCount)
        {
            throw new CommandHandleException(101, "재료아이템이 부족합니다.");
        }
        if (nProtectionItemId > 0 && m_myHero.GetItemCount(nProtectionItemId) < nRefinementCount)
        {
            throw new CommandHandleException(102, "보호아이템이 부족합니다.");
        }
        bool bOldOwned = heroMainGear.owned;
        MainGearTier mainGearTier = heroMainGear.gear.tier;
        int nMainGearGrade = heroMainGear.gear.grade.id;
        MainGearOptionAttrPool mainGearOptionAttrPool = mainGearTier.GetOptionAttrPool(nMainGearGrade);
        heroMainGear.ClearRefinement();
        for (int nTargetTurn = 1; nTargetTurn <= nRefinementCount; nTargetTurn++)
        {
            HeroMainGearRefinement refinement = new HeroMainGearRefinement(heroMainGear, nTargetTurn);
            foreach (HeroMainGearOptionAttr mainGearOptionAttr in heroMainGear.optionAttrs)
            {
                int nIndex2 = mainGearOptionAttr.index;
                HeroMainGearRefinementAttr refinementAttr = new HeroMainGearRefinementAttr(nIndex2);
                if (protectedOptionAttrIndices.Contains(nIndex2))
                {
                    refinementAttr.SetAttrValue(mainGearOptionAttr.attrGrade, mainGearOptionAttr.attrId, mainGearOptionAttr.attrValue);
                    refinementAttr.isProtected = true;
                }
                else
                {
                    MainGearOptionAttrPoolEntry newOptionAttr = mainGearOptionAttrPool.SelectEntry();
                    refinementAttr.SetAttrValue(newOptionAttr.attrGrade, newOptionAttr.attrId, newOptionAttr.attrValue);
                    refinementAttr.isProtected = false;
                }
                refinement.AddAttr(refinementAttr);
            }
            heroMainGear.AddRefinement(refinement);
        }
        bool bFirstUsedOwn = heroMainGear.owned;
        int nMaterialItemOwnCount = 0;
        int nMaterialItemUnOwnCount = 0;
        int nProtectionItemOwnCount = 0;
        int nProtectionItemUnOwnCount = 0;
        m_myHero.UseItem(nMaterialItemId, bFirstUsedOwn, nRefinementCount, m_changedInventorySlots, out nMaterialItemOwnCount, out nMaterialItemUnOwnCount);
        if (nProtectionItemId > 0)
        {
            m_myHero.UseItem(nProtectionItemId, bFirstUsedOwn, nRefinementCount, m_changedInventorySlots, out nProtectionItemOwnCount, out nProtectionItemUnOwnCount);
        }
        m_myHero.mainGearRefinementDailyCount.value += nRefinementCount;
        if (nMaterialItemOwnCount > 0 || nProtectionItemOwnCount > 0)
        {
            heroMainGear.owned = true;
        }
        SaveToDB(heroMainGear, bOldOwned, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, nProtectionItemId, nProtectionItemOwnCount, nProtectionItemUnOwnCount);
        MainGearRefineResponseBody resBody = new MainGearRefineResponseBody();
        resBody.date = m_currentDate;
        resBody.refinementDailyCount = m_myHero.mainGearRefinementDailyCount.value;
        resBody.refinements = HeroMainGearRefinement.ToPDHeroMainGearRefinements(heroMainGear.refinements).ToArray();
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
        SendResponseOK(resBody);
    }

    private void SaveToDB(HeroMainGear heroMainGear, bool bOldOwned, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nProtectionItemId, int nProtectionItemOwnCount, int nProtectionItemUnOwnCount)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_MainGearRefinementCount(m_myHero.id, m_myHero.mainGearRefinementDailyCount.date, m_myHero.mainGearRefinementDailyCount.value));
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
        }
        dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroMainGearRefinementAttrs(heroMainGear.id));
        foreach (HeroMainGearRefinement refinement in heroMainGear.refinements)
        {
            foreach (HeroMainGearRefinementAttr attr in refinement.attrs)
            {
                dbWork.AddSqlCommand(GameDac.CSC_AddHeroMainGearRefinementAttr(refinement.heroMainGear.id, refinement.turn, attr.index, attr.grade, attr.attrId, attr.attrValue.id));
            }
        }
        dbWork.Schedule();
        SaveToDB_AddHeroMainGearRefinementLog(heroMainGear, bOldOwned, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, nProtectionItemId, nProtectionItemOwnCount, nProtectionItemUnOwnCount);
    }

    private void SaveToDB_AddHeroMainGearRefinementLog(HeroMainGear heroMainGear, bool bOldOwned, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount, int nProtectionItemId, int nProtectionItemOwnCount, int nProtectionItemUnOwnCount)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            Guid logId = Guid.NewGuid();
            logWork.AddSqlCommand(GameLogDac.CSC_AddHeroMainGearRefinementLog(logId, heroMainGear.hero.id, heroMainGear.id, bOldOwned, heroMainGear.owned, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, nProtectionItemId, nProtectionItemOwnCount, nProtectionItemUnOwnCount, m_currentTime));
            foreach (HeroMainGearRefinement refinement in heroMainGear.refinements)
            {
                foreach (HeroMainGearRefinementAttr attr in refinement.attrs)
                {
                    logWork.AddSqlCommand(GameLogDac.CSC_AddHeroMainGearRefinmenetDetailLog(Guid.NewGuid(), logId, attr.refinement.turn, attr.index, attr.grade, attr.attrId, attr.attrValue.id, attr.isProtected));
                }
            }
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
