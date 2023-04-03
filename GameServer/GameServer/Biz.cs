using ServerFramework;

namespace GameServer;

public static class Biz
{
	public static void RegisterFieldOfHonorRanker(Hero hero)
	{
		int nRanking = hero.fieldOfHonorRanking;
		int nCurrentMaxRanking = Cache.instance.currentFieldOfHonorMaxRanking;
		if (nRanking != 0 || nCurrentMaxRanking >= 9999)
		{
			return;
		}
		int nNewRanking = (hero.fieldOfHonorRanking = nCurrentMaxRanking + 1);
		FieldOfHonorHero newRanker = hero.ToFieldOfHonorHero();
		Cache.instance.SetFieldOfHonorRanker(newRanker);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_FieldOfHonorRanking(hero.id, hero.fieldOfHonorRanking));
		dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateFieldOfHonorHero(newRanker));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroRealAttrs(hero.id));
		foreach (FieldOfHonorHeroRealAttr realAttr in newRanker.realAttrs.Values)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroRealAttr(realAttr));
		}
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroSkills(hero.id));
		foreach (FieldOfHonorHeroSkill skill in newRanker.skills.Values)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroSkill(skill));
		}
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroWings(hero.id));
		foreach (FieldOfHonorHeroWing wing in newRanker.wings.Values)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroWing(wing));
		}
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroWingEnchants(hero.id));
		foreach (FieldOfHonorHeroWingPart wingPart in newRanker.wingParts)
		{
			foreach (FieldOfHonorHeroWingEnchant wingEnchant in wingPart.enchants)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroWingEnchant(wingEnchant));
			}
		}
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroMainGearOptionAttrs(hero.id));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroEquippedMainGears(hero.id));
		FieldOfHonorHeroEquippedMainGear equippedWeapon = newRanker.equippedWeapon;
		if (equippedWeapon != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroEquippedMainGear(equippedWeapon));
			foreach (FieldOfHonorHeroMainGearOptionAttr optionAttr2 in newRanker.equippedWeapon.optionAttrs)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroMainGearOptionAttr(optionAttr2));
			}
		}
		FieldOfHonorHeroEquippedMainGear equippedArmor = newRanker.equippedArmor;
		if (equippedArmor != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroEquippedMainGear(newRanker.equippedArmor));
			foreach (FieldOfHonorHeroMainGearOptionAttr optionAttr in newRanker.equippedArmor.optionAttrs)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroMainGearOptionAttr(optionAttr));
			}
		}
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroSubGearRuneSockets(hero.id));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroSubGearSoulstoneSockets(hero.id));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroEquippedSubGears(hero.id));
		foreach (FieldOfHonorHeroEquippedSubGear heroSubGear in newRanker.equippedSubGears.Values)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroEquippedSubGear(heroSubGear));
			foreach (FieldOfHonorHeroSubGearRuneSocket runeSocket in heroSubGear.runeSockets.Values)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroSubGearRuneSocket(runeSocket));
			}
			foreach (FieldOfHonorHeroSubGearSoulstoneSocket soulstoneSocket in heroSubGear.soulstoneSockets.Values)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroSubGearSoulstoneSocket(soulstoneSocket));
			}
		}
		dbWork.Schedule();
	}

	public static void UpdateFieldOfHonorRanker(Hero hero)
	{
		int nRanking = hero.fieldOfHonorRanking;
		if (nRanking <= 0 && nRanking > 9999)
		{
			return;
		}
		FieldOfHonorHero ranker = hero.ToFieldOfHonorHero();
		Cache.instance.SetFieldOfHonorRanker(ranker);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(hero.id);
		dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateFieldOfHonorHero(ranker));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroRealAttrs(hero.id));
		foreach (FieldOfHonorHeroRealAttr realAttr in ranker.realAttrs.Values)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroRealAttr(realAttr));
		}
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroSkills(hero.id));
		foreach (FieldOfHonorHeroSkill skill in ranker.skills.Values)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroSkill(skill));
		}
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroWings(hero.id));
		foreach (FieldOfHonorHeroWing wing in ranker.wings.Values)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroWing(wing));
		}
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroWingEnchants(hero.id));
		foreach (FieldOfHonorHeroWingPart wingPart in ranker.wingParts)
		{
			foreach (FieldOfHonorHeroWingEnchant wingEnchant in wingPart.enchants)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroWingEnchant(wingEnchant));
			}
		}
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroMainGearOptionAttrs(hero.id));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroEquippedMainGears(hero.id));
		FieldOfHonorHeroEquippedMainGear equippedWeapon = ranker.equippedWeapon;
		if (equippedWeapon != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroEquippedMainGear(equippedWeapon));
			foreach (FieldOfHonorHeroMainGearOptionAttr optionAttr2 in ranker.equippedWeapon.optionAttrs)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroMainGearOptionAttr(optionAttr2));
			}
		}
		FieldOfHonorHeroEquippedMainGear equippedArmor = ranker.equippedArmor;
		if (equippedArmor != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroEquippedMainGear(ranker.equippedArmor));
			foreach (FieldOfHonorHeroMainGearOptionAttr optionAttr in ranker.equippedArmor.optionAttrs)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroMainGearOptionAttr(optionAttr));
			}
		}
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroSubGearRuneSockets(hero.id));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroSubGearSoulstoneSockets(hero.id));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorHeroEquippedSubGears(hero.id));
		foreach (FieldOfHonorHeroEquippedSubGear heroSubGear in ranker.equippedSubGears.Values)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroEquippedSubGear(heroSubGear));
			foreach (FieldOfHonorHeroSubGearRuneSocket runeSocket in heroSubGear.runeSockets.Values)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroSubGearRuneSocket(runeSocket));
			}
			foreach (FieldOfHonorHeroSubGearSoulstoneSocket soulstoneSocket in heroSubGear.soulstoneSockets.Values)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHeroSubGearSoulstoneSocket(soulstoneSocket));
			}
		}
		dbWork.Schedule();
	}
}
