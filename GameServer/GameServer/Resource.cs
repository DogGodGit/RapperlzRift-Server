using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using ServerFramework;

namespace GameServer;

public class Resource
{
    #region Properties

    public const int kYRotationType_Fixed = 1;

	public const int kYRotationType_Random = 2;

	public const int kMainGearMultipleRefinementCount = 3;

	public const float kChattingIntervalFactor = 0.9f;

	public const int kStaminaRecoveryAmount = 1;

	public const int kNationTransmissionExitYRotationType_Fixed = 1;

	public const int kNationTransmissionExitYRotationType_Random = 2;

	public const int kHeroSearchMinNameLength = 2;

    #endregion

    #region Fields

    private int m_nSectorSize = 20;

	private Dictionary<int, Language> m_languages = new Dictionary<int, Language>();

	private Language m_defaultLanguage;

	private int m_nMaxUserConnectionCount;

	private Regex m_heroNameRegex;

	private Regex m_guildNameRegex;

	private int m_nMaxHeroCount;

	private int m_nStartContinentId;

	private Vector3 m_startPosition = Vector3.zero;

	private float m_fStartRadius;

	private int m_nStartYRotationType;

	private float m_fStartYRotation;

	private int m_nMailRetentionDay;

	private float m_fBattleModeDuration;

	private int m_nSpecialSkillId;

	private int m_nSpecialSkillMaxLak;

	private int m_nFreeImmediateRevivalDailyCount;

	private int m_nAutoSaftyRevivalWaitingTime;

	private Vector3 m_startContinentSaftyRevivalPosition = Vector3.zero;

	private float m_fStartContinentSaftyRevivalRadius;

	private int m_nStartContinentSaftyRevivalYRotationType;

	private float m_fStartContinentSaftyRevivalYRotation;

	private int m_nSaftyRevivalContinentId;

	private Vector3 m_saftyRevivalPosition = Vector3.zero;

	private float m_fSaftyRevivalRadius;

	private int m_nSaftyRevivalYRotationType;

	private float m_fSaftyRevivalYRotation;

	private int m_nSimpleShopSellSlotCount;

	private long m_lnWeekendAttendItemRewardId;

	private int m_nMainGearOptionAttrMinCount;

	private int m_nMainGearOptionAttrMaxCount;

	private int m_nMainGearRefimentItemId;

	private int m_nMainGearDisassembleSlotCount;

	private int m_nPartyMemberMaxCount;

	private int m_nPartyMemberLogOutDuration;

	private int m_nPartyInvitationLifetime;

	private int m_nPartyApplicationLifetime;

	private int m_nPartyCallCoolTime;

	private int m_nRestRewardRequiredHeroLevel;

	private int m_nRestRewardGoldReceiveExpRate;

	private int m_nRestRewardDiaReceiveExpRate;

	private int m_nChattingMaxLength;

	private int m_nChattingMinInterval;

	private int m_nWorldChattingItemId;

	private Item m_mountLevelUpItem;

	private int m_nMountQualityUpRequiredLevelUpCount;

	private float m_fEquippedMountAttrFactor;

	private int m_nMountGearOptionAttrCount;

	private int m_nMountGearRefinementItemId;

	private int m_nMountAwakeningRequiredHeroLevel;

	private Item m_mountAwakeningItem;

	private int m_nMountPotionAttrItemId;

	private int m_nDungeonFreeSweepDailyCount;

	private int m_nDungeonSweepItemId;

	private int m_nWingEnchantItemId;

	private int m_nWingEnchantExp;

	private int m_nWingVisibleMinVipLevel;

	private int m_nMaxStamina;

	private int m_nStaminaRecoveryTime;

	private int m_nCartNormalSpeed;

	private int m_nCartHighSpeed;

	private int m_nCartHighSpeedDuration;

	private int m_nCartHighSpeedDurationExtension;

	private int m_nCartAccelCoolTime;

	private int m_nCartAccelSuccessRate;

	private int m_nPvpMinHeroLevel;

	private List<PvpExploit> m_pvpExploits = new List<PvpExploit>();

	private int m_nWorldLevelExpBuffMinHeroLevel;

	private int m_nNationTransmissionRequiredHeroLevel;

	private Vector3 m_nationTransmissionExitPosition = Vector3.zero;

	private float m_fNationTransmissionExitRadius;

	private int m_nNationTransmissionYRotationType;

	private float m_fNationTransmissionYRotation;

	private int m_nBountyHunterQuestMaxCount;

	private int m_nTodayMissionCount;

	private MysteryBoxQuest m_mysteryBoxQuest;

	private SecretLetterQuest m_secretLetterQuest;

	private DimensionRaidQuest m_dimensionRaidQuest;

	private HolyWarQuest m_holyWarQuest;

	private DimensionInfiltrationEvent m_dimensionInfiltrationEvent;

	private BattlefieldSupportEvent m_battlefieldSupportEvent;

	private SafeTimeEvent m_safeTimeEvent;

	private int m_nRankingDisplayMaxCount;

	private int m_nGuildRankingDisplayMaxCount;

	private int m_nPresentPopularityPointRankingDisplayMaxCount;

	private int m_nPresentContributionPointRankingDisplayMaxCount;

	private int m_nGuildRequiredHeroLevel;

	private int m_nGuildCreationRequiredVipLevel;

	private int m_nGuildCreationRequiredDia;

	private int m_nGuildRejoinIntervalTime;

	private int m_nGuildApplicationReceptionMaxCount;

	private int m_nGuildDailyApplicationMaxCount;

	private int m_nGuildDailyBanishmentMaxCount;

	private int m_nGuildInvitationLifetime;

	private int m_nGuildNoticeMaxLength;

	private int m_nGuildViceMasterCount;

	private int m_nGuildLordCount;

	private int m_nGuildCallLifetime;

	private float m_fGuildCallRadius;

	private int m_nGuildDailyObjectiveNoticeCoolTime;

	private int m_nDefaultGuildWeeklyObjectiveId;

	private int m_nGuildHuntingDonationMaxCount;

	private Item m_guildHuntingDonationItem;

	private ItemReward m_guildHuntingDonationReward;

	private ItemReward m_guildHuntingDonationCompletionReward;

	private int m_nNationCallLifeTime;

	private float m_fNationCallRadius;

	private int m_nRankOpenRequiredMainQuestNo;

	private int m_nWingOpenRequiredHeroLevel;

	private int m_nWingOpenProvideWingId;

	private int m_nNationWarMonsterBattleModeDuration;

	private int m_nCreatureCardShopRandomProductCount;

	private int m_nCreatureCardShopPaidRefreshDia;

	private int m_nAccelerationRequiredMoveDuration;

	private int m_nAccelerationMoveSpeed;

	private int m_nSceneryQuestRequiredMainQuestNo;

	private int m_nMonsterGroggyDuration;

	private int m_nMonsterStealDuration;

	private int m_nOpenGiftRequiredHeroLevel;

	private int m_nOpen7DayEventRequiredMainQuestNo;

	private int m_nBountyHunterQuestRequiredHeroLevel;

	private int m_nTaskConsignmentRequiredVipLevel;

	private int m_nEliteMonsterKillApplicationRequiredHeroLevel;

	private int m_nWarehouseRequiredVipLevel;

	private int m_nFreeWarehouseSlotCount;

	private int m_nWingMemoryPieceInstallationRequiredHeroLevel;

	private int m_nOrdealQuestSlotCount;

	private int m_nCreatureMaxCount;

	private int m_nCreatureCheerMaxCount;

	private float m_fCreatureCheerAttrFactor;

	private float m_fCreatureEvaluationFactor;

	private int m_nCreatureAdditionalAttrCount;

	private int m_nCreatureSkillSlotMaxCount;

	private int m_nCreatureSkillSlotBaseOpenCount;

	private int m_nCreatureCompositionSkillProtectionItemId;

	private int m_nCreatureInjectionExpRetrievalRate;

	private Item m_creatureInjectionExpItem;

	private int m_nCreatureVariationRequiredItemId;

	private int m_nCreatureAdditionalAttrSwitchRequiredItemId;

	private int m_nCreatureReleaseExpRetrievalRate;

	private GuildTerritoryNpc m_guildBlessingGuildTerritoryNpc;

	private ItemReward m_open7DayEventCostumeItemReward;

	private int m_nOpen7DayEventCostumeRewardRequiredItemId;

	private int m_nOpen7DayEventCostumeRewardRequiredItemCount;

	private int m_nNationAllianceUnavailableStartTime;

	private int m_nNationAllianceUnavailableEndTime;

	private int m_nNationAllianceRequiredFund;

	private int m_nNationAllianceRenounceUnavailableDuration;

	private int m_nNationBasePower;

	private int m_nJobChangeRequiredHeroLevel;

	private int m_nJobChangeRequiredItemId;

	private Item m_costumeEnchantItem;

	private int m_nCostumeCollectionActivationItemId;

	private int m_nCostumeCollectionShuffleItemId;

	private int m_nCostumeCollectionShuffleItemCount;

	private HashSet<string> m_nameBanWords = new HashSet<string>();

	private HashSet<string> m_chatBanWords = new HashSet<string>();

	private SFTextFilter m_nameBanWordTextFilter = new SFTextFilter();

	private SFTextFilter m_chatBanWordTextFilter = new SFTextFilter();

	private Dictionary<int, Nation> m_nations = new Dictionary<int, Nation>();

	private Dictionary<int, NationNoblesse> m_nationNoblesses = new Dictionary<int, NationNoblesse>();

	private Dictionary<int, Attr> m_attrs = new Dictionary<int, Attr>();

	private Dictionary<long, AttrValue> m_attrValues = new Dictionary<long, AttrValue>();

	private Dictionary<int, AbnormalState> m_abnormalStates = new Dictionary<int, AbnormalState>();

	private Dictionary<int, Job> m_jobs = new Dictionary<int, Job>();

	private Dictionary<int, JobSkillMaster> m_jobSkillMasters = new Dictionary<int, JobSkillMaster>();

	private List<JobLevelMaster> m_jobLevelMasters = new List<JobLevelMaster>();

	private List<Rank> m_ranks = new List<Rank>();

	private Dictionary<int, RankActiveSkill> m_rankActiveSkills = new Dictionary<int, RankActiveSkill>();

	private Dictionary<int, RankPassiveSkill> m_rankPassiveSkills = new Dictionary<int, RankPassiveSkill>();

	private Dictionary<int, Location> m_locations = new Dictionary<int, Location>();

	private Dictionary<int, Continent> m_continents = new Dictionary<int, Continent>();

	private Dictionary<int, ContinentObject> m_continentObjects = new Dictionary<int, ContinentObject>();

	private ItemGrade[] m_itemGrades = new ItemGrade[5];

	private Dictionary<int, ItemType> m_itemTypes = new Dictionary<int, ItemType>();

	private Dictionary<int, Item> m_items = new Dictionary<int, Item>();

	private Item[] m_creatureFeeds;

	private Dictionary<int, ItemCompositionRecipe> m_itemCompositionRecipes = new Dictionary<int, ItemCompositionRecipe>();

	private Dictionary<long, ExpReward> m_expRewards = new Dictionary<long, ExpReward>();

	private Dictionary<long, GoldReward> m_goldRewards = new Dictionary<long, GoldReward>();

	private Dictionary<long, ItemReward> m_itemRewards = new Dictionary<long, ItemReward>();

	private Dictionary<long, ExploitPointReward> m_exploitPointRewards = new Dictionary<long, ExploitPointReward>();

	private Dictionary<long, HonorPointReward> m_honorPointRewards = new Dictionary<long, HonorPointReward>();

	private Dictionary<long, NationFundReward> m_nationFundRewards = new Dictionary<long, NationFundReward>();

	private Dictionary<long, OwnDiaReward> m_ownDiaRewards = new Dictionary<long, OwnDiaReward>();

	private Dictionary<long, GuildContributionPointReward> m_guildContributionPointRewards = new Dictionary<long, GuildContributionPointReward>();

	private Dictionary<long, GuildFundReward> m_guildFundRewards = new Dictionary<long, GuildFundReward>();

	private Dictionary<long, GuildBuildingPointReward> m_guildBuildingPointRewards = new Dictionary<long, GuildBuildingPointReward>();

	private Dictionary<long, GuildPointReward> m_guildPointRewards = new Dictionary<long, GuildPointReward>();

	private MainGearCategory[] m_mainGearCategory = new MainGearCategory[2];

	private MainGearType[] m_mainGearTypes = new MainGearType[5];

	private MainGearGrade[] m_mainGearGrade = new MainGearGrade[5];

	private MainGearQuality[] m_mainGearQuality = new MainGearQuality[6];

	private List<MainGearTier> m_mainGearTiers = new List<MainGearTier>();

	private Dictionary<int, MainGear> m_mainGears = new Dictionary<int, MainGear>();

	private MainGearSet[,,] m_mainGearSets;

	private List<MainGearEnchantLevel> m_mainGearEnchantLevels = new List<MainGearEnchantLevel>();

	private List<MainGearEnchantStep> m_mainGearEnchantStep = new List<MainGearEnchantStep>();

	private List<MainGearEnchantLevelSet> m_mainGearEnchantLevelSets = new List<MainGearEnchantLevelSet>();

	private Dictionary<int, MainGearRefinementRecipe> m_mainGearRefinementRecipes = new Dictionary<int, MainGearRefinementRecipe>();

	private Dictionary<int, SubGear> m_subGears = new Dictionary<int, SubGear>();

	private SubGearGrade[] m_subGearGrade = new SubGearGrade[7];

	private List<SubGearSoulstoneLevelSet> m_subGearSoulstoneLevelSets = new List<SubGearSoulstoneLevelSet>();

	private Dictionary<int, MonsterCharacter> m_monsterCharacters = new Dictionary<int, MonsterCharacter>();

	private Dictionary<int, Monster> m_monsters = new Dictionary<int, Monster>();

	private Dictionary<long, MonsterArrange> m_monsterArrange = new Dictionary<long, MonsterArrange>();

	private Dictionary<int, MonsterSkill> m_monsterSkills = new Dictionary<int, MonsterSkill>();

	private Dictionary<int, Portal> m_portals = new Dictionary<int, Portal>();

	private Dictionary<int, Npc> m_npcs = new Dictionary<int, Npc>();

	private Dictionary<int, NpcShop> m_npcShops = new Dictionary<int, NpcShop>();

	private Dictionary<int, NpcShopProduct> m_npcShopProducts = new Dictionary<int, NpcShopProduct>();

	private List<MainQuest> m_mainQuests = new List<MainQuest>();

	private TreatOfFarmQuest m_treatOfFarmQuest;

	private Dictionary<int, BountyHunterQuest> m_bountyHunterQuests = new Dictionary<int, BountyHunterQuest>();

	private BountyHunterQuestRewardCollection[] m_bountyHunterQuestRewardCollection = new BountyHunterQuestRewardCollection[5];

	private FishingQuest m_fishingQuest;

	private Dictionary<int, SimpleShopProduct> m_simpleShopProducts = new Dictionary<int, SimpleShopProduct>();

	private List<PaidImmediateRevival> m_paidImmediateRevivals = new List<PaidImmediateRevival>();

	private List<InventorySlotExtendRecipe> m_inventorySlotExtendRecipes = new List<InventorySlotExtendRecipe>();

	private List<RestRewardTime> m_restRewardTimes = new List<RestRewardTime>();

	private Dictionary<int, PickPool> m_pickPools = new Dictionary<int, PickPool>();

	private Dictionary<int, DropCountPool> m_dropCountPools = new Dictionary<int, DropCountPool>();

	private Dictionary<int, DropObjectPool> m_dropObjectPools = new Dictionary<int, DropObjectPool>();

	private Dictionary<int, LevelUpRewardEntry> m_levelUpRewardEntries = new Dictionary<int, LevelUpRewardEntry>();

	private Dictionary<int, AccessRewardEntry> m_accessRewardEntries = new Dictionary<int, AccessRewardEntry>();

	private List<DailyAttendRewardEntry> m_dailyAttendRewardEntries = new List<DailyAttendRewardEntry>();

	private HashSet<DayOfWeek> m_weekendAttendRewardAvailableDaysOfWeek = new HashSet<DayOfWeek>();

	private Dictionary<int, SeriesMission> m_seriesMissions = new Dictionary<int, SeriesMission>();

	private Dictionary<int, TodayMission> m_todayMissions = new Dictionary<int, TodayMission>();

	private TodayMissionPool[] m_todayMissionPools;

	private List<AttainmentEntry> m_attainmentEntries = new List<AttainmentEntry>();

	private List<VipLevel> m_vipLevels = new List<VipLevel>();

	private List<MainQuestDungeon> m_mainQuestDungeons = new List<MainQuestDungeon>();

	private Dictionary<int, Mount> m_mounts = new Dictionary<int, Mount>();

	private List<MountLevelMaster> m_mountLevelMasters = new List<MountLevelMaster>();

	private List<MountQualityMaster> m_mountQualityMasters = new List<MountQualityMaster>();

	private List<MountAwakeningLevelMaster> m_mountAwakeningLevelMaster = new List<MountAwakeningLevelMaster>();

	private List<MountPotionAttrCount> m_mountPotionAttrCount = new List<MountPotionAttrCount>();

	private MountGearSlot[] m_mountGearSlots = new MountGearSlot[6];

	private MountGearType[] m_mountGearTypes = new MountGearType[6];

	private MountGearGrade[] m_mountGearGrades = new MountGearGrade[5];

	private MountGearQuality[] m_mountGearQualities = new MountGearQuality[6];

	private Dictionary<int, MountGear> m_mountGears = new Dictionary<int, MountGear>();

	private Dictionary<int, MountGearPickBoxRecipe> m_mountGearPickBoxRecipes = new Dictionary<int, MountGearPickBoxRecipe>();

	private List<Elemental> m_elementals = new List<Elemental>();

	private List<StoryDungeon> m_storyDungeons = new List<StoryDungeon>();

	private Dictionary<int, Wing> m_wings = new Dictionary<int, Wing>();

	private Dictionary<int, WingPart> m_wingParts = new Dictionary<int, WingPart>();

	private List<WingStep> m_wingSteps = new List<WingStep>();

	private WingEnchantCountPool m_wingEnchantCountPool = new WingEnchantCountPool();

	private Dictionary<int, WingMemoryPieceType> m_wingMemoryPieceTypes = new Dictionary<int, WingMemoryPieceType>();

	private ExpDungeon m_expDungeon;

	private GoldDungeon m_goldDungeon;

	private UndergroundMaze m_undergroundMaze;

	private List<UndergroundMazePortal> m_undergroundMazePortals = new List<UndergroundMazePortal>();

	private List<UndergroundMazeNpc> m_undergroundMazeNpcs = new List<UndergroundMazeNpc>();

	private CartGrade[] m_cartGrades = new CartGrade[5];

	private Dictionary<int, Cart> m_carts = new Dictionary<int, Cart>();

	private ArtifactRoom m_artifactRoom;

	private Dictionary<int, TodayTask> m_todayTasks = new Dictionary<int, TodayTask>();

	private List<AchievementReward> m_achievementRewards = new List<AchievementReward>();

	private AncientRelic m_ancientRelic;

	private Dictionary<int, HonorShopProduct> m_honorShopProducts = new Dictionary<int, HonorShopProduct>();

	private List<LevelRankingReward> m_levelRankingRewards = new List<LevelRankingReward>();

	private FieldOfHonor m_fieldOfHonor;

	private List<GuildLevel> m_guildLevels = new List<GuildLevel>();

	private GuildMemberGrade[] m_guildMemberGrade = new GuildMemberGrade[4];

	private Dictionary<int, GuildDonationEntry> m_guildDonationEntries = new Dictionary<int, GuildDonationEntry>();

	private GuildTerritory m_guildTerritory;

	private Dictionary<int, GuildTerritoryNpc> m_guildTerritoryNpcs = new Dictionary<int, GuildTerritoryNpc>();

	private GuildFarmQuest m_guildFarmQuest;

	private GuildFoodWarehouse m_guildFoodWarehouse;

	private GuildAltar m_guildAltar;

	private Dictionary<int, GuildBuilding> m_guildBuildings = new Dictionary<int, GuildBuilding>();

	private GuildBuilding m_guildLobby;

	private GuildBuilding m_guildLaboratory;

	private GuildBuilding m_guildShop;

	private GuildBuilding m_guildTankFactory;

	private Dictionary<int, GuildSkill> m_guildSkills = new Dictionary<int, GuildSkill>();

	private GuildMissionQuest m_guildMissionQuest;

	private GuildSupplySupportQuest m_guildSupplySupportQuest;

	private GuildHuntingQuest m_guildHuntingQuest;

	private Dictionary<int, GuildContent> m_guildContents = new Dictionary<int, GuildContent>();

	private Dictionary<int, GuildContent> m_guildDailyObjectiveContents = new Dictionary<int, GuildContent>();

	private int m_nGuildDailyObjectiveContentTotalPoint;

	private List<GuildDailyObjectiveReward> m_guildDailyObjectiveRewards = new List<GuildDailyObjectiveReward>();

	private Dictionary<int, GuildWeeklyObjective> m_guildWeeklyObjectives = new Dictionary<int, GuildWeeklyObjective>();

	private Dictionary<int, GuildBlessingBuff> m_guildBlessingBuffs = new Dictionary<int, GuildBlessingBuff>();

	private SupplySupportQuest m_supplySupportQuest;

	private Dictionary<int, NationDonationEntry> m_nationDonationEntries = new Dictionary<int, NationDonationEntry>();

	private NationWar m_nationWar;

	private SoulCoveter m_soulCoveter;

	private Dictionary<int, IllustratedBookCategory> m_illustratedBookCategories = new Dictionary<int, IllustratedBookCategory>();

	private Dictionary<int, IllustratedBookType> m_illustratedBookTypes = new Dictionary<int, IllustratedBookType>();

	private Dictionary<int, IllustratedBook> m_illustratedBooks = new Dictionary<int, IllustratedBook>();

	private List<IllustratedBookExplorationStep> m_illustratedBookExplorationSteps = new List<IllustratedBookExplorationStep>();

	private Dictionary<int, SceneryQuest> m_sceneryQuests = new Dictionary<int, SceneryQuest>();

	private Dictionary<int, Accomplishment> m_accomplishments = new Dictionary<int, Accomplishment>();

	private Dictionary<int, TitleType> m_titleTypes = new Dictionary<int, TitleType>();

	private Dictionary<int, Title> m_title = new Dictionary<int, Title>();

	private CreatureCardGrade[] m_creatureCardGrades = new CreatureCardGrade[5];

	private Dictionary<int, CreatureCard> m_creatureCards = new Dictionary<int, CreatureCard>();

	private CreatureCardCollectionGrade[] m_creatureCardCollectionGrades = new CreatureCardCollectionGrade[5];

	private Dictionary<int, CreatureCardCollectionCategory> m_creatureCardCollectionCategories = new Dictionary<int, CreatureCardCollectionCategory>();

	private Dictionary<int, CreatureCardCollection> m_creaturceCardCollections = new Dictionary<int, CreatureCardCollection>();

	private List<CreatureCardShopRefreshSchedule> m_creatureCardShopRefreshSchedules = new List<CreatureCardShopRefreshSchedule>();

	private Dictionary<int, CreatureCardShopFixedProduct> m_creatureCardShopFixedProducts = new Dictionary<int, CreatureCardShopFixedProduct>();

	private Dictionary<int, CreatureCardShopRandomProduct> m_creatureCardShopRandomProducts = new Dictionary<int, CreatureCardShopRandomProduct>();

	private int m_nCreatureCardShopRandomProductsTotalPoint;

	private Dictionary<int, EliteMonsterCategory> m_eliteMonsterCategories = new Dictionary<int, EliteMonsterCategory>();

	private Dictionary<int, EliteMonsterMaster> m_eliteMonsterMasters = new Dictionary<int, EliteMonsterMaster>();

	private Dictionary<int, EliteMonster> m_eliteMonsters = new Dictionary<int, EliteMonster>();

	private EliteDungeon m_eliteDungeon;

	private ProofOfValor m_proofOfValor;

	private List<StaminaBuyCount> m_staminaBuyCounts = new List<StaminaBuyCount>();

	private List<StaminaRecoverySchedule> m_staminaRecoverySchedules = new List<StaminaRecoverySchedule>();

	private int m_nTotalStaminaOfStaminaRecoverySchedules;

	private Dictionary<int, MonsterKillExpFactor> m_monsterKillExpFactors = new Dictionary<int, MonsterKillExpFactor>();

	private Dictionary<int, WorldLevelExpFactor> m_worldLevelExpFactors = new Dictionary<int, WorldLevelExpFactor>();

	private List<PartyExpFactor> m_partyExpFactors = new List<PartyExpFactor>();

	private List<JobCommonSkill> m_jobCommonSkills = new List<JobCommonSkill>();

	private List<RookieGift> m_rookieGifts = new List<RookieGift>();

	private Dictionary<int, OpenGift> m_openGifts = new Dictionary<int, OpenGift>();

	private DailyQuest m_dailyQuest;

	private WeeklyQuest m_weeklyQuest;

	private WisdomTemple m_wisdomTemple;

	private Dictionary<int, Open7DayEventDay> m_open7DayEventDays = new Dictionary<int, Open7DayEventDay>();

	private Dictionary<int, Open7DayEventMission> m_open7DayEventMissions = new Dictionary<int, Open7DayEventMission>();

	private Dictionary<int, Open7DayEventProduct> m_open7DayEventProducts = new Dictionary<int, Open7DayEventProduct>();

	private Dictionary<int, Retrieval> m_retrievals = new Dictionary<int, Retrieval>();

	private RuinsReclaim m_ruinsReclaim;

	private Dictionary<int, TaskConsignment> m_taskConsignments = new Dictionary<int, TaskConsignment>();

	private TrueHeroQuest m_trueHeroQuest;

	private InfiniteWar m_infiniteWar;

	private FieldBossEvent m_fieldBossEvent;

	private LimitationGift m_limitationGift;

	private WeekendReward m_weekendReward;

	private List<WarehouseSlotExtendRecipe> m_warehouseSlotExtendRecipes = new List<WarehouseSlotExtendRecipe>();

	private Dictionary<int, DiaShopCategory> m_diaShopCatregories = new Dictionary<int, DiaShopCategory>();

	private Dictionary<int, DiaShopProduct> m_diaShopProducts = new Dictionary<int, DiaShopProduct>();

	private FearAltar m_fearAltar;

	private Dictionary<int, SubQuest> m_subQuests = new Dictionary<int, SubQuest>();

	private WarMemory m_warMemory;

	private List<OrdealQuest> m_ordealQuests = new List<OrdealQuest>();

	private OsirisRoom m_osirisRoom;

	private Dictionary<int, MoneyBuff> m_moneyBuffs = new Dictionary<int, MoneyBuff>();

	private int m_nFriendMaxCount;

	private int m_nTempFriendMaxCount;

	private int m_nDeadRecordMaxCount;

	private int m_nBlacklistEntryMaxCount;

	private int m_nBlessingQuestListMaxCount;

	private int m_nBlessingQuestRequiredHeroLevel;

	private int m_nBlessingListMaxCount;

	private Dictionary<int, Blessing> m_blessings = new Dictionary<int, Blessing>();

	private Dictionary<int, BlessingTargetLevel> m_blessingTargetLevels = new Dictionary<int, BlessingTargetLevel>();

	private int m_nOwnerProspectQuestListMaxCount;

	private int m_nTargetProspectQuestListMaxCount;

	private Dictionary<int, Biography> m_biographies = new Dictionary<int, Biography>();

	private Dictionary<int, BiographyQuestDungeon> m_biographyQuestDungeons = new Dictionary<int, BiographyQuestDungeon>();

	private ItemLuckyShop m_itemLuckyShop;

	private CreatureCardLuckyShop m_creatureCardLuckyShop;

	private Dictionary<int, CreatureCharacter> m_creatureCharacters = new Dictionary<int, CreatureCharacter>();

	private CreatureGrade[] m_creatureGrades = new CreatureGrade[5];

	private Dictionary<int, Creature> m_creatures = new Dictionary<int, Creature>();

	private List<CreatureLevel> m_creatureLevels = new List<CreatureLevel>();

	private Dictionary<int, CreatureBaseAttr> m_creatureBaseAttrs = new Dictionary<int, CreatureBaseAttr>();

	private Dictionary<int, CreatureAdditionalAttr> m_creatureAdditionalAttrs = new Dictionary<int, CreatureAdditionalAttr>();

	private int m_nTotalCreatureAdditionalAttrPickPoint;

	private List<CreatureSkillGrade> m_creatureSkillGrades = new List<CreatureSkillGrade>();

	private int m_nTotalCreatureSkillGradePickPoint;

	private Dictionary<int, CreatureSkill> m_creatureSkills = new Dictionary<int, CreatureSkill>();

	private List<CreatureSkillCountPoolEntry> m_creatureSkillCountPool = new List<CreatureSkillCountPoolEntry>();

	private int m_nTotalCreautreSkillCountPickPoint;

	private List<CreatureSkillSlotOpenRecipe> m_creatureSkillSlotOpenRecipes = new List<CreatureSkillSlotOpenRecipe>();

	private Dictionary<int, CreatureSkillSlotProtection> m_creatureSkillSlotPortections = new Dictionary<int, CreatureSkillSlotProtection>();

	private List<CreatureInjectionLevel> m_creatureInjectionLevels = new List<CreatureInjectionLevel>();

	private List<CreatureInjectionLevelUpEntry> m_creatureInjectionLevelUpEntries = new List<CreatureInjectionLevelUpEntry>();

	private int m_nTotalCreatureInjectionLevelUpEntryPickPoint;

	private DragonNest m_dragonNest;

	private Dictionary<int, Present> m_presents = new Dictionary<int, Present>();

	private Dictionary<int, WeeklyPresentPopularityPointRankingRewardGroup> m_weeklyPresentPopularityPointRankingRewardGroups = new Dictionary<int, WeeklyPresentPopularityPointRankingRewardGroup>();

	private Dictionary<int, WeeklyPresentContributionPointRankingRewardGroup> m_weeklyPresentContributionPointRankingRewardGroups = new Dictionary<int, WeeklyPresentContributionPointRankingRewardGroup>();

	private Dictionary<int, Costume> m_costumes = new Dictionary<int, Costume>();

	private Dictionary<int, CostumeEffect> m_costumeEffects = new Dictionary<int, CostumeEffect>();

	private List<CostumeEnchantLevel> m_costumeEnchantLevels = new List<CostumeEnchantLevel>();

	private Dictionary<int, CostumeCollection> m_costumeCollections = new Dictionary<int, CostumeCollection>();

	private CreatureFarmQuest m_creatureFarmQuest;

	private Dictionary<int, CashProduct> m_cashProducts = new Dictionary<int, CashProduct>();

	private FirstChargeEvent m_firstChargeEvent;

	private RechargeEvent m_rechargeEvent;

	private Dictionary<int, ChargeEvent> m_chargeEvents = new Dictionary<int, ChargeEvent>();

	private DailyChargeEvent m_dailyChargeEvent;

	private Dictionary<int, ConsumeEvent> m_consumeEvents = new Dictionary<int, ConsumeEvent>();

	private DailyConsumeEvent m_dailyConsumeEvent;

	private List<JobChangeQuest> m_jobChangeQuests = new List<JobChangeQuest>();

	private Dictionary<int, PotionAttr> m_potionAttrs = new Dictionary<int, PotionAttr>();

	private AnkouTomb m_ankouTomb;

	private Dictionary<int, Constellation> m_constellations = new Dictionary<int, Constellation>();

	private ArtifactRequiredConditionType m_artifactRequiredConditionType = ArtifactRequiredConditionType.HeroLevel;

	private int m_nArtifactRequiredConditionValue;

	private int m_nArtifactMaxLevel;

	private List<Artifact> m_artifacts = new List<Artifact>();

	private List<ArtifactLevelUpMaterial> m_artifactLevelUpMaterials = new List<ArtifactLevelUpMaterial>();

	private TradeShip m_tradeShip;

	private Dictionary<int, SystemMessage> m_systemMessages = new Dictionary<int, SystemMessage>();

	private Dictionary<int, TimeDesignationEvent> m_timeDesignationEvents = new Dictionary<int, TimeDesignationEvent>();

	private static Resource s_instance = new Resource();

    #endregion

    #region Properties

    public int sectorSize => m_nSectorSize;

	public Language defaultLanguage => m_defaultLanguage;

	public int maxUserConnectionCount => m_nMaxUserConnectionCount;

	public Regex heroNameRegex => m_heroNameRegex;

	public Regex guildNameRegex => m_guildNameRegex;

	public int maxHeroCount => m_nMaxHeroCount;

	public int mailRetentionDay => m_nMailRetentionDay;

	public int startContinentId => m_nStartContinentId;

	public Vector3 startPosition => m_startPosition;

	public float startRadius => m_fStartRadius;

	public int startYRotationType => m_nStartYRotationType;

	public float startYRotation => m_fStartYRotation;

	public int mainGearOptionAttrMinCount => m_nMainGearOptionAttrMinCount;

	public int mainGearOptionAttrMaxCount => m_nMainGearOptionAttrMaxCount;

	public int mainGearRefinementItemId => m_nMainGearRefimentItemId;

	public float battleModeDuration => m_fBattleModeDuration;

	public int specialSkillId => m_nSpecialSkillId;

	public int specialSkillMaxLak => m_nSpecialSkillMaxLak;

	public int freeImmediateRevivalDailyCount => m_nFreeImmediateRevivalDailyCount;

	public int autoSaftyRevivalWaitingTime => m_nAutoSaftyRevivalWaitingTime;

	public Vector3 startContinentSaftyRevivalPosition => m_startContinentSaftyRevivalPosition;

	public float startContinentSaftyRevivalRadius => m_fStartContinentSaftyRevivalRadius;

	public int startContinentSaftyRevivalYRotationType => m_nStartContinentSaftyRevivalYRotationType;

	public float startContinentSaftyRevivalYRotation => m_fStartContinentSaftyRevivalYRotation;

	public int saftyRevivalContinentId => m_nSaftyRevivalContinentId;

	public Vector3 saftyRevivalPosition => m_saftyRevivalPosition;

	public float saftyRevivalRadius => m_fSaftyRevivalRadius;

	public int saftyRevivalYRotationType => m_nSaftyRevivalYRotationType;

	public float saftyRevivalYRotation => m_fSaftyRevivalRadius;

	public int simpleShopSellSlotCount => m_nSimpleShopSellSlotCount;

	public int mainGearDisassembleSlotCount => m_nMainGearDisassembleSlotCount;

	public long weekendAttendItemRewardId => m_lnWeekendAttendItemRewardId;

	public int todayMissionCount => m_nTodayMissionCount;

	public int partyMemberMaxCount => m_nPartyMemberMaxCount;

	public int partyMemberLogOutDuration => m_nPartyMemberLogOutDuration;

	public int partyInvitationLifetime => m_nPartyInvitationLifetime;

	public int partyApplicationLifetime => m_nPartyApplicationLifetime;

	public int partyCallCoolTime => m_nPartyCallCoolTime;

	public int restRewardRequiredHeroLevel => m_nRestRewardRequiredHeroLevel;

	public int restRewardGoldReceiveExpRate => m_nRestRewardGoldReceiveExpRate;

	public int restRewardDiaReceiveExpRate => m_nRestRewardDiaReceiveExpRate;

	public int chattingMaxLength => m_nChattingMaxLength;

	public int chattingMinInterval => m_nChattingMinInterval;

	public int worldChattingItemId => m_nWorldChattingItemId;

	public Item mountLevelUpItem => m_mountLevelUpItem;

	public int mountQualityUpRequiredLevelUpCount => m_nMountQualityUpRequiredLevelUpCount;

	public float equippedMountAttrFactor => m_fEquippedMountAttrFactor;

	public int mountGearOptionAttrCount => m_nMountGearOptionAttrCount;

	public int mountGearRefinementItemId => m_nMountGearRefinementItemId;

	public int mountAwakeningRequiredHeroLevel => m_nMountAwakeningRequiredHeroLevel;

	public Item mountAwakeningItem => m_mountAwakeningItem;

	public int mountPotionAttrItemId => m_nMountPotionAttrItemId;

	public int dungeonFreeSweepDailyCount => m_nDungeonFreeSweepDailyCount;

	public int dungeonSweepItemId => m_nDungeonSweepItemId;

	public int wingEnchantItemId => m_nWingEnchantItemId;

	public int wingEnchantExp => m_nWingEnchantExp;

	public int wingVisibleMinVipLevel => m_nWingVisibleMinVipLevel;

	public int bountyHunterMaxCount => m_nBountyHunterQuestMaxCount;

	public int rankingDisplayMaxCount => m_nRankingDisplayMaxCount;

	public int guildRankingDisplayMaxCount => m_nGuildRankingDisplayMaxCount;

	public int presentPopularityPointRankingDisplayMaxCount => m_nPresentPopularityPointRankingDisplayMaxCount;

	public int presentContributionPointRankingDisplayMaxCount => m_nPresentContributionPointRankingDisplayMaxCount;

	public int guildRequiredHeroLevel => m_nGuildRequiredHeroLevel;

	public int guildCreationRequiredVipLevel => m_nGuildCreationRequiredVipLevel;

	public int guildCreationRequiredDia => m_nGuildCreationRequiredDia;

	public int guildRejoinIntervalTime => m_nGuildRejoinIntervalTime;

	public int guildApplicationReceptionMaxCount => m_nGuildApplicationReceptionMaxCount;

	public int guildDailyApplicationMaxCount => m_nGuildDailyApplicationMaxCount;

	public int guildDailyBanishmentMaxCount => m_nGuildDailyBanishmentMaxCount;

	public int guildInvitationLifetime => m_nGuildInvitationLifetime;

	public int guildNoticeMaxLength => m_nGuildNoticeMaxLength;

	public int guildViceMasterCount => m_nGuildViceMasterCount;

	public int guildLordCount => m_nGuildLordCount;

	public int guildCallLifetime => m_nGuildCallLifetime;

	public float guildCallRadius => m_fGuildCallRadius;

	public int guildDailyObjectiveNoticeCoolTime => m_nGuildDailyObjectiveNoticeCoolTime;

	public int defaultGuildWeeklyObjectiveId => m_nDefaultGuildWeeklyObjectiveId;

	public int guildHuntingDonationMaxCount => m_nGuildHuntingDonationMaxCount;

	public Item guildHuntingDonationItem => m_guildHuntingDonationItem;

	public ItemReward guildHuntingDonationReward => m_guildHuntingDonationReward;

	public ItemReward guildHuntingDonationCompletionReward => m_guildHuntingDonationCompletionReward;

	public int nationCallLifeTime => m_nNationCallLifeTime;

	public float nationCallRadius => m_fNationCallRadius;

	public GuildTerritory guildTerritory => m_guildTerritory;

	public GuildFarmQuest guildFarmQuest => m_guildFarmQuest;

	public GuildFoodWarehouse guildFoodWarehouse => m_guildFoodWarehouse;

	public GuildAltar guildAltar => m_guildAltar;

	public Dictionary<int, GuildBuilding> guildBuildings => m_guildBuildings;

	public GuildBuilding guildLobby => m_guildLobby;

	public GuildBuilding guildLaboratory => m_guildLaboratory;

	public GuildBuilding guildShop => m_guildShop;

	public GuildBuilding guildTankFactory => m_guildTankFactory;

	public GuildMissionQuest guildMissionQuest => m_guildMissionQuest;

	public GuildHuntingQuest guildHuntingQuest => m_guildHuntingQuest;

	public int rankOpenRequiredMainQuestNo => m_nRankOpenRequiredMainQuestNo;

	public Dictionary<int, RankActiveSkill> rankActiveSkills => m_rankActiveSkills;

	public Dictionary<int, RankPassiveSkill> rankPassiveSkills => m_rankPassiveSkills;

	public int wingOpenRequiredHeroLevel => m_nWingOpenRequiredHeroLevel;

	public int wingOpenProvideWingId => m_nWingOpenProvideWingId;

	public Wing wingOpenProvideWing => GetWing(m_nWingOpenProvideWingId);

	public int nationWarMonsterBattleModeDuration => m_nNationWarMonsterBattleModeDuration;

	public int creatureCardShopRandomProductCount => m_nCreatureCardShopRandomProductCount;

	public int creatureCardShopPaidRefreshDia => m_nCreatureCardShopPaidRefreshDia;

	public int accelerationRequiredMoveDuration => m_nAccelerationRequiredMoveDuration;

	public int accelerationMoveSpeed => m_nAccelerationMoveSpeed;

	public int sceneryQuestRequiredMainQuestNo => m_nSceneryQuestRequiredMainQuestNo;

	public int monsterGroggyDuration => m_nMonsterGroggyDuration;

	public int monsterStealDuration => m_nMonsterStealDuration;

	public int openGiftRequiredHeroLevel => m_nOpenGiftRequiredHeroLevel;

	public int open7DayEventRequiredMainQuestNo => m_nOpen7DayEventRequiredMainQuestNo;

	public int bountyQuestRequiredHeroLevel => m_nBountyHunterQuestRequiredHeroLevel;

	public int taskConsignmentRequiredVipLevel => m_nTaskConsignmentRequiredVipLevel;

	public int eliteMonsterKillApplicationRequiredHeroLevel => m_nEliteMonsterKillApplicationRequiredHeroLevel;

	public int warehouseRequiredVipLevel => m_nWarehouseRequiredVipLevel;

	public int freeWarehouseSlotCount => m_nFreeWarehouseSlotCount;

	public int wingMemoryPieceInstallationRequiredHeroLevel => m_nWingMemoryPieceInstallationRequiredHeroLevel;

	public int ordealQuestSlotCount => m_nOrdealQuestSlotCount;

	public int creatureMaxCount => m_nCreatureMaxCount;

	public int creatureCheerMaxCount => m_nCreatureCheerMaxCount;

	public float creatureCheerAttrFactor => m_fCreatureCheerAttrFactor;

	public float creatureEvaluationFactor => m_fCreatureEvaluationFactor;

	public int creatureAdditionalAttrCount => m_nCreatureAdditionalAttrCount;

	public int creatureSkillSlotMaxCount => m_nCreatureSkillSlotMaxCount;

	public int creatureSkillSlotBaseOpenCount => m_nCreatureSkillSlotBaseOpenCount;

	public int creatureCompositionSkillProtectionItemId => m_nCreatureCompositionSkillProtectionItemId;

	public int creatureInjectionExpRetrievalRate => m_nCreatureInjectionExpRetrievalRate;

	public Item creatureInjectionExpItem => m_creatureInjectionExpItem;

	public int creatureVariationRequiredItemId => m_nCreatureVariationRequiredItemId;

	public int creatureAdditionalAttrSwitchRequiredItemId => m_nCreatureAdditionalAttrSwitchRequiredItemId;

	public int creatureReleaseExpRetrievalRate => m_nCreatureReleaseExpRetrievalRate;

	public GuildTerritoryNpc guildBlessingGuildTerritoryNpc => m_guildBlessingGuildTerritoryNpc;

	public ItemReward open7DayEventCostumeItemReward => m_open7DayEventCostumeItemReward;

	public int open7DayEventCostumeRewardRequiredItemId => m_nOpen7DayEventCostumeRewardRequiredItemId;

	public int open7DayEventCostumeRewardRequiredItemCount => m_nOpen7DayEventCostumeRewardRequiredItemCount;

	public int nationAllianceUnavailableStartTime => m_nNationAllianceUnavailableStartTime;

	public int nationAllianceUnavailableEndTime => m_nNationAllianceUnavailableEndTime;

	public int nationAllianceRequiredFund => m_nNationAllianceRequiredFund;

	public int nationAllianceRenounceUnavailableDuration => m_nNationAllianceRenounceUnavailableDuration;

	public int nationBasePower => m_nNationBasePower;

	public int jobChangeRequiredHeroLevel => m_nJobChangeRequiredHeroLevel;

	public int jobChangeRequiredItemId => m_nJobChangeRequiredItemId;

	public Item costumeEnchantItem => m_costumeEnchantItem;

	public int costumeCollectionActivationItemId => m_nCostumeCollectionActivationItemId;

	public int costumeCollectionShuffleItemId => m_nCostumeCollectionShuffleItemId;

	public int costumeCollectionShuffleItemCount => m_nCostumeCollectionShuffleItemCount;

	public SFTextFilter nameBanWordFilter => m_nameBanWordTextFilter;

	public SFTextFilter chatBanWordFilter => m_chatBanWordTextFilter;

	public Item[] creatureFeeds => m_creatureFeeds;

	public MainGearEnchantLevel maxMainGearEnchantLevel => m_mainGearEnchantLevels.LastOrDefault();

	public MainQuest startMainQuest => GetMainQuest(1);

	public MainQuest lastMainQuest => m_mainQuests.LastOrDefault();

	public TreatOfFarmQuest treatOfFarmQuest => m_treatOfFarmQuest;

	public FishingQuest fishingQuest => m_fishingQuest;

	public Dictionary<int, Job> jobs => m_jobs;

	public JobLevelMaster lastJobLevelMaster => m_jobLevelMasters.LastOrDefault();

	public InventorySlotExtendRecipe lastInventorySlotExtnedRecipe => m_inventorySlotExtendRecipes.LastOrDefault();

	public RestRewardTime startRestRewardTime => m_restRewardTimes.FirstOrDefault();

	public RestRewardTime lastRestRewardTime => m_restRewardTimes.LastOrDefault();

	public int dailyAttendRewardDayCount => m_dailyAttendRewardEntries.Count;

	public Dictionary<int, SeriesMission> seriesMissions => m_seriesMissions;

	public WingStep lastWingStep => m_wingSteps.LastOrDefault();

	public WingEnchantCountPool wingEnchantCountPool => m_wingEnchantCountPool;

	public Dictionary<int, WingPart> wingParts => m_wingParts;

	public int maxStamina => m_nMaxStamina;

	public int staminaRecoveryTime => m_nStaminaRecoveryTime;

	public int cartNormalSpeed => m_nCartNormalSpeed;

	public int cartHighSpeed => m_nCartHighSpeed;

	public int cartHighSpeedDuration => m_nCartHighSpeedDuration;

	public int cartHighSpeedDurationExtension => m_nCartHighSpeedDurationExtension;

	public int cartAccelCoolTime => m_nCartAccelCoolTime;

	public int cartAccelSuccessRate => m_nCartAccelSuccessRate;

	public int pvpMinHeroLevel => m_nPvpMinHeroLevel;

	public int worldLevelExpBuffMinHeroLevel => m_nWorldLevelExpBuffMinHeroLevel;

	public Dictionary<int, Continent> continents => m_continents;

	public List<StoryDungeon> storyDungeons => m_storyDungeons;

	public ExpDungeon expDungeon => m_expDungeon;

	public GoldDungeon goldDungeon => m_goldDungeon;

	public UndergroundMaze undergroundMaze => m_undergroundMaze;

	public ArtifactRoom artifactRoom => m_artifactRoom;

	public AncientRelic ancientRelic => m_ancientRelic;

	public Dictionary<int, HonorShopProduct> honorShopProducts => m_honorShopProducts;

	public FieldOfHonor fieldOfHonor => m_fieldOfHonor;

	public Dictionary<int, Nation> nations => m_nations;

	public int nationTransmissionRequiredHeroLevel => m_nNationTransmissionRequiredHeroLevel;

	public MysteryBoxQuest mysteryBoxQuest => m_mysteryBoxQuest;

	public SecretLetterQuest secretLetterQuest => m_secretLetterQuest;

	public DimensionRaidQuest dimensionRaidQuest => m_dimensionRaidQuest;

	public HolyWarQuest holyWarQuest => m_holyWarQuest;

	public DimensionInfiltrationEvent dimensionInfiltrationEvent => m_dimensionInfiltrationEvent;

	public BattlefieldSupportEvent battlefieldSupportEvent => m_battlefieldSupportEvent;

	public SafeTimeEvent safeTimeEvent => m_safeTimeEvent;

	public Dictionary<int, TodayTask> todayTasks => m_todayTasks;

	public List<AchievementReward> ahievementRewards => m_achievementRewards;

	public AchievementReward lastAchievementReward => m_achievementRewards.LastOrDefault();

	public GuildSupplySupportQuest guildSupplySupportQuest => m_guildSupplySupportQuest;

	public SupplySupportQuest supplySupportQuest => m_supplySupportQuest;

	public Dictionary<int, NationNoblesse> nationNoblesses => m_nationNoblesses;

	public NationWar nationWar => m_nationWar;

	public SoulCoveter soulCoveter => m_soulCoveter;

	public Dictionary<int, SceneryQuest> sceneryQuests => m_sceneryQuests;

	public Dictionary<int, Accomplishment> accomplishments => m_accomplishments;

	public List<CreatureCardShopRefreshSchedule> creatureCardShopRefreshSchedules => m_creatureCardShopRefreshSchedules;

	public CreatureCardShopRefreshSchedule lastCreatureCardShopRefreshSchedule => m_creatureCardShopRefreshSchedules.LastOrDefault();

	public Dictionary<int, EliteMonsterMaster> eliteMonsterMasters => m_eliteMonsterMasters;

	public EliteDungeon eliteDungeon => m_eliteDungeon;

	public ProofOfValor proofOfValor => m_proofOfValor;

	public StaminaBuyCount lastStaminaBuyCount => m_staminaBuyCounts.LastOrDefault();

	public List<StaminaRecoverySchedule> staminaRecoverySchedules => m_staminaRecoverySchedules;

	public StaminaRecoverySchedule lastStaminaRecoverySchedule => m_staminaRecoverySchedules.LastOrDefault();

	public int totalStaminaOfStaminaRecoverySchedules => m_nTotalStaminaOfStaminaRecoverySchedules;

	public List<JobCommonSkill> jobCommonSkills => m_jobCommonSkills;

	public DailyQuest dailyQuest => m_dailyQuest;

	public WeeklyQuest weeklyQuest => m_weeklyQuest;

	public WisdomTemple wisdomTemple => m_wisdomTemple;

	public Dictionary<int, Open7DayEventDay> open7DayEventDays => m_open7DayEventDays;

	public Dictionary<int, Open7DayEventMission> open7DayEventMissions => m_open7DayEventMissions;

	public Dictionary<int, Open7DayEventProduct> open7DayEventProducts => m_open7DayEventProducts;

	public Dictionary<int, Retrieval> retrievals => m_retrievals;

	public RuinsReclaim ruinsReclaim => m_ruinsReclaim;

	public Dictionary<int, TaskConsignment> taskConsignments => m_taskConsignments;

	public TrueHeroQuest trueHeroQuest => m_trueHeroQuest;

	public InfiniteWar infiniteWar => m_infiniteWar;

	public FieldBossEvent fieldBossEvent => m_fieldBossEvent;

	public LimitationGift limitationGift => m_limitationGift;

	public WeekendReward weekendReward => m_weekendReward;

	public WarehouseSlotExtendRecipe lastWarehouseSlotExtendRecipe => m_warehouseSlotExtendRecipes.LastOrDefault();

	public int lastWarehouseSlotExtendRecipeSlotCount
	{
		get
		{
			if (lastWarehouseSlotExtendRecipe == null)
			{
				return 0;
			}
			return lastWarehouseSlotExtendRecipe.slotCount;
		}
	}

	public FearAltar fearAltar => m_fearAltar;

	public WarMemory warMemory => m_warMemory;

	public OsirisRoom osirisRoom => m_osirisRoom;

	public int friendMaxCount => m_nFriendMaxCount;

	public int tempFriendMaxCount => m_nTempFriendMaxCount;

	public int deadRecordMaxCount => m_nDeadRecordMaxCount;

	public int blacklistEntryMaxCount => m_nBlacklistEntryMaxCount;

	public int blessingQuestListMaxCount => m_nBlessingQuestListMaxCount;

	public int blessingQuestRequiredHeroLevel => m_nBlessingQuestRequiredHeroLevel;

	public int blessingListMaxCount => m_nBlessingListMaxCount;

	public int ownerProspectQuestListMaxCount => m_nOwnerProspectQuestListMaxCount;

	public int targetProspectQuestListMaxCount => m_nTargetProspectQuestListMaxCount;

	public ItemLuckyShop itemLuckyShop => m_itemLuckyShop;

	public CreatureCardLuckyShop creatureCardLuckyShop => m_creatureCardLuckyShop;

	public CreatureLevel lastCreatureLevel => m_creatureLevels.LastOrDefault();

	public DragonNest dragonNest => m_dragonNest;

	public int lastCostumeEnchantLevel => m_costumeEnchantLevels.Count;

	public CreatureFarmQuest creatureFarmQuest => m_creatureFarmQuest;

	public FirstChargeEvent firstChargeEvent => m_firstChargeEvent;

	public RechargeEvent rechargeEvent => m_rechargeEvent;

	public DailyChargeEvent dailyChargeEvent => m_dailyChargeEvent;

	public DailyConsumeEvent dailyConsumeEvent => m_dailyConsumeEvent;

	public Dictionary<int, PotionAttr> potionAttrs => m_potionAttrs;

	public AnkouTomb ankouTomb => m_ankouTomb;

	public Dictionary<int, Constellation> constellations => m_constellations;

	public ArtifactRequiredConditionType artifactRequiredConditionType => m_artifactRequiredConditionType;

	public int artifactRequiredConditionValue => m_nArtifactRequiredConditionValue;

	public int artifactMaxLevel => m_nArtifactMaxLevel;

	public int maxArtifactNo => m_artifacts.Count;

	public TradeShip tradeShip => m_tradeShip;

	public Dictionary<int, TimeDesignationEvent> timeDesignationEvents => m_timeDesignationEvents;

	public static Resource instance => s_instance;

    #endregion

    private Resource()
	{
	}

	public void Init()
	{
		SFLogUtil.Info(GetType(), "Resource.Init() started.");
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		LoadUserDBResources(currentTime);
		InitLate();
		SFLogUtil.Info(GetType(), "Resource.Init() finished.");
	}

	private void LoadUserDBResources(DateTimeOffset currentTime)
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenUserDBConnection();
			foreach (DataRow dr9 in UserDac.Languages(conn, null))
			{
				Language language = new Language();
				language.Set(dr9);
				m_languages.Add(language.id, language);
			}
			foreach (DataRow dr13 in UserDac.SupportedLanguages(conn, null))
			{
				int nLanguageId = Convert.ToInt32(dr13["languageId"]);
				Language language2 = GetLanguage(nLanguageId);
				if (language2 != null)
				{
					language2.supported = true;
				}
			}
			DataRow drSystemSetting = UserDac.SystemSetting(conn, null);
			if (drSystemSetting == null)
			{
				throw new Exception(Resources.Exception.Resource_LoadUserDBResources_01);
			}
			m_heroNameRegex = new Regex(Convert.ToString(drSystemSetting["heroNameRegex"]));
			m_guildNameRegex = new Regex(Convert.ToString(drSystemSetting["guildNameRegex"]));
			m_nMaxUserConnectionCount = Convert.ToInt32(drSystemSetting["maxUserConnectionCount"]);
			int nDefaultLanguageId = Convert.ToInt32(drSystemSetting["defaultLanguageId"]);
			m_defaultLanguage = GetLanguage(nDefaultLanguageId);
			if (m_defaultLanguage == null)
			{
				throw new Exception(Resources.Exception.Resource_LoadUserDBResources_02 + " nDefaultLanguageId = " + nDefaultLanguageId);
			}
			DataRow drGameConfig = UserDac.GameConfig(conn, null);
			if (drGameConfig == null)
			{
				throw new Exception(Resources.Exception.Resource_LoadUserDBResources_03);
			}
			m_nMaxHeroCount = Convert.ToInt32(drGameConfig["maxHeroCount"]);
			m_nStartContinentId = Convert.ToInt32(drGameConfig["startContinentId"]);
			if (m_nStartContinentId <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_04 + "m_nStartContinentId = " + m_nStartContinentId);
			}
			m_startPosition.x = Convert.ToSingle(drGameConfig["startXPosition"]);
			m_startPosition.y = Convert.ToSingle(drGameConfig["startYPosition"]);
			m_startPosition.z = Convert.ToSingle(drGameConfig["startZPosition"]);
			m_fStartRadius = Convert.ToSingle(drGameConfig["startRadius"]);
			m_nStartYRotationType = Convert.ToInt32(drGameConfig["startYRotationType"]);
			m_fStartYRotation = Convert.ToSingle(drGameConfig["startYRotation"]);
			m_nMailRetentionDay = Convert.ToInt32(drGameConfig["mailRetentionDay"]);
			m_fBattleModeDuration = Convert.ToSingle(drGameConfig["battleModeDuration"]);
			m_nSpecialSkillId = Convert.ToInt32(drGameConfig["specialSkillId"]);
			m_nSpecialSkillMaxLak = Convert.ToInt32(drGameConfig["specialSkillMaxLak"]);
			m_nFreeImmediateRevivalDailyCount = Convert.ToInt32(drGameConfig["freeImmediateRevivalDailyCount"]);
			m_nAutoSaftyRevivalWaitingTime = Convert.ToInt32(drGameConfig["autoSaftyRevivalWatingTime"]);
			m_startContinentSaftyRevivalPosition.x = Convert.ToSingle(drGameConfig["startContinentSaftyRevivalXPosition"]);
			m_startContinentSaftyRevivalPosition.y = Convert.ToSingle(drGameConfig["startContinentSaftyRevivalYPosition"]);
			m_startContinentSaftyRevivalPosition.z = Convert.ToSingle(drGameConfig["startContinentSaftyRevivalZPosition"]);
			m_fStartContinentSaftyRevivalRadius = Convert.ToSingle(drGameConfig["startContinentSaftyRevivalRadius"]);
			m_nStartContinentSaftyRevivalYRotationType = Convert.ToInt32(drGameConfig["startContinentSaftyRevivalYRotationType"]);
			m_fStartContinentSaftyRevivalYRotation = Convert.ToSingle(drGameConfig["startContinentSaftyRevivalYRotation"]);
			m_nSaftyRevivalContinentId = Convert.ToInt32(drGameConfig["saftyRevivalContinentId"]);
			if (m_nSaftyRevivalContinentId <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_05 + "m_nSaftyRevivalContinentId = " + m_nSaftyRevivalContinentId);
			}
			m_saftyRevivalPosition.x = Convert.ToSingle(drGameConfig["saftyRevivalXPosition"]);
			m_saftyRevivalPosition.y = Convert.ToSingle(drGameConfig["saftyRevivalYPosition"]);
			m_saftyRevivalPosition.z = Convert.ToSingle(drGameConfig["saftyRevivalZPosition"]);
			m_fSaftyRevivalRadius = Convert.ToSingle(drGameConfig["saftyRevivalRadius"]);
			m_nSaftyRevivalYRotationType = Convert.ToInt32(drGameConfig["saftyRevivalYRotationType"]);
			m_fSaftyRevivalYRotation = Convert.ToSingle(drGameConfig["saftyRevivalYRotation"]);
			m_nSimpleShopSellSlotCount = Convert.ToInt32(drGameConfig["simpleShopSellSlotCount"]);
			if (m_nSimpleShopSellSlotCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_06 + "m_nSimpleShopSellSlotCount = " + m_nSimpleShopSellSlotCount);
			}
			m_lnWeekendAttendItemRewardId = Convert.ToInt64(drGameConfig["weekendAttendItemRewardId"]);
			if (m_lnWeekendAttendItemRewardId < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_07 + "m_lnWeekendAttendItemRewardId = " + m_lnWeekendAttendItemRewardId);
			}
			m_nTodayMissionCount = Convert.ToInt32(drGameConfig["todayMissionCount"]);
			if (m_nTodayMissionCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_08 + "m_nTodayMissionCount = " + m_nTodayMissionCount);
			}
			m_nMainGearOptionAttrMinCount = Convert.ToInt32(drGameConfig["mainGearOptionAttrMinCount"]);
			if (m_nMainGearOptionAttrMinCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_09 + "m_nMainGearOptionAttrMinCount = " + m_nMainGearOptionAttrMinCount);
			}
			m_nMainGearOptionAttrMaxCount = Convert.ToInt32(drGameConfig["mainGearOptionAttrMaxCount"]);
			if (m_nMainGearOptionAttrMinCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_10 + "m_nMainGearOptionAttrMinCount = " + m_nMainGearOptionAttrMinCount);
			}
			m_nMainGearRefimentItemId = Convert.ToInt32(drGameConfig["mainGearRefinementItemId"]);
			if (m_nMainGearRefimentItemId <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_11 + "m_nMainGearRefimentItemId = " + m_nMainGearRefimentItemId);
			}
			m_nMainGearDisassembleSlotCount = Convert.ToInt32(drGameConfig["mainGearDisassembleSlotCount"]);
			if (m_nMainGearDisassembleSlotCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_12 + "m_nMainGearDisassembleSlotCount = " + m_nMainGearDisassembleSlotCount);
			}
			m_nPartyMemberMaxCount = Convert.ToInt32(drGameConfig["partyMemberMaxCount"]);
			if (m_nPartyMemberMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_13 + "m_nPartyMemberMaxCount = " + m_nPartyMemberMaxCount);
			}
			m_nPartyMemberLogOutDuration = Convert.ToInt32(drGameConfig["partyMemberLogOutDuration"]);
			if (m_nPartyMemberLogOutDuration < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_14 + "m_nPartyMemberLogOutDuration = " + m_nPartyMemberLogOutDuration);
			}
			m_nPartyInvitationLifetime = Convert.ToInt32(drGameConfig["partyInvitationLifetime"]);
			if (m_nPartyInvitationLifetime <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_15 + "m_nPartyInvitationLifetime = " + m_nPartyInvitationLifetime);
			}
			m_nPartyApplicationLifetime = Convert.ToInt32(drGameConfig["partyApplicationLifetime"]);
			if (m_nPartyApplicationLifetime <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_16 + "m_nPartyApplicationLifetime = " + m_nPartyApplicationLifetime);
			}
			m_nPartyCallCoolTime = Convert.ToInt32(drGameConfig["partyCallCoolTime"]);
			if (m_nPartyCallCoolTime < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_17 + "m_nPartyCallCoolTime = " + m_nPartyCallCoolTime);
			}
			m_nRestRewardRequiredHeroLevel = Convert.ToInt32(drGameConfig["restRewardRequiredHeroLevel"]);
			if (m_nRestRewardRequiredHeroLevel <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_18 + "m_nRestRewardRequiredHeroLevel = " + m_nRestRewardRequiredHeroLevel);
			}
			m_nRestRewardGoldReceiveExpRate = Convert.ToInt32(drGameConfig["restRewardGoldReceiveExpRate"]);
			if (m_nRestRewardGoldReceiveExpRate < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_19 + "m_nRestRewardGoldReceiveExpRate = " + m_nRestRewardGoldReceiveExpRate);
			}
			m_nRestRewardDiaReceiveExpRate = Convert.ToInt32(drGameConfig["restRewardDiaReceiveExpRate"]);
			if (m_nRestRewardDiaReceiveExpRate < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_20 + "m_nRestRewardDiaReceiveExpRate = " + m_nRestRewardDiaReceiveExpRate);
			}
			m_nChattingMaxLength = Convert.ToInt32(drGameConfig["chattingMaxLength"]);
			if (m_nChattingMaxLength <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_21 + "m_nChattingMaxLength = " + m_nChattingMaxLength);
			}
			m_nChattingMinInterval = Convert.ToInt32(drGameConfig["chattingMinInterval"]);
			if (m_nChattingMinInterval < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_22 + "m_nChattingMinInterval = " + m_nChattingMinInterval);
			}
			m_nWorldChattingItemId = Convert.ToInt32(drGameConfig["worldChattingItemId"]);
			if (m_nWorldChattingItemId < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_23 + "m_nWorldChattingItemId = " + m_nWorldChattingItemId);
			}
			int nMountLevelUpItemId = Convert.ToInt32(drGameConfig["mountLevelUpItemId"]);
			m_nMountQualityUpRequiredLevelUpCount = Convert.ToInt32(drGameConfig["mountQualityUpRequiredLevelUpCount"]);
			if (m_nMountQualityUpRequiredLevelUpCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_24 + "m_nMountQualityUpRequiredLevelUpCount = " + m_nMountQualityUpRequiredLevelUpCount);
			}
			m_fEquippedMountAttrFactor = Convert.ToSingle(drGameConfig["equippedMountAttrFactor"]);
			if (m_fEquippedMountAttrFactor <= 0f)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_25 + "m_fEquippedMountAttrFactor = " + m_fEquippedMountAttrFactor);
			}
			m_nMountGearOptionAttrCount = Convert.ToInt32(drGameConfig["mountGearOptionAttrCount"]);
			if (m_nMountGearOptionAttrCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_26 + "m_nMountGearOptionAttrCount = " + m_nMountGearOptionAttrCount);
			}
			m_nMountGearRefinementItemId = Convert.ToInt32(drGameConfig["mountGearRefinementItemId"]);
			if (m_nMountGearRefinementItemId <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_27 + "m_nMountGearRefinementItemId = " + m_nMountGearRefinementItemId);
			}
			m_nMountAwakeningRequiredHeroLevel = Convert.ToInt32(drGameConfig["mountAwakeningRequiredHeroLevel"]);
			if (m_nMountGearRefinementItemId <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_28 + "m_nMountAwakeningRequiredHeroLevel = " + m_nMountAwakeningRequiredHeroLevel);
			}
			int nMountAwakeningItemId = Convert.ToInt32(drGameConfig["mountAwakeningItemId"]);
			m_nMountPotionAttrItemId = Convert.ToInt32(drGameConfig["mountPotionAttrItemId"]);
			if (m_nMountGearRefinementItemId <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_29 + "m_nMountPotionAttrItemId = " + m_nMountPotionAttrItemId);
			}
			m_nDungeonFreeSweepDailyCount = Convert.ToInt32(drGameConfig["dungeonFreeSweepDailyCount"]);
			m_nDungeonSweepItemId = Convert.ToInt32(drGameConfig["dungeonSweepItemId"]);
			m_nWingEnchantItemId = Convert.ToInt32(drGameConfig["wingEnchantItemId"]);
			if (m_nWingEnchantItemId <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_30 + "m_nWingEnchantItemId = " + m_nWingEnchantItemId);
			}
			m_nWingEnchantExp = Convert.ToInt32(drGameConfig["wingEnchantExp"]);
			if (m_nWingEnchantExp < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_31 + "m_nWingEnchantExp = " + m_nWingEnchantExp);
			}
			m_nWingVisibleMinVipLevel = Convert.ToInt32(drGameConfig["wingVisibleMinVipLevel"]);
			if (m_nWingVisibleMinVipLevel < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_32 + "m_nWingVisibleMinVipLevel = " + m_nWingVisibleMinVipLevel);
			}
			m_nMaxStamina = Convert.ToInt32(drGameConfig["maxStamina"]);
			if (m_nMaxStamina <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_33 + "m_nMaxStamina = " + m_nMaxStamina);
			}
			m_nStaminaRecoveryTime = Convert.ToInt32(drGameConfig["staminaRecoveryTime"]);
			if (m_nStaminaRecoveryTime <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_34 + "m_nStaminaRecoveryTime = " + m_nStaminaRecoveryTime);
			}
			m_nCartNormalSpeed = Convert.ToInt32(drGameConfig["cartNormalSpeed"]);
			if (m_nCartNormalSpeed <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_35 + "m_nCartNormalSpeed = " + m_nCartNormalSpeed);
			}
			m_nCartHighSpeed = Convert.ToInt32(drGameConfig["cartHighSpeed"]);
			if (m_nCartHighSpeed <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_36 + "m_nCartHighSpeed = " + m_nCartHighSpeed);
			}
			m_nCartHighSpeedDuration = Convert.ToInt32(drGameConfig["cartHighSpeedDuration"]);
			if (m_nCartHighSpeedDuration <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_37 + "m_nCartHighSpeedDuration = " + m_nCartHighSpeedDuration);
			}
			m_nCartHighSpeedDurationExtension = Convert.ToInt32(drGameConfig["cartHighSpeedDurationExtension"]);
			if (m_nCartHighSpeedDurationExtension <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_38 + "m_nCartHighSpeedDurationExtension = " + m_nCartHighSpeedDurationExtension);
			}
			m_nCartAccelCoolTime = Convert.ToInt32(drGameConfig["cartAccelCoolTime"]);
			if (m_nCartAccelCoolTime < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_39 + "m_nCartAccelCoolTime = " + m_nCartAccelCoolTime);
			}
			m_nCartAccelSuccessRate = Convert.ToInt32(drGameConfig["cartAccelSuccessRate"]);
			if (m_nCartAccelSuccessRate <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_40 + "m_nCartAccelSuccessRate = " + m_nCartAccelSuccessRate);
			}
			m_nPvpMinHeroLevel = Convert.ToInt32(drGameConfig["pvpMinHeroLevel"]);
			if (m_nPvpMinHeroLevel < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_41 + "m_nPvpMinHeroLevel = " + m_nPvpMinHeroLevel);
			}
			m_nWorldLevelExpBuffMinHeroLevel = Convert.ToInt32(drGameConfig["worldLevelExpBuffMinHeroLevel"]);
			if (m_nWorldLevelExpBuffMinHeroLevel <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_42 + "m_nWorldLevelExpBuffMinHeroLevel = " + m_nWorldLevelExpBuffMinHeroLevel);
			}
			m_nNationTransmissionRequiredHeroLevel = Convert.ToInt32(drGameConfig["nationTransmissionRequiredHeroLevel"]);
			m_nationTransmissionExitPosition.x = Convert.ToSingle(drGameConfig["nationTransmissionExitXPosition"]);
			m_nationTransmissionExitPosition.y = Convert.ToSingle(drGameConfig["nationTransmissionExitYPosition"]);
			m_nationTransmissionExitPosition.z = Convert.ToSingle(drGameConfig["nationTransmissionExitZPosition"]);
			m_fNationTransmissionExitRadius = Convert.ToSingle(drGameConfig["nationTransmissionExitRadius"]);
			m_nNationTransmissionYRotationType = Convert.ToInt32(drGameConfig["nationTransmissionExitYRotationType"]);
			m_fNationTransmissionYRotation = Convert.ToSingle(drGameConfig["nationTransmissionExitYRotation"]);
			m_nBountyHunterQuestMaxCount = Convert.ToInt32(drGameConfig["bountyHunterQuestMaxCount"]);
			if (m_nBountyHunterQuestMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_43 + "m_nBountyHunterQuestMaxCount = " + m_nBountyHunterQuestMaxCount);
			}
			m_nRankingDisplayMaxCount = Convert.ToInt32(drGameConfig["rankingDisplayMaxCount"]);
			if (m_nRankingDisplayMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_44 + "m_nRankingDisplayMaxCount = " + m_nRankingDisplayMaxCount);
			}
			m_nGuildRankingDisplayMaxCount = Convert.ToInt32(drGameConfig["guildRankingDisplayMaxCount"]);
			if (m_nGuildRankingDisplayMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_45 + "m_nGuildRankingDisplayMaxCount = " + m_nGuildRankingDisplayMaxCount);
			}
			m_nPresentPopularityPointRankingDisplayMaxCount = Convert.ToInt32(drGameConfig["presentPopularityPointRankingDisplayMaxCount"]);
			if (m_nPresentPopularityPointRankingDisplayMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_46 + "m_nPresentPopularityPointRankingDisplayMaxCount = " + m_nPresentPopularityPointRankingDisplayMaxCount);
			}
			m_nPresentContributionPointRankingDisplayMaxCount = Convert.ToInt32(drGameConfig["presentContributionPointRankingDisplayMaxCount"]);
			if (m_nPresentContributionPointRankingDisplayMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_47 + "m_nPresentContributionPointRankingDisplayMaxCount = " + m_nPresentContributionPointRankingDisplayMaxCount);
			}
			m_nGuildRequiredHeroLevel = Convert.ToInt32(drGameConfig["guildRequiredHeroLevel"]);
			if (m_nGuildRequiredHeroLevel <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_48 + "m_nGuildRequiredHeroLevel = " + m_nGuildRequiredHeroLevel);
			}
			m_nGuildCreationRequiredVipLevel = Convert.ToInt32(drGameConfig["guildCreationRequiredVipLevel"]);
			if (m_nGuildCreationRequiredVipLevel < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_49 + "m_nGuildCreationRequiredVipLevel = " + m_nGuildCreationRequiredVipLevel);
			}
			m_nGuildCreationRequiredDia = Convert.ToInt32(drGameConfig["guildCreationRequiredDia"]);
			if (m_nGuildCreationRequiredDia <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_50 + "m_nGuildCreationRequiredDia = " + m_nGuildCreationRequiredDia);
			}
			m_nGuildRejoinIntervalTime = Convert.ToInt32(drGameConfig["guildRejoinIntervalTime"]);
			if (m_nGuildRejoinIntervalTime <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_51 + "m_nGuildRejoinIntervalTime = " + m_nGuildRejoinIntervalTime);
			}
			m_nGuildApplicationReceptionMaxCount = Convert.ToInt32(drGameConfig["guildApplicationReceptionMaxCount"]);
			if (m_nGuildApplicationReceptionMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_52 + "m_nGuildApplicationReceptionMaxCount = " + m_nGuildApplicationReceptionMaxCount);
			}
			m_nGuildDailyApplicationMaxCount = Convert.ToInt32(drGameConfig["guildDailyApplicationMaxCount"]);
			if (m_nGuildDailyApplicationMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_53 + "m_nGuildDailyApplicationMaxCount = " + m_nGuildDailyApplicationMaxCount);
			}
			m_nGuildDailyBanishmentMaxCount = Convert.ToInt32(drGameConfig["guildDailyBanishmentMaxCount"]);
			if (m_nGuildDailyBanishmentMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_54 + "m_nGuildDailyBanishmentMaxCount = " + m_nGuildDailyBanishmentMaxCount);
			}
			m_nGuildInvitationLifetime = Convert.ToInt32(drGameConfig["guildInvitationLifetime"]);
			if (m_nGuildInvitationLifetime <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_55 + "m_nGuildInvitationLifetime = " + m_nGuildInvitationLifetime);
			}
			m_nGuildNoticeMaxLength = Convert.ToInt32(drGameConfig["guildNoticeMaxLength"]);
			if (m_nGuildNoticeMaxLength <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_56 + " m_nGuildNoticeMaxLength = " + m_nGuildNoticeMaxLength);
			}
			m_nGuildViceMasterCount = Convert.ToInt32(drGameConfig["guildViceMasterCount"]);
			if (m_nGuildViceMasterCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_57 + "m_nGuildViceMasterCount = " + m_nGuildViceMasterCount);
			}
			m_nGuildLordCount = Convert.ToInt32(drGameConfig["guildLordCount"]);
			if (m_nGuildLordCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_58 + "m_nGuildLordCount = " + m_nGuildLordCount);
			}
			m_nGuildCallLifetime = Convert.ToInt32(drGameConfig["guildCallLifetime"]);
			if (m_nGuildCallLifetime < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_59 + "m_nGuildCallLifetime = " + m_nGuildCallLifetime);
			}
			m_fGuildCallRadius = Convert.ToSingle(drGameConfig["guildCallRadius"]);
			if (m_fGuildCallRadius <= 0f)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_60 + "m_fGuildCallRadius = " + m_fGuildCallRadius);
			}
			m_nGuildDailyObjectiveNoticeCoolTime = Convert.ToInt32(drGameConfig["guildDailyObjectiveNoticeCoolTime"]);
			if (m_nGuildDailyObjectiveNoticeCoolTime <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_61 + "m_nGuildDailyObjectiveNoticeCoolTime = " + m_nGuildDailyObjectiveNoticeCoolTime);
			}
			m_nDefaultGuildWeeklyObjectiveId = Convert.ToInt32(drGameConfig["defaultGuildWeeklyObjectiveId"]);
			if (m_nDefaultGuildWeeklyObjectiveId <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_62 + "m_nDefaultGuildWeeklyObjectiveId = " + m_nDefaultGuildWeeklyObjectiveId);
			}
			m_nGuildHuntingDonationMaxCount = Convert.ToInt32(drGameConfig["guildHuntingDonationMaxCount"]);
			if (m_nGuildHuntingDonationMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_63 + "m_nGuildHuntingDonationMaxCount = " + m_nGuildHuntingDonationMaxCount);
			}
			int nGuildHuntingDonationItemId = Convert.ToInt32(drGameConfig["guildHuntingDonationItemId"]);
			long lnGuildHuntingDonationRewardId = Convert.ToInt64(drGameConfig["guildHuntingDonationItemRewardId"]);
			long lnGuildHuntingDonationCompletionItemRewardId = Convert.ToInt64(drGameConfig["guildHuntingDonationCompletionItemRewardId"]);
			m_nRankOpenRequiredMainQuestNo = Convert.ToInt32(drGameConfig["rankOpenRequiredMainQuestNo"]);
			if (m_nRankOpenRequiredMainQuestNo <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_64 + "m_nRankOpenRequiredMainQuestNo = " + m_nRankOpenRequiredMainQuestNo);
			}
			m_nWingOpenRequiredHeroLevel = Convert.ToInt32(drGameConfig["wingOpenRequiredHeroLevel"]);
			if (m_nWingOpenRequiredHeroLevel <= 1)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_65 + "m_nWingOpenRequiredHeroLevel = " + m_nWingOpenRequiredHeroLevel);
			}
			m_nWingOpenProvideWingId = Convert.ToInt32(drGameConfig["wingOpenProvideWingId"]);
			if (m_nWingOpenProvideWingId < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_66 + "m_nWingOpenProvideWingId = " + m_nWingOpenProvideWingId);
			}
			m_nNationCallLifeTime = Convert.ToInt32(drGameConfig["nationCallLifetime"]);
			if (m_nNationCallLifeTime <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_67 + "m_nNationCallLifeTime = " + m_nNationCallLifeTime);
			}
			m_fNationCallRadius = Convert.ToSingle(drGameConfig["nationCallRadius"]);
			if (m_fNationCallRadius <= 0f)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_68 + "m_fNationCallRadius = " + m_fNationCallRadius);
			}
			m_nNationWarMonsterBattleModeDuration = Convert.ToInt32(drGameConfig["nationWarMonsterBattleModeDuration"]);
			if ((float)m_nNationWarMonsterBattleModeDuration <= 0f)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_69 + "m_nNationWarMonsterBattleModeDuration = " + m_nNationWarMonsterBattleModeDuration);
			}
			m_nCreatureCardShopRandomProductCount = Convert.ToInt32(drGameConfig["creatureCardShopRandomProductCount"]);
			if (m_nCreatureCardShopRandomProductCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_70 + "m_nCreatureCardShopRandomProductCount = " + m_nCreatureCardShopRandomProductCount);
			}
			m_nCreatureCardShopPaidRefreshDia = Convert.ToInt32(drGameConfig["creatureCardShopPaidRefreshDia"]);
			if (m_nCreatureCardShopPaidRefreshDia <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_71 + "m_nCreatureCardShopPaidRefreshDia = " + m_nCreatureCardShopPaidRefreshDia);
			}
			m_nAccelerationRequiredMoveDuration = Convert.ToInt32(drGameConfig["accelerationRequiredMoveDuration"]);
			if (m_nAccelerationRequiredMoveDuration <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_72 + "m_nAccelerationRequiredMoveDuration = " + m_nAccelerationRequiredMoveDuration);
			}
			m_nAccelerationMoveSpeed = Convert.ToInt32(drGameConfig["accelerationMoveSpeed"]);
			if (m_nAccelerationMoveSpeed <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_73 + "m_nAccelerationMoveSpeed = " + m_nAccelerationMoveSpeed);
			}
			m_nSceneryQuestRequiredMainQuestNo = Convert.ToInt32(drGameConfig["sceneryQuestRequiredMainQuestNo"]);
			if (m_nSceneryQuestRequiredMainQuestNo <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_74 + "m_nSceneryQuestRequiredMainQuestNo = " + m_nSceneryQuestRequiredMainQuestNo);
			}
			m_nMonsterGroggyDuration = Convert.ToInt32(drGameConfig["monsterGroggyDuration"]);
			if (m_nMonsterGroggyDuration <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_75 + "m_nMonsterGroggyDuration = " + m_nMonsterGroggyDuration);
			}
			m_nMonsterStealDuration = Convert.ToInt32(drGameConfig["monsterStealDuration"]);
			if (m_nMonsterStealDuration <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_76 + "m_nMonsterStealDuration = " + m_nMonsterStealDuration);
			}
			m_nOpenGiftRequiredHeroLevel = Convert.ToInt32(drGameConfig["openGiftRequiredHeroLevel"]);
			if (m_nOpenGiftRequiredHeroLevel <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_77 + "m_nOpenGiftRequiredHeroLevel = " + m_nOpenGiftRequiredHeroLevel);
			}
			m_nOpen7DayEventRequiredMainQuestNo = Convert.ToInt32(drGameConfig["open7DayEventRequiredMainQuestNo"]);
			if (m_nOpen7DayEventRequiredMainQuestNo < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_78 + "m_nOpen7DayEventRequiredMainQuestNo = " + m_nOpen7DayEventRequiredMainQuestNo);
			}
			m_nBountyHunterQuestRequiredHeroLevel = Convert.ToInt32(drGameConfig["bountyHunterQuestRequiredHeroLevel"]);
			if (m_nBountyHunterQuestRequiredHeroLevel < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_79 + "m_nBountyHunterQuestRequiredHeroLevel = " + m_nBountyHunterQuestRequiredHeroLevel);
			}
			m_nTaskConsignmentRequiredVipLevel = Convert.ToInt32(drGameConfig["taskConsignmentRequiredVipLevel"]);
			if (m_nTaskConsignmentRequiredVipLevel < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_80);
			}
			m_nEliteMonsterKillApplicationRequiredHeroLevel = Convert.ToInt32(drGameConfig["eliteMonsterKillApplicationRequiredHeroLevel"]);
			if (m_nEliteMonsterKillApplicationRequiredHeroLevel < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_81 + "m_nEliteMonsterKillApplicationRequiredHeroLevel = " + m_nEliteMonsterKillApplicationRequiredHeroLevel);
			}
			m_nWarehouseRequiredVipLevel = Convert.ToInt32(drGameConfig["warehouseRequiredVipLevel"]);
			if (m_nWarehouseRequiredVipLevel < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_82 + "m_nWarehouseRequiredVipLevel = " + m_nWarehouseRequiredVipLevel);
			}
			m_nFreeWarehouseSlotCount = Convert.ToInt32(drGameConfig["freeWarehouseSlotCount"]);
			if (m_nFreeWarehouseSlotCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_83 + "m_nFreeWarehouseSlotCount = " + m_nFreeWarehouseSlotCount);
			}
			m_nWingMemoryPieceInstallationRequiredHeroLevel = Convert.ToInt32(drGameConfig["wingMemoryPieceInstallationRequiredHeroLevel"]);
			if (m_nWingMemoryPieceInstallationRequiredHeroLevel < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_84 + "m_nWingMemoryPieceInstallationRequiredHeroLevel = " + m_nWingMemoryPieceInstallationRequiredHeroLevel);
			}
			m_nOrdealQuestSlotCount = Convert.ToInt32(drGameConfig["ordealQuestSlotCount"]);
			if (m_nOrdealQuestSlotCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_85 + "m_nOrdealQuestSlotCount = " + m_nOrdealQuestSlotCount);
			}
			m_nFriendMaxCount = Convert.ToInt32(drGameConfig["friendMaxCount"]);
			if (m_nFriendMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_86 + "m_nFriendMaxCount = " + m_nFriendMaxCount);
			}
			m_nTempFriendMaxCount = Convert.ToInt32(drGameConfig["tempFriendMaxCount"]);
			if (m_nTempFriendMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_87 + "m_nTempFriendMaxCount = " + m_nTempFriendMaxCount);
			}
			m_nDeadRecordMaxCount = Convert.ToInt32(drGameConfig["deadRecordMaxCount"]);
			if (m_nDeadRecordMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_88 + "m_nDeadRecordMaxCount = " + m_nDeadRecordMaxCount);
			}
			m_nBlacklistEntryMaxCount = Convert.ToInt32(drGameConfig["blacklistEntryMaxCount"]);
			if (m_nBlacklistEntryMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_89 + "m_nBlacklistEntryMaxCount = " + m_nBlacklistEntryMaxCount);
			}
			m_nBlessingQuestListMaxCount = Convert.ToInt32(drGameConfig["blessingQuestListMaxCount"]);
			if (m_nBlessingQuestListMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_90 + "m_nBlessingQuestListMaxCount = " + m_nBlessingQuestListMaxCount);
			}
			m_nBlessingQuestRequiredHeroLevel = Convert.ToInt32(drGameConfig["blessingQuestRequiredHeroLevel"]);
			if (m_nBlessingQuestRequiredHeroLevel < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_91 + "m_nBlessingQuestRequiredHeroLevel = " + m_nBlessingQuestRequiredHeroLevel);
			}
			m_nBlessingListMaxCount = Convert.ToInt32(drGameConfig["blessingListMaxCount"]);
			if (m_nBlessingListMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_92 + "m_nBlessingListMaxCount = " + m_nBlessingListMaxCount);
			}
			m_nOwnerProspectQuestListMaxCount = Convert.ToInt32(drGameConfig["ownerProspectQuestListMaxCount"]);
			if (m_nOwnerProspectQuestListMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_93 + "m_nOwnerProspectQuestListMaxCount = " + m_nOwnerProspectQuestListMaxCount);
			}
			m_nTargetProspectQuestListMaxCount = Convert.ToInt32(drGameConfig["targetProspectQuestListMaxCount"]);
			if (m_nTargetProspectQuestListMaxCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_95 + "m_nTargetProspectQuestListMaxCount = " + m_nTargetProspectQuestListMaxCount);
			}
			m_nCreatureMaxCount = Convert.ToInt32(drGameConfig["creatureMaxCount"]);
			if (m_nCreatureMaxCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_96 + "m_nCreatureMaxCount  = " + m_nCreatureMaxCount);
			}
			m_nCreatureCheerMaxCount = Convert.ToInt32(drGameConfig["creatureCheerMaxCount"]);
			if (m_nCreatureCheerMaxCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_97 + "m_nCreatureCheerMaxCount = " + m_nCreatureCheerMaxCount);
			}
			m_fCreatureCheerAttrFactor = Convert.ToSingle(drGameConfig["creatureCheerAttrFactor"]);
			if (m_fCreatureCheerAttrFactor <= 0f)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_98 + "m_fCreatureCheerAttrFactor = " + m_fCreatureCheerAttrFactor);
			}
			m_fCreatureEvaluationFactor = Convert.ToSingle(drGameConfig["creatureEvaluationFactor"]);
			if (m_fCreatureEvaluationFactor <= 0f)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_99 + "m_fCreatureEvaluationFactor = " + m_fCreatureEvaluationFactor);
			}
			m_nCreatureAdditionalAttrCount = Convert.ToInt32(drGameConfig["creatureAdditionalAttrCount"]);
			if (m_nCreatureAdditionalAttrCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_100 + "m_nCreatureAdditionalAttrCount = " + m_nCreatureAdditionalAttrCount);
			}
			m_nCreatureSkillSlotMaxCount = Convert.ToInt32(drGameConfig["creatureSkillSlotMaxCount"]);
			if (m_nCreatureSkillSlotMaxCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_101 + "m_nCreatureSkillSlotMaxCount = " + m_nCreatureSkillSlotMaxCount);
			}
			m_nCreatureSkillSlotBaseOpenCount = Convert.ToInt32(drGameConfig["creatureSkillSlotBaseOpenCount"]);
			if (m_nCreatureSkillSlotBaseOpenCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_102 + "m_nCreatureSkillSlotBaseOpenCount = " + m_nCreatureSkillSlotBaseOpenCount);
			}
			m_nCreatureCompositionSkillProtectionItemId = Convert.ToInt32(drGameConfig["creatureCompositionSkillProtectionItemId"]);
			if (m_nCreatureCompositionSkillProtectionItemId < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_103 + "m_nCreatureCompositionSkillProtectionItemId = " + m_nCreatureCompositionSkillProtectionItemId);
			}
			m_nCreatureInjectionExpRetrievalRate = Convert.ToInt32(drGameConfig["creatureInjectionExpRetrievalRate"]);
			if (m_nCreatureInjectionExpRetrievalRate < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_104 + "m_nCreatureInjectionExpRetrievalRate = " + m_nCreatureInjectionExpRetrievalRate);
			}
			m_nCreatureVariationRequiredItemId = Convert.ToInt32(drGameConfig["creatureVariationRequiredItemId"]);
			if (m_nCreatureVariationRequiredItemId < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_105 + "m_nCreatureVariationRequiredItemId = " + m_nCreatureVariationRequiredItemId);
			}
			m_nCreatureAdditionalAttrSwitchRequiredItemId = Convert.ToInt32(drGameConfig["creatureAdditionalAttrSwitchRequiredItemId"]);
			if (m_nCreatureAdditionalAttrSwitchRequiredItemId < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_106 + "m_nCreatureAdditionalAttrSwitchRequiredItemId = " + m_nCreatureAdditionalAttrSwitchRequiredItemId);
			}
			m_nCreatureReleaseExpRetrievalRate = Convert.ToInt32(drGameConfig["creatureReleaseExpRetrievalRate"]);
			if (m_nCreatureReleaseExpRetrievalRate < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_107 + "m_nCreatureReleaseExpRetrievalRate = " + m_nCreatureReleaseExpRetrievalRate);
			}
			int nGuildBlessingGuildTerritoryNpcId = Convert.ToInt32(drGameConfig["guildBlessingGuildTerritoryNpcId"]);
			long lnOpen7DayEventCostumeItemRewardId = Convert.ToInt64(drGameConfig["open7DayEventCostumeItemRewardId"]);
			m_nOpen7DayEventCostumeRewardRequiredItemId = Convert.ToInt32(drGameConfig["open7DayEventCostumeRewardRequiredItemId"]);
			if (m_nOpen7DayEventCostumeRewardRequiredItemId <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_108 + "m_nOpen7DayEventCostumeRewardRequiredItemId = " + m_nOpen7DayEventCostumeRewardRequiredItemId);
			}
			m_nOpen7DayEventCostumeRewardRequiredItemCount = Convert.ToInt32(drGameConfig["open7DayEventCostumeRewardRequiredItemCount"]);
			if (m_nOpen7DayEventCostumeRewardRequiredItemCount <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_109 + "m_nOpen7DayEventCostumeRewardRequiredItemCount = " + m_nOpen7DayEventCostumeRewardRequiredItemCount);
			}
			m_nNationAllianceUnavailableStartTime = Convert.ToInt32(drGameConfig["nationAllianceUnavailableStartTime"]);
			if (m_nNationAllianceUnavailableStartTime < 0 || m_nNationAllianceUnavailableStartTime >= 86400)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_110 + "m_nNationAllianceUnavailableStartTime = " + m_nNationAllianceUnavailableStartTime);
			}
			m_nNationAllianceUnavailableEndTime = Convert.ToInt32(drGameConfig["nationAllianceUnavailableEndTime"]);
			if (m_nNationAllianceUnavailableEndTime < 0 || m_nNationAllianceUnavailableEndTime >= 86400)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_111 + "m_nNationAllianceUnavailableEndTime = " + m_nNationAllianceUnavailableEndTime);
			}
			if (m_nNationAllianceUnavailableStartTime >= m_nNationAllianceUnavailableEndTime)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_112 + "m_nNationAllianceUnavailableStartTime = " + m_nNationAllianceUnavailableStartTime + ", m_nNationAllianceUnavailableEndTime = " + m_nNationAllianceUnavailableEndTime);
			}
			m_nNationAllianceRequiredFund = Convert.ToInt32(drGameConfig["nationAllianceRequiredFund"]);
			if (m_nNationAllianceRequiredFund < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_113 + "m_nNationAllianceRequiredFund = " + m_nNationAllianceRequiredFund);
			}
			m_nNationAllianceRenounceUnavailableDuration = Convert.ToInt32(drGameConfig["NationAllianceRenounceUnavailableDuration"]);
			if (m_nNationAllianceRenounceUnavailableDuration < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_114 + " m_nNationAllianceRenounceUnavailableDuration = " + m_nNationAllianceRenounceUnavailableDuration);
			}
			m_nNationBasePower = Convert.ToInt32(drGameConfig["nationBasePower"]);
			if (m_nNationBasePower < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_115 + "m_nNationBasePower = " + m_nNationBasePower);
			}
			m_nJobChangeRequiredHeroLevel = Convert.ToInt32(drGameConfig["jobChangeRequiredHeroLevel"]);
			if (m_nJobChangeRequiredHeroLevel <= 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_116 + "_nJobChangeRequiredHeroLevel = " + m_nJobChangeRequiredHeroLevel);
			}
			m_nJobChangeRequiredItemId = Convert.ToInt32(drGameConfig["jobChangeRequiredItemId"]);
			if (m_nJobChangeRequiredItemId < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_117 + "m_nJobChangeRequiredItemId = " + m_nJobChangeRequiredItemId);
			}
			int nCostumeEnchantItemId = Convert.ToInt32(drGameConfig["costumeEnchantItemId"]);
			m_nCostumeCollectionActivationItemId = Convert.ToInt32(drGameConfig["costumeCollectionActivationItemId"]);
			if (m_nCostumeCollectionActivationItemId < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_118 + "m_nCostumeCollectionActivationItemId = " + m_nCostumeCollectionActivationItemId);
			}
			m_nCostumeCollectionShuffleItemId = Convert.ToInt32(drGameConfig["costumeCollectionShuffleItemId"]);
			if (m_nCostumeCollectionShuffleItemId < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_119 + "m_nCostumeCollectionShuffleItemId = " + m_nCostumeCollectionShuffleItemId);
			}
			m_nCostumeCollectionShuffleItemCount = Convert.ToInt32(drGameConfig["costumeCollectionShuffleItemCount"]);
			if (m_nCostumeCollectionShuffleItemCount < 0)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_120 + "m_nCostumeCollectionShuffleItemCount = " + m_nCostumeCollectionShuffleItemCount);
			}
			foreach (DataRow dr28 in UserDac.BanWords(conn, null))
			{
				int nType = Convert.ToInt32(dr28["type"]);
				if (!Enum.IsDefined(typeof(BanWordType), nType))
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_121 + "nType = " + nType);
					continue;
				}
				BanWordType type = (BanWordType)nType;
				string sWord = Convert.ToString(dr28["word"]);
				switch (type)
				{
				case BanWordType.Name:
					m_nameBanWords.Add(sWord);
					break;
				case BanWordType.Chat:
					m_chatBanWords.Add(sWord);
					break;
				}
			}
			foreach (string word in m_nameBanWords)
			{
				m_nameBanWordTextFilter.AddElement(word);
			}
			foreach (string word2 in m_chatBanWords)
			{
				m_chatBanWordTextFilter.AddElement(word2);
			}
			DataRowCollection drcNations = UserDac.Nations(conn, null);
			foreach (DataRow dr39 in drcNations)
			{
				Nation nation = new Nation();
				nation.Set(dr39);
				m_nations.Add(nation.id, nation);
			}
			foreach (DataRow dr42 in UserDac.Attrs(conn, null))
			{
				Attr attr = new Attr();
				attr.Set(dr42);
				m_attrs.Add(attr.id, attr);
			}
			foreach (DataRow dr45 in UserDac.AttrValues(conn, null))
			{
				AttrValue attrValue = new AttrValue();
				attrValue.Set(dr45);
				AddAttrValue(attrValue);
			}
			foreach (DataRow dr48 in UserDac.ExpRewards(conn, null))
			{
				ExpReward reward8 = new ExpReward();
				reward8.Set(dr48);
				AddExpReward(reward8);
			}
			foreach (DataRow dr51 in UserDac.GoldRewards(conn, null))
			{
				GoldReward reward9 = new GoldReward();
				reward9.Set(dr51);
				AddGoldReward(reward9);
			}
			foreach (DataRow dr55 in UserDac.ExploitPointRewards(conn, null))
			{
				ExploitPointReward reward10 = new ExploitPointReward();
				reward10.Set(dr55);
				AddExploitPointReward(reward10);
			}
			foreach (DataRow dr59 in UserDac.HonorPointRewards(conn, null))
			{
				HonorPointReward reward12 = new HonorPointReward();
				reward12.Set(dr59);
				AddHonorPointReward(reward12);
			}
			foreach (DataRow dr63 in UserDac.NationFundRewards(conn, null))
			{
				NationFundReward reward13 = new NationFundReward();
				reward13.Set(dr63);
				AddNationFundReward(reward13);
			}
			foreach (DataRow dr67 in UserDac.OwnDiaRewards(conn, null))
			{
				OwnDiaReward reward15 = new OwnDiaReward();
				reward15.Set(dr67);
				AddOwnDiaReward(reward15);
			}
			foreach (DataRow dr71 in UserDac.GuildContributionPointRewards(conn, null))
			{
				GuildContributionPointReward reward18 = new GuildContributionPointReward();
				reward18.Set(dr71);
				AddGuildContributionPointReward(reward18);
			}
			foreach (DataRow dr70 in UserDac.GuildFundRewards(conn, null))
			{
				GuildFundReward reward17 = new GuildFundReward();
				reward17.Set(dr70);
				AddGuildFundReward(reward17);
			}
			foreach (DataRow dr69 in UserDac.GuildBuildingPointRewards(conn, null))
			{
				GuildBuildingPointReward reward16 = new GuildBuildingPointReward();
				reward16.Set(dr69);
				AddGuildBuildingPointReward(reward16);
			}
			foreach (DataRow dr68 in UserDac.GuildPointRewards(conn, null))
			{
				GuildPointReward reward14 = new GuildPointReward();
				reward14.Set(dr68);
				AddGuildPointReward(reward14);
			}
			foreach (DataRow dr66 in UserDac.PvpExploits(conn, null))
			{
				PvpExploit pvpExploit = new PvpExploit();
				pvpExploit.Set(dr66);
				m_pvpExploits.Add(pvpExploit);
			}
			foreach (DataRow dr65 in UserDac.ItemGrades(conn, null))
			{
				ItemGrade grade = new ItemGrade();
				grade.Set(dr65);
				m_itemGrades[grade.id - 1] = grade;
			}
			foreach (DataRow dr64 in UserDac.ItemTypes(conn, null))
			{
				ItemType type2 = new ItemType();
				type2.Set(dr64);
				AddItemType(type2);
			}
			foreach (DataRow dr62 in UserDac.Items(conn, null))
			{
				int nItemType = Convert.ToInt32(dr62["itemType"]);
				ItemType itemType = GetItemType(nItemType);
				if (itemType == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_122 + "nItemType = " + nItemType);
					continue;
				}
				Item item3 = new Item(itemType);
				item3.Set(dr62);
				AddItem(item3);
				itemType.AddItem(item3);
			}
			ItemType creatureFeedItemType = GetItemType(37);
			if (creatureFeedItemType == null)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_123);
			}
			else
			{
				m_creatureFeeds = new Item[creatureFeedItemType.items.Count];
				m_creatureFeeds = creatureFeedItemType.items.Values.OrderByDescending((Item v) => v.value1).ToArray();
			}
			ItemType creatureEssence = GetItemType(38);
			if (creatureEssence == null)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_124);
			}
			else
			{
				m_creatureInjectionExpItem = creatureEssence.items.Values.FirstOrDefault();
			}
			m_mountLevelUpItem = GetItem(nMountLevelUpItemId);
			if (m_mountLevelUpItem == null)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_125 + "nMountLevelUpItemId = " + nMountLevelUpItemId);
			}
			m_mountAwakeningItem = GetItem(nMountAwakeningItemId);
			if (m_mountAwakeningItem == null)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_126 + "nMountAwakeningItemId = " + nMountAwakeningItemId);
			}
			Item dungeonSweepItem = GetItem(m_nDungeonSweepItemId);
			if (dungeonSweepItem == null)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_127 + " m_nDungeonSweepItemId = " + m_nDungeonSweepItemId);
			}
			else if (dungeonSweepItem.type.id != 10)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_128 + "m_nDungeonSweepItemId = " + m_nDungeonSweepItemId);
			}
			m_guildHuntingDonationItem = GetItem(nGuildHuntingDonationItemId);
			if (m_guildHuntingDonationItem == null)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_129 + "nGuildHuntingDonationItemId = " + nGuildHuntingDonationItemId);
			}
			m_costumeEnchantItem = GetItem(nCostumeEnchantItemId);
			if (m_costumeEnchantItem == null)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_130 + "nCostumeEnchantItemId = " + nCostumeEnchantItemId);
			}
			foreach (DataRow dr61 in UserDac.ItemCompositionRecipes(conn, null))
			{
				ItemCompositionRecipe recipe2 = new ItemCompositionRecipe();
				recipe2.Set(dr61);
				AddItemCompositionRecipe(recipe2);
			}
			foreach (DataRow dr60 in UserDac.ItemRewards(conn, null))
			{
				ItemReward reward11 = new ItemReward();
				reward11.Set(dr60);
				AddItemReward(reward11);
			}
			m_guildHuntingDonationReward = GetItemReward(lnGuildHuntingDonationRewardId);
			if (m_guildHuntingDonationItem == null)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_131 + "lnGuildHuntingDonationRewardId = " + lnGuildHuntingDonationRewardId);
			}
			m_guildHuntingDonationCompletionReward = GetItemReward(lnGuildHuntingDonationCompletionItemRewardId);
			if (m_guildHuntingDonationCompletionReward == null)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_132 + "lnGuildHuntingDonationCompletionItemRewardId = " + lnGuildHuntingDonationCompletionItemRewardId);
			}
			m_open7DayEventCostumeItemReward = GetItemReward(lnOpen7DayEventCostumeItemRewardId);
			if (m_open7DayEventCostumeItemReward == null)
			{
				SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_133 + "lnOpen7DayEventCostumeItemRewardId = " + lnOpen7DayEventCostumeItemRewardId);
			}
			foreach (DataRow dr58 in UserDac.Jobs(conn, null))
			{
				Job job9 = new Job();
				job9.Set(dr58);
				m_jobs.Add(job9.id, job9);
			}
			foreach (DataRow dr57 in UserDac.AbnormalStates(conn, null))
			{
				AbnormalState abnormalState3 = new AbnormalState();
				abnormalState3.Set(dr57);
				m_abnormalStates.Add(abnormalState3.id, abnormalState3);
			}
			foreach (DataRow dr56 in UserDac.AbnormalStateLevels(conn, null))
			{
				int nAbnormalStateId2 = Convert.ToInt32(dr56["abnormalStateId"]);
				AbnormalState abnormalState2 = GetAbnormalState(nAbnormalStateId2);
				if (abnormalState2 == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_134 + "nAbnormalStateId = " + nAbnormalStateId2);
					continue;
				}
				if (abnormalState2.sourceType != 1)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_135 + "nAbnormalStateId = " + nAbnormalStateId2);
					continue;
				}
				int nJobId8 = Convert.ToInt32(dr56["jobId"]);
				Job job8 = GetJob(nJobId8);
				if (job8 == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_136 + "nJobId = " + nJobId8);
					continue;
				}
				JobAbnormalState jobAbnormalState = abnormalState2.GetOrCreateJobAbnormalState(job8);
				AbnormalStateJobSkillLevel abnormalStateJobSkillLevel = new AbnormalStateJobSkillLevel(jobAbnormalState);
				abnormalStateJobSkillLevel.Set(dr56);
				jobAbnormalState.AddAbnormalStateJobSkillLevel(abnormalStateJobSkillLevel);
			}
			foreach (DataRow dr54 in UserDac.AbnormalStateRankSkillLevels(conn, null))
			{
				int nAbnormalStateId = Convert.ToInt32(dr54["abnormalStateId"]);
				AbnormalState abnormalState = GetAbnormalState(nAbnormalStateId);
				if (abnormalState == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_137 + "nAbnormalStateId = " + nAbnormalStateId);
					continue;
				}
				if (abnormalState.sourceType != 2)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_138 + "nAbnormalStateId = " + nAbnormalStateId);
					continue;
				}
				AbnormalStateRankSkillLevel abnormalStateRankSkillLevel = new AbnormalStateRankSkillLevel(abnormalState);
				abnormalStateRankSkillLevel.Set(dr54);
				abnormalState.AddAbnormalStateRankSkillLevel(abnormalStateRankSkillLevel);
			}
			foreach (DataRow dr53 in UserDac.JobSkillMasters(conn, null))
			{
				JobSkillMaster master2 = new JobSkillMaster();
				master2.Set(dr53);
				AddJobSkillMaster(master2);
			}
			foreach (DataRow dr52 in UserDac.JobSkillLevelMasters(conn, null))
			{
				JobSkillLevelMaster levelMaster2 = new JobSkillLevelMaster();
				levelMaster2.Set(dr52);
				int nSkillId3 = Convert.ToInt32(dr52["skillId"]);
				JobSkillMaster skillMaster2 = GetJobSkillMaster(nSkillId3);
				if (skillMaster2 == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_139 + "nSkillId = " + nSkillId3);
				}
				else
				{
					skillMaster2.AddLevel(levelMaster2);
				}
			}
			foreach (DataRow dr50 in UserDac.JobSkills(conn, null))
			{
				int nSkillId2 = Convert.ToInt32(dr50["skillId"]);
				JobSkillMaster skillMaster = GetJobSkillMaster(nSkillId2);
				if (skillMaster == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_140 + " nSkillId = " + nSkillId2);
					continue;
				}
				JobSkill jobSkill6 = new JobSkill(skillMaster);
				jobSkill6.Set(dr50);
				int nJobId7 = Convert.ToInt32(dr50["jobId"]);
				Job job7 = GetJob(nJobId7);
				if (job7 == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_141 + "nJobId = " + nJobId7);
				}
				else
				{
					job7.AddSkill(jobSkill6);
				}
			}
			foreach (DataRow dr49 in UserDac.JobskillLevels(conn, null))
			{
				JobSkillLevel level4 = new JobSkillLevel();
				level4.Set(dr49);
				int nJobId6 = Convert.ToInt32(dr49["jobId"]);
				Job job6 = GetJob(nJobId6);
				if (job6 == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_142 + "nJobId = " + nJobId6);
					continue;
				}
				int nJobSkillId4 = Convert.ToInt32(dr49["skillId"]);
				JobSkill jobSkill5 = job6.GetSkill(nJobSkillId4);
				if (jobSkill5 == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_143 + "nJobSkillId = " + nJobSkillId4);
					continue;
				}
				jobSkill5.AddLevel(level4);
				int nLevel2 = Convert.ToInt32(dr49["level"]);
				JobSkillLevelMaster levelMaster = jobSkill5.skillMaster.GetLevel(nLevel2);
				if (levelMaster == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_144 + "nJobSkillId = " + nJobSkillId4 + ", level = " + nLevel2);
				}
				else
				{
					level4.levelMaster = levelMaster;
				}
			}
			foreach (DataRow dr47 in UserDac.JobSkillHits(conn, null))
			{
				JobSkillHit jobSkillHit2 = new JobSkillHit();
				jobSkillHit2.Set(dr47);
				int nJobId5 = Convert.ToInt32(dr47["jobId"]);
				Job job5 = GetJob(nJobId5);
				if (job5 == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_145 + "nJobId = " + nJobId5);
					continue;
				}
				int nJobSkillId3 = Convert.ToInt32(dr47["skillId"]);
				JobSkill jobSkill4 = job5.GetSkill(nJobSkillId3);
				if (jobSkill4 == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_146 + " nJobSkillId = " + nJobSkillId3);
				}
				else
				{
					jobSkill4.AddJobSkillHit(jobSkillHit2);
				}
			}
			foreach (DataRow dr46 in UserDac.JobSkillHitAbnormalStates(conn, null))
			{
				JobSkillHitAbnormalState jobSkillHitAbnormalState = new JobSkillHitAbnormalState();
				jobSkillHitAbnormalState.Set(dr46);
				int nJobId4 = Convert.ToInt32(dr46["jobId"]);
				Job job4 = GetJob(nJobId4);
				if (job4 == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_147 + "nJobId = " + nJobId4);
					continue;
				}
				int nSkillId = Convert.ToInt32(dr46["skillId"]);
				JobSkill jobSkill3 = job4.GetSkill(nSkillId);
				if (jobSkill3 == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_148 + "nJobId = " + nJobId4 + ", nSkillId = " + nSkillId);
					continue;
				}
				int nHitId = Convert.ToInt32(dr46["hitId"]);
				JobSkillHit jobSkillHit = jobSkill3.GetJobSkillHit(nHitId);
				if (jobSkillHit == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_149 + "nJobId = " + nJobId4 + ", nSkillId = " + nSkillId + ", nHitId = " + nHitId);
				}
				else
				{
					jobSkillHit.AddAbnormalState(jobSkillHitAbnormalState);
				}
			}
			foreach (DataRow dr44 in UserDac.JobChainSkills(conn, null))
			{
				JobChainSkill jobChainSkill2 = new JobChainSkill();
				jobChainSkill2.Set(dr44);
				int nJobId3 = Convert.ToInt32(dr44["jobId"]);
				Job job3 = GetJob(nJobId3);
				if (job3 == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_150 + "nJobId = " + nJobId3);
					continue;
				}
				int nJobSkillId2 = Convert.ToInt32(dr44["skillId"]);
				JobSkill jobSkill2 = job3.GetSkill(nJobSkillId2);
				if (jobSkill2 == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_151 + "nJobSkillId = " + nJobSkillId2);
				}
				else
				{
					jobSkill2.AddChainSkill(jobChainSkill2);
				}
			}
			foreach (DataRow dr43 in UserDac.JobChainSkillHits(conn, null))
			{
				JobChainSkillHit jobChainSkillHit = new JobChainSkillHit();
				jobChainSkillHit.Set(dr43);
				int nJobId2 = Convert.ToInt32(dr43["jobId"]);
				Job job2 = GetJob(nJobId2);
				if (job2 == null)
				{
					SFLogUtil.Warn(GetType(), Resources.Exception.Resource_LoadUserDBResources_152 + "nJobId = " + nJobId2);
					continue;
				}
				int nJobSkillId = Convert.ToInt32(dr43["skillId"]);
				JobSkill jobSkill = job2.GetSkill(nJobSkillId);
				if (jobSkill == null)
				{
					SFLogUtil.Warn(GetType(), "[직업연계스킬적중 목록] 직업스킬이 존재하지 않습니다. nJobSkillId = " + nJobSkillId);
					continue;
				}
				int nJobChainSkillId = Convert.ToInt32(dr43["chainSkillId"]);
				JobChainSkill jobChainSkill = jobSkill.GetChainSkill(nJobChainSkillId);
				if (jobChainSkill == null)
				{
					SFLogUtil.Warn(GetType(), "[직업연계스킬적중 목록] 직업연계스킬이 존재하지 않습니다. nJobChainSkillId = " + nJobChainSkillId);
				}
				else
				{
					jobChainSkill.AddJobChainSkillHit(jobChainSkillHit);
				}
			}
			foreach (DataRow dr41 in UserDac.JobLevelMasters(conn, null))
			{
				JobLevelMaster master = new JobLevelMaster();
				master.Set(dr41);
				AddJobLevelMaster(master);
			}
			foreach (DataRow dr40 in UserDac.JobLevels(conn, null))
			{
				JobLevel level3 = new JobLevel();
				level3.Set(dr40);
				int nJobId = Convert.ToInt32(dr40["jobId"]);
				Job job = GetJob(nJobId);
				if (job == null)
				{
					SFLogUtil.Warn(GetType(), "[직업레벨 목록] 직업이 존재하지 않습니다. nJobId = " + nJobId + ", level = " + level3.level);
				}
				else
				{
					job.AddLevel(level3);
				}
			}
			LoadUserDBResources_Rank(conn);
			LoadUserDBResources_MainGear(conn);
			LoadUserDBResources_SubGear(conn);
			LoadUserDBResources_Monster(conn);
			LoadUserDBResources_CreatureCard(conn);
			LoadUserDBResources_CreatureCardCollection(conn);
			LoadUserDBResources_CreatureCardShop(conn);
			LoadUserDBResources_Creature(conn);
			LoadUserDBResources_Place(conn);
			LoadUserDBResources_Mount(conn);
			LoadUserDBResources_Cart(conn);
			foreach (DataRow dr38 in UserDac.MainQuests(conn, null))
			{
				MainQuest mainQuest2 = new MainQuest();
				mainQuest2.Set(dr38);
				AddMainQuest(mainQuest2);
			}
			foreach (DataRow dr37 in UserDac.MainQuestRewards(conn, null))
			{
				MainQuestReward reward7 = new MainQuestReward();
				reward7.Set(dr37);
				int nMainQuestNo = Convert.ToInt32(dr37["mainQuestNo"]);
				MainQuest mainQuest = GetMainQuest(nMainQuestNo);
				if (mainQuest == null)
				{
					SFLogUtil.Warn(GetType(), "[메인퀘스트보상 목록] 메인퀘스트가 존재하지 않습니다. nMainQuestNo = " + nMainQuestNo);
				}
				else
				{
					mainQuest.AddReward(reward7);
				}
			}
			foreach (DataRow dr36 in UserDac.SimpleShopProducts(conn, null))
			{
				SimpleShopProduct shop = new SimpleShopProduct();
				shop.Set(dr36);
				AddSimpleShop(shop);
			}
			foreach (DataRow dr35 in UserDac.PaidImmediateRevivals(conn, null))
			{
				PaidImmediateRevival paidImmediateRevival = new PaidImmediateRevival();
				paidImmediateRevival.Set(dr35);
				m_paidImmediateRevivals.Add(paidImmediateRevival);
			}
			foreach (DataRow dr34 in UserDac.InventorySlotExtendRecipe(conn, null))
			{
				InventorySlotExtendRecipe recipe = new InventorySlotExtendRecipe();
				recipe.Set(dr34);
				AddInventorySlotExtendRecipe(recipe);
			}
			foreach (DataRow dr33 in UserDac.RestRewardTimes(conn, null))
			{
				RestRewardTime cost = new RestRewardTime();
				cost.Set(dr33);
				AddRestRewardTime(cost);
			}
			foreach (DataRow dr32 in UserDac.PickPoolEntries(conn, null))
			{
				int nPoolId = Convert.ToInt32(dr32["poolId"]);
				PickPool pool = GetPickPool(nPoolId);
				if (pool == null)
				{
					pool = new PickPool(nPoolId);
					m_pickPools.Add(nPoolId, pool);
				}
				PickPoolEntry entry11 = new PickPoolEntry();
				entry11.Set(dr32);
				pool.AddEntry(entry11);
			}
			foreach (DataRow dr31 in UserDac.LevelUpRewardEntries(conn, null))
			{
				LevelUpRewardEntry entry10 = new LevelUpRewardEntry();
				entry10.Set(dr31);
				AddLevelUpRewardEntry(entry10);
			}
			foreach (DataRow dr30 in UserDac.LevelUpRewardItems(conn, null))
			{
				LevelUpRewardItem item2 = new LevelUpRewardItem();
				item2.Set(dr30);
				int nEntryId2 = Convert.ToInt32(dr30["entryId"]);
				LevelUpRewardEntry entry9 = GetLevelUpRewardEntry(nEntryId2);
				if (entry9 == null)
				{
					SFLogUtil.Warn(GetType(), "[레벨업보상아이템 목록] 레벨업보상항목이 존재하지 않습니다. nEntryId = " + nEntryId2);
				}
				else
				{
					entry9.AddRewardItem(item2);
				}
			}
			foreach (DataRow dr29 in UserDac.DailyAttendRewardEntries(conn, null))
			{
				DailyAttendRewardEntry entry8 = new DailyAttendRewardEntry();
				entry8.Set(dr29);
				AddDailyAttendRewardEntry(entry8);
			}
			foreach (DataRow dr27 in UserDac.WeekendAttendRewardAvailableDaysOfWeek(conn, null))
			{
				int nDayOfWeek = Convert.ToInt32(dr27["dayOfWeek"]);
				if (!Enum.IsDefined(typeof(DayOfWeek), nDayOfWeek))
				{
					SFLogUtil.Warn(GetType(), "[주말출석보상가능요일 목록] 유효한 요일이 아닙니다. nDayOfWeek = " + nDayOfWeek);
				}
				m_weekendAttendRewardAvailableDaysOfWeek.Add((DayOfWeek)nDayOfWeek);
			}
			foreach (DataRow dr26 in UserDac.AccessRewardEntries(conn, null))
			{
				AccessRewardEntry entry7 = new AccessRewardEntry();
				entry7.Set(dr26);
				AddAccessRewardEntry(entry7);
			}
			foreach (DataRow dr25 in UserDac.AccessRewardItems(conn, null))
			{
				AccessRewardItem item = new AccessRewardItem();
				item.Set(dr25);
				int nEntryId = Convert.ToInt32(dr25["entryId"]);
				AccessRewardEntry entry6 = GetAccessRewardEntry(nEntryId);
				if (entry6 == null)
				{
					SFLogUtil.Warn(GetType(), "[접속보상아이템 목록] 접속보상항목이 존재하지 않습니다. nEntryId = " + nEntryId);
				}
				else
				{
					entry6.AddRewardItem(item);
				}
			}
			foreach (DataRow dr24 in UserDac.SeriesMissions(conn, null))
			{
				SeriesMission mission3 = new SeriesMission();
				mission3.Set(dr24);
				AddSeriesMission(mission3);
			}
			foreach (DataRow dr23 in UserDac.SeriesMissionSteps(conn, null))
			{
				SeriesMissionStep step5 = new SeriesMissionStep();
				step5.Set(dr23);
				int nMissionId2 = Convert.ToInt32(dr23["missionId"]);
				SeriesMission mission2 = GetSeriesMission(nMissionId2);
				if (mission2 == null)
				{
					SFLogUtil.Warn(GetType(), "[연속미션단계 목록] 미션이 존재하지 않습니다. nMissionId = " + nMissionId2);
				}
				else
				{
					mission2.AddStep(step5);
				}
			}
			foreach (DataRow dr22 in UserDac.SeriesMissionStepRewards(conn, null))
			{
				SeriesMissionStepReward reward6 = new SeriesMissionStepReward();
				reward6.Set(dr22);
				int nMissionId = Convert.ToInt32(dr22["missionId"]);
				SeriesMission mission = GetSeriesMission(nMissionId);
				if (mission == null)
				{
					SFLogUtil.Warn(GetType(), "[연속미션단계보상 목록] 미션이 존재하지 않습니다. nMissionId = " + nMissionId);
					continue;
				}
				int nStep3 = Convert.ToInt32(dr22["step"]);
				SeriesMissionStep step4 = mission.GetStep(nStep3);
				if (step4 == null)
				{
					SFLogUtil.Warn(GetType(), "[연속미션단계보상 목록] 단계가 존재하지 않습니다. nMissionId = " + nMissionId + ", nStep = " + nStep3);
				}
				else
				{
					step4.AddReward(reward6);
				}
			}
			LoadUserDBResources_TodayMission(conn);
			foreach (DataRow dr21 in UserDac.AttainmentEntries(conn, null))
			{
				AttainmentEntry entry5 = new AttainmentEntry();
				entry5.Set(dr21);
				AddAttainmentEntry(entry5);
			}
			foreach (DataRow dr20 in UserDac.AttainmentEntryRewards(conn, null))
			{
				int nEntryNo = Convert.ToInt32(dr20["entryNo"]);
				AttainmentEntry entry4 = GetAttainmentEntry(nEntryNo);
				if (entry4 == null)
				{
					SFLogUtil.Warn(GetType(), "[도달항목보상 목록] 도달항목이 존재하지 않습니다. nEntryNo = " + nEntryNo);
					continue;
				}
				AttainmentEntryReward reward5 = new AttainmentEntryReward(entry4);
				reward5.Set(dr20);
				entry4.AddReward(reward5);
			}
			foreach (DataRow dr19 in UserDac.VipLevels(conn, null))
			{
				VipLevel vipLevel = new VipLevel();
				vipLevel.Set(dr19);
				m_vipLevels.Add(vipLevel);
			}
			foreach (DataRow dr18 in UserDac.VipLevelRewards(conn, null))
			{
				int nLevel = Convert.ToInt32(dr18["vipLevel"]);
				VipLevel level2 = GetVipLevel(nLevel);
				if (level2 == null)
				{
					SFLogUtil.Warn(GetType(), "[VIP레벨보상 목록] VIP 레벨이 존재하지 않습니다. nLevel = " + nLevel);
					continue;
				}
				VipLevelReward reward4 = new VipLevelReward(level2);
				reward4.Set(dr18);
				level2.AddReward(reward4);
			}
			foreach (DataRow dr17 in UserDac.Elementals(conn, null))
			{
				Elemental elemental = new Elemental();
				elemental.Set(dr17);
				AddElemental(elemental);
			}
			foreach (DataRow dr16 in UserDac.Wings(conn, null))
			{
				Wing wing = new Wing();
				wing.Set(dr16);
				AddWing(wing);
			}
			foreach (DataRow dr15 in UserDac.WingParts(conn, null))
			{
				WingPart part2 = new WingPart();
				part2.Set(dr15);
				AddWingPart(part2);
			}
			foreach (DataRow dr14 in UserDac.WingSteps(conn, null))
			{
				WingStep step3 = new WingStep();
				step3.Set(dr14);
				AddWingStep(step3);
			}
			foreach (DataRow dr12 in UserDac.WingStepParts(conn, null))
			{
				WingStepPart part = new WingStepPart();
				part.Set(dr12);
				int nStep2 = Convert.ToInt32(dr12["step"]);
				WingStep step2 = GetWingStep(nStep2);
				if (step2 == null)
				{
					SFLogUtil.Warn(GetType(), "[날개단계파트 목록] 날개단계가 존재하지 않습니다. nStep = " + nStep2);
				}
				else
				{
					step2.AddPart(part);
				}
			}
			foreach (DataRow dr11 in UserDac.WingStepLevels(conn, null))
			{
				WingStepLevel level = new WingStepLevel();
				level.Set(dr11);
				int nStep = Convert.ToInt32(dr11["step"]);
				WingStep step = GetWingStep(nStep);
				if (step == null)
				{
					SFLogUtil.Warn(GetType(), "[날개단계레벨 목록] 날개단계가 존재하지 않습니다. nStep = " + nStep);
				}
				else
				{
					step.AddLevel(level);
				}
			}
			foreach (DataRow dr10 in UserDac.WingEnchantCountPoolEntries(conn, null))
			{
				WingEnchantCountPoolEntry entry3 = new WingEnchantCountPoolEntry();
				entry3.Set(dr10);
				m_wingEnchantCountPool.AddEntry(entry3);
			}
			LoadUserDBResources_TreatOfFarmQuest(conn);
			LoadUserDBResources_BountyHunterQuest(conn);
			LoadUserDBResources_FishingQuest(conn);
			LoadUserDBResources_MysteryBoxQuest(conn);
			LoadUserDBResources_SecretLetterQuest(conn);
			LoadUserDBResources_DimensionRaidQuest(conn);
			LoadUserDBResources_HolyWarQuest(conn);
			LoadUserDBResources_SupplySupportQuest(conn);
			LoadUserDBResources_DimensionInfiltrationEvent(conn);
			LoadUserDBResources_BattlefieldSupportEvent(conn);
			LoadUserDBResources_SafeTimeEvent(conn);
			foreach (DataRow dr8 in UserDac.TodayTasks(conn, null))
			{
				TodayTask task = new TodayTask();
				task.Set(dr8);
				AddTodayTask(task);
			}
			foreach (DataRow dr7 in UserDac.AchievementRewards(conn, null))
			{
				AchievementReward reward3 = new AchievementReward();
				reward3.Set(dr7);
				AddAchivementReward(reward3);
			}
			foreach (DataRow dr6 in UserDac.AchievementRewardEntries(conn, null))
			{
				int nNo = Convert.ToInt32(dr6["rewardNo"]);
				AchievementReward reward2 = GetAchivementReward(nNo);
				if (reward2 == null)
				{
					SFLogUtil.Warn(GetType(), "[달성보상항목 목록] 달성보상이 존재하지 않습니다. nNo = " + nNo);
					continue;
				}
				AchievementRewardEntry entry2 = new AchievementRewardEntry(reward2);
				entry2.Set(dr6);
				reward2.AddEntry(entry2);
			}
			LoadUserDBResources_HonorShop(conn);
			foreach (DataRow dr5 in UserDac.LevelRankingRewards(conn, null))
			{
				LevelRankingReward reward = new LevelRankingReward();
				reward.Set(dr5);
				AddLevelRankingReward(reward);
			}
			LoadUserDBResources_Guild(conn);
			LoadUserDBResources_GuildTerritory(conn);
			LoadUserDBResources_GuildFarmQuest(conn);
			LoadUserDBResources_GuildFoodWarehouse(conn);
			LoadUserDBResources_GuildAltar(conn);
			LoadUserDBResources_GuildMissionQuest(conn);
			LoadUserDBResources_GuildSupplySupportQuest(conn);
			LoadUserDBResources_GuildHuntingQuest(conn);
			LoadUserDBResources_GuildTodayObjective(conn);
			LoadUserDBResources_GuildBlessingBuff(conn);
			m_guildBlessingGuildTerritoryNpc = GetGuildTerritoryNpc(nGuildBlessingGuildTerritoryNpcId);
			if (m_guildBlessingGuildTerritoryNpc == null)
			{
				SFLogUtil.Warn(GetType(), "[게임구성] 길드축복길드영지NPC가 존재하지 않습니다. nGuildBlessingGuildTerritoryNpcId = " + nGuildBlessingGuildTerritoryNpcId);
			}
			foreach (DataRow dr4 in UserDac.NationDonationEntries(conn, null))
			{
				NationDonationEntry entry = new NationDonationEntry();
				entry.Set(dr4);
				AddNationDonationEntry(entry);
			}
			LoadUserDBResources_NationNoblesse(conn);
			LoadUserDBResources_NationWar(conn);
			LoadUserDBResources_IllustratedBook(conn);
			foreach (DataRow dr3 in UserDac.SceneryQuests(conn, null))
			{
				SceneryQuest quest = new SceneryQuest();
				quest.Set(dr3);
				AddSceneryQuest(quest);
			}
			foreach (DataRow dr2 in UserDac.Accomplishments(conn, null))
			{
				Accomplishment accomplishment = new Accomplishment();
				accomplishment.Set(dr2);
				AddAccomplishment(accomplishment);
			}
			LoadUserDBResources_Title(conn);
			LoadUserDBResources_EliteMonster(conn);
			DataRow drEliteDungeon = UserDac.EliteDungeon(conn, null);
			if (drEliteDungeon != null)
			{
				EliteDungeon eliteDungeon = new EliteDungeon();
				eliteDungeon.Set(drEliteDungeon);
				m_eliteDungeon = eliteDungeon;
			}
			LoadUserDBResources_StaminaRecovery(conn);
			LoadUserDBResources_ExpFactor(conn);
			foreach (DataRow dr in UserDac.JobCommonSkills(conn, null))
			{
				JobCommonSkill skill = new JobCommonSkill();
				skill.Set(dr);
				AddJobCommonSkill(skill);
			}
			LoadUserDBResources_NpcShop(conn);
			LoadUserDBResources_RookieGift(conn);
			LoadUserDBResources_OpenGift(conn);
			LoadUserDBResources_DailyQuest(conn);
			LoadUserDBResources_WeeklyQuest(conn);
			LoadUserDBResources_Open7DayEvent(conn);
			LoadUserDBResources_Retrieval(conn);
			LoadUserDBResources_TaskConsignment(conn);
			LoadUserDBResources_TrueHeroQuest(conn);
			LoadUserDBResources_FieldBoss(conn);
			LoadUserDBResources_LimitationGift(conn);
			LoadUserDBResources_WeekendReward(conn);
			LoadUserDBResources_Warehouse(conn);
			LoadUserDBResources_DiaShop(conn);
			LoadUserDBResources_WingMemoryPiece(conn);
			LoadUserDBResources_SubQuest(conn);
			LoadUserDBResources_OrdealQuest(conn);
			LoadUserDBResources_MoneyBuff(conn);
			LoadUserDBResources_Biography(conn);
			LoadUserDBResources_ItemLuckyShop(conn);
			LoadUserDBResources_CreatureCardLuckyShop(conn);
			LoadUserDBResources_Blessing(conn);
			LoadUserDBResources_Present(conn);
			LoadUserDBResources_Costume(conn);
			LoadUserDBResources_CostumeCollections(conn);
			LoadUserDBResources_CreatureFarmQuest(conn);
			LoadUserDBResources_JobChangeQuest(conn);
			LoadUserDBResources_CashProduct(conn);
			LoadUserDBResources_FirstChargeEvent(conn);
			LoadUserDBResources_RechargeEvent(conn);
			LoadUserDBResources_ChargeEvent(conn, currentTime.DateTime);
			LoadUserDBResources_DailyChargeEvent(conn);
			LoadUserDBResources_ConsumeEvent(conn, currentTime.DateTime);
			LoadUserDBResources_DailyConsumeEvent(conn);
			LoadUserDBResources_PotionAttr(conn);
			LoadUserDBResources_Constellation(conn);
			LoadUserDBResources_Artifact(conn, drGameConfig);
			LoadUserDBResources_SystemMessage(conn);
			LoadUserDBResources_TimeDesignationEvent(conn, currentTime.DateTime);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void LoadUserDBResources_DimensionInfiltrationEvent(SqlConnection conn)
	{
		DataRow dr = UserDac.DimensionInfiltrationEvent(conn, null);
		if (dr == null)
		{
			SFLogUtil.Warn(GetType(), "[차원잠입이벤트] 이벤트가 존재하지 않습니다.");
			return;
		}
		m_dimensionInfiltrationEvent = new DimensionInfiltrationEvent();
		m_dimensionInfiltrationEvent.Set(dr);
	}

	private void LoadUserDBResources_BattlefieldSupportEvent(SqlConnection conn)
	{
		DataRow dr = UserDac.BattlefieldSupportEvent(conn, null);
		if (dr == null)
		{
			SFLogUtil.Warn(GetType(), "[전장지원이벤트] 이벤트가 존재하지 않습니다.");
			return;
		}
		m_battlefieldSupportEvent = new BattlefieldSupportEvent();
		m_battlefieldSupportEvent.Set(dr);
	}

	private void LoadUserDBResources_SafeTimeEvent(SqlConnection conn)
	{
		DataRow drSafeTimeEvent = UserDac.SafeTimeEvent(conn, null);
		if (drSafeTimeEvent == null)
		{
			SFLogUtil.Warn(GetType(), "[안전시간이벤트] 이벤트가 존재하지 않습니다.");
			return;
		}
		SafeTimeEvent safeTimeEvent = new SafeTimeEvent();
		safeTimeEvent.Set(drSafeTimeEvent);
		m_safeTimeEvent = safeTimeEvent;
	}

	private void LoadUserDBResources_HonorShop(SqlConnection conn)
	{
		foreach (DataRow dr in UserDac.HonorShopProducts(conn, null))
		{
			HonorShopProduct product = new HonorShopProduct();
			product.Set(dr);
			m_honorShopProducts.Add(product.id, product);
		}
	}

	private void LoadUserDBResources_MysteryBoxQuest(SqlConnection conn)
	{
		DataRow drQuest = UserDac.MysteryBoxQuest(conn, null);
		if (drQuest == null)
		{
			SFLogUtil.Warn(GetType(), "[의문의상자퀘스트] 퀘스트가 존재하지 않습니다.");
			return;
		}
		MysteryBoxQuest quest = new MysteryBoxQuest();
		quest.Set(drQuest);
		m_mysteryBoxQuest = quest;
		foreach (DataRow dr3 in UserDac.MysteryBoxGrades(conn, null))
		{
			MysteryBoxGrade grade = new MysteryBoxGrade();
			grade.Set(dr3);
			m_mysteryBoxQuest.AddBoxGrade(grade);
		}
		foreach (DataRow dr2 in UserDac.MysteryBoxGradePoolEntries(conn, null))
		{
			int nPoolId = Convert.ToInt32(dr2["poolId"]);
			MysteryBoxGradePool pool = m_mysteryBoxQuest.GetOrCreateBoxGradePool(nPoolId);
			MysteryBoxGradePoolEntry entry = new MysteryBoxGradePoolEntry(pool);
			entry.Set(dr2);
			pool.AddEntry(entry);
		}
		foreach (DataRow dr in UserDac.MysteryBoxQuestRewards(conn, null))
		{
			MysteryBoxQuestReward reward = new MysteryBoxQuestReward();
			reward.Set(dr);
			m_mysteryBoxQuest.AddReward(reward);
		}
		m_mysteryBoxQuest.SetLate();
	}

	private void LoadUserDBResources_SecretLetterQuest(SqlConnection conn)
	{
		DataRow drQuest = UserDac.SecretLetterQuest(conn, null);
		if (drQuest == null)
		{
			SFLogUtil.Warn(GetType(), "[밀서퀘스트] 퀘스트가 존재하지 않습니다.");
			return;
		}
		SecretLetterQuest quest = new SecretLetterQuest();
		quest.Set(drQuest);
		m_secretLetterQuest = quest;
		foreach (DataRow dr3 in UserDac.SecretLetterGrades(conn, null))
		{
			SecretLetterGrade grade = new SecretLetterGrade();
			grade.Set(dr3);
			m_secretLetterQuest.AddLetterGrade(grade);
		}
		foreach (DataRow dr2 in UserDac.SecretLetterGradePoolEntries(conn, null))
		{
			int nPoolId = Convert.ToInt32(dr2["poolId"]);
			SecretLetterGradePool pool = m_secretLetterQuest.GetOrCreateLetterGradePool(nPoolId);
			SecretLetterGradePoolEntry entry = new SecretLetterGradePoolEntry(pool);
			entry.Set(dr2);
			pool.AddEntry(entry);
		}
		foreach (DataRow dr in UserDac.SecretLetterQuestRewards(conn, null))
		{
			SecretLetterQuestReward reward = new SecretLetterQuestReward();
			reward.Set(dr);
			m_secretLetterQuest.AddReward(reward);
		}
		m_secretLetterQuest.SetLate();
	}

	private void LoadUserDBResources_DimensionRaidQuest(SqlConnection conn)
	{
		DataRow drQuest = UserDac.DimensionRaidQuest(conn, null);
		if (drQuest == null)
		{
			SFLogUtil.Warn(GetType(), "[차원습격퀘스트] 퀘스트가 존재하지 않습니다.");
			return;
		}
		DimensionRaidQuest quest = new DimensionRaidQuest();
		quest.Set(drQuest);
		m_dimensionRaidQuest = quest;
		foreach (DataRow dr2 in UserDac.DimensionRaidQuestSteps(conn, null))
		{
			DimensionRaidQuestStep step = new DimensionRaidQuestStep();
			step.Set(dr2);
			m_dimensionRaidQuest.AddStep(step);
		}
		foreach (DataRow dr in UserDac.DimensionRaidQuestRewards(conn, null))
		{
			DimensionRaidQuestReward reward = new DimensionRaidQuestReward();
			reward.Set(dr);
			m_dimensionRaidQuest.AddReward(reward);
		}
	}

	private void LoadUserDBResources_HolyWarQuest(SqlConnection conn)
	{
		DataRow drQuest = UserDac.HolyWarQuest(conn, null);
		if (drQuest == null)
		{
			SFLogUtil.Warn(GetType(), "[위대한성전퀘스트] 퀘스트가 존재하지 않습니다.");
			return;
		}
		HolyWarQuest quest = new HolyWarQuest();
		quest.Set(drQuest);
		m_holyWarQuest = quest;
		foreach (DataRow dr3 in UserDac.HolyWarQuestSchedules(conn, null))
		{
			HolyWarQuestSchedule schedule = new HolyWarQuestSchedule();
			schedule.Set(dr3);
			m_holyWarQuest.AddSchedule(schedule);
		}
		foreach (DataRow dr2 in UserDac.HolyWarQuestGloryLevels(conn, null))
		{
			HolyWarQuestGloryLevel level = new HolyWarQuestGloryLevel();
			level.Set(dr2);
			m_holyWarQuest.AddGloryLevel(level);
		}
		foreach (DataRow dr in UserDac.HolyWarQuestRewards(conn, null))
		{
			HolyWarQuestReward reward = new HolyWarQuestReward();
			reward.Set(dr);
			m_holyWarQuest.AddReward(reward);
		}
	}

	private void LoadUserDBResources_SupplySupportQuest(SqlConnection conn)
	{
		DataRow drSupplySupportQuest = UserDac.SupplySupportQuest(conn, null);
		if (drSupplySupportQuest == null)
		{
			SFLogUtil.Warn(GetType(), "보급지원퀘스트가 존재하지 않습니다.");
			return;
		}
		SupplySupportQuest supplySupportQuest = new SupplySupportQuest();
		supplySupportQuest.Set(drSupplySupportQuest);
		m_supplySupportQuest = supplySupportQuest;
		foreach (DataRow dr4 in UserDac.SupplySupportQuestWayPoints(conn, null))
		{
			SupplySupportQuestWayPoint wayPoint = new SupplySupportQuestWayPoint(m_supplySupportQuest);
			wayPoint.Set(dr4);
			m_supplySupportQuest.AddWayPoint(wayPoint);
		}
		foreach (DataRow dr6 in UserDac.SupplySupportQuestCarts(conn, null))
		{
			SupplySupportQuestCart cart2 = new SupplySupportQuestCart(m_supplySupportQuest);
			cart2.Set(dr6);
			m_supplySupportQuest.AddCart(cart2);
		}
		foreach (DataRow dr5 in UserDac.SupplySupportQuestChangeCartPoolEntries(conn, null))
		{
			int nCartId = Convert.ToInt32(dr5["cartId"]);
			SupplySupportQuestCart cart = m_supplySupportQuest.GetCart(nCartId);
			if (cart == null)
			{
				SFLogUtil.Warn(GetType(), "[보급지원퀘스트수레변경풀항목 목록] 보급지원퀘스트카드가 존재하지 않습니다. nCartId = " + nCartId);
				continue;
			}
			SupplySupportQuestChangeCartPoolEntry changeCartPoolEntry = new SupplySupportQuestChangeCartPoolEntry(cart);
			changeCartPoolEntry.Set(dr5);
			cart.AddChangeCartPoolEntry(changeCartPoolEntry);
		}
		foreach (DataRow dr3 in UserDac.SupplySupportQuestRewards(conn, null))
		{
			SupplySupportQuestReward reward = new SupplySupportQuestReward(m_supplySupportQuest);
			reward.Set(dr3);
			m_supplySupportQuest.AddReward(reward);
		}
		foreach (DataRow dr2 in UserDac.SupplySupportQuestOrders(conn, null))
		{
			SupplySupportQuestOrder order2 = new SupplySupportQuestOrder(m_supplySupportQuest);
			order2.Set(dr2);
			m_supplySupportQuest.AddOrder(order2);
		}
		foreach (DataRow dr in UserDac.SupplySupportQuestCartPoolEntries(conn, null))
		{
			int nOrderId = Convert.ToInt32(dr["orderId"]);
			SupplySupportQuestOrder order = m_supplySupportQuest.GetOrder(nOrderId);
			if (order == null)
			{
				SFLogUtil.Warn(GetType(), "[보급지원퀘스트수레풀항목 목록] 보급지원퀘스트지령이 존재하지 않습니다. nOrderId = " + nOrderId);
				continue;
			}
			SupplySupportQuestCartPoolEntry cartPoolEntry = new SupplySupportQuestCartPoolEntry(order);
			cartPoolEntry.Set(dr);
			order.AddCartPoolEntry(cartPoolEntry);
		}
	}

	private void LoadUserDBResources_Cart(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.CartGrades(conn, null))
		{
			CartGrade grade = new CartGrade();
			grade.Set(dr2);
			m_cartGrades[grade.id - 1] = grade;
		}
		foreach (DataRow dr in UserDac.Carts(conn, null))
		{
			Cart cart = new Cart();
			cart.Set(dr);
			m_carts.Add(cart.id, cart);
		}
	}

	private void LoadUserDBResources_Monster(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.DropCountPoolEntries(conn, null))
		{
			int nPoolId = Convert.ToInt32(dr2["dropCountPoolId"]);
			if (nPoolId <= 0)
			{
				SFLogUtil.Warn(GetType(), "[드롭개수풀항물 목록] 드롭개수풀ID가 유효하지 않습니다. nPoolId = " + nPoolId);
				continue;
			}
			DropCountPool pool = GetOrCreateDropCountPool(nPoolId);
			DropCountPoolEntry entry = new DropCountPoolEntry(pool);
			entry.Set(dr2);
			pool.AddEntry(entry);
		}
		foreach (DataRow dr8 in UserDac.DropObjectPoolEntries(conn, null))
		{
			int nPoolId2 = Convert.ToInt32(dr8["dropObjectPoolId"]);
			if (nPoolId2 <= 0)
			{
				SFLogUtil.Warn(GetType(), "[드롭객체풀항목 목록] 드롭객체풀ID가 유효하지 않습니다. nPoolId = " + nPoolId2);
				continue;
			}
			DropObjectPool pool2 = GetOrCreateDropObjectPool(nPoolId2);
			DropObjectPoolEntry entry2 = new DropObjectPoolEntry(pool2);
			entry2.Set(dr8);
			pool2.AddEntry(entry2);
		}
		foreach (DataRow dr7 in UserDac.MonsterCharacters(conn, null))
		{
			MonsterCharacter monsterCharacter = new MonsterCharacter();
			monsterCharacter.Set(dr7);
			m_monsterCharacters.Add(monsterCharacter.id, monsterCharacter);
		}
		foreach (DataRow dr6 in UserDac.Monsters(conn, null))
		{
			Monster monster2 = new Monster();
			monster2.Set(dr6);
			m_monsters.Add(monster2.id, monster2);
		}
		foreach (DataRow dr5 in UserDac.MonsterArranges(conn, null))
		{
			MonsterArrange arrange = new MonsterArrange();
			arrange.Set(dr5);
			m_monsterArrange.Add(arrange.id, arrange);
		}
		foreach (DataRow dr4 in UserDac.MonsterSkills(conn, null))
		{
			MonsterSkill monsterSkill = new MonsterSkill();
			monsterSkill.Set(dr4);
			m_monsterSkills.Add(monsterSkill.skillId, monsterSkill);
		}
		foreach (DataRow dr3 in UserDac.MonsterOwnSkills(conn, null))
		{
			MonsterOwnSkill monsterOwnSkill = new MonsterOwnSkill();
			monsterOwnSkill.Set(dr3);
			int nMonsterId = Convert.ToInt32(dr3["monsterId"]);
			Monster monster = GetMonster(nMonsterId);
			if (monster == null)
			{
				SFLogUtil.Warn(GetType(), "[몬스터보유스킬 목록] 몬스터가 존재하지 않습니다. nMonsterId = " + nMonsterId);
			}
			else
			{
				monster.AddOwnSkill(monsterOwnSkill);
			}
		}
		foreach (DataRow dr in UserDac.MonsterSkillHits(conn, null))
		{
			MonsterSkillHit hit = new MonsterSkillHit();
			hit.Set(dr);
			int nSkillId = Convert.ToInt32(dr["skillId"]);
			MonsterSkill skill = GetMonsterSkill(nSkillId);
			if (skill == null)
			{
				SFLogUtil.Warn(GetType(), "[몬스터스킬적중 목록] 몬스터스킬이 존재하지 않습니다. nSkillId = " + nSkillId);
			}
			else
			{
				skill.AddMonsterSkillHit(hit);
			}
		}
	}

	private void LoadUserDBResources_Place(SqlConnection conn)
	{
		LoadUserDBResources_Place_Continent(conn);
		LoadUserDBResources_Place_MainQuestDungeon(conn);
		LoadUserDBResources_Place_StoryDungeon(conn);
		LoadUserDBResources_Place_ExpDungeon(conn);
		LoadUserDBResources_Place_GoldDungeon(conn);
		LoadUserDBResources_Place_UndergroundMaze(conn);
		LoadUserDBResources_Place_ArtifactRoom(conn);
		LoadUserDBResources_Place_AncientRelic(conn);
		LoadUserDBResources_Place_FieldOfHonor(conn);
		LoadUserDBResources_Place_SoulCoveter(conn);
		LoadUserDBResources_Place_ProofOfValor(conn);
		LoadUserDBResources_Place_WisdomTemple(conn);
		LoadUserDBResources_Place_RuinsReclaim(conn);
		LoadUserDBResources_Place_InfiniteWar(conn);
		LoadUserDBResources_Place_FearAltar(conn);
		LoadUserDBResources_Place_WarMemory(conn);
		LoadUserDBResources_Place_OsirisRoom(conn);
		LoadUserDBResources_Place_BiographyQuestDungeon(conn);
		LoadUserDBResources_Place_DragonNest(conn);
		LoadUserDBResources_Place_AnkouTomb(conn);
		LoadUserDBResources_Place_TradeShip(conn);
	}

	private void LoadUserDBResources_Place_Continent(SqlConnection conn)
	{
		foreach (DataRow dr3 in UserDac.Continents(conn, null))
		{
			Continent continent3 = new Continent();
			continent3.Set(dr3);
			m_continents.Add(continent3.id, continent3);
			AddLocation(continent3);
		}
		foreach (DataRow dr6 in UserDac.ContinentMonsterArranges(conn, null))
		{
			ContinentMonsterArrange monsterArrange = new ContinentMonsterArrange();
			monsterArrange.Set(dr6);
			int nContinentId4 = Convert.ToInt32(dr6["continentId"]);
			Continent continent5 = GetContinent(nContinentId4);
			if (continent5 == null)
			{
				SFLogUtil.Warn(GetType(), "[대륙몬스터배치 목록] 대륙이 존재하지 않습니다. nContinentId = " + nContinentId4);
			}
			else
			{
				continent5.AddMonsterArrange(monsterArrange);
			}
		}
		DataRowCollection drcPortals = UserDac.Portals(conn, null);
		foreach (DataRow dr8 in drcPortals)
		{
			Portal portal2 = new Portal();
			portal2.Set(dr8);
			int nContinentId3 = Convert.ToInt32(dr8["continentId"]);
			Continent continent4 = GetContinent(nContinentId3);
			if (continent4 == null)
			{
				SFLogUtil.Warn(GetType(), "[포탈 목록] 대륙이 존재하지 않습니다. nContinentId = " + nContinentId3);
				continue;
			}
			if (!continent4.isNationTerritory)
			{
				SFLogUtil.Warn(GetType(), "[포탈 목록] 대륙이 유효하지 않습니다. nContinentId = " + nContinentId3);
				continue;
			}
			continent4.AddPortal(portal2);
			m_portals.Add(portal2.id, portal2);
		}
		foreach (DataRow dr7 in drcPortals)
		{
			int nPortalId = Convert.ToInt32(dr7["portalId"]);
			Portal portal = GetPortal(nPortalId);
			if (portal == null)
			{
				SFLogUtil.Warn(GetType(), "[포탈 목록] 포탈이 존재하지 않습니다. nPortalId = " + nPortalId);
				continue;
			}
			int nLinkedPortalId = Convert.ToInt32(dr7["linkedPortalId"]);
			Portal linkedPortal = GetPortal(nLinkedPortalId);
			if (linkedPortal == null)
			{
				SFLogUtil.Warn(GetType(), "[포탈 목록] 연결포탈이 존재하지 않습니다. nPortalId = " + nPortalId + ", nLinkedPortalId = " + nLinkedPortalId);
			}
			else
			{
				portal.linkedPortal = linkedPortal;
			}
		}
		foreach (DataRow dr5 in UserDac.ContinentObjects(conn, null))
		{
			ContinentObject obj = new ContinentObject();
			obj.Set(dr5);
			AddContinentObject(obj);
		}
		foreach (DataRow dr4 in UserDac.ContinentObjectArranges(conn, null))
		{
			ContinentObjectArrange arrange = new ContinentObjectArrange();
			arrange.Set(dr4);
			int nContinentId2 = Convert.ToInt32(dr4["continentId"]);
			Continent continent2 = GetContinent(nContinentId2);
			if (continent2 == null)
			{
				SFLogUtil.Warn(GetType(), "[대륙오브젝트배치 목록] 대륙이 존재하지 않습니다. nContinentId = " + nContinentId2 + ", no = " + arrange.no);
			}
			else
			{
				continent2.AddObjectArrange(arrange);
			}
		}
		foreach (DataRow dr2 in UserDac.Npcs(conn, null))
		{
			int nContinentId = Convert.ToInt32(dr2["continentId"]);
			Continent continent = GetContinent(nContinentId);
			if (continent == null)
			{
				SFLogUtil.Warn(GetType(), "[NPC 목록] 대륙이 존재하지 않습니다. nContinentId = " + nContinentId);
				continue;
			}
			Npc npc2 = new Npc(continent);
			npc2.Set(dr2);
			m_npcs.Add(npc2.id, npc2);
			continent.AddNpc(npc2);
		}
		foreach (DataRow dr in UserDac.ContinentTransmissionExits(conn, null))
		{
			int nNpcId = Convert.ToInt32(dr["npcId"]);
			Npc npc = GetNpc(nNpcId);
			if (npc == null)
			{
				SFLogUtil.Warn(GetType(), "[대륙전송출구 목록] NPC가 존재하지 않습니다. nNpcId = " + nNpcId);
			}
			else if (npc.type != 2)
			{
				SFLogUtil.Warn(GetType(), "[대륙전송출구 목록] NPC가 대륙전송 타입이 아닙니다. nNpcId = " + nNpcId + ", npcType = " + npc.type);
			}
			else
			{
				ContinentTransmissionExit continentTransmissionExit = new ContinentTransmissionExit(npc);
				continentTransmissionExit.Set(dr);
				npc.AddContinentTransmissionExit(continentTransmissionExit);
			}
		}
	}

	private void LoadUserDBResources_Place_MainQuestDungeon(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.MainQuestDungeons(conn, null))
		{
			MainQuestDungeon dungeon3 = new MainQuestDungeon();
			dungeon3.Set(dr2);
			AddMainQuestDungeon(dungeon3);
		}
		foreach (DataRow dr4 in UserDac.MainQuestDungeonSteps(conn, null))
		{
			int nDungeonId3 = Convert.ToInt32(dr4["dungeonId"]);
			MainQuestDungeon dungeon4 = GetMainQuestDungeon(nDungeonId3);
			if (dungeon4 == null)
			{
				SFLogUtil.Warn(GetType(), "[메인퀘스트던전단계 목록] 메인퀘스트던전이 존재하지 않습니다. nDungeonId = " + nDungeonId3);
				continue;
			}
			MainQuestDungeonStep step3 = new MainQuestDungeonStep(dungeon4);
			step3.Set(dr4);
			dungeon4.AddStep(step3);
		}
		foreach (DataRow dr3 in UserDac.MainQuestDungeonMonsterArranges(conn, null))
		{
			int nDungeonId2 = Convert.ToInt32(dr3["dungeonId"]);
			MainQuestDungeon dungeon2 = GetMainQuestDungeon(nDungeonId2);
			if (dungeon2 == null)
			{
				SFLogUtil.Warn(GetType(), "[메인퀘스트던전몬스터배치 목록] 메인퀘스트던전이 존재하지 않습니다. nDungeonId = " + nDungeonId2);
				continue;
			}
			int nStepNo2 = Convert.ToInt32(dr3["step"]);
			MainQuestDungeonStep step2 = dungeon2.GetStep(nStepNo2);
			if (step2 == null)
			{
				SFLogUtil.Warn(GetType(), "[메인퀘스트던전몬스터배치 목록] 메인퀘스트던전단계가 존재하지 않습니다. nDungeonId = " + nDungeonId2 + ", nStepNo = " + nStepNo2);
			}
			else
			{
				MainQuestDungeonMonsterArrange arrange2 = new MainQuestDungeonMonsterArrange(step2);
				arrange2.Set(dr3);
				step2.AddMonsterArrange(arrange2);
			}
		}
		foreach (DataRow dr in UserDac.MainQuestDungeonSummons(conn, null))
		{
			int nDungeonId = Convert.ToInt32(dr["dungeonId"]);
			MainQuestDungeon dungeon = GetMainQuestDungeon(nDungeonId);
			if (dungeon == null)
			{
				SFLogUtil.Warn(GetType(), "[메인퀘스트던전소환 목록] 메인퀘스트던전이 존재하지 않습니다. nDungeonId = " + nDungeonId);
				continue;
			}
			int nStepNo = Convert.ToInt32(dr["step"]);
			MainQuestDungeonStep step = dungeon.GetStep(nStepNo);
			if (step == null)
			{
				SFLogUtil.Warn(GetType(), "[메인퀘스트던전소환 목록] 메인퀘스트던전단계가 존재하지 않습니다. nDungeonId = " + nDungeonId + ", nStepNo = " + nStepNo);
				continue;
			}
			int nArrangeNo = Convert.ToInt32(dr["arrangeNo"]);
			MainQuestDungeonMonsterArrange arrange = step.GetMonsterArrange(nArrangeNo);
			if (arrange == null)
			{
				SFLogUtil.Warn(GetType(), "메인퀘스트던전소환 목록] 메인퀘스트던전몬스터배치가 존재하지 않습니다. nDungeonId = " + nDungeonId + ", nStepNo = " + nStepNo + ", nArrangeNo = " + nArrangeNo);
			}
			else
			{
				MainQuestDungeonSummon summon = new MainQuestDungeonSummon(arrange);
				summon.Set(dr);
				arrange.AddSummon(summon);
			}
		}
	}

	private void LoadUserDBResources_Place_StoryDungeon(SqlConnection conn)
	{
		foreach (DataRow dr4 in UserDac.StoryDungeons(conn, null))
		{
			StoryDungeon storyDungeon4 = new StoryDungeon();
			storyDungeon4.Set(dr4);
			AddStoryDungeon(storyDungeon4);
		}
		foreach (DataRow dr6 in UserDac.StoryDungeonDifficulties(conn, null))
		{
			int nDungeonNo6 = Convert.ToInt32(dr6["dungeonNo"]);
			StoryDungeon storyDungeon5 = GetStoryDungeon(nDungeonNo6);
			if (storyDungeon5 == null)
			{
				SFLogUtil.Warn(GetType(), "[스토리던전난이도 목록] 스토리던전이 존재하지 않습니다. nDungeonNo = " + nDungeonNo6);
				continue;
			}
			StoryDungeonDifficulty difficulty6 = new StoryDungeonDifficulty(storyDungeon5);
			difficulty6.Set(dr6);
			storyDungeon5.AddDifficulty(difficulty6);
		}
		foreach (DataRow dr7 in UserDac.StoryDungeonRewards(conn, null))
		{
			int nDungeonNo5 = Convert.ToInt32(dr7["dungeonNo"]);
			StoryDungeon StoryDungeon2 = GetStoryDungeon(nDungeonNo5);
			if (StoryDungeon2 == null)
			{
				SFLogUtil.Warn(GetType(), "[스토리던전보상 목록] 스토리던전이 존재하지 않습니다. nDungeonNo = " + nDungeonNo5);
				continue;
			}
			int nDifficulty5 = Convert.ToInt32(dr7["difficulty"]);
			StoryDungeonDifficulty difficulty5 = StoryDungeon2.GetDifficulty(nDifficulty5);
			if (difficulty5 == null)
			{
				SFLogUtil.Warn(GetType(), "[스토리던전보상 목록] 스토리던전난이도가 존재하지 않습니다. nDungeonNo = " + nDungeonNo5 + ", nDifficulty = " + nDifficulty5);
			}
			else
			{
				StoryDungeonReward reward = new StoryDungeonReward(difficulty5);
				reward.Set(dr7);
				difficulty5.AddReward(reward);
			}
		}
		foreach (DataRow dr5 in UserDac.StoryDungeonSweepRewards(conn, null))
		{
			int nDungeonNo4 = Convert.ToInt32(dr5["dungeonNo"]);
			StoryDungeon StoryDungeon = GetStoryDungeon(nDungeonNo4);
			if (StoryDungeon == null)
			{
				SFLogUtil.Warn(GetType(), "[스토리던전소탕보상 목록] 스토리던전이 존재하지 않습니다. nDungeonNo = " + nDungeonNo4);
				continue;
			}
			int nDifficulty4 = Convert.ToInt32(dr5["difficulty"]);
			StoryDungeonDifficulty difficulty4 = StoryDungeon.GetDifficulty(nDifficulty4);
			if (difficulty4 == null)
			{
				SFLogUtil.Warn(GetType(), "[스토리던전소탕보상 목록] 스토리던전난이도가 존재하지 않습니다. nDungeonNo = " + nDungeonNo4 + ", nDifficulty = " + nDifficulty4);
			}
			else
			{
				StoryDungeonSweepReward sweepReward = new StoryDungeonSweepReward(difficulty4);
				sweepReward.Set(dr5);
				difficulty4.AddSweepReward(sweepReward);
			}
		}
		foreach (DataRow dr3 in UserDac.StoryDungeonSteps(conn, null))
		{
			int nDungeonNo3 = Convert.ToInt32(dr3["dungeonNo"]);
			StoryDungeon storyDungeon3 = GetStoryDungeon(nDungeonNo3);
			if (storyDungeon3 == null)
			{
				SFLogUtil.Warn(GetType(), "[스토리던전단계 목록] 스토리던전이 존재하지 않습니다. nDungeonNo = " + nDungeonNo3);
				continue;
			}
			int nDifficulty3 = Convert.ToInt32(dr3["difficulty"]);
			StoryDungeonDifficulty difficulty3 = storyDungeon3.GetDifficulty(nDifficulty3);
			if (difficulty3 == null)
			{
				SFLogUtil.Warn(GetType(), "[스토리던전단계 목록] 스토리던전난이도가 존재하지 않습니다. nDungeonNo = " + nDungeonNo3 + ", nDifficulty = " + nDifficulty3);
			}
			else
			{
				StoryDungeonStep step2 = new StoryDungeonStep(difficulty3);
				step2.Set(dr3);
				difficulty3.AddStep(step2);
			}
		}
		foreach (DataRow dr2 in UserDac.StoryDungeonMonsterArranges(conn, null))
		{
			int nDungeonNo2 = Convert.ToInt32(dr2["dungeonNo"]);
			StoryDungeon storyDungeon2 = GetStoryDungeon(nDungeonNo2);
			if (storyDungeon2 == null)
			{
				SFLogUtil.Warn(GetType(), "[스토리던전몬스터배치 목록] 스토리던전챕터가 존재하지 않습니다. storyDungeon = " + storyDungeon2);
				continue;
			}
			int nDifficulty2 = Convert.ToInt32(dr2["difficulty"]);
			StoryDungeonDifficulty difficulty2 = storyDungeon2.GetDifficulty(nDifficulty2);
			if (difficulty2 == null)
			{
				SFLogUtil.Warn(GetType(), "[스토리던전몬스터배치 목록] 스토리던전난이도가 존재하지 않습니다. nDungeonNo = " + nDungeonNo2 + ", nDifficulty = " + nDifficulty2);
				continue;
			}
			int nStepNo = Convert.ToInt32(dr2["step"]);
			StoryDungeonStep step = difficulty2.GetStep(nStepNo);
			if (step == null)
			{
				SFLogUtil.Warn(GetType(), "[스토리던전몬스터배치 목록] 스토리던전단계가 존재하지 않습니다. nDungeonNo = " + nDungeonNo2 + ", nDifficulty = " + nDifficulty2 + ", nStepNo = " + nStepNo);
			}
			else
			{
				StoryDungeonMonsterArrange monsterArrange = new StoryDungeonMonsterArrange(step);
				monsterArrange.Set(dr2);
				step.AddMonsterArrange(monsterArrange);
			}
		}
		foreach (DataRow dr in UserDac.StoryDungeonTraps(conn, null))
		{
			int nDungeonNo = Convert.ToInt32(dr["dungeonNo"]);
			StoryDungeon storyDungeon = GetStoryDungeon(nDungeonNo);
			if (storyDungeon == null)
			{
				SFLogUtil.Warn(GetType(), "[스토리던전함정 목록] 스토리던전이 존재하지 않습니다. nDungeonNo = " + nDungeonNo);
				continue;
			}
			int nDifficulty = Convert.ToInt32(dr["difficulty"]);
			StoryDungeonDifficulty difficulty = storyDungeon.GetDifficulty(nDifficulty);
			if (difficulty == null)
			{
				SFLogUtil.Warn(GetType(), "[스토리던전함정 목록] 스토리던전난이도가 존재하지 않습니다. nDungeonNo = " + nDungeonNo + ", nDifficulty = " + nDifficulty);
			}
			else
			{
				StoryDungeonTrap trap = new StoryDungeonTrap(difficulty);
				trap.Set(dr);
				difficulty.AddTrap(trap);
			}
		}
	}

	private void LoadUserDBResources_Place_ExpDungeon(SqlConnection conn)
	{
		DataRow drExpDungeon = UserDac.ExpDungeon(conn, null);
		if (drExpDungeon == null)
		{
			SFLogUtil.Warn(GetType(), "[경험치던전] 경험치던전이 존재하지 않습니다.");
			return;
		}
		ExpDungeon expDungeon = new ExpDungeon();
		expDungeon.Set(drExpDungeon);
		m_expDungeon = expDungeon;
		AddLocation(expDungeon);
		foreach (DataRow dr3 in UserDac.ExpDungeonDifficulties(conn, null))
		{
			ExpDungeonDifficulty difficulty3 = new ExpDungeonDifficulty(m_expDungeon);
			difficulty3.Set(dr3);
			m_expDungeon.AddDifficulty(difficulty3);
		}
		foreach (DataRow dr2 in UserDac.ExpDungeonDifficultyWaves(conn, null))
		{
			int nDifficulty2 = Convert.ToInt32(dr2["difficulty"]);
			ExpDungeonDifficulty difficulty2 = m_expDungeon.GetDifficulty(nDifficulty2);
			if (difficulty2 == null)
			{
				SFLogUtil.Warn(GetType(), "[경험치던전난이도웨이브 목록] 경험치던전난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty2);
				continue;
			}
			ExpDungeonDifficultyWave wave2 = new ExpDungeonDifficultyWave(difficulty2);
			wave2.Set(dr2);
			difficulty2.AddWave(wave2);
		}
		foreach (DataRow dr in UserDac.ExpDungeonMonsterArranges(conn, null))
		{
			int nDifficulty = Convert.ToInt32(dr["difficulty"]);
			ExpDungeonDifficulty difficulty = m_expDungeon.GetDifficulty(nDifficulty);
			if (difficulty == null)
			{
				SFLogUtil.Warn(GetType(), "[경험치던전몬스터배치 목록] 경험치던전난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty);
				continue;
			}
			int nWaveNo = Convert.ToInt32(dr["waveNo"]);
			ExpDungeonDifficultyWave wave = difficulty.GetWave(nWaveNo);
			if (wave == null)
			{
				SFLogUtil.Warn(GetType(), "[경험치던전몬스터배치 목록] 경험치던전난이도웨이브가 존재하지 않습니다. nDifficulty = " + nDifficulty + ", nWaveNo = " + nWaveNo);
			}
			else
			{
				ExpDungeonMonsterArrange arrange = new ExpDungeonMonsterArrange(wave);
				arrange.Set(dr);
				wave.AddMonsterArrange(arrange);
			}
		}
	}

	private void LoadUserDBResources_Place_GoldDungeon(SqlConnection conn)
	{
		DataRow drGoldDungeon = UserDac.GoldDungeon(conn, null);
		if (drGoldDungeon == null)
		{
			SFLogUtil.Warn(GetType(), "[골드던전] 골드던전이 존재하지 않습니다.");
			return;
		}
		GoldDungeon goldDungeon = new GoldDungeon();
		goldDungeon.Set(drGoldDungeon);
		m_goldDungeon = goldDungeon;
		AddLocation(goldDungeon);
		foreach (DataRow dr4 in UserDac.GoldDungeonDifficulties(conn, null))
		{
			GoldDungeonDifficulty difficulty4 = new GoldDungeonDifficulty(m_goldDungeon);
			difficulty4.Set(dr4);
			m_goldDungeon.AddDifficulty(difficulty4);
		}
		foreach (DataRow dr3 in UserDac.GoldDungeonSteps(conn, null))
		{
			int nDifficulty3 = Convert.ToInt32(dr3["difficulty"]);
			GoldDungeonDifficulty difficulty3 = m_goldDungeon.GetDifficulty(nDifficulty3);
			if (difficulty3 == null)
			{
				SFLogUtil.Warn(GetType(), "[골드던전단계 목록] 골드던전난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty3);
				continue;
			}
			GoldDungeonStep step3 = new GoldDungeonStep(difficulty3);
			step3.Set(dr3);
			difficulty3.AddStep(step3);
		}
		foreach (DataRow dr2 in UserDac.GoldDungeonStepWaves(conn, null))
		{
			int nDifficulty2 = Convert.ToInt32(dr2["difficulty"]);
			GoldDungeonDifficulty difficulty2 = m_goldDungeon.GetDifficulty(nDifficulty2);
			if (difficulty2 == null)
			{
				SFLogUtil.Warn(GetType(), "[골드던전단계웨이브 목록] 골드던전난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty2);
				continue;
			}
			int nStep2 = Convert.ToInt32(dr2["step"]);
			GoldDungeonStep step2 = difficulty2.GetStep(nStep2);
			if (step2 == null)
			{
				SFLogUtil.Warn(GetType(), "[골드던전단계웨이브 목록] 골드던전단계가 존재하지 않습니다. nDifficulty= " + nDifficulty2 + ", nStep = " + nStep2);
			}
			else
			{
				GoldDungeonStepWave wave = new GoldDungeonStepWave(step2);
				wave.Set(dr2);
				step2.AddWave(wave);
			}
		}
		foreach (DataRow dr in UserDac.GoldDungeonStepMonsterArranges(conn, null))
		{
			int nDifficulty = Convert.ToInt32(dr["difficulty"]);
			GoldDungeonDifficulty difficulty = m_goldDungeon.GetDifficulty(nDifficulty);
			if (difficulty == null)
			{
				SFLogUtil.Warn(GetType(), "[골드던전단계몬스터배치 목록] 골드던전난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty);
				continue;
			}
			int nStep = Convert.ToInt32(dr["step"]);
			GoldDungeonStep step = difficulty.GetStep(nStep);
			if (step == null)
			{
				SFLogUtil.Warn(GetType(), "[골드던전단계몬스터배치 목록] 골드던전단계가 존재하지 않습니다. nDifficulty= " + nDifficulty + ", nStep = " + nStep);
			}
			else
			{
				GoldDungeonStepMonsterArrange monsterArrange = new GoldDungeonStepMonsterArrange(step);
				monsterArrange.Set(dr);
				step.AddMonsterArrange(monsterArrange);
			}
		}
	}

	private void LoadUserDBResources_Place_UndergroundMaze(SqlConnection conn)
	{
		DataRow drUndergroundMaze = UserDac.UndergroundMaze(conn, null);
		if (drUndergroundMaze == null)
		{
			SFLogUtil.Warn(GetType(), "[지하미로] 지하미로가 존재하지 않습니다.");
			return;
		}
		UndergroundMaze undergroundMaze = new UndergroundMaze();
		undergroundMaze.Set(drUndergroundMaze);
		m_undergroundMaze = undergroundMaze;
		AddLocation(undergroundMaze);
		foreach (DataRow dr4 in UserDac.UndergroundMazeFloors(conn, null))
		{
			UndergroundMazeFloor floor3 = new UndergroundMazeFloor(m_undergroundMaze);
			floor3.Set(dr4);
			m_undergroundMaze.AddFloor(floor3);
		}
		foreach (DataRow dr7 in UserDac.UndergroundMazeEntrances(conn, null))
		{
			UndergroundMazeEntrance entrance = new UndergroundMazeEntrance(m_undergroundMaze);
			entrance.Set(dr7);
			m_undergroundMaze.AddEntrance(entrance);
		}
		DataRowCollection drcUndergroundMazePortals = UserDac.UndergroundMazePortals(conn, null);
		foreach (DataRow dr6 in drcUndergroundMazePortals)
		{
			int nFloor3 = Convert.ToInt32(dr6["floor"]);
			UndergroundMazeFloor floor4 = m_undergroundMaze.GetFloor(nFloor3);
			if (floor4 == null)
			{
				SFLogUtil.Warn(GetType(), "[지하미로포탈 목록] 지하미로층이 존재하지 않습니다. nFloor = " + nFloor3);
				continue;
			}
			UndergroundMazePortal portal2 = new UndergroundMazePortal(floor4);
			portal2.Set(dr6);
			floor4.AddPortal(portal2);
			m_undergroundMazePortals.Add(portal2);
		}
		foreach (DataRow dr5 in drcUndergroundMazePortals)
		{
			int nPortalId = Convert.ToInt32(dr5["portalId"]);
			UndergroundMazePortal portal = GetUndergroundMazePortal(nPortalId);
			if (portal == null)
			{
				SFLogUtil.Warn(GetType(), "[지하미로포탈 목록 - 연결포탈] 포탈이 존재하지 않습니다. nPortalId = " + nPortalId);
				continue;
			}
			int nLinkedPortalId = Convert.ToInt32(dr5["linkedPortalId"]);
			UndergroundMazePortal linkedPortal = GetUndergroundMazePortal(nLinkedPortalId);
			if (linkedPortal == null)
			{
				SFLogUtil.Warn(GetType(), "[지하미로포탈 목록 - 연결포탈] 연결포탈이 존재하지 않습니다. nLinkedPortalId = " + nLinkedPortalId);
			}
			else
			{
				portal.linkedPortal = linkedPortal;
			}
		}
		foreach (DataRow dr3 in UserDac.UndergroundMazeMonsterArranges(conn, null))
		{
			int nFloor2 = Convert.ToInt32(dr3["floor"]);
			UndergroundMazeFloor floor2 = m_undergroundMaze.GetFloor(nFloor2);
			if (floor2 == null)
			{
				SFLogUtil.Warn(GetType(), "[지하미로몬스터배치 목록] 지하미로층이 존재하지 않습니다. nFloor = " + nFloor2);
				continue;
			}
			UndergroundMazeMonsterArrange arrange = new UndergroundMazeMonsterArrange(floor2);
			arrange.Set(dr3);
			floor2.AddArrange(arrange);
		}
		foreach (DataRow dr2 in UserDac.UndergroundMazeNpcs(conn, null))
		{
			int nFloor = Convert.ToInt32(dr2["floor"]);
			UndergroundMazeFloor floor = m_undergroundMaze.GetFloor(nFloor);
			if (floor == null)
			{
				SFLogUtil.Warn(GetType(), "[지하미로NPC 목록] 지하미로층이 존재하지 않습니다. nFloor = " + nFloor);
				continue;
			}
			UndergroundMazeNpc npc2 = new UndergroundMazeNpc(floor);
			npc2.Set(dr2);
			floor.AddNpc(npc2);
			m_undergroundMazeNpcs.Add(npc2);
		}
		foreach (DataRow dr in UserDac.UndergroundMazeNpcTransmissionEntries(conn, null))
		{
			int nNpcId = Convert.ToInt32(dr["npcId"]);
			UndergroundMazeNpc npc = GetUndergroundMazeNpc(nNpcId);
			if (npc == null)
			{
				SFLogUtil.Warn(GetType(), "[지하미로NPC전송항목 목록] 지하미로NPC가 존재하지 않습니다. nNpcId = " + nNpcId);
				continue;
			}
			UndergroundMazeNpcTransmissionEntry entry = new UndergroundMazeNpcTransmissionEntry(npc);
			entry.Set(dr);
			npc.AddTransmissionEntry(entry);
		}
	}

	private void LoadUserDBResources_Place_ArtifactRoom(SqlConnection conn)
	{
		DataRow drArtifactRoom = UserDac.ArtifactRoom(conn, null);
		if (drArtifactRoom == null)
		{
			SFLogUtil.Warn(GetType(), "[고대유물의방] 고대유물의방이 존재하지 않습니다.");
			return;
		}
		ArtifactRoom artifactRoom = new ArtifactRoom();
		artifactRoom.Set(drArtifactRoom);
		m_artifactRoom = artifactRoom;
		AddLocation(artifactRoom);
		foreach (DataRow dr2 in UserDac.ArtifactRoomFloors(conn, null))
		{
			ArtifactRoomFloor floor2 = new ArtifactRoomFloor(m_artifactRoom);
			floor2.Set(dr2);
			m_artifactRoom.AddFloor(floor2);
		}
		foreach (DataRow dr in UserDac.ArtifactRoomMonsterArranges(conn, null))
		{
			int nFloor = Convert.ToInt32(dr["floor"]);
			ArtifactRoomFloor floor = m_artifactRoom.GetFloor(nFloor);
			if (floor == null)
			{
				SFLogUtil.Warn(GetType(), "[고대유물의방몬스터배치 목록] 고대유물의방층이 존재하지 않습니다. nFloor = " + nFloor);
				continue;
			}
			ArtifactRoomMonsterArrange arrange = new ArtifactRoomMonsterArrange(floor);
			arrange.Set(dr);
			floor.AddArrange(arrange);
		}
	}

	private void LoadUserDBResources_Place_AncientRelic(SqlConnection conn)
	{
		DataRow drAncientRelic = UserDac.AncientRelic(conn, null);
		if (drAncientRelic == null)
		{
			SFLogUtil.Warn(GetType(), "[고대인의유적] 고대인의유적이 존재하지 않습니다.");
			return;
		}
		AncientRelic ancientRelic = new AncientRelic();
		ancientRelic.Set(drAncientRelic);
		m_ancientRelic = ancientRelic;
		AddLocation(ancientRelic);
		foreach (DataRow dr4 in UserDac.AncientRelicMonsterAttrFactors(conn, null))
		{
			AncientRelicMonsterAttrFactor monsterAttrFactor = new AncientRelicMonsterAttrFactor(m_ancientRelic);
			monsterAttrFactor.Set(dr4);
			m_ancientRelic.AddMonsterAttrFactor(monsterAttrFactor);
		}
		foreach (DataRow dr8 in UserDac.AncientRelicRoutes(conn, null))
		{
			AncientRelicRoute route = new AncientRelicRoute(m_ancientRelic);
			route.Set(dr8);
			m_ancientRelic.AddRoute(route);
		}
		foreach (DataRow dr7 in UserDac.AncientRelicTraps(conn, null))
		{
			AncientRelicTrap trap = new AncientRelicTrap(m_ancientRelic);
			trap.Set(dr7);
			m_ancientRelic.AddTrap(trap);
		}
		foreach (DataRow dr6 in UserDac.AncientRelicSteps(conn, null))
		{
			AncientRelicStep step5 = new AncientRelicStep(m_ancientRelic);
			step5.Set(dr6);
			m_ancientRelic.AddStep(step5);
		}
		foreach (DataRow dr5 in UserDac.AncientRelicStepRoutes(conn, null))
		{
			int nStep4 = Convert.ToInt32(dr5["step"]);
			AncientRelicStep step4 = m_ancientRelic.GetStep(nStep4);
			if (step4 == null)
			{
				SFLogUtil.Warn(GetType(), "[고대인의유적단계이동경로 목록] 고대인의유적단계가 존재하지 않습니다. nStep = " + nStep4);
				continue;
			}
			AncientRelicStepRoute stepRoute = new AncientRelicStepRoute(step4);
			stepRoute.Set(dr5);
			step4.AddRoute(stepRoute);
		}
		foreach (DataRow dr3 in UserDac.AncientRelicStepRewardPoolEntries(conn, null))
		{
			int nStep3 = Convert.ToInt32(dr3["step"]);
			AncientRelicStep step3 = m_ancientRelic.GetStep(nStep3);
			if (step3 == null)
			{
				SFLogUtil.Warn(GetType(), "[고대인의유적단계보상풀항목 목록] 고대인의유적단계가 존재하지 않습니다. nStep = " + nStep3);
				continue;
			}
			int nLevel = Convert.ToInt32(dr3["level"]);
			AncientRelicStepRewardPoolCollection rewardPoolCollection = step3.GetOrCreateRewardPoolCollection(nLevel);
			int nPoolId = Convert.ToInt32(dr3["poolId"]);
			AncientRelicStepRewardPool rewardPool = rewardPoolCollection.GetOrCreatePool(nPoolId);
			AncientRelicStepRewardPoolEntry rewardPoolEntry = new AncientRelicStepRewardPoolEntry(rewardPool);
			rewardPoolEntry.Set(dr3);
			rewardPool.AddEntry(rewardPoolEntry);
		}
		foreach (DataRow dr2 in UserDac.AncientRelicStepWaves(conn, null))
		{
			int nStep2 = Convert.ToInt32(dr2["step"]);
			AncientRelicStep step2 = m_ancientRelic.GetStep(nStep2);
			if (step2 == null)
			{
				SFLogUtil.Warn(GetType(), "[고대인의유적단계웨이브 목록] 고대인의유적단계가 존재하지 않습니다. nStep = " + nStep2);
				continue;
			}
			AncientRelicStepWave wave2 = new AncientRelicStepWave(step2);
			wave2.Set(dr2);
			step2.AddWave(wave2);
		}
		foreach (DataRow dr in UserDac.AncientRelicMonsterArranges(conn, null))
		{
			int nStep = Convert.ToInt32(dr["step"]);
			AncientRelicStep step = m_ancientRelic.GetStep(nStep);
			if (step == null)
			{
				SFLogUtil.Warn(GetType(), "[고대인의유적몬스터배치 목록] 고대인의유적단계가 존재하지 않습니다. nStep = " + nStep);
				continue;
			}
			int nWaveNo = Convert.ToInt32(dr["waveNo"]);
			AncientRelicStepWave wave = step.GetWave(nWaveNo);
			if (wave == null)
			{
				SFLogUtil.Warn(GetType(), "[고대인의유적몬스터배치 목록] 고대인의유적단계웨이브가 존재하지 않습니다. nStep = " + nStep + ", nWaveNo = " + nWaveNo);
			}
			else
			{
				AncientRelicMonsterArrange arrange = new AncientRelicMonsterArrange(wave);
				arrange.Set(dr);
				wave.AddArrange(arrange);
			}
		}
	}

	private void LoadUserDBResources_Place_FieldOfHonor(SqlConnection conn)
	{
		DataRow drFieldOfHonor = UserDac.FieldOfHonor(conn, null);
		if (drFieldOfHonor == null)
		{
			SFLogUtil.Warn(GetType(), "[결투장] 결투장이 존재하지 않습니다.");
			return;
		}
		FieldOfHonor fieldOfHonor = new FieldOfHonor();
		fieldOfHonor.Set(drFieldOfHonor);
		m_fieldOfHonor = fieldOfHonor;
		AddLocation(fieldOfHonor);
		foreach (DataRow dr3 in UserDac.FieldOfHonorLevelRewards(conn, null))
		{
			FieldOfHonorLevelReward levelReward = new FieldOfHonorLevelReward(m_fieldOfHonor);
			levelReward.Set(dr3);
			m_fieldOfHonor.AddLevelReward(levelReward);
		}
		foreach (DataRow dr2 in UserDac.FieldOfHonorRankingRewards(conn, null))
		{
			FieldOfHonorRankingReward rankingReward = new FieldOfHonorRankingReward(m_fieldOfHonor);
			rankingReward.Set(dr2);
			m_fieldOfHonor.AddRankingReward(rankingReward);
		}
		foreach (DataRow dr in UserDac.FieldOfHonorTargets(conn, null))
		{
			FieldOfHonorTarget target = new FieldOfHonorTarget(m_fieldOfHonor);
			target.Set(dr);
			m_fieldOfHonor.AddTarget(target);
		}
	}

	private void LoadUserDBResources_Place_SoulCoveter(SqlConnection conn)
	{
		DataRow drSoulCoveter = UserDac.SoulCoveter(conn, null);
		if (drSoulCoveter == null)
		{
			SFLogUtil.Warn(GetType(), "[영혼을탐하는자] 영혼을탐하는자가 존재하지 않습니다.");
			return;
		}
		SoulCoveter soulCoveter = new SoulCoveter();
		soulCoveter.Set(drSoulCoveter);
		m_soulCoveter = soulCoveter;
		AddLocation(soulCoveter);
		foreach (DataRow dr4 in UserDac.SoulCoveterDifficulties(conn, null))
		{
			SoulCoveterDifficulty difficulty4 = new SoulCoveterDifficulty(m_soulCoveter);
			difficulty4.Set(dr4);
			m_soulCoveter.AddDifficulty(difficulty4);
		}
		foreach (DataRow dr3 in UserDac.SoulCoveterDifficultyRewards(conn, null))
		{
			int nDifficulty3 = Convert.ToInt32(dr3["difficulty"]);
			SoulCoveterDifficulty difficulty3 = m_soulCoveter.GetDifficulty(nDifficulty3);
			if (difficulty3 == null)
			{
				SFLogUtil.Warn(GetType(), "[영혼을탐하는자난이도보상 목록] 영혼을탐하는자 난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty3);
				continue;
			}
			SoulCoveterDifficultyReward reward = new SoulCoveterDifficultyReward(difficulty3);
			reward.Set(dr3);
			difficulty3.AddReward(reward);
		}
		foreach (DataRow dr2 in UserDac.SoulCoveterDifficultyWaves(conn, null))
		{
			int nDifficulty2 = Convert.ToInt32(dr2["difficulty"]);
			SoulCoveterDifficulty difficulty2 = m_soulCoveter.GetDifficulty(nDifficulty2);
			if (difficulty2 == null)
			{
				SFLogUtil.Warn(GetType(), "[영혼을탐하는자난이도웨이브 목록] 영혼을탐하는자난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty2);
				continue;
			}
			SoulCoveterDifficultyWave wave2 = new SoulCoveterDifficultyWave(difficulty2);
			wave2.Set(dr2);
			difficulty2.AddWave(wave2);
		}
		foreach (DataRow dr in UserDac.SoulCoveterMonsterArranges(conn, null))
		{
			int nDifficulty = Convert.ToInt32(dr["difficulty"]);
			SoulCoveterDifficulty difficulty = m_soulCoveter.GetDifficulty(nDifficulty);
			if (difficulty == null)
			{
				SFLogUtil.Warn(GetType(), "[영혼을탐하는자몬스터배치 목록] 영혼을탐하는자난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty);
				continue;
			}
			int nWaveNo = Convert.ToInt32(dr["waveNo"]);
			SoulCoveterDifficultyWave wave = difficulty.GetWave(nWaveNo);
			if (wave == null)
			{
				SFLogUtil.Warn(GetType(), "[영혼을탐하는자몬스터배치 목록] 영혼을탐하는자난이도웨이브가 존재하지 않습니다. nDifficulty = " + nDifficulty + ", nWaveNo = " + nWaveNo);
			}
			else
			{
				SoulCoveterMonsterArrange monsterArrange = new SoulCoveterMonsterArrange(wave);
				monsterArrange.Set(dr);
				wave.AddMonsterArrange(monsterArrange);
			}
		}
	}

	private void LoadUserDBResources_Place_ProofOfValor(SqlConnection conn)
	{
		DataRow drProolOfValor = UserDac.ProofOfValor(conn, null);
		if (drProolOfValor == null)
		{
			SFLogUtil.Warn(GetType(), "[용맹의증명] 용맹의증명이 존재하지 않습니다.");
			return;
		}
		ProofOfValor proofOfValor = new ProofOfValor();
		proofOfValor.Set(drProolOfValor);
		m_proofOfValor = proofOfValor;
		AddLocation(proofOfValor);
		foreach (DataRow dr4 in UserDac.ProofOfValorBuffBoxs(conn, null))
		{
			ProofOfValorBuffBox buffBox = new ProofOfValorBuffBox(m_proofOfValor);
			buffBox.Set(dr4);
			m_proofOfValor.AddBuffBox(buffBox);
		}
		foreach (DataRow dr8 in UserDac.ProofOfValorBuffBoxArranges(conn, null))
		{
			ProofOfValorBuffBoxArrange buffBoxArrange = new ProofOfValorBuffBoxArrange();
			buffBoxArrange.Set(dr8);
			m_proofOfValor.AddBuffBoxArrange(buffBoxArrange);
		}
		foreach (DataRow dr10 in UserDac.ProofOfValorMonsterAttrFactors(conn, null))
		{
			ProofOfValorMonsterAttrFactor monsterAttrFactor = new ProofOfValorMonsterAttrFactor(m_proofOfValor);
			monsterAttrFactor.Set(dr10);
			m_proofOfValor.AddMonsterAttrFactor(monsterAttrFactor);
		}
		foreach (DataRow dr9 in UserDac.ProofOfValorPaidRefreshs(conn, null))
		{
			ProofOfValorPaidRefresh paidRefresh = new ProofOfValorPaidRefresh(m_proofOfValor);
			paidRefresh.Set(dr9);
			m_proofOfValor.AddPaidRefresh(paidRefresh);
		}
		foreach (DataRow dr7 in UserDac.ProofOfValorCreatureCardPoolEntries(conn, null))
		{
			int nCreatureCardPoolId = Convert.ToInt32(dr7["creatureCardPoolId"]);
			ProofOfValorCreatureCardPool creatureCardPool = m_proofOfValor.GetOrCreateCreatureCardPool(nCreatureCardPoolId);
			ProofOfValorCreatureCardPoolEntry creatureCardPoolEntry = new ProofOfValorCreatureCardPoolEntry(creatureCardPool);
			creatureCardPoolEntry.Set(dr7);
			creatureCardPool.AddEntry(creatureCardPoolEntry);
		}
		foreach (DataRow dr6 in UserDac.ProofOfValorBossMonsterArranges(conn, null))
		{
			ProofOfValorBossMonsterArrange bossMonsterArrange2 = new ProofOfValorBossMonsterArrange(m_proofOfValor);
			bossMonsterArrange2.Set(dr6);
			m_proofOfValor.AddBossMonsterArrange(bossMonsterArrange2);
		}
		foreach (DataRow dr5 in UserDac.ProofOfValorNormalMonsterArranges(conn, null))
		{
			int nBossMonsterArrange = Convert.ToInt32(dr5["proofOfValorBossMonsterArrangeId"]);
			ProofOfValorBossMonsterArrange bossMonsterArrange = m_proofOfValor.GetBossMonaterArrange(nBossMonsterArrange);
			if (bossMonsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "[용맹의증명일반몬스터배치 목록] 용맹의증명보스몬스터배치가 존재하지 않습니다. nBossMonsterArrange = " + nBossMonsterArrange);
				continue;
			}
			ProofOfValorNormalMonsterArrange normalMonsterArrange = new ProofOfValorNormalMonsterArrange(bossMonsterArrange);
			normalMonsterArrange.Set(dr5);
			bossMonsterArrange.AddNormalMonsterArrange(normalMonsterArrange);
		}
		foreach (DataRow dr3 in UserDac.ProofOfValorRefreshSchedules(conn, null))
		{
			ProofOfValorRefreshSchedule refreshSchedule = new ProofOfValorRefreshSchedule(m_proofOfValor);
			refreshSchedule.Set(dr3);
			m_proofOfValor.AddRefreshSchedule(refreshSchedule);
		}
		foreach (DataRow dr2 in UserDac.ProofOfValorRewards(conn, null))
		{
			ProofOfValorReward reward = new ProofOfValorReward(m_proofOfValor);
			reward.Set(dr2);
			m_proofOfValor.AddReward(reward);
		}
		foreach (DataRow dr in UserDac.ProofOfValorClearGrades(conn, null))
		{
			ProofOfValorClearGrade clearGrade = new ProofOfValorClearGrade(m_proofOfValor);
			clearGrade.Set(dr);
			m_proofOfValor.AddClearGrade(clearGrade);
		}
	}

	private void LoadUserDBResources_Place_WisdomTemple(SqlConnection conn)
	{
		DataRow drWisdomTemple = UserDac.WisdomTemple(conn, null);
		if (drWisdomTemple == null)
		{
			SFLogUtil.Warn(GetType(), "[지혜의신전] 지혜의신전이 존재하지 않습니다.");
			return;
		}
		WisdomTemple wisdomTemple = new WisdomTemple();
		wisdomTemple.Set(drWisdomTemple);
		m_wisdomTemple = wisdomTemple;
		AddLocation(wisdomTemple);
		foreach (DataRow dr6 in UserDac.WisdomTempleMonsterAttrFactors(conn, null))
		{
			WisdomTempleMonsterAttrFactor monsterAttrFactor = new WisdomTempleMonsterAttrFactor(m_wisdomTemple);
			monsterAttrFactor.Set(dr6);
			m_wisdomTemple.AddMonsterAttrFactor(monsterAttrFactor);
		}
		foreach (DataRow dr9 in UserDac.WisdomTempleColorMatchingObjects(conn, null))
		{
			WisdomTempleColorMatchingObject colorMatchingObject = new WisdomTempleColorMatchingObject(m_wisdomTemple);
			colorMatchingObject.Set(dr9);
			m_wisdomTemple.AddColorMatchingObject(colorMatchingObject);
		}
		foreach (DataRow dr12 in UserDac.WisdomTempleArrangePositions(conn, null))
		{
			WisdomTempleArrangePosition arrangePosition2 = new WisdomTempleArrangePosition(m_wisdomTemple);
			arrangePosition2.Set(dr12);
			m_wisdomTemple.AddArrangePosition(arrangePosition2);
		}
		foreach (DataRow dr15 in UserDac.WisdomTempleSweepRewards(conn, null))
		{
			WisdomTempleSweepReward sweepReward = new WisdomTempleSweepReward(m_wisdomTemple);
			sweepReward.Set(dr15);
			m_wisdomTemple.AddSweepReward(sweepReward);
		}
		foreach (DataRow dr14 in UserDac.WisdomTemplePuzzleRewards(conn, null))
		{
			WisdomTemplePuzzleReward puzzleReward = new WisdomTemplePuzzleReward(m_wisdomTemple);
			puzzleReward.Set(dr14);
			m_wisdomTemple.AddPuzzleReward(puzzleReward);
		}
		foreach (DataRow dr13 in UserDac.WisdomTempleSteps(conn, null))
		{
			WisdomTempleStep step5 = new WisdomTempleStep(m_wisdomTemple);
			step5.Set(dr13);
			m_wisdomTemple.AddStep(step5);
		}
		foreach (DataRow dr11 in UserDac.WisdomTempleQuizMonsterPositions(conn, null))
		{
			int nStepNo4 = Convert.ToInt32(dr11["stepNo"]);
			WisdomTempleStep step4 = m_wisdomTemple.GetStep(nStepNo4);
			if (step4 == null)
			{
				SFLogUtil.Warn(GetType(), "[지혜의신전단계퀴즈위치 목록] 지혜의신전단계가 존재하지 않습니다. nStepNo = " + nStepNo4);
				continue;
			}
			int nRow = Convert.ToInt32(dr11["row"]);
			int nCol = Convert.ToInt32(dr11["col"]);
			WisdomTempleArrangePosition arrangePosition = m_wisdomTemple.GetArrangePosition(nRow, nCol);
			if (arrangePosition == null)
			{
				SFLogUtil.Warn(GetType(), "[지혜의신전단계퀴즈위치 목록] 지혜의신전배치위치가 존재하지 않습니다. nRow = " + nRow + ", nCol = " + nCol);
			}
			else
			{
				WisdomTempleQuizMonsterPosition quizMonsterPosition = new WisdomTempleQuizMonsterPosition(step4, arrangePosition);
				step4.AddQuizMonsterPosition(quizMonsterPosition);
			}
		}
		foreach (DataRow dr10 in UserDac.WisdomTempleQuizPoolEntries(conn, null))
		{
			int nStepNo3 = Convert.ToInt32(dr10["stepNo"]);
			WisdomTempleStep step3 = m_wisdomTemple.GetStep(nStepNo3);
			if (step3 == null)
			{
				SFLogUtil.Warn(GetType(), "[지혜의신전퀴즈풀항목 목록] 지혜의신전단계가 존재하지 않습니다. nStepNo = " + nStepNo3);
				continue;
			}
			WisdomTempleQuizPoolEntry quizPoolEntry3 = new WisdomTempleQuizPoolEntry(step3);
			quizPoolEntry3.Set(dr10);
			step3.AddQuizPoolEntry(quizPoolEntry3);
		}
		foreach (DataRow dr8 in UserDac.WisdomTempleQuizRightAnswerPoolEntries(conn, null))
		{
			int nStepNo2 = Convert.ToInt32(dr8["stepNo"]);
			WisdomTempleStep step2 = m_wisdomTemple.GetStep(nStepNo2);
			if (step2 == null)
			{
				SFLogUtil.Warn(GetType(), "[지혜의신전퀴즈정답풀항목 목록] 지혜의신전단계가 존재하지 않습니다. nStepNo = " + nStepNo2);
				continue;
			}
			int nQuizNo2 = Convert.ToInt32(dr8["quizNo"]);
			WisdomTempleQuizPoolEntry quizPoolEntry2 = step2.GetQuizPoolEntry(nQuizNo2);
			if (quizPoolEntry2 == null)
			{
				SFLogUtil.Warn(GetType(), "[지혜의신전퀴즈정답풀항목 목록] 지혜의신전퀴즈풀항목이 존재하지 않습니다. nStepNo = " + nStepNo2 + ", nQuizNo = " + nQuizNo2);
			}
			else
			{
				WisdomTempleQuizRightAnswerPoolEntry quizRightPoolEntry = new WisdomTempleQuizRightAnswerPoolEntry(quizPoolEntry2);
				quizRightPoolEntry.Set(dr8);
				quizPoolEntry2.AddRightAnswerPoolEntry(quizRightPoolEntry);
			}
		}
		foreach (DataRow dr7 in UserDac.WisdomTempleQuizWrongAnswerPoolEntries(conn, null))
		{
			int nStepNo = Convert.ToInt32(dr7["stepNo"]);
			WisdomTempleStep step = m_wisdomTemple.GetStep(nStepNo);
			if (step == null)
			{
				SFLogUtil.Warn(GetType(), "[지혜의신전퀴즈오답풀항목 목록] 지혜의신전단계가 존재하지 않습니다. nStepNo = " + nStepNo);
				continue;
			}
			int nQuizNo = Convert.ToInt32(dr7["quizNo"]);
			WisdomTempleQuizPoolEntry quizPoolEntry = step.GetQuizPoolEntry(nQuizNo);
			if (quizPoolEntry == null)
			{
				SFLogUtil.Warn(GetType(), "[지혜의신전퀴즈오답풀항목 목록] 지혜의신전퀴즈풀항목이 존재하지 않습니다. nStepNo = " + nStepNo + ", nQuizNo = " + nQuizNo);
			}
			else
			{
				WisdomTempleQuizWrongAnswerPoolEntry quizWrongPoolEntry = new WisdomTempleQuizWrongAnswerPoolEntry(quizPoolEntry);
				quizWrongPoolEntry.Set(dr7);
				quizPoolEntry.AddWrongAnswerPoolEntry(quizWrongPoolEntry);
			}
		}
		foreach (DataRow dr5 in UserDac.WisdomTemplePuzzles(conn, null))
		{
			WisdomTemplePuzzle puzzle = new WisdomTemplePuzzle(m_wisdomTemple);
			puzzle.Set(dr5);
			m_wisdomTemple.AddPuzzle(puzzle);
		}
		foreach (DataRow dr4 in UserDac.WisdomTempleStepRewards(conn, null))
		{
			WisdomTempleStepReward stepReward = new WisdomTempleStepReward(m_wisdomTemple);
			stepReward.Set(dr4);
			m_wisdomTemple.AddStepReward(stepReward);
		}
		foreach (DataRow dr3 in UserDac.WisdomTempleFindTreasureBoxCounts(conn, null))
		{
			WisdomTempleFindTreasureBoxCount findTreasureBoxCount = new WisdomTempleFindTreasureBoxCount(m_wisdomTemple);
			findTreasureBoxCount.Set(dr3);
			m_wisdomTemple.AddFindTreasureBoxCount(findTreasureBoxCount);
		}
		foreach (DataRow dr2 in UserDac.WisdomTemplePuzzleRewardObjectOffsets(conn, null))
		{
			WisdomTemplePuzzleRewardObjectOffset puzzleRewardObjectOffset = new WisdomTemplePuzzleRewardObjectOffset(m_wisdomTemple);
			puzzleRewardObjectOffset.Set(dr2);
			m_wisdomTemple.AddPuzzleRewardObjectOffset(puzzleRewardObjectOffset);
		}
		foreach (DataRow dr in UserDac.WisdomTemplePuzzleRewardPoolEntries(conn, null))
		{
			WisdomTemplePuzzleRewardPoolEntry puzzleRewardPoolEntry = new WisdomTemplePuzzleRewardPoolEntry(m_wisdomTemple);
			puzzleRewardPoolEntry.Set(dr);
			m_wisdomTemple.AddPuzzleRewardPoolEntry(puzzleRewardPoolEntry);
		}
	}

	private void LoadUserDBResources_Place_RuinsReclaim(SqlConnection conn)
	{
		DataRow drRuinsReclaim = UserDac.RuinsReclaim(conn, null);
		if (drRuinsReclaim == null)
		{
			SFLogUtil.Warn(GetType(), "[유적탈환] 유적탈환이 존재하지 않습니다.");
			return;
		}
		RuinsReclaim ruinsReclaim = new RuinsReclaim();
		ruinsReclaim.Set(drRuinsReclaim);
		m_ruinsReclaim = ruinsReclaim;
		AddLocation(ruinsReclaim);
		foreach (DataRow dr5 in UserDac.RuinsReclaimMonsterAttrFactors(conn, null))
		{
			RuinsReclaimMonsterAttrFactor monsterAttrFactor = new RuinsReclaimMonsterAttrFactor(m_ruinsReclaim);
			monsterAttrFactor.Set(dr5);
			m_ruinsReclaim.AddMonsterAttrFactor(monsterAttrFactor);
		}
		foreach (DataRow dr8 in UserDac.RuinsReclaimRevivalPoints(conn, null))
		{
			RuinsReclaimRevivalPoint revivalPoint = new RuinsReclaimRevivalPoint(m_ruinsReclaim);
			revivalPoint.Set(dr8);
			m_ruinsReclaim.AddRevivalPoint(revivalPoint);
		}
		foreach (DataRow dr11 in UserDac.RuinsReclaimTraps(conn, null))
		{
			RuinsReclaimTrap trap = new RuinsReclaimTrap(m_ruinsReclaim);
			trap.Set(dr11);
			m_ruinsReclaim.AddTrap(trap);
		}
		foreach (DataRow dr15 in UserDac.RuinsReclaimPortals(conn, null))
		{
			RuinsReclaimPortal portal = new RuinsReclaimPortal(m_ruinsReclaim);
			portal.Set(dr15);
			m_ruinsReclaim.AddPortal(portal);
		}
		foreach (DataRow dr17 in UserDac.RuinsReclaimOpenSchedules(conn, null))
		{
			RuinsReclaimOpenSchedule openSchedule = new RuinsReclaimOpenSchedule(m_ruinsReclaim);
			openSchedule.Set(dr17);
			m_ruinsReclaim.AddOpenSchedule(openSchedule);
		}
		foreach (DataRow dr16 in UserDac.RuinsReclaimSteps(conn, null))
		{
			RuinsReclaimStep step5 = new RuinsReclaimStep(m_ruinsReclaim);
			step5.Set(dr16);
			m_ruinsReclaim.AddStep(step5);
		}
		foreach (DataRow dr14 in UserDac.RuinsReclaimObjectArranges(conn, null))
		{
			int nStepNo4 = Convert.ToInt32(dr14["stepNo"]);
			RuinsReclaimStep step4 = m_ruinsReclaim.GetStep(nStepNo4);
			if (step4 == null)
			{
				SFLogUtil.Warn(GetType(), "[유적탈환오브젝트배치 목록] 유적탈환단계가 존재하지 않습니다. nStepNo = " + nStepNo4);
				continue;
			}
			RuinsReclaimObjectArrange objectArrange = new RuinsReclaimObjectArrange(step4);
			objectArrange.Set(dr14);
			step4.AddObjectArrange(objectArrange);
		}
		foreach (DataRow dr13 in UserDac.RuinsReclaimStepRewards(conn, null))
		{
			int nStepNo3 = Convert.ToInt32(dr13["stepNo"]);
			RuinsReclaimStep step3 = m_ruinsReclaim.GetStep(nStepNo3);
			if (step3 == null)
			{
				SFLogUtil.Warn(GetType(), "[유적탈환단계보상 목록] 유적탈환단계가 존재하지 않습니다. nStepNo = " + nStepNo3);
				continue;
			}
			RuinsReclaimStepReward stepReward = new RuinsReclaimStepReward(step3);
			stepReward.Set(dr13);
			step3.AddReward(stepReward);
		}
		foreach (DataRow dr12 in UserDac.RuinsReclaimStepWaves(conn, null))
		{
			int nStepNo2 = Convert.ToInt32(dr12["stepNo"]);
			RuinsReclaimStep step2 = m_ruinsReclaim.GetStep(nStepNo2);
			if (step2 == null)
			{
				SFLogUtil.Warn(GetType(), "[유적탈환단계웨이브 목록] 유적탈환단계가 존재하지 않습니다. nStepNo = " + nStepNo2);
				continue;
			}
			RuinsReclaimStepWave wave2 = new RuinsReclaimStepWave(step2);
			wave2.Set(dr12);
			step2.AddWave(wave2);
		}
		foreach (DataRow dr10 in UserDac.RuinsReclaimStepWaveSkills(conn, null))
		{
			int nStepNo = Convert.ToInt32(dr10["stepNo"]);
			RuinsReclaimStep step = m_ruinsReclaim.GetStep(nStepNo);
			if (step == null)
			{
				SFLogUtil.Warn(GetType(), "[유적탈환단계웨이브스킬 목록] 유적탈환단계가 존재하지 않습니다. nStepNo = " + nStepNo);
				continue;
			}
			int nWaveNo = Convert.ToInt32(dr10["waveNo"]);
			RuinsReclaimStepWave wave = step.GetWave(nWaveNo);
			if (wave == null)
			{
				SFLogUtil.Warn(GetType(), "[유적탈환단계웨이브스킬 목록] 유적탈환단계웨이브가 존재하지 않습니다. nStepNo = " + nStepNo + ", nWaveNo = " + nWaveNo);
			}
			else
			{
				RuinsReclaimStepWaveSkill skill = new RuinsReclaimStepWaveSkill(wave);
				skill.Set(dr10);
				wave.SetSkill(skill);
			}
		}
		foreach (DataRow dr9 in UserDac.RuinsReclaimMonsterArranges(conn, null))
		{
			RuinsReclaimMonsterArrange monsterArrange = new RuinsReclaimMonsterArrange(m_ruinsReclaim);
			monsterArrange.Set(dr9);
			m_ruinsReclaim.AddMonsterArrange(monsterArrange);
			monsterArrange.wave?.AddMonsterArrange(monsterArrange);
		}
		foreach (DataRow dr7 in UserDac.RuinsReclaimSummonMonsterArranges(conn, null))
		{
			int nArrangeKey = Convert.ToInt32(dr7["arrangeKey"]);
			RuinsReclaimMonsterArrange arrange = m_ruinsReclaim.GetMonsterArrange(nArrangeKey);
			if (arrange == null)
			{
				SFLogUtil.Warn(GetType(), "[유적탈환소환몬스터배치 목록] 유적탈환몬스터배치가 존재하지 않습니다. nArrangeKey = " + nArrangeKey);
				continue;
			}
			RuinsReclaimSummonMonsterArrange summonMonsterArrange = new RuinsReclaimSummonMonsterArrange(arrange);
			summonMonsterArrange.Set(dr7);
			arrange.AddSummonMonsterArrange(summonMonsterArrange);
		}
		foreach (DataRow dr6 in UserDac.RuinsReclaimRandomRewardPoolEntries(conn, null))
		{
			RuinsReclaimRandomRewardPoolEntry randomRewardPoolEntry = new RuinsReclaimRandomRewardPoolEntry(m_ruinsReclaim);
			randomRewardPoolEntry.Set(dr6);
			m_ruinsReclaim.AddRandomRewardPoolEntry(randomRewardPoolEntry);
		}
		foreach (DataRow dr4 in UserDac.RuinsReclaimRewards(conn, null))
		{
			RuinsReclaimReward reward = new RuinsReclaimReward(m_ruinsReclaim);
			reward.Set(dr4);
			m_ruinsReclaim.AddReward(reward);
		}
		foreach (DataRow dr3 in UserDac.RuinsReclaimMonsterTerminatorRewardPoolEntries(conn, null))
		{
			RuinsReclaimMonsterTerminatorRewardPoolEntry monsterTerminatorRewardPoolEntry = new RuinsReclaimMonsterTerminatorRewardPoolEntry(m_ruinsReclaim);
			monsterTerminatorRewardPoolEntry.Set(dr3);
			m_ruinsReclaim.AddMonsterTerminatorRewardPoolEntry(monsterTerminatorRewardPoolEntry);
		}
		foreach (DataRow dr2 in UserDac.RuinsReclaimUltimateAttackKingRewardPoolEntries(conn, null))
		{
			RuinsReclaimUltimateAttackKingRewardPoolEntry ultimateAttackKingRewardPoolEntry = new RuinsReclaimUltimateAttackKingRewardPoolEntry(m_ruinsReclaim);
			ultimateAttackKingRewardPoolEntry.Set(dr2);
			m_ruinsReclaim.AddUltimateAttackKingRewardPoolEntry(ultimateAttackKingRewardPoolEntry);
		}
		foreach (DataRow dr in UserDac.RuinsReclaimPartyVolunteerRewardPoolEntries(conn, null))
		{
			RuinsReclaimPartyVolunteerRewardPoolEntry partyVolunteerRewardPoolEntry = new RuinsReclaimPartyVolunteerRewardPoolEntry(m_ruinsReclaim);
			partyVolunteerRewardPoolEntry.Set(dr);
			m_ruinsReclaim.AddPartyVolunteerRewardPoolEntry(partyVolunteerRewardPoolEntry);
		}
	}

	private void LoadUserDBResources_Place_InfiniteWar(SqlConnection conn)
	{
		DataRow drInfiniteWar = UserDac.InfiniteWar(conn, null);
		if (drInfiniteWar == null)
		{
			SFLogUtil.Warn(GetType(), "[무한대전] 무한대전이 존재하지 않습니다.");
			return;
		}
		InfiniteWar infiniteWar = new InfiniteWar();
		infiniteWar.Set(drInfiniteWar);
		m_infiniteWar = infiniteWar;
		AddLocation(infiniteWar);
		foreach (DataRow dr4 in UserDac.InfiniteWarBuffBoxs(conn, null))
		{
			InfiniteWarBuffBox buffBox = new InfiniteWarBuffBox(m_infiniteWar);
			buffBox.Set(dr4);
			m_infiniteWar.AddBuffBox(buffBox);
		}
		foreach (DataRow dr7 in UserDac.InfiniteWarMonsterAttrFactors(conn, null))
		{
			InfiniteWarMonsterAttrFactor monsterAttrFactor = new InfiniteWarMonsterAttrFactor(m_infiniteWar);
			monsterAttrFactor.Set(dr7);
			m_infiniteWar.AddMonsterAttrFactor(monsterAttrFactor);
		}
		foreach (DataRow dr6 in UserDac.InfiniteWarMonsterArranges(conn, null))
		{
			InfiniteWarMonsterArrange monsterArrange = new InfiniteWarMonsterArrange(m_infiniteWar);
			monsterArrange.Set(dr6);
			m_infiniteWar.AddMonsterArrange(monsterArrange);
		}
		foreach (DataRow dr5 in UserDac.InfiniteWarOpenSchedules(conn, null))
		{
			InfiniteWarOpenSchedule openSchedule = new InfiniteWarOpenSchedule(m_infiniteWar);
			openSchedule.Set(dr5);
			m_infiniteWar.AddOpenSchedule(openSchedule);
		}
		foreach (DataRow dr3 in UserDac.InfiniteWarStartPositions(conn, null))
		{
			InfiniteWarStartPosition startPosition = new InfiniteWarStartPosition(m_infiniteWar);
			startPosition.Set(dr3);
			m_infiniteWar.AddStartPosition(startPosition);
		}
		foreach (DataRow dr2 in UserDac.InfiniteWarRewards(conn, null))
		{
			InfiniteWarReward reward = new InfiniteWarReward(m_infiniteWar);
			reward.Set(dr2);
			m_infiniteWar.AddReward(reward);
		}
		foreach (DataRow dr in UserDac.InfiniteWarRankingRewards(conn, null))
		{
			InfiniteWarRankingReward rankingReward = new InfiniteWarRankingReward(m_infiniteWar);
			rankingReward.Set(dr);
			m_infiniteWar.AddRankingReward(rankingReward);
		}
	}

	private void LoadUserDBResources_Place_FearAltar(SqlConnection conn)
	{
		DataRow drFearAltar = UserDac.FearAltar(conn, null);
		if (drFearAltar == null)
		{
			SFLogUtil.Warn(GetType(), "[공포의제단] 공포의제단이 존재하지 않습니다.");
			return;
		}
		FearAltar fearAltar = new FearAltar();
		fearAltar.Set(drFearAltar);
		m_fearAltar = fearAltar;
		foreach (DataRow dr3 in UserDac.FearAltarMonsterAttrFactors(conn, null))
		{
			FearAltarMonsterAttrFactor monsterAttrFactor = new FearAltarMonsterAttrFactor(m_fearAltar);
			monsterAttrFactor.Set(dr3);
			m_fearAltar.AddMonsterAttrFactor(monsterAttrFactor);
		}
		foreach (DataRow dr7 in UserDac.FearAltarRewards(conn, null))
		{
			FearAltarReward reward = new FearAltarReward(m_fearAltar);
			reward.Set(dr7);
			m_fearAltar.AddReward(reward);
		}
		foreach (DataRow dr9 in UserDac.FearAltarHalidomCollectionRewards(conn, null))
		{
			FearAltarHalidomCollectionReward halidomCollectionReward = new FearAltarHalidomCollectionReward(m_fearAltar);
			halidomCollectionReward.Set(dr9);
			m_fearAltar.AddHalidomCollectionReward(halidomCollectionReward);
		}
		foreach (DataRow dr8 in UserDac.FearAltarHalidomElementals(conn, null))
		{
			FearAltarHalidomElemental halidomElemental = new FearAltarHalidomElemental(m_fearAltar);
			halidomElemental.Set(dr8);
			m_fearAltar.AddHalidomElemental(halidomElemental);
		}
		foreach (DataRow dr6 in UserDac.FearAltarHalidomLevels(conn, null))
		{
			FearAltarHalidomLevel halidomLevel = new FearAltarHalidomLevel(m_fearAltar);
			halidomLevel.Set(dr6);
			m_fearAltar.AddHalidomLevel(halidomLevel);
		}
		foreach (DataRow dr5 in UserDac.FearAltarHalidoms(conn, null))
		{
			FearAltarHalidom halidom = new FearAltarHalidom(m_fearAltar);
			halidom.Set(dr5);
			m_fearAltar.AddHalidom(halidom);
		}
		foreach (DataRow dr4 in UserDac.FearAltarStages(conn, null))
		{
			FearAltarStage stage2 = new FearAltarStage(m_fearAltar);
			stage2.Set(dr4);
			m_fearAltar.AddStage(stage2);
			AddLocation(stage2);
		}
		foreach (DataRow dr2 in UserDac.FearAltarStageWaves(conn, null))
		{
			int nStageId = Convert.ToInt32(dr2["stageId"]);
			FearAltarStage stage = m_fearAltar.GetStage(nStageId);
			if (stage == null)
			{
				SFLogUtil.Warn(GetType(), "[공포의제단스테이지웨이브 목록] 스테이지가 존재하지 않습니다. nStageId = " + nStageId);
				continue;
			}
			FearAltarStageWave wave = new FearAltarStageWave(stage);
			wave.Set(dr2);
			stage.AddWave(wave);
		}
		foreach (DataRow dr in UserDac.FearAltarStageWaveMonsterArranges(conn, null))
		{
			FearAltarStageWaveMonsterArrange monsterArrange = new FearAltarStageWaveMonsterArrange(m_fearAltar);
			monsterArrange.Set(dr);
			m_fearAltar.AddMonsterArrange(monsterArrange);
			monsterArrange.wave?.AddMonsterArrange(monsterArrange);
		}
	}

	private void LoadUserDBResources_Place_WarMemory(SqlConnection conn)
	{
		DataRow drWarMemory = UserDac.WarMemory(conn, null);
		if (drWarMemory == null)
		{
			SFLogUtil.Warn(GetType(), "[전쟁의기억] 전쟁의기억이 존재하지 않습니다.");
			return;
		}
		WarMemory warMemory = new WarMemory();
		warMemory.Set(drWarMemory);
		m_warMemory = warMemory;
		AddLocation(warMemory);
		foreach (DataRow dr3 in UserDac.WarMemoryMonsterAttrFactors(conn, null))
		{
			WarMemoryMonsterAttrFactor monsterAttrFactor = new WarMemoryMonsterAttrFactor(m_warMemory);
			monsterAttrFactor.Set(dr3);
			m_warMemory.AddMonsterAttrFactor(monsterAttrFactor);
		}
		foreach (DataRow dr7 in UserDac.WarMemoryStartPositions(conn, null))
		{
			WarMemoryStartPosition startPosition = new WarMemoryStartPosition(m_warMemory);
			startPosition.Set(dr7);
			m_warMemory.AddStartPosition(startPosition);
		}
		foreach (DataRow dr9 in UserDac.WarMemorySchedules(conn, null))
		{
			WarMemorySchedule schedule = new WarMemorySchedule(m_warMemory);
			schedule.Set(dr9);
			m_warMemory.AddSchedule(schedule);
		}
		foreach (DataRow dr8 in UserDac.WarMemoryRewards(conn, null))
		{
			WarMemoryReward reward = new WarMemoryReward(m_warMemory);
			reward.Set(dr8);
			m_warMemory.AddReward(reward);
		}
		foreach (DataRow dr6 in UserDac.WarMemoryRankingRewards(conn, null))
		{
			WarMemoryRankingReward rankingReward = new WarMemoryRankingReward(m_warMemory);
			rankingReward.Set(dr6);
			m_warMemory.AddRankingReward(rankingReward);
		}
		foreach (DataRow dr5 in UserDac.WarMemoryWaves(conn, null))
		{
			WarMemoryWave wave2 = new WarMemoryWave(m_warMemory);
			wave2.Set(dr5);
			m_warMemory.AddWave(wave2);
		}
		foreach (DataRow dr4 in UserDac.WarMemoryTransformationObjects(conn, null))
		{
			int nWaveNo = Convert.ToInt32(dr4["waveNo"]);
			WarMemoryWave wave = m_warMemory.GetWave(nWaveNo);
			if (wave == null)
			{
				SFLogUtil.Warn(GetType(), "[전쟁의기억변신오브젝트 목록] 웨이브가 존재하지 않습니다. nWaveNo = " + nWaveNo);
				continue;
			}
			WarMemoryTransformationObject transformationObject = new WarMemoryTransformationObject(wave);
			transformationObject.Set(dr4);
			wave.AddTransformationObject(transformationObject);
		}
		foreach (DataRow dr2 in UserDac.WarMemoryMonsterArranges(conn, null))
		{
			WarMemoryMonsterArrange monsterArrange2 = new WarMemoryMonsterArrange(m_warMemory);
			monsterArrange2.Set(dr2);
			m_warMemory.AddMonsterArrange(monsterArrange2);
			monsterArrange2.wave?.AddMonsterArrange(monsterArrange2);
		}
		foreach (DataRow dr in UserDac.WarMemorySummonMonsterArranges(conn, null))
		{
			int nArrangeKey = Convert.ToInt32(dr["arrangeKey"]);
			WarMemoryMonsterArrange monsterArrange = m_warMemory.GetMonsterArrange(nArrangeKey);
			if (monsterArrange == null)
			{
				SFLogUtil.Warn(GetType(), "[전쟁의기억소환몬스터배치 목록] 몬스터배치가 존재하지 않습니다. nArrangeKey = " + nArrangeKey);
				continue;
			}
			WarMemorySummonMonsterArrange summonMonsterArrange = new WarMemorySummonMonsterArrange(monsterArrange);
			summonMonsterArrange.Set(dr);
			monsterArrange.AddSummonMonsterArrange(summonMonsterArrange);
		}
	}

	private void LoadUserDBResources_Place_OsirisRoom(SqlConnection conn)
	{
		DataRow drOsirisRoom = UserDac.OsirisRoom(conn, null);
		if (drOsirisRoom == null)
		{
			SFLogUtil.Warn(GetType(), "[오시리스의방] 오시리스의방이 존재하지 않습니다.");
			return;
		}
		OsirisRoom osirisRoom = new OsirisRoom();
		osirisRoom.Set(drOsirisRoom);
		m_osirisRoom = osirisRoom;
		AddLocation(osirisRoom);
		foreach (DataRow dr3 in UserDac.OsirisRoomDifficulties(conn, null))
		{
			OsirisRoomDifficulty difficulty3 = new OsirisRoomDifficulty(m_osirisRoom);
			difficulty3.Set(dr3);
			m_osirisRoom.AddDifficulty(difficulty3);
		}
		foreach (DataRow dr2 in UserDac.OsirisRoomDifficultyWaves(conn, null))
		{
			int nDifficulty2 = Convert.ToInt32(dr2["difficulty"]);
			OsirisRoomDifficulty difficulty2 = m_osirisRoom.GetDifficulty(nDifficulty2);
			if (difficulty2 == null)
			{
				SFLogUtil.Warn(GetType(), "[오시리스의방난이도웨이브 목록] 오시리스의방난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty2);
				continue;
			}
			OsirisRoomDifficultyWave wave2 = new OsirisRoomDifficultyWave(difficulty2);
			wave2.Set(dr2);
			difficulty2.AddWave(wave2);
		}
		foreach (DataRow dr in UserDac.OsirisRoomMonsterArranges(conn, null))
		{
			int nDifficulty = Convert.ToInt32(dr["difficulty"]);
			OsirisRoomDifficulty difficulty = m_osirisRoom.GetDifficulty(nDifficulty);
			if (difficulty == null)
			{
				SFLogUtil.Warn(GetType(), "[오시리스의방몬스터배치 목록] 오시리스의방난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty);
				continue;
			}
			int nWaveNo = Convert.ToInt32(dr["waveNo"]);
			OsirisRoomDifficultyWave wave = difficulty.GetWave(nWaveNo);
			if (wave == null)
			{
				SFLogUtil.Warn(GetType(), "[오시리스의방몬스터배치 목록] 오시리스의방난이도웨이브가 존재하지 않습니다. nDifficulty = " + nDifficulty + ", nWaveNo = " + nWaveNo);
			}
			else
			{
				OsirisRoomMonsterArrange monsterArrange = new OsirisRoomMonsterArrange(wave);
				monsterArrange.Set(dr);
				wave.AddMonsterArrange(monsterArrange);
			}
		}
	}

	private void LoadUserDBResources_Place_BiographyQuestDungeon(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.BiographyQuestDungeons(conn, null))
		{
			BiographyQuestDungeon dungeon3 = new BiographyQuestDungeon();
			dungeon3.Set(dr2);
			AddBiographyQuestDungeon(dungeon3);
		}
		foreach (DataRow dr3 in UserDac.BiographyQuestDungeonWaves(conn, null))
		{
			int nDungeonId2 = Convert.ToInt32(dr3["dungeonId"]);
			BiographyQuestDungeon dungeon2 = GetBiographyQuestDungeon(nDungeonId2);
			if (dungeon2 == null)
			{
				SFLogUtil.Warn(GetType(), "[전기퀘스트던전웨이브 목록] 전기퀘스트던전이 존재하지 않습니다. nDungeonId = " + nDungeonId2);
				continue;
			}
			BiographyQuestDungeonWave wave2 = new BiographyQuestDungeonWave(dungeon2);
			wave2.Set(dr3);
			dungeon2.AddWave(wave2);
		}
		foreach (DataRow dr in UserDac.BiographyQuestMonsterArranges(conn, null))
		{
			int nDungeonId = Convert.ToInt32(dr["dungeonId"]);
			BiographyQuestDungeon dungeon = GetBiographyQuestDungeon(nDungeonId);
			if (dungeon == null)
			{
				SFLogUtil.Warn(GetType(), "[전기퀘스트몬스터배치 목록] 전기퀘스트던전이 존재하지 않습니다. nDungeonId = " + nDungeonId);
				continue;
			}
			int nWaveNo = Convert.ToInt32(dr["waveNo"]);
			BiographyQuestDungeonWave wave = dungeon.GetWave(nWaveNo);
			if (wave == null)
			{
				SFLogUtil.Warn(GetType(), "[전기퀘스트몬스터배치 목록] 전기퀘스트던전웨이브가 존재하지 않습니다. nDungeonId = " + nDungeonId + ", nWaveNo = " + nWaveNo);
			}
			else
			{
				BiographyQuestMonsterArrange arrange = new BiographyQuestMonsterArrange(wave);
				arrange.Set(dr);
				wave.AddMonsterArrange(arrange);
			}
		}
	}

	private void LoadUserDBResources_Place_DragonNest(SqlConnection conn)
	{
		DataRow drDragonNest = UserDac.DragonNest(conn, null);
		if (drDragonNest == null)
		{
			SFLogUtil.Warn(GetType(), "[용의둥지] 용의둥지가 존재하지 않습니다.");
			return;
		}
		DragonNest dragonNest = new DragonNest();
		dragonNest.Set(drDragonNest);
		m_dragonNest = dragonNest;
		AddLocation(dragonNest);
		foreach (DataRow dr4 in UserDac.DragonNestMonsterAttrFactors(conn, null))
		{
			DragonNestMonsterAttrFactor monsterAttrFactor = new DragonNestMonsterAttrFactor(m_dragonNest);
			monsterAttrFactor.Set(dr4);
			m_dragonNest.AddMonsterAttrFactor(monsterAttrFactor);
		}
		foreach (DataRow dr5 in UserDac.DragonNestTraps(conn, null))
		{
			DragonNestTrap trap = new DragonNestTrap(m_dragonNest);
			trap.Set(dr5);
			m_dragonNest.AddTrap(trap);
		}
		foreach (DataRow dr3 in UserDac.DragonNestSteps(conn, null))
		{
			DragonNestStep step3 = new DragonNestStep(m_dragonNest);
			step3.Set(dr3);
			m_dragonNest.AddStep(step3);
		}
		foreach (DataRow dr2 in UserDac.DragonNestStepReward(conn, null))
		{
			int nStepNo2 = Convert.ToInt32(dr2["stepNo"]);
			DragonNestStep step2 = m_dragonNest.GetStep(nStepNo2);
			if (step2 == null)
			{
				SFLogUtil.Warn(GetType(), "[용의둥지단계보상 목록] 용의둥지단계가 존재하지 않습니다. nStepNo = " + nStepNo2);
				continue;
			}
			DragonNestStepReward reward = new DragonNestStepReward(step2);
			reward.Set(dr2);
			step2.AddReward(reward);
		}
		foreach (DataRow dr in UserDac.DragonNestMonsterArranges(conn, null))
		{
			int nStepNo = Convert.ToInt32(dr["stepNo"]);
			DragonNestStep step = m_dragonNest.GetStep(nStepNo);
			if (step == null)
			{
				SFLogUtil.Warn(GetType(), "[용의둥지몬스터배치 목록] 용의둥지단계가 존재하지 않습니다. nStepNo = " + nStepNo);
				continue;
			}
			DragonNestMonsterArrange monsterArrange = new DragonNestMonsterArrange(step);
			monsterArrange.Set(dr);
			step.AddMonsterArrange(monsterArrange);
		}
	}

	private void LoadUserDBResources_Place_AnkouTomb(SqlConnection conn)
	{
		DataRow drAnkouTomb = UserDac.AnkouTomb(conn, null);
		if (drAnkouTomb == null)
		{
			SFLogUtil.Warn(GetType(), "[안쿠의무덤] 안쿠의무덤이 존재하지 않습니다.");
			return;
		}
		AnkouTomb ankouTomb = new AnkouTomb();
		ankouTomb.Set(drAnkouTomb);
		m_ankouTomb = ankouTomb;
		AddLocation(ankouTomb);
		foreach (DataRow dr4 in UserDac.AnkouTombSchedules(conn, null))
		{
			AnkouTombSchedule schedule = new AnkouTombSchedule(m_ankouTomb);
			schedule.Set(dr4);
			m_ankouTomb.AddSchedule(schedule);
		}
		foreach (DataRow dr3 in UserDac.AnkouTombDifficulty(conn, null))
		{
			AnkouTombDifficulty difficulty3 = new AnkouTombDifficulty(m_ankouTomb);
			difficulty3.Set(dr3);
			m_ankouTomb.AddDifficulty(difficulty3);
		}
		foreach (DataRow dr2 in UserDac.AnkouTombRewardPoolEntries(conn, null))
		{
			int nDifficulty2 = Convert.ToInt32(dr2["difficulty"]);
			AnkouTombDifficulty difficulty2 = m_ankouTomb.GetDifficulty(nDifficulty2);
			if (difficulty2 == null)
			{
				SFLogUtil.Warn(GetType(), "[안쿠의무덤보상풀항목 목록] 난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty2);
				continue;
			}
			AnkouTombRewardPoolEntry rewardPoolEntry = new AnkouTombRewardPoolEntry(difficulty2);
			rewardPoolEntry.Set(dr2);
			difficulty2.AddRewardPoolEntry(rewardPoolEntry);
		}
		foreach (DataRow dr in UserDac.AnkouTombMonsterArranges(conn, null))
		{
			int nDifficulty = Convert.ToInt32(dr["difficulty"]);
			AnkouTombDifficulty difficulty = m_ankouTomb.GetDifficulty(nDifficulty);
			if (difficulty == null)
			{
				SFLogUtil.Warn(GetType(), "[안쿠의무덤몬스터배치 목록] 난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty);
				continue;
			}
			int nWaveNo = Convert.ToInt32(dr["waveNo"]);
			AnkouTombWave wave = difficulty.GetWave(nWaveNo);
			if (wave == null)
			{
				SFLogUtil.Warn(GetType(), "[안쿠의무덤몬스터배치 목록] 웨이브번호가 유효하지 않습니다. nDifficulty = " + nDifficulty + ", nWaveNo = " + nWaveNo);
			}
			else
			{
				AnkouTombMonsterArrange monsterArrange = new AnkouTombMonsterArrange(wave);
				monsterArrange.Set(dr);
				wave.AddMonsterArrange(monsterArrange);
			}
		}
	}

	private void LoadUserDBResources_Place_TradeShip(SqlConnection conn)
	{
		DataRow drTradeShip = UserDac.TradeShip(conn, null);
		if (drTradeShip == null)
		{
			SFLogUtil.Warn(GetType(), "[무역선탈환] 무역선탈환이 존재하지 않습니다.");
			return;
		}
		TradeShip tradeShip = new TradeShip();
		tradeShip.Set(drTradeShip);
		m_tradeShip = tradeShip;
		foreach (DataRow dr5 in UserDac.TradeShipSchedules(conn, null))
		{
			TradeShipSchedule schedule = new TradeShipSchedule(m_tradeShip);
			schedule.Set(dr5);
			m_tradeShip.AddSchedule(schedule);
		}
		foreach (DataRow dr8 in UserDac.TradeShipSteps(conn, null))
		{
			TradeShipStep step4 = new TradeShipStep(m_tradeShip);
			step4.Set(dr8);
			m_tradeShip.AddStep(step4);
		}
		foreach (DataRow dr9 in UserDac.TradeShipDifficulties(conn, null))
		{
			TradeShipDifficulty difficulty7 = new TradeShipDifficulty(m_tradeShip);
			difficulty7.Set(dr9);
			m_tradeShip.AddDifficulty(difficulty7);
		}
		foreach (DataRow dr7 in UserDac.TradeShipRewardPoolEntries(conn, null))
		{
			int nDifficulty6 = Convert.ToInt32(dr7["difficulty"]);
			TradeShipDifficulty difficulty6 = m_tradeShip.GetDifficulty(nDifficulty6);
			if (difficulty6 == null)
			{
				SFLogUtil.Warn(GetType(), "[무역선탈환보상풀항목 목록] 무역선탈환난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty6);
				continue;
			}
			TradeShipRewardPoolEntry rewardPoolEntry = new TradeShipRewardPoolEntry(difficulty6);
			rewardPoolEntry.Set(dr7);
			difficulty6.AddRewardPoolEntry(rewardPoolEntry);
		}
		foreach (DataRow dr6 in UserDac.TradeShipMonsterArranges(conn, null))
		{
			int nDifficulty5 = Convert.ToInt32(dr6["difficulty"]);
			TradeShipDifficulty difficulty5 = m_tradeShip.GetDifficulty(nDifficulty5);
			if (difficulty5 == null)
			{
				SFLogUtil.Warn(GetType(), "[무역선탈환몬스터배치 목록] 무역선탈환난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty5);
				continue;
			}
			int nStepNo3 = Convert.ToInt32(dr6["stepNo"]);
			TradeShipDifficultyStep step3 = difficulty5.GetStep(nStepNo3);
			if (step3 == null)
			{
				SFLogUtil.Warn(GetType(), "[무역선탈환몬스터배치 목록] 무역선탈환난이도단계가 존재하지 않습니다. nDifficulty = " + nDifficulty5 + ", nStepNo = " + nStepNo3);
			}
			else
			{
				TradeShipMonsterArrange monsterArrange = new TradeShipMonsterArrange(step3);
				monsterArrange.Set(dr6);
				step3.AddMonsterArrange(monsterArrange);
			}
		}
		foreach (DataRow dr4 in UserDac.TradeShipAdditionalMonsterArrangePoolEntries(conn, null))
		{
			int nDifficulty4 = Convert.ToInt32(dr4["difficulty"]);
			TradeShipDifficulty difficulty4 = m_tradeShip.GetDifficulty(nDifficulty4);
			if (difficulty4 == null)
			{
				SFLogUtil.Warn(GetType(), "[무역선탈환추가몬스터배치풀항목 목록] 무역선탈환난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty4);
				continue;
			}
			int nStepNo2 = Convert.ToInt32(dr4["stepNo"]);
			TradeShipDifficultyStep step2 = difficulty4.GetStep(nStepNo2);
			if (step2 == null)
			{
				SFLogUtil.Warn(GetType(), "[무역선탈환추가몬스터배치풀항목 목록] 무역선탈환난이도단계가 존재하지 않습니다. nDifficulty = " + nDifficulty4 + ", nStepNo = " + nStepNo2);
			}
			else
			{
				TradeShipAdditionalMonsterArrangePoolEntry additionalMonsterArrangePoolEntry2 = new TradeShipAdditionalMonsterArrangePoolEntry(step2);
				additionalMonsterArrangePoolEntry2.Set(dr4);
				step2.AddAdditionalMonsterArrangePoolEntry(additionalMonsterArrangePoolEntry2);
			}
		}
		foreach (DataRow dr3 in UserDac.TradeShipAdditionalMonsterArranges(conn, null))
		{
			int nDifficulty3 = Convert.ToInt32(dr3["difficulty"]);
			TradeShipDifficulty difficulty3 = m_tradeShip.GetDifficulty(nDifficulty3);
			if (difficulty3 == null)
			{
				SFLogUtil.Warn(GetType(), "[무역선탈환추가몬스터배치 목록] 무역선탈환난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty3);
				continue;
			}
			int nStepNo = Convert.ToInt32(dr3["stepNo"]);
			TradeShipDifficultyStep step = difficulty3.GetStep(nStepNo);
			if (step == null)
			{
				SFLogUtil.Warn(GetType(), "[무역선탈환추가몬스터배치 목록] 무역선탈환난이도단계가 존재하지 않습니다. nDifficulty = " + nDifficulty3 + ", nStepNo = " + nStepNo);
				continue;
			}
			int nEntryNo = Convert.ToInt32(dr3["entryNo"]);
			TradeShipAdditionalMonsterArrangePoolEntry additionalMonsterArrangePoolEntry = step.GetAdditionalMonsterArrangePoolEntry(nEntryNo);
			if (additionalMonsterArrangePoolEntry == null)
			{
				SFLogUtil.Warn(GetType(), "[무역선탈환추가몬스터배치 목록] 무역선탈환추가몬스터배치풀항목이 존재하지 않습니다. nDifficulty = " + nDifficulty3 + ", nStepNo = " + nStepNo);
			}
			else
			{
				TradeShipAdditionalMonsterArrange additionalMonsterArrange = new TradeShipAdditionalMonsterArrange(additionalMonsterArrangePoolEntry);
				additionalMonsterArrange.Set(dr3);
				additionalMonsterArrangePoolEntry.AddMonsterArrange(additionalMonsterArrange);
			}
		}
		foreach (DataRow dr2 in UserDac.TradeShipObjects(conn, null))
		{
			int nDifficulty2 = Convert.ToInt32(dr2["difficulty"]);
			TradeShipDifficulty difficulty2 = m_tradeShip.GetDifficulty(nDifficulty2);
			if (difficulty2 == null)
			{
				SFLogUtil.Warn(GetType(), "[무역선탈환오브젝트 목록] 무역선탈환난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty2);
				continue;
			}
			TradeShipObject obj2 = new TradeShipObject(difficulty2);
			obj2.Set(dr2);
			difficulty2.AddObject(obj2);
		}
		foreach (DataRow dr in UserDac.TradeShipObjectDestroyerRewards(conn, null))
		{
			int nDifficulty = Convert.ToInt32(dr["difficulty"]);
			TradeShipDifficulty difficulty = m_tradeShip.GetDifficulty(nDifficulty);
			if (difficulty == null)
			{
				SFLogUtil.Warn(GetType(), "[무역선탈환오브젝트파괴자보상 목록] 무역선탈환난이도가 존재하지 않습니다. nDifficulty = " + nDifficulty);
				continue;
			}
			int nObjectId = Convert.ToInt32(dr["objectId"]);
			TradeShipObject obj = difficulty.GetObject(nObjectId);
			if (obj == null)
			{
				SFLogUtil.Warn(GetType(), "[무역선탈환오브젝트파괴자보상 목록] 무역선탈환오브젝트가 존재하지 않습니다. nDifficulty = " + nDifficulty);
				continue;
			}
			TradeShipObjectDestroyerReward objectDestroyerReward = new TradeShipObjectDestroyerReward(obj);
			objectDestroyerReward.Set(dr);
			obj.AddDestroyerReward(objectDestroyerReward);
		}
	}

	private void LoadUserDBResources_Rank(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.Ranks(conn, null))
		{
			Rank rank = new Rank();
			rank.Set(dr2);
			m_ranks.Add(rank);
		}
		foreach (DataRow dr5 in UserDac.RankAttrs(conn, null))
		{
			int nRankNo = Convert.ToInt32(dr5["rankNo"]);
			Rank rank2 = GetRank(nRankNo);
			if (rank2 == null)
			{
				SFLogUtil.Warn(GetType(), "[계급속성 목록] 계급이 존재하지 않습니다. nRankNo = " + nRankNo);
				continue;
			}
			RankAttr attr = new RankAttr(rank2);
			attr.Set(dr5);
			rank2.AddAttr(attr);
		}
		foreach (DataRow dr9 in UserDac.RankRewards(conn, null))
		{
			int nRankNo2 = Convert.ToInt32(dr9["rankNo"]);
			Rank rank3 = GetRank(nRankNo2);
			if (rank3 == null)
			{
				SFLogUtil.Warn(GetType(), "[계급보상 목록] 계급이 존재하지 않습니다. nRankNo = " + nRankNo2);
				continue;
			}
			RankReward reward = new RankReward(rank3);
			reward.Set(dr9);
			rank3.AddReward(reward);
		}
		foreach (DataRow dr8 in UserDac.RankActiveSkills(conn, null))
		{
			RankActiveSkill rankActiveSkill2 = new RankActiveSkill();
			rankActiveSkill2.Set(dr8);
			AddRankActiveSkill(rankActiveSkill2);
			rankActiveSkill2.requiredRank?.AddActiveSkills(rankActiveSkill2);
		}
		foreach (DataRow dr7 in UserDac.RankActiveSkillLevels(conn, null))
		{
			int nSkillId3 = Convert.ToInt32(dr7["skillId"]);
			RankActiveSkill rankActiveSkill = GetRankActiveSkill(nSkillId3);
			if (rankActiveSkill == null)
			{
				SFLogUtil.Warn(GetType(), "[계급액티브스킬레벨 목록] 계급액티브스킬이 존재하지 않습니다. nSkillId = " + nSkillId3);
				continue;
			}
			RankActiveSkillLevel rankActiveSkillLevel = new RankActiveSkillLevel(rankActiveSkill);
			rankActiveSkillLevel.Set(dr7);
			rankActiveSkill.AddLevel(rankActiveSkillLevel);
		}
		foreach (DataRow dr6 in UserDac.RankPassiveSkills(conn, null))
		{
			RankPassiveSkill rankPassiveSkill3 = new RankPassiveSkill();
			rankPassiveSkill3.Set(dr6);
			AddRankPassiveSkill(rankPassiveSkill3);
			rankPassiveSkill3.requiredRank?.AddPassiveSkill(rankPassiveSkill3);
		}
		foreach (DataRow dr4 in UserDac.RankPassiveSkillAttrs(conn, null))
		{
			int nSkillId2 = Convert.ToInt32(dr4["skillId"]);
			RankPassiveSkill rankPassiveSkill2 = GetRankPassiveSkill(nSkillId2);
			if (rankPassiveSkill2 == null)
			{
				SFLogUtil.Warn(GetType(), "[계급패시브스킬속성 목록] 계급패시브스킬이 존재하지 않습니다. nSkillId = " + nSkillId2);
				continue;
			}
			RankPassiveSkillAttr rankPassiveSkillAttr2 = new RankPassiveSkillAttr(rankPassiveSkill2);
			rankPassiveSkillAttr2.Set(dr4);
			rankPassiveSkill2.AddAttr(rankPassiveSkillAttr2);
		}
		foreach (DataRow dr3 in UserDac.RankPassiveSkillLevels(conn, null))
		{
			RankPassiveSkillLevel rankPassiveSkillLevel2 = new RankPassiveSkillLevel();
			rankPassiveSkillLevel2.Set(dr3);
			rankPassiveSkillLevel2.skill?.AddLevel(rankPassiveSkillLevel2);
		}
		foreach (DataRow dr in UserDac.RankPassiveSkillAttrLevels(conn, null))
		{
			int nSkillId = Convert.ToInt32(dr["skillId"]);
			RankPassiveSkill rankPassiveSkill = GetRankPassiveSkill(nSkillId);
			if (rankPassiveSkill == null)
			{
				SFLogUtil.Warn(GetType(), "[계급패시브스킬레벨 목록] 계급패시브스킬이 존재하지 않습니다. nSkillId = " + nSkillId);
				continue;
			}
			int nAttrId = Convert.ToInt32(dr["attrId"]);
			RankPassiveSkillAttr rankPassiveSkillAttr = rankPassiveSkill.GetAttr(nAttrId);
			if (rankPassiveSkillAttr == null)
			{
				SFLogUtil.Warn(GetType(), "[계급패시브스킬레벨 목록] 계급패시브스킬속성이 존재하지 않습니다. nSkillId = " + nSkillId + ", nAttrId = " + nAttrId);
				continue;
			}
			int nLevel = Convert.ToInt32(dr["level"]);
			RankPassiveSkillLevel rankPassiveSkillLevel = rankPassiveSkill.GetLevel(nLevel);
			if (rankPassiveSkillLevel == null)
			{
				SFLogUtil.Warn(GetType(), "계급패시브스킬레벨 목록] 계급패시브스킬레벨이 존재하지 않습니다. nSkillId = " + nSkillId + ", nLevel = " + nLevel);
			}
			else
			{
				RankPassiveSkillAttrLevel rankPassiveSkillAttrLevel = new RankPassiveSkillAttrLevel(rankPassiveSkillAttr, rankPassiveSkillLevel);
				rankPassiveSkillAttrLevel.Set(dr);
				rankPassiveSkillAttr.AddAttrLevel(rankPassiveSkillAttrLevel);
				rankPassiveSkillLevel.AddAttrLevel(rankPassiveSkillAttrLevel);
			}
		}
	}

	private void LoadUserDBResources_MainGear(SqlConnection conn)
	{
		foreach (DataRow dr5 in UserDac.MainGearCategories(conn, null))
		{
			MainGearCategory category = new MainGearCategory();
			category.Set(dr5);
			AddMainGearCategory(category);
		}
		foreach (DataRow dr7 in UserDac.MainGearTypes(conn, null))
		{
			MainGearType type = new MainGearType();
			type.Set(dr7);
			AddMainGearType(type);
		}
		foreach (DataRow dr10 in UserDac.MainGearTiers(conn, null))
		{
			MainGearTier tier4 = new MainGearTier();
			tier4.Set(dr10);
			AddMainGearTier(tier4);
		}
		foreach (DataRow dr14 in UserDac.MainGearGrades(conn, null))
		{
			MainGearGrade grade = new MainGearGrade();
			grade.Set(dr14);
			AddMainGearGrade(grade);
		}
		foreach (DataRow dr18 in UserDac.MainGearQualities(conn, null))
		{
			MainGearQuality quality = new MainGearQuality();
			quality.Set(dr18);
			AddMainGearQuality(quality);
		}
		foreach (DataRow dr17 in UserDac.MainGears(conn, null))
		{
			MainGear mainGear = new MainGear();
			mainGear.Set(dr17);
			AddMainGear(mainGear);
		}
		foreach (DataRow dr16 in UserDac.MainGearBaseAttrs(conn, null))
		{
			MainGearBaseAttr attr4 = new MainGearBaseAttr();
			attr4.Set(dr16);
			int nGearId2 = Convert.ToInt32(dr16["mainGearId"]);
			MainGear gear2 = GetMainGear(nGearId2);
			if (gear2 == null)
			{
				SFLogUtil.Warn(GetType(), "[메인장비기본속성 목록] 메인장비가 존재하지않습니다. nGearId = " + nGearId2);
			}
			else
			{
				gear2.AddBaseAttr(attr4);
			}
		}
		foreach (DataRow dr15 in UserDac.MainGearBaseAttrEnchantLevels(conn, null))
		{
			MainGearBaseAttrEnchantLevel level2 = new MainGearBaseAttrEnchantLevel();
			level2.Set(dr15);
			int nGearId = Convert.ToInt32(dr15["mainGearId"]);
			MainGear gear = GetMainGear(nGearId);
			if (gear == null)
			{
				SFLogUtil.Warn(GetType(), "[메인장비기본속성강화레벨 목록] 메인장비가 존재하지않습니다. nGearId = " + nGearId);
				continue;
			}
			int nAttr = Convert.ToInt32(dr15["attrId"]);
			MainGearBaseAttr attr3 = gear.GetBaseAttr(nAttr);
			if (attr3 == null)
			{
				SFLogUtil.Warn(GetType(), "[메인장비기본속성강화레벨 목록] 메인장비기본속성이 존재하지 않습니다. nAttr = " + nAttr);
			}
			else
			{
				attr3.AddEnchantLevel(level2);
			}
		}
		foreach (DataRow dr13 in UserDac.MainGearEnchantSteps(conn, null))
		{
			MainGearEnchantStep step2 = new MainGearEnchantStep();
			step2.Set(dr13);
			AddMainGearEnchantStep(step2);
		}
		foreach (DataRow dr12 in UserDac.MainGearEnchantLevels(conn, null))
		{
			MainGearEnchantLevel level = new MainGearEnchantLevel();
			level.Set(dr12);
			AddMainGearEnchantLevel(level);
			int nStep = Convert.ToInt32(dr12["step"]);
			MainGearEnchantStep step = GetMainGearEnchantStep(nStep);
			if (step == null)
			{
				SFLogUtil.Warn(GetType(), "[메인장비강화레벨 목록] 존재하지 않는 메인장비강화단계입니다. nStep = " + nStep);
			}
			else
			{
				step.AddLevel(level);
			}
		}
		foreach (DataRow dr11 in UserDac.MainGearOptionAttrPoolEntries(conn, null))
		{
			int nTier4 = Convert.ToInt32(dr11["tier"]);
			MainGearTier tier3 = GetMainGearTier(nTier4);
			if (tier3 == null)
			{
				SFLogUtil.Warn(GetType(), "[메인장비옵션속성풀항목 목록] 존재하지 않는 메인장비티어입니다. nTier = " + nTier4);
				continue;
			}
			int nGrade4 = Convert.ToInt32(dr11["grade"]);
			MainGearOptionAttrPool pool3 = tier3.GetOptionAttrPool(nGrade4);
			if (pool3 == null)
			{
				SFLogUtil.Warn(GetType(), "[메인장비옵션속성풀항목 목록] 존재하지 않는 메인장비옵션속성풀입니다. nTier = " + nTier4 + ", nGrade = " + nGrade4);
			}
			else
			{
				MainGearOptionAttrPoolEntry entry3 = new MainGearOptionAttrPoolEntry();
				entry3.Set(dr11);
				pool3.AddEntry(entry3);
			}
		}
		foreach (DataRow dr9 in UserDac.MainGearRefinementRecieps(conn, null))
		{
			MainGearRefinementRecipe recipe = new MainGearRefinementRecipe();
			recipe.Set(dr9);
			AddMainGearRefinementRecipe(recipe);
		}
		foreach (DataRow dr8 in UserDac.MainGearDisassembleResultCountPoolEntries(conn, null))
		{
			int nTier3 = Convert.ToInt32(dr8["tier"]);
			MainGearTier tier2 = GetMainGearTier(nTier3);
			if (tier2 == null)
			{
				SFLogUtil.Warn(GetType(), "[메인장비분해결과갯수풀항목 목록] 존재하지 않는 메인장비티어입니다. nTier = " + nTier3);
				continue;
			}
			int nGrade3 = Convert.ToInt32(dr8["grade"]);
			MainGearDisassembleResultCountPool pool2 = tier2.GetDisassembleResultCountPool(nGrade3);
			if (pool2 == null)
			{
				SFLogUtil.Warn(GetType(), "[메인장비분해결과갯수풀항목 목록] 존재하지 않는 메인장비분해결과갯수풀입니다. nTier = " + nTier3 + ", nGrade = " + nGrade3);
			}
			else
			{
				MainGearDisassembleResultCountPoolEntry entry2 = new MainGearDisassembleResultCountPoolEntry();
				entry2.Set(dr8);
				pool2.AddEntry(entry2);
			}
		}
		foreach (DataRow dr6 in UserDac.MainGearDisassembleResultPoolEntries(conn, null))
		{
			int nTier2 = Convert.ToInt32(dr6["tier"]);
			MainGearTier tier = GetMainGearTier(nTier2);
			if (tier == null)
			{
				SFLogUtil.Warn(GetType(), "[메인장비분해결과풀항목 목록] 존재하지 않는 메인장비티어입니다. nTier = " + nTier2);
				continue;
			}
			int nGrade2 = Convert.ToInt32(dr6["grade"]);
			MainGearDisassembleResultPool pool = tier.GetDisassembleResultPool(nGrade2);
			if (pool == null)
			{
				SFLogUtil.Warn(GetType(), "[메인장비분해결과풀항목 목록] 존재하지 않는 메인장비분해결과풀입니다. nTier = " + nTier2 + ", nGrade = " + nGrade2);
			}
			MainGearDisassembleResultPoolEntry entry = new MainGearDisassembleResultPoolEntry();
			entry.Set(dr6);
			pool.AddEntry(entry);
		}
		m_mainGearSets = new MainGearSet[m_mainGearTiers.Count, 5, 6];
		foreach (DataRow dr4 in UserDac.MainGearSets(conn, null))
		{
			MainGearSet mainGearSet2 = new MainGearSet();
			mainGearSet2.Set(dr4);
			AddMainGearSet(mainGearSet2);
		}
		foreach (DataRow dr3 in UserDac.MainGearSetAttrs(conn, null))
		{
			MainGearSetAttr attr2 = new MainGearSetAttr();
			attr2.Set(dr3);
			int nTier = Convert.ToInt32(dr3["tier"]);
			int nGrade = Convert.ToInt32(dr3["grade"]);
			int nQuality = Convert.ToInt32(dr3["quality"]);
			MainGearSet mainGearSet = GetMainGearSet(nTier, nGrade, nQuality);
			if (mainGearSet == null)
			{
				SFLogUtil.Warn(GetType(), "[메인장비세트효과속성 목록] 메인장비세트효과가 존재하지 않습니다. nTier = " + nTier + ", nGrade = " + nGrade + ", nQuality = " + nQuality);
			}
			else
			{
				mainGearSet.AddAttr(attr2);
			}
		}
		foreach (DataRow dr2 in UserDac.MainGearEnchantLevelSet(conn, null))
		{
			MainGearEnchantLevelSet enchantLevelSet2 = new MainGearEnchantLevelSet();
			enchantLevelSet2.Set(dr2);
			AddMainGearEnchantLevelSet(enchantLevelSet2);
		}
		foreach (DataRow dr in UserDac.MainGearEnchantLevelSetAttr(conn, null))
		{
			MainGearEnchantLevelSetAttr attr = new MainGearEnchantLevelSetAttr();
			attr.Set(dr);
			int nSetNo = Convert.ToInt32(dr["setNo"]);
			MainGearEnchantLevelSet enchantLevelSet = GetMainGearEnchantLevelSet(nSetNo);
			if (enchantLevelSet == null)
			{
				SFLogUtil.Warn(GetType(), "[메인장비강화레벨세트효과속성 목록] 메인장비강화레벨세트효과가 존재하지 않습니다. nSetNo = " + nSetNo);
			}
			else
			{
				enchantLevelSet.AddAttr(attr);
			}
		}
	}

	private void LoadUserDBResources_SubGear(SqlConnection conn)
	{
		foreach (DataRow dr5 in UserDac.SubGears(conn, null))
		{
			SubGear gear = new SubGear();
			gear.Set(dr5);
			AddSubGear(gear);
		}
		foreach (DataRow dr8 in UserDac.SubGearRuneSockets(conn, null))
		{
			SubGearRuneSocket socket = new SubGearRuneSocket();
			socket.Set(dr8);
			int nGearId2 = Convert.ToInt32(dr8["subGearId"]);
			SubGear gear4 = GetSubGear(nGearId2);
			if (gear4 == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비룬소켓 목록] 보조장비가 존재하지 않습니다. nGearId = " + nGearId2);
			}
			else
			{
				gear4.AddRuneSocket(socket);
			}
		}
		foreach (DataRow dr12 in UserDac.SubGearRuneSocketAvailableItemTypes(conn, null))
		{
			int nGearId4 = Convert.ToInt32(dr12["subGearId"]);
			SubGear gear5 = GetSubGear(nGearId4);
			if (gear5 == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비룬소켓가능한아이템타입 목록] 보조장비가 존재하지 않습니다. nGearId = " + nGearId4);
				continue;
			}
			int nSocketIndex = Convert.ToInt32(dr12["socketIndex"]);
			SubGearRuneSocket socket3 = gear5.GetRuneSocket(nSocketIndex);
			if (socket3 == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비룬소켓가능한아이템타입 목록] 룬소켓이 존재하지 않습니다. nGearId = " + nGearId4 + ", nSocketIndex = " + nSocketIndex);
				continue;
			}
			int nItemType = Convert.ToInt32(dr12["itemType"]);
			ItemType itemType = GetItemType(nItemType);
			if (itemType == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비룬소켓가능한아이템타입 목록] 아이템 타입이 존재하지 않습니다. nGearId = " + nGearId4 + ", nSocketIndex = " + nSocketIndex + ", nItemType = " + nItemType);
			}
			else
			{
				socket3.AddAvailableItemType(itemType.id);
			}
		}
		foreach (DataRow dr11 in UserDac.SubGearSoulstoneSockets(conn, null))
		{
			SubGearSoulstoneSocket socket2 = new SubGearSoulstoneSocket();
			socket2.Set(dr11);
			int nGearId3 = Convert.ToInt32(dr11["subGearId"]);
			SubGear gear3 = GetSubGear(nGearId3);
			if (gear3 == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비소울스톤소켓 목록] 보조장비가 존재하지 않습니다. nGearId = " + nGearId3);
			}
			else
			{
				gear3.AddSoulstoneSocket(socket2);
			}
		}
		foreach (DataRow dr10 in UserDac.SubGearGrades(conn, null))
		{
			SubGearGrade grade = new SubGearGrade();
			grade.Set(dr10);
			AddSubGearGrade(grade);
		}
		foreach (DataRow dr9 in UserDac.SubGearNames(conn, null))
		{
			SubGearName name = new SubGearName();
			name.Set(dr9);
			int nGearId = Convert.ToInt32(dr9["subGearId"]);
			SubGear gear2 = GetSubGear(nGearId);
			if (gear2 == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비이름 목록] 보조장비가 존재하지 않습니다. nGearId = " + nGearId);
			}
			else
			{
				gear2.AddName(name);
			}
		}
		foreach (DataRow dr7 in UserDac.SubGearLevels(conn, null))
		{
			SubGearLevel level3 = new SubGearLevel();
			level3.Set(dr7);
			int nSubGearId4 = Convert.ToInt32(dr7["subGearId"]);
			SubGear subGear4 = GetSubGear(nSubGearId4);
			if (subGear4 == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비레벨 목록] 보조장비가 존재하지 않습니다. nSubGearId = " + nSubGearId4);
			}
			else
			{
				subGear4.AddLevel(level3);
			}
		}
		foreach (DataRow dr6 in UserDac.SubGearLevelQualities(conn, null))
		{
			SubGearLevelQuality quality2 = new SubGearLevelQuality();
			quality2.Set(dr6);
			int nSubGearId3 = Convert.ToInt32(dr6["subGearId"]);
			SubGear subGear3 = GetSubGear(nSubGearId3);
			if (subGear3 == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비레벨품질 목록] 보조장비가 존재하지 않습니다. nSubGearId = " + nSubGearId3);
				continue;
			}
			int nLevel2 = Convert.ToInt32(dr6["level"]);
			SubGearLevel level2 = subGear3.GetLevel(nLevel2);
			if (level2 == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비레벨품질 목록] 레벨이 존재하지 않습니다. nSubGearId = " + nSubGearId3 + ", nLevel = " + nLevel2);
			}
			else
			{
				level2.AddQuality(quality2);
			}
		}
		foreach (DataRow dr4 in UserDac.SubGearAttrs(conn, null))
		{
			SubGearAttr attr2 = new SubGearAttr();
			attr2.Set(dr4);
			int nSubGearId2 = Convert.ToInt32(dr4["subGearId"]);
			SubGear subGear2 = GetSubGear(nSubGearId2);
			if (subGear2 == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비속성 목록] 보조장비가 존재하지 않습니다. nSubGearId = " + nSubGearId2);
			}
			else
			{
				subGear2.AddAttr(attr2);
			}
		}
		foreach (DataRow dr3 in UserDac.SubGearAttrValues(conn, null))
		{
			SubGearAttrValue attrValue = new SubGearAttrValue();
			attrValue.Set(dr3);
			int nSubGearId = Convert.ToInt32(dr3["subGearId"]);
			SubGear subGear = GetSubGear(nSubGearId);
			if (subGear == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비속성값 목록] 보조장비가 존재하지 않습니다. nSubGearId = " + nSubGearId);
				continue;
			}
			int nLevel = Convert.ToInt32(dr3["level"]);
			SubGearLevel level = subGear.GetLevel(nLevel);
			if (level == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비속성값 목록] 레벨이 존재하지 않습니다. nLevel = " + nLevel);
				continue;
			}
			int nQuality = Convert.ToInt32(dr3["quality"]);
			SubGearLevelQuality quality = level.GetQuality(nQuality);
			if (quality == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비속성값 목록] 품질이 존재하지 않습니다. nQuality = " + nQuality);
			}
			else
			{
				quality.AddAttrValue(attrValue);
			}
		}
		foreach (DataRow dr2 in UserDac.SubGearSoulstoneLevelSets(conn, null))
		{
			SubGearSoulstoneLevelSet soulstoneLevelSet2 = new SubGearSoulstoneLevelSet();
			soulstoneLevelSet2.Set(dr2);
			AddSubGearSoulstoneLevelSet(soulstoneLevelSet2);
		}
		foreach (DataRow dr in UserDac.SubGearSoulstoneLevelSetAttrs(conn, null))
		{
			SubGearSoulstoneLevelSetAttr attr = new SubGearSoulstoneLevelSetAttr();
			attr.Set(dr);
			int nSetNo = Convert.ToInt32(dr["setNo"]);
			SubGearSoulstoneLevelSet soulstoneLevelSet = GetSubGearSoulstoneLevelSet(nSetNo);
			if (soulstoneLevelSet == null)
			{
				SFLogUtil.Warn(GetType(), "[보조장비소울스톤레벨세트효과속성 목록] 보조장비소울스톤레벨세트효과가 존재하지 않습니다. nSetNo = " + nSetNo);
			}
			else
			{
				soulstoneLevelSet.AddAttr(attr);
			}
		}
	}

	private void LoadUserDBResources_Mount(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.Mounts(conn, null))
		{
			Mount mount2 = new Mount();
			mount2.Set(dr2);
			AddMount(mount2);
		}
		foreach (DataRow dr6 in UserDac.MountQualityMasters(conn, null))
		{
			MountQualityMaster master2 = new MountQualityMaster();
			master2.Set(dr6);
			AddMountQualityMaster(master2);
		}
		foreach (DataRow dr10 in UserDac.MountLevelMasters(conn, null))
		{
			MountLevelMaster master3 = new MountLevelMaster();
			master3.Set(dr10);
			AddMountLevelMaster(master3);
		}
		foreach (DataRow dr14 in UserDac.MountLevels(conn, null))
		{
			MountLevel level2 = new MountLevel();
			level2.Set(dr14);
			int nId3 = Convert.ToInt32(dr14["mountId"]);
			Mount mount4 = GetMount(nId3);
			if (mount4 == null)
			{
				SFLogUtil.Warn(GetType(), "[탈것레벨 목록] 탈것이 존재하지 않습니다. nId = " + nId3);
			}
			else
			{
				mount4.AddLevel(level2);
			}
		}
		foreach (DataRow dr15 in UserDac.MountQualities(conn, null))
		{
			MountQuality quality2 = new MountQuality();
			quality2.Set(dr15);
			int nId2 = Convert.ToInt32(dr15["mountId"]);
			Mount mount3 = GetMount(nId2);
			if (mount3 == null)
			{
				SFLogUtil.Warn(GetType(), "[탈것품질 목록] 탈것이 존재하지 않습니다. nId = " + nId2);
			}
			else
			{
				mount3.AddQuality(quality2);
			}
		}
		foreach (DataRow dr13 in UserDac.MountGearSlots(conn, null))
		{
			MountGearSlot slot = new MountGearSlot();
			slot.Set(dr13);
			m_mountGearSlots[slot.index] = slot;
		}
		foreach (DataRow dr12 in UserDac.MountGearTypes(conn, null))
		{
			MountGearType type = new MountGearType();
			type.Set(dr12);
			m_mountGearTypes[type.id - 1] = type;
		}
		foreach (DataRow dr11 in UserDac.MountGearGrades(conn, null))
		{
			MountGearGrade grade = new MountGearGrade();
			grade.Set(dr11);
			m_mountGearGrades[grade.id - 1] = grade;
		}
		foreach (DataRow dr9 in UserDac.MountGearQualities(conn, null))
		{
			MountGearQuality quality = new MountGearQuality();
			quality.Set(dr9);
			m_mountGearQualities[quality.id - 1] = quality;
		}
		foreach (DataRow dr8 in UserDac.MountGears(conn, null))
		{
			MountGear gear = new MountGear();
			gear.Set(dr8);
			AddMountGear(gear);
		}
		foreach (DataRow dr7 in UserDac.MountGearOptionAttrPoolEntries(conn, null))
		{
			int nId = Convert.ToInt32(dr7["mountGearId"]);
			MountGear mountGear = GetMountGear(nId);
			if (mountGear == null)
			{
				SFLogUtil.Warn(GetType(), "[탈것장비옵션속성풀항목 목록] 탈것장비가 존재하지 않습니다. nId = " + nId);
				continue;
			}
			MountGearOptionAttrPoolEntry entry = new MountGearOptionAttrPoolEntry();
			entry.Set(dr7);
			mountGear.AddOptionAttrPoolEntry(entry);
		}
		foreach (DataRow dr5 in UserDac.MountGearPickBoxRecipes(conn, null))
		{
			MountGearPickBoxRecipe recipe = new MountGearPickBoxRecipe();
			recipe.Set(dr5);
			AddMountGearPickBoxRecipes(recipe);
		}
		foreach (DataRow dr4 in UserDac.MountAwakeningLevelMasters(conn, null))
		{
			MountAwakeningLevelMaster master = new MountAwakeningLevelMaster();
			master.Set(dr4);
			AddMountAwakeningLevelMaster(master);
		}
		foreach (DataRow dr3 in UserDac.MountAwakeningLevels(conn, null))
		{
			int nMountId = Convert.ToInt32(dr3["mountId"]);
			Mount mount = GetMount(nMountId);
			if (mount == null)
			{
				SFLogUtil.Warn(GetType(), "[탈것각성레벨 목록] 탈것이 존재하지 않습니다. nMountId = " + nMountId);
				continue;
			}
			MountAwakeningLevel level = new MountAwakeningLevel(mount);
			level.Set(dr3);
			mount.AddAwakeningLevel(level);
		}
		foreach (DataRow dr in UserDac.MountPotionAttrCounts(conn, null))
		{
			int nCount = Convert.ToInt32(dr["count"]);
			MountPotionAttrCount count = GetMountPotionAttrCount(nCount);
			if (count == null)
			{
				count = new MountPotionAttrCount(nCount);
				AddMountPotionAttrCount(count);
			}
			MountPotaionAttrValue attrValue = new MountPotaionAttrValue(count);
			attrValue.Set(dr);
			count.AddAttrValue(attrValue);
		}
	}

	private void LoadUserDBResources_TreatOfFarmQuest(SqlConnection conn)
	{
		DataRow drTreatOfFarmQuest = UserDac.TreatOfFarmQuest(conn, null);
		if (drTreatOfFarmQuest != null)
		{
			TreatOfFarmQuest treatOfFarmQuest = new TreatOfFarmQuest();
			treatOfFarmQuest.Set(drTreatOfFarmQuest);
			m_treatOfFarmQuest = treatOfFarmQuest;
		}
		foreach (DataRow dr3 in UserDac.TreatOfFarmQuestRewards(conn, null))
		{
			TreatOfFarmQuestReward reward = new TreatOfFarmQuestReward();
			reward.Set(dr3);
			m_treatOfFarmQuest.AddReward(reward);
		}
		foreach (DataRow dr2 in UserDac.TreatOfFarmQuestMissions(conn, null))
		{
			TreatOfFarmQuestMission mission2 = new TreatOfFarmQuestMission();
			mission2.Set(dr2);
			m_treatOfFarmQuest.AddMission(mission2);
		}
		foreach (DataRow dr in UserDac.TreatOfFarmQuestMonsterArranges(conn, null))
		{
			TreatOfFarmQuestMonsterArrange arrange = new TreatOfFarmQuestMonsterArrange();
			arrange.Set(dr);
			int nMissionId = Convert.ToInt32(dr["missionId"]);
			TreatOfFarmQuestMission mission = m_treatOfFarmQuest.GetMission(nMissionId);
			if (mission == null)
			{
				SFLogUtil.Warn(GetType(), "[농장의위협퀘스트출몰몬스터 목록] 농장의위협퀘스트미션풀항목이 존재하지 않습니다. nMissionId = " + nMissionId);
			}
			else
			{
				mission.AddMonsterArrange(arrange);
			}
		}
	}

	private void LoadUserDBResources_BountyHunterQuest(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.BountyHunterQuests(conn, null))
		{
			BountyHunterQuest quest = new BountyHunterQuest();
			quest.Set(dr2);
			AddBountyHunterQuest(quest);
		}
		for (int i = 0; i < 5; i++)
		{
			m_bountyHunterQuestRewardCollection[i] = new BountyHunterQuestRewardCollection(i + 1);
		}
		foreach (DataRow dr in UserDac.BountyHunterQuestRewards(conn, null))
		{
			BountyHunterQuestReward reward = new BountyHunterQuestReward();
			reward.Set(dr);
			int nItemGrade = Convert.ToInt32(dr["questItemGrade"]);
			BountyHunterQuestRewardCollection rewardCollection = GetBountyHunterQuestRewardCollection(nItemGrade);
			if (rewardCollection == null)
			{
				SFLogUtil.Warn(GetType(), "[현상금사냥꾼퀘스트보상목록] 아이템등급이 유효하지 않습니다. nItemGrade = " + nItemGrade);
			}
			else
			{
				rewardCollection.AddReward(reward);
			}
		}
	}

	private void LoadUserDBResources_FishingQuest(SqlConnection conn)
	{
		DataRow drFishingQuest = UserDac.FishingQuest(conn, null);
		if (drFishingQuest == null)
		{
			SFLogUtil.Warn(GetType(), "낚시퀘스트가 존재하지 않습니다.");
			return;
		}
		FishingQuest fishingQuest = new FishingQuest();
		fishingQuest.Set(drFishingQuest);
		m_fishingQuest = fishingQuest;
		foreach (DataRow dr3 in UserDac.FishinhQuestSpots(conn, null))
		{
			FishingQuestSpot spot2 = new FishingQuestSpot();
			spot2.Set(dr3);
			m_fishingQuest.AddSpot(spot2);
		}
		foreach (DataRow dr2 in UserDac.FishingQuestGuildTerritorySpots(conn, null))
		{
			FishingQuestGuildTerritorySpot spot = new FishingQuestGuildTerritorySpot();
			spot.Set(dr2);
			m_fishingQuest.AddGuildTerritorySpot(spot);
		}
		foreach (DataRow dr in UserDac.FishingQuestCastingRewards(conn, null))
		{
			FishingQuestCastingReward reward = new FishingQuestCastingReward();
			reward.Set(dr);
			int nBaitItemId = Convert.ToInt32(dr["baitItemId"]);
			FishingQuestBait bait = m_fishingQuest.GetBait(nBaitItemId);
			if (bait == null)
			{
				SFLogUtil.Warn(GetType(), "미끼가 존재하지 않습니다. nBaitItemId = " + nBaitItemId);
			}
			else
			{
				bait.AddReward(reward);
			}
		}
	}

	private void LoadUserDBResources_TodayMission(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.TodayMissions(conn, null))
		{
			TodayMission mission3 = new TodayMission();
			mission3.Set(dr2);
			AddTodayMission(mission3);
		}
		foreach (DataRow dr in UserDac.TodayMissionRewards(conn, null))
		{
			TodayMissionReward reward = new TodayMissionReward();
			reward.Set(dr);
			int nMissionId = Convert.ToInt32(dr["missionId"]);
			TodayMission mission2 = GetTodayMission(nMissionId);
			if (mission2 == null)
			{
				SFLogUtil.Warn(GetType(), "[오늘의미션보상 목록] 미션이 존재하지 않습니다. nMissionId = " + nMissionId);
			}
			else
			{
				mission2.AddReward(reward);
			}
		}
		m_todayMissionPools = new TodayMissionPool[lastJobLevelMaster.level];
		for (int i = 1; i <= lastJobLevelMaster.level; i++)
		{
			TodayMissionPool pool = new TodayMissionPool(i);
			foreach (TodayMission mission in m_todayMissions.Values)
			{
				if (mission.id != 10 && mission.heroMinLevel <= pool.level)
				{
					pool.Add(mission);
				}
			}
			m_todayMissionPools[pool.level - 1] = pool;
		}
	}

	private void LoadUserDBResources_Guild(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.GuildLevels(conn, null))
		{
			GuildLevel level3 = new GuildLevel();
			level3.Set(dr2);
			AddGuildLevel(level3);
		}
		foreach (DataRow dr6 in UserDac.GuildMemberGrades(conn, null))
		{
			GuildMemberGrade grade = new GuildMemberGrade();
			grade.Set(dr6);
			AddGuildMemberGrade(grade);
		}
		foreach (DataRow dr8 in UserDac.GuildDonationEntries(conn, null))
		{
			GuildDonationEntry entry = new GuildDonationEntry();
			entry.Set(dr8);
			AddGuildDonationEntry(entry);
		}
		foreach (DataRow dr7 in UserDac.GuildBuildings(conn, null))
		{
			GuildBuilding building2 = new GuildBuilding();
			building2.Set(dr7);
			AddGuildBuilding(building2);
		}
		foreach (DataRow dr5 in UserDac.GuildBuildingLevels(conn, null))
		{
			int nBuildingId = Convert.ToInt32(dr5["buildingId"]);
			GuildBuilding building = GetGuildBuilding(nBuildingId);
			if (building == null)
			{
				SFLogUtil.Warn(GetType(), "길드건물이 존재하지 않습니다. nBuildingId = " + nBuildingId);
				continue;
			}
			GuildBuildingLevel level4 = new GuildBuildingLevel(building);
			level4.Set(dr5);
			building.AddLevel(level4);
		}
		foreach (DataRow dr4 in UserDac.GuildSkills(conn, null))
		{
			GuildSkill skill3 = new GuildSkill();
			skill3.Set(dr4);
			AddGuildSkill(skill3);
		}
		foreach (DataRow dr3 in UserDac.GuildSkillLevels(conn, null))
		{
			int nSkillId2 = Convert.ToInt32(dr3["guildSkillId"]);
			GuildSkill skill2 = GetGuildSkill(nSkillId2);
			if (skill2 == null)
			{
				SFLogUtil.Warn(GetType(), "스킬이 존재하지 않습니다. nSkillId = " + nSkillId2);
				continue;
			}
			GuildSkillLevel level2 = new GuildSkillLevel(skill2);
			level2.Set(dr3);
			skill2.AddLevel(level2);
		}
		foreach (DataRow dr in UserDac.GuildSkillAttrValues(conn, null))
		{
			int nSkillId = Convert.ToInt32(dr["guildSkillId"]);
			GuildSkill skill = GetGuildSkill(nSkillId);
			if (skill == null)
			{
				SFLogUtil.Warn(GetType(), "스킬이 존재하지 않습니다. nSkillId = " + nSkillId);
				continue;
			}
			int nSkillLevel = Convert.ToInt32(dr["level"]);
			GuildSkillLevel level = skill.GetLevel(nSkillLevel);
			if (level == null)
			{
				SFLogUtil.Warn(GetType(), "레벨이 존재하지 않습니다. nSkillId = " + nSkillId + ", nSkillLevel = " + nSkillLevel);
			}
			else
			{
				GuildSkillLevelAttrValue attrValue = new GuildSkillLevelAttrValue(level);
				attrValue.Set(dr);
				level.AddAttrValues(attrValue);
			}
		}
	}

	private void LoadUserDBResources_GuildTerritory(SqlConnection conn)
	{
		DataRow drGuidlTerritory = UserDac.GuildTerritory(conn, null);
		if (drGuidlTerritory == null)
		{
			SFLogUtil.Warn(GetType(), "[길드] 길드영지가 존재하지 않습니다.");
		}
		else
		{
			GuildTerritory guildTerritory = new GuildTerritory();
			guildTerritory.Set(drGuidlTerritory);
			m_guildTerritory = guildTerritory;
			AddLocation(guildTerritory);
		}
		foreach (DataRow dr in UserDac.GuildTerritoryNpcs(conn, null))
		{
			GuildTerritoryNpc npc = new GuildTerritoryNpc();
			npc.Set(dr);
			m_guildTerritoryNpcs.Add(npc.id, npc);
		}
	}

	private void LoadUserDBResources_GuildFarmQuest(SqlConnection conn)
	{
		DataRow drGuidlFarmQuest = UserDac.GuildFarmQuest(conn, null);
		if (drGuidlFarmQuest == null)
		{
			SFLogUtil.Warn(GetType(), "[길드] 길드농장퀘스트가 존재하지 않습니다.");
			return;
		}
		m_guildFarmQuest = new GuildFarmQuest();
		m_guildFarmQuest.Set(drGuidlFarmQuest);
		foreach (DataRow dr in UserDac.GuildFarmQuestRewards(conn, null))
		{
			GuildFarmQuestReward reward = new GuildFarmQuestReward();
			reward.Set(dr);
			m_guildFarmQuest.AddReward(reward);
		}
	}

	private void LoadUserDBResources_GuildFoodWarehouse(SqlConnection conn)
	{
		DataRow drGuildFoodWarehouse = UserDac.GuildFoodWarehouse(conn, null);
		if (drGuildFoodWarehouse == null)
		{
			SFLogUtil.Warn(GetType(), "[길드] 길드군량창고가 존재하지 않습니다.");
			return;
		}
		m_guildFoodWarehouse = new GuildFoodWarehouse();
		m_guildFoodWarehouse.Set(drGuildFoodWarehouse);
		foreach (DataRow dr2 in UserDac.GuildFoodWarehouseLevels(conn, null))
		{
			GuildFoodWarehouseLevel level = new GuildFoodWarehouseLevel();
			level.Set(dr2);
			m_guildFoodWarehouse.AddLevel(level);
		}
		foreach (DataRow dr in UserDac.GuildFoodWarehouseStockRewards(conn, null))
		{
			GuildFoodWareshouseStockReward reward = new GuildFoodWareshouseStockReward();
			reward.Set(dr);
			m_guildFoodWarehouse.AddStockReward(reward);
		}
	}

	private void LoadUserDBResources_GuildAltar(SqlConnection conn)
	{
		DataRow drGuildAltar = UserDac.GuildAltar(conn, null);
		if (drGuildAltar == null)
		{
			SFLogUtil.Warn(GetType(), "[길드] 길드제단이 존재하지 않습니다.");
			return;
		}
		m_guildAltar = new GuildAltar();
		m_guildAltar.Set(drGuildAltar);
		foreach (DataRow dr2 in UserDac.GuildAltarRewards(conn, null))
		{
			GuildAltarReward reward = new GuildAltarReward();
			reward.Set(dr2);
			m_guildAltar.AddReward(reward);
		}
		foreach (DataRow dr in UserDac.GuildAltarDefenseMonsterAttrFactors(conn, null))
		{
			GuildAltarDefenseMonsterAttrFactor factor = new GuildAltarDefenseMonsterAttrFactor();
			factor.Set(dr);
			m_guildAltar.AddDefenseMonsterAttrFactor(factor);
		}
	}

	private void LoadUserDBResources_GuildMissionQuest(SqlConnection conn)
	{
		DataRow drGuildMissionQuest = UserDac.GuildMissionQuest(conn, null);
		if (drGuildMissionQuest == null)
		{
			SFLogUtil.Warn(GetType(), "[길드미션퀘스트] 길드미션퀘스트가 존재하지않습니다.");
			return;
		}
		m_guildMissionQuest = new GuildMissionQuest();
		m_guildMissionQuest.Set(drGuildMissionQuest);
		foreach (DataRow dr2 in UserDac.GuildMissions(conn, null))
		{
			GuildMission mission = new GuildMission();
			mission.Set(dr2);
			m_guildMissionQuest.AddMission(mission);
		}
		foreach (DataRow dr in UserDac.GuildMissionQuestRewards(conn, null))
		{
			GuildMissionQuestReward reward = new GuildMissionQuestReward();
			reward.Set(dr);
			m_guildMissionQuest.AddReward(reward);
		}
	}

	private void LoadUserDBResources_GuildSupplySupportQuest(SqlConnection conn)
	{
		DataRow drGuildSupplySupportQuest = UserDac.GuildSupplySupportQuest(conn, null);
		if (drGuildSupplySupportQuest == null)
		{
			SFLogUtil.Warn(GetType(), "길드물자지원퀘스트가 존재하지 않습니다.");
			return;
		}
		GuildSupplySupportQuest guildSupplySupportQuest = new GuildSupplySupportQuest();
		guildSupplySupportQuest.Set(drGuildSupplySupportQuest);
		m_guildSupplySupportQuest = guildSupplySupportQuest;
		foreach (DataRow dr in UserDac.GuildSupplySupportQuestRewards(conn, null))
		{
			GuildSupplySupportQuestReward reward = new GuildSupplySupportQuestReward(m_guildSupplySupportQuest);
			reward.Set(dr);
			m_guildSupplySupportQuest.AddReward(reward);
		}
	}

	private void LoadUserDBResources_GuildHuntingQuest(SqlConnection conn)
	{
		DataRow drGuildHuntingQuest = UserDac.GuildHuntingQuest(conn, null);
		if (drGuildHuntingQuest == null)
		{
			return;
		}
		m_guildHuntingQuest = new GuildHuntingQuest();
		m_guildHuntingQuest.Set(drGuildHuntingQuest);
		foreach (DataRow dr in UserDac.GuildHuntingQuestObjectives(conn, null))
		{
			GuildHuntingQuestObjective objective = new GuildHuntingQuestObjective();
			objective.Set(dr);
			m_guildHuntingQuest.AddObjective(objective);
		}
	}

	private void LoadUserDBResources_GuildTodayObjective(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.GuildContents(conn, null))
		{
			GuildContent content = new GuildContent();
			content.Set(dr2);
			AddGuildContent(content);
		}
		foreach (DataRow dr3 in UserDac.GuildDailyObjectiveRewards(conn, null))
		{
			GuildDailyObjectiveReward reward2 = new GuildDailyObjectiveReward();
			reward2.Set(dr3);
			AddGuildDailyObjectiveReward(reward2);
		}
		foreach (DataRow dr in UserDac.GuildWeeklyObjectives(conn, null))
		{
			GuildWeeklyObjective reward = new GuildWeeklyObjective();
			reward.Set(dr);
			AddGuildWeeklyObjective(reward);
		}
	}

	private void LoadUserDBResources_GuildBlessingBuff(SqlConnection conn)
	{
		foreach (DataRow dr in UserDac.GuildBlessingBuffs(conn, null))
		{
			GuildBlessingBuff buff = new GuildBlessingBuff();
			buff.Set(dr);
			AddGuildBlessingBuff(buff);
		}
	}

	private void LoadUserDBResources_NationNoblesse(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.NationNoblesses(conn, null))
		{
			NationNoblesse nationNoblesse3 = new NationNoblesse();
			nationNoblesse3.Set(dr2);
			AddNationNoblesse(nationNoblesse3);
		}
		foreach (DataRow dr3 in UserDac.NationNoblesseAttrs(conn, null))
		{
			int nNoblesseId2 = Convert.ToInt32(dr3["noblesseId"]);
			NationNoblesse nationNoblesse2 = GetNationNoblesse(nNoblesseId2);
			if (nationNoblesse2 == null)
			{
				SFLogUtil.Warn(GetType(), "[국가관직속성 목록] 관직이 존재하지 않습니다. nNoblesseId = " + nNoblesseId2);
				continue;
			}
			NationNoblesseAttr attr = new NationNoblesseAttr(nationNoblesse2);
			attr.Set(dr3);
			nationNoblesse2.AddAttr(attr);
		}
		foreach (DataRow dr in UserDac.NationNoblesseAppointmentAuthorities(conn, null))
		{
			int nNoblesseId = Convert.ToInt32(dr["noblesseId"]);
			NationNoblesse nationNoblesse = GetNationNoblesse(nNoblesseId);
			if (nationNoblesse == null)
			{
				SFLogUtil.Warn(GetType(), "[국가관직임명권한 목록] 관직이 존재하지 않습니다. nNoblesseId = " + nNoblesseId);
				continue;
			}
			NationNoblesseAppointmentAuthority appointmentAuthority = new NationNoblesseAppointmentAuthority(nationNoblesse);
			appointmentAuthority.Set(dr);
			nationNoblesse.AddAppointmentAuthority(appointmentAuthority);
		}
	}

	private void LoadUserDBResources_NationWar(SqlConnection conn)
	{
		DataRow drNationWar = UserDac.NationWar(conn, null);
		if (drNationWar == null)
		{
			SFLogUtil.Warn(GetType(), "국가전이 존재하지 않습니다.");
			return;
		}
		NationWar nationWar = new NationWar();
		nationWar.Set(drNationWar);
		m_nationWar = nationWar;
		foreach (DataRow dr4 in UserDac.NationWarNpcs(conn, null))
		{
			NationWarNpc npc = new NationWarNpc(m_nationWar);
			npc.Set(dr4);
			m_nationWar.AddNpc(npc);
		}
		foreach (DataRow dr8 in UserDac.NationWarTransmissionExits(conn, null))
		{
			int nNpcId = Convert.ToInt32(dr8["npcId"]);
			NationWarNpc npc2 = m_nationWar.GetNpc(nNpcId);
			if (npc2 == null)
			{
				SFLogUtil.Warn(GetType(), "[국가전전송출구 목록] NPC가 존재하지 않습니다. nNpcId = " + nNpcId);
				continue;
			}
			NationWarTransmissionExit transmissionExit = new NationWarTransmissionExit(npc2);
			transmissionExit.Set(dr8);
			npc2.AddTransmissionExit(transmissionExit);
		}
		foreach (DataRow dr12 in UserDac.NationWarPaidTransmissions(conn, null))
		{
			NationWarPaidTransmission paidTransmission = new NationWarPaidTransmission(m_nationWar);
			paidTransmission.Set(dr12);
			m_nationWar.AddPaidTransmission(paidTransmission);
		}
		foreach (DataRow dr11 in UserDac.NationWarHeroObjectiveEntries(conn, null))
		{
			NationWarHeroObjectiveEntry heroObjectiveEntry = new NationWarHeroObjectiveEntry(m_nationWar);
			heroObjectiveEntry.Set(dr11);
			m_nationWar.AddHeroObjectiveEntry(heroObjectiveEntry);
		}
		foreach (DataRow dr10 in UserDac.NationWarExpRewards(conn, null))
		{
			NationWarExpReward expReward = new NationWarExpReward(m_nationWar);
			expReward.Set(dr10);
			m_nationWar.AddExpReward(expReward);
		}
		foreach (DataRow dr9 in UserDac.NationWarAvailableDayOfWeeks(conn, null))
		{
			NationWarAvailableDayOfWeek availableDayOfWeek = new NationWarAvailableDayOfWeek(m_nationWar);
			availableDayOfWeek.Set(dr9);
			m_nationWar.AddAvailableDayOfWeek(availableDayOfWeek);
		}
		foreach (DataRow dr7 in UserDac.SystemNationWarDeclarations(conn, null))
		{
			SystemNationWarDeclaration systemDeclaration = new SystemNationWarDeclaration(m_nationWar);
			systemDeclaration.Set(dr7);
			m_nationWar.AddSystemDeclaration(systemDeclaration);
		}
		foreach (DataRow dr6 in UserDac.NationWarRevivalPoints(conn, null))
		{
			NationWarRevivalPoint revivalPoint2 = new NationWarRevivalPoint(m_nationWar);
			revivalPoint2.Set(dr6);
			m_nationWar.AddRevivalPoint(revivalPoint2);
		}
		foreach (DataRow dr5 in UserDac.NationWarMonsterArranges(conn, null))
		{
			NationWarMonsterArrange monsterArrange = new NationWarMonsterArrange(m_nationWar);
			monsterArrange.Set(dr5);
			m_nationWar.AddMonsterArrange(monsterArrange);
		}
		foreach (DataRow dr3 in UserDac.NationWarRevivalPointActivationConditions(conn, null))
		{
			int nRevivalPointId = Convert.ToInt32(dr3["revivalPointId"]);
			NationWarRevivalPoint revivalPoint = m_nationWar.GetRevivalPoint(nRevivalPointId);
			if (revivalPoint == null)
			{
				SFLogUtil.Warn(GetType(), "[국가전부활포인트활성조건 목록] 부활포인트ID가 유효하지 않습니다. nRevivalPointId = " + nRevivalPointId);
				continue;
			}
			NationWarRevivalPointActivationCondition activationCondition = new NationWarRevivalPointActivationCondition(revivalPoint);
			activationCondition.Set(dr3);
			revivalPoint.AddActivationCondition(activationCondition);
		}
		foreach (DataRow dr2 in UserDac.NationWarRankingRewards(conn, null))
		{
			NationWarRankingReward rankingReward = new NationWarRankingReward(m_nationWar);
			rankingReward.Set(dr2);
			m_nationWar.AddRankingReward(rankingReward);
		}
		foreach (DataRow dr in UserDac.NationWarPointRewards(conn, null))
		{
			NationWarPointReward pointReward = new NationWarPointReward(m_nationWar);
			pointReward.Set(dr);
			m_nationWar.AddPointReward(pointReward);
		}
	}

	private void LoadUserDBResources_IllustratedBook(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.IllustratedBookCategories(conn, null))
		{
			IllustratedBookCategory category = new IllustratedBookCategory();
			category.Set(dr2);
			AddIllustratedBookCategory(category);
		}
		foreach (DataRow dr6 in UserDac.IllustratedBookTypes(conn, null))
		{
			IllustratedBookType type = new IllustratedBookType();
			type.Set(dr6);
			AddIllustartedBookType(type);
			type.category?.AddType(type);
		}
		foreach (DataRow dr7 in UserDac.IllustratedBooks(conn, null))
		{
			IllustratedBook illustratedBook2 = new IllustratedBook();
			illustratedBook2.Set(dr7);
			AddIllustratedBook(illustratedBook2);
			illustratedBook2.type?.AddIllustratedBook(illustratedBook2);
		}
		foreach (DataRow dr5 in UserDac.IllustratedBookAttrs(conn, null))
		{
			int nIllustratedBookId = Convert.ToInt32(dr5["illustratedBookId"]);
			IllustratedBook illustratedBook = GetIllustratedBook(nIllustratedBookId);
			if (illustratedBook == null)
			{
				SFLogUtil.Warn(GetType(), "[도감속성 목록] 도감이 존재하지 않습니다. nIllustratedBookId = " + nIllustratedBookId);
				continue;
			}
			IllustratedBookAttr attr2 = new IllustratedBookAttr(illustratedBook);
			attr2.Set(dr5);
			illustratedBook.AddAttrs(attr2);
		}
		foreach (DataRow dr4 in UserDac.IllustratedBookExplorationSteps(conn, null))
		{
			IllustratedBookExplorationStep step3 = new IllustratedBookExplorationStep();
			step3.Set(dr4);
			AddIllustratedBookExplorationStep(step3);
		}
		foreach (DataRow dr3 in UserDac.IllustratedBookExplorationStepAttrs(conn, null))
		{
			int nStepNo2 = Convert.ToInt32(dr3["stepNo"]);
			IllustratedBookExplorationStep step2 = GetIllustratedBookExplorationStep(nStepNo2);
			if (step2 == null)
			{
				SFLogUtil.Warn(GetType(), "[도감탐험단계속성 목록] 도감탐험단계가 존재하지 않습니다. nStepNo = " + nStepNo2);
				continue;
			}
			IllustratedBookExplorationStepAttr attr = new IllustratedBookExplorationStepAttr(step2);
			attr.Set(dr3);
			step2.AddAttr(attr);
		}
		foreach (DataRow dr in UserDac.IllustratedBookExplorationStepRewards(conn, null))
		{
			int nStepNo = Convert.ToInt32(dr["stepNo"]);
			IllustratedBookExplorationStep step = GetIllustratedBookExplorationStep(nStepNo);
			if (step == null)
			{
				SFLogUtil.Warn(GetType(), "[도감탐험단계보상 목록] 도감탐험단계가 존재하지 않습니다. nStepNo = " + nStepNo);
				continue;
			}
			IllustratedBookExplorationStepReward reward = new IllustratedBookExplorationStepReward(step);
			reward.Set(dr);
			step.AddReward(reward);
		}
	}

	private void LoadUserDBResources_Title(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.TitleTypes(conn, null))
		{
			TitleType type = new TitleType();
			type.Set(dr2);
			AddTitleType(type);
		}
		foreach (DataRow dr4 in UserDac.Titles(conn, null))
		{
			Title title3 = new Title();
			title3.Set(dr4);
			AddTitle(title3);
		}
		foreach (DataRow dr3 in UserDac.TitleActiveAttrs(conn, null))
		{
			int nTitleId2 = Convert.ToInt32(dr3["titleId"]);
			Title title2 = GetTitle(nTitleId2);
			if (title2 == null)
			{
				SFLogUtil.Warn(GetType(), "[칭호액티브속성 목록] 칭호가 존재하지 않습니다. nTitleId = " + nTitleId2);
				continue;
			}
			TitleActiveAttr attr2 = new TitleActiveAttr(title2);
			attr2.Set(dr3);
			title2.AddActiveAttr(attr2);
		}
		foreach (DataRow dr in UserDac.TitlePassiveAttrs(conn, null))
		{
			int nTitleId = Convert.ToInt32(dr["titleId"]);
			Title title = GetTitle(nTitleId);
			if (title == null)
			{
				SFLogUtil.Warn(GetType(), "[칭호패시브속성 목록] 칭호가 존재하지 않습니다. nTitleId = " + nTitleId);
				continue;
			}
			TitlePassiveAttr attr = new TitlePassiveAttr(title);
			attr.Set(dr);
			title.AddPassiveAttr(attr);
		}
	}

	private void LoadUserDBResources_CreatureCard(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.CreatureCardGrades(conn, null))
		{
			CreatureCardGrade grade = new CreatureCardGrade();
			grade.Set(dr2);
			AddCreatureCardGrade(grade);
		}
		foreach (DataRow dr in UserDac.CreatureCards(conn, null))
		{
			CreatureCard card = new CreatureCard();
			card.Set(dr);
			AddCreatureCard(card);
		}
	}

	private void LoadUserDBResources_CreatureCardCollection(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.CreatureCardCollectionGrades(conn, null))
		{
			CreatureCardCollectionGrade grade = new CreatureCardCollectionGrade();
			grade.Set(dr2);
			AddCreatureCardCollectionGrade(grade);
		}
		foreach (DataRow dr5 in UserDac.CreatureCardCollectionCategories(conn, null))
		{
			CreatureCardCollectionCategory category2 = new CreatureCardCollectionCategory();
			category2.Set(dr5);
			AddCreatureCardCollectionCategory(category2);
		}
		foreach (DataRow dr4 in UserDac.CreatureCardCollections(conn, null))
		{
			int nCategoryId = Convert.ToInt32(dr4["categoryId"]);
			CreatureCardCollectionCategory category = GetCreatureCardCollectionCategory(nCategoryId);
			if (category == null)
			{
				SFLogUtil.Warn(GetType(), "[크리처카드컬렉션 목록] 크리처카드컬렉션카테고리가 존재하지 않습니다. nCategoryId = " + nCategoryId);
				continue;
			}
			CreatureCardCollection collection3 = new CreatureCardCollection(category);
			collection3.Set(dr4);
			AddCreatureCardCollection(collection3);
			category.AddCollection(collection3);
		}
		foreach (DataRow dr3 in UserDac.CreatureCardCollectionAttrs(conn, null))
		{
			int nCollectionId2 = Convert.ToInt32(dr3["collectionId"]);
			CreatureCardCollection collection2 = GetCreatureCardCollection(nCollectionId2);
			if (collection2 == null)
			{
				SFLogUtil.Warn(GetType(), "[크리처카드컬렉션속성 목록] 크리처카드컬렉션이 존재하지 않습니다. nCollectionId = " + nCollectionId2);
				continue;
			}
			CreatureCardCollectionAttr attr = new CreatureCardCollectionAttr(collection2);
			attr.Set(dr3);
			collection2.AddAttr(attr);
		}
		foreach (DataRow dr in UserDac.CreatureCardCollectionEntries(conn, null))
		{
			int nCollectionId = Convert.ToInt32(dr["collectionId"]);
			CreatureCardCollection collection = GetCreatureCardCollection(nCollectionId);
			if (collection == null)
			{
				SFLogUtil.Warn(GetType(), "[크리처카드컬렉션항목 목록] 크리처카드컬렉션이 존재하지 않습니다. nCollectionId = " + nCollectionId);
				continue;
			}
			CreatureCardCollectionEntry entry = new CreatureCardCollectionEntry(collection);
			entry.Set(dr);
			collection.AddEntry(entry);
		}
	}

	private void LoadUserDBResources_CreatureCardShop(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.CreatureCardShopRefreshSchedules(conn, null))
		{
			CreatureCardShopRefreshSchedule schedule = new CreatureCardShopRefreshSchedule();
			schedule.Set(dr2);
			AddCreatureCardShopRefreshSchedule(schedule);
		}
		foreach (DataRow dr3 in UserDac.CreatureCardShopFixedProducts(conn, null))
		{
			CreatureCardShopFixedProduct product2 = new CreatureCardShopFixedProduct();
			product2.Set(dr3);
			AddCreatureCardShopFixedProduct(product2);
		}
		foreach (DataRow dr in UserDac.CreatureCardShopRandomProducts(conn, null))
		{
			CreatureCardShopRandomProduct product = new CreatureCardShopRandomProduct();
			product.Set(dr);
			AddCreatureCardShopRandomProduct(product);
			m_nCreatureCardShopRandomProductsTotalPoint += product.point;
		}
	}

	private void LoadUserDBResources_EliteMonster(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.EliteMonsterCategories(conn, null))
		{
			EliteMonsterCategory category = new EliteMonsterCategory();
			category.Set(dr2);
			AddEliteMonsterCategory(category);
			category.continent?.SetEliteMonsterCategory(category);
		}
		foreach (DataRow dr5 in UserDac.EliteMonsterMasters(conn, null))
		{
			EliteMonsterMaster master2 = new EliteMonsterMaster();
			master2.Set(dr5);
			AddEliteMonsterMaster(master2);
			master2.category?.AddMaster(master2);
		}
		foreach (DataRow dr4 in UserDac.EliteMonsterSpawnSchedules(conn, null))
		{
			int nEliteMonsterMasterId = Convert.ToInt32(dr4["eliteMonsterMasterId"]);
			EliteMonsterMaster master = GetEliteMonsterMaster(nEliteMonsterMasterId);
			if (master == null)
			{
				SFLogUtil.Warn(GetType(), "[정예몬스터출몰스케줄 목록] 정예몬스터마스터가 존재하지 않습니다. nEliteMonsterMasterId = " + nEliteMonsterMasterId);
				continue;
			}
			EliteMonsterSpawnSchedule spawnSchedule = new EliteMonsterSpawnSchedule(master);
			spawnSchedule.Set(dr4);
			master.AddSpawnSchedule(spawnSchedule);
		}
		foreach (DataRow dr3 in UserDac.EliteMonsters(conn, null))
		{
			EliteMonster eliteMonster2 = new EliteMonster();
			eliteMonster2.Set(dr3);
			AddEliteMonster(eliteMonster2);
			eliteMonster2.master?.AddEliteMonster(eliteMonster2);
		}
		foreach (DataRow dr in UserDac.EliteMonsterKillAttrValues(conn, null))
		{
			int nEliteMonsterId = Convert.ToInt32(dr["eliteMonsterId"]);
			EliteMonster eliteMonster = GetEliteMonster(nEliteMonsterId);
			if (eliteMonster == null)
			{
				SFLogUtil.Warn(GetType(), "[정예몬스터처치속성 목록] 정예몬스터가 존재하지 않습니다. nEliteMonsterId = " + nEliteMonsterId);
				continue;
			}
			EliteMonsterKillAttrValue killAttrValue = new EliteMonsterKillAttrValue(eliteMonster);
			killAttrValue.Set(dr);
			eliteMonster.AddKillAttrValue(killAttrValue);
		}
	}

	private void LoadUserDBResources_StaminaRecovery(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.StaminaBuyCounts(conn, null))
		{
			StaminaBuyCount count = new StaminaBuyCount();
			count.Set(dr2);
			AddStaminaBuyCounts(count);
		}
		foreach (DataRow dr in UserDac.StaminaRecoverySchedule(conn, null))
		{
			StaminaRecoverySchedule schedule = new StaminaRecoverySchedule();
			schedule.Set(dr);
			AddStaminaRecoverySchedule(schedule);
		}
	}

	private void LoadUserDBResources_ExpFactor(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.MonsterKillExpFactors(conn, null))
		{
			MonsterKillExpFactor monsterKillExpFactor = new MonsterKillExpFactor();
			monsterKillExpFactor.Set(dr2);
			AddMonsterKillExpFactor(monsterKillExpFactor);
		}
		foreach (DataRow dr3 in UserDac.WorldLevelExpFactors(conn, null))
		{
			WorldLevelExpFactor worldLevelExpFactor = new WorldLevelExpFactor();
			worldLevelExpFactor.Set(dr3);
			AddWorldLevelExpFactor(worldLevelExpFactor);
		}
		foreach (DataRow dr in UserDac.PartyExpFactors(conn, null))
		{
			PartyExpFactor partyExpFactor = new PartyExpFactor();
			partyExpFactor.Set(dr);
			AddPartyExpFactor(partyExpFactor);
		}
	}

	private void LoadUserDBResources_NpcShop(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.NpcShops(conn, null))
		{
			NpcShop shop3 = new NpcShop();
			shop3.Set(dr2);
			AddNpcShop(shop3);
		}
		foreach (DataRow dr3 in UserDac.NpcShopCategories(conn, null))
		{
			int nShopId2 = Convert.ToInt32(dr3["shopId"]);
			NpcShop shop2 = GetNpcShop(nShopId2);
			if (shop2 == null)
			{
				SFLogUtil.Warn(GetType(), "[NPC상점카테고리 목록] NPC상점이 존재하지 않습니다. nShopId = " + nShopId2);
				continue;
			}
			NpcShopCategory category2 = new NpcShopCategory(shop2);
			category2.Set(dr3);
			shop2.AddCategory(category2);
		}
		foreach (DataRow dr in UserDac.NpcShopProducts(conn, null))
		{
			int nShopId = Convert.ToInt32(dr["shopId"]);
			NpcShop shop = GetNpcShop(nShopId);
			if (shop == null)
			{
				SFLogUtil.Warn(GetType(), "[NPC상점상품 목록] NPC상점이 존재하지 않습니다. nShopId = " + nShopId);
				continue;
			}
			int nCategoryId = Convert.ToInt32(dr["categoryId"]);
			NpcShopCategory category = shop.GetCategory(nCategoryId);
			if (category == null)
			{
				SFLogUtil.Warn(GetType(), "[NPC상점상품 목록] NPC상점카테고리가 존재하지 않습니다. nShopId = " + nShopId + ", nCategoryId = " + nCategoryId);
			}
			else
			{
				NpcShopProduct product = new NpcShopProduct(category);
				product.Set(dr);
				category.AddNpcShopProduct(product);
				AddNpcShopProduct(product);
			}
		}
	}

	private void LoadUserDBResources_RookieGift(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.RookieGifts(conn, null))
		{
			RookieGift gift2 = new RookieGift();
			gift2.Set(dr2);
			AddRookieGift(gift2);
		}
		foreach (DataRow dr in UserDac.RookieGiftRewards(conn, null))
		{
			int nNo = Convert.ToInt32(dr["giftNo"]);
			RookieGift gift = GetRookieGift(nNo);
			if (gift == null)
			{
				SFLogUtil.Warn(GetType(), "[신병선물보상 목록] 신병선물이 존재하지 않습니다. nNo = " + nNo);
				continue;
			}
			RookieGiftReward reward = new RookieGiftReward(gift);
			reward.Set(dr);
			gift.AddReward(reward);
		}
	}

	private void LoadUserDBResources_OpenGift(SqlConnection conn)
	{
		foreach (DataRow dr in UserDac.OpenGiftRewards(conn, null))
		{
			int nDay = Convert.ToInt32(dr["day"]);
			OpenGift gift = GetOpenGift(nDay);
			if (gift == null)
			{
				gift = new OpenGift(nDay);
				AddOpenGift(gift);
			}
			OpenGiftReward reward = new OpenGiftReward(gift);
			reward.Set(dr);
			gift.AddReward(reward);
		}
	}

	private void LoadUserDBResources_DailyQuest(SqlConnection conn)
	{
		DataRow drDailyQuest = UserDac.DailyQuest(conn, null);
		if (drDailyQuest == null)
		{
			return;
		}
		m_dailyQuest = new DailyQuest();
		m_dailyQuest.Set(drDailyQuest);
		foreach (DataRow dr3 in UserDac.DailyQuestRewards(conn, null))
		{
			DailyQuestReward reward = new DailyQuestReward();
			reward.Set(dr3);
			m_dailyQuest.AddReward(reward);
		}
		foreach (DataRow dr2 in UserDac.DailyQuestGrades(conn, null))
		{
			DailyQuestGrade grade = new DailyQuestGrade();
			grade.Set(dr2);
			m_dailyQuest.AddGrade(grade);
		}
		foreach (DataRow dr in UserDac.DailyQuestMissions(conn, null))
		{
			DailyQuestMission mission = new DailyQuestMission();
			mission.Set(dr);
			m_dailyQuest.AddMission(mission);
		}
	}

	private void LoadUserDBResources_WeeklyQuest(SqlConnection conn)
	{
		DataRow drWeeklyQuest = UserDac.WeeklyQuest(conn, null);
		if (drWeeklyQuest == null)
		{
			return;
		}
		m_weeklyQuest = new WeeklyQuest();
		m_weeklyQuest.Set(drWeeklyQuest);
		foreach (DataRow dr3 in UserDac.WeeklyQuestRoundRewards(conn, null))
		{
			WeeklyQuestRoundReward reward2 = new WeeklyQuestRoundReward();
			reward2.Set(dr3);
			WeeklyQuestRound round = m_weeklyQuest.GetRound(reward2.roundNo);
			if (round == null)
			{
				SFLogUtil.Warn(GetType(), "[주간퀘스트라운드보상 목록] 주간퀘스트라운드가 존재하지 않습니다. reward.roundNo = " + reward2.roundNo + ", reward.level = " + reward2.level);
			}
			else
			{
				round.AddReward(reward2);
			}
		}
		foreach (DataRow dr2 in UserDac.WeeklyQuestTenRoudRewards(conn, null))
		{
			WeeklyQuestTenRoundReward reward = new WeeklyQuestTenRoundReward();
			reward.Set(dr2);
			m_weeklyQuest.AddTenRoundReward(reward);
		}
		foreach (DataRow dr in UserDac.WeeklyQuestMissions(conn, null))
		{
			WeeklyQuestMission mission = new WeeklyQuestMission();
			mission.Set(dr);
			m_weeklyQuest.AddMission(mission);
		}
	}

	private void LoadUserDBResources_Open7DayEvent(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.Open7DayEventDays(conn, null))
		{
			Open7DayEventDay day = new Open7DayEventDay();
			day.Set(dr2);
			m_open7DayEventDays.Add(day.day, day);
		}
		foreach (DataRow dr4 in UserDac.Open7DayEventProducts(conn, null))
		{
			Open7DayEventProduct product = new Open7DayEventProduct();
			product.Set(dr4);
			AddOpen7DayEventDayProduct(product);
		}
		foreach (DataRow dr3 in UserDac.Open7DayEventMissions(conn, null))
		{
			Open7DayEventMission mission2 = new Open7DayEventMission();
			mission2.Set(dr3);
			AddOpen7DayEventMission(mission2);
		}
		foreach (DataRow dr in UserDac.Open7DayEventMissionRewards(conn, null))
		{
			int nMissionId = Convert.ToInt32(dr["missionId"]);
			Open7DayEventMission mission = GetOpen7DayEventMission(nMissionId);
			if (mission == null)
			{
				SFLogUtil.Warn(GetType(), "[오픈7일이벤트미션보상 목록] 미션이 존재하지 않습니다. nMissionId = " + nMissionId);
				continue;
			}
			Open7DayEventMissionReward reward = new Open7DayEventMissionReward(mission);
			reward.Set(dr);
			mission.AddReward(reward);
		}
	}

	private void LoadUserDBResources_Retrieval(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.Retrievals(conn, null))
		{
			Retrieval retrieval2 = new Retrieval();
			retrieval2.Set(dr2);
			AddRetireval(retrieval2);
		}
		foreach (DataRow dr in UserDac.RetrievalRewards(conn, null))
		{
			int nId = Convert.ToInt32(dr["retrievalId"]);
			Retrieval retrieval = GetRetrieval(nId);
			if (retrieval == null)
			{
				SFLogUtil.Warn(GetType(), "[회수보상 목록] 회수가 존재하지 않습니다. nId = " + nId);
				continue;
			}
			RetrievalReward reward = new RetrievalReward(retrieval);
			reward.Set(dr);
			retrieval.AddReward(reward);
		}
	}

	private void LoadUserDBResources_TaskConsignment(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.TaskConsignments(conn, null))
		{
			TaskConsignment consignment3 = new TaskConsignment();
			consignment3.Set(dr2);
			AddTaskConsignment(consignment3);
		}
		foreach (DataRow dr3 in UserDac.TaskConsignmentExpRewards(conn, null))
		{
			int nConsignmentId2 = Convert.ToInt32(dr3["consignmentId"]);
			TaskConsignment consignment2 = GetTaskConsignment(nConsignmentId2);
			if (consignment2 == null)
			{
				SFLogUtil.Warn(GetType(), "[할일위탁경험치보상 목록] 할일위탁이 존재하지 않습니다. nConsignmentId = " + nConsignmentId2);
				continue;
			}
			TaskConsignmentExpReward expReward = new TaskConsignmentExpReward(consignment2);
			expReward.Set(dr3);
			consignment2.AddExpReward(expReward);
		}
		foreach (DataRow dr in UserDac.TaskConsignmentItemRewards(conn, null))
		{
			int nConsignmentId = Convert.ToInt32(dr["consignmentId"]);
			TaskConsignment consignment = GetTaskConsignment(nConsignmentId);
			if (consignment == null)
			{
				SFLogUtil.Warn(GetType(), "[할일위탁아이템보상 목록] 할일위탁이 존재하지 않습니다. nConsignmentId = " + nConsignmentId);
				continue;
			}
			TaskConsignmentItemReward itemReward = new TaskConsignmentItemReward(consignment);
			itemReward.Set(dr);
			consignment.AddItemReward(itemReward);
		}
	}

	private void LoadUserDBResources_TrueHeroQuest(SqlConnection conn)
	{
		DataRow drTrueHeroQuest = UserDac.TrueHeroQuest(conn, null);
		if (drTrueHeroQuest == null)
		{
			return;
		}
		m_trueHeroQuest = new TrueHeroQuest();
		m_trueHeroQuest.Set(drTrueHeroQuest);
		foreach (DataRow dr2 in UserDac.TrueHeroQuestSteps(conn, null))
		{
			TrueHeroQuestStep step = new TrueHeroQuestStep(m_trueHeroQuest);
			step.Set(dr2);
			m_trueHeroQuest.AddStep(step);
		}
		foreach (DataRow dr in UserDac.TrueHeroQuestRewards(conn, null))
		{
			TrueHeroQuestReward reward = new TrueHeroQuestReward(m_trueHeroQuest);
			reward.Set(dr);
			m_trueHeroQuest.AddReward(reward);
		}
	}

	private void LoadUserDBResources_FieldBoss(SqlConnection conn)
	{
		DataRow drFieldBossEvent = UserDac.FieldBossEvent(conn, null);
		if (drFieldBossEvent == null)
		{
			SFLogUtil.Warn(GetType(), "[필드보스이벤트] 필드보스이벤트가 존재하지 않습니다.");
			return;
		}
		FieldBossEvent fieldBossEvent = new FieldBossEvent();
		fieldBossEvent.Set(drFieldBossEvent);
		m_fieldBossEvent = fieldBossEvent;
		foreach (DataRow dr2 in UserDac.FieldBossEventSchedules(conn, null))
		{
			FieldBossEventSchedule schedule = new FieldBossEventSchedule(m_fieldBossEvent);
			schedule.Set(dr2);
			m_fieldBossEvent.AddSchedule(schedule);
		}
		foreach (DataRow dr in UserDac.FieldBosses(conn, null))
		{
			FieldBoss fieldBoss = new FieldBoss(m_fieldBossEvent);
			fieldBoss.Set(dr);
			m_fieldBossEvent.AddFieldBoss(fieldBoss);
		}
	}

	private void LoadUserDBResources_LimitationGift(SqlConnection conn)
	{
		DataRow drLimitationGift = UserDac.LimitationGift(conn, null);
		if (drLimitationGift == null)
		{
			return;
		}
		m_limitationGift = new LimitationGift();
		m_limitationGift.Set(drLimitationGift);
		foreach (DataRow dr3 in UserDac.LimitationGiftRewardDayOfWeeks(conn, null))
		{
			int nDayOfWeek = Convert.ToInt32(dr3["dayOfWeek"]);
			if (!Enum.IsDefined(typeof(DayOfWeek), nDayOfWeek))
			{
				SFLogUtil.Warn(GetType(), "[한정선물보상요일 목록] 요일이 유효하지 않습니다. nDayOfWeek = " + nDayOfWeek);
			}
			else
			{
				m_limitationGift.AddDayOfWeek((DayOfWeek)nDayOfWeek);
			}
		}
		foreach (DataRow dr2 in UserDac.LimitationGiftRewardSchedules(conn, null))
		{
			LimitationGiftRewardSchedule schedule2 = new LimitationGiftRewardSchedule();
			schedule2.Set(dr2);
			m_limitationGift.AddSchedule(schedule2);
		}
		foreach (DataRow dr in UserDac.LimitationGiftRewards(conn, null))
		{
			int nSchduleId = Convert.ToInt32(dr["scheduleId"]);
			LimitationGiftRewardSchedule schedule = m_limitationGift.GetSchedule(nSchduleId);
			if (schedule == null)
			{
				SFLogUtil.Warn(GetType(), "[한정선물보상 목록] 스케쥴이 존재하지 않습니다. nSchduleId = " + nSchduleId);
				continue;
			}
			LimitationGiftReward reward = new LimitationGiftReward(schedule);
			reward.Set(dr);
			schedule.AddReward(reward);
		}
	}

	private void LoadUserDBResources_WeekendReward(SqlConnection conn)
	{
		DataRow drWeekendReward = UserDac.WeekendReward(conn, null);
		if (drWeekendReward == null)
		{
			return;
		}
		m_weekendReward = new WeekendReward();
		m_weekendReward.Set(drWeekendReward);
		foreach (DataRow dr in UserDac.WeekendRewardNumberPoolEntries(conn, null))
		{
			int nSelectionNo = Convert.ToInt32(dr["selectionNo"]);
			WeekendRewardNumberPool pool = m_weekendReward.GetNumberPool(nSelectionNo);
			if (pool == null)
			{
				pool = new WeekendRewardNumberPool(nSelectionNo);
				m_weekendReward.AddNumberPool(pool);
			}
			WeekendRewardNumberPoolEntry entry = new WeekendRewardNumberPoolEntry(pool);
			entry.Set(dr);
			pool.AddEntry(entry);
		}
	}

	private void LoadUserDBResources_Warehouse(SqlConnection conn)
	{
		foreach (DataRow dr in UserDac.WarehouseSlotExtendRecipes(conn, null))
		{
			WarehouseSlotExtendRecipe recipe = new WarehouseSlotExtendRecipe();
			recipe.Set(dr);
			AddWarehouseSlotExtendRecipe(recipe);
		}
	}

	private void LoadUserDBResources_DiaShop(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.DiaShopCategories(conn, null))
		{
			DiaShopCategory category = new DiaShopCategory();
			category.Set(dr2);
			AddDiaShopCategory(category);
		}
		foreach (DataRow dr in UserDac.DiaShopProducts(conn, null))
		{
			DiaShopProduct product = new DiaShopProduct();
			product.Set(dr);
			AddDiaShopProducts(product);
		}
	}

	private void LoadUserDBResources_WingMemoryPiece(SqlConnection conn)
	{
		foreach (DataRow dr3 in UserDac.WingMemoryPiecesSlots(conn, null))
		{
			int nWingId4 = Convert.ToInt32(dr3["wingId"]);
			Wing wing4 = GetWing(nWingId4);
			if (wing4 == null)
			{
				SFLogUtil.Warn(GetType(), "[날개기억조각슬롯 목록] 날개가 존재하지 않습니다. nWingId = " + nWingId4);
				continue;
			}
			WingMemoryPieceSlot slot = new WingMemoryPieceSlot(wing4);
			slot.Set(dr3);
			wing4.AddMemoryPieceSlot(slot);
		}
		foreach (DataRow dr6 in UserDac.WingMemoryPieceSlotSteps(conn, null))
		{
			int nWingId5 = Convert.ToInt32(dr6["wingId"]);
			Wing wing5 = GetWing(nWingId5);
			if (wing5 == null)
			{
				SFLogUtil.Warn(GetType(), "[날개기억조각슬롯단계 목록] 날개가 존재하지 않습니다. nWingId = " + nWingId5);
				continue;
			}
			int nIndex = Convert.ToInt32(dr6["slotIndex"]);
			WingMemoryPieceSlot slot2 = wing5.GetMemoryPieceSlot(nIndex);
			if (slot2 == null)
			{
				SFLogUtil.Warn(GetType(), "[날개기억조각슬롯단계 목록] 슬롯이 존재하지 않습니다. nWingId = " + nWingId5 + ", nIndex = " + nIndex);
			}
			else
			{
				WingMemoryPieceSlotStep slotStep = new WingMemoryPieceSlotStep(slot2);
				slotStep.Set(dr6);
				slot2.AddStep(slotStep);
			}
		}
		foreach (DataRow dr5 in UserDac.WingMemoryPieceTypes(conn, null))
		{
			WingMemoryPieceType type = new WingMemoryPieceType();
			type.Set(dr5);
			AddWingMemoryPieceType(type);
		}
		foreach (DataRow dr4 in UserDac.WingMemoryPieceSteps(conn, null))
		{
			int nWingId3 = Convert.ToInt32(dr4["wingId"]);
			Wing wing3 = GetWing(nWingId3);
			if (wing3 == null)
			{
				SFLogUtil.Warn(GetType(), "[날개기억조각슬롯단계 목록] 날개가 존재하지 않습니다. nWingId = " + nWingId3);
				continue;
			}
			WingMemoryPieceStep step3 = new WingMemoryPieceStep();
			step3.Set(dr4);
			wing3.AddMemoryPieceStep(step3);
		}
		foreach (DataRow dr2 in UserDac.WingMemoryPieceCriticalCountPoolEntries(conn, null))
		{
			int nWingId2 = Convert.ToInt32(dr2["wingId"]);
			Wing wing2 = GetWing(nWingId2);
			if (wing2 == null)
			{
				SFLogUtil.Warn(GetType(), "[날개기억조각크리티컬수량풀항목 목록] 날개가 존재하지 않습니다. nWingId = " + nWingId2);
				continue;
			}
			int nStep2 = Convert.ToInt32(dr2["step"]);
			WingMemoryPieceStep step2 = wing2.GetMemoryPieceStep(nStep2);
			if (step2 == null)
			{
				SFLogUtil.Warn(GetType(), "[날개기억조각크리티컬수량풀항목 목록] 단계가 존재하지 않습니다. nWingId = " + nWingId2 + ", nStep = " + nStep2);
			}
			else
			{
				WingMemoryPieceCriticalCountPoolEntry entry2 = new WingMemoryPieceCriticalCountPoolEntry(step2);
				entry2.Set(dr2);
				step2.AddCriticalCountEnty(entry2);
			}
		}
		foreach (DataRow dr in UserDac.WingMemoryPieceSuccessFactorPoolEntries(conn, null))
		{
			int nWingId = Convert.ToInt32(dr["wingId"]);
			Wing wing = GetWing(nWingId);
			if (wing == null)
			{
				SFLogUtil.Warn(GetType(), "[날개기억조각크리티컬수량풀항목 목록] 날개가 존재하지 않습니다. nWingId = " + nWingId);
				continue;
			}
			int nStep = Convert.ToInt32(dr["step"]);
			WingMemoryPieceStep step = wing.GetMemoryPieceStep(nStep);
			if (step == null)
			{
				SFLogUtil.Warn(GetType(), "[날개기억조각크리티컬수량풀항목 목록] 단계가 존재하지 않습니다. nWingId = " + nWingId + ", nStep = " + nStep);
			}
			else
			{
				WingMemoryPieceSuccessFactorPoolEntry entry = new WingMemoryPieceSuccessFactorPoolEntry(step);
				entry.Set(dr);
				step.AddSuccessFactorEntry(entry);
			}
		}
	}

	private void LoadUserDBResources_SubQuest(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.SubQuests(conn, null))
		{
			SubQuest quest2 = new SubQuest();
			quest2.Set(dr2);
			AddSubQuest(quest2);
		}
		foreach (DataRow dr in UserDac.SubQuestRewards(conn, null))
		{
			int nQuestId = Convert.ToInt32(dr["questId"]);
			SubQuest quest = GetSubQuest(nQuestId);
			if (quest == null)
			{
				SFLogUtil.Warn(GetType(), "서브퀘스트가 존재하지 않습니다. nQuestId = " + nQuestId);
				continue;
			}
			SubQuestReward reward = new SubQuestReward(quest);
			reward.Set(dr);
			quest.AddReward(reward);
		}
	}

	private void LoadUserDBResources_OrdealQuest(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.OrdealQuests(conn, null))
		{
			OrdealQuest quest2 = new OrdealQuest();
			quest2.Set(dr2);
			AddOrdealQuest(quest2);
		}
		foreach (DataRow dr in UserDac.OrdealQuestMissions(conn, null))
		{
			int nQuestNo = Convert.ToInt32(dr["questNo"]);
			OrdealQuest quest = GetOrdealQuest(nQuestNo);
			if (quest == null)
			{
				SFLogUtil.Warn(GetType(), "[시련퀘스트미션 목록] 시련퀘스트가 존재하지 않습니다. nQuestNo = " + nQuestNo);
				continue;
			}
			int nIndex = Convert.ToInt32(dr["slotIndex"]);
			OrdealQuestSlot slot = quest.GetSlot(nIndex);
			if (slot == null)
			{
				SFLogUtil.Warn(GetType(), "[시련퀘스트미션 목록] 시련퀘스트슬롯이 존재하지 않습니다. nQuestNo = " + nQuestNo + ", nIndex = " + nIndex);
			}
			else
			{
				OrdealQuestMission mission = new OrdealQuestMission(slot);
				mission.Set(dr);
				slot.AddMission(mission);
			}
		}
	}

	private void LoadUserDBResources_MoneyBuff(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.MoneyBuffs(conn, null))
		{
			MoneyBuff moneyBuff2 = new MoneyBuff();
			moneyBuff2.Set(dr2);
			AddMoneyBuff(moneyBuff2);
		}
		foreach (DataRow dr in UserDac.MoneyBuffAttrs(conn, null))
		{
			int nBuffId = Convert.ToInt32(dr["buffId"]);
			MoneyBuff moneyBuff = GetMoneyBuff(nBuffId);
			if (moneyBuff == null)
			{
				SFLogUtil.Warn(GetType(), "[재화버프속성 목록] 재화버프가 존재하지 않습니다. nBuffId = " + nBuffId);
				continue;
			}
			MoneyBuffAttr attr = new MoneyBuffAttr(moneyBuff);
			attr.Set(dr);
			moneyBuff.AddAttr(attr);
		}
	}

	private void LoadUserDBResources_Biography(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.Biographies(conn, null))
		{
			Biography biography3 = new Biography();
			biography3.Set(dr2);
			AddBiography(biography3);
		}
		foreach (DataRow dr3 in UserDac.BioGraphyRewards(conn, null))
		{
			int nBiographyId2 = Convert.ToInt32(dr3["biographyId"]);
			Biography biography2 = GetBiography(nBiographyId2);
			if (biography2 == null)
			{
				SFLogUtil.Warn(GetType(), "전기가 존재하지 않습니다. nBiographyId = " + nBiographyId2);
				continue;
			}
			BiographyReward reward = new BiographyReward(biography2);
			reward.Set(dr3);
			biography2.AddReward(reward);
		}
		foreach (DataRow dr in UserDac.BiographyQuests(conn, null))
		{
			int nBiographyId = Convert.ToInt32(dr["biographyId"]);
			Biography biography = GetBiography(nBiographyId);
			if (biography == null)
			{
				SFLogUtil.Warn(GetType(), "전기가 존재하지 않습니다. nBiographyId = " + nBiographyId);
				continue;
			}
			BiographyQuest quest = new BiographyQuest(biography);
			quest.Set(dr);
			biography.AddQuest(quest);
		}
	}

	private void LoadUserDBResources_ItemLuckyShop(SqlConnection conn)
	{
		DataRow drItemLuckyShop = UserDac.ItemLuckyShop(conn, null);
		if (drItemLuckyShop == null)
		{
			return;
		}
		m_itemLuckyShop = new ItemLuckyShop();
		m_itemLuckyShop.Set(drItemLuckyShop);
		foreach (DataRow dr2 in UserDac.ItemLuckyShopNormalPoolEntries(conn, null))
		{
			ItemLuckyShopNormalPoolEntry entry2 = new ItemLuckyShopNormalPoolEntry();
			entry2.Set(dr2);
			m_itemLuckyShop.AddNormalEntry(entry2);
		}
		foreach (DataRow dr in UserDac.ItemLuckyShopSpecialPoolEntries(conn, null))
		{
			ItemLuckyShopSpecialPoolEntry entry = new ItemLuckyShopSpecialPoolEntry();
			entry.Set(dr);
			m_itemLuckyShop.AddSpecialEntry(entry);
		}
	}

	private void LoadUserDBResources_CreatureCardLuckyShop(SqlConnection conn)
	{
		DataRow drCreatureCardLuckyShop = UserDac.CreatureCardLuckyShop(conn, null);
		if (drCreatureCardLuckyShop == null)
		{
			return;
		}
		m_creatureCardLuckyShop = new CreatureCardLuckyShop();
		m_creatureCardLuckyShop.Set(drCreatureCardLuckyShop);
		foreach (DataRow dr2 in UserDac.CreatureCardLuckyShopNormalPoolEntries(conn, null))
		{
			CreatureCardLuckyShopNormalPoolEntry entry2 = new CreatureCardLuckyShopNormalPoolEntry();
			entry2.Set(dr2);
			m_creatureCardLuckyShop.AddNormalEntry(entry2);
		}
		foreach (DataRow dr in UserDac.CreatureCardLuckyShopSpecialPoolEntries(conn, null))
		{
			CreatureCardLuckyShopSpecialPoolEntry entry = new CreatureCardLuckyShopSpecialPoolEntry();
			entry.Set(dr);
			m_creatureCardLuckyShop.AddSpecialEntry(entry);
		}
	}

	private void LoadUserDBResources_Blessing(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.Blessings(conn, null))
		{
			Blessing blessing = new Blessing();
			blessing.Set(dr2);
			m_blessings.Add(blessing.id, blessing);
		}
		foreach (DataRow dr4 in UserDac.BlessingTargetLevels(conn, null))
		{
			BlessingTargetLevel level = new BlessingTargetLevel();
			level.Set(dr4);
			m_blessingTargetLevels.Add(level.id, level);
		}
		foreach (DataRow dr3 in UserDac.ProspectQuestOwnerRewards(conn, null))
		{
			int nTargetLevelId2 = Convert.ToInt32(dr3["targetLevelId"]);
			BlessingTargetLevel targetLevel2 = GetBlessingTargetLevel(nTargetLevelId2);
			if (targetLevel2 == null)
			{
				SFLogUtil.Warn(GetType(), "[유망자퀘스트소유자보상 목록] 축복대상레벨이 존재하지 않습니다. nTargetLevelId = " + nTargetLevelId2);
				continue;
			}
			ProspectQuestOwnerReward reward2 = new ProspectQuestOwnerReward(targetLevel2);
			reward2.Set(dr3);
			targetLevel2.AddProspectQuestOwnerReward(reward2);
		}
		foreach (DataRow dr in UserDac.ProspectQuestTargetRewards(conn, null))
		{
			int nTargetLevelId = Convert.ToInt32(dr["targetLevelId"]);
			BlessingTargetLevel targetLevel = GetBlessingTargetLevel(nTargetLevelId);
			if (targetLevel == null)
			{
				SFLogUtil.Warn(GetType(), "[유망자퀘스트대상자보상 목록] 축복대상레벨이 존재하지 않습니다. nTargetLevelId = " + nTargetLevelId);
				continue;
			}
			ProspectQuestTargetReward reward = new ProspectQuestTargetReward(targetLevel);
			reward.Set(dr);
			targetLevel.AddProspectQuestTargetReward(reward);
		}
	}

	private void LoadUserDBResources_Present(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.Presents(conn, null))
		{
			Present present = new Present();
			present.Set(dr2);
			AddPresent(present);
		}
		foreach (DataRow dr5 in UserDac.WeeklyPresentPopularityPointRankingRewardGroups(conn, null))
		{
			WeeklyPresentPopularityPointRankingRewardGroup group4 = new WeeklyPresentPopularityPointRankingRewardGroup();
			group4.Set(dr5);
			AddWeeklyPresentPopularityPointRankingRewardGroup(group4);
		}
		foreach (DataRow dr4 in UserDac.WeeklyPresentPopularityPointRankingRewards(conn, null))
		{
			int nGroupNo2 = Convert.ToInt32(dr4["groupNo"]);
			WeeklyPresentPopularityPointRankingRewardGroup group3 = GetWeeklyPresentPopularityPointRankingRewardGroup(nGroupNo2);
			if (group3 == null)
			{
				SFLogUtil.Warn(GetType(), "[주간선물인기점수랭킹보상 목록] 그룹이 존재하지 않습니다. nGroupNo = " + nGroupNo2);
				continue;
			}
			WeeklyPresentPopularityPointRankingReward reward2 = new WeeklyPresentPopularityPointRankingReward(group3);
			reward2.Set(dr4);
			group3.AddReward(reward2);
		}
		foreach (DataRow dr3 in UserDac.WeeklyPresentContributionPointRankingRewardGroups(conn, null))
		{
			WeeklyPresentContributionPointRankingRewardGroup group2 = new WeeklyPresentContributionPointRankingRewardGroup();
			group2.Set(dr3);
			AddWeeklyPresentContributionPointRankingRewardGroup(group2);
		}
		foreach (DataRow dr in UserDac.WeeklyPresentContributionPointRankingRewards(conn, null))
		{
			int nGroupNo = Convert.ToInt32(dr["groupNo"]);
			WeeklyPresentContributionPointRankingRewardGroup group = GetWeeklyPresentContributionPointRankingRewardGroup(nGroupNo);
			if (group == null)
			{
				SFLogUtil.Warn(GetType(), "[주간선물공헌점수랭킹보상 목록] 그룹이 존재하지 않습니다. nGroupNo = " + nGroupNo);
				continue;
			}
			WeeklyPresentContributionPointRankingReward reward = new WeeklyPresentContributionPointRankingReward(group);
			reward.Set(dr);
			group.AddReward(reward);
		}
	}

	private void LoadUserDBResources_Creature(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.CreatureSkilllGrades(conn, null))
		{
			CreatureSkillGrade grade = new CreatureSkillGrade();
			grade.Set(dr2);
			AddCreatureSkillGrade(grade);
		}
		foreach (DataRow dr6 in UserDac.CreatureSkills(conn, null))
		{
			CreatureSkill skill = new CreatureSkill();
			skill.Set(dr6);
			AddCreatureSkill(skill);
		}
		foreach (DataRow dr10 in UserDac.CreatureSkillAttrs(conn, null))
		{
			int nSkillId = Convert.ToInt32(dr10["skillId"]);
			CreatureSkill skill2 = GetCreatureSkill(nSkillId);
			if (skill2 == null)
			{
				SFLogUtil.Warn(GetType(), "[크리처스킬속성 목록] 크리처스킬이 존재하지 않습니다. nSkillId = " + nSkillId);
				continue;
			}
			CreatureSkillAttr attr4 = new CreatureSkillAttr(skill2);
			attr4.Set(dr10);
			skill2.AddAttr(attr4);
		}
		foreach (DataRow dr16 in UserDac.CreatureSkillCountPoolEntries(conn, null))
		{
			CreatureSkillCountPoolEntry entry3 = new CreatureSkillCountPoolEntry();
			entry3.Set(dr16);
			AddCreatureSkillCountPoolEntry(entry3);
		}
		foreach (DataRow dr17 in UserDac.CreatureCharacters(conn, null))
		{
			CreatureCharacter character2 = new CreatureCharacter();
			character2.Set(dr17);
			AddCreatureCharacter(character2);
		}
		foreach (DataRow dr15 in UserDac.CreatureCharacterSkillPoolEntries(conn, null))
		{
			int nCharacterId = Convert.ToInt32(dr15["creatureCharacterId"]);
			CreatureCharacter character = GetCreatureCharacter(nCharacterId);
			if (character == null)
			{
				SFLogUtil.Warn(GetType(), "[크리처캐릭터스킬풀항목 목록] 캐릭터가 존재하지 않습니다. nCharacterId = " + nCharacterId);
				continue;
			}
			CreatureCharacterSkillPoolEntry entry2 = new CreatureCharacterSkillPoolEntry();
			entry2.Set(dr15);
			character.AddSkillPoolEntry(entry2);
		}
		foreach (DataRow dr14 in UserDac.CreatureSkillSlotOpenRecipes(conn, null))
		{
			CreatureSkillSlotOpenRecipe recipe = new CreatureSkillSlotOpenRecipe();
			recipe.Set(dr14);
			AddCreatureSkillSlotOpenRecipe(recipe);
		}
		foreach (DataRow dr13 in UserDac.CreatureSkillSlotProtections(conn, null))
		{
			CreatureSkillSlotProtection protection = new CreatureSkillSlotProtection();
			protection.Set(dr13);
			AddCreatureSkillSlotProtection(protection);
		}
		foreach (DataRow dr12 in UserDac.CreatureGrades(conn, null))
		{
			CreatureGrade grade2 = new CreatureGrade();
			grade2.Set(dr12);
			AddCreatureGrade(grade2);
		}
		foreach (DataRow dr11 in UserDac.Creatures(conn, null))
		{
			Creature creature2 = new Creature();
			creature2.Set(dr11);
			AddCreature(creature2);
		}
		foreach (DataRow dr9 in UserDac.CreatureBaseAttrs(conn, null))
		{
			CreatureBaseAttr attr3 = new CreatureBaseAttr();
			attr3.Set(dr9);
			AddCreatureBaseAttr(attr3);
		}
		foreach (DataRow dr8 in UserDac.CreatureBaseAttrValues(conn, null))
		{
			int nCreatureId = Convert.ToInt32(dr8["creatureId"]);
			Creature creature = instance.GetCreature(nCreatureId);
			if (creature == null)
			{
				SFLogUtil.Warn(GetType(), "[크리처기본속성값 목록] 크리처가 존재하지 않습니다. nCreatureId = " + nCreatureId);
				continue;
			}
			CreatureBaseAttrValue attrValue2 = new CreatureBaseAttrValue(creature);
			attrValue2.Set(dr8);
			creature.AddBaseAttrValue(attrValue2);
		}
		foreach (DataRow dr7 in UserDac.CreatureAdditionalAttrs(conn, null))
		{
			CreatureAdditionalAttr attr2 = new CreatureAdditionalAttr();
			attr2.Set(dr7);
			AddCreatureAdditionalAttr(attr2);
		}
		foreach (DataRow dr5 in UserDac.CreatureInjectionLevels(conn, null))
		{
			CreatureInjectionLevel level2 = new CreatureInjectionLevel();
			level2.Set(dr5);
			AddCreatureInjectionLevel(level2);
		}
		foreach (DataRow dr4 in UserDac.CreatureAdditionalAttrValues(conn, null))
		{
			int nAttrId = Convert.ToInt32(dr4["attrId"]);
			CreatureAdditionalAttr attr = GetCreatureAdditionalAttr(nAttrId);
			if (attr == null)
			{
				SFLogUtil.Warn(GetType(), "[크리처추가속성값 목록] 크리처추가속성이 존재하지 않습니다. nAttrId = " + nAttrId);
				continue;
			}
			CreatureAdditionalAttrValue attrValue = new CreatureAdditionalAttrValue(attr);
			attrValue.Set(dr4);
			attr.AddAttrValue(attrValue);
		}
		foreach (DataRow dr3 in UserDac.CreatureLevels(conn, null))
		{
			CreatureLevel level = new CreatureLevel();
			level.Set(dr3);
			AddCreatureLevel(level);
		}
		foreach (DataRow dr in UserDac.CreatureInjectionLevelUpEntries(conn, null))
		{
			CreatureInjectionLevelUpEntry entry = new CreatureInjectionLevelUpEntry();
			entry.Set(dr);
			AddCreatureInjectionLevelUpEntry(entry);
		}
	}

	private void LoadUserDBResources_Costume(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.Costumes(conn, null))
		{
			Costume costume2 = new Costume();
			costume2.Set(dr2);
			AddCostume(costume2);
		}
		foreach (DataRow dr5 in UserDac.CostumeEnchantLevels(conn, null))
		{
			CostumeEnchantLevel level = new CostumeEnchantLevel();
			level.Set(dr5);
			AddCostumeEnchantLevel(level);
		}
		foreach (DataRow dr4 in UserDac.CostumeAttrs(conn, null))
		{
			int nCostumeId2 = Convert.ToInt32(dr4["costumeId"]);
			Costume costume3 = GetCostume(nCostumeId2);
			if (costume3 == null)
			{
				SFLogUtil.Warn(GetType(), "[코스튬속성 목록] 코스튬이 존재하지 않습니다. nCostumeId = " + nCostumeId2);
				continue;
			}
			CostumeAttr attr2 = new CostumeAttr(costume3);
			attr2.Set(dr4);
			costume3.AddAttr(attr2);
		}
		foreach (DataRow dr3 in UserDac.CostumeEnchantLevelAttrs(conn, null))
		{
			int nCostumeId = Convert.ToInt32(dr3["costumeId"]);
			Costume costume = GetCostume(nCostumeId);
			if (costume == null)
			{
				SFLogUtil.Warn(GetType(), "[코스튬강화레벨속성 목록] 코스튬이 존재하지 않습니다. nCostumeId = " + nCostumeId);
				continue;
			}
			int nAttrId = Convert.ToInt32(dr3["attrId"]);
			CostumeAttr attr = costume.GetAttr(nAttrId);
			if (attr == null)
			{
				SFLogUtil.Warn(GetType(), "[코스튬강화레벨속성 목록] 속성이 존재하지 않습니다. nCostumeId = " + nCostumeId + ", nAttrId = " + nAttrId);
			}
			else
			{
				CostumeEnchantLevelAttr levelAttr = new CostumeEnchantLevelAttr(attr);
				levelAttr.Set(dr3);
				attr.AddEnchantLevelAttr(levelAttr);
			}
		}
		foreach (DataRow dr in UserDac.CostumeEffects(conn, null))
		{
			CostumeEffect costumeEffect = new CostumeEffect();
			costumeEffect.Set(dr);
			AddCostumeEffect(costumeEffect);
		}
	}

	private void LoadUserDBResources_CostumeCollections(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.CostumeCollections(conn, null))
		{
			CostumeCollection collection3 = new CostumeCollection();
			collection3.Set(dr2);
			AddCostumeCollection(collection3);
		}
		foreach (DataRow dr3 in UserDac.CostumeCollectionAttrs(conn, null))
		{
			int nCollectionId2 = Convert.ToInt32(dr3["costumeCollectionId"]);
			CostumeCollection collection2 = GetCostumeCollection(nCollectionId2);
			if (collection2 == null)
			{
				SFLogUtil.Warn(GetType(), "[코스튬콜렉션속성 목록] 콜렉션이 존재하지 않습니다. nCollectionId = " + nCollectionId2);
				continue;
			}
			CostumeCollectionAttr attr = new CostumeCollectionAttr(collection2);
			attr.Set(dr3);
			collection2.AddAttr(attr);
		}
		foreach (DataRow dr in UserDac.CostumeCollectionEntries(conn, null))
		{
			int nCollectionId = Convert.ToInt32(dr["costumeCollectionId"]);
			CostumeCollection collection = GetCostumeCollection(nCollectionId);
			if (collection == null)
			{
				SFLogUtil.Warn(GetType(), "[코스튬콜렉션항목 목록] 콜렉션이 존재하지 않습니다. nCollectionId = " + nCollectionId);
				continue;
			}
			CostumeCollectionEntry entry = new CostumeCollectionEntry(collection);
			entry.Set(dr);
			collection.AddEntry(entry);
		}
	}

	private void LoadUserDBResources_CreatureFarmQuest(SqlConnection conn)
	{
		DataRow drCreatureFarmQuest = UserDac.CreatureFarmQuest(conn, null);
		if (drCreatureFarmQuest == null)
		{
			return;
		}
		m_creatureFarmQuest = new CreatureFarmQuest();
		m_creatureFarmQuest.Set(drCreatureFarmQuest);
		foreach (DataRow dr3 in UserDac.CreatureFarmQuestExpRewards(conn, null))
		{
			CreatureFarmQuestExpReward reward2 = new CreatureFarmQuestExpReward();
			reward2.Set(dr3);
			m_creatureFarmQuest.AddExpReward(reward2);
		}
		foreach (DataRow dr5 in UserDac.CreatureFarmQuestItemReawrds(conn, null))
		{
			CreatureFarmQuestItemReward reward3 = new CreatureFarmQuestItemReward();
			reward3.Set(dr5);
			m_creatureFarmQuest.AddItemReward(reward3);
		}
		foreach (DataRow dr4 in UserDac.CreatureFarmQuestMissions(conn, null))
		{
			CreatureFarmQuestMission mission3 = new CreatureFarmQuestMission();
			mission3.Set(dr4);
			m_creatureFarmQuest.AddMission(mission3);
		}
		foreach (DataRow dr2 in UserDac.CreatureFarmQuestMissionMonsterArranges(conn, null))
		{
			int nMissionNo2 = Convert.ToInt32(dr2["missionNo"]);
			CreatureFarmQuestMission mission2 = m_creatureFarmQuest.GetMission(nMissionNo2);
			if (mission2 == null)
			{
				SFLogUtil.Warn(GetType(), "[크리처농장퀘스트미션몬스터배치 목록] 크리처농장퀘스트미션이 존재하지 않습니다. nMissionNo = " + nMissionNo2);
				continue;
			}
			CreatureFarmQuestMissionMonsterArrange arrange = new CreatureFarmQuestMissionMonsterArrange(mission2);
			arrange.Set(dr2);
			mission2.AddMonsterArrange(arrange);
		}
		foreach (DataRow dr in UserDac.CreatureFarmQuestMissionRewards(conn, null))
		{
			int nMissionNo = Convert.ToInt32(dr["missionNo"]);
			CreatureFarmQuestMission mission = m_creatureFarmQuest.GetMission(nMissionNo);
			if (mission == null)
			{
				SFLogUtil.Warn(GetType(), "[크리처농장퀘스트미션보상 목록] 크리처농장퀘스트미션이 존재하지 않습니다. nMissionNo = " + nMissionNo);
				continue;
			}
			CreatureFarmQuestMissionReward reward = new CreatureFarmQuestMissionReward(mission);
			reward.Set(dr);
			mission.AddReward(reward);
		}
	}

	private void LoadUserDBResources_CashProduct(SqlConnection conn)
	{
		foreach (DataRow dr in UserDac.CashProducts(conn, null))
		{
			CashProduct product = new CashProduct();
			product.Set(dr);
			AddCashProduct(product);
		}
	}

	private void LoadUserDBResources_FirstChargeEvent(SqlConnection conn)
	{
		DataRow drEvent = UserDac.FirstChargeEvent(conn, null);
		if (drEvent == null)
		{
			SFLogUtil.Warn(GetType(), "[첫충전이벤트] 이벤트가 존재하지 않습니다.");
			return;
		}
		FirstChargeEvent evt = new FirstChargeEvent();
		evt.Set(drEvent);
		m_firstChargeEvent = evt;
		foreach (DataRow dr in UserDac.FirstChargeEventRewards(conn, null))
		{
			FirstChargeEventReward reward = new FirstChargeEventReward(m_firstChargeEvent);
			reward.Set(dr);
			m_firstChargeEvent.AddReward(reward);
		}
	}

	private void LoadUserDBResources_RechargeEvent(SqlConnection conn)
	{
		DataRow drEvent = UserDac.RechargeEvent(conn, null);
		if (drEvent == null)
		{
			SFLogUtil.Warn(GetType(), "[재충전이벤트] 이벤트가 존재하지 않습니다.");
			return;
		}
		RechargeEvent evt = new RechargeEvent();
		evt.Set(drEvent);
		m_rechargeEvent = evt;
		foreach (DataRow dr in UserDac.RechargeEventRewards(conn, null))
		{
			RechargeEventReward reward = new RechargeEventReward(m_rechargeEvent);
			reward.Set(dr);
			m_rechargeEvent.AddReward(reward);
		}
	}

	private void LoadUserDBResources_ChargeEvent(SqlConnection conn, DateTime currentTime)
	{
		foreach (DataRow dr2 in UserDac.ChargeEvents(conn, null, currentTime))
		{
			ChargeEvent evt3 = new ChargeEvent();
			evt3.Set(dr2);
			AddChargeEvent(evt3);
		}
		foreach (DataRow dr3 in UserDac.ChargeEventMissions(conn, null, currentTime))
		{
			int nEventId2 = Convert.ToInt32(dr3["eventId"]);
			ChargeEvent evt2 = GetChargeEvent(nEventId2);
			if (evt2 == null)
			{
				SFLogUtil.Warn(GetType(), "[충전이벤트미션 목록] 이벤트가 존재하지 않습니다. nEventId = " + nEventId2);
				continue;
			}
			ChargeEventMission mission2 = new ChargeEventMission(evt2);
			mission2.Set(dr3);
			evt2.AddMission(mission2);
		}
		foreach (DataRow dr in UserDac.ChargeEventMissionRewards(conn, null, currentTime))
		{
			int nEventId = Convert.ToInt32(dr["eventId"]);
			ChargeEvent evt = GetChargeEvent(nEventId);
			if (evt == null)
			{
				SFLogUtil.Warn(GetType(), "[충전이벤트미션보상 목록] 이벤트가 존재하지 않습니다. nEventId = " + nEventId);
				continue;
			}
			int nMissionNo = Convert.ToInt32(dr["missionNo"]);
			ChargeEventMission mission = evt.GetMission(nMissionNo);
			if (mission == null)
			{
				SFLogUtil.Warn(GetType(), "[충전이벤트미션보상 목록] 미션이 존재하지 않습니다. nEventId = " + nEventId + ", nMissionNo = " + nMissionNo);
			}
			else
			{
				ChargeEventMissionReward reward = new ChargeEventMissionReward(mission);
				reward.Set(dr);
				mission.AddReward(reward);
			}
		}
	}

	private void LoadUserDBResources_DailyChargeEvent(SqlConnection conn)
	{
		DataRow drEvent = UserDac.DailyChargeEvent(conn, null);
		if (drEvent == null)
		{
			SFLogUtil.Warn(GetType(), "[매일충전이벤트] 이벤트가 존재하지 않습니다.");
			return;
		}
		DailyChargeEvent evt = new DailyChargeEvent();
		evt.Set(drEvent);
		m_dailyChargeEvent = evt;
		foreach (DataRow dr2 in UserDac.DailyChargeEventMissions(conn, null))
		{
			DailyChargeEventMission mission2 = new DailyChargeEventMission(m_dailyChargeEvent);
			mission2.Set(dr2);
			m_dailyChargeEvent.AddMission(mission2);
		}
		foreach (DataRow dr in UserDac.DailyChargeEventMissionRewards(conn, null))
		{
			int nMissionNo = Convert.ToInt32(dr["missionNo"]);
			DailyChargeEventMission mission = m_dailyChargeEvent.GetMission(nMissionNo);
			if (mission == null)
			{
				SFLogUtil.Warn(GetType(), "[매일충전이벤트미션보상 목록] 미션이 존재하지 않습니다. nMissionNo = " + nMissionNo);
				continue;
			}
			DailyChargeEventMissionReward reward = new DailyChargeEventMissionReward(mission);
			reward.Set(dr);
			mission.AddReward(reward);
		}
	}

	private void LoadUserDBResources_ConsumeEvent(SqlConnection conn, DateTime currentTime)
	{
		foreach (DataRow dr2 in UserDac.ConsumeEvents(conn, null, currentTime))
		{
			ConsumeEvent evt3 = new ConsumeEvent();
			evt3.Set(dr2);
			AddConsumeEvent(evt3);
		}
		foreach (DataRow dr3 in UserDac.ConsumeEventMissions(conn, null, currentTime))
		{
			int nEventId2 = Convert.ToInt32(dr3["eventId"]);
			ConsumeEvent evt2 = GetConsumeEvent(nEventId2);
			if (evt2 == null)
			{
				SFLogUtil.Warn(GetType(), "[소비이벤트미션 목록] 이벤트가 존재하지 않습니다. nEventId = " + nEventId2);
				continue;
			}
			ConsumeEventMission mission2 = new ConsumeEventMission(evt2);
			mission2.Set(dr3);
			evt2.AddMission(mission2);
		}
		foreach (DataRow dr in UserDac.ConsumeEventMissionRewards(conn, null, currentTime))
		{
			int nEventId = Convert.ToInt32(dr["eventId"]);
			ConsumeEvent evt = GetConsumeEvent(nEventId);
			if (evt == null)
			{
				SFLogUtil.Warn(GetType(), "[소비이벤트미션보상 목록] 이벤트가 존재하지 않습니다. nEventId = " + nEventId);
				continue;
			}
			int nMissionNo = Convert.ToInt32(dr["missionNo"]);
			ConsumeEventMission mission = evt.GetMission(nMissionNo);
			if (mission == null)
			{
				SFLogUtil.Warn(GetType(), "[소비이벤트미션보상 목록] 미션이 존재하지 않습니다. nEventId = " + nEventId + ", nMissionNo = " + nMissionNo);
			}
			else
			{
				ConsumeEventMissionReward reward = new ConsumeEventMissionReward(mission);
				reward.Set(dr);
				mission.AddReward(reward);
			}
		}
	}

	private void LoadUserDBResources_DailyConsumeEvent(SqlConnection conn)
	{
		DataRow drEvent = UserDac.DailyConsumeEvent(conn, null);
		if (drEvent == null)
		{
			SFLogUtil.Warn(GetType(), "[매일소비이벤트] 이벤트가 존재하지 않습니다.");
			return;
		}
		DailyConsumeEvent evt = new DailyConsumeEvent();
		evt.Set(drEvent);
		m_dailyConsumeEvent = evt;
		foreach (DataRow dr2 in UserDac.DailyConsumeEventMissions(conn, null))
		{
			DailyConsumeEventMission mission2 = new DailyConsumeEventMission(m_dailyConsumeEvent);
			mission2.Set(dr2);
			m_dailyConsumeEvent.AddMission(mission2);
		}
		foreach (DataRow dr in UserDac.DailyConsumeEventMissionRewards(conn, null))
		{
			int nMissionNo = Convert.ToInt32(dr["missionNo"]);
			DailyConsumeEventMission mission = m_dailyConsumeEvent.GetMission(nMissionNo);
			if (mission == null)
			{
				SFLogUtil.Warn(GetType(), "[매일소비이벤트미션보상 목록] 미션이 존재하지 않습니다. nMissionNo = " + nMissionNo);
				continue;
			}
			DailyConsumeEventMissionReward reward = new DailyConsumeEventMissionReward(mission);
			reward.Set(dr);
			mission.AddReward(reward);
		}
	}

	private void LoadUserDBResources_PotionAttr(SqlConnection conn)
	{
		foreach (DataRow dr in UserDac.PotionAttrs(conn, null))
		{
			PotionAttr attr = new PotionAttr();
			attr.Set(dr);
			AddPotionAttr(attr);
		}
	}

	private void LoadUserDBResources_JobChangeQuest(SqlConnection conn)
	{
		foreach (DataRow dr2 in UserDac.JobChangeQuests(conn, null))
		{
			JobChangeQuest quest2 = new JobChangeQuest();
			quest2.Set(dr2);
			AddJobChangeQuest(quest2);
		}
		foreach (DataRow dr in UserDac.JobChangeQuestDifficulties(conn, null))
		{
			int nQuestNo = Convert.ToInt32(dr["questNo"]);
			JobChangeQuest quest = GetJobChangeQuest(nQuestNo);
			if (quest == null)
			{
				SFLogUtil.Warn(GetType(), "[전직퀘스트난이도 목록] 퀘스트가 존재하지 않습니다. nQuestNo = " + nQuestNo);
				continue;
			}
			JobChangeQuestDifficulty difficulty = new JobChangeQuestDifficulty(quest);
			difficulty.Set(dr);
			quest.AddDifficulty(difficulty);
		}
	}

	private void LoadUserDBResources_Constellation(SqlConnection conn)
	{
		foreach (DataRow dr3 in UserDac.Constellations(conn, null))
		{
			Constellation constellation4 = new Constellation();
			constellation4.Set(dr3);
			AddConstellation(constellation4);
		}
		foreach (DataRow dr6 in UserDac.ConstellationSteps(conn, null))
		{
			int nConstellationId5 = Convert.ToInt32(dr6["constellationId"]);
			Constellation constellation6 = GetConstellation(nConstellationId5);
			if (constellation6 == null)
			{
				SFLogUtil.Warn(GetType(), "[별자리단계 목록] 별자리가 존재하지 않습니다. nConstellationId = " + nConstellationId5);
				continue;
			}
			ConstellationStep step5 = new ConstellationStep(constellation6);
			step5.Set(dr6);
			constellation6.AddStep(step5);
		}
		foreach (DataRow dr5 in UserDac.ConstellationCycles(conn, null))
		{
			int nConstellationId4 = Convert.ToInt32(dr5["constellationId"]);
			Constellation constellation5 = GetConstellation(nConstellationId4);
			if (constellation5 == null)
			{
				SFLogUtil.Warn(GetType(), "[별자리단계사이클 목록] 별자리가 존재하지 않습니다. nConstellationId = " + nConstellationId4);
				continue;
			}
			int nStep4 = Convert.ToInt32(dr5["step"]);
			ConstellationStep step4 = constellation5.GetStep(nStep4);
			if (step4 == null)
			{
				SFLogUtil.Warn(GetType(), "[별자리단계사이클 목록] 단계가 존재하지 않습니다. nConstellationId = " + nConstellationId4 + ", nStep = " + nStep4);
			}
			else
			{
				ConstellationCycle cycle4 = new ConstellationCycle(step4);
				cycle4.Set(dr5);
				step4.AddCycle(cycle4);
			}
		}
		foreach (DataRow dr4 in UserDac.ConstellationCycleBuffs(conn, null))
		{
			int nConstellationId3 = Convert.ToInt32(dr4["constellationId"]);
			Constellation constellation3 = GetConstellation(nConstellationId3);
			if (constellation3 == null)
			{
				SFLogUtil.Warn(GetType(), "[별자리단계사이클버프 목록] 별자리가 존재하지 않습니다. nConstellationId = " + nConstellationId3);
				continue;
			}
			int nStep3 = Convert.ToInt32(dr4["step"]);
			ConstellationStep step3 = constellation3.GetStep(nStep3);
			if (step3 == null)
			{
				SFLogUtil.Warn(GetType(), "[별자리단계사이클버프 목록] 단계가 존재하지 않습니다. nConstellationId = " + nConstellationId3 + ", nStep = " + nStep3);
				continue;
			}
			int nCycle3 = Convert.ToInt32(dr4["cycle"]);
			ConstellationCycle cycle3 = step3.GetCycle(nCycle3);
			if (cycle3 == null)
			{
				SFLogUtil.Warn(GetType(), "[별자리단계사이클버프 목록] 사이클이 존재하지 않습니다. nConstellationId = " + nConstellationId3 + ", nStep = " + nStep3 + ", nCycle = " + nCycle3);
			}
			else
			{
				ConstellationCycleBuff buff2 = new ConstellationCycleBuff(cycle3);
				buff2.Set(dr4);
				cycle3.AddBuff(buff2);
			}
		}
		foreach (DataRow dr2 in UserDac.ConstellationEntries(conn, null))
		{
			int nConstellationId2 = Convert.ToInt32(dr2["constellationId"]);
			Constellation constellation2 = GetConstellation(nConstellationId2);
			if (constellation2 == null)
			{
				SFLogUtil.Warn(GetType(), "[별자리단계사이클항목 목록] 별자리가 존재하지 않습니다. nConstellationId = " + nConstellationId2);
				continue;
			}
			int nStep2 = Convert.ToInt32(dr2["step"]);
			ConstellationStep step2 = constellation2.GetStep(nStep2);
			if (step2 == null)
			{
				SFLogUtil.Warn(GetType(), "[별자리단계사이클항목 목록] 단계가 존재하지 않습니다. nConstellationId = " + nConstellationId2 + ", nStep = " + nStep2);
				continue;
			}
			int nCycle2 = Convert.ToInt32(dr2["cycle"]);
			ConstellationCycle cycle2 = step2.GetCycle(nCycle2);
			if (cycle2 == null)
			{
				SFLogUtil.Warn(GetType(), "[별자리단계사이클항목 목록] 사이클이 존재하지 않습니다. nConstellationId = " + nConstellationId2 + ", nStep = " + nStep2 + ", nCycle = " + nCycle2);
			}
			else
			{
				ConstellationEntry entry2 = new ConstellationEntry(cycle2);
				entry2.Set(dr2);
				cycle2.AddEntry(entry2);
			}
		}
		foreach (DataRow dr in UserDac.ConstellationEntryBuffs(conn, null))
		{
			int nConstellationId = Convert.ToInt32(dr["constellationId"]);
			Constellation constellation = GetConstellation(nConstellationId);
			if (constellation == null)
			{
				SFLogUtil.Warn(GetType(), "[별자리단계사이클항목버프 목록] 별자리가 존재하지 않습니다. nConstellationId = " + nConstellationId);
				continue;
			}
			int nStep = Convert.ToInt32(dr["step"]);
			ConstellationStep step = constellation.GetStep(nStep);
			if (step == null)
			{
				SFLogUtil.Warn(GetType(), "[별자리단계사이클항목버프 목록] 단계가 존재하지 않습니다. nConstellationId = " + nConstellationId + ", nStep = " + nStep);
				continue;
			}
			int nCycle = Convert.ToInt32(dr["cycle"]);
			ConstellationCycle cycle = step.GetCycle(nCycle);
			if (cycle == null)
			{
				SFLogUtil.Warn(GetType(), "[별자리단계사이클항목버프 목록] 사이클이 존재하지 않습니다. nConstellationId = " + nConstellationId + ", nStep = " + nStep + ", nCycle = " + nCycle);
				continue;
			}
			int nEntryNo = Convert.ToInt32(dr["entryNo"]);
			ConstellationEntry entry = cycle.GetEntry(nEntryNo);
			if (entry == null)
			{
				SFLogUtil.Warn(GetType(), "[별자리단계사이클항목버프 목록] 항목이 존재하지 않습니다. nConstellationId = " + nConstellationId + ", nStep = " + nStep + ", nCycle = " + nCycle + ", nEntryNo = " + nEntryNo);
			}
			else
			{
				ConstellationEntryBuff buff = new ConstellationEntryBuff(entry);
				buff.Set(dr);
				entry.AddBuff(buff);
			}
		}
	}

	private void LoadUserDBResources_Artifact(SqlConnection conn, DataRow drGameConfig)
	{
		int nArtifactRequiredConditionType = Convert.ToInt32(drGameConfig["artifactRequiredConditionType"]);
		if (!Enum.IsDefined(typeof(ArtifactRequiredConditionType), nArtifactRequiredConditionType))
		{
			SFLogUtil.Warn(GetType(), "[아티팩트 게임설정] 요구조건타입이 유효하지 않습니다. nArtifactRequiredConditionType = " + nArtifactRequiredConditionType);
		}
		m_nArtifactRequiredConditionValue = Convert.ToInt32(drGameConfig["artifactRequiredConditionValue"]);
		m_nArtifactMaxLevel = Convert.ToInt32(drGameConfig["artifactMaxLevel"]);
		if (m_nArtifactMaxLevel <= 0)
		{
			SFLogUtil.Warn(GetType(), "[아티팩트 게임설정] 최대레벨이 유효하지 않습니다. m_nArtifactMaxLevel = " + m_nArtifactMaxLevel);
		}
		foreach (DataRow dr3 in UserDac.Artifacts(conn, null))
		{
			Artifact artifact3 = new Artifact();
			artifact3.Set(dr3);
			AddArtifact(artifact3);
		}
		foreach (DataRow dr5 in UserDac.ArtifactAttrs(conn, null))
		{
			int nArtifactNo3 = Convert.ToInt32(dr5["artifactNo"]);
			Artifact artifact4 = GetArtifact(nArtifactNo3);
			if (artifact4 == null)
			{
				SFLogUtil.Warn(GetType(), "[아티팩트속성 목록] 아티팩트가 존재하지 않습니다. nArtifactNo = " + nArtifactNo3);
			}
			else
			{
				artifact4.AddAttr(Convert.ToInt32(dr5["attrId"]));
			}
		}
		foreach (DataRow dr4 in UserDac.ArtifactLevels(conn, null))
		{
			int nArtifactNo2 = Convert.ToInt32(dr4["artifactNo"]);
			Artifact artifact2 = GetArtifact(nArtifactNo2);
			if (artifact2 == null)
			{
				SFLogUtil.Warn(GetType(), "[아티팩트레벨 목록] 아티팩트가 존재하지 않습니다. nArtifactNo = " + nArtifactNo2);
				continue;
			}
			ArtifactLevel level2 = new ArtifactLevel(artifact2);
			level2.Set(dr4);
			artifact2.AddLevel(level2);
		}
		foreach (DataRow dr2 in UserDac.ArtifactLevelAttrs(conn, null))
		{
			int nArtifactNo = Convert.ToInt32(dr2["artifactNo"]);
			Artifact artifact = GetArtifact(nArtifactNo);
			if (artifact == null)
			{
				SFLogUtil.Warn(GetType(), "[아티팩트레벨속성 목록] 아티팩트가 존재하지 않습니다. nArtifactNo = " + nArtifactNo);
				continue;
			}
			int nLevel = Convert.ToInt32(dr2["level"]);
			ArtifactLevel level = artifact.GetLevel(nLevel);
			if (level == null)
			{
				SFLogUtil.Warn(GetType(), "[아티팩트레벨속성 목록] 아티팩트레벨이 존재하지 않습니다. nArtifactNo = " + nArtifactNo + ", nLevel = " + nLevel);
			}
			else
			{
				ArtifactLevelAttr attr = new ArtifactLevelAttr(level);
				attr.Set(dr2);
				level.AddAttr(attr);
			}
		}
		foreach (DataRow dr in UserDac.ArtifactLevelUpMaterials(conn, null))
		{
			ArtifactLevelUpMaterial material = new ArtifactLevelUpMaterial();
			material.Set(dr);
			AddArtifactLevelUpMaterial(material);
		}
	}

	private void LoadUserDBResources_SystemMessage(SqlConnection conn)
	{
		foreach (DataRow dr in UserDac.SystemMessages(conn, null))
		{
			SystemMessage message = new SystemMessage();
			message.Set(dr);
			AddSystemMessage(message);
		}
	}

	private void LoadUserDBResources_TimeDesignationEvent(SqlConnection conn, DateTime currentTime)
	{
		foreach (DataRow dr2 in UserDac.TimeDesignationEvents(conn, null, currentTime))
		{
			TimeDesignationEvent designationEvent2 = new TimeDesignationEvent();
			designationEvent2.Set(dr2);
			AddTimeDesignationEvent(designationEvent2);
		}
		foreach (DataRow dr in UserDac.TimeDesignationEventRewards(conn, null, currentTime))
		{
			int nEventId = Convert.ToInt32(dr["eventId"]);
			TimeDesignationEvent designationEvent = GetTimeDesignationEvent(nEventId);
			if (designationEvent == null)
			{
				SFLogUtil.Warn(GetType(), "[시간지정이벤트보상 목록] 이벤트가 존재하지 않습니다. nEventId = " + nEventId);
				continue;
			}
			TimeDesignationEventReward reward = new TimeDesignationEventReward(designationEvent);
			reward.Set(dr);
			designationEvent.AddReward(reward);
		}
	}

	private void InitLate()
	{
		foreach (VipLevel vipLevel in m_vipLevels)
		{
			vipLevel.SetLate();
		}
	}

	public Language GetLanguage(int nId)
	{
		if (!m_languages.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool IsValidHeroName(string sName)
	{
		if (sName == null)
		{
			return false;
		}
		return m_heroNameRegex.IsMatch(sName);
	}

	public bool IsValidGuildName(string sName)
	{
		if (sName == null)
		{
			return false;
		}
		return m_guildNameRegex.IsMatch(sName);
	}

	public bool IsNameBanWord(string sWord)
	{
		return m_nameBanWordTextFilter.ContainsElement(sWord);
	}

	public Attr GetAttr(int nAttrId)
	{
		if (!m_attrs.TryGetValue(nAttrId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddAttrValue(AttrValue attrValue)
	{
		if (attrValue == null)
		{
			throw new ArgumentNullException("attrValue");
		}
		m_attrValues.Add(attrValue.id, attrValue);
	}

	public AttrValue GetAttrValue(long lnId)
	{
		if (!m_attrValues.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	public AbnormalState GetAbnormalState(int nId)
	{
		if (!m_abnormalStates.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public Nation GetNation(int nId)
	{
		if (!m_nations.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public Continent GetContinent(int nContinentId)
	{
		if (!m_continents.TryGetValue(nContinentId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddContinentObject(ContinentObject obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		m_continentObjects.Add(obj.id, obj);
	}

	public ContinentObject GetContinentObject(int nId)
	{
		if (!m_continentObjects.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public Job GetJob(int nId)
	{
		if (!m_jobs.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddJobSkillMaster(JobSkillMaster master)
	{
		if (master == null)
		{
			throw new ArgumentNullException("master");
		}
		m_jobSkillMasters.Add(master.skillId, master);
	}

	public JobSkillMaster GetJobSkillMaster(int nSkillId)
	{
		if (!m_jobSkillMasters.TryGetValue(nSkillId, out var value))
		{
			return null;
		}
		return value;
	}

	public Rank GetRank(int nRankNo)
	{
		int nIndex = nRankNo - 1;
		if (nIndex < 0 || nIndex >= m_ranks.Count)
		{
			return null;
		}
		return m_ranks[nIndex];
	}

	private void AddRankActiveSkill(RankActiveSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_rankActiveSkills.Add(skill.id, skill);
	}

	public RankActiveSkill GetRankActiveSkill(int nSkillId)
	{
		if (!m_rankActiveSkills.TryGetValue(nSkillId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddRankPassiveSkill(RankPassiveSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_rankPassiveSkills.Add(skill.id, skill);
	}

	public RankPassiveSkill GetRankPassiveSkill(int nSkillId)
	{
		if (!m_rankPassiveSkills.TryGetValue(nSkillId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddLocation(Location location)
	{
		if (location == null)
		{
			throw new ArgumentException("location");
		}
		if (!m_locations.ContainsKey(location.locationId))
			m_locations.Add(location.locationId, location);
		else
			SFLogUtil.Error(GetType(), "[AddLocation]: An item with the same key has been added.");

    }

	public Location GetLocation(int nLocationId)
	{
		if (!m_locations.TryGetValue(nLocationId, out var value))
		{
			return null;
		}
		return value;
	}

	private ItemGrade GetItemGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_itemGrades.Length)
		{
			return null;
		}
		return m_itemGrades[nIndex];
	}

	private void AddItemType(ItemType type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("itemType");
		}
		m_itemTypes.Add(type.id, type);
	}

	public ItemType GetItemType(int nId)
	{
		if (!m_itemTypes.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool ContainsItemType(int nItemType)
	{
		return m_itemTypes.ContainsKey(nItemType);
	}

	private void AddItem(Item item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		m_items.Add(item.id, item);
	}

	public Item GetItem(int nId)
	{
		if (!m_items.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddExpReward(ExpReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_expRewards.Add(reward.id, reward);
	}

	public ExpReward GetExpReward(long lnId)
	{
		if (!m_expRewards.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddGoldReward(GoldReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_goldRewards.Add(reward.id, reward);
	}

	public GoldReward GetGoldReward(long lnId)
	{
		if (!m_goldRewards.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddItemReward(ItemReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_itemRewards.Add(reward.id, reward);
	}

	public ItemReward GetItemReward(long lnId)
	{
		if (!m_itemRewards.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddExploitPointReward(ExploitPointReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_exploitPointRewards.Add(reward.id, reward);
	}

	public ExploitPointReward GetExploitPointReward(long lnId)
	{
		if (!m_exploitPointRewards.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddHonorPointReward(HonorPointReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_honorPointRewards.Add(reward.honorPointRewardId, reward);
	}

	public HonorPointReward GetHonorPointReward(long lnId)
	{
		if (!m_honorPointRewards.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddNationFundReward(NationFundReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_nationFundRewards.Add(reward.id, reward);
	}

	public NationFundReward GetNationFundReward(long lnId)
	{
		if (!m_nationFundRewards.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddOwnDiaReward(OwnDiaReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_ownDiaRewards.Add(reward.id, reward);
	}

	public OwnDiaReward GetOwnDiaReward(long lnId)
	{
		if (!m_ownDiaRewards.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddGuildContributionPointReward(GuildContributionPointReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_guildContributionPointRewards.Add(reward.id, reward);
	}

	public GuildContributionPointReward GetGuildContributionPointReward(long lnId)
	{
		if (!m_guildContributionPointRewards.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddGuildFundReward(GuildFundReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_guildFundRewards.Add(reward.id, reward);
	}

	public GuildFundReward GetGuildFundReward(long lnId)
	{
		if (!m_guildFundRewards.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddGuildBuildingPointReward(GuildBuildingPointReward reawrd)
	{
		if (reawrd == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_guildBuildingPointRewards.Add(reawrd.id, reawrd);
	}

	public GuildBuildingPointReward GetGuildBuildingPointReward(long lnId)
	{
		if (!m_guildBuildingPointRewards.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddGuildPointReward(GuildPointReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reawrd");
		}
		m_guildPointRewards.Add(reward.id, reward);
	}

	public GuildPointReward GetGuildPointReward(long lnId)
	{
		if (!m_guildPointRewards.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddItemCompositionRecipe(ItemCompositionRecipe recipe)
	{
		if (recipe == null)
		{
			throw new ArgumentNullException("recipe");
		}
		m_itemCompositionRecipes.Add(recipe.materialItem.id, recipe);
	}

	public ItemCompositionRecipe GetItemCompositionRecipe(int nMaterialItemId)
	{
		if (!m_itemCompositionRecipes.TryGetValue(nMaterialItemId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddMainGearCategory(MainGearCategory category)
	{
		if (category == null)
		{
			throw new ArgumentNullException("mainGearCategory");
		}
		m_mainGearCategory[category.id - 1] = category;
	}

	public MainGearCategory GetMainGearCategory(int nCategory)
	{
		int nIndex = nCategory - 1;
		if (nIndex < 0 || nIndex >= m_mainGearCategory.Length)
		{
			return null;
		}
		return m_mainGearCategory[nIndex];
	}

	private void AddMainGearType(MainGearType gearType)
	{
		if (gearType == null)
		{
			throw new ArgumentNullException("gearType");
		}
		m_mainGearTypes[gearType.id - 1] = gearType;
	}

	public MainGearType GetMainGearType(int nType)
	{
		int nIndex = nType - 1;
		if (nIndex < 0 || nIndex >= m_mainGearTypes.Length)
		{
			return null;
		}
		return m_mainGearTypes[nIndex];
	}

	private void AddMainGearTier(MainGearTier tier)
	{
		if (tier == null)
		{
			throw new ArgumentNullException("tier");
		}
		m_mainGearTiers.Add(tier);
	}

	public MainGearTier GetMainGearTier(int nTier)
	{
		int nIndex = nTier - 1;
		if (nIndex < 0 || nIndex >= m_mainGearTiers.Count)
		{
			return null;
		}
		return m_mainGearTiers[nIndex];
	}

	public MainGearTier GetAvailableMaxMainGearTier(int nHeroLevel)
	{
		MainGearTier mainGearTier = null;
		foreach (MainGearTier tier in m_mainGearTiers)
		{
			if (nHeroLevel >= tier.requiredHeroLevel)
			{
				mainGearTier = tier;
				continue;
			}
			return mainGearTier;
		}
		return mainGearTier;
	}

	private void AddMainGearGrade(MainGearGrade grade)
	{
		if (grade == null)
		{
			throw new ArgumentNullException("grade");
		}
		m_mainGearGrade[grade.id - 1] = grade;
	}

	public MainGearGrade GetMainGearGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_mainGearGrade.Length)
		{
			return null;
		}
		return m_mainGearGrade[nIndex];
	}

	private void AddMainGearQuality(MainGearQuality quality)
	{
		if (quality == null)
		{
			throw new ArgumentNullException("quality");
		}
		m_mainGearQuality[quality.id - 1] = quality;
	}

	public MainGearQuality GetMainGearQuality(int nQuality)
	{
		int nIndex = nQuality - 1;
		if (nIndex < 0 || nIndex >= m_mainGearQuality.Length)
		{
			return null;
		}
		return m_mainGearQuality[nIndex];
	}

	private void AddMainGear(MainGear gear)
	{
		if (gear == null)
		{
			throw new ArgumentNullException("mainGear");
		}
		m_mainGears.Add(gear.id, gear);
	}

	public MainGear GetMainGear(int nId)
	{
		if (!m_mainGears.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddMainGearSet(MainGearSet mainGearSet)
	{
		if (mainGearSet == null)
		{
			throw new ArgumentNullException("mainGearSet");
		}
		m_mainGearSets[mainGearSet.tier - 1, mainGearSet.grade - 1, mainGearSet.quality - 1] = mainGearSet;
	}

	public MainGearSet GetMainGearSet(int nTier, int nGrade, int nQuality)
	{
		int nTierIndex = nTier - 1;
		if (nTierIndex < 0 || nTierIndex > m_mainGearSets.GetUpperBound(0))
		{
			return null;
		}
		int nGradeIndex = nGrade - 1;
		if (nGradeIndex < 0 || nGradeIndex > m_mainGearSets.GetUpperBound(1))
		{
			return null;
		}
		int nQaulityIndex = nQuality - 1;
		if (nQaulityIndex < 0 || nQaulityIndex > m_mainGearSets.GetUpperBound(2))
		{
			return null;
		}
		return m_mainGearSets[nTierIndex, nGradeIndex, nQaulityIndex];
	}

	private void AddMainGearEnchantStep(MainGearEnchantStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("mainGearEnchantStep");
		}
		m_mainGearEnchantStep.Add(step);
	}

	public MainGearEnchantStep GetMainGearEnchantStep(int nStep)
	{
		int nIndex = nStep - 1;
		if (nIndex < 0 || nIndex >= m_mainGearEnchantStep.Count)
		{
			return null;
		}
		return m_mainGearEnchantStep[nIndex];
	}

	private void AddMainGearEnchantLevel(MainGearEnchantLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("mainGearEnchantLevel");
		}
		m_mainGearEnchantLevels.Add(level);
	}

	public MainGearEnchantLevel GetMainGearEnchantLevel(int nEnchantLevel)
	{
		if (nEnchantLevel < 0 || nEnchantLevel >= m_mainGearEnchantLevels.Count)
		{
			return null;
		}
		return m_mainGearEnchantLevels[nEnchantLevel];
	}

	private void AddMainGearEnchantLevelSet(MainGearEnchantLevelSet levelSet)
	{
		if (levelSet == null)
		{
			throw new ArgumentNullException("levelSet");
		}
		m_mainGearEnchantLevelSets.Add(levelSet);
	}

	public MainGearEnchantLevelSet GetMainGearEnchantLevelSet(int nSetNo)
	{
		int nIndex = nSetNo - 1;
		if (nIndex < 0 || nIndex >= m_mainGearEnchantLevelSets.Count)
		{
			return null;
		}
		return m_mainGearEnchantLevelSets[nIndex];
	}

	private void AddMainGearRefinementRecipe(MainGearRefinementRecipe recipe)
	{
		if (recipe == null)
		{
			throw new ArgumentNullException("recipe");
		}
		m_mainGearRefinementRecipes.Add(recipe.protectionCount, recipe);
	}

	public MainGearRefinementRecipe GetMainGearRefinementRecipe(int nProtectionCount)
	{
		if (!m_mainGearRefinementRecipes.TryGetValue(nProtectionCount, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddSubGear(SubGear gear)
	{
		if (gear == null)
		{
			throw new ArgumentNullException("subGear");
		}
		m_subGears.Add(gear.id, gear);
	}

	public SubGear GetSubGear(int nId)
	{
		if (!m_subGears.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddSubGearGrade(SubGearGrade grade)
	{
		if (grade == null)
		{
			throw new ArgumentNullException("grade");
		}
		m_subGearGrade[grade.id - 1] = grade;
	}

	public SubGearGrade GetSubGearGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_subGearGrade.Length)
		{
			return null;
		}
		return m_subGearGrade[nIndex];
	}

	private void AddSubGearSoulstoneLevelSet(SubGearSoulstoneLevelSet soulstoneLevelSet)
	{
		if (soulstoneLevelSet == null)
		{
			throw new ArgumentNullException("soulstoneLevelSet");
		}
		m_subGearSoulstoneLevelSets.Add(soulstoneLevelSet);
	}

	public SubGearSoulstoneLevelSet GetSubGearSoulstoneLevelSet(int nSetNo)
	{
		int nIndex = nSetNo - 1;
		if (nIndex < 0 || nIndex >= m_subGearSoulstoneLevelSets.Count)
		{
			return null;
		}
		return m_subGearSoulstoneLevelSets[nIndex];
	}

	public MonsterCharacter GetMonsterCharacter(int nId)
	{
		if (!m_monsterCharacters.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public Monster GetMonster(int nId)
	{
		if (!m_monsters.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public MonsterArrange GetMonsterArrange(long lnId)
	{
		if (!m_monsterArrange.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	public MonsterSkill GetMonsterSkill(int nId)
	{
		if (!m_monsterSkills.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public Portal GetPortal(int nId)
	{
		if (!m_portals.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public Npc GetNpc(int nId)
	{
		if (!m_npcs.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddNpcShop(NpcShop shop)
	{
		if (shop == null)
		{
			throw new ArgumentNullException("shop");
		}
		m_npcShops.Add(shop.id, shop);
	}

	public NpcShop GetNpcShop(int nId)
	{
		if (!m_npcShops.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddNpcShopProduct(NpcShopProduct product)
	{
		if (product == null)
		{
			throw new ArgumentNullException("product");
		}
		m_npcShopProducts.Add(product.id, product);
	}

	public NpcShopProduct GetNpcShopProduct(int nId)
	{
		if (!m_npcShopProducts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddMainQuest(MainQuest mainQuest)
	{
		if (mainQuest == null)
		{
			throw new ArgumentNullException("mainQuest");
		}
		m_mainQuests.Add(mainQuest);
	}

	public MainQuest GetMainQuest(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_mainQuests.Count)
		{
			return null;
		}
		return m_mainQuests[nIndex];
	}

	private void AddBountyHunterQuest(BountyHunterQuest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_bountyHunterQuests.Add(quest.id, quest);
	}

	public BountyHunterQuest GetBountyQuest(int nId)
	{
		if (!m_bountyHunterQuests.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public BountyHunterQuestRewardCollection GetBountyHunterQuestRewardCollection(int nItemGrade)
	{
		int nIndex = nItemGrade - 1;
		if (nIndex < 0 || nIndex >= m_bountyHunterQuests.Count)
		{
			return null;
		}
		return m_bountyHunterQuestRewardCollection[nIndex];
	}

	private void AddJobLevelMaster(JobLevelMaster master)
	{
		if (master == null)
		{
			throw new ArgumentNullException("jobLevelMaster");
		}
		m_jobLevelMasters.Add(master);
	}

	public JobLevelMaster GetJobLevelMaster(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_jobLevelMasters.Count)
		{
			return null;
		}
		return m_jobLevelMasters[nIndex];
	}

	private void AddSimpleShop(SimpleShopProduct product)
	{
		if (product == null)
		{
			throw new ArgumentNullException("simpleShopProduct");
		}
		m_simpleShopProducts.Add(product.id, product);
	}

	public SimpleShopProduct GetSimpleShop(int nProductId)
	{
		if (!m_simpleShopProducts.TryGetValue(nProductId, out var value))
		{
			return null;
		}
		return value;
	}

	public Vector3 SelectStartContinentSaftyRevivalPosition()
	{
		return Util.SelectPoint(m_startContinentSaftyRevivalPosition, m_fStartContinentSaftyRevivalRadius);
	}

	public float SelectStartContinentSaftyRevivalYRotation()
	{
		if (m_nStartContinentSaftyRevivalYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fStartContinentSaftyRevivalYRotation);
		}
		return m_fStartContinentSaftyRevivalYRotation;
	}

	public Vector3 SelectSaftyRevivalPosition()
	{
		return Util.SelectPoint(m_saftyRevivalPosition, m_fSaftyRevivalRadius);
	}

	public float SelectSaftyRevivalYRotation()
	{
		if (m_nSaftyRevivalYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fSaftyRevivalYRotation);
		}
		return m_fSaftyRevivalYRotation;
	}

	public PaidImmediateRevival GetPaidImmediateRevival(int nCount)
	{
		int nRevivalCount = Math.Min(nCount, m_paidImmediateRevivals.Count());
		int nIndex = nRevivalCount - 1;
		if (nIndex < 0)
		{
			return null;
		}
		return m_paidImmediateRevivals[nIndex];
	}

	private void AddInventorySlotExtendRecipe(InventorySlotExtendRecipe recipe)
	{
		if (recipe == null)
		{
			throw new ArgumentNullException("recipe");
		}
		m_inventorySlotExtendRecipes.Add(recipe);
	}

	public InventorySlotExtendRecipe GetInventorySlotExtendRecipe(int nSlotCount)
	{
		int nIndex = nSlotCount - 1;
		if (nIndex < 0 || nIndex >= m_inventorySlotExtendRecipes.Count)
		{
			return null;
		}
		return m_inventorySlotExtendRecipes[nIndex];
	}

	private void AddRestRewardTime(RestRewardTime cost)
	{
		if (cost == null)
		{
			throw new ArgumentNullException("cost");
		}
		m_restRewardTimes.Add(cost);
	}

	public RestRewardTime GetRestRewardTime(int nRestTime)
	{
		int nIndex = nRestTime - startRestRewardTime.restTime;
		if (nIndex < 0 || nIndex >= m_restRewardTimes.Count)
		{
			return null;
		}
		return m_restRewardTimes[nIndex];
	}

	public RestRewardTime GetTargetRestRewardTime(int nRestTime)
	{
		if (nRestTime < startRestRewardTime.restTime)
		{
			return null;
		}
		int nLastRestTime = lastRestRewardTime.restTime;
		if (nRestTime > nLastRestTime)
		{
			nRestTime = nLastRestTime;
		}
		return GetRestRewardTime(nRestTime);
	}

	public PickPool GetPickPool(int nId)
	{
		if (!m_pickPools.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddLevelUpRewardEntry(LevelUpRewardEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_levelUpRewardEntries.Add(entry.id, entry);
	}

	public LevelUpRewardEntry GetLevelUpRewardEntry(int nId)
	{
		if (!m_levelUpRewardEntries.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddDailyAttendRewardEntry(DailyAttendRewardEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_dailyAttendRewardEntries.Add(entry);
	}

	public DailyAttendRewardEntry GetDailyAttendRewardEntry(int nDay)
	{
		int nIndex = nDay - 1;
		if (nIndex < 0 || nIndex >= m_dailyAttendRewardEntries.Count)
		{
			return null;
		}
		return m_dailyAttendRewardEntries[nIndex];
	}

	public bool IsWeekendAttendRewardDayOfWeek(DayOfWeek dayOfWeek)
	{
		foreach (DayOfWeek weekend in m_weekendAttendRewardAvailableDaysOfWeek)
		{
			if (dayOfWeek == weekend)
			{
				return true;
			}
		}
		return false;
	}

	private void AddAccessRewardEntry(AccessRewardEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_accessRewardEntries.Add(entry.id, entry);
	}

	public AccessRewardEntry GetAccessRewardEntry(int nId)
	{
		if (!m_accessRewardEntries.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddSeriesMission(SeriesMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_seriesMissions.Add(mission.id, mission);
	}

	public SeriesMission GetSeriesMission(int nId)
	{
		if (!m_seriesMissions.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddTodayMission(TodayMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_todayMissions.Add(mission.id, mission);
	}

	public TodayMission GetTodayMission(int nId)
	{
		if (!m_todayMissions.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public TodayMissionPool GetTodayMissionPool(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_todayMissionPools.Length)
		{
			return null;
		}
		return m_todayMissionPools[nIndex];
	}

	private void AddAttainmentEntry(AttainmentEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_attainmentEntries.Add(entry);
	}

	public AttainmentEntry GetAttainmentEntry(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_attainmentEntries.Count)
		{
			return null;
		}
		return m_attainmentEntries[nIndex];
	}

	private void AddVipLevel(VipLevel vipLevel)
	{
		if (vipLevel == null)
		{
			throw new ArgumentNullException("vipLevel");
		}
		m_vipLevels.Add(vipLevel);
	}

	public VipLevel GetVipLevel(int nLevel)
	{
		if (nLevel < 0 || nLevel >= m_vipLevels.Count)
		{
			return null;
		}
		return m_vipLevels[nLevel];
	}

	public VipLevel GetVipLevelByPoint(int nTotalPoint)
	{
		if (nTotalPoint < 0)
		{
			throw new ArgumentOutOfRangeException("nTotalPoint");
		}
		VipLevel vipLevel = null;
		foreach (VipLevel level in m_vipLevels)
		{
			if (nTotalPoint >= level.requiredAccVipPoint)
			{
				vipLevel = level;
				continue;
			}
			return vipLevel;
		}
		return vipLevel;
	}

	public Vector3 SelectStartPosition()
	{
		return Util.SelectPoint(m_startPosition, m_fStartRadius);
	}

	public float SelectYRotation()
	{
		if (m_nStartYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fStartYRotation);
		}
		return m_fStartYRotation;
	}

	public DropCountPool GetDropCountPool(int nPoolId)
	{
		if (!m_dropCountPools.TryGetValue(nPoolId, out var value))
		{
			return null;
		}
		return value;
	}

	private DropCountPool GetOrCreateDropCountPool(int nPoolId)
	{
		DropCountPool pool = GetDropCountPool(nPoolId);
		if (pool == null)
		{
			pool = new DropCountPool(nPoolId);
			m_dropCountPools.Add(pool.id, pool);
		}
		return pool;
	}

	public DropObjectPool GetDropObjectPool(int nPoolId)
	{
		if (!m_dropObjectPools.TryGetValue(nPoolId, out var value))
		{
			return null;
		}
		return value;
	}

	private DropObjectPool GetOrCreateDropObjectPool(int nPoolId)
	{
		DropObjectPool pool = GetDropObjectPool(nPoolId);
		if (pool == null)
		{
			pool = new DropObjectPool(nPoolId);
			m_dropObjectPools.Add(pool.id, pool);
		}
		return pool;
	}

	private void AddMainQuestDungeon(MainQuestDungeon dungeon)
	{
		if (dungeon == null)
		{
			throw new ArgumentNullException("dungeon");
		}
		m_mainQuestDungeons.Add(dungeon);
		AddLocation(dungeon);
	}

	public MainQuestDungeon GetMainQuestDungeon(int nDungeonId)
	{
		int nIndex = nDungeonId - 1;
		if (nIndex < 0 || nIndex >= m_mainQuestDungeons.Count)
		{
			return null;
		}
		return m_mainQuestDungeons[nIndex];
	}

	private void AddMount(Mount mount)
	{
		if (mount == null)
		{
			throw new ArgumentNullException("mount");
		}
		m_mounts.Add(mount.id, mount);
	}

	public Mount GetMount(int nId)
	{
		if (!m_mounts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddMountLevelMaster(MountLevelMaster master)
	{
		if (master == null)
		{
			throw new ArgumentNullException("master");
		}
		m_mountLevelMasters.Add(master);
	}

	public MountLevelMaster GetMountLevelMaster(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_mountLevelMasters.Count)
		{
			return null;
		}
		return m_mountLevelMasters[nIndex];
	}

	private void AddMountQualityMaster(MountQualityMaster master)
	{
		if (master == null)
		{
			throw new ArgumentNullException("master");
		}
		m_mountQualityMasters.Add(master);
	}

	public MountQualityMaster GetMountQualityMaster(int nQuality)
	{
		int nIndex = nQuality - 1;
		if (nIndex < 0 || nIndex >= m_mountQualityMasters.Count)
		{
			return null;
		}
		return m_mountQualityMasters[nIndex];
	}

	private void AddMountAwakeningLevelMaster(MountAwakeningLevelMaster master)
	{
		if (master == null)
		{
			throw new ArgumentNullException("master");
		}
		m_mountAwakeningLevelMaster.Add(master);
	}

	public MountAwakeningLevelMaster GetMountAwakeningLevelMaster(int nLevel)
	{
		if (nLevel < 0 || nLevel > m_mountAwakeningLevelMaster.Count)
		{
			return null;
		}
		return m_mountAwakeningLevelMaster[nLevel];
	}

	public void AddMountPotionAttrCount(MountPotionAttrCount count)
	{
		if (count == null)
		{
			throw new ArgumentNullException("count");
		}
		m_mountPotionAttrCount.Add(count);
	}

	public MountPotionAttrCount GetMountPotionAttrCount(int nCount)
	{
		int nIndex = nCount - 1;
		if (nIndex < 0 || nIndex >= m_mountPotionAttrCount.Count)
		{
			return null;
		}
		return m_mountPotionAttrCount[nIndex];
	}

	public MountGearSlot GetMountGearSlot(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_mountGearSlots.Length)
		{
			return null;
		}
		return m_mountGearSlots[nIndex];
	}

	public MountGearType GetMountGearType(int nType)
	{
		int nIndex = nType - 1;
		if (nIndex < 0 || nIndex >= m_mountGearTypes.Length)
		{
			return null;
		}
		return m_mountGearTypes[nIndex];
	}

	public MountGearGrade GetMountGearGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_mountGearTypes.Length)
		{
			return null;
		}
		return m_mountGearGrades[nIndex];
	}

	public MountGearQuality GetMountGearQuality(int nQuality)
	{
		int nIndex = nQuality - 1;
		if (nIndex < 0 || nIndex >= m_mountGearTypes.Length)
		{
			return null;
		}
		return m_mountGearQualities[nIndex];
	}

	private void AddMountGear(MountGear gear)
	{
		if (gear == null)
		{
			throw new ArgumentNullException("gear");
		}
		m_mountGears.Add(gear.id, gear);
	}

	public MountGear GetMountGear(int nId)
	{
		if (!m_mountGears.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddMountGearPickBoxRecipes(MountGearPickBoxRecipe recipe)
	{
		if (recipe == null)
		{
			throw new ArgumentNullException("recipe");
		}
		if (recipe.resultItem == null)
		{
			throw new Exception("탈것장비뽑기상자레시피에 결과아이템이 존재하지 않습니다.");
		}
		m_mountGearPickBoxRecipes.Add(recipe.resultItem.id, recipe);
	}

	public MountGearPickBoxRecipe GetMountGearPickBoxRecipe(int nItemId)
	{
		if (!m_mountGearPickBoxRecipes.TryGetValue(nItemId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddElemental(Elemental elemental)
	{
		if (elemental == null)
		{
			throw new ArgumentNullException("elemental");
		}
		m_elementals.Add(elemental);
	}

	public Elemental GetElemental(int nId)
	{
		int nIndex = nId - 1;
		if (nIndex < 0 || nIndex >= m_elementals.Count)
		{
			return null;
		}
		return m_elementals[nIndex];
	}

	public void AddStoryDungeon(StoryDungeon storyDungeon)
	{
		if (storyDungeon == null)
		{
			throw new ArgumentNullException("storyDungeon");
		}
		m_storyDungeons.Add(storyDungeon);
		AddLocation(storyDungeon);
	}

	public StoryDungeon GetStoryDungeon(int nDungeonNo)
	{
		int nIndex = nDungeonNo - 1;
		if (nIndex < 0 || nIndex >= m_storyDungeons.Count)
		{
			return null;
		}
		return m_storyDungeons[nIndex];
	}

	private void AddWing(Wing wing)
	{
		if (wing == null)
		{
			throw new ArgumentNullException("wing");
		}
		m_wings.Add(wing.id, wing);
	}

	public Wing GetWing(int nId)
	{
		if (!m_wings.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddWingPart(WingPart part)
	{
		if (part == null)
		{
			throw new ArgumentNullException("part");
		}
		m_wingParts.Add(part.id, part);
	}

	public WingPart GetWingPart(int nId)
	{
		if (!m_wingParts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddWingStep(WingStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_wingSteps.Add(step);
	}

	public WingStep GetWingStep(int nStep)
	{
		int nIndex = nStep - 1;
		if (nIndex < 0 || nIndex >= m_wingSteps.Count)
		{
			return null;
		}
		return m_wingSteps[nIndex];
	}

	public void AddWingMemoryPieceType(WingMemoryPieceType type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		m_wingMemoryPieceTypes.Add(type.type, type);
	}

	public WingMemoryPieceType GetWingMemoryPieceType(int nType)
	{
		if (!m_wingMemoryPieceTypes.TryGetValue(nType, out var value))
		{
			return null;
		}
		return value;
	}

	public CartGrade GetCartGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_cartGrades.Length)
		{
			return null;
		}
		return m_cartGrades[nIndex];
	}

	public Cart GetCart(int nId)
	{
		if (!m_carts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public Vector3 SelectNationTransmissionExitPosition()
	{
		return Util.SelectPoint(m_nationTransmissionExitPosition, m_fNationTransmissionExitRadius);
	}

	public float SelectNationTransmissionExitRotationY()
	{
		if (m_nNationTransmissionYRotationType != 1)
		{
			return SFRandom.NextFloat(0f, m_fNationTransmissionYRotation);
		}
		return m_fNationTransmissionYRotation;
	}

	public UndergroundMazePortal GetUndergroundMazePortal(int nPortalId)
	{
		int nIndex = nPortalId - 1;
		if (nIndex < 0 || nIndex >= m_undergroundMazePortals.Count)
		{
			return null;
		}
		return m_undergroundMazePortals[nIndex];
	}

	public UndergroundMazeNpc GetUndergroundMazeNpc(int nNpcId)
	{
		int nIndex = nNpcId - 1;
		if (nIndex < 0 || nIndex >= m_undergroundMazeNpcs.Count)
		{
			return null;
		}
		return m_undergroundMazeNpcs[nIndex];
	}

	private void AddTodayTask(TodayTask task)
	{
		if (task == null)
		{
			throw new ArgumentNullException("task");
		}
		m_todayTasks.Add(task.id, task);
	}

	public TodayTask GetTodayTask(int nId)
	{
		if (!m_todayTasks.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddAchivementReward(AchievementReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_achievementRewards.Add(reward);
	}

	public AchievementReward GetAchivementReward(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_achievementRewards.Count)
		{
			return null;
		}
		return m_achievementRewards[nIndex];
	}

	public PvpExploit GetPvpExploit(int nLevelGap)
	{
		if (m_pvpExploits.Count == 0)
		{
			return null;
		}
		int nIndex = nLevelGap - m_pvpExploits.First().levelGap;
		if (nIndex < 0 || nIndex >= m_pvpExploits.Count)
		{
			return null;
		}
		return m_pvpExploits[nIndex];
	}

	public HonorShopProduct GetHonorShopProduct(int nId)
	{
		if (!m_honorShopProducts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddLevelRankingReward(LevelRankingReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_levelRankingRewards.Add(reward);
	}

	public LevelRankingReward GetLevelRankingReward(int nRanking)
	{
		foreach (LevelRankingReward reward in m_levelRankingRewards)
		{
			if (nRanking >= reward.highRanking && nRanking <= reward.lowRanking)
			{
				return reward;
			}
		}
		return null;
	}

	private void AddGuildLevel(GuildLevel guildLevel)
	{
		if (guildLevel == null)
		{
			throw new ArgumentNullException("guildLevel");
		}
		m_guildLevels.Add(guildLevel);
	}

	public GuildLevel GetGuildLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_guildLevels.Count)
		{
			return null;
		}
		return m_guildLevels[nIndex];
	}

	private void AddGuildMemberGrade(GuildMemberGrade grade)
	{
		if (grade == null)
		{
			throw new ArgumentNullException("grade");
		}
		m_guildMemberGrade[grade.id - 1] = grade;
	}

	public GuildMemberGrade GetGuildMemberGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_guildMemberGrade.Length)
		{
			return null;
		}
		return m_guildMemberGrade[nIndex];
	}

	private void AddGuildDonationEntry(GuildDonationEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_guildDonationEntries.Add(entry.id, entry);
	}

	public GuildDonationEntry GetGuildDonationEntry(int nId)
	{
		if (!m_guildDonationEntries.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public GuildTerritoryNpc GetGuildTerritoryNpc(int nId)
	{
		if (!m_guildTerritoryNpcs.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddGuildBuilding(GuildBuilding building)
	{
		if (building == null)
		{
			throw new ArgumentNullException("building");
		}
		m_guildBuildings.Add(building.id, building);
		switch (building.id)
		{
		case 1:
			m_guildLobby = building;
			break;
		case 2:
			m_guildLaboratory = building;
			break;
		case 3:
			m_guildShop = building;
			break;
		case 4:
			m_guildTankFactory = building;
			break;
		}
	}

	public GuildBuilding GetGuildBuilding(int nId)
	{
		if (!m_guildBuildings.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddGuildSkill(GuildSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_guildSkills.Add(skill.id, skill);
	}

	public GuildSkill GetGuildSkill(int nId)
	{
		if (!m_guildSkills.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddGuildContent(GuildContent content)
	{
		if (content == null)
		{
			throw new ArgumentNullException("content");
		}
		m_guildContents.Add(content.id, content);
		if (content.isDailyObjective)
		{
			m_guildDailyObjectiveContents.Add(content.id, content);
			m_nGuildDailyObjectiveContentTotalPoint += content.point;
		}
	}

	public GuildContent SelectGuildContent()
	{
		return Util.SelectPickEntry(m_guildDailyObjectiveContents.Values, m_nGuildDailyObjectiveContentTotalPoint);
	}

	private void AddGuildDailyObjectiveReward(GuildDailyObjectiveReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_guildDailyObjectiveRewards.Add(reward);
	}

	public GuildDailyObjectiveReward GetGuildDailyObjectiveReward(int nId)
	{
		int nIndex = nId - 1;
		if (nIndex < 0 || nIndex >= m_guildDailyObjectiveRewards.Count)
		{
			return null;
		}
		return m_guildDailyObjectiveRewards[nIndex];
	}

	private void AddGuildWeeklyObjective(GuildWeeklyObjective objective)
	{
		if (objective == null)
		{
			throw new ArgumentNullException("objective");
		}
		m_guildWeeklyObjectives.Add(objective.id, objective);
	}

	public GuildWeeklyObjective GetGuildWeeklyObjective(int nId)
	{
		if (!m_guildWeeklyObjectives.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddGuildBlessingBuff(GuildBlessingBuff buff)
	{
		if (buff == null)
		{
			throw new ArgumentNullException("buff");
		}
		m_guildBlessingBuffs.Add(buff.id, buff);
	}

	public GuildBlessingBuff GetGuildBlessingBuff(int nId)
	{
		if (!m_guildBlessingBuffs.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddNationNoblesse(NationNoblesse nationNoblesse)
	{
		if (nationNoblesse == null)
		{
			throw new ArgumentNullException("nationNoblesse");
		}
		m_nationNoblesses.Add(nationNoblesse.id, nationNoblesse);
	}

	public NationNoblesse GetNationNoblesse(int nId)
	{
		if (!m_nationNoblesses.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddNationDonationEntry(NationDonationEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_nationDonationEntries.Add(entry.id, entry);
	}

	public NationDonationEntry GetNationDonationEntry(int nId)
	{
		if (!m_nationDonationEntries.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddIllustratedBookCategory(IllustratedBookCategory category)
	{
		if (category == null)
		{
			throw new ArgumentNullException("category");
		}
		m_illustratedBookCategories.Add(category.id, category);
	}

	public IllustratedBookCategory GetIllustratedBookCategory(int nCategoryId)
	{
		if (!m_illustratedBookCategories.TryGetValue(nCategoryId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddIllustartedBookType(IllustratedBookType type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		m_illustratedBookTypes.Add(type.type, type);
	}

	public IllustratedBookType GetIllustratedBookType(int nType)
	{
		if (!m_illustratedBookTypes.TryGetValue(nType, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddIllustratedBook(IllustratedBook illstratedBook)
	{
		if (illstratedBook == null)
		{
			throw new ArgumentNullException("illstratedBook");
		}
		m_illustratedBooks.Add(illstratedBook.id, illstratedBook);
	}

	public IllustratedBook GetIllustratedBook(int nId)
	{
		if (!m_illustratedBooks.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddIllustratedBookExplorationStep(IllustratedBookExplorationStep illustratedBookExplorationStep)
	{
		if (illustratedBookExplorationStep == null)
		{
			throw new ArgumentNullException("illustratedBookExplorationStep");
		}
		m_illustratedBookExplorationSteps.Add(illustratedBookExplorationStep);
	}

	public IllustratedBookExplorationStep GetIllustratedBookExplorationStep(int nStepNo)
	{
		int nIndex = nStepNo - 1;
		if (nIndex < 0 || nIndex >= m_illustratedBookExplorationSteps.Count)
		{
			return null;
		}
		return m_illustratedBookExplorationSteps[nIndex];
	}

	private void AddSceneryQuest(SceneryQuest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_sceneryQuests.Add(quest.id, quest);
	}

	public SceneryQuest GetSceneryQuest(int nQuestId)
	{
		if (!m_sceneryQuests.TryGetValue(nQuestId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddAccomplishment(Accomplishment accomplishment)
	{
		if (accomplishment == null)
		{
			throw new ArgumentNullException("accomplishment");
		}
		m_accomplishments.Add(accomplishment.id, accomplishment);
	}

	public Accomplishment GetAccomplishment(int nId)
	{
		if (!m_accomplishments.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddTitleType(TitleType type)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		m_titleTypes.Add(type.id, type);
	}

	public TitleType GetTitleType(int nId)
	{
		if (!m_titleTypes.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddTitle(Title title)
	{
		if (title == null)
		{
			throw new ArgumentNullException("title");
		}
		m_title.Add(title.id, title);
	}

	public Title GetTitle(int nId)
	{
		if (!m_title.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddCreatureCardGrade(CreatureCardGrade grade)
	{
		if (grade == null)
		{
			throw new ArgumentNullException("grade");
		}
		m_creatureCardGrades[grade.id - 1] = grade;
	}

	public CreatureCardGrade GetCreatureCardGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_creatureCardGrades.Length)
		{
			return null;
		}
		return m_creatureCardGrades[nIndex];
	}

	private void AddCreatureCard(CreatureCard card)
	{
		if (card == null)
		{
			throw new ArgumentNullException("card");
		}
		m_creatureCards.Add(card.id, card);
	}

	public CreatureCard GetCreatureCard(int nId)
	{
		if (!m_creatureCards.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddCreatureCardCollectionGrade(CreatureCardCollectionGrade grade)
	{
		if (grade == null)
		{
			throw new ArgumentNullException("grade");
		}
		m_creatureCardCollectionGrades[grade.id - 1] = grade;
	}

	public CreatureCardCollectionGrade GetCreatureCardCollectionGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_creatureCardCollectionGrades.Length)
		{
			return null;
		}
		return m_creatureCardCollectionGrades[nIndex];
	}

	private void AddCreatureCardCollectionCategory(CreatureCardCollectionCategory category)
	{
		if (category == null)
		{
			throw new ArgumentNullException("category");
		}
		m_creatureCardCollectionCategories.Add(category.id, category);
	}

	public CreatureCardCollectionCategory GetCreatureCardCollectionCategory(int nId)
	{
		if (!m_creatureCardCollectionCategories.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddCreatureCardCollection(CreatureCardCollection collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		m_creaturceCardCollections.Add(collection.id, collection);
	}

	public CreatureCardCollection GetCreatureCardCollection(int nId)
	{
		if (!m_creaturceCardCollections.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddCreatureCardShopRefreshSchedule(CreatureCardShopRefreshSchedule schedule)
	{
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_creatureCardShopRefreshSchedules.Add(schedule);
	}

	public CreatureCardShopRefreshSchedule GetCreatureCardShopRefreshSchedule(int nId)
	{
		int nIndex = nId - 1;
		if (nIndex < 0 || nIndex < m_creatureCardShopRefreshSchedules.Count)
		{
			return null;
		}
		return m_creatureCardShopRefreshSchedules[nIndex];
	}

	private void AddCreatureCardShopFixedProduct(CreatureCardShopFixedProduct product)
	{
		if (product == null)
		{
			throw new ArgumentNullException("product");
		}
		m_creatureCardShopFixedProducts.Add(product.id, product);
	}

	public CreatureCardShopFixedProduct GetCreatureCardShopFixedProduct(int nId)
	{
		if (!m_creatureCardShopFixedProducts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddCreatureCardShopRandomProduct(CreatureCardShopRandomProduct product)
	{
		if (product == null)
		{
			throw new ArgumentNullException("product");
		}
		m_creatureCardShopRandomProducts.Add(product.id, product);
	}

	public CreatureCardShopRandomProduct GetCreatureCardShopRandomProduct(int nId)
	{
		if (!m_creatureCardShopRandomProducts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<CreatureCardShopRandomProduct> SelectCreatureCardShopRandomProducts()
	{
		return Util.SelectPickEntries(m_creatureCardShopRandomProducts.Values, m_nCreatureCardShopRandomProductsTotalPoint, m_nCreatureCardShopRandomProductCount, bDuplicated: false);
	}

	private void AddEliteMonsterCategory(EliteMonsterCategory category)
	{
		if (category == null)
		{
			throw new ArgumentNullException("category");
		}
		m_eliteMonsterCategories.Add(category.id, category);
	}

	public EliteMonsterCategory GetEliteMonsterCategoryByCategoryId(int nCategoryId)
	{
		if (!m_eliteMonsterCategories.TryGetValue(nCategoryId, out var category))
		{
			return null;
		}
		return category;
	}

	public EliteMonsterCategory GetEliteMonsterCategoryByContinentId(int nContinentId)
	{
		foreach (EliteMonsterCategory category in m_eliteMonsterCategories.Values)
		{
			if (category.continentId == nContinentId)
			{
				return category;
			}
		}
		return null;
	}

	private void AddEliteMonsterMaster(EliteMonsterMaster master)
	{
		if (master == null)
		{
			throw new ArgumentNullException("master");
		}
		m_eliteMonsterMasters.Add(master.id, master);
	}

	public EliteMonsterMaster GetEliteMonsterMaster(int nMasterId)
	{
		if (!m_eliteMonsterMasters.TryGetValue(nMasterId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddEliteMonster(EliteMonster eliteMonster)
	{
		if (eliteMonster == null)
		{
			throw new ArgumentNullException("eliteMonster");
		}
		m_eliteMonsters.Add(eliteMonster.id, eliteMonster);
	}

	public EliteMonster GetEliteMonster(int nMonsterId)
	{
		if (!m_eliteMonsters.TryGetValue(nMonsterId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddStaminaBuyCounts(StaminaBuyCount count)
	{
		if (count == null)
		{
			throw new ArgumentNullException("count");
		}
		m_staminaBuyCounts.Add(count);
	}

	public StaminaBuyCount GetStaminaBuyCount(int nCount)
	{
		int nIndex = nCount - 1;
		if (nIndex < 0 || nIndex >= m_staminaBuyCounts.Count)
		{
			return null;
		}
		return m_staminaBuyCounts[nIndex];
	}

	private void AddStaminaRecoverySchedule(StaminaRecoverySchedule schedule)
	{
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_staminaRecoverySchedules.Add(schedule);
		m_nTotalStaminaOfStaminaRecoverySchedules += schedule.recoveryStamina;
	}

	public StaminaRecoverySchedule GetStaminaRecoverySchedule(int nId)
	{
		int nIndex = nId - 1;
		if (nIndex < 0 || nIndex >= m_staminaRecoverySchedules.Count)
		{
			return null;
		}
		return m_staminaRecoverySchedules[nIndex];
	}

	public int GetStaminaSumToStaminaRecoverySchedule(int nId)
	{
		int nSum = 0;
		foreach (StaminaRecoverySchedule schedule in m_staminaRecoverySchedules)
		{
			if (schedule.id <= nId)
			{
				nSum += schedule.recoveryStamina;
				continue;
			}
			return nSum;
		}
		return nSum;
	}

	private void AddMonsterKillExpFactor(MonsterKillExpFactor factor)
	{
		if (factor == null)
		{
			throw new ArgumentNullException("factor");
		}
		m_monsterKillExpFactors.Add(factor.levelGap, factor);
	}

	public MonsterKillExpFactor GetMonsterKillExpFactor(int nLevelGap)
	{
		if (!m_monsterKillExpFactors.TryGetValue(nLevelGap, out var value))
		{
			return null;
		}
		return value;
	}

	public float GetMonsterKillExpFactorValue(int nLevelGap)
	{
		return GetMonsterKillExpFactor(nLevelGap)?.expFactor ?? 1f;
	}

	private void AddWorldLevelExpFactor(WorldLevelExpFactor factor)
	{
		if (factor == null)
		{
			throw new ArgumentNullException("factor");
		}
		m_worldLevelExpFactors.Add(factor.levelGap, factor);
	}

	public WorldLevelExpFactor GetWorldLevelExpFactor(int nLevelGap)
	{
		if (!m_worldLevelExpFactors.TryGetValue(nLevelGap, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddPartyExpFactor(PartyExpFactor factor)
	{
		if (factor == null)
		{
			throw new ArgumentNullException("factor");
		}
		m_partyExpFactors.Add(factor);
	}

	public PartyExpFactor GetPartyExpFactor(int nMemberCount)
	{
		int nIndex = nMemberCount - 1;
		if (nIndex < 0 || nIndex >= m_partyExpFactors.Count)
		{
			return null;
		}
		return m_partyExpFactors[nIndex];
	}

	public float GetPartyExpFactorValue(int nMemberCount)
	{
		return GetPartyExpFactor(nMemberCount)?.expFactor ?? 1f;
	}

	private void AddJobCommonSkill(JobCommonSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_jobCommonSkills.Add(skill);
	}

	public JobCommonSkill GetJobCommonSkill(int nSkillId)
	{
		int nIndex = nSkillId - 1;
		if (nIndex < 0 || nIndex >= m_jobCommonSkills.Count)
		{
			return null;
		}
		return m_jobCommonSkills[nIndex];
	}

	private void AddRookieGift(RookieGift gift)
	{
		if (gift == null)
		{
			throw new ArgumentNullException("gift");
		}
		m_rookieGifts.Add(gift);
	}

	public RookieGift GetRookieGift(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_rookieGifts.Count)
		{
			return null;
		}
		return m_rookieGifts[nIndex];
	}

	private void AddOpenGift(OpenGift gift)
	{
		if (gift == null)
		{
			throw new ArgumentNullException("gift");
		}
		m_openGifts.Add(gift.day, gift);
	}

	public OpenGift GetOpenGift(int nDay)
	{
		if (!m_openGifts.TryGetValue(nDay, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddOpen7DayEventDay(Open7DayEventDay day)
	{
		if (day == null)
		{
			throw new ArgumentNullException("day");
		}
		m_open7DayEventDays.Add(day.day, day);
	}

	public Open7DayEventDay GetOpen7DayEventDay(int nDay)
	{
		if (!m_open7DayEventDays.TryGetValue(nDay, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddOpen7DayEventMission(Open7DayEventMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_open7DayEventMissions.Add(mission.id, mission);
	}

	public Open7DayEventMission GetOpen7DayEventMission(int nId)
	{
		if (!m_open7DayEventMissions.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddOpen7DayEventDayProduct(Open7DayEventProduct product)
	{
		if (product == null)
		{
			throw new ArgumentNullException("product");
		}
		m_open7DayEventProducts.Add(product.id, product);
	}

	public Open7DayEventProduct GetOpen7DayEventDayProduct(int nId)
	{
		if (!m_open7DayEventProducts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddRetireval(Retrieval retrieval)
	{
		if (retrieval == null)
		{
			throw new ArgumentNullException("retrieval");
		}
		m_retrievals.Add(retrieval.id, retrieval);
	}

	public Retrieval GetRetrieval(int nId)
	{
		if (!m_retrievals.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddTaskConsignment(TaskConsignment consignment)
	{
		if (consignment == null)
		{
			throw new ArgumentNullException("consignment");
		}
		m_taskConsignments.Add(consignment.id, consignment);
	}

	public TaskConsignment GetTaskConsignment(int nId)
	{
		if (!m_taskConsignments.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddWarehouseSlotExtendRecipe(WarehouseSlotExtendRecipe recipe)
	{
		if (recipe == null)
		{
			throw new ArgumentNullException("recipe");
		}
		m_warehouseSlotExtendRecipes.Add(recipe);
	}

	public WarehouseSlotExtendRecipe GetWarehouseSlotExtendRecipe(int nSlotCount)
	{
		int nIndex = nSlotCount - 1;
		if (nIndex < 0 || nIndex > m_warehouseSlotExtendRecipes.Count)
		{
			return null;
		}
		return m_warehouseSlotExtendRecipes[nIndex];
	}

	public void AddDiaShopCategory(DiaShopCategory category)
	{
		if (category == null)
		{
			throw new ArgumentNullException("category");
		}
		m_diaShopCatregories.Add(category.id, category);
	}

	public DiaShopCategory GetDiaShopCategory(int nId)
	{
		if (!m_diaShopCatregories.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddDiaShopProducts(DiaShopProduct product)
	{
		if (product == null)
		{
			throw new ArgumentNullException("product");
		}
		m_diaShopProducts.Add(product.id, product);
	}

	public DiaShopProduct GetDiaShopProduct(int nId)
	{
		if (!m_diaShopProducts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddSubQuest(SubQuest subQuest)
	{
		if (subQuest == null)
		{
			throw new ArgumentNullException("subQuest");
		}
		m_subQuests.Add(subQuest.id, subQuest);
	}

	public SubQuest GetSubQuest(int nId)
	{
		if (!m_subQuests.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void GetAutoAcceptionSubQuests(SubQuestRequiredConditionType conditionType, int nConditionValue, ICollection<SubQuest> buffer)
	{
		foreach (SubQuest quest in m_subQuests.Values)
		{
			if (quest.isAutoAccepted && quest.requiredConditionType == conditionType && quest.requiredConditionValue == nConditionValue)
			{
				buffer.Add(quest);
			}
		}
	}

	public void AddOrdealQuest(OrdealQuest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_ordealQuests.Add(quest);
	}

	public OrdealQuest GetOrdealQuest(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_ordealQuests.Count)
		{
			return null;
		}
		return m_ordealQuests[nIndex];
	}

	private void AddMoneyBuff(MoneyBuff moneyBuff)
	{
		if (moneyBuff == null)
		{
			throw new ArgumentNullException("moneyBuff");
		}
		m_moneyBuffs.Add(moneyBuff.id, moneyBuff);
	}

	public MoneyBuff GetMoneyBuff(int nId)
	{
		if (!m_moneyBuffs.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddBiography(Biography biography)
	{
		if (biography == null)
		{
			throw new ArgumentNullException("biography");
		}
		m_biographies.Add(biography.id, biography);
	}

	public Biography GetBiography(int nId)
	{
		if (!m_biographies.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddBiographyQuestDungeon(BiographyQuestDungeon dungeon)
	{
		if (dungeon == null)
		{
			throw new ArgumentException("dungeon");
		}
		m_biographyQuestDungeons.Add(dungeon.id, dungeon);
		AddLocation(dungeon);
	}

	public BiographyQuestDungeon GetBiographyQuestDungeon(int nId)
	{
		if (!m_biographyQuestDungeons.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public Blessing GetBlessing(int nId)
	{
		if (!m_blessings.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public BlessingTargetLevel GetBlessingTargetLevel(int nId)
	{
		if (!m_blessingTargetLevels.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public BlessingTargetLevel GetBlessingTargetLevelByLevel(int nLevel)
	{
		foreach (BlessingTargetLevel level in m_blessingTargetLevels.Values)
		{
			if (level.targetHeroLevel == nLevel)
			{
				return level;
			}
		}
		return null;
	}

	private void AddCreatureCharacter(CreatureCharacter character)
	{
		if (character == null)
		{
			throw new ArgumentNullException("character");
		}
		m_creatureCharacters.Add(character.id, character);
	}

	public CreatureCharacter GetCreatureCharacter(int nId)
	{
		if (!m_creatureCharacters.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddCreatureGrade(CreatureGrade grade)
	{
		if (grade == null)
		{
			throw new ArgumentNullException("grade");
		}
		m_creatureGrades[grade.grade - 1] = grade;
	}

	public CreatureGrade GetCreatureGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_creatureGrades.Length)
		{
			return null;
		}
		return m_creatureGrades[nIndex];
	}

	private void AddCreature(Creature creature)
	{
		if (creature == null)
		{
			throw new ArgumentNullException("creature");
		}
		m_creatures.Add(creature.id, creature);
	}

	public Creature GetCreature(int nId)
	{
		if (!m_creatures.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddCreatureLevel(CreatureLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_creatureLevels.Add(level);
	}

	public CreatureLevel GetCreatureLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_creatureLevels.Count)
		{
			return null;
		}
		return m_creatureLevels[nIndex];
	}

	private void AddCreatureBaseAttr(CreatureBaseAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_creatureBaseAttrs.Add(attr.attrId, attr);
	}

	public CreatureBaseAttr GetCreatureBaseAttr(int nId)
	{
		if (!m_creatureBaseAttrs.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddCreatureAdditionalAttr(CreatureAdditionalAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentException("attr");
		}
		m_creatureAdditionalAttrs.Add(attr.attrId, attr);
		m_nTotalCreatureAdditionalAttrPickPoint += attr.point;
	}

	public CreatureAdditionalAttr GetCreatureAdditionalAttr(int nAttrId)
	{
		if (!m_creatureAdditionalAttrs.TryGetValue(nAttrId, out var value))
		{
			return null;
		}
		return value;
	}

	public CreatureAdditionalAttr SelectCreatureAdditionalAttr()
	{
		return Util.SelectPickEntry(m_creatureAdditionalAttrs.Values, m_nTotalCreatureAdditionalAttrPickPoint);
	}

	public List<CreatureAdditionalAttr> SelectCreatureAdditionalAttrs()
	{
		return Util.SelectPickEntries(m_creatureAdditionalAttrs.Values, m_nTotalCreatureAdditionalAttrPickPoint, m_nCreatureAdditionalAttrCount, bDuplicated: false);
	}

	private void AddCreatureSkillGrade(CreatureSkillGrade grade)
	{
		if (grade == null)
		{
			throw new ArgumentNullException("grade");
		}
		m_creatureSkillGrades.Add(grade);
		m_nTotalCreatureSkillGradePickPoint += grade.point;
	}

	public CreatureSkillGrade GetCreatureSkillGrade(int nGrade)
	{
		int nIndex = nGrade - 1;
		if (nIndex < 0 || nIndex >= m_creatureSkillGrades.Count)
		{
			return null;
		}
		return m_creatureSkillGrades[nIndex];
	}

	public CreatureSkillGrade SelectCreatureSkillGrade()
	{
		return Util.SelectPickEntry(m_creatureSkillGrades, m_nTotalCreatureSkillGradePickPoint);
	}

	private void AddCreatureSkill(CreatureSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_creatureSkills.Add(skill.id, skill);
	}

	public CreatureSkill GetCreatureSkill(int nId)
	{
		if (!m_creatureSkills.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddCreatureSkillCountPoolEntry(CreatureSkillCountPoolEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_creatureSkillCountPool.Add(entry);
		m_nTotalCreautreSkillCountPickPoint += entry.point;
	}

	public CreatureSkillCountPoolEntry GetCreatureSkillCountPoolEntry(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_creatureSkillCountPool.Count)
		{
			return null;
		}
		return m_creatureSkillCountPool[nIndex];
	}

	public CreatureSkillCountPoolEntry SelectCreatureSkillCountPoolEntry()
	{
		return Util.SelectPickEntry(m_creatureSkillCountPool, m_nTotalCreautreSkillCountPickPoint);
	}

	private void AddCreatureSkillSlotOpenRecipe(CreatureSkillSlotOpenRecipe recipe)
	{
		if (recipe == null)
		{
			throw new ArgumentNullException("recipe");
		}
		m_creatureSkillSlotOpenRecipes.Add(recipe);
	}

	public CreatureSkillSlotOpenRecipe GetCreatureSkillSlotOpenRecipe(int nSlotCount)
	{
		int nIndex = nSlotCount - 1;
		if (nIndex < 0 || nIndex >= m_creatureSkillSlotOpenRecipes.Count)
		{
			return null;
		}
		return m_creatureSkillSlotOpenRecipes[nIndex];
	}

	private void AddCreatureSkillSlotProtection(CreatureSkillSlotProtection protection)
	{
		if (protection == null)
		{
			throw new ArgumentNullException("protection");
		}
		m_creatureSkillSlotPortections.Add(protection.count, protection);
	}

	public CreatureSkillSlotProtection GetCreatureSkillSlotProtection(int nCount)
	{
		if (!m_creatureSkillSlotPortections.TryGetValue(nCount, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddCreatureInjectionLevel(CreatureInjectionLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_creatureInjectionLevels.Add(level);
	}

	public CreatureInjectionLevel GetCreatureInjectionLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_creatureInjectionLevels.Count)
		{
			return null;
		}
		return m_creatureInjectionLevels[nIndex];
	}

	private void AddCreatureInjectionLevelUpEntry(CreatureInjectionLevelUpEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_creatureInjectionLevelUpEntries.Add(entry);
		m_nTotalCreatureInjectionLevelUpEntryPickPoint += entry.point;
	}

	public CreatureInjectionLevelUpEntry GetCreatureInjectionLevelUpEntry(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_creatureInjectionLevelUpEntries.Count)
		{
			return null;
		}
		return m_creatureInjectionLevelUpEntries[nIndex];
	}

	private void AddPresent(Present present)
	{
		m_presents.Add(present.id, present);
	}

	public Present GetPresent(int nId)
	{
		if (!m_presents.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddWeeklyPresentPopularityPointRankingRewardGroup(WeeklyPresentPopularityPointRankingRewardGroup group)
	{
		m_weeklyPresentPopularityPointRankingRewardGroups.Add(group.no, group);
	}

	public WeeklyPresentPopularityPointRankingRewardGroup GetWeeklyPresentPopularityPointRankingRewardGroup(int nNo)
	{
		if (!m_weeklyPresentPopularityPointRankingRewardGroups.TryGetValue(nNo, out var value))
		{
			return null;
		}
		return value;
	}

	public WeeklyPresentPopularityPointRankingRewardGroup GetWeeklyPresentPopularityPointRankingRewardGroupByRanking(int nRanking)
	{
		foreach (WeeklyPresentPopularityPointRankingRewardGroup group in m_weeklyPresentPopularityPointRankingRewardGroups.Values)
		{
			if (nRanking >= group.highRanking && nRanking <= group.lowRanking)
			{
				return group;
			}
		}
		return null;
	}

	private void AddWeeklyPresentContributionPointRankingRewardGroup(WeeklyPresentContributionPointRankingRewardGroup group)
	{
		m_weeklyPresentContributionPointRankingRewardGroups.Add(group.no, group);
	}

	public WeeklyPresentContributionPointRankingRewardGroup GetWeeklyPresentContributionPointRankingRewardGroup(int nNo)
	{
		if (!m_weeklyPresentContributionPointRankingRewardGroups.TryGetValue(nNo, out var value))
		{
			return null;
		}
		return value;
	}

	public WeeklyPresentContributionPointRankingRewardGroup GetWeeklyPresentContributionPointRankingRewardGroupByRanking(int nRanking)
	{
		foreach (WeeklyPresentContributionPointRankingRewardGroup group in m_weeklyPresentContributionPointRankingRewardGroups.Values)
		{
			if (nRanking >= group.highRanking && nRanking <= group.lowRanking)
			{
				return group;
			}
		}
		return null;
	}

	public CreatureInjectionLevelUpEntry SelectCreatureInjectionLevelUpEntry()
	{
		return Util.SelectPickEntry(m_creatureInjectionLevelUpEntries, m_nTotalCreatureInjectionLevelUpEntryPickPoint);
	}

	private void AddCostume(Costume costume)
	{
		if (costume == null)
		{
			throw new ArgumentNullException("costume");
		}
		m_costumes.Add(costume.id, costume);
	}

	public Costume GetCostume(int nCostumeId)
	{
		if (!m_costumes.TryGetValue(nCostumeId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddCostumeEffect(CostumeEffect costumeEffect)
	{
		if (costumeEffect == null)
		{
			throw new ArgumentNullException("costumeEffect");
		}
		m_costumeEffects.Add(costumeEffect.id, costumeEffect);
	}

	public CostumeEffect GetCostumeEffect(int nCostumeEffectId)
	{
		if (!m_costumeEffects.TryGetValue(nCostumeEffectId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddCostumeEnchantLevel(CostumeEnchantLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		m_costumeEnchantLevels.Add(level);
	}

	public CostumeEnchantLevel GetCostumeEnchantLevel(int nLevel)
	{
		if (nLevel < 0 || nLevel >= m_costumeEnchantLevels.Count)
		{
			return null;
		}
		return m_costumeEnchantLevels[nLevel];
	}

	public void AddCostumeCollection(CostumeCollection collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		m_costumeCollections.Add(collection.id, collection);
	}

	public CostumeCollection GetCostumeCollection(int nId)
	{
		if (!m_costumeCollections.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public CostumeCollection SelectCostumeCollection(int nExclusiveId)
	{
		List<CostumeCollection> colletionPool = new List<CostumeCollection>();
		int nTotalPickPoint = 0;
		foreach (CostumeCollection collection in m_costumeCollections.Values)
		{
			if (collection.id != nExclusiveId)
			{
				colletionPool.Add(collection);
				nTotalPickPoint += collection.point;
			}
		}
		return Util.SelectPickEntry(colletionPool, nTotalPickPoint);
	}

	private void AddCashProduct(CashProduct product)
	{
		if (product == null)
		{
			throw new ArgumentNullException("product");
		}
		m_cashProducts.Add(product.id, product);
	}

	public CashProduct GetCashProduct(int nId)
	{
		if (!m_cashProducts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddChargeEvent(ChargeEvent evt)
	{
		m_chargeEvents.Add(evt.id, evt);
	}

	public ChargeEvent GetChargeEvent(int nId)
	{
		if (!m_chargeEvents.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public ChargeEvent GetChargeEventByTime(DateTime time)
	{
		foreach (ChargeEvent evt in m_chargeEvents.Values)
		{
			if (evt.IsEventTime(time))
			{
				return evt;
			}
		}
		return null;
	}

	private void AddConsumeEvent(ConsumeEvent evt)
	{
		m_consumeEvents.Add(evt.id, evt);
	}

	public ConsumeEvent GetConsumeEvent(int nId)
	{
		if (!m_consumeEvents.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public ConsumeEvent GetConsumeEventByTime(DateTime time)
	{
		foreach (ConsumeEvent evt in m_consumeEvents.Values)
		{
			if (evt.IsEventTime(time))
			{
				return evt;
			}
		}
		return null;
	}

	public bool IsNationAllianceUnavailableTime(DateTimeOffset time)
	{
		int nTime = (int)time.TimeOfDay.TotalSeconds;
		if (nTime >= m_nNationAllianceUnavailableStartTime && nTime < m_nNationAllianceUnavailableEndTime)
		{
			return true;
		}
		return false;
	}

	private void AddJobChangeQuest(JobChangeQuest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_jobChangeQuests.Add(quest);
	}

	public JobChangeQuest GetJobChangeQuest(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_jobChangeQuests.Count)
		{
			return null;
		}
		return m_jobChangeQuests[nIndex];
	}

	private void AddPotionAttr(PotionAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_potionAttrs.Add(attr.id, attr);
	}

	public PotionAttr GetPotionAttr(int nAttrId)
	{
		if (!m_potionAttrs.TryGetValue(nAttrId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddConstellation(Constellation constellation)
	{
		if (constellation == null)
		{
			throw new ArgumentNullException("constellation");
		}
		m_constellations.Add(constellation.id, constellation);
	}

	public Constellation GetConstellation(int nId)
	{
		if (!m_constellations.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddArtifact(Artifact artifact)
	{
		if (artifact == null)
		{
			throw new ArgumentNullException("artifact");
		}
		m_artifacts.Add(artifact);
	}

	public Artifact GetArtifact(int nNo)
	{
		int nIndex = nNo - 1;
		if (nIndex < 0 || nIndex >= m_artifacts.Count)
		{
			return null;
		}
		return m_artifacts[nIndex];
	}

	private void AddArtifactLevelUpMaterial(ArtifactLevelUpMaterial material)
	{
		if (material == null)
		{
			throw new ArgumentNullException("material");
		}
		m_artifactLevelUpMaterials.Add(material);
	}

	public ArtifactLevelUpMaterial GetArtifactLevelUpMaterial(int nTier, int nGrade)
	{
		ArtifactLevelUpMaterial material = null;
		for (int i = 0; i < m_artifactLevelUpMaterials.Count; i++)
		{
			material = m_artifactLevelUpMaterials[i];
			if (material.tier == nTier && material.grade == nGrade)
			{
				return material;
			}
		}
		return null;
	}

	public bool IsValidArtifactNo(int nArtifiactNo)
	{
		if (nArtifiactNo >= 1)
		{
			return nArtifiactNo <= maxArtifactNo;
		}
		return false;
	}

	private void AddSystemMessage(SystemMessage message)
	{
		if (message == null)
		{
			throw new ArgumentNullException("message");
		}
		m_systemMessages.Add(message.id, message);
	}

	public SystemMessage GetSystemMessage(int nId)
	{
		if (!m_systemMessages.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool CheckSystemMessageCondition(int nId, int nValue)
	{
		return GetSystemMessage(nId)?.CheckConditionValue(nValue) ?? false;
	}

	private void AddTimeDesignationEvent(TimeDesignationEvent designationEvent)
	{
		if (designationEvent == null)
		{
			throw new ArgumentNullException("designationEvent");
		}
		m_timeDesignationEvents.Add(designationEvent.id, designationEvent);
	}

	public TimeDesignationEvent GetTimeDesignationEvent(int nId)
	{
		if (!m_timeDesignationEvents.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}
}
