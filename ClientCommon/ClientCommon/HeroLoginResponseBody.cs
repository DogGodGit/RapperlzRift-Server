using System;

namespace ClientCommon;

public class HeroLoginResponseBody : ResponseBody
{
	public DateTimeOffset time;

	public DateTime date;

	public DateTime serverOpenDate;

	public int serverMaxLevel;

	public int nationId;

	public int secretLetterQuestTargetNationId;

	public PDNationInstance[] nationInsts;

	public PDNationPowerRanking[] nationPowerRankings;

	public PDNationAlliance[] nationAlliances;

	public PDNationAllianceApplication[] nationAllianceApplications;

	public Guid id;

	public string name;

	public int jobId;

	public int level;

	public long exp;

	public int maxHP;

	public int hp;

	public int ownDia;

	public int unOwnDia;

	public long gold;

	public int honorPoint;

	public int vipPoint;

	public int[] receivedVipLevelRewards;

	public int lak;

	public int stamina;

	public float staminaAutoRecoveryRemainingTime;

	public int dailyStaminaBuyCount;

	public int rankNo;

	public DateTime rankRewardReceivedDate;

	public int rankRewardReceivedRankNo;

	public PDFullHeroMainGear[] mainGears;

	public Guid equippedWeaponId;

	public Guid equippedArmorId;

	public PDFullHeroSubGear[] subGears;

	public PDHeroSkill[] skills;

	public PDHeroMount[] mounts;

	public int equippedMountId;

	public bool bIsRiding;

	public PDHeroMountGear[] mountGears;

	public Guid[] equippedMountGears;

	public int mountGearRefinementDailyCount;

	public int paidInventorySlotCount;

	public PDInventorySlot[] placedInventorySlots;

	public PDMail[] mails;

	public int initEntranceLocationId;

	public int initEntranceLocationParam;

	public int previousContinentId;

	public int previousNationId;

	public int mainGearEnchantDailyCount;

	public int mainGearRefinementDailyCount;

	public PDHeroMainQuest currentMainQuest;

	public PDHeroTreatOfFarmQuest treatOfFarmQuest;

	public PDHeroBountyHunterQuest bountyHunterQuest;

	public int bountyHunterDailyStartCount;

	public PDHeroFishingQuest fishingQuest;

	public int fishingQuestDailyStartCount;

	public int freeImmediateRevivalDailyCount;

	public int paidImmediateRevivalDailyCount;

	public int restTime;

	public PDParty party;

	public int expPotionDailyUseCount;

	public int[] receivedLevelUpRewards;

	public int[] receivedDailyAcessRewards;

	public float dailyAccessTime;

	public DateTime receivedAttendRewardDate;

	public int receivedAttendRewardDay;

	public PDHeroSeriesMission[] seriesMissions;

	public PDHeroTodayMission[] todayMissions;

	public int rewardedAttainmentEntryNo;

	public PDHeroWing[] wings;

	public int equippedWingId;

	public int wingStep;

	public int wingLevel;

	public int wingExp;

	public PDHeroWingPart[] wingParts;

	public int freeSweepDailyCount;

	public PDStoryDungeonPlay[] storyDungeonPlays;

	public PDStoryDungeonClear[] storyDungeonClears;

	public int expDungeonDailyPlayCount;

	public int[] expDungeonClearedDifficulties;

	public int goldDungeonDailyPlayCount;

	public int[] goldDungeonClearedDifficulties;

	public float undergroundMazeDailyPlayTime;

	public int artifactRoomBestFloor;

	public int artifactRoomCurrentFloor;

	public int artifactRoomDailyInitCount;

	public int artifactRoomSweepProgressFloor;

	public float artifactRoomSweepRemainingTime;

	public int ancientRelicDailyPlayCount;

	public int fieldOfHonorDailyPlayCount;

	public int rewardedDailyFieldOfHonorRankingNo;

	public int dailyFieldOfHonorRankingNo;

	public int dailyfieldOfHonorRanking;

	public int mainGearEnchantLevelSetNo;

	public int subGearSoulstoneLevelSetNo;

	public int expScrollDailyUseCount;

	public int expScrollItemId;

	public float expScrollRemainingTime;

	public int exploitPoint;

	public int dailyExploitPoint;

	public int dailyMysteryBoxQuestStartCount;

	public PDHeroMysteryBoxQuest mysteryBoxQuest;

	public int dailySecretLetterQuestStartCount;

	public PDHeroSecretLetterQuest secretLetterQuest;

	public int dailyDimensionRaidQuestStartCount;

	public PDHeroDimensionRaidQuest dimensionRaidQuest;

	public int[] dailyHolyWarQuestStartSchedules;

	public PDHeroHolyWarQuest holyWarQuest;

	public int dailySupplySupportQuestStartCount;

	public PDHeroSupplySupportQuest supplySupportQuest;

	public PDHeroTodayTask[] todayTasks;

	public int achievementDailyPoint;

	public int receivedAchievementRewardNo;

	public PDCartInstance ridingCartInst;

	public int distortionScrollDailyUseCount;

	public float remainingDistortionTime;

	public int dailyServerLevelRakingNo;

	public int dailyServerLevelRanking;

	public int rewardedDailyServerLevelRankingNo;

	public PDGuild guild;

	public int guildMemberGrade;

	public int totalGuildContributionPoint;

	public int guildContributionPoint;

	public int guildPoint;

	public float guildRejoinRemainingTime;

	public DateTime guildDailyRewardReceivedDate;

	public PDHeroGuildApplication[] guildApplications;

	public int dailyGuildApplicationCount;

	public int dailyGuildDonationCount;

	public int dailyGuildFarmQuestStartCount;

	public PDHeroGuildFarmQuest guildFarmQuest;

	public PDHeroGuildSkill[] guildSkills;

	public int dailyGuildFoodWarehouseStockCount;

	public Guid receivedGuildFoodWarehouseCollectionId;

	public int guildMoralPoint;

	public float guildAltarDefenseMissionRemainingCoolTime;

	public DateTime guildAltarRewardReceivedDate;

	public PDHeroGuildMissionQuest guildMissionQuest;

	public PDGuildSupplySupportQuestPlay guildSupplySupportQuestPlay;

	public PDHeroGuildHuntingQuest guildHuntingQuest;

	public int dailyGuildHuntingQuestStartCount;

	public DateTime guildHuntingDonationDate;

	public DateTime guildHuntingDonationRewardReceivedDate;

	public int guildDailyObjectiveRewardReceivedNo;

	public DateTime guildWeeklyObjectiveRewardReceivedDate;

	public float guildDailyObjectiveNoticeRemainingCoolTime;

	public PDNationWarDeclaration[] nationWarDeclarations;

	public int weeklyNationWarDeclarationCount;

	public bool nationWarJoined;

	public int nationWarKillCount;

	public int nationWarAssistCount;

	public int nationWarDeadCount;

	public int nationWarImmediateRevivalCount;

	public int dailyNationWarFreeTransmissionCount;

	public int dailyNationWarPaidTransmissionCount;

	public int dailyNationWarCallCount;

	public float nationWarCallRemainingCoolTime;

	public int dailyNationWarConvergingAttackCount;

	public float nationWarConvergingAttackRemainingCoolTime;

	public int nationWarConvergingAttackTargetArrangeId;

	public PDSimpleNationWarMonsterInstance[] nationWarMonsterInsts;

	public int dailyNationDonationCount;

	public int weeklySoulCoveterPlayCount;

	public int illustratedBookExplorationStepNo;

	public int explorationPoint;

	public DateTime illustratedBookExplorationStepRewardReceivedDate;

	public int illustratedBookExplorationStepRewardReceivedStepNo;

	public int[] activationIllustratedBookIds;

	public int[] completedSceneryQuests;

	public int[] rewardedAccomplishments;

	public int accMonsterKillCount;

	public int accSoulCoveterPlayCount;

	public int accEpicBaitItemUseCount;

	public int accLegendBaitItemUseCount;

	public int accNationWarWinCount;

	public int accNationWarKillCount;

	public int accNationWarCommanderKillCount;

	public int accNationWarImmediateRevivalCount;

	public long maxGold;

	public long maxBattlePower;

	public int maxAcquisitionMainGearGrade;

	public int maxEquippedMainGearEnchantLevel;

	public PDHeroTitle[] titles;

	public int displayTitleId;

	public int activationTitleId;

	public int soulPowder;

	public PDHeroCreatureCard[] creatureCards;

	public int[] activatedCreatureCardCollections;

	public int creatureCardCollectionFamePoint;

	public int[] purchasedCreatureCardShopFixedProducts;

	public PDHeroCreatureCardShopRandomProduct[] creatureCardShopRandomProducts;

	public int dailyCreatureCardShopPaidRefreshCount;

	public PDHeroEliteMonsterKill[] heroEliteMonsterKills;

	public int[] spawnedEliteMonsters;

	public int dailyEliteDungeonPlayCount;

	public int lootingItemMinGrade;

	public bool proofOfValorCleared;

	public int dailyProofOfValorPlayCount;

	public int dailyProofOfValorFreeRefreshCount;

	public int dailyProofOfValorPaidRefreshCount;

	public int poofOfValorPaidRefreshCount;

	public PDHeroProofOfValorInstance heroProofOfValorInstance;

	public int customPresetHair;

	public int customFaceJawHeight;

	public int customFaceJawWidth;

	public int customFaceJawEndHeight;

	public int customFaceWidth;

	public int customFaceEyebrowHeight;

	public int customFaceEyebrowRotation;

	public int customFaceEyesWidth;

	public int customFaceNoseHeight;

	public int customFaceNoseWidth;

	public int customFaceMouthHeight;

	public int customFaceMouthWidth;

	public int customBodyHeadSize;

	public int customBodyArmsLength;

	public int customBodyArmsWidth;

	public int customBodyChestSize;

	public int customBodyWaistWidth;

	public int customBodyHipsSize;

	public int customBodyPelvisWidth;

	public int customBodyLegsLength;

	public int customBodyLegsWidth;

	public int customColorSkin;

	public int customColorEyes;

	public int customColorBeardAndEyebrow;

	public int customColorHair;

	public bool todayMissionTutorialStarted;

	public PDHeroNpcShopProduct[] heroNpcShopProducts;

	public PDHeroRankActiveSkill[] rankActiveSkills;

	public PDHeroRankPassiveSkill[] rankPassiveSkills;

	public int selectedRankActiveSkillId;

	public float rankActiveSkillRemainingCoolTime;

	public int spiritStone;

	public int rookieGiftNo;

	public float rookieGiftRemainingTime;

	public int[] receivedOpenGiftRewards;

	public DateTime regDate;

	public int dailyQuestAcceptionCount;

	public int dailyQuestFreeRefreshCount;

	public PDHeroDailyQuest[] dailyQuests;

	public PDHeroWeeklyQuest weeklyQuest;

	public bool wisdomTempleCleared;

	public int dailyWisdomTemplePlayCount;

	public int[] rewardedOpen7DayEventMissions;

	public int[] purchasedOpen7DayEventProducts;

	public PDHeroOpen7DayEventProgressCount[] open7DayEventProgressCounts;

	public bool open7DayEventRewarded;

	public PDHeroRetrievalProgressCount[] retrievalProgressCounts;

	public PDHeroRetrieval[] retrievals;

	public int dailyRuinsReclaimFreePlayCount;

	public PDHeroTaskConsignment[] taskConsignments;

	public PDHeroTaskConsignmentStartCount[] taskConsignmentStartCounts;

	public PDHeroTrueHeroQuest trueHeroQuest;

	public int dailyInfiniteWarPlayCount;

	public int[] spawnedFieldBosses;

	public int[] rewardedLimitationGiftScheduleIds;

	public PDHeroWeekendReward weekendReward;

	public int paidWarehouseSlotCount;

	public PDWarehouseSlot[] placedWarehouseSlots;

	public PDHeroDiaShopProductBuyCount[] dailyDiaShopProductBuyCounts;

	public PDHeroDiaShopProductBuyCount[] totalDiaShopProductBuyCounts;

	public int dailyFearAltarPlayCount;

	public int weeklyFearAltarHalidomCollectionRewardNo;

	public int[] weeklyFearAltarHalidoms;

	public int[] weeklyRewardReceivedFearAltarHalidomElementals;

	public PDHeroSubQuest[] subQuests;

	public int dailyWarMemoryFreePlayCount;

	public PDHeroOrdealQuest ordealQuest;

	public int dailyOsirisRoomPlayCount;

	public PDHeroBiography[] biographies;

	public PDFriend[] friends;

	public PDTempFriend[] tempFriends;

	public PDBlacklistEntry[] blacklistEntries;

	public PDDeadRecord[] deadRecords;

	public PDHeroProspectQuest[] ownerProspectQuests;

	public PDHeroProspectQuest[] targetProspectQuests;

	public float itemLuckyShopFreePickRemainingTime;

	public int itemLuckyShopFreePickCount;

	public int itemLuckyShopPick1TimeCount;

	public int itemLuckyShopPick5TimeCount;

	public float creatureCardLuckyShopFreePickRemainingTime;

	public int creatureCardLuckyShopFreePickCount;

	public int creatureCardLuckyShopPick1TimeCount;

	public int creatureCardLuckyShopPick5TimeCount;

	public PDHeroCreature[] creatures;

	public Guid participatedCreatureId;

	public int dailyCreatureVariationCount;

	public int weeklyPresentPopularityPoint;

	public int weeklyPresentContributionPoint;

	public int nationWeeklyPresentPopularityPointRankingNo;

	public int nationWeeklyPresentPopularityPointRanking;

	public int rewardedNationWeeklyPresentPopularityPointRankingNo;

	public int nationWeeklyPresentContributionPointRankingNo;

	public int nationWeeklyPresentContributionPointRanking;

	public int rewardedNationWeeklyPresentContributionPointRankingNo;

	public PDHeroCostume[] costumes;

	public int equippedCostumeId;

	public int costumeCollectionId;

	public bool costumeCollectionActivated;

	public int dailyCreatureFarmQuestAcceptionCount;

	public PDHeroCreatureFarmQuest creatureFarmQuest;

	public PDCashProductPurchaseCount[] cashProductPurchaseCounts;

	public bool firstChargeEventObjectiveCompleted;

	public bool firstChargeEventRewarded;

	public int rechargeEventAccUnOwnDia;

	public bool rechargeEventRewarded;

	public PDAccountChargeEvent chargeEvent;

	public int dailyChargeEventAccUnOwnDia;

	public int[] rewardedDailyChargeEventMissions;

	public PDAccountConsumeEvent consumeEvent;

	public int dailyConsumeEventAccDia;

	public int[] rewardedDailyConsumeEventMissions;

	public PDHeroJobChangeQuest jobChangeQuest;

	public PDHeroPotionAttr[] potionAttrs;

	public int dailyAnkouTombPlayCount;

	public PDHeroAnkouTombBestRecord[] myAnkouTombBestRecords;

	public PDHeroAnkouTombBestRecord[] serverAnkouTombBestRecords;

	public PDHeroConstellation[] constellations;

	public int starEssense;

	public int dailyStarEssenseItemUseCount;

	public int artifactNo;

	public int artifactLevel;

	public int artifactExp;

	public int equippedArtifactNo;

	public int dailyTradeShipPlayCount;

	public PDHeroTradeShipBestRecord[] myTradeShipBestRecords;

	public PDHeroTradeShipBestRecord[] serverTradeShipBestRecords;

	public override void Serialize(PacketWriter writer)
	{
		base.Serialize(writer);
		writer.Write(time);
		writer.Write(date);
		writer.Write(serverOpenDate);
		writer.Write(serverMaxLevel);
		writer.Write(nationId);
		writer.Write(secretLetterQuestTargetNationId);
		writer.Write(nationInsts);
		writer.Write(nationPowerRankings);
		writer.Write(nationAlliances);
		writer.Write(nationAllianceApplications);
		writer.Write(id);
		writer.Write(name);
		writer.Write(jobId);
		writer.Write(level);
		writer.Write(exp);
		writer.Write(maxHP);
		writer.Write(hp);
		writer.Write(ownDia);
		writer.Write(unOwnDia);
		writer.Write(gold);
		writer.Write(honorPoint);
		writer.Write(vipPoint);
		writer.Write(receivedVipLevelRewards);
		writer.Write(lak);
		writer.Write(stamina);
		writer.Write(staminaAutoRecoveryRemainingTime);
		writer.Write(dailyStaminaBuyCount);
		writer.Write(rankNo);
		writer.Write(rankRewardReceivedDate);
		writer.Write(rankRewardReceivedRankNo);
		writer.Write(mainGears);
		writer.Write(equippedWeaponId);
		writer.Write(equippedArmorId);
		writer.Write(subGears);
		writer.Write(skills);
		writer.Write(mounts);
		writer.Write(equippedMountId);
		writer.Write(bIsRiding);
		writer.Write(mountGears);
		writer.Write(equippedMountGears);
		writer.Write(mountGearRefinementDailyCount);
		writer.Write(paidInventorySlotCount);
		writer.Write(placedInventorySlots);
		writer.Write(mails);
		writer.Write(initEntranceLocationId);
		writer.Write(initEntranceLocationParam);
		writer.Write(previousContinentId);
		writer.Write(previousNationId);
		writer.Write(mainGearEnchantDailyCount);
		writer.Write(mainGearRefinementDailyCount);
		writer.Write(currentMainQuest);
		writer.Write(treatOfFarmQuest);
		writer.Write(bountyHunterQuest);
		writer.Write(bountyHunterDailyStartCount);
		writer.Write(fishingQuest);
		writer.Write(fishingQuestDailyStartCount);
		writer.Write(freeImmediateRevivalDailyCount);
		writer.Write(paidImmediateRevivalDailyCount);
		writer.Write(restTime);
		writer.Write(party);
		writer.Write(expPotionDailyUseCount);
		writer.Write(receivedLevelUpRewards);
		writer.Write(receivedDailyAcessRewards);
		writer.Write(dailyAccessTime);
		writer.Write(receivedAttendRewardDate);
		writer.Write(receivedAttendRewardDay);
		writer.Write(seriesMissions);
		writer.Write(todayMissions);
		writer.Write(rewardedAttainmentEntryNo);
		writer.Write(wings);
		writer.Write(equippedWingId);
		writer.Write(wingStep);
		writer.Write(wingLevel);
		writer.Write(wingExp);
		writer.Write(wingParts);
		writer.Write(freeSweepDailyCount);
		writer.Write(storyDungeonPlays);
		writer.Write(storyDungeonClears);
		writer.Write(expDungeonDailyPlayCount);
		writer.Write(expDungeonClearedDifficulties);
		writer.Write(goldDungeonDailyPlayCount);
		writer.Write(goldDungeonClearedDifficulties);
		writer.Write(undergroundMazeDailyPlayTime);
		writer.Write(artifactRoomBestFloor);
		writer.Write(artifactRoomCurrentFloor);
		writer.Write(artifactRoomDailyInitCount);
		writer.Write(artifactRoomSweepProgressFloor);
		writer.Write(artifactRoomSweepRemainingTime);
		writer.Write(ancientRelicDailyPlayCount);
		writer.Write(fieldOfHonorDailyPlayCount);
		writer.Write(rewardedDailyFieldOfHonorRankingNo);
		writer.Write(dailyFieldOfHonorRankingNo);
		writer.Write(dailyfieldOfHonorRanking);
		writer.Write(mainGearEnchantLevelSetNo);
		writer.Write(subGearSoulstoneLevelSetNo);
		writer.Write(expScrollDailyUseCount);
		writer.Write(expScrollItemId);
		writer.Write(expScrollRemainingTime);
		writer.Write(exploitPoint);
		writer.Write(dailyExploitPoint);
		writer.Write(dailyMysteryBoxQuestStartCount);
		writer.Write(mysteryBoxQuest);
		writer.Write(dailySecretLetterQuestStartCount);
		writer.Write(secretLetterQuest);
		writer.Write(dailyDimensionRaidQuestStartCount);
		writer.Write(dimensionRaidQuest);
		writer.Write(dailyHolyWarQuestStartSchedules);
		writer.Write(holyWarQuest);
		writer.Write(dailySupplySupportQuestStartCount);
		writer.Write(supplySupportQuest);
		writer.Write(todayTasks);
		writer.Write(achievementDailyPoint);
		writer.Write(receivedAchievementRewardNo);
		writer.Write(ridingCartInst);
		writer.Write(distortionScrollDailyUseCount);
		writer.Write(remainingDistortionTime);
		writer.Write(dailyServerLevelRakingNo);
		writer.Write(dailyServerLevelRanking);
		writer.Write(rewardedDailyServerLevelRankingNo);
		writer.Write(guild);
		writer.Write(guildMemberGrade);
		writer.Write(totalGuildContributionPoint);
		writer.Write(guildContributionPoint);
		writer.Write(guildPoint);
		writer.Write(guildRejoinRemainingTime);
		writer.Write(guildDailyRewardReceivedDate);
		writer.Write(guildApplications);
		writer.Write(dailyGuildApplicationCount);
		writer.Write(dailyGuildDonationCount);
		writer.Write(dailyGuildFarmQuestStartCount);
		writer.Write(guildFarmQuest);
		writer.Write(guildSkills);
		writer.Write(dailyGuildFoodWarehouseStockCount);
		writer.Write(receivedGuildFoodWarehouseCollectionId);
		writer.Write(guildMoralPoint);
		writer.Write(guildAltarDefenseMissionRemainingCoolTime);
		writer.Write(guildAltarRewardReceivedDate);
		writer.Write(guildMissionQuest);
		writer.Write(guildSupplySupportQuestPlay);
		writer.Write(guildHuntingQuest);
		writer.Write(dailyGuildHuntingQuestStartCount);
		writer.Write(guildHuntingDonationDate);
		writer.Write(guildHuntingDonationRewardReceivedDate);
		writer.Write(guildDailyObjectiveRewardReceivedNo);
		writer.Write(guildWeeklyObjectiveRewardReceivedDate);
		writer.Write(guildDailyObjectiveNoticeRemainingCoolTime);
		writer.Write(nationWarDeclarations);
		writer.Write(weeklyNationWarDeclarationCount);
		writer.Write(nationWarJoined);
		writer.Write(nationWarKillCount);
		writer.Write(nationWarAssistCount);
		writer.Write(nationWarDeadCount);
		writer.Write(nationWarImmediateRevivalCount);
		writer.Write(dailyNationWarFreeTransmissionCount);
		writer.Write(dailyNationWarPaidTransmissionCount);
		writer.Write(dailyNationWarCallCount);
		writer.Write(nationWarCallRemainingCoolTime);
		writer.Write(dailyNationWarConvergingAttackCount);
		writer.Write(nationWarConvergingAttackRemainingCoolTime);
		writer.Write(nationWarConvergingAttackTargetArrangeId);
		writer.Write(nationWarMonsterInsts);
		writer.Write(dailyNationDonationCount);
		writer.Write(weeklySoulCoveterPlayCount);
		writer.Write(illustratedBookExplorationStepNo);
		writer.Write(explorationPoint);
		writer.Write(illustratedBookExplorationStepRewardReceivedDate);
		writer.Write(illustratedBookExplorationStepRewardReceivedStepNo);
		writer.Write(activationIllustratedBookIds);
		writer.Write(completedSceneryQuests);
		writer.Write(rewardedAccomplishments);
		writer.Write(accMonsterKillCount);
		writer.Write(accSoulCoveterPlayCount);
		writer.Write(accEpicBaitItemUseCount);
		writer.Write(accLegendBaitItemUseCount);
		writer.Write(accNationWarWinCount);
		writer.Write(accNationWarKillCount);
		writer.Write(accNationWarCommanderKillCount);
		writer.Write(accNationWarImmediateRevivalCount);
		writer.Write(maxGold);
		writer.Write(maxBattlePower);
		writer.Write(maxAcquisitionMainGearGrade);
		writer.Write(maxEquippedMainGearEnchantLevel);
		writer.Write(titles);
		writer.Write(displayTitleId);
		writer.Write(activationTitleId);
		writer.Write(soulPowder);
		writer.Write(creatureCards);
		writer.Write(activatedCreatureCardCollections);
		writer.Write(creatureCardCollectionFamePoint);
		writer.Write(purchasedCreatureCardShopFixedProducts);
		writer.Write(creatureCardShopRandomProducts);
		writer.Write(dailyCreatureCardShopPaidRefreshCount);
		writer.Write(heroEliteMonsterKills);
		writer.Write(spawnedEliteMonsters);
		writer.Write(dailyEliteDungeonPlayCount);
		writer.Write(lootingItemMinGrade);
		writer.Write(proofOfValorCleared);
		writer.Write(dailyProofOfValorPlayCount);
		writer.Write(dailyProofOfValorFreeRefreshCount);
		writer.Write(dailyProofOfValorPaidRefreshCount);
		writer.Write(poofOfValorPaidRefreshCount);
		writer.Write(heroProofOfValorInstance);
		writer.Write(customPresetHair);
		writer.Write(customFaceJawHeight);
		writer.Write(customFaceJawWidth);
		writer.Write(customFaceJawEndHeight);
		writer.Write(customFaceWidth);
		writer.Write(customFaceEyebrowHeight);
		writer.Write(customFaceEyebrowRotation);
		writer.Write(customFaceEyesWidth);
		writer.Write(customFaceNoseHeight);
		writer.Write(customFaceNoseWidth);
		writer.Write(customFaceMouthHeight);
		writer.Write(customFaceMouthWidth);
		writer.Write(customBodyHeadSize);
		writer.Write(customBodyArmsLength);
		writer.Write(customBodyArmsWidth);
		writer.Write(customBodyChestSize);
		writer.Write(customBodyWaistWidth);
		writer.Write(customBodyHipsSize);
		writer.Write(customBodyPelvisWidth);
		writer.Write(customBodyLegsLength);
		writer.Write(customBodyLegsWidth);
		writer.Write(customColorSkin);
		writer.Write(customColorEyes);
		writer.Write(customColorBeardAndEyebrow);
		writer.Write(customColorHair);
		writer.Write(todayMissionTutorialStarted);
		writer.Write(heroNpcShopProducts);
		writer.Write(rankActiveSkills);
		writer.Write(rankPassiveSkills);
		writer.Write(selectedRankActiveSkillId);
		writer.Write(rankActiveSkillRemainingCoolTime);
		writer.Write(spiritStone);
		writer.Write(rookieGiftNo);
		writer.Write(rookieGiftRemainingTime);
		writer.Write(receivedOpenGiftRewards);
		writer.Write(regDate);
		writer.Write(dailyQuestAcceptionCount);
		writer.Write(dailyQuestFreeRefreshCount);
		writer.Write(dailyQuests);
		writer.Write(weeklyQuest);
		writer.Write(wisdomTempleCleared);
		writer.Write(dailyWisdomTemplePlayCount);
		writer.Write(rewardedOpen7DayEventMissions);
		writer.Write(purchasedOpen7DayEventProducts);
		writer.Write(open7DayEventProgressCounts);
		writer.Write(open7DayEventRewarded);
		writer.Write(retrievalProgressCounts);
		writer.Write(retrievals);
		writer.Write(dailyRuinsReclaimFreePlayCount);
		writer.Write(taskConsignments);
		writer.Write(taskConsignmentStartCounts);
		writer.Write(trueHeroQuest);
		writer.Write(dailyInfiniteWarPlayCount);
		writer.Write(spawnedFieldBosses);
		writer.Write(rewardedLimitationGiftScheduleIds);
		writer.Write(weekendReward);
		writer.Write(paidWarehouseSlotCount);
		writer.Write(placedWarehouseSlots);
		writer.Write(dailyDiaShopProductBuyCounts);
		writer.Write(totalDiaShopProductBuyCounts);
		writer.Write(dailyFearAltarPlayCount);
		writer.Write(weeklyFearAltarHalidomCollectionRewardNo);
		writer.Write(weeklyFearAltarHalidoms);
		writer.Write(weeklyRewardReceivedFearAltarHalidomElementals);
		writer.Write(subQuests);
		writer.Write(dailyWarMemoryFreePlayCount);
		writer.Write(ordealQuest);
		writer.Write(dailyOsirisRoomPlayCount);
		writer.Write(biographies);
		writer.Write(friends);
		writer.Write(tempFriends);
		writer.Write(blacklistEntries);
		writer.Write(deadRecords);
		writer.Write(ownerProspectQuests);
		writer.Write(targetProspectQuests);
		writer.Write(itemLuckyShopFreePickRemainingTime);
		writer.Write(itemLuckyShopFreePickCount);
		writer.Write(itemLuckyShopPick1TimeCount);
		writer.Write(itemLuckyShopPick5TimeCount);
		writer.Write(creatureCardLuckyShopFreePickRemainingTime);
		writer.Write(creatureCardLuckyShopFreePickCount);
		writer.Write(creatureCardLuckyShopPick1TimeCount);
		writer.Write(creatureCardLuckyShopPick5TimeCount);
		writer.Write(creatures);
		writer.Write(participatedCreatureId);
		writer.Write(dailyCreatureVariationCount);
		writer.Write(weeklyPresentPopularityPoint);
		writer.Write(weeklyPresentContributionPoint);
		writer.Write(nationWeeklyPresentPopularityPointRankingNo);
		writer.Write(nationWeeklyPresentPopularityPointRanking);
		writer.Write(rewardedNationWeeklyPresentPopularityPointRankingNo);
		writer.Write(nationWeeklyPresentContributionPointRankingNo);
		writer.Write(nationWeeklyPresentContributionPointRanking);
		writer.Write(rewardedNationWeeklyPresentContributionPointRankingNo);
		writer.Write(costumes);
		writer.Write(equippedCostumeId);
		writer.Write(costumeCollectionId);
		writer.Write(costumeCollectionActivated);
		writer.Write(dailyCreatureFarmQuestAcceptionCount);
		writer.Write(creatureFarmQuest);
		writer.Write(cashProductPurchaseCounts);
		writer.Write(firstChargeEventObjectiveCompleted);
		writer.Write(firstChargeEventRewarded);
		writer.Write(rechargeEventAccUnOwnDia);
		writer.Write(rechargeEventRewarded);
		writer.Write(chargeEvent);
		writer.Write(dailyChargeEventAccUnOwnDia);
		writer.Write(rewardedDailyChargeEventMissions);
		writer.Write(consumeEvent);
		writer.Write(dailyConsumeEventAccDia);
		writer.Write(rewardedDailyConsumeEventMissions);
		writer.Write(jobChangeQuest);
		writer.Write(potionAttrs);
		writer.Write(dailyAnkouTombPlayCount);
		writer.Write(myAnkouTombBestRecords);
		writer.Write(serverAnkouTombBestRecords);
		writer.Write(constellations);
		writer.Write(starEssense);
		writer.Write(dailyStarEssenseItemUseCount);
		writer.Write(artifactNo);
		writer.Write(artifactLevel);
		writer.Write(artifactExp);
		writer.Write(equippedArtifactNo);
		writer.Write(dailyTradeShipPlayCount);
		writer.Write(myTradeShipBestRecords);
		writer.Write(serverTradeShipBestRecords);
	}

	public override void Deserialize(PacketReader reader)
	{
		base.Deserialize(reader);
		time = reader.ReadDateTimeOffset();
		date = reader.ReadDateTime();
		serverOpenDate = reader.ReadDateTime();
		serverMaxLevel = reader.ReadInt32();
		nationId = reader.ReadInt32();
		secretLetterQuestTargetNationId = reader.ReadInt32();
		nationInsts = reader.ReadPDPacketDatas<PDNationInstance>();
		nationPowerRankings = reader.ReadPDPacketDatas<PDNationPowerRanking>();
		nationAlliances = reader.ReadPDPacketDatas<PDNationAlliance>();
		nationAllianceApplications = reader.ReadPDPacketDatas<PDNationAllianceApplication>();
		id = reader.ReadGuid();
		name = reader.ReadString();
		jobId = reader.ReadInt32();
		level = reader.ReadInt32();
		exp = reader.ReadInt64();
		maxHP = reader.ReadInt32();
		hp = reader.ReadInt32();
		ownDia = reader.ReadInt32();
		unOwnDia = reader.ReadInt32();
		gold = reader.ReadInt64();
		honorPoint = reader.ReadInt32();
		vipPoint = reader.ReadInt32();
		receivedVipLevelRewards = reader.ReadInts();
		lak = reader.ReadInt32();
		stamina = reader.ReadInt32();
		staminaAutoRecoveryRemainingTime = reader.ReadSingle();
		dailyStaminaBuyCount = reader.ReadInt32();
		rankNo = reader.ReadInt32();
		rankRewardReceivedDate = reader.ReadDateTime();
		rankRewardReceivedRankNo = reader.ReadInt32();
		mainGears = reader.ReadPDPacketDatas<PDFullHeroMainGear>();
		equippedWeaponId = reader.ReadGuid();
		equippedArmorId = reader.ReadGuid();
		subGears = reader.ReadPDPacketDatas<PDFullHeroSubGear>();
		skills = reader.ReadPDPacketDatas<PDHeroSkill>();
		mounts = reader.ReadPDPacketDatas<PDHeroMount>();
		equippedMountId = reader.ReadInt32();
		bIsRiding = reader.ReadBoolean();
		mountGears = reader.ReadPDPacketDatas<PDHeroMountGear>();
		equippedMountGears = reader.ReadGuids();
		mountGearRefinementDailyCount = reader.ReadInt32();
		paidInventorySlotCount = reader.ReadInt32();
		placedInventorySlots = reader.ReadPDPacketDatas<PDInventorySlot>();
		mails = reader.ReadPDPacketDatas<PDMail>();
		initEntranceLocationId = reader.ReadInt32();
		initEntranceLocationParam = reader.ReadInt32();
		previousContinentId = reader.ReadInt32();
		previousNationId = reader.ReadInt32();
		mainGearEnchantDailyCount = reader.ReadInt32();
		mainGearRefinementDailyCount = reader.ReadInt32();
		currentMainQuest = reader.ReadPDPacketData<PDHeroMainQuest>();
		treatOfFarmQuest = reader.ReadPDPacketData<PDHeroTreatOfFarmQuest>();
		bountyHunterQuest = reader.ReadPDPacketData<PDHeroBountyHunterQuest>();
		bountyHunterDailyStartCount = reader.ReadInt32();
		fishingQuest = reader.ReadPDPacketData<PDHeroFishingQuest>();
		fishingQuestDailyStartCount = reader.ReadInt32();
		freeImmediateRevivalDailyCount = reader.ReadInt32();
		paidImmediateRevivalDailyCount = reader.ReadInt32();
		restTime = reader.ReadInt32();
		party = reader.ReadPDPacketData<PDParty>();
		expPotionDailyUseCount = reader.ReadInt32();
		receivedLevelUpRewards = reader.ReadInts();
		receivedDailyAcessRewards = reader.ReadInts();
		dailyAccessTime = reader.ReadSingle();
		receivedAttendRewardDate = reader.ReadDateTime();
		receivedAttendRewardDay = reader.ReadInt32();
		seriesMissions = reader.ReadPDPacketDatas<PDHeroSeriesMission>();
		todayMissions = reader.ReadPDPacketDatas<PDHeroTodayMission>();
		rewardedAttainmentEntryNo = reader.ReadInt32();
		wings = reader.ReadPDPacketDatas<PDHeroWing>();
		equippedWingId = reader.ReadInt32();
		wingStep = reader.ReadInt32();
		wingLevel = reader.ReadInt32();
		wingExp = reader.ReadInt32();
		wingParts = reader.ReadPDPacketDatas<PDHeroWingPart>();
		freeSweepDailyCount = reader.ReadInt32();
		storyDungeonPlays = reader.ReadPDPacketDatas<PDStoryDungeonPlay>();
		storyDungeonClears = reader.ReadPDPacketDatas<PDStoryDungeonClear>();
		expDungeonDailyPlayCount = reader.ReadInt32();
		expDungeonClearedDifficulties = reader.ReadInts();
		goldDungeonDailyPlayCount = reader.ReadInt32();
		goldDungeonClearedDifficulties = reader.ReadInts();
		undergroundMazeDailyPlayTime = reader.ReadSingle();
		artifactRoomBestFloor = reader.ReadInt32();
		artifactRoomCurrentFloor = reader.ReadInt32();
		artifactRoomDailyInitCount = reader.ReadInt32();
		artifactRoomSweepProgressFloor = reader.ReadInt32();
		artifactRoomSweepRemainingTime = reader.ReadSingle();
		ancientRelicDailyPlayCount = reader.ReadInt32();
		fieldOfHonorDailyPlayCount = reader.ReadInt32();
		rewardedDailyFieldOfHonorRankingNo = reader.ReadInt32();
		dailyFieldOfHonorRankingNo = reader.ReadInt32();
		dailyfieldOfHonorRanking = reader.ReadInt32();
		mainGearEnchantLevelSetNo = reader.ReadInt32();
		subGearSoulstoneLevelSetNo = reader.ReadInt32();
		expScrollDailyUseCount = reader.ReadInt32();
		expScrollItemId = reader.ReadInt32();
		expScrollRemainingTime = reader.ReadSingle();
		exploitPoint = reader.ReadInt32();
		dailyExploitPoint = reader.ReadInt32();
		dailyMysteryBoxQuestStartCount = reader.ReadInt32();
		mysteryBoxQuest = reader.ReadPDPacketData<PDHeroMysteryBoxQuest>();
		dailySecretLetterQuestStartCount = reader.ReadInt32();
		secretLetterQuest = reader.ReadPDPacketData<PDHeroSecretLetterQuest>();
		dailyDimensionRaidQuestStartCount = reader.ReadInt32();
		dimensionRaidQuest = reader.ReadPDPacketData<PDHeroDimensionRaidQuest>();
		dailyHolyWarQuestStartSchedules = reader.ReadInts();
		holyWarQuest = reader.ReadPDPacketData<PDHeroHolyWarQuest>();
		dailySupplySupportQuestStartCount = reader.ReadInt32();
		supplySupportQuest = reader.ReadPDPacketData<PDHeroSupplySupportQuest>();
		todayTasks = reader.ReadPDPacketDatas<PDHeroTodayTask>();
		achievementDailyPoint = reader.ReadInt32();
		receivedAchievementRewardNo = reader.ReadInt32();
		ridingCartInst = reader.ReadPDCartInstance();
		distortionScrollDailyUseCount = reader.ReadInt32();
		remainingDistortionTime = reader.ReadSingle();
		dailyServerLevelRakingNo = reader.ReadInt32();
		dailyServerLevelRanking = reader.ReadInt32();
		rewardedDailyServerLevelRankingNo = reader.ReadInt32();
		guild = reader.ReadPDPacketData<PDGuild>();
		guildMemberGrade = reader.ReadInt32();
		totalGuildContributionPoint = reader.ReadInt32();
		guildContributionPoint = reader.ReadInt32();
		guildPoint = reader.ReadInt32();
		guildRejoinRemainingTime = reader.ReadSingle();
		guildDailyRewardReceivedDate = reader.ReadDateTime();
		guildApplications = reader.ReadPDPacketDatas<PDHeroGuildApplication>();
		dailyGuildApplicationCount = reader.ReadInt32();
		dailyGuildDonationCount = reader.ReadInt32();
		dailyGuildFarmQuestStartCount = reader.ReadInt32();
		guildFarmQuest = reader.ReadPDPacketData<PDHeroGuildFarmQuest>();
		guildSkills = reader.ReadPDPacketDatas<PDHeroGuildSkill>();
		dailyGuildFoodWarehouseStockCount = reader.ReadInt32();
		receivedGuildFoodWarehouseCollectionId = reader.ReadGuid();
		guildMoralPoint = reader.ReadInt32();
		guildAltarDefenseMissionRemainingCoolTime = reader.ReadSingle();
		guildAltarRewardReceivedDate = reader.ReadDateTime();
		guildMissionQuest = reader.ReadPDPacketData<PDHeroGuildMissionQuest>();
		guildSupplySupportQuestPlay = reader.ReadPDPacketData<PDGuildSupplySupportQuestPlay>();
		guildHuntingQuest = reader.ReadPDPacketData<PDHeroGuildHuntingQuest>();
		dailyGuildHuntingQuestStartCount = reader.ReadInt32();
		guildHuntingDonationDate = reader.ReadDateTime();
		guildHuntingDonationRewardReceivedDate = reader.ReadDateTime();
		guildDailyObjectiveRewardReceivedNo = reader.ReadInt32();
		guildWeeklyObjectiveRewardReceivedDate = reader.ReadDateTime();
		guildDailyObjectiveNoticeRemainingCoolTime = reader.ReadSingle();
		nationWarDeclarations = reader.ReadPDPacketDatas<PDNationWarDeclaration>();
		weeklyNationWarDeclarationCount = reader.ReadInt32();
		nationWarJoined = reader.ReadBoolean();
		nationWarKillCount = reader.ReadInt32();
		nationWarAssistCount = reader.ReadInt32();
		nationWarDeadCount = reader.ReadInt32();
		nationWarImmediateRevivalCount = reader.ReadInt32();
		dailyNationWarFreeTransmissionCount = reader.ReadInt32();
		dailyNationWarPaidTransmissionCount = reader.ReadInt32();
		dailyNationWarCallCount = reader.ReadInt32();
		nationWarCallRemainingCoolTime = reader.ReadSingle();
		dailyNationWarConvergingAttackCount = reader.ReadInt32();
		nationWarConvergingAttackRemainingCoolTime = reader.ReadSingle();
		nationWarConvergingAttackTargetArrangeId = reader.ReadInt32();
		nationWarMonsterInsts = reader.ReadPDPacketDatas<PDSimpleNationWarMonsterInstance>();
		dailyNationDonationCount = reader.ReadInt32();
		weeklySoulCoveterPlayCount = reader.ReadInt32();
		illustratedBookExplorationStepNo = reader.ReadInt32();
		explorationPoint = reader.ReadInt32();
		illustratedBookExplorationStepRewardReceivedDate = reader.ReadDateTime();
		illustratedBookExplorationStepRewardReceivedStepNo = reader.ReadInt32();
		activationIllustratedBookIds = reader.ReadInts();
		completedSceneryQuests = reader.ReadInts();
		rewardedAccomplishments = reader.ReadInts();
		accMonsterKillCount = reader.ReadInt32();
		accSoulCoveterPlayCount = reader.ReadInt32();
		accEpicBaitItemUseCount = reader.ReadInt32();
		accLegendBaitItemUseCount = reader.ReadInt32();
		accNationWarWinCount = reader.ReadInt32();
		accNationWarKillCount = reader.ReadInt32();
		accNationWarCommanderKillCount = reader.ReadInt32();
		accNationWarImmediateRevivalCount = reader.ReadInt32();
		maxGold = reader.ReadInt64();
		maxBattlePower = reader.ReadInt64();
		maxAcquisitionMainGearGrade = reader.ReadInt32();
		maxEquippedMainGearEnchantLevel = reader.ReadInt32();
		titles = reader.ReadPDPacketDatas<PDHeroTitle>();
		displayTitleId = reader.ReadInt32();
		activationTitleId = reader.ReadInt32();
		soulPowder = reader.ReadInt32();
		creatureCards = reader.ReadPDPacketDatas<PDHeroCreatureCard>();
		activatedCreatureCardCollections = reader.ReadInts();
		creatureCardCollectionFamePoint = reader.ReadInt32();
		purchasedCreatureCardShopFixedProducts = reader.ReadInts();
		creatureCardShopRandomProducts = reader.ReadPDPacketDatas<PDHeroCreatureCardShopRandomProduct>();
		dailyCreatureCardShopPaidRefreshCount = reader.ReadInt32();
		heroEliteMonsterKills = reader.ReadPDPacketDatas<PDHeroEliteMonsterKill>();
		spawnedEliteMonsters = reader.ReadInts();
		dailyEliteDungeonPlayCount = reader.ReadInt32();
		lootingItemMinGrade = reader.ReadInt32();
		proofOfValorCleared = reader.ReadBoolean();
		dailyProofOfValorPlayCount = reader.ReadInt32();
		dailyProofOfValorFreeRefreshCount = reader.ReadInt32();
		dailyProofOfValorPaidRefreshCount = reader.ReadInt32();
		poofOfValorPaidRefreshCount = reader.ReadInt32();
		heroProofOfValorInstance = reader.ReadPDPacketData<PDHeroProofOfValorInstance>();
		customPresetHair = reader.ReadInt32();
		customFaceJawHeight = reader.ReadInt32();
		customFaceJawWidth = reader.ReadInt32();
		customFaceJawEndHeight = reader.ReadInt32();
		customFaceWidth = reader.ReadInt32();
		customFaceEyebrowHeight = reader.ReadInt32();
		customFaceEyebrowRotation = reader.ReadInt32();
		customFaceEyesWidth = reader.ReadInt32();
		customFaceNoseHeight = reader.ReadInt32();
		customFaceNoseWidth = reader.ReadInt32();
		customFaceMouthHeight = reader.ReadInt32();
		customFaceMouthWidth = reader.ReadInt32();
		customBodyHeadSize = reader.ReadInt32();
		customBodyArmsLength = reader.ReadInt32();
		customBodyArmsWidth = reader.ReadInt32();
		customBodyChestSize = reader.ReadInt32();
		customBodyWaistWidth = reader.ReadInt32();
		customBodyHipsSize = reader.ReadInt32();
		customBodyPelvisWidth = reader.ReadInt32();
		customBodyLegsLength = reader.ReadInt32();
		customBodyLegsWidth = reader.ReadInt32();
		customColorSkin = reader.ReadInt32();
		customColorEyes = reader.ReadInt32();
		customColorBeardAndEyebrow = reader.ReadInt32();
		customColorHair = reader.ReadInt32();
		todayMissionTutorialStarted = reader.ReadBoolean();
		heroNpcShopProducts = reader.ReadPDPacketDatas<PDHeroNpcShopProduct>();
		rankActiveSkills = reader.ReadPDPacketDatas<PDHeroRankActiveSkill>();
		rankPassiveSkills = reader.ReadPDPacketDatas<PDHeroRankPassiveSkill>();
		selectedRankActiveSkillId = reader.ReadInt32();
		rankActiveSkillRemainingCoolTime = reader.ReadSingle();
		spiritStone = reader.ReadInt32();
		rookieGiftNo = reader.ReadInt32();
		rookieGiftRemainingTime = reader.ReadSingle();
		receivedOpenGiftRewards = reader.ReadInts();
		regDate = reader.ReadDateTime();
		dailyQuestAcceptionCount = reader.ReadInt32();
		dailyQuestFreeRefreshCount = reader.ReadInt32();
		dailyQuests = reader.ReadPDPacketDatas<PDHeroDailyQuest>();
		weeklyQuest = reader.ReadPDPacketData<PDHeroWeeklyQuest>();
		wisdomTempleCleared = reader.ReadBoolean();
		dailyWisdomTemplePlayCount = reader.ReadInt32();
		rewardedOpen7DayEventMissions = reader.ReadInts();
		purchasedOpen7DayEventProducts = reader.ReadInts();
		open7DayEventProgressCounts = reader.ReadPDPacketDatas<PDHeroOpen7DayEventProgressCount>();
		open7DayEventRewarded = reader.ReadBoolean();
		retrievalProgressCounts = reader.ReadPDPacketDatas<PDHeroRetrievalProgressCount>();
		retrievals = reader.ReadPDPacketDatas<PDHeroRetrieval>();
		dailyRuinsReclaimFreePlayCount = reader.ReadInt32();
		taskConsignments = reader.ReadPDPacketDatas<PDHeroTaskConsignment>();
		taskConsignmentStartCounts = reader.ReadPDPacketDatas<PDHeroTaskConsignmentStartCount>();
		trueHeroQuest = reader.ReadPDPacketData<PDHeroTrueHeroQuest>();
		dailyInfiniteWarPlayCount = reader.ReadInt32();
		spawnedFieldBosses = reader.ReadInts();
		rewardedLimitationGiftScheduleIds = reader.ReadInts();
		weekendReward = reader.ReadPDPacketData<PDHeroWeekendReward>();
		paidWarehouseSlotCount = reader.ReadInt32();
		placedWarehouseSlots = reader.ReadPDPacketDatas<PDWarehouseSlot>();
		dailyDiaShopProductBuyCounts = reader.ReadPDPacketDatas<PDHeroDiaShopProductBuyCount>();
		totalDiaShopProductBuyCounts = reader.ReadPDPacketDatas<PDHeroDiaShopProductBuyCount>();
		dailyFearAltarPlayCount = reader.ReadInt32();
		weeklyFearAltarHalidomCollectionRewardNo = reader.ReadInt32();
		weeklyFearAltarHalidoms = reader.ReadInts();
		weeklyRewardReceivedFearAltarHalidomElementals = reader.ReadInts();
		subQuests = reader.ReadPDPacketDatas<PDHeroSubQuest>();
		dailyWarMemoryFreePlayCount = reader.ReadInt32();
		ordealQuest = reader.ReadPDPacketData<PDHeroOrdealQuest>();
		dailyOsirisRoomPlayCount = reader.ReadInt32();
		biographies = reader.ReadPDPacketDatas<PDHeroBiography>();
		friends = reader.ReadPDPacketDatas<PDFriend>();
		tempFriends = reader.ReadPDPacketDatas<PDTempFriend>();
		blacklistEntries = reader.ReadPDPacketDatas<PDBlacklistEntry>();
		deadRecords = reader.ReadPDPacketDatas<PDDeadRecord>();
		ownerProspectQuests = reader.ReadPDPacketDatas<PDHeroProspectQuest>();
		targetProspectQuests = reader.ReadPDPacketDatas<PDHeroProspectQuest>();
		itemLuckyShopFreePickRemainingTime = reader.ReadSingle();
		itemLuckyShopFreePickCount = reader.ReadInt32();
		itemLuckyShopPick1TimeCount = reader.ReadInt32();
		itemLuckyShopPick5TimeCount = reader.ReadInt32();
		creatureCardLuckyShopFreePickRemainingTime = reader.ReadSingle();
		creatureCardLuckyShopFreePickCount = reader.ReadInt32();
		creatureCardLuckyShopPick1TimeCount = reader.ReadInt32();
		creatureCardLuckyShopPick5TimeCount = reader.ReadInt32();
		creatures = reader.ReadPDPacketDatas<PDHeroCreature>();
		participatedCreatureId = reader.ReadGuid();
		dailyCreatureVariationCount = reader.ReadInt32();
		weeklyPresentPopularityPoint = reader.ReadInt32();
		weeklyPresentContributionPoint = reader.ReadInt32();
		nationWeeklyPresentPopularityPointRankingNo = reader.ReadInt32();
		nationWeeklyPresentPopularityPointRanking = reader.ReadInt32();
		rewardedNationWeeklyPresentPopularityPointRankingNo = reader.ReadInt32();
		nationWeeklyPresentContributionPointRankingNo = reader.ReadInt32();
		nationWeeklyPresentContributionPointRanking = reader.ReadInt32();
		rewardedNationWeeklyPresentContributionPointRankingNo = reader.ReadInt32();
		costumes = reader.ReadPDPacketDatas<PDHeroCostume>();
		equippedCostumeId = reader.ReadInt32();
		costumeCollectionId = reader.ReadInt32();
		costumeCollectionActivated = reader.ReadBoolean();
		dailyCreatureFarmQuestAcceptionCount = reader.ReadInt32();
		creatureFarmQuest = reader.ReadPDPacketData<PDHeroCreatureFarmQuest>();
		cashProductPurchaseCounts = reader.ReadPDPacketDatas<PDCashProductPurchaseCount>();
		firstChargeEventObjectiveCompleted = reader.ReadBoolean();
		firstChargeEventRewarded = reader.ReadBoolean();
		rechargeEventAccUnOwnDia = reader.ReadInt32();
		rechargeEventRewarded = reader.ReadBoolean();
		chargeEvent = reader.ReadPDPacketData<PDAccountChargeEvent>();
		dailyChargeEventAccUnOwnDia = reader.ReadInt32();
		rewardedDailyChargeEventMissions = reader.ReadInts();
		consumeEvent = reader.ReadPDPacketData<PDAccountConsumeEvent>();
		dailyConsumeEventAccDia = reader.ReadInt32();
		rewardedDailyConsumeEventMissions = reader.ReadInts();
		jobChangeQuest = reader.ReadPDPacketData<PDHeroJobChangeQuest>();
		potionAttrs = reader.ReadPDPacketDatas<PDHeroPotionAttr>();
		dailyAnkouTombPlayCount = reader.ReadInt32();
		myAnkouTombBestRecords = reader.ReadPDPacketDatas<PDHeroAnkouTombBestRecord>();
		serverAnkouTombBestRecords = reader.ReadPDPacketDatas<PDHeroAnkouTombBestRecord>();
		constellations = reader.ReadPDPacketDatas<PDHeroConstellation>();
		starEssense = reader.ReadInt32();
		dailyStarEssenseItemUseCount = reader.ReadInt32();
		artifactNo = reader.ReadInt32();
		artifactLevel = reader.ReadInt32();
		artifactExp = reader.ReadInt32();
		equippedArtifactNo = reader.ReadInt32();
		dailyTradeShipPlayCount = reader.ReadInt32();
		myTradeShipBestRecords = reader.ReadPDPacketDatas<PDHeroTradeShipBestRecord>();
		serverTradeShipBestRecords = reader.ReadPDPacketDatas<PDHeroTradeShipBestRecord>();
	}
}
