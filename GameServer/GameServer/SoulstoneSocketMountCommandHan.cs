using System.Collections.Generic;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class SoulstoneSocketMountCommandHandler : InGameCommandHandler<SoulstoneSocketMountCommandBody, SoulstoneSocketMountResponseBody>
{
	public const short kResult_NotOpendSoulstoneSocket = 101;

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
			throw new CommandHandleException(1, "영웅보조장비 ID가 유효하지 않습니다 nSubGearId = " + nSubGearId);
		}
		if (nSocketIndex < 0)
		{
			throw new CommandHandleException(1, "소울스톤 소켓 인덱스가 유효하지 않습니다. nSocketIndex = " + nSocketIndex);
		}
		if (nItemId <= 0)
		{
			throw new CommandHandleException(1, "아이템 ID가 유효하지 않습니다. nItemId = " + nItemId);
		}
		HeroSubGear heroSubGear = m_myHero.GetSubGear(nSubGearId);
		if (heroSubGear == null)
		{
			throw new CommandHandleException(1, "영웅보조장비가 존재하지 않습니다. nSubGearId = " + nSubGearId);
		}
		if (!heroSubGear.equipped)
		{
			throw new CommandHandleException(1, "장착되지 않은 영웅보조장비입니다.");
		}
		HeroSoulstoneSocket heroSoulstoneSocket = heroSubGear.GetSoulstoneSocket(nSocketIndex);
		if (heroSoulstoneSocket == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 소울스톤 소켓입니다. nSocketIndex = " + nSocketIndex);
		}
		if (!heroSoulstoneSocket.isOpened)
		{
			throw new CommandHandleException(101, "개방되지 않은 소울스톤 소켓입니다.");
		}
		if (!heroSoulstoneSocket.isEmpty)
		{
			throw new CommandHandleException(1, "소울스톤이 이미 장착되어있습니다.");
		}
		Item socketItem = Resource.instance.GetItem(nItemId);
		if (socketItem == null)
		{
			throw new CommandHandleException(1, "아이템이 존재하지 않습니다. nItemId = " + nItemId);
		}
		if (socketItem.type.id != heroSoulstoneSocket.socket.itemType)
		{
			throw new CommandHandleException(1, "해당 아이템은 소울스톤이 아닙니다.");
		}
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		if (m_myHero.GetItemCount(socketItem.id) <= 0)
		{
			throw new CommandHandleException(102, "아이템이 부족합니다.");
		}
		m_myHero.UseItem(socketItem.id, bFirstUseOwn: true, 1, changedInventorySlots);
		heroSoulstoneSocket.Mount(socketItem);
		m_targetInventorySlot = changedInventorySlots[0];
		heroSubGear.RefreshAttrTotalValues();
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB(heroSoulstoneSocket);
		SoulstoneSocketMountResponseBody resBody = new SoulstoneSocketMountResponseBody();
		resBody.changedInventorySlot = m_targetInventorySlot.ToPDInventorySlot();
		resBody.maxHp = m_myHero.realMaxHP;
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroSoulstoneSocket heroSoulstoneSocket)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(m_targetInventorySlot));
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroSubGearSoulstoneSocket(heroSoulstoneSocket.subGear.hero.id, heroSoulstoneSocket.subGear.subGearId, heroSoulstoneSocket.index, heroSoulstoneSocket.itemId));
		dbWork.Schedule();
	}
}
