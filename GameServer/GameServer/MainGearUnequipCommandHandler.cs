using System;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MainGearUnequipCommandHandler : InGameCommandHandler<MainGearUnequipCommandBody, MainGearUnequipResponseBody>
{
	public const short kResult_NotEnoughInventory = 101;

	private HeroMainGear m_targetMainGear;

	private InventorySlot m_changedInventorySlot;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid heroMainGearId = (Guid)m_body.heroMainGearId;
		m_targetMainGear = m_myHero.GetMainGear(heroMainGearId);
		if (m_targetMainGear == null)
		{
			throw new CommandHandleException(1, "존재하지않는 메인장비입니다. heroMainGearId = " + heroMainGearId);
		}
		if (!m_targetMainGear.isEquipped)
		{
			throw new CommandHandleException(1, "장착중인 장비가 아닙니다.");
		}
		InventorySlot targetInventorySlot = m_myHero.GetEmptyInventorySlot();
		if (targetInventorySlot == null)
		{
			throw new CommandHandleException(101, "인벤토리가 부족합니다.");
		}
		if (m_targetMainGear.isWeapon)
		{
			m_myHero.equippedWeapon = null;
		}
		else
		{
			m_myHero.equippedArmor = null;
		}
		targetInventorySlot.Place(m_targetMainGear);
		m_changedInventorySlot = targetInventorySlot;
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		if (m_myHero.currentPlace != null)
		{
			Place currentPlace = m_myHero.currentPlace;
			ServerEvent.SendHeroMainGearUnequip(currentPlace.GetDynamicClientPeers(m_myHero.sector, m_myHero.id), m_myHero.id, m_targetMainGear.id);
		}
		SaveToDB();
		MainGearUnequipResponseBody resBody = new MainGearUnequipResponseBody();
		resBody.changedInventorySlotIndex = m_changedInventorySlot.index;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_MainGear(m_myHero));
		dbWork.AddSqlCommand(GameDacEx.CSC_AddInventorySlot(m_changedInventorySlot));
		dbWork.Schedule();
	}
}
