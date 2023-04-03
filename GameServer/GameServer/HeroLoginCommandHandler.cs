using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class HeroLoginCommandHandler : LobbyCommandHandler<HeroLoginCommandBody, HeroLoginResponseBody>
{
	public const short kResult_HeroNotExist = 101;

	public const short kResult_NotCreationCompleted = 102;

	private Guid m_id = Guid.Empty;

	private Hero m_hero;

	private DateTimeOffset m_currentTime = DateTimeOffset.MinValue;

	private DateTime m_currentDate = DateTime.MinValue.Date;

	private DateTime m_currentWeekStartDate = DateTime.MinValue.Date;

	private DateTime m_yesterday = DateTime.MinValue.Date;

	private DataRow m_drHero;

	private DataRowCollection m_drcHeroMainGears;

	private DataRowCollection m_drcHeroMainGearOptionAttrs;

	private DataRowCollection m_drcHeroMainGearRefinements;

	private DataRowCollection m_drcHeroSubGears;

	private DataRowCollection m_drcHeroSubGearSoulstoneSockets;

	private DataRowCollection m_drcHeroSubGearRuneSockets;

	private DataRowCollection m_drcInventorySlots;

	private DataRowCollection m_drcMails;

	private DataRowCollection m_drcMailAttachments;

	private DataRowCollection m_drcHeroSkills;

	private DataRow m_drCurrentHeroMainQuest;

	private DataRow m_drTreatOfFarmQuest;

	private DataRowCollection m_drcTreatOfFarmQuestMissions;

	private DataRow m_drHeroBountyHunterQuest;

	private int m_nBountyHunterQuestDailyCount;

	private DataRowCollection m_drcHeroLevelUpRewards;

	private DataRowCollection m_drcHeroAccessRewards;

	private DataRowCollection m_drcHeroSeriesMissions;

	private DataRowCollection m_drcHeroTodayMissions;

	private DataRowCollection m_drcHeroMainQuestDungeonRewards;

	private DataRowCollection m_drcHeroMounts;

	private DataRowCollection m_drcHeroMountGears;

	private DataRowCollection m_drcHeroMountGearOptionAttrs;

	private DataRowCollection m_drcEquippedMountGearSlots;

	private DataRowCollection m_drcHeroWings;

	private DataRowCollection m_drcHeroWingEnchants;

	private DataRowCollection m_drcHeroWingMemoryPieceSlots;

	private DataRowCollection m_drcStoryDungeonClears;

	private DataRowCollection m_drcStoryDungeonEnterCountsOfDate;

	private DataRowCollection m_drcExpDungeonClearDifficulties;

	private DataRow m_drExpDungeonEnterCountOfDate;

	private DataRowCollection m_drcGoldDungeonClearDifficulties;

	private DataRow m_drGoldDungeonEnterCountOfDate;

	private int m_nMysteryBoxQuestStartCount;

	private DataRow m_drPerformingMysteryBoxQuest;

	private int m_nSecretLetterQuestStartCount;

	private DataRow m_drPerformingSecretLetterQuest;

	private int m_nDimensionRaidQuestStartCount;

	private DataRow m_drPerformingDimensionRaidQuest;

	private DataRowCollection m_drcStartedHolyWarQuests;

	private DataRow m_drPerformingHolyWarQuest;

	private DataRowCollection m_drcHeroTodayTasks;

	private DataRow m_drAncientRelicEnterCountOfDate;

	private DataRowCollection m_drcFieldOfHonorTargets;

	private DataRow m_drFieldOfHonorEnterCountOfDate;

	private DataRowCollection m_drcFieldOfHonorHistories;

	private int m_nDailyGuildApplicationCount;

	private int m_nDailyGuildFarmQuestStartCount;

	private DataRow m_drPerformingGuildFarmQuest;

	private DataRowCollection m_drcGuildSkillLevels;

	private DataRow m_drHeroGuildMissionQuest;

	private DataRowCollection m_drcHeroGuildMissionQuestMissions;

	private DataRow m_drHeroGuildHuntingQuest;

	private int m_nDailyGuildHuntingQuestCount;

	private int m_nSupplySupportQuestStartCount;

	private DataRow m_drSupplySupportQuest;

	private DataRowCollection m_drcSupplySupportQuestVisitedWayPoints;

	private DataRow m_drNationWarMember;

	private DataRow m_drSoulCoveterEnterCountOfWeekly;

	private DataRowCollection m_drcHeroIllustratedBooks;

	private DataRowCollection m_drcHeroSceneryQuests;

	private int m_nAccSoulCoveterPlayCount;

	private DataRowCollection m_drcHeroAccomplishmentRewards;

	private DataRowCollection m_drcHeroTitles;

	private DataRowCollection m_drcHeroCreatureCards;

	private DataRowCollection m_drcHeroCreatureCardCollections;

	private DataRowCollection m_drcHeroCreatureCardFixedProductBuys;

	private DataRowCollection m_drcHeroCreatureCardRandomProducts;

	private DataRowCollection m_drcHeroEliteMonsterKills;

	private DataRow m_drEliteDungeonEnterCountOfDate;

	private int m_nProofOfValorClearedCount;

	private DataRow m_drProofOfValorEnterCountOfDate;

	private DataRow m_drLastHeroProofOfValorInstance;

	private DataRowCollection m_drcHeroNpcShopProducts;

	private DataRowCollection m_drcHeroRankActiveSkills;

	private DataRowCollection m_drcHeroRankPassiveSkills;

	private DataRowCollection m_drcHeroOpenGiftRewards;

	private int m_nDailyQuestAcceptionCount;

	private DataRowCollection m_drcHeroDailyQuests;

	private DataRow m_drWeeklyQuest;

	private DataRow m_drWisdomTempleEnterCountOfDate;

	private DataRowCollection m_drcOpen7DayEventMissions;

	private DataRowCollection m_drcOpen7DayEventProducts;

	private DataRowCollection m_drcOpen7DayEventProgressCounts;

	private DataRowCollection m_drcPrevRetrivalProgressCounts;

	private DataRowCollection m_drcCurrRetrivalProgressCounts;

	private DataRowCollection m_drcRetrievals;

	private DataRowCollection m_drcCurrentTaskConsignments;

	private DataRowCollection m_drcTaskConsignmentCountsOfDate;

	private DataRow m_drTrueHeroQuest;

	private DataRowCollection m_drcLimitationGiftRewards;

	private DataRow m_drWeekendReward;

	private DataRowCollection m_drcWarehouseSlots;

	private DataRowCollection m_drcDiaShopProducts;

	private DataRowCollection m_drcDiaShopProductsAccBuyCounts;

	private DataRowCollection m_drcHeroFearAltarHalidoms;

	private DataRowCollection m_drcHeroFearAltarHalidomElementalRewards;

	private DataRowCollection m_drcSubQuests;

	private DataRow m_drOrdealQuest;

	private DataRowCollection m_drcOrdealQuestMissions;

	private DataRowCollection m_drcFriends;

	private DataRowCollection m_drcTempFriends;

	private DataRowCollection m_drcBlacklistEntries;

	private DataRowCollection m_drcDeadRecords;

	private DataRowCollection m_drcBiographies;

	private DataRowCollection m_drcBiographyQuests;

	private DataRowCollection m_drcOwnerProspectQuests;

	private DataRowCollection m_drcTargetProspectQuests;

	private DataRowCollection m_drcCreatures;

	private DataRowCollection m_drcCreatureBaseAttrs;

	private DataRowCollection m_drcCreatureAdditionalAttrs;

	private DataRowCollection m_drcCreatureSkills;

	private DataRow m_drWeeklyPresentPopularityPoint;

	private DataRow m_drWeeklyPresentContributionPoint;

	private DataRowCollection m_drcHeroCostumes;

	private DataRow m_drCreatureFarmQuest;

	private int m_nCreatureFarmQuestAcceptionCount;

	private DataRow m_drJobChangeQuest;

	private DataRowCollection m_drcPotionAttrs;

	private DataRowCollection m_drcHeroAnkouTombBestRecords;

	private DataRowCollection m_drcConstellation;

	private DataRowCollection m_drcHeroTradeShipBestRecords;

	private DataRowCollection m_drcTimeDesignationEvents;

	protected override bool globalLockRequired => true;

	protected override void HandleLobbyCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "비디가 null입니다.");
		}
		m_id = (Guid)m_body.id;
		if (m_id == Guid.Empty)
		{
			throw new CommandHandleException(1, "영웅ID가 유효하지 않습니다.");
		}
		m_currentTime = DateTimeUtil.currentTime;
		m_currentDate = m_currentTime.Date;
		m_currentWeekStartDate = DateTimeUtil.GetWeekStartDate(m_currentTime);
		m_yesterday = m_currentDate.AddDays(-1.0);
		m_hero = new Hero(m_myAccount, m_id, m_currentTime, null);
		m_hero.BeginLogIn();
		m_myAccount.currentHero = m_hero;
		Cache.instance.AddHero(m_hero);
		try
		{
			SFRunnableQueuingWork work = RunnableQueuingWorkUtil.CreateHeroWork(m_id);
			work.runnable = new SFAction(GetHeroInfo);
			RunWork(work);
		}
		catch (Exception ex)
		{
			m_hero.LogOut();
			throw new CommandHandleException(1, ex.Message, ex);
		}
	}

	private void GetHeroInfo()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			m_drHero = GameDac.Hero(conn, null, m_id);
			if (m_drHero == null)
			{
				throw new CommandHandleException(101, "해당 영웅이 존재하지 않습니다. id = " + m_id);
			}
			Guid accountId = (Guid)m_drHero["accountId"];
			bool bCreated = Convert.ToBoolean(m_drHero["created"]);
			if (accountId != m_myAccount.id)
			{
				throw new CommandHandleException(1, "나의 영웅이 아닙니다. id = " + m_id);
			}
			if (!bCreated)
			{
				throw new CommandHandleException(102, "영웅 생성이 완료되지 않았습니다. id = " + m_id);
			}
			m_drcHeroMainGears = GameDac.HeroMainGears(conn, null, m_id);
			m_drcHeroMainGearOptionAttrs = GameDac.HeroMainGearOptionAttrs(conn, null, m_id);
			m_drcHeroMainGearRefinements = GameDac.HeroMainGearRefinementAttrs(conn, null, m_id);
			m_drcHeroSubGears = GameDac.HeroSubGears(conn, null, m_id);
			m_drcHeroSubGearSoulstoneSockets = GameDac.HeroSubGearSoulstoneSockets(conn, null, m_id);
			m_drcHeroSubGearRuneSockets = GameDac.HeroSubGearRuneSockets(conn, null, m_id);
			m_drcInventorySlots = GameDac.InventorySlots(conn, null, m_id);
			m_drcMails = GameDac.Mails(conn, null, m_id, m_currentTime);
			m_drcMailAttachments = GameDac.MailAttachments(conn, null, m_id, m_currentTime);
			m_drcHeroSkills = GameDac.HeroSkills(conn, null, m_id);
			m_drCurrentHeroMainQuest = GameDac.HeroMainQuest_Current(conn, null, m_id);
			m_drTreatOfFarmQuest = GameDac.TreatOfFarmQuest(conn, null, m_id, m_currentDate);
			if (m_drTreatOfFarmQuest != null)
			{
				Guid treatOfFamrQuestId = (Guid)m_drTreatOfFarmQuest["instanceId"];
				m_drcTreatOfFarmQuestMissions = GameDac.TreatOfFarmQuestMissions(conn, null, treatOfFamrQuestId);
			}
			m_nMysteryBoxQuestStartCount = GameDac.MysteryBoxQuestStartCount(conn, null, m_id, m_currentDate);
			m_drPerformingMysteryBoxQuest = GameDac.PerformingMysteryBoxQuest(conn, null, m_id);
			m_nSecretLetterQuestStartCount = GameDac.SecretLetterQuestStartCount(conn, null, m_id, m_currentDate);
			m_drPerformingSecretLetterQuest = GameDac.PerformingSecretLetterQuest(conn, null, m_id);
			m_nDimensionRaidQuestStartCount = GameDac.DimensionRaidQuestStartCount(conn, null, m_id, m_currentDate);
			m_drPerformingDimensionRaidQuest = GameDac.PerformingDimensionRaidQuest(conn, null, m_id);
			m_drcStartedHolyWarQuests = GameDac.StartedHolyWarQuests(conn, null, m_id, m_currentDate);
			m_drPerformingHolyWarQuest = GameDac.PerformingHolyWarQuest(conn, null, m_id);
			m_drHeroBountyHunterQuest = GameDac.BountyHunterQuest_OnStart(conn, null, m_id);
			m_nBountyHunterQuestDailyCount = GameDac.BountyHunterQuest_DailyCount(conn, null, m_id, m_currentDate);
			m_drcHeroLevelUpRewards = GameDac.HeroLevelUpRewards(conn, null, m_id);
			m_drcHeroAccessRewards = GameDac.HeroAccessRewards(conn, null, m_id, m_currentDate);
			m_drcHeroSeriesMissions = GameDac.HeroSeriesMissions(conn, null, m_id);
			m_drcHeroTodayMissions = GameDac.HeroTodayMissions(conn, null, m_id, m_currentDate);
			m_drcHeroMainQuestDungeonRewards = GameDac.HeroMainQuestDungeonRewards(conn, null, m_id);
			m_drcHeroMounts = GameDac.HeroMounts(conn, null, m_id);
			m_drcHeroMountGears = GameDac.HeroMountGears(conn, null, m_id);
			m_drcHeroMountGearOptionAttrs = GameDac.HeroMountGearOptionAttrs(conn, null, m_id);
			m_drcEquippedMountGearSlots = GameDac.EquippedHeroMountGearSlots(conn, null, m_id);
			m_drcHeroWings = GameDac.HeroWings(conn, null, m_id);
			m_drcHeroWingEnchants = GameDac.HeroWingEnchants(conn, null, m_id);
			m_drcHeroWingMemoryPieceSlots = GameDac.HeroWingMemoryPieceSlots(conn, null, m_id);
			m_drcStoryDungeonClears = GameDac.StoryDungeonClears(conn, null, m_id);
			m_drcStoryDungeonEnterCountsOfDate = GameDac.StoryDungeonEnterCountsOfDate(conn, null, m_id, m_currentDate);
			m_drcExpDungeonClearDifficulties = GameDac.ExpDungeonClearDifficulties(conn, null, m_id);
			m_drExpDungeonEnterCountOfDate = GameDac.ExpDungeonEnterCountOfDate(conn, null, m_id, m_currentDate);
			m_drcGoldDungeonClearDifficulties = GameDac.GoldDungeonClearDifficulties(conn, null, m_id);
			m_drGoldDungeonEnterCountOfDate = GameDac.GoldDungeonEnterCountOfDate(conn, null, m_id, m_currentDate);
			m_drcHeroTodayTasks = GameDac.HeroTodayTasks(conn, null, m_id, m_currentDate);
			m_drAncientRelicEnterCountOfDate = GameDac.AncientRelicEnterCountOfDate(conn, null, m_id, m_currentDate);
			m_drcFieldOfHonorTargets = GameDac.FieldOfHonorTargets(conn, null, m_id);
			m_drFieldOfHonorEnterCountOfDate = GameDac.FieldOfHonorEnterCountOfDate(conn, null, m_id, m_currentDate);
			m_drcFieldOfHonorHistories = GameDac.FieldOfHonorHistories(conn, null, m_id, 3);
			m_nDailyGuildApplicationCount = GameDac.GuildApplicationCountOfDate(conn, null, m_id, m_currentDate);
			m_nDailyGuildFarmQuestStartCount = GameDac.HeroGuildFarmQuestCountOfDate(conn, null, m_id, m_currentDate);
			m_drPerformingGuildFarmQuest = GameDac.PerformingHeroGuildFarmQuest(conn, null, m_id);
			m_drcGuildSkillLevels = GameDac.GuildSkillLevels(conn, null, m_id);
			m_drHeroGuildMissionQuest = GameDac.HeroGuildMissionQuest(conn, null, m_id, m_currentDate);
			if (m_drHeroGuildMissionQuest != null)
			{
				Guid instanceId2 = (Guid)m_drHeroGuildMissionQuest["instanceId"];
				m_drcHeroGuildMissionQuestMissions = GameDac.HeroGuildMissionQuestMissions(conn, null, instanceId2);
			}
			m_drHeroGuildHuntingQuest = GameDac.HeroGuildHuntingQuest(conn, null, m_id);
			m_nDailyGuildHuntingQuestCount = GameDac.HeroGuildHuntingQuest_DateCount(conn, null, m_id, m_currentDate);
			m_nSupplySupportQuestStartCount = GameDac.SupplySupportQuestCountOfDate(conn, null, m_id, m_currentDate);
			m_drSupplySupportQuest = GameDac.SupplySupportQuest_OnStart(conn, null, m_id);
			if (m_drSupplySupportQuest != null)
			{
				Guid instanceId = SFDBUtil.ToGuid(m_drSupplySupportQuest["instanceId"], Guid.Empty);
				m_drcSupplySupportQuestVisitedWayPoints = GameDac.SupplySupportQuestVisitedWayPoints(conn, null, instanceId);
			}
			m_drNationWarMember = GameDac.NationWarMember(conn, null, m_id, m_currentDate);
			m_drSoulCoveterEnterCountOfWeekly = GameDac.SoulCoveterEnterCountOfWeekly(conn, null, m_id, DateTimeUtil.GetWeekStartDate(m_currentDate));
			m_drcHeroIllustratedBooks = GameDac.HeroIllustratedBooks(conn, null, m_id);
			m_drcHeroSceneryQuests = GameDac.HeroSceneryQuests(conn, null, m_id);
			m_nAccSoulCoveterPlayCount = GameDac.SoulCoverterInstanceMember_AccEnterCount(conn, null, m_id);
			m_drcHeroAccomplishmentRewards = GameDac.HeroAccomplishmentRewards(conn, null, m_id);
			m_drcHeroTitles = GameDac.HeroTitles(conn, null, m_id);
			m_drcHeroCreatureCards = GameDac.HeroCreatureCards(conn, null, m_id);
			m_drcHeroCreatureCardCollections = GameDac.HeroCreatureCardCollections(conn, null, m_id);
			m_drcHeroCreatureCardFixedProductBuys = GameDac.HeroCreatureCardShopFixedProductBuys(conn, null, m_id);
			m_drcHeroCreatureCardRandomProducts = GameDac.HeroCreatureCardShopRandomProducts(conn, null, m_id);
			m_drcHeroEliteMonsterKills = GameDac.HeroEliteMonsterKills(conn, null, m_id);
			m_drEliteDungeonEnterCountOfDate = GameDac.EliteDungeonEnterCountOfDate(conn, null, m_id, m_currentDate);
			m_nProofOfValorClearedCount = GameDac.ProofOfValorClearedCount(conn, null, m_id);
			m_drProofOfValorEnterCountOfDate = GameDac.ProofOfValorEnterCountOfDate(conn, null, m_id, m_currentDate);
			m_drLastHeroProofOfValorInstance = GameDac.LastHeroProofOfValorInstance(conn, null, m_id);
			m_drcHeroNpcShopProducts = GameDac.HeroNpcShopProducts(conn, null, m_id);
			m_drcHeroRankActiveSkills = GameDac.HeroRankActiveSkills(conn, null, m_id);
			m_drcHeroRankPassiveSkills = GameDac.HeroRankPassiveSkills(conn, null, m_id);
			m_drcHeroOpenGiftRewards = GameDac.HeroOpenGiftRewards(conn, null, m_id);
			m_nDailyQuestAcceptionCount = GameDac.HeroDailyQuest_Count(conn, null, m_id, m_currentDate);
			m_drcHeroDailyQuests = GameDac.HeroDailyQuests(conn, null, m_id);
			m_drWeeklyQuest = GameDac.HeroWeeklyQuest(conn, null, m_id);
			m_drWisdomTempleEnterCountOfDate = GameDac.WisdomTempleEnterCountOfDate(conn, null, m_id, m_currentDate);
			m_drcOpen7DayEventMissions = GameDac.HeroOpen7DayEventMissions(conn, null, m_id);
			m_drcOpen7DayEventProducts = GameDac.HeroOpen7DayEventProducts(conn, null, m_id);
			m_drcOpen7DayEventProgressCounts = GameDac.HeroOpen7DayEventProgressCounts(conn, null, m_id);
			m_drcPrevRetrivalProgressCounts = GameDac.HeroRetrievalProgressCounts(conn, null, m_id, m_yesterday);
			m_drcCurrRetrivalProgressCounts = GameDac.HeroRetrievalProgressCounts(conn, null, m_id, m_currentDate);
			m_drcRetrievals = GameDac.HeroRetrievals(conn, null, m_id, m_currentDate);
			m_drcCurrentTaskConsignments = GameDac.HeroTaskConsignments_Current(conn, null, m_id);
			m_drcTaskConsignmentCountsOfDate = GameDac.HeroTaskConsignmentCountsOfDate(conn, null, m_id, m_currentDate);
			m_drTrueHeroQuest = GameDac.HeroTrueHeroQuest(conn, null, m_id);
			m_drcLimitationGiftRewards = GameDac.HeroLimitationGiftRewards(conn, null, m_id, m_currentDate);
			m_drWeekendReward = GameDac.HeroWeekendReward(conn, null, m_id);
			m_drcWarehouseSlots = GameDac.WarehouseSlots(conn, null, m_id);
			m_drcDiaShopProducts = GameDac.HeroDiaShopProducts(conn, null, m_id, m_currentDate);
			m_drcDiaShopProductsAccBuyCounts = GameDac.HeroDiaShopProducts_AccBuyCount(conn, null, m_id);
			m_drcHeroFearAltarHalidoms = GameDac.HeroFearAltarHalidoms(conn, null, m_hero.id, DateTimeUtil.GetWeekStartDate(m_currentDate));
			m_drcHeroFearAltarHalidomElementalRewards = GameDac.HeroFearAltarHalidomElementalRewards(conn, null, m_hero.id, DateTimeUtil.GetWeekStartDate(m_currentDate));
			m_drcSubQuests = GameDac.HeroSubQuests(conn, null, m_id);
			m_drOrdealQuest = GameDac.HeroOrdealQuest(conn, null, m_id);
			if (m_drOrdealQuest != null)
			{
				int nOrdealQuestNo = Convert.ToInt32(m_drOrdealQuest["questNo"]);
				m_drcOrdealQuestMissions = GameDac.HeroOrdealQuestMissions(conn, null, m_id, nOrdealQuestNo);
			}
			m_drcFriends = GameDac.Friends(conn, null, m_id);
			m_drcTempFriends = GameDac.TempFriends(conn, null, m_id);
			m_drcBlacklistEntries = GameDac.BlacklistEntries(conn, null, m_id);
			m_drcDeadRecords = GameDac.DeadRecords(conn, null, m_id);
			m_drcBiographies = GameDac.HeroBiographies(conn, null, m_id);
			m_drcBiographyQuests = GameDac.HeroBiographyQuests(conn, null, m_id);
			m_drcOwnerProspectQuests = GameDac.HeroProspectQuestsOfHero_Owner(conn, null, m_id);
			m_drcTargetProspectQuests = GameDac.HeroProspectQuestsOfHero_Target(conn, null, m_id);
			m_drcCreatures = GameDac.HeroCreatures(conn, null, m_id);
			m_drcCreatureBaseAttrs = GameDac.HeroCreatureBaseAttrs(conn, null, m_id);
			m_drcCreatureAdditionalAttrs = GameDac.HeroCreatureAdditionalAttrs(conn, null, m_id);
			m_drcCreatureSkills = GameDac.HeroCreatureSkills(conn, null, m_id);
			m_drWeeklyPresentPopularityPoint = GameDac.HeroWeeklyPresentPopularityPoint(conn, null, m_id, m_currentWeekStartDate);
			m_drWeeklyPresentContributionPoint = GameDac.HeroWeeklyPresentContributionPoint(conn, null, m_id, m_currentWeekStartDate);
			m_drcHeroCostumes = GameDac.HeroCostumes(conn, null, m_id);
			m_drCreatureFarmQuest = GameDac.HeroCreatureFarmQuest(conn, null, m_id);
			m_nCreatureFarmQuestAcceptionCount = GameDac.HeroCreatureFarmQuest_AcceptionCount(conn, null, m_id, m_currentDate);
			m_drJobChangeQuest = GameDac.HeroJobChangeQuest(conn, null, m_id);
			m_drcPotionAttrs = GameDac.HeroPotionAttrs(conn, null, m_id);
			m_drcHeroAnkouTombBestRecords = GameDac.HeroAnkouTombBestRecords(conn, null, m_id);
			m_drcConstellation = GameDac.HeroConstellationSteps(conn, null, m_id);
			m_drcHeroTradeShipBestRecords = GameDac.HeroTradeShipBestRecords(conn, null, m_id);
			m_drcTimeDesignationEvents = GameDac.HeroTimeDesignationEventRewards(conn, null, m_id);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	protected override void OnWork_Success(SFWork work)
	{
		try
		{
			base.OnWork_Success(work);
			CompleteLogIn();
		}
		catch (Exception ex)
		{
			m_hero.LogOut();
			throw new CommandHandleException(1, ex.Message, ex);
		}
	}

	protected override void OnWork_Error(SFWork work, Exception error)
	{
		base.OnWork_Error(work, error);
		m_hero.LogOut();
	}

	private void CompleteLogIn()
	{
		m_hero.CompleteLogIn(m_currentTime, m_drHero, m_drcHeroMainGears, m_drcHeroMainGearOptionAttrs, m_drcHeroMainGearRefinements, m_drcHeroSubGears, m_drcHeroSubGearSoulstoneSockets, m_drcHeroSubGearRuneSockets, m_drcInventorySlots, m_drcMails, m_drcMailAttachments, m_drcHeroSkills, m_drCurrentHeroMainQuest, m_drTreatOfFarmQuest, m_drcTreatOfFarmQuestMissions, m_drHeroBountyHunterQuest, m_nBountyHunterQuestDailyCount, m_drcHeroLevelUpRewards, m_drcHeroAccessRewards, m_drcHeroSeriesMissions, m_drcHeroTodayMissions, m_drcHeroMainQuestDungeonRewards, m_drcHeroMounts, m_drcHeroMountGears, m_drcHeroMountGearOptionAttrs, m_drcEquippedMountGearSlots, m_drcHeroWings, m_drcHeroWingEnchants, m_drcHeroWingMemoryPieceSlots, m_drcStoryDungeonClears, m_drcStoryDungeonEnterCountsOfDate, m_drcExpDungeonClearDifficulties, m_drExpDungeonEnterCountOfDate, m_drcGoldDungeonClearDifficulties, m_drGoldDungeonEnterCountOfDate, m_nMysteryBoxQuestStartCount, m_drPerformingMysteryBoxQuest, m_nSecretLetterQuestStartCount, m_drPerformingSecretLetterQuest, m_nDimensionRaidQuestStartCount, m_drPerformingDimensionRaidQuest, m_drcStartedHolyWarQuests, m_drPerformingHolyWarQuest, m_drcHeroTodayTasks, m_drAncientRelicEnterCountOfDate, m_drcFieldOfHonorTargets, m_drFieldOfHonorEnterCountOfDate, m_drcFieldOfHonorHistories, m_nDailyGuildApplicationCount, m_nDailyGuildFarmQuestStartCount, m_drPerformingGuildFarmQuest, m_drcGuildSkillLevels, m_drHeroGuildMissionQuest, m_drcHeroGuildMissionQuestMissions, m_drHeroGuildHuntingQuest, m_nDailyGuildHuntingQuestCount, m_nSupplySupportQuestStartCount, m_drSupplySupportQuest, m_drcSupplySupportQuestVisitedWayPoints, m_drNationWarMember, m_drSoulCoveterEnterCountOfWeekly, m_drcHeroIllustratedBooks, m_drcHeroSceneryQuests, m_nAccSoulCoveterPlayCount, m_drcHeroAccomplishmentRewards, m_drcHeroTitles, m_drcHeroCreatureCards, m_drcHeroCreatureCardCollections, m_drcHeroCreatureCardFixedProductBuys, m_drcHeroCreatureCardRandomProducts, m_drcHeroEliteMonsterKills, m_drEliteDungeonEnterCountOfDate, m_nProofOfValorClearedCount, m_drProofOfValorEnterCountOfDate, m_drLastHeroProofOfValorInstance, m_drcHeroNpcShopProducts, m_drcHeroRankActiveSkills, m_drcHeroRankPassiveSkills, m_drcHeroOpenGiftRewards, m_nDailyQuestAcceptionCount, m_drcHeroDailyQuests, m_drWeeklyQuest, m_drWisdomTempleEnterCountOfDate, m_drcOpen7DayEventMissions, m_drcOpen7DayEventProducts, m_drcOpen7DayEventProgressCounts, m_drcPrevRetrivalProgressCounts, m_drcCurrRetrivalProgressCounts, m_drcRetrievals, m_drcCurrentTaskConsignments, m_drcTaskConsignmentCountsOfDate, m_drTrueHeroQuest, m_drcLimitationGiftRewards, m_drWeekendReward, m_drcWarehouseSlots, m_drcDiaShopProducts, m_drcDiaShopProductsAccBuyCounts, m_drcHeroFearAltarHalidoms, m_drcHeroFearAltarHalidomElementalRewards, m_drcSubQuests, m_drOrdealQuest, m_drcOrdealQuestMissions, m_drcFriends, m_drcTempFriends, m_drcBlacklistEntries, m_drcDeadRecords, m_drcBiographies, m_drcBiographyQuests, m_drcOwnerProspectQuests, m_drcTargetProspectQuests, m_drcCreatures, m_drcCreatureBaseAttrs, m_drcCreatureAdditionalAttrs, m_drcCreatureSkills, m_drWeeklyPresentPopularityPoint, m_drWeeklyPresentContributionPoint, m_drcHeroCostumes, m_drCreatureFarmQuest, m_nCreatureFarmQuestAcceptionCount, m_drJobChangeQuest, m_drcPotionAttrs, m_drcHeroAnkouTombBestRecords, m_drcConstellation, m_drcHeroTradeShipBestRecords, m_drcTimeDesignationEvents);
		Location targetLocation = null;
		int nTargetLocationParam = 0;
		Vector3 targetPosition = Vector3.zero;
		float fTargetRotationY = 0f;
		Resource res = Resource.instance;
		targetLocation = m_hero.lastLocation;
		nTargetLocationParam = m_hero.lastLocationParam;
		targetPosition = m_hero.lastPosition;
		fTargetRotationY = m_hero.lastYRotation;
		if (targetLocation != null && targetLocation.locationType != 0 && targetLocation.locationType != LocationType.UndergroundMaze && targetLocation.locationType != LocationType.GuildTerritory)
		{
			m_hero.Revive(bSendEvent: false);
		}
		if (targetLocation != null)
		{
			switch (targetLocation.locationType)
			{
			case LocationType.GuildTerritory:
				if (m_hero.guildMember == null)
				{
					targetLocation = m_hero.previousContinent;
					nTargetLocationParam = m_hero.previousNationId;
					targetPosition = m_hero.previousPosition;
					fTargetRotationY = m_hero.previousYRotation;
				}
				break;
			default:
				targetLocation = m_hero.previousContinent;
				nTargetLocationParam = m_hero.previousNationId;
				targetPosition = m_hero.previousPosition;
				fTargetRotationY = m_hero.previousYRotation;
				break;
			case LocationType.Continent:
			case LocationType.UndergroundMaze:
				break;
			}
		}
		else
		{
			targetLocation = m_hero.previousContinent;
			nTargetLocationParam = m_hero.previousNationId;
			targetPosition = m_hero.previousPosition;
			fTargetRotationY = m_hero.previousYRotation;
		}
		if (targetLocation == null)
		{
			targetLocation = res.GetContinent(res.startContinentId);
			nTargetLocationParam = m_hero.nationId;
			targetPosition = res.SelectStartPosition();
			fTargetRotationY = res.SelectYRotation();
		}
		m_hero.placeEntranceParam = new HeroInitEnterParam(targetLocation, nTargetLocationParam, targetPosition, fTargetRotationY);
		HeroLoginResponseBody resBody = new HeroLoginResponseBody();
		resBody.time = (DateTimeOffset)m_currentTime;
		resBody.date = (DateTime)m_currentDate;
		resBody.serverOpenDate = (DateTime)Cache.instance.serverOpenDate;
		resBody.serverMaxLevel = Cache.instance.serverMaxLevel;
		NationInstance nationInst = m_hero.nationInst;
		resBody.nationId = nationInst.nationId;
		resBody.secretLetterQuestTargetNationId = nationInst.secretLetterQuestTargetNationId;
		resBody.nationInsts = Cache.instance.GetPDNationInstances().ToArray();
		resBody.nationPowerRankings = Cache.instance.GetPDNationPowerRankings().ToArray();
		resBody.nationAlliances = Cache.instance.GetPDNationAlliances(m_currentTime).ToArray();
		int nNationId = nationInst.nationId;
		resBody.nationAllianceApplications = Cache.instance.GetPDNationAllianceApplicationsOfNation(nNationId).ToArray();
		resBody.id = (Guid)m_hero.id;
		resBody.name = m_hero.name;
		resBody.jobId = m_hero.jobId;
		resBody.level = m_hero.level;
		resBody.exp = m_hero.exp;
		resBody.maxHP = m_hero.realMaxHP;
		resBody.hp = m_hero.hp;
		resBody.ownDia = m_hero.ownDia;
		resBody.unOwnDia = m_hero.unOwnDia;
		resBody.gold = m_hero.gold;
		resBody.honorPoint = m_hero.honorPoint;
		resBody.vipPoint = m_hero.totalVipPoint;
		resBody.receivedVipLevelRewards = m_myAccount.receivedVipLevelRewards.ToArray();
		resBody.lak = m_hero.lak;
		resBody.stamina = m_hero.stamina;
		resBody.staminaAutoRecoveryRemainingTime = m_hero.GetStaminaRecoveryRemainingTime(m_currentTime);
		resBody.dailyStaminaBuyCount = m_hero.dailyStaminaBuyCount.value;
		resBody.rankNo = m_hero.rankNo;
		resBody.rankRewardReceivedDate = (DateTime)m_hero.rankRewardReceivedDate;
		resBody.rankRewardReceivedRankNo = m_hero.rankRewardReceivedRankNo;
		resBody.mainGears = m_hero.GetPDFullHeroMainGears().ToArray();
		resBody.equippedWeaponId = (Guid)((m_hero.equippedWeapon != null) ? m_hero.equippedWeapon.id : Guid.Empty);
		resBody.equippedArmorId = (Guid)((m_hero.equippedArmor != null) ? m_hero.equippedArmor.id : Guid.Empty);
		resBody.subGears = m_hero.GetPDFullHeroSubGears().ToArray();
		resBody.skills = m_hero.GetPDHeroSkills().ToArray();
		resBody.mounts = m_hero.GetPDHeroMounts().ToArray();
		resBody.equippedMountId = m_hero.equippedMountId;
		resBody.bIsRiding = m_hero.isRiding;
		resBody.mountGears = m_hero.GetPDHeroMountGears().ToArray();
		resBody.equippedMountGears = (Guid[])(object)m_hero.GetPDEquippedMountGearSlots().ToArray();
		resBody.paidInventorySlotCount = m_hero.paidInventorySlotCount;
		resBody.placedInventorySlots = m_hero.GetPlacedPDInventorySlots().ToArray();
		resBody.mails = m_hero.GetPDMails().ToArray();
		resBody.initEntranceLocationId = targetLocation.locationId;
		resBody.initEntranceLocationParam = nTargetLocationParam;
		resBody.previousContinentId = m_hero.previousContinentId;
		resBody.previousNationId = m_hero.previousNationId;
		resBody.mainGearEnchantDailyCount = m_hero.mainGearEnchantDailyCount.value;
		resBody.mainGearRefinementDailyCount = m_hero.mainGearRefinementDailyCount.value;
		resBody.currentMainQuest = m_hero.GetPDHeroMainQuest();
		resBody.treatOfFarmQuest = ((m_hero.treatOfFarmQuest != null) ? m_hero.treatOfFarmQuest.ToPDHeroTreatOfFarmQuest(m_currentTime) : null);
		resBody.bountyHunterQuest = ((m_hero.bountyHunterQuest != null) ? m_hero.bountyHunterQuest.ToPDHeroBountyHunterQuest() : null);
		resBody.bountyHunterDailyStartCount = m_hero.bountyHunterQuestDailyStartCount.value;
		resBody.fishingQuest = ((m_hero.fishingQuest != null) ? m_hero.fishingQuest.ToPDHeroFishingQuest() : null);
		resBody.fishingQuestDailyStartCount = m_hero.fishinQuestDailyStartCount.value;
		resBody.freeImmediateRevivalDailyCount = m_hero.freeImmediateRevivalDailyCount.value;
		resBody.paidImmediateRevivalDailyCount = m_hero.paidImmediateRevivalDailyCount.value;
		resBody.restTime = m_hero.restTime;
		resBody.seriesMissions = m_hero.GetPDHeroSeriesMissions().ToArray();
		resBody.todayMissions = m_hero.todayMissionCollection.GetPDHeroTodayMissions().ToArray();
		if (m_hero.partyMember != null)
		{
			resBody.party = m_hero.partyMember.party.ToPDParty(m_currentTime);
		}
		resBody.expPotionDailyUseCount = m_hero.expPotionDailyUseCount.value;
		resBody.receivedLevelUpRewards = m_hero.receivedLevelUpRewards.ToArray();
		resBody.receivedDailyAcessRewards = m_hero.GetAccessRewardEntryId().ToArray();
		resBody.dailyAccessTime = m_hero.dailyAccessTime;
		resBody.receivedAttendRewardDate = (DateTime)m_hero.dailyAttendReawrdDay.date;
		resBody.receivedAttendRewardDay = m_hero.dailyAttendReawrdDay.value;
		resBody.rewardedAttainmentEntryNo = m_hero.rewardedAttainmentEntryNo;
		resBody.mountGearRefinementDailyCount = m_hero.mountGearRefinementDailyCount.value;
		resBody.wings = HeroWing.ToPDHeroWings(m_hero.wings.Values).ToArray();
		resBody.equippedWingId = m_hero.equippedWingId;
		resBody.wingStep = m_hero.wingStep.step;
		resBody.wingLevel = m_hero.wingStepLevel.level;
		resBody.wingExp = m_hero.wingExp;
		resBody.wingParts = m_hero.GetPDHeroWingParts().ToArray();
		resBody.freeSweepDailyCount = m_hero.freeSweepDailyCount.value;
		resBody.storyDungeonPlays = m_hero.GetPDStoryDungeonPlays(m_currentDate).ToArray();
		resBody.storyDungeonClears = m_hero.GetPDStoryDungeonClears().ToArray();
		m_hero.RefreshDailyExpDungeonPlayCount(m_currentDate);
		resBody.expDungeonDailyPlayCount = m_hero.dailyExpDungeonPlayCount.value;
		resBody.expDungeonClearedDifficulties = m_hero.expDungeonClearDifficulties.ToArray();
		m_hero.RefreshDailyGoldDungeonPlayCount(m_currentDate);
		resBody.goldDungeonDailyPlayCount = m_hero.dailyGoldDungeonPlayCount.value;
		resBody.goldDungeonClearedDifficulties = m_hero.goldDungeonClearDifficulties.ToArray();
		resBody.undergroundMazeDailyPlayTime = m_hero.undergroundMazePlayTime;
		resBody.artifactRoomBestFloor = m_hero.artifactRoomBestFloor;
		resBody.artifactRoomCurrentFloor = m_hero.artifactRoomCurrentFloor;
		resBody.artifactRoomSweepRemainingTime = m_hero.GetArtifactRoomSweepRemainingTime(m_currentTime);
		resBody.artifactRoomSweepProgressFloor = m_hero.artifactRoomSweepProgressFloor;
		resBody.artifactRoomDailyInitCount = m_hero.artifactRoomDailyInitCount.value;
		m_hero.RefreshDailyAncientRelicPlayCount(m_currentDate);
		resBody.ancientRelicDailyPlayCount = m_hero.dailyAncientRelicPlayCount.value;
		m_hero.RefreshDailyFieldOfHonorPlayCount(m_currentDate);
		resBody.fieldOfHonorDailyPlayCount = m_hero.dailyFieldOfHonorPlayCount.value;
		resBody.dailyFieldOfHonorRankingNo = DailyFieldOfHonorRankingManager.instance.rankingNo;
		resBody.rewardedDailyFieldOfHonorRankingNo = m_hero.rewardedDailyFieldOfHonorRankingNo;
		resBody.dailyfieldOfHonorRanking = DailyFieldOfHonorRankingManager.instance.GetRankingOfHero(m_id)?.ranking ?? 0;
		resBody.mainGearEnchantLevelSetNo = m_hero.mainGearEnchantLevelSetNo;
		resBody.subGearSoulstoneLevelSetNo = m_hero.subGearSoulstoneLevelSetNo;
		resBody.expScrollDailyUseCount = m_hero.expScrollDailyUseCount.value;
		resBody.expScrollRemainingTime = m_hero.GetExpScrollRemainingTime(m_currentTime);
		resBody.expScrollItemId = m_hero.expScrollItemId;
		resBody.exploitPoint = m_hero.exploitPoint;
		resBody.dailyExploitPoint = m_hero.dailyExploitPoint.value;
		resBody.dailyMysteryBoxQuestStartCount = m_hero.dailyMysteryBoxQuestStartCount.value;
		resBody.mysteryBoxQuest = ((m_hero.mysteryBoxQuest != null) ? m_hero.mysteryBoxQuest.ToPDHeroMysteryBoxQuest() : null);
		resBody.dailySecretLetterQuestStartCount = m_hero.dailySecretLetterQuestStartCount.value;
		resBody.secretLetterQuest = ((m_hero.secretLetterQuest != null) ? m_hero.secretLetterQuest.ToPDHeroSecretLetterQuest() : null);
		resBody.dailyDimensionRaidQuestStartCount = m_hero.dailyDimensionRaidQuestStartCount.value;
		resBody.dimensionRaidQuest = ((m_hero.dimensionRaidQuest != null) ? m_hero.dimensionRaidQuest.ToPDHeroDimensionRaidQuest() : null);
		resBody.dailyHolyWarQuestStartSchedules = m_hero.dailyHolyWarQuestStartScheduleCollection.schedules.ToArray();
		resBody.holyWarQuest = ((m_hero.holyWarQuest != null) ? m_hero.holyWarQuest.ToPDHeroHolyWarQuest(m_currentTime) : null);
		HeroSupplySupportQuest supplySupportQuest = m_hero.supplySupportQuest;
		resBody.dailySupplySupportQuestStartCount = m_hero.dailySupplySupportQuestStartCount.value;
		resBody.supplySupportQuest = supplySupportQuest?.ToPDHeroSupplySupportQuest(m_currentTime);
		resBody.todayTasks = m_hero.todayTaskCollection.GetPDHeroTodayTasks().ToArray();
		resBody.achievementDailyPoint = m_hero.achievementDailyPoint.value;
		resBody.receivedAchievementRewardNo = m_hero.receivedAchievementRewardNo;
		resBody.ridingCartInst = ((m_hero.ridingCartInst != null) ? m_hero.ridingCartInst.ToPDCartInstanceWithLock(m_currentTime) : null);
		resBody.distortionScrollDailyUseCount = m_hero.distortionScrollDailyUseCount.value;
		resBody.remainingDistortionTime = m_hero.GetRemainingDistortionTime(m_currentTime);
		resBody.dailyServerLevelRakingNo = DailyServerLevelRankingManager.instance.rankingNo;
		resBody.dailyServerLevelRanking = DailyServerLevelRankingManager.instance.GetRankingOfHero(m_id)?.ranking ?? 0;
		resBody.rewardedDailyServerLevelRankingNo = m_hero.rewardedDailyServerLevelRankingNo;
		GuildMember guildMember = m_hero.guildMember;
		if (guildMember != null)
		{
			_ = guildMember.guild;
			resBody.guild = m_hero.guildMember.guild.ToPDGuild(m_currentTime);
			resBody.guildMemberGrade = m_hero.guildMember.grade.id;
		}
		resBody.totalGuildContributionPoint = m_hero.totalGuildContributionPoint;
		resBody.guildContributionPoint = m_hero.guildContributionPoint;
		resBody.guildPoint = m_hero.guildPoint;
		resBody.guildRejoinRemainingTime = m_hero.GetGuildJoinWaitTime(m_currentTime);
		resBody.guildDailyRewardReceivedDate = (DateTime)m_hero.guildDailyRewardReceivedDate;
		resBody.guildApplications = GuildApplication.GetPDMyGuildApplications(m_hero.guildApplications.Values).ToArray();
		resBody.dailyGuildApplicationCount = m_hero.dailyGuildApplicationCount.value;
		resBody.dailyGuildDonationCount = m_hero.dailyGuildDonationCount.value;
		resBody.dailyGuildFarmQuestStartCount = m_hero.dailyGuildFarmQuestStartCount.value;
		resBody.guildFarmQuest = ((m_hero.guildFarmQuest != null) ? m_hero.guildFarmQuest.ToPDHeroGuildFarmQuest() : null);
		resBody.guildSkills = m_hero.GetPDHeroGuildSkills().ToArray();
		resBody.dailyGuildFoodWarehouseStockCount = m_hero.dailyGuildFoodWarehouseStockCount.value;
		resBody.receivedGuildFoodWarehouseCollectionId = (Guid)m_hero.receivedGuildFoodWarehouseCollectionId;
		resBody.guildMoralPoint = m_hero.guildMoralPoint;
		resBody.guildAltarDefenseMissionRemainingCoolTime = m_hero.GetGuildAltarDefenseMissionRemainingCoolTime(m_currentTime);
		resBody.guildAltarRewardReceivedDate = (DateTime)m_hero.guildAltarRewardReceivedDate;
		resBody.guildMissionQuest = ((m_hero.guildMissionQuest != null) ? m_hero.guildMissionQuest.ToPDHeroGuildMissionQuest() : null);
		if (m_hero.guildSupplySupportQuestPlay != null)
		{
			resBody.guildSupplySupportQuestPlay = m_hero.guildSupplySupportQuestPlay.ToPDGuildSupplySupportQuestPlay(m_currentTime);
		}
		resBody.guildHuntingQuest = ((m_hero.guildHuntingQuest != null) ? m_hero.guildHuntingQuest.ToPDHeroGuildHuntingQuest() : null);
		resBody.dailyGuildHuntingQuestStartCount = m_hero.dailyGuildHuntingQuestCount.value;
		resBody.guildHuntingDonationDate = (DateTime)m_hero.guildHuntingDonationDate;
		resBody.guildHuntingDonationRewardReceivedDate = (DateTime)m_hero.guildHuntingDonationCompletionRewardReceivedDate;
		resBody.guildDailyObjectiveRewardReceivedNo = m_hero.receivedGuildDailyObjectiveRewardNo.value;
		resBody.guildWeeklyObjectiveRewardReceivedDate = (DateTime)m_hero.guildWeeklyObjectiveRewardReceivedDate;
		resBody.guildDailyObjectiveNoticeRemainingCoolTime = m_hero.GetGuildDailyObjecvtiveNoticeRemainingCoolTime(m_currentTime);
		NationWarManager nationWarManager = Cache.instance.nationWarManager;
		nationWarManager.Refresh(m_currentDate);
		resBody.nationWarDeclarations = nationWarManager.GetPDNationWarDeclarations().ToArray();
		nationInst.RefreshWeeklyNationWarDeclarationCount(m_currentDate);
		resBody.weeklyNationWarDeclarationCount = m_hero.nationInst.weeklyNationWarDeclarationCount.value;
		NationWarMember nationWarMember = m_hero.nationWarMember;
		if (nationWarMember != null)
		{
			resBody.nationWarJoined = nationWarMember.rewarded;
			resBody.nationWarKillCount = nationWarMember.killCount;
			resBody.nationWarAssistCount = nationWarMember.assistCount;
			resBody.nationWarDeadCount = nationWarMember.deadCount;
			resBody.nationWarImmediateRevivalCount = nationWarMember.immediateRevivalCount;
		}
		else
		{
			resBody.nationWarJoined = false;
			resBody.nationWarKillCount = 0;
			resBody.nationWarAssistCount = 0;
			resBody.nationWarDeadCount = 0;
			resBody.nationWarImmediateRevivalCount = 0;
		}
		m_hero.RefreshDailyNationWarFreeTransmissionCount(m_currentDate);
		resBody.dailyNationWarFreeTransmissionCount = m_hero.dailyNationWarFreeTransmissionCount.value;
		m_hero.RefreshDailyNationWarPaidTransmissionCount(m_currentDate);
		resBody.dailyNationWarPaidTransmissionCount = m_hero.dailyNationWarPaidTransmissionCount.value;
		nationInst.RefreshDailyNationWarCallCount(m_currentDate);
		resBody.dailyNationWarCallCount = nationInst.dailyNationWarCallCount.value;
		resBody.nationWarCallRemainingCoolTime = nationInst.GetNationWarCallRemainingCoolTime(m_currentTime);
		nationInst.RefreshDailyConvergingAttackCount(m_currentDate);
		resBody.dailyNationWarConvergingAttackCount = nationInst.dailyConvergingAttackCount.value;
		resBody.nationWarConvergingAttackRemainingCoolTime = nationInst.GetNationWarConvergingAttackRemainingCoolTime(m_currentTime);
		resBody.nationWarConvergingAttackTargetArrangeId = nationInst.nationWarConvergingAttack?.targetArrangeId ?? 0;
		NationWarInstance nationWarInst = m_hero.nationInst.nationWarInst;
		List<PDSimpleNationWarMonsterInstance> nationWarMonsterInsts = new List<PDSimpleNationWarMonsterInstance>();
		if (nationWarInst != null)
		{
			nationWarMonsterInsts = nationWarInst.GetPDSimpleNationWarMonsterInstances();
		}
		resBody.nationWarMonsterInsts = nationWarMonsterInsts.ToArray();
		resBody.dailyNationDonationCount = m_hero.dailyNationDonationCount.value;
		m_hero.RefreshWeeklySoulCoveterPlayCount(m_currentDate);
		resBody.weeklySoulCoveterPlayCount = m_hero.weeklySoulCoveterPlayCount.value;
		resBody.illustratedBookExplorationStepNo = m_hero.illustratedBookExplorationStepNo;
		resBody.explorationPoint = m_hero.explorationPoint;
		resBody.illustratedBookExplorationStepRewardReceivedDate = (DateTime)m_hero.illustratedBookExplorationStepRewardReceivedDate;
		resBody.illustratedBookExplorationStepRewardReceivedStepNo = m_hero.illustratedBookExplorationStepRewardReceivedStepNo;
		resBody.activationIllustratedBookIds = m_hero.GetIllustratedBooks().ToArray();
		resBody.completedSceneryQuests = m_hero.sceneryQuestCompletions.ToArray();
		resBody.rewardedAccomplishments = m_hero.rewardedAccomplishments.ToArray();
		resBody.accMonsterKillCount = m_hero.accMonsterKillCount;
		resBody.accSoulCoveterPlayCount = m_hero.accSoulCoveterPlayCount;
		resBody.accEpicBaitItemUseCount = m_hero.accEpicBaitItemUseCount;
		resBody.accLegendBaitItemUseCount = m_hero.accLegendBaitItemUseCount;
		resBody.accNationWarWinCount = m_hero.accNationWarWinCount;
		resBody.accNationWarKillCount = m_hero.accNationWarKillCount;
		resBody.accNationWarCommanderKillCount = m_hero.accNationWarCommanderKillCount;
		resBody.accNationWarImmediateRevivalCount = m_hero.accNationWarImmediateRevivalCount;
		resBody.maxGold = m_hero.maxGold;
		resBody.maxBattlePower = m_hero.maxBattlePower;
		resBody.maxAcquisitionMainGearGrade = m_hero.maxAcquisitionMainGearGrade;
		resBody.maxEquippedMainGearEnchantLevel = m_hero.maxEquippedMainGearEnchantLevel;
		resBody.titles = m_hero.GetPDHeroTitles(m_currentTime).ToArray();
		resBody.displayTitleId = m_hero.displayTitleId;
		resBody.activationTitleId = m_hero.activationTitleId;
		resBody.soulPowder = m_hero.soulPowder;
		resBody.creatureCards = HeroCreatureCard.ToPDHeroCreatureCards(m_hero.creatureCards.Values).ToArray();
		resBody.activatedCreatureCardCollections = m_hero.activatedCreatureCardCollections.Keys.ToArray();
		resBody.creatureCardCollectionFamePoint = m_hero.creatureCardCollectionFamePoint;
		resBody.purchasedCreatureCardShopFixedProducts = m_hero.purchasedCreatureCardShopFixedProducts.ToArray();
		resBody.creatureCardShopRandomProducts = HeroCreatureCardShopRandomProduct.ToPDHeroCreatureCardShopRandomProducts(m_hero.creatureCardShopRandomProducts.Values).ToArray();
		resBody.dailyCreatureCardShopPaidRefreshCount = m_hero.dailyCreatureCardShopPaidRefreshCount.value;
		resBody.heroEliteMonsterKills = m_hero.GetPDHeroEliteMonsterKills().ToArray();
		resBody.spawnedEliteMonsters = nationInst.GetSpawnedEliteMonsterIds().ToArray();
		m_hero.RefreshDailyEliteDungeonPlayCount(m_currentDate);
		resBody.dailyEliteDungeonPlayCount = m_hero.dailyEliteDungeonPlayCount.value;
		resBody.lootingItemMinGrade = (int)m_hero.lootingItemMinGrade;
		resBody.proofOfValorCleared = m_hero.proofOfValorCleared;
		m_hero.RefreshDailyProofOfValorPlayCount(m_currentDate);
		resBody.dailyProofOfValorPlayCount = m_hero.dailyProofOfValorPlayCount.value;
		m_hero.RefreshDailyProofOfValorFreeRefreshCount(m_currentDate);
		resBody.dailyProofOfValorFreeRefreshCount = m_hero.dailyProofOfValorFreeRefreshCount.value;
		m_hero.RefreshDailyProofOfValorPaidRefreshCount(m_currentDate);
		resBody.dailyProofOfValorPaidRefreshCount = m_hero.dailyProofOfValorPaidRefreshCount.value;
		resBody.poofOfValorPaidRefreshCount = m_hero.proofOfValorPaidRefreshCount;
		resBody.heroProofOfValorInstance = m_hero.heroProofOfValorInst.ToPDHeroProofOfValorInstance();
		resBody.customPresetHair = m_hero.customPresetHair;
		resBody.customFaceJawHeight = m_hero.customFaceJawHeight;
		resBody.customFaceJawWidth = m_hero.customFaceJawWidth;
		resBody.customFaceJawEndHeight = m_hero.customFaceJawEndHeight;
		resBody.customFaceWidth = m_hero.customFaceWidth;
		resBody.customFaceEyebrowHeight = m_hero.customFaceEyebrowHeight;
		resBody.customFaceEyebrowRotation = m_hero.customFaceEyebrowRotation;
		resBody.customFaceEyesWidth = m_hero.customFaceEyesWidth;
		resBody.customFaceNoseHeight = m_hero.customFaceNoseHeight;
		resBody.customFaceNoseWidth = m_hero.customFaceNoseWidth;
		resBody.customFaceMouthHeight = m_hero.customFaceMouthHeight;
		resBody.customFaceMouthWidth = m_hero.customFaceMouthWidth;
		resBody.customBodyHeadSize = m_hero.customBodyHeadSize;
		resBody.customBodyArmsLength = m_hero.customBodyArmsLength;
		resBody.customBodyArmsWidth = m_hero.customBodyArmsWidth;
		resBody.customBodyChestSize = m_hero.customBodyChestSize;
		resBody.customBodyWaistWidth = m_hero.customBodyWaistWidth;
		resBody.customBodyHipsSize = m_hero.customBodyHipsSize;
		resBody.customBodyPelvisWidth = m_hero.customBodyPelvisWidth;
		resBody.customBodyLegsLength = m_hero.customBodyLegsLength;
		resBody.customBodyLegsWidth = m_hero.customBodyLegsWidth;
		resBody.customColorSkin = m_hero.customColorSkin;
		resBody.customColorEyes = m_hero.customColorEyes;
		resBody.customColorBeardAndEyebrow = m_hero.customColorBeardAndEyebrow;
		resBody.customColorHair = m_hero.customColorHair;
		resBody.todayMissionTutorialStarted = m_hero.todayMissionTutorialStarted;
		resBody.heroNpcShopProducts = HeroNpcShopProduct.ToPDHeroNpcShopProducts(m_hero.npcShopProducts.Values).ToArray();
		resBody.rankActiveSkills = m_hero.GetPDHeroRankActiveSkills().ToArray();
		resBody.rankPassiveSkills = m_hero.GetPDHeroRankPassiveSkills().ToArray();
		resBody.selectedRankActiveSkillId = m_hero.selectedRankActiveSkillId;
		resBody.rankActiveSkillRemainingCoolTime = m_hero.GetRankActiveSkillRemainingCoolTime(m_currentTime);
		resBody.spiritStone = m_hero.spiritStone;
		resBody.rookieGiftNo = m_hero.rookieGiftNo;
		resBody.rookieGiftRemainingTime = m_hero.GetCurrentRookieGiftRemainingTime(m_currentTime);
		resBody.receivedOpenGiftRewards = m_hero.receivedOpenGiftRewards.ToArray();
		resBody.regDate = (DateTime)m_hero.regTime.Date;
		resBody.dailyQuestAcceptionCount = m_hero.dailyQuestAcceptionCount.value;
		resBody.dailyQuestFreeRefreshCount = m_hero.dailyQuestFreeRefreshCount.value;
		resBody.dailyQuests = m_hero.GetPDHeroDailyQuests(m_currentTime).ToArray();
		resBody.weeklyQuest = ((m_hero.weeklyQuest != null) ? m_hero.weeklyQuest.ToPDHeroWeeklyQuest() : null);
		resBody.wisdomTempleCleared = m_hero.wisdomTempleCleared;
		m_hero.RefreshDailyWisdomTemplePlayCount(m_currentDate);
		resBody.dailyWisdomTemplePlayCount = m_hero.dailyWisdomTemplePlayCount.value;
		resBody.rewardedOpen7DayEventMissions = m_hero.rewardedOpen7DayEventMissions.ToArray();
		resBody.purchasedOpen7DayEventProducts = m_hero.purchasedOpen7DayEventProducts.ToArray();
		resBody.open7DayEventProgressCounts = HeroOpen7DayEventProgressCount.ToPDHeroOpen7DayEventProgressCounts(m_hero.open7DayEventProgressCounts.Values).ToArray();
		resBody.open7DayEventRewarded = m_hero.open7DayEventRewarded;
		resBody.retrievalProgressCounts = m_hero.GetPDHeroRetreivalProgressCounts().ToArray();
		resBody.retrievals = HeroRetrieval.ToPDHeroRetrievals(m_hero.retrievals.Values).ToArray();
		m_hero.RefreshDailyRuinsReclaimFreePlayCount(m_currentDate);
		resBody.dailyRuinsReclaimFreePlayCount = m_hero.dailyRuinsReclaimFreePlayCount.value;
		resBody.taskConsignments = HeroTaskConsignment.ToPDHeroTaskConsignments(m_hero.taskConsignments.Values, m_currentTime).ToArray();
		resBody.taskConsignmentStartCounts = HeroTaskConsignmentStartCount.ToPDHeroTaskConsignmentStartCounts(m_hero.taskConsignmentStartCounts.Values).ToArray();
		resBody.trueHeroQuest = ((m_hero.trueHeroQuest != null) ? m_hero.trueHeroQuest.ToPDHeroTrueHeroQuest() : null);
		m_hero.RefreshDailyInfiniteWarPlayCount(m_currentDate);
		resBody.dailyInfiniteWarPlayCount = m_hero.dailyInfiniteWarPlayCount.value;
		resBody.spawnedFieldBosses = Cache.instance.GetSpawnedFieldBosses(m_hero.nationId).ToArray();
		resBody.rewardedLimitationGiftScheduleIds = m_hero.rewardedLimitationGiftScheduleIds.ToArray();
		resBody.weekendReward = ((m_hero.weekendReward != null) ? m_hero.weekendReward.ToPDHeroWeekendReward() : null);
		resBody.paidWarehouseSlotCount = m_hero.paidWarehouseSlotCount;
		resBody.placedWarehouseSlots = m_hero.GetPlacedPDWarehouseSlots().ToArray();
		resBody.dailyDiaShopProductBuyCounts = HeroDiaShopProductBuyCount.ToPDHeroDiaShopProductBuyCounts(m_hero.dailyDiaShopProductBuyCounts.Values).ToArray();
		resBody.totalDiaShopProductBuyCounts = HeroDiaShopProductBuyCount.ToPDHeroDiaShopProductBuyCounts(m_hero.totalDiaShopProductBuyCounts.Values).ToArray();
		m_hero.RefreshDailyFearAltarPlayCount(m_currentDate);
		resBody.dailyFearAltarPlayCount = m_hero.dailyFearAltarPlayCount.value;
		m_hero.RefreshWeeklyFearAltarHalidomCollectionRewardNo(m_currentDate);
		resBody.weeklyFearAltarHalidomCollectionRewardNo = m_hero.weeklyFearAltarHalidomCollectionRewardNo.value;
		m_hero.RefreshFearAltarHalidom(m_currentDate);
		resBody.weeklyFearAltarHalidoms = m_hero.GetFearAltarHalidoms().ToArray();
		m_hero.RefreshFearAltarHalidomElementalReward(m_currentDate);
		resBody.weeklyRewardReceivedFearAltarHalidomElementals = m_hero.GetFearAltarHalidomCollectionReward().ToArray();
		resBody.subQuests = HeroSubQuest.ToPDHeroSubQuests(m_hero.subQuests.Values).ToArray();
		m_hero.RefreshDailyWarMemoryFreePlayCount(m_currentDate);
		resBody.dailyWarMemoryFreePlayCount = m_hero.dailyWarMemoryFreePlayCount.value;
		resBody.ordealQuest = ((m_hero.ordealQuest != null) ? m_hero.ordealQuest.ToPDHeroOrdealQuest(m_currentTime) : null);
		m_hero.RefreshDailyOsirisRoomPlayCount(m_currentDate);
		resBody.dailyOsirisRoomPlayCount = m_hero.dailyOsirisRoomPlayCount.value;
		resBody.biographies = HeroBiography.ToPDHeroBiographies(m_hero.biographies.Values).ToArray();
		resBody.friends = m_hero.GetPDFriends().ToArray();
		resBody.tempFriends = m_hero.GetPDTempFriends(m_currentTime).ToArray();
		resBody.blacklistEntries = m_hero.GetPDBlacklistEntries().ToArray();
		resBody.deadRecords = m_hero.GetPDDeadRecords(m_currentTime).ToArray();
		resBody.ownerProspectQuests = m_hero.GetOwnerPDHeroProspectQuests(m_currentTime).ToArray();
		resBody.targetProspectQuests = m_hero.GetTargetPDHeroProspectQuests(m_currentTime).ToArray();
		resBody.itemLuckyShopFreePickRemainingTime = m_hero.GetRemainingItemLuckyShopFreePickTime(m_currentTime);
		resBody.itemLuckyShopFreePickCount = m_hero.itemLuckyShopFreePickCount;
		resBody.itemLuckyShopPick1TimeCount = m_hero.itemLuckyShopPick1TimeCount;
		resBody.itemLuckyShopPick5TimeCount = m_hero.itemLuckyShopPick5TimeCount;
		resBody.creatureCardLuckyShopFreePickRemainingTime = m_hero.GetRemainingCreatureCardLuckyShopFreePickTime(m_currentTime);
		resBody.creatureCardLuckyShopFreePickCount = m_hero.creatureCardLuckyShopFreePickCount;
		resBody.creatureCardLuckyShopPick1TimeCount = m_hero.creatureCardLuckyShopPick1TimeCount;
		resBody.creatureCardLuckyShopPick5TimeCount = m_hero.creatureCardLuckyShopPick5TimeCount;
		resBody.creatures = HeroCreature.ToPDHeroCreatures(m_hero.creatures.Values).ToArray();
		resBody.participatedCreatureId = (Guid)m_hero.participationCreatureId;
		resBody.dailyCreatureVariationCount = m_hero.dailyCreatureVariationCount.value;
		resBody.weeklyPresentPopularityPoint = m_hero.weeklyPresentPopularityPoint;
		resBody.weeklyPresentContributionPoint = m_hero.weeklyPresentContributionPoint;
		resBody.nationWeeklyPresentPopularityPointRankingNo = NationWeeklyPresentPopularityPointRankingManager.instance.rankingNo;
		resBody.nationWeeklyPresentPopularityPointRanking = nationInst.GetWeeklyPresentPopularityPointRankingValueOfHero(m_id);
		resBody.rewardedNationWeeklyPresentPopularityPointRankingNo = m_hero.rewardedNationWeeklyPresentPopularityPointRankingNo;
		resBody.nationWeeklyPresentContributionPointRankingNo = NationWeeklyPresentContributionPointRankingManager.instance.rankingNo;
		resBody.nationWeeklyPresentContributionPointRanking = nationInst.GetWeeklyPresentContributionPointRankingValueOfHero(m_id);
		resBody.rewardedNationWeeklyPresentContributionPointRankingNo = m_hero.rewardedNationWeeklyPresentContributionPointRankingNo;
		resBody.costumes = m_hero.GetPDHeroCostumes(m_currentTime).ToArray();
		resBody.equippedCostumeId = m_hero.equippedCostumeId;
		resBody.costumeCollectionId = m_hero.costumeCollectionId;
		resBody.costumeCollectionActivated = m_hero.costumeCollectionActivated;
		resBody.dailyCreatureFarmQuestAcceptionCount = m_hero.dailyCreatureFarmAcceptionCount.value;
		resBody.creatureFarmQuest = ((m_hero.creatureFarmQuest != null) ? m_hero.creatureFarmQuest.ToPDHeroCreatureFarmQuest() : null);
		resBody.cashProductPurchaseCounts = m_myAccount.GetPDCashProductPurchaseCounts().ToArray();
		resBody.firstChargeEventObjectiveCompleted = m_myAccount.firstChargeEventObjectiveCompleted;
		resBody.firstChargeEventRewarded = m_myAccount.firstChargeEventRewarded;
		resBody.rechargeEventAccUnOwnDia = m_myAccount.rechargeEventAccUnOwnDia;
		resBody.rechargeEventRewarded = m_myAccount.rechargeEventRewarded;
		ChargeEvent chargeEvent = Resource.instance.GetChargeEventByTime(m_currentTime.DateTime);
		if (chargeEvent != null)
		{
			resBody.chargeEvent = m_myAccount.GetOrCreateChargeEvent(chargeEvent.id).ToPDAccountChargeEvent();
		}
		resBody.dailyChargeEventAccUnOwnDia = m_myAccount.dailyChargeEventAccUnOwnDia;
		resBody.rewardedDailyChargeEventMissions = m_myAccount.rewardedDailyChargeEventMissions.ToArray();
		ConsumeEvent consumeEvent = Resource.instance.GetConsumeEventByTime(m_currentTime.DateTime);
		if (consumeEvent != null)
		{
			resBody.consumeEvent = m_myAccount.GetOrCreateConsumeEvent(consumeEvent.id).ToPDAccountConsumeEvent();
		}
		resBody.dailyConsumeEventAccDia = m_myAccount.dailyConsumeEventAccDia;
		resBody.rewardedDailyConsumeEventMissions = m_myAccount.rewardedDailyConsumeEventMissions.ToArray();
		resBody.jobChangeQuest = ((m_hero.jobChangeQuest != null) ? m_hero.jobChangeQuest.ToPDHeroJobChangeQuest(m_currentTime) : null);
		resBody.potionAttrs = HeroPotionAttr.ToPDHeroPotionAttrs(m_hero.potionAttrs.Values).ToArray();
		m_hero.RefreshDailyAnkouTombPlayCount(m_currentDate);
		resBody.dailyAnkouTombPlayCount = m_hero.dailyAnkouTombPlayCount.value;
		resBody.myAnkouTombBestRecords = m_hero.GetPDHeroAnkouTombBestRecord().ToArray();
		resBody.serverAnkouTombBestRecords = Cache.instance.GetPDAnkouTombServerBestRecords().ToArray();
		resBody.constellations = HeroConstellation.ToPDHeroConstellations(m_hero.constellations.Values).ToArray();
		resBody.starEssense = m_hero.starEssense;
		resBody.dailyStarEssenseItemUseCount = m_hero.dailyStarEssensItemUseCount.value;
		resBody.artifactNo = m_hero.artifactNo;
		resBody.artifactLevel = m_hero.artifactLevel;
		resBody.artifactExp = m_hero.artifactExp;
		resBody.equippedArtifactNo = m_hero.equippedArtifactNo;
		m_hero.RefreshDailyTradeShipPlayCount(m_currentDate);
		resBody.dailyTradeShipPlayCount = m_hero.dailyTradeShipPlayCount.value;
		resBody.myTradeShipBestRecords = m_hero.GetPDHeroTradeShipBestRecord().ToArray();
		resBody.serverTradeShipBestRecords = Cache.instance.GetPDTradeShipServerBestRecords().ToArray();
		SendResponseOK(resBody);
	}
}
