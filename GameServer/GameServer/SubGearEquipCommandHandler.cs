using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SubGearEquipCommandHandler : InGameCommandHandler<SubGearEquipCommandBody, SubGearEquipResponseBody>
{
	private HeroSubGear m_targetSubGear;

	private InventorySlot m_changedInventorySlot;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nSubGearId = m_body.subGearId;
		if (nSubGearId <= 0)
		{
			throw new CommandHandleException(1, "유효하지않는 보조장비ID 입니다. nSubGearId = " + nSubGearId);
		}
		m_targetSubGear = m_myHero.GetSubGear(nSubGearId);
		if (m_targetSubGear == null)
		{
			throw new CommandHandleException(1, "존재하지않는 보조장비입니다. nSubGearId = " + nSubGearId);
		}
		if (m_targetSubGear.level > m_myHero.level)
		{
			throw new CommandHandleException(1, "영웅 레벨이 부족하여 장착할 수 없습니다.");
		}
		if (m_targetSubGear.equipped)
		{
			throw new CommandHandleException(1, "이미 장착된 보조장비입니다.");
		}
		InventorySlot targetInventorySlot = m_targetSubGear.inventorySlot;
		if (targetInventorySlot == null)
		{
			throw new CommandHandleException(1, "보조장비가 인벤토리에 있지 않습니다.");
		}
		targetInventorySlot.Clear();
		m_targetSubGear.equipped = true;
		m_changedInventorySlot = targetInventorySlot;
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SubGearEquipResponseBody resBody = new SubGearEquipResponseBody();
		resBody.maxHp = m_myHero.realMaxHP;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_DeleteInventorySlot(m_changedInventorySlot.hero.id, m_changedInventorySlot.index));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSubGear_Equip(m_targetSubGear.hero.id, m_targetSubGear.subGearId, m_targetSubGear.equipped));
		dbWork.Schedule();
	}
}
