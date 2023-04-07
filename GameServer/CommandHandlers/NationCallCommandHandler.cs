using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class NationCallCommandHandler : InGameCommandHandler<NationCallCommandBody, NationCallResponseBody>
{
    public const short kResult_NoNationNoblesse = 101;

    public const short kResult_NoAuthority = 102;

    public const short kResult_NotEnoughItem = 103;

    public const short kResult_NationWar = 104;

    private InventorySlot m_targetInventorySlot;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        m_currentTime = DateTimeUtil.currentTime;
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nSlotIndex = m_body.slotIndex;
        if (nSlotIndex < 0)
        {
            throw new CommandHandleException(1, "슬롯 인덱스가 유효하지 않습니다. nSlotIndex = " + nSlotIndex);
        }
        if (!(m_myHero.currentPlace is ContinentInstance currentPlace))
        {
            throw new CommandHandleException(1, "현재 장소에선 사용할 수 없는 명령입니다.");
        }
        NationInstance nationInst = m_myHero.nationInst;
        if (nationInst.nationWarInst != null)
        {
            throw new CommandHandleException(104, "국가전이 진행중입니다.");
        }
        NationNoblesse noblesse = m_myHero.nationNoblesse;
        if (noblesse == null)
        {
            throw new CommandHandleException(101, "관직이 존재하지 않습니다.");
        }
        if (!noblesse.nationCallEnabled)
        {
            throw new CommandHandleException(102, "국가소집할 권한이 없습니다.");
        }
        m_targetInventorySlot = m_myHero.GetInventorySlot(nSlotIndex);
        if (m_targetInventorySlot == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
        }
        if (m_targetInventorySlot.isEmpty)
        {
            throw new CommandHandleException(1, "빈 슬롯입니다. nSlotIndex = " + nSlotIndex);
        }
        if (m_targetInventorySlot.obj.inventoryObjectType != 3)
        {
            throw new CommandHandleException(1, "해당 인벤토리 슬롯 오브젝트타입은 아이템이 아닙니다.");
        }
        ItemInventoryObject itemObject = (ItemInventoryObject)m_targetInventorySlot.obj;
        Item targetItem = itemObject.item;
        bool bItemOwned = itemObject.owned;
        if (itemObject.count < 1)
        {
            throw new CommandHandleException(103, "아이템 수량이 부족합니다.");
        }
        if (!targetItem.IsUseableHeroLevel(m_myHero.level))
        {
            throw new CommandHandleException(1, "아이템을 사용할 수 없는 레벨입니다.");
        }
        if (targetItem.type.id != 24)
        {
            throw new CommandHandleException(1, "아이템 타입이 국가소집아이템이 아닙니다.");
        }
        m_myHero.UseItem(nSlotIndex, 1);
        nationInst.CallNationMembers(m_myHero, currentPlace.continent, currentPlace.nationId);
        SaveToDB();
        SaveToGameLogDB(targetItem.id, bItemOwned, 1);
        NationCallResponseBody resBody = new NationCallResponseBody();
        resBody.changedInventorySlot = m_targetInventorySlot.ToPDInventorySlot();
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_targetInventorySlot));
        dbWork.Schedule();
    }

    private void SaveToGameLogDB(int nItemId, bool bItemOwned, int nUsedItemCount)
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            int nItemOwnCount = 0;
            int nItemUnOwnCount = 0;
            if (bItemOwned)
            {
                nItemOwnCount = nUsedItemCount;
            }
            else
            {
                nItemUnOwnCount = nUsedItemCount;
            }
            logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_myHero.id, nItemId, nItemOwnCount, nItemUnOwnCount, m_currentTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
