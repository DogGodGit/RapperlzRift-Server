using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class HpPotionUseCommandHandler : InGameCommandHandler<HpPotionUseCommandBody, HpPotionUseResponseBody>
{
    public const short kResult_Dead = 101;

    public const short kResult_FullHP = 102;

    public const short kResult_NotCoolDownUseHpPotion = 103;

    private InventorySlot m_inventorySlot;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nInventorySlotIndex = m_body.inventorySlotIndex;
        m_inventorySlot = m_myHero.GetInventorySlot(nInventorySlotIndex);
        if (m_inventorySlot == null)
        {
            throw new CommandHandleException(1, "해당 인벤토리슬롯이 존재하지 않습니다. nInventorySlotIndex = " + nInventorySlotIndex);
        }
        if (!(m_inventorySlot.obj is ItemInventoryObject obj))
        {
            throw new CommandHandleException(1, "해당 인벤토리슬롯의 오브젝트가 아이템타입이 아닙니다.");
        }
        Item item = obj.item;
        bool bOwned = obj.owned;
        if (item.type.id != 1)
        {
            throw new CommandHandleException(1, "해당 인벤토리의 아이템이 HP포션이 아닙니다.");
        }
        if (!item.IsUseableHeroLevel(m_myHero.level))
        {
            throw new CommandHandleException(1, "사용할 수 있는 영웅레벨이 아닙니다.");
        }
        m_currentTime = DateTimeUtil.currentTime;
        if (m_myHero.isDead)
        {
            throw new CommandHandleException(101, "영웅이 죽은 상태입니다.");
        }
        if (m_myHero.isFullHp)
        {
            throw new CommandHandleException(102, "영웅의 체력이 가득 차 있습니다.");
        }
        float fElapsedTime = DateTimeUtil.GetTimeSpanSeconds(m_myHero.hpPotionLastUsedTime, m_currentTime);
        if (fElapsedTime < item.value1 * 0.9f)
        {
            throw new CommandHandleException(103, "쿨타임이 경과하지 않았습니다.. fElapsedTime = " + fElapsedTime);
        }
        Place currentPlace = m_myHero.currentPlace;
        if (currentPlace == null)
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        if (!currentPlace.hpPotionUseEnabled)
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        m_myHero.UseItem(nInventorySlotIndex, 1);
        int nUsedOwnCount = 0;
        int nUsedUnOwnCount = 0;
        if (bOwned)
        {
            nUsedOwnCount = 1;
        }
        else
        {
            nUsedUnOwnCount = 1;
        }
        int nRestoreHpAmount = (int)Math.Floor(m_myHero.realMaxHP * (item.value2 / 10000f));
        m_myHero.RestoreHP(nRestoreHpAmount, bSendEventToMyself: false, bSendEventToOthers: true);
        m_myHero.hpPotionLastUsedTime = m_currentTime;
        SaveToDB(item.id, nUsedOwnCount, nUsedUnOwnCount);
        HpPotionUseResponseBody resBody = new HpPotionUseResponseBody();
        resBody.changedInventorySlot = m_inventorySlot.ToPDInventorySlot();
        resBody.hp = m_myHero.hp;
        SendResponseOK(resBody);
    }

    private void SaveToDB(int nItemId, int nItemOwnCount, int nItemUnOwnCount)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_inventorySlot));
        dbWork.Schedule();
        SaveToDB_AddItemUseLog(nItemId, nItemOwnCount, nItemUnOwnCount);
    }

    private void SaveToDB_AddItemUseLog(int nItemId, int nItemOwnCount, int nItemUnOwnCount)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, nItemId, nItemOwnCount, nItemUnOwnCount, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
