using System;
using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class CostumeEnchantCommandHandler : InGameCommandHandler<CostumeEnchantCommandBody, CostumeEnchantResponseBody>
{
    public const short kResult_NotEnoughItem = 101;

    private HeroCostume m_heroCostume;

    private int m_nOldEnchantLevel;

    private int m_nOldLuckyValue;

    private int m_nUsedItemId;

    private int m_nUsedItemOwnCount;

    private int m_nUsedItemUnOwnCount;

    private HashSet<InventorySlot> m_changedInventorySlots = new HashSet<InventorySlot>();

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nCostumeId = m_body.costumeId;
        if (nCostumeId <= 0)
        {
            throw new CommandHandleException(1, "코스튬ID가 유효하지 않습니다. nCostumeId = " + nCostumeId);
        }
        m_heroCostume = m_myHero.GetCostume(nCostumeId);
        if (m_heroCostume == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 영웅코스튬입니다. nCostumeId = " + nCostumeId);
        }
        if (m_heroCostume.enchantLevel >= Resource.instance.lastCostumeEnchantLevel)
        {
            throw new CommandHandleException(1, "영웅코스튬강화레벨이 마지막레벨입니다.");
        }
        Item costumeEnchantItem = Resource.instance.costumeEnchantItem;
        m_nUsedItemId = costumeEnchantItem.id;
        CostumeEnchantLevel costumeEnchantLevel = Resource.instance.GetCostumeEnchantLevel(m_heroCostume.enchantLevel);
        int nRequiredItemCount = costumeEnchantLevel.nextLevelUpRequiredItemCount;
        if (m_myHero.GetItemCount(m_nUsedItemId) < nRequiredItemCount)
        {
            throw new CommandHandleException(101, "코스튬강화아이템이 부족합니다.");
        }
        m_nOldEnchantLevel = m_heroCostume.enchantLevel;
        m_nOldLuckyValue = m_heroCostume.luckyValue;
        bool bSuccess = false;
        int nLuckyValue = m_heroCostume.luckyValue;
        int nNextLevelUpMaxLuckyValue = costumeEnchantLevel.nextLevelUpMaxLuckyValue;
        bSuccess = nLuckyValue >= nNextLevelUpMaxLuckyValue || Util.DrawLots(costumeEnchantLevel.nextLevelUpSuccessRate);
        if (bSuccess)
        {
            m_heroCostume.enchantLevel++;
            m_heroCostume.luckyValue = 0;
        }
        else
        {
            m_heroCostume.luckyValue += costumeEnchantItem.value1;
        }
        m_myHero.UseItem(costumeEnchantItem.id, bFisetUseOwn: true, nRequiredItemCount, m_changedInventorySlots, out m_nUsedItemOwnCount, out m_nUsedItemUnOwnCount);
        m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
        SaveToDB();
        SaveToLogDB();
        CostumeEnchantResponseBody resBody = new CostumeEnchantResponseBody();
        resBody.enchantLevel = m_heroCostume.enchantLevel;
        resBody.luckyValue = m_heroCostume.luckyValue;
        resBody.maxHP = m_myHero.realMaxHP;
        resBody.changedInventorySlots = InventorySlot.ToPDInventorySlots(m_changedInventorySlots).ToArray();
        SendResponseOK(resBody);
        if (bSuccess && Resource.instance.CheckSystemMessageCondition(6, m_heroCostume.enchantLevel))
        {
            SystemMessage.SendCostumeEnchantment(m_myHero, m_heroCostume);
        }
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroCostume_EnchantLevel(m_heroCostume.hero.id, m_heroCostume.costume.id, m_heroCostume.enchantLevel, m_heroCostume.luckyValue));
        foreach (InventorySlot slot in m_changedInventorySlots)
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
            logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCostumeEnchantLog(Guid.NewGuid(), m_myHero.id, m_heroCostume.costumeId, m_nOldEnchantLevel, m_nOldLuckyValue, m_heroCostume.enchantLevel, m_heroCostume.luckyValue, m_nUsedItemId, m_nUsedItemOwnCount, m_nUsedItemUnOwnCount, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
