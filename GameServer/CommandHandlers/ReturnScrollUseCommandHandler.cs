using System;
using ClientCommon;

namespace GameServer.CommandHandlers;

public class ReturnScrollUseCommandHandler : InGameCommandHandler<ReturnScrollUseCommandBody, ReturnScrollUseResponseBody>
{
    public const short kResult_Dead = 101;

    public const short kResult_NotCoolDownUsedReturnScroll = 103;

    public const short kResult_ReturnScrollUsing = 104;

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nInventorySlotIndex = m_body.inventorySlotIndex;
        InventorySlot inventorySlot = m_myHero.GetInventorySlot(nInventorySlotIndex);
        if (inventorySlot == null)
        {
            throw new CommandHandleException(1, "해당 인벤토리슬롯이 존재하지 않습니다. nInventorySlotIndex = " + nInventorySlotIndex);
        }
        if (!(inventorySlot.obj is ItemInventoryObject obj))
        {
            throw new CommandHandleException(1, "해당 인벤토리슬롯의 오브젝트가 아이템타입이 아닙니다.");
        }
        Item item = obj.item;
        if (item.type.id != 2)
        {
            throw new CommandHandleException(1, "해당 인벤토리의 아이템이 마을귀환서가 아닙니다.");
        }
        if (!item.IsUseableHeroLevel(m_myHero.level))
        {
            throw new CommandHandleException(1, "사용할 수 있는 영웅레벨이 아닙니다.");
        }
        Place currentPlace = m_myHero.currentPlace;
        if (currentPlace == null)
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        if (!currentPlace.returnScrollUseEnabled)
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        if (m_myHero.isDead)
        {
            throw new CommandHandleException(101, "영웅이 죽은 상태입니다.");
        }
        if (m_myHero.isRiding)
        {
            throw new CommandHandleException(1, "영웅이 탈것을 타고있는 상태입니다.");
        }
        if (m_myHero.isRidingCart)
        {
            throw new CommandHandleException(1, "영웅이 카트에 탑승중입니다.");
        }
        if (m_myHero.moving)
        {
            throw new CommandHandleException(1, "영웅이 이동중입니다.");
        }
        if (m_myHero.autoHunting)
        {
            throw new CommandHandleException(1, "영웅이 자동사냥중입니다.");
        }
        HeroExclusiveAction currentExclusiveAction = m_myHero.currentExclusiveAction;
        if (currentExclusiveAction != 0)
        {
            throw new CommandHandleException(1, "영웅이 다른 행동중입니다. currentExclusiveAction = " + currentExclusiveAction);
        }
        DateTimeOffset currentTime = DateTimeUtil.currentTime;
        float fElapsedTime = DateTimeUtil.GetTimeSpanSeconds(m_myHero.returnScrollLastUsedTime, currentTime);
        if (fElapsedTime < item.value1 * 0.9f)
        {
            throw new CommandHandleException(103, "아직 마을귀환서를 사용할 수 없습니다. fElapsedTime = " + fElapsedTime);
        }
        if (m_myHero.isReturnScrollUsing)
        {
            throw new CommandHandleException(104, "이미 마을귀환서를 사용하는 중입니다.");
        }
        m_myHero.StartReturnScrollUse(item.value2, inventorySlot.index);
        SendResponseOK(null);
    }
}
