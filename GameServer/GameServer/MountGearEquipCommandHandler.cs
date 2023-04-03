using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MountGearEquipCommandHandler : InGameCommandHandler<MountGearEquipCommandBody, MountGearEquipResponseBody>
{
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
		if (targetHeroMountGear.isEquipped)
		{
			throw new CommandHandleException(1, "이미 장착된 영웅탈것장비 입니다.");
		}
		if (targetHeroMountGear.gear.requiredHeroLevel > m_myHero.level)
		{
			throw new CommandHandleException(1, "영웅레벨에 맞지않는 영웅탈것장비 입니다.");
		}
		if (targetHeroMountGear.inventorySlot == null)
		{
			throw new CommandHandleException(1, "인벤토리에 없는 영웅탈것장비 입니다.");
		}
		MountGearSlot mountGearSlot = targetHeroMountGear.gear.type.gearSlot;
		if (mountGearSlot.openHeroLevel > m_myHero.level)
		{
			throw new CommandHandleException(1, "아직 개방되지 않은 탈것장비슬롯입니다. openHeroLevel = " + mountGearSlot.openHeroLevel);
		}
		HeroMountGearSlot targetMountGearSlot = m_myHero.GetMountGearSlot(mountGearSlot.index);
		HeroMountGear oldHeroMountGear = targetMountGearSlot.heroMountGear;
		if (oldHeroMountGear != null)
		{
			targetMountGearSlot.Unequip();
		}
		InventorySlot targetHeroMountGearPlacedInventorySlot = targetHeroMountGear.inventorySlot;
		targetHeroMountGearPlacedInventorySlot.Clear();
		targetMountGearSlot.Equip(targetHeroMountGear);
		if (oldHeroMountGear != null)
		{
			targetHeroMountGearPlacedInventorySlot.Place(oldHeroMountGear);
		}
		if (!targetHeroMountGear.owned)
		{
			targetHeroMountGear.owned = true;
		}
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB(targetHeroMountGear, targetHeroMountGearPlacedInventorySlot, targetMountGearSlot);
		MountGearEquipResponseBody resBody = new MountGearEquipResponseBody();
		resBody.changedInventorySlotIndex = (targetHeroMountGearPlacedInventorySlot.isEmpty ? (-1) : targetHeroMountGearPlacedInventorySlot.index);
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB(HeroMountGear heroMountGear, InventorySlot inventorySlot, HeroMountGearSlot mountGearSlot)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMountGear_Owned(heroMountGear.id, heroMountGear.owned));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(inventorySlot));
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateEquippedHeroMountGearSlot(mountGearSlot.hero.id, mountGearSlot.index, mountGearSlot.heroMountGear.id));
		dbWork.Schedule();
	}
}
