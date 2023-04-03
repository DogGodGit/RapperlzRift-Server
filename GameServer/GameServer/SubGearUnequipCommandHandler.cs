using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SubGearUnequipCommandHandler : InGameCommandHandler<SubGearUnequipCommandBody, SubGearUnequipResponseBody>
{
	public const short kResult_NotEnoughInventory = 101;

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
			throw new CommandHandleException(1, "보조장비 ID가 유효하지 않습니다. nSubGearId = " + nSubGearId);
		}
		m_targetSubGear = m_myHero.GetSubGear(nSubGearId);
		if (m_targetSubGear == null)
		{
			throw new CommandHandleException(1, "보조장비가 존재하지 않습니다. nSubGearId = " + nSubGearId);
		}
		if (!m_targetSubGear.equipped)
		{
			throw new CommandHandleException(1, "장착되지 않은 보조장비입니다.");
		}
		InventorySlot targetInventorySlot = m_myHero.GetEmptyInventorySlot();
		if (targetInventorySlot == null)
		{
			throw new CommandHandleException(101, "인벤토리가 부족합니다.");
		}
		targetInventorySlot.Place(m_targetSubGear);
		m_targetSubGear.equipped = false;
		m_changedInventorySlot = targetInventorySlot;
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		SubGearUnequipResponseBody resBody = new SubGearUnequipResponseBody();
		resBody.changedInventorySlotIndex = m_changedInventorySlot.index;
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_AddInventorySlot(m_changedInventorySlot));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSubGear_Equip(m_targetSubGear.hero.id, m_targetSubGear.subGearId, m_targetSubGear.equipped));
		dbWork.Schedule();
	}
}
