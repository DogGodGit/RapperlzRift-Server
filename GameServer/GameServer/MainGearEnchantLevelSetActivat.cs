using System;
using ClientCommon;
using GameServer.CommandHandlers;
using ServerFramework;

namespace GameServer;

public class MainGearEnchantLevelSetActivateCommandHandler : InGameCommandHandler<MainGearEnchantLevelSetActivateCommandBody, MainGearEnchantLevelSetActivateResponseBody>
{
	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	protected override void HandleInGameCommand()
	{
		m_currentTime = DateTimeUtil.currentTime;
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		int nSetNo = m_body.setNo;
		if (nSetNo <= 0)
		{
			throw new CommandHandleException(1, "세트번호가 유효하지 않습니다. nSetNo = " + nSetNo);
		}
		if (nSetNo != m_myHero.mainGearEnchantLevelSetNo + 1)
		{
			throw new CommandHandleException(1, "해당 메인장비강화레벨세트는 활성화할 수 없습니다. nSetNo = " + nSetNo);
		}
		MainGearEnchantLevelSet targetMainGearEnchantLevelSet = Resource.instance.GetMainGearEnchantLevelSet(nSetNo);
		if (targetMainGearEnchantLevelSet == null)
		{
			throw new CommandHandleException(1, "해당 메인장비강화레벨세트효과가 존재하지 않습니다. nSetNo = " + nSetNo);
		}
		HeroMainGear weaponMainGear = m_myHero.equippedWeapon;
		HeroMainGear armorMainGear = m_myHero.equippedArmor;
		int nWeaponEnchantLevel = 0;
		int nArmorEnchantLevel = 0;
		if (weaponMainGear != null)
		{
			nWeaponEnchantLevel = weaponMainGear.enchantLevel;
		}
		if (armorMainGear != null)
		{
			nArmorEnchantLevel = armorMainGear.enchantLevel;
		}
		if (nWeaponEnchantLevel + nArmorEnchantLevel < targetMainGearEnchantLevelSet.requiredTotalEnchnatLevel)
		{
			throw new CommandHandleException(1, "메인장비 강화레벨 총합이 부족합니다. requiredTotalEnchantLevel = " + targetMainGearEnchantLevelSet.requiredTotalEnchnatLevel);
		}
		m_myHero.mainGearEnchantLevelSet = targetMainGearEnchantLevelSet;
		m_myHero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SaveToDB(nWeaponEnchantLevel, nArmorEnchantLevel);
		MainGearEnchantLevelSetActivateResponseBody resBody = new MainGearEnchantLevelSetActivateResponseBody();
		resBody.maxHp = m_myHero.realMaxHP;
		SendResponseOK(resBody);
	}

	private void SaveToDB(int nWeaponHeroMainGearEnchantLevel, int nArmorHeroMainGearEnchantLevel)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_myHero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_MainGearEnchantLevelSetNo(m_myHero.id, m_myHero.mainGearEnchantLevelSetNo));
		dbWork.Schedule();
		SaveToDB_AddHeroMainGearEnchantLevelSetActivationLog(nWeaponHeroMainGearEnchantLevel, nArmorHeroMainGearEnchantLevel);
	}

	private void SaveToDB_AddHeroMainGearEnchantLevelSetActivationLog(int nWeaponHeroMainGearEnchantLevel, int nArmorHeroMainGearEnchantLevel)
	{
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroMainGearEnchantLevelSetActiviationLog(Guid.NewGuid(), m_myHero.id, m_myHero.mainGearEnchantLevelSet.setNo, m_myHero.equippedWeaponId, nWeaponHeroMainGearEnchantLevel, m_myHero.equippedArmorId, nArmorHeroMainGearEnchantLevel, m_currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			LogError(null, ex, bStackTrace: true);
		}
	}
}
