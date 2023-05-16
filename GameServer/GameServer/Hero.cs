using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class Hero : Unit
{
	public const int kUpdateInterval = 500;

	private Account m_account;

	private Guid m_id = Guid.Empty;

	private HeroStatus m_status;

	private Timer m_updateTimer;

	private DateTimeOffset? m_lastLoginTime = null;

	private DateTimeOffset? m_lastLogoutTime = null;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private bool m_bIsInitEntranceCopmleted;

	private Hero m_controller;

	private string m_sName;

	private NationInstance m_nationInst;

	private Job m_job;

	private int m_nLevel;

	private long m_lnExp;

	private DateTimeOffset m_levelUpdateTime = DateTimeOffset.MinValue;

	private long m_lnBattlePower;

	private DateTimeOffset m_battlePowerUpdateTime = DateTimeOffset.MinValue;

	private int m_nRealBaseMaxHP;

	private int m_nRealBasePhysicalOffense;

	private int m_nRealBaseMagicalOffense;

	private int m_nRealBasePhysicalDefense;

	private int m_nRealBaseMagicalDefense;

	private int m_nNormalMaxHP;

	private int m_nNormalPhysicalOffense;

	private int m_nNormalMagicalOffense;

	private int m_nNormalPhysicalDefense;

	private int m_nNormalMagicalDefense;

	private int m_nNormalCritical;

	private int m_nNormalCriticalResistance;

	private int m_nNormalCriticalDamageIncRate;

	private int m_nNormalCriticalDamageDecRate;

	private int m_nNormalPenetration;

	private int m_nNormalBlock;

	private int m_nNormalFireOffense;

	private int m_nNormalFireDefense;

	private int m_nNormalLightningOffense;

	private int m_nNormalLightningDefense;

	private int m_nNormalDarkOffense;

	private int m_nNormalDarkDefense;

	private int m_nNormalHolyOffense;

	private int m_nNormalHolyDefense;

	private int m_nNormalDamageIncRate;

	private int m_nNormalDamageDecRate;

	private int m_nNormalStunResistance;

	private int m_nNormalSnareResistance;

	private int m_nNormalSilenceResistance;

	private int m_nNormalBaseMaxHPIncRate;

	private int m_nNormalBaseOffenseIncRate;

	private int m_nNormalBasePhysicalDefenseIncRate;

	private int m_nNormalBaseMagicalDefenseIncRate;

	private int m_nNormalOffense;

	private Dictionary<int, HeroSkill> m_skills = new Dictionary<int, HeroSkill>();

	private int m_nLastCastSkillId;

	private bool m_bMoving;

	private DateTimeOffset m_moveStartTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_manualMoveStartTime = DateTimeOffset.MinValue;

	private bool m_bManualMoving;

	private bool m_bWalking;

	private int m_nMoveSpeed;

	private bool m_bAcceleration;

	private DateTimeOffset m_moveVerificationStartTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_moveVerificationEndTime = DateTimeOffset.MinValue;

	private float m_fMoveVerificationDistance;

	private int m_nAbnormalMoveSpeedCount;

	private List<InventorySlot> m_inventorySlots = new List<InventorySlot>();

	private Dictionary<int, HeroInventoryItem> m_inventoryItems = new Dictionary<int, HeroInventoryItem>();

	private int m_nPaidInventorySlotCount;

	private Dictionary<Guid, HeroMainGear> m_mainGears = new Dictionary<Guid, HeroMainGear>();

	private HeroMainGear m_equippedWeapon;

	private HeroMainGear m_equippedArmor;

	private DateValuePair<int> m_mainGearEnchantDailyCount = new DateValuePair<int>();

	private DateValuePair<int> m_mainGearRefinementDailyCount = new DateValuePair<int>();

	private MainGearEnchantLevelSet m_mainGearEnchantLevelSet;

	private Dictionary<int, HeroSubGear> m_subGears = new Dictionary<int, HeroSubGear>();

	private SubGearSoulstoneLevelSet m_subGearSoulstoneLevelSet;

	private Location m_lastLocation;

	private int m_nLastLocationParam;

	private Guid m_lastInstanceId = Guid.Empty;

	private Vector3 m_lastPosition = Vector3.zero;

	private float m_fLastYRotation;

	private Nation m_previousNation;

	private Continent m_previousContinent;

	private Vector3 m_previousPosition = Vector3.zero;

	private float m_fPreviousYRotation;

	private PlaceEntranceParam m_placeEntranceParam;

	protected Dictionary<Guid, long> m_receivedDamages = new Dictionary<Guid, long>();

	private HashSet<MonsterInstance> m_ownMonsterInsts = new HashSet<MonsterInstance>();

	private HashSet<MonsterInstance> m_aggroMonsterInsts = new HashSet<MonsterInstance>();

	private Dictionary<Guid, Mail> m_mails = new Dictionary<Guid, Mail>();

	private Dictionary<Guid, Mail> m_deliveredMails = new Dictionary<Guid, Mail>();

	private int m_nOwnDia;

	private int m_nVipPoint;

	private VipLevel m_vipLevel;

	private long m_lnGold;

	private int m_nLak;

	private int m_nHonorPoint;

	private int m_nSoulPowder;

	private bool m_bIsBattleMode;

	private DateTimeOffset m_battleModeStartTime = DateTimeOffset.MinValue;

	private HeroMainQuest m_currentHeroMainQuest;

	private HeroMainQuestTransformationMonsterEffect m_mainQuestTransformationMonsterEffect;

	private Dictionary<int, HeroMainQuestTransformationMonsterSkill> m_mainQuestTransformationMonsterSkills = new Dictionary<int, HeroMainQuestTransformationMonsterSkill>();

	private HeroTreatOfFarmQuest m_treatOfFarmQuest;

	private HeroBountyHunterQuest m_bountyHunterQuest;

	private DateValuePair<int> m_bountyHunterQuestDailyStartCount = new DateValuePair<int>();

	private HeroFishingQuest m_fishingQuest;

	private DateValuePair<int> m_fishingQuestDailyStartCount = new DateValuePair<int>();

	private Timer m_fishingTimer;

	private ContinentObjectInstance m_currentInteractionContinentObjectInst;

	private Timer m_continentObjectInteractionTimer;

	private DateValuePair<int> m_freeImmediateRevivalDailyCount = new DateValuePair<int>();

	private DateValuePair<int> m_paidImmediateRevivalDailyCount = new DateValuePair<int>();

	private int m_nRestTime;

	private PartyMember m_partyMember;

	private Dictionary<long, PartyInvitation> m_partyInvitations = new Dictionary<long, PartyInvitation>();

	private Dictionary<long, PartyApplication> m_partyApplications = new Dictionary<long, PartyApplication>();

	private DateTimeOffset m_hpPotionLastUsedTime = DateTimeOffset.MinValue;

	private bool m_bIsReturnScrollUsing;

	private Timer m_returnScrollUseTimer;

	private DateTimeOffset m_returnScrollLastUsedTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_lastChatttingTime = DateTimeOffset.MinValue;

	private HashSet<int> m_receivedLevelUpRewards = new HashSet<int>();

	private DateValuePair<int> m_dailyAttendRewardDay = new DateValuePair<int>();

	private Dictionary<int, HeroAccessReward> m_accessRewards = new Dictionary<int, HeroAccessReward>();

	private DateTimeOffset m_dailyAccessTimeUpdateTime = DateTimeOffset.MinValue;

	private float m_fDailyAccessTime;

	private Dictionary<int, HeroSeriesMission> m_seriesMissions = new Dictionary<int, HeroSeriesMission>();

	private HeroTodayMissionCollection m_todayMissionCollection;

	private int m_nRewardedAttainmentEntryNo;

	private DateValuePair<int> m_expPotionDailyUseCount = new DateValuePair<int>();

	private List<HeroMainQuestDungeonReward> m_heroMainQuestDungeonRewards = new List<HeroMainQuestDungeonReward>();

	private Dictionary<int, HeroMount> m_mounts = new Dictionary<int, HeroMount>();

	private HeroMount m_equippedMount;

	private bool m_bIsRiding;

	private Dictionary<Guid, HeroMountGear> m_mountGears = new Dictionary<Guid, HeroMountGear>();

	private HeroMountGearSlot[] m_mountGearSlots = new HeroMountGearSlot[6];

	private DateValuePair<int> m_mountGearRefinementDailyCount = new DateValuePair<int>();

	private Dictionary<int, HeroWing> m_wings = new Dictionary<int, HeroWing>();

	private HeroWing m_equippedWing;

	private WingStepLevel m_wingStepLevel;

	private int m_nWingExp;

	private Dictionary<int, HeroWingPart> m_wingParts = new Dictionary<int, HeroWingPart>();

	private int m_nStamina;

	private DateTimeOffset? m_staminaUpdateTime = null;

	private DateValuePair<int> m_staminaRecoverySchedule = new DateValuePair<int>();

	private DateValuePair<int> m_dailyStaminaBuyCount = new DateValuePair<int>();

	private DateValuePair<int> m_freeSweepDailyCount = new DateValuePair<int>();

	private Dictionary<int, int> m_storyDungeonClearMaxDifficulties = new Dictionary<int, int>();

	private List<StoryDungeonPlay> m_storyDungeonPlays = new List<StoryDungeonPlay>();

	private HashSet<int> m_expDungeonClearDifficulties = new HashSet<int>();

	private DateValuePair<int> m_dailyExpDungeonPlayCount = new DateValuePair<int>();

	private HashSet<int> m_goldDungeonClearDifficulties = new HashSet<int>();

	private DateValuePair<int> m_dailyGoldDungeonPlayCount = new DateValuePair<int>();

	private DateTime m_undergroundMazeDate = DateTime.MinValue.Date;

	private DateTimeOffset m_undergroundMazeStartTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_undergroundMazeLastTickTime = DateTimeOffset.MinValue;

	private float m_fUndergroundMazePlayTime;

	private Guid m_undergroundMazeLogId = Guid.Empty;

	private int m_nArtifactRoomBestFloor;

	private int m_nArtifactRoomCurrentFloor;

	private DateValuePair<int> m_artifactRoomDailyInitCount = new DateValuePair<int>();

	private DateTimeOffset? m_artifactRoomSweepStartTime = null;

	private bool m_bIsArtifactRoomSweepCompleted;

	private int m_nArtifactRoomSweepProgressFloor;

	private DateValuePair<int> m_expScrollDailyUseCount = new DateValuePair<int>();

	private DateTimeOffset? m_expScrollStartTime = null;

	private int m_nExpScrollDuration;

	private Item m_expScrollItem;

	private CartInstance m_ridingCartInst;

	private int m_nPvpKillCount;

	private int m_nPvpAssistCount;

	private int m_nExploitPoint;

	private DateTimeOffset m_exploitPointUpdateTime = DateTimeOffset.MinValue;

	private DateValuePair<int> m_dailyExploitPoint = new DateValuePair<int>();

	private Rank m_rank;

	private int m_nRankRewardReceivedRankNo;

	private DateTime m_rankRewardReceivedDate = DateTime.MinValue.Date;

	private DateValuePair<int> m_dailyMysteryBoxQuestStartCount = new DateValuePair<int>();

	private HeroMysteryBoxQuest m_mysteryBoxQuest;

	private DateValuePair<int> m_dailySecretLetterQuestStartCount = new DateValuePair<int>();

	private HeroSecretLetterQuest m_secretLetterQuest;

	private DateValuePair<int> m_dailyDimensionRaidQuestStartCount = new DateValuePair<int>();

	private HeroDimensionRaidQuest m_dimensionRaidQuest;

	private HeroHolyWarQuestStartScheduleCollection m_dailyHolyWarQuestStartScheduleCollection = new HeroHolyWarQuestStartScheduleCollection();

	private HeroHolyWarQuest m_holyWarQuest;

	private HeroTodayTaskCollection m_todayTaskCollection;

	private DateValuePair<int> m_achivementDailyPoint = new DateValuePair<int>();

	private int m_nReceivedAchivementRewardNo;

	private DateValuePair<int> m_dailyAncientRelicPlayCount = new DateValuePair<int>();

	private DateTimeOffset m_ancientRelicTrapLastHitTime = DateTimeOffset.MinValue;

	private HeroAncientRelicTrapEffect m_ancientRelicAbnormalStateEffect;

	private int m_nRewardedDailyServerLevelRankingNo;

	private DateValuePair<int> m_distortionScrollDailyUseCount = new DateValuePair<int>();

	private DateTimeOffset? m_distortionScrollStartTime = null;

	private int m_nDistortionScrollDuration;

	private Timer m_distortionTimer;

	private int m_nFieldOfHonorRanking;

	private int m_nFieldOfHonorSuccessiveCount;

	private int m_nRewardedDailyFieldOfHonorRankingNo;

	private Guid m_fieldOfHonorHeroGuildId = Guid.Empty;

	private string m_sFieldOfHonorHeroGuildName;

	private int m_nFieldOfHonorHeroGuildMemberGrade;

	private List<int> m_fieldOfHonorTargets = new List<int>();

	private DateValuePair<int> m_dailyFieldOfHonorPlayCount = new DateValuePair<int>();

	private List<FieldOfHonorHistory> m_fieldOfHonorHistories = new List<FieldOfHonorHistory>();

	private GuildMember m_guildMember;

	private DateValuePair<int> m_dailyGuildDonationCount = new DateValuePair<int>();

	private DateTimeOffset m_guildWithdrawalTime = DateTimeOffset.MinValue;

	private DateTime m_guildDailyRewardReceivedDate = DateTime.MinValue.Date;

	private Dictionary<Guid, GuildApplication> m_guildApplications = new Dictionary<Guid, GuildApplication>();

	private DateValuePair<int> m_dailyGuildApplicationCount = new DateValuePair<int>();

	private Dictionary<Guid, GuildInvitation> m_guildInvitations = new Dictionary<Guid, GuildInvitation>();

	private DateValuePair<int> m_dailyGuildFarmQuestStartCount = new DateValuePair<int>();

	private HeroGuildFarmQuest m_guildFarmQuest;

	private DateValuePair<int> m_dailyGuildFoodWarehouseStockCount = new DateValuePair<int>();

	private Guid m_receivedGuildFoodWarehouseCollectionId = Guid.Empty;

	private DateValuePair<int> m_guildMoralPoint = new DateValuePair<int>();

	private DateTime m_guildAltarRewardReceivedDate = DateTime.MinValue.Date;

	private HeroGuildAltarSpellInjectionMission m_guildAltarSpellInjectionMission;

	private HeroGuildAltarDefenseMission m_guildAltarDefenseMission;

	private DateTimeOffset m_guildAltarDefenseMissionStartTime = DateTimeOffset.MinValue;

	private Dictionary<int, HeroGuildSkill> m_guildSkills = new Dictionary<int, HeroGuildSkill>();

	private HeroGuildMissionQuest m_guildMissionQuest;

	private HeroGuildHuntingQuest m_guildHuntingQuest;

	private DateValuePair<int> m_dailyGuildHuntingQuest = new DateValuePair<int>();

	private DateTime m_guildHuntingDonationDate = DateTime.MinValue;

	private DateTime m_guildHuntingDonationCompletionRewardReceivedDate = DateTime.MinValue;

	private MatchingRoom m_matchingRoom;

	private DateValuePair<int> m_dailySupplySupportQuestStartCount = new DateValuePair<int>();

	private HeroSupplySupportQuest m_supplySupportQuest;

	private DateValuePair<int> m_dailyNationDonationCount = new DateValuePair<int>();

	private NationNoblesse m_nationNoblesse;

	private DateValuePair<int> m_dailyNationWarFreeTransmissionCount = new DateValuePair<int>();

	private DateValuePair<int> m_dailyNationWarPaidTransmissionCount = new DateValuePair<int>();

	private NationWarMember m_nationWarMember;

	private NationWarInstance m_allianceNationWarInst;

	private int m_nAllianceNationWarKillCount;

	private GuildSupplySupportQuestPlay m_guildSupplySupportQuestPlay;

	private DateValuePair<int> m_weeklySoulCoveterPlayCount = new DateValuePair<int>();

	private DateValuePair<int> m_receivedDailyObjectiveRewardNo = new DateValuePair<int>();

	private DateTime m_guildWeeklyObjectiveRewardReceivedDate = DateTime.MinValue.Date;

	private DateTimeOffset m_guildDailyObjectiveNoticeTime = DateTimeOffset.MinValue;

	private IllustratedBookExplorationStep m_illustratedBookExplorationStep;

	private int m_nExplorationPoint;

	private DateTime m_illustratedBookExplorationStepRewardReceivedDate = DateTime.MinValue;

	private int m_nIllustratedBookExplorationStepRewardReceivedStepNo;

	private Dictionary<int, HeroIllustratedBook> m_illustratedBooks = new Dictionary<int, HeroIllustratedBook>();

	private HashSet<int> m_sceneryQuestCompletions = new HashSet<int>();

	private Dictionary<int, HeroSceneryQuest> m_sceneryQuests = new Dictionary<int, HeroSceneryQuest>();

	private HashSet<int> m_rewardedAccomplishments = new HashSet<int>();

	private int m_nAccomplishmentPoint;

	private int m_nAccMonsterKillCount;

	private int m_nAccSoulCoveterPlayCount;

	private int m_nAccEpicBaitItemUseCount;

	private int m_nAccLegendBaitItemUseCount;

	private int m_nAccNationWarWinCount;

	private int m_nAccNationWarKillCount;

	private int m_nAccNationWarCommanderKillCount;

	private int m_nAccNationWarImmediateRevivalCount;

	private long m_lnMaxGold;

	private long m_lnMaxBattlePower;

	private int m_nMaxAcquisitionMainGearGrade;

	private int m_nMaxEquippedMainGearEnchantLevel;

	private Dictionary<int, HeroTitle> m_titles = new Dictionary<int, HeroTitle>();

	private HeroTitle m_displayTitle;

	private HeroTitle m_activationTitle;

	private Dictionary<int, HeroCreatureCard> m_creatureCards = new Dictionary<int, HeroCreatureCard>();

	private Dictionary<int, HeroCreatureCardCollection> m_activatedCreatureCardCollections = new Dictionary<int, HeroCreatureCardCollection>();

	private int m_nCreatureCardCollectionFamePoint;

	private DateTimeOffset m_creatureCardCollectionFamePointUpdateTime = DateTimeOffset.MinValue;

	private HashSet<int> m_purchasedCreatureCardShopFixedProducts = new HashSet<int>();

	private Dictionary<int, HeroCreatureCardShopRandomProduct> m_creatureCardShopRandomProducts = new Dictionary<int, HeroCreatureCardShopRandomProduct>();

	private DateTime m_creatureCardShopRefreshDate = DateTime.MinValue.Date;

	private int m_nCreatureCardShopRefreshScheduleId;

	private Guid m_creatureCardShopId = Guid.Empty;

	private DateValuePair<int> m_dailyCreatureCardShopPaidRefreshCount = new DateValuePair<int>();

	private Dictionary<int, HeroEliteMonsterKill> m_heroEliteMonsterKills = new Dictionary<int, HeroEliteMonsterKill>();

	private DateValuePair<int> m_dailyEliteDungeonPlayCount = new DateValuePair<int>();

	private LootingItemMinGrade m_lootingItemMinGrade;

	private bool m_bProofOfValorCleared;

	private DateValuePair<int> m_dailyProofOfValorPlayCount = new DateValuePair<int>();

	private HeroProofOfValorInstance m_heroProofOfValorInst;

	private DateValuePair<int> m_dailyProofOfValorFreeRefreshCount = new DateValuePair<int>();

	private DateValuePair<int> m_dailyProofOfvalorPaidRefreshCount = new DateValuePair<int>();

	private int m_nProofOfValorPaidRefreshCount;

	private DateTime m_proofOfValorAutoRefreshDate = DateTime.MinValue;

	private int m_nProofOfValorAutoRefreshScheduleId;

	private HeroProofOfValorBuff m_proofOfValorBuff;

	private int m_nCustomPresetHair;

	private int m_nCustomFaceJawHeight;

	private int m_nCustomFaceJawWidth;

	private int m_nCustomFaceJawEndHeight;

	private int m_nCustomFaceWidth;

	private int m_nCustomFaceEyebrowHeight;

	private int m_nCustomFaceEyebrowRotation;

	private int m_nCustomFaceEyesWidth;

	private int m_nCustomFaceNoseHeight;

	private int m_nCustomFaceNoseWidth;

	private int m_nCustomFaceMouthHeight;

	private int m_nCustomFaceMouthWidth;

	private int m_nCustomBodyHeadSize;

	private int m_nCustomBodyArmsLength;

	private int m_nCustomBodyArmsWidth;

	private int m_nCustomBodyChestSize;

	private int m_nCustomBodyWaistWidth;

	private int m_nCustomBodyHipsSize;

	private int m_nCustomBodyPelvisWidth;

	private int m_nCustomBodyLegsLength;

	private int m_nCustomBodyLegsWidth;

	private int m_nCustomColorSkin;

	private int m_nCustomColorEyes;

	private int m_nCustomColorBeardAndEyebrow;

	private int m_nCustomColorHair;

	private int m_nTamingMonsterId;

	private Dictionary<int, HeroTamingMonsterSkill> m_tamingMonsterSkills = new Dictionary<int, HeroTamingMonsterSkill>();

	private bool m_bTodayMissionTutorialStarted;

	private Dictionary<int, HeroJobCommonSkill> m_jobCommonSkills = new Dictionary<int, HeroJobCommonSkill>();

	private MonsterInstance m_stealGroggyMonsterInst;

	private Timer m_stealTimer;

	private Dictionary<int, HeroNpcShopProduct> m_npcShopProducts = new Dictionary<int, HeroNpcShopProduct>();

	private Dictionary<int, HeroRankActiveSkill> m_rankActiveSkills = new Dictionary<int, HeroRankActiveSkill>();

	private Dictionary<int, HeroRankPassiveSkill> m_rankPassiveSkills = new Dictionary<int, HeroRankPassiveSkill>();

	private HeroRankActiveSkill m_selectedRankActiveSkill;

	private DateTimeOffset m_rankActiveSkillCastingTime = DateTimeOffset.MinValue;

	private int m_nSpiritStone;

	private int m_nRookieGiftNo;

	private float m_fRookieGiftLoginDuration;

	private DateTimeOffset m_rookieGitfLoginStartTime = DateTimeOffset.MinValue;

	private HashSet<int> m_receivedOpenGiftRewards = new HashSet<int>();

	private DateValuePair<int> m_dailyQuestFreeRefreshCount = new DateValuePair<int>();

	private DateValuePair<int> m_dailyQuestAcceptionCount = new DateValuePair<int>();

	private HeroDailyQuest[] m_dailyQuests;

	private HeroWeeklyQuest m_weeklyQuest;

	private bool m_bWisdomTempleCleared;

	private DateValuePair<int> m_dailyWisdomTemplePlayCount = new DateValuePair<int>();

	private WisdomTempleObjectInstance m_currentInteractionWisdomTempleObjectInst;

	private Timer m_wisdomTempleObjectInteractionTimer;

	private HashSet<int> m_rewardedOpen7DayEventMissions = new HashSet<int>();

	private HashSet<int> m_purchasedOpen7DayEventProducts = new HashSet<int>();

	private Dictionary<int, HeroOpen7DayEventProgressCount> m_open7DayEventProgressCounts = new Dictionary<int, HeroOpen7DayEventProgressCount>();

	private bool m_bOpen7DayEventRewarded;

	private DateTime m_retrievalDate = DateTime.MinValue.Date;

	private Dictionary<int, HeroRetrieval> m_retrievals = new Dictionary<int, HeroRetrieval>();

	private Dictionary<DateTime, HeroRetrievalProgressCountCollection> m_retrievalProgressCountCollections = new Dictionary<DateTime, HeroRetrievalProgressCountCollection>();

	private DateValuePair<int> m_dailyRuinsReclaimFreePlayCount = new DateValuePair<int>();

	private DateTimeOffset m_ruinsReclaimTrapLastHitTime = DateTimeOffset.MinValue;

	private HeroRuinsReclaimTransformationMonsterEffect m_ruinsReclaimTransformationMonsterEffect;

	private bool m_bRuinsReclaimDebuffEffect;

	private RuinsReclaimObjectInstance m_currentInteractionRuinsReclaimObjectInst;

	private Timer m_ruinsReclaimObjectInteractionTimer;

	private Dictionary<Guid, HeroTaskConsignment> m_taskConsignments = new Dictionary<Guid, HeroTaskConsignment>();

	private DateTime m_taskConsignmentStartCountDate = DateTime.MinValue.Date;

	private Dictionary<int, HeroTaskConsignmentStartCount> m_taskConsignmentStartCounts = new Dictionary<int, HeroTaskConsignmentStartCount>();

	private HeroTrueHeroQuest m_trueHeroQuest;

	private Timer m_trueHeroQuestInteractionTimer;

	private DateValuePair<int> m_dailyInfiniteWarPlayCount = new DateValuePair<int>();

	private HeroInfiniteWarBuff m_infiniteWarBuff;

	private DateTime m_limitationGiftDate = DateTime.MinValue.Date;

	private HashSet<int> m_rewardedLimitationGiftScheduleIds = new HashSet<int>();

	private HeroWeekendReward m_weekendReward;

	private int m_nPaidWarehouseSlotCount;

	private Dictionary<int, HeroWarehouseItem> m_warehouseItems = new Dictionary<int, HeroWarehouseItem>();

	private List<WarehouseSlot> m_warehouseSlots = new List<WarehouseSlot>();

	private DateTime m_dailyDiaShopProductBuyCountsDate = DateTime.MinValue.Date;

	private Dictionary<int, HeroDiaShopProductBuyCount> m_dailyDiaShopProductBuyCounts = new Dictionary<int, HeroDiaShopProductBuyCount>();

	private Dictionary<int, HeroDiaShopProductBuyCount> m_totalDiaShopProductBuyCounts = new Dictionary<int, HeroDiaShopProductBuyCount>();

	private DateValuePair<int> m_dailyFearAltarPlayCount = new DateValuePair<int>();

	private DateValuePair<int> m_weeklyFearAltarHalidomCollectionRewardNo = new DateValuePair<int>();

	private DateTime m_fearAltarHalidomWeekStartDate = DateTime.MinValue;

	private Dictionary<int, HeroFearAltarHalidom> m_fearAltarHalidoms = new Dictionary<int, HeroFearAltarHalidom>();

	private DateTime m_fearAltarHalidomElementalRewardWeekStartDate = DateTime.MinValue;

	private Dictionary<int, HeroFearAltarHalidomElementalReward> m_fearAltarHalidomElementalRewards = new Dictionary<int, HeroFearAltarHalidomElementalReward>();

	private Dictionary<int, HeroSubQuest> m_subQuests = new Dictionary<int, HeroSubQuest>();

	private Dictionary<int, HeroSubQuest> m_currentSubQuests = new Dictionary<int, HeroSubQuest>();

	private DateValuePair<int> m_dailyWarMemoryFreePlayCount = new DateValuePair<int>();

	private int m_nWarMemoryStartPositionIndex = -1;

	private WarMemoryTransformationObjectInstance m_currentInteractionWarMemoryTransformationObjectInst;

	private Timer m_warMemoryTransformationObjectInteractionTimer;

	private HeroWarMemoryTransformationMonsterEffect m_warMemoryTransformationMonsterEffect;

	private Dictionary<int, HeroWarMemoryTransformationMonsterSkill> m_warMemoryTransformationMonsterSkills = new Dictionary<int, HeroWarMemoryTransformationMonsterSkill>();

	private HeroOrdealQuest m_ordealQuest;

	private DateValuePair<int> m_dailyOsirisRoomPlayCount = new DateValuePair<int>();

	private HeroOsirisRoomMoneyBuff m_osirisRoomMoneyBuff;

	private Dictionary<Guid, Friend> m_friends = new Dictionary<Guid, Friend>();

	private Dictionary<long, FriendApplication> m_friendApplications = new Dictionary<long, FriendApplication>();

	private Dictionary<long, FriendApplication> m_receivedFriendApplications = new Dictionary<long, FriendApplication>();

	private List<TempFriend> m_tempFriends = new List<TempFriend>();

	private Dictionary<Guid, BlacklistEntry> m_blacklistEntries = new Dictionary<Guid, BlacklistEntry>();

	private List<DeadRecord> m_deadRecords = new List<DeadRecord>();

	private Dictionary<int, HeroBiography> m_biographies = new Dictionary<int, HeroBiography>();

	private DateTime m_itemLuckyShopPickDate = DateTime.MinValue.Date;

	private DateTimeOffset m_itemLuckyShopFreePickTime = DateTimeOffset.MinValue;

	private int m_nItemLuckyShopFreePickCount;

	private int m_nItemLuckyShopPick1TimeCount;

	private int m_nItemLuckyShopPick5TimeCount;

	private DateTime m_creatureCardLuckyShopPickDate = DateTime.MinValue.Date;

	private DateTimeOffset m_creatureCardLuckyShopFreePickTime = DateTimeOffset.MinValue;

	private int m_nCreatureCardLuckyShopFreePickCount;

	private int m_nCreatureCardLuckyShopPick1TimeCount;

	private int m_nCreatureCardLuckyShopPick5TimeCount;

	private Dictionary<long, HeroBlessingQuest> m_blessingQuests = new Dictionary<long, HeroBlessingQuest>();

	private Dictionary<long, HeroBlessing> m_receivedBlessings = new Dictionary<long, HeroBlessing>();

	private Dictionary<Guid, HeroProspectQuest> m_ownerProspectQuests = new Dictionary<Guid, HeroProspectQuest>();

	private Dictionary<Guid, HeroProspectQuest> m_targetProspectQuests = new Dictionary<Guid, HeroProspectQuest>();

	private Dictionary<Guid, HeroCreature> m_creatures = new Dictionary<Guid, HeroCreature>();

	private HeroCreature m_participationCreature;

	private DateValuePair<int> m_dailyCreatureVariationCount = new DateValuePair<int>();

	private DateTimeOffset m_dragonNestTrapLastHitTime = DateTimeOffset.MinValue;

	private HeroDragonNestTrapEffect m_dragonNestTrapEffect;

	private DateTime m_weeklyPresentPopularityPointStartDate = DateTime.MinValue.Date;

	private int m_nWeeklyPresentPopularityPoint;

	private DateTimeOffset m_weeklyPresentPopularityPointUpdateTime = DateTimeOffset.MinValue;

	private int m_nRewardedNationWeeklyPresentPopularityPointRankingNo;

	private DateTime m_weeklyPresentContributionPointStartDate = DateTime.MinValue.Date;

	private int m_nWeeklyPresentContributionPoint;

	private DateTimeOffset m_weeklyPresentContributionPointUpdateTime = DateTimeOffset.MinValue;

	private int m_nRewardedNationWeeklyPresentContributionPointRankingNo;

	private Dictionary<int, HeroCostume> m_costumes = new Dictionary<int, HeroCostume>();

	private HeroCostume m_equippedCostume;

	private CostumeCollection m_costumeCollection;

	private bool m_bCostumeCollectionActivated;

	private HeroCreatureFarmQuest m_creatureFarmQuest;

	private DateValuePair<int> m_dailyCreatureFarmAcceptionCount = new DateValuePair<int>();

	private bool m_bAutoHunting;

	private bool m_bIsSafeMode;

	private DateTimeOffset m_safeModeWaitStartTime = DateTimeOffset.MinValue;

	private HeroJobChangeQuest m_jobChangeQuest;

	private Dictionary<int, HeroPotionAttr> m_potionAttrs = new Dictionary<int, HeroPotionAttr>();

	private DateValuePair<int> m_dailyAnkouTombPlayCount = new DateValuePair<int>();

	private Dictionary<int, HeroAnkouTombBestRecord> m_ankouTombBestRecords = new Dictionary<int, HeroAnkouTombBestRecord>();

	private HeroAnkouTombMoneyBuff m_ankouTombMoneyBuff;

	private Dictionary<int, HeroConstellation> m_constellations = new Dictionary<int, HeroConstellation>();

	private int m_nStarEssense;

	private DateValuePair<int> m_dailyStarEssensItemUseCount = new DateValuePair<int>();

	private int m_nArtifactNo;

	private int m_nArtifactLevel;

	private int m_nArtifactExp;

	private int m_nEquippedArtifactNo;

	private DateValuePair<int> m_dailyTradeShipPlayCount = new DateValuePair<int>();

	private Dictionary<int, HeroTradeShipBestRecord> m_tradeShipBestRecords = new Dictionary<int, HeroTradeShipBestRecord>();

	private HeroTradeShipMoneyBuff m_tradeShipMoneyBuff;

	private int m_nGMTargetMainQuestNo;

	private HashSet<int> m_timeDesignationEventRewards = new HashSet<int>();

	public override UnitType type => UnitType.Hero;

	public Account account => m_account;

	public Guid id => m_id;

	public bool isReal => m_id == m_controller.id;

	public Hero controller => m_controller;

	public bool heroSkillEnabled => !isTransformMonster;

	public override bool skillEnabled => base.skillEnabled;

	public override bool hitEnabled
	{
		get
		{
			if (base.hitEnabled && !isRidingCart)
			{
				return !isTransformRuinsReclaimMonster;
			}
			return false;
		}
	}

	public override bool abnormalStateDamageEnabled
	{
		get
		{
			if (base.hitEnabled && !isRidingCart)
			{
				return !isTransformRuinsReclaimMonster;
			}
			return false;
		}
	}

	public override bool abnormalStateHitEnabled => !isTransformRuinsReclaimMonster;

	public override bool increaseDamageEnabledByProbability => true;

	public override bool blockEnabled => true;

	public HeroStatus status => m_status;

	public bool isLoggingIn => m_status == HeroStatus.LoggingIn;

	public bool isLoggedIn => m_status == HeroStatus.LoggedIn;

	public bool isLoggedOut => m_status == HeroStatus.LoggedOut;

	public bool isInitEntranceCopmleted
	{
		get
		{
			return m_bIsInitEntranceCopmleted;
		}
		set
		{
			m_bIsInitEntranceCopmleted = value;
		}
	}

	public string name => m_sName;

	public DateTimeOffset? lastLoginTime => m_lastLoginTime;

	public DateTimeOffset? lastLogoutTime => m_lastLogoutTime;

	public DateTimeOffset regTime => m_regTime;

	public NationInstance nationInst => m_nationInst;

	public int nationId => m_nationInst.nationId;

	public Job job => m_job;

	public int jobId => m_job.id;

	public int baseJobId => m_job.baseJobId;

	public int level => m_nLevel;

	public long exp => m_lnExp;

	public DateTimeOffset levelUpdateTime => m_levelUpdateTime;

	public long battlePower => m_lnBattlePower;

	public DateTimeOffset battlePowerUpdateTime => m_battlePowerUpdateTime;

	public Dictionary<int, HeroSkill> skills => m_skills;

	public int lastCastSkillId
	{
		get
		{
			return m_nLastCastSkillId;
		}
		set
		{
			m_nLastCastSkillId = value;
		}
	}

	public bool moving => m_bMoving;

	public bool manualMoving => m_bManualMoving;

	public override int moveSpeed => m_nMoveSpeed;

	public Location lastLocation => m_lastLocation;

	public int lastLocationId
	{
		get
		{
			if (m_lastLocation == null)
			{
				return 0;
			}
			return m_lastLocation.locationId;
		}
	}

	public int lastLocationParam => m_nLastLocationParam;

	public Guid lastInstanceId => m_lastInstanceId;

	public Vector3 lastPosition => m_lastPosition;

	public float lastYRotation => m_fLastYRotation;

	public Nation previousNation => m_previousNation;

	public int previousNationId
	{
		get
		{
			if (m_previousNation == null)
			{
				return 0;
			}
			return m_previousNation.id;
		}
	}

	public Continent previousContinent => m_previousContinent;

	public int previousContinentId
	{
		get
		{
			if (m_previousContinent == null)
			{
				return 0;
			}
			return m_previousContinent.id;
		}
	}

	public Vector3 previousPosition => m_previousPosition;

	public float previousYRotation => m_fPreviousYRotation;

	public PlaceEntranceParam placeEntranceParam
	{
		get
		{
			return m_placeEntranceParam;
		}
		set
		{
			m_placeEntranceParam = value;
		}
	}

	public Dictionary<Guid, long> receivedDamages => m_receivedDamages;

	public HashSet<MonsterInstance> ownMonsterInsts => m_ownMonsterInsts;

	public HashSet<MonsterInstance> aggroMonsterInsts => m_aggroMonsterInsts;

	public object syncObject => m_account.syncObject;

	public int paidInventorySlotCount
	{
		get
		{
			return m_nPaidInventorySlotCount;
		}
		set
		{
			m_nPaidInventorySlotCount = value;
		}
	}

	public int inventorySlotCount => m_inventorySlots.Count;

	public int emptyInventorySlotCount
	{
		get
		{
			int nCount = 0;
			foreach (InventorySlot slot in m_inventorySlots)
			{
				if (slot.isEmpty)
				{
					nCount++;
				}
			}
			return nCount;
		}
	}

	public HeroMainGear equippedWeapon
	{
		get
		{
			return m_equippedWeapon;
		}
		set
		{
			m_equippedWeapon = value;
		}
	}

	public Guid equippedWeaponId
	{
		get
		{
			if (m_equippedWeapon == null)
			{
				return Guid.Empty;
			}
			return m_equippedWeapon.id;
		}
	}

	public int eqquippedWeaponLevel
	{
		get
		{
			if (m_equippedWeapon == null)
			{
				return 0;
			}
			return m_equippedWeapon.enchantLevel;
		}
	}

	public HeroMainGear equippedArmor
	{
		get
		{
			return m_equippedArmor;
		}
		set
		{
			m_equippedArmor = value;
		}
	}

	public Guid equippedArmorId
	{
		get
		{
			if (m_equippedArmor == null)
			{
				return Guid.Empty;
			}
			return m_equippedArmor.id;
		}
	}

	public int equippedArmorLevel
	{
		get
		{
			if (m_equippedArmor == null)
			{
				return 0;
			}
			return m_equippedArmor.enchantLevel;
		}
	}

	public int equippedMainGearsMaxLevel
	{
		get
		{
			if (eqquippedWeaponLevel <= equippedArmorLevel)
			{
				return equippedArmorLevel;
			}
			return eqquippedWeaponLevel;
		}
	}

	public Dictionary<Guid, Mail> mails => m_mails;

	public int ownDia => m_nOwnDia;

	public int unOwnDia => m_account.unOwnDia;

	public int dia => m_nOwnDia + unOwnDia;

	public int vipPoint => m_nVipPoint;

	public int totalVipPoint => m_nVipPoint + m_account.vipPoint;

	public VipLevel vipLevel => m_vipLevel;

	public long gold => m_lnGold;

	public int honorPoint => m_nHonorPoint;

	public int soulPowder => m_nSoulPowder;

	public DateValuePair<int> mainGearEnchantDailyCount => m_mainGearEnchantDailyCount;

	public DateValuePair<int> mainGearRefinementDailyCount => m_mainGearRefinementDailyCount;

	public int lak => m_nLak;

	public bool isBattleMode => m_bIsBattleMode;

	public float radius => m_job.radius;

	public HeroExclusiveAction currentExclusiveAction
	{
		get
		{
			if (m_currentInteractionContinentObjectInst != null)
			{
				return HeroExclusiveAction.ContinentObjectInteraction;
			}
			if (m_bIsReturnScrollUsing)
			{
				return HeroExclusiveAction.ReturnScrollUse;
			}
			if (m_fishingTimer != null)
			{
				return HeroExclusiveAction.Fishing;
			}
			if (isMysteryBoxPicking)
			{
				return HeroExclusiveAction.MysteryBoxPick;
			}
			if (isSecretLetterPicking)
			{
				return HeroExclusiveAction.SecretLetterPick;
			}
			if (isDimensionRaidInteracting)
			{
				return HeroExclusiveAction.DimensionRaidInteraction;
			}
			if (isGuildFarmInteracting)
			{
				return HeroExclusiveAction.GuildFarmInteraction;
			}
			if (m_guildAltarSpellInjectionMission != null)
			{
				return HeroExclusiveAction.GuildAltarSpellInjectionMission;
			}
			if (m_stealGroggyMonsterInst != null)
			{
				return HeroExclusiveAction.GroggyMonsterSteal;
			}
			if (m_currentInteractionWisdomTempleObjectInst != null)
			{
				return HeroExclusiveAction.WisdomTempleObjectInteraction;
			}
			if (m_currentInteractionRuinsReclaimObjectInst != null)
			{
				return HeroExclusiveAction.RuinsReclaimObjectInteraction;
			}
			if (m_trueHeroQuestInteractionTimer != null)
			{
				return HeroExclusiveAction.TrueHeroQuestInteraction;
			}
			if (m_currentInteractionWarMemoryTransformationObjectInst != null)
			{
				return HeroExclusiveAction.WarMemoryObjectInteraction;
			}
			return HeroExclusiveAction.None;
		}
	}

	public HeroMainQuest currentHeroMainQuest
	{
		get
		{
			return m_currentHeroMainQuest;
		}
		set
		{
			m_currentHeroMainQuest = value;
		}
	}

	public int completedMainQuestNo
	{
		get
		{
			if (m_currentHeroMainQuest == null)
			{
				return 0;
			}
			if (!m_currentHeroMainQuest.completed)
			{
				return m_currentHeroMainQuest.mainQuest.no - 1;
			}
			return m_currentHeroMainQuest.mainQuest.no;
		}
	}

	public HeroMainQuestTransformationMonsterEffect mainQuestTransformationMonsterEffect => m_mainQuestTransformationMonsterEffect;

	public bool isTransformMainQuestMonster => m_mainQuestTransformationMonsterEffect != null;

	public HeroTreatOfFarmQuest treatOfFarmQuest
	{
		get
		{
			return m_treatOfFarmQuest;
		}
		set
		{
			m_treatOfFarmQuest = value;
		}
	}

	public HeroBountyHunterQuest bountyHunterQuest
	{
		get
		{
			return m_bountyHunterQuest;
		}
		set
		{
			m_bountyHunterQuest = value;
		}
	}

	public DateValuePair<int> bountyHunterQuestDailyStartCount => m_bountyHunterQuestDailyStartCount;

	public HeroFishingQuest fishingQuest
	{
		get
		{
			return m_fishingQuest;
		}
		set
		{
			m_fishingQuest = value;
		}
	}

	public DateValuePair<int> fishinQuestDailyStartCount => m_fishingQuestDailyStartCount;

	public bool isFishing => m_fishingTimer != null;

	public ContinentObjectInstance currentInteractionContinentObjectInst => m_currentInteractionContinentObjectInst;

	public DateValuePair<int> freeImmediateRevivalDailyCount => m_freeImmediateRevivalDailyCount;

	public DateValuePair<int> paidImmediateRevivalDailyCount => m_paidImmediateRevivalDailyCount;

	public int restTime
	{
		get
		{
			return m_nRestTime;
		}
		set
		{
			m_nRestTime = value;
		}
	}

	public PartyMember partyMember
	{
		get
		{
			return m_partyMember;
		}
		set
		{
			m_partyMember = value;
		}
	}

	public DateTimeOffset hpPotionLastUsedTime
	{
		get
		{
			return m_hpPotionLastUsedTime;
		}
		set
		{
			m_hpPotionLastUsedTime = value;
		}
	}

	public bool isReturnScrollUsing => m_bIsReturnScrollUsing;

	public DateTimeOffset returnScrollLastUsedTime => m_returnScrollLastUsedTime;

	public DateTimeOffset lastChattingTime
	{
		get
		{
			return m_lastChatttingTime;
		}
		set
		{
			m_lastChatttingTime = value;
		}
	}

	public HashSet<int> receivedLevelUpRewards => m_receivedLevelUpRewards;

	public DateValuePair<int> dailyAttendReawrdDay => m_dailyAttendRewardDay;

	public Dictionary<int, HeroAccessReward> accessRewards => m_accessRewards;

	public DateTimeOffset dailyAccessTimeUpdateTime => m_dailyAccessTimeUpdateTime;

	public float dailyAccessTime => m_fDailyAccessTime;

	public HeroTodayMissionCollection todayMissionCollection => m_todayMissionCollection;

	public int rewardedAttainmentEntryNo
	{
		get
		{
			return m_nRewardedAttainmentEntryNo;
		}
		set
		{
			m_nRewardedAttainmentEntryNo = value;
		}
	}

	public DateValuePair<int> expPotionDailyUseCount => m_expPotionDailyUseCount;

	public bool isRiding => m_bIsRiding;

	public HeroMount equippedMount
	{
		get
		{
			return m_equippedMount;
		}
		set
		{
			m_equippedMount = value;
		}
	}

	public int equippedMountId
	{
		get
		{
			if (m_equippedMount == null)
			{
				return 0;
			}
			return m_equippedMount.mount.id;
		}
	}

	public int maxMountLevel
	{
		get
		{
			int nMaxLevel = 0;
			foreach (HeroMount mount in m_mounts.Values)
			{
				if (nMaxLevel < mount.level)
				{
					nMaxLevel = mount.level;
				}
			}
			return nMaxLevel;
		}
	}

	public DateValuePair<int> mountGearRefinementDailyCount => m_mountGearRefinementDailyCount;

	public Dictionary<int, HeroWing> wings => m_wings;

	public HeroWing equippedWing => m_equippedWing;

	public int equippedWingId
	{
		get
		{
			if (m_equippedWing == null)
			{
				return 0;
			}
			return m_equippedWing.wing.id;
		}
	}

	public WingStep wingStep => m_wingStepLevel.step;

	public WingStepLevel wingStepLevel => m_wingStepLevel;

	public int wingExp => m_nWingExp;

	public Dictionary<int, HeroWingPart> wingParts => m_wingParts;

	public int stamina => m_nStamina;

	public DateTimeOffset? staminaUpdateTime => m_staminaUpdateTime;

	public bool isFullStamina => m_nStamina >= Resource.instance.maxStamina;

	public DateValuePair<int> staminaRecoverySchedule => m_staminaRecoverySchedule;

	public DateValuePair<int> dailyStaminaBuyCount => m_dailyStaminaBuyCount;

	public DateValuePair<int> freeSweepDailyCount => m_freeSweepDailyCount;

	public DateValuePair<int> dailyExpDungeonPlayCount => m_dailyExpDungeonPlayCount;

	public DateValuePair<int> dailyGoldDungeonPlayCount => m_dailyGoldDungeonPlayCount;

	public MainGearEnchantLevelSet mainGearEnchantLevelSet
	{
		get
		{
			return m_mainGearEnchantLevelSet;
		}
		set
		{
			m_mainGearEnchantLevelSet = value;
		}
	}

	public int mainGearEnchantLevelSetNo
	{
		get
		{
			if (m_mainGearEnchantLevelSet == null)
			{
				return 0;
			}
			return m_mainGearEnchantLevelSet.setNo;
		}
	}

	public int subGearsMaxLevel
	{
		get
		{
			int nMaxLevel = 0;
			foreach (HeroSubGear subGear in m_subGears.Values)
			{
				if (nMaxLevel < subGear.level)
				{
					nMaxLevel = subGear.level;
				}
			}
			return nMaxLevel;
		}
	}

	public SubGearSoulstoneLevelSet subGearSoulstoneLevelSet
	{
		get
		{
			return m_subGearSoulstoneLevelSet;
		}
		set
		{
			m_subGearSoulstoneLevelSet = value;
		}
	}

	public int subGearSoulstoneLevelSetNo
	{
		get
		{
			if (m_subGearSoulstoneLevelSet == null)
			{
				return 0;
			}
			return m_subGearSoulstoneLevelSet.setNo;
		}
	}

	public int totalMountedSoulstoneLevel
	{
		get
		{
			int nTotalMountedSoulstoneLevel = 0;
			foreach (HeroSubGear subGear in m_subGears.Values)
			{
				if (subGear.equipped)
				{
					nTotalMountedSoulstoneLevel += subGear.totalMountedSoulstoneLevel;
				}
			}
			return nTotalMountedSoulstoneLevel;
		}
	}

	public HashSet<int> expDungeonClearDifficulties => m_expDungeonClearDifficulties;

	public HashSet<int> goldDungeonClearDifficulties => m_goldDungeonClearDifficulties;

	public DateTime undergroundMazeDate => m_undergroundMazeDate;

	public DateTimeOffset undergroundMazeStartTime => m_undergroundMazeStartTime;

	public DateTimeOffset undergroundMazeLastTickTime => m_undergroundMazeLastTickTime;

	public float undergroundMazePlayTime => m_fUndergroundMazePlayTime;

	public Guid undergroundMazeLogId => m_undergroundMazeLogId;

	public int artifactRoomBestFloor => m_nArtifactRoomBestFloor;

	public DateValuePair<int> artifactRoomDailyInitCount => m_artifactRoomDailyInitCount;

	public int artifactRoomCurrentFloor => m_nArtifactRoomCurrentFloor;

	public DateTimeOffset? artifactRoomSweepStartTime => m_artifactRoomSweepStartTime;

	public bool isArtifactRoomSweepCompleted => m_bIsArtifactRoomSweepCompleted;

	public int artifactRoomSweepProgressFloor => m_nArtifactRoomSweepProgressFloor;

	public DateValuePair<int> expScrollDailyUseCount => m_expScrollDailyUseCount;

	public DateTimeOffset? expScrollStartTime => m_expScrollStartTime;

	public int expScrollDuration => m_nExpScrollDuration;

	public Item expScrollItem => m_expScrollItem;

	public int expScrollItemId
	{
		get
		{
			if (m_expScrollItem == null)
			{
				return 0;
			}
			return m_expScrollItem.id;
		}
	}

	public CartInstance ridingCartInst
	{
		get
		{
			return m_ridingCartInst;
		}
		set
		{
			m_ridingCartInst = value;
		}
	}

	public bool isRidingCart => m_ridingCartInst != null;

	public int pvpKillCount => m_nPvpKillCount;

	public int pvpAssistCount => m_nPvpAssistCount;

	public int exploitPoint => m_nExploitPoint;

	public DateTimeOffset exploitPointUpdateTime => m_exploitPointUpdateTime;

	public DateValuePair<int> dailyExploitPoint => m_dailyExploitPoint;

	public Rank rank => m_rank;

	public int rankNo
	{
		get
		{
			if (m_rank == null)
			{
				return 0;
			}
			return m_rank.no;
		}
	}

	public int rankRewardReceivedRankNo
	{
		get
		{
			return m_nRankRewardReceivedRankNo;
		}
		set
		{
			m_nRankRewardReceivedRankNo = value;
		}
	}

	public DateTime rankRewardReceivedDate
	{
		get
		{
			return m_rankRewardReceivedDate;
		}
		set
		{
			m_rankRewardReceivedDate = value;
		}
	}

	public DateValuePair<int> dailyMysteryBoxQuestStartCount => m_dailyMysteryBoxQuestStartCount;

	public HeroMysteryBoxQuest mysteryBoxQuest
	{
		get
		{
			return m_mysteryBoxQuest;
		}
		set
		{
			m_mysteryBoxQuest = value;
		}
	}

	public bool isMysteryBoxPicking
	{
		get
		{
			if (m_mysteryBoxQuest == null)
			{
				return false;
			}
			return m_mysteryBoxQuest.isPicking;
		}
	}

	public DateValuePair<int> dailySecretLetterQuestStartCount => m_dailySecretLetterQuestStartCount;

	public HeroSecretLetterQuest secretLetterQuest
	{
		get
		{
			return m_secretLetterQuest;
		}
		set
		{
			m_secretLetterQuest = value;
		}
	}

	public bool isSecretLetterPicking
	{
		get
		{
			if (m_secretLetterQuest == null)
			{
				return false;
			}
			return m_secretLetterQuest.isPicking;
		}
	}

	public DateValuePair<int> dailyDimensionRaidQuestStartCount => m_dailyDimensionRaidQuestStartCount;

	public HeroDimensionRaidQuest dimensionRaidQuest
	{
		get
		{
			return m_dimensionRaidQuest;
		}
		set
		{
			m_dimensionRaidQuest = value;
		}
	}

	public bool isDimensionRaidInteracting
	{
		get
		{
			if (m_dimensionRaidQuest == null)
			{
				return false;
			}
			return m_dimensionRaidQuest.isInteracting;
		}
	}

	public HeroHolyWarQuestStartScheduleCollection dailyHolyWarQuestStartScheduleCollection => m_dailyHolyWarQuestStartScheduleCollection;

	public HeroHolyWarQuest holyWarQuest
	{
		get
		{
			return m_holyWarQuest;
		}
		set
		{
			m_holyWarQuest = value;
		}
	}

	public HeroTodayTaskCollection todayTaskCollection => m_todayTaskCollection;

	public DateValuePair<int> achievementDailyPoint => m_achivementDailyPoint;

	public int receivedAchievementRewardNo
	{
		get
		{
			return m_nReceivedAchivementRewardNo;
		}
		set
		{
			m_nReceivedAchivementRewardNo = value;
		}
	}

	public DateValuePair<int> dailyAncientRelicPlayCount => m_dailyAncientRelicPlayCount;

	public bool isAncientRelicTrapEffect => m_ancientRelicAbnormalStateEffect != null;

	public MatchingRoom matchingRoom
	{
		get
		{
			return m_matchingRoom;
		}
		set
		{
			m_matchingRoom = value;
		}
	}

	public bool isMatching => m_matchingRoom != null;

	public int rewardedDailyServerLevelRankingNo
	{
		get
		{
			return m_nRewardedDailyServerLevelRankingNo;
		}
		set
		{
			m_nRewardedDailyServerLevelRankingNo = value;
		}
	}

	public DateValuePair<int> distortionScrollDailyUseCount => m_distortionScrollDailyUseCount;

	public DateTimeOffset? distortionScrollStartTime => m_distortionScrollStartTime;

	public int distortionScrollDuration => m_nDistortionScrollDuration;

	public bool isDistorting => m_distortionTimer != null;

	public bool isUseableDistortionScroll
	{
		get
		{
			if (!isBattleMode)
			{
				return !isTrueHeroQuestWaitingStep;
			}
			return false;
		}
	}

	public int fieldOfHonorRanking
	{
		get
		{
			return m_nFieldOfHonorRanking;
		}
		set
		{
			m_nFieldOfHonorRanking = value;
		}
	}

	public int fieldOfHonorSuccessiveCount
	{
		get
		{
			return m_nFieldOfHonorSuccessiveCount;
		}
		set
		{
			m_nFieldOfHonorSuccessiveCount = value;
		}
	}

	public int rewardedDailyFieldOfHonorRankingNo
	{
		get
		{
			return m_nRewardedDailyFieldOfHonorRankingNo;
		}
		set
		{
			m_nRewardedDailyFieldOfHonorRankingNo = value;
		}
	}

	public List<int> fieldOfHonorTargets => m_fieldOfHonorTargets;

	public DateValuePair<int> dailyFieldOfHonorPlayCount => m_dailyFieldOfHonorPlayCount;

	public List<FieldOfHonorHistory> fieldOfHonorHistories => m_fieldOfHonorHistories;

	public GuildMember guildMember
	{
		get
		{
			return m_guildMember;
		}
		set
		{
			m_guildMember = value;
		}
	}

	public int totalGuildContributionPoint
	{
		get
		{
			if (m_guildMember == null)
			{
				return 0;
			}
			return m_guildMember.totalContributionPoint;
		}
	}

	public int guildContributionPoint
	{
		get
		{
			if (m_guildMember == null)
			{
				return 0;
			}
			return m_guildMember.contributionPoint;
		}
	}

	public int guildPoint
	{
		get
		{
			if (m_guildMember == null)
			{
				return 0;
			}
			return m_guildMember.guildPoint;
		}
	}

	public DateTimeOffset guildWithdrawalTime
	{
		get
		{
			return m_guildWithdrawalTime;
		}
		set
		{
			m_guildWithdrawalTime = value;
		}
	}

	public DateTime guildDailyRewardReceivedDate
	{
		get
		{
			return m_guildDailyRewardReceivedDate;
		}
		set
		{
			m_guildDailyRewardReceivedDate = value;
		}
	}

	public Dictionary<Guid, GuildApplication> guildApplications => m_guildApplications;

	public DateValuePair<int> dailyGuildApplicationCount => m_dailyGuildApplicationCount;

	public DateValuePair<int> dailyGuildDonationCount => m_dailyGuildDonationCount;

	public DateValuePair<int> dailyGuildFarmQuestStartCount => m_dailyGuildFarmQuestStartCount;

	public HeroGuildFarmQuest guildFarmQuest => m_guildFarmQuest;

	public bool isGuildFarmInteracting
	{
		get
		{
			if (m_guildFarmQuest == null)
			{
				return false;
			}
			return m_guildFarmQuest.isInteracting;
		}
	}

	public DateValuePair<int> dailyGuildFoodWarehouseStockCount => m_dailyGuildFoodWarehouseStockCount;

	public Guid receivedGuildFoodWarehouseCollectionId
	{
		get
		{
			return m_receivedGuildFoodWarehouseCollectionId;
		}
		set
		{
			m_receivedGuildFoodWarehouseCollectionId = value;
		}
	}

	public DateTime guildMoralPointDate => m_guildMoralPoint.date;

	public int guildMoralPoint => m_guildMoralPoint.value;

	public DateTime guildAltarRewardReceivedDate
	{
		get
		{
			return m_guildAltarRewardReceivedDate;
		}
		set
		{
			m_guildAltarRewardReceivedDate = value;
		}
	}

	public HeroGuildAltarSpellInjectionMission guildAltarSpellInjectionMission => m_guildAltarSpellInjectionMission;

	public HeroGuildAltarDefenseMission guildAltarDefenseMission => m_guildAltarDefenseMission;

	public DateTimeOffset guildAltarDefenseMissionStartTime => m_guildAltarDefenseMissionStartTime;

	public HeroGuildMissionQuest guildMissionQuest
	{
		get
		{
			return m_guildMissionQuest;
		}
		set
		{
			m_guildMissionQuest = value;
		}
	}

	public HeroGuildHuntingQuest guildHuntingQuest
	{
		get
		{
			return m_guildHuntingQuest;
		}
		set
		{
			m_guildHuntingQuest = value;
		}
	}

	public DateValuePair<int> dailyGuildHuntingQuestCount => m_dailyGuildHuntingQuest;

	public DateTime guildHuntingDonationDate
	{
		get
		{
			return m_guildHuntingDonationDate;
		}
		set
		{
			m_guildHuntingDonationDate = value;
		}
	}

	public DateTime guildHuntingDonationCompletionRewardReceivedDate
	{
		get
		{
			return m_guildHuntingDonationCompletionRewardReceivedDate;
		}
		set
		{
			m_guildHuntingDonationCompletionRewardReceivedDate = value;
		}
	}

	public DateValuePair<int> receivedGuildDailyObjectiveRewardNo => m_receivedDailyObjectiveRewardNo;

	public DateTime guildWeeklyObjectiveRewardReceivedDate
	{
		get
		{
			return m_guildWeeklyObjectiveRewardReceivedDate;
		}
		set
		{
			m_guildWeeklyObjectiveRewardReceivedDate = value;
		}
	}

	public DateTimeOffset guildDailyObjectiveNoticeTime
	{
		get
		{
			return m_guildDailyObjectiveNoticeTime;
		}
		set
		{
			m_guildDailyObjectiveNoticeTime = value;
		}
	}

	public DateValuePair<int> dailySupplySupportQuestStartCount => m_dailySupplySupportQuestStartCount;

	public HeroSupplySupportQuest supplySupportQuest
	{
		get
		{
			return m_supplySupportQuest;
		}
		set
		{
			m_supplySupportQuest = value;
		}
	}

	public NationNoblesse nationNoblesse => m_nationNoblesse;

	public int nationNoblesseId
	{
		get
		{
			if (m_nationNoblesse == null)
			{
				return 0;
			}
			return m_nationNoblesse.id;
		}
	}

	public DateValuePair<int> dailyNationWarFreeTransmissionCount => m_dailyNationWarFreeTransmissionCount;

	public DateValuePair<int> dailyNationWarPaidTransmissionCount => m_dailyNationWarPaidTransmissionCount;

	public NationWarMember nationWarMember
	{
		get
		{
			return m_nationWarMember;
		}
		set
		{
			m_nationWarMember = value;
		}
	}

	public NationWarInstance allianceNationWarInst => m_allianceNationWarInst;

	public DateValuePair<int> dailyNationDonationCount => m_dailyNationDonationCount;

	public GuildSupplySupportQuestPlay guildSupplySupportQuestPlay
	{
		get
		{
			return m_guildSupplySupportQuestPlay;
		}
		set
		{
			m_guildSupplySupportQuestPlay = value;
		}
	}

	public DateValuePair<int> weeklySoulCoveterPlayCount => m_weeklySoulCoveterPlayCount;

	public IllustratedBookExplorationStep illustratedBookExplorationStep => m_illustratedBookExplorationStep;

	public int illustratedBookExplorationStepNo
	{
		get
		{
			if (m_illustratedBookExplorationStep == null)
			{
				return 0;
			}
			return m_illustratedBookExplorationStep.no;
		}
	}

	public int explorationPoint => m_nExplorationPoint;

	public DateTime illustratedBookExplorationStepRewardReceivedDate
	{
		get
		{
			return m_illustratedBookExplorationStepRewardReceivedDate;
		}
		set
		{
			m_illustratedBookExplorationStepRewardReceivedDate = value;
		}
	}

	public int illustratedBookExplorationStepRewardReceivedStepNo
	{
		get
		{
			return m_nIllustratedBookExplorationStepRewardReceivedStepNo;
		}
		set
		{
			m_nIllustratedBookExplorationStepRewardReceivedStepNo = value;
		}
	}

	public HashSet<int> sceneryQuestCompletions => m_sceneryQuestCompletions;

	public HashSet<int> rewardedAccomplishments => m_rewardedAccomplishments;

	public int accomplishmentPoint
	{
		get
		{
			return m_nAccomplishmentPoint;
		}
		set
		{
			m_nAccomplishmentPoint = value;
		}
	}

	public int accMonsterKillCount
	{
		get
		{
			return m_nAccMonsterKillCount;
		}
		set
		{
			m_nAccMonsterKillCount = value;
		}
	}

	public int accSoulCoveterPlayCount
	{
		get
		{
			return m_nAccSoulCoveterPlayCount;
		}
		set
		{
			m_nAccSoulCoveterPlayCount = value;
		}
	}

	public int accEpicBaitItemUseCount
	{
		get
		{
			return m_nAccEpicBaitItemUseCount;
		}
		set
		{
			m_nAccEpicBaitItemUseCount = value;
		}
	}

	public int accLegendBaitItemUseCount
	{
		get
		{
			return m_nAccLegendBaitItemUseCount;
		}
		set
		{
			m_nAccLegendBaitItemUseCount = value;
		}
	}

	public int accNationWarWinCount
	{
		get
		{
			return m_nAccNationWarWinCount;
		}
		set
		{
			m_nAccNationWarWinCount = value;
		}
	}

	public int accNationWarKillCount
	{
		get
		{
			return m_nAccNationWarKillCount;
		}
		set
		{
			m_nAccNationWarKillCount = value;
		}
	}

	public int accNationWarCommanderKillCount
	{
		get
		{
			return m_nAccNationWarCommanderKillCount;
		}
		set
		{
			m_nAccNationWarCommanderKillCount = value;
		}
	}

	public int accNationWarImmediateRevivalCount
	{
		get
		{
			return m_nAccNationWarImmediateRevivalCount;
		}
		set
		{
			m_nAccNationWarImmediateRevivalCount = value;
		}
	}

	public long maxGold
	{
		get
		{
			return m_lnMaxGold;
		}
		set
		{
			m_lnMaxGold = value;
		}
	}

	public long maxBattlePower
	{
		get
		{
			return m_lnMaxBattlePower;
		}
		set
		{
			m_lnMaxBattlePower = value;
		}
	}

	public int maxAcquisitionMainGearGrade
	{
		get
		{
			return m_nMaxAcquisitionMainGearGrade;
		}
		set
		{
			m_nMaxAcquisitionMainGearGrade = value;
		}
	}

	public int maxEquippedMainGearEnchantLevel
	{
		get
		{
			return m_nMaxEquippedMainGearEnchantLevel;
		}
		set
		{
			m_nMaxEquippedMainGearEnchantLevel = value;
		}
	}

	public HeroTitle displayTitle => m_displayTitle;

	public int displayTitleId
	{
		get
		{
			if (m_displayTitle == null)
			{
				return 0;
			}
			return m_displayTitle.title.id;
		}
	}

	public HeroTitle activationTitle => m_activationTitle;

	public int activationTitleId
	{
		get
		{
			if (m_activationTitle == null)
			{
				return 0;
			}
			return m_activationTitle.title.id;
		}
	}

	public Dictionary<int, HeroCreatureCard> creatureCards => m_creatureCards;

	public Dictionary<int, HeroCreatureCardCollection> activatedCreatureCardCollections => m_activatedCreatureCardCollections;

	public int creatureCardCollectionFamePoint => m_nCreatureCardCollectionFamePoint;

	public DateTimeOffset creatureCardCollectionFamePointUpdateTime => m_creatureCardCollectionFamePointUpdateTime;

	public HashSet<int> purchasedCreatureCardShopFixedProducts => m_purchasedCreatureCardShopFixedProducts;

	public Dictionary<int, HeroCreatureCardShopRandomProduct> creatureCardShopRandomProducts => m_creatureCardShopRandomProducts;

	public DateTime creatureCardShopRefreshDate => m_creatureCardShopRefreshDate;

	public int creatureCardShopRefreshScheduleId => m_nCreatureCardShopRefreshScheduleId;

	public Guid creatureCardShopId => m_creatureCardShopId;

	public DateValuePair<int> dailyCreatureCardShopPaidRefreshCount => m_dailyCreatureCardShopPaidRefreshCount;

	public DateValuePair<int> dailyEliteDungeonPlayCount => m_dailyEliteDungeonPlayCount;

	public LootingItemMinGrade lootingItemMinGrade => m_lootingItemMinGrade;

	public bool proofOfValorCleared => m_bProofOfValorCleared;

	public DateValuePair<int> dailyProofOfValorPlayCount => m_dailyProofOfValorPlayCount;

	public HeroProofOfValorInstance heroProofOfValorInst => m_heroProofOfValorInst;

	public DateValuePair<int> dailyProofOfValorFreeRefreshCount => m_dailyProofOfValorFreeRefreshCount;

	public DateValuePair<int> dailyProofOfValorPaidRefreshCount => m_dailyProofOfvalorPaidRefreshCount;

	public int proofOfValorPaidRefreshCount
	{
		get
		{
			return m_nProofOfValorPaidRefreshCount;
		}
		set
		{
			m_nProofOfValorPaidRefreshCount = value;
		}
	}

	public DateTime proofOfValorAutoRefreshDate
	{
		get
		{
			return m_proofOfValorAutoRefreshDate;
		}
		set
		{
			m_proofOfValorAutoRefreshDate = value;
		}
	}

	public int proofOfValorAutoRefreshScheduleId
	{
		get
		{
			return m_nProofOfValorAutoRefreshScheduleId;
		}
		set
		{
			m_nProofOfValorAutoRefreshScheduleId = value;
		}
	}

	public HeroProofOfValorBuff proofOfValorBuff => m_proofOfValorBuff;

	public int customPresetHair
	{
		get
		{
			return m_nCustomPresetHair;
		}
		set
		{
			m_nCustomPresetHair = value;
		}
	}

	public int customFaceJawHeight
	{
		get
		{
			return m_nCustomFaceJawHeight;
		}
		set
		{
			m_nCustomFaceJawHeight = value;
		}
	}

	public int customFaceJawWidth
	{
		get
		{
			return m_nCustomFaceJawWidth;
		}
		set
		{
			m_nCustomFaceJawWidth = value;
		}
	}

	public int customFaceJawEndHeight
	{
		get
		{
			return m_nCustomFaceJawEndHeight;
		}
		set
		{
			m_nCustomFaceJawEndHeight = value;
		}
	}

	public int customFaceWidth
	{
		get
		{
			return m_nCustomFaceWidth;
		}
		set
		{
			m_nCustomFaceWidth = value;
		}
	}

	public int customFaceEyebrowHeight
	{
		get
		{
			return m_nCustomFaceEyebrowHeight;
		}
		set
		{
			m_nCustomFaceEyebrowHeight = value;
		}
	}

	public int customFaceEyebrowRotation
	{
		get
		{
			return m_nCustomFaceEyebrowRotation;
		}
		set
		{
			m_nCustomFaceEyebrowRotation = value;
		}
	}

	public int customFaceEyesWidth
	{
		get
		{
			return m_nCustomFaceEyesWidth;
		}
		set
		{
			m_nCustomFaceEyesWidth = value;
		}
	}

	public int customFaceNoseHeight
	{
		get
		{
			return m_nCustomFaceNoseHeight;
		}
		set
		{
			m_nCustomFaceNoseHeight = value;
		}
	}

	public int customFaceNoseWidth
	{
		get
		{
			return m_nCustomFaceNoseWidth;
		}
		set
		{
			m_nCustomFaceNoseWidth = value;
		}
	}

	public int customFaceMouthHeight
	{
		get
		{
			return m_nCustomFaceMouthHeight;
		}
		set
		{
			m_nCustomFaceMouthHeight = value;
		}
	}

	public int customFaceMouthWidth
	{
		get
		{
			return m_nCustomFaceMouthWidth;
		}
		set
		{
			m_nCustomFaceMouthWidth = value;
		}
	}

	public int customBodyHeadSize
	{
		get
		{
			return m_nCustomBodyHeadSize;
		}
		set
		{
			m_nCustomBodyHeadSize = value;
		}
	}

	public int customBodyArmsLength
	{
		get
		{
			return m_nCustomBodyArmsLength;
		}
		set
		{
			m_nCustomBodyArmsLength = value;
		}
	}

	public int customBodyArmsWidth
	{
		get
		{
			return m_nCustomBodyArmsWidth;
		}
		set
		{
			m_nCustomBodyArmsWidth = value;
		}
	}

	public int customBodyChestSize
	{
		get
		{
			return m_nCustomBodyChestSize;
		}
		set
		{
			m_nCustomBodyChestSize = value;
		}
	}

	public int customBodyWaistWidth
	{
		get
		{
			return m_nCustomBodyWaistWidth;
		}
		set
		{
			m_nCustomBodyWaistWidth = value;
		}
	}

	public int customBodyHipsSize
	{
		get
		{
			return m_nCustomBodyHipsSize;
		}
		set
		{
			m_nCustomBodyHipsSize = value;
		}
	}

	public int customBodyPelvisWidth
	{
		get
		{
			return m_nCustomBodyPelvisWidth;
		}
		set
		{
			m_nCustomBodyPelvisWidth = value;
		}
	}

	public int customBodyLegsLength
	{
		get
		{
			return m_nCustomBodyLegsLength;
		}
		set
		{
			m_nCustomBodyLegsLength = value;
		}
	}

	public int customBodyLegsWidth
	{
		get
		{
			return m_nCustomBodyLegsWidth;
		}
		set
		{
			m_nCustomBodyLegsWidth = value;
		}
	}

	public int customColorSkin
	{
		get
		{
			return m_nCustomColorSkin;
		}
		set
		{
			m_nCustomColorSkin = value;
		}
	}

	public int customColorEyes
	{
		get
		{
			return m_nCustomColorEyes;
		}
		set
		{
			m_nCustomColorEyes = value;
		}
	}

	public int customColorBeardAndEyebrow
	{
		get
		{
			return m_nCustomColorBeardAndEyebrow;
		}
		set
		{
			m_nCustomColorBeardAndEyebrow = value;
		}
	}

	public int customColorHair
	{
		get
		{
			return m_nCustomColorHair;
		}
		set
		{
			m_nCustomColorHair = value;
		}
	}

	public bool isMonsterTaming
	{
		get
		{
			if (m_nTamingMonsterId == 0)
			{
				return false;
			}
			return true;
		}
	}

	public int tamingMonsterId => m_nTamingMonsterId;

	public bool todayMissionTutorialStarted
	{
		get
		{
			return m_bTodayMissionTutorialStarted;
		}
		set
		{
			m_bTodayMissionTutorialStarted = value;
		}
	}

	public Dictionary<int, HeroNpcShopProduct> npcShopProducts => m_npcShopProducts;

	public HeroRankActiveSkill selectedRankActiveSkill
	{
		get
		{
			return m_selectedRankActiveSkill;
		}
		set
		{
			m_selectedRankActiveSkill = value;
		}
	}

	public int selectedRankActiveSkillId
	{
		get
		{
			if (m_selectedRankActiveSkill == null)
			{
				return 0;
			}
			return m_selectedRankActiveSkill.skillId;
		}
	}

	public DateTimeOffset rankActiveSkillCastingTime
	{
		get
		{
			return m_rankActiveSkillCastingTime;
		}
		set
		{
			m_rankActiveSkillCastingTime = value;
		}
	}

	public int spiritStone => m_nSpiritStone;

	public int rookieGiftNo
	{
		get
		{
			return m_nRookieGiftNo;
		}
		set
		{
			m_nRookieGiftNo = value;
		}
	}

	public float rookieGiftLoginDuration
	{
		get
		{
			return m_fRookieGiftLoginDuration;
		}
		set
		{
			m_fRookieGiftLoginDuration = value;
		}
	}

	public DateTimeOffset rookieGitfLoginStartTime
	{
		get
		{
			return m_rookieGitfLoginStartTime;
		}
		set
		{
			m_rookieGitfLoginStartTime = value;
		}
	}

	public HashSet<int> receivedOpenGiftRewards => m_receivedOpenGiftRewards;

	public DateValuePair<int> dailyQuestFreeRefreshCount => m_dailyQuestFreeRefreshCount;

	public DateValuePair<int> dailyQuestAcceptionCount => m_dailyQuestAcceptionCount;

	public HeroDailyQuest[] dailyQuests => m_dailyQuests;

	public HeroWeeklyQuest weeklyQuest => m_weeklyQuest;

	public bool wisdomTempleCleared
	{
		get
		{
			return m_bWisdomTempleCleared;
		}
		set
		{
			m_bWisdomTempleCleared = value;
		}
	}

	public DateValuePair<int> dailyWisdomTemplePlayCount => m_dailyWisdomTemplePlayCount;

	public WisdomTempleObjectInstance currentInteractionWisdomTempleObjectInst => m_currentInteractionWisdomTempleObjectInst;

	public HashSet<int> rewardedOpen7DayEventMissions => m_rewardedOpen7DayEventMissions;

	public HashSet<int> purchasedOpen7DayEventProducts => m_purchasedOpen7DayEventProducts;

	public Dictionary<int, HeroOpen7DayEventProgressCount> open7DayEventProgressCounts => m_open7DayEventProgressCounts;

	public bool isOpen7DayEventCompleted
	{
		get
		{
			foreach (Open7DayEventMission mission in Resource.instance.open7DayEventMissions.Values)
			{
				if (!IsRewardedOpen7DayEventMission(mission.id))
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool open7DayEventRewarded
	{
		get
		{
			return m_bOpen7DayEventRewarded;
		}
		set
		{
			m_bOpen7DayEventRewarded = value;
		}
	}

	public DateTime retrievalDate => m_retrievalDate;

	public Dictionary<int, HeroRetrieval> retrievals => m_retrievals;

	public DateValuePair<int> dailyRuinsReclaimFreePlayCount => m_dailyRuinsReclaimFreePlayCount;

	public bool isTransformRuinsReclaimMonster => m_ruinsReclaimTransformationMonsterEffect != null;

	public bool ruinsReclaimDebuffEffect => m_bRuinsReclaimDebuffEffect;

	public Dictionary<Guid, HeroTaskConsignment> taskConsignments => m_taskConsignments;

	public Dictionary<int, HeroTaskConsignmentStartCount> taskConsignmentStartCounts => m_taskConsignmentStartCounts;

	public HeroTrueHeroQuest trueHeroQuest => m_trueHeroQuest;

	public Timer trueHeroQuestInteractionTimer => m_trueHeroQuestInteractionTimer;

	public bool isTrueHeroQuestWaitingStep
	{
		get
		{
			if (m_trueHeroQuest != null)
			{
				return m_trueHeroQuest.waitingTimer != null;
			}
			return false;
		}
	}

	public DateValuePair<int> dailyInfiniteWarPlayCount => m_dailyInfiniteWarPlayCount;

	public HeroInfiniteWarBuff infiniteWarBuff => m_infiniteWarBuff;

	public HashSet<int> rewardedLimitationGiftScheduleIds => m_rewardedLimitationGiftScheduleIds;

	public HeroWeekendReward weekendReward
	{
		get
		{
			return m_weekendReward;
		}
		set
		{
			m_weekendReward = value;
		}
	}

	public int paidWarehouseSlotCount
	{
		get
		{
			return m_nPaidWarehouseSlotCount;
		}
		set
		{
			m_nPaidWarehouseSlotCount = value;
		}
	}

	public int emptyWarehouseSlotCount
	{
		get
		{
			int nCount = 0;
			foreach (WarehouseSlot slot in m_warehouseSlots)
			{
				if (slot.isEmpty)
				{
					nCount++;
				}
			}
			return nCount;
		}
	}

	public DateTime dailyDiaShopProductBuyCountsDate => m_dailyDiaShopProductBuyCountsDate;

	public Dictionary<int, HeroDiaShopProductBuyCount> dailyDiaShopProductBuyCounts => m_dailyDiaShopProductBuyCounts;

	public Dictionary<int, HeroDiaShopProductBuyCount> totalDiaShopProductBuyCounts => m_totalDiaShopProductBuyCounts;

	public DateValuePair<int> dailyFearAltarPlayCount => m_dailyFearAltarPlayCount;

	public DateValuePair<int> weeklyFearAltarHalidomCollectionRewardNo => m_weeklyFearAltarHalidomCollectionRewardNo;

	public int fearAltarHalidomCount => m_fearAltarHalidoms.Count;

	public Dictionary<int, HeroSubQuest> subQuests => m_subQuests;

	public Dictionary<int, HeroSubQuest> currentSubQuests => m_currentSubQuests;

	public DateValuePair<int> dailyWarMemoryFreePlayCount => m_dailyWarMemoryFreePlayCount;

	public int warMemoryStartPositionIndex
	{
		get
		{
			return m_nWarMemoryStartPositionIndex;
		}
		set
		{
			m_nWarMemoryStartPositionIndex = value;
		}
	}

	public bool isTransformWarMemoryMonster => m_warMemoryTransformationMonsterEffect != null;

	public HeroOrdealQuest ordealQuest
	{
		get
		{
			return m_ordealQuest;
		}
		set
		{
			m_ordealQuest = value;
		}
	}

	public DateValuePair<int> dailyOsirisRoomPlayCount => m_dailyOsirisRoomPlayCount;

	public Dictionary<Guid, Friend> friends => m_friends;

	public bool isFriendListFull => m_friends.Count >= Resource.instance.friendMaxCount;

	public Dictionary<Guid, BlacklistEntry> blacklistEntries => m_blacklistEntries;

	public bool isBlacklistFull => m_blacklistEntries.Count >= Resource.instance.blacklistEntryMaxCount;

	public Dictionary<int, HeroBiography> biographies => m_biographies;

	public bool isTransformMonster
	{
		get
		{
			if (!isMonsterTaming && !isTransformRuinsReclaimMonster && !isTransformWarMemoryMonster)
			{
				return isTransformMainQuestMonster;
			}
			return true;
		}
	}

	public DateTime itemLuckyShopPickDate => m_itemLuckyShopPickDate;

	public DateTimeOffset itemLuckyShopFreePickTime
	{
		get
		{
			return m_itemLuckyShopFreePickTime;
		}
		set
		{
			m_itemLuckyShopFreePickTime = value;
		}
	}

	public int itemLuckyShopFreePickCount
	{
		get
		{
			return m_nItemLuckyShopFreePickCount;
		}
		set
		{
			m_nItemLuckyShopFreePickCount = value;
		}
	}

	public int itemLuckyShopPick1TimeCount
	{
		get
		{
			return m_nItemLuckyShopPick1TimeCount;
		}
		set
		{
			m_nItemLuckyShopPick1TimeCount = value;
		}
	}

	public int itemLuckyShopPick5TimeCount
	{
		get
		{
			return m_nItemLuckyShopPick5TimeCount;
		}
		set
		{
			m_nItemLuckyShopPick5TimeCount = value;
		}
	}

	public DateTime creatureCardLuckyShopPickDate => m_creatureCardLuckyShopPickDate;

	public DateTimeOffset creatureCardLuckyShopFreePickTime
	{
		get
		{
			return m_creatureCardLuckyShopFreePickTime;
		}
		set
		{
			m_creatureCardLuckyShopFreePickTime = value;
		}
	}

	public int creatureCardLuckyShopFreePickCount
	{
		get
		{
			return m_nCreatureCardLuckyShopFreePickCount;
		}
		set
		{
			m_nCreatureCardLuckyShopFreePickCount = value;
		}
	}

	public int creatureCardLuckyShopPick1TimeCount
	{
		get
		{
			return m_nCreatureCardLuckyShopPick1TimeCount;
		}
		set
		{
			m_nCreatureCardLuckyShopPick1TimeCount = value;
		}
	}

	public int creatureCardLuckyShopPick5TimeCount
	{
		get
		{
			return m_nCreatureCardLuckyShopPick5TimeCount;
		}
		set
		{
			m_nCreatureCardLuckyShopPick5TimeCount = value;
		}
	}

	public bool isBlessingQuestListFull => m_blessingQuests.Count >= Resource.instance.blessingQuestListMaxCount;

	public bool isReceivedBlessingListFull => m_receivedBlessings.Count >= Resource.instance.blessingListMaxCount;

	public Dictionary<Guid, HeroProspectQuest> ownerProspectQuests => m_ownerProspectQuests;

	public bool isOwnerProspectQuestListFull => m_ownerProspectQuests.Count >= Resource.instance.ownerProspectQuestListMaxCount;

	public Dictionary<Guid, HeroProspectQuest> targetProspectQuests => m_targetProspectQuests;

	public bool isTargetProspectQuestListFull => m_targetProspectQuests.Count >= Resource.instance.targetProspectQuestListMaxCount;

	public Dictionary<Guid, HeroCreature> creatures => m_creatures;

	public int creatureCount => m_creatures.Count;

	public int cheeredCreatureCount
	{
		get
		{
			int nCheeredCount = 0;
			foreach (HeroCreature creature in m_creatures.Values)
			{
				if (creature.cheered)
				{
					nCheeredCount++;
				}
			}
			return nCheeredCount;
		}
	}

	public HeroCreature participationCreature
	{
		get
		{
			return m_participationCreature;
		}
		set
		{
			m_participationCreature = value;
		}
	}

	public Guid participationCreatureId
	{
		get
		{
			if (m_participationCreature == null)
			{
				return Guid.Empty;
			}
			return m_participationCreature.instanceId;
		}
	}

	public DateValuePair<int> dailyCreatureVariationCount => m_dailyCreatureVariationCount;

	public int maxCreatureLevel
	{
		get
		{
			int nMaxLevel = 0;
			foreach (HeroCreature creature in m_creatures.Values)
			{
				if (nMaxLevel < creature.level)
				{
					nMaxLevel = creature.level;
				}
			}
			return nMaxLevel;
		}
	}

	public bool isDragonNestTrapEffect => m_dragonNestTrapEffect != null;

	public bool isPlayingCartQuest
	{
		get
		{
			if ((m_currentHeroMainQuest == null || m_currentHeroMainQuest.mainQuest.type != 6) && m_supplySupportQuest == null)
			{
				return m_guildSupplySupportQuestPlay != null;
			}
			return true;
		}
	}

	public DateTime weeklyPresentPopularityPointStartDate => m_weeklyPresentPopularityPointStartDate;

	public int weeklyPresentPopularityPoint => m_nWeeklyPresentPopularityPoint;

	public DateTimeOffset weeklyPresentPopularityPointUpdateTime => m_weeklyPresentPopularityPointUpdateTime;

	public int rewardedNationWeeklyPresentPopularityPointRankingNo
	{
		get
		{
			return m_nRewardedNationWeeklyPresentPopularityPointRankingNo;
		}
		set
		{
			m_nRewardedNationWeeklyPresentPopularityPointRankingNo = value;
		}
	}

	public DateTime weeklyPresentContributionPointStartDate => m_weeklyPresentContributionPointStartDate;

	public int weeklyPresentContributionPoint => m_nWeeklyPresentContributionPoint;

	public DateTimeOffset weeklyPresentContributionPointUpdateTime => m_weeklyPresentContributionPointUpdateTime;

	public int rewardedNationWeeklyPresentContributionPointRankingNo
	{
		get
		{
			return m_nRewardedNationWeeklyPresentContributionPointRankingNo;
		}
		set
		{
			m_nRewardedNationWeeklyPresentContributionPointRankingNo = value;
		}
	}

	public HeroCostume equippedCostume => m_equippedCostume;

	public int equippedCostumeId
	{
		get
		{
			if (m_equippedCostume == null)
			{
				return 0;
			}
			return m_equippedCostume.costume.id;
		}
	}

	public CostumeCollection costumeCollection
	{
		get
		{
			return m_costumeCollection;
		}
		set
		{
			m_costumeCollection = value;
		}
	}

	public int costumeCollectionId
	{
		get
		{
			if (m_costumeCollection == null)
			{
				return 0;
			}
			return m_costumeCollection.id;
		}
	}

	public bool costumeCollectionActivated
	{
		get
		{
			return m_bCostumeCollectionActivated;
		}
		set
		{
			m_bCostumeCollectionActivated = value;
		}
	}

	public HeroCreatureFarmQuest creatureFarmQuest
	{
		get
		{
			return m_creatureFarmQuest;
		}
		set
		{
			m_creatureFarmQuest = value;
		}
	}

	public DateValuePair<int> dailyCreatureFarmAcceptionCount => m_dailyCreatureFarmAcceptionCount;

	public bool autoHunting => m_bAutoHunting;

	public bool isSafeMode => m_bIsSafeMode;

	public HeroJobChangeQuest jobChangeQuest
	{
		get
		{
			return m_jobChangeQuest;
		}
		set
		{
			m_jobChangeQuest = value;
		}
	}

	public Dictionary<int, HeroPotionAttr> potionAttrs => m_potionAttrs;

	public DateValuePair<int> dailyAnkouTombPlayCount => m_dailyAnkouTombPlayCount;

	public Dictionary<int, HeroConstellation> constellations => m_constellations;

	public int starEssense => m_nStarEssense;

	public DateValuePair<int> dailyStarEssensItemUseCount => m_dailyStarEssensItemUseCount;

	public int artifactNo => m_nArtifactNo;

	public int artifactLevel => m_nArtifactLevel;

	public int artifactExp => m_nArtifactExp;

	public int equippedArtifactNo => m_nEquippedArtifactNo;

	public bool isArtifactOpened => m_nArtifactNo > 0;

	public bool isArtifactLast => m_nArtifactNo >= Resource.instance.maxArtifactNo;

	public bool isArtifactLevelMax => m_nArtifactLevel >= Resource.instance.artifactMaxLevel;

	public DateValuePair<int> dailyTradeShipPlayCount => m_dailyTradeShipPlayCount;

	public Hero(Account account, Guid id, DateTimeOffset currentTime, Hero controller)
	{
		if (account == null)
		{
			throw new ArgumentNullException("account");
		}
		m_account = account;
		m_id = id;
		m_controller = controller;
		if (m_controller == null)
		{
			m_controller = this;
		}
		Resource res = Resource.instance;
		for (int i = 0; i < m_mountGearSlots.Length; i++)
		{
			m_mountGearSlots[i] = new HeroMountGearSlot(this, i);
		}
		foreach (WingPart part in Resource.instance.wingParts.Values)
		{
			HeroWingPart heroWingPart = new HeroWingPart(this, part);
			m_wingParts.Add(heroWingPart.part.id, heroWingPart);
		}
		foreach (JobCommonSkill skill in res.jobCommonSkills)
		{
			HeroJobCommonSkill heroSkill = new HeroJobCommonSkill(this, skill);
			m_jobCommonSkills.Add(heroSkill.skill.skillId, heroSkill);
		}
		m_dailyQuests = new HeroDailyQuest[Resource.instance.dailyQuest.slotCount];
		InitUnit(currentTime);
		m_updateTimer = new Timer(OnUpdateTimerTick);
		m_updateTimer.Change(500, 500);
	}

	public void BeginLogIn()
	{
		if (m_status != 0)
		{
			throw new InvalidOperationException("영웅 상태가 유효하지 않습니다..");
		}
		m_status = HeroStatus.LoggingIn;
	}

	public void CompleteLogIn(DateTimeOffset currentTime, DataRow drHero, DataRowCollection drcHeroMainGears, DataRowCollection drcHeroMainGearOptionAttrs, DataRowCollection drcHeroMainGearRefinements, DataRowCollection drcHeroSubGears, DataRowCollection drcHeroSubGearSoulstoneSockets, DataRowCollection drcHeroSubGearRuneSockets, DataRowCollection drcInventorySlots, DataRowCollection drcMails, DataRowCollection drcMailAttachments, DataRowCollection drcHeroSkills, DataRow drCurrentHeroMainQuest, DataRow drTreatOfFarmQuest, DataRowCollection drcTreatOfFarmMissions, DataRow drBountyHunterQuest, int nBountyHunterQuestDailyCount, DataRowCollection drcHeroLevelUpRewards, DataRowCollection drcHeroAccessRewards, DataRowCollection drcHeroSeriesMissions, DataRowCollection drcHeroTodayMissions, DataRowCollection drcHeroMainQuestDungeonRewards, DataRowCollection drcHeroMounts, DataRowCollection drcHeroMountGears, DataRowCollection drcHeroMountGearOptionAttrs, DataRowCollection drcEquippedMountGearSlots, DataRowCollection drcHeroWings, DataRowCollection drcHeroWingEnchants, DataRowCollection drcHeroWingMemoryPieceSlots, DataRowCollection drcStoryDungeonClears, DataRowCollection drcStoryDungeonEnterCountsOfDate, DataRowCollection drcExpDungeonClearDifficulties, DataRow drExpDungeonEnterCountOfDate, DataRowCollection drcGoldDungeonClearDifficulties, DataRow drGoldDungeonEnterCountOfDate, int nMysteryBoxQuestStartCount, DataRow drPerformingMysteryBoxQuest, int nSecretLetterQuestStartCount, DataRow drPerformingSecretLetterQuest, int nDimensionRaidQuestStartCount, DataRow drPerformingDimensionRaidQuest, DataRowCollection drcStartedHolyWarQuests, DataRow drPerformingHolyWarQuest, DataRowCollection drcHeroTodayTasks, DataRow drAncientRelicEnterCountOfDate, DataRowCollection drcFieldOfHonorTargets, DataRow drFieldOfHonorEnterCountOfDate, DataRowCollection drcFieldOfHonorHistories, int nDailyGuildApplicationCount, int nDailyGuildFarmQuestStartCount, DataRow drPerformingGuildFarmQuest, DataRowCollection drcGuildSkillLevels, DataRow drHeroGuildMissionQuest, DataRowCollection drcHeroGuildMissionQuestMissions, DataRow drHeroGuildHuntingQuest, int nDailyGuildHuntingQuestCount, int nSupplySupportQuestStartCount, DataRow drSupplySupportQuest, DataRowCollection drcSupplySupportQuestVisitedWayPoint, DataRow drNationWarMember, DataRow drSoulCoveterEnterCountOfWeekly, DataRowCollection drcHeroIllustratedBooks, DataRowCollection drcHeroSceneryQuests, int nAccSoulCoveterPlayCount, DataRowCollection drcHeroAccomplishmentRewards, DataRowCollection drcHeroTitles, DataRowCollection drcHeroCreatureCards, DataRowCollection drcHeroCreatureCardCollections, DataRowCollection drcHeroCreatureCardFixedProductBuys, DataRowCollection drcHeroCreatureCardRandomProducts, DataRowCollection drcHeroEliteMonsterKills, DataRow drEliteDungeonEnterCountOfDate, int nProofOfValorClaeredCount, DataRow drProofOfValorEnterCountOfDate, DataRow drLastHeroProofOfValorInstance, DataRowCollection drcHeroNpcShopProducts, DataRowCollection drcHeroRankActiveSkills, DataRowCollection drcHeroRankPassiveSkills, DataRowCollection drcHeroOpenGiftRewards, int nDailyQuestAcceptionCount, DataRowCollection drcHeroDailyQuests, DataRow drWeeklyQuest, DataRow drWisdomTempleEnterCountOfDate, DataRowCollection drcOpen7DayEventMissions, DataRowCollection drcOpen7DayEventProducts, DataRowCollection drcOpen7DayEventProgressCounts, DataRowCollection drcPrevRetrievalProgressCounts, DataRowCollection drcCurrRetrievalProgressCounts, DataRowCollection drcRetrievals, DataRowCollection drcCurrentTaskConsignments, DataRowCollection drcTaskConsignmentCountsOfDate, DataRow drTrueHeroQuest, DataRowCollection drcLimitationGiftRewards, DataRow drWeekendReward, DataRowCollection drcWarehouseSlots, DataRowCollection drcDiaShopProducts, DataRowCollection m_drcDiaShopProductsAccBuyCounts, DataRowCollection drcHeroFearAltarHalidoms, DataRowCollection drcHeroFearAltarHalidomElementalRewards, DataRowCollection drcSubQuests, DataRow drOrdealQuest, DataRowCollection drcOrdealQuestMissions, DataRowCollection drcFriends, DataRowCollection drcTempFriends, DataRowCollection drcBlacklistEntries, DataRowCollection drcDeadRecords, DataRowCollection drcBiographies, DataRowCollection drcBiographyQuests, DataRowCollection drcOwnerProspectQuests, DataRowCollection drcTargetProspectQuests, DataRowCollection drcCreatures, DataRowCollection drcCreatureBaseAttrs, DataRowCollection drcCreatureAdditionalAttrs, DataRowCollection drcCreatureSkills, DataRow drWeeklyPresentPopularityPoint, DataRow drWeeklyPresentContributionPoint, DataRowCollection drcHeroCostumes, DataRow drCreatureFarmQuest, int nCreatureFarmQuestAcceptionCount, DataRow drJobChangeQuest, DataRowCollection drcPotionAttrs, DataRowCollection drcHeroAnkouTombBestRecords, DataRowCollection drcConstellation, DataRowCollection drcHeroTradeShipBestRecords, DataRowCollection drcTimeDesignationEvents)
	{
		if (!isLoggingIn)
		{
			throw new InvalidOperationException("영웅 상태가 로그인중이 아닙니다.");
		}
		DateTime currentDate = currentTime.Date;
		DateTime yesterDay = currentDate.AddDays(-1.0);
		Resource res = Resource.instance;
		m_lastLoginTime = currentTime;
		m_account.RefreshDailyChargeEvent(currentDate);
		m_account.RefreshDailyConsumeEvent(currentDate);
		CompleteLogIn_Base(drHero, currentTime);
		CompleteLogIn_HeroMainGears(currentTime, drcHeroMainGears, drcHeroMainGearOptionAttrs, drcHeroMainGearRefinements);
		Guid equippedMainGearWeaponId = (Guid)drHero["weaponHeroMainGearId"];
		Guid equippedMainGearArmorId = (Guid)drHero["armorHeroMainGearId"];
		m_equippedWeapon = GetMainGear(equippedMainGearWeaponId);
		m_equippedArmor = GetMainGear(equippedMainGearArmorId);
		CompleteLogIn_HeroSubGears(drcHeroSubGears, drcHeroSubGearSoulstoneSockets, drcHeroSubGearRuneSockets);
		CompleteLogIn_Mount(drcHeroMounts, drcHeroMountGears, drcHeroMountGearOptionAttrs, drcEquippedMountGearSlots);
		int nEquippedMountId = SFDBUtil.ToInt32(drHero["equippedMountId"]);
		m_equippedMount = GetMount(nEquippedMountId);
		if (m_bIsRiding)
		{
			GetOnMount();
		}
		JobLevelMaster levelMaster = m_job.GetLevel(m_nLevel).master;
		CreateInventorySlots(levelMaster.inventorySlotAccCount + m_nPaidInventorySlotCount);
		foreach (DataRow dr8 in drcInventorySlots)
		{
			int nSlotIndex = SFDBUtil.ToInt32(dr8["slotIndex"]);
			InventorySlot slot = GetInventorySlot(nSlotIndex);
			if (slot == null)
			{
				throw new Exception("존재하지않는 인벤토리 슬롯입니다. nSlotIndex = " + nSlotIndex);
			}
			int nType = SFDBUtil.ToInt32(dr8["slotType"]);
			switch (nType)
			{
			case 1:
			{
				Guid herMainGearId = (Guid)dr8["heroMainGearId"];
				HeroMainGear mainGear = GetMainGear(herMainGearId);
				if (mainGear == null)
				{
					throw new Exception("존재하지 않는 메인장비 입니다. herMainGearId = " + herMainGearId);
				}
				slot.Place(mainGear);
				break;
			}
			case 2:
			{
				int nSubGearId = SFDBUtil.ToInt32(dr8["subGearId"]);
				HeroSubGear subGear = GetSubGear(nSubGearId);
				if (subGear == null)
				{
					throw new Exception("존재하지 않는 보조장비 입니다. nSubGearId = " + nSubGearId);
				}
				slot.Place(subGear);
				break;
			}
			case 3:
			{
				int nItemId = SFDBUtil.ToInt32(dr8["itemId"]);
				bool bOwned = SFDBUtil.ToBoolean(dr8["itemOwned"]);
				int nCount2 = SFDBUtil.ToInt32(dr8["itemCount"]);
				Item item = Resource.instance.GetItem(nItemId);
				HeroInventoryItem heroInventoryItem = GetOrCreateInventoryItem(item);
				ItemInventoryObject itemInventoryObject = new ItemInventoryObject(heroInventoryItem, bOwned, nCount2);
				GetInventorySlot(nSlotIndex).Place(itemInventoryObject);
				heroInventoryItem.AddObject(itemInventoryObject);
				break;
			}
			case 4:
			{
				Guid heroMountGearId = (Guid)dr8["heroMountGearId"];
				HeroMountGear heroMountGear = GetMountGear(heroMountGearId);
				if (heroMountGear == null)
				{
					throw new Exception("존재하지 않는 탈것장비 입니다. heroMountGearId = " + heroMountGearId);
				}
				slot.Place(heroMountGear);
				break;
			}
			default:
				throw new Exception("유효하지않은 인벤토리 슬롯 타입입니다. nType = " + nType);
			}
		}
		CompleteLogIn_Mail(drcMails, drcMailAttachments, currentTime);
		foreach (DataRow dr22 in drcHeroSkills)
		{
			int nSkillId3 = SFDBUtil.ToInt32(dr22["skillId"]);
			JobSkill skill3 = m_job.GetSkill(nSkillId3);
			if (skill3 == null)
			{
				throw new Exception("해당 스킬이 존재하지 않습니다. nSkillId = " + nSkillId3);
			}
			HeroSkill heroSkill3 = new HeroSkill(this, skill3);
			heroSkill3.Init(dr22);
			AddSkill(heroSkill3);
		}
		if (drCurrentHeroMainQuest != null)
		{
			HeroMainQuest heroMainQuest = new HeroMainQuest(this);
			heroMainQuest.Init(drCurrentHeroMainQuest);
			m_currentHeroMainQuest = heroMainQuest;
			if (heroMainQuest.mainQuest.type == 6 && !heroMainQuest.completed)
			{
				MainQuestCartInstance cartInst = new MainQuestCartInstance();
				lock (cartInst.syncObject)
				{
					cartInst.Init(heroMainQuest, currentTime);
					if (heroMainQuest.isCartRiding)
					{
						cartInst.isRiding = true;
						m_ridingCartInst = cartInst;
					}
				}
			}
		}
		foreach (DataRow dr21 in drcHeroMainQuestDungeonRewards)
		{
			HeroMainQuestDungeonReward heroMainQuestDungeonReward = new HeroMainQuestDungeonReward();
			heroMainQuestDungeonReward.Init(dr21);
			AddHeroMainQuestDungeonReward(heroMainQuestDungeonReward);
		}
		if (m_lastLogoutTime.HasValue)
		{
			int nRestTime = (int)(currentTime - m_lastLogoutTime.Value).TotalMinutes;
			if (nRestTime > m_nRestTime)
			{
				m_nRestTime = nRestTime;
			}
		}
		foreach (DataRow dr20 in drcHeroLevelUpRewards)
		{
			int nEntryId = SFDBUtil.ToInt32(dr20["entryId"]);
			m_receivedLevelUpRewards.Add(nEntryId);
		}
		foreach (DataRow dr19 in drcHeroAccessRewards)
		{
			HeroAccessReward reward = new HeroAccessReward(this);
			reward.Init(dr19);
			m_accessRewards.Add(reward.id, reward);
		}
		foreach (SeriesMission mission2 in Resource.instance.seriesMissions.Values)
		{
			HeroSeriesMission heroMission2 = new HeroSeriesMission(this, mission2, 1);
			AddSeriesMission(heroMission2);
		}
		foreach (DataRow dr18 in drcHeroSeriesMissions)
		{
			int nMissionId = SFDBUtil.ToInt32(dr18["missionId"]);
			int nCurrentStep = SFDBUtil.ToInt32(dr18["currentStep"]);
			int nProgressCount = SFDBUtil.ToInt32(dr18["progressCount"]);
			HeroSeriesMission heroMission = GetSeriesMission(nMissionId);
			if (nCurrentStep <= heroMission.mission.lastStep.step)
			{
				heroMission.currentStep = nCurrentStep;
				heroMission.progressCount = nProgressCount;
			}
			else
			{
				RemoveSeriesMission(nMissionId);
			}
		}
		m_todayMissionCollection = new HeroTodayMissionCollection(this, currentDate);
		foreach (DataRow dr17 in drcHeroTodayMissions)
		{
			HeroTodayMission mission = new HeroTodayMission(m_todayMissionCollection);
			mission.Init(dr17);
			m_todayMissionCollection.AddMission(mission);
		}
		if (m_todayMissionCollection.missions.Count == 0)
		{
			m_todayMissionCollection.InitializeMissions();
		}
		foreach (DataRow dr16 in drcHeroWings)
		{
			HeroWing wing = new HeroWing(this);
			wing.Init(dr16);
			AddWing(wing);
		}
		int nEquippedWingId = SFDBUtil.ToInt32(drHero["equippedWingId"]);
		if (nEquippedWingId > 0)
		{
			m_equippedWing = GetWing(nEquippedWingId);
		}
		foreach (DataRow dr15 in drcHeroWingEnchants)
		{
			int nPartId = SFDBUtil.ToInt32(dr15["partId"]);
			HeroWingPart heroWingPart = GetWingPart(nPartId);
			HeroWingEnchant wingEnchant = new HeroWingEnchant(heroWingPart);
			wingEnchant.Init(dr15);
			heroWingPart.AddEnchant(wingEnchant);
		}
		foreach (HeroWingPart wingPart in m_wingParts.Values)
		{
			wingPart.RefreshAttrTotalValues();
		}
		CompleteLogin_WingMemoryPieceSlots(drcHeroWingMemoryPieceSlots);
		foreach (DataRow dr14 in drcStoryDungeonClears)
		{
			int nDungeon = SFDBUtil.ToInt32(dr14["dungeonNo"]);
			int nMaxDifficulty = SFDBUtil.ToInt32(dr14["maxDifficulty"]);
			m_storyDungeonClearMaxDifficulties.Add(nDungeon, nMaxDifficulty);
		}
		foreach (DataRow dr13 in drcStoryDungeonEnterCountsOfDate)
		{
			DateTime date = SFDBUtil.ToDateTime(dr13["date"]);
			int nDungeonNo = SFDBUtil.ToInt32(dr13["dungeonNo"]);
			int nCount = SFDBUtil.ToInt32(dr13["cnt"]);
			m_storyDungeonPlays.Add(new StoryDungeonPlay(nDungeonNo, date, nCount));
		}
		foreach (DataRow dr12 in drcExpDungeonClearDifficulties)
		{
			int nDifficulty2 = SFDBUtil.ToInt32(dr12["difficulty"]);
			m_expDungeonClearDifficulties.Add(nDifficulty2);
		}
		if (drExpDungeonEnterCountOfDate != null)
		{
			m_dailyExpDungeonPlayCount.date = SFDBUtil.ToDateTime(drExpDungeonEnterCountOfDate["date"]);
			m_dailyExpDungeonPlayCount.value = SFDBUtil.ToInt32(drExpDungeonEnterCountOfDate["cnt"]);
		}
		foreach (DataRow dr11 in drcGoldDungeonClearDifficulties)
		{
			int nDifficulty = SFDBUtil.ToInt32(dr11["difficulty"]);
			m_goldDungeonClearDifficulties.Add(nDifficulty);
		}
		if (drGoldDungeonEnterCountOfDate != null)
		{
			m_dailyGoldDungeonPlayCount.date = SFDBUtil.ToDateTime(drGoldDungeonEnterCountOfDate["date"]);
			m_dailyGoldDungeonPlayCount.value = SFDBUtil.ToInt32(drGoldDungeonEnterCountOfDate["cnt"]);
		}
		if (drAncientRelicEnterCountOfDate != null)
		{
			m_dailyAncientRelicPlayCount.date = SFDBUtil.ToDateTime(drAncientRelicEnterCountOfDate["date"]);
			m_dailyAncientRelicPlayCount.value = SFDBUtil.ToInt32(drAncientRelicEnterCountOfDate["cnt"]);
		}
		foreach (DataRow dr10 in drcFieldOfHonorTargets)
		{
			int nRanking = SFDBUtil.ToInt32(dr10["ranking"]);
			m_fieldOfHonorTargets.Add(nRanking);
		}
		if (drFieldOfHonorEnterCountOfDate != null)
		{
			m_dailyFieldOfHonorPlayCount.date = SFDBUtil.ToDateTime(drFieldOfHonorEnterCountOfDate["date"]);
			m_dailyFieldOfHonorPlayCount.value = SFDBUtil.ToInt32(drFieldOfHonorEnterCountOfDate["cnt"]);
		}
		foreach (DataRow dr9 in drcFieldOfHonorHistories)
		{
			FieldOfHonorHistory history = new FieldOfHonorHistory(this);
			history.Init(dr9);
			m_fieldOfHonorHistories.Add(history);
		}
		CompleteLogIn_Stamina(drHero, currentTime, currentDate);
		CompleteLogIn_TreatOfFarmQuest(currentTime, drTreatOfFarmQuest, drcTreatOfFarmMissions);
		CopmlteLogIn_BountyHunterQuest(currentDate, drBountyHunterQuest, nBountyHunterQuestDailyCount);
		CompleteLogIn_MysteryBoxQuest(currentDate, nMysteryBoxQuestStartCount, drPerformingMysteryBoxQuest);
		CompleteLogIn_SecretLetterQuest(currentDate, nSecretLetterQuestStartCount, drPerformingSecretLetterQuest);
		CompleteLogIn_DimensionRaidQuest(currentDate, nDimensionRaidQuestStartCount, drPerformingDimensionRaidQuest);
		CompleteLogIn_HolyWarQuest(currentDate, drcStartedHolyWarQuests, drPerformingHolyWarQuest);
		CompleteLogin_SupplySupportQuest(currentTime, nSupplySupportQuestStartCount, drSupplySupportQuest, drcSupplySupportQuestVisitedWayPoint);
		CompleteLogin_TodayTask(currentDate, drcHeroTodayTasks);
		CompleteLogin_Guild(currentDate, drHero);
		CompleteLogin_GuildApplication(currentDate, nDailyGuildApplicationCount);
		CompleteLogin_GuildSkills(drcGuildSkillLevels);
		CompleteLogin_GuildFarmQuest(currentDate, nDailyGuildFarmQuestStartCount, drPerformingGuildFarmQuest);
		CompleteLogin_GuildFoodWarehouse(currentDate, drHero);
		CompleteLogin_GuildAltar(currentDate, drHero);
		CompleteLogin_GuildMissionQuest(currentTime, currentDate, drHeroGuildMissionQuest, drcHeroGuildMissionQuestMissions);
		CompleteLogin_GuildSupplySupportQuest(currentTime);
		CompleteLogin_GuildHuntingQuest(drHero, drHeroGuildHuntingQuest, nDailyGuildHuntingQuestCount, currentDate, currentTime);
		CompleteLogin_GuildEvent(drHero, currentDate);
		CompleteLogIn_Friend(drcFriends);
		CompleteLogIn_TempFriend(drcTempFriends);
		CompleteLogIn_Blacklist(drcBlacklistEntries);
		CompleteLogIn_DeadRecord(drcDeadRecords);
		CompleteLogIn_Party();
		NationNoblesseInstance noblesseInst = m_nationInst.GetNoblesseInstanceByHeroId(m_id);
		if (noblesseInst != null)
		{
			m_nationNoblesse = Resource.instance.GetNationNoblesse(noblesseInst.id);
		}
		if (drNationWarMember != null)
		{
			NationWarManager nationWarManager = Cache.instance.nationWarManager;
			NationWarDeclaration declaration = null;
			NationWarInstance nationWarInst = m_nationInst.nationWarInst;
			if (nationWarInst != null)
			{
				declaration = nationWarInst.declaration;
			}
			Guid declarationId = SFDBUtil.ToGuid(drNationWarMember["declarationId"], Guid.Empty);
			if (declaration != null)
			{
				if (declaration.id == declarationId)
				{
					NationWarMember nationWarMember = new NationWarMember(declaration, m_id, m_sName);
					nationWarMember.Init(drNationWarMember);
					m_nationWarMember = nationWarMember;
				}
			}
			else
			{
				NationWarResult result = nationWarManager.GetNationWarResult(declarationId);
				if (result != null)
				{
					m_nationWarMember = result.GetNationWarMember(m_id);
				}
			}
		}
		if (drSoulCoveterEnterCountOfWeekly != null)
		{
			m_weeklySoulCoveterPlayCount.date = SFDBUtil.ToDateTime(drSoulCoveterEnterCountOfWeekly["dateOfMonday"], DateTime.MinValue.Date);
			m_weeklySoulCoveterPlayCount.value = SFDBUtil.ToInt32(drSoulCoveterEnterCountOfWeekly["cnt"]);
		}
		foreach (DataRow dr7 in drcHeroIllustratedBooks)
		{
			HeroIllustratedBook heroIllustratedBook = new HeroIllustratedBook(this);
			heroIllustratedBook.Init(dr7);
			AddIllustratedBook(heroIllustratedBook);
		}
		foreach (DataRow dr6 in drcHeroSceneryQuests)
		{
			int nQuestId = SFDBUtil.ToInt32(dr6["questId"]);
			AddSceneryQuestCompletion(nQuestId);
		}
		CompleteLogin_Accomplishment(drcHeroAccomplishmentRewards, drHero, nAccSoulCoveterPlayCount);
		CompleteLogin_Title(drcHeroTitles, currentTime, drHero);
		CompleteLogin_CreatureCard(drcHeroCreatureCards, drcHeroCreatureCardCollections, drHero);
		CompleteLogin_CreatureCardShop(drcHeroCreatureCardFixedProductBuys, drcHeroCreatureCardRandomProducts, drHero, currentTime);
		foreach (DataRow dr5 in drcHeroEliteMonsterKills)
		{
			HeroEliteMonsterKill heroEliteMonsterKill = new HeroEliteMonsterKill(this);
			heroEliteMonsterKill.Init(dr5);
			AddHeroEliteMonsterKill(heroEliteMonsterKill);
		}
		if (drEliteDungeonEnterCountOfDate != null)
		{
			m_dailyEliteDungeonPlayCount.date = SFDBUtil.ToDateTime(drEliteDungeonEnterCountOfDate["date"], DateTime.MinValue);
			m_dailyEliteDungeonPlayCount.value = SFDBUtil.ToInt32(drEliteDungeonEnterCountOfDate["cnt"]);
		}
		CompleteLogin_ProofOfValor(nProofOfValorClaeredCount, drProofOfValorEnterCountOfDate, drLastHeroProofOfValorInstance, currentTime);
		foreach (DataRow dr4 in drcHeroNpcShopProducts)
		{
			HeroNpcShopProduct product = new HeroNpcShopProduct(this);
			product.Init(dr4);
			AddNpcShopProduct(product);
		}
		foreach (DataRow dr3 in drcHeroRankActiveSkills)
		{
			int nSkillId2 = SFDBUtil.ToInt32(dr3["skillId"]);
			RankActiveSkill skill2 = res.GetRankActiveSkill(nSkillId2);
			if (skill2 == null)
			{
				throw new Exception("해당 스킬이 존재하지 않습니다. nSkillId = " + nSkillId2);
			}
			HeroRankActiveSkill heroSkill2 = new HeroRankActiveSkill(this, skill2);
			heroSkill2.Init(dr3);
			AddRankActiveSkill(heroSkill2);
		}
		int nSelectedRankActiveSkillId = SFDBUtil.ToInt32(drHero["selectedRankActiveSkillId"]);
		if (nSelectedRankActiveSkillId > 0)
		{
			m_selectedRankActiveSkill = GetRankActiveSkill(nSelectedRankActiveSkillId);
			if (m_selectedRankActiveSkill == null)
			{
				throw new Exception("선택된계급액티브스킬ID가 유효하지 않습니다. nSelectedRankActiveSkillId = " + nSelectedRankActiveSkillId);
			}
		}
		foreach (DataRow dr2 in drcHeroRankPassiveSkills)
		{
			int nSkillId = SFDBUtil.ToInt32(dr2["skillId"]);
			RankPassiveSkill skill = res.GetRankPassiveSkill(nSkillId);
			if (skill == null)
			{
				throw new Exception("해당 스킬이 존재하지 않습니다. nSkillId = " + nSkillId);
			}
			HeroRankPassiveSkill heroSkill = new HeroRankPassiveSkill(this, skill);
			heroSkill.Init(dr2);
			AddRankPassiveSkill(heroSkill);
		}
		CompleteLogin_RookieGift(drHero, currentTime);
		foreach (DataRow dr in drcHeroOpenGiftRewards)
		{
			int nDay = SFDBUtil.ToInt32(dr["day"]);
			AddReceivedOpenGiftReward(nDay);
		}
		CompleteLogin_DailyQuest(drHero, currentDate, nDailyQuestAcceptionCount, drcHeroDailyQuests);
		CompleteLogin_WeeklyQuest(drWeeklyQuest, currentTime);
		if (drWisdomTempleEnterCountOfDate != null)
		{
			m_dailyWisdomTemplePlayCount.date = SFDBUtil.ToDateTime(drWisdomTempleEnterCountOfDate["date"], DateTime.MinValue);
			m_dailyWisdomTemplePlayCount.value = SFDBUtil.ToInt32(drWisdomTempleEnterCountOfDate["cnt"]);
		}
		CompleteLogin_Open7DayEvent(drcOpen7DayEventMissions, drcOpen7DayEventProducts, drcOpen7DayEventProgressCounts);
		CompleteLogin_Retrieval(yesterDay, currentDate, drcPrevRetrievalProgressCounts, drcCurrRetrievalProgressCounts, drcRetrievals);
		CompleteLogin_Consignments(drcCurrentTaskConsignments, drcTaskConsignmentCountsOfDate, currentDate);
		CompleteLogin_TrueHeroQuest(drTrueHeroQuest);
		CompleteLogin_LimitationGift(currentDate, drcLimitationGiftRewards);
		CompleteLogin_WeekendReward(drWeekendReward, currentDate);
		CompleteLogin_Warehouse(drcWarehouseSlots);
		CompleteLogin_DiaShopProducts(currentDate, drcDiaShopProducts, m_drcDiaShopProductsAccBuyCounts);
		CompleteLogin_FearAltar(currentTime.Date, drcHeroFearAltarHalidoms, drcHeroFearAltarHalidomElementalRewards);
		CompleteLogin_SubQuest(drcSubQuests);
		CompleteLogin_OrdealQuest(drOrdealQuest, drcOrdealQuestMissions);
		CompleteLogin_Biography(drcBiographies, drcBiographyQuests);
		CompleteLogin_ProspectQuest(drcOwnerProspectQuests, drcTargetProspectQuests, currentTime);
		CompleteLogin_Creatures(drcCreatures, drcCreatureBaseAttrs, drcCreatureAdditionalAttrs, drcCreatureSkills, drHero);
		CompleteLogIn_Present(drHero, drWeeklyPresentPopularityPoint, drWeeklyPresentContributionPoint, currentTime);
		CompleteLogIn_Costume(drHero, drcHeroCostumes, currentTime);
		CompleteLogin_CreatureFarmQuest(currentTime, currentDate, nCreatureFarmQuestAcceptionCount, drCreatureFarmQuest);
		CompleteLogin_JobChangeQuest(drJobChangeQuest, currentTime);
		CompleteLogin_PotionAttr(drcPotionAttrs);
		CompleteLogin_AnkouTomb(drcHeroAnkouTombBestRecords);
		ComleteLogin_Constellation(drcConstellation);
		CompleteLogIn_Artifact(drHero);
		CompleteLogin_TradeShip(drcHeroTradeShipBestRecords);
		CompelteLogin_GM(currentTime);
		CompleteLogin_TimeDesignationEvent(drcTimeDesignationEvents, currentTime);
		RefreshRealValues(bSendMaxHpChangedToOthers: false);
		RefreshMoveSpeed();
		m_account.SetLastHeroId(m_id);
		m_nationInst.AddHero(this);
		m_status = HeroStatus.LoggedIn;
	}

	private void CompleteLogIn_Base(DataRow drHero, DateTimeOffset currentTime)
	{
		DateTime currentDate = currentTime.Date;
		m_nationInst = Cache.instance.GetNationInstance(SFDBUtil.ToInt32(drHero["nationId"]));
		m_sName = SFDBUtil.ToString(drHero["name"]);
		m_job = Resource.instance.GetJob(SFDBUtil.ToInt32(drHero["jobId"]));
		m_nLevel = SFDBUtil.ToInt32(drHero["level"]);
		m_lnExp = SFDBUtil.ToInt64(drHero["exp"]);
		m_levelUpdateTime = (DateTimeOffset)drHero["levelUpdateTime"];
		m_lnBattlePower = SFDBUtil.ToInt64(drHero["battlePower"]);
		m_battlePowerUpdateTime = (DateTimeOffset)drHero["battlePowerUpdateTime"];
		m_lastLocation = Resource.instance.GetLocation(SFDBUtil.ToInt32(drHero["lastLocationId"]));
		m_nLastLocationParam = SFDBUtil.ToInt32(drHero["lastLocationParam"]);
		m_lastInstanceId = SFDBUtil.ToGuid(drHero["lastInstanceId"]);
		m_lastPosition.x = SFDBUtil.ToSingle(drHero["lastXPosition"]);
		m_lastPosition.y = SFDBUtil.ToSingle(drHero["lastYPosition"]);
		m_lastPosition.z = SFDBUtil.ToSingle(drHero["lastZPosition"]);
		m_fLastYRotation = SFDBUtil.ToSingle(drHero["lastYRotation"]);
		m_previousNation = Resource.instance.GetNation(SFDBUtil.ToInt32(drHero["previousNationId"]));
		m_previousContinent = Resource.instance.GetContinent(SFDBUtil.ToInt32(drHero["previousContinentId"]));
		m_previousPosition.x = SFDBUtil.ToSingle(drHero["previousXPosition"]);
		m_previousPosition.y = SFDBUtil.ToSingle(drHero["previousYPosition"]);
		m_previousPosition.z = SFDBUtil.ToSingle(drHero["previousZPosition"]);
		m_fPreviousYRotation = SFDBUtil.ToSingle(drHero["previousYRotation"]);
		m_nHP = SFDBUtil.ToInt32(drHero["hp"]);
		m_nPaidInventorySlotCount = SFDBUtil.ToInt32(drHero["paidInventorySlotCount"]);
		m_nOwnDia = SFDBUtil.ToInt32(drHero["ownDia"]);
		m_lnGold = SFDBUtil.ToInt64(drHero["gold"]);
		m_nHonorPoint = SFDBUtil.ToInt32(drHero["honorPoint"]);
		m_nSoulPowder = SFDBUtil.ToInt32(drHero["soulPowder"]);
		m_nVipPoint = SFDBUtil.ToInt32(drHero["vipPoint"]);
		m_vipLevel = Resource.instance.GetVipLevelByPoint(totalVipPoint);
		m_mainGearEnchantDailyCount.date = SFDBUtil.ToDateTime(drHero["mainGearEnchantDate"], DateTime.MinValue.Date);
		m_mainGearEnchantDailyCount.value = SFDBUtil.ToInt32(drHero["mainGearEnchantCount"]);
		RefreshMainGearEnchantDailyCount(currentDate);
		m_mainGearRefinementDailyCount.date = SFDBUtil.ToDateTime(drHero["mainGearRefinementDate"], DateTime.MinValue.Date);
		m_mainGearRefinementDailyCount.value = SFDBUtil.ToInt32(drHero["mainGearRefinementCount"]);
		RefreshMainGearRefinementDailyCount(currentDate);
		m_nLak = SFDBUtil.ToInt32(drHero["lak"]);
		m_freeImmediateRevivalDailyCount.date = SFDBUtil.ToDateTime(drHero["freeImmediateRevivalDate"], DateTime.MinValue.Date);
		m_freeImmediateRevivalDailyCount.value = SFDBUtil.ToInt32(drHero["freeImmediateRevivalCount"]);
		RefreshFreeImmediateRevivalDailyCount(currentDate);
		m_paidImmediateRevivalDailyCount.date = SFDBUtil.ToDateTime(drHero["paidImmediateRevivalDate"], DateTime.MinValue.Date);
		m_paidImmediateRevivalDailyCount.value = SFDBUtil.ToInt32(drHero["paidImmediateRevivalCount"]);
		RefreshPaidImmediateRevivalDailyCount(currentDate);
		m_lastLogoutTime = SFDBUtil.ToNullableDateTimeOffset(drHero["lastLogoutTime"]);
		m_nRestTime = SFDBUtil.ToInt32(drHero["restTime"]);
		m_dailyAttendRewardDay.date = SFDBUtil.ToDateTime(drHero["dailyAttendRewardDate"], DateTime.MinValue.Date);
		m_dailyAttendRewardDay.value = SFDBUtil.ToInt32(drHero["dailyAttendRewardDay"]);
		m_dailyAccessTimeUpdateTime = SFDBUtil.ToDateTimeOffset(drHero["dailyAccessTimeUpdateTime"], DateTimeOffset.MinValue);
		m_fDailyAccessTime = SFDBUtil.ToSingle(drHero["dailyAccessTime"]);
		if (m_dailyAccessTimeUpdateTime.Date != currentTime.Date)
		{
			m_fDailyAccessTime = 0f;
			m_dailyAccessTimeUpdateTime = currentTime;
		}
		m_expPotionDailyUseCount.date = SFDBUtil.ToDateTime(drHero["expPotionUseDate"], DateTime.MinValue.Date);
		m_expPotionDailyUseCount.value = SFDBUtil.ToInt32(drHero["expPotionUseCount"]);
		RefreshExpPotionDailyUseCount(currentDate);
		m_mountGearRefinementDailyCount.date = SFDBUtil.ToDateTime(drHero["mountGearRefinementDate"], DateTime.MinValue.Date);
		m_mountGearRefinementDailyCount.value = SFDBUtil.ToInt32(drHero["mountGearRefinementCount"]);
		RefreshMountGearDailyRefinementCount(currentDate);
		int nWingStep = SFDBUtil.ToInt32(drHero["wingStep"]);
		int nWingLevel = SFDBUtil.ToInt32(drHero["wingLevel"]);
		m_wingStepLevel = Resource.instance.GetWingStep(nWingStep).GetLevel(nWingLevel);
		m_nWingExp = SFDBUtil.ToInt32(drHero["wingExp"]);
		m_freeSweepDailyCount.date = SFDBUtil.ToDateTime(drHero["freeSweepDate"], DateTime.MinValue.Date);
		m_freeSweepDailyCount.value = SFDBUtil.ToInt32(drHero["freeSweepCount"]);
		RefreshFreeSweepDailyCount(currentDate);
		m_bIsRiding = SFDBUtil.ToBoolean(drHero["isRiding"]);
		int nMainGearEnchantLevelSetNo = SFDBUtil.ToInt32(drHero["mainGearEnchantLevelSetNo"]);
		m_mainGearEnchantLevelSet = Resource.instance.GetMainGearEnchantLevelSet(nMainGearEnchantLevelSetNo);
		int nSubGearSoulstoneLevelSetNo = SFDBUtil.ToInt32(drHero["subGearSoulstoneLevelSetNo"]);
		m_subGearSoulstoneLevelSet = Resource.instance.GetSubGearSoulstoneLevelSet(nSubGearSoulstoneLevelSetNo);
		m_expScrollDailyUseCount.date = SFDBUtil.ToDateTime(drHero["expScrollUseDate"], DateTime.MinValue.Date);
		m_expScrollDailyUseCount.value = SFDBUtil.ToInt32(drHero["expScrollUseCount"]);
		RefreshExpScrollDailyUseCount(currentDate);
		m_expScrollStartTime = SFDBUtil.ToNullableDateTimeOffset(drHero["expScrollStartTime"]);
		m_nExpScrollDuration = SFDBUtil.ToInt32(drHero["expScrollDuration"]);
		int nExpScrollItemId = SFDBUtil.ToInt32(drHero["expScrollItemId"]);
		m_expScrollItem = Resource.instance.GetItem(nExpScrollItemId);
		m_undergroundMazeDate = SFDBUtil.ToDateTime(drHero["undergroundMazeDate"], DateTime.MinValue.Date);
		m_fUndergroundMazePlayTime = SFDBUtil.ToInt32(drHero["undergroundMazePlayTIme"]);
		RefreshUndergroundMazeDailyPlayTime(currentDate);
		m_nArtifactRoomBestFloor = SFDBUtil.ToInt32(drHero["artifactRoomBestFloor"]);
		m_nArtifactRoomCurrentFloor = SFDBUtil.ToInt32(drHero["artifactRoomCurrentFloor"]);
		if (m_nArtifactRoomCurrentFloor <= 0)
		{
			m_nArtifactRoomCurrentFloor = 1;
		}
		m_artifactRoomDailyInitCount.date = SFDBUtil.ToDateTime(drHero["artifactRoomInitDate"], DateTime.MaxValue.Date);
		m_artifactRoomDailyInitCount.value = SFDBUtil.ToInt32(drHero["artifactRoomInitCount"]);
		RefreshAartifactRoomDailyInitCount(currentDate);
		m_artifactRoomSweepStartTime = SFDBUtil.ToNullableDateTimeOffset(drHero["artifactRoomSweepStartTime"]);
		UpdateArtifactRoomSweepFloor(currentTime, bSendEvent: false);
		m_nExploitPoint = SFDBUtil.ToInt32(drHero["exploitPoint"]);
		m_exploitPointUpdateTime = (DateTimeOffset)drHero["exploitPointUpdateTime"];
		m_dailyExploitPoint.date = SFDBUtil.ToDateTime(drHero["dailyExploitPointDate"], DateTime.MinValue.Date);
		m_dailyExploitPoint.value = SFDBUtil.ToInt32(drHero["dailyExploitPoint"]);
		RefreshDailyExploitPoint(currentDate);
		int nRankNo = SFDBUtil.ToInt32(drHero["rankNo"]);
		if (nRankNo > 0)
		{
			m_rank = Resource.instance.GetRank(nRankNo);
			if (m_rank == null)
			{
				throw new Exception("계급이 존재하지 않습니다. nRankNo = " + nRankNo);
			}
		}
		m_nRankRewardReceivedRankNo = SFDBUtil.ToInt32(drHero["rankRewardReceivedRankNo"]);
		m_rankRewardReceivedDate = SFDBUtil.ToDateTime(drHero["rankRewardReceivedDate"], DateTime.MinValue.Date);
		m_fishingQuestDailyStartCount.date = SFDBUtil.ToDateTime(drHero["fishingQuestDate"], DateTime.MinValue.Date);
		m_fishingQuestDailyStartCount.value = SFDBUtil.ToInt32(drHero["fishingQuestCount"]);
		int nFishingQuestBaitItemId = SFDBUtil.ToInt32(drHero["fishingQuestBaitItemId"]);
		if (nFishingQuestBaitItemId > 0)
		{
			FishingQuestBait fishingQuestBait = Resource.instance.fishingQuest.GetBait(nFishingQuestBaitItemId);
			int nFishingQuestBaitCastingCount = SFDBUtil.ToInt32(drHero["fishingQuestCastingCount"]);
			m_fishingQuest = new HeroFishingQuest(this, m_fishingQuestDailyStartCount.date, m_fishingQuestDailyStartCount.value, fishingQuestBait, nFishingQuestBaitCastingCount);
		}
		RefreshFishingQuestDailyStartCount(currentDate);
		m_achivementDailyPoint.date = SFDBUtil.ToDateTime(drHero["achievementPointDate"], DateTime.MinValue.Date);
		m_achivementDailyPoint.value = SFDBUtil.ToInt32(drHero["achievementPoint"]);
		m_nReceivedAchivementRewardNo = SFDBUtil.ToInt32(drHero["achievementRewardNo"]);
		RefreshAchievementDailyPoint(currentDate);
		m_nRewardedDailyServerLevelRankingNo = SFDBUtil.ToInt32(drHero["rewardedDailyServerLevelRankingNo"]);
		m_distortionScrollDailyUseCount.date = SFDBUtil.ToDateTime(drHero["distortionScrollUseDate"], DateTime.MinValue.Date);
		m_distortionScrollDailyUseCount.value = SFDBUtil.ToInt32(drHero["distortionScrollUseCount"]);
		RefreshDistortionScrollDailyUseCount(currentDate);
		m_distortionScrollStartTime = SFDBUtil.ToNullableDateTimeOffset(drHero["distortionScrollStartTime"]);
		m_nDistortionScrollDuration = SFDBUtil.ToInt32(drHero["distortionScrollDuration"]);
		float fRemainingDistortionTime = GetRemainingDistortionTime(currentTime);
		if (fRemainingDistortionTime > 0f)
		{
			StartDistortionTimer((int)(fRemainingDistortionTime * 1000f));
		}
		m_bTodayMissionTutorialStarted = SFDBUtil.ToBoolean(drHero["todayMissionTutorialStarted"]);
		m_nRewardedAttainmentEntryNo = SFDBUtil.ToInt32(drHero["rewardedAttainmentEntryNo"]);
		m_nFieldOfHonorRanking = SFDBUtil.ToInt32(drHero["fieldOfHonorRanking"]);
		m_nFieldOfHonorSuccessiveCount = SFDBUtil.ToInt32(drHero["fieldOfHonorSuccessiveCount"]);
		m_nRewardedDailyFieldOfHonorRankingNo = SFDBUtil.ToInt32(drHero["rewardedDailyFieldOfHonorRankingNo"]);
		m_dailyNationDonationCount.date = SFDBUtil.ToDateTime(drHero["nationDonationDate"], DateTime.MinValue.Date);
		m_dailyNationDonationCount.value = SFDBUtil.ToInt32(drHero["nationDonationCount"]);
		RefreshDailyNationDonationCount(currentDate);
		m_dailyNationWarFreeTransmissionCount.date = SFDBUtil.ToDateTime(drHero["nationWarFreeTransmissionDate"], DateTime.MinValue);
		m_dailyNationWarFreeTransmissionCount.value = SFDBUtil.ToInt32(drHero["nationWarFreeTransmissionCount"]);
		RefreshDailyNationWarFreeTransmissionCount(currentDate);
		m_dailyNationWarPaidTransmissionCount.date = SFDBUtil.ToDateTime(drHero["nationWarPaidTransmissionDate"], DateTime.MinValue);
		m_dailyNationWarPaidTransmissionCount.value = SFDBUtil.ToInt32(drHero["nationWarPaidTransmissionCount"]);
		m_nExplorationPoint = SFDBUtil.ToInt32(drHero["explorationPoint"]);
		int nIllustratedBookExplorationStepNo = SFDBUtil.ToInt32(drHero["illustratedBookExplorationStepNo"]);
		if (nIllustratedBookExplorationStepNo > 0)
		{
			m_illustratedBookExplorationStep = Resource.instance.GetIllustratedBookExplorationStep(nIllustratedBookExplorationStepNo);
		}
		m_illustratedBookExplorationStepRewardReceivedDate = SFDBUtil.ToDateTime(drHero["illustratedBookExplorationStepRewardReceivedDate"], DateTime.MinValue);
		m_nIllustratedBookExplorationStepRewardReceivedStepNo = SFDBUtil.ToInt32(drHero["illustratedBookExplorationStepRewardReceivedStepNo"]);
		RefreshDailyNationWarPaidTransmissionCount(currentDate);
		int nLootingItemMinGrade = SFDBUtil.ToInt32(drHero["lootingItemMinGrade"]);
		if (!Enum.IsDefined(typeof(LootingItemMinGrade), nLootingItemMinGrade))
		{
			throw new Exception("루팅아이템최소등급이 유효하지 않습니다. nLootingItemMinGrade = " + nLootingItemMinGrade);
		}
		m_lootingItemMinGrade = (LootingItemMinGrade)nLootingItemMinGrade;
		m_dailyProofOfValorFreeRefreshCount.date = SFDBUtil.ToDateTime(drHero["proofOfValorFreeRefreshDate"], DateTime.MinValue);
		m_dailyProofOfValorFreeRefreshCount.value = SFDBUtil.ToInt32(drHero["proofOfValorFreeRefreshCount"]);
		m_dailyProofOfvalorPaidRefreshCount.date = SFDBUtil.ToDateTime(drHero["proofOfValorDailyPaidRefreshDate"]);
		m_dailyProofOfvalorPaidRefreshCount.value = SFDBUtil.ToInt32(drHero["proofOfValorDailyPaidRefreshCount"]);
		m_nProofOfValorPaidRefreshCount = SFDBUtil.ToInt32(drHero["proofOfValorPaidRefreshCount"]);
		m_proofOfValorAutoRefreshDate = SFDBUtil.ToDateTime(drHero["proofOfValorAutoRefreshDate"], DateTime.MinValue);
		m_nProofOfValorAutoRefreshScheduleId = SFDBUtil.ToInt32(drHero["proofOfValorAutoRefreshScheduleId"]);
		m_nCustomPresetHair = SFDBUtil.ToInt32(drHero["customPresetHair"]);
		m_nCustomFaceJawHeight = SFDBUtil.ToInt32(drHero["customFaceJawHeight"]);
		m_nCustomFaceJawWidth = SFDBUtil.ToInt32(drHero["customFaceJawWidth"]);
		m_nCustomFaceJawEndHeight = SFDBUtil.ToInt32(drHero["customFaceJawEndHeight"]);
		m_nCustomFaceWidth = SFDBUtil.ToInt32(drHero["customFaceWidth"]);
		m_nCustomFaceEyebrowHeight = SFDBUtil.ToInt32(drHero["customFaceEyebrowHeight"]);
		m_nCustomFaceEyebrowRotation = SFDBUtil.ToInt32(drHero["customFaceEyebrowRotation"]);
		m_nCustomFaceEyesWidth = SFDBUtil.ToInt32(drHero["customFaceEyesWidth"]);
		m_nCustomFaceNoseHeight = SFDBUtil.ToInt32(drHero["customFaceNoseHeight"]);
		m_nCustomFaceNoseWidth = SFDBUtil.ToInt32(drHero["customFaceNoseWidth"]);
		m_nCustomFaceMouthHeight = SFDBUtil.ToInt32(drHero["customFaceMouthHeight"]);
		m_nCustomFaceMouthWidth = SFDBUtil.ToInt32(drHero["customFaceMouthWidth"]);
		m_nCustomBodyHeadSize = SFDBUtil.ToInt32(drHero["customBodyHeadSize"]);
		m_nCustomBodyArmsLength = SFDBUtil.ToInt32(drHero["customBodyArmsLength"]);
		m_nCustomBodyArmsWidth = SFDBUtil.ToInt32(drHero["customBodyArmsWidth"]);
		m_nCustomBodyChestSize = SFDBUtil.ToInt32(drHero["customBodyChestSize"]);
		m_nCustomBodyWaistWidth = SFDBUtil.ToInt32(drHero["customBodyWaistWidth"]);
		m_nCustomBodyHipsSize = SFDBUtil.ToInt32(drHero["customBodyHipsSize"]);
		m_nCustomBodyPelvisWidth = SFDBUtil.ToInt32(drHero["customBodyPelvisWidth"]);
		m_nCustomBodyLegsLength = SFDBUtil.ToInt32(drHero["customBodyLegsLength"]);
		m_nCustomBodyLegsWidth = SFDBUtil.ToInt32(drHero["customBodyLegsWidth"]);
		m_nCustomColorSkin = SFDBUtil.ToInt32(drHero["customColorSkin"]);
		m_nCustomColorEyes = SFDBUtil.ToInt32(drHero["customColorEyes"]);
		m_nCustomColorBeardAndEyebrow = SFDBUtil.ToInt32(drHero["customColorBeardAndEyebrow"]);
		m_nCustomColorHair = SFDBUtil.ToInt32(drHero["customColorHair"]);
		m_rankActiveSkillCastingTime = SFDBUtil.ToDateTimeOffset(drHero["rankActiveSkillCastingTime"], DateTimeOffset.MinValue);
		m_nSpiritStone = SFDBUtil.ToInt32(drHero["spiritStone"]);
		m_bWisdomTempleCleared = SFDBUtil.ToBoolean(drHero["wisdomTempleCleared"]);
		m_dailyRuinsReclaimFreePlayCount.date = SFDBUtil.ToDateTime(drHero["ruinsReclaimFreePlayDate"], DateTime.MinValue);
		m_dailyRuinsReclaimFreePlayCount.value = SFDBUtil.ToInt32(drHero["ruinsReclaimFreePlayCount"]);
		m_dailyInfiniteWarPlayCount.date = SFDBUtil.ToDateTime(drHero["infiniteWarPlayDate"], DateTime.MinValue);
		m_dailyInfiniteWarPlayCount.value = SFDBUtil.ToInt32(drHero["infiniteWarPlayCount"]);
		m_regTime = SFDBUtil.ToDateTimeOffset(drHero["regTime"]);
		m_nPaidWarehouseSlotCount = SFDBUtil.ToInt32(drHero["paidWarehouseSlotCount"]);
		m_dailyFearAltarPlayCount.date = SFDBUtil.ToDateTime(drHero["fearAltarPlayDate"], DateTime.MinValue);
		m_dailyFearAltarPlayCount.value = SFDBUtil.ToInt32(drHero["fearAltarPlayCount"]);
		m_weeklyFearAltarHalidomCollectionRewardNo.date = SFDBUtil.ToDateTime(drHero["fearAltarHalidomCollectionRewardWeekStartDate"], DateTime.MinValue);
		m_weeklyFearAltarHalidomCollectionRewardNo.value = SFDBUtil.ToInt32(drHero["fearAltarHalidomCollectionRewardNo"]);
		m_dailyWarMemoryFreePlayCount.date = SFDBUtil.ToDateTime(drHero["warMemoryFreePlayDate"], DateTime.MinValue);
		m_dailyWarMemoryFreePlayCount.value = SFDBUtil.ToInt32(drHero["warMemoryFreePlayCount"]);
		m_dailyOsirisRoomPlayCount.date = SFDBUtil.ToDateTime(drHero["osirisRoomPlayDate"], DateTime.MinValue);
		m_dailyOsirisRoomPlayCount.value = SFDBUtil.ToInt32(drHero["osirisRoomPlayCount"]);
		m_itemLuckyShopPickDate = SFDBUtil.ToDateTime(drHero["itemLuckyShopPickDate"], DateTime.MinValue.Date);
		m_itemLuckyShopFreePickTime = SFDBUtil.ToDateTimeOffset(drHero["itemLuckyShopFreePickTime"], DateTimeOffset.MinValue);
		m_nItemLuckyShopFreePickCount = SFDBUtil.ToInt32(drHero["itemLuckyShopFreePickCount"]);
		m_nItemLuckyShopPick1TimeCount = SFDBUtil.ToInt32(drHero["itemLuckyShopPick1TimeCount"]);
		m_nItemLuckyShopPick5TimeCount = SFDBUtil.ToInt32(drHero["itemLuckyShopPick5TimeCount"]);
		m_creatureCardLuckyShopPickDate = SFDBUtil.ToDateTime(drHero["creatureCardLuckyShopPickDate"], DateTime.MinValue.Date);
		m_creatureCardLuckyShopFreePickTime = SFDBUtil.ToDateTimeOffset(drHero["creatureCardLuckyShopFreePickTime"], DateTimeOffset.MinValue);
		m_nCreatureCardLuckyShopFreePickCount = SFDBUtil.ToInt32(drHero["creatureCardLuckyShopFreePickCount"]);
		m_nCreatureCardLuckyShopPick1TimeCount = SFDBUtil.ToInt32(drHero["creatureCardLuckyShopPick1TimeCount"]);
		m_nCreatureCardLuckyShopPick5TimeCount = SFDBUtil.ToInt32(drHero["creatureCardLuckyShopPick5TimeCount"]);
		RefreshItemLuckyShopPickCount(currentDate);
		RefreshCreatureCardLuckyShopPickCount(currentDate);
		m_dailyCreatureVariationCount.date = SFDBUtil.ToDateTime(drHero["creatureVariationDate"], DateTime.MinValue.Date);
		m_dailyCreatureVariationCount.value = SFDBUtil.ToInt32(drHero["creatureVariationCount"]);
		RefreshDailyCreatureVariationCount(currentDate);
		m_bOpen7DayEventRewarded = SFDBUtil.ToBoolean(drHero["open7DayEventRewarded"]);
		m_dailyAnkouTombPlayCount.date = SFDBUtil.ToDateTime(drHero["ankouTombPlayDate"], DateTime.MinValue.Date);
		m_dailyAnkouTombPlayCount.value = SFDBUtil.ToInt32(drHero["ankouTombPlayCount"]);
		m_nStarEssense = SFDBUtil.ToInt32(drHero["starEssense"]);
		m_dailyStarEssensItemUseCount.date = SFDBUtil.ToDateTime(drHero["starEssenseItemUseDate"], DateTime.MinValue.Date);
		m_dailyStarEssensItemUseCount.value = SFDBUtil.ToInt32(drHero["starEssenseItemUseCount"]);
		int nCostumeCollectionId = SFDBUtil.ToInt32(drHero["costumeCollectionId"]);
		m_costumeCollection = Resource.instance.GetCostumeCollection(nCostumeCollectionId);
		m_bCostumeCollectionActivated = SFDBUtil.ToBoolean(drHero["costumeCollectionActivated"]);
		if (m_costumeCollection == null)
		{
			InitCostumeCollection();
		}
		m_dailyTradeShipPlayCount.date = SFDBUtil.ToDateTime(drHero["tradeShipPlayDate"], DateTime.MinValue.Date);
		m_dailyTradeShipPlayCount.value = SFDBUtil.ToInt32(drHero["tradeShipPlayCount"]);
		m_nGMTargetMainQuestNo = SFDBUtil.ToInt32(drHero["gmTargetMainQuestNo"]);
	}

	private void CompleteLogIn_HeroMainGears(DateTimeOffset time, DataRowCollection drcHeroMainGears, DataRowCollection drcHeroMainGearOptionAttrs, DataRowCollection drcHeroMainGearRefinements)
	{
		foreach (DataRow dr2 in drcHeroMainGears)
		{
			HeroMainGear mainGear2 = new HeroMainGear(this);
			mainGear2.Init(dr2);
			AddMainGear(mainGear2, bInit: true, time);
		}
		foreach (DataRow dr3 in drcHeroMainGearOptionAttrs)
		{
			HeroMainGearOptionAttr attr2 = new HeroMainGearOptionAttr();
			attr2.Init(dr3);
			Guid heroMainGearId2 = (Guid)dr3["heroMainGearId"];
			HeroMainGear heroMainGear2 = GetMainGear(heroMainGearId2);
			if (heroMainGear2 == null)
			{
				throw new Exception("존재하지 않는 영웅메인장비입니다. heroMainGearId = " + heroMainGearId2);
			}
			heroMainGear2.AddOptinAttr(attr2);
		}
		foreach (DataRow dr in drcHeroMainGearRefinements)
		{
			HeroMainGearRefinementAttr attr = new HeroMainGearRefinementAttr();
			attr.Init(dr);
			Guid heroMainGearId = (Guid)dr["heroMainGearId"];
			HeroMainGear heroMainGear = GetMainGear(heroMainGearId);
			if (heroMainGear == null)
			{
				throw new Exception("존재하지 않는 영웅메인장비입니다. heroMainGearId = " + heroMainGearId);
			}
			int nTurn = SFDBUtil.ToInt32(dr["turn"]);
			HeroMainGearRefinement refinement = heroMainGear.GetRefinement(nTurn);
			if (refinement == null)
			{
				refinement = new HeroMainGearRefinement(heroMainGear, nTurn);
				heroMainGear.AddRefinement(refinement);
			}
			refinement.AddAttr(attr);
		}
		foreach (HeroMainGear mainGear in m_mainGears.Values)
		{
			mainGear.RefreshAttrTotalValues();
		}
	}

	private void CompleteLogIn_HeroSubGears(DataRowCollection drcHeroSubGears, DataRowCollection drcHeroSubGearSoulstoneSockets, DataRowCollection drcHeroSubGearRuneSockets)
	{
		foreach (DataRow dr2 in drcHeroSubGears)
		{
			HeroSubGear subGear2 = new HeroSubGear(this);
			subGear2.Init(dr2);
			AddSubGear(subGear2);
		}
		foreach (DataRow dr3 in drcHeroSubGearSoulstoneSockets)
		{
			int nSubGearId2 = SFDBUtil.ToInt32(dr3["subGearId"]);
			HeroSubGear gear2 = GetSubGear(nSubGearId2);
			if (gear2 == null)
			{
				throw new Exception("보조장비가 존재하지 않습니다. nSubGearId = " + nSubGearId2);
			}
			int nSocketIndex2 = SFDBUtil.ToInt32(dr3["socketIndex"]);
			HeroSoulstoneSocket socket2 = gear2.GetSoulstoneSocket(nSocketIndex2);
			if (socket2 == null)
			{
				throw new Exception("소울스톤 소켓이 존재하지 않습니다. nSocketIndex = " + nSocketIndex2);
			}
			int nItemId2 = SFDBUtil.ToInt32(dr3["itemId"]);
			Item item2 = Resource.instance.GetItem(nItemId2);
			if (item2 == null)
			{
				throw new Exception("아이템이 존재하지 않습니다. nSocketIndex = " + nSocketIndex2 + ", itemId = " + nItemId2);
			}
			socket2.Mount(item2);
		}
		foreach (DataRow dr in drcHeroSubGearRuneSockets)
		{
			int nSubGearId = SFDBUtil.ToInt32(dr["subGearId"]);
			HeroSubGear gear = GetSubGear(nSubGearId);
			if (gear == null)
			{
				throw new Exception("보조장비가 존재하지 않습니다. nSubGearId = " + nSubGearId);
			}
			int nSocketIndex = SFDBUtil.ToInt32(dr["socketIndex"]);
			HeroRuneSocket socket = gear.GetRuneSocket(nSocketIndex);
			if (socket == null)
			{
				throw new Exception("룬 소켓이 존재하지 않습니다. nSocketIndex = " + nSocketIndex);
			}
			int nItemId = SFDBUtil.ToInt32(dr["itemId"]);
			Item item = Resource.instance.GetItem(nItemId);
			if (item == null)
			{
				throw new Exception("아이템이 존재하지 않습니다. nSocketIndex = " + nSocketIndex + ", itemId = " + nItemId);
			}
			socket.Mount(item);
		}
		foreach (HeroSubGear subGear in m_subGears.Values)
		{
			subGear.RefreshAttrTotalValues();
		}
	}

	private void CompleteLogIn_Mail(DataRowCollection drcMails, DataRowCollection drcMailAttachments, DateTimeOffset currentTime)
	{
		foreach (DataRow dr2 in drcMails)
		{
			Mail mail3 = new Mail();
			mail3.Init(dr2, currentTime);
			AddMail(mail3, bSendEvent: false);
		}
		foreach (DataRow dr in drcMailAttachments)
		{
			Guid mailId = (Guid)dr["mailId"];
			Mail mail2 = GetMail(mailId);
			MailAttachment attachment = new MailAttachment();
			attachment.Init(dr);
			mail2.AddAttachment(attachment);
		}
		foreach (Mail mail in m_deliveredMails.Values)
		{
			if (!ContainsMail(mail.id))
			{
				AddMail(mail, bSendEvent: false);
			}
		}
		m_deliveredMails.Clear();
		RefreshMailBox(currentTime);
	}

	private void CompleteLogIn_Party()
	{
		PartyMember partyMember = Cache.instance.GetPartyMember(m_id);
		if (partyMember != null)
		{
			m_partyMember = partyMember;
			m_partyMember.LogIn(this);
		}
	}

	private void CompleteLogIn_Mount(DataRowCollection drcHeroMounts, DataRowCollection drcHeroMountGears, DataRowCollection drcHeroMountGearOptionAttrs, DataRowCollection drcEquippedMountGearSlots)
	{
		foreach (DataRow dr2 in drcHeroMounts)
		{
			HeroMount mount = new HeroMount(this);
			mount.Init(dr2);
			AddMount(mount);
			mount.RefreshAttrTotalValues();
		}
		foreach (DataRow dr4 in drcHeroMountGears)
		{
			HeroMountGear mountGear3 = new HeroMountGear(this);
			mountGear3.Init(dr4);
			AddMountGear(mountGear3);
		}
		foreach (DataRow dr3 in drcHeroMountGearOptionAttrs)
		{
			HeroMountGearOptionAttr attr = new HeroMountGearOptionAttr();
			attr.Init(dr3);
			Guid gearId = (Guid)dr3["heroMountGearId"];
			HeroMountGear gear = GetMountGear(gearId);
			if (gear == null)
			{
				throw new Exception("존재하지 않는 탈것장비입니다. gearId = " + gearId);
			}
			gear.AddOptionAttr(attr);
		}
		foreach (HeroMountGear mountGear2 in m_mountGears.Values)
		{
			mountGear2.RefreshAttrTotalValues();
		}
		foreach (DataRow dr in drcEquippedMountGearSlots)
		{
			int nSlotIndex = SFDBUtil.ToInt32(dr["slotIndex"]);
			Guid heroMountGearId = (Guid)dr["heroMountGearId"];
			HeroMountGear mountGear = GetMountGear(heroMountGearId);
			if (mountGear == null)
			{
				throw new Exception(string.Concat("존재하지 않는 탈것장비입니다. heroId = ", m_id, ", heroMountGearId = ", heroMountGearId));
			}
			HeroMountGearSlot mountGearSlot = GetMountGearSlot(nSlotIndex);
			if (mountGearSlot == null)
			{
				throw new Exception("존재하지 않는 장착된탈것장비슬롯입니다.");
			}
			mountGearSlot.Equip(mountGear);
		}
	}

	private void CompleteLogIn_TreatOfFarmQuest(DateTimeOffset currentTime, DataRow drTreatOfFarmQuest, DataRowCollection drcTreatOfFarmQuestMissions)
	{
		if (drTreatOfFarmQuest == null)
		{
			return;
		}
		HeroTreatOfFarmQuest quest = new HeroTreatOfFarmQuest(this);
		quest.Init(drTreatOfFarmQuest);
		m_treatOfFarmQuest = quest;
		foreach (DataRow dr in drcTreatOfFarmQuestMissions)
		{
			if (SFDBUtil.ToInt32(dr["status"]) == 0)
			{
				HeroTreatOfFarmQuestMission newMission = new HeroTreatOfFarmQuestMission(m_treatOfFarmQuest);
				newMission.Init(dr);
				m_treatOfFarmQuest.currentMission = newMission;
			}
			else
			{
				m_treatOfFarmQuest.completedMissionCount++;
			}
		}
		if (m_treatOfFarmQuest.currentMission == null)
		{
			return;
		}
		HeroTreatOfFarmQuestMission missionInst = m_treatOfFarmQuest.currentMission;
		TreatOfFarmQuestMission mission = missionInst.mission;
		if (!missionInst.monsterSpawnTime.HasValue)
		{
			return;
		}
		Place place = null;
		place = ((!mission.targetContinent.isNationTerritory) ? ((ContinentInstance)Cache.instance.GetDisputeContinentInstance(mission.targetContinent.id)) : ((ContinentInstance)m_nationInst.GetContinentInstance(mission.targetContinent.id)));
		lock (place.syncObject)
		{
			foreach (MonsterInstance monsterInst in place.monsterInsts.Values)
			{
				if (monsterInst is TreatOfFarmQuestMonsterInstance monsterInstance && monsterInstance.questHeroId == m_id)
				{
					missionInst.targetMonsterInst = monsterInstance;
					break;
				}
			}
		}
		if (missionInst.targetMonsterInst == null)
		{
			m_treatOfFarmQuest.FailCurrentMission(currentTime, bSendEvent: false);
		}
	}

	private void CopmlteLogIn_BountyHunterQuest(DateTime currentDate, DataRow drBountyHunterQuest, int nBountyHunterQuestDailyStartCount)
	{
		if (drBountyHunterQuest != null)
		{
			HeroBountyHunterQuest quest = new HeroBountyHunterQuest(this);
			quest.Init(drBountyHunterQuest);
			m_bountyHunterQuest = quest;
		}
		m_bountyHunterQuestDailyStartCount.date = currentDate;
		m_bountyHunterQuestDailyStartCount.value = nBountyHunterQuestDailyStartCount;
	}

	private void CompleteLogIn_MysteryBoxQuest(DateTime currentDate, int nStartCount, DataRow drPerformingQuest)
	{
		m_dailyMysteryBoxQuestStartCount.date = currentDate;
		m_dailyMysteryBoxQuestStartCount.value = nStartCount;
		if (drPerformingQuest != null)
		{
			HeroMysteryBoxQuest quest = new HeroMysteryBoxQuest(this);
			quest.Init(drPerformingQuest);
			m_mysteryBoxQuest = quest;
		}
	}

	private void CompleteLogIn_SecretLetterQuest(DateTime currentDate, int nStartCount, DataRow drPerformingQuest)
	{
		m_dailySecretLetterQuestStartCount.date = currentDate;
		m_dailySecretLetterQuestStartCount.value = nStartCount;
		if (drPerformingQuest != null)
		{
			HeroSecretLetterQuest quest = new HeroSecretLetterQuest(this, 0);
			quest.Init(drPerformingQuest);
			m_secretLetterQuest = quest;
		}
	}

	private void CompleteLogIn_DimensionRaidQuest(DateTime currentDate, int nStartCount, DataRow drPerformingQuest)
	{
		m_dailyDimensionRaidQuestStartCount.date = currentDate;
		m_dailyDimensionRaidQuestStartCount.value = nStartCount;
		if (drPerformingQuest != null)
		{
			HeroDimensionRaidQuest quest = new HeroDimensionRaidQuest(this, 0);
			quest.Init(drPerformingQuest);
			m_dimensionRaidQuest = quest;
		}
	}

	private void CompleteLogIn_HolyWarQuest(DateTime currentDate, DataRowCollection drcStartedQuests, DataRow drPerformingQuest)
	{
		m_dailyHolyWarQuestStartScheduleCollection.date = currentDate;
		foreach (DataRow dr in drcStartedQuests)
		{
			int nScheduleId = SFDBUtil.ToInt32(dr["scheduleId"]);
			m_dailyHolyWarQuestStartScheduleCollection.AddSchedule(nScheduleId);
		}
		if (drPerformingQuest != null)
		{
			HeroHolyWarQuest quest = new HeroHolyWarQuest(this);
			quest.Init(drPerformingQuest);
			m_holyWarQuest = quest;
		}
	}

	private void CompleteLogin_SupplySupportQuest(DateTimeOffset time, int nStartCount, DataRow drSupplySupportQuest, DataRowCollection drcSupplySupportQuestVisitedWayPoint)
	{
		m_dailySupplySupportQuestStartCount.date = time.Date;
		m_dailySupplySupportQuestStartCount.value = nStartCount;
		if (drSupplySupportQuest != null)
		{
			HeroSupplySupportQuest heroQuest = new HeroSupplySupportQuest(this);
			heroQuest.Init(drSupplySupportQuest);
			foreach (DataRow dr in drcSupplySupportQuestVisitedWayPoint)
			{
				int nWayPoint = SFDBUtil.ToInt32(dr["wayPoint"]);
				heroQuest.AddVisitedWayPoint(nWayPoint);
			}
			m_supplySupportQuest = heroQuest;
			SupplySupportQuestCartInstance cartInst2 = new SupplySupportQuestCartInstance();
			lock (cartInst2.syncObject)
			{
				cartInst2.Init(heroQuest, time, bFirstCreation: false);
				if (heroQuest.isCartRiding)
				{
					cartInst2.isRiding = true;
					m_ridingCartInst = cartInst2;
				}
			}
		}
		if (m_supplySupportQuest == null)
		{
			return;
		}
		SupplySupportQuestCartInstance cartInst = m_supplySupportQuest.cartInst;
		lock (cartInst.syncObject)
		{
			if (supplySupportQuest.GetRemainingTime(time) <= 0f || cartInst.isDead)
			{
				supplySupportQuest.ProcessLifetimeEnd(time, bSendFailEvent: false);
			}
		}
	}

	private void CompleteLogin_TodayTask(DateTime currentDate, DataRowCollection drcHeroTodayTasks)
	{
		m_todayTaskCollection = new HeroTodayTaskCollection(this, currentDate);
		foreach (DataRow dr in drcHeroTodayTasks)
		{
			int nId = SFDBUtil.ToInt32(dr["taskId"]);
			int nProgressCount = SFDBUtil.ToInt32(dr["progressCount"]);
			HeroTodayTask todayTask = new HeroTodayTask(m_todayTaskCollection, Resource.instance.GetTodayTask(nId), nProgressCount);
			m_todayTaskCollection.AddTodayTask(todayTask);
		}
	}

	private void CompleteLogin_Guild(DateTime currentDate, DataRow drHero)
	{
		m_guildWithdrawalTime = SFDBUtil.ToDateTimeOffset(drHero["guildWithdrawalTime"], DateTimeOffset.MinValue);
		m_guildDailyRewardReceivedDate = SFDBUtil.ToDateTime(drHero["guildDailyRewardReceivedDate"], DateTime.MinValue.Date);
		m_dailyGuildDonationCount.date = SFDBUtil.ToDateTime(drHero["guildDonationDate"], DateTime.MinValue.Date);
		m_dailyGuildDonationCount.value = SFDBUtil.ToInt32(drHero["guildDonationCount"]);
		RefreshDailyGuildDonationCount(currentDate);
		m_guildMember = Cache.instance.GetGuildMember(m_id);
		if (m_guildMember != null)
		{
			m_guildMember.LogIn(this);
		}
	}

	private void CompleteLogin_GuildApplication(DateTime currentDate, int nDailyGuildApplicationCount)
	{
		HeroGuildApplicationCollection appCollection = Cache.instance.GetHeroGuildApplicationCollection(m_id);
		if (appCollection != null)
		{
			foreach (GuildApplication app in appCollection.applications.Values)
			{
				app.LogInHero(this);
				AddGuildApplication(app);
			}
		}
		m_dailyGuildApplicationCount.date = currentDate;
		m_dailyGuildApplicationCount.value = nDailyGuildApplicationCount;
	}

	private void CompleteLogin_GuildSkills(DataRowCollection drcGuildSkillLevels)
	{
		foreach (DataRow dr in drcGuildSkillLevels)
		{
			HeroGuildSkill guildSkill = new HeroGuildSkill(this);
			guildSkill.Init(dr);
			AddGuildSkill(guildSkill);
			guildSkill.RefreshRealLevel();
		}
	}

	private void CompleteLogin_GuildFarmQuest(DateTime currentDate, int nDailyStartCount, DataRow drPerformingQuest)
	{
		m_dailyGuildFarmQuestStartCount.date = currentDate;
		m_dailyGuildFarmQuestStartCount.value = nDailyStartCount;
		if (drPerformingQuest != null && m_guildMember != null)
		{
			Guid questGuildId = (Guid)drPerformingQuest["guildId"];
			if (questGuildId == m_guildMember.guild.id)
			{
				m_guildFarmQuest = new HeroGuildFarmQuest(this);
				m_guildFarmQuest.Init(drPerformingQuest);
			}
		}
	}

	private void CompleteLogin_GuildFoodWarehouse(DateTime currentDate, DataRow drHero)
	{
		m_dailyGuildFoodWarehouseStockCount.date = SFDBUtil.ToDateTime(drHero["guildFoodWarehouseStockDate"], DateTime.MinValue.Date);
		m_dailyGuildFoodWarehouseStockCount.value = SFDBUtil.ToInt32(drHero["guildFoodWarehouseStockCount"]);
		RefreshDailyGuildFoodWarehouseStockCount(currentDate);
		m_receivedGuildFoodWarehouseCollectionId = (Guid)drHero["receivedGuildFoodWarehouseCollectionId"];
	}

	private void CompleteLogin_GuildAltar(DateTime currentDate, DataRow drHero)
	{
		m_guildMoralPoint.date = SFDBUtil.ToDateTime(drHero["guildMoralPointDate"], DateTime.MinValue.Date);
		m_guildMoralPoint.value = SFDBUtil.ToInt32(drHero["guildMoralPoint"]);
		RefreshGuildMoralPoint(currentDate);
		m_guildAltarRewardReceivedDate = SFDBUtil.ToDateTime(drHero["guildAltarRewardReceivedDate"], DateTime.MinValue.Date);
		m_guildAltarDefenseMissionStartTime = SFDBUtil.ToDateTimeOffset(drHero["guildAltarDefenseStartTime"], DateTimeOffset.MinValue);
	}

	private void CompleteLogin_GuildMissionQuest(DateTimeOffset currentTime, DateTime currentDate, DataRow drHeroGuildMissionQuest, DataRowCollection drcHeroGuildMissionQuestMissions)
	{
		if (drHeroGuildMissionQuest == null)
		{
			return;
		}
		m_guildMissionQuest = new HeroGuildMissionQuest(this, currentDate);
		m_guildMissionQuest.Init(drHeroGuildMissionQuest);
		if (drcHeroGuildMissionQuestMissions == null)
		{
			return;
		}
		int nDailyGuildMissionCompletionCount = 0;
		HeroGuildMissionQuestMission guildMission = null;
		foreach (DataRow dr in drcHeroGuildMissionQuestMissions)
		{
			switch (SFDBUtil.ToInt32(dr["status"]))
			{
			case 0:
				guildMission = new HeroGuildMissionQuestMission(m_guildMissionQuest);
				guildMission.Init(dr);
				m_guildMissionQuest.SetCurrentMission(guildMission);
				break;
			case 1:
				nDailyGuildMissionCompletionCount++;
				break;
			}
		}
		m_guildMissionQuest.completionCount = nDailyGuildMissionCompletionCount;
		if (guildMission == null)
		{
			return;
		}
		if (guildMission.mission.type == GuildMissionType.Summon)
		{
			Continent targetContinent = Resource.instance.GetContinent(guildMission.spawnedMonsterContinentId);
			Place place = null;
			place = ((!targetContinent.isNationTerritory) ? ((ContinentInstance)Cache.instance.GetDisputeContinentInstance(targetContinent.id)) : ((ContinentInstance)m_nationInst.GetContinentInstance(targetContinent.id)));
			lock (place.syncObject)
			{
				foreach (MonsterInstance monsterInst in place.monsterInsts.Values)
				{
					if (monsterInst is GuildMissionMonsterInstance monsterInstance && monsterInstance.missionHeroId == m_id)
					{
						guildMission.monsterInst = monsterInstance;
						break;
					}
				}
			}
			if (guildMission.monsterInst == null)
			{
				m_guildMissionQuest.FailCurrentMission(currentTime, bSendEvent: false);
			}
		}
		if (m_guildMissionQuest.currentMission != null)
		{
			HeroGuildMissionQuestMission currentMission = m_guildMissionQuest.currentMission;
			if (m_guildMember == null || !(m_guildMember.guild.id == currentMission.guildId))
			{
				m_guildMissionQuest.FailCurrentMission(currentTime, bSendEvent: false);
			}
		}
	}

	private void CompleteLogin_GuildSupplySupportQuest(DateTimeOffset time)
	{
		if (m_guildMember == null)
		{
			return;
		}
		Guild guild = m_guildMember.guild;
		GuildSupplySupportQuestPlay guildSupplySupportQuestPlay = guild.guildSupplySupportQuestPlay;
		if (guildSupplySupportQuestPlay == null || m_id != guildSupplySupportQuestPlay.heroId)
		{
			return;
		}
		m_guildSupplySupportQuestPlay = guildSupplySupportQuestPlay;
		GuildSupplySupportQuestCartInstance cartInst = new GuildSupplySupportQuestCartInstance();
		lock (cartInst.syncObject)
		{
			cartInst.Init(guildSupplySupportQuestPlay, this, time, bFirstCreation: false);
			if (guildSupplySupportQuestPlay.isCartRiding)
			{
				cartInst.isRiding = true;
				m_ridingCartInst = cartInst;
			}
		}
	}

	private void CompleteLogin_GuildHuntingQuest(DataRow drHero, DataRow drHeroGuildHuntionQuest, int nDailyGuildHuntingCount, DateTime date, DateTimeOffset time)
	{
		m_guildHuntingDonationDate = SFDBUtil.ToDateTime(drHero["guildHuntingDonationDate"], DateTime.MinValue.Date);
		m_guildHuntingDonationCompletionRewardReceivedDate = SFDBUtil.ToDateTime(drHero["guildHuntingDonationCompletionRewardReceivedDate"], DateTime.MinValue.Date);
		m_dailyGuildHuntingQuest.date = date;
		m_dailyGuildHuntingQuest.value = nDailyGuildHuntingCount;
		if (drHeroGuildHuntionQuest != null)
		{
			HeroGuildHuntingQuest guildHuntingQuest = new HeroGuildHuntingQuest(m_guildMember);
			guildHuntingQuest.Init(drHeroGuildHuntionQuest);
			m_guildHuntingQuest = guildHuntingQuest;
		}
		if (m_guildMember == null)
		{
			FailGuildHuntingQuest(time);
		}
	}

	private void CompleteLogin_GuildEvent(DataRow drHero, DateTime date)
	{
		m_guildWeeklyObjectiveRewardReceivedDate = SFDBUtil.ToDateTime(drHero["guildWeeklyObjectiveRewardReceivedDate"], DateTime.MinValue.Date);
		m_receivedDailyObjectiveRewardNo.date = SFDBUtil.ToDateTime(drHero["guildDailyObjectiveRewardReceivedDate"], DateTime.MinValue.Date);
		m_receivedDailyObjectiveRewardNo.value = SFDBUtil.ToInt32(drHero["guildDailyObjectiveRewardReceivedNo"]);
		RefreshGuildDailyObjectiveReceivedReward(date);
	}

	private void CompleteLogin_Accomplishment(DataRowCollection drcHeroAccomplishmentRewards, DataRow drHero, int nAccSoulCoveterPlayCount)
	{
		foreach (DataRow dr in drcHeroAccomplishmentRewards)
		{
			int nId = SFDBUtil.ToInt32(dr["accomplishmentId"]);
			AddRewardedAccomplishment(nId);
			Accomplishment accomplishment = Resource.instance.GetAccomplishment(nId);
			if (accomplishment != null)
			{
				m_nAccomplishmentPoint += accomplishment.point;
			}
		}
		m_nAccMonsterKillCount = SFDBUtil.ToInt32(drHero["accMonsterKillCount"]);
		m_nAccEpicBaitItemUseCount = SFDBUtil.ToInt32(drHero["accEpicBaitItemUseCount"]);
		m_nAccLegendBaitItemUseCount = SFDBUtil.ToInt32(drHero["accLegendBaitItemUseCount"]);
		m_nAccNationWarWinCount = SFDBUtil.ToInt32(drHero["accNationWarWinCount"]);
		m_nAccNationWarKillCount = SFDBUtil.ToInt32(drHero["accNationWarKillCount"]);
		m_nAccNationWarCommanderKillCount = SFDBUtil.ToInt32(drHero["accNationWarCommanderKillCount"]);
		m_nAccNationWarImmediateRevivalCount = SFDBUtil.ToInt32(drHero["accNationWarImmediateRevivalCount"]);
		m_lnMaxGold = SFDBUtil.ToInt64(drHero["maxGold"]);
		m_lnMaxBattlePower = SFDBUtil.ToInt64(drHero["maxBattlePower"]);
		m_nMaxAcquisitionMainGearGrade = SFDBUtil.ToInt32(drHero["maxAcquisitionMainGearGrade"]);
		m_nMaxEquippedMainGearEnchantLevel = SFDBUtil.ToInt32(drHero["maxEquippedMainGearEnchantLevel"]);
		m_nAccSoulCoveterPlayCount = nAccSoulCoveterPlayCount;
	}

	private void CompleteLogin_Title(DataRowCollection drcHeroTitles, DateTimeOffset time, DataRow drHero)
	{
		foreach (DataRow dr in drcHeroTitles)
		{
			HeroTitle title = new HeroTitle(this);
			title.Init(dr);
			AddTitle(title);
			title.Init_LifetimeTimer(time);
		}
		int nDisplayTitleId = SFDBUtil.ToInt32(drHero["displayTitleId"]);
		HeroTitle displayTitle = (m_displayTitle = GetTitle(nDisplayTitleId));
		int nActivationTitleId = SFDBUtil.ToInt32(drHero["activationTitleId"]);
		HeroTitle activationTitle = (m_activationTitle = GetTitle(nActivationTitleId));
	}

	private void CompleteLogin_CreatureCard(DataRowCollection drcHeroCreatureCards, DataRowCollection drcHeroCreatureCardCollections, DataRow drHero)
	{
		foreach (DataRow dr2 in drcHeroCreatureCards)
		{
			HeroCreatureCard card = new HeroCreatureCard(this);
			card.Init(dr2);
			AddCreatureCard(card);
		}
		foreach (DataRow dr in drcHeroCreatureCardCollections)
		{
			HeroCreatureCardCollection collection = new HeroCreatureCardCollection(this);
			collection.Init(dr);
			AddActivatedCreatureCardCollection(collection);
		}
		m_nCreatureCardCollectionFamePoint = SFDBUtil.ToInt32(drHero["creatureCardCollectionFamePoint"]);
		m_creatureCardCollectionFamePointUpdateTime = SFDBUtil.ToDateTimeOffset(drHero["creatureCardCollectionFamePointUpdateTime"], DateTimeOffset.MinValue);
	}

	private void CompleteLogin_CreatureCardShop(DataRowCollection drcHeroCreatureCardShopFixedProducts, DataRowCollection drcHeroCreatureCardShopRandomProducts, DataRow drHero, DateTimeOffset currentTime)
	{
		foreach (DataRow dr in drcHeroCreatureCardShopFixedProducts)
		{
			int nProductId = SFDBUtil.ToInt32(dr["productId"]);
			AddPurchasedCreatureCardShopFixedProduct(nProductId);
		}
		foreach (DataRow dr2 in drcHeroCreatureCardShopRandomProducts)
		{
			HeroCreatureCardShopRandomProduct product = new HeroCreatureCardShopRandomProduct(this);
			product.Init(dr2);
			AddCreatureCardShopRandomProduct(product);
		}
		m_creatureCardShopRefreshDate = SFDBUtil.ToDateTime(drHero["creatureCardShopRefreshDate"], DateTime.MinValue.Date);
		m_nCreatureCardShopRefreshScheduleId = SFDBUtil.ToInt32(drHero["creatureCardShopRefreshScheduleId"]);
		m_creatureCardShopId = (Guid)drHero["creatureCardShopId"];
		m_dailyCreatureCardShopPaidRefreshCount.date = SFDBUtil.ToDateTime(drHero["creatureCardShopPaidRefreshDate"], DateTime.MinValue.Date);
		m_dailyCreatureCardShopPaidRefreshCount.value = SFDBUtil.ToInt32(drHero["creatureCardShopPaidRefreshCount"]);
		RefreshDailyCreatureCardShopPaidRefreshCount(currentTime.Date);
		CreatureCardShopRefreshSchedule targetSchedule = null;
		DateTime targetDate = currentTime.Date;
		float fTimeOfDay = (float)currentTime.TimeOfDay.TotalSeconds;
		foreach (CreatureCardShopRefreshSchedule schedule in Resource.instance.creatureCardShopRefreshSchedules)
		{
			if (!((float)schedule.refreshTime > fTimeOfDay))
			{
				targetSchedule = schedule;
				continue;
			}
			break;
		}
		if (targetSchedule == null)
		{
			targetDate = targetDate.AddDays(-1.0);
			targetSchedule = Resource.instance.lastCreatureCardShopRefreshSchedule;
		}
		if (targetSchedule != null && (targetDate != m_creatureCardShopRefreshDate || targetSchedule.id != m_nCreatureCardShopRefreshScheduleId))
		{
			RefreshCreatureCardShopOnSchedule(targetDate, targetSchedule.id, bSendEvent: false, currentTime);
		}
	}

	private void CompleteLogIn_Stamina(DataRow drHero, DateTimeOffset currentTime, DateTime currentDate)
	{
		m_nStamina = SFDBUtil.ToInt32(drHero["stamina"]);
		m_staminaUpdateTime = SFDBUtil.ToNullableDateTimeOffset(drHero["staminaUpdateTime"]);
		AutoRecoveryStamina(currentTime, bSendEvent: false);
		m_dailyStaminaBuyCount.date = SFDBUtil.ToDateTime(drHero["staminaBuyDate"], DateTime.MinValue.Date);
		m_dailyStaminaBuyCount.value = SFDBUtil.ToInt32(drHero["staminaBuyCount"]);
		RefreshDailyStaminaBuyCount(currentTime.Date);
		m_staminaRecoverySchedule.date = SFDBUtil.ToDateTime(drHero["staminaRecoveryDate"], DateTime.MinValue.Date);
		m_staminaRecoverySchedule.value = SFDBUtil.ToInt32(drHero["staminaRecoveryScheduleId"]);
		StaminaRecoverySchedule targetSchedule = null;
		DateTime targetDate = currentDate;
		StaminaRecoverySchedule lastSchedule = Resource.instance.lastStaminaRecoverySchedule;
		float fTimeOfDay = (float)currentTime.TimeOfDay.TotalSeconds;
		foreach (StaminaRecoverySchedule schedule in Resource.instance.staminaRecoverySchedules)
		{
			if (!((float)schedule.recoveryTime > fTimeOfDay))
			{
				targetSchedule = schedule;
				continue;
			}
			break;
		}
		if (targetSchedule == null)
		{
			targetDate = targetDate.AddDays(-1.0);
			targetSchedule = lastSchedule;
		}
		if (targetSchedule != null && (targetDate != m_staminaRecoverySchedule.date || targetSchedule.id != m_staminaRecoverySchedule.value))
		{
			int nTotalRecoveryStamina = 0;
			if (m_staminaRecoverySchedule.value > 0)
			{
				int nElapsedDays = (int)(targetDate - m_staminaRecoverySchedule.date).TotalDays;
				int nMaxRecoveredStamina = Resource.instance.totalStaminaOfStaminaRecoverySchedules;
				int nTargetScheduleStamina = Resource.instance.GetStaminaSumToStaminaRecoverySchedule(targetSchedule.id);
				int nCurrentScheduleStamina = Resource.instance.GetStaminaSumToStaminaRecoverySchedule(m_staminaRecoverySchedule.value);
				nTotalRecoveryStamina = nElapsedDays * nMaxRecoveredStamina + nTargetScheduleStamina - nCurrentScheduleStamina;
			}
			RecoveryStaminaOnSchedule(targetDate, targetSchedule.id, nTotalRecoveryStamina, bSendEvent: false, currentTime);
		}
	}

	private void CompleteLogin_ProofOfValor(int nProofOfValorClearedCount, DataRow drProofOfValorEnterCountOfDate, DataRow drLastHeroProofOfValorInstance, DateTimeOffset time)
	{
		m_bProofOfValorCleared = nProofOfValorClearedCount > 0;
		if (drProofOfValorEnterCountOfDate != null)
		{
			m_dailyProofOfValorPlayCount.date = SFDBUtil.ToDateTime(drProofOfValorEnterCountOfDate["date"], DateTime.MinValue);
			m_dailyProofOfValorPlayCount.value = SFDBUtil.ToInt32(drProofOfValorEnterCountOfDate["cnt"]);
		}
		if (drLastHeroProofOfValorInstance != null)
		{
			HeroProofOfValorInstance inst = new HeroProofOfValorInstance(this);
			inst.Init(drLastHeroProofOfValorInstance);
			if (inst.status == 0)
			{
				m_heroProofOfValorInst = inst;
			}
		}
		ProofOfValor proofOfValor = Resource.instance.proofOfValor;
		ProofOfValorRefreshSchedule targetSchedule = null;
		DateTime targetDate = time.Date;
		float fTimeOfDay = (float)time.TimeOfDay.TotalSeconds;
		foreach (ProofOfValorRefreshSchedule schedule in proofOfValor.refreshSchedules)
		{
			if (!((float)schedule.refreshTime > fTimeOfDay))
			{
				targetSchedule = schedule;
				continue;
			}
			break;
		}
		if (targetSchedule == null)
		{
			targetDate = targetDate.AddDays(-1.0);
			targetSchedule = proofOfValor.lastRefreshSchedule;
		}
		if (targetSchedule != null && (targetDate != m_proofOfValorAutoRefreshDate || targetSchedule.id != m_nProofOfValorAutoRefreshScheduleId))
		{
			RefreshProofOfValorAutoRefreshSchedule(targetDate, targetSchedule.id, bSendEvent: false, time);
		}
		if (m_heroProofOfValorInst == null)
		{
			CreateHeroProofOfValorInstance(time, bIsRefreshPaidCount: false);
		}
	}

	private void CompleteLogin_RookieGift(DataRow drHero, DateTimeOffset currentTime)
	{
		m_nRookieGiftNo = SFDBUtil.ToInt32(drHero["rookieGiftNo"]);
		m_fRookieGiftLoginDuration = SFDBUtil.ToSingle(drHero["rookieGiftLoginDuration"]);
		m_rookieGitfLoginStartTime = currentTime;
		if (m_nRookieGiftNo <= 0)
		{
			m_nRookieGiftNo = 1;
			m_fRookieGiftLoginDuration = 0f;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_RookieGift(m_id, m_nRookieGiftNo, m_fRookieGiftLoginDuration));
			dbWork.Schedule();
		}
	}

	private void CompleteLogin_DailyQuest(DataRow drHero, DateTime currentDate, int nDailyQuestAcceptionCount, DataRowCollection drcHeroDailyQuests)
	{
		m_dailyQuestFreeRefreshCount.date = SFDBUtil.ToDateTime(drHero["dailyQuestFreeRefreshDate"], DateTime.MinValue.Date);
		m_dailyQuestFreeRefreshCount.value = SFDBUtil.ToInt32(drHero["dailyQuestFreeRefreshCount"]);
		RefreshDailyQuestFreeRefreshCount(currentDate);
		m_dailyQuestAcceptionCount.date = currentDate;
		m_dailyQuestAcceptionCount.value = nDailyQuestAcceptionCount;
		foreach (DataRow dr in drcHeroDailyQuests)
		{
			HeroDailyQuest quest = new HeroDailyQuest(this);
			quest.Init(dr);
			SetDailyQuest(quest);
		}
		InitDailyQuests(bSendEvent: false);
	}

	private void CompleteLogin_WeeklyQuest(DataRow drWeeklyQuest, DateTimeOffset currentTime)
	{
		if (drWeeklyQuest != null)
		{
			HeroWeeklyQuest weeklyQuest = new HeroWeeklyQuest(this);
			weeklyQuest.Init(drWeeklyQuest);
			m_weeklyQuest = weeklyQuest;
		}
		InitWeeklyQuest(currentTime, bSendEvent: false);
	}

	private void CompleteLogin_Open7DayEvent(DataRowCollection drcOpen7DayEventMissions, DataRowCollection drcOpen7DayEventProducts, DataRowCollection drcOpen7DayEventProgressCounts)
	{
		foreach (DataRow dr2 in drcOpen7DayEventMissions)
		{
			int nMissionId = SFDBUtil.ToInt32(dr2["missionId"]);
			AddRewardedOpen7DayEventMission(nMissionId);
		}
		foreach (DataRow dr3 in drcOpen7DayEventProducts)
		{
			int nProductId = SFDBUtil.ToInt32(dr3["productId"]);
			AddPurchasedOpen7DayEventProducts(nProductId);
		}
		foreach (DataRow dr in drcOpen7DayEventProgressCounts)
		{
			HeroOpen7DayEventProgressCount progressCount = new HeroOpen7DayEventProgressCount(this);
			progressCount.Init(dr);
			AddOpen7DayEventProgressCount(progressCount);
		}
	}

	private void CompleteLogin_Retrieval(DateTime yesterDay, DateTime currentDate, DataRowCollection drcPrevRetrievalProgressCounts, DataRowCollection drcCurrRetrievalProgressCounts, DataRowCollection drcRetreivals)
	{
		m_retrievalDate = currentDate;
		foreach (DataRow dr2 in drcRetreivals)
		{
			HeroRetrieval retrieval = new HeroRetrieval(this);
			retrieval.Init(dr2);
			AddRetrieval(retrieval);
		}
		HeroRetrievalProgressCountCollection prevRetrievalProgressCountCollection = GetOrCreateRetrivalProgressCountCollection(yesterDay);
		foreach (DataRow dr3 in drcPrevRetrievalProgressCounts)
		{
			HeroRetrievalProgressCount progressCount2 = new HeroRetrievalProgressCount(prevRetrievalProgressCountCollection);
			progressCount2.Init(dr3);
			prevRetrievalProgressCountCollection.AddProgressCount(progressCount2);
		}
		HeroRetrievalProgressCountCollection currRetrievalProgressCountCollection = GetOrCreateRetrivalProgressCountCollection(currentDate);
		foreach (DataRow dr in drcCurrRetrievalProgressCounts)
		{
			HeroRetrievalProgressCount progressCount = new HeroRetrievalProgressCount(currRetrievalProgressCountCollection);
			progressCount.Init(dr);
			currRetrievalProgressCountCollection.AddProgressCount(progressCount);
		}
	}

	private void CompleteLogin_Consignments(DataRowCollection drcCurrentTaskConsignments, DataRowCollection drcTaskConsignmentCountsOfDate, DateTime date)
	{
		foreach (DataRow dr2 in drcCurrentTaskConsignments)
		{
			HeroTaskConsignment consignment = new HeroTaskConsignment(this);
			consignment.Init(dr2);
			AddTaskConsignment(consignment);
		}
		m_taskConsignmentStartCountDate = date;
		foreach (DataRow dr in drcTaskConsignmentCountsOfDate)
		{
			HeroTaskConsignmentStartCount startCount = new HeroTaskConsignmentStartCount();
			startCount.Init(dr);
			AddTaskConsignmentStartCount(startCount);
		}
	}

	private void CompleteLogin_TrueHeroQuest(DataRow drTrueHeroQuest)
	{
		if (drTrueHeroQuest != null)
		{
			m_trueHeroQuest = new HeroTrueHeroQuest(this);
			m_trueHeroQuest.Init(drTrueHeroQuest);
		}
	}

	private void CompleteLogin_LimitationGift(DateTime date, DataRowCollection drcLimitationGiftRewards)
	{
		m_limitationGiftDate = date;
		foreach (DataRow dr in drcLimitationGiftRewards)
		{
			int nRewardedScheduleId = SFDBUtil.ToInt32(dr["scheduleId"]);
			m_rewardedLimitationGiftScheduleIds.Add(nRewardedScheduleId);
		}
	}

	private void CompleteLogin_WeekendReward(DataRow drWeekendReward, DateTime date)
	{
		if (drWeekendReward != null)
		{
			m_weekendReward = new HeroWeekendReward(this);
			m_weekendReward.Init(drWeekendReward);
		}
		RefreshWeekendReward(date);
	}

	private void CompleteLogin_Warehouse(DataRowCollection drcWarehouseSlots)
	{
		CreateWarehouseSlots(Resource.instance.freeWarehouseSlotCount + m_nPaidWarehouseSlotCount);
		foreach (DataRow dr in drcWarehouseSlots)
		{
			int nSlotIndex = SFDBUtil.ToInt32(dr["slotIndex"]);
			WarehouseSlot slot = GetWarehouseSlot(nSlotIndex);
			if (slot == null)
			{
				throw new Exception("존재하지않는 창고 슬롯입니다. nSlotIndex = " + nSlotIndex);
			}
			int nType = SFDBUtil.ToInt32(dr["slotType"]);
			switch (nType)
			{
			case 1:
			{
				Guid herMainGearId = (Guid)dr["heroMainGearId"];
				HeroMainGear mainGear = GetMainGear(herMainGearId);
				if (mainGear == null)
				{
					throw new Exception("존재하지 않는 메인장비 입니다. herMainGearId = " + herMainGearId);
				}
				slot.Place(mainGear);
				break;
			}
			case 2:
			{
				int nSubGearId = SFDBUtil.ToInt32(dr["subGearId"]);
				HeroSubGear subGear = GetSubGear(nSubGearId);
				if (subGear == null)
				{
					throw new Exception("존재하지 않는 보조장비 입니다. nSubGearId = " + nSubGearId);
				}
				slot.Place(subGear);
				break;
			}
			case 3:
			{
				int nItemId = SFDBUtil.ToInt32(dr["itemId"]);
				bool bOwned = SFDBUtil.ToBoolean(dr["itemOwned"]);
				int nCount = SFDBUtil.ToInt32(dr["itemCount"]);
				Item item = Resource.instance.GetItem(nItemId);
				HeroWarehouseItem heroWarehouseItem = GetOrCreateWarehouseItem(item);
				ItemWarehouseObject itemWarehouseObject = new ItemWarehouseObject(heroWarehouseItem, bOwned, nCount);
				slot.Place(itemWarehouseObject);
				heroWarehouseItem.AddObject(itemWarehouseObject);
				break;
			}
			case 4:
			{
				Guid heroMountGearId = (Guid)dr["heroMountGearId"];
				HeroMountGear heroMountGear = GetMountGear(heroMountGearId);
				if (heroMountGear == null)
				{
					throw new Exception("존재하지 않는 탈것장비 입니다. heroMountGearId = " + heroMountGearId);
				}
				slot.Place(heroMountGear);
				break;
			}
			default:
				throw new Exception("유효하지않은 창고 슬롯 타입입니다. nType = " + nType);
			}
		}
	}

	public void CompleteLogin_DiaShopProducts(DateTime date, DataRowCollection drcDiaShopProducts, DataRowCollection drcDiaShopProductsAccBuyCounts)
	{
		m_dailyDiaShopProductBuyCountsDate = date;
		foreach (DataRow dr2 in drcDiaShopProducts)
		{
			HeroDiaShopProductBuyCount buyCount = new HeroDiaShopProductBuyCount(this);
			buyCount.Init(dr2);
			AddDailyDiaShopProductBuyCount(buyCount);
		}
		foreach (DataRow dr in drcDiaShopProductsAccBuyCounts)
		{
			HeroDiaShopProductBuyCount totalBuyCount = new HeroDiaShopProductBuyCount(this);
			totalBuyCount.Init(dr);
			AddTotalDiaShopProductBuyCount(totalBuyCount);
		}
	}

	private void CompleteLogin_FearAltar(DateTime date, DataRowCollection drcHeroFearAltarHalidoms, DataRowCollection drcHeroFearAltarHalidomElementalRewards)
	{
		DateTime weekStartDate = (m_fearAltarHalidomWeekStartDate = DateTimeUtil.GetWeekStartDate(date));
		foreach (DataRow drHeroFearAltarHalidom in drcHeroFearAltarHalidoms)
		{
			HeroFearAltarHalidom heroHalidom = new HeroFearAltarHalidom(this);
			heroHalidom.Init(drHeroFearAltarHalidom);
			AddFearAltarHalidom(heroHalidom);
		}
		m_fearAltarHalidomElementalRewardWeekStartDate = weekStartDate;
		foreach (DataRow drHeroAltarHalidomElementalReward in drcHeroFearAltarHalidomElementalRewards)
		{
			HeroFearAltarHalidomElementalReward heroHalidomReward = new HeroFearAltarHalidomElementalReward(this);
			heroHalidomReward.Init(drHeroAltarHalidomElementalReward);
			AddFearAltarHalidomElementalReward(heroHalidomReward);
		}
	}

	private void CompleteLogin_WingMemoryPieceSlots(DataRowCollection drcWingMemoryPieceSlots)
	{
		foreach (DataRow dr in drcWingMemoryPieceSlots)
		{
			int nWingId = SFDBUtil.ToInt32(dr["wingId"]);
			HeroWing heroWing2 = GetWing(nWingId);
			int nIndex = SFDBUtil.ToInt32(dr["slotIndex"]);
			int nAccAttrValue = SFDBUtil.ToInt32(dr["accAttrValue"]);
			HeroWingMemoryPieceSlot slot = heroWing2.GetMemoryPieceSlot(nIndex);
			slot.IncreaseAccAttrValue(nAccAttrValue);
		}
		foreach (HeroWing heroWing in m_wings.Values)
		{
			if (heroWing.wing.memoryPieceInstallationEnabled)
			{
				heroWing.RefreshAttrTotalValues();
			}
		}
	}

	private void CompleteLogin_SubQuest(DataRowCollection drcSubQuests)
	{
		foreach (DataRow dr in drcSubQuests)
		{
			HeroSubQuest quest = new HeroSubQuest(this);
			quest.Init(dr);
			AddSubQuest(quest);
		}
	}

	private void CompleteLogin_OrdealQuest(DataRow drOrdealQuest, DataRowCollection drcOrdealQuestMissions)
	{
		if (drOrdealQuest == null)
		{
			return;
		}
		m_ordealQuest = new HeroOrdealQuest(this);
		m_ordealQuest.Init(drOrdealQuest);
		foreach (DataRow dr in drcOrdealQuestMissions)
		{
			int nIndex = SFDBUtil.ToInt32(dr["slotIndex"]);
			if (!SFDBUtil.ToBoolean(dr["completed"]))
			{
				HeroOrdealQuestSlot slot = m_ordealQuest.GetSlot(nIndex);
				slot.Init(dr);
			}
		}
	}

	private void CompleteLogIn_Friend(DataRowCollection drcFriends)
	{
		foreach (DataRow dr in drcFriends)
		{
			Friend friend = new Friend(this);
			friend.Init(dr);
			AddFriend(friend);
		}
	}

	private void CompleteLogIn_TempFriend(DataRowCollection drcTempFriends)
	{
		foreach (DataRow dr in drcTempFriends)
		{
			TempFriend friend = new TempFriend(this);
			friend.Init(dr);
			AddTempFriend(friend);
		}
	}

	private void CompleteLogIn_Blacklist(DataRowCollection drcBlacklistEntries)
	{
		foreach (DataRow dr in drcBlacklistEntries)
		{
			BlacklistEntry entry = new BlacklistEntry(this);
			entry.Init(dr);
			AddBlacklistEntry(entry);
		}
	}

	private void CompleteLogIn_DeadRecord(DataRowCollection drcDeadRecords)
	{
		foreach (DataRow dr in drcDeadRecords)
		{
			DeadRecord record = new DeadRecord(this);
			record.Init(dr);
			AddDeadRecord(record);
		}
	}

	private void CompleteLogin_Biography(DataRowCollection heroBiographies, DataRowCollection heroBiographyQuests)
	{
		foreach (DataRow dr2 in heroBiographies)
		{
			HeroBiography biography2 = new HeroBiography(this);
			biography2.Init(dr2);
			AddBiography(biography2);
		}
		foreach (DataRow dr in heroBiographyQuests)
		{
			int nBiographyId = SFDBUtil.ToInt32(dr["biographyId"]);
			HeroBiography biography = GetBiography(nBiographyId);
			HeroBiographyQuest quest = new HeroBiographyQuest(biography);
			quest.Init(dr);
			biography.SetQuest(quest);
		}
	}

	private void CompleteLogin_ProspectQuest(DataRowCollection drcOwnerProspectQuests, DataRowCollection drcTargetProspectQuests, DateTimeOffset currentTime)
	{
		foreach (DataRow dr2 in drcOwnerProspectQuests)
		{
			Guid instanceId2 = (Guid)dr2["questInstanceId"];
			HeroProspectQuest quest2 = Cache.instance.GetProspectQuest(instanceId2);
			if (quest2 == null)
			{
				quest2 = new HeroProspectQuest();
				quest2.Init(dr2, currentTime);
				if (!quest2.isFailed)
				{
					Cache.instance.AddProspectQuest(quest2);
				}
			}
			AddOwnerProspectQuest(quest2);
		}
		foreach (DataRow dr in drcTargetProspectQuests)
		{
			Guid instanceId = (Guid)dr["questInstanceId"];
			HeroProspectQuest quest = Cache.instance.GetProspectQuest(instanceId);
			if (quest == null)
			{
				quest = new HeroProspectQuest();
				quest.Init(dr, currentTime);
				if (!quest.isFailed)
				{
					Cache.instance.AddProspectQuest(quest);
				}
			}
			AddTargetProspectQuest(quest);
		}
	}

	private void CompleteLogin_Creatures(DataRowCollection drcCreatures, DataRowCollection drcCreatureBaseAttrs, DataRowCollection drcCreatureAdditionalAttrs, DataRowCollection drcCreatureSkills, DataRow drHero)
	{
		foreach (DataRow dr2 in drcCreatures)
		{
			HeroCreature creature3 = new HeroCreature(this);
			creature3.Init(dr2);
			AddCreature(creature3, bInit: true);
		}
		foreach (DataRow dr4 in drcCreatureBaseAttrs)
		{
			Guid creatureId3 = (Guid)dr4["heroCreatureId"];
			HeroCreature creature4 = GetCreature(creatureId3);
			HeroCreatureBaseAttr attr2 = new HeroCreatureBaseAttr(creature4);
			attr2.Init(dr4);
			creature4.AddBaseAttr(attr2);
		}
		foreach (DataRow dr3 in drcCreatureAdditionalAttrs)
		{
			Guid creatureId2 = (Guid)dr3["heroCreatureId"];
			HeroCreature creature2 = GetCreature(creatureId2);
			HeroCreatureAdditionalAttr attr = new HeroCreatureAdditionalAttr(creature2);
			attr.Init(dr3);
			creature2.AddAdditionalAttr(attr);
		}
		foreach (DataRow dr in drcCreatureSkills)
		{
			Guid creatureId = (Guid)dr["heroCreatureId"];
			HeroCreature creature = GetCreature(creatureId);
			HeroCreatureSkill skill = new HeroCreatureSkill(creature);
			skill.Init(dr);
			creature.AddSkill(skill);
		}
		m_participationCreature = GetCreature((Guid)drHero["participationHeroCreatureId"]);
	}

	private void CompleteLogIn_Present(DataRow drHero, DataRow drWeeklyPresentPopularityPoint, DataRow drWeeklyPresentContributionPoint, DateTimeOffset currentTime)
	{
		m_nRewardedNationWeeklyPresentPopularityPointRankingNo = SFDBUtil.ToInt32(drHero["rewardedNationWeeklyPresentPopularityPointRankingNo"]);
		m_nRewardedNationWeeklyPresentContributionPointRankingNo = SFDBUtil.ToInt32(drHero["rewardedNationWeeklyPresentContributionPointRankingNo"]);
		DateTime currentWeekStartDate = DateTimeUtil.GetWeekStartDate(currentTime);
		if (drWeeklyPresentPopularityPoint != null)
		{
			m_weeklyPresentPopularityPointStartDate = SFDBUtil.ToDateTime(drWeeklyPresentPopularityPoint["weekStartDate"]);
			m_nWeeklyPresentPopularityPoint = SFDBUtil.ToInt32(drWeeklyPresentPopularityPoint["point"]);
			m_weeklyPresentPopularityPointUpdateTime = (DateTimeOffset)drWeeklyPresentPopularityPoint["pointUpdateTime"];
		}
		RefreshWeeklyPresentPopularityPoint(currentWeekStartDate);
		if (drWeeklyPresentContributionPoint != null)
		{
			m_weeklyPresentContributionPointStartDate = SFDBUtil.ToDateTime(drWeeklyPresentContributionPoint["weekStartDate"]);
			m_nWeeklyPresentContributionPoint = SFDBUtil.ToInt32(drWeeklyPresentContributionPoint["point"]);
			m_weeklyPresentContributionPointUpdateTime = (DateTimeOffset)drWeeklyPresentContributionPoint["pointUpdateTime"];
		}
		RefreshWeeklyPresentContributionPoint(currentWeekStartDate);
	}

	private void CompleteLogIn_Costume(DataRow drHero, DataRowCollection drcHeroCostumes, DateTimeOffset time)
	{
		foreach (DataRow dr in drcHeroCostumes)
		{
			HeroCostume heroCostume = new HeroCostume(this);
			heroCostume.Init(dr);
			AddCostume(heroCostume);
		}
		int nEquippedCostumeId = SFDBUtil.ToInt32(drHero["equippedCostumeId"]);
		if (nEquippedCostumeId > 0)
		{
			m_equippedCostume = GetCostume(nEquippedCostumeId);
			if (m_equippedCostume == null)
			{
				throw new Exception("존재하지 않는 코스튬입니다. nEquippedCostumeId = " + nEquippedCostumeId);
			}
		}
		CheckCostumePeriodLimitTime(time, bSendEvent: false);
	}

	private void CompleteLogin_CreatureFarmQuest(DateTimeOffset time, DateTime date, int nCreatureFarmQuestAcceptionCount, DataRow drCreatureFarmQuest)
	{
		m_dailyCreatureFarmAcceptionCount.date = date;
		m_dailyCreatureFarmAcceptionCount.value = nCreatureFarmQuestAcceptionCount;
		if (drCreatureFarmQuest == null)
		{
			return;
		}
		m_creatureFarmQuest = new HeroCreatureFarmQuest(this);
		m_creatureFarmQuest.Init(drCreatureFarmQuest);
		if (m_creatureFarmQuest.mission == null || m_creatureFarmQuest.mission.targetType != CreatureFarmQuestMissionTargetType.ExclusiveMonsterHunt)
		{
			return;
		}
		Continent targetContinent = m_creatureFarmQuest.mission.targetContinent;
		ContinentInstance targetContinentInstance = null;
		targetContinentInstance = ((!targetContinent.isNationTerritory) ? ((ContinentInstance)Cache.instance.GetDisputeContinentInstance(targetContinent.id)) : ((ContinentInstance)m_nationInst.GetContinentInstance(targetContinent.id)));
		lock (targetContinentInstance.syncObject)
		{
			foreach (MonsterInstance monsterInst in targetContinentInstance.monsterInsts.Values)
			{
				if (monsterInst is CreatureFarmQuestMissionMonsterInstance monsterInstance && monsterInstance.questHeroId == m_id)
				{
					m_creatureFarmQuest.targetMonsterInst = monsterInstance;
					break;
				}
			}
			if (m_creatureFarmQuest.targetMonsterInst == null)
			{
				m_creatureFarmQuest.CompleteMission(time, bSendEvent: false);
			}
		}
	}

	private void CompleteLogin_JobChangeQuest(DataRow drJobChangeQuest, DateTimeOffset time)
	{
		if (drJobChangeQuest == null)
		{
			return;
		}
		m_jobChangeQuest = new HeroJobChangeQuest(this);
		m_jobChangeQuest.Init(drJobChangeQuest);
		if (!m_jobChangeQuest.isAccepted)
		{
			return;
		}
		float fRemainingTime = m_jobChangeQuest.GetRemainingTime(time);
		JobChangeQuest quest = m_jobChangeQuest.quest;
		if (quest.limitTime > 0 && fRemainingTime <= 0f)
		{
			m_jobChangeQuest.Fail(bSendEvent: false, time);
		}
		else
		{
			if (m_jobChangeQuest.quest.type != JobChangeQuestType.ExclusiveMonsterHunt || m_jobChangeQuest.difficulty <= 0)
			{
				return;
			}
			JobChangeQuestDifficulty difficulty = quest.GetDifficulty(m_jobChangeQuest.difficulty);
			Place targetContinentInstance = null;
			if (difficulty.isTargetPlaceGuildTerrtory)
			{
				if (m_guildMember == null)
				{
					m_jobChangeQuest.Fail(bSendEvent: false, time);
					return;
				}
				targetContinentInstance = m_guildMember.guild.territoryInst;
			}
			else
			{
				Continent targetContinent = m_jobChangeQuest.quest.targetContinent;
				targetContinentInstance = ((!targetContinent.isNationTerritory) ? ((ContinentInstance)Cache.instance.GetDisputeContinentInstance(targetContinent.id)) : ((ContinentInstance)m_nationInst.GetContinentInstance(targetContinent.id)));
			}
			if (targetContinentInstance != null)
			{
				lock (targetContinentInstance.syncObject)
				{
					foreach (MonsterInstance monsterInst in targetContinentInstance.monsterInsts.Values)
					{
						if (monsterInst is JobChangeQuestMonsterInstance monsterInstance && monsterInstance.questHeroId == m_id)
						{
							m_jobChangeQuest.monsterInst = monsterInstance;
							break;
						}
					}
				}
			}
			if (quest.limitTime > 0 && m_jobChangeQuest.monsterInst == null)
			{
				m_jobChangeQuest.StartLimitTimer((int)(fRemainingTime * 1000f));
			}
		}
	}

	private void CompleteLogin_PotionAttr(DataRowCollection drcPotionAttrs)
	{
		foreach (DataRow dr in drcPotionAttrs)
		{
			HeroPotionAttr attr = new HeroPotionAttr(this);
			attr.Init(dr);
			AddPotionAttr(attr);
		}
	}

	private void CompleteLogin_AnkouTomb(DataRowCollection drcHeroAnkouTombBestRecords)
	{
		foreach (DataRow dr in drcHeroAnkouTombBestRecords)
		{
			HeroAnkouTombBestRecord bestRecord = new HeroAnkouTombBestRecord();
			bestRecord.Init(dr);
			AddAnkouTombBestRecord(bestRecord);
		}
	}

	private void ComleteLogin_Constellation(DataRowCollection drcConstellation)
	{
		foreach (DataRow dr in drcConstellation)
		{
			int nConstellationId = SFDBUtil.ToInt32(dr["constellationId"]);
			Constellation constellation = Resource.instance.GetConstellation(nConstellationId);
			HeroConstellation heroConstellation = GetConstellation(nConstellationId);
			if (heroConstellation == null)
			{
				heroConstellation = new HeroConstellation(this, nConstellationId);
				m_constellations.Add(heroConstellation.id, heroConstellation);
			}
			int nStep = SFDBUtil.ToInt32(dr["step"]);
			constellation.GetStep(nStep);
			HeroConstellationStep heroStep = heroConstellation.GetStep(nStep);
			if (heroStep == null)
			{
				heroStep = new HeroConstellationStep(heroConstellation, nStep);
				heroConstellation.AddStep(heroStep);
			}
			heroStep.InIt(dr);
		}
	}

	private void CompleteLogIn_Artifact(DataRow drHero)
	{
		m_nArtifactNo = SFDBUtil.ToInt32(drHero["artifactNo"]);
		m_nArtifactLevel = SFDBUtil.ToInt32(drHero["artifactLevel"]);
		m_nArtifactExp = SFDBUtil.ToInt32(drHero["artifactExp"]);
		m_nEquippedArtifactNo = SFDBUtil.ToInt32(drHero["equippedArtifactNo"]);
	}

	private void CompleteLogin_TradeShip(DataRowCollection drcHeroTradeShipBestRecords)
	{
		foreach (DataRow dr in drcHeroTradeShipBestRecords)
		{
			HeroTradeShipBestRecord bestRecord = new HeroTradeShipBestRecord();
			bestRecord.Init(dr);
			AddTradeShipBestRecord(bestRecord);
		}
	}

	private void CompelteLogin_GM(DateTimeOffset currentTime)
	{
		GM_CompleteTargetMainQuest(currentTime);
	}

	private void CompleteLogin_TimeDesignationEvent(DataRowCollection drcTimeDesignationEvent, DateTimeOffset currentTime)
	{
		foreach (DataRow dr in drcTimeDesignationEvent)
		{
			int nEventId = SFDBUtil.ToInt32(dr["eventId"]);
			AddTimeDesignationEventReward(nEventId);
		}
		List<TimeDesignationEvent> timeDesignationEvents = new List<TimeDesignationEvent>();
		foreach (TimeDesignationEvent evt in Resource.instance.timeDesignationEvents.Values)
		{
			if (evt.IsEventTime(currentTime.DateTime))
			{
				timeDesignationEvents.Add(evt);
			}
		}
		if (timeDesignationEvents.Count > 0)
		{
			ReceiveTimeDesignationEventRewards(timeDesignationEvents, bSendEvent: false, currentTime);
		}
	}

	public void LogOut()
	{
		if (isLoggingIn || isLoggedIn)
		{
			DateTimeOffset currentTime = DateTimeUtil.currentTime;
			if (isLoggedIn)
			{
				m_lastLogoutTime = currentTime;
				LogOut_MatchingRoom();
				LogOut_Party();
				LogOut_Guild();
				LogOut_GuildAltarDefenseMission();
				LogOut_MainQuest();
				LogOut_SupplySupportQuest();
				LogOut_GuildSupplySupportQuest();
				LogOut_Titles();
				LogOut_RookieGift();
				LogOut_Interaction();
				LogOut_Place();
				LogOut_Friend();
				LogOut_JobChangeQuest();
				m_nationInst.RemoveHero(m_id);
				RefreshDailyAccessTime(currentTime);
				GameDacEx.Logout(this);
			}
			Cache.instance.RemoveHero(m_id);
			m_account.currentHero = null;
			m_status = HeroStatus.LoggedOut;
			Release();
		}
	}

	private void LogOut_Party()
	{
		if (m_partyMember != null)
		{
			m_partyMember.LogOut();
			m_partyMember = null;
		}
		CancelAllPartyApplications();
		RefuseAllPartyInvitations();
	}

	private void LogOut_Guild()
	{
		if (m_guildMember != null)
		{
			m_guildMember.LogOut();
		}
		foreach (GuildApplication application in m_guildApplications.Values)
		{
			application.LogOutHero();
		}
		RefuseAllGuildInvitations();
	}

	private void LogOut_GuildAltarDefenseMission()
	{
		if (m_guildAltarDefenseMission != null)
		{
			m_guildAltarDefenseMission.Fail(bSendEventToMySelf: false);
		}
	}

	private void LogOut_MainQuest()
	{
		if (m_currentHeroMainQuest != null && !m_currentHeroMainQuest.completed && m_currentHeroMainQuest.mainQuest.type == 6)
		{
			LogOut_MainQuest_Transport();
		}
	}

	private void LogOut_MainQuest_Transport()
	{
		MainQuestCartInstance cartInst = m_currentHeroMainQuest.cartInst;
		if (cartInst != null)
		{
			CartSynchronizer.Exec(cartInst, new SFAction<MainQuestCartInstance>(LogOut_MainQuest_Transport_Cart, cartInst));
		}
	}

	private void LogOut_MainQuest_Transport_Cart(MainQuestCartInstance cartInst)
	{
		((ContinentInstance)cartInst.currentPlace)?.ExitCart(cartInst, bSendEvent: true, bResetPlaceReferenceOfCartInst: true);
		Cache.instance.RemoveCartInstance(cartInst);
	}

	private void LogOut_SupplySupportQuest()
	{
		if (m_supplySupportQuest != null)
		{
			SupplySupportQuestCartInstance cartInst = m_supplySupportQuest.cartInst;
			if (cartInst != null)
			{
				CartSynchronizer.Exec(cartInst, new SFAction<SupplySupportQuestCartInstance>(LogOut_SupplySupportQuest_Cart, cartInst));
			}
		}
	}

	private void LogOut_SupplySupportQuest_Cart(SupplySupportQuestCartInstance cartInst)
	{
		m_supplySupportQuest.RefreshCartInfo();
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateSupplySupportQuest_Cart(cartInst.quest));
		dbWork.Schedule();
		((ContinentInstance)cartInst.currentPlace)?.ExitCart(cartInst, bSendEvent: true, bResetPlaceReferenceOfCartInst: true);
		Cache.instance.RemoveCartInstance(cartInst);
	}

	private void LogOut_GuildSupplySupportQuest()
	{
		if (m_guildSupplySupportQuestPlay != null)
		{
			GuildSupplySupportQuestCartInstance cartInst = m_guildSupplySupportQuestPlay.cartInst;
			if (cartInst != null)
			{
				CartSynchronizer.Exec(cartInst, new SFAction<GuildSupplySupportQuestCartInstance>(LogOut_GuildSupplySupportQuest_Cart, cartInst));
			}
		}
	}

	private void LogOut_GuildSupplySupportQuest_Cart(GuildSupplySupportQuestCartInstance cartInst)
	{
		m_guildSupplySupportQuestPlay.RefreshCartInfo();
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateGuildSupplySupportQuest_Cart(cartInst.quest));
		dbWork.Schedule();
		((ContinentInstance)cartInst.currentPlace)?.ExitCart(cartInst, bSendEvent: true, bResetPlaceReferenceOfCartInst: true);
		Cache.instance.RemoveCartInstance(cartInst);
	}

	private void LogOut_Titles()
	{
		foreach (HeroTitle title in m_titles.Values)
		{
			title.Release();
		}
	}

	private void LogOut_Place()
	{
		if (m_bIsInitEntranceCopmleted)
		{
			if (m_currentPlace != null)
			{
				m_lastLocation = m_currentPlace.location;
				m_nLastLocationParam = m_currentPlace.locationParam;
				m_lastInstanceId = m_currentPlace.instanceId;
				m_lastPosition = m_position;
				m_fLastYRotation = m_fRotationY;
				m_currentPlace.Exit(this, isLogOut: true, null);
			}
			else
			{
				m_lastLocation = null;
				m_nLastLocationParam = 0;
				m_lastInstanceId = Guid.Empty;
				m_lastPosition = Vector3.zero;
				m_fLastYRotation = 0f;
			}
		}
	}

	private void LogOut_RookieGift()
	{
		if (Resource.instance.GetRookieGift(m_nRookieGiftNo) != null)
		{
			m_fRookieGiftLoginDuration = GetCurrentRookieGiftLoginDuration(m_lastLogoutTime.Value);
		}
	}

	private void LogOut_Interaction()
	{
		CancelContinentObjectInteraction(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelRuinsReclaimObjectInteraction(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelTrueHeroQuestInteraction(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelWisdomTempleObjectInteraction(bSendEvent: false);
		CancelWarMemoryTransformationObjectInteraction(bSendEventToMyself: false, bSendEventToOthers: false);
	}

	private void LogOut_Friend()
	{
		CancelAllFriendApplications();
		RefuseAllFriendApplications();
	}

	private void LogOut_JobChangeQuest()
	{
		if (m_jobChangeQuest != null && m_jobChangeQuest.limitTimer != null)
		{
			m_jobChangeQuest.DisposeLimitTimer();
		}
	}

	protected override void InvokeRunWorkInternal(ISFRunnable work)
	{
		HeroSynchronizer.Exec(this, new SFAction<ISFRunnable>(base.RunWorkInternal, work));
	}

	private void DisposeUpdateTimer()
	{
		if (m_updateTimer != null)
		{
			m_updateTimer.Dispose();
			m_updateTimer = null;
		}
	}

	private void OnUpdateTimerTick(object state)
	{
		AddWork(new SFAction(base.OnUpdate), bGlobalLockRequired: false);
	}

	protected override void ReleaseInternal()
	{
		DisposeUpdateTimer();
		foreach (PartyApplication app in m_partyApplications.Values)
		{
			app.Release();
		}
		DisposeDistortionTimer();
		DisposeReturnScrollUseTimer();
		DisposeFishingTimer();
		DisposeStealTimer();
		DisposeTrueHeroQuestInteractionTimer();
		if (m_mysteryBoxQuest != null)
		{
			m_mysteryBoxQuest.Release();
		}
		if (m_secretLetterQuest != null)
		{
			m_secretLetterQuest.Release();
		}
		if (m_dimensionRaidQuest != null)
		{
			m_dimensionRaidQuest.Release();
		}
		if (m_guildFarmQuest != null)
		{
			m_guildFarmQuest.Release();
		}
		if (m_guildAltarSpellInjectionMission != null)
		{
			m_guildAltarSpellInjectionMission.Release();
		}
		if (m_trueHeroQuest != null)
		{
			m_trueHeroQuest.Release();
		}
		if (m_supplySupportQuest != null)
		{
			m_supplySupportQuest.Release();
		}
		base.ReleaseInternal();
	}

	protected override void OnUpdateInternal()
	{
		if (isLoggedIn)
		{
			base.OnUpdateInternal();
			OnUpdate_ManageBattleMode();
			OnUpdate_AutoRecoveryStamina();
			OnUpdate_UpdateUndergroundMazePlayTime();
			OnUpdate_UpdateArtifactRoomSweepFloor();
			OnUpdate_ProcessMainQuestForMove();
			OnUpdate_ProcessTreatOfFarmQuestMissionForMonsterSpawn();
			OnUpdate_ProcessJobChangeQuestForMonsterSpawn();
			OnUpdate_UpdateSceneryQuest();
			OnUpdate_ManageAccelerationRequiredMoveTime();
			OnUpdate_CheckCostumePeriodLimitTime();
			OnUpdate_ManageSafeModeRequiredTime();
		}
	}

	protected override void OnDateChanged()
	{
		base.OnDateChanged();
		OnDateChanged_RefreshTodayMissionList();
		OnDateChanged_RefreshWeeklyQuest();
	}

	private void OnUpdate_ManageBattleMode()
	{
		try
		{
			ManageBattleMode();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void ManageBattleMode()
	{
		if (m_bIsBattleMode)
		{
			DateTimeOffset currentTime = DateTimeUtil.currentTime;
			if (!((currentTime - m_battleModeStartTime).TotalSeconds < (double)Resource.instance.battleModeDuration))
			{
				FinishedBattleMode();
			}
		}
	}

	private void OnUpdate_AutoRecoveryStamina()
	{
		try
		{
			AutoRecoveryStamina(m_currentUpdateTime, bSendEvent: true);
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void AutoRecoveryStamina(DateTimeOffset time, bool bSendEvent)
	{
		if (isFullStamina)
		{
			return;
		}
		if (!m_staminaUpdateTime.HasValue)
		{
			m_staminaUpdateTime = time;
			return;
		}
		TimeSpan staminaRecoveryTime = time - m_staminaUpdateTime.Value;
		int nStaminaRecoveryTime = Resource.instance.staminaRecoveryTime;
		if (!(staminaRecoveryTime.TotalSeconds < (double)nStaminaRecoveryTime))
		{
			int nStaminaRecoverySeconds = (int)Math.Floor(staminaRecoveryTime.TotalSeconds);
			int nQuotient = nStaminaRecoverySeconds / nStaminaRecoveryTime;
			m_staminaUpdateTime = m_staminaUpdateTime.Value.AddSeconds(nQuotient * nStaminaRecoveryTime);
			RecorveryStamina(nQuotient, time);
			if (bSendEvent)
			{
				ServerEvent.SendStaminaAutoRecovery(m_account.peer, m_nStamina);
			}
		}
	}

	public void RecoveryStaminaOnSchedule(DateTime date, int nScheduleId, int nAmount, bool bSendEvent, DateTimeOffset currentTime)
	{
		m_staminaRecoverySchedule.date = date;
		m_staminaRecoverySchedule.value = nScheduleId;
		if (!isFullStamina)
		{
			RecorveryStamina(nAmount, currentTime);
		}
		if (bSendEvent)
		{
			ServerEvent.SendStaminaScheduleRecovery(m_account.peer, m_nStamina);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Stamina(m_id, m_nStamina));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_StaminaRecoverySchedule(m_id, m_staminaRecoverySchedule.date, m_staminaRecoverySchedule.value));
		dbWork.Schedule();
	}

	public void RecorveryStamina(int nAmount, DateTimeOffset currentTime)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount == 0)
		{
			return;
		}
		int nOldStamina = m_nStamina;
		AddStamina(nAmount, bOverflowEnabled: false);
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroStaminaRecoveryLog(Guid.NewGuid(), m_id, nAmount, nOldStamina, m_nStamina, currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public float GetStaminaRecoveryRemainingTime(DateTimeOffset time)
	{
		float fRemainingTime = 0f;
		if (m_staminaUpdateTime.HasValue)
		{
			fRemainingTime = (float)((double)Resource.instance.staminaRecoveryTime - (time - m_staminaUpdateTime.Value).TotalSeconds);
			if (fRemainingTime < 0f)
			{
				fRemainingTime = 0f;
			}
		}
		return fRemainingTime;
	}

	public void UseStamina(int nAmount, DateTimeOffset time)
	{
		if (nAmount > 0)
		{
			bool bOldIsFull = isFullStamina;
			m_nStamina -= nAmount;
			if (m_nStamina <= 0)
			{
				m_nStamina = 0;
			}
			if (bOldIsFull && !isFullStamina)
			{
				m_staminaUpdateTime = time;
			}
		}
	}

	public void AddStamina(int nAmount, bool bOverflowEnabled)
	{
		if (nAmount > 0)
		{
			if (bOverflowEnabled)
			{
				m_nStamina += nAmount;
			}
			else
			{
				m_nStamina = Math.Min(m_nStamina + nAmount, Resource.instance.maxStamina);
			}
		}
	}

	public void RefreshDailyStaminaBuyCount(DateTime date)
	{
		if (!(date == m_dailyStaminaBuyCount.date))
		{
			m_dailyStaminaBuyCount.date = date;
			m_dailyStaminaBuyCount.value = 0;
		}
	}

	private void OnUpdate_UpdateUndergroundMazePlayTime()
	{
		try
		{
			UpdateUndergroundMazePlayTime(m_currentUpdateTime);
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void UpdateUndergroundMazePlayTime(DateTimeOffset time)
	{
		if (m_currentPlace is UndergroundMazeInstance undergroundMazeInst && !(m_fUndergroundMazePlayTime >= (float)Resource.instance.undergroundMaze.limitTime))
		{
			DateTime lastTickDate = m_undergroundMazeLastTickTime.Date;
			DateTime currentDate = time.Date;
			RefreshUndergroundMazeDailyPlayTime(currentDate);
			if (m_undergroundMazeDate == lastTickDate)
			{
				m_fUndergroundMazePlayTime += (float)(time - m_undergroundMazeLastTickTime).TotalSeconds;
			}
			else
			{
				m_fUndergroundMazePlayTime = (float)time.TimeOfDay.TotalSeconds;
			}
			m_undergroundMazeLastTickTime = time;
			if (m_fUndergroundMazePlayTime >= (float)Resource.instance.undergroundMaze.limitTime)
			{
				undergroundMazeInst.OnHeroLimitTimeExpired(this);
			}
		}
	}

	private void OnUpdate_UpdateArtifactRoomSweepFloor()
	{
		try
		{
			UpdateArtifactRoomSweepFloor(m_currentUpdateTime, bSendEvent: true);
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void UpdateArtifactRoomSweepFloor(DateTimeOffset time, bool bSendEvent)
	{
		if (!m_artifactRoomSweepStartTime.HasValue || m_bIsArtifactRoomSweepCompleted)
		{
			return;
		}
		float fElapsedTime = (float)(time - m_artifactRoomSweepStartTime.Value).TotalSeconds;
		int nTotalDuration = 0;
		int nProgressFloor = 0;
		foreach (ArtifactRoomFloor floor in Resource.instance.artifactRoom.floors)
		{
			if (floor.floor > m_nArtifactRoomBestFloor)
			{
				break;
			}
			if (floor.floor >= m_nArtifactRoomCurrentFloor)
			{
				nTotalDuration += floor.sweepDuration;
				if (fElapsedTime < (float)nTotalDuration)
				{
					nProgressFloor = floor.floor;
					break;
				}
			}
		}
		if (fElapsedTime < (float)nTotalDuration)
		{
			if (nProgressFloor != m_nArtifactRoomSweepProgressFloor)
			{
				m_nArtifactRoomSweepProgressFloor = nProgressFloor;
				if (bSendEvent)
				{
					ServerEvent.SendArtifactRoomSweepNextFloorStart(m_account.peer, m_nArtifactRoomSweepProgressFloor);
				}
			}
		}
		else
		{
			m_nArtifactRoomSweepProgressFloor = m_nArtifactRoomBestFloor;
			m_bIsArtifactRoomSweepCompleted = true;
			if (bSendEvent)
			{
				ServerEvent.SendArtifactRoomSweepCompleted(m_account.peer);
			}
		}
	}

	public void SetCurrentPlace(Place place)
	{
		m_currentPlace = place;
	}

	public void AddExp(long lnAmount, bool bSendExpAcquisitionEvent, bool bSaveToDBForLevelUp)
	{
		if (lnAmount < 0)
		{
			throw new ArgumentOutOfRangeException("lnAmount");
		}
		if (lnAmount == 0)
		{
			return;
		}
		m_lnExp += lnAmount;
		JobLevelMaster jobLevelMaster = m_job.GetLevel(m_nLevel).master;
		if (jobLevelMaster.isMaxLevel)
		{
			return;
		}
		int nOldLevel = m_nLevel;
		long lnNextLevelUpExp = 0L;
		do
		{
			lnNextLevelUpExp = jobLevelMaster.nextLevelUpExp;
			if (m_lnExp < lnNextLevelUpExp)
			{
				break;
			}
			m_lnExp -= lnNextLevelUpExp;
			m_nLevel++;
			jobLevelMaster = m_job.GetLevel(m_nLevel).master;
		}
		while (!jobLevelMaster.isMaxLevel);
		if (nOldLevel != m_nLevel)
		{
			OnLevelUp(nOldLevel, bSaveToDBForLevelUp);
		}
		if (bSendExpAcquisitionEvent)
		{
			ServerEvent.SendExpAcquisition(m_account.peer, lnAmount, m_nLevel, m_lnExp, m_nRealMaxHP, m_nHP);
		}
	}

	private void OnLevelUp(int nOldLevel, bool bSaveToDBForLevelUp)
	{
		m_levelUpdateTime = DateTimeUtil.currentTime;
		RefreshRealValues(bSendMaxHpChangedToOthers: true);
		if (!base.isDead)
		{
			m_nHP = m_nRealMaxHP;
		}
		JobLevelMaster jobLevelMaster = m_job.GetLevel(m_nLevel).master;
		int nSlotChangeCount = jobLevelMaster.inventorySlotAccCount + m_nPaidInventorySlotCount - m_inventorySlots.Count;
		if (nSlotChangeCount > 0)
		{
			CreateInventorySlots(nSlotChangeCount);
		}
		OnLevelUp_WingOpen();
		InitDailyQuests(bSendEvent: true);
		InitWeeklyQuest(m_levelUpdateTime, bSendEvent: true);
		ProcessAutoAcceptedSubQuestsForHeroLevel(nOldLevel, m_levelUpdateTime);
		OnLevelUp_OrdealQuest(m_levelUpdateTime);
		OnLevelUp_ProspectQuest();
		OnLevelUp_ProcessJoinNationWar(m_levelUpdateTime);
		OnLevelUp_Artifact();
		OnLevelUp_Constellation();
		if (bSaveToDBForLevelUp)
		{
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(this));
			dbWork.Schedule();
		}
		if (m_currentPlace != null)
		{
			ServerEvent.SendHeroLevelUp(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_nLevel, m_nRealMaxHP, m_nHP);
		}
		Cache.instance.OnHeroLevelUp(this, nOldLevel);
	}

	private void OnLevelUp_WingOpen()
	{
		if (m_nLevel < Resource.instance.wingOpenRequiredHeroLevel)
		{
			return;
		}
		Wing wing = Resource.instance.wingOpenProvideWing;
		if (wing != null && !ContainsWing(wing.id))
		{
			HeroWing heroWing = new HeroWing(this, wing);
			heroWing.Init();
			AddWing(heroWing);
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroWing(m_id, wing.id, heroWing.memoryPieceStep));
			dbWork.Schedule();
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWingOpenLog(Guid.NewGuid(), m_id, wing.id, DateTimeUtil.currentTime));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex);
			}
			ServerEvent.SendWingAcquisition(m_account.peer, heroWing.ToPDHeroWing());
		}
	}

	private void OnLevelUp_OrdealQuest(DateTimeOffset time)
	{
		int nQuestNo = 0;
		if (m_ordealQuest != null)
		{
			if (!m_ordealQuest.completed)
			{
				return;
			}
			nQuestNo = m_ordealQuest.no + 1;
		}
		else
		{
			nQuestNo = 1;
		}
		OrdealQuest ordealQuest = Resource.instance.GetOrdealQuest(nQuestNo);
		if (ordealQuest != null && ordealQuest.requiredHeroLevel <= m_nLevel)
		{
			m_ordealQuest = new HeroOrdealQuest(this);
			m_ordealQuest.Start(ordealQuest, time);
			ServerEvent.SendOrdealQuestAccepted(m_account.peer, m_ordealQuest.ToPDHeroOrdealQuest(time));
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroOrdealQuest(m_ordealQuest.hero.id, m_ordealQuest.no, time));
			HeroOrdealQuestSlot[] slots = m_ordealQuest.slots;
			foreach (HeroOrdealQuestSlot slot in slots)
			{
				dbWork.AddSqlCommand(GameDac.CSC_AddHeroOrdealQuestMission(slot.quest.hero.id, slot.quest.no, slot.index, slot.mission.no, time));
			}
			dbWork.Schedule();
		}
	}

	private void OnLevelUp_ProspectQuest()
	{
		foreach (HeroProspectQuest quest in m_targetProspectQuests.Values)
		{
			quest.OnTargetLevelUp(m_nLevel);
		}
	}

	private void OnLevelUp_ProcessJoinNationWar(DateTimeOffset time)
	{
		AddWork(new SFAction<DateTimeOffset>(ProcessJoinNationWar, time), bGlobalLockRequired: true);
	}

	private void ProcessJoinNationWar(DateTimeOffset time)
	{
		if (!(m_currentPlace is NationContinentInstance nationContinentInst))
		{
			return;
		}
		int nNationId = nationContinentInst.nationId;
		NationWarInstance nationWarInst = Cache.instance.GetNationInstance(nNationId).nationWarInst;
		if (nationWarInst != null && nNationId == nationWarInst.defenseNation.id && nationContinentInst.continent.isNationWarTarget && !nationWarInst.ContainsHero(this) && m_nLevel >= Resource.instance.pvpMinHeroLevel)
		{
			if (isDistorting)
			{
				CancelDistortion(bSendEventToMyself: true);
			}
			nationWarInst.AddHero(this, time);
		}
	}

	protected override void RefreshMoveSpeed()
	{
		int nOldMoveSpeed = m_nMoveSpeed;
		if (m_bIsRiding)
		{
			m_nMoveSpeed = (int)Math.Floor(m_equippedMount.mount.moveSpeed);
		}
		else if (m_bAcceleration)
		{
			m_nMoveSpeed = Resource.instance.accelerationMoveSpeed;
		}
		else if (isTransformRuinsReclaimMonster)
		{
			if (m_bWalking)
			{
				m_nMoveSpeed = m_ruinsReclaimTransformationMonsterEffect.transformationMonster.moveSpeed;
			}
			else
			{
				m_nMoveSpeed = m_ruinsReclaimTransformationMonsterEffect.transformationMonster.battleMoveSpeed;
			}
		}
		else if (isTransformWarMemoryMonster)
		{
			if (m_bWalking)
			{
				m_nMoveSpeed = m_warMemoryTransformationMonsterEffect.transformationMonster.moveSpeed;
			}
			else
			{
				m_nMoveSpeed = m_warMemoryTransformationMonsterEffect.transformationMonster.battleMoveSpeed;
			}
		}
		else if (isTransformMainQuestMonster)
		{
			if (m_bWalking)
			{
				m_nMoveSpeed = m_mainQuestTransformationMonsterEffect.transformationMonster.moveSpeed;
			}
			else
			{
				m_nMoveSpeed = m_mainQuestTransformationMonsterEffect.transformationMonster.battleMoveSpeed;
			}
		}
		else if (m_bWalking)
		{
			m_nMoveSpeed = m_job.walkMoveSpeed;
		}
		else
		{
			m_nMoveSpeed = m_job.moveSpeed;
		}
		int nChangeMoveSpeed = 0;
		foreach (AbnormalStateEffect effect in m_abnormalStateEffects.Values)
		{
			if (effect.abnormalState.id == 11)
			{
				nChangeMoveSpeed += effect.abnormalStateLevel.value1;
			}
		}
		if (m_ancientRelicAbnormalStateEffect != null)
		{
			nChangeMoveSpeed += m_ancientRelicAbnormalStateEffect.moveSpeed;
		}
		if (m_dragonNestTrapEffect != null)
		{
			nChangeMoveSpeed += m_dragonNestTrapEffect.moveSpeed;
		}
		m_nMoveSpeed += (int)Math.Floor((float)m_nMoveSpeed * ((float)nChangeMoveSpeed / 10000f));
		if (m_nMoveSpeed < 0)
		{
			m_nMoveSpeed = 0;
		}
		if (!(m_moveVerificationStartTime == DateTimeOffset.MinValue) && nOldMoveSpeed != m_nMoveSpeed && m_bMoving)
		{
			DateTimeOffset currentTime = DateTimeUtil.currentTime;
			VerifyMoveSpeed(currentTime);
			StartMoveVerification(currentTime);
		}
	}

	public override void RefreshRealValues(bool bSendMaxHpChangedToOthers)
	{
		base.RefreshRealValues(bSendMaxHpChangedToOthers);
	}

	protected override void RefreshRealValues_Sum()
	{
		base.RefreshRealValues_Sum();
		if (isTransformWarMemoryMonster)
		{
			Monster warMemoryTransformationMonster = m_warMemoryTransformationMonsterEffect.transformationMonster;
			m_nRealMaxHP += warMemoryTransformationMonster.maxHP;
			m_nRealPhysicalOffense += warMemoryTransformationMonster.physicalOffense;
			return;
		}
		if (isTransformMainQuestMonster)
		{
			Monster mainQuestTransformationMonster = m_mainQuestTransformationMonsterEffect.transformationMonster;
			m_nRealMaxHP += mainQuestTransformationMonster.maxHP;
			m_nRealPhysicalOffense += mainQuestTransformationMonster.physicalOffense;
			return;
		}
		if (isReal)
		{
			JobLevel jobLevel = m_job.GetLevel(m_nLevel);
			m_nRealMaxHP += jobLevel.maxHp;
			m_nRealPhysicalOffense += jobLevel.physicalOffense;
			m_nRealMagicalOffense += jobLevel.magicalOffense;
			m_nRealPhysicalDefense += jobLevel.physicalDefense;
			m_nRealMagicalDefense += jobLevel.magicalDefense;
			m_nRealBaseMaxHP = m_nRealMaxHP;
			m_nRealBasePhysicalOffense = m_nRealPhysicalOffense;
			m_nRealBaseMagicalOffense = m_nRealMagicalOffense;
			m_nRealBasePhysicalDefense = m_nRealPhysicalDefense;
			m_nRealBaseMagicalDefense = m_nRealMagicalDefense;
		}
		if (!isReal)
		{
			m_nRealMaxHP += m_nNormalMaxHP;
			m_nRealPhysicalOffense += m_nNormalPhysicalOffense;
			m_nRealMagicalOffense += m_nNormalMagicalOffense;
			m_nRealPhysicalDefense += m_nNormalPhysicalDefense;
			m_nRealMagicalDefense += m_nNormalMagicalDefense;
			m_nRealCritical += m_nNormalCritical;
			m_nRealCriticalResistance += m_nNormalCriticalResistance;
			m_nRealCriticalDamageIncRate += m_nNormalCriticalDamageIncRate;
			m_nRealCriticalDamageDecRate += m_nNormalCriticalDamageDecRate;
			m_nRealPenetration += m_nNormalPenetration;
			m_nRealBlock += m_nNormalBlock;
			m_nRealFireOffense += m_nNormalFireOffense;
			m_nRealFireDefense += m_nNormalFireDefense;
			m_nRealLightningOffense += m_nNormalLightningOffense;
			m_nRealLightningDefense += m_nNormalLightningDefense;
			m_nRealDarkOffense += m_nNormalDarkOffense;
			m_nRealDarkDefense += m_nNormalDarkDefense;
			m_nRealHolyOffense += m_nNormalHolyOffense;
			m_nRealHolyDefense += m_nNormalHolyDefense;
			m_nRealDamageIncRate += m_nNormalDamageIncRate;
			m_nRealDamageDecRate += m_nNormalDamageDecRate;
			m_nRealStunResistance += m_nNormalStunResistance;
			m_nRealSnareResistance += m_nNormalSnareResistance;
			m_nRealSilenceResistance += m_nNormalSilenceResistance;
			m_nRealBaseMaxHPIncRate += m_nNormalBaseMaxHPIncRate;
			m_nRealBaseOffenseIncRate += m_nNormalBaseOffenseIncRate;
			m_nRealBasePhysicalDefenseIncRate += m_nNormalBasePhysicalDefenseIncRate;
			m_nRealBaseMagicalDefenseIncRate += m_nNormalBaseMagicalDefenseIncRate;
			m_nRealOffense += m_nNormalOffense;
		}
		if (!isReal)
		{
			return;
		}
		if (m_rank != null)
		{
			foreach (RankAttr attr14 in m_rank.attrs.Values)
			{
				AddAttrRealValue(attr14.id, attr14.attrValue.value);
			}
		}
		foreach (HeroRankPassiveSkill skill2 in m_rankPassiveSkills.Values)
		{
			foreach (RankPassiveSkillAttrLevel attrLevel in skill2.skillLevel.attrLevels.Values)
			{
				AddAttrRealValue(attrLevel.attr.id, attrLevel.value);
			}
		}
		foreach (HeroMount mount in m_mounts.Values)
		{
			MountAwakeningLevelMaster mountAwakeningLevelMaster = mount.mount.GetAwakeningLevel(mount.awakeningLevel).levelMaster;
			float fFactor2 = 0f;
			fFactor2 = ((!mount.isEquipped) ? mountAwakeningLevelMaster.unequippedAttrFacter : Resource.instance.equippedMountAttrFactor);
			foreach (AttrValuePair attrValue6 in mount.attrTotalValues.Values)
			{
				AddAttrRealValue(attrValue6.id, (int)((float)attrValue6.value * fFactor2));
			}
		}
		RefreshValues_Sum_MainGears();
		RefreshValues_Sum_SubGears();
		RefreshValues_Sum_MountGears();
		foreach (HeroWing heroWing in m_wings.Values)
		{
			Wing wing = heroWing.wing;
			AddAttrRealValue(wing.attrId, wing.attrValue.value);
			if (!wing.memoryPieceInstallationEnabled)
			{
				continue;
			}
			foreach (AttrValuePair attrValue5 in heroWing.attrTotalValues.Values)
			{
				AddAttrRealValue(attrValue5.id, attrValue5.value);
			}
		}
		foreach (HeroWingPart heroWingPart in m_wingParts.Values)
		{
			foreach (AttrValuePair attrValue4 in heroWingPart.attrTotalValues.Values)
			{
				AddAttrRealValue(attrValue4.id, attrValue4.value);
			}
		}
		if (m_mainGearEnchantLevelSet != null)
		{
			foreach (MainGearEnchantLevelSetAttr attr13 in m_mainGearEnchantLevelSet.attrs)
			{
				AddAttrRealValue(attr13.attrId, attr13.attrValue.value);
			}
		}
		if (m_subGearSoulstoneLevelSet != null)
		{
			foreach (SubGearSoulstoneLevelSetAttr attr12 in m_subGearSoulstoneLevelSet.attrs)
			{
				AddAttrRealValue(attr12.attrId, attr12.attrValue.value);
			}
		}
		RefreshValues_Sum_GuildSkills();
		if (m_nationNoblesse != null)
		{
			foreach (NationNoblesseAttr attr11 in m_nationNoblesse.attrs)
			{
				AddAttrRealValue(attr11.id, attr11.attrValue.value);
			}
		}
		if (m_illustratedBookExplorationStep != null)
		{
			foreach (IllustratedBookExplorationStepAttr attr10 in m_illustratedBookExplorationStep.attrs)
			{
				AddAttrRealValue(attr10.id, attr10.attrValue.value);
			}
		}
		foreach (HeroIllustratedBook heroIllustratedBook in m_illustratedBooks.Values)
		{
			foreach (IllustratedBookAttr attr9 in heroIllustratedBook.illustratedBook.attrs)
			{
				AddAttrRealValue(attr9.id, attr9.attrValue.value);
			}
		}
		foreach (HeroTitle heroTitle in m_titles.Values)
		{
			if (heroTitle.isActivated)
			{
				foreach (TitleActiveAttr attr8 in heroTitle.title.activeAttrs)
				{
					AddAttrRealValue(attr8.attrId, attr8.value);
				}
				continue;
			}
			foreach (TitlePassiveAttr attr7 in heroTitle.title.passiveAttrs)
			{
				AddAttrRealValue(attr7.attrId, attr7.value);
			}
		}
		foreach (HeroCreatureCardCollection heroCreatureCardCollection in m_activatedCreatureCardCollections.Values)
		{
			foreach (CreatureCardCollectionAttr attr6 in heroCreatureCardCollection.collection.attrs)
			{
				AddAttrRealValue(attr6.attrId, attr6.value);
			}
		}
		foreach (HeroEliteMonsterKill heroEliteMonsterKill in m_heroEliteMonsterKills.Values)
		{
			EliteMonster eliteMonster = heroEliteMonsterKill.eliteMonster;
			EliteMonsterKillAttrValue attrValue3 = eliteMonster.GetKillAttrValue_LessThanOrEqualValue(heroEliteMonsterKill.killCount);
			if (attrValue3 != null)
			{
				AddAttrRealValue(eliteMonster.attrId, attrValue3.value);
			}
		}
		foreach (HeroCreature creature in m_creatures.Values)
		{
			float fFactor = 0f;
			if (creature.participated)
			{
				fFactor = 1f;
			}
			else
			{
				if (!creature.cheered)
				{
					continue;
				}
				fFactor = Resource.instance.creatureCheerAttrFactor;
			}
			foreach (HeroCreatureBaseAttr baseAttr in creature.baseAttrs.Values)
			{
				CreatureBaseAttrValue attrValue2 = baseAttr.attr;
				int nIncValue = (int)((float)(creature.level * attrValue2.incAttrValue) * ((float)creature.quality / 1000f));
				AddAttrRealValue(attrValue2.attr.attrId, (int)((float)(baseAttr.baseValue + nIncValue) * fFactor));
			}
			foreach (HeroCreatureAdditionalAttr additionalAttr in creature.additionalAttrs)
			{
				CreatureAdditionalAttrValue attrValue = additionalAttr.attr.GetAttrValue(creature.injectionLevel);
				AddAttrRealValue(attrValue.attr.attrId, (int)((float)attrValue.attrValue.value * fFactor));
			}
			HeroCreatureSkill[] array = creature.skills;
			foreach (HeroCreatureSkill skill in array)
			{
				if (skill.skillAttr != null)
				{
					CreatureSkillAttr attr5 = skill.skillAttr;
					AddAttrRealValue(attr5.skill.attrId, (int)((float)attr5.attrValue.value * fFactor));
				}
			}
		}
		foreach (HeroPotionAttr heroPotionAttr in m_potionAttrs.Values)
		{
			PotionAttr potionAttr = heroPotionAttr.potionAttr;
			AddAttrRealValue(potionAttr.attrId, potionAttr.incAttrValue * heroPotionAttr.count);
		}
		foreach (HeroConstellation constellation in m_constellations.Values)
		{
			foreach (HeroConstellationStep step in constellation.steps)
			{
				ConstellationStep constellationStep = step.entry.cycle.step;
				ConstellationCycle constellationCycle = null;
				constellationCycle = ((!step.entry.isLastEntry || !step.activated) ? constellationStep.GetCycle(step.entry.cycle.cycle - 1) : step.entry.cycle);
				if (constellationCycle != null)
				{
					foreach (ConstellationCycleBuff cylceBuff in constellationCycle.buffs)
					{
						AddAttrRealValue(cylceBuff.attrId, cylceBuff.attrValue);
					}
				}
				ConstellationEntry constellationEntry = null;
				constellationEntry = ((!step.activated) ? step.entry.cycle.GetEntry(step.entry.no - 1) : step.entry);
				if (constellationEntry == null)
				{
					continue;
				}
				foreach (ConstellationEntryBuff entryBuff in constellationEntry.buffs)
				{
					AddAttrRealValue(entryBuff.attrId, entryBuff.attrValue);
				}
			}
		}
		if (m_nArtifactNo > 0)
		{
			Artifact artifact = null;
			ArtifactLevel level = null;
			for (int nArtifactNo = 1; nArtifactNo < m_nArtifactNo; nArtifactNo++)
			{
				artifact = Resource.instance.GetArtifact(nArtifactNo);
				if (artifact == null)
				{
					continue;
				}
				level = artifact.lastLevel;
				if (level == null)
				{
					continue;
				}
				foreach (ArtifactLevelAttr attr3 in level.attrs.Values)
				{
					AddAttrRealValue(attr3.id, attr3.value);
				}
			}
			artifact = Resource.instance.GetArtifact(m_nArtifactNo);
			if (artifact != null)
			{
				level = artifact.GetLevel(m_nArtifactLevel);
				if (level != null)
				{
					foreach (ArtifactLevelAttr attr4 in level.attrs.Values)
					{
						AddAttrRealValue(attr4.id, attr4.value);
					}
				}
			}
		}
		foreach (HeroCostume heroCostume in m_costumes.Values)
		{
			Costume costume = heroCostume.costume;
			foreach (CostumeAttr attr2 in costume.attrs.Values)
			{
				CostumeEnchantLevelAttr levelAttr = attr2.GetEnchantLevelAttr(heroCostume.enchantLevel);
				AddAttrRealValue(attr2.attrId, levelAttr.attrValue);
			}
		}
		if (m_costumeCollection == null || !m_bCostumeCollectionActivated)
		{
			return;
		}
		foreach (CostumeCollectionAttr attr in m_costumeCollection.attrs.Values)
		{
			AddAttrRealValue(attr.attrId, attr.attrValue);
		}
	}

	private void RefreshValues_Sum_MainGears()
	{
		if (m_equippedWeapon != null)
		{
			AddRealValue_Sum_HeroMainGear(m_equippedWeapon);
		}
		if (m_equippedArmor != null)
		{
			AddRealValue_Sum_HeroMainGear(m_equippedArmor);
		}
		if (m_equippedWeapon == null || m_equippedArmor == null || m_equippedWeapon.gear.tier != m_equippedArmor.gear.tier || m_equippedWeapon.gear.grade != m_equippedArmor.gear.grade || m_equippedWeapon.gear.quality != m_equippedArmor.gear.quality)
		{
			return;
		}
		int nTier = m_equippedArmor.gear.tier.id;
		int nGrade = m_equippedArmor.gear.grade.id;
		int nQuality = m_equippedArmor.gear.quality.id;
		MainGearSet mainGearSet = Resource.instance.GetMainGearSet(nTier, nGrade, nQuality);
		if (mainGearSet == null)
		{
			return;
		}
		foreach (MainGearSetAttr setAttr in mainGearSet.attrs)
		{
			AddAttrRealValue(setAttr.attrId, setAttr.attrValue.value);
		}
	}

	private void RefreshValues_Sum_SubGears()
	{
		foreach (HeroSubGear heroSubGear in m_subGears.Values)
		{
			if (heroSubGear.equipped)
			{
				AddRealValue_Sum_HeroSubGear(heroSubGear);
			}
		}
	}

	private void RefreshValues_Sum_MountGears()
	{
		HeroMountGearSlot[] mountGearSlots = m_mountGearSlots;
		foreach (HeroMountGearSlot mountGearSlot in mountGearSlots)
		{
			if (!mountGearSlot.isEmpty)
			{
				AddRealValue_Sum_HeroMountGear(mountGearSlot.heroMountGear);
			}
		}
	}

	private void AddRealValue_Sum_HeroMainGear(HeroMainGear heroMainGear)
	{
		if (heroMainGear == null || !heroMainGear.equipable)
		{
			return;
		}
		foreach (AttrValuePair attrValuePair in heroMainGear.attrTotalValues.Values)
		{
			AddAttrRealValue(attrValuePair.id, attrValuePair.value);
		}
	}

	private void AddRealValue_Sum_HeroSubGear(HeroSubGear heroSubGear)
	{
		if (heroSubGear == null)
		{
			return;
		}
		foreach (AttrValuePair attrValuePair in heroSubGear.attrTotalValues.Values)
		{
			AddAttrRealValue(attrValuePair.id, attrValuePair.value);
		}
	}

	private void AddRealValue_Sum_HeroMountGear(HeroMountGear heroMountGear)
	{
		if (heroMountGear == null)
		{
			return;
		}
		foreach (AttrValuePair attrValuePair in heroMountGear.attrTotalValues.Values)
		{
			AddAttrRealValue(attrValuePair.id, attrValuePair.value);
		}
	}

	private void RefreshValues_Sum_GuildSkills()
	{
		if (m_guildMember == null)
		{
			return;
		}
		foreach (HeroGuildSkill heroSkill in m_guildSkills.Values)
		{
			GuildSkillLevel skillLevel = heroSkill.skill.GetLevel(heroSkill.realLevel);
			foreach (GuildSkillLevelAttrValue attrValue in skillLevel.attrValues.Values)
			{
				AddAttrRealValue(attrValue.attrId, attrValue.value);
			}
		}
	}

	protected override void RefreshRealValues_Multiplication()
	{
		base.RefreshRealValues_Multiplication();
		if (isTransformWarMemoryMonster)
		{
			WarMemoryMonsterAttrFactor attrFactor = m_warMemoryTransformationMonsterEffect.attrFactor;
			m_nRealMaxHP = (int)Math.Floor((float)m_nRealMaxHP * attrFactor.offenseFactor);
			m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * attrFactor.offenseFactor);
		}
		else if (isReal)
		{
			m_nRealMaxHP += (int)Math.Floor((float)m_nRealBaseMaxHP * ((float)m_nRealBaseMaxHPIncRate / 10000f));
			m_nRealPhysicalOffense += (int)Math.Floor((float)m_nRealBasePhysicalOffense * ((float)m_nRealBaseOffenseIncRate / 10000f));
			m_nRealMagicalOffense += (int)Math.Floor((float)m_nRealBaseMagicalOffense * ((float)m_nRealBaseOffenseIncRate / 10000f));
			m_nRealPhysicalDefense += (int)Math.Floor((float)m_nRealBasePhysicalDefense * ((float)m_nRealBasePhysicalDefenseIncRate / 10000f));
			m_nRealMagicalDefense += (int)Math.Floor((float)m_nRealBaseMagicalDefense * ((float)m_nRealBaseMagicalDefenseIncRate / 10000f));
		}
	}

	protected override void RefreshRealValues_LastApply()
	{
		base.RefreshRealValues_LastApply();
		if (!isTransformWarMemoryMonster)
		{
			if (isReal)
			{
				if (m_job.offenseType == OffenseType.Physical)
				{
					m_nRealPhysicalOffense += m_nRealOffense;
				}
				else
				{
					m_nRealMagicalOffense += m_nRealOffense;
				}
			}
			if (isReal)
			{
				RefreshBattlePower();
				RefreshNormalAttr();
			}
		}
		if (m_proofOfValorBuff != null)
		{
			float fOffenseFactor = m_proofOfValorBuff.offenseFactor;
			if (fOffenseFactor > 0f)
			{
				m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * fOffenseFactor);
				m_nRealMagicalOffense = (int)Math.Floor((float)m_nRealMagicalOffense * fOffenseFactor);
			}
			float fPhysicalDefenseFactor2 = m_proofOfValorBuff.physicalDefenseFactor;
			if (fPhysicalDefenseFactor2 > 0f)
			{
				m_nRealPhysicalDefense = (int)Math.Floor((float)m_nRealPhysicalDefense * fPhysicalDefenseFactor2);
			}
		}
		if (m_bRuinsReclaimDebuffEffect)
		{
			float fOffenseDebuffFactor = Resource.instance.ruinsReclaim.debuffAreaOffenseFactor;
			if (fOffenseDebuffFactor > 0f)
			{
				m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * fOffenseDebuffFactor);
				m_nRealMagicalOffense = (int)Math.Floor((float)m_nRealMagicalOffense * fOffenseDebuffFactor);
			}
		}
		if (m_infiniteWarBuff != null)
		{
			float fOffenseFactor2 = m_infiniteWarBuff.offenseFactor;
			if (fOffenseFactor2 > 0f)
			{
				m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * fOffenseFactor2);
				m_nRealMagicalOffense = (int)Math.Floor((float)m_nRealMagicalOffense * fOffenseFactor2);
			}
			float fPhysicalDefenseFactor = m_infiniteWarBuff.defenseFactor;
			if (fPhysicalDefenseFactor > 0f)
			{
				m_nRealPhysicalDefense = (int)Math.Floor((float)m_nRealPhysicalDefense * fPhysicalDefenseFactor);
			}
		}
		if (m_osirisRoomMoneyBuff != null)
		{
			foreach (MoneyBuffAttr attr3 in m_osirisRoomMoneyBuff.moneyBuff.attrs)
			{
				MultiplyRealValue(attr3.id, attr3.attrFactor);
			}
		}
		if (m_ankouTombMoneyBuff != null)
		{
			foreach (MoneyBuffAttr attr2 in m_ankouTombMoneyBuff.moneyBuff.attrs)
			{
				MultiplyRealValue(attr2.id, attr2.attrFactor);
			}
		}
		if (m_tradeShipMoneyBuff != null)
		{
			foreach (MoneyBuffAttr attr in m_tradeShipMoneyBuff.moneyBuff.attrs)
			{
				MultiplyRealValue(attr.id, attr.attrFactor);
			}
		}
		_ = m_nRealMaxHP;
		int nBasePhysicalOffense = m_nRealPhysicalOffense;
		int nBaseMagicalOffense = m_nRealMagicalOffense;
		int nBasePhysicalDefense = m_nRealPhysicalDefense;
		int nBaseMagicalDefense = m_nRealMagicalDefense;
		_ = m_nRealCritical;
		_ = m_nRealCriticalResistance;
		_ = m_nRealCriticalDamageIncRate;
		_ = m_nRealCriticalDamageDecRate;
		_ = m_nRealPenetration;
		_ = m_nRealBlock;
		int nBaseFireOffense = m_nRealFireOffense;
		int nBaseFireDefense = m_nRealFireDefense;
		int nBaseLightningOffense = m_nRealLightningOffense;
		int nBaseLightningDefense = m_nRealLightningDefense;
		int nBaseDarkOffense = m_nRealDarkOffense;
		int nBaseDarkDefense = m_nRealDarkDefense;
		int nBaseHolyOffense = m_nRealHolyOffense;
		int nBaseHolyDefense = m_nRealHolyDefense;
		_ = m_nRealDamageIncRate;
		_ = m_nRealDamageDecRate;
		_ = m_nRealStunResistance;
		_ = m_nRealSnareResistance;
		_ = m_nRealSilenceResistance;
		foreach (AbnormalStateEffect effect in m_abnormalStateEffects.Values)
		{
			AbnormalStateLevel abnormalStateLevel = effect.abnormalStateLevel;
			switch (effect.abnormalState.id)
			{
			case 5:
			{
				int nValue1 = abnormalStateLevel.value1;
				m_nRealPhysicalOffense += (int)Math.Floor((float)nBasePhysicalOffense * ((float)nValue1 / 10000f));
				m_nRealMagicalOffense += (int)Math.Floor((float)nBaseMagicalOffense * ((float)nValue1 / 10000f));
				break;
			}
			case 6:
			{
				int nValue2 = abnormalStateLevel.value1;
				m_nRealPhysicalDefense += (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue2 / 10000f));
				m_nRealMagicalDefense += (int)Math.Floor((float)nBaseMagicalDefense * ((float)nValue2 / 10000f));
				break;
			}
			case 7:
			{
				int nValue3 = abnormalStateLevel.value1;
				m_nRealFireDefense += (int)Math.Floor((float)nBaseFireDefense * ((float)nValue3 / 10000f));
				m_nRealLightningDefense += (int)Math.Floor((float)nBaseLightningDefense * ((float)nValue3 / 10000f));
				m_nRealDarkDefense += (int)Math.Floor((float)nBaseDarkDefense * ((float)nValue3 / 10000f));
				m_nRealHolyDefense += (int)Math.Floor((float)nBaseHolyDefense * ((float)nValue3 / 10000f));
				break;
			}
			case 13:
			{
				int nValue4 = abnormalStateLevel.value1;
				int nValue14 = abnormalStateLevel.value2;
				m_nRealCritical += nValue4;
				m_nRealCriticalDamageIncRate = nValue14;
				break;
			}
			case 103:
			{
				int nValue5 = abnormalStateLevel.value1;
				int nValue15 = abnormalStateLevel.value2;
				m_nRealPhysicalDefense += (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue5 / 10000f)) + nValue15;
				break;
			}
			case 104:
			{
				int nValue6 = abnormalStateLevel.value1;
				int nValue16 = abnormalStateLevel.value2;
				m_nRealPhysicalDefense += (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue6 / 10000f)) + nValue16;
				break;
			}
			case 105:
			{
				int nValue7 = abnormalStateLevel.value1;
				int nValue17 = abnormalStateLevel.value2;
				int nValue23 = abnormalStateLevel.value3;
				m_nRealPhysicalOffense = (int)Math.Floor((float)nBasePhysicalOffense * ((float)nValue7 / 10000f)) + nValue17;
				m_nRealPenetration = nValue23;
				break;
			}
			case 106:
			{
				int nValue8 = abnormalStateLevel.value1;
				int nValue18 = abnormalStateLevel.value2;
				int nValue24 = abnormalStateLevel.value3;
				m_nRealPhysicalOffense = (int)Math.Floor((float)nBasePhysicalOffense * ((float)nValue8 / 10000f)) + nValue18;
				m_nRealPenetration = nValue24;
				break;
			}
			case 107:
			{
				int nValue9 = abnormalStateLevel.value1;
				int nValue19 = abnormalStateLevel.value2;
				int nValue25 = abnormalStateLevel.value3;
				m_nRealPhysicalOffense = (int)Math.Floor((float)nBasePhysicalOffense * ((float)nValue9 / 10000f)) + nValue19;
				m_nRealCritical += nValue25;
				break;
			}
			case 108:
			{
				int nValue10 = abnormalStateLevel.value1;
				int nValue20 = abnormalStateLevel.value2;
				int nValue26 = abnormalStateLevel.value3;
				int nValue29 = abnormalStateLevel.value4;
				m_nRealMagicalOffense = (int)Math.Floor((float)nBaseMagicalOffense * ((float)nValue10 / 10000f)) + nValue20;
				m_nRealPhysicalDefense = (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue26 / 10000f)) + nValue29;
				m_nRealMagicalDefense = (int)Math.Floor((float)nBaseMagicalDefense * ((float)nValue26 / 10000f)) + nValue29;
				break;
			}
			case 109:
			{
				int nValue11 = abnormalStateLevel.value1;
				int nValue21 = abnormalStateLevel.value2;
				int nValue27 = abnormalStateLevel.value3;
				int nValue30 = abnormalStateLevel.value4;
				m_nRealMagicalOffense = (int)Math.Floor((float)nBaseMagicalOffense * ((float)nValue11 / 10000f)) + nValue21;
				m_nRealPhysicalDefense = (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue27 / 10000f)) + nValue30;
				m_nRealMagicalDefense = (int)Math.Floor((float)nBaseMagicalDefense * ((float)nValue27 / 10000f)) + nValue30;
				break;
			}
			case 110:
			{
				int nValue12 = abnormalStateLevel.value1;
				int nValue22 = abnormalStateLevel.value2;
				int nValue28 = abnormalStateLevel.value3;
				int nValue31 = abnormalStateLevel.value4;
				int nValue32 = abnormalStateLevel.value5;
				int nValue33 = abnormalStateLevel.value6;
				m_nRealMagicalOffense = (int)Math.Floor((float)nBaseMagicalOffense * ((float)nValue12 / 10000f)) + nValue22;
				m_nRealPhysicalDefense = (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue28 / 10000f)) + nValue31;
				m_nRealMagicalDefense = (int)Math.Floor((float)nBaseMagicalDefense * ((float)nValue28 / 10000f)) + nValue31;
				m_nRealFireOffense = (int)Math.Floor((float)nBaseFireOffense * ((float)nValue32 / 10000f)) + nValue33;
				m_nRealLightningOffense = (int)Math.Floor((float)nBaseLightningOffense * ((float)nValue32 / 10000f)) + nValue33;
				m_nRealDarkOffense = (int)Math.Floor((float)nBaseDarkOffense * ((float)nValue32 / 10000f)) + nValue33;
				m_nRealHolyOffense = (int)Math.Floor((float)nBaseHolyOffense * ((float)nValue32 / 10000f)) + nValue33;
				break;
			}
			case 111:
			{
				int nValue13 = abnormalStateLevel.value1;
				m_nRealPhysicalDefense = (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue13 / 10000f));
				m_nRealMagicalDefense = (int)Math.Floor((float)nBaseMagicalDefense * ((float)nValue13 / 10000f));
				break;
			}
			}
		}
	}

	public void RefreshBattlePower()
	{
		Resource res = Resource.instance;
		long lnOldBattlePower = m_lnBattlePower;
		int nFireOffenseBattlePower = m_nRealFireOffense * res.GetAttr(12).battlePowerFactor;
		int nLightningOffenseBattlePower = m_nRealLightningOffense * res.GetAttr(14).battlePowerFactor;
		int nDarkOffenseBattlePower = m_nRealDarkOffense * res.GetAttr(16).battlePowerFactor;
		int nHolyOffenseBattlePower = m_nRealHolyOffense * res.GetAttr(18).battlePowerFactor;
		int nElementalId = m_job.elementalId;
		if (res.GetElemental(nElementalId) != null)
		{
			switch (nElementalId)
			{
			case 1:
				nFireOffenseBattlePower = (int)Math.Floor((float)nFireOffenseBattlePower * 1.8f);
				break;
			case 2:
				nLightningOffenseBattlePower = (int)Math.Floor((float)nLightningOffenseBattlePower * 1.8f);
				break;
			case 3:
				nHolyOffenseBattlePower = (int)Math.Floor((float)nHolyOffenseBattlePower * 1.8f);
				break;
			case 4:
				nDarkOffenseBattlePower = (int)Math.Floor((float)nDarkOffenseBattlePower * 1.8f);
				break;
			}
		}
		m_lnBattlePower = m_nRealMaxHP * res.GetAttr(1).battlePowerFactor + m_nRealPhysicalOffense * res.GetAttr(2).battlePowerFactor + m_nRealMagicalOffense * res.GetAttr(3).battlePowerFactor + m_nRealPhysicalDefense * res.GetAttr(4).battlePowerFactor + m_nRealMagicalDefense * res.GetAttr(5).battlePowerFactor + m_nRealCritical * res.GetAttr(6).battlePowerFactor + m_nRealCriticalResistance * res.GetAttr(7).battlePowerFactor + m_nRealCriticalDamageIncRate * res.GetAttr(8).battlePowerFactor + m_nRealCriticalDamageDecRate * res.GetAttr(9).battlePowerFactor + m_nRealPenetration * res.GetAttr(10).battlePowerFactor + m_nRealBlock * res.GetAttr(11).battlePowerFactor + nFireOffenseBattlePower + m_nRealFireDefense * res.GetAttr(13).battlePowerFactor + nLightningOffenseBattlePower + m_nRealLightningDefense * res.GetAttr(15).battlePowerFactor + nDarkOffenseBattlePower + m_nRealDarkDefense * res.GetAttr(17).battlePowerFactor + nHolyOffenseBattlePower + m_nRealHolyDefense * res.GetAttr(19).battlePowerFactor + m_nRealDamageIncRate * res.GetAttr(20).battlePowerFactor + m_nRealDamageDecRate * res.GetAttr(21).battlePowerFactor + m_nRealStunResistance * res.GetAttr(22).battlePowerFactor + m_nRealSnareResistance * res.GetAttr(23).battlePowerFactor + m_nRealSilenceResistance * res.GetAttr(24).battlePowerFactor;
		foreach (HeroSkill heroSkill in m_skills.Values)
		{
			if (heroSkill.isOpened)
			{
				JobSkillLevel skillLevel = heroSkill.skill.GetLevel(heroSkill.level);
				m_lnBattlePower += skillLevel.battlePower;
			}
		}
		if (lnOldBattlePower == m_lnBattlePower)
		{
			return;
		}
		m_battlePowerUpdateTime = DateTimeUtil.currentTime;
		if (isReal)
		{
			if (m_lnBattlePower > m_lnMaxBattlePower)
			{
				m_lnMaxBattlePower = m_lnBattlePower;
			}
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_BattlePower(m_id, m_lnBattlePower, m_battlePowerUpdateTime));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_MaxBattlePower(m_id, m_lnMaxBattlePower));
			dbWork.Schedule();
		}
	}

	private void RefreshNormalAttr()
	{
		m_nNormalMaxHP = m_nRealMaxHP;
		m_nNormalPhysicalOffense = m_nRealPhysicalOffense;
		m_nNormalMagicalOffense = m_nRealMagicalOffense;
		m_nNormalPhysicalDefense = m_nRealPhysicalDefense;
		m_nNormalMagicalDefense = m_nRealMagicalDefense;
		m_nNormalCritical = m_nRealCritical;
		m_nNormalCriticalResistance = m_nRealCriticalResistance;
		m_nNormalCriticalDamageIncRate = m_nRealCriticalDamageIncRate;
		m_nNormalCriticalDamageDecRate = m_nRealCriticalDamageDecRate;
		m_nNormalPenetration = m_nRealPenetration;
		m_nNormalBlock = m_nRealBlock;
		m_nNormalFireOffense = m_nRealFireOffense;
		m_nNormalFireDefense = m_nRealFireDefense;
		m_nNormalLightningOffense = m_nRealLightningOffense;
		m_nNormalLightningDefense = m_nRealLightningDefense;
		m_nNormalDarkOffense = m_nRealDarkOffense;
		m_nNormalDarkDefense = m_nRealDarkDefense;
		m_nNormalHolyOffense = m_nRealHolyOffense;
		m_nNormalHolyDefense = m_nRealHolyDefense;
		m_nNormalDamageIncRate = m_nRealDamageIncRate;
		m_nNormalDamageDecRate = m_nRealDamageDecRate;
		m_nNormalStunResistance = m_nRealStunResistance;
		m_nNormalSnareResistance = m_nRealSnareResistance;
		m_nNormalSilenceResistance = m_nRealSilenceResistance;
		m_nNormalBaseMaxHPIncRate = m_nRealBaseMaxHPIncRate;
		m_nNormalBaseOffenseIncRate = m_nRealBaseOffenseIncRate;
		m_nNormalBasePhysicalDefenseIncRate = m_nRealBasePhysicalDefenseIncRate;
		m_nNormalBaseMagicalDefenseIncRate = m_nRealBaseMagicalDefenseIncRate;
		m_nNormalOffense = m_nRealOffense;
	}

	public List<PDAttrValuePair> GetPDAttrValuePairsForRealAttrValues()
	{
		List<PDAttrValuePair> insts = new List<PDAttrValuePair>();
		insts.Add(new PDAttrValuePair(1, m_nRealMaxHP));
		insts.Add(new PDAttrValuePair(2, m_nRealPhysicalOffense));
		insts.Add(new PDAttrValuePair(3, m_nRealMagicalOffense));
		insts.Add(new PDAttrValuePair(4, m_nRealPhysicalDefense));
		insts.Add(new PDAttrValuePair(5, m_nRealMagicalDefense));
		insts.Add(new PDAttrValuePair(6, m_nRealCritical));
		insts.Add(new PDAttrValuePair(7, m_nRealCriticalResistance));
		insts.Add(new PDAttrValuePair(8, m_nRealCriticalDamageIncRate));
		insts.Add(new PDAttrValuePair(9, m_nRealCriticalDamageDecRate));
		insts.Add(new PDAttrValuePair(10, m_nRealPenetration));
		insts.Add(new PDAttrValuePair(11, m_nRealBlock));
		insts.Add(new PDAttrValuePair(12, m_nRealFireOffense));
		insts.Add(new PDAttrValuePair(13, m_nRealFireDefense));
		insts.Add(new PDAttrValuePair(14, m_nRealLightningOffense));
		insts.Add(new PDAttrValuePair(15, m_nRealLightningDefense));
		insts.Add(new PDAttrValuePair(16, m_nRealDarkOffense));
		insts.Add(new PDAttrValuePair(17, m_nRealDarkDefense));
		insts.Add(new PDAttrValuePair(18, m_nRealHolyOffense));
		insts.Add(new PDAttrValuePair(19, m_nRealHolyDefense));
		insts.Add(new PDAttrValuePair(20, m_nRealDamageIncRate));
		insts.Add(new PDAttrValuePair(21, m_nRealDamageDecRate));
		insts.Add(new PDAttrValuePair(22, m_nRealStunResistance));
		insts.Add(new PDAttrValuePair(23, m_nRealSnareResistance));
		insts.Add(new PDAttrValuePair(24, m_nRealSilenceResistance));
		insts.Add(new PDAttrValuePair(25, m_nRealBaseMaxHPIncRate));
		insts.Add(new PDAttrValuePair(26, m_nRealBaseOffenseIncRate));
		insts.Add(new PDAttrValuePair(27, m_nRealBasePhysicalDefenseIncRate));
		insts.Add(new PDAttrValuePair(28, m_nRealBaseMagicalDefenseIncRate));
		insts.Add(new PDAttrValuePair(29, m_nRealOffense));
		return insts;
	}

	public void StartMove(bool bIsWalking, bool bIsManualMoving)
	{
		if (!m_bMoving)
		{
			m_moveStartTime = DateTimeUtil.currentTime;
			m_bMoving = true;
			if (bIsWalking)
			{
				StartWalking();
			}
			else
			{
				StartRunning(m_moveStartTime);
			}
			if (bIsManualMoving)
			{
				StartManualMoving(m_moveStartTime);
			}
			StartMoveVerification(m_moveStartTime);
		}
	}

	public void EndMove()
	{
		if (m_bMoving)
		{
			m_bMoving = false;
			StopManualMoving();
			StopAcceleration();
			VerifyMoveSpeed(DateTimeUtil.currentTime);
		}
	}

	private void InitMoveStartTime(DateTimeOffset time)
	{
		if (m_bMoving)
		{
			m_moveStartTime = DateTimeUtil.currentTime;
			if (m_bManualMoving)
			{
				m_manualMoveStartTime = time;
			}
		}
	}

	private void StartMoveVerification(DateTimeOffset time)
	{
		m_moveVerificationStartTime = time;
		m_fMoveVerificationDistance = 0f;
	}

	private void VerifyMoveSpeed(DateTimeOffset time)
	{
		m_moveVerificationEndTime = time;
		float fTime = (float)(m_moveVerificationEndTime - m_moveVerificationStartTime).TotalSeconds;
		if (!(fTime >= 1f))
		{
			return;
		}
		float fMoveSpeed = m_fMoveVerificationDistance / fTime;
		if (fMoveSpeed > (float)m_nMoveSpeed * 2f)
		{
			m_nAbnormalMoveSpeedCount++;
			if (m_nAbnormalMoveSpeedCount > 2)
			{
				SFLogUtil.Warn(GetType(), string.Concat("[HeroAbnormalMoveSpeedLog] heroId = ", m_id, ", moveSpeed = ", fMoveSpeed));
				AddWork(new SFAction(LogoutByAbnormalMoveSpeed), bGlobalLockRequired: true);
			}
		}
	}

	public void LogoutByAbnormalMoveSpeed()
	{
		if (isLoggedIn)
		{
			LogOut();
		}
	}

	public void OnMove(Vector3 previousPosition)
	{
		if (m_bMoving)
		{
			float fDistance = MathUtil.Distance_3D(previousPosition, m_position);
			m_fMoveVerificationDistance += fDistance;
		}
	}

	public void StartManualMoving(DateTimeOffset time)
	{
		if (!m_bManualMoving)
		{
			m_manualMoveStartTime = time;
			m_bManualMoving = true;
		}
	}

	public void StopManualMoving()
	{
		if (m_bManualMoving)
		{
			m_bManualMoving = false;
			if (m_currentPlace is ContinentInstance)
			{
				StopAcceleration();
			}
		}
	}

	public void StartWalking()
	{
		if (!m_bIsRiding && !m_bWalking)
		{
			m_bWalking = true;
			StopAcceleration();
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroMoveModeChanged(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_bWalking);
			}
		}
	}

	public void StartRunning(DateTimeOffset time)
	{
		if (!m_bIsRiding && m_bWalking)
		{
			m_bWalking = false;
			InitMoveStartTime(time);
			RefreshMoveSpeed();
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroMoveModeChanged(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_bWalking);
			}
		}
	}

	public PDHero ToPDHero(DateTimeOffset currentTime)
	{
		PDHero inst = new PDHero();
		inst.id = (Guid)m_id;
		inst.name = m_sName;
		inst.nationId = m_nationInst.nationId;
		inst.jobId = m_job.id;
		inst.level = m_nLevel;
		inst.vipLevel = ((m_vipLevel != null) ? m_vipLevel.level : 0);
		inst.rankNo = rankNo;
		inst.maxHP = m_nRealMaxHP;
		inst.hp = m_nHP;
		inst.isWalking = m_bWalking;
		inst.accelerationMode = m_bAcceleration;
		inst.battlePower = m_lnBattlePower;
		List<PDAbnormalStateEffect> abnormalStateEffects = new List<PDAbnormalStateEffect>();
		foreach (AbnormalStateEffect effect in m_abnormalStateEffects.Values)
		{
			abnormalStateEffects.Add(effect.ToPDAbnormalStateEffect(currentTime));
		}
		inst.abnormalStateEffects = abnormalStateEffects.ToArray();
		inst.equippedWeapon = ((m_equippedWeapon != null) ? m_equippedWeapon.ToPDHeroMainGear() : null);
		inst.equippedArmor = ((m_equippedArmor != null) ? m_equippedArmor.ToPDHeroMainGear() : null);
		inst.equippedWingId = equippedWingId;
		inst.position = m_position;
		inst.rotationY = m_fRotationY;
		inst.isRiding = m_bIsRiding;
		if (m_bIsRiding)
		{
			Mount mount = m_equippedMount.mount;
			inst.mountId = mount.id;
			inst.mountLevel = m_equippedMount.level;
		}
		inst.isBattleMode = m_bIsBattleMode;
		inst.isFishing = isFishing;
		inst.isDistorting = isDistorting;
		inst.isMysteryBoxPicking = isMysteryBoxPicking;
		inst.pickedMysteryBoxGrade = ((m_mysteryBoxQuest != null) ? m_mysteryBoxQuest.pickedBoxGrade : 0);
		inst.isSecretLetterPicking = isSecretLetterPicking;
		inst.pickedSecretLetterGrade = ((m_secretLetterQuest != null) ? m_secretLetterQuest.pickedLetterGrade : 0);
		if (isReal)
		{
			if (m_guildMember != null)
			{
				inst.guildId = (Guid)m_guildMember.guild.id;
				inst.guildName = m_guildMember.guild.name;
				inst.guildMemberGrade = m_guildMember.grade.id;
			}
		}
		else
		{
			inst.guildId = (Guid)m_fieldOfHonorHeroGuildId;
			inst.guildName = m_sFieldOfHonorHeroGuildName;
			inst.guildMemberGrade = m_nFieldOfHonorHeroGuildMemberGrade;
		}
		inst.displayTitleId = displayTitleId;
		inst.customPresetHair = m_nCustomPresetHair;
		inst.customFaceJawHeight = m_nCustomFaceJawHeight;
		inst.customFaceJawWidth = m_nCustomFaceJawWidth;
		inst.customFaceJawEndHeight = m_nCustomFaceJawEndHeight;
		inst.customFaceWidth = m_nCustomFaceWidth;
		inst.customFaceEyebrowHeight = m_nCustomFaceEyebrowHeight;
		inst.customFaceEyebrowRotation = m_nCustomFaceEyebrowRotation;
		inst.customFaceEyesWidth = m_nCustomFaceEyesWidth;
		inst.customFaceNoseHeight = m_nCustomFaceNoseHeight;
		inst.customFaceNoseWidth = m_nCustomFaceNoseWidth;
		inst.customFaceMouthHeight = m_nCustomFaceMouthHeight;
		inst.customFaceMouthWidth = m_nCustomFaceMouthWidth;
		inst.customBodyHeadSize = m_nCustomBodyHeadSize;
		inst.customBodyArmsLength = m_nCustomBodyArmsLength;
		inst.customBodyArmsWidth = m_nCustomBodyArmsWidth;
		inst.customBodyChestSize = m_nCustomBodyChestSize;
		inst.customBodyWaistWidth = m_nCustomBodyWaistWidth;
		inst.customBodyHipsSize = m_nCustomBodyHipsSize;
		inst.customBodyPelvisWidth = m_nCustomBodyPelvisWidth;
		inst.customBodyLegsLength = m_nCustomBodyLegsLength;
		inst.customBodyLegsWidth = m_nCustomBodyLegsWidth;
		inst.customColorSkin = m_nCustomColorSkin;
		inst.customColorEyes = m_nCustomColorEyes;
		inst.customColorBeardAndEyebrow = m_nCustomColorBeardAndEyebrow;
		inst.customColorHair = m_nCustomColorHair;
		inst.transformationMonsterId = (isTransformMainQuestMonster ? m_mainQuestTransformationMonsterEffect.transformationMonster.id : 0);
		inst.participatedCreatureId = ((m_participationCreature != null) ? m_participationCreature.creature.id : 0);
		inst.equippedCostumeId = ((m_equippedCostume != null) ? m_equippedCostume.costumeId : 0);
		inst.appliedCostumeEffectId = ((m_equippedCostume != null) ? m_equippedCostume.costumeEffectId : 0);
		inst.isSafeMode = m_bIsSafeMode;
		inst.equippedArtifactNo = m_nEquippedArtifactNo;
		return inst;
	}

	public PDHero ToPDHeroWithLock(DateTimeOffset currentTime)
	{
		lock (syncObject)
		{
			return ToPDHero(currentTime);
		}
	}

	public PDSimpleHero ToPDSimpleHero()
	{
		PDSimpleHero inst = new PDSimpleHero();
		inst.id = (Guid)m_id;
		inst.name = m_sName;
		inst.nationId = nationId;
		inst.noblesseId = ((m_nationNoblesse != null) ? m_nationNoblesse.id : 0);
		inst.jobId = m_job.id;
		inst.level = m_nLevel;
		inst.vipLevel = m_vipLevel.level;
		inst.battlePower = m_lnBattlePower;
		return inst;
	}

	public PDSimpleHero ToPDSimpleHeroWithLock()
	{
		lock (syncObject)
		{
			return ToPDSimpleHero();
		}
	}

	public override PDAttacker ToPDAttacker()
	{
		PDHeroAttacker inst = new PDHeroAttacker();
		inst.heroId = (Guid)m_id;
		inst.name = m_sName;
		inst.nationId = nationId;
		return inst;
	}

	public PDSearchHero ToPDSearchHero()
	{
		PDSearchHero inst = new PDSearchHero();
		inst.heroId = (Guid)m_id;
		inst.name = m_sName;
		inst.nationId = nationId;
		inst.jobId = m_job.id;
		inst.level = m_nLevel;
		return inst;
	}

	public void CreateInventorySlots(int nSlotCount)
	{
		for (int nIndex = 0; nIndex < nSlotCount; nIndex++)
		{
			InventorySlot slot = new InventorySlot(this, m_inventorySlots.Count);
			m_inventorySlots.Add(slot);
		}
	}

	public InventorySlot GetInventorySlot(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_inventorySlots.Count)
		{
			return null;
		}
		return m_inventorySlots[nIndex];
	}

	public InventorySlot GetEmptyInventorySlot()
	{
		foreach (InventorySlot slot in m_inventorySlots)
		{
			if (slot.isEmpty)
			{
				return slot;
			}
		}
		return null;
	}

	public List<PDInventorySlot> GetPlacedPDInventorySlots()
	{
		List<PDInventorySlot> results = new List<PDInventorySlot>();
		foreach (InventorySlot slot in m_inventorySlots)
		{
			if (!slot.isEmpty)
			{
				results.Add(slot.ToPDInventorySlot());
			}
		}
		return results;
	}

	public bool IsAvailableInventory(ResultItemCollection resultItemCollection)
	{
		return IsAvailableInventory(resultItemCollection, emptyInventorySlotCount);
	}

	public bool IsAvailableInventory(ResultItemCollection resultItemCollection, int nRemainingEmptyInventoryCount)
	{
		if (resultItemCollection == null)
		{
			throw new ArgumentNullException("resultItemCollection");
		}
		foreach (ResultItem rewardItem in resultItemCollection.resultItems)
		{
			int nAvailabelSpace = GetInventoryAvailableSpaceWithoutEmptySlots(rewardItem.item, rewardItem.owned);
			int nRemaingCount = rewardItem.count - nAvailabelSpace;
			if (nRemaingCount > 0)
			{
				int nMaxCountPerSlot = rewardItem.item.type.maxCountPerInventorySlot;
				int nRequiredSlotCount = (nRemaingCount - 1) / nMaxCountPerSlot + 1;
				if (nRemainingEmptyInventoryCount < nRequiredSlotCount)
				{
					return false;
				}
				nRemainingEmptyInventoryCount -= nRequiredSlotCount;
			}
		}
		return true;
	}

	private void AddInventoryItem(HeroInventoryItem inventoryItem)
	{
		m_inventoryItems.Add(inventoryItem.item.id, inventoryItem);
	}

	private void RemoveInventoryItem(int nItemId)
	{
		m_inventoryItems.Remove(nItemId);
	}

	private HeroInventoryItem GetInventoryItem(int nItemId)
	{
		if (!m_inventoryItems.TryGetValue(nItemId, out var value))
		{
			return null;
		}
		return value;
	}

	private HeroInventoryItem GetOrCreateInventoryItem(Item item)
	{
		HeroInventoryItem inventoryItem = GetInventoryItem(item.id);
		if (inventoryItem == null)
		{
			inventoryItem = new HeroInventoryItem(this, item);
			AddInventoryItem(inventoryItem);
		}
		return inventoryItem;
	}

	public int GetItemCount(int nItemId, bool bOwned)
	{
		HeroInventoryItem inventoryItem = GetInventoryItem(nItemId);
		if (inventoryItem == null)
		{
			return 0;
		}
		if (!bOwned)
		{
			return inventoryItem.unOwnCount;
		}
		return inventoryItem.ownCount;
	}

	public int GetItemCount(int nItemId)
	{
		return GetInventoryItem(nItemId)?.count ?? 0;
	}

	public int UseItemOnly(int nItemId, bool bOwned, int nCount, ICollection<InventorySlot> changedInventorySlots)
	{
		HeroInventoryItem inventoryItem = GetInventoryItem(nItemId);
		if (inventoryItem == null)
		{
			return 0;
		}
		int nUsedCount = inventoryItem.UseOnly(bOwned, nCount, changedInventorySlots);
		if (inventoryItem.isEmpty)
		{
			RemoveInventoryItem(nItemId);
		}
		return nUsedCount;
	}

	public void UseItem(int nItemId, bool bFisetUseOwn, int nCount, ICollection<InventorySlot> changedInventorySlots, out int nUsedOwnCount, out int nUsedUnOwnCount)
	{
		nUsedOwnCount = 0;
		nUsedUnOwnCount = 0;
		HeroInventoryItem inventoryItem = GetInventoryItem(nItemId);
		if (inventoryItem != null)
		{
			inventoryItem.Use(bFisetUseOwn, nCount, changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
			if (inventoryItem.isEmpty)
			{
				RemoveInventoryItem(nItemId);
			}
		}
	}

	public void UseItem(int nItemId, bool bFirstUseOwn, int nCount, ICollection<InventorySlot> changedInventorySlots)
	{
		int nUsedOwnCount = 0;
		int nUsedUnOwnCount = 0;
		UseItem(nItemId, bFirstUseOwn, nCount, changedInventorySlots, out nUsedOwnCount, out nUsedUnOwnCount);
	}

	public void UseItem(int nInventorySlotIndex, int nCount)
	{
		InventorySlot slot = GetInventorySlot(nInventorySlotIndex);
		if (slot == null)
		{
			throw new Exception("인벤토리슬롯이 존재하지 않습니다. nInventorySlotIndex = " + nInventorySlotIndex);
		}
		if (!(slot.obj is ItemInventoryObject obj))
		{
			throw new Exception("해당 인벤토리슬롯에 아이템인벤토리객체가 존재하지 않습니다. nInventorySlotIndex = " + nInventorySlotIndex);
		}
		if (nCount < 0)
		{
			throw new ArgumentOutOfRangeException("nCount");
		}
		if (nCount == 0)
		{
			return;
		}
		if (nCount > obj.count)
		{
			throw new Exception("사용수량이 보유수량보다 큽니다.");
		}
		obj.count -= nCount;
		if (obj.isEmpty)
		{
			slot.Clear();
			HeroInventoryItem heroInventoryItem = obj.heroInventoryItem;
			heroInventoryItem.RemoveObject(nInventorySlotIndex);
			if (heroInventoryItem.isEmpty)
			{
				RemoveInventoryItem(heroInventoryItem.item.id);
			}
		}
	}

	public int GetInventoryAvailableSpace(Item item, bool bOwned)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		int nAvailableSpace = 0;
		HeroInventoryItem inventoryItem = GetInventoryItem(item.id);
		if (inventoryItem != null)
		{
			nAvailableSpace += inventoryItem.GetAvailableSpace(bOwned);
		}
		return nAvailableSpace + GetInventoryAvailableSpaceOfEmptySlots(item);
	}

	public int GetInventoryAvailableSpaceWithoutEmptySlots(Item item, bool bOwned)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		return GetInventoryItem(item.id)?.GetAvailableSpace(bOwned) ?? 0;
	}

	public int GetInventoryAvailableSpaceOfEmptySlots(Item item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		return emptyInventorySlotCount * item.type.maxCountPerInventorySlot;
	}

	public int AddItem(Item item, bool bOwned, int nCount, ICollection<InventorySlot> changedInventorySlots)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		if (changedInventorySlots == null)
		{
			throw new ArgumentNullException("changedInventorySlots");
		}
		if (nCount < 0)
		{
			throw new ArgumentOutOfRangeException("nCount");
		}
		if (nCount == 0)
		{
			return 0;
		}
		HeroInventoryItem inventoryItem = GetOrCreateInventoryItem(item);
		int nRemainingCount = inventoryItem.AddItem(bOwned, nCount, changedInventorySlots);
		nRemainingCount = AddItemToEmptyInventorySlots(inventoryItem, bOwned, nRemainingCount, changedInventorySlots);
		if (inventoryItem.isEmpty)
		{
			RemoveInventoryItem(inventoryItem.item.id);
		}
		return nRemainingCount;
	}

	private int AddItemToEmptyInventorySlots(HeroInventoryItem inventoryItem, bool bOwned, int nCount, ICollection<InventorySlot> changedInventorySlots)
	{
		if (nCount == 0)
		{
			return 0;
		}
		int nRemainingCount = nCount;
		foreach (InventorySlot slot in m_inventorySlots)
		{
			if (slot.isEmpty)
			{
				ItemInventoryObject inventoryObject = new ItemInventoryObject(inventoryItem, bOwned);
				slot.Place(inventoryObject);
				inventoryItem.AddObject(inventoryObject);
				int nAddCount = Math.Min(inventoryObject.availableSpace, nRemainingCount);
				inventoryObject.count += nAddCount;
				nRemainingCount -= nAddCount;
				changedInventorySlots.Add(inventoryObject.inventorySlot);
				if (nRemainingCount <= 0)
				{
					return nRemainingCount;
				}
			}
		}
		return nRemainingCount;
	}

	public void AddMainGear(HeroMainGear mainGear, bool bInit, DateTimeOffset time)
	{
		if (mainGear == null)
		{
			throw new ArgumentNullException("mainGear");
		}
		if (mainGear.hero != this)
		{
			throw new Exception("해당 메인장비는 이 영웅에 추가할 수 없습니다. herMainGearId = " + mainGear.id);
		}
		m_mainGears.Add(mainGear.id, mainGear);
		if (!bInit)
		{
			int nMainGearGrade = mainGear.gear.grade.id;
			if (nMainGearGrade > m_nMaxAcquisitionMainGearGrade)
			{
				m_nMaxAcquisitionMainGearGrade = nMainGearGrade;
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
				dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_MaxAcquisitionGearGrade(m_id, m_nMaxAcquisitionMainGearGrade));
				dbWork.Schedule();
			}
			if (Resource.instance.CheckSystemMessageCondition(1, nMainGearGrade))
			{
				SystemMessage.SendMainGearAcquirment(this, mainGear.gear.id);
			}
		}
	}

	public HeroMainGear GetMainGear(Guid id)
	{
		if (!m_mainGears.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveMainGear(Guid mainGearId)
	{
		m_mainGears.Remove(mainGearId);
	}

	public List<PDFullHeroMainGear> GetPDFullHeroMainGears()
	{
		return HeroMainGear.ToPDFullHeroMainGears(m_mainGears.Values);
	}

	public void AddSubGear(HeroSubGear subGear)
	{
		if (subGear == null)
		{
			throw new ArgumentNullException("subGear");
		}
		if (subGear.hero != this)
		{
			throw new Exception("해당 보조장비는 이 영웅에 추가할 수 없습니다. subGearId = " + subGear.subGearId);
		}
		m_subGears.Add(subGear.subGearId, subGear);
	}

	public HeroSubGear GetSubGear(int nId)
	{
		if (!m_subGears.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool ContainsSubGear(int nId)
	{
		return m_subGears.ContainsKey(nId);
	}

	public List<PDFullHeroSubGear> GetPDFullHeroSubGears()
	{
		return HeroSubGear.ToPDFullHeroSubGears(m_subGears.Values);
	}

	public List<PDFullHeroSubGear> GetEquippedPDFullHeroSubGears()
	{
		List<PDFullHeroSubGear> insts = new List<PDFullHeroSubGear>();
		foreach (HeroSubGear subGear in m_subGears.Values)
		{
			if (subGear.equipped)
			{
				insts.Add(subGear.ToPDFullHeroSubGear());
			}
		}
		return insts;
	}

	public void AddOwnMonster(MonsterInstance monsterInst)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		m_ownMonsterInsts.Add(monsterInst);
	}

	public void RemoveOwnMonster(MonsterInstance monsterInst)
	{
		m_ownMonsterInsts.Remove(monsterInst);
	}

	public void ClearOwnMonsters()
	{
		m_ownMonsterInsts.Clear();
	}

	protected override void OnDamage()
	{
		base.OnDamage();
		if (m_lastAttacker is Hero heroAttacker)
		{
			AddReceivedDamage(heroAttacker, m_nLastHPDamage);
			heroAttacker.InitSafeModeWaitStartTime(m_lastDamageTime);
		}
		CancelReturnScrollUse(bSendEventToMyself: true, bSendEventToOthers: true);
		if (base.isDead)
		{
			StopMonsterTame();
			CancelWarMemoryTransformationMonsterEffect(bResetHP: false, bSendEventToOthers: true);
			CancelMainQuestTransformationMonsterEffect(bResetHP: false, bSendEventMyself: true, bSendEventToOthers: true, new List<long>());
		}
	}

	private void AddReceivedDamage(Hero attacker, long lnDamage)
	{
		SetReceivedDamage(attacker.id, GetReceivedDamage(attacker.id) + lnDamage);
	}

	public long GetReceivedDamage(Guid attackerId)
	{
		if (!m_receivedDamages.TryGetValue(attackerId, out var value))
		{
			return 0L;
		}
		return value;
	}

	public void SetReceivedDamage(Guid attackerId, long lnDamage)
	{
		m_receivedDamages[attackerId] = lnDamage;
	}

	private void ClearReceivedDamages()
	{
		m_receivedDamages.Clear();
	}

	protected override void OnHit(HitResult hitResult)
	{
		base.OnHit(hitResult);
		StartBattleMode(hitResult.time);
	}

	protected override void OnDead()
	{
		base.OnDead();
		foreach (MonsterInstance monsterInst2 in m_ownMonsterInsts)
		{
			if (monsterInst2.ownerType == MonsterOwnerType.Target)
			{
				monsterInst2.SetOwnership(this, MonsterOwnerType.Controller);
			}
		}
		foreach (MonsterInstance monsterInst in m_aggroMonsterInsts)
		{
			monsterInst.RemoveAggro(m_id);
		}
		m_aggroMonsterInsts.Clear();
		ClearPvpKillCount();
		ClearPvpAssistCount();
		CancelAllExclusiveActions();
		EndMove();
		StopAncientRelicTrapAbnormalStateEffect();
		StopProofOfValorBuff();
		StopInfiniteWarBuff();
		StopDragonNestTrapEffect();
		EndAutoHunt();
	}

	protected override void SendHitEvent(HitResult hitResult)
	{
		PDHitResult pdHitResult = hitResult.ToPDHitResult();
		ServerEvent.SendHeroHit(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_id, pdHitResult);
	}

	public override void OnHPRestored(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		base.OnHPRestored(bSendEventToMyself, bSendEventToOthers);
		if (bSendEventToMyself)
		{
			ServerEvent.SendHeroHpRestored(new ClientPeer[1] { m_account.peer }, m_id, m_nHP);
		}
		if (bSendEventToOthers && m_currentPlace != null)
		{
			ServerEvent.SendHeroHpRestored(m_currentPlace.GetInterestClientPeers(m_sector, m_id), m_id, m_nHP);
		}
	}

	public void AddAggroMonster(MonsterInstance monsterInst)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		m_aggroMonsterInsts.Add(monsterInst);
	}

	public bool RemoveAggroMonster(MonsterInstance monsterInst)
	{
		return m_aggroMonsterInsts.Remove(monsterInst);
	}

	public void ClearAggroMonsters()
	{
		m_aggroMonsterInsts.Clear();
	}

	public void AddMail(Mail mail, bool bSendEvent)
	{
		if (mail == null)
		{
			throw new ArgumentNullException("mail");
		}
		if (mail.hero != null)
		{
			throw new Exception("해당 메일은 영웅에 추가되어 있습니다.");
		}
		m_mails.Add(mail.id, mail);
		mail.hero = this;
		if (bSendEvent)
		{
			ServerEvent.SendNewMail(m_account.peer, mail.ToPDMail());
		}
	}

	public Mail GetMail(Guid mailId)
	{
		if (!m_mails.TryGetValue(mailId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool ContainsMail(Guid mailId)
	{
		return m_mails.ContainsKey(mailId);
	}

	public void RemoveMail(Guid mailId)
	{
		m_mails.Remove(mailId);
	}

	public void RefreshMailBox(DateTimeOffset currentTime)
	{
		Mail[] array = m_mails.Values.ToArray();
		foreach (Mail mail in array)
		{
			mail.UpdateRemainingTime(currentTime);
			if (mail.expired)
			{
				RemoveMail(mail.id);
			}
		}
	}

	public List<PDMail> GetPDMails()
	{
		return Mail.ToPDMails(m_mails.Values);
	}

	public void AddDeliveredMail(Mail mail)
	{
		if (mail == null)
		{
			throw new ArgumentNullException("mail");
		}
		m_deliveredMails.Add(mail.id, mail);
	}

	public void AddOwnDia(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nOwnDia += nAmount;
		}
	}

	public void UseOwnDia(int nAmount, DateTimeOffset time)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			if (m_nOwnDia < nAmount)
			{
				throw new Exception("보유한 귀속다이아가 부족합니다.");
			}
			m_nOwnDia -= nAmount;
			m_account.ProcessConsumeEvent(nAmount, time);
			m_account.ProcessDailyConsumeEvent(nAmount, time);
		}
	}

	public void UseDia(int nAmount, DateTimeOffset time, out int nUsedOwnDia, out int nUsedUnOwnDia)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		nUsedOwnDia = 0;
		nUsedUnOwnDia = 0;
		if (nAmount != 0)
		{
			if (dia < nAmount)
			{
				throw new Exception("보유한 다이아가 부족합니다.");
			}
			nUsedOwnDia = Math.Min(nAmount, m_nOwnDia);
			nUsedUnOwnDia = nAmount - nUsedOwnDia;
			UseOwnDia(nUsedOwnDia, time);
			m_account.UseUnOwnDia(nUsedUnOwnDia, time);
		}
	}

	public void UseDia(int nAmount, DateTimeOffset time)
	{
		UseDia(nAmount, time, out var _, out var _);
	}

	private void AddVipPoint(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nVipPoint += nAmount;
		}
	}

	public void AddVipPoint(int nAccountAmount, int nHeroAmount)
	{
		m_account.AddVipPoint(nAccountAmount);
		AddVipPoint(nHeroAmount);
		RefreshVipLevel();
	}

	public void RefreshVipLevel()
	{
		VipLevel newVipLevel = Resource.instance.GetVipLevelByPoint(totalVipPoint);
		if (newVipLevel.level != m_vipLevel.level)
		{
			m_vipLevel = newVipLevel;
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroVipLevelChanged(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_vipLevel.level);
			}
		}
	}

	public void AddGold(long lnAmount)
	{
		if (lnAmount < 0)
		{
			throw new ArgumentOutOfRangeException("lnAmount");
		}
		if (lnAmount != 0)
		{
			m_lnGold += lnAmount;
			if (m_lnGold > m_lnMaxGold)
			{
				m_lnMaxGold = m_lnGold;
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
				dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_MaxGold(m_id, m_lnMaxGold));
				dbWork.Schedule();
			}
		}
	}

	public void UseGold(long lnAmount)
	{
		if (lnAmount < 0)
		{
			throw new ArgumentOutOfRangeException("lnAmount");
		}
		if (lnAmount != 0)
		{
			if (lnAmount > m_lnGold)
			{
				throw new Exception("보유한 골드가 부족합니다.");
			}
			m_lnGold -= lnAmount;
		}
	}

	public void AddHonorPoint(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nHonorPoint += nAmount;
		}
	}

	public void UseHonorPoint(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			if (nAmount > m_nHonorPoint)
			{
				throw new Exception("보유한 명예포인트가 부족합니다.");
			}
			m_nHonorPoint -= nAmount;
		}
	}

	public void AddSoulPowder(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nSoulPowder += nAmount;
		}
	}

	public void UseSoulPowder(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			if (nAmount > m_nSoulPowder)
			{
				throw new Exception("보유한 영혼가루가 부족합니다.");
			}
			m_nSoulPowder -= nAmount;
		}
	}

	public void RefreshMainGearEnchantDailyCount(DateTime date)
	{
		if (!(date == m_mainGearEnchantDailyCount.date))
		{
			m_mainGearEnchantDailyCount.date = date;
			m_mainGearEnchantDailyCount.value = 0;
		}
	}

	public void RefreshMainGearRefinementDailyCount(DateTime date)
	{
		if (!(date == m_mainGearRefinementDailyCount.date))
		{
			m_mainGearRefinementDailyCount.date = date;
			m_mainGearRefinementDailyCount.value = 0;
		}
	}

	public void AddSkill(HeroSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_skills.Add(skill.skillId, skill);
	}

	public HeroSkill GetSkill(int nId)
	{
		if (!m_skills.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDHeroSkill> GetPDHeroSkills()
	{
		List<PDHeroSkill> results = new List<PDHeroSkill>();
		foreach (HeroSkill heroSkill in m_skills.Values)
		{
			results.Add(heroSkill.ToPDHeroSkill());
		}
		return results;
	}

	public void ClearSkills()
	{
		m_skills.Clear();
	}

	public void AddLak(int nAmount)
	{
		if (!isReal)
		{
			return;
		}
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			int nMaxLak = Resource.instance.specialSkillMaxLak;
			if (m_nLak < nMaxLak)
			{
				m_nLak = Math.Min(m_nLak + nAmount, nMaxLak);
				ServerEvent.SendLakAcquisition(m_account.peer, m_nLak);
			}
		}
	}

	public void UseLak(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			if (m_nLak < nAmount)
			{
				throw new Exception("보유한 라크가 부족합니다.");
			}
			m_nLak -= nAmount;
		}
	}

	public void StartBattleMode(DateTimeOffset time)
	{
		m_battleModeStartTime = time;
		if (!m_bIsBattleMode)
		{
			m_bIsBattleMode = true;
			GetOffMount(bSendEventToMyself: true);
			if (isReal)
			{
				ServerEvent.SendBattleModeStart(m_account.peer);
			}
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroBattleModeStart(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
			}
		}
	}

	public void FinishedBattleMode()
	{
		if (m_bIsBattleMode)
		{
			m_bIsBattleMode = false;
			m_battleModeStartTime = DateTimeOffset.MinValue;
			if (isReal)
			{
				ServerEvent.SendBattleModeEnd(m_account.peer);
			}
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroBattleModeEnd(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
			}
		}
	}

	public void BackupLocationInfo()
	{
		if (m_currentPlace is ContinentInstance currentContinentInst)
		{
			m_previousContinent = currentContinentInst.continent;
			if (currentContinentInst.isNationTerritory)
			{
				m_previousNation = ((NationContinentInstance)currentContinentInst).nation;
			}
			else
			{
				m_previousNation = null;
			}
			m_previousPosition = m_position;
			m_fPreviousYRotation = m_fRotationY;
		}
	}

	public void RestoreLocationInfo()
	{
		m_position = m_previousPosition;
		m_fRotationY = m_fPreviousYRotation;
	}

	public void SetPreviousContinentAndRotation(Continent continent, Nation nation)
	{
		if (continent == null)
		{
			throw new ArgumentNullException("continent");
		}
		m_previousContinent = continent;
		m_previousNation = nation;
	}

	public void SetPreviousPositionAndRotation(Vector3 position, float fRotationY)
	{
		m_previousPosition = position;
		m_fPreviousYRotation = fRotationY;
	}

	public PDHeroMainQuest GetPDHeroMainQuest()
	{
		if (m_currentHeroMainQuest == null)
		{
			return null;
		}
		return m_currentHeroMainQuest.ToPDHeroMainQuest();
	}

	public bool IsMainQuestCompleted(int nMainQuestNo)
	{
		return nMainQuestNo <= completedMainQuestNo;
	}

	public void TransformMainQuestMonster(DateTimeOffset time, ICollection<long> removedAbnormalStateEffects)
	{
		if (m_currentHeroMainQuest == null)
		{
			return;
		}
		MainQuest mainQuest = m_currentHeroMainQuest.mainQuest;
		if (mainQuest.transformationMonster == null)
		{
			return;
		}
		GetOffMount(bSendEventToMyself: true);
		if (isRidingCart)
		{
			m_ridingCartInst.GetOff(time, bSendEvent: true);
		}
		HeroMainQuestTransformationMonsterEffect effect = new HeroMainQuestTransformationMonsterEffect();
		effect.Init(this, mainQuest, time);
		m_mainQuestTransformationMonsterEffect = effect;
		foreach (MonsterOwnSkill ownSkill in m_mainQuestTransformationMonsterEffect.transformationMonster.ownSkills)
		{
			HeroMainQuestTransformationMonsterSkill skill = new HeroMainQuestTransformationMonsterSkill(this, ownSkill.skill);
			m_mainQuestTransformationMonsterSkills.Add(skill.skillId, skill);
		}
		foreach (AbnormalStateEffect abnormalStateeffect in m_abnormalStateEffects.Values)
		{
			removedAbnormalStateEffects.Add(abnormalStateeffect.id);
		}
		if (m_abnormalStateEffects.Count > 0)
		{
			ClearAllAbnormalStateEffects();
		}
		StopAcceleration();
		RefreshRealValues(bSendMaxHpChangedToOthers: false);
		RefreshMoveSpeed();
		m_nHP = m_nRealMaxHP;
		ServerEvent.SendHeroMainQuestMonsterTransformationStarted(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_mainQuestTransformationMonsterEffect.transformationMonster.id, m_nRealMaxHP, m_nHP, removedAbnormalStateEffects.ToArray());
	}

	public void ProcessMainQuestTransformationMonsterEffectFinished()
	{
		if (m_mainQuestTransformationMonsterEffect == null)
		{
			return;
		}
		m_mainQuestTransformationMonsterEffect = null;
		ClearMainQuestTransformationMonsterSkill();
		List<long> removedAbnormalStateEffects = new List<long>();
		foreach (AbnormalStateEffect abnormalStateeffect in m_abnormalStateEffects.Values)
		{
			removedAbnormalStateEffects.Add(abnormalStateeffect.id);
		}
		if (m_abnormalStateEffects.Count > 0)
		{
			ClearAllAbnormalStateEffects();
		}
		InitMoveStartTime(DateTimeUtil.currentTime);
		RefreshRealValues(bSendMaxHpChangedToOthers: false);
		RefreshMoveSpeed();
		m_nHP = m_nRealMaxHP;
		ServerEvent.SendMainQuestMonsterTransformationFinished(m_account.peer, m_nRealMaxHP, m_nHP, removedAbnormalStateEffects.ToArray());
		ServerEvent.SendHeroMainQuestMonsterTransformationFinished(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_nRealMaxHP, m_nHP, removedAbnormalStateEffects.ToArray());
	}

	public void CancelMainQuestTransformationMonsterEffect(bool bResetHP, bool bSendEventMyself, bool bSendEventToOthers, ICollection<long> removedAbnormalStateEffects)
	{
		if (m_mainQuestTransformationMonsterEffect == null)
		{
			return;
		}
		m_mainQuestTransformationMonsterEffect.Stop();
		m_mainQuestTransformationMonsterEffect = null;
		ClearMainQuestTransformationMonsterSkill();
		foreach (AbnormalStateEffect abnormalStateeffect in m_abnormalStateEffects.Values)
		{
			removedAbnormalStateEffects.Add(abnormalStateeffect.id);
		}
		if (m_abnormalStateEffects.Count > 0)
		{
			ClearAllAbnormalStateEffects();
		}
		InitMoveStartTime(DateTimeUtil.currentTime);
		RefreshRealValues(bSendMaxHpChangedToOthers: false);
		RefreshMoveSpeed();
		if (bResetHP)
		{
			m_nHP = m_nRealMaxHP;
		}
		if (bSendEventMyself)
		{
			ServerEvent.SendMainQuestMonsterTransformationCanceled(m_account.peer, m_nRealMaxHP, m_nHP, removedAbnormalStateEffects.ToArray());
		}
		if (bSendEventToOthers)
		{
			ServerEvent.SendHeroMainQuestMonsterTransformationCanceled(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_nRealMaxHP, m_nHP, removedAbnormalStateEffects.ToArray());
		}
	}

	public HeroMainQuestTransformationMonsterSkill GetMainQuestTransformationMonsterSkill(int nSkillId)
	{
		if (!m_mainQuestTransformationMonsterSkills.TryGetValue(nSkillId, out var value))
		{
			return null;
		}
		return value;
	}

	private void ClearMainQuestTransformationMonsterSkill()
	{
		m_mainQuestTransformationMonsterSkills.Clear();
	}

	protected override bool IsAbnormalStateHitEnabled(Unit source, AbnormalState abnormalState)
	{
		if (!base.IsAbnormalStateHitEnabled(source, abnormalState))
		{
			return false;
		}
		if (source is Hero sourceHero && abnormalState.type == 1)
		{
			if (sourceHero.isSafeMode)
			{
				return false;
			}
			if (sourceHero.isDistorting)
			{
				return false;
			}
			if (m_bIsSafeMode)
			{
				return false;
			}
		}
		return true;
	}

	protected override void SendAbnormalStateEffectStart(AbnormalStateEffect effect, int nDamageAbsorbShieldRemainingAbsorbAmount, IEnumerable<long> removedAbnormalStateEffects)
	{
		if (m_currentPlace != null)
		{
			ServerEvent.SendHeroAbnormalStateEffectStart(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_id, effect.id, effect.abnormalState.id, effect.sourceJobId, effect.abnormalStateLevel.level, effect.duration, nDamageAbsorbShieldRemainingAbsorbAmount, removedAbnormalStateEffects.ToArray());
			return;
		}
		ServerEvent.SendHeroAbnormalStateEffectStart(new ClientPeer[1] { m_account.peer }, m_id, effect.id, effect.abnormalState.id, effect.sourceJobId, effect.abnormalStateLevel.level, effect.duration, nDamageAbsorbShieldRemainingAbsorbAmount, removedAbnormalStateEffects.ToArray());
	}

	protected override void SendAbnormalStateEffectHit(AbnormalStateEffect effect, IEnumerable<long> removedAbnormalStateEffects)
	{
		PDAttacker pdAttacker = m_lastAttacker.ToPDAttacker();
		ServerEvent.SendHeroAbnormalStateEffectHit(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_id, m_nHP, effect.id, m_nLastDamage, m_nLastHPDamage, removedAbnormalStateEffects.ToArray(), pdAttacker);
	}

	protected override void OnAbnormalStateEffectHit()
	{
		base.OnAbnormalStateEffectHit();
		StartBattleMode(m_lastDamageTime);
	}

	protected override void SendAbnormalStateEffectFinishedEvent(AbnormalStateEffect effect)
	{
		if (m_currentPlace != null)
		{
			ServerEvent.SendHeroAbnormalStateEffectFinished(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_id, effect.id);
			return;
		}
		ServerEvent.SendHeroAbnormalStateEffectFinished(new ClientPeer[1] { m_account.peer }, m_id, effect.id);
	}

	protected override bool IsAbnormalStateDamageHitEnabled(AbnormalStateEffect effect)
	{
		if (!base.IsAbnormalStateDamageHitEnabled(effect))
		{
			return false;
		}
		if (effect.source is Hero)
		{
			if (m_bIsSafeMode)
			{
				return false;
			}
			if (isDistorting)
			{
				return false;
			}
		}
		return true;
	}

	private void OnUpdate_ProcessMainQuestForMove()
	{
		try
		{
			ProcessMainQuestForMove();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void ProcessMainQuestForMove()
	{
		if (!base.isDead && m_currentHeroMainQuest != null && !m_currentHeroMainQuest.isObjectiveCompleted)
		{
			MainQuest mainQuest = m_currentHeroMainQuest.mainQuest;
			if (mainQuest.type == 1 && m_currentPlace is ContinentInstance currentPlace && currentPlace.IsSame(mainQuest.targetContinent.id, m_nationInst.nationId) && mainQuest.TargetAreaContains(m_position))
			{
				m_currentHeroMainQuest.IncreaseProgressCount();
			}
		}
	}

	public void ProcessMainQuestForHunt(MonsterInstance monsterInst)
	{
		if (!base.isDead && m_currentHeroMainQuest != null && !m_currentHeroMainQuest.isObjectiveCompleted)
		{
			MainQuest mainQuest = m_currentHeroMainQuest.mainQuest;
			if (mainQuest.type == 2 && mainQuest.targetMonster.id == monsterInst.monster.id)
			{
				m_currentHeroMainQuest.IncreaseProgressCountByMonsterQuest(monsterInst.instanceId);
			}
		}
	}

	public void ProcessMainQuestForAcquisition(MonsterInstance monsterInst)
	{
		if (!base.isDead && m_currentHeroMainQuest != null && !m_currentHeroMainQuest.isObjectiveCompleted)
		{
			MainQuest mainQuest = m_currentHeroMainQuest.mainQuest;
			if (mainQuest.type == 3 && mainQuest.targetMonster.id == monsterInst.monster.id && Util.DrawLots(mainQuest.targetAcquisitionRate))
			{
				m_currentHeroMainQuest.IncreaseProgressCountByMonsterQuest(monsterInst.instanceId);
			}
		}
	}

	private void ProcessMainQuestForInteraction(ContinentObject continentObject)
	{
		if (!base.isDead && m_currentHeroMainQuest != null && !m_currentHeroMainQuest.isObjectiveCompleted)
		{
			MainQuest mainQuest = m_currentHeroMainQuest.mainQuest;
			if (mainQuest.type == 4 && mainQuest.targetObjectId == continentObject.id)
			{
				m_currentHeroMainQuest.IncreaseProgressCount();
			}
		}
	}

	public bool IsAvailableMainQuestInteractionObject(int nObjectId)
	{
		if (m_currentHeroMainQuest == null)
		{
			return false;
		}
		if (m_currentHeroMainQuest.isObjectiveCompleted)
		{
			return false;
		}
		MainQuest mainQuest = m_currentHeroMainQuest.mainQuest;
		if (mainQuest.type != 4)
		{
			return false;
		}
		if (mainQuest.targetObjectId != nObjectId)
		{
			return false;
		}
		return true;
	}

	public void ProcessMainQuestForContent(int nId)
	{
		if (m_currentHeroMainQuest != null && !m_currentHeroMainQuest.isObjectiveCompleted)
		{
			MainQuest mainQuest = m_currentHeroMainQuest.mainQuest;
			if (mainQuest.type == 7 && mainQuest.targetContentId == nId)
			{
				m_currentHeroMainQuest.IncreaseProgressCount();
			}
		}
	}

	private void OnUpdate_ProcessTreatOfFarmQuestMissionForMonsterSpawn()
	{
		try
		{
			ProcessTreatOfFarmQuestMissionForMonsterSpawn();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void ProcessTreatOfFarmQuestMissionForMonsterSpawn()
	{
		if (base.isDead || m_treatOfFarmQuest == null)
		{
			return;
		}
		HeroTreatOfFarmQuestMission heroTreatOfFarmMission = m_treatOfFarmQuest.currentMission;
		if (heroTreatOfFarmMission != null && heroTreatOfFarmMission.targetMonsterInst == null)
		{
			TreatOfFarmQuestMission mission = heroTreatOfFarmMission.mission;
			if (m_currentPlace is ContinentInstance currentPlace && currentPlace.IsSame(mission.targetContinent.id, m_nationInst.nationId) && mission.IsTargetAreaPosition(m_position, radius))
			{
				DateTimeOffset currentTime = DateTimeUtil.currentTime;
				TreatOfFarmQuest quest = Resource.instance.treatOfFarmQuest;
				_ = quest.monsterKillLimitTime;
				TreatOfFarmQuestMonsterArrange monsterArrange = heroTreatOfFarmMission.targetMonsterArrange;
				TreatOfFarmQuestMonsterInstance monsterInst = new TreatOfFarmQuestMonsterInstance();
				monsterInst.Init(currentPlace, monsterArrange, this, heroTreatOfFarmMission.id, currentTime);
				currentPlace.SpawnMonster(monsterInst, currentTime);
				heroTreatOfFarmMission.targetMonsterInst = monsterInst;
				ServerEvent.SendTreatOfFarmQuestMissionMonsterSpawned(m_account.peer, monsterInst.instanceId, monsterInst.position, monsterInst.remainingLifetime);
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
				dbWork.AddSqlCommand(GameDac.CSC_UpdateTreatOfFamrMission_MonsterSpawn(heroTreatOfFarmMission.id, currentTime));
				dbWork.Schedule();
			}
		}
	}

	public void ProcessTreatOfFarmQuestMission(TreatOfFarmQuestMonsterInstance monsterInst, DateTimeOffset currentTime)
	{
		if (m_treatOfFarmQuest == null)
		{
			return;
		}
		HeroTreatOfFarmQuestMission heroTreatOfFarmMission = m_treatOfFarmQuest.currentMission;
		if (heroTreatOfFarmMission != null && !(heroTreatOfFarmMission.id != monsterInst.questMissionInstanceId))
		{
			if (!base.isDead && monsterInst.currentPlace == m_currentPlace && monsterInst.IsQuestAreaPosition(m_position))
			{
				ProcessTreatOfFarmQuestMission_Complete(currentTime);
			}
			else
			{
				ProcessTreatOfFarmQuestMission_Fail(currentTime);
			}
		}
	}

	private void ProcessTreatOfFarmQuestMission_Complete(DateTimeOffset currentTime)
	{
		DateTime currentDate = currentTime.Date;
		HeroTreatOfFarmQuestMission heroTreatOfFarmMission = m_treatOfFarmQuest.currentMission;
		TreatOfFarmQuest treatOfFarmQuest = Resource.instance.treatOfFarmQuest;
		m_treatOfFarmQuest.CompleteCurrentMission(currentTime);
		TreatOfFarmQuestReward treatOfFarmReward = treatOfFarmQuest.GetReward(m_nLevel);
		HashSet<InventorySlot> changedInventorySlots = new HashSet<InventorySlot>();
		Mail mail = null;
		long lnRewardExp = 0L;
		if (treatOfFarmReward != null)
		{
			lnRewardExp = treatOfFarmReward.missionCompletionExpRewardValue;
			ItemReward itemReward = treatOfFarmReward.missionCompletionItemReward;
			if (lnRewardExp > 0)
			{
				lnRewardExp = (long)Math.Floor((float)lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_nLevel));
				AddExp(lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
			}
			int nRemainingItemCount = 0;
			if (itemReward != null)
			{
				nRemainingItemCount = AddItem(itemReward.item, itemReward.owned, itemReward.count, changedInventorySlots);
			}
			if (nRemainingItemCount > 0)
			{
				mail = Mail.Create("MAIL_REWARD_N_2", "MAIL_REWARD_D_2", currentTime);
				mail.AddAttachmentWithNo(new MailAttachment(itemReward.item, nRemainingItemCount, itemReward.owned));
				AddMail(mail, bSendEvent: true);
			}
		}
		int nCompletedMissionCount = m_treatOfFarmQuest.completedMissionCount;
		HeroTreatOfFarmQuestMission newHeroTreatOfFamrMission = null;
		if (m_treatOfFarmQuest.regTime.Date == currentDate && !m_treatOfFarmQuest.objectiveCompleted)
		{
			newHeroTreatOfFamrMission = new HeroTreatOfFarmQuestMission(m_treatOfFarmQuest, treatOfFarmQuest.SelectMission());
			m_treatOfFarmQuest.currentMission = newHeroTreatOfFamrMission;
		}
		PDHeroTreatOfFarmQuestMission pdNextMission = newHeroTreatOfFamrMission?.ToPDHeroTreatOfFarmMission(currentTime);
		ServerEvent.SendTreatOfFarmQuestMissionComplete(m_account.peer, nCompletedMissionCount, pdNextMission, lnRewardExp, m_nLevel, m_lnExp, m_nRealMaxHP, m_nHP, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(this));
		foreach (InventorySlot slot in changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateTreatOfFarmMission_Status(heroTreatOfFarmMission.id, 1, currentTime));
		if (newHeroTreatOfFamrMission != null)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddTreatOfFarmQuestMission(newHeroTreatOfFamrMission.id, newHeroTreatOfFamrMission.quest.id, newHeroTreatOfFamrMission.mission.id, currentTime));
		}
		if (mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(mail));
		}
		dbWork.Schedule();
		if (m_treatOfFarmQuest.regTime.Date == currentDate)
		{
			ProcessTodayTask(13, currentDate);
			ProcessRetrievalProgressCount(10, currentDate);
			ProcessMainQuestForContent(4);
			ProcessSubQuestForContent(4);
		}
	}

	private void ProcessTreatOfFarmQuestMission_Fail(DateTimeOffset currentTime)
	{
		m_treatOfFarmQuest.FailCurrentMission(currentTime, bSendEvent: true);
	}

	public void RefreshFreeImmediateRevivalDailyCount(DateTime date)
	{
		if (!(m_freeImmediateRevivalDailyCount.date == date))
		{
			m_freeImmediateRevivalDailyCount.date = date;
			m_freeImmediateRevivalDailyCount.value = 0;
		}
	}

	public void RefreshPaidImmediateRevivalDailyCount(DateTime date)
	{
		if (!(m_paidImmediateRevivalDailyCount.date == date))
		{
			m_paidImmediateRevivalDailyCount.date = date;
			m_paidImmediateRevivalDailyCount.value = 0;
		}
	}

	public void ClearPaidImmediateRevivalDailyCount(DateTime date)
	{
		RefreshPaidImmediateRevivalDailyCount(date);
		m_paidImmediateRevivalDailyCount.value = 0;
	}

	public bool Revive(bool bSendEvent)
	{
		if (!base.isDead)
		{
			return false;
		}
		m_nHP = m_nRealMaxHP;
		ClearReceivedDamages();
		if (bSendEvent && m_currentPlace != null)
		{
			ServerEvent.SendHeroRevived(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_nHP, m_position, m_fRotationY);
		}
		return true;
	}

	public void StartContinentObjectInteraction(ContinentObjectInstance continentObjectInst)
	{
		if (continentObjectInst == null)
		{
			throw new ArgumentNullException("continentObjectInst");
		}
		m_currentInteractionContinentObjectInst = continentObjectInst;
		if (continentObjectInst.isPublic)
		{
			continentObjectInst.interactionHero = this;
		}
		ContinentObject continentObject = continentObjectInst.obj;
		int nDuration = (int)(continentObject.interactionDuration * 1000f);
		if (nDuration < 0)
		{
			nDuration = 0;
		}
		m_continentObjectInteractionTimer = new Timer(OnContinentObjectInteractionTimerTick, continentObjectInst, -1, -1);
		m_continentObjectInteractionTimer.Change(nDuration, -1);
		ServerEvent.SendHeroContinentObjectInteractionStart(m_currentPlace.GetInterestClientPeers(m_sector, continentObjectInst.sector, m_id), m_id, continentObjectInst.instanceId);
	}

	private void OnContinentObjectInteractionTimerTick(object state)
	{
		AddWork(new SFAction<ContinentObjectInstance>(ProcessContinentObjectInteractionTimerTick, (ContinentObjectInstance)state), bGlobalLockRequired: true);
	}

	private void ProcessContinentObjectInteractionTimerTick(ContinentObjectInstance continentObjectInst)
	{
		DisposeContinentObjectInteractionTimer();
		if (m_currentInteractionContinentObjectInst == continentObjectInst)
		{
			ServerEvent.SendHeroContinentObjectInteractionFinished(m_currentPlace.GetInterestClientPeers(m_sector, m_currentInteractionContinentObjectInst.sector, Guid.Empty), m_id, m_currentInteractionContinentObjectInst.instanceId);
			((ContinentInstance)m_currentPlace).OnContinentObjectInteractionFinished(m_currentInteractionContinentObjectInst);
			m_currentInteractionContinentObjectInst = null;
			DateTimeOffset currentTime = DateTimeUtil.currentTime;
			ProcessMainQuestForInteraction(continentObjectInst.obj);
			ProcessGuildMissionForAnnounce(continentObjectInst.obj);
			ProcessGuildHuntingQuestForInteract(continentObjectInst.obj);
			ProcessDailyQuestForInteraction(continentObjectInst.obj, currentTime);
			ProcessWeeklyQuestRoundForInteraction(continentObjectInst.obj, currentTime);
			ProcessSubQuestForInteraction(continentObjectInst.obj);
			ProcessBiographyQuestsForInteraction(continentObjectInst.obj);
			ProcessCreatureFarmQuestMissionForInteraction(continentObjectInst.obj, currentTime);
			ProcessJobChangeQuestForInteraction(continentObjectInst.obj);
		}
	}

	private void DisposeContinentObjectInteractionTimer()
	{
		if (m_continentObjectInteractionTimer != null)
		{
			m_continentObjectInteractionTimer.Dispose();
			m_continentObjectInteractionTimer = null;
		}
	}

	public void CancelContinentObjectInteraction(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		if (m_currentInteractionContinentObjectInst != null)
		{
			DisposeContinentObjectInteractionTimer();
			long lnContinentObjectInstId = m_currentInteractionContinentObjectInst.instanceId;
			if (bSendEventToMyself)
			{
				ServerEvent.SendHeroContinentObjectInteractionCancel(new ClientPeer[1] { m_account.peer }, m_id, lnContinentObjectInstId);
			}
			if (bSendEventToOthers)
			{
				ServerEvent.SendHeroContinentObjectInteractionCancel(m_currentPlace.GetInterestClientPeers(m_sector, m_currentInteractionContinentObjectInst.sector, m_id), m_id, lnContinentObjectInstId);
			}
			m_currentInteractionContinentObjectInst.interactionHero = null;
			m_currentInteractionContinentObjectInst = null;
		}
	}

	public bool IsAvailableInteractionObject(int nObjectId, DateTimeOffset currentTime)
	{
		if (!IsAvailableMainQuestInteractionObject(nObjectId) && !IsAvailableGuildMissionInteractionObject(nObjectId) && !IsAvailableGuildHuntingQuestInteractionObject(nObjectId) && !IsAvailableDailyQuestInteractionObject(nObjectId, currentTime) && !IsAvailableWeeklyQuestRoundInteractionObject(nObjectId) && !IsAvailableSubQuestInteractionObject(nObjectId) && !IsAvailableBiographyQuestsInteractionObject(nObjectId) && !IsAvailableCreatureFarmQuestMissionInteractionObject(nObjectId))
		{
			return IsAvailableJobChangeQuestInteractionObject(nObjectId);
		}
		return true;
	}

	private void AddPartyInvitation(PartyInvitation invitation)
	{
		m_partyInvitations.Add(invitation.no, invitation);
	}

	private void RemovePartyInvitation(long lnNo)
	{
		m_partyInvitations.Remove(lnNo);
	}

	public PartyInvitation GetPartyInvitation(long lnNo)
	{
		if (!m_partyInvitations.TryGetValue(lnNo, out var value))
		{
			return null;
		}
		return value;
	}

	public void OnPartyInvited(PartyInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		AddPartyInvitation(invitation);
		ServerEvent.SendPartyInvitationArrived(m_account.peer, invitation.ToPDPartyInvitation());
	}

	public void OnPartyInvitationCanceled(PartyInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		RemovePartyInvitation(invitation.no);
		ServerEvent.SendPartyInvitationCanceled(m_account.peer, invitation.no);
	}

	public void OnPartyInvitationLifetimeEnded(PartyInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		RemovePartyInvitation(invitation.no);
		ServerEvent.SendPartyInvitationLifetimeEnded(m_account.peer, invitation.no);
	}

	public void AcceptPartyInvitation(PartyInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		RemovePartyInvitation(invitation.no);
		invitation.party.OnInvitationAccepted(invitation);
	}

	public void RefusePartyInvitation(PartyInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		RemovePartyInvitation(invitation.no);
		invitation.party.OnInvitationRefused(invitation);
	}

	public void RefuseAllPartyInvitations()
	{
		PartyInvitation[] array = m_partyInvitations.Values.ToArray();
		foreach (PartyInvitation invitation in array)
		{
			RefusePartyInvitation(invitation);
		}
	}

	public PartyApplication ApplyToParty(Party party, DateTimeOffset time)
	{
		if (party == null)
		{
			throw new ArgumentNullException("party");
		}
		PartyApplication app = new PartyApplication(party, this, time);
		AddPartyApplication(app);
		party.OnApplied(app);
		return app;
	}

	private void AddPartyApplication(PartyApplication app)
	{
		m_partyApplications.Add(app.no, app);
	}

	private void RemovePartyApplication(PartyApplication app)
	{
		m_partyApplications.Remove(app.no);
		app.Release();
	}

	public PartyApplication GetPartyApplication(long lnNo)
	{
		if (!m_partyApplications.TryGetValue(lnNo, out var value))
		{
			return null;
		}
		return value;
	}

	public bool ContainsPartyApplicationForParty(Guid partyId)
	{
		foreach (PartyApplication app in m_partyApplications.Values)
		{
			if (app.party.id == partyId)
			{
				return true;
			}
		}
		return false;
	}

	public void OnPartyApplicationLifetimeEnded(PartyApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		RemovePartyApplication(app);
		ServerEvent.SendPartyApplicationLifetimeEnded(m_account.peer, app.no);
	}

	public void OnPartyApplicationAccepted(PartyApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		RemovePartyApplication(app);
		ServerEvent.SendPartyApplicationAccepted(m_account.peer, app.no, app.party.ToPDParty(DateTimeUtil.currentTime));
	}

	public void OnPartyApplicationRefused(PartyApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		RemovePartyApplication(app);
		ServerEvent.SendPartyApplicationRefused(m_account.peer, app.no);
	}

	public void CancelPartyApplication(PartyApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		RemovePartyApplication(app);
		app.party.OnApplicationCanceled(app);
	}

	public void CancelAllPartyApplications()
	{
		PartyApplication[] array = m_partyApplications.Values.ToArray();
		foreach (PartyApplication app in array)
		{
			CancelPartyApplication(app);
		}
	}

	public void StartReturnScrollUse(int nDuration, int nInventorySlotIndex)
	{
		if (nDuration < 0)
		{
			throw new ArgumentOutOfRangeException("nDuration");
		}
		if (!m_bIsReturnScrollUsing)
		{
			m_returnScrollUseTimer = new Timer(OnReturnScrollUseTimerTick, nInventorySlotIndex, -1, -1);
			m_returnScrollUseTimer.Change(nDuration * 1000, -1);
			m_bIsReturnScrollUsing = true;
			ServerEvent.SendHeroReturnScrollUseStart(base.currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
		}
	}

	private void OnReturnScrollUseTimerTick(object state)
	{
		AddWork(new SFAction<int>(ProcessReturnScrollUseTimerTick, (int)state), bGlobalLockRequired: true);
	}

	private void ProcessReturnScrollUseTimerTick(int nInventorySlotIndex)
	{
		DisposeReturnScrollUseTimer();
		if (!m_bIsReturnScrollUsing)
		{
			return;
		}
		InventorySlot inventorySlot = GetInventorySlot(nInventorySlotIndex);
		if (inventorySlot == null || !(inventorySlot.obj is ItemInventoryObject obj))
		{
			return;
		}
		Item item = obj.item;
		if (item.type.id != 2 || m_currentPlace == null || (base.currentPlace.placeType != PlaceType.NationContinent && base.currentPlace.placeType != PlaceType.DisputeContinent) || base.isDead)
		{
			return;
		}
		Resource res = Resource.instance;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		Continent targetContinent = res.GetContinent(res.saftyRevivalContinentId);
		if (targetContinent == null)
		{
			return;
		}
		int nTargetNationId = nationId;
		Vector3 targetPosition = res.SelectSaftyRevivalPosition();
		float fTargetRotationY = res.SelectSaftyRevivalYRotation();
		FinishReturnScrollUse(currentTime);
		int nUsedOwnCount = 0;
		int nUsedUnOwnCount = 0;
		List<InventorySlot> changedInventorySlot = new List<InventorySlot>();
		UseItem(item.id, bFisetUseOwn: true, 1, changedInventorySlot, out nUsedOwnCount, out nUsedUnOwnCount);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (InventorySlot slot in changedInventorySlot)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateOrDeleteInventorySlot(slot));
		}
		dbWork.Schedule();
		ServerEvent.SendHeroReturnScrollUseFinished(base.currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
		m_currentPlace.Exit(this, isLogOut: false, new ContinentEnterForReturnScrollUseParam(targetContinent, nTargetNationId, targetPosition, fTargetRotationY));
		ServerEvent.SendReturnScrollUseFinished(m_account.peer, changedInventorySlot[0].ToPDInventorySlot(), targetContinent.id, nTargetNationId);
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddItemUseLog(Guid.NewGuid(), m_id, item.id, nUsedOwnCount, nUsedUnOwnCount, currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void DisposeReturnScrollUseTimer()
	{
		if (m_returnScrollUseTimer != null)
		{
			m_returnScrollUseTimer.Dispose();
			m_returnScrollUseTimer = null;
		}
	}

	public void FinishReturnScrollUse(DateTimeOffset time)
	{
		if (m_bIsReturnScrollUsing)
		{
			m_bIsReturnScrollUsing = false;
			m_returnScrollLastUsedTime = time;
		}
	}

	public void CancelReturnScrollUse(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		if (m_bIsReturnScrollUsing)
		{
			DisposeReturnScrollUseTimer();
			m_bIsReturnScrollUsing = false;
			if (bSendEventToMyself)
			{
				ServerEvent.SendReturnScrollUseCancel(m_account.peer);
			}
			if (bSendEventToOthers && m_currentPlace != null)
			{
				ServerEvent.SendHeroReturnScrollUseCancel(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
			}
		}
	}

	public void CancelAllExclusiveActions()
	{
		CancelContinentObjectInteraction(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelReturnScrollUse(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelFishing(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelMysteryBoxPicking(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelSecretLetterPicking(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelDimensionRaidInteraction(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelGuildFarmQuestInteraction(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelGuildAltarSpellInjectionMission(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelGroggyMonsterItemSteal(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelWisdomTempleObjectInteraction(bSendEvent: false);
		CancelRuinsReclaimObjectInteraction(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelTrueHeroQuestInteraction(bSendEventToMyself: false, bSendEventToOthers: false);
		CancelWarMemoryTransformationObjectInteraction(bSendEventToMyself: false, bSendEventToOthers: false);
	}

	public bool IsChattingIntervalElapsed(DateTimeOffset time)
	{
		return (time - m_lastChatttingTime).TotalSeconds >= (double)((float)Resource.instance.chattingMinInterval * 0.9f);
	}

	public void AddLevelUpReward(LevelUpRewardEntry entry)
	{
		if (entry == null)
		{
			throw new ArgumentNullException("entry");
		}
		m_receivedLevelUpRewards.Add(entry.id);
	}

	public bool IsLevelUpRewardReceived(int nEntryId)
	{
		return m_receivedLevelUpRewards.Contains(nEntryId);
	}

	public void AddAccessReward(HeroAccessReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_accessRewards.Add(reward.id, reward);
	}

	public List<int> GetAccessRewardEntryId()
	{
		List<int> insts = new List<int>();
		foreach (HeroAccessReward reward in m_accessRewards.Values)
		{
			insts.Add(reward.id);
		}
		return insts;
	}

	public bool IsAccessRewardReceived(DateTime date, int nEntryId)
	{
		foreach (HeroAccessReward reward in m_accessRewards.Values)
		{
			if (date == reward.date && nEntryId == reward.id)
			{
				return true;
			}
		}
		return false;
	}

	public void RefreshDailyAccessTime(DateTimeOffset currentTime)
	{
		if (m_dailyAccessTimeUpdateTime.Date == currentTime.Date)
		{
			m_fDailyAccessTime += (float)(currentTime - m_dailyAccessTimeUpdateTime).TotalSeconds;
			m_dailyAccessTimeUpdateTime = currentTime;
		}
		else
		{
			m_fDailyAccessTime = (float)currentTime.TimeOfDay.TotalSeconds;
			m_dailyAccessTimeUpdateTime = currentTime;
			m_accessRewards.Clear();
		}
	}

	public void RefreshExpPotionDailyUseCount(DateTime date)
	{
		if (!(date == m_expPotionDailyUseCount.date))
		{
			m_expPotionDailyUseCount.date = date;
			m_expPotionDailyUseCount.value = 0;
		}
	}

	public void AddHeroMainQuestDungeonReward(HeroMainQuestDungeonReward reward)
	{
		if (reward == null)
		{
			throw new ArgumentNullException("reward");
		}
		m_heroMainQuestDungeonRewards.Add(reward);
	}

	public HeroMainQuestDungeonReward GetHeroMainQuestDungeonReward(int nDungeonId, int nStep)
	{
		foreach (HeroMainQuestDungeonReward reward in m_heroMainQuestDungeonRewards)
		{
			if (reward.dungeonId == nDungeonId && reward.step == nStep)
			{
				return reward;
			}
		}
		return null;
	}

	public bool ContainsMount(int nMountId)
	{
		if (nMountId <= 0)
		{
			throw new ArgumentOutOfRangeException("nMountId");
		}
		return m_mounts.ContainsKey(nMountId);
	}

	public void AddMount(HeroMount mount)
	{
		if (mount == null)
		{
			throw new ArgumentNullException("mount");
		}
		m_mounts.Add(mount.mount.id, mount);
	}

	public HeroMount GetMount(int nId)
	{
		if (!m_mounts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDHeroMount> GetPDHeroMounts()
	{
		List<PDHeroMount> insts = new List<PDHeroMount>();
		foreach (HeroMount mount in m_mounts.Values)
		{
			insts.Add(mount.ToPDHeroMount());
		}
		return insts;
	}

	public void AddMountGear(HeroMountGear gear)
	{
		if (gear == null)
		{
			throw new ArgumentNullException("gear");
		}
		m_mountGears.Add(gear.id, gear);
	}

	public void RemoveMountGear(Guid id)
	{
		m_mountGears.Remove(id);
	}

	public HeroMountGear GetMountGear(Guid id)
	{
		if (!m_mountGears.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDHeroMountGear> GetPDHeroMountGears()
	{
		List<PDHeroMountGear> insts = new List<PDHeroMountGear>();
		foreach (HeroMountGear gear in m_mountGears.Values)
		{
			insts.Add(gear.ToPDHeroMountGear());
		}
		return insts;
	}

	public HeroMountGearSlot GetMountGearSlot(int nSlotIndex)
	{
		if (nSlotIndex < 0 || nSlotIndex >= m_mountGearSlots.Length)
		{
			return null;
		}
		return m_mountGearSlots[nSlotIndex];
	}

	public List<Guid> GetPDEquippedMountGearSlots()
	{
		List<Guid> insts = new List<Guid>();
		HeroMountGearSlot[] mountGearSlots = m_mountGearSlots;
		foreach (HeroMountGearSlot slot in mountGearSlots)
		{
			if (!slot.isEmpty)
			{
				insts.Add(slot.heroMountGear.id);
			}
		}
		return insts;
	}

	public void RefreshMountGearDailyRefinementCount(DateTime date)
	{
		if (!(date == m_mountGearRefinementDailyCount.date))
		{
			m_mountGearRefinementDailyCount.date = date;
			m_mountGearRefinementDailyCount.value = 0;
		}
	}

	protected override void SendMaxHPChangedEventToOthers()
	{
		if (m_currentPlace != null)
		{
			ServerEvent.SendHeroMaxHpChangedEvent(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_nRealMaxHP, m_nHP);
		}
	}

	public bool ContainsWing(int nWingId)
	{
		return m_wings.ContainsKey(nWingId);
	}

	public void AddWing(HeroWing wing)
	{
		if (wing == null)
		{
			throw new ArgumentNullException("wing");
		}
		m_wings.Add(wing.wing.id, wing);
	}

	public HeroWing GetWing(int nId)
	{
		if (!m_wings.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<int> GetHeroWings()
	{
		List<int> insts = new List<int>();
		foreach (HeroWing wing in m_wings.Values)
		{
			insts.Add(wing.wing.id);
		}
		return insts;
	}

	public void EquipWing(HeroWing wing)
	{
		if (wing == null)
		{
			throw new ArgumentNullException("wing");
		}
		if (m_equippedWing != wing)
		{
			m_equippedWing = wing;
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroEquippedWingChanged(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_equippedWing.wing.id);
			}
		}
	}

	public void AddWingPart(HeroWingPart part)
	{
		if (part == null)
		{
			throw new ArgumentNullException("part");
		}
		m_wingParts.Add(part.part.id, part);
	}

	public HeroWingPart GetWingPart(int nId)
	{
		if (!m_wingParts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDHeroWingPart> GetPDHeroWingParts()
	{
		List<PDHeroWingPart> insts = new List<PDHeroWingPart>();
		foreach (HeroWingPart part in m_wingParts.Values)
		{
			insts.Add(part.ToPDHeroWingPart());
		}
		return insts;
	}

	public void AddWingEnchantExp(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount == 0)
		{
			return;
		}
		WingStep wingStep = m_wingStepLevel.step;
		if (wingStep.isLastStep && m_wingStepLevel.isLastLevel)
		{
			return;
		}
		m_nWingExp += nAmount;
		int nNextLevelUpRequiredExp = m_wingStepLevel.nextLevelUpRequiredExp;
		if (m_nWingExp >= nNextLevelUpRequiredExp)
		{
			if (m_wingStepLevel.isLastLevel)
			{
				m_wingStepLevel = Resource.instance.GetWingStep(wingStep.step + 1).GetLevel(1);
			}
			else
			{
				m_wingStepLevel = wingStep.GetLevel(m_wingStepLevel.level + 1);
			}
			m_nWingExp -= nNextLevelUpRequiredExp;
		}
		if (wingStep.isLastStep && m_wingStepLevel.isLastLevel)
		{
			m_nWingExp = 0;
		}
	}

	public int GetStoryDungeonClearMaxDifficulty(int nDungeonNo)
	{
		if (!m_storyDungeonClearMaxDifficulties.TryGetValue(nDungeonNo, out var nValue))
		{
			return 0;
		}
		return nValue;
	}

	public void SetStoryDungeonClearMaxDifficulty(int nDungeonNo, int nDifficulty)
	{
		m_storyDungeonClearMaxDifficulties[nDungeonNo] = nDifficulty;
	}

	public List<PDStoryDungeonClear> GetPDStoryDungeonClears()
	{
		List<PDStoryDungeonClear> clears = new List<PDStoryDungeonClear>();
		foreach (int nDungeonNo in m_storyDungeonClearMaxDifficulties.Keys)
		{
			int nDifficulty = GetStoryDungeonClearMaxDifficulty(nDungeonNo);
			if (nDifficulty > 0)
			{
				PDStoryDungeonClear clear = new PDStoryDungeonClear();
				clear.dungeonNo = nDungeonNo;
				clear.clearMaxDifficulty = nDifficulty;
				clears.Add(clear);
			}
		}
		return clears;
	}

	private StoryDungeonPlay GetSotryDungeonPlay(int nDungeonNo, DateTime date)
	{
		foreach (StoryDungeonPlay play in m_storyDungeonPlays)
		{
			if (play.dungeonNo == nDungeonNo && play.date == date)
			{
				return play;
			}
		}
		return null;
	}

	public StoryDungeonPlay GetOrCreateStoryDungeonPlay(int nDungeonNo, DateTime date)
	{
		StoryDungeonPlay play = GetSotryDungeonPlay(nDungeonNo, date);
		if (play == null)
		{
			play = new StoryDungeonPlay(nDungeonNo, date, 0);
			m_storyDungeonPlays.Add(play);
		}
		return play;
	}

	public List<PDStoryDungeonPlay> GetPDStoryDungeonPlays(DateTime date)
	{
		List<PDStoryDungeonPlay> plays = new List<PDStoryDungeonPlay>();
		foreach (StoryDungeonPlay storyDungeonPlay in m_storyDungeonPlays)
		{
			if (storyDungeonPlay.date == date && storyDungeonPlay.enterCount > 0)
			{
				plays.Add(storyDungeonPlay.ToPDStoryDungeonPlay());
			}
		}
		return plays;
	}

	public int GetStoryDungeonAvailableEnterCount(int nDungeonNo, DateTime date)
	{
		StoryDungeon storyDungeon = Resource.instance.GetStoryDungeon(nDungeonNo);
		if (storyDungeon == null)
		{
			throw new ArgumentOutOfRangeException("nDungeonNo");
		}
		return Math.Max(storyDungeon.enterCount - GetOrCreateStoryDungeonPlay(nDungeonNo, date).enterCount, 0);
	}

	public void StoryDungeonTrapHit(int nDamage, DateTimeOffset time)
	{
		if (!hitEnabled || nDamage <= 0)
		{
			return;
		}
		HashSet<long> removedAbnormalStateEffects = new HashSet<long>();
		List<AbnormalStateEffect> absorbShieldEffects = new List<AbnormalStateEffect>();
		AbnormalStateEffect immortalityEffect = null;
		foreach (AbnormalStateEffect effect in m_abnormalStateEffects.Values)
		{
			switch (effect.abnormalState.id)
			{
			case 8:
				nDamage = (int)Math.Floor((float)nDamage * ((float)effect.abnormalStateLevel.value1 / 10000f));
				break;
			case 10:
			case 102:
			case 109:
				absorbShieldEffects.Add(effect);
				break;
			case 106:
				if (immortalityEffect == null)
				{
					immortalityEffect = effect;
				}
				break;
			}
		}
		List<PDAbnormalStateEffectDamageAbsorbShield> changedAbnormalStateEffectDamageAbsorbShields = new List<PDAbnormalStateEffectDamageAbsorbShield>();
		absorbShieldEffects.Sort(AbnormalStateEffect.Compare);
		foreach (AbnormalStateEffect absorbShieldEffect in absorbShieldEffects)
		{
			if (nDamage > 0)
			{
				nDamage = absorbShieldEffect.AbsorbDamage(nDamage);
				int nRemainingDamageAbsorbShield = absorbShieldEffect.damageAbsorbShieldRemainingAmount;
				if (nRemainingDamageAbsorbShield <= 0 && (absorbShieldEffect.abnormalState.id == 102 || absorbShieldEffect.abnormalState.id == 10))
				{
					absorbShieldEffect.Stop();
					RemoveAbnormalStateEffect(absorbShieldEffect.id);
					removedAbnormalStateEffects.Add(absorbShieldEffect.id);
				}
				PDAbnormalStateEffectDamageAbsorbShield shield = new PDAbnormalStateEffectDamageAbsorbShield();
				shield.abnormalStateEffectInstanceId = absorbShieldEffect.id;
				shield.remainingAbsorbShieldAmount = nRemainingDamageAbsorbShield;
				changedAbnormalStateEffectDamageAbsorbShields.Add(shield);
				continue;
			}
			break;
		}
		bool bIsImmortalityEffect = false;
		if (nDamage >= m_nHP && immortalityEffect != null)
		{
			bIsImmortalityEffect = immortalityEffect.UseImmortalityEffect();
		}
		Damage(null, nDamage, time, bIsImmortalityEffect, removedAbnormalStateEffects);
		if (base.isDead)
		{
			OnDead();
			m_currentPlace.OnUnitDead(this);
		}
		ServerEvent.SendStoryDungeonTrapHit(m_account.peer, bIsImmortalityEffect, m_nHP, m_nLastDamage, m_nLastHPDamage, changedAbnormalStateEffectDamageAbsorbShields.ToArray(), removedAbnormalStateEffects.ToArray());
	}

	public void RefreshFreeSweepDailyCount(DateTime date)
	{
		if (!(m_freeSweepDailyCount.date == date))
		{
			m_freeSweepDailyCount.date = date;
			m_freeSweepDailyCount.value = 0;
		}
	}

	public void GetOnMount()
	{
		if (!m_bIsRiding && m_equippedMount != null)
		{
			m_bIsRiding = true;
			StopAcceleration();
			RefreshMoveSpeed();
			Mount mount = m_equippedMount.mount;
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroMountGetOn(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, mount.id, m_equippedMount.level);
			}
		}
	}

	public void GetOffMount(bool bSendEventToMyself)
	{
		if (m_bIsRiding)
		{
			m_bIsRiding = false;
			InitMoveStartTime(DateTimeUtil.currentTime);
			RefreshMoveSpeed();
			List<ClientPeer> targetPeers = new List<ClientPeer>();
			if (m_currentPlace != null)
			{
				m_currentPlace.GetDynamicClientPeers(m_sector, m_id, targetPeers);
			}
			if (bSendEventToMyself)
			{
				targetPeers.Add(m_account.peer);
			}
			ServerEvent.SendHeroMountGetOff(targetPeers, m_id);
		}
	}

	public void AddExpDungeonClearDifficulty(int nDifficulty)
	{
		m_expDungeonClearDifficulties.Add(nDifficulty);
	}

	public bool IsClearedExpDungeonDifficulty(int nDifficulty)
	{
		return m_expDungeonClearDifficulties.Contains(nDifficulty);
	}

	public void RefreshDailyExpDungeonPlayCount(DateTime date)
	{
		if (!(m_dailyExpDungeonPlayCount.date == date))
		{
			m_dailyExpDungeonPlayCount.date = date;
			m_dailyExpDungeonPlayCount.value = 0;
		}
	}

	public int GetExpDungeonAvailableEnterCount(DateTime date)
	{
		RefreshDailyExpDungeonPlayCount(date);
		return Math.Max(m_vipLevel.expDungeonEnterCount - m_dailyExpDungeonPlayCount.value, 0);
	}

	public void RefreshExpScrollDailyUseCount(DateTime date)
	{
		if (!(date == m_expScrollDailyUseCount.date))
		{
			m_expScrollDailyUseCount.date = date;
			m_expScrollDailyUseCount.value = 0;
		}
	}

	public void ApplyExpScroll(Item item, DateTimeOffset time)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		if (item.type.id != 11)
		{
			throw new ArgumentException("경험치주문서가 아닙니다. itemId = " + item.id);
		}
		if (item != m_expScrollItem || !IsExpScrollActiveTime(time))
		{
			m_nExpScrollDuration = 0;
			m_expScrollStartTime = time;
			m_expScrollItem = item;
		}
		m_nExpScrollDuration += item.value1;
	}

	private float GetExpScrollElapsedTime(DateTimeOffset time)
	{
		return (float)(time - m_expScrollStartTime.Value).TotalSeconds;
	}

	public float GetExpScrollRemainingTime(DateTimeOffset time)
	{
		if (m_expScrollItem == null)
		{
			return 0f;
		}
		float fExpScrollElapsedTime = GetExpScrollElapsedTime(time);
		if (fExpScrollElapsedTime >= (float)m_nExpScrollDuration)
		{
			return 0f;
		}
		return (float)m_nExpScrollDuration - fExpScrollElapsedTime;
	}

	public bool IsExpScrollActiveTime(DateTimeOffset time)
	{
		return GetExpScrollRemainingTime(time) > 0f;
	}

	public void AddGoldDungeonClearDifficulty(int nDifficulty)
	{
		m_goldDungeonClearDifficulties.Add(nDifficulty);
	}

	public bool IsClearedGoldDungeonDifficulty(int nDifficulty)
	{
		return m_goldDungeonClearDifficulties.Contains(nDifficulty);
	}

	public void RefreshDailyGoldDungeonPlayCount(DateTime date)
	{
		if (!(m_dailyGoldDungeonPlayCount.date == date))
		{
			m_dailyGoldDungeonPlayCount.date = date;
			m_dailyGoldDungeonPlayCount.value = 0;
		}
	}

	public int GetGoldDungeonAvailableEnterCount(DateTime date)
	{
		RefreshDailyGoldDungeonPlayCount(date);
		return Math.Max(m_vipLevel.goldDungeonEnterCount - m_dailyGoldDungeonPlayCount.value, 0);
	}

	public void RefreshUndergroundMazeDailyPlayTime(DateTime date)
	{
		if (!(m_undergroundMazeDate == date))
		{
			m_undergroundMazeDate = date;
			m_fUndergroundMazePlayTime = 0f;
		}
	}

	public void StartUndergroundMazePlay(DateTimeOffset time)
	{
		RefreshUndergroundMazeDailyPlayTime(time.Date);
		m_undergroundMazeStartTime = time;
		m_undergroundMazeLastTickTime = time;
		m_undergroundMazeLogId = Guid.NewGuid();
	}

	public void RefreshBountyHunterQuestDailyStartCount(DateTime date)
	{
		if (!(m_bountyHunterQuestDailyStartCount.date == date))
		{
			m_bountyHunterQuestDailyStartCount.date = date;
			m_bountyHunterQuestDailyStartCount.value = 0;
		}
	}

	public void ProcessBountyHunterQuest(MonsterInstance monsterInst, DateTimeOffset currentTime)
	{
		if (!base.isDead && m_bountyHunterQuest != null && !m_bountyHunterQuest.objectiveCompleted)
		{
			BountyHunterQuest bountyQuest = m_bountyHunterQuest.quest;
			if (bountyQuest.targetMonsterMinLevel <= monsterInst.monster.level && m_bountyHunterQuest.IncreaseProgressCount(monsterInst.instanceId))
			{
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
				dbWork.AddSqlCommand(GameDac.CSC_UpdateBountyHunterQuest_ProgressCount(m_bountyHunterQuest.id, m_bountyHunterQuest.progressCount));
				dbWork.Schedule();
			}
		}
	}

	public void RefreshFishingQuestDailyStartCount(DateTime date)
	{
		if (!(m_fishingQuestDailyStartCount.date == date))
		{
			m_fishingQuestDailyStartCount.date = date;
			m_fishingQuestDailyStartCount.value = 0;
		}
	}

	public void StartFishing()
	{
		FishingQuest fishingQuest = Resource.instance.fishingQuest;
		int nDuration = fishingQuest.castingInterval * 1000;
		if (nDuration < 0)
		{
			nDuration = 0;
		}
		m_fishingTimer = new Timer(OnFishingTimerTick);
		m_fishingTimer.Change(nDuration, nDuration);
		ServerEvent.SendHeroFishingStarted(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
	}

	private void OnFishingTimerTick(object state)
	{
		AddWork(new SFAction(ProcessFishingTimerTick), bGlobalLockRequired: true);
	}

	private void ProcessFishingTimerTick()
	{
		if (m_fishingTimer == null)
		{
			return;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		FishingQuest fishingQuest = Resource.instance.fishingQuest;
		FishingQuestBait fishingQuestBait = m_fishingQuest.bait;
		long lnExpReward = 0L;
		FishingQuestCastingReward fishingQuestCastingReward = fishingQuestBait.GetReward(m_nLevel);
		if (fishingQuestCastingReward != null)
		{
			lnExpReward = fishingQuestCastingReward.expRewardValue;
		}
		if (lnExpReward > 0)
		{
			float fExpRewardFactor = 1f;
			if (m_partyMember != null)
			{
				Party party = m_partyMember.party;
				foreach (PartyMember member in party.members)
				{
					if (m_id == member.id)
					{
						continue;
					}
					Hero memberHero = m_currentPlace.GetHero(member.id);
					if (memberHero == null)
					{
						continue;
					}
					lock (memberHero.syncObject)
					{
						if (!MathUtil.CircleContains(m_position, fishingQuest.partyRadius, memberHero.position) || !memberHero.isFishing)
						{
							continue;
						}
						fExpRewardFactor = fishingQuest.partyExpRewardFactor;
						break;
					}
				}
				lnExpReward = (long)((float)lnExpReward * fExpRewardFactor);
			}
			if (m_currentPlace is GuildTerritoryInstance)
			{
				lnExpReward = (long)((float)lnExpReward * Resource.instance.fishingQuest.guildExpRewardFactor);
				Guild myGuild = m_guildMember.guild;
				if (myGuild.isBlessingBuffRunning)
				{
					lnExpReward = (long)((float)lnExpReward * myGuild.blessingBuff.expRewardFactor);
				}
			}
			lnExpReward = (long)Math.Floor((float)lnExpReward * Cache.instance.GetWorldLevelExpFactor(m_nLevel));
			AddExp(lnExpReward, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		}
		m_fishingQuest.castingCount++;
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddFishingQuestRewardLog(Guid.NewGuid(), m_id, m_fishingQuest.date, m_fishingQuest.startCount, m_fishingQuest.bait.itemId, m_fishingQuest.castingCount, lnExpReward, currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
		ServerEvent.SendFishingCastingCompleted(m_account.peer, m_fishingQuest.castingCount, lnExpReward, m_nLevel, m_lnExp, m_nRealMaxHP, m_nHP);
		if (m_fishingQuest.isCompleted)
		{
			CompleteFishing();
		}
		int nHeroFishingQuestBaitId = ((m_fishingQuest != null) ? m_fishingQuest.bait.itemId : 0);
		int nHeroFishingQuestCastingCount = ((m_fishingQuest != null) ? m_fishingQuest.castingCount : 0);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(this));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_FishingQuest_Casting(m_id, nHeroFishingQuestBaitId, nHeroFishingQuestCastingCount));
		dbWork.Schedule();
	}

	private void DisposeFishingTimer()
	{
		if (m_fishingTimer != null)
		{
			m_fishingTimer.Dispose();
			m_fishingTimer = null;
		}
	}

	public void CancelFishing(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		if (m_fishingTimer != null)
		{
			DisposeFishingTimer();
			if (bSendEventToMyself)
			{
				ServerEvent.SendFishingCanceled(m_account.peer);
			}
			if (bSendEventToOthers && m_currentPlace != null)
			{
				ServerEvent.SendHeroFishingCanceled(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
			}
		}
	}

	public void CompleteFishing()
	{
		DisposeFishingTimer();
		m_fishingQuest = null;
		if (m_currentPlace != null)
		{
			ServerEvent.SendHeroFishingCompleted(m_currentPlace.GetInterestClientPeers(m_sector, m_id), m_id);
		}
	}

	public void IncreasePvpKillCount(int nLevelGap, DateTimeOffset time)
	{
		m_nPvpKillCount++;
		DateTime date = time.Date;
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		int nExploitPoint = 0;
		PvpExploit pvpExploit = Resource.instance.GetPvpExploit(nLevelGap);
		if (pvpExploit != null)
		{
			nExploitPoint = pvpExploit.killExploitPointRewardValue;
		}
		BattlefieldSupportEvent battlefieldSupportEvent = Resource.instance.battlefieldSupportEvent;
		if (battlefieldSupportEvent != null && battlefieldSupportEvent.IsEventTime(fTime))
		{
			nExploitPoint = (int)Math.Floor((float)nExploitPoint * battlefieldSupportEvent.killExploitPointFactor);
		}
		DimensionInfiltrationEvent dimensionInfiltrationEvent = Resource.instance.dimensionInfiltrationEvent;
		if (dimensionInfiltrationEvent != null && dimensionInfiltrationEvent.IsEventTime(fTime))
		{
			nExploitPoint = (int)Math.Floor((float)nExploitPoint * dimensionInfiltrationEvent.killExploitPointFactor);
		}
		AddExploitPoint(nExploitPoint, time, bSaveToDB: true);
		ServerEvent.SendPvpKill(m_account.peer, date, m_nPvpKillCount, m_nExploitPoint, m_dailyExploitPoint.value);
	}

	public void ClearPvpKillCount()
	{
		if (m_nPvpKillCount != 0)
		{
			m_nPvpKillCount = 0;
		}
	}

	public void IncreasePvpAssistCount(int nLevelGap, DateTimeOffset time)
	{
		m_nPvpAssistCount++;
		DateTime date = time.Date;
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		int nExploitPoint = 0;
		PvpExploit pvpExploit = Resource.instance.GetPvpExploit(nLevelGap);
		if (pvpExploit != null)
		{
			nExploitPoint = pvpExploit.assistExploitPointRewardValue;
		}
		BattlefieldSupportEvent battlefieldSupportEvent = Resource.instance.battlefieldSupportEvent;
		if (battlefieldSupportEvent != null && battlefieldSupportEvent.IsEventTime(fTime))
		{
			nExploitPoint = (int)Math.Floor((float)nExploitPoint * battlefieldSupportEvent.killExploitPointFactor);
		}
		DimensionInfiltrationEvent dimensionInfiltrationEvent = Resource.instance.dimensionInfiltrationEvent;
		if (dimensionInfiltrationEvent != null && dimensionInfiltrationEvent.IsEventTime(fTime))
		{
			nExploitPoint = (int)Math.Floor((float)nExploitPoint * dimensionInfiltrationEvent.killExploitPointFactor);
		}
		AddExploitPoint(nExploitPoint, time, bSaveToDB: true);
		ServerEvent.SendPvpAssist(m_account.peer, date, m_nPvpAssistCount, m_nExploitPoint, m_dailyExploitPoint.value);
	}

	public void ClearPvpAssistCount()
	{
		if (m_nPvpAssistCount != 0)
		{
			m_nPvpAssistCount = 0;
		}
	}

	public void RefreshDailyExploitPoint(DateTime date)
	{
		if (!(m_dailyExploitPoint.date == date))
		{
			m_dailyExploitPoint.date = date;
			m_dailyExploitPoint.value = 0;
		}
	}

	private int AddDailyExploitPoint(int nAmount, DateTime date)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount <= 0)
		{
			return 0;
		}
		RefreshDailyExploitPoint(date);
		int nOldValue = m_dailyExploitPoint.value;
		m_dailyExploitPoint.value += nAmount;
		if (m_dailyExploitPoint.value > m_vipLevel.dailyMaxExploitPoint)
		{
			m_dailyExploitPoint.value = m_vipLevel.dailyMaxExploitPoint;
		}
		return m_dailyExploitPoint.value - nOldValue;
	}

	public int AddExploitPoint(int nAmount, DateTimeOffset time, bool bSaveToDB)
	{
		int nAddedAmount = AddDailyExploitPoint(nAmount, time.Date);
		if (nAddedAmount <= 0)
		{
			return 0;
		}
		m_nExploitPoint += nAddedAmount;
		m_exploitPointUpdateTime = time;
		if (bSaveToDB)
		{
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Exploit(this));
			dbWork.Schedule();
		}
		ProcessOrdealQuestMissions(OrdealQuestMissionType.ExploitPoint, nAddedAmount, time);
		return nAddedAmount;
	}

	public void SetRank(Rank rank)
	{
		if (rank == null)
		{
			throw new ArgumentNullException("rank");
		}
		m_rank = rank;
		_ = Resource.instance;
		if (m_rank.activeSkills.Count == 0 && m_rank.passiveSkills.Count == 0)
		{
			return;
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (RankActiveSkill rankActiveSkill in m_rank.activeSkills.Values)
		{
			HeroRankActiveSkill heroRankActiveSkill = new HeroRankActiveSkill(this, rankActiveSkill);
			heroRankActiveSkill.level = 1;
			AddRankActiveSkill(heroRankActiveSkill);
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroRankActiveSkill(m_id, heroRankActiveSkill.skillId, heroRankActiveSkill.level));
			if (m_selectedRankActiveSkill == null)
			{
				m_selectedRankActiveSkill = heroRankActiveSkill;
				dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SelectedRankActiveSkill(m_id, m_selectedRankActiveSkill.skillId));
			}
		}
		foreach (RankPassiveSkill rankPassiveSkill in m_rank.passiveSkills.Values)
		{
			HeroRankPassiveSkill heroRankPassiveSkill = new HeroRankPassiveSkill(this, rankPassiveSkill);
			heroRankPassiveSkill.level = 1;
			AddRankPassiveSkill(heroRankPassiveSkill);
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroRankPassiveSkill(m_id, heroRankPassiveSkill.skillId, heroRankPassiveSkill.level));
		}
		dbWork.Schedule();
	}

	public void RefreshDailyMysteryBoxQuestStartCount(DateTime date)
	{
		if (!(m_dailyMysteryBoxQuestStartCount.date == date))
		{
			m_dailyMysteryBoxQuestStartCount.date = date;
			m_dailyMysteryBoxQuestStartCount.value = 0;
		}
	}

	public void CancelMysteryBoxPicking(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		if (m_mysteryBoxQuest != null)
		{
			m_mysteryBoxQuest.CancelPick(bSendEventToMyself, bSendEventToOthers);
		}
	}

	public void RefreshDailySecretLetterQuestStartCount(DateTime date)
	{
		if (!(m_dailySecretLetterQuestStartCount.date == date))
		{
			m_dailySecretLetterQuestStartCount.date = date;
			m_dailySecretLetterQuestStartCount.value = 0;
		}
	}

	public void CancelSecretLetterPicking(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		if (m_secretLetterQuest != null)
		{
			m_secretLetterQuest.CancelPick(bSendEventToMyself, bSendEventToOthers);
		}
	}

	public void RefreshDailyDimensionRaidQuestStartCount(DateTime date)
	{
		if (!(m_dailyDimensionRaidQuestStartCount.date == date))
		{
			m_dailyDimensionRaidQuestStartCount.date = date;
			m_dailyDimensionRaidQuestStartCount.value = 0;
		}
	}

	public void CancelDimensionRaidInteraction(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		if (m_dimensionRaidQuest != null)
		{
			m_dimensionRaidQuest.CancelInteraction(bSendEventToMyself, bSendEventToOthers);
		}
	}

	public void RefreshDailyHolyWarQuestStartScheduleCollection(DateTime date)
	{
		if (!(m_dailyHolyWarQuestStartScheduleCollection.date == date))
		{
			m_dailyHolyWarQuestStartScheduleCollection.date = date;
			m_dailyHolyWarQuestStartScheduleCollection.ClearSchedules();
		}
	}

	public void ProcessHolyWarQuest(DateTimeOffset time)
	{
		if (m_holyWarQuest != null && !(m_holyWarQuest.GetRemainingTime(time) <= 0f))
		{
			m_holyWarQuest.IncreaseKillCount();
		}
	}

	public void SetArtifactRoomCurrentFloor(int nfloor)
	{
		m_nArtifactRoomCurrentFloor = nfloor;
		int nCompletedFloor = nfloor - 1;
		if (m_nArtifactRoomBestFloor < nCompletedFloor)
		{
			m_nArtifactRoomBestFloor = nCompletedFloor;
		}
	}

	public void InitArtifactRoomFloor()
	{
		m_nArtifactRoomCurrentFloor = 1;
	}

	public void RefreshAartifactRoomDailyInitCount(DateTime date)
	{
		if (!(m_artifactRoomDailyInitCount.date == date))
		{
			m_artifactRoomDailyInitCount.date = date;
			m_artifactRoomDailyInitCount.value = 0;
		}
	}

	public int GetArtifactRoomAvailableInitCount(DateTime date)
	{
		RefreshAartifactRoomDailyInitCount(date);
		return Math.Max(m_vipLevel.artifactRoomInitMaxCount - m_artifactRoomDailyInitCount.value, 0);
	}

	public void StartArtifactRoomSweep(DateTimeOffset time)
	{
		m_artifactRoomSweepStartTime = time;
		m_nArtifactRoomSweepProgressFloor = m_nArtifactRoomCurrentFloor;
	}

	public void StopArtifactRoomSweep()
	{
		m_artifactRoomSweepStartTime = null;
		m_nArtifactRoomSweepProgressFloor = 0;
	}

	public void CompleteArtifactRoomSweep()
	{
		m_artifactRoomSweepStartTime = null;
		m_bIsArtifactRoomSweepCompleted = false;
		m_nArtifactRoomSweepProgressFloor = 0;
	}

	public float GetArtifactRoomSweepRemainingTime(DateTimeOffset time)
	{
		if (!m_artifactRoomSweepStartTime.HasValue)
		{
			return 0f;
		}
		float fRemainingTime = 0f;
		foreach (ArtifactRoomFloor floor in Resource.instance.artifactRoom.floors)
		{
			if (floor.floor <= m_nArtifactRoomBestFloor)
			{
				if (floor.floor >= m_nArtifactRoomSweepProgressFloor)
				{
					fRemainingTime += (float)floor.sweepDuration;
				}
				continue;
			}
			break;
		}
		return Math.Max(fRemainingTime - (float)(time - m_artifactRoomSweepStartTime.Value).TotalSeconds, 0f);
	}

	private void AddSeriesMission(HeroSeriesMission mission)
	{
		if (mission == null)
		{
			throw new ArgumentNullException("mission");
		}
		m_seriesMissions.Add(mission.mission.id, mission);
	}

	public HeroSeriesMission GetSeriesMission(int nId)
	{
		if (!m_seriesMissions.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveSeriesMission(int nId)
	{
		m_seriesMissions.Remove(nId);
	}

	public void ProcessSeriesMission(int nMissionId)
	{
		GetSeriesMission(nMissionId)?.IncreaseProgressCount();
	}

	public List<PDHeroSeriesMission> GetPDHeroSeriesMissions()
	{
		List<PDHeroSeriesMission> insts = new List<PDHeroSeriesMission>();
		foreach (HeroSeriesMission mission in m_seriesMissions.Values)
		{
			insts.Add(mission.ToPDHeroSeriesMission());
		}
		return insts;
	}

	public void ProcessTodayMission(int nMissionId, DateTimeOffset currentTime)
	{
		if (!(currentTime.Date != m_todayMissionCollection.date))
		{
			HeroTodayMission mission = m_todayMissionCollection.GetMission(nMissionId);
			if (mission != null && !mission.isObjectiveCompleted)
			{
				mission.IncreaseProgressCount();
			}
		}
	}

	private void OnDateChanged_RefreshTodayMissionList()
	{
		try
		{
			RefreshTodayMissionList(m_currentUpdateTime.Date);
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void RefreshTodayMissionList(DateTime currentDate)
	{
		if (!(m_todayMissionCollection.date == currentDate))
		{
			m_todayMissionCollection = new HeroTodayMissionCollection(this, currentDate);
			m_todayMissionCollection.InitializeMissions();
			ServerEvent.SendTodayMissionListChanged(m_account.peer, currentDate, m_todayMissionCollection.GetPDHeroTodayMissions().ToArray());
		}
	}

	private void OnDateChanged_RefreshWeeklyQuest()
	{
		try
		{
			InitWeeklyQuest(m_currentUpdateTime, bSendEvent: true);
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public void RefreshTodayTaskCollection(DateTime date)
	{
		if (!(date == m_todayTaskCollection.date))
		{
			m_todayTaskCollection.date = date;
			m_todayTaskCollection.ClearTasks();
		}
	}

	public void ProcessTodayTask(int nId, DateTime date)
	{
		RefreshTodayTaskCollection(date);
		RefreshAchievementDailyPoint(date);
		m_todayTaskCollection.ProcessTask(nId);
	}

	public void RefreshAchievementDailyPoint(DateTime date)
	{
		if (!(date == m_achivementDailyPoint.date))
		{
			m_achivementDailyPoint.date = date;
			m_achivementDailyPoint.value = 0;
			m_nReceivedAchivementRewardNo = 0;
		}
	}

	public void RefreshDailyAncientRelicPlayCount(DateTime date)
	{
		if (!(m_dailyAncientRelicPlayCount.date == date))
		{
			m_dailyAncientRelicPlayCount.date = date;
			m_dailyAncientRelicPlayCount.value = 0;
		}
	}

	public int GetAncientRelicAvailableEnterCount(DateTime date)
	{
		RefreshDailyAncientRelicPlayCount(date);
		return Math.Max(m_vipLevel.ancientRelicEnterCount - m_dailyAncientRelicPlayCount.value, 0);
	}

	public void CancelMatching(int nBanishType)
	{
		if (m_matchingRoom != null)
		{
			m_matchingRoom.BanishHero(this, nBanishType);
		}
	}

	public void HitAncientRelicTrap(int nDamage, int nMoveSpeed, int nDuration, DateTimeOffset time)
	{
		if (!hitEnabled || nDamage < 0)
		{
			return;
		}
		float fElapsedTime = (float)(time - m_ancientRelicTrapLastHitTime).TotalSeconds;
		if (fElapsedTime < 3f)
		{
			return;
		}
		if (m_ancientRelicAbnormalStateEffect != null)
		{
			m_ancientRelicAbnormalStateEffect.Stop();
		}
		m_ancientRelicTrapLastHitTime = time;
		HashSet<long> removedAbnormalStateEffects = new HashSet<long>();
		List<AbnormalStateEffect> absorbShieldEffects = new List<AbnormalStateEffect>();
		AbnormalStateEffect immortalityEffect = null;
		foreach (AbnormalStateEffect effect2 in m_abnormalStateEffects.Values)
		{
			switch (effect2.abnormalState.id)
			{
			case 8:
				nDamage = (int)Math.Floor((float)nDamage * ((float)effect2.abnormalStateLevel.value1 / 10000f));
				break;
			case 10:
			case 102:
			case 109:
				absorbShieldEffects.Add(effect2);
				break;
			case 106:
				if (immortalityEffect == null)
				{
					immortalityEffect = effect2;
				}
				break;
			}
		}
		List<PDAbnormalStateEffectDamageAbsorbShield> changedAbnormalStateEffectDamageAbsorbShields = new List<PDAbnormalStateEffectDamageAbsorbShield>();
		absorbShieldEffects.Sort(AbnormalStateEffect.Compare);
		foreach (AbnormalStateEffect absorbShieldEffect in absorbShieldEffects)
		{
			if (nDamage > 0)
			{
				nDamage = absorbShieldEffect.AbsorbDamage(nDamage);
				int nRemainingDamageAbsorbShield = absorbShieldEffect.damageAbsorbShieldRemainingAmount;
				if (nRemainingDamageAbsorbShield <= 0 && (absorbShieldEffect.abnormalState.id == 102 || absorbShieldEffect.abnormalState.id == 10))
				{
					absorbShieldEffect.Stop();
					RemoveAbnormalStateEffect(absorbShieldEffect.id);
					removedAbnormalStateEffects.Add(absorbShieldEffect.id);
				}
				PDAbnormalStateEffectDamageAbsorbShield shield = new PDAbnormalStateEffectDamageAbsorbShield();
				shield.abnormalStateEffectInstanceId = absorbShieldEffect.id;
				shield.remainingAbsorbShieldAmount = nRemainingDamageAbsorbShield;
				changedAbnormalStateEffectDamageAbsorbShields.Add(shield);
				continue;
			}
			break;
		}
		bool bIsImmortalityEffect = false;
		if (nDamage >= m_nHP && immortalityEffect != null)
		{
			bIsImmortalityEffect = immortalityEffect.UseImmortalityEffect();
		}
		Damage(null, nDamage, time, bIsImmortalityEffect, removedAbnormalStateEffects);
		if (base.isDead)
		{
			OnDead();
			m_currentPlace.OnUnitDead(this);
		}
		else
		{
			HeroAncientRelicTrapEffect effect = new HeroAncientRelicTrapEffect();
			effect.Init(this, nMoveSpeed, nDuration, time);
			m_ancientRelicAbnormalStateEffect = effect;
			RefreshMoveSpeed();
		}
		ServerEvent.SendAncientRelicTrapHit(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_id, bIsImmortalityEffect, m_nHP, m_nLastDamage, m_nLastHPDamage, changedAbnormalStateEffectDamageAbsorbShields.ToArray(), removedAbnormalStateEffects.ToArray());
	}

	public void ProcessAncientRelicTrapAbnormalStateEffectFinished()
	{
		if (m_ancientRelicAbnormalStateEffect != null)
		{
			m_ancientRelicAbnormalStateEffect = null;
			RefreshMoveSpeed();
			ServerEvent.SendAncientRelicTrapEffectFinished(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_id);
		}
	}

	public void StopAncientRelicTrapAbnormalStateEffect()
	{
		if (m_ancientRelicAbnormalStateEffect != null)
		{
			m_ancientRelicAbnormalStateEffect.Stop();
			m_ancientRelicAbnormalStateEffect = null;
			RefreshMoveSpeed();
		}
	}

	public void RefreshDistortionScrollDailyUseCount(DateTime date)
	{
		if (!(date == m_distortionScrollDailyUseCount.date))
		{
			m_distortionScrollDailyUseCount.date = date;
			m_distortionScrollDailyUseCount.value = 0;
		}
	}

	public float GetRemainingDistortionTime(DateTimeOffset time)
	{
		if (!m_distortionScrollStartTime.HasValue)
		{
			return 0f;
		}
		float fElapsedTime = (float)(time - m_distortionScrollStartTime.Value).TotalSeconds;
		if (fElapsedTime >= (float)m_nDistortionScrollDuration)
		{
			return 0f;
		}
		return (float)m_nDistortionScrollDuration - fElapsedTime;
	}

	public void StartDistortion(DateTimeOffset startTime, int nDuration)
	{
		m_distortionScrollStartTime = startTime;
		m_nDistortionScrollDuration = nDuration;
		StartDistortionTimer(nDuration * 1000);
	}

	public void StartDistortionTimer(int nDuration)
	{
		if (nDuration > 0)
		{
			m_distortionTimer = new Timer(OnDistortionTimerTick);
			m_distortionTimer.Change(nDuration, -1);
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroDistortionStarted(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
			}
		}
	}

	private void OnDistortionTimerTick(object state)
	{
		AddWork(new SFAction(ProcessDistortionTimerTick), bGlobalLockRequired: false);
	}

	private void ProcessDistortionTimerTick()
	{
		DisposeDistortionTimer();
		if (m_currentPlace != null)
		{
			ServerEvent.SendHeroDistortionFinished(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
		}
	}

	private void DisposeDistortionTimer()
	{
		if (m_distortionTimer != null)
		{
			m_distortionTimer.Dispose();
			m_distortionTimer = null;
		}
	}

	public void CancelDistortion(bool bSendEventToMyself)
	{
		if (m_distortionTimer != null)
		{
			DisposeDistortionTimer();
			m_distortionScrollStartTime = null;
			m_nDistortionScrollDuration = 0;
			if (bSendEventToMyself)
			{
				ServerEvent.SendDistortionCanceled(m_account.peer);
			}
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroDistortionCanceled(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
			}
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_DistortionCanceled(m_id));
			dbWork.Schedule();
		}
	}

	public FieldOfHonorHero ToFieldOfHonorHero()
	{
		FieldOfHonorHero inst = new FieldOfHonorHero();
		inst.ranking = m_nFieldOfHonorRanking;
		inst.id = m_id;
		inst.job = m_job;
		inst.nation = m_nationInst.nation;
		inst.name = m_sName;
		inst.level = m_nLevel;
		inst.battlePower = m_lnBattlePower;
		inst.rankId = rankNo;
		inst.equippedWingId = equippedWingId;
		inst.wingStep = m_wingStepLevel.step.step;
		inst.wingLevel = m_wingStepLevel.level;
		inst.displayTitleId = displayTitleId;
		if (m_guildMember != null)
		{
			inst.guildId = m_guildMember.guild.id;
			inst.guildName = m_guildMember.guild.name;
			inst.guildMemberGrade = m_guildMember.grade.id;
		}
		inst.mainGearEnchantLevelSetNo = mainGearEnchantLevelSetNo;
		inst.subGearSoulstoneLevelSetNo = subGearSoulstoneLevelSetNo;
		inst.customPresetHair = m_nCustomPresetHair;
		inst.customFaceJawHeight = m_nCustomFaceJawHeight;
		inst.customFaceJawWidth = m_nCustomFaceJawWidth;
		inst.customFaceJawEndHeight = m_nCustomFaceJawEndHeight;
		inst.customFaceWidth = m_nCustomFaceWidth;
		inst.customFaceEyebrowHeight = m_nCustomFaceEyebrowHeight;
		inst.customFaceEyebrowRotation = m_nCustomFaceEyebrowRotation;
		inst.customFaceEyesWidth = m_nCustomFaceEyesWidth;
		inst.customFaceNoseHeight = m_nCustomFaceNoseHeight;
		inst.customFaceNoseWidth = m_nCustomFaceNoseWidth;
		inst.customFaceMouthHeight = m_nCustomFaceMouthHeight;
		inst.customFaceMouthWidth = m_nCustomFaceMouthWidth;
		inst.customBodyHeadSize = m_nCustomBodyHeadSize;
		inst.customBodyArmsLength = m_nCustomBodyArmsLength;
		inst.customBodyArmsWidth = m_nCustomBodyArmsWidth;
		inst.customBodyChestSize = m_nCustomBodyChestSize;
		inst.customBodyWaistWidth = m_nCustomBodyWaistWidth;
		inst.customBodyHipsSize = m_nCustomBodyHipsSize;
		inst.customBodyPelvisWidth = m_nCustomBodyPelvisWidth;
		inst.customBodyLegsLength = m_nCustomBodyLegsLength;
		inst.customBodyLegsWidth = m_nCustomBodyLegsWidth;
		inst.customColorSkin = m_nCustomColorSkin;
		inst.customColorEyes = m_nCustomColorEyes;
		inst.customColorBeardAndEyebrow = m_nCustomColorBeardAndEyebrow;
		inst.customColorHair = m_nCustomColorHair;
		inst.costumeId = ((m_equippedCostume != null) ? m_equippedCostume.costumeId : 0);
		inst.costumeEffectId = ((m_equippedCostume != null) ? m_equippedCostume.costumeEffectId : 0);
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 1, m_nNormalMaxHP));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 2, m_nNormalPhysicalOffense));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 3, m_nNormalMagicalOffense));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 4, m_nNormalPhysicalDefense));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 5, m_nNormalMagicalDefense));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 6, m_nNormalCritical));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 7, m_nNormalCriticalResistance));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 8, m_nNormalCriticalDamageIncRate));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 9, m_nNormalCriticalDamageDecRate));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 10, m_nNormalPenetration));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 11, m_nNormalBlock));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 12, m_nNormalFireOffense));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 13, m_nNormalFireDefense));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 14, m_nNormalLightningOffense));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 15, m_nNormalLightningDefense));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 16, m_nNormalDarkOffense));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 17, m_nNormalDarkDefense));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 18, m_nNormalHolyOffense));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 19, m_nNormalHolyDefense));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 20, m_nNormalDamageIncRate));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 21, m_nNormalDamageDecRate));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 22, m_nNormalStunResistance));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 23, m_nNormalSnareResistance));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 24, m_nNormalSilenceResistance));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 25, m_nNormalBaseMaxHPIncRate));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 26, m_nNormalBaseOffenseIncRate));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 27, m_nNormalBasePhysicalDefenseIncRate));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 28, m_nNormalBaseMagicalDefenseIncRate));
		inst.AddRealAttr(CreateFieldOfHonorHeroRealAttr(inst, 29, m_nNormalOffense));
		foreach (HeroSkill skill in m_skills.Values)
		{
			inst.AddSkill(skill.ToFieldOfHonorHeroSkill(inst));
		}
		foreach (HeroWing wing in m_wings.Values)
		{
			FieldOfHonorHeroWing fieldOfHonorHeroWing = wing.ToFieldOfHonorHeroWing(inst);
			inst.AddWing(fieldOfHonorHeroWing);
		}
		foreach (HeroWingPart wingPart in m_wingParts.Values)
		{
			FieldOfHonorHeroWingPart fieldOfHonorHeroWingPart = wingPart.ToFieldOfHonorHeroWingPart(inst);
			fieldOfHonorHeroWingPart.RefreshAttrTotalValues();
			inst.AddWingPart(fieldOfHonorHeroWingPart);
		}
		if (m_equippedWeapon != null)
		{
			inst.SetEquippedMainGear(m_equippedWeapon.ToFieldOfHonorHeroEquippedMainGear(inst));
			inst.equippedWeapon.RefreshAttrTotalValues();
		}
		if (m_equippedArmor != null)
		{
			inst.SetEquippedMainGear(m_equippedArmor.ToFieldOfHonorHeroEquippedMainGear(inst));
			inst.equippedArmor.RefreshAttrTotalValues();
		}
		foreach (HeroSubGear heroSubGear in m_subGears.Values)
		{
			if (heroSubGear.equipped)
			{
				FieldOfHonorHeroEquippedSubGear fieldOfHonorHeroEquippedsubGear = heroSubGear.ToFieldOfHonorHeroEquippedSubGear(inst);
				fieldOfHonorHeroEquippedsubGear.RefreshAttrTotalValues();
				inst.AddEquippedSubGear(fieldOfHonorHeroEquippedsubGear);
			}
		}
		return inst;
	}

	private FieldOfHonorHeroRealAttr CreateFieldOfHonorHeroRealAttr(FieldOfHonorHero fieldOfHonorHero, int nAttrId, int nAttrValue)
	{
		FieldOfHonorHeroRealAttr inst = new FieldOfHonorHeroRealAttr(fieldOfHonorHero);
		inst.id = nAttrId;
		inst.value = nAttrValue;
		return inst;
	}

	public void RefreshDailyFieldOfHonorPlayCount(DateTime date)
	{
		if (!(m_dailyFieldOfHonorPlayCount.date == date))
		{
			m_dailyFieldOfHonorPlayCount.date = date;
			m_dailyFieldOfHonorPlayCount.value = 0;
		}
	}

	public int GetFieldOfHonorAvailableEnterCount(DateTime date)
	{
		RefreshDailyFieldOfHonorPlayCount(date);
		return Math.Max(m_vipLevel.fieldOfHonorEnterCount - m_dailyFieldOfHonorPlayCount.value, 0);
	}

	public bool ContainsFieldOfHonorTargetRanking(int nRanking)
	{
		return m_fieldOfHonorTargets.Contains(nRanking);
	}

	public void SetHeroByFieldOfHonorHero(FieldOfHonorHero fieldOfHonorHero)
	{
		if (fieldOfHonorHero == null)
		{
			throw new ArgumentNullException("fieldOfHonorHero");
		}
		Resource res = Resource.instance;
		m_job = fieldOfHonorHero.job;
		m_nationInst = Cache.instance.GetNationInstance(fieldOfHonorHero.nationId);
		m_sName = fieldOfHonorHero.name;
		m_nLevel = fieldOfHonorHero.level;
		m_lnBattlePower = fieldOfHonorHero.battlePower;
		m_rank = res.GetRank(fieldOfHonorHero.rankId);
		m_fieldOfHonorHeroGuildId = fieldOfHonorHero.guildId;
		m_sFieldOfHonorHeroGuildName = fieldOfHonorHero.guildName;
		m_nFieldOfHonorHeroGuildMemberGrade = fieldOfHonorHero.guildMemberGrade;
		m_nCustomPresetHair = fieldOfHonorHero.customPresetHair;
		m_nCustomFaceJawHeight = fieldOfHonorHero.customFaceJawHeight;
		m_nCustomFaceJawWidth = fieldOfHonorHero.customFaceJawWidth;
		m_nCustomFaceJawEndHeight = fieldOfHonorHero.customFaceJawEndHeight;
		m_nCustomFaceWidth = fieldOfHonorHero.customFaceWidth;
		m_nCustomFaceEyebrowHeight = fieldOfHonorHero.customFaceEyebrowHeight;
		m_nCustomFaceEyebrowRotation = fieldOfHonorHero.customFaceEyebrowRotation;
		m_nCustomFaceEyesWidth = fieldOfHonorHero.customFaceEyesWidth;
		m_nCustomFaceNoseHeight = fieldOfHonorHero.customFaceNoseHeight;
		m_nCustomFaceNoseWidth = fieldOfHonorHero.customFaceNoseWidth;
		m_nCustomFaceMouthHeight = fieldOfHonorHero.customFaceMouthHeight;
		m_nCustomFaceMouthWidth = fieldOfHonorHero.customFaceMouthWidth;
		m_nCustomBodyHeadSize = fieldOfHonorHero.customBodyHeadSize;
		m_nCustomBodyArmsLength = fieldOfHonorHero.customBodyArmsLength;
		m_nCustomBodyArmsWidth = fieldOfHonorHero.customBodyArmsWidth;
		m_nCustomBodyChestSize = fieldOfHonorHero.customBodyChestSize;
		m_nCustomBodyWaistWidth = fieldOfHonorHero.customBodyWaistWidth;
		m_nCustomBodyHipsSize = fieldOfHonorHero.customBodyHipsSize;
		m_nCustomBodyPelvisWidth = fieldOfHonorHero.customBodyPelvisWidth;
		m_nCustomBodyLegsLength = fieldOfHonorHero.customBodyLegsLength;
		m_nCustomBodyLegsWidth = fieldOfHonorHero.customBodyLegsWidth;
		m_nCustomColorSkin = fieldOfHonorHero.customColorSkin;
		m_nCustomColorEyes = fieldOfHonorHero.customColorEyes;
		m_nCustomColorBeardAndEyebrow = fieldOfHonorHero.customColorBeardAndEyebrow;
		m_nCustomColorHair = fieldOfHonorHero.customColorHair;
		int nCostumeId = fieldOfHonorHero.costumeId;
		if (nCostumeId != 0)
		{
			m_equippedCostume = new HeroCostume(this);
			m_equippedCostume.Init(res.GetCostume(nCostumeId), fieldOfHonorHero.costumeEffectId);
		}
		foreach (FieldOfHonorHeroRealAttr realAttr in fieldOfHonorHero.realAttrs.Values)
		{
			switch (realAttr.id)
			{
			case 1:
				m_nNormalMaxHP = realAttr.value;
				break;
			case 2:
				m_nNormalPhysicalOffense = realAttr.value;
				break;
			case 3:
				m_nNormalMagicalOffense = realAttr.value;
				break;
			case 4:
				m_nNormalPhysicalDefense = realAttr.value;
				break;
			case 5:
				m_nNormalMagicalDefense = realAttr.value;
				break;
			case 6:
				m_nNormalCritical = realAttr.value;
				break;
			case 7:
				m_nNormalCriticalResistance = realAttr.value;
				break;
			case 8:
				m_nNormalCriticalDamageIncRate = realAttr.value;
				break;
			case 9:
				m_nNormalCriticalDamageDecRate = realAttr.value;
				break;
			case 10:
				m_nNormalPenetration = realAttr.value;
				break;
			case 11:
				m_nNormalBlock = realAttr.value;
				break;
			case 12:
				m_nNormalFireOffense = realAttr.value;
				break;
			case 13:
				m_nNormalFireDefense = realAttr.value;
				break;
			case 14:
				m_nNormalLightningOffense = realAttr.value;
				break;
			case 15:
				m_nNormalLightningDefense = realAttr.value;
				break;
			case 16:
				m_nNormalDarkOffense = realAttr.value;
				break;
			case 17:
				m_nNormalDarkDefense = realAttr.value;
				break;
			case 18:
				m_nNormalHolyOffense = realAttr.value;
				break;
			case 19:
				m_nNormalHolyDefense = realAttr.value;
				break;
			case 20:
				m_nNormalDamageIncRate = realAttr.value;
				break;
			case 21:
				m_nNormalDamageDecRate = realAttr.value;
				break;
			case 22:
				m_nNormalStunResistance = realAttr.value;
				break;
			case 23:
				m_nNormalSnareResistance = realAttr.value;
				break;
			case 24:
				m_nNormalSilenceResistance = realAttr.value;
				break;
			case 25:
				m_nNormalBaseMaxHPIncRate = realAttr.value;
				break;
			case 26:
				m_nNormalBaseOffenseIncRate = realAttr.value;
				break;
			case 27:
				m_nNormalBasePhysicalDefenseIncRate = realAttr.value;
				break;
			case 28:
				m_nNormalBaseMagicalDefenseIncRate = realAttr.value;
				break;
			case 29:
				m_nNormalOffense = realAttr.value;
				break;
			}
		}
		foreach (FieldOfHonorHeroSkill fieldOfHonorHeroSkill in fieldOfHonorHero.skills.Values)
		{
			HeroSkill heroSkill = new HeroSkill(this, m_job.GetSkill(fieldOfHonorHeroSkill.id));
			heroSkill.SetHeroSkillByFieldOfHonorHeroSkill(fieldOfHonorHeroSkill);
			AddSkill(heroSkill);
		}
		foreach (FieldOfHonorHeroWing wing in fieldOfHonorHero.wings.Values)
		{
			HeroWing heroWing = new HeroWing(this, res.GetWing(wing.id));
			AddWing(heroWing);
		}
		int nEquippedWingId = fieldOfHonorHero.equippedWingId;
		if (nEquippedWingId > 0)
		{
			m_equippedWing = GetWing(nEquippedWingId);
		}
		m_wingStepLevel = res.GetWingStep(fieldOfHonorHero.wingStep).GetLevel(fieldOfHonorHero.wingLevel);
		foreach (FieldOfHonorHeroWingPart fieldOfHonorHeroWingPart in fieldOfHonorHero.wingParts)
		{
			HeroWingPart heroWingPart = GetWingPart(fieldOfHonorHeroWingPart.id);
			heroWingPart.SetHeroWingPartByFieldOfHonorHeroWingPart(fieldOfHonorHeroWingPart);
		}
		Title title = Resource.instance.GetTitle(fieldOfHonorHero.displayTitleId);
		if (title != null)
		{
			m_displayTitle = new HeroTitle(this, title);
		}
		FieldOfHonorHeroEquippedMainGear fieldOfHonorHeroEquippedWeapon = fieldOfHonorHero.equippedWeapon;
		if (fieldOfHonorHeroEquippedWeapon != null)
		{
			m_equippedWeapon = new HeroMainGear(this);
			m_equippedWeapon.SetHeroMainGearByFieldOfHonorHeroEquippedMainGear(fieldOfHonorHeroEquippedWeapon);
		}
		FieldOfHonorHeroEquippedMainGear fieldOfHonorHeroEquippedArmor = fieldOfHonorHero.equippedArmor;
		if (fieldOfHonorHeroEquippedArmor != null)
		{
			m_equippedArmor = new HeroMainGear(this);
			m_equippedArmor.SetHeroMainGearByFieldOfHonorHeroEquippedMainGear(fieldOfHonorHeroEquippedArmor);
		}
		foreach (FieldOfHonorHeroEquippedSubGear value in fieldOfHonorHero.equippedSubGears.Values)
		{
			_ = value;
			new HeroSubGear(this);
		}
		RefreshRealValues(bSendMaxHpChangedToOthers: false);
		m_nHP = m_nRealMaxHP;
	}

	public void SortFieldOfHonorHistory()
	{
		m_fieldOfHonorHistories.Sort(FieldOfHonorHistory.Compare);
	}

	public void AddFieldOfHonorHistory(FieldOfHonorHistory history)
	{
		if (history == null)
		{
			throw new ArgumentNullException("history");
		}
		m_fieldOfHonorHistories.Add(history);
	}

	public FieldOfHonorHistory GetFieldOfHonorHistory(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_fieldOfHonorHistories.Count)
		{
			return null;
		}
		return m_fieldOfHonorHistories[nIndex];
	}

	public float GetGuildJoinWaitTime(DateTimeOffset currentTime)
	{
		float fWaitTime = (float)Resource.instance.guildRejoinIntervalTime - (float)(currentTime - m_guildWithdrawalTime).TotalSeconds;
		if (fWaitTime < 0f)
		{
			fWaitTime = 0f;
		}
		return fWaitTime;
	}

	public void RefreshDailyGuildDonationCount(DateTime date)
	{
		if (!(date == m_dailyGuildDonationCount.date))
		{
			m_dailyGuildDonationCount.date = date;
			m_dailyGuildDonationCount.value = 0;
		}
	}

	public void AddGuildApplication(GuildApplication application)
	{
		if (application == null)
		{
			throw new ArgumentNullException("application");
		}
		m_guildApplications.Add(application.id, application);
	}

	public GuildApplication GetGuildApplication(Guid id)
	{
		if (!m_guildApplications.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public bool ContainsGuildApplication(Guid guildId)
	{
		foreach (GuildApplication application in m_guildApplications.Values)
		{
			if (guildId == application.guild.id)
			{
				return true;
			}
		}
		return false;
	}

	public void RemoveGuildApplication(Guid id)
	{
		m_guildApplications.Remove(id);
	}

	public void RefreshDailyGuildApplicationCount(DateTime date)
	{
		if (!(date == m_dailyGuildApplicationCount.date))
		{
			m_dailyGuildApplicationCount.date = date;
			m_dailyGuildApplicationCount.value = 0;
		}
	}

	public void AddGuildInvitation(GuildInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		m_guildInvitations.Add(invitation.id, invitation);
	}

	public void RemoveGuildInvitation(Guid id)
	{
		m_guildInvitations.Remove(id);
	}

	public GuildInvitation GetGuildInvitation(Guid id)
	{
		if (!m_guildInvitations.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public void OnGuildInvitationLifetimeEnded(GuildInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		RemoveGuildInvitation(invitation.id);
		ServerEvent.SendGuildInvitationLifetimeEnded(m_account.peer, invitation.id, invitation.target.id, invitation.target.name);
	}

	public void OnGuildInvited(GuildInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		AddGuildInvitation(invitation);
		ServerEvent.SendGuildInvitationArrived(m_account.peer, invitation.ToPDHeroGuildInvitation());
	}

	public void RefuseGuildInvitation(GuildInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		RemoveGuildInvitation(invitation.id);
		invitation.guild.OnInvitationRefused(invitation);
	}

	public void RefuseAllGuildInvitations()
	{
		GuildInvitation[] array = m_guildInvitations.Values.ToArray();
		foreach (GuildInvitation invitation in array)
		{
			RefuseGuildInvitation(invitation);
		}
	}

	public void OnGuildInvitationCanceled(Guid invitationId)
	{
		RemoveGuildInvitation(invitationId);
		ServerEvent.SendGuildInvitationCanceled(m_account.peer, invitationId);
	}

	public void SetGuildFarmQuest(HeroGuildFarmQuest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_guildFarmQuest = quest;
	}

	public void RemoveGuildFarmQuest()
	{
		if (m_guildFarmQuest != null)
		{
			m_guildFarmQuest.Release();
			m_guildFarmQuest = null;
		}
	}

	public void RefreshDailyGuildFarmQuestStartCount(DateTime date)
	{
		if (!(m_dailyGuildFarmQuestStartCount.date == date))
		{
			m_dailyGuildFarmQuestStartCount.date = date;
			m_dailyGuildFarmQuestStartCount.value = 0;
		}
	}

	public void CancelGuildFarmQuestInteraction(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		if (m_guildFarmQuest != null)
		{
			m_guildFarmQuest.CancelInteraction(bSendEventToMyself, bSendEventToOthers);
		}
	}

	public void RefreshDailyGuildFoodWarehouseStockCount(DateTime date)
	{
		if (!(m_dailyGuildFoodWarehouseStockCount.date == date))
		{
			m_dailyGuildFoodWarehouseStockCount.date = date;
			m_dailyGuildFoodWarehouseStockCount.value = 0;
		}
	}

	public void RefreshGuildMoralPoint(DateTime date)
	{
		if (!(m_guildMoralPoint.date == date))
		{
			m_guildMoralPoint.date = date;
			m_guildMoralPoint.value = 0;
		}
	}

	public int AddGuildMoralPoint(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount == 0)
		{
			return 0;
		}
		int nOldValue = m_guildMoralPoint.value;
		m_guildMoralPoint.value += nAmount;
		int nMaxPoint = Resource.instance.guildAltar.dailyHeroMaxMoralPoint;
		if (m_guildMoralPoint.value > nMaxPoint)
		{
			m_guildMoralPoint.value = nMaxPoint;
		}
		return m_guildMoralPoint.value - nOldValue;
	}

	public void StartGuildAltarSpellInjectionMission()
	{
		HeroGuildAltarSpellInjectionMission mission = new HeroGuildAltarSpellInjectionMission(this);
		mission.Start();
		m_guildAltarSpellInjectionMission = mission;
	}

	public void CancelGuildAltarSpellInjectionMission(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		if (m_guildAltarSpellInjectionMission != null)
		{
			m_guildAltarSpellInjectionMission.Cancel(bSendEventToMyself, bSendEventToOthers);
			m_guildAltarSpellInjectionMission = null;
		}
	}

	public void RemoveGuildAltarSpellInjectionMission()
	{
		if (m_guildAltarSpellInjectionMission != null)
		{
			m_guildAltarSpellInjectionMission.Release();
			m_guildAltarSpellInjectionMission = null;
		}
	}

	public float GetGuildAltarDefenseMissionRemainingCoolTime(DateTimeOffset time)
	{
		return (float)Math.Max((double)Resource.instance.guildAltar.defenseCoolTime - (time - m_guildAltarDefenseMissionStartTime).TotalSeconds, 0.0);
	}

	public bool IsGuildAltarDefenseMissionCoolTimeElapsed(DateTimeOffset time)
	{
		return (time - m_guildAltarDefenseMissionStartTime).TotalSeconds >= (double)((float)Resource.instance.guildAltar.defenseCoolTime * 0.9f);
	}

	public void StartGuildAltarDefenseMission(GuildAltarMonsterInstance monsterInst, DateTimeOffset time)
	{
		m_guildAltarDefenseMission = new HeroGuildAltarDefenseMission(this, monsterInst);
		m_guildAltarDefenseMission.Start();
		m_guildAltarDefenseMissionStartTime = time;
	}

	public void RemoveGuildAltarDefenseMission()
	{
		if (m_guildAltarDefenseMission != null)
		{
			m_guildAltarDefenseMission.Release();
			m_guildAltarDefenseMission = null;
		}
	}

	public void CompleteGuildAltar(DateTimeOffset currentTime)
	{
		if (m_guildMember == null)
		{
			return;
		}
		Guild guild = m_guildMember.guild;
		GuildAltar guildAltar = Resource.instance.guildAltar;
		GuildAltarReward reward = guildAltar.GetReward(m_nLevel);
		int nRewardGuildFund = guildAltar.guildFundRewardValue;
		int nRewardGuildBuildingPoint = guildAltar.guildBuildingPointRewardValue;
		int nRewardGuildContributionPoint = guildAltar.guildContributionPointRewardValue;
		long lnRewardExp = 0L;
		if (reward != null)
		{
			lnRewardExp = reward.expRewardValue;
		}
		guild.AddFund(nRewardGuildFund, m_id);
		guild.AddBuildingPoint(nRewardGuildBuildingPoint, m_id);
		AddGuildContributionPoint(nRewardGuildContributionPoint);
		AddExp(lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		ServerEvent.SendGuildAltarCompleted(m_account.peer, lnRewardExp, m_nLevel, m_lnExp, m_nRealMaxHP, m_nHP, m_guildMember.contributionPoint, m_guildMember.totalContributionPoint, guild.fund, guild.buildingPoint);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(guild.id);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_id));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_Fund(guild.id, guild.fund));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_BuildingPoint(guild.id, guild.buildingPoint));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(this));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_GuildContributionPoint(this));
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildAltarCompletionRewardLog(Guid.NewGuid(), guild.id, m_id, lnRewardExp, nRewardGuildContributionPoint, nRewardGuildFund, nRewardGuildBuildingPoint, currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public void AddGuildSkill(HeroGuildSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_guildSkills.Add(skill.skill.id, skill);
	}

	public HeroGuildSkill GetGuildSkill(int nId)
	{
		if (!m_guildSkills.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDHeroGuildSkill> GetPDHeroGuildSkills()
	{
		List<PDHeroGuildSkill> insts = new List<PDHeroGuildSkill>();
		foreach (HeroGuildSkill skill in m_guildSkills.Values)
		{
			insts.Add(skill.ToPDHeroGuildSkill());
		}
		return insts;
	}

	public bool RefreshGuildSkillRealLevels()
	{
		bool bChanged = false;
		foreach (HeroGuildSkill skill in m_guildSkills.Values)
		{
			if (skill.RefreshRealLevel())
			{
				bChanged = true;
			}
		}
		return bChanged;
	}

	public void ProcessGuildMissionForHunt(MonsterInstance monsterInst, DateTimeOffset currentTime)
	{
		if (!base.isDead && m_guildMissionQuest != null)
		{
			HeroGuildMissionQuestMission heroGuildMission = m_guildMissionQuest.currentMission;
			if (heroGuildMission != null && !heroGuildMission.isCompleted && heroGuildMission.mission.type == GuildMissionType.Hunt && heroGuildMission.mission.targetMonster.id == monsterInst.monster.id)
			{
				m_guildMissionQuest.ProgressCurrentMission(currentTime);
			}
		}
	}

	public void ProcessGuildMissionForSummon(GuildMissionMonsterInstance monsterInst, DateTimeOffset currentTime)
	{
		if (m_guildMissionQuest == null)
		{
			return;
		}
		HeroGuildMissionQuestMission heroGuildMission = m_guildMissionQuest.currentMission;
		if (heroGuildMission != null && !(heroGuildMission.id != monsterInst.questMissionInstanceId))
		{
			if (!base.isDead && monsterInst.currentPlace == m_currentPlace && monsterInst.IsQuestAreaPosition(m_position))
			{
				m_guildMissionQuest.ProgressCurrentMission(currentTime);
			}
			else
			{
				m_guildMissionQuest.FailCurrentMission(currentTime, bSendEvent: true);
			}
		}
	}

	public bool IsAvailableGuildMissionInteractionObject(int nObjectId)
	{
		if (m_guildMissionQuest == null)
		{
			return false;
		}
		if (m_guildMissionQuest.completed)
		{
			return false;
		}
		HeroGuildMissionQuestMission heroGuildMission = m_guildMissionQuest.currentMission;
		if (heroGuildMission == null)
		{
			return false;
		}
		if (heroGuildMission.isCompleted)
		{
			return false;
		}
		GuildMission guildMission = heroGuildMission.mission;
		if (guildMission.type != GuildMissionType.Announce)
		{
			return false;
		}
		if (guildMission.targetContinentObject.id != nObjectId)
		{
			return false;
		}
		return true;
	}

	private void ProcessGuildMissionForAnnounce(ContinentObject continentObject)
	{
		if (base.isDead || m_guildMissionQuest == null || m_guildMissionQuest.completed)
		{
			return;
		}
		HeroGuildMissionQuestMission heroGuildMission = m_guildMissionQuest.currentMission;
		if (heroGuildMission != null && !heroGuildMission.isCompleted)
		{
			GuildMission guildMission = heroGuildMission.mission;
			if (guildMission.type == GuildMissionType.Announce && guildMission.targetContinentObject.id == continentObject.id)
			{
				m_guildMissionQuest.ProgressCurrentMission(DateTimeUtil.currentTime);
				ServerEvent.SendGuildSpiritAnnounced(nationInst.GetClientPeers(Guid.Empty), m_guildMember.guild.id, m_guildMember.guild.name, m_id, m_sName, ((ContinentInstance)m_currentPlace).continent.id);
			}
		}
	}

	public void RefreshGuildDailyObjectiveReceivedReward(DateTime date)
	{
		if (!(date == m_receivedDailyObjectiveRewardNo.date))
		{
			m_receivedDailyObjectiveRewardNo.date = date;
			m_receivedDailyObjectiveRewardNo.value = 0;
		}
	}

	private void LogOut_MatchingRoom()
	{
		if (m_matchingRoom != null)
		{
			m_matchingRoom.BanishHero(this, 5);
			m_matchingRoom = null;
		}
	}

	public void RefreshDailySupplySupportQuestStartCount(DateTime date)
	{
		if (!(m_dailySupplySupportQuestStartCount.date == date))
		{
			m_dailySupplySupportQuestStartCount.date = date;
			m_dailySupplySupportQuestStartCount.value = 0;
		}
	}

	public void AddGuildContributionPoint(int nAmount)
	{
		if (m_guildMember == null)
		{
			throw new InvalidOperationException("길드멤버가 아닙니다.");
		}
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_guildMember.contributionPoint += nAmount;
			m_guildMember.totalContributionPoint += nAmount;
		}
	}

	public void UseGuildContributionPoint(int nAmount)
	{
		if (m_guildMember == null)
		{
			throw new InvalidOperationException("길드멤버가 아닙니다.");
		}
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			if (nAmount > m_guildMember.contributionPoint)
			{
				throw new Exception("공헌점수가 부족합니다.");
			}
			m_guildMember.contributionPoint -= nAmount;
		}
	}

	public void RefreshDailyGuildHuntingQuestCount(DateTime date)
	{
		if (!(date == m_dailyGuildHuntingQuest.date))
		{
			m_dailyGuildHuntingQuest.date = date;
			m_dailyGuildHuntingQuest.value = 0;
		}
	}

	public void ProcessGuildHuntingQuestForHunt(MonsterInstance monsterInst)
	{
		if (base.isDead)
		{
			return;
		}
		HeroGuildHuntingQuest heroGuildHuntingQuest = m_guildHuntingQuest;
		if (heroGuildHuntingQuest != null && !heroGuildHuntingQuest.isObjectiveCompleted)
		{
			GuildHuntingQuestObjective guildHuntingQuestObjective = heroGuildHuntingQuest.objective;
			if (guildHuntingQuestObjective.type == GuildHuntingQuestObjectiveType.Hunt && guildHuntingQuestObjective.targetMonster.id == monsterInst.monster.id)
			{
				m_guildHuntingQuest.IncreaseProgressCount();
			}
		}
	}

	public bool IsAvailableGuildHuntingQuestInteractionObject(int nObjectId)
	{
		HeroGuildHuntingQuest heroGuildHuntingQuest = m_guildHuntingQuest;
		if (heroGuildHuntingQuest == null)
		{
			return false;
		}
		if (heroGuildHuntingQuest.isObjectiveCompleted)
		{
			return false;
		}
		GuildHuntingQuestObjective guildHuntingQuestObjective = heroGuildHuntingQuest.objective;
		if (guildHuntingQuestObjective.type != GuildHuntingQuestObjectiveType.Interact)
		{
			return false;
		}
		if (guildHuntingQuestObjective.targetContinentObject.id != nObjectId)
		{
			return false;
		}
		return true;
	}

	public void ProcessGuildHuntingQuestForInteract(ContinentObject continentObject)
	{
		if (base.isDead)
		{
			return;
		}
		HeroGuildHuntingQuest heroGuildHuntingQuest = m_guildHuntingQuest;
		if (heroGuildHuntingQuest != null && !heroGuildHuntingQuest.isObjectiveCompleted)
		{
			GuildHuntingQuestObjective guildHuntingQuestObjective = heroGuildHuntingQuest.objective;
			if (guildHuntingQuestObjective.type == GuildHuntingQuestObjectiveType.Interact && guildHuntingQuestObjective.targetContinentObject.id == continentObject.id)
			{
				m_guildHuntingQuest.IncreaseProgressCount();
			}
		}
	}

	public void FailGuildHuntingQuest(DateTimeOffset time)
	{
		if (m_guildHuntingQuest != null)
		{
			HeroGuildHuntingQuest heroGuildHuntingQuest = m_guildHuntingQuest;
			m_guildHuntingQuest = null;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildHuntingQuest_Status(heroGuildHuntingQuest.id, 3, time));
			dbWork.Schedule();
		}
	}

	public float GetGuildDailyObjecvtiveNoticeRemainingCoolTime(DateTimeOffset time)
	{
		float fElapsedTime = (float)(time - m_guildDailyObjectiveNoticeTime).TotalSeconds;
		return Math.Max((float)Resource.instance.guildDailyObjectiveNoticeCoolTime - fElapsedTime, 0f);
	}

	public bool IsGuildDailyObjectiveNoticeCoolTimeElapsed(DateTimeOffset time)
	{
		float fElapsedTime = (float)(time - m_guildDailyObjectiveNoticeTime).TotalSeconds;
		return fElapsedTime >= (float)Resource.instance.guildDailyObjectiveNoticeCoolTime * 0.9f;
	}

	public void SetNationNoblesse(NationNoblesse nationNoblesse)
	{
		if (m_nationNoblesse != nationNoblesse)
		{
			m_nationNoblesse = nationNoblesse;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
			ServerEvent.SendMaxHpChanged(m_account.peer, m_nRealMaxHP, m_nHP);
		}
	}

	public void RefreshDailyNationWarFreeTransmissionCount(DateTime date)
	{
		if (!(m_dailyNationWarFreeTransmissionCount.date == date))
		{
			m_dailyNationWarFreeTransmissionCount.date = date;
			m_dailyNationWarFreeTransmissionCount.value = 0;
		}
	}

	public void RefreshDailyNationWarPaidTransmissionCount(DateTime date)
	{
		if (!(m_dailyNationWarFreeTransmissionCount.date == date))
		{
			m_dailyNationWarPaidTransmissionCount.date = date;
			m_dailyNationWarPaidTransmissionCount.value = 0;
		}
	}

	public void NationWarIncreaseKillCount(DateTimeOffset time)
	{
		if (m_nationWarMember != null)
		{
			m_nationWarMember.killCount++;
			m_nationWarMember.lastKillTime = time;
			int nKillCount = m_nationWarMember.killCount;
			m_nAccNationWarKillCount++;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateNationWarMember_KillCount(m_nationWarMember.declaration.id, m_id, nKillCount));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_NationWarKillCount(m_id, m_nAccNationWarKillCount));
			dbWork.Schedule();
			ServerEvent.SendNationWarKillCountUpdated(m_account.peer, nKillCount, m_nAccNationWarKillCount);
			if (nKillCount % 10 == 0)
			{
				m_nationInst.nationWarInst.OnHeroMultiKill(this);
			}
		}
	}

	public void NationWarIncreaseAssistCount()
	{
		if (m_nationWarMember != null)
		{
			m_nationWarMember.assistCount++;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateNationWarMember_AssistCount(m_nationWarMember.declaration.id, m_id, m_nationWarMember.assistCount));
			dbWork.Schedule();
			ServerEvent.SendNationWarAssistCountUpdated(m_account.peer, m_nationWarMember.assistCount);
		}
	}

	public void NationWarIncreaseDeadCount()
	{
		if (m_nationWarMember != null)
		{
			m_nationWarMember.deadCount++;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateNationWarMember_DeadCount(m_nationWarMember.declaration.id, m_id, m_nationWarMember.deadCount));
			dbWork.Schedule();
			ServerEvent.SendNationWarDeadCountUpdated(m_account.peer, m_nationWarMember.deadCount);
		}
	}

	public void NationWarIncreaseImmediateRevivalCount(DateTimeOffset time)
	{
		if (m_nationWarMember != null)
		{
			m_nationWarMember.immediateRevivalCount++;
			m_nAccNationWarImmediateRevivalCount++;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateNationWarMember_ImmediateRevivalCount(m_nationWarMember.declaration.id, m_id, m_nationWarMember.immediateRevivalCount));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_NationWarImmediateRevivalCount(m_id, m_nAccNationWarImmediateRevivalCount));
			dbWork.Schedule();
			ServerEvent.SendNationWarImmediateRevivalCountUpdated(m_account.peer, m_nationWarMember.immediateRevivalCount, m_nAccNationWarImmediateRevivalCount);
		}
	}

	public void JoinAllianceNationWar(NationWarInstance nationWarInst)
	{
		if (nationWarInst == null)
		{
			throw new ArgumentNullException("nationWarInst");
		}
		m_allianceNationWarInst = nationWarInst;
	}

	public void ClearAllianceNationWar()
	{
		if (m_allianceNationWarInst != null)
		{
			m_allianceNationWarInst.RemoveHero(this);
			m_allianceNationWarInst = null;
			m_nAllianceNationWarKillCount = 0;
		}
	}

	public void IncreaseAllianceNationWarKillCount()
	{
		if (m_allianceNationWarInst != null)
		{
			m_nAllianceNationWarKillCount++;
			if (m_nAllianceNationWarKillCount % 10 == 0)
			{
				m_allianceNationWarInst.OnHeroMultiKill(this);
			}
		}
	}

	public void RefreshDailyNationDonationCount(DateTime date)
	{
		if (!(date == m_dailyNationDonationCount.date))
		{
			m_dailyNationDonationCount.date = date;
			m_dailyNationDonationCount.value = 0;
		}
	}

	public void RefreshWeeklySoulCoveterPlayCount(DateTime date)
	{
		DateTime dateOfMonday = DateTimeUtil.GetWeekStartDate(date);
		if (!(m_weeklySoulCoveterPlayCount.date == dateOfMonday))
		{
			m_weeklySoulCoveterPlayCount.date = dateOfMonday;
			m_weeklySoulCoveterPlayCount.value = 0;
		}
	}

	public int GetSoulCoveterAvailableEnterCount(DateTime date)
	{
		RefreshWeeklySoulCoveterPlayCount(date);
		return Math.Max(m_vipLevel.soulCoveterWeeklyEnterCount - m_weeklySoulCoveterPlayCount.value, 0);
	}

	public void AddExplorationPoint(int nAmount)
	{
		if (nAmount != 0)
		{
			if (nAmount < 0)
			{
				throw new ArgumentOutOfRangeException("nAmount");
			}
			m_nExplorationPoint += nAmount;
		}
	}

	public void SetIllustratedBookExplorationStep(IllustratedBookExplorationStep step)
	{
		if (step == null)
		{
			throw new ArgumentNullException("step");
		}
		m_illustratedBookExplorationStep = step;
	}

	public void AddIllustratedBook(HeroIllustratedBook illustratedBook)
	{
		if (illustratedBook == null)
		{
			throw new ArgumentNullException("illustratedBook");
		}
		m_illustratedBooks.Add(illustratedBook.illustratedBook.id, illustratedBook);
	}

	public bool ContainsIllustratedBook(int nIllustratedBookId)
	{
		return m_illustratedBooks.ContainsKey(nIllustratedBookId);
	}

	public List<int> GetIllustratedBooks()
	{
		List<int> results = new List<int>();
		foreach (HeroIllustratedBook illustratedBook in m_illustratedBooks.Values)
		{
			results.Add(illustratedBook.illustratedBook.id);
		}
		return results;
	}

	public void AddSceneryQuestCompletion(int nQuestId)
	{
		m_sceneryQuestCompletions.Add(nQuestId);
	}

	public bool ContainsSceneryQuestCompletion(int nQuestId)
	{
		return m_sceneryQuestCompletions.Contains(nQuestId);
	}

	public void AddSceneryQuest(HeroSceneryQuest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_sceneryQuests.Add(quest.questId, quest);
	}

	public HeroSceneryQuest GetSceneryQuest(int nQuestId)
	{
		if (!m_sceneryQuests.TryGetValue(nQuestId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveSceneryQuest(int nQuestId)
	{
		m_sceneryQuests.Remove(nQuestId);
	}

	private void OnUpdate_UpdateSceneryQuest()
	{
		try
		{
			UpdateSceneryQuest();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void UpdateSceneryQuest()
	{
		DateTimeOffset currentTIme = DateTimeUtil.currentTime;
		HeroSceneryQuest[] array = m_sceneryQuests.Values.ToArray();
		foreach (HeroSceneryQuest quest in array)
		{
			quest.OnUpdate(currentTIme);
		}
	}

	public void AddRewardedAccomplishment(int nId)
	{
		if (nId <= 0)
		{
			throw new ArgumentOutOfRangeException("nId");
		}
		m_rewardedAccomplishments.Add(nId);
	}

	public bool IsRewardedAccomplishment(int nId)
	{
		return m_rewardedAccomplishments.Contains(nId);
	}

	public long GetAccomplishmentProgressingValue(AccomplishmentType type)
	{
		return type switch
		{
			AccomplishmentType.AchieveHeroLevel => m_nLevel, 
			AccomplishmentType.AchieveBattlePower => m_lnMaxBattlePower, 
			AccomplishmentType.AcquireGold => m_lnMaxGold, 
			AccomplishmentType.MainQuestCompletion => completedMainQuestNo, 
			AccomplishmentType.KillMonster => m_nAccMonsterKillCount, 
			AccomplishmentType.SoulCoveterPlayCount => m_nAccSoulCoveterPlayCount, 
			AccomplishmentType.EpicBaitItemUseCount => m_nAccEpicBaitItemUseCount, 
			AccomplishmentType.LegendBaitItemUseCount => m_nAccLegendBaitItemUseCount, 
			AccomplishmentType.NationWarWinCount => m_nAccNationWarWinCount, 
			AccomplishmentType.NationWarKillCount => m_nAccNationWarKillCount, 
			AccomplishmentType.NationWarCommanderkillCount => m_nAccNationWarCommanderKillCount, 
			AccomplishmentType.NationWarImmediateRevivalCount => m_nAccNationWarImmediateRevivalCount, 
			AccomplishmentType.AcquireMainGearGrade => m_nMaxAcquisitionMainGearGrade, 
			AccomplishmentType.EquipMainGearEnchantLevel => m_nMaxEquippedMainGearEnchantLevel, 
			AccomplishmentType.CreatureCardActivation => 0L, 
			_ => 0L, 
		};
	}

	public bool IsCompletedAccomplishmentObjective(AccomplishmentType type, long lnObjectiveValue)
	{
		switch (type)
		{
		case AccomplishmentType.AchieveHeroLevel:
		case AccomplishmentType.AchieveBattlePower:
		case AccomplishmentType.AcquireGold:
		case AccomplishmentType.MainQuestCompletion:
		case AccomplishmentType.KillMonster:
		case AccomplishmentType.SoulCoveterPlayCount:
		case AccomplishmentType.EpicBaitItemUseCount:
		case AccomplishmentType.LegendBaitItemUseCount:
		case AccomplishmentType.NationWarWinCount:
		case AccomplishmentType.NationWarKillCount:
		case AccomplishmentType.NationWarCommanderkillCount:
		case AccomplishmentType.NationWarImmediateRevivalCount:
		case AccomplishmentType.AcquireMainGearGrade:
		case AccomplishmentType.EquipMainGearEnchantLevel:
			return GetAccomplishmentProgressingValue(type) >= lnObjectiveValue;
		case AccomplishmentType.CreatureCardActivation:
			return IsCompletedCreatureCardCollectionCategory((int)lnObjectiveValue);
		default:
			return false;
		}
	}

	public void ProcessAccomplishment_MonsterKill()
	{
		m_nAccMonsterKillCount++;
		ServerEvent.SendAccMonsterKillCountUpdated(m_account.peer, m_nAccMonsterKillCount);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_MonsterKillCount(m_id, m_nAccMonsterKillCount));
		dbWork.Schedule();
	}

	public void AddTitle(HeroTitle title)
	{
		if (title == null)
		{
			throw new ArgumentNullException("title");
		}
		m_titles.Add(title.title.id, title);
	}

	public bool ContainsTitle(int nId)
	{
		return m_titles.ContainsKey(nId);
	}

	public HeroTitle GetTitle(int nTitleId)
	{
		if (!m_titles.TryGetValue(nTitleId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDHeroTitle> GetPDHeroTitles(DateTimeOffset time)
	{
		List<PDHeroTitle> insts = new List<PDHeroTitle>();
		foreach (HeroTitle title in m_titles.Values)
		{
			insts.Add(title.ToPDHeroTitle(time));
		}
		return insts;
	}

	public void SetDisplayTitle(HeroTitle title)
	{
		if (title != m_displayTitle)
		{
			m_displayTitle = title;
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroDisplayTitleChanged(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, displayTitleId);
			}
		}
	}

	public void SetActivationTitle(HeroTitle title)
	{
		m_activationTitle = title;
	}

	public void DeleteTitle(HeroTitle title, bool bInit)
	{
		if (title == null)
		{
			throw new ArgumentNullException("title");
		}
		bool bUpdateActivationTitle = false;
		bool bUpdateDisplayTitle = false;
		if (title.isActivated)
		{
			m_activationTitle = null;
			bUpdateActivationTitle = true;
		}
		if (title.isDisplayed)
		{
			m_displayTitle = null;
			bUpdateDisplayTitle = true;
			if (!bInit && m_currentPlace != null)
			{
				ServerEvent.SendHeroDisplayTitleChanged(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, 0);
			}
		}
		m_titles.Remove(title.title.id);
		RefreshRealValues(bSendMaxHpChangedToOthers: true);
		if (!bInit)
		{
			ServerEvent.SendTitleLifetimeEnded(m_account.peer, title.title.id);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		if (bUpdateDisplayTitle)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_DisplayTitle(m_id, displayTitleId));
		}
		if (bUpdateActivationTitle)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ActivationTitle(m_id, activationTitleId));
		}
		dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroTitle(m_id, title.title.id));
		dbWork.Schedule();
	}

	public void AddCreatureCard(HeroCreatureCard card)
	{
		if (card == null)
		{
			throw new ArgumentNullException("card");
		}
		m_creatureCards.Add(card.card.id, card);
	}

	public bool HasCreatureCard(int nId)
	{
		return m_creatureCards.ContainsKey(nId);
	}

	public HeroCreatureCard GetCreatureCard(int nId)
	{
		if (!m_creatureCards.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public HeroCreatureCard GetOrCreateCreatureCard(CreatureCard creatureCard)
	{
		if (creatureCard == null)
		{
			throw new ArgumentNullException("creatureCard");
		}
		HeroCreatureCard card = GetCreatureCard(creatureCard.id);
		if (card == null)
		{
			card = new HeroCreatureCard(this, creatureCard);
			AddCreatureCard(card);
		}
		return card;
	}

	public HeroCreatureCard IncreaseCreatureCardCount(CreatureCard creatureCard)
	{
		HeroCreatureCard heroCreatureCard = GetOrCreateCreatureCard(creatureCard);
		heroCreatureCard.count++;
		if (Resource.instance.CheckSystemMessageCondition(3, creatureCard.grade.id))
		{
			SystemMessage.SendCreatureCardAcquirement(this, creatureCard.id);
		}
		return heroCreatureCard;
	}

	public void RemoveCreatureCard(int nId)
	{
		m_creatureCards.Remove(nId);
	}

	public void AddActivatedCreatureCardCollection(HeroCreatureCardCollection collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		m_activatedCreatureCardCollections.Add(collection.collection.id, collection);
	}

	public HeroCreatureCardCollection GetActivatedCreatureCardCollection(int nId)
	{
		if (!m_activatedCreatureCardCollections.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool IsCreatureCardCollectionActivated(int nId)
	{
		return m_activatedCreatureCardCollections.ContainsKey(nId);
	}

	public bool IsCompletedCreatureCardCollectionCategory(int nCategoryId)
	{
		CreatureCardCollectionCategory category = Resource.instance.GetCreatureCardCollectionCategory(nCategoryId);
		if (category == null)
		{
			return false;
		}
		foreach (int nCollectionId in category.collections.Keys)
		{
			if (!IsCreatureCardCollectionActivated(nCollectionId))
			{
				return false;
			}
		}
		return true;
	}

	public void AddPurchasedCreatureCardShopFixedProduct(int nProductId)
	{
		m_purchasedCreatureCardShopFixedProducts.Add(nProductId);
	}

	public bool IsPurchasedCreatureCardShopFixedProduct(int nProductId)
	{
		return m_purchasedCreatureCardShopFixedProducts.Contains(nProductId);
	}

	public void AddCreatureCardShopRandomProduct(HeroCreatureCardShopRandomProduct product)
	{
		if (product == null)
		{
			throw new ArgumentNullException("product");
		}
		m_creatureCardShopRandomProducts.Add(product.product.id, product);
	}

	public HeroCreatureCardShopRandomProduct GetCreatureCardShopRandomProduct(int nId)
	{
		if (!m_creatureCardShopRandomProducts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RefreshCreatureCardShopOnSchedule(DateTime date, int nScheduleId, bool bSendEvent, DateTimeOffset currentTime)
	{
		m_creatureCardShopRefreshDate = date;
		m_nCreatureCardShopRefreshScheduleId = nScheduleId;
		RefreshCreatureCardShop();
		if (bSendEvent)
		{
			ServerEvent.SendCreatureCardShopRefreshed(m_account.peer, HeroCreatureCardShopRandomProduct.ToPDHeroCreatureCardShopRandomProducts(m_creatureCardShopRandomProducts.Values).ToArray());
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CreatureCardShopId(m_id, m_creatureCardShopId));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CreatureCardShopSchedule(m_id, m_creatureCardShopRefreshDate, m_nCreatureCardShopRefreshScheduleId));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroCreatureCardshopFixedProductBuy(m_id));
		dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroCreatureCardShopRandomProducts(m_id));
		foreach (HeroCreatureCardShopRandomProduct product2 in m_creatureCardShopRandomProducts.Values)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroCreatureCardShopRandomProduct(product2.hero.id, product2.product.id));
		}
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardShopLog(m_creatureCardShopId, m_id, 2, 0, 0, currentTime));
			foreach (HeroCreatureCardShopRandomProduct product in m_creatureCardShopRandomProducts.Values)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroCreatureCardShopRandomProductLog(m_creatureCardShopId, product.product.id, product.product.creatureCard.id, currentTime));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public void RefreshCreatureCardShop()
	{
		m_creatureCardShopId = Guid.NewGuid();
		m_purchasedCreatureCardShopFixedProducts.Clear();
		m_creatureCardShopRandomProducts.Clear();
		List<CreatureCardShopRandomProduct> selectedCreaturCardShopRandomProducts = Resource.instance.SelectCreatureCardShopRandomProducts();
		foreach (CreatureCardShopRandomProduct product in selectedCreaturCardShopRandomProducts)
		{
			HeroCreatureCardShopRandomProduct heroProduct = new HeroCreatureCardShopRandomProduct(this, product);
			AddCreatureCardShopRandomProduct(heroProduct);
		}
	}

	public void RefreshDailyCreatureCardShopPaidRefreshCount(DateTime date)
	{
		if (!(date == m_dailyCreatureCardShopPaidRefreshCount.date))
		{
			m_dailyCreatureCardShopPaidRefreshCount.date = date;
			m_dailyCreatureCardShopPaidRefreshCount.value = 0;
		}
	}

	public void AddCreatureCardFamePoint(int nAmount, DateTimeOffset currentTime)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nCreatureCardCollectionFamePoint += nAmount;
			m_creatureCardCollectionFamePointUpdateTime = currentTime;
		}
	}

	public void IncreaseEliteMonsterKillCount(EliteMonster eliteMonster)
	{
		if (eliteMonster == null)
		{
			throw new ArgumentNullException("eliteMonster");
		}
		HeroEliteMonsterKill heroEliteMonsterKill = GetOrCreateHeroEliteMonsterkill(eliteMonster);
		heroEliteMonsterKill.killCount++;
		if (eliteMonster.ContainsKillAttrValue(heroEliteMonsterKill.killCount))
		{
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroEliteMonsterSkill(m_id, heroEliteMonsterKill.eliteMonster.id, heroEliteMonsterKill.killCount));
		dbWork.Schedule();
		ServerEvent.SendEliteMonsterKillCountUpdated(m_account.peer, heroEliteMonsterKill.eliteMonster.id, heroEliteMonsterKill.killCount, m_nRealMaxHP);
	}

	private void AddHeroEliteMonsterKill(HeroEliteMonsterKill heroEliteMonsterKill)
	{
		if (heroEliteMonsterKill == null)
		{
			throw new ArgumentNullException("heroEliteMonsterKill");
		}
		m_heroEliteMonsterKills.Add(heroEliteMonsterKill.eliteMonster.id, heroEliteMonsterKill);
	}

	private HeroEliteMonsterKill GetHeroEliteMonsterKill(int nEliteMonsterId)
	{
		if (!m_heroEliteMonsterKills.TryGetValue(nEliteMonsterId, out var value))
		{
			return null;
		}
		return value;
	}

	private HeroEliteMonsterKill GetOrCreateHeroEliteMonsterkill(EliteMonster eliteMonster)
	{
		HeroEliteMonsterKill heroEliteMonsterKill = GetHeroEliteMonsterKill(eliteMonster.id);
		if (heroEliteMonsterKill == null)
		{
			heroEliteMonsterKill = new HeroEliteMonsterKill(this);
			heroEliteMonsterKill.Init(eliteMonster);
			AddHeroEliteMonsterKill(heroEliteMonsterKill);
		}
		return heroEliteMonsterKill;
	}

	public List<PDHeroEliteMonsterKill> GetPDHeroEliteMonsterKills()
	{
		List<PDHeroEliteMonsterKill> results = new List<PDHeroEliteMonsterKill>();
		foreach (HeroEliteMonsterKill heroEliteMonsterKill in m_heroEliteMonsterKills.Values)
		{
			results.Add(heroEliteMonsterKill.ToPDHeroEliteMonsterKill());
		}
		return results;
	}

	public void RefreshDailyEliteDungeonPlayCount(DateTime date)
	{
		if (!(m_dailyEliteDungeonPlayCount.date == date))
		{
			m_dailyEliteDungeonPlayCount.date = date;
			m_dailyEliteDungeonPlayCount.value = 0;
		}
	}

	public int GetEliteDungeonAvailableEnterCount(DateTimeOffset time)
	{
		RefreshDailyEliteDungeonPlayCount(time.Date);
		return Math.Max(GetEliteDungeonEnterCount(time) - m_dailyEliteDungeonPlayCount.value, 0);
	}

	private int GetEliteDungeonEnterCount(DateTimeOffset time)
	{
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		EliteDungeon eliteDungeon = Resource.instance.eliteDungeon;
		int nQuotient = (int)Math.Floor(fTime / (float)eliteDungeon.enterCountAddInterval);
		return eliteDungeon.baseEnterCount + nQuotient;
	}

	public void SetBattleSetting(LootingItemMinGrade lootingItemMinGrade)
	{
		m_lootingItemMinGrade = lootingItemMinGrade;
	}

	public bool IsAvailableLootItemGrade(int nGrade)
	{
		return (int)m_lootingItemMinGrade <= nGrade;
	}

	public void RefreshProofOfValorAutoRefreshSchedule(DateTime refreshDate, int nScheduleId, bool bSendEvent, DateTimeOffset time)
	{
		if (m_heroProofOfValorInst == null || m_heroProofOfValorInst.status == 0)
		{
			RefreshHeroProofOfValorInstance(time);
			DateTime date = (m_proofOfValorAutoRefreshDate = time.Date);
			m_nProofOfValorAutoRefreshScheduleId = nScheduleId;
			m_nProofOfValorPaidRefreshCount = 0;
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroProofOfValorInstance(m_heroProofOfValorInst.id, m_heroProofOfValorInst.hero.id, m_heroProofOfValorInst.bossMonsterArrange.id, m_heroProofOfValorInst.creatureCard.id, 0, m_nLevel, 0, time));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ProofOfValorAutoRefresh(m_id, m_proofOfValorAutoRefreshDate, m_nProofOfValorAutoRefreshScheduleId));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ProofOfValorPaidRefreshCount(m_id, m_nProofOfValorPaidRefreshCount));
			dbWork.Schedule();
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroProofOfValorRefreshLog(Guid.NewGuid(), m_id, 1, m_heroProofOfValorInst.id, 0, 0, time));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
			}
			if (bSendEvent)
			{
				ServerEvent.SendProofOfValorRefreshed(m_account.peer, m_heroProofOfValorInst.ToPDHeroProofOfValorInstance(), m_nProofOfValorPaidRefreshCount);
			}
		}
	}

	public void CreateHeroProofOfValorInstance(DateTimeOffset time, bool bIsRefreshPaidCount)
	{
		RefreshHeroProofOfValorInstance(time);
		if (bIsRefreshPaidCount)
		{
			m_nProofOfValorPaidRefreshCount = 0;
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroProofOfValorInstance(m_heroProofOfValorInst.id, m_heroProofOfValorInst.hero.id, m_heroProofOfValorInst.bossMonsterArrange.id, m_heroProofOfValorInst.creatureCard.id, 0, m_heroProofOfValorInst.level, 0, time));
		if (bIsRefreshPaidCount)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ProofOfValorPaidRefreshCount(m_id, m_nProofOfValorPaidRefreshCount));
		}
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroProofOfValorRefreshLog(Guid.NewGuid(), m_id, 1, m_heroProofOfValorInst.id, 0, 0, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public void RefreshHeroProofOfValorInstance(DateTimeOffset time)
	{
		ProofOfValor proofOfValor = Resource.instance.proofOfValor;
		HeroProofOfValorInstance inst = new HeroProofOfValorInstance(this);
		inst.Init(proofOfValor.SelectBossMonsterArrange(), time);
		inst.level = m_nLevel;
		m_heroProofOfValorInst = inst;
	}

	public void RefreshDailyProofOfValorPlayCount(DateTime date)
	{
		if (!(m_dailyProofOfValorPlayCount.date == date))
		{
			m_dailyProofOfValorPlayCount.date = date;
			m_dailyProofOfValorPlayCount.value = 0;
		}
	}

	public int GetProofOfValorAvailableEnterCount(DateTime date)
	{
		RefreshDailyProofOfValorPlayCount(date);
		return Math.Max(m_vipLevel.proofOfValorEnterCount - m_dailyProofOfValorPlayCount.value, 0);
	}

	public void RefreshDailyProofOfValorFreeRefreshCount(DateTime date)
	{
		if (!(m_dailyProofOfValorFreeRefreshCount.date == date))
		{
			m_dailyProofOfValorFreeRefreshCount.date = date;
			m_dailyProofOfValorFreeRefreshCount.value = 0;
		}
	}

	public void RefreshDailyProofOfValorPaidRefreshCount(DateTime date)
	{
		if (!(m_dailyProofOfvalorPaidRefreshCount.date == date))
		{
			m_dailyProofOfvalorPaidRefreshCount.date = date;
			m_dailyProofOfvalorPaidRefreshCount.value = 0;
		}
	}

	public void AcquireProofOfValorBuffBox(ProofOfValorBuffBoxInstance buffBoxInst, DateTimeOffset time)
	{
		if (buffBoxInst == null)
		{
			throw new ArgumentNullException("buffBoxInst");
		}
		if (m_proofOfValorBuff != null)
		{
			StopProofOfValorBuff();
		}
		ProofOfValorBuffBox buffBox = buffBoxInst.buffBox;
		float fHpRecevoeryFactor = buffBox.hpRecoveryFactor;
		if (fHpRecevoeryFactor > 0f)
		{
			RestoreHP((int)Math.Floor((float)m_nRealMaxHP * fHpRecevoeryFactor), bSendEventToMyself: false, bSendEventToOthers: false);
		}
		if (buffBox.offenseFactor > 0f || buffBox.physicalDefenseFactor > 0f)
		{
			HeroProofOfValorBuff buff = new HeroProofOfValorBuff();
			buff.Init(this, buffBox, time);
			m_proofOfValorBuff = buff;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		ProofOfValorInstance proofOfValorInst = (ProofOfValorInstance)m_currentPlace;
		proofOfValorInst.RemoveAllBuffBox();
	}

	public void ProcessProofOfValorBuffFinished()
	{
		if (m_proofOfValorBuff != null)
		{
			m_proofOfValorBuff = null;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
			ServerEvent.SendProofOfValorBuffFinished(m_account.peer);
		}
	}

	public void StopProofOfValorBuff()
	{
		if (m_proofOfValorBuff != null)
		{
			m_proofOfValorBuff.Stop();
			m_proofOfValorBuff = null;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
	}

	public void TameMonster(MonsterInstance monsterInst)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		foreach (MonsterOwnSkill ownSkill in monsterInst.monster.ownSkills)
		{
			HeroTamingMonsterSkill tamingMonsterSkill = new HeroTamingMonsterSkill(this, ownSkill.skill);
			m_tamingMonsterSkills.Add(tamingMonsterSkill.skillId, tamingMonsterSkill);
		}
		m_nTamingMonsterId = monsterInst.monster.id;
		StopAcceleration();
	}

	public void StopMonsterTame()
	{
		if (isMonsterTaming)
		{
			m_nTamingMonsterId = 0;
			ClearTamingMonsterSkill();
			InitMoveStartTime(DateTimeUtil.currentTime);
		}
	}

	public HeroTamingMonsterSkill GetTamingMonsterSkill(int nSkillId)
	{
		if (!m_tamingMonsterSkills.TryGetValue(nSkillId, out var value))
		{
			return null;
		}
		return value;
	}

	private void ClearTamingMonsterSkill()
	{
		m_tamingMonsterSkills.Clear();
	}

	private void OnUpdate_ManageAccelerationRequiredMoveTime()
	{
		try
		{
			ManageAccelerationRequiredMoveTime();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void ManageAccelerationRequiredMoveTime()
	{
		if (m_currentPlace == null || !m_currentPlace.location.accelerationEnabled || !m_bMoving || m_bWalking || m_bAcceleration || m_bIsRiding || isRidingCart || isTransformMonster)
		{
			return;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		float fElapsedTime = 0f;
		if (m_currentPlace is ContinentInstance)
		{
			if (!m_bManualMoving)
			{
				return;
			}
			fElapsedTime = (float)(currentTime - m_manualMoveStartTime).TotalSeconds;
		}
		else
		{
			fElapsedTime = (float)(currentTime - m_moveStartTime).TotalSeconds;
		}
		if (!(fElapsedTime < (float)Resource.instance.accelerationRequiredMoveDuration))
		{
			m_bAcceleration = true;
			RefreshMoveSpeed();
			ServerEvent.SendAccelerationStarted(m_account.peer);
			ServerEvent.SendHeroAccelerationStarted(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
		}
	}

	public void StopAcceleration()
	{
		if (m_bAcceleration)
		{
			m_bAcceleration = false;
			RefreshMoveSpeed();
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroAccelerationEnded(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
			}
		}
	}

	public HeroJobCommonSkill GetJobCommonSkill(int nSkillId)
	{
		if (!m_jobCommonSkills.TryGetValue(nSkillId, out var value))
		{
			return null;
		}
		return value;
	}

	public void StartGroggyMonsterItemSteal(MonsterInstance inst)
	{
		m_stealGroggyMonsterInst = inst;
		int nStealDuration = Resource.instance.monsterStealDuration * 1000;
		m_stealTimer = new Timer(OnStealTimerTick);
		m_stealTimer.Change(nStealDuration, -1);
		ServerEvent.SendHeroGroggyMonsterItemStealStart(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
	}

	private void OnStealTimerTick(object state)
	{
		AddWork(new SFAction(StealGroggyMonsterItem), bGlobalLockRequired: false);
	}

	private void StealGroggyMonsterItem()
	{
		if (m_stealGroggyMonsterInst == null)
		{
			return;
		}
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		PDItemBooty booty = null;
		Monster stealGroggyMonster = m_stealGroggyMonsterInst.monster;
		int nStealSuccessRate = stealGroggyMonster.stealSuccessRate;
		if (Util.DrawLots(nStealSuccessRate))
		{
			ItemReward itemReward = stealGroggyMonster.stealItemReward;
			if (itemReward != null)
			{
				Item item = itemReward.item;
				int nCount = itemReward.count;
				bool bOwned = itemReward.owned;
				AddItem(item, bOwned, nCount, changedInventorySlots);
				booty = new PDItemBooty();
				booty.id = item.id;
				booty.count = nCount;
				booty.owned = bOwned;
			}
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			foreach (InventorySlot slot in changedInventorySlots)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
			}
			dbWork.Schedule();
		}
		m_currentPlace.RemoveMonster(m_stealGroggyMonsterInst, bSendEvent: true);
		m_stealGroggyMonsterInst = null;
		DisposeStealTimer();
		ServerEvent.SendGroggyMonsterItemStealFinished(m_account.peer, booty, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
		ServerEvent.SendHeroGroggyMonsterItemStealFinished(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
	}

	public void CancelGroggyMonsterItemSteal(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		if (m_stealGroggyMonsterInst != null)
		{
			m_stealGroggyMonsterInst = null;
			DisposeStealTimer();
			if (bSendEventToMyself)
			{
				ServerEvent.SendGroggyMonsterItemStealCancel(m_account.peer);
			}
			if (bSendEventToOthers && m_currentPlace != null)
			{
				ServerEvent.SendHeroGroggyMonsterItemStealCancel(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
			}
		}
	}

	private void DisposeStealTimer()
	{
		if (m_stealTimer != null)
		{
			m_stealTimer.Dispose();
			m_stealTimer = null;
		}
	}

	public void AddNpcShopProduct(HeroNpcShopProduct product)
	{
		if (product == null)
		{
			throw new ArgumentNullException("product");
		}
		m_npcShopProducts.Add(product.product.id, product);
	}

	public HeroNpcShopProduct GetNpcShopProduct(int nId)
	{
		if (!m_npcShopProducts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void AddRankActiveSkill(HeroRankActiveSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_rankActiveSkills.Add(skill.skillId, skill);
	}

	public HeroRankActiveSkill GetRankActiveSkill(int nSkillId)
	{
		if (!m_rankActiveSkills.TryGetValue(nSkillId, out var value))
		{
			return null;
		}
		return value;
	}

	public float GetRankActiveSkillRemainingCoolTime(DateTimeOffset time)
	{
		if (m_selectedRankActiveSkill == null)
		{
			return 0f;
		}
		float fElapsedTime = DateTimeUtil.GetTimeSpanSeconds(m_rankActiveSkillCastingTime, time);
		return Math.Max(m_selectedRankActiveSkill.skill.coolTime * 0.9f - fElapsedTime, 0f);
	}

	public List<PDHeroRankActiveSkill> GetPDHeroRankActiveSkills()
	{
		List<PDHeroRankActiveSkill> results = new List<PDHeroRankActiveSkill>();
		foreach (HeroRankActiveSkill heroSkill in m_rankActiveSkills.Values)
		{
			results.Add(heroSkill.ToPDHeroRankActiveSkill());
		}
		return results;
	}

	private void AddRankPassiveSkill(HeroRankPassiveSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		m_rankPassiveSkills.Add(skill.skillId, skill);
	}

	public HeroRankPassiveSkill GetRankPassiveSkill(int nSkillId)
	{
		if (!m_rankPassiveSkills.TryGetValue(nSkillId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDHeroRankPassiveSkill> GetPDHeroRankPassiveSkills()
	{
		List<PDHeroRankPassiveSkill> results = new List<PDHeroRankPassiveSkill>();
		foreach (HeroRankPassiveSkill heroSkill in m_rankPassiveSkills.Values)
		{
			results.Add(heroSkill.ToPDHeroPassiveSkill());
		}
		return results;
	}

	public void AddSpiritStone(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nSpiritStone += nAmount;
		}
	}

	public void UseSpiritStone(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			if (nAmount > m_nSpiritStone)
			{
				throw new Exception("보유한 정령석이 부족합니다.");
			}
			m_nSpiritStone -= nAmount;
		}
	}

	public float GetCurrentRookieGiftLoginDuration(DateTimeOffset currentTime)
	{
		return m_fRookieGiftLoginDuration + (float)(currentTime - m_rookieGitfLoginStartTime).TotalSeconds;
	}

	public float GetCurrentRookieGiftRemainingTime(DateTimeOffset currentTime)
	{
		RookieGift rookieGift = Resource.instance.GetRookieGift(m_nRookieGiftNo);
		if (rookieGift == null)
		{
			return 0f;
		}
		return Math.Max((float)rookieGift.waitingTime - GetCurrentRookieGiftLoginDuration(currentTime), 0f);
	}

	public void AddReceivedOpenGiftReward(int nDay)
	{
		m_receivedOpenGiftRewards.Add(nDay);
	}

	public bool IsReceivedOpenGift(int nDay)
	{
		return m_receivedOpenGiftRewards.Contains(nDay);
	}

	public int GetElapsedDaysFromCreation(DateTimeOffset time)
	{
		return (int)(time.Date - m_regTime.Date).TotalDays + 1;
	}

	public void RefreshDailyQuestFreeRefreshCount(DateTime date)
	{
		if (!(m_dailyQuestFreeRefreshCount.date == date))
		{
			m_dailyQuestFreeRefreshCount.date = date;
			m_dailyQuestFreeRefreshCount.value = 0;
		}
	}

	public void RefreshDailyQuestAcceptionCount(DateTime date)
	{
		if (!(m_dailyQuestAcceptionCount.date == date))
		{
			m_dailyQuestAcceptionCount.date = date;
			m_dailyQuestAcceptionCount.value = 0;
		}
	}

	public void InitDailyQuests(bool bSendEvent)
	{
		if (m_nLevel < Resource.instance.dailyQuest.requiredHeroLevel)
		{
			return;
		}
		List<HeroDailyQuest> m_addedQuests = new List<HeroDailyQuest>();
		for (int nSlotIndex = 0; nSlotIndex < m_dailyQuests.Length; nSlotIndex++)
		{
			if (m_dailyQuests[nSlotIndex] == null)
			{
				HeroDailyQuest newQuest = CreateHeroDailyQuest(nSlotIndex);
				if (newQuest != null)
				{
					m_dailyQuests[nSlotIndex] = newQuest;
					m_addedQuests.Add(newQuest);
				}
			}
		}
		if (m_addedQuests.Count == 0)
		{
			return;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (bSendEvent)
		{
			ServerEvent.SendHeroDailyQuestCreated(m_account.peer, HeroDailyQuest.ToPDHeroDailyQuests(m_addedQuests, currentTime).ToArray());
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (HeroDailyQuest quest2 in m_addedQuests)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroDailyQuest(quest2.id, quest2.hero.id, quest2.slotIndex, quest2.mission.id, currentTime));
		}
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			foreach (HeroDailyQuest quest in m_addedQuests)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroDailyQuestCreationLog(quest.id, quest.hero.id, quest.slotIndex, quest.mission.id, currentTime));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public HeroDailyQuest CreateHeroDailyQuest(int nSlotIndex)
	{
		List<int> exclusiveMissionIds = new List<int>();
		HeroDailyQuest[] array = m_dailyQuests;
		foreach (HeroDailyQuest heroQuest in array)
		{
			if (heroQuest != null)
			{
				exclusiveMissionIds.Add(heroQuest.mission.id);
			}
		}
		DailyQuestMissionPool missionPool = Resource.instance.dailyQuest.GetMissionPool(m_nLevel);
		DailyQuestMission selectedMission = missionPool.SelectMission(exclusiveMissionIds);
		if (selectedMission == null)
		{
			return null;
		}
		return new HeroDailyQuest(this, nSlotIndex, selectedMission);
	}

	public void SetDailyQuest(HeroDailyQuest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_dailyQuests[quest.slotIndex] = quest;
	}

	public HeroDailyQuest GetDailyQuest(Guid id)
	{
		HeroDailyQuest[] array = m_dailyQuests;
		foreach (HeroDailyQuest quest in array)
		{
			if (quest != null && id == quest.id)
			{
				return quest;
			}
		}
		return null;
	}

	public List<PDHeroDailyQuest> GetPDHeroDailyQuests(DateTimeOffset currentTime)
	{
		List<PDHeroDailyQuest> insts = new List<PDHeroDailyQuest>();
		HeroDailyQuest[] array = m_dailyQuests;
		foreach (HeroDailyQuest quest in array)
		{
			if (quest != null)
			{
				insts.Add(quest.ToPDHeroDailyQuest(currentTime));
			}
		}
		return insts;
	}

	public bool IsAvailableRefreshDailyQuest()
	{
		HeroDailyQuest[] array = m_dailyQuests;
		foreach (HeroDailyQuest quest in array)
		{
			if (quest == null || quest.isCreated)
			{
				return true;
			}
		}
		return false;
	}

	public void ProcessDailyQuestForHunt(MonsterInstance monsterInst, DateTimeOffset currentTime)
	{
		if (base.isDead)
		{
			return;
		}
		List<PDHeroDailyQuestProgressCount> progressCounts = new List<PDHeroDailyQuestProgressCount>();
		HeroDailyQuest[] array = m_dailyQuests;
		foreach (HeroDailyQuest quest in array)
		{
			if (quest != null && !quest.IsMissionCompleted(currentTime))
			{
				DailyQuestMission mission = quest.mission;
				if (mission.type == DailyQuestMissionType.Hunt && mission.targetMonster.id == monsterInst.monster.id && quest.IncreaseProgressCountByMonsterQuest(monsterInst.instanceId))
				{
					progressCounts.Add(quest.ToPDHeroDailyQuestProgressCount());
				}
			}
		}
		if (progressCounts.Count != 0)
		{
			ServerEvent.SendHeroDailyQuestProgressCountUpdated(m_account.peer, progressCounts.ToArray());
		}
	}

	public bool IsAvailableDailyQuestInteractionObject(int nObjectId, DateTimeOffset currentTime)
	{
		HeroDailyQuest[] array = m_dailyQuests;
		foreach (HeroDailyQuest quest in array)
		{
			if (quest != null && !quest.IsMissionCompleted(currentTime))
			{
				DailyQuestMission mission = quest.mission;
				if (mission.type == DailyQuestMissionType.Interaction && mission.targetContinentObject.id == nObjectId)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void ProcessDailyQuestForInteraction(ContinentObject continentObject, DateTimeOffset currentTime)
	{
		if (base.isDead)
		{
			return;
		}
		List<PDHeroDailyQuestProgressCount> progressCounts = new List<PDHeroDailyQuestProgressCount>();
		HeroDailyQuest[] array = m_dailyQuests;
		foreach (HeroDailyQuest quest in array)
		{
			if (quest != null && !quest.IsMissionCompleted(currentTime))
			{
				DailyQuestMission mission = quest.mission;
				if (mission.type == DailyQuestMissionType.Interaction && mission.targetContinentObject.id == continentObject.id)
				{
					quest.IncreaseProgressCount();
					progressCounts.Add(quest.ToPDHeroDailyQuestProgressCount());
				}
			}
		}
		if (progressCounts.Count != 0)
		{
			ServerEvent.SendHeroDailyQuestProgressCountUpdated(m_account.peer, progressCounts.ToArray());
		}
	}

	public void InitWeeklyQuest(DateTimeOffset currentTime, bool bSendEvent)
	{
		WeeklyQuest weeklyQuest = Resource.instance.weeklyQuest;
		if (m_nLevel < weeklyQuest.requiredHeroLevel)
		{
			return;
		}
		DateTime dateOfMonday = DateTimeUtil.GetWeekStartDate(currentTime);
		if (m_weeklyQuest == null || m_weeklyQuest.weekStartDate != dateOfMonday)
		{
			m_weeklyQuest = new HeroWeeklyQuest(this, dateOfMonday);
		}
		if (m_weeklyQuest.isCompleted || m_weeklyQuest.isRoundValid)
		{
			return;
		}
		if (m_weeklyQuest.roundNo == 0)
		{
			m_weeklyQuest.SetNextRound();
		}
		else
		{
			m_weeklyQuest.RefreshCurrentRound();
		}
		if (bSendEvent)
		{
			ServerEvent.SendWeeklyQuestCreated(m_account.peer, m_weeklyQuest.ToPDHeroWeeklyQuest());
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroWeeklyQuest(m_weeklyQuest));
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWeeklyQuestRoundCreationLog(m_weeklyQuest.roundId, m_weeklyQuest.hero.id, m_weeklyQuest.weekStartDate, m_weeklyQuest.roundNo, m_weeklyQuest.roundMissionId, currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public void ProcessWeeklyQuestRoundForHunt(MonsterInstance monstInst, DateTimeOffset currentTime)
	{
		if (base.isDead)
		{
			return;
		}
		HeroWeeklyQuest heroWeeklyQuest = m_weeklyQuest;
		if (heroWeeklyQuest != null && !heroWeeklyQuest.isCompleted && heroWeeklyQuest.roundMission != null && !heroWeeklyQuest.isRoundMissionCompleted)
		{
			WeeklyQuestMission weeklyQuestMission = m_weeklyQuest.roundMission;
			if (weeklyQuestMission.type == WeeklyQuestMissionType.Hunt && weeklyQuestMission.targetMonster.id == monstInst.monster.id)
			{
				m_weeklyQuest.IncreaseProgressCountByHuntMission(monstInst.instanceId, currentTime);
			}
		}
	}

	public bool IsAvailableWeeklyQuestRoundInteractionObject(int nObjectId)
	{
		HeroWeeklyQuest heroWeeklyQuest = m_weeklyQuest;
		if (heroWeeklyQuest == null)
		{
			return false;
		}
		if (heroWeeklyQuest.isCompleted)
		{
			return false;
		}
		if (heroWeeklyQuest.roundMission == null)
		{
			return false;
		}
		if (heroWeeklyQuest.isRoundMissionCompleted)
		{
			return false;
		}
		WeeklyQuestMission weeklyQuestMission = m_weeklyQuest.roundMission;
		if (weeklyQuestMission.type != WeeklyQuestMissionType.Interaction)
		{
			return false;
		}
		if (weeklyQuestMission.targetContinentObject.id != nObjectId)
		{
			return false;
		}
		return true;
	}

	private void ProcessWeeklyQuestRoundForInteraction(ContinentObject continentObject, DateTimeOffset currentTime)
	{
		if (base.isDead)
		{
			return;
		}
		HeroWeeklyQuest heroWeeklyQuest = m_weeklyQuest;
		if (heroWeeklyQuest != null && !heroWeeklyQuest.isCompleted && heroWeeklyQuest.roundMission != null && !heroWeeklyQuest.isRoundMissionCompleted)
		{
			WeeklyQuestMission weeklyQuestMission = m_weeklyQuest.roundMission;
			if (weeklyQuestMission.type == WeeklyQuestMissionType.Interaction && weeklyQuestMission.targetContinentObject.id == continentObject.id)
			{
				m_weeklyQuest.IncreaseProgressCount(currentTime);
			}
		}
	}

	public void RefreshDailyWisdomTemplePlayCount(DateTime date)
	{
		if (!(m_dailyWisdomTemplePlayCount.date == date))
		{
			m_dailyWisdomTemplePlayCount.date = date;
			m_dailyWisdomTemplePlayCount.value = 0;
		}
	}

	public int GetWisdomTempleAvailableEnterCount(DateTime date)
	{
		RefreshDailyWisdomTemplePlayCount(date);
		return Math.Max(1 - m_dailyWisdomTemplePlayCount.value, 0);
	}

	public void StartWisdomTempleObjectInteraction(WisdomTempleObjectInstance wisdomTempleObjectInst)
	{
		if (wisdomTempleObjectInst == null)
		{
			throw new ArgumentNullException("wisdomTempleObjectInst");
		}
		m_currentInteractionWisdomTempleObjectInst = wisdomTempleObjectInst;
		m_currentInteractionWisdomTempleObjectInst.interactionHero = this;
		int nDuration = (int)wisdomTempleObjectInst.interactionDuration * 1000;
		m_wisdomTempleObjectInteractionTimer = new Timer(OnWisdomTempleObjectInteractionTimerTick, wisdomTempleObjectInst, -1, -1);
		m_wisdomTempleObjectInteractionTimer.Change(nDuration, -1);
	}

	private void OnWisdomTempleObjectInteractionTimerTick(object state)
	{
		AddWork(new SFAction<WisdomTempleObjectInstance>(ProcessWisdomTempleObjectInteractionTimerTick, (WisdomTempleObjectInstance)state), bGlobalLockRequired: false);
	}

	private void ProcessWisdomTempleObjectInteractionTimerTick(WisdomTempleObjectInstance wisdomTempleObjectInst)
	{
		DisposeWisdomTempleObjectInteractionTimer();
		if (m_currentInteractionWisdomTempleObjectInst == wisdomTempleObjectInst && !m_currentInteractionWisdomTempleObjectInst.isReleased)
		{
			WisdomTempleInstance wisdomTempleInst = (WisdomTempleInstance)m_currentPlace;
			if (m_currentInteractionWisdomTempleObjectInst.type == 1)
			{
				wisdomTempleInst.OnColorMatchingObjectInteractionComplete((WisdomTempleColorMatchingObjectInstance)m_currentInteractionWisdomTempleObjectInst);
			}
			else
			{
				wisdomTempleInst.OnPuzzleRewardObjectInteractionComplete((WisdomTemplePuzzleRewardObjectInstance)m_currentInteractionWisdomTempleObjectInst);
			}
			m_currentInteractionWisdomTempleObjectInst = null;
		}
	}

	public void CancelWisdomTempleObjectInteraction(bool bSendEvent)
	{
		if (m_currentInteractionWisdomTempleObjectInst == null)
		{
			return;
		}
		DisposeWisdomTempleObjectInteractionTimer();
		if (bSendEvent)
		{
			if (m_currentInteractionWisdomTempleObjectInst.type == 1)
			{
				ServerEvent.SendWisdomTempleColorMatchingObjectInteractionCancel(m_account.peer);
			}
			else
			{
				ServerEvent.SendWisdomTemplePuzzleRewardObjectInteractionCancel(m_account.peer);
			}
		}
		m_currentInteractionWisdomTempleObjectInst.interactionHero = null;
		m_currentInteractionWisdomTempleObjectInst = null;
	}

	private void DisposeWisdomTempleObjectInteractionTimer()
	{
		if (m_wisdomTempleObjectInteractionTimer != null)
		{
			m_wisdomTempleObjectInteractionTimer.Dispose();
			m_wisdomTempleObjectInteractionTimer = null;
		}
	}

	public void AddRewardedOpen7DayEventMission(int nMissionId)
	{
		m_rewardedOpen7DayEventMissions.Add(nMissionId);
	}

	public bool IsRewardedOpen7DayEventMission(int nMissionId)
	{
		return m_rewardedOpen7DayEventMissions.Contains(nMissionId);
	}

	public void AddPurchasedOpen7DayEventProducts(int nProductId)
	{
		m_purchasedOpen7DayEventProducts.Add(nProductId);
	}

	public bool IsPurchasedOpen7DayEventProduct(int nProductId)
	{
		return m_purchasedOpen7DayEventProducts.Contains(nProductId);
	}

	public void AddOpen7DayEventProgressCount(HeroOpen7DayEventProgressCount progressCount)
	{
		if (progressCount == null)
		{
			throw new ArgumentNullException("progressCount");
		}
		m_open7DayEventProgressCounts.Add(progressCount.type, progressCount);
	}

	public HeroOpen7DayEventProgressCount GetOpen7DayEventProgressCount(int nType)
	{
		if (!m_open7DayEventProgressCounts.TryGetValue(nType, out var value))
		{
			return null;
		}
		return value;
	}

	public void IncreaseOpen7DayEventProgressCount(int nType)
	{
		HeroOpen7DayEventProgressCount progressCount = GetOpen7DayEventProgressCount(nType);
		if (progressCount == null)
		{
			progressCount = new HeroOpen7DayEventProgressCount(this, nType);
			AddOpen7DayEventProgressCount(progressCount);
		}
		progressCount.IncreaseProgressCount();
	}

	public long GetOpen7DayEventMissionProgressValue(int nType)
	{
		long lnProgressValue = 0L;
		if (Open7DayEventMission.IsProgressCountAccumulationType(nType))
		{
			lnProgressValue = GetOpen7DayEventProgressCount(nType)?.accProgressCount ?? 0;
		}
		else
		{
			switch (nType)
			{
			case 1:
				lnProgressValue = m_nLevel;
				break;
			case 3:
				lnProgressValue = subGearsMaxLevel;
				break;
			case 6:
				lnProgressValue = totalMountedSoulstoneLevel;
				break;
			case 7:
				lnProgressValue = m_lnBattlePower;
				break;
			case 10:
				lnProgressValue = equippedMainGearsMaxLevel;
				break;
			case 12:
				lnProgressValue = rankNo;
				break;
			}
		}
		return lnProgressValue;
	}

	public void AddRetrieval(HeroRetrieval retrieval)
	{
		if (retrieval == null)
		{
			throw new ArgumentNullException("retrieval");
		}
		m_retrievals.Add(retrieval.id, retrieval);
	}

	public HeroRetrieval GetRetrieval(int nId)
	{
		if (!m_retrievals.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RefreshRetrievals(DateTime date)
	{
		if (!(m_retrievalDate == date))
		{
			m_retrievalDate = date;
			m_retrievals.Clear();
		}
	}

	public bool GetAvailableRetrieval(int nRetrievalId)
	{
		return nRetrievalId switch
		{
			1 => m_nLevel >= Resource.instance.expDungeon.GetDifficulty(1).requiredHeroLevel, 
			2 => m_nLevel >= Resource.instance.fieldOfHonor.requiredHeroLevel, 
			3 => m_nLevel >= Resource.instance.fearAltar.requiredHeroLevel, 
			4 => m_nLevel >= Resource.instance.bountyQuestRequiredHeroLevel, 
			5 => m_nLevel >= Resource.instance.dimensionRaidQuest.requiredHeroLevel, 
			6 => m_nLevel >= Resource.instance.supplySupportQuest.requiredHeroLevel, 
			7 => m_nLevel >= Resource.instance.fishingQuest.requiredHeroLevel, 
			8 => m_nLevel >= Resource.instance.mysteryBoxQuest.requiredHeroLevel, 
			9 => m_nLevel >= Resource.instance.secretLetterQuest.requiredHeroLevel, 
			10 => m_nLevel >= Resource.instance.treatOfFarmQuest.requiredHeroLevel, 
			11 => m_nLevel >= Resource.instance.ancientRelic.requiredHeroLevel, 
			12 => m_nLevel >= Resource.instance.wisdomTemple.requiredHeroLevel, 
			13 => m_nLevel >= Resource.instance.dailyQuest.requiredHeroLevel, 
			_ => false, 
		};
	}

	public int GetRetrievalTargetCount(int nRetrievalId)
	{
		int nTargetCount = 0;
		switch (nRetrievalId)
		{
		case 1:
			nTargetCount = m_vipLevel.expDungeonEnterCount;
			break;
		case 2:
			nTargetCount = m_vipLevel.fieldOfHonorEnterCount;
			break;
		case 3:
			nTargetCount = m_vipLevel.fieldOfHonorEnterCount;
			break;
		case 4:
			nTargetCount = Resource.instance.bountyHunterMaxCount;
			break;
		case 5:
			nTargetCount = Resource.instance.dimensionRaidQuest.limitCount;
			break;
		case 6:
			nTargetCount = Resource.instance.supplySupportQuest.limitCount;
			break;
		case 7:
			nTargetCount = Resource.instance.fishingQuest.limitCount;
			break;
		case 8:
			nTargetCount = Resource.instance.mysteryBoxQuest.limitCount;
			break;
		case 9:
			nTargetCount = Resource.instance.secretLetterQuest.limitCount;
			break;
		case 10:
			nTargetCount = Resource.instance.treatOfFarmQuest.limitCount;
			break;
		case 11:
			nTargetCount = m_vipLevel.ancientRelicEnterCount;
			break;
		case 12:
			nTargetCount = 1;
			break;
		case 13:
			nTargetCount = Resource.instance.dailyQuest.playCount;
			break;
		}
		return nTargetCount;
	}

	public void AddRetrievalProgressCountCollection(HeroRetrievalProgressCountCollection collection)
	{
		if (collection == null)
		{
			throw new ArgumentNullException("collection");
		}
		m_retrievalProgressCountCollections.Add(collection.date, collection);
	}

	public HeroRetrievalProgressCountCollection GetRetrivalProgressCountCollection(DateTime date)
	{
		if (!m_retrievalProgressCountCollections.TryGetValue(date, out var value))
		{
			return null;
		}
		return value;
	}

	public HeroRetrievalProgressCountCollection GetOrCreateRetrivalProgressCountCollection(DateTime date)
	{
		HeroRetrievalProgressCountCollection collection = GetRetrivalProgressCountCollection(date);
		if (collection == null)
		{
			collection = new HeroRetrievalProgressCountCollection(this, date);
			AddRetrievalProgressCountCollection(collection);
		}
		return collection;
	}

	public void ProcessRetrievalProgressCount(int nId, DateTime date)
	{
		HeroRetrievalProgressCountCollection retrievalProgressCountCollection = GetOrCreateRetrivalProgressCountCollection(date);
		retrievalProgressCountCollection.ProcessProgressCount(nId);
	}

	public List<PDHeroRetrievalProgressCount> GetPDHeroRetreivalProgressCounts()
	{
		List<PDHeroRetrievalProgressCount> insts = new List<PDHeroRetrievalProgressCount>();
		foreach (HeroRetrievalProgressCountCollection collection in m_retrievalProgressCountCollections.Values)
		{
			insts.AddRange(HeroRetrievalProgressCount.ToPDHeroRetrievalProrgressCounts(collection.progressCounts.Values));
		}
		return insts;
	}

	public void RefreshDailyRuinsReclaimFreePlayCount(DateTime date)
	{
		if (!(m_dailyRuinsReclaimFreePlayCount.date == date))
		{
			m_dailyRuinsReclaimFreePlayCount.date = date;
			m_dailyRuinsReclaimFreePlayCount.value = 0;
		}
	}

	public int GetRuinsReclaimAvailableFreeEnterCount(DateTime date)
	{
		RefreshDailyRuinsReclaimFreePlayCount(date);
		return Math.Max(Resource.instance.ruinsReclaim.freeEnterCount - m_dailyRuinsReclaimFreePlayCount.value, 0);
	}

	public void HitRuinsReclaimTrap(int nDamage, DateTimeOffset time)
	{
		if (!hitEnabled || nDamage <= 0)
		{
			return;
		}
		float fElapsedTime = DateTimeUtil.GetTimeSpanSeconds(m_ruinsReclaimTrapLastHitTime, time);
		if (fElapsedTime < 1f)
		{
			return;
		}
		m_ruinsReclaimTrapLastHitTime = time;
		HashSet<long> removedAbnormalStateEffects = new HashSet<long>();
		List<AbnormalStateEffect> absorbShieldEffects = new List<AbnormalStateEffect>();
		AbnormalStateEffect immortalityEffect = null;
		foreach (AbnormalStateEffect effect in m_abnormalStateEffects.Values)
		{
			switch (effect.abnormalState.id)
			{
			case 8:
				nDamage = (int)Math.Floor((float)nDamage * ((float)effect.abnormalStateLevel.value1 / 10000f));
				break;
			case 10:
			case 102:
			case 109:
				absorbShieldEffects.Add(effect);
				break;
			case 106:
				if (immortalityEffect == null)
				{
					immortalityEffect = effect;
				}
				break;
			}
		}
		List<PDAbnormalStateEffectDamageAbsorbShield> changedAbnormalStateEffectDamageAbsorbShields = new List<PDAbnormalStateEffectDamageAbsorbShield>();
		absorbShieldEffects.Sort(AbnormalStateEffect.Compare);
		foreach (AbnormalStateEffect absorbShieldEffect in absorbShieldEffects)
		{
			if (nDamage > 0)
			{
				nDamage = absorbShieldEffect.AbsorbDamage(nDamage);
				int nRemainingDamageAbsorbShield = absorbShieldEffect.damageAbsorbShieldRemainingAmount;
				if (nRemainingDamageAbsorbShield <= 0 && (absorbShieldEffect.abnormalState.id == 102 || absorbShieldEffect.abnormalState.id == 10))
				{
					absorbShieldEffect.Stop();
					RemoveAbnormalStateEffect(absorbShieldEffect.id);
					removedAbnormalStateEffects.Add(absorbShieldEffect.id);
				}
				PDAbnormalStateEffectDamageAbsorbShield shield = new PDAbnormalStateEffectDamageAbsorbShield();
				shield.abnormalStateEffectInstanceId = absorbShieldEffect.id;
				shield.remainingAbsorbShieldAmount = nRemainingDamageAbsorbShield;
				changedAbnormalStateEffectDamageAbsorbShields.Add(shield);
				continue;
			}
			break;
		}
		bool bIsImmortalityEffect = false;
		if (nDamage >= m_nHP && immortalityEffect != null)
		{
			bIsImmortalityEffect = immortalityEffect.UseImmortalityEffect();
		}
		Damage(null, nDamage, time, bIsImmortalityEffect, removedAbnormalStateEffects);
		if (base.isDead)
		{
			OnDead();
			m_currentPlace.OnUnitDead(this);
		}
		ServerEvent.SendRuinsReclaimTrapHit(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_id, bIsImmortalityEffect, m_nHP, m_nLastDamage, m_nLastHPDamage, changedAbnormalStateEffectDamageAbsorbShields.ToArray(), removedAbnormalStateEffects.ToArray());
	}

	public void TransformRuinsReclaimMonster(RuinsReclaimStepWaveSkill skill)
	{
		if (skill == null)
		{
			throw new ArgumentNullException("skill");
		}
		if (base.isDead)
		{
			return;
		}
		if (m_ruinsReclaimTransformationMonsterEffect != null)
		{
			m_ruinsReclaimTransformationMonsterEffect.Stop();
		}
		HeroRuinsReclaimTransformationMonsterEffect effect = new HeroRuinsReclaimTransformationMonsterEffect();
		effect.Init(this, skill, DateTimeUtil.currentTime);
		m_ruinsReclaimTransformationMonsterEffect = effect;
		List<long> removedAbnormalStateEffects = new List<long>();
		foreach (AbnormalStateEffect abnormalStateeffect in m_abnormalStateEffects.Values)
		{
			removedAbnormalStateEffects.Add(abnormalStateeffect.id);
		}
		if (m_abnormalStateEffects.Count > 0)
		{
			ClearAllAbnormalStateEffects();
			RefreshRealValues(bSendMaxHpChangedToOthers: false);
		}
		StopAcceleration();
		RefreshMoveSpeed();
		int nTransformationMonsterId = effect.skill.transformationMonster.id;
		ServerEvent.SendRuinsReclaimMonsterTransformationStart(m_account.peer, nTransformationMonsterId, removedAbnormalStateEffects.ToArray());
		ServerEvent.SendHeroRuinsReclaimMonsterTransformationStart(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, nTransformationMonsterId, removedAbnormalStateEffects.ToArray());
	}

	public void ProcessRuinsReclaimTransformationMonsterEffectFinished()
	{
		if (m_ruinsReclaimTransformationMonsterEffect != null)
		{
			m_ruinsReclaimTransformationMonsterEffect = null;
			InitMoveStartTime(DateTimeUtil.currentTime);
			RefreshMoveSpeed();
			CancelRuinsReclaimObjectInteraction(bSendEventToMyself: true, bSendEventToOthers: true);
			ServerEvent.SendRuinsReclaimMonsterTransformationFinished(m_account.peer);
			ServerEvent.SendHeroRuinsReclaimMonsterTransformationFinished(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
		}
	}

	public void CancelRuinsReclaimMonsterTransformation()
	{
		if (m_ruinsReclaimTransformationMonsterEffect != null)
		{
			m_ruinsReclaimTransformationMonsterEffect.Stop();
			m_ruinsReclaimTransformationMonsterEffect = null;
			InitMoveStartTime(DateTimeUtil.currentTime);
			RefreshMoveSpeed();
		}
	}

	public void StartRuinsReclaimObjectInteraction(RuinsReclaimObjectInstance ruinsReclaimObjectInst)
	{
		if (ruinsReclaimObjectInst == null)
		{
			throw new ArgumentNullException("ruinsReclaimObjectInst");
		}
		m_currentInteractionRuinsReclaimObjectInst = ruinsReclaimObjectInst;
		m_currentInteractionRuinsReclaimObjectInst.interactionHero = this;
		if (m_currentInteractionRuinsReclaimObjectInst.type == 1)
		{
			ServerEvent.SendHeroRuinsReclaimRewardObjectInteractionStart(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, ruinsReclaimObjectInst.instanceId);
		}
		else
		{
			ServerEvent.SendHeroRuinsReclaimMonsterTransformationCancelObjectInteractionStart(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, ruinsReclaimObjectInst.instanceId);
		}
		int nDuration = (int)ruinsReclaimObjectInst.interactionDuration * 1000;
		m_ruinsReclaimObjectInteractionTimer = new Timer(OnRuinsReclaimObjectInteractionTimerTick, ruinsReclaimObjectInst, -1, -1);
		m_ruinsReclaimObjectInteractionTimer.Change(nDuration, -1);
	}

	private void OnRuinsReclaimObjectInteractionTimerTick(object state)
	{
		AddWork(new SFAction<RuinsReclaimObjectInstance>(ProcessRuinsReclaimObjectInteractionTimerTick, (RuinsReclaimObjectInstance)state), bGlobalLockRequired: false);
	}

	private void ProcessRuinsReclaimObjectInteractionTimerTick(RuinsReclaimObjectInstance ruinsReclaimObjectInst)
	{
		DisposeRuinsReclaimObjectInteractionTimer();
		if (m_currentInteractionRuinsReclaimObjectInst == ruinsReclaimObjectInst && !m_currentInteractionRuinsReclaimObjectInst.isReleased)
		{
			RuinsReclaimInstance ruinsReclaimInst = (RuinsReclaimInstance)m_currentPlace;
			if (m_currentInteractionRuinsReclaimObjectInst.type == 1)
			{
				ruinsReclaimInst.OnRewardObjectInteractionFinish((RuinsReclaimRewardObjectInstance)m_currentInteractionRuinsReclaimObjectInst);
			}
			else
			{
				ruinsReclaimInst.OnMonsterTransformationCancelObjectInteractionFinish((RuinsReclaimMonsterTransformationCancelObjectInstance)m_currentInteractionRuinsReclaimObjectInst);
			}
			m_currentInteractionRuinsReclaimObjectInst = null;
		}
	}

	public void CancelRuinsReclaimObjectInteraction(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		if (m_currentInteractionRuinsReclaimObjectInst == null)
		{
			return;
		}
		DisposeRuinsReclaimObjectInteractionTimer();
		if (bSendEventToMyself)
		{
			if (m_currentInteractionRuinsReclaimObjectInst.type == 1)
			{
				ServerEvent.SendRuinsReclaimRewardObjectInteractionCancel(m_account.peer);
			}
			else
			{
				ServerEvent.SendRuinsReclaimMonsterTransformationCancelObjectInteractionCancel(m_account.peer);
			}
		}
		if (bSendEventToOthers && m_currentPlace != null)
		{
			if (m_currentInteractionRuinsReclaimObjectInst.type == 1)
			{
				ServerEvent.SendHeroRuinsReclaimRewardObjectInteractionCancel(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_currentInteractionRuinsReclaimObjectInst.instanceId);
			}
			else
			{
				ServerEvent.SendHeroRuinsReclaimMonsterTransformationCancelObjectInteractionCancel(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_currentInteractionRuinsReclaimObjectInst.instanceId);
			}
		}
		m_currentInteractionRuinsReclaimObjectInst.interactionHero = null;
		m_currentInteractionRuinsReclaimObjectInst = null;
	}

	private void DisposeRuinsReclaimObjectInteractionTimer()
	{
		if (m_ruinsReclaimObjectInteractionTimer != null)
		{
			m_ruinsReclaimObjectInteractionTimer.Dispose();
			m_ruinsReclaimObjectInteractionTimer = null;
		}
	}

	public void StartRuinsReclaimDebuffEffect()
	{
		if (!m_bRuinsReclaimDebuffEffect)
		{
			m_bRuinsReclaimDebuffEffect = true;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
			ServerEvent.SendRuinsReclaimDebuffEffectStart(m_account.peer);
		}
	}

	public void StopRuinsReclaimDebuffEffect(bool bSendEvent)
	{
		if (m_bRuinsReclaimDebuffEffect)
		{
			m_bRuinsReclaimDebuffEffect = false;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
			if (bSendEvent)
			{
				ServerEvent.SendRuinsReclaimDebuffEffectStop(m_account.peer);
			}
		}
	}

	public void AddTaskConsignment(HeroTaskConsignment consignment)
	{
		if (consignment == null)
		{
			throw new ArgumentNullException("consignment");
		}
		m_taskConsignments.Add(consignment.instanceId, consignment);
	}

	public HeroTaskConsignment GetTaskConsignment(Guid instanceId)
	{
		if (!m_taskConsignments.TryGetValue(instanceId, out var value))
		{
			return null;
		}
		return value;
	}

	public void CompleteTaskConsignment(Guid instanceId)
	{
		m_taskConsignments.Remove(instanceId);
	}

	public void AddTaskConsignmentStartCount(HeroTaskConsignmentStartCount startCount)
	{
		if (startCount == null)
		{
			throw new ArgumentNullException("startCount");
		}
		m_taskConsignmentStartCounts.Add(startCount.consignmentId, startCount);
	}

	public HeroTaskConsignmentStartCount GetTaskConsignmentStartCount(int nConsignmentId)
	{
		if (!m_taskConsignmentStartCounts.TryGetValue(nConsignmentId, out var value))
		{
			return null;
		}
		return value;
	}

	public int GetTaskConsignmentStartCountValue(int nConsignmentId)
	{
		return GetTaskConsignmentStartCount(nConsignmentId)?.count ?? 0;
	}

	public void RefreshTaskConsignmentStartCounts(DateTime date)
	{
		if (!(m_taskConsignmentStartCountDate == date))
		{
			m_taskConsignmentStartCountDate = date;
			m_taskConsignmentStartCounts.Clear();
		}
	}

	public int GetRemainingTaskConsignmentStartCount(int nConsginmentId, DateTime date)
	{
		RefreshTaskConsignmentStartCounts(date);
		int nCurrentStartCount = GetTaskConsignmentStartCountValue(nConsginmentId);
		int nTargetStartCount = TaskConsignment.GetTargetStartCount(nConsginmentId);
		return nTargetStartCount - nCurrentStartCount;
	}

	public bool IsConsignedTask(int nConsignmentId)
	{
		foreach (HeroTaskConsignment heroConsignment in m_taskConsignments.Values)
		{
			if (heroConsignment.consignment.id == nConsignmentId)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsTaskConsignmentTargetTaskStarted(int nConsignmentId, DateTime date)
	{
		switch (nConsignmentId)
		{
		case 1:
			RefreshUndergroundMazeDailyPlayTime(date);
			return m_fUndergroundMazePlayTime > 0f;
		case 2:
			RefreshDailyCreatureFarmAcceptionCount(date);
			return m_dailyCreatureFarmAcceptionCount.value > 0;
		case 3:
			RefreshDailyGuildHuntingQuestCount(date);
			return m_dailyGuildHuntingQuest.value > 0;
		case 4:
			if (m_guildMissionQuest != null)
			{
				return m_guildMissionQuest.date == date;
			}
			return false;
		case 5:
			return false;
		default:
			return false;
		}
	}

	public void SetTrueHeroQuest(HeroTrueHeroQuest trueHeroQuest)
	{
		if (trueHeroQuest == null)
		{
			throw new ArgumentNullException("trueHeroQuest");
		}
		m_trueHeroQuest = trueHeroQuest;
	}

	public void StartTrueHeroQuestInteraction()
	{
		int nDuration = (int)(Resource.instance.trueHeroQuest.targetObjectInteractionDuration * 1000f);
		if (nDuration < 0)
		{
			nDuration = 0;
		}
		m_trueHeroQuestInteractionTimer = new Timer(OnTrueHeroQuestInteractionTimerTick);
		m_trueHeroQuestInteractionTimer.Change(nDuration, -1);
	}

	private void OnTrueHeroQuestInteractionTimerTick(object state)
	{
		AddWork(new SFAction(ProcessTrueHeroQuestInteractionTimerTick), bGlobalLockRequired: false);
	}

	private void ProcessTrueHeroQuestInteractionTimerTick()
	{
		if (m_trueHeroQuestInteractionTimer != null)
		{
			DisposeTrueHeroQuestInteractionTimer();
			if (isDistorting)
			{
				CancelDistortion(bSendEventToMyself: true);
			}
			m_trueHeroQuest.StartWaiting();
			ServerEvent.SendTrueHeroQuestStepInteractionFinished(m_account.peer);
			ContinentInstance currentPlace = m_currentPlace as ContinentInstance;
			ServerEvent.SendHeroTrueHeroQuestStepInteractionFinished(currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
			Global.instance.AddWork(new SFAction<int, int, Guid, string, int, Vector3>(SendTrueHeroQuestTaunted, currentPlace.nationId, m_nationInst.nationId, m_id, m_sName, currentPlace.continent.id, m_position));
		}
	}

	public void SendTrueHeroQuestTaunted(int nTargetNationId, int nHeroNationId, Guid heroId, string sHeroName, int nContinentId, Vector3 position)
	{
		NationInstance nationInst = Cache.instance.GetNationInstance(nTargetNationId);
		ServerEvent.SendTrueHeroQuestTaunted(nationInst.GetClientPeers(Guid.Empty), nHeroNationId, m_id, m_sName, nContinentId, m_position);
	}

	private void DisposeTrueHeroQuestInteractionTimer()
	{
		if (m_trueHeroQuestInteractionTimer != null)
		{
			m_trueHeroQuestInteractionTimer.Dispose();
			m_trueHeroQuestInteractionTimer = null;
		}
	}

	public void CancelTrueHeroQuestInteraction(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		if (m_trueHeroQuestInteractionTimer != null)
		{
			DisposeTrueHeroQuestInteractionTimer();
			if (bSendEventToMyself)
			{
				ServerEvent.SendTrueHeroQuestStepInteractionCanceled(m_account.peer);
			}
			if (bSendEventToOthers)
			{
				ServerEvent.SendHeroTrueHeroQuestStepInteractionCanceled(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
			}
		}
	}

	public void CancelTrueHeroQuestStepWaiting(bool bIsLogOut)
	{
		if (m_trueHeroQuest != null && m_trueHeroQuest.waitingTimer != null)
		{
			m_trueHeroQuest.CancelWaiting(!bIsLogOut);
		}
	}

	public void RefreshDailyInfiniteWarPlayCount(DateTime date)
	{
		if (!(m_dailyInfiniteWarPlayCount.date == date))
		{
			m_dailyInfiniteWarPlayCount.date = date;
			m_dailyInfiniteWarPlayCount.value = 0;
		}
	}

	public int GetInfiniteWarAvailableEnterCount(DateTime date)
	{
		RefreshDailyInfiniteWarPlayCount(date);
		return Math.Max(Resource.instance.infiniteWar.enterCount - m_dailyInfiniteWarPlayCount.value, 0);
	}

	public void AcquireInfiniteWarBuffBox(InfiniteWarBuffBoxInstance buffBoxInst, DateTimeOffset time)
	{
		if (buffBoxInst == null)
		{
			throw new ArgumentNullException("buffBoxInst");
		}
		InfiniteWarBuffBox buffBox = buffBoxInst.buffBox;
		if (m_infiniteWarBuff != null)
		{
			StopInfiniteWarBuff();
		}
		float fHpRecoveryFactor = buffBox.hpRecoveryFactor;
		if (fHpRecoveryFactor > 0f)
		{
			RestoreHP((int)Math.Floor((float)m_nRealMaxHP * fHpRecoveryFactor), bSendEventToMyself: false, bSendEventToOthers: false);
		}
		if (buffBox.offenseFactor > 0f || buffBox.defenseFactor > 0f)
		{
			HeroInfiniteWarBuff buff = new HeroInfiniteWarBuff();
			buff.Init(this, buffBox, time);
			m_infiniteWarBuff = buff;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		InfiniteWarInstance infiniteWarInst = (InfiniteWarInstance)m_currentPlace;
		infiniteWarInst.RemoveBuffBox(buffBoxInst);
		ServerEvent.SendHeroInfiniteWarBuffBoxAcquisition(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_nHP, buffBoxInst.instanceId);
	}

	public void ProcessInfiniteWarBuffFinished()
	{
		if (m_infiniteWarBuff != null)
		{
			m_infiniteWarBuff = null;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
			ServerEvent.SendInfiniteWarBuffFinished(m_account.peer);
		}
	}

	public void StopInfiniteWarBuff()
	{
		if (m_infiniteWarBuff != null)
		{
			m_infiniteWarBuff.Stop();
			m_infiniteWarBuff = null;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
	}

	public void ReceiveLimitationGiftReward(int nScheduleId)
	{
		m_rewardedLimitationGiftScheduleIds.Add(nScheduleId);
	}

	public bool IsRewardedLimitationGiftSchedule(int nScheduleId)
	{
		return m_rewardedLimitationGiftScheduleIds.Contains(nScheduleId);
	}

	public void RefreshLimitationGiftRewards(DateTime date)
	{
		if (!(m_limitationGiftDate == date))
		{
			m_limitationGiftDate = date;
			m_rewardedLimitationGiftScheduleIds.Clear();
		}
	}

	public void RefreshWeekendReward(DateTime date)
	{
		DateTime weekStartDate = DateTimeUtil.GetWeekStartDate(date, DayOfWeek.Tuesday);
		if (m_weekendReward == null || !(m_weekendReward.weekStartDate == weekStartDate))
		{
			m_weekendReward = new HeroWeekendReward(this, weekStartDate);
		}
	}

	public void CreateWarehouseSlots(int nSlotCount)
	{
		for (int nIndex = 0; nIndex < nSlotCount; nIndex++)
		{
			WarehouseSlot slot = new WarehouseSlot(this, m_warehouseSlots.Count);
			m_warehouseSlots.Add(slot);
		}
	}

	public WarehouseSlot GetWarehouseSlot(int nIndex)
	{
		if (nIndex < 0 || nIndex >= m_warehouseSlots.Count)
		{
			return null;
		}
		return m_warehouseSlots[nIndex];
	}

	public WarehouseSlot GetEmptyWarehouseSlot()
	{
		foreach (WarehouseSlot slot in m_warehouseSlots)
		{
			if (slot.isEmpty)
			{
				return slot;
			}
		}
		return null;
	}

	public List<PDWarehouseSlot> GetPlacedPDWarehouseSlots()
	{
		List<PDWarehouseSlot> results = new List<PDWarehouseSlot>();
		foreach (WarehouseSlot slot in m_warehouseSlots)
		{
			if (!slot.isEmpty)
			{
				results.Add(slot.ToPDWarehouseSlot());
			}
		}
		return results;
	}

	private void AddWarehouseItem(HeroWarehouseItem warehouseItem)
	{
		m_warehouseItems.Add(warehouseItem.item.id, warehouseItem);
	}

	private void RemoveWarehouseItem(int nItemId)
	{
		m_warehouseItems.Remove(nItemId);
	}

	private HeroWarehouseItem GetWarehouseItem(int nItemId)
	{
		if (!m_warehouseItems.TryGetValue(nItemId, out var value))
		{
			return null;
		}
		return value;
	}

	private HeroWarehouseItem GetOrCreateWarehouseItem(Item item)
	{
		HeroWarehouseItem warehouseItem = GetWarehouseItem(item.id);
		if (warehouseItem == null)
		{
			warehouseItem = new HeroWarehouseItem(this, item);
			AddWarehouseItem(warehouseItem);
		}
		return warehouseItem;
	}

	public int GetWarehouseAvailableSpace(Item item, bool bOwned)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		int nAvailableSpace = 0;
		HeroWarehouseItem warehouseItem = GetWarehouseItem(item.id);
		if (warehouseItem != null)
		{
			nAvailableSpace += warehouseItem.GetAvailableSpace(bOwned);
		}
		return nAvailableSpace + GetWarehouseAvailableSpaceOfEmptySlots(item);
	}

	public int GetWarehouseAvailableSpaceOfEmptySlots(Item item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		return emptyWarehouseSlotCount * item.type.maxCountPerInventorySlot;
	}

	public int AddWarehosueItem(Item item, bool bOwned, int nCount, ICollection<WarehouseSlot> changedWarehouseSlots)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}
		if (changedWarehouseSlots == null)
		{
			throw new ArgumentNullException("changedWarehouseSlots");
		}
		if (nCount < 0)
		{
			throw new ArgumentOutOfRangeException("nCount");
		}
		if (nCount == 0)
		{
			return 0;
		}
		HeroWarehouseItem warehouseItem = GetOrCreateWarehouseItem(item);
		int nRemainingCount = warehouseItem.AddItem(bOwned, nCount, changedWarehouseSlots);
		nRemainingCount = AddItemToEmptyWarehouseSlots(warehouseItem, bOwned, nRemainingCount, changedWarehouseSlots);
		if (warehouseItem.isEmpty)
		{
			RemoveWarehouseItem(warehouseItem.item.id);
		}
		return nRemainingCount;
	}

	private int AddItemToEmptyWarehouseSlots(HeroWarehouseItem warehouseItem, bool bOwned, int nCount, ICollection<WarehouseSlot> changedWarehouseSlots)
	{
		if (nCount == 0)
		{
			return 0;
		}
		int nRemainingCount = nCount;
		foreach (WarehouseSlot slot in m_warehouseSlots)
		{
			if (slot.isEmpty)
			{
				ItemWarehouseObject warehouseObject = new ItemWarehouseObject(warehouseItem, bOwned);
				slot.Place(warehouseObject);
				warehouseItem.AddObject(warehouseObject);
				int nAddCount = Math.Min(warehouseObject.availableSpace, nRemainingCount);
				warehouseObject.count += nAddCount;
				nRemainingCount -= nAddCount;
				changedWarehouseSlots.Add(warehouseObject.warehouseSlot);
				if (nRemainingCount <= 0)
				{
					return nRemainingCount;
				}
			}
		}
		return nRemainingCount;
	}

	public int UseWarehouseItemOnly(int nItemId, bool bOwned, int nCount, ICollection<WarehouseSlot> changedWarehouseSlots)
	{
		HeroWarehouseItem warehouseItem = GetWarehouseItem(nItemId);
		if (warehouseItem == null)
		{
			return 0;
		}
		int nUsedCount = warehouseItem.UseOnly(bOwned, nCount, changedWarehouseSlots);
		if (warehouseItem.isEmpty)
		{
			RemoveWarehouseItem(nItemId);
		}
		return nUsedCount;
	}

	public void UseWarehouseItem(int nSlotIndex, int nCount)
	{
		WarehouseSlot slot = GetWarehouseSlot(nSlotIndex);
		if (slot == null)
		{
			throw new Exception("창고슬롯이 존재하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		if (!(slot.obj is ItemWarehouseObject obj))
		{
			throw new Exception("해당 창고슬롯에 아이템창고객체가 존재하지 않습니다. nSlotIndex = " + nSlotIndex);
		}
		if (nCount < 0)
		{
			throw new ArgumentOutOfRangeException("nCount");
		}
		if (nCount == 0)
		{
			return;
		}
		if (nCount > obj.count)
		{
			throw new Exception("사용수량이 보유수량보다 큽니다.");
		}
		obj.count -= nCount;
		if (obj.isEmpty)
		{
			slot.Clear();
			HeroWarehouseItem heroWarehouseItem = obj.heroWarehouseItem;
			heroWarehouseItem.RemoveObject(nSlotIndex);
			if (heroWarehouseItem.isEmpty)
			{
				RemoveWarehouseItem(heroWarehouseItem.item.id);
			}
		}
	}

	public void AddDailyDiaShopProductBuyCount(HeroDiaShopProductBuyCount product)
	{
		if (product == null)
		{
			throw new ArgumentNullException("product");
		}
		m_dailyDiaShopProductBuyCounts.Add(product.productId, product);
	}

	public HeroDiaShopProductBuyCount GetDailyDiaShopProductBuyCount(int nProductId)
	{
		if (!m_dailyDiaShopProductBuyCounts.TryGetValue(nProductId, out var value))
		{
			return null;
		}
		return value;
	}

	public HeroDiaShopProductBuyCount GetOrCreateDailyDiaShopProductBuyCount(int nProductId)
	{
		HeroDiaShopProductBuyCount product = GetDailyDiaShopProductBuyCount(nProductId);
		if (product == null)
		{
			product = new HeroDiaShopProductBuyCount(this, nProductId, 0);
			AddDailyDiaShopProductBuyCount(product);
		}
		return product;
	}

	public void RefreshDailyDiaShopProductBuyCounts(DateTime date)
	{
		if (!(m_dailyDiaShopProductBuyCountsDate == date))
		{
			m_dailyDiaShopProductBuyCountsDate = date;
			m_dailyDiaShopProductBuyCounts.Clear();
		}
	}

	public void AddTotalDiaShopProductBuyCount(HeroDiaShopProductBuyCount product)
	{
		if (product == null)
		{
			throw new ArgumentNullException("product");
		}
		m_totalDiaShopProductBuyCounts.Add(product.productId, product);
	}

	public HeroDiaShopProductBuyCount GetTotalDiaShopProductBuyCount(int nProductId)
	{
		if (!m_totalDiaShopProductBuyCounts.TryGetValue(nProductId, out var value))
		{
			return null;
		}
		return value;
	}

	public HeroDiaShopProductBuyCount GetOrCreateTotalDiaShopProductBuyCount(int nProductId)
	{
		HeroDiaShopProductBuyCount product = GetTotalDiaShopProductBuyCount(nProductId);
		if (product == null)
		{
			product = new HeroDiaShopProductBuyCount(this, nProductId, 0);
			AddTotalDiaShopProductBuyCount(product);
		}
		return product;
	}

	public void RefreshDailyFearAltarPlayCount(DateTime date)
	{
		if (!(m_dailyFearAltarPlayCount.date == date))
		{
			m_dailyFearAltarPlayCount.date = date;
			m_dailyFearAltarPlayCount.value = 0;
		}
	}

	public int GetFearAltarAvailableEnterCount(DateTime date)
	{
		RefreshDailyFearAltarPlayCount(date);
		return Math.Max(m_vipLevel.fearAltarEnterCount - m_dailyFearAltarPlayCount.value, 0);
	}

	public void RefreshWeeklyFearAltarHalidomCollectionRewardNo(DateTime date)
	{
		DateTime weekStartDate = DateTimeUtil.GetWeekStartDate(date);
		if (!(m_weeklyFearAltarHalidomCollectionRewardNo.date == weekStartDate))
		{
			m_weeklyFearAltarHalidomCollectionRewardNo.date = weekStartDate;
			m_weeklyFearAltarHalidomCollectionRewardNo.value = 0;
		}
	}

	public void RefreshFearAltarHalidom(DateTime date)
	{
		DateTime weekStartDate = DateTimeUtil.GetWeekStartDate(date);
		if (!(m_fearAltarHalidomWeekStartDate == weekStartDate))
		{
			m_fearAltarHalidomWeekStartDate = weekStartDate;
			m_fearAltarHalidoms.Clear();
		}
	}

	public void AcquireFearAltarHalidom(FearAltarHalidomMonsterInstance monsterInst, DateTimeOffset time)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		Guid fearAltarInstnaceId = monsterInst.currentPlace.instanceId;
		DateTime weekStartDate = DateTimeUtil.GetWeekStartDate(time.Date);
		FearAltarHalidom halidom = monsterInst.halidom;
		HeroFearAltarHalidom heroHalidom = new HeroFearAltarHalidom(this);
		heroHalidom.Init(weekStartDate, halidom);
		AddFearAltarHalidom(heroHalidom);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroFearAltarHalidom(m_id, weekStartDate, halidom.id));
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddFearAltarHalidomAcquisitionLog(Guid.NewGuid(), fearAltarInstnaceId, m_id, halidom.id, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		ServerEvent.SendFearAltarHalidomAcquisition(m_account.peer, weekStartDate, halidom.id);
	}

	public void AddFearAltarHalidom(HeroFearAltarHalidom heroHalidom)
	{
		if (heroHalidom == null)
		{
			throw new ArgumentNullException("heroHalidom");
		}
		m_fearAltarHalidoms.Add(heroHalidom.halidom.id, heroHalidom);
	}

	public bool ContainsFearAltarHalidom(int nHalidomId)
	{
		return m_fearAltarHalidoms.ContainsKey(nHalidomId);
	}

	public List<int> GetFearAltarHalidoms()
	{
		List<int> results = new List<int>();
		foreach (HeroFearAltarHalidom halidom in m_fearAltarHalidoms.Values)
		{
			results.Add(halidom.halidom.id);
		}
		return results;
	}

	public bool CheckFearAltarHalidomElementalLevelIsFull(int nHalidomElementalId)
	{
		List<HeroFearAltarHalidom> heroHalidoms = new List<HeroFearAltarHalidom>();
		foreach (HeroFearAltarHalidom heroHalidom2 in m_fearAltarHalidoms.Values)
		{
			if (nHalidomElementalId == heroHalidom2.halidom.elemental.id)
			{
				heroHalidoms.Add(heroHalidom2);
			}
		}
		bool bIsFull = true;
		foreach (FearAltarHalidomLevel halidomLevel in Resource.instance.fearAltar.halidomLevels.Values)
		{
			bool bExistLevel = false;
			foreach (HeroFearAltarHalidom heroHalidom in heroHalidoms)
			{
				if (heroHalidom.halidom.level.level == halidomLevel.level)
				{
					bExistLevel = true;
					break;
				}
			}
			if (!bExistLevel)
			{
				return false;
			}
		}
		return bIsFull;
	}

	public void RefreshFearAltarHalidomElementalReward(DateTime date)
	{
		DateTime weekStartDate = DateTimeUtil.GetWeekStartDate(date);
		if (!(m_fearAltarHalidomElementalRewardWeekStartDate == weekStartDate))
		{
			m_fearAltarHalidomElementalRewardWeekStartDate = weekStartDate;
			m_fearAltarHalidomElementalRewards.Clear();
		}
	}

	public void AddFearAltarHalidomElementalReward(HeroFearAltarHalidomElementalReward heroReward)
	{
		if (heroReward == null)
		{
			throw new ArgumentNullException("heroReward");
		}
		m_fearAltarHalidomElementalRewards.Add(heroReward.halidomElemental.id, heroReward);
	}

	public bool ContainsFearAltarHalidomElementalReward(int nHalidomElementalId)
	{
		return m_fearAltarHalidomElementalRewards.ContainsKey(nHalidomElementalId);
	}

	public List<int> GetFearAltarHalidomCollectionReward()
	{
		List<int> results = new List<int>();
		foreach (HeroFearAltarHalidomElementalReward reward in m_fearAltarHalidomElementalRewards.Values)
		{
			results.Add(reward.halidomElemental.id);
		}
		return results;
	}

	public PDFearAltarHero ToPDFearAltarHero()
	{
		PDFearAltarHero inst = new PDFearAltarHero();
		inst.heroId = (Guid)m_id;
		inst.name = m_sName;
		inst.jobId = m_job.id;
		inst.level = m_nLevel;
		return inst;
	}

	public void AddSubQuest(HeroSubQuest subQuest)
	{
		if (subQuest == null)
		{
			throw new ArgumentNullException("subQuest");
		}
		m_subQuests.Add(subQuest.quest.id, subQuest);
		if (subQuest.isAccepted)
		{
			m_currentSubQuests.Add(subQuest.quest.id, subQuest);
		}
	}

	public HeroSubQuest GetSubQuest(int nId)
	{
		if (!m_subQuests.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveSubQuest(int nId)
	{
		m_subQuests.Remove(nId);
	}

	public bool IsReceivedSubQuest(int nId)
	{
		return m_subQuests.ContainsKey(nId);
	}

	public HeroSubQuest GetCurrentSubQuest(int nId)
	{
		if (!m_currentSubQuests.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveCurrentSubQuest(int nId)
	{
		m_currentSubQuests.Remove(nId);
	}

	public void ProcessAutoAcceptedSubQuestsForMainQuest(int nOldCompletedMainQuestNo, DateTimeOffset currentTime, bool bSendEvent)
	{
		List<SubQuest> subQuests = new List<SubQuest>();
		for (int i = nOldCompletedMainQuestNo + 1; i <= completedMainQuestNo; i++)
		{
			Resource.instance.GetAutoAcceptionSubQuests(SubQuestRequiredConditionType.MainQuestNo, i, subQuests);
		}
		if (subQuests.Count != 0)
		{
			ProcessAutoAcceptedSubQuests(subQuests, currentTime, bSendEvent);
		}
	}

	public void ProcessAutoAcceptedSubQuestsForHeroLevel(int nOldLevel, DateTimeOffset currentTime)
	{
		List<SubQuest> subQuests = new List<SubQuest>();
		for (int i = nOldLevel + 1; i <= m_nLevel; i++)
		{
			Resource.instance.GetAutoAcceptionSubQuests(SubQuestRequiredConditionType.HeroLevel, i, subQuests);
		}
		if (subQuests.Count != 0)
		{
			ProcessAutoAcceptedSubQuests(subQuests, currentTime, bSendEvent: true);
		}
	}

	private void ProcessAutoAcceptedSubQuests(IEnumerable<SubQuest> subQuests, DateTimeOffset currentTime, bool bSendEvent)
	{
		List<HeroSubQuest> addedHeroSubQuests = new List<HeroSubQuest>();
		foreach (SubQuest subQuest2 in subQuests)
		{
			if (!IsReceivedSubQuest(subQuest2.id))
			{
				HeroSubQuest heroSubQuest = new HeroSubQuest(this, subQuest2);
				AddSubQuest(heroSubQuest);
				addedHeroSubQuests.Add(heroSubQuest);
			}
		}
		if (addedHeroSubQuests.Count == 0)
		{
			return;
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (HeroSubQuest quest in addedHeroSubQuests)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroSubQuest(m_id, quest.quest.id, currentTime));
		}
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			foreach (HeroSubQuest subQuest in addedHeroSubQuests)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroSubQuestAcceptanceLog(Guid.NewGuid(), m_id, subQuest.quest.id, m_levelUpdateTime));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		if (bSendEvent)
		{
			ServerEvent.SendSubQuestsAccepted(m_account.peer, HeroSubQuest.ToPDHeroSubQuests(addedHeroSubQuests).ToArray());
		}
	}

	public void ProcessSubQuestForHunt(MonsterInstance monsterInst)
	{
		if (base.isDead)
		{
			return;
		}
		List<HeroSubQuest> changedHeroSubQuests = new List<HeroSubQuest>();
		foreach (HeroSubQuest heroSubQuest in m_currentSubQuests.Values)
		{
			if (!heroSubQuest.isObjectiveCompleted)
			{
				SubQuest subQuest2 = heroSubQuest.quest;
				if (subQuest2.type == SubQuestObjectiveType.Hunt && subQuest2.targetMonster.id == monsterInst.monster.id && heroSubQuest.IncreaseProgressCountBySubQuest(monsterInst.instanceId))
				{
					changedHeroSubQuests.Add(heroSubQuest);
				}
			}
		}
		if (changedHeroSubQuests.Count == 0)
		{
			return;
		}
		ServerEvent.SendSubQuestProgressCountsUpdated(m_account.peer, HeroSubQuest.ToPDHeroSubQuestProgressCounts(changedHeroSubQuests).ToArray());
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (HeroSubQuest subQuest in changedHeroSubQuests)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSubQuest_ProgressCount(subQuest.hero.id, subQuest.quest.id, subQuest.progressCount));
		}
		dbWork.Schedule();
	}

	public void ProcessSubQuestForAcquisition(MonsterInstance monsterInst)
	{
		if (base.isDead)
		{
			return;
		}
		List<HeroSubQuest> changedHeroSubQuests = new List<HeroSubQuest>();
		foreach (HeroSubQuest heroSubQuest in m_currentSubQuests.Values)
		{
			if (!heroSubQuest.isObjectiveCompleted)
			{
				SubQuest subQuest2 = heroSubQuest.quest;
				if (subQuest2.type == SubQuestObjectiveType.Acquisition && subQuest2.targetMonster.id == monsterInst.monster.id && Util.DrawLots(subQuest2.targetAcquisitionRate) && heroSubQuest.IncreaseProgressCountBySubQuest(monsterInst.instanceId))
				{
					changedHeroSubQuests.Add(heroSubQuest);
				}
			}
		}
		if (changedHeroSubQuests.Count == 0)
		{
			return;
		}
		ServerEvent.SendSubQuestProgressCountsUpdated(m_account.peer, HeroSubQuest.ToPDHeroSubQuestProgressCounts(changedHeroSubQuests).ToArray());
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (HeroSubQuest subQuest in changedHeroSubQuests)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSubQuest_ProgressCount(subQuest.hero.id, subQuest.quest.id, subQuest.progressCount));
		}
		dbWork.Schedule();
	}

	private void ProcessSubQuestForInteraction(ContinentObject continentObject)
	{
		if (base.isDead)
		{
			return;
		}
		List<HeroSubQuest> changedHeroSubQuests = new List<HeroSubQuest>();
		foreach (HeroSubQuest heroSubQuest in m_currentSubQuests.Values)
		{
			if (!heroSubQuest.isObjectiveCompleted)
			{
				SubQuest subQuest2 = heroSubQuest.quest;
				if (subQuest2.type == SubQuestObjectiveType.Interaction && subQuest2.targetContinentObject.id == continentObject.id)
				{
					heroSubQuest.IncreaseProgressCount();
					changedHeroSubQuests.Add(heroSubQuest);
				}
			}
		}
		if (changedHeroSubQuests.Count == 0)
		{
			return;
		}
		ServerEvent.SendSubQuestProgressCountsUpdated(m_account.peer, HeroSubQuest.ToPDHeroSubQuestProgressCounts(changedHeroSubQuests).ToArray());
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (HeroSubQuest subQuest in changedHeroSubQuests)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSubQuest_ProgressCount(subQuest.hero.id, subQuest.quest.id, subQuest.progressCount));
		}
		dbWork.Schedule();
	}

	public bool IsAvailableSubQuestInteractionObject(int nObjectId)
	{
		foreach (HeroSubQuest heroSubQuest in m_currentSubQuests.Values)
		{
			if (!heroSubQuest.isObjectiveCompleted)
			{
				SubQuest subQuest = heroSubQuest.quest;
				if (subQuest.type == SubQuestObjectiveType.Interaction && subQuest.targetContinentObject.id == nObjectId)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void ProcessSubQuestForContent(int nId)
	{
		List<HeroSubQuest> changedHeroSubQuests = new List<HeroSubQuest>();
		foreach (HeroSubQuest heroSubQuest in m_currentSubQuests.Values)
		{
			if (!heroSubQuest.isObjectiveCompleted)
			{
				SubQuest subQuest2 = heroSubQuest.quest;
				if (subQuest2.type == SubQuestObjectiveType.Content && subQuest2.targetContentId == nId)
				{
					heroSubQuest.IncreaseProgressCount();
					changedHeroSubQuests.Add(heroSubQuest);
				}
			}
		}
		if (changedHeroSubQuests.Count == 0)
		{
			return;
		}
		ServerEvent.SendSubQuestProgressCountsUpdated(m_account.peer, HeroSubQuest.ToPDHeroSubQuestProgressCounts(changedHeroSubQuests).ToArray());
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (HeroSubQuest subQuest in changedHeroSubQuests)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroSubQuest_ProgressCount(subQuest.hero.id, subQuest.quest.id, subQuest.progressCount));
		}
		dbWork.Schedule();
	}

	public void RefreshDailyWarMemoryFreePlayCount(DateTime date)
	{
		if (!(m_dailyWarMemoryFreePlayCount.date == date))
		{
			m_dailyWarMemoryFreePlayCount.date = date;
			m_dailyWarMemoryFreePlayCount.value = 0;
		}
	}

	public int GetWarMemoryAvailableFreeEnterCount(DateTime date)
	{
		RefreshDailyWarMemoryFreePlayCount(date);
		return Math.Max(Resource.instance.warMemory.freeEnterCount - m_dailyWarMemoryFreePlayCount.value, 0);
	}

	public void StartWarMemoryTransformationObjectInteraction(WarMemoryTransformationObjectInstance warMemoryTransformationObjectInst)
	{
		if (warMemoryTransformationObjectInst == null)
		{
			throw new ArgumentNullException("warMemoryTransformationObjectInst");
		}
		m_currentInteractionWarMemoryTransformationObjectInst = warMemoryTransformationObjectInst;
		m_currentInteractionWarMemoryTransformationObjectInst.interactionHero = this;
		ServerEvent.SendHeroWarMemoryTransformationObjectInteractionStart(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, warMemoryTransformationObjectInst.instanceId);
		int nDuration = (int)warMemoryTransformationObjectInst.interactionDuration * 1000;
		m_warMemoryTransformationObjectInteractionTimer = new Timer(OnWarMemoryTransformationObjectInteractionTimerTick, warMemoryTransformationObjectInst, -1, -1);
		m_warMemoryTransformationObjectInteractionTimer.Change(nDuration, -1);
	}

	private void OnWarMemoryTransformationObjectInteractionTimerTick(object state)
	{
		AddWork(new SFAction<WarMemoryTransformationObjectInstance>(ProcessWarMemoryTransformationObjectInteractionTimerTick, (WarMemoryTransformationObjectInstance)state), bGlobalLockRequired: false);
	}

	private void ProcessWarMemoryTransformationObjectInteractionTimerTick(WarMemoryTransformationObjectInstance warMemoryTransformationObjectInst)
	{
		DisposeWarMemoryTransformationObjectInteractionTimer();
		if (m_currentInteractionWarMemoryTransformationObjectInst != warMemoryTransformationObjectInst || m_currentInteractionWarMemoryTransformationObjectInst.isReleased)
		{
			return;
		}
		WarMemoryInstance warMemoryInst = (WarMemoryInstance)m_currentPlace;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		HeroWarMemoryPoint heroPoint = warMemoryInst.GetOrCreateHeroPoint(this);
		heroPoint.AddPoint(currentTime, m_currentInteractionWarMemoryTransformationObjectInst.transformationObject.transformationPoint);
		WarMemoryMonsterAttrFactor attrFactor = warMemoryInst.warMemory.GetMonsterAttrFactor(warMemoryInst.averageHeroLevel);
		HeroWarMemoryTransformationMonsterEffect effect = new HeroWarMemoryTransformationMonsterEffect();
		effect.Init(this, warMemoryTransformationObjectInst.transformationObject, attrFactor, currentTime);
		m_warMemoryTransformationMonsterEffect = effect;
		foreach (MonsterOwnSkill ownSkill in m_warMemoryTransformationMonsterEffect.transformationMonster.ownSkills)
		{
			HeroWarMemoryTransformationMonsterSkill skill = new HeroWarMemoryTransformationMonsterSkill(this, ownSkill.skill);
			m_warMemoryTransformationMonsterSkills.Add(skill.skillId, skill);
		}
		List<long> removedAbnormalStateEffects = new List<long>();
		foreach (AbnormalStateEffect abnormalStateeffect in m_abnormalStateEffects.Values)
		{
			removedAbnormalStateEffects.Add(abnormalStateeffect.id);
		}
		if (m_abnormalStateEffects.Count > 0)
		{
			ClearAllAbnormalStateEffects();
		}
		StopAcceleration();
		RefreshRealValues(bSendMaxHpChangedToOthers: false);
		RefreshMoveSpeed();
		m_nHP = m_nRealMaxHP;
		warMemoryInst.OnTransformationObjectInteractionFinish(m_currentInteractionWarMemoryTransformationObjectInst);
		ServerEvent.SendWarMemoryTransformationObjectInteractionFinished(m_account.peer, heroPoint.point, heroPoint.lastPointAcquisitionTime.Ticks, m_nRealMaxHP, m_nHP, removedAbnormalStateEffects.ToArray());
		ServerEvent.SendHeroWarMemoryTransformationObjectInteractionFinished(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_currentInteractionWarMemoryTransformationObjectInst.instanceId, heroPoint.point, heroPoint.lastPointAcquisitionTime.Ticks, m_nRealMaxHP, m_nHP, removedAbnormalStateEffects.ToArray());
		m_currentInteractionWarMemoryTransformationObjectInst = null;
	}

	public void CancelWarMemoryTransformationObjectInteraction(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		if (m_currentInteractionWarMemoryTransformationObjectInst != null)
		{
			DisposeWarMemoryTransformationObjectInteractionTimer();
			if (bSendEventToMyself)
			{
				ServerEvent.SendWarMemoryTransformationObjectInteractionCancel(m_account.peer);
			}
			if (bSendEventToOthers && m_currentPlace != null)
			{
				ServerEvent.SendHeroWarMemoryTransformationObjectInteractionCancel(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_currentInteractionWarMemoryTransformationObjectInst.instanceId);
			}
			m_currentInteractionWarMemoryTransformationObjectInst.interactionHero = null;
			m_currentInteractionWarMemoryTransformationObjectInst = null;
		}
	}

	private void DisposeWarMemoryTransformationObjectInteractionTimer()
	{
		if (m_warMemoryTransformationObjectInteractionTimer != null)
		{
			m_warMemoryTransformationObjectInteractionTimer.Dispose();
			m_warMemoryTransformationObjectInteractionTimer = null;
		}
	}

	public void ProcessWarMemoryTransformationMonsterEffectFinished()
	{
		if (m_warMemoryTransformationMonsterEffect == null)
		{
			return;
		}
		m_warMemoryTransformationMonsterEffect = null;
		ClearWarMemoryTransformationMonsterSkill();
		List<long> removedAbnormalStateEffects = new List<long>();
		foreach (AbnormalStateEffect abnormalStateeffect in m_abnormalStateEffects.Values)
		{
			removedAbnormalStateEffects.Add(abnormalStateeffect.id);
		}
		if (m_abnormalStateEffects.Count > 0)
		{
			ClearAllAbnormalStateEffects();
		}
		InitMoveStartTime(DateTimeUtil.currentTime);
		RefreshRealValues(bSendMaxHpChangedToOthers: false);
		RefreshMoveSpeed();
		m_nHP = m_nRealMaxHP;
		ServerEvent.SendWarMemoryMonsterTransformationFinished(m_account.peer, m_nRealMaxHP, m_nHP, removedAbnormalStateEffects.ToArray());
		ServerEvent.SendHeroWarMemoryMonsterTransformationFinished(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_nRealMaxHP, m_nHP, removedAbnormalStateEffects.ToArray());
	}

	public void CancelWarMemoryTransformationMonsterEffect(bool bResetHP, bool bSendEventToOthers)
	{
		if (m_warMemoryTransformationMonsterEffect == null)
		{
			return;
		}
		m_warMemoryTransformationMonsterEffect.Stop();
		m_warMemoryTransformationMonsterEffect = null;
		ClearWarMemoryTransformationMonsterSkill();
		List<long> removedAbnormalStateEffects = new List<long>();
		foreach (AbnormalStateEffect abnormalStateeffect in m_abnormalStateEffects.Values)
		{
			removedAbnormalStateEffects.Add(abnormalStateeffect.id);
		}
		if (m_abnormalStateEffects.Count > 0)
		{
			ClearAllAbnormalStateEffects();
		}
		InitMoveStartTime(DateTimeUtil.currentTime);
		RefreshRealValues(bSendMaxHpChangedToOthers: false);
		RefreshMoveSpeed();
		if (bResetHP)
		{
			m_nHP = m_nRealMaxHP;
		}
		ServerEvent.SendWarMemoryMonsterTransformationCancel(m_account.peer, m_nRealMaxHP, m_nHP, removedAbnormalStateEffects.ToArray());
		if (bSendEventToOthers)
		{
			ServerEvent.SendHeroWarMemoryMonsterTransformationCancel(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_nRealMaxHP, m_nHP, removedAbnormalStateEffects.ToArray());
		}
	}

	public HeroWarMemoryTransformationMonsterSkill GetWarMemoryTransformationMonsterSkill(int nSkillId)
	{
		if (!m_warMemoryTransformationMonsterSkills.TryGetValue(nSkillId, out var value))
		{
			return null;
		}
		return value;
	}

	private void ClearWarMemoryTransformationMonsterSkill()
	{
		m_warMemoryTransformationMonsterSkills.Clear();
	}

	public void ProcessOrdealQuestMissions(OrdealQuestMissionType type, int nProgressCount, DateTimeOffset time)
	{
		if (m_ordealQuest == null || m_ordealQuest.completed)
		{
			return;
		}
		List<HeroOrdealQuestSlot> changedSlots = new List<HeroOrdealQuestSlot>();
		HeroOrdealQuestSlot[] slots = m_ordealQuest.slots;
		foreach (HeroOrdealQuestSlot slot in slots)
		{
			if (slot.mission != null && slot.mission.type == type && !slot.IsObjectiveCompleted(time))
			{
				slot.IncreaseProgressCount(nProgressCount);
				changedSlots.Add(slot);
			}
		}
		if (changedSlots.Count == 0)
		{
			return;
		}
		ServerEvent.SendOrdealQuestSlotProgressCountsUpdated(m_account.peer, HeroOrdealQuestSlot.ToPDHeroOrdealQuestSlotProgressCounts(changedSlots).ToArray());
		SFSqlQueuingWork dbwork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (HeroOrdealQuestSlot slot2 in changedSlots)
		{
			dbwork.AddSqlCommand(GameDac.CSC_UpdateHeroOrdealQuestMission_ProgressCount(m_id, slot2.quest.no, slot2.index, slot2.mission.no, slot2.progressCount));
		}
		dbwork.Schedule();
	}

	public void ProcessOrdealQuestMissionsForHunt(MonsterInstance monsterInst, DateTimeOffset time)
	{
		if (base.isDead || m_ordealQuest == null || m_ordealQuest.completed)
		{
			return;
		}
		List<HeroOrdealQuestSlot> changedSlots = new List<HeroOrdealQuestSlot>();
		HeroOrdealQuestSlot[] slots = m_ordealQuest.slots;
		foreach (HeroOrdealQuestSlot slot in slots)
		{
			if (slot.mission != null && !slot.IsObjectiveCompleted(time))
			{
				OrdealQuestMission ordealQuestMission = slot.mission;
				if (ordealQuestMission.type == OrdealQuestMissionType.MounsterHunt && slot.IncreaseProgressCountByMonsterHuntType(monsterInst.instanceId))
				{
					changedSlots.Add(slot);
				}
			}
		}
		if (changedSlots.Count == 0)
		{
			return;
		}
		ServerEvent.SendOrdealQuestSlotProgressCountsUpdated(m_account.peer, HeroOrdealQuestSlot.ToPDHeroOrdealQuestSlotProgressCounts(changedSlots).ToArray());
		SFSqlQueuingWork dbwork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (HeroOrdealQuestSlot slot2 in changedSlots)
		{
			dbwork.AddSqlCommand(GameDac.CSC_UpdateHeroOrdealQuestMission_ProgressCount(m_id, slot2.quest.no, slot2.index, slot2.mission.no, slot2.progressCount));
		}
		dbwork.Schedule();
	}

	public void RefreshDailyOsirisRoomPlayCount(DateTime date)
	{
		if (!(m_dailyOsirisRoomPlayCount.date == date))
		{
			m_dailyOsirisRoomPlayCount.date = date;
			m_dailyOsirisRoomPlayCount.value = 0;
		}
	}

	public int GetOsirisRoomAvailableEnterCount(DateTime date)
	{
		RefreshDailyOsirisRoomPlayCount(date);
		return Math.Max(m_vipLevel.osirisRoomEnterCount - m_dailyOsirisRoomPlayCount.value, 0);
	}

	public void SetOsirisRoomMoneyBuff(MoneyBuff moneyBuff, DateTimeOffset time)
	{
		if (moneyBuff == null)
		{
			throw new ArgumentNullException("moneyBuff");
		}
		StopOsirisRoomMoneyBuff(bSendEvent: false);
		HeroOsirisRoomMoneyBuff heroMoneyBuff = new HeroOsirisRoomMoneyBuff();
		heroMoneyBuff.Init(this, moneyBuff, time);
		m_osirisRoomMoneyBuff = heroMoneyBuff;
		RefreshRealValues(bSendMaxHpChangedToOthers: true);
	}

	public void ProcessOsirisRoomMoneyBuffFinished()
	{
		if (m_osirisRoomMoneyBuff != null)
		{
			m_osirisRoomMoneyBuff = null;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
			ServerEvent.SendOsirisRoomMoneyBuffFinished(m_account.peer, m_nRealMaxHP, m_nHP);
		}
	}

	public void StopOsirisRoomMoneyBuff(bool bSendEvent)
	{
		if (m_osirisRoomMoneyBuff != null)
		{
			m_osirisRoomMoneyBuff.Stop();
			m_osirisRoomMoneyBuff = null;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
			if (bSendEvent)
			{
				ServerEvent.SendOsirisRoomMoneyBuffCancel(m_account.peer, m_nRealMaxHP, m_nHP);
			}
		}
	}

	public Friend AddFriend(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		Friend friend = new Friend(this);
		friend.Init(hero);
		AddFriend(friend);
		return friend;
	}

	private void AddFriend(Friend friend)
	{
		m_friends.Add(friend.id, friend);
	}

	public bool RemoveFriend(Guid id)
	{
		return m_friends.Remove(id);
	}

	public Friend GetFriend(Guid id)
	{
		if (!m_friends.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public bool IsFriend(Guid id)
	{
		return m_friends.ContainsKey(id);
	}

	public List<PDFriend> GetPDFriends()
	{
		return Friend.ToPDFriends(m_friends.Values);
	}

	public FriendApplication ApplyFriend(Hero target)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		FriendApplication app = new FriendApplication(this, target);
		AddFriendApplication(app);
		target.OnFriendApplicationReceived(app);
		return app;
	}

	public void OnFriendApplicationAccepted(FriendApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		RemoveFriendApplication(app.no);
		Friend newFriend = null;
		Hero target = app.target;
		if (!IsFriend(target.id))
		{
			newFriend = AddFriend(target);
		}
		if (newFriend != null)
		{
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_AddFriend(m_id, newFriend.id));
			dbWork.Schedule();
		}
		ServerEvent.SendFriendApplicationAccepted(m_account.peer, app.no, newFriend?.ToPDFriend());
	}

	public void OnFriendApplicationRefused(long lnNo)
	{
		RemoveFriendApplication(lnNo);
		ServerEvent.SendFriendApplicationRefused(m_account.peer, lnNo);
	}

	private void AddFriendApplication(FriendApplication app)
	{
		m_friendApplications.Add(app.no, app);
	}

	public void RemoveFriendApplication(long lnNo)
	{
		m_friendApplications.Remove(lnNo);
	}

	public FriendApplication GetFriendApplication(long lnNo)
	{
		if (!m_friendApplications.TryGetValue(lnNo, out var value))
		{
			return null;
		}
		return value;
	}

	public bool ContainsFriendApplication(Guid targetId)
	{
		foreach (FriendApplication app in m_friendApplications.Values)
		{
			if (app.target.id == targetId)
			{
				return true;
			}
		}
		return false;
	}

	public void CancelFriendApplication(FriendApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		RemoveFriendApplication(app.no);
		app.target.OnFriendApplicationCanceled(app.no);
	}

	public void CancelAllFriendApplications()
	{
		FriendApplication[] array = m_friendApplications.Values.ToArray();
		foreach (FriendApplication app in array)
		{
			lock (app.target.syncObject)
			{
				CancelFriendApplication(app);
			}
		}
	}

	public Friend AcceptFriendApplication(FriendApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		RemoveReceivedFriendApplication(app.no);
		Friend newFriend = null;
		Hero applicant = app.applicant;
		if (!IsFriend(applicant.id))
		{
			newFriend = AddFriend(applicant);
		}
		applicant.OnFriendApplicationAccepted(app);
		return newFriend;
	}

	public void OnFriendApplicationReceived(FriendApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		AddReceivedFriendApplication(app);
		ServerEvent.SendFriendApplicationReceived(m_account.peer, app.ToPDFriendApplication());
	}

	public void OnFriendApplicationCanceled(long lnNo)
	{
		RemoveReceivedFriendApplication(lnNo);
		ServerEvent.SendFriendApplicationCanceled(m_account.peer, lnNo);
	}

	private void AddReceivedFriendApplication(FriendApplication app)
	{
		m_receivedFriendApplications.Add(app.no, app);
	}

	private void RemoveReceivedFriendApplication(long lnNo)
	{
		m_receivedFriendApplications.Remove(lnNo);
	}

	public FriendApplication GetReceivedFriendApplication(long lnNo)
	{
		if (!m_receivedFriendApplications.TryGetValue(lnNo, out var value))
		{
			return null;
		}
		return value;
	}

	public void RefuseFriendApplication(FriendApplication app)
	{
		RemoveReceivedFriendApplication(app.no);
		app.applicant.OnFriendApplicationRefused(app.no);
	}

	public void RefuseAllFriendApplications()
	{
		FriendApplication[] array = m_receivedFriendApplications.Values.ToArray();
		foreach (FriendApplication app in array)
		{
			lock (app.applicant.syncObject)
			{
				RefuseFriendApplication(app);
			}
		}
	}

	public void AddTempFriend(Guid id, string sName, int nNationId, int nJobId, int nLevel, long lnBattlePower, DateTimeOffset time)
	{
		if (!IsTempFriend(id))
		{
			TempFriend newFriend = new TempFriend(this);
			newFriend.Init(id, sName, nNationId, nJobId, nLevel, lnBattlePower, time);
			AddTempFriend(newFriend);
			TempFriend removedTempFriend = null;
			if (m_tempFriends.Count > Resource.instance.tempFriendMaxCount)
			{
				removedTempFriend = m_tempFriends[0];
				m_tempFriends.RemoveAt(0);
			}
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_AddTempFriend(newFriend.owner.id, newFriend.id, newFriend.regTime));
			if (removedTempFriend != null)
			{
				dbWork.AddSqlCommand(GameDac.CSC_DeleteTempFriend(removedTempFriend.owner.id, removedTempFriend.id));
			}
			dbWork.Schedule();
			ServerEvent.SendTempFriendAdded(m_account.peer, newFriend.ToPDTempFriend(time), removedTempFriend?.id ?? Guid.Empty);
		}
	}

	public void AddTempFriend(Hero hero, DateTimeOffset time)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		AddTempFriend(hero.id, hero.name, hero.nationId, hero.jobId, hero.level, hero.battlePower, time);
	}

	private void AddTempFriend(TempFriend friend)
	{
		m_tempFriends.Add(friend);
	}

	public bool IsTempFriend(Guid id)
	{
		for (int i = 0; i < m_tempFriends.Count; i++)
		{
			if (m_tempFriends[i].id == id)
			{
				return true;
			}
		}
		return false;
	}

	public List<PDTempFriend> GetPDTempFriends(DateTimeOffset time)
	{
		return TempFriend.ToPDTempFriends(m_tempFriends, time);
	}

	public BlacklistEntry AddBlacklistEntry(Guid heroId, string sName, int nNationId, int nJobId, int nLevel, long lnBattlePower)
	{
		BlacklistEntry entry = new BlacklistEntry(this);
		entry.Init(heroId, sName, nNationId, nJobId, nLevel, lnBattlePower);
		AddBlacklistEntry(entry);
		return entry;
	}

	private void AddBlacklistEntry(BlacklistEntry entry)
	{
		m_blacklistEntries.Add(entry.heroId, entry);
	}

	public bool RemoveBlacklistEntry(Guid heroId)
	{
		return m_blacklistEntries.Remove(heroId);
	}

	public bool IsBlacklistEntry(Guid heroId)
	{
		return m_blacklistEntries.ContainsKey(heroId);
	}

	public List<PDBlacklistEntry> GetPDBlacklistEntries()
	{
		return BlacklistEntry.ToPDBlacklistEntries(m_blacklistEntries.Values);
	}

	public void AddDeadRecord(Hero killer, DateTimeOffset time)
	{
		if (killer == null)
		{
			throw new ArgumentNullException("killer");
		}
		DeadRecord newRecord = new DeadRecord(this);
		newRecord.Init(killer, time);
		AddDeadRecord(newRecord);
		DeadRecord removedRecord = null;
		if (m_deadRecords.Count > Resource.instance.deadRecordMaxCount)
		{
			removedRecord = m_deadRecords[0];
			m_deadRecords.RemoveAt(0);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDac.CSC_AddDeadRecord(newRecord.id, newRecord.owner.id, newRecord.killerId, newRecord.regTime));
		if (removedRecord != null)
		{
			dbWork.AddSqlCommand(GameDac.CSC_DeleteDeadRecord(removedRecord.id));
		}
		dbWork.Schedule();
		ServerEvent.SendDeadRecordAdded(m_account.peer, newRecord.ToPDDeadRecord(time), removedRecord?.id ?? Guid.Empty);
	}

	private void AddDeadRecord(DeadRecord record)
	{
		m_deadRecords.Add(record);
	}

	public List<PDDeadRecord> GetPDDeadRecords(DateTimeOffset time)
	{
		return DeadRecord.ToPDDeadRecords(m_deadRecords, time);
	}

	public void AddBiography(HeroBiography biography)
	{
		if (biography == null)
		{
			throw new ArgumentNullException("biography");
		}
		m_biographies.Add(biography.biography.id, biography);
	}

	public HeroBiography GetBiography(int nId)
	{
		if (!m_biographies.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool IsStartedBiography(int nBiographyId)
	{
		return m_biographies.ContainsKey(nBiographyId);
	}

	public void ProcessBiographyQuestsForHunt(MonsterInstance monsterInst)
	{
		if (base.isDead || m_biographies.Count == 0)
		{
			return;
		}
		List<HeroBiography> targetHeroBiographies = new List<HeroBiography>();
		foreach (HeroBiography heroBiography2 in m_biographies.Values)
		{
			if (heroBiography2.completed)
			{
				continue;
			}
			HeroBiographyQuest heroBiographyQuest = heroBiography2.quest;
			if (heroBiographyQuest != null && !heroBiographyQuest.isObjectiveCompleted)
			{
				BiographyQuest biographyQuest = heroBiographyQuest.quest;
				if (biographyQuest.type == BiographyQuestType.Hunt && biographyQuest.targetMonster.id == monsterInst.monster.id && heroBiographyQuest.IncreaseProgressCountByHunt(monsterInst.instanceId))
				{
					targetHeroBiographies.Add(heroBiography2);
				}
			}
		}
		if (targetHeroBiographies.Count == 0)
		{
			return;
		}
		ServerEvent.SendBiographyQuestProgressCountsUpdated(m_account.peer, HeroBiography.ToPDHeroBiographyQuestProgressCounts(targetHeroBiographies).ToArray());
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (HeroBiography heroBiography in targetHeroBiographies)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroBiography_ProgressCount(heroBiography.hero.id, heroBiography.biography.id, heroBiography.quest.quest.no, heroBiography.quest.progressCount));
		}
		dbWork.Schedule();
	}

	private void ProcessBiographyQuestsForInteraction(ContinentObject continentObject)
	{
		if (base.isDead || m_biographies.Count == 0)
		{
			return;
		}
		List<HeroBiography> targetHeroBiographies = new List<HeroBiography>();
		foreach (HeroBiography heroBiography2 in m_biographies.Values)
		{
			if (heroBiography2.completed)
			{
				continue;
			}
			HeroBiographyQuest heroBiographyQuest = heroBiography2.quest;
			if (heroBiographyQuest != null && !heroBiographyQuest.isObjectiveCompleted)
			{
				BiographyQuest biographyQuest = heroBiographyQuest.quest;
				if (biographyQuest.type == BiographyQuestType.ObjectInteraction && biographyQuest.targetContinentObject.id == continentObject.id)
				{
					heroBiographyQuest.IncreaseProgressCount();
					targetHeroBiographies.Add(heroBiography2);
				}
			}
		}
		if (targetHeroBiographies.Count == 0)
		{
			return;
		}
		ServerEvent.SendBiographyQuestProgressCountsUpdated(m_account.peer, HeroBiography.ToPDHeroBiographyQuestProgressCounts(targetHeroBiographies).ToArray());
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (HeroBiography heroBiography in targetHeroBiographies)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroBiography_ProgressCount(heroBiography.hero.id, heroBiography.biography.id, heroBiography.quest.quest.no, heroBiography.quest.progressCount));
		}
		dbWork.Schedule();
	}

	public bool IsAvailableBiographyQuestsInteractionObject(int nObjectId)
	{
		if (m_biographies.Count == 0)
		{
			return false;
		}
		foreach (HeroBiography heroBiography in m_biographies.Values)
		{
			if (heroBiography.completed)
			{
				continue;
			}
			HeroBiographyQuest heroBiographyQuest = heroBiography.quest;
			if (heroBiographyQuest != null && !heroBiographyQuest.isObjectiveCompleted)
			{
				BiographyQuest biographyQuest = heroBiographyQuest.quest;
				if (biographyQuest.type == BiographyQuestType.ObjectInteraction && biographyQuest.targetContinentObject.id == nObjectId)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void ProcessBiographyQuestsForDungeon(int nDungeonId)
	{
		if (base.isDead || m_biographies.Count == 0)
		{
			return;
		}
		List<HeroBiography> targetHeroBiographies = new List<HeroBiography>();
		foreach (HeroBiography heroBiography2 in m_biographies.Values)
		{
			if (heroBiography2.completed)
			{
				continue;
			}
			HeroBiographyQuest heroBiographyQuest = heroBiography2.quest;
			if (heroBiographyQuest != null && !heroBiographyQuest.isObjectiveCompleted)
			{
				BiographyQuest biographyQuest = heroBiographyQuest.quest;
				if (biographyQuest.type == BiographyQuestType.BiographyDungeon && biographyQuest.targetDungeonId == nDungeonId)
				{
					heroBiographyQuest.IncreaseProgressCount();
					targetHeroBiographies.Add(heroBiography2);
				}
			}
		}
		if (targetHeroBiographies.Count == 0)
		{
			return;
		}
		ServerEvent.SendBiographyQuestProgressCountsUpdated(m_account.peer, HeroBiography.ToPDHeroBiographyQuestProgressCounts(targetHeroBiographies).ToArray());
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (HeroBiography heroBiography in targetHeroBiographies)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroBiography_ProgressCount(heroBiography.hero.id, heroBiography.biography.id, heroBiography.quest.quest.no, heroBiography.quest.progressCount));
		}
		dbWork.Schedule();
	}

	public float GetRemainingItemLuckyShopFreePickTime(DateTimeOffset time)
	{
		ItemLuckyShop luckyShop = Resource.instance.itemLuckyShop;
		if (luckyShop == null)
		{
			return 0f;
		}
		float fElapsedTime = (float)(time - m_itemLuckyShopFreePickTime).TotalSeconds;
		return Math.Max((float)luckyShop.freePickWaitingTime - fElapsedTime, 0f);
	}

	public void RefreshItemLuckyShopPickCount(DateTime date)
	{
		if (!(m_itemLuckyShopPickDate == date))
		{
			m_itemLuckyShopPickDate = date;
			m_nItemLuckyShopFreePickCount = 0;
			m_nItemLuckyShopPick1TimeCount = 0;
			m_nItemLuckyShopPick5TimeCount = 0;
		}
	}

	public float GetRemainingCreatureCardLuckyShopFreePickTime(DateTimeOffset time)
	{
		CreatureCardLuckyShop luckyShop = Resource.instance.creatureCardLuckyShop;
		if (luckyShop == null)
		{
			return 0f;
		}
		float fElapsedTime = (float)(time - m_creatureCardLuckyShopFreePickTime).TotalSeconds;
		return Math.Max((float)luckyShop.freePickWaitingTime - fElapsedTime, 0f);
	}

	public void RefreshCreatureCardLuckyShopPickCount(DateTime date)
	{
		if (!(m_creatureCardLuckyShopPickDate == date))
		{
			m_creatureCardLuckyShopPickDate = date;
			m_nCreatureCardLuckyShopFreePickCount = 0;
			m_nCreatureCardLuckyShopPick1TimeCount = 0;
			m_nCreatureCardLuckyShopPick5TimeCount = 0;
		}
	}

	private void AddBlessingQuest(HeroBlessingQuest quest)
	{
		m_blessingQuests.Add(quest.id, quest);
	}

	public HeroBlessingQuest GetBlessingQuest(long lnId)
	{
		if (!m_blessingQuests.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	private void StartBlessingQuest(Guid targetHeroId, string sTargetName, BlessingTargetLevel targetLevel)
	{
		HeroBlessingQuest quest = new HeroBlessingQuest(this, targetHeroId, sTargetName, targetLevel);
		AddBlessingQuest(quest);
		ServerEvent.SendBlessingQuestStarted(m_account.peer, quest.ToPDHeroBlessingQuest());
	}

	public void StartBlessingQuest(Guid targetHeroId, string sTargetName, IEnumerable<BlessingTargetLevel> targetLevels)
	{
		foreach (BlessingTargetLevel targetLevel in targetLevels)
		{
			if (!isBlessingQuestListFull)
			{
				StartBlessingQuest(targetHeroId, sTargetName, targetLevel);
				continue;
			}
			break;
		}
	}

	public void RemoveBlessingQuest(long lnId)
	{
		m_blessingQuests.Remove(lnId);
	}

	public void RemoveAllBlessingQuests()
	{
		m_blessingQuests.Clear();
	}

	public HeroBlessing ReceiveBlessing(Blessing blessing, BlessingTargetLevel targetLevel, Guid senderHeroId, string sSenderName)
	{
		HeroBlessing heroBlessing = new HeroBlessing(this, blessing, targetLevel, senderHeroId, sSenderName);
		AddReceivedBlessing(heroBlessing);
		ServerEvent.SendBlessingReceived(m_account.peer, heroBlessing.ToPDHeroBlessing());
		return heroBlessing;
	}

	private void AddReceivedBlessing(HeroBlessing blessing)
	{
		m_receivedBlessings.Add(blessing.instanceId, blessing);
	}

	public HeroBlessing GetReceivedBlessing(long lnInstance)
	{
		if (!m_receivedBlessings.TryGetValue(lnInstance, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveReceivedBlessing(long lnInstance)
	{
		m_receivedBlessings.Remove(lnInstance);
	}

	public void RemoveAllReceivedBlessings()
	{
		m_receivedBlessings.Clear();
	}

	private void AddOwnerProspectQuest(HeroProspectQuest quest)
	{
		m_ownerProspectQuests.Add(quest.instanceId, quest);
	}

	public void RemoveOwnerProspectQuest(Guid instanceId)
	{
		m_ownerProspectQuests.Remove(instanceId);
	}

	public HeroProspectQuest GetOwnerProspectQuest(Guid instanceId)
	{
		if (!m_ownerProspectQuests.TryGetValue(instanceId, out var value))
		{
			return null;
		}
		return value;
	}

	public void StartOwnerProspectQuest(HeroProspectQuest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		AddOwnerProspectQuest(quest);
	}

	public void OnOwnerProspectQuestCompleted(Guid instanceId)
	{
		ServerEvent.SendOwnerProspectQuestCompleted(m_account.peer, instanceId);
	}

	public void OnOwnerProspectQuestFailed(Guid instanceId)
	{
		RemoveOwnerProspectQuest(instanceId);
		ServerEvent.SendOwnerProspectQuestFailed(m_account.peer, instanceId);
	}

	public List<PDHeroProspectQuest> GetOwnerPDHeroProspectQuests(DateTimeOffset time)
	{
		return HeroProspectQuest.ToPDHeroProspectQuests(m_ownerProspectQuests.Values, time);
	}

	private void AddTargetProspectQuest(HeroProspectQuest quest)
	{
		m_targetProspectQuests.Add(quest.instanceId, quest);
	}

	public void RemoveTargetProspectQuest(Guid instanceId)
	{
		m_targetProspectQuests.Remove(instanceId);
	}

	public HeroProspectQuest GetTargetProspectQuest(Guid instanceId)
	{
		if (!m_targetProspectQuests.TryGetValue(instanceId, out var value))
		{
			return null;
		}
		return value;
	}

	public void OnTargetProspectQuestStarted(HeroProspectQuest quest, DateTimeOffset currentTime)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		AddTargetProspectQuest(quest);
		ServerEvent.SendTargetProspectQuestStarted(m_account.peer, quest.ToPDHeroProspectQuest(currentTime));
	}

	public void OnTargetProspectQuestCompleted(Guid instanceId)
	{
		ServerEvent.SendTargetProspectQuestCompleted(m_account.peer, instanceId);
	}

	public void OnTargetProspectQuestFailed(Guid instanceId)
	{
		RemoveTargetProspectQuest(instanceId);
		ServerEvent.SendTargetProspectQuestFailed(m_account.peer, instanceId);
	}

	public List<PDHeroProspectQuest> GetTargetPDHeroProspectQuests(DateTimeOffset time)
	{
		return HeroProspectQuest.ToPDHeroProspectQuests(m_targetProspectQuests.Values, time);
	}

	public void AddCreature(HeroCreature creature, bool bInit)
	{
		if (creature == null)
		{
			throw new ArgumentNullException("creature");
		}
		m_creatures.Add(creature.instanceId, creature);
		if (!bInit && Resource.instance.CheckSystemMessageCondition(4, creature.creature.grade.grade))
		{
			SystemMessage.SendCreatureAcquirement(this, creature.creature.id);
		}
	}

	public HeroCreature GetCreature(Guid id)
	{
		if (!m_creatures.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveCreature(Guid id)
	{
		m_creatures.Remove(id);
	}

	public void ParticipateCreature(HeroCreature creature)
	{
		if (creature == null)
		{
			throw new ArgumentNullException("creature");
		}
		m_participationCreature = creature;
		if (m_currentPlace != null)
		{
			ServerEvent.SendHeroCreatureParticipated(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_participationCreature.creature.id);
		}
	}

	public void CancelCreatureParticipation()
	{
		if (m_participationCreature != null)
		{
			m_participationCreature = null;
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroCreatureParticipationCanceled(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
			}
		}
	}

	public void RefreshDailyCreatureVariationCount(DateTime date)
	{
		if (!(m_dailyCreatureVariationCount.date == date))
		{
			m_dailyCreatureVariationCount.date = date;
			m_dailyCreatureVariationCount.value = 0;
		}
	}

	public void HitDragonNestTrap(DragonNestTrapInstance dragonNestTrapInst, DateTimeOffset time)
	{
		if (dragonNestTrapInst == null || !hitEnabled)
		{
			return;
		}
		float fElapsedTime = (float)(time - m_dragonNestTrapLastHitTime).TotalSeconds;
		if (fElapsedTime < 3f)
		{
			return;
		}
		if (m_dragonNestTrapEffect != null)
		{
			m_dragonNestTrapEffect.Stop();
		}
		m_dragonNestTrapLastHitTime = time;
		DragonNestTrap trap = dragonNestTrapInst.trap;
		DragonNestMonsterAttrFactor attrFactor = dragonNestTrapInst.attrFactor;
		int nDamage = (int)Math.Floor((float)trap.damage * attrFactor.trapDamageFactor);
		HashSet<long> removedAbnormalStateEffects = new HashSet<long>();
		List<AbnormalStateEffect> absorbShieldEffects = new List<AbnormalStateEffect>();
		AbnormalStateEffect immortalityEffect = null;
		foreach (AbnormalStateEffect effect2 in m_abnormalStateEffects.Values)
		{
			switch (effect2.abnormalState.id)
			{
			case 8:
				nDamage = (int)Math.Floor((float)nDamage * ((float)effect2.abnormalStateLevel.value1 / 10000f));
				break;
			case 10:
			case 102:
			case 109:
				absorbShieldEffects.Add(effect2);
				break;
			case 106:
				if (immortalityEffect == null)
				{
					immortalityEffect = effect2;
				}
				break;
			}
		}
		List<PDAbnormalStateEffectDamageAbsorbShield> changedAbnormalStateEffectDamageAbsorbShields = new List<PDAbnormalStateEffectDamageAbsorbShield>();
		absorbShieldEffects.Sort(AbnormalStateEffect.Compare);
		foreach (AbnormalStateEffect absorbShieldEffect in absorbShieldEffects)
		{
			if (nDamage > 0)
			{
				nDamage = absorbShieldEffect.AbsorbDamage(nDamage);
				int nRemainingDamageAbsorbShield = absorbShieldEffect.damageAbsorbShieldRemainingAmount;
				if (nRemainingDamageAbsorbShield <= 0 && (absorbShieldEffect.abnormalState.id == 102 || absorbShieldEffect.abnormalState.id == 10))
				{
					absorbShieldEffect.Stop();
					RemoveAbnormalStateEffect(absorbShieldEffect.id);
					removedAbnormalStateEffects.Add(absorbShieldEffect.id);
				}
				PDAbnormalStateEffectDamageAbsorbShield shield = new PDAbnormalStateEffectDamageAbsorbShield();
				shield.abnormalStateEffectInstanceId = absorbShieldEffect.id;
				shield.remainingAbsorbShieldAmount = nRemainingDamageAbsorbShield;
				changedAbnormalStateEffectDamageAbsorbShields.Add(shield);
				continue;
			}
			break;
		}
		bool bIsImmortalityEffect = false;
		if (nDamage >= m_nHP && immortalityEffect != null)
		{
			bIsImmortalityEffect = immortalityEffect.UseImmortalityEffect();
		}
		Damage(null, nDamage, time, bIsImmortalityEffect, removedAbnormalStateEffects);
		if (base.isDead)
		{
			OnDead();
			m_currentPlace.OnUnitDead(this);
		}
		else
		{
			DragonNest dragonNest = trap.dragonNest;
			HeroDragonNestTrapEffect effect = new HeroDragonNestTrapEffect();
			effect.Init(this, trap, dragonNest.trapPenaltyMoveSpeed, dragonNest.trapPenaltyDuration, time);
			m_dragonNestTrapEffect = effect;
			RefreshMoveSpeed();
		}
		ServerEvent.SendHeroDragonNestTrapHit(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_id, bIsImmortalityEffect, m_nHP, m_nLastDamage, m_nLastHPDamage, changedAbnormalStateEffectDamageAbsorbShields.ToArray(), removedAbnormalStateEffects.ToArray());
	}

	public void ProcessDragonNestTrapEffectFinished()
	{
		if (m_dragonNestTrapEffect != null)
		{
			m_dragonNestTrapEffect = null;
			RefreshMoveSpeed();
			ServerEvent.SendDragonNestTrapEffectFinished(m_account.peer);
			ServerEvent.SendHeroDragonNestTrapEffectFinished(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
		}
	}

	public void StopDragonNestTrapEffect()
	{
		if (m_dragonNestTrapEffect != null)
		{
			m_dragonNestTrapEffect.Stop();
			m_dragonNestTrapEffect = null;
			RefreshMoveSpeed();
		}
	}

	public void RefreshWeeklyPresentPopularityPoint(DateTime weekStartDate)
	{
		if (!(m_weeklyPresentPopularityPointStartDate == weekStartDate))
		{
			m_weeklyPresentPopularityPointStartDate = weekStartDate;
			m_nWeeklyPresentPopularityPoint = 0;
			m_weeklyPresentPopularityPointUpdateTime = DateTimeOffset.MinValue;
		}
	}

	public void AddWeeklyPresentPopularityPoint(int nAmount, DateTimeOffset time)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nWeeklyPresentPopularityPoint += nAmount;
			m_weeklyPresentPopularityPointUpdateTime = time;
		}
	}

	public void RefreshWeeklyPresentContributionPoint(DateTime weekStartDate)
	{
		if (!(m_weeklyPresentContributionPointStartDate == weekStartDate))
		{
			m_weeklyPresentContributionPointStartDate = weekStartDate;
			m_nWeeklyPresentContributionPoint = 0;
			m_weeklyPresentContributionPointUpdateTime = DateTimeOffset.MinValue;
		}
	}

	public void AddWeeklyPresentContributionPoint(int nAmount, DateTimeOffset time)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nWeeklyPresentContributionPoint += nAmount;
			m_weeklyPresentContributionPointUpdateTime = time;
		}
	}

	public void AddCostume(HeroCostume heroCostume)
	{
		if (heroCostume == null)
		{
			throw new ArgumentNullException("heroCostume");
		}
		m_costumes.Add(heroCostume.costume.id, heroCostume);
	}

	public HeroCostume GetCostume(int nId)
	{
		if (!m_costumes.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool ContainsCostume(int nId)
	{
		return m_costumes.Keys.Contains(nId);
	}

	public List<PDHeroCostume> GetPDHeroCostumes(DateTimeOffset time)
	{
		List<PDHeroCostume> results = new List<PDHeroCostume>();
		foreach (HeroCostume heroCostume in m_costumes.Values)
		{
			results.Add(heroCostume.ToPDHeroCostume(time));
		}
		return results;
	}

	public void EquipCostume(HeroCostume heroCostume)
	{
		m_equippedCostume = heroCostume;
		if (m_currentPlace != null)
		{
			ServerEvent.SendHeroCostumeEquipped(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_equippedCostume.costumeId, m_equippedCostume.costumeEffectId);
		}
	}

	public void UnequipCostume()
	{
		if (m_equippedCostume != null)
		{
			m_equippedCostume = null;
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroCostumeUnequipped(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
			}
		}
	}

	private void OnUpdate_CheckCostumePeriodLimitTime()
	{
		try
		{
			CheckCostumePeriodLimitTime(DateTimeUtil.currentTime, bSendEvent: true);
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void CheckCostumePeriodLimitTime(DateTimeOffset currentTime, bool bSendEvent)
	{
		if (!isReal)
		{
			return;
		}
		HeroCostume[] array = m_costumes.Values.ToArray();
		foreach (HeroCostume heroCostume in array)
		{
			if (!heroCostume.costume.isUnlimit && !(heroCostume.GetRemainingTime(currentTime) > 0f))
			{
				bool bUnequippedCostume = false;
				if (heroCostume.isEquipped)
				{
					UnequipCostume();
					bUnequippedCostume = true;
				}
				m_costumes.Remove(heroCostume.costume.id);
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
				dbWork.AddSqlCommand(GameDac.CSC_DeleteHeroCostume(m_id, heroCostume.costumeId));
				if (bUnequippedCostume)
				{
					dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_EquippedHeroCostume(m_id, equippedCostumeId));
				}
				dbWork.Schedule();
				if (bSendEvent)
				{
					ServerEvent.SendCostumePeriodExpired(m_account.peer, heroCostume.costumeId);
				}
			}
		}
	}

	public void InitCostumeCollection()
	{
		m_costumeCollection = Resource.instance.SelectCostumeCollection(costumeCollectionId);
		m_bCostumeCollectionActivated = false;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CostumeCollectionId(m_id, costumeCollectionId));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_CostumeCollectionActivation(m_id, m_bCostumeCollectionActivated));
		dbWork.Schedule();
	}

	public void RefreshDailyCreatureFarmAcceptionCount(DateTime date)
	{
		if (!(m_dailyCreatureFarmAcceptionCount.date == date))
		{
			m_dailyCreatureFarmAcceptionCount.date = date;
			m_dailyCreatureFarmAcceptionCount.value = 0;
		}
	}

	public void AcceptCreatureFarmQuest(HeroCreatureFarmQuest creatureFarmQuest)
	{
		if (creatureFarmQuest == null)
		{
			throw new ArgumentNullException("creatureFarmQuest");
		}
		m_creatureFarmQuest = creatureFarmQuest;
	}

	public void CompleteCreatureFarmQuest()
	{
		if (creatureFarmQuest != null)
		{
			m_creatureFarmQuest = null;
		}
	}

	public void ProcessCreatureFarmQuestMissionMonsterSpawn(DateTimeOffset currentTime)
	{
		if (m_creatureFarmQuest == null || m_creatureFarmQuest.targetMonsterInst != null)
		{
			return;
		}
		CreatureFarmQuestMission mission = m_creatureFarmQuest.mission;
		if (mission != null && mission.targetType == CreatureFarmQuestMissionTargetType.ExclusiveMonsterHunt)
		{
			ContinentInstance targetContinentInstance = null;
			targetContinentInstance = ((!mission.targetContinent.isNationTerritory) ? ((ContinentInstance)Cache.instance.GetDisputeContinentInstance(mission.targetContinent.id)) : ((ContinentInstance)m_nationInst.GetContinentInstance(mission.targetContinent.id)));
			if (targetContinentInstance != null)
			{
				_ = Resource.instance.creatureFarmQuest;
				CreatureFarmQuestMissionMonsterArrange monsterArrange = mission.GetMonsterArrange(m_nLevel);
				CreatureFarmQuestMissionMonsterInstance monsterInst = new CreatureFarmQuestMissionMonsterInstance();
				monsterInst.Init(targetContinentInstance, monsterArrange, this, m_creatureFarmQuest.instanceId, mission.no, mission.targetAutoCompletionTime, currentTime);
				targetContinentInstance.SpawnMonster(monsterInst, currentTime);
				m_creatureFarmQuest.targetMonsterInst = monsterInst;
				ServerEvent.SendCreatureFarmQuestMissionMonsterSpawned(m_account.peer, monsterInst.instanceId, monsterInst.position, monsterInst.remainingLifetime);
			}
		}
	}

	public void ProcessCreatureFarmQuestMission_ExclusivMonster(CreatureFarmQuestMissionMonsterInstance monsterInst, DateTimeOffset currentTime)
	{
		if (m_creatureFarmQuest != null && !(m_creatureFarmQuest.instanceId != monsterInst.questMissionInstanceId))
		{
			CreatureFarmQuestMission mission = m_creatureFarmQuest.mission;
			if (mission != null && mission.no == monsterInst.questMissionNo)
			{
				m_creatureFarmQuest.IncreaseProgressCount(currentTime);
			}
		}
	}

	private void ProcessCreatureFarmQuestMissionForInteraction(ContinentObject continentObject, DateTimeOffset currentTime)
	{
		if (!base.isDead && m_creatureFarmQuest != null)
		{
			CreatureFarmQuestMission mission = m_creatureFarmQuest.mission;
			if (mission != null && mission.targetType == CreatureFarmQuestMissionTargetType.Interaction && mission.targetContinentObject.id == continentObject.id)
			{
				m_creatureFarmQuest.IncreaseProgressCount(currentTime);
			}
		}
	}

	public bool IsAvailableCreatureFarmQuestMissionInteractionObject(int nObjectId)
	{
		if (m_creatureFarmQuest == null)
		{
			return false;
		}
		CreatureFarmQuestMission mission = m_creatureFarmQuest.mission;
		if (mission == null)
		{
			return false;
		}
		if (mission.targetType != CreatureFarmQuestMissionTargetType.Interaction)
		{
			return false;
		}
		if (mission.targetContinentObject.id != nObjectId)
		{
			return false;
		}
		return true;
	}

	public void StartAutoHunt()
	{
		if (!m_bAutoHunting)
		{
			m_bAutoHunting = true;
			m_safeModeWaitStartTime = DateTimeUtil.currentTime;
		}
	}

	public void EndAutoHunt()
	{
		if (m_bAutoHunting)
		{
			m_bAutoHunting = false;
			StopSafeMode();
		}
	}

	private void OnUpdate_ManageSafeModeRequiredTime()
	{
		try
		{
			ManageSafeModeRequiredTime();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void ManageSafeModeRequiredTime()
	{
		SafeTimeEvent safeTimeEvent = Resource.instance.safeTimeEvent;
		if (safeTimeEvent == null || m_nLevel < Resource.instance.pvpMinHeroLevel || !(m_currentPlace is NationContinentInstance nationContinentInst) || nationContinentInst.nationId != nationId || !m_bAutoHunting)
		{
			return;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		int nTime = (int)currentTime.TimeOfDay.TotalSeconds;
		if (!safeTimeEvent.IsEventTime(nTime))
		{
			if (m_bIsSafeMode)
			{
				StopSafeMode();
			}
		}
		else
		{
			if (m_bIsSafeMode)
			{
				return;
			}
			DateTimeOffset startTime = currentTime.Date.AddSeconds(safeTimeEvent.startTime);
			if (startTime > m_safeModeWaitStartTime)
			{
				m_safeModeWaitStartTime = currentTime;
			}
			int nRequiredAutoDuration = safeTimeEvent.requiredAutoDuration;
			float fElapsedTime = DateTimeUtil.GetTimeSpanSeconds(m_safeModeWaitStartTime, currentTime);
			if (!((float)nRequiredAutoDuration > fElapsedTime))
			{
				m_bIsSafeMode = true;
				ServerEvent.SendSafeModeStarted(m_account.peer);
				if (m_currentPlace != null)
				{
					ServerEvent.SendHeroSafeModeStarted(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
				}
			}
		}
	}

	public void StopSafeMode()
	{
		if (m_bIsSafeMode)
		{
			m_bIsSafeMode = false;
			ServerEvent.SendSafeModeEnded(m_account.peer);
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroSafeModeEnded(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id);
			}
		}
	}

	public void InitSafeModeWaitStartTime(DateTimeOffset time)
	{
		if (m_bAutoHunting)
		{
			m_safeModeWaitStartTime = time;
		}
	}

	public void ProcessJobChangeQuestForHunt(MonsterInstance monsterInst)
	{
		if (base.isDead || m_jobChangeQuest == null || !m_jobChangeQuest.isAccepted || m_jobChangeQuest.isObjectiveCompleted)
		{
			return;
		}
		JobChangeQuest jobChangeQuest = m_jobChangeQuest.quest;
		if (jobChangeQuest.type != JobChangeQuestType.MonsterHunt || jobChangeQuest.targetMonster.id != monsterInst.monster.id || !(m_currentPlace is ContinentInstance currentPlace))
		{
			return;
		}
		if (jobChangeQuest.isTargetOwnNation)
		{
			if (currentPlace.nationId != m_nationInst.nationId)
			{
				return;
			}
		}
		else if (currentPlace.nationId == m_nationInst.nationId)
		{
			return;
		}
		m_jobChangeQuest.IncreaseProgressCount();
	}

	private void ProcessJobChangeQuestForInteraction(ContinentObject continentObject)
	{
		if (base.isDead || m_jobChangeQuest == null || !m_jobChangeQuest.isAccepted || m_jobChangeQuest.isObjectiveCompleted)
		{
			return;
		}
		JobChangeQuest jobChangeQuest = m_jobChangeQuest.quest;
		if (jobChangeQuest.type != JobChangeQuestType.Interaction || jobChangeQuest.targetContinentObject.id != continentObject.id || !(m_currentPlace is ContinentInstance currentPlace))
		{
			return;
		}
		if (jobChangeQuest.isTargetOwnNation)
		{
			if (currentPlace.nationId != m_nationInst.nationId)
			{
				return;
			}
		}
		else if (currentPlace.nationId == m_nationInst.nationId)
		{
			return;
		}
		m_jobChangeQuest.IncreaseProgressCount();
	}

	public bool IsAvailableJobChangeQuestInteractionObject(int nObjectId)
	{
		if (m_jobChangeQuest == null)
		{
			return false;
		}
		if (!m_jobChangeQuest.isAccepted)
		{
			return false;
		}
		if (m_jobChangeQuest.isObjectiveCompleted)
		{
			return false;
		}
		JobChangeQuest jobChangeQuest = m_jobChangeQuest.quest;
		if (jobChangeQuest.type != JobChangeQuestType.Interaction)
		{
			return false;
		}
		if (jobChangeQuest.targetContinentObject.id != nObjectId)
		{
			return false;
		}
		if (!(m_currentPlace is ContinentInstance currentPlace))
		{
			return false;
		}
		if (jobChangeQuest.isTargetOwnNation)
		{
			if (currentPlace.nationId != m_nationInst.nationId)
			{
				return false;
			}
		}
		else if (currentPlace.nationId == m_nationInst.nationId)
		{
			return false;
		}
		return true;
	}

	public void ProcessJobChangeQuestForExclusiveMonster(JobChangeQuestMonsterInstance monsterInst, DateTimeOffset currentTime)
	{
		if (m_jobChangeQuest != null && !(m_jobChangeQuest.instanceId != monsterInst.questInstanceId))
		{
			if (!base.isDead && monsterInst.currentPlace == m_currentPlace && monsterInst.IsQuestAreaPosition(m_position))
			{
				m_jobChangeQuest.IncreaseProgressCount();
				return;
			}
			m_jobChangeQuest.monsterInst = null;
			m_jobChangeQuest.Fail(bSendEvent: true, currentTime);
		}
	}

	private void OnUpdate_ProcessJobChangeQuestForMonsterSpawn()
	{
		try
		{
			ProcessJobChangeQuestForMonsterSpawn();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void ProcessJobChangeQuestForMonsterSpawn()
	{
		if (base.isDead || m_jobChangeQuest == null || !m_jobChangeQuest.isAccepted || m_jobChangeQuest.isObjectiveCompleted || m_jobChangeQuest.monsterInst != null || m_jobChangeQuest.difficulty <= 0)
		{
			return;
		}
		JobChangeQuest jobChangeQuest = m_jobChangeQuest.quest;
		JobChangeQuestDifficulty jobChangeQuestDifficulty = jobChangeQuest.GetDifficulty(m_jobChangeQuest.difficulty);
		MonsterArrange targetMonsterArrange = null;
		Vector3 selectedPosition = Vector3.zero;
		float fSelectedRotationY = 0f;
		if (jobChangeQuestDifficulty.isTargetPlaceGuildTerrtory)
		{
			if (!(m_currentPlace is GuildTerritoryInstance) || !jobChangeQuest.IsTargetGuildTerritoryAreaPosition(m_position, radius))
			{
				return;
			}
			targetMonsterArrange = jobChangeQuest.targetGuildMonsterArrange;
			selectedPosition = jobChangeQuest.SelectPositionOnGuildTerritory();
			fSelectedRotationY = jobChangeQuest.SelectRotationY();
		}
		else
		{
			if (!(m_currentPlace is ContinentInstance currentPlace) || currentPlace.continent.id != jobChangeQuest.targetContinent.id)
			{
				return;
			}
			if (jobChangeQuest.isTargetOwnNation)
			{
				if (currentPlace.nationId != m_nationInst.nationId)
				{
					return;
				}
			}
			else if (currentPlace.nationId == m_nationInst.nationId)
			{
				return;
			}
			if (!jobChangeQuest.IsTargetContinentAreaPosition(m_position, radius))
			{
				return;
			}
			targetMonsterArrange = jobChangeQuest.targetMonsterArrange;
			selectedPosition = jobChangeQuest.SelectPositionOnContinent();
			fSelectedRotationY = jobChangeQuest.SelectRotationY();
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		float fQuestRemainingTime = m_jobChangeQuest.GetRemainingTime(currentTime);
		m_jobChangeQuest.DisposeLimitTimer();
		JobChangeQuestMonsterInstance monsterInst = new JobChangeQuestMonsterInstance();
		monsterInst.Init(m_currentPlace, targetMonsterArrange, selectedPosition, fSelectedRotationY, this, m_jobChangeQuest.instanceId, fQuestRemainingTime, currentTime);
		m_currentPlace.SpawnMonster(monsterInst, currentTime);
		m_jobChangeQuest.monsterInst = monsterInst;
		ServerEvent.SendJobChangeQuestMonsterSpawned(m_account.peer, monsterInst.instanceId, monsterInst.position, monsterInst.remainingLifetime);
	}

	public void ChangeJob(Job job)
	{
		if (job == null)
		{
			throw new ArgumentNullException("job");
		}
		m_job = job;
		List<HeroSkill> heroSkills = m_skills.Values.ToList();
		ClearSkills();
		foreach (HeroSkill oldSkill in heroSkills)
		{
			JobSkill jobSkill = m_job.GetSkill(oldSkill.skillId);
			int nSkillLevel = oldSkill.level;
			HeroSkill newSkill = new HeroSkill(this, jobSkill);
			newSkill.level = nSkillLevel;
			AddSkill(newSkill);
		}
		if (m_currentPlace != null)
		{
			ServerEvent.SendHeroJobChanged(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_job.id);
		}
	}

	public void AddPotionAttr(HeroPotionAttr attr)
	{
		if (attr == null)
		{
			throw new ArgumentNullException("attr");
		}
		m_potionAttrs.Add(attr.potionAttr.id, attr);
	}

	public HeroPotionAttr GetPotionAttr(int nId)
	{
		if (!m_potionAttrs.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public HeroPotionAttr GetOrCreatePotiaonAttr(PotionAttr potionAttr)
	{
		HeroPotionAttr heroPotionAttr = GetPotionAttr(potionAttr.id);
		if (heroPotionAttr == null)
		{
			heroPotionAttr = new HeroPotionAttr(this, potionAttr);
			AddPotionAttr(heroPotionAttr);
		}
		return heroPotionAttr;
	}

	public void RefreshDailyAnkouTombPlayCount(DateTime date)
	{
		if (!(m_dailyAnkouTombPlayCount.date == date))
		{
			m_dailyAnkouTombPlayCount.date = date;
			m_dailyAnkouTombPlayCount.value = 0;
		}
	}

	public int GetAnkouTombAvailableEnterCount(DateTime date)
	{
		RefreshDailyAnkouTombPlayCount(date);
		return Math.Max(Resource.instance.ankouTomb.enterCount - m_dailyAnkouTombPlayCount.value, 0);
	}

	public void AddAnkouTombBestRecord(HeroAnkouTombBestRecord bestRecord)
	{
		if (bestRecord == null)
		{
			throw new ArgumentNullException("bestRecord");
		}
		m_ankouTombBestRecords.Add(bestRecord.difficulty, bestRecord);
	}

	public void RemoveAnkouTombBestRecord(int nDifficulty)
	{
		m_ankouTombBestRecords.Remove(nDifficulty);
	}

	public HeroAnkouTombBestRecord GetAnkouTombBestRecord(int nDifficulty)
	{
		if (!m_ankouTombBestRecords.TryGetValue(nDifficulty, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDHeroAnkouTombBestRecord> GetPDHeroAnkouTombBestRecord()
	{
		List<PDHeroAnkouTombBestRecord> results = new List<PDHeroAnkouTombBestRecord>();
		foreach (HeroAnkouTombBestRecord bestRecord in m_ankouTombBestRecords.Values)
		{
			results.Add(bestRecord.ToPDHeroAnkouTombBestRecord());
		}
		return results;
	}

	public bool RefreshAnkouTombBestRecord(HeroAnkouTombPoint heroPoint)
	{
		if (heroPoint == null)
		{
			throw new ArgumentNullException("heroPoint");
		}
		int nDifficulty = heroPoint.difficulty;
		HeroAnkouTombBestRecord oldBestRecord = GetAnkouTombBestRecord(nDifficulty);
		if (oldBestRecord != null)
		{
			if (heroPoint.point <= oldBestRecord.point)
			{
				return false;
			}
			RemoveAnkouTombBestRecord(nDifficulty);
		}
		HeroAnkouTombBestRecord newBestRecord = new HeroAnkouTombBestRecord();
		newBestRecord.Init(heroPoint);
		AddAnkouTombBestRecord(newBestRecord);
		return true;
	}

	public void SetAnkouTombMoneyBuff(MoneyBuff moneyBuff, DateTimeOffset time)
	{
		if (moneyBuff == null)
		{
			throw new ArgumentNullException("moneyBuff");
		}
		StopAnkouTombMoneyBuff(bSendEvent: false);
		HeroAnkouTombMoneyBuff heroMoneyBuff = new HeroAnkouTombMoneyBuff();
		heroMoneyBuff.Init(this, moneyBuff, time);
		m_ankouTombMoneyBuff = heroMoneyBuff;
		RefreshRealValues(bSendMaxHpChangedToOthers: true);
	}

	public void ProcessAnkouTombMoneyBuffFinished()
	{
		if (m_ankouTombMoneyBuff != null)
		{
			m_ankouTombMoneyBuff = null;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
			ServerEvent.SendAnkouTombMoneyBuffFinished(m_account.peer, m_nRealMaxHP, m_nHP);
		}
	}

	public void StopAnkouTombMoneyBuff(bool bSendEvent)
	{
		if (m_ankouTombMoneyBuff != null)
		{
			m_ankouTombMoneyBuff.Stop();
			m_ankouTombMoneyBuff = null;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
			if (bSendEvent)
			{
				ServerEvent.SendAnkouTombMoneyBuffCancel(m_account.peer, m_nRealMaxHP, m_nHP);
			}
		}
	}

	public void AddConstellation(HeroConstellation constellaiton)
	{
		if (constellaiton == null)
		{
			throw new ArgumentNullException("constellation");
		}
		m_constellations.Add(constellaiton.id, constellaiton);
	}

	public HeroConstellation GetConstellation(int nId)
	{
		if (!m_constellations.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool IsOpenedConstellation(int nId)
	{
		return m_constellations.ContainsKey(nId);
	}

	public HeroConstellation CreateConstellation(Constellation constellation)
	{
		if (constellation == null)
		{
			throw new ArgumentNullException("constellation");
		}
		HeroConstellation heroConstellation = new HeroConstellation(this, constellation.id);
		heroConstellation.CreateStep(constellation.GetStep(1));
		AddConstellation(heroConstellation);
		return heroConstellation;
	}

	private void OpenConstellations(IEnumerable<Constellation> constellations, bool bSendEvent)
	{
		List<HeroConstellation> opendHeroConstellations = new List<HeroConstellation>();
		foreach (Constellation constellation in constellations)
		{
			opendHeroConstellations.Add(CreateConstellation(constellation));
		}
		if (opendHeroConstellations.Count == 0)
		{
			return;
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (HeroConstellation opendConstellation in opendHeroConstellations)
		{
			HeroConstellationStep step = opendConstellation.GetStep(1);
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroConstellationStep(opendConstellation.hero.id, opendConstellation.id, step.step, step.entry.cycle.cycle, step.entry.no));
		}
		dbWork.Schedule();
		if (bSendEvent)
		{
			ServerEvent.SendConstellationOpened(m_account.peer, HeroConstellation.ToPDHeroConstellations(opendHeroConstellations).ToArray());
		}
	}

	private void OnLevelUp_Constellation()
	{
		List<Constellation> constellations = new List<Constellation>();
		foreach (Constellation constellation in Resource.instance.constellations.Values)
		{
			if (constellation.requiredConditonType == ConstellationRequiredConditionType.HeroLevel && constellation.requiredConditionValue <= m_nLevel && !IsOpenedConstellation(constellation.id))
			{
				constellations.Add(constellation);
			}
		}
		if (constellations.Count != 0)
		{
			OpenConstellations(constellations, bSendEvent: true);
		}
	}

	public void ProcessConstellationOpenForMainQuest(bool bSendEvent)
	{
		List<Constellation> constellations = new List<Constellation>();
		foreach (Constellation constellation in Resource.instance.constellations.Values)
		{
			if (constellation.requiredConditonType == ConstellationRequiredConditionType.MainQuest && IsMainQuestCompleted(constellation.requiredConditionValue) && !IsOpenedConstellation(constellation.id))
			{
				constellations.Add(constellation);
			}
		}
		if (constellations.Count != 0)
		{
			OpenConstellations(constellations, bSendEvent);
		}
	}

	public void AddStarEssense(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nStarEssense += nAmount;
		}
	}

	public void UseStarEssense(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			if (nAmount > m_nStarEssense)
			{
				throw new Exception("소지한 별의정수가 사용량보다 작습니다.");
			}
			m_nStarEssense -= nAmount;
		}
	}

	public void RefreshDailyStarEssenseItemUseCount(DateTime date)
	{
		if (!(m_dailyStarEssensItemUseCount.date == date))
		{
			m_dailyStarEssensItemUseCount.date = date;
			m_dailyStarEssensItemUseCount.value = 0;
		}
	}

	public void AddArtifactExp(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount == 0 || !isArtifactOpened || (isArtifactLast && isArtifactLevelMax))
		{
			return;
		}
		m_nArtifactExp += nAmount;
		bool bIsLevelUp = false;
		int nNextLevelUpRequiredExp = 0;
		Artifact artifact = Resource.instance.GetArtifact(m_nArtifactNo);
		while (true)
		{
			nNextLevelUpRequiredExp = artifact.GetLevel(m_nArtifactLevel).nextLevelUpRequiredExp;
			if (m_nArtifactExp < nNextLevelUpRequiredExp)
			{
				break;
			}
			bIsLevelUp = true;
			m_nArtifactLevel++;
			m_nArtifactExp -= nNextLevelUpRequiredExp;
			if (isArtifactLevelMax)
			{
				if (isArtifactLast)
				{
					m_nArtifactExp = 0;
					break;
				}
				m_nArtifactNo++;
				m_nArtifactLevel = 0;
				artifact = Resource.instance.GetArtifact(m_nArtifactNo);
			}
		}
		if (bIsLevelUp)
		{
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
	}

	public void SetEquippedArtifact(int ArtifactNo)
	{
		if (m_nEquippedArtifactNo != ArtifactNo)
		{
			m_nEquippedArtifactNo = ArtifactNo;
			if (m_currentPlace != null)
			{
				ServerEvent.SendHeroEquippedArtifactChanged(m_currentPlace.GetDynamicClientPeers(m_sector, m_id), m_id, m_nEquippedArtifactNo);
			}
		}
	}

	private void OpenArtifact(bool bSendEvent)
	{
		if (!isArtifactOpened)
		{
			m_nArtifactNo = 1;
			m_nArtifactLevel = 0;
			m_nArtifactExp = 0;
			SetEquippedArtifact(m_nArtifactNo);
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_ArtifactOpen(this));
			dbWork.Schedule();
			if (bSendEvent)
			{
				ServerEvent.SendArtifactOpened(m_account.peer, m_nArtifactNo, m_nArtifactLevel, m_nArtifactExp, m_nEquippedArtifactNo, m_nRealMaxHP, m_nHP);
			}
		}
	}

	private void OnLevelUp_Artifact()
	{
		if (Resource.instance.artifactRequiredConditionType == ArtifactRequiredConditionType.HeroLevel)
		{
			int nRequiredHeroLevel = Resource.instance.artifactRequiredConditionValue;
			if (m_nLevel >= nRequiredHeroLevel)
			{
				OpenArtifact(bSendEvent: true);
			}
		}
	}

	public void ProcessArtifactOpenForMainQuest(bool bSendEvent)
	{
		if (Resource.instance.artifactRequiredConditionType == ArtifactRequiredConditionType.MainQuest)
		{
			int nRequiredMainQuestNo = Resource.instance.artifactRequiredConditionValue;
			if (IsMainQuestCompleted(nRequiredMainQuestNo))
			{
				OpenArtifact(bSendEvent);
			}
		}
	}

	public void RefreshDailyTradeShipPlayCount(DateTime date)
	{
		if (!(m_dailyTradeShipPlayCount.date == date))
		{
			m_dailyTradeShipPlayCount.date = date;
			m_dailyTradeShipPlayCount.value = 0;
		}
	}

	public int GetTradeShipAvailableEnterCount(DateTime date)
	{
		RefreshDailyTradeShipPlayCount(date);
		return Math.Max(m_vipLevel.tradeShipEnterCount - m_dailyTradeShipPlayCount.value, 0);
	}

	public void AddTradeShipBestRecord(HeroTradeShipBestRecord bestRecord)
	{
		if (bestRecord == null)
		{
			throw new ArgumentNullException("bestRecord");
		}
		m_tradeShipBestRecords.Add(bestRecord.difficulty, bestRecord);
	}

	public void RemoveTradeShipBestRecord(int nDifficulty)
	{
		m_tradeShipBestRecords.Remove(nDifficulty);
	}

	public HeroTradeShipBestRecord GetTradeShipBestRecord(int nDifficulty)
	{
		if (!m_tradeShipBestRecords.TryGetValue(nDifficulty, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDHeroTradeShipBestRecord> GetPDHeroTradeShipBestRecord()
	{
		List<PDHeroTradeShipBestRecord> results = new List<PDHeroTradeShipBestRecord>();
		foreach (HeroTradeShipBestRecord bestRecord in m_tradeShipBestRecords.Values)
		{
			results.Add(bestRecord.ToPDHeroTradeShipBestRecord());
		}
		return results;
	}

	public bool RefreshTradeShipBestRecord(HeroTradeShipPoint heroPoint)
	{
		if (heroPoint == null)
		{
			throw new ArgumentNullException("heroPoint");
		}
		int nDifficulty = heroPoint.difficulty;
		HeroTradeShipBestRecord oldBestRecord = GetTradeShipBestRecord(nDifficulty);
		if (oldBestRecord != null)
		{
			if (heroPoint.point <= oldBestRecord.point)
			{
				return false;
			}
			RemoveTradeShipBestRecord(nDifficulty);
		}
		HeroTradeShipBestRecord newBestRecord = new HeroTradeShipBestRecord();
		newBestRecord.Init(heroPoint);
		AddTradeShipBestRecord(newBestRecord);
		return true;
	}

	public void SetTradeShipMoneyBuff(MoneyBuff moneyBuff, DateTimeOffset time)
	{
		if (moneyBuff == null)
		{
			throw new ArgumentNullException("moneyBuff");
		}
		StopTradeShipMoneyBuff(bSendEvent: false);
		HeroTradeShipMoneyBuff heroMoneyBuff = new HeroTradeShipMoneyBuff();
		heroMoneyBuff.Init(this, moneyBuff, time);
		m_tradeShipMoneyBuff = heroMoneyBuff;
		RefreshRealValues(bSendMaxHpChangedToOthers: true);
	}

	public void ProcessTradeShipMoneyBuffFinished()
	{
		if (m_tradeShipMoneyBuff != null)
		{
			m_tradeShipMoneyBuff = null;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
			ServerEvent.SendTradeShipMoneyBuffFinished(m_account.peer, m_nRealMaxHP, m_nHP);
		}
	}

	public void StopTradeShipMoneyBuff(bool bSendEvent)
	{
		if (m_tradeShipMoneyBuff != null)
		{
			m_tradeShipMoneyBuff.Stop();
			m_tradeShipMoneyBuff = null;
			RefreshRealValues(bSendMaxHpChangedToOthers: true);
			if (bSendEvent)
			{
				ServerEvent.SendTradeShipMoneyBuffCancel(m_account.peer, m_nRealMaxHP, m_nHP);
			}
		}
	}

	private void GM_CompleteTargetMainQuest(DateTimeOffset currentTime)
	{
		int nTargetMainQuestNo = m_nGMTargetMainQuestNo;
		int nStartMainQuestNo = completedMainQuestNo + 1;
		if (nStartMainQuestNo > nTargetMainQuestNo)
		{
			return;
		}
		List<MainQuest> mainQuests = new List<MainQuest>();
		ResultItemCollection resultItemCollection = new ResultItemCollection();
		long lnRewardExp = 0L;
		long lnRewardGold = 0L;
		for (int nQstNo = nStartMainQuestNo; nQstNo <= nTargetMainQuestNo; nQstNo++)
		{
			MainQuest mainQuest2 = Resource.instance.GetMainQuest(nQstNo);
			if (mainQuest2 == null)
			{
				break;
			}
			int nRemainingEmptyInventoryCount = emptyInventorySlotCount;
			foreach (MainQuestReward reward2 in mainQuest2.rewards)
			{
				if (reward2.jobId > 0 && reward2.jobId != baseJobId)
				{
					continue;
				}
				switch (reward2.type)
				{
				case MainQuestRewardType.MainGear:
					if (nRemainingEmptyInventoryCount < 1)
					{
						throw new Exception("[GM툴 메인퀘스트 완료] 인벤토리가 부족합니다.");
					}
					nRemainingEmptyInventoryCount--;
					break;
				case MainQuestRewardType.SubGear:
					if (!ContainsSubGear(reward2.subGear.id))
					{
						if (nRemainingEmptyInventoryCount < 1)
						{
							throw new Exception("[GM툴 메인퀘스트 완료] 인벤토리가 부족합니다.");
						}
						nRemainingEmptyInventoryCount--;
					}
					break;
				case MainQuestRewardType.Item:
				{
					ItemReward itemReward2 = reward2.itemReward;
					resultItemCollection.AddResultItemCount(itemReward2.item, itemReward2.owned, itemReward2.count);
					break;
				}
				}
			}
			if (!IsAvailableInventory(resultItemCollection, nRemainingEmptyInventoryCount))
			{
				throw new Exception("[GM툴 메인퀘스트 완료] 인벤토리가 부족합니다.");
			}
			lnRewardExp += mainQuest2.expRewardValue;
			lnRewardGold += mainQuest2.goldRewardValue;
			mainQuests.Add(mainQuest2);
		}
		List<HeroMainQuest> heroMainQuests = new List<HeroMainQuest>();
		List<HeroMainGear> rewardMainGears = new List<HeroMainGear>();
		List<HeroSubGear> rewardSubGears = new List<HeroSubGear>();
		HashSet<InventorySlot> changedInventorySlots = new HashSet<InventorySlot>();
		List<HeroMount> rewardMounts = new List<HeroMount>();
		List<HeroCreatureCard> changedCreatureCards = new List<HeroCreatureCard>();
		foreach (MainQuest mainQuest in mainQuests)
		{
			foreach (MainQuestReward reward in mainQuest.rewards)
			{
				if (reward.jobId > 0 && reward.jobId != baseJobId)
				{
					continue;
				}
				switch (reward.type)
				{
				case MainQuestRewardType.MainGear:
				{
					int nEnchantLevel = 0;
					bool bOwned = reward.mainGearOwned;
					HeroMainGear heroMainGear = new HeroMainGear(this);
					heroMainGear.Init(reward.mainGear, nEnchantLevel, bOwned, currentTime);
					AddMainGear(heroMainGear, bInit: false, currentTime);
					InventorySlot emptySlot = GetEmptyInventorySlot();
					emptySlot.Place(heroMainGear);
					changedInventorySlots.Add(emptySlot);
					rewardMainGears.Add(heroMainGear);
					break;
				}
				case MainQuestRewardType.SubGear:
				{
					SubGear rewardSubGear = reward.subGear;
					int nLevel = 1;
					if (!ContainsSubGear(rewardSubGear.id))
					{
						HeroSubGear heroSubGear = new HeroSubGear(this);
						heroSubGear.Init(rewardSubGear, nLevel, currentTime);
						AddSubGear(heroSubGear);
						InventorySlot emptySlot2 = GetEmptyInventorySlot();
						emptySlot2.Place(heroSubGear);
						changedInventorySlots.Add(emptySlot2);
						rewardSubGears.Add(heroSubGear);
					}
					break;
				}
				case MainQuestRewardType.Item:
				{
					ItemReward itemReward = reward.itemReward;
					AddItem(itemReward.item, itemReward.owned, itemReward.count, changedInventorySlots);
					break;
				}
				case MainQuestRewardType.Mount:
				{
					Mount mount = reward.mount;
					int nMountLevel = 1;
					int nMountSatiety = 0;
					if (!ContainsMount(mount.id))
					{
						HeroMount heroMount2 = new HeroMount(this, mount, nMountLevel, nMountSatiety);
						AddMount(heroMount2);
						heroMount2.RefreshAttrTotalValues();
						rewardMounts.Add(heroMount2);
					}
					break;
				}
				case MainQuestRewardType.CreatureCard:
				{
					CreatureCard creatureCard = reward.creatureCard;
					HeroCreatureCard heroCreatureCard2 = GetOrCreateCreatureCard(creatureCard);
					heroCreatureCard2.count++;
					changedCreatureCards.Add(heroCreatureCard2);
					break;
				}
				}
			}
			HeroMainQuest heroMainQuest2 = new HeroMainQuest(this, mainQuest);
			heroMainQuest2.completed = true;
			heroMainQuests.Add(heroMainQuest2);
			if (mainQuest.no == nTargetMainQuestNo)
			{
				m_currentHeroMainQuest = heroMainQuest2;
			}
		}
		lnRewardExp = (long)((float)lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_nLevel));
		AddExp(lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		AddGold(lnRewardGold);
		bool bIsRankOpened = false;
		if (m_rank == null && nTargetMainQuestNo >= Resource.instance.rankOpenRequiredMainQuestNo)
		{
			bIsRankOpened = true;
			SetRank(Resource.instance.GetRank(1));
		}
		ProcessAutoAcceptedSubQuestsForMainQuest(nStartMainQuestNo - 1, currentTime, bSendEvent: false);
		ProcessArtifactOpenForMainQuest(bSendEvent: false);
		ProcessConstellationOpenForMainQuest(bSendEvent: false);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(this));
		dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(this));
		if (bIsRankOpened)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Rank(this));
		}
		foreach (HeroMainQuest heroMainQuest in heroMainQuests)
		{
			if (heroMainQuest.mainQuest.no != nStartMainQuestNo)
			{
				dbWork.AddSqlCommand(GameDac.CSC_AddHeroMainQuest(m_id, heroMainQuest.mainQuest.no, heroMainQuest.isCartRiding, heroMainQuest.cartContinentId, heroMainQuest.cartPosition.x, heroMainQuest.cartPosition.y, heroMainQuest.cartPosition.z, heroMainQuest.cartRotationY, currentTime));
			}
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroMainQuest_Complete(m_id, heroMainQuest.mainQuest.no, currentTime));
		}
		foreach (HeroMainGear mainGear in rewardMainGears)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddHeroMainGear(mainGear));
		}
		foreach (HeroSubGear subGear in rewardSubGears)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroSubGear(subGear.hero.id, subGear.subGearId, subGear.level, subGear.quality, subGear.equipped, subGear.regTime));
		}
		foreach (InventorySlot slot in changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		foreach (HeroMount heroMount in rewardMounts)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroMount(heroMount.hero.id, heroMount.mount.id, heroMount.level, heroMount.satiety));
		}
		foreach (HeroCreatureCard heroCreatureCard in changedCreatureCards)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroCreatureCard(heroCreatureCard));
		}
		dbWork.Schedule();
	}

	public void AddTimeDesignationEventReward(int nEventId)
	{
		m_timeDesignationEventRewards.Add(nEventId);
	}

	public bool IsReceivedTimeDesignationEventReward(int nEventId)
	{
		return m_timeDesignationEventRewards.Contains(nEventId);
	}

	public void ReceiveTimeDesignationEventRewards(IEnumerable<TimeDesignationEvent> timeDesignationEvents, bool bSendEvent, DateTimeOffset currentTime)
	{
		List<int> rewardedTimeDesignationEventIds = new List<int>();
		List<Mail> mails = new List<Mail>();
		foreach (TimeDesignationEvent evt in timeDesignationEvents)
		{
			int nEventId2 = evt.id;
			if (!IsReceivedTimeDesignationEventReward(nEventId2))
			{
				AddTimeDesignationEventReward(nEventId2);
				rewardedTimeDesignationEventIds.Add(nEventId2);
				Mail mail2 = evt.CreateMail(currentTime);
				mails.Add(mail2);
				AddMail(mail2, bSendEvent);
			}
		}
		if (rewardedTimeDesignationEventIds.Count == 0)
		{
			return;
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		foreach (int nEventId in rewardedTimeDesignationEventIds)
		{
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroTimeDesignationEventReward(m_id, nEventId));
		}
		foreach (Mail mail in mails)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(mail));
		}
		dbWork.Schedule();
	}
}
