using System;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class CostumeItemUseCommandHandler : InGameCommandHandler<CostumeItemUseCommandBody, CostumeItemUseResponseBody>
{
    private InventorySlot m_changedInventorySlot;

    private HeroCostume m_heroCostume;

    private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

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
        m_changedInventorySlot = m_myHero.GetInventorySlot(nSlotIndex);
        if (m_changedInventorySlot == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
        }
        if (m_changedInventorySlot.isEmpty)
        {
            throw new CommandHandleException(1, "빈 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
        }
        if (m_changedInventorySlot.obj.inventoryObjectType != 3)
        {
            throw new CommandHandleException(1, "해당 인벤토리 슬롯의 오브젝트는 아이템이 아닙니다. nSlotIndex = " + nSlotIndex);
        }
        ItemInventoryObject itemObj = (ItemInventoryObject)m_changedInventorySlot.obj;
        Item targetItem = itemObj.item;
        if (targetItem.type.id != 43)
        {
            throw new CommandHandleException(1, "해당 아이템은 코스튬이 아닙니다.");
        }
        if (!targetItem.IsUseableHeroLevel(m_myHero.level))
        {
            throw new CommandHandleException(1, "해당 아이템을 사용할 수 있는 레벨이 아닙니다.");
        }
        if (m_myHero.ContainsCostume(targetItem.value1))
        {
            throw new CommandHandleException(1, "이미 소유한 코스튬입니다.");
        }
        m_myHero.UseItem(nSlotIndex, 1);
        Costume costume = Resource.instance.GetCostume(targetItem.value1);
        m_heroCostume = new HeroCostume(m_myHero);
        m_heroCostume.Init(costume, m_currentTime);
        m_myHero.AddCostume(m_heroCostume);
        SaveToDB();
        SaveToDB_Log();
        CostumeItemUseResponseBody resBody = new CostumeItemUseResponseBody();
        resBody.costumeId = m_heroCostume.costumeId;
        resBody.remainingTime = m_heroCostume.GetRemainingTime(m_currentTime);
        resBody.changedInventorySlot = m_changedInventorySlot.ToPDInventorySlot();
        SendResponseOK(resBody);
    }

    private void SaveToDB()
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_changedInventorySlot));
        dbWork.AddSqlCommand(GameDac.CSC_AddHeroCostume(m_myHero.id, m_heroCostume.costumeId, m_heroCostume.costumeEffectId, m_heroCostume.regTime));
        dbWork.Schedule();
    }

    private void SaveToDB_Log()
    {
        try
        {
            SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
            logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCostumeAcquisitionLog(Guid.NewGuid(), m_myHero.id, m_heroCostume.costumeId, m_heroCostume.regTime));
            logWork.Schedule();
        }
        catch (Exception ex)
        {
            LogError(null, ex, bStackTrace: true);
        }
    }
}
