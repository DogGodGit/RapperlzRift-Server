namespace ClientCommon;

public enum ClientCommandName : short
{
	None,
	Test,
	Test2,
	GetTime,
	Login,
	LobbyInfo,
	HeroCreate,
	HeroNamingTutorialComplete,
	HeroNameSet,
	HeroDelete,
	HeroLogin,
	HeroInitEnter,
	HeroLogout,
	HeroPosition,
	HeroInfo,
	HeroSearch,
	MailReceive,
	MailReceiveAll,
	MailDelete,
	MailDeleteAll,
	MainGearEquip,
	MainGearUnequip,
	MainGearEnchant,
	MainGearTransit,
	MainGearRefine,
	MainGearRefinementApply,
	MainGearDisassemble,
	MainGearEnchantLevelSetActivate,
	SubGearEquip,
	SubGearUnequip,
	SoulstoneSocketMount,
	SoulstoneSocketUnmount,
	MountedSoulstoneCompose,
	RuneSocketMount,
	RuneSocketUnmount,
	SubGearGradeUp,
	SubGearLevelUp,
	SubGearLevelUpTotally,
	SubGearQualityUp,
	SubGearSoulstoneLevelSetActivate,
	SkillLevelUp,
	SkillLevelUpTotally,
	ImmediateRevive,
	ContinentSaftyRevive,
	ContinentEnterForSaftyRevival,
	PrevContinentEnter,
	PortalEnter,
	PortalExit,
	ContinentObjectInteractionStart,
	ContinentTransmission,
	ContinentEnterForContinentTransmission,
	ContinentSaftyAreaEnter,
	ContinentEnterForContinentBanished,
	MainQuestAccept,
	MainQuestComplete,
	TreatOfFarmQuestAccept,
	TreatOfFarmQuestComplete,
	TreatOfFarmQuestMissionAccept,
	TreatOfFarmQuestMissionAbandon,
	BountyHunterQuestScrollUse,
	BountyHunterQuestAbandon,
	BountyHunterQuestComplete,
	FishingBaitUse,
	FishingStart,
	SimpleShopBuy,
	SimpleShopSell,
	InventorySlotExtend,
	ItemCompose,
	ItemComposeTotally,
	HpPotionUse,
	ReturnScrollUse,
	ContinentEnterForReturnScrollUse,
	MainGearBoxUse,
	PickBoxUse,
	ExpPotionUse,
	ExpScrollUse,
	TitleItemUse,
	GoldItemUse,
	OwnDiaItemUse,
	HonorPointItemUse,
	ExploitPointItemUse,
	WingItemUse,
	StarEssenseItemUse,
	PremiumStarEssenseItemUse,
	RestRewardReceiveFree,
	RestRewardReceiveGold,
	RestRewardReceiveDia,
	PartySurroundingHeroList,
	PartySurroundingPartyList,
	PartyCreate,
	PartyExit,
	PartyMasterChange,
	PartyMemberBanish,
	PartyCall,
	PartyDisband,
	PartyApply,
	PartyApplicationAccept,
	PartyApplicationRefuse,
	PartyInvite,
	PartyInvitationAccept,
	PartyInvitationRefuse,
	ChattingMessageSend,
	LevelUpRewardReceive,
	AttendRewardReceive,
	DailyAccessTimeRewardReceive,
	SeriesMissionRewardReceive,
	TodayMissionRewardReceive,
	TodayMissionTutorialStart,
	AttainmentRewardReceive,
	ContinentExitForMainQuestDungeonEnter,
	MainQuestDungeonEnter,
	MainQuestDungeonAbandon,
	MainQuestDungeonExit,
	MainQuestDungeonSaftyRevive,
	MountEquip,
	MountLevelUp,
	MountGearEquip,
	MountGearUnequip,
	MountGearPickBoxMake,
	MountGearPickBoxMakeTotally,
	MountGearRefine,
	MountGetOn,
	MountAwakeningLevelUp,
	MountAttrPotionUse,
	MountItemUse,
	WingEquip,
	WingEnchant,
	WingEnchantTotally,
	WingMemoryPieceInstall,
	ContinentExitForStoryDungeonEnter,
	StoryDungeonEnter,
	StoryDungeonAbandon,
	StoryDungeonExit,
	StoryDungeonRevive,
	StoryDungeonSweep,
	StoryDungeonMonsterTame,
	ContinentExitForExpDungeonEnter,
	ExpDungeonEnter,
	ExpDungeonAbandon,
	ExpDungeonExit,
	ExpDungeonRevive,
	ExpDungeonSweep,
	ContinentExitForGoldDungeonEnter,
	GoldDungeonEnter,
	GoldDungeonAbandon,
	GoldDungeonExit,
	GoldDungeonRevive,
	GoldDungeonSweep,
	NationTransmission,
	ContinentEnterForNationTransmission,
	NationDonate,
	ContinentExitForUndergroundMazeEnter,
	UndergroundMazeEnter,
	UndergroundMazeExit,
	UndergroundMazeTransmission,
	UndergroundMazeEnterForTransmission,
	UndergroundMazePortalEnter,
	UndergroundMazePortalExit,
	UndergroundMazeRevive,
	UndergroundMazeEnterForUndergroundMazeRevive,
	ContinentExitForArtifactRoomEnter,
	ArtifactRoomEnter,
	ArtifactRoomAbandon,
	ArtifactRoomExit,
	ArtifactRoomNextFloorChallenge,
	ArtifactRoomInit,
	ArtifactRoomSweep,
	ArtifactRoomSweepAccelerate,
	ArtifactRoomSweepStop,
	ArtifactRoomSweepComplete,
	MysteryBoxQuestAccept,
	MysteryBoxQuestComplete,
	MysteryBoxPickStart,
	SecretLetterQuestAccept,
	SecretLetterQuestComplete,
	SecretLetterPickStart,
	DimensionRaidQuestAccept,
	DimensionRaidQuestComplete,
	DimensionRaidInteractionStart,
	HolyWarQuestAccept,
	HolyWarQuestComplete,
	AchievementRewardReceive,
	AncientRelicMatchingStart,
	AncientRelicMatchingCancel,
	AncientRelicEnter,
	AncientRelicAbandon,
	AncientRelicExit,
	AncientRelicRevive,
	VipLevelRewardReceive,
	RankAcquire,
	RankRewardReceive,
	RankActiveSkillSelect,
	RankActiveSkillLevelUp,
	RankPassiveSkillLevelUp,
	HonorShopProductBuy,
	ServerBattlePowerRanking,
	ServerLevelRanking,
	ServerJobBattlePowerRanking,
	ServerGuildRanking,
	ServerCreatureCardRanking,
	ServerIllustratedBookRanking,
	NationBattlePowerRanking,
	NationExploitPointRanking,
	NationGuildRanking,
	ServerLevelRankingRewardReceive,
	DistortionScrollUse,
	DistortionCancel,
	CartGetOn,
	CartGetOff,
	CartAccelerate,
	CartPortalEnter,
	CartPortalExit,
	FieldOfHonorInfo,
	FieldOfHonorTopRankingList,
	ContinentExitForFieldOfHonorChallenge,
	FieldOfHonorChallenge,
	FieldOfHonorAbandon,
	FieldOfHonorExit,
	FieldOfHonorRankingRewardReceive,
	FieldOfHonorRankerInfo,
	GuildCreate,
	GuildList,
	GuildApply,
	GuildExit,
	GuildInvite,
	GuildNoticeSet,
	GuildDonate,
	GuildDailyRewardReceive,
	GuildMemberTabInfo,
	GuildMemberBanish,
	GuildAppoint,
	GuildMasterTransfer,
	GuildApplicationList,
	GuildApplicationAccept,
	GuildApplicationRefuse,
	GuildInvitationAccept,
	GuildInvitationRefuse,
	ContinentExitForGuildTerritoryEnter,
	GuildTerritoryEnter,
	GuildTerritoryExit,
	GuildTerritoryRevive,
	GuildTerritoryEnterForGuildTerritoryRevival,
	GuildBuildingLevelUp,
	GuildSkillLevelUp,
	SupplySupportQuestAccept,
	SupplySupportQuestComplete,
	SupplySupportQuestCartChange,
	GuildFarmQuestAccept,
	GuildFarmQuestComplete,
	GuildFarmQuestInteractionStart,
	GuildFarmQuestAbandon,
	GuildFoodWarehouseInfo,
	GuildFoodWarehouseStock,
	GuildFoodWarehouseCollect,
	GuildFoodWarehouseRewardReceive,
	GuildAltarDonate,
	GuildAltarSpellInjectionMissionStart,
	GuildAltarDefenseMissionStart,
	GuildAltarRewardReceive,
	GuildMissionQuestAccept,
	GuildMissionAccept,
	GuildMissionAbandon,
	GuildMissionTargetNpcInteract,
	GuildCall,
	GuildCallTransmission,
	ContinentEnterForGuildCallTransmission,
	GuildSupplySupportQuestAccept,
	GuildSupplySupportQuestComplete,
	GuildHuntingQuestAccept,
	GuildHuntingQuestAbandon,
	GuildHuntingQuestComplete,
	GuildHuntingDonate,
	GuildHuntingDonationRewardReceive,
	GuildDailyObjectiveNotice,
	GuildDailyObjectiveRewardReceive,
	GuildDailyObjectiveCompletionMemberList,
	GuildWeeklyObjectiveSet,
	GuildWeeklyObjectiveRewardReceive,
	NationNoblesseAppoint,
	NationNoblesseDismiss,
	NationWarDeclaration,
	NationWarJoin,
	ContinentEnterForNationWarJoin,
	NationWarRevive,
	ContinentEnterForNationWarRevive,
	NationWarTransmission,
	ContinentEnterForNationWarTransmission,
	NationWarNpcTransmission,
	ContinentEnterForNationWarNpcTransmission,
	NationWarInfo,
	NationWarCall,
	NationWarCallTransmission,
	ContinentEnterForNationWarCallTransmission,
	NationWarConvergingAttack,
	NationWarResult,
	NationWarHistory,
	NationCall,
	NationCallTransmission,
	ContinentEnterForNationCallTransmission,
	SoulCoveterMatchingStart,
	SoulCoveterMatchingCancel,
	SoulCoveterEnter,
	SoulCoveterAbandon,
	SoulCoveterExit,
	SoulCoveterRevive,
	IllustratedBookUse,
	IllustratedBookExplorationStepAcquire,
	IllustratedBookExplorationStepRewardReceive,
	SceneryQuestStart,
	AccomplishmentRewardReceive,
	AccomplishmentRewardReceiveAll,
	DisplayTitleSet,
	ActivationTitleSet,
	CreatureCardDisassemble,
	CreatureCardDisassembleAll,
	CreatureCardCompose,
	CreatureCardCollectionActivate,
	CreatureCardShopFixedProductBuy,
	CreatureCardShopRandomProductBuy,
	CreatureCardShopPaidRefresh,
	ContinentExitForEliteDungeonEnter,
	EliteDungeonEnter,
	EliteDungeonAbandon,
	EliteDungeonExit,
	EliteDungeonRevive,
	StaminaBuy,
	BattleSettingSet,
	ProofOfValorRefresh,
	ContinentExitForProofOfValorEnter,
	ProofOfValorEnter,
	ProofOfValorAbandon,
	ProofOfValorExit,
	ProofOfValorSweep,
	ProofOfValorBuffBoxAcquire,
	GroggyMonsterItemStealStart,
	NpcShopProductBuy,
	RookieGiftReceive,
	OpenGiftReceive,
	DailyQuestAccept,
	DailyQuestComplete,
	DailyQuestRefresh,
	DailyQuestMissionImmediatlyComplete,
	DailyQuestAbandon,
	WeeklyQuestRoundAccept,
	WeeklyQuestRoundImmediatlyComplete,
	WeeklyQuestTenRoundImmediatlyComplete,
	WeeklyQuestRoundRefresh,
	WeeklyQuestRoundMoveMissionComplete,
	ContinentExitForWisdomTempleEnter,
	WisdomTempleEnter,
	WisdomTempleExit,
	WisdomTempleAbandon,
	WisdomTempleColorMatchingObjectInteractionStart,
	WisdomTempleColorMatchingObjectCheck,
	WisdomTemplePuzzleRewardObjectInteractionStart,
	WisdomTempleSweep,
	Open7DayEventMissionRewardReceive,
	Open7DayEventProductBuy,
	Open7DayEventRewardReceive,
	RetrieveGold,
	RetrieveGoldAll,
	RetrieveDia,
	RetrieveDiaAll,
	RuinsReclaimMatchingStart,
	RuinsReclaimMatchingCancel,
	RuinsReclaimEnter,
	RuinsReclaimExit,
	RuinsReclaimAbandon,
	RuinsReclaimPortalEnter,
	RuinsReclaimRevive,
	RuinsReclaimMonsterTransformationCancelObjectInteractionStart,
	RuinsReclaimRewardObjectInteractionStart,
	TaskConsignmentStart,
	TaskConsignmentComplete,
	TaskConsignmentImmediatelyComplete,
	TrueHeroQuestAccept,
	TrueHeroQuestStepInteractionStart,
	TrueHeroQuestComplete,
	InfiniteWarMatchingStart,
	InfiniteWarMatchingCancel,
	InfiniteWarEnter,
	InfiniteWarExit,
	InfiniteWarAbandon,
	InfiniteWarRevive,
	InfiniteWarBuffBoxAcquire,
	LimitationGiftRewardReceive,
	WeekendRewardSelect,
	WeekendRewardReceive,
	WarehouseDeposit,
	WarehouseWithdraw,
	WarehouseSlotExtend,
	DiaShopProductBuy,
	FearAltarMatchingStart,
	FearAltarMatchingCancel,
	FearAltarEnter,
	FearAltarExit,
	FearAltarAbandon,
	FearAltarRevive,
	FearAltarHalidomElementalRewardReceive,
	FearAltarHalidomCollectionRewardReceive,
	SubQuestAccept,
	SubQuestComplete,
	SubQuestAbandon,
	WarMemoryMatchingStart,
	WarMemoryMatchingCancel,
	WarMemoryEnter,
	WarMemoryExit,
	WarMemoryAbandon,
	WarMemoryRevive,
	WarMemoryTransformationObjectInteractionStart,
	OrdealQuestComplete,
	OrdealQuestSlotComplete,
	ContinentExitForOsirisRoomEnter,
	OsirisRoomEnter,
	OsirisRoomExit,
	OsirisRoomAbandon,
	OsirisRoomMoneyBuffActivate,
	FriendList,
	FriendDelete,
	FriendApply,
	FriendApplicationAccept,
	FriendApplicationRefuse,
	HeroSearchForFriendApplication,
	BlacklistEntryAdd,
	BlacklistEntryDelete,
	BiographyStart,
	BiographyComplete,
	BiographyQuestAccept,
	BiographyQuestComplete,
	BiographyQuestMoveObjectiveComplete,
	BiographyQuestNpcConversationComplete,
	ContinentExitForBiographyQuestDungeonEnter,
	BiographyQuestDungeonEnter,
	BiographyQuestDungeonAbandon,
	BiographyQuestDungeonExit,
	BiographyQuestDungeonRevive,
	ItemLuckyShopFreePick,
	ItemLuckyShop1TimePick,
	ItemLuckyShop5TimePick,
	CreatureCardLuckyShopFreePick,
	CreatureCardLuckyShop1TimePick,
	CreatureCardLuckyShop5TimePick,
	BlessingQuestDeleteAll,
	BlessingQuestBlessingSend,
	BlessingRewardReceive,
	BlessingDeleteAll,
	OwnerProspectQuestRewardReceive,
	OwnerProspectQuestRewardReceiveAll,
	TargetProspectQuestRewardReceive,
	TargetProspectQuestRewardReceiveAll,
	DragonNestMatchingStart,
	DragonNestMatchingCancel,
	DragonNestEnter,
	DragonNestExit,
	DragonNestAbandon,
	DragonNestRevive,
	CreatureParticipate,
	CreatureParticipationCancel,
	CreatureCheer,
	CreatureCheerCancel,
	CreatureRear,
	CreatureRelease,
	CreatureInject,
	CreatureInjectionRetrieval,
	CreatureVary,
	CreatureAdditionalAttrSwitch,
	CreatureSkillSlotOpen,
	CreatureCompose,
	CreatureEggUse,
	PresentSend,
	PresentReply,
	ServerPresentPopularityPointRanking,
	NationWeeklyPresentPopularityPointRanking,
	NationWeeklyPresentPopularityPointRankingRewardReceive,
	ServerPresentContributionPointRanking,
	NationWeeklyPresentContributionPointRanking,
	NationWeeklyPresentContributionPointRankingRewardReceive,
	CostumeItemUse,
	CostumeEquip,
	CostumeUnequip,
	CostumeEffectApply,
	CostumeEnchant,
	CostumeCollectionShuffle,
	CostumeCollectionActivate,
	CreatureFarmQuestAccept,
	CreatureFarmQuestComplete,
	CreatureFarmQuestMissionMoveObjectiveComplete,
	GuildBlessingBuffStart,
	NationAllianceApply,
	NationAllianceApplicationAccept,
	NationAllianceApplicationCancel,
	NationAllianceApplicationReject,
	NationAllianceBreak,
	CashProductPurchaseStart,
	CashProductPurchaseCancel,
	CashProductPurchaseFail,
	CashProductPurchaseComplete,
	FirstChargeEventRewardReceive,
	RechargeEventRewardReceive,
	ChargeEventMissionRewardReceive,
	DailyChargeEventMissionRewardReceive,
	ConsumeEventMissionRewardReceive,
	DailyConsumeEventMissionRewardReceive,
	JobChangeQuestAccept,
	JobChangeQuestComplete,
	HeroJobChange,
	HeroAttrPotionUse,
	HeroAttrPotionUseAll,
	AnkouTombMatchingStart,
	AnkouTombMatchingCancel,
	AnkouTombEnter,
	AnkouTombExit,
	AnkouTombAbandon,
	AnkouTombRevive,
	AnkouTombMoneyBuffActivate,
	AnkouTombAdditionalRewardExpReceive,
	ConstellationEntryActivate,
	ConstellationStepOpen,
	ArtifactLevelUp,
	ArtifactEquip,
	TradeShipMatchingStart,
	TradeShipMatchingCancel,
	TradeShipEnter,
	TradeShipExit,
	TradeShipAbandon,
	TradeShipRevive,
	TradeShipMoneyBuffActivate,
	TradeShipAdditionalRewardExpReceive
}
