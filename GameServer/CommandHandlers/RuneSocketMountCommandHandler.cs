using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer.CommandHandlers;

public class RuneSocketMountCommandHandler : InGameCommandHandler<RuneSocketMountCommandBody, RuneSocketMountResponseBody>
{
    public const short kResult_NotOpenedRuneSocket = 101;

    public const short kResult_NotEnoughItem = 102;

    private InventorySlot m_targetInventorySlot;

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        int nSubGearId = m_body.subGearId;
        int nSocketIndex = m_body.socketIndex;
        int nItemId = m_body.itemId;
        if (nSubGearId <= 0)
        {
            throw new CommandHandleException(1, "보조장비 ID가 유효하지 않습니다. nSubGearId = " + nSubGearId);
        }
        if (nSocketIndex < 0)
        {
            throw new CommandHandleException(1, "룬 소켓 인덱스가 유효하지 않습니다. nSocketIndex = " + nSocketIndex);
        }
        if (nItemId <= 0)
        {
            throw new CommandHandleException(1, "아이템 ID가 유효하지 않습니다. nItemId = " + nItemId);
        }
        HeroSubGear heroSubGear = m_myHero.GetSubGear(nSubGearId);
        if (heroSubGear == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 영웅보조장비입니다. nHeroSubGearId = " + nSubGearId);
        }
        if (!heroSubGear.equipped)
        {
            throw new CommandHandleException(1, "장착되지 않은 영웅보조장비입니다.");
        }
        HeroRuneSocket heroSubGearRuneSocket = heroSubGear.GetRuneSocket(nSocketIndex);
        if (heroSubGearRuneSocket == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 룬소켓입니다. nSocketIndex = " + nSocketIndex);
        }
        if (!heroSubGearRuneSocket.isOpened)
        {
            throw new CommandHandleException(101, "해당 룬소켓은 아직 개방되지 않았습니다.");
        }
        if (!heroSubGearRuneSocket.isEmpty)
        {
            throw new CommandHandleException(1, "룬이 이미 장착되어있습니다.");
        }
        Item socketItem = Resource.instance.GetItem(nItemId);
        if (socketItem == null)
        {
            throw new CommandHandleException(1, "존재하지 않는 아이템입니다.");
        }
        if (!heroSubGearRuneSocket.socket.ContainsAvailableItemType(socketItem.type.id))
        {
            throw new CommandHandleException(1, "해당 아이템은 소켓에 장착가능한 아이템이 아닙니다.");
        }
        List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
        if (m_myHero.GetItemCount(socketItem.id) <= 0)
        {
            throw new CommandHandleException(102, "아이템이 부족합니다.");
        }
        m_myHero.UseItem(socketItem.id, bFirstUseOwn: true, 1, changedInventorySlots);
        heroSubGearRuneSocket.Mount(socketItem);
        m_targetInventorySlot = changedInventorySlots[0];
        heroSubGear.RefreshAttrTotalValues();
        m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
        SaveToDB(heroSubGearRuneSocket);
        RuneSocketMountResponseBody resBody = new RuneSocketMountResponseBody();
        resBody.changedInventorySlot = m_targetInventorySlot.ToPDInventorySlot();
        resBody.maxHp = m_myHero.realMaxHP;
        SendResponseOK(resBody);
    }

    private void SaveToDB(HeroRuneSocket heroSubGearRuneSocket)
    {
        SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
        dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_targetInventorySlot));
        dbWork.AddSqlCommand(GameDac.CSC_AddHeroSubGearRuneSocket(heroSubGearRuneSocket.subGear.hero.id, heroSubGearRuneSocket.subGear.subGearId, heroSubGearRuneSocket.index, heroSubGearRuneSocket.itemId));
        dbWork.Schedule();
    }
}
