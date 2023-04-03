using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MountGearUnequipCommandHandler : InGameCommandHandler<MountGearUnequipCommandBody, MountGearUnequipResponseBody>
{
	public const short kResult_NotEnoughInventory = 101;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid heroMountGearId = (Guid)m_body.heroMountGearId;
		HeroMountGear targetHeroMountGear = m_myHero.GetMountGear(heroMountGearId);
		if (targetHeroMountGear == null)
		{
			throw new CommandHandleException(1, "존재하지 않는 영웅탈것장비 입니다. heroMountGearId = " + heroMountGearId);
		}
		if (targetHeroMountGear.gearSlot == null)
		{
			throw new CommandHandleException(1, "장착한 장비가 아닙니다. heroMountGearId = " + heroMountGearId);
		}
		if (m_myHero.emptyInventorySlotCount <= 0)
		{
			throw new CommandHandleException(101, "빈 인벤토리 슬롯이 없습니다.");
		}
		HeroMountGearSlot mountGearSlot = targetHeroMountGear.gearSlot;
		mountGearSlot.Unequip();
		InventorySlot targetInventorySlot = m_myHero.GetEmptyInventorySlot();
		targetInventorySlot.Place(targetHeroMountGear);
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB(targetInventorySlot, mountGearSlot);
		MountGearUnequipResponseBody resBody = new MountGearUnequipResponseBody();
		resBody.changedInventorySlotIndex = targetInventorySlot.index;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB(InventorySlot inventorySlot, HeroMountGearSlot mountGearSlot)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_AddInventorySlot(inventorySlot));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteEquippedHeroMountGearSlot(mountGearSlot.hero.id, mountGearSlot.index));
		dbWork.Schedule();
	}
}
