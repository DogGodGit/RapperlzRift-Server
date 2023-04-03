using System.Collections.Generic;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class RuneSocketUnmountCommandHandler : InGameCommandHandler<RuneSocketUnmountCommandBody, RuneSocketUnmountResponseBody>
{
	public const short kResult_NotEnoughInventory = 101;

	private InventorySlot m_targetInventorySlot;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nSubGearId = m_body.subGearId;
		int nSocketIndex = m_body.socketIndex;
		if (nSubGearId <= 0)
		{
			throw new CommandHandleException(1, "보조장비 ID가 유효하지 않습니다. nSubGearId = " + nSubGearId);
		}
		if (nSocketIndex < 0)
		{
			throw new CommandHandleException(1, "룬소켓 인덱스가 유효하지 않습니다. nSocketIndex = " + nSocketIndex);
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
		HeroRuneSocket heroSubGearRuneSocket = heroSubGear.GetRuneSocket(nSocketIndex);
		if (heroSubGearRuneSocket == null)
		{
			throw new CommandHandleException(1, "존재하지 않은 룬소켓입니다. nSocketIndex = " + nSocketIndex);
		}
		if (heroSubGearRuneSocket.isEmpty)
		{
			throw new CommandHandleException(1, "빈 룬소켓입니다.");
		}
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		Item socketItem = heroSubGearRuneSocket.item;
		bool bSubGearOwned = true;
		if (m_myHero.GetInventoryAvailableSpace(socketItem, bSubGearOwned) <= 0)
		{
			throw new CommandHandleException(101, "인벤토리가 부족합니다.");
		}
		m_myHero.AddItem(heroSubGearRuneSocket.item, bSubGearOwned, 1, changedInventorySlots);
		heroSubGearRuneSocket.Unmount();
		m_targetInventorySlot = changedInventorySlots[0];
		heroSubGear.RefreshAttrTotalValues();
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB(heroSubGearRuneSocket);
		RuneSocketUnmountResponseBody resBody = new RuneSocketUnmountResponseBody();
		resBody.changedInventorySlot = m_targetInventorySlot.ToPDInventorySlot();
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroRuneSocket heroSubGearRuneSocket)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(m_targetInventorySlot));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroSubGearRuneSocket(heroSubGearRuneSocket.subGear.hero.id, heroSubGearRuneSocket.subGear.subGearId, heroSubGearRuneSocket.index));
		dbWork.Schedule();
	}
}
