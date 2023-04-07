using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class SkillLevelUpCommandHandler : InGameCommandHandler<SkillLevelUpCommandBody, SkillLevelUpResponseBody>
{
    public const short kResult_NotEnoughGold = 101;

    public const short kResult_NotEnoughItem = 102;

    private List<InventorySlot> m_changedInventorySlots = new List<InventorySlot>();

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nJobSkillId = m_body.skillId;
        if (nJobSkillId <= 0)
        {
            throw new CommandHandleException(1, "직업스킬 ID가 유효하지 않습니다. nJobSkillId = " + nJobSkillId);
        }
        HeroSkill heroSkill = m_myHero.GetSkill(nJobSkillId);
        if (heroSkill == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 영웅스킬입니다. nJobSkillId = " + nJobSkillId);
        }
        if (!heroSkill.isOpened)
        {
            throw new CommandHandleException(1, "사용할 수 없는 스킬입니다.");
        }
        if (heroSkill.isMaxLevel)
        {
            throw new CommandHandleException(1, "최대 레벨입니다.");
        }
        int nOldLevel = heroSkill.level;
        JobSkillMaster skillMaster = heroSkill.skill.skillMaster;
        JobSkillLevelMaster currentSkillLevelMaster = skillMaster.GetLevel(heroSkill.level);
        if (m_myHero.level < currentSkillLevelMaster.nextLevelUpRequiredHeroLevel)
        {
            throw new CommandHandleException(1, "영웅레벨이 부족합니다.");
        }
        long lnNextLevelUpGold = currentSkillLevelMaster.nextLevelUpGold;
        int nNextLevelUpItemId = currentSkillLevelMaster.nextLevelUpItemId;
        int nNextLevelUpItemCount = currentSkillLevelMaster.nextLevelUpItemCount;
        if (m_myHero.gold < lnNextLevelUpGold)
        {
            throw new CommandHandleException(101, "골드가 부족합니다.");
        }
        if (nNextLevelUpItemId > 0 && m_myHero.GetItemCount(nNextLevelUpItemId) < nNextLevelUpItemCount)
        {
            throw new CommandHandleException(102, "아이템 수량이 부족합니다.");
        }
        int nUsedOwnCount = 0;
        int nUsedUnOwnCount = 0;
        if (nNextLevelUpItemId > 0)
        {
            m_myHero.UseItem(nNextLevelUpItemId, bFisetUseOwn: true, nNextLevelUpItemCount, m_changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
        }
        m_myHero.UseGold(lnNextLevelUpGold);
        heroSkill.level++;
        m_myHero.RefreshBattlePower();
        SaveToDB(heroSkill, nOldLevel, lnNextLevelUpGold, nNextLevelUpItemId, nUsedOwnCount, nUsedUnOwnCount);
        SkillLevelUpResponseBody resBody = new SkillLevelUpResponseBody();
        resBody.skillLevel = heroSkill.level;
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
        resBody.gold = m_myHero.gold;
        SendResponseOK(resBody);
    }

    private void SaveToDB(HeroSkill heroSkill, int nOldLevel, long lnUsedGold, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_myHero.id, m_myHero.gold));
        foreach (InventorySlot slot in m_changedInventorySlots)
        {
            dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
        }
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSkill_Level(heroSkill.hero.id, heroSkill.skillId, heroSkill.level));
        dbWork.Schedule();
        SaveToDB_AddHeroSkillLevelUpLog(heroSkill, nOldLevel, lnUsedGold, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount);
    }

    private void SaveToDB_AddHeroSkillLevelUpLog(HeroSkill heroSkill, int nOldLevel, long lnUsedGold, int nMaterialItemId, int nMaterialItemOwnCount, int nMaterialItemUnOwnCount)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddHeroSkillLevelUpLog(Guid.NewGuid(), heroSkill.hero.id, heroSkill.skillId, nOldLevel, heroSkill.level, lnUsedGold, nMaterialItemId, nMaterialItemOwnCount, nMaterialItemUnOwnCount, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
