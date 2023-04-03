using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ServerFramework;

namespace GameServer;

public static class GameDacEx
{
	public static void Logout(Hero hero)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(hero.id);
		dbWork.AddSqlCommand(CSC_UpdateHero_LogOut(hero));
		dbWork.AddSqlCommand(CSC_UpdateHero_OwnDia(hero));
		dbWork.AddSqlCommand(CSC_UpdateAccount_UnOwnDia(hero.account));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroRealAttrValues(hero.id));
		dbWork.AddSqlCommand(CSC_AddHeroRealAttrValues(hero));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_RookieGift(hero.id, hero.rookieGiftNo, hero.rookieGiftLoginDuration));
		dbWork.Schedule();
	}

	public static SqlCommand CSC_UpdateHero_LogOut(Hero hero)
	{
		return GameDac.CSC_UpdateHero_LogOut(hero.id, hero.level, hero.exp, hero.lak, hero.hp, hero.stamina, hero.staminaUpdateTime, hero.lastLoginTime, hero.lastLogoutTime, hero.lastLocationId, hero.lastLocationParam, hero.lastInstanceId, hero.lastPosition.x, hero.lastPosition.y, hero.lastPosition.z, hero.lastYRotation, hero.previousNationId, hero.previousContinentId, hero.previousPosition.x, hero.previousPosition.y, hero.previousPosition.z, hero.previousYRotation, hero.restTime, hero.dailyAccessTimeUpdateTime, hero.dailyAccessTime, hero.isRiding, hero.undergroundMazeDate, (int)Math.Floor(hero.undergroundMazePlayTime), hero.artifactRoomSweepStartTime);
	}

	public static SqlCommand CSC_UpdateHero_Level(Hero hero)
	{
		return GameDac.CSC_UpdateHero_Level(hero.id, hero.level, hero.exp, hero.levelUpdateTime);
	}

	public static SqlCommand CSC_UpdateHero_Exploit(Hero hero)
	{
		return GameDac.CSC_UpdateHero_Exploit(hero.id, hero.exploitPoint, hero.exploitPointUpdateTime, hero.dailyExploitPoint.date, hero.dailyExploitPoint.value);
	}

	public static SqlCommand CSC_UpdateHero_Rank(Hero hero)
	{
		return GameDac.CSC_UpdateHero_Rank(hero.id, hero.rankNo);
	}

	public static SqlCommand CSC_UpdateHero_RankReward(Hero hero)
	{
		return GameDac.CSC_UpdateHero_RankReward(hero.id, hero.rankRewardReceivedRankNo, hero.rankRewardReceivedDate);
	}

	public static SqlCommand CSC_UpdateHero_Gold(Hero hero)
	{
		return GameDac.CSC_UpdateHero_Gold(hero.id, hero.gold);
	}

	public static SqlCommand CSC_UpdateHero_HonorPoint(Hero hero)
	{
		return GameDac.CSC_UpdateHero_HonorPoint(hero.id, hero.honorPoint);
	}

	public static SqlCommand CSC_UpdateHero_CreatureCardCollectionFamePoint(Hero hero)
	{
		return GameDac.CSC_UpdateHero_CreaturCardCollectionFamePoint(hero.id, hero.creatureCardCollectionFamePoint, hero.creatureCardCollectionFamePointUpdateTime);
	}

	public static SqlCommand CSC_UpdateHero_ArtifactLevelUp(Hero hero)
	{
		return GameDac.CSC_UpdateHero_ArtifactLevelUp(hero.id, hero.artifactNo, hero.artifactLevel, hero.artifactExp);
	}

	public static SqlCommand CSC_UpdateHero_ArtifactOpen(Hero hero)
	{
		return GameDac.CSC_UpdateHero_ArtifactOpen(hero.id, hero.artifactNo, hero.artifactLevel, hero.artifactExp, hero.equippedArtifactNo);
	}

	public static SqlCommand CSC_AddInventorySlot(InventorySlot slot)
	{
		Guid heroId = slot.hero.id;
		IInventoryObject obj = slot.obj;
		int nType = obj.inventoryObjectType;
		Guid? herMainGearId = null;
		int nSubGearId = 0;
		int nItemId = 0;
		int nItemCount = 0;
		bool bItemOwned = false;
		Guid? heroMountGearId = null;
		switch (nType)
		{
		case 1:
			herMainGearId = ((HeroMainGear)obj).id;
			break;
		case 2:
			nSubGearId = ((HeroSubGear)obj).subGearId;
			break;
		case 3:
		{
			ItemInventoryObject itemObject = (ItemInventoryObject)obj;
			nItemId = itemObject.itemId;
			nItemCount = itemObject.count;
			bItemOwned = itemObject.owned;
			break;
		}
		case 4:
			heroMountGearId = ((HeroMountGear)obj).id;
			break;
		default:
			throw new Exception("유효하지 않은 인벤토리 객체입니다.");
		}
		return GameDac.CSC_AddInventorySlot(heroId, slot.index, nType, herMainGearId, nSubGearId, nItemId, nItemCount, bItemOwned, heroMountGearId);
	}

	public static SqlCommand CSC_UpdateInventorySlot(InventorySlot slot)
	{
		Guid heroId = slot.hero.id;
		IInventoryObject obj = slot.obj;
		int nType = obj.inventoryObjectType;
		Guid? herMainGearId = null;
		int nSubGearId = 0;
		int nItemId = 0;
		int nItemCount = 0;
		bool bItemOwned = false;
		Guid? heroMountGearId = null;
		switch (nType)
		{
		case 1:
			herMainGearId = ((HeroMainGear)obj).id;
			break;
		case 2:
			nSubGearId = ((HeroSubGear)obj).subGearId;
			break;
		case 3:
		{
			ItemInventoryObject itemObject = (ItemInventoryObject)obj;
			nItemId = itemObject.itemId;
			nItemCount = itemObject.count;
			bItemOwned = itemObject.owned;
			break;
		}
		case 4:
			heroMountGearId = ((HeroMountGear)obj).id;
			break;
		default:
			throw new Exception("유효하지 않은 인벤토리 객체입니다.");
		}
		return GameDac.CSC_UpdateInventorySlot(heroId, slot.index, nType, herMainGearId, nSubGearId, nItemId, nItemCount, bItemOwned, heroMountGearId);
	}

	public static SqlCommand CSC_AddOrUpdateInventorySlot(InventorySlot slot)
	{
		Guid heroId = slot.hero.id;
		IInventoryObject obj = slot.obj;
		int nType = obj.inventoryObjectType;
		Guid? herMainGearId = null;
		int nSubGearId = 0;
		int nItemId = 0;
		int nItemCount = 0;
		bool bItemOwned = false;
		Guid? heroMountGearId = null;
		switch (nType)
		{
		case 1:
			herMainGearId = ((HeroMainGear)obj).id;
			break;
		case 2:
			nSubGearId = ((HeroSubGear)obj).subGearId;
			break;
		case 3:
		{
			ItemInventoryObject itemObject = (ItemInventoryObject)obj;
			nItemId = itemObject.itemId;
			nItemCount = itemObject.count;
			bItemOwned = itemObject.owned;
			break;
		}
		case 4:
			heroMountGearId = ((HeroMountGear)obj).id;
			break;
		default:
			throw new Exception("유효하지 않은 인벤토리 객체입니다.");
		}
		return GameDac.CSC_AddOrUpdateInventorySlot(heroId, slot.index, nType, herMainGearId, nSubGearId, nItemId, nItemCount, bItemOwned, heroMountGearId);
	}

	public static SqlCommand CSC_UpdateOrDeleteInventorySlot(InventorySlot slot)
	{
		if (slot.isEmpty)
		{
			return GameDac.CSC_DeleteInventorySlot(slot.hero.id, slot.index);
		}
		return CSC_UpdateInventorySlot(slot);
	}

	public static SqlCommand CSC_ApplyChangedInventorySlots(InventorySlot slot)
	{
		if (slot.isEmpty)
		{
			return GameDac.CSC_DeleteInventorySlot(slot.hero.id, slot.index);
		}
		return CSC_AddOrUpdateInventorySlot(slot);
	}

	public static SqlCommand CSC_UpdateHero_MainGear(Hero hero)
	{
		return GameDac.CSC_UpdateHero_MainGear(hero.id, hero.equippedWeaponId, hero.equippedArmorId);
	}

	public static List<SqlCommand> CSC_AddMail(Mail mail)
	{
		List<SqlCommand> sqlCommands = new List<SqlCommand>();
		sqlCommands.Add(GameDac.CSC_AddMail(mail.id, mail.hero.id, mail.titleType, mail.title, mail.contentType, mail.content, mail.regTime, mail.expireTime));
		foreach (MailAttachment attachment in mail.attachments)
		{
			sqlCommands.Add(GameDac.CSC_AddMailAttachments(mail.id, attachment.no, attachment.item.id, attachment.itemCount, attachment.itemOwned));
		}
		return sqlCommands;
	}

	public static List<SqlCommand> CSC_AddHeroMainGear(HeroMainGear mainGear)
	{
		List<SqlCommand> sqlCommands = new List<SqlCommand>();
		sqlCommands.Add(GameDac.CSC_AddHeroMainGear(mainGear.id, mainGear.hero.id, mainGear.gear.id, mainGear.enchantLevel, mainGear.owned, mainGear.regTime));
		foreach (HeroMainGearOptionAttr attr in mainGear.optionAttrs)
		{
			sqlCommands.Add(GameDac.CSC_AddHeroMainGearOptionAttr(attr.heroMainGear.id, attr.index, attr.attrGrade, attr.attrId, attr.attrValue.id));
		}
		return sqlCommands;
	}

	public static SqlCommand CSC_UpdateAccount_UnOwnDia(Account account)
	{
		return GameDac.CSC_UpdateAccount_UnOwnDia(account.id, account.baseUnOwnDia, account.bonusUnOwnDia);
	}

	public static SqlCommand CSC_UpdateHero_OwnDia(Hero hero)
	{
		return GameDac.CSC_UpdateHero_OwnDia(hero.id, hero.ownDia);
	}

	public static List<SqlCommand> CSC_AddHeroMountGear(HeroMountGear mountGear)
	{
		List<SqlCommand> sqlCommands = new List<SqlCommand>();
		sqlCommands.Add(GameDac.CSC_AddHeroMountGear(mountGear.id, mountGear.hero.id, mountGear.gear.id, mountGear.owned, mountGear.regTime));
		foreach (HeroMountGearOptionAttr attr in mountGear.optionAttrs)
		{
			sqlCommands.Add(GameDac.CSC_AddHeroMountGearOptionAttr(attr.gear.id, attr.index, attr.grade, attr.attrId, attr.attrValue.id));
		}
		return sqlCommands;
	}

	public static List<SqlCommand> CSC_AddHeroRealAttrValues(Hero hero)
	{
		List<SqlCommand> sqlCommands = new List<SqlCommand>();
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 1, hero.realMaxHP));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 2, hero.realPhysicalOffense));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 3, hero.realMagicalOffense));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 4, hero.realPhysicalDefense));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 5, hero.realMagicalDefense));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 6, hero.realCritical));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 7, hero.realCriticalResistance));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 8, hero.realCriticalDamageIncRate));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 9, hero.realCriticalDamageDecRate));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 10, hero.realPenetration));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 11, hero.realBlock));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 12, hero.realFireOffense));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 13, hero.realFireDefense));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 14, hero.realLightningOffense));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 15, hero.realLightningDefense));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 16, hero.realDarkOffense));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 17, hero.realDarkDefense));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 18, hero.realHolyOffense));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 19, hero.realHolyDefense));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 20, hero.realDamageIncRate));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 21, hero.realDamageDecRate));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 22, hero.realStunResistance));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 23, hero.realSnareResistance));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 24, hero.realSilenceResistance));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 25, hero.realBaseMaxHPIncRate));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 26, hero.realBaseOffenseIncRate));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 27, hero.realBasePhysicalDenfenseIncRate));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 28, hero.realBaseMagicalDefenseIncRate));
		sqlCommands.Add(GameDac.CSC_AddHeroRealAttrValue(hero.id, 29, hero.realOffense));
		return sqlCommands;
	}

	public static SqlCommand CSC_UpdateHeroMainQuest_Cart(HeroMainQuest quest)
	{
		return GameDac.CSC_UpdateHeroMainQuest_Cart(quest.hero.id, quest.mainQuest.no, quest.isCartRiding, quest.cartContinentId, quest.cartPosition.x, quest.cartPosition.y, quest.cartPosition.z, quest.cartRotationY);
	}

	public static SqlCommand CSC_AddFieldOfHonorHistory(FieldOfHonorHistory history)
	{
		return GameDac.CSC_AddFieldOfHonorHistory(history.hero.id, history.id, history.type, history.targetHeroId, history.oldRanking, history.ranking, history.isWin, history.regTime);
	}

	public static SqlCommand CSC_AddOrUpdateFieldOfHonorHero(FieldOfHonorHero hero)
	{
		return GameDac.CSC_AddOrUpdateFieldOfHonorHero(hero.id, hero.jobId, hero.nationId, hero.name, hero.level, hero.battlePower, hero.rankId, hero.equippedWingId, hero.wingStep, hero.wingLevel, hero.displayTitleId, hero.guildId, hero.guildName, hero.guildMemberGrade, hero.mainGearEnchantLevelSetNo, hero.subGearSoulstoneLevelSetNo, hero.customPresetHair, hero.customFaceJawHeight, hero.customFaceJawWidth, hero.customFaceJawEndHeight, hero.customFaceWidth, hero.customFaceEyebrowHeight, hero.customFaceEyebrowRotation, hero.customFaceEyesWidth, hero.customFaceNoseHeight, hero.customFaceNoseWidth, hero.customFaceMouthHeight, hero.customFaceMouthWidth, hero.customBodyHeadSize, hero.customBodyArmsLength, hero.customBodyArmsWidth, hero.customBodyChestSize, hero.customBodyWaistWidth, hero.customBodyHipsSize, hero.customBodyPelvisWidth, hero.customBodyLegsLength, hero.customBodyLegsWidth, hero.customColorSkin, hero.customColorEyes, hero.customColorBeardAndEyebrow, hero.customColorHair, hero.costumeId, hero.costumeEffectId);
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroRealAttr(FieldOfHonorHeroRealAttr realAttr)
	{
		return GameDac.CSC_AddFieldOfHonorHeroRealAttr(realAttr.hero.id, realAttr.id, realAttr.value);
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroSkill(FieldOfHonorHeroSkill skill)
	{
		return GameDac.CSC_AddFieldOfHonorHeroSkill(skill.hero.id, skill.id, skill.level);
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroWing(FieldOfHonorHeroWing wing)
	{
		return GameDac.CSC_AddFieldOfHonorHeroWing(wing.hero.id, wing.id);
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroWingEnchant(FieldOfHonorHeroWingEnchant wingEnchant)
	{
		return GameDac.CSC_AddFieldOfHonorHeroWingEnchant(wingEnchant.hero.id, wingEnchant.part.id, wingEnchant.step, wingEnchant.level, wingEnchant.enchantCount);
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroEquippedMainGear(FieldOfHonorHeroEquippedMainGear heroMainGear)
	{
		return GameDac.CSC_AddFieldOfHonorHeroEquippedMainGear(heroMainGear.heroMainGearId, heroMainGear.hero.id, heroMainGear.mainGear.id, heroMainGear.enchantLevel);
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroMainGearOptionAttr(FieldOfHonorHeroMainGearOptionAttr optionAttr)
	{
		return GameDac.CSC_AddFieldOfHonorHeroMainGearOptionAttr(optionAttr.heroMainGear.heroMainGearId, optionAttr.index, optionAttr.grade, optionAttr.attrId, optionAttr.attrValue.id);
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroEquippedSubGear(FieldOfHonorHeroEquippedSubGear heroSubGear)
	{
		return GameDac.CSC_AddFieldOfHonorHeroEquippedSubGear(heroSubGear.hero.id, heroSubGear.subGear.id, heroSubGear.level, heroSubGear.quality);
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroSubGearRuneSocket(FieldOfHonorHeroSubGearRuneSocket runeSocket)
	{
		return GameDac.CSC_AddFieldOfHonorHeroSubGearRuneSocket(runeSocket.hero.id, runeSocket.equippedSubGear.subGear.id, runeSocket.index, runeSocket.item.id);
	}

	public static SqlCommand CSC_AddFieldOfHonorHeroSubGearSoulstoneSocket(FieldOfHonorHeroSubGearSoulstoneSocket soulstoneSocket)
	{
		return GameDac.CSC_AddFieldOfHonorHeroSubGearSoulstoneSocket(soulstoneSocket.hero.id, soulstoneSocket.equippedSubGear.subGear.id, soulstoneSocket.index, soulstoneSocket.item.id);
	}

	public static SqlCommand CSC_UpdateSupplySupportQuest_Cart(HeroSupplySupportQuest quest)
	{
		return GameDac.CSC_UpdateSupplySupportQuest_Cart(quest.id, quest.cart.id, quest.cartHp, quest.isCartRiding, quest.cartContinentId, quest.cartPosition.x, quest.cartPosition.y, quest.cartPosition.z, quest.cartYRotation);
	}

	public static List<SqlCommand> CSC_AddGuild(Guild guild, DateTimeOffset currentTime)
	{
		List<SqlCommand> sqlCommands = new List<SqlCommand>();
		sqlCommands.Add(GameDac.CSC_AddGuild(guild.id, guild.name, guild.nationInst.nationId, guild.foodWarehouseLevel, guild.foodWarehouseExp, guild.dailyObjectiveDate, guild.dailyObjectiveContentId, guild.weeklyObjectiveDate, guild.weeklyObjectiveId, currentTime));
		foreach (GuildBuildingInstance building in guild.buildingInsts.Values)
		{
			sqlCommands.Add(GameDac.CSC_AddGuildBuilding(guild.id, building.level.building.id, building.level.level));
		}
		return sqlCommands;
	}

	public static SqlCommand CSC_UpdateHero_GuildContributionPoint(Hero hero)
	{
		return GameDac.CSC_UpdateHero_GuildContributionPoint(hero.id, hero.totalGuildContributionPoint, hero.guildContributionPoint);
	}

	public static SqlCommand CSC_UpdateGuild_FoodWarehouse(Guild guild)
	{
		return GameDac.CSC_UpdateGuild_FoodWarehouse(guild.id, guild.foodWarehouseLevel, guild.foodWarehouseExp, guild.foodWarehouseCollectionId);
	}

	public static SqlCommand CSC_AddNationWarMember(NationWarMember member)
	{
		return GameDac.CSC_AddNationWarMember(member.declaration.id, member.heroId, member.killCount, member.assistCount, member.deadCount, member.immediateRevivalCount, member.regTime, member.rewarded);
	}

	public static SqlCommand CSC_UpdateGuildSupplySupportQuest_Cart(GuildSupplySupportQuestPlay quest)
	{
		return GameDac.CSC_UpdateGuildSupplySupportQuest_Cart(quest.id, quest.cart.id, quest.cartHp, quest.isCartRiding, quest.cartContinentId, quest.cartPosition.x, quest.cartPosition.y, quest.cartPosition.z, quest.cartYRotation);
	}

	public static SqlCommand CSC_AddOrUpdateHeroCreatureCard(HeroCreatureCard card)
	{
		return GameDac.CSC_AddOrUpdateHeroCreatureCard(card.hero.id, card.card.id, card.count);
	}

	public static SqlCommand CSC_UpdateOrDeleteHeroCreatureCard(HeroCreatureCard card)
	{
		if (card.count <= 0)
		{
			return GameDac.CSC_DeleteHeroCreatureCard(card.hero.id, card.card.id);
		}
		return GameDac.CSC_UpdateHeroCreatureCard(card.hero.id, card.card.id, card.count);
	}

	public static SqlCommand CSC_AddOrUpdateHeroWeeklyQuest(HeroWeeklyQuest heroWeeklyQuest)
	{
		return GameDac.CSC_AddOrUpdateHeroWeeklyQuest(heroWeeklyQuest.hero.id, heroWeeklyQuest.weekStartDate, heroWeeklyQuest.roundNo, heroWeeklyQuest.roundId, heroWeeklyQuest.roundMissionId, heroWeeklyQuest.roundProgressCount, (int)heroWeeklyQuest.roundStatus);
	}

	public static SqlCommand CSC_UpdateWarehouseSlot(WarehouseSlot slot)
	{
		Guid heroId = slot.hero.id;
		IWarehouseObject obj = slot.obj;
		int nType = obj.warehouseObjectType;
		Guid herMainGearId = Guid.Empty;
		int nSubGearId = 0;
		int nItemId = 0;
		int nItemCount = 0;
		bool bItemOwned = false;
		Guid heroMountGearId = Guid.Empty;
		switch (nType)
		{
		case 1:
			herMainGearId = ((HeroMainGear)obj).id;
			break;
		case 2:
			nSubGearId = ((HeroSubGear)obj).subGearId;
			break;
		case 3:
		{
			ItemWarehouseObject itemObject = (ItemWarehouseObject)obj;
			nItemId = itemObject.itemId;
			nItemCount = itemObject.count;
			bItemOwned = itemObject.owned;
			break;
		}
		case 4:
			heroMountGearId = ((HeroMountGear)obj).id;
			break;
		default:
			throw new Exception("유효하지 않은 창고 객체입니다.");
		}
		return GameDac.CSC_UpdateWarehouseSlot(heroId, slot.index, nType, herMainGearId, nSubGearId, nItemId, nItemCount, bItemOwned, heroMountGearId);
	}

	public static SqlCommand CSC_AddOrUpdateWarehouseSlot(WarehouseSlot slot)
	{
		Guid heroId = slot.hero.id;
		IWarehouseObject obj = slot.obj;
		int nType = obj.warehouseObjectType;
		Guid herMainGearId = Guid.Empty;
		int nSubGearId = 0;
		int nItemId = 0;
		int nItemCount = 0;
		bool bItemOwned = false;
		Guid heroMountGearId = Guid.Empty;
		switch (nType)
		{
		case 1:
			herMainGearId = ((HeroMainGear)obj).id;
			break;
		case 2:
			nSubGearId = ((HeroSubGear)obj).subGearId;
			break;
		case 3:
		{
			ItemWarehouseObject itemObject = (ItemWarehouseObject)obj;
			nItemId = itemObject.itemId;
			nItemCount = itemObject.count;
			bItemOwned = itemObject.owned;
			break;
		}
		case 4:
			heroMountGearId = ((HeroMountGear)obj).id;
			break;
		default:
			throw new Exception("유효하지 않은 창고 객체입니다.");
		}
		return GameDac.CSC_AddOrUpdateWarehouseSlot(heroId, slot.index, nType, herMainGearId, nSubGearId, nItemId, nItemCount, bItemOwned, heroMountGearId);
	}

	public static SqlCommand CSC_UpdateOrDeleteWarehouseSlot(WarehouseSlot slot)
	{
		if (slot.isEmpty)
		{
			return GameDac.CSC_DeleteWarehouseSlot(slot.hero.id, slot.index);
		}
		return CSC_UpdateWarehouseSlot(slot);
	}

	public static SqlCommand CSC_ApplyChangedWarehouseSlots(WarehouseSlot slot)
	{
		if (slot.isEmpty)
		{
			return GameDac.CSC_DeleteWarehouseSlot(slot.hero.id, slot.index);
		}
		return CSC_AddOrUpdateWarehouseSlot(slot);
	}

	public static List<SqlCommand> CSC_AddHeroCreature(HeroCreature creature)
	{
		List<SqlCommand> sqlCommands = new List<SqlCommand>();
		sqlCommands.Add(GameDac.CSC_AddHeroCreature(creature.instanceId, creature.hero.id, creature.creature.id, creature.level, creature.injectionLevel, creature.quality));
		foreach (HeroCreatureBaseAttr baseAttr in creature.baseAttrs.Values)
		{
			sqlCommands.Add(GameDac.CSC_AddHeroCreatureBaseAttr(baseAttr.creature.instanceId, baseAttr.attr.attr.attrId, baseAttr.baseValue));
		}
		foreach (HeroCreatureAdditionalAttr additionalAttr in creature.additionalAttrs)
		{
			sqlCommands.Add(GameDac.CSC_AddHeroCreatureAdditionalAttr(additionalAttr.creature.instanceId, additionalAttr.no, additionalAttr.attr.attrId));
		}
		HeroCreatureSkill[] skills = creature.skills;
		foreach (HeroCreatureSkill skill in skills)
		{
			if (skill.skillAttr != null)
			{
				sqlCommands.Add(GameDac.CSC_AddHeroCreatureSkill(skill.creature.instanceId, skill.slotIndex, skill.skillAttr.skill.id, skill.skillAttr.grade.grade));
			}
		}
		return sqlCommands;
	}

	public static List<SqlCommand> CSC_DeleteHeroCreature(HeroCreature creature)
	{
		List<SqlCommand> sqlCommands = new List<SqlCommand>();
		sqlCommands.Add(GameDac.CSC_DeleteHeroCreature(creature.instanceId));
		sqlCommands.Add(GameDac.CSC_DeleteHeroCreatureBaseAttrs(creature.instanceId));
		sqlCommands.Add(GameDac.CSC_DeleteHeroCreatureAdditionalAttrs(creature.instanceId));
		sqlCommands.Add(GameDac.CSC_DeleteHeroCreatureSkills(creature.instanceId));
		return sqlCommands;
	}
}
