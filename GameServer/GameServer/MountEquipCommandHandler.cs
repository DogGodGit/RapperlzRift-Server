using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MountEquipCommandHandler : InGameCommandHandler<MountEquipCommandBody, MountEquipResponseBody>
{
	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nMountId = m_body.mountId;
		if (nMountId <= 0)
		{
			throw new CommandHandleException(1, "탈것ID가 유효하지 않습니다. nMountId = " + nMountId);
		}
		HeroMount targetHeroMount = m_myHero.GetMount(nMountId);
		if (targetHeroMount == null)
		{
			throw new CommandHandleException(1, "영웅탈것이 존재하지 않습니다. nMountId = " + nMountId);
		}
		if (targetHeroMount.isEquipped)
		{
			throw new CommandHandleException(1, "이미 장착한 영웅탈것입니다.");
		}
		m_myHero.equippedMount = targetHeroMount;
		if (m_myHero.isRiding)
		{
			m_myHero.GetOffMount(bSendEventToMyself: false);
		}
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB();
		MountEquipResponseBody resBody = new MountEquipResponseBody();
		resBody.maxHp = m_myHero.realMaxHP;
		resBody.hp = m_myHero.hp;
		resBody.isRiding = m_myHero.isRiding;
		SendResponseOK(resBody);
	}

	private void SaveToDB()
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_EquippedMount(m_myHero.id, m_myHero.equippedMountId));
		dbWork.Schedule();
	}
}
