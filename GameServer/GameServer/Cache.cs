using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class Cache
{
	public const int kUpdateInterval = 200;

	private bool m_bReleased;

	private Timer m_updateTimer;

	private DateTimeOffset m_prevUpdateTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_currentUpdateTime = DateTimeOffset.MinValue;

	private float m_fCurrentDeltaTime;

	private DateTime m_serverOpenDate = DateTime.MinValue;

	private Dictionary<Guid, Place> m_places = new Dictionary<Guid, Place>();

	private Dictionary<int, NationInstance> m_nationInsts = new Dictionary<int, NationInstance>();

	private Dictionary<Guid, NationAlliance> m_nationAlliances = new Dictionary<Guid, NationAlliance>();

	private Dictionary<int, DisputeContinentInstance> m_disputeContinentInsts = new Dictionary<int, DisputeContinentInstance>();

	private MailDeliveryManager m_mailDeliveryManager = new MailDeliveryManager();

	private Dictionary<Guid, Account> m_accounts = new Dictionary<Guid, Account>();

	private Dictionary<Guid, Hero> m_heroes = new Dictionary<Guid, Hero>();

	private Dictionary<long, CartInstance> m_cartInsts = new Dictionary<long, CartInstance>();

	private Dictionary<Guid, Party> m_parties = new Dictionary<Guid, Party>();

	private Dictionary<Guid, PartyMember> m_partyMembers = new Dictionary<Guid, PartyMember>();

	private AncientRelicMatchingManager m_ancientRelicMatchingManager;

	private FieldOfHonorHero[] m_fieldOfHonorRankers = new FieldOfHonorHero[9999];

	private int m_nCurrentFieldOfHonorMaxRanking;

	private Dictionary<Guid, Guild> m_guilds = new Dictionary<Guid, Guild>();

	private Dictionary<Guid, GuildMember> m_guildMembers = new Dictionary<Guid, GuildMember>();

	private Dictionary<Guid, GuildApplication> m_guildApplications = new Dictionary<Guid, GuildApplication>();

	private Dictionary<Guid, HeroGuildApplicationCollection> m_heroGuildApplicationCollections = new Dictionary<Guid, HeroGuildApplicationCollection>();

	private NationWarManager m_nationWarManager;

	private SoulCoveterMatchingManager m_soulCoveterMatchingManager;

	private int m_nServerMaxLevel;

	private RuinsReclaimMatchingManager m_ruinsReclaimMatchingManager;

	private InfiniteWarMatchingManager m_infiniteWarMatchingManager;

	private FieldBossEventSchedule m_currentFieldBossEventSchedule;

	private FearAltarMatchingManager m_fearAltarMatchingManager;

	private WarMemoryMatchingManager m_warMemoryMatchingManager;

	private Dictionary<Guid, HeroProspectQuest> m_prospectQuests = new Dictionary<Guid, HeroProspectQuest>();

	private DragonNestMatchingManager m_dragonNestMatchingManager;

	private Dictionary<Guid, NationAllianceApplication> m_nationAllianceApplications = new Dictionary<Guid, NationAllianceApplication>();

	private AnkouTombMatchingManager m_ankouTombMatchingManager;

	private Dictionary<int, HeroAnkouTombBestRecord> m_ankouTombServerBestRecords = new Dictionary<int, HeroAnkouTombBestRecord>();

	private TradeShipMatchingManager m_tradeShipMatchingManager;

	private Dictionary<int, HeroTradeShipBestRecord> m_tradeShipServerBestRecords = new Dictionary<int, HeroTradeShipBestRecord>();

	private static Cache s_instance = new Cache();

	public bool released => m_bReleased;

	public DateTimeOffset prevUpdateTime => m_prevUpdateTime;

	public DateTimeOffset currentUpdateTime => m_currentUpdateTime;

	public float currentDaltaTime => m_fCurrentDeltaTime;

	public bool isUserConnectionFull
	{
		get
		{
			if (Resource.instance.maxUserConnectionCount > 0)
			{
				return m_accounts.Count >= Resource.instance.maxUserConnectionCount;
			}
			return false;
		}
	}

	public DateTime serverOpenDate => m_serverOpenDate;

	public AncientRelicMatchingManager ancientRelicMatchingManager => m_ancientRelicMatchingManager;

	public NationWarManager nationWarManager => m_nationWarManager;

	public SoulCoveterMatchingManager soulCoveterMatchingManager => m_soulCoveterMatchingManager;

	public RuinsReclaimMatchingManager ruinsReclaimMatchingManager => m_ruinsReclaimMatchingManager;

	public InfiniteWarMatchingManager infiniteWarMatchingManager => m_infiniteWarMatchingManager;

	public FearAltarMatchingManager fearAltarMatchingManager => m_fearAltarMatchingManager;

	public WarMemoryMatchingManager warMemoryMatchingManager => m_warMemoryMatchingManager;

	public DragonNestMatchingManager dragonNestMatchingManager => m_dragonNestMatchingManager;

	public AnkouTombMatchingManager ankouTombMatchingManager => m_ankouTombMatchingManager;

	public TradeShipMatchingManager tradeShipMatchingManager => m_tradeShipMatchingManager;

	public Dictionary<Guid, Hero> heroes => m_heroes;

	public Dictionary<int, NationInstance> nationInsts => m_nationInsts;

	public Dictionary<Guid, NationAlliance> nationAlliances => m_nationAlliances;

	public int currentFieldOfHonorMaxRanking => m_nCurrentFieldOfHonorMaxRanking;

	public int serverMaxLevel => m_nServerMaxLevel;

	public static Cache instance => s_instance;

	private Cache()
	{
	}

	public void Init()
	{
		SFLogUtil.Info(GetType(), "Cache.Init() started.");
		InitWithGameDB();
		DBWork_UpdateGameServer_CurrentUserCount(0);
		InitWorld();
		InitFieldOfHonorRanker();
		InitGuilds();
		ServerBattlePowerRankingManager.instance.Init();
		ServerLevelRankingManager.instance.Init();
		DailyServerLevelRankingManager.instance.Init();
		ServerJobBattlePowerRankingManager.instance.Init();
		NationBattlePowerRankingManager.instance.Init();
		NationExploitPointRankingManager.instance.Init();
		DailyFieldOfHonorRankingManager.instance.Init();
		ServerGuildRankingManager.instance.Init();
		NationGuildRankingManager.instance.Init();
		ServerCreatureCardRankingManager.instance.Init();
		ServerIllustratedBookRankingManager.instance.Init();
		ServerPresentContributionPointRankingManager.instance.Init();
		NationWeeklyPresentContributionPointRankingManager.instance.Init();
		ServerPresentPopularityPointRankingManager.instance.Init();
		NationWeeklyPresentPopularityPointRankingManager.instance.Init();
		ServerNationPowerRankingManager.instance.Init();
		m_prevUpdateTime = DateTimeUtil.currentTime;
		m_currentUpdateTime = m_prevUpdateTime;
		m_fCurrentDeltaTime = 0f;
		lock (GlobalNoticeMananger.instance.syncObject)
		{
			GlobalNoticeMananger.instance.Init();
		}
		lock (ServerNoticeManager.instance.syncObject)
		{
			ServerNoticeManager.instance.Init();
		}
		InitProspectQuests();
		InitAnkouTombBestRecord();
		InitTradeShipBestRecord();
		m_updateTimer = new Timer(OnUpdateTimerTick);
		m_updateTimer.Change(200, 200);
		SFLogUtil.Info(GetType(), "Cache.Init() finished.");
	}

	private void InitWithGameDB()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			DataRow drSystem = GameDac.System(conn, null);
			m_serverOpenDate = SFDBUtil.ToDateTime(drSystem["serverOpenDate"], DateTime.MinValue);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void InitWorld()
	{
		Resource res = Resource.instance;
		InitNationAlliance();
		foreach (Nation nation in res.nations.Values)
		{
			InitNationInstance(nation);
		}
		foreach (NationInstance nationInst2 in m_nationInsts.Values)
		{
			nationInst2.RefreshSecretLetterQuestTargetNation(bSendEvent: false);
		}
		InitNationAllianceApplication();
		foreach (Continent continent in res.continents.Values)
		{
			if (continent.isNationTerritory)
			{
				foreach (Nation nation2 in res.nations.Values)
				{
					NationContinentInstance continentInst2 = new NationContinentInstance();
					NationInstance nationInst3 = GetNationInstance(nation2.id);
					if (nationInst3 != null)
					{
						lock (continentInst2.syncObject)
						{
							continentInst2.InitNationContinent(nationInst3, continent);
							nationInst3.AddContinentInstance(continentInst2);
							AddPlace(continentInst2);
						}
					}
				}
			}
			else
			{
				DisputeContinentInstance continentInst = new DisputeContinentInstance();
				lock (continentInst.syncObject)
				{
					continentInst.InitDisputeContinent(continent);
					AddDisputeContinentInstance(continentInst);
					AddPlace(continentInst);
				}
			}
		}
		UndergroundMaze undergroundMaze = res.undergroundMaze;
		foreach (NationInstance nationInst in m_nationInsts.Values)
		{
			foreach (UndergroundMazeFloor floor in undergroundMaze.floors)
			{
				UndergroundMazeInstance undergroundMazeInst = new UndergroundMazeInstance();
				lock (undergroundMazeInst.syncObject)
				{
					undergroundMazeInst.Init(floor, nationInst.nation);
					nationInst.AddUndergroundMazeInstance(undergroundMazeInst);
					AddPlace(undergroundMazeInst);
				}
			}
		}
		m_ancientRelicMatchingManager = new AncientRelicMatchingManager(res.ancientRelic);
		NationWar nationWar = res.nationWar;
		m_nationWarManager = new NationWarManager(nationWar);
		InitNationWar();
		m_soulCoveterMatchingManager = new SoulCoveterMatchingManager(res.soulCoveter);
		m_ruinsReclaimMatchingManager = new RuinsReclaimMatchingManager(res.ruinsReclaim);
		m_infiniteWarMatchingManager = new InfiniteWarMatchingManager(res.infiniteWar);
		m_fearAltarMatchingManager = new FearAltarMatchingManager(res.fearAltar);
		m_warMemoryMatchingManager = new WarMemoryMatchingManager(res.warMemory);
		m_dragonNestMatchingManager = new DragonNestMatchingManager(res.dragonNest);
		m_ankouTombMatchingManager = new AnkouTombMatchingManager(res.ankouTomb);
		m_tradeShipMatchingManager = new TradeShipMatchingManager(res.tradeShip);
		SFLogUtil.Info(GetType(), "Cache.InitWorld() finished.");
	}

	private void InitNationInstance(Nation nation)
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			DataRow drNationInstance = GameDac.NationInstance(conn, null, nation.id);
			if (drNationInstance == null)
			{
				throw new Exception("[국가인스턴스 목록] 국가인스턴스가 존재하지 않습니다. nationId = " + nation.id);
			}
			NationInstance nationInst = new NationInstance(nation);
			nationInst.Init(drNationInstance);
			AddNationInstance(nationInst);
			Guid allianceId = (Guid)drNationInstance["allianceId"];
			if (allianceId != Guid.Empty)
			{
				NationAlliance nationAlliance = GetNationAlliance(allianceId);
				nationAlliance.AddNationInstance(nationInst);
			}
			foreach (DataRow dr2 in GameDac.NationIncumbentNoblesses(conn, null, nation.id))
			{
				int nNoblesseId2 = Convert.ToInt32(dr2["noblesseId"]);
				NationNoblesseInstance noblesseInst2 = nationInst.GetNoblesseInstanceByNoblesseId(nNoblesseId2);
				if (noblesseInst2 == null)
				{
					SFLogUtil.Warn(GetType(), "[국가현직관직 목록] 국가관직인스턴스가 존재하지 않습니다. nationId = " + nation.id + ", nNoblesseId = " + nNoblesseId2);
				}
				else
				{
					noblesseInst2.heroId = SFDBUtil.ToGuid(dr2["heroId"], Guid.Empty);
					noblesseInst2.heroName = Convert.ToString(dr2["name"]);
					noblesseInst2.heroJobId = Convert.ToInt32(dr2["jobId"]);
				}
			}
			foreach (DataRow dr in GameDac.NationNoblesseAppointment(conn, null, nation.id))
			{
				int nNoblesseId = Convert.ToInt32(dr["noblesseId"]);
				NationNoblesseInstance noblesseInst = nationInst.GetNoblesseInstanceByNoblesseId(nNoblesseId);
				if (noblesseInst == null)
				{
					SFLogUtil.Warn(GetType(), "[국가관직임명 목록] 국가관직임명이 존재하지 않습니다. nationId = " + nation.id + ", nNoblesseId = " + nNoblesseId);
				}
				else
				{
					noblesseInst.appointmentDate = SFDBUtil.ToDateTimeOffset(dr["appointmentTime"], DateTimeOffset.MinValue).Date;
				}
			}
			DataRow drNationWarDeclarationCountOfWeekly = GameDac.NationWarDeclarationCountOfWeekly(conn, null, nation.id, DateTimeUtil.GetWeekStartDate(DateTimeUtil.currentTime.Date));
			if (drNationWarDeclarationCountOfWeekly != null)
			{
				DateTime dateOfMonday = SFDBUtil.ToDateTime(drNationWarDeclarationCountOfWeekly["dateOfMonday"], DateTime.MinValue);
				int nCount = Convert.ToInt32(drNationWarDeclarationCountOfWeekly["cnt"]);
				nationInst.SetWeeklyNationWarDeclarationCount(dateOfMonday, nCount);
			}
			int nNationHeroCount = (nationInst.nationHeroCount = GameDac.NationHeroCount(conn, null, nation.id));
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void InitNationWar()
	{
		DateTime currentDate = DateTimeUtil.currentTime.Date;
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			foreach (DataRow dr2 in GameDac.NationWarDeclaration_ByPast(conn, null, currentDate))
			{
				NationWarDeclaration pastNationWar = new NationWarDeclaration();
				pastNationWar.Init(dr2);
				m_nationWarManager.AddPastNationWar(pastNationWar);
			}
			foreach (DataRow dr in GameDac.NationWarDeclaration_OnDeclare(conn, null, currentDate))
			{
				NationWarDeclaration nationWarDeclaration = new NationWarDeclaration();
				nationWarDeclaration.Init(dr);
				m_nationWarManager.AddNationWarDeclaration(nationWarDeclaration);
			}
			m_nationWarManager.Init(currentDate);
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void InitNationAllianceApplication()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			foreach (DataRow dr in GameDac.NationAllianceApplications(conn, null))
			{
				NationAllianceApplication nationAllianceApplication = new NationAllianceApplication();
				nationAllianceApplication.Init(dr);
				AddNationAllianceApplication(nationAllianceApplication);
			}
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void InitNationAlliance()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			foreach (DataRow dr in GameDac.Alliances(conn, null))
			{
				NationAlliance nationAlliance = new NationAlliance();
				nationAlliance.Init(dr);
				AddNationAlliance(nationAlliance);
			}
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void InitFieldOfHonorRanker()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			foreach (DataRow dr4 in GameDac.FieldOfHonorHeroes(conn, null, 9999))
			{
				FieldOfHonorHero hero5 = new FieldOfHonorHero();
				hero5.Init(dr4);
				SetFieldOfHonorRanker(hero5);
			}
			foreach (DataRow dr7 in GameDac.FieldOfHonorHeroSkills(conn, null, 9999))
			{
				Guid heroId7 = SFDBUtil.ToGuid(dr7["heroId"]);
				FieldOfHonorHero hero9 = GetFieldOfHonorHeroOfHeroId(heroId7);
				if (hero9 == null)
				{
					SFLogUtil.Warn(GetType(), "[결투장영웅스킬 목록] 결투장영웅이 존재하지 않습니다. heroId = " + heroId7);
					continue;
				}
				FieldOfHonorHeroSkill skill = new FieldOfHonorHeroSkill(hero9);
				skill.Init(dr7);
				hero9.AddSkill(skill);
			}
			foreach (DataRow dr10 in GameDac.FieldOfHonorHeroWings(conn, null, 9999))
			{
				Guid heroId9 = SFDBUtil.ToGuid(dr10["heroId"]);
				FieldOfHonorHero hero10 = GetFieldOfHonorHeroOfHeroId(heroId9);
				if (hero10 == null)
				{
					SFLogUtil.Warn(GetType(), "[결투장영웅날개 목록] 결투장영웅이 존재하지 않습니다. heroId = " + heroId9);
					continue;
				}
				FieldOfHonorHeroWing wing = new FieldOfHonorHeroWing(hero10);
				wing.Init(dr10);
				hero10.AddWing(wing);
			}
			foreach (DataRow dr9 in GameDac.FieldOfHonorHeroWingEnchants(conn, null, 9999))
			{
				Guid heroId8 = SFDBUtil.ToGuid(dr9["heroId"]);
				FieldOfHonorHero hero8 = GetFieldOfHonorHeroOfHeroId(heroId8);
				if (hero8 == null)
				{
					SFLogUtil.Warn(GetType(), "[결투장영웅날개강화 목록] 결투장영웅이 존재하지 않습니다. heroId = " + heroId8);
					continue;
				}
				int nPartId = Convert.ToInt32(dr9["partId"]);
				FieldOfHonorHeroWingPart wingPart = hero8.GetWingPart(nPartId);
				if (wingPart == null)
				{
					wingPart = new FieldOfHonorHeroWingPart(hero8, nPartId);
					hero8.AddWingPart(wingPart);
				}
				FieldOfHonorHeroWingEnchant wingEnchant = new FieldOfHonorHeroWingEnchant(wingPart);
				wingEnchant.Init(dr9);
				wingPart.AddEnchant(wingEnchant);
			}
			foreach (DataRow dr8 in GameDac.FieldOfHonorHeroEquippedMainGears(conn, null, 9999))
			{
				Guid heroId6 = SFDBUtil.ToGuid(dr8["heroId"]);
				FieldOfHonorHero hero7 = GetFieldOfHonorHeroOfHeroId(heroId6);
				if (hero7 == null)
				{
					SFLogUtil.Warn(GetType(), "[결투장영웅착용메인장비 목록] 결투장영웅이 존재하지 않습니다. heroId = " + heroId6);
					continue;
				}
				FieldOfHonorHeroEquippedMainGear equippedMainGear2 = new FieldOfHonorHeroEquippedMainGear(hero7);
				equippedMainGear2.Init(dr8);
				hero7.SetEquippedMainGear(equippedMainGear2);
			}
			foreach (DataRow dr6 in GameDac.FieldOfHonorHeroMainGearOptionAttrs(conn, null, 9999))
			{
				Guid heroId5 = SFDBUtil.ToGuid(dr6["heroId"]);
				FieldOfHonorHero hero6 = GetFieldOfHonorHeroOfHeroId(heroId5);
				if (hero6 == null)
				{
					SFLogUtil.Warn(GetType(), "[결투장영웅메인장비옵션속성 목록] 결투장영웅이 존재하지 않습니다. heroId = " + heroId5);
					continue;
				}
				Guid heroMainGearId = SFDBUtil.ToGuid(dr6["heroMainGearId"]);
				FieldOfHonorHeroEquippedMainGear equippedMainGear = hero6.GetEquippedMainGear(heroMainGearId);
				if (equippedMainGear == null)
				{
					SFLogUtil.Warn(GetType(), string.Concat("[결투장영웅메인장비옵션속성 목록] 결투장영웅착용메인장비가 존재하지 않습니다. heroId = ", heroId5, ", heroMainGearId = ", heroMainGearId));
				}
				else
				{
					FieldOfHonorHeroMainGearOptionAttr mainGearOptionAttr = new FieldOfHonorHeroMainGearOptionAttr(equippedMainGear);
					mainGearOptionAttr.Init(dr6);
					equippedMainGear.AddOptionAttr(mainGearOptionAttr);
				}
			}
			foreach (DataRow dr5 in GameDac.FieldOfHonorHeroEquippedSubGears(conn, null, 9999))
			{
				Guid heroId4 = SFDBUtil.ToGuid(dr5["heroId"]);
				FieldOfHonorHero hero4 = GetFieldOfHonorHeroOfHeroId(heroId4);
				if (hero4 == null)
				{
					SFLogUtil.Warn(GetType(), "[결투장영웅착용보조장비 목록] 결투장영웅이 존재하지 않습니다. heroId = " + heroId4);
					continue;
				}
				FieldOfHonorHeroEquippedSubGear equippedSubGear3 = new FieldOfHonorHeroEquippedSubGear(hero4);
				equippedSubGear3.Init(dr5);
				hero4.AddEquippedSubGear(equippedSubGear3);
			}
			foreach (DataRow dr3 in GameDac.FieldOfHonorHeroSubGearRuneSockets(conn, null, 9999))
			{
				Guid heroId3 = SFDBUtil.ToGuid(dr3["heroId"]);
				FieldOfHonorHero hero3 = GetFieldOfHonorHeroOfHeroId(heroId3);
				if (hero3 == null)
				{
					SFLogUtil.Warn(GetType(), "[결투장영웅보조장비룬소켓 목록] 결투장영웅이 존재하지 않습니다. heroId = " + heroId3);
					continue;
				}
				int nSubGearId2 = Convert.ToInt32(dr3["subGearId"]);
				FieldOfHonorHeroEquippedSubGear equippedSubGear2 = hero3.GetEquippedSubGear(nSubGearId2);
				if (equippedSubGear2 == null)
				{
					SFLogUtil.Warn(GetType(), string.Concat("[결투장영웅부조장비룬소켓 목록] 결투장영웅장착보조장비가 존재하지 않습니다. heroId = ", heroId3, ", nSubGearId = ", nSubGearId2));
				}
				else
				{
					FieldOfHonorHeroSubGearRuneSocket subGearRuneSokcet = new FieldOfHonorHeroSubGearRuneSocket(equippedSubGear2);
					subGearRuneSokcet.Init(dr3);
					equippedSubGear2.AddRuneSocket(subGearRuneSokcet);
				}
			}
			foreach (DataRow dr2 in GameDac.FieldOfHonorHeroSubGearSoulstoneSockets(conn, null, 9999))
			{
				Guid heroId2 = SFDBUtil.ToGuid(dr2["heroId"]);
				FieldOfHonorHero hero2 = GetFieldOfHonorHeroOfHeroId(heroId2);
				if (hero2 == null)
				{
					SFLogUtil.Warn(GetType(), "[결투장영웅보조장비소울스톤소켓 목록] 결투장영웅이 존재하지 않습니다. heroId = " + heroId2);
					continue;
				}
				int nSubGearId = Convert.ToInt32(dr2["subGearId"]);
				FieldOfHonorHeroEquippedSubGear equippedSubGear = hero2.GetEquippedSubGear(nSubGearId);
				if (equippedSubGear == null)
				{
					SFLogUtil.Warn(GetType(), string.Concat("[결투장영웅보조장비소울스톤소켓 목록] 결투장영웅장착보조장비가 존재하지 않습니다. heroId = ", heroId2, ", nSubGearId = ", nSubGearId));
				}
				else
				{
					FieldOfHonorHeroSubGearSoulstoneSocket subGearSoulstonenSocket = new FieldOfHonorHeroSubGearSoulstoneSocket(equippedSubGear);
					subGearSoulstonenSocket.Init(dr2);
					equippedSubGear.AddSoulstoneSocket(subGearSoulstonenSocket);
				}
			}
			foreach (DataRow dr in GameDac.FieldOfHonorHeroRealAttrs(conn, null, 9999))
			{
				Guid heroId = SFDBUtil.ToGuid(dr["heroId"]);
				FieldOfHonorHero hero = GetFieldOfHonorHeroOfHeroId(heroId);
				if (hero == null)
				{
					SFLogUtil.Warn(GetType(), "[결투장영웅실제속성 목록] 결투장영웅이 존재하지 않습니다. heroId = " + heroId);
					continue;
				}
				FieldOfHonorHeroRealAttr realAttr = new FieldOfHonorHeroRealAttr(hero);
				realAttr.Init(dr);
				hero.AddRealAttr(realAttr);
			}
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void InitGuilds()
	{
		SqlConnection conn = null;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		DateTime currentDate = currentTime.Date;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			foreach (DataRow dr6 in GameDac.Guilds(conn, null))
			{
				Guild guild7 = new Guild();
				guild7.Init(dr6, currentTime);
				AddGuild(guild7);
			}
			foreach (DataRow dr5 in GameDac.GuildMembers(conn, null))
			{
				Guid guildId6 = (Guid)dr5["guildId"];
				Guild guild6 = GetGuild(guildId6);
				if (guild6 != null)
				{
					GuildMember member = new GuildMember(guild6);
					member.Init(dr5);
					guild6.AddMember(member);
					if (member.grade.id == 1)
					{
						guild6.master = member;
					}
				}
			}
			foreach (DataRow dr4 in GameDac.GuildApplications(conn, null))
			{
				Guid guildId5 = (Guid)dr4["guildId"];
				Guild guild5 = GetGuild(guildId5);
				if (guild5 != null)
				{
					GuildApplication application = new GuildApplication(guild5);
					application.Init(dr4);
					guild5.AddApplication(application);
				}
			}
			foreach (DataRow dr3 in GameDac.GuildBuildings(conn, null))
			{
				Guid guildId4 = (Guid)dr3["guildId"];
				Guild guild4 = GetGuild(guildId4);
				if (guild4 != null)
				{
					GuildBuildingInstance building = new GuildBuildingInstance(guild4);
					building.Init(dr3);
					guild4.AddBuildingInstance(building);
				}
			}
			foreach (Guild guild3 in m_guilds.Values)
			{
				Guid guildId3 = guild3.id;
				int nDailySupplySupportQuestCountOfDate = GameDac.GuildSupplySupportQuestCountOfDate(conn, null, guildId3, currentDate);
				guild3.RefreshDailyGuildSupplySupportQuestStartCount(currentDate);
				guild3.dailyGuildSupplySupportQuestStartCount.value = nDailySupplySupportQuestCountOfDate;
				DataRow drGuildSupplySupportQuest_OnAccept = GameDac.GuildSupplySupportQuest_OnAccept(conn, null, guildId3);
				if (drGuildSupplySupportQuest_OnAccept != null)
				{
					GuildSupplySupportQuestPlay quest = new GuildSupplySupportQuestPlay(guild3);
					quest.Init(drGuildSupplySupportQuest_OnAccept);
					guild3.SetGuildSupplySupportQuest(quest, null);
					if (quest.GetRemainingTime(currentTime) <= 0f || quest.cartHp <= 0)
					{
						quest.ProcessLifetimeEnd(currentTime, bSendFailEvent: false);
					}
				}
			}
			foreach (DataRow dr2 in GameDac.GuildDailyObjectiveComletions(conn, null, currentDate))
			{
				Guid guildId2 = (Guid)dr2["guildId"];
				Guild guild2 = GetGuild(guildId2);
				if (guild2 != null)
				{
					GuildDailyObjectiveCompletionMember completionMember = new GuildDailyObjectiveCompletionMember(guild2);
					completionMember.Init(dr2);
					guild2.AddDailyObjectiveCompletionMember(completionMember);
				}
			}
			DateTime dateOfMonday = DateTimeUtil.GetWeekStartDate(currentDate);
			foreach (DataRow dr in GameDac.GuildDailyObjectiveCompletions_WeeklyCompletionCount(conn, null, dateOfMonday))
			{
				Guid guildId = (Guid)dr["guildId"];
				Guild guild = GetGuild(guildId);
				if (guild != null)
				{
					int nCount = (guild.weeklyObjectiveCompletionMemberCount = Convert.ToInt32(dr["cnt"]));
				}
			}
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void InitProspectQuests()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			foreach (DataRow dr in GameDac.HeroProspectQuests_Progressing(conn, null))
			{
				HeroProspectQuest quest = new HeroProspectQuest();
				quest.Init(dr, DateTimeUtil.currentTime);
				if (!quest.isFailed)
				{
					AddProspectQuest(quest);
				}
			}
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void InitAnkouTombBestRecord()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			foreach (DataRow dr in GameDac.AnkouTombServerBestRecords(conn, null))
			{
				HeroAnkouTombBestRecord bestRecord = new HeroAnkouTombBestRecord();
				bestRecord.Init(dr);
				AddAnkouTombServerBestRecord(bestRecord);
			}
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
	}

	private void InitTradeShipBestRecord()
	{
		SqlConnection conn = null;
		try
		{
			conn = DBUtil.OpenGameDBConnection();
			foreach (DataRow dr in GameDac.TradeShipServerBestRecords(conn, null))
			{
				HeroTradeShipBestRecord bestRecord = new HeroTradeShipBestRecord();
				bestRecord.Init(dr);
				AddTradeShipServerBestRecord(bestRecord);
			}
			SFDBUtil.Close(ref conn);
		}
		finally
		{
			SFDBUtil.Close(ref conn);
		}
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
		Global.instance.AddWork(new SFAction(OnUpdate));
	}

	private void OnUpdate()
	{
		m_prevUpdateTime = m_currentUpdateTime;
		m_currentUpdateTime = DateTimeUtil.currentTime;
		m_fCurrentDeltaTime = (float)(m_currentUpdateTime - m_prevUpdateTime).TotalSeconds;
		if (m_prevUpdateTime.Date != m_currentUpdateTime.Date)
		{
			OnDateChanged();
		}
		m_mailDeliveryManager.OnUpdate();
		m_nationWarManager.OnUpdate(m_currentUpdateTime);
		OnUpdate_UpdateGameServer_CurrentUserCount();
		OnUpdate_SaveConnectionLog();
		ServerBattlePowerRankingManager.instance.OnUpdate();
		ServerLevelRankingManager.instance.OnUpdate();
		DailyServerLevelRankingManager.instance.OnUpdate();
		ServerJobBattlePowerRankingManager.instance.OnUpdate();
		NationBattlePowerRankingManager.instance.OnUpdate();
		NationExploitPointRankingManager.instance.OnUpdate();
		DailyFieldOfHonorRankingManager.instance.OnUpdate();
		ServerGuildRankingManager.instance.OnUpdate();
		NationGuildRankingManager.instance.OnUpdate();
		ServerCreatureCardRankingManager.instance.OnUpdate();
		ServerIllustratedBookRankingManager.instance.OnUpdate();
		ServerPresentContributionPointRankingManager.instance.OnUpdate();
		NationWeeklyPresentContributionPointRankingManager.instance.OnUpdate();
		ServerPresentPopularityPointRankingManager.instance.OnUpdate();
		NationWeeklyPresentPopularityPointRankingManager.instance.OnUpdate();
		ServerNationPowerRankingManager.instance.OnUpdate();
		foreach (Guild guild in m_guilds.Values)
		{
			guild.OnUpdate(m_currentUpdateTime);
		}
		OnUpdate_RefreshCreatureCardShop();
		OnUpdate_StaminaRecovery();
		OnUpdate_RefreshProofOfValorSchedule();
		OnUpdate_CheckFieldBossStartSchedule();
		OnUpdate_CheckFieldBossEndSchedule();
		OnUpdate_CheckTimeDesignationEvent();
	}

	private void OnDateChanged()
	{
		ServerEvent.SendDateChanged(GetClientPeers(Guid.Empty), m_currentUpdateTime);
		foreach (NationInstance nationInst in m_nationInsts.Values)
		{
			nationInst.OnDateChanged();
		}
		foreach (Guild guild in m_guilds.Values)
		{
			guild.OnDateChanged(m_currentUpdateTime);
		}
	}

	private void OnUpdate_UpdateGameServer_CurrentUserCount()
	{
		try
		{
			UpdateGameServer_CurrentUserCount();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void UpdateGameServer_CurrentUserCount()
	{
		if (m_prevUpdateTime.Second / 10 != m_currentUpdateTime.Second / 10)
		{
			int nClientCount = GameServerApp.inst.clientPeerCount;
			DBWork_UpdateGameServer_CurrentUserCount(nClientCount);
		}
	}

	private void DBWork_UpdateGameServer_CurrentUserCount(int nCurrentUserCount)
	{
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateUserDBContentWork(QueuingWorkContentId.UserDB_GameServer);
		dbWork.AddSqlCommand(UserDac.CSC_UpdateGameServer_CurrentUserCount(GameServerApp.inst.serverId, nCurrentUserCount));
		dbWork.Schedule();
	}

	private void OnUpdate_SaveConnectionLog()
	{
		try
		{
			SaveConnectionLog();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void SaveConnectionLog()
	{
		if (m_prevUpdateTime.Second / 10 == m_currentUpdateTime.Second / 10)
		{
			return;
		}
		Guid logId = Guid.NewGuid();
		SFSqlStandaloneWork dbWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
		int nClientCount = GameServerApp.inst.clientPeerCount;
		int nHeroCount = m_heroes.Count;
		dbWork.AddSqlCommand(GameLogDac.CSC_AddConnectionLog(logId, nClientCount, nHeroCount, m_currentUpdateTime));
		foreach (NationInstance nationInst in m_nationInsts.Values)
		{
			int nNationHeroCount = nationInst.heroCount;
			dbWork.AddSqlCommand(GameLogDac.CSC_AddNationConnectionLog(logId, nationInst.nationId, nNationHeroCount));
		}
		dbWork.Schedule();
	}

	public void OnTearDown()
	{
		Account[] array = m_accounts.Values.ToArray();
		foreach (Account account in array)
		{
			AccountSynchronizer.Exec(account, new SFAction(account.LogOut));
		}
		DBWork_UpdateGameServer_CurrentUserCount(0);
	}

	public void Release()
	{
		if (m_bReleased)
		{
			return;
		}
		DisposeUpdateTimer();
		foreach (Account account in m_accounts.Values)
		{
			lock (account.syncObject)
			{
				account.Release();
			}
		}
		foreach (CartInstance cartInst in m_cartInsts.Values)
		{
			lock (cartInst.syncObject)
			{
				cartInst.Release();
			}
		}
		foreach (Party party in m_parties.Values)
		{
			party.Release();
		}
		foreach (Guild guild in m_guilds.Values)
		{
			guild.Release();
		}
		if (m_nationWarManager != null)
		{
			m_nationWarManager.Release();
		}
		foreach (Place place in m_places.Values)
		{
			lock (place.syncObject)
			{
				place.Release();
			}
		}
		if (m_ancientRelicMatchingManager != null)
		{
			m_ancientRelicMatchingManager.Release();
		}
		if (m_soulCoveterMatchingManager != null)
		{
			m_soulCoveterMatchingManager.Release();
		}
		if (m_ruinsReclaimMatchingManager != null)
		{
			m_ruinsReclaimMatchingManager.Release();
		}
		if (m_infiniteWarMatchingManager != null)
		{
			m_infiniteWarMatchingManager.Release();
		}
		if (m_fearAltarMatchingManager != null)
		{
			m_fearAltarMatchingManager.Release();
		}
		if (m_warMemoryMatchingManager != null)
		{
			m_warMemoryMatchingManager.Release();
		}
		if (m_dragonNestMatchingManager != null)
		{
			m_dragonNestMatchingManager.Release();
		}
		if (m_ankouTombMatchingManager != null)
		{
			m_ankouTombMatchingManager.Release();
		}
		if (m_tradeShipMatchingManager != null)
		{
			m_tradeShipMatchingManager.Release();
		}
		lock (GlobalNoticeMananger.instance.syncObject)
		{
			GlobalNoticeMananger.instance.Release();
		}
		lock (ServerNoticeManager.instance.syncObject)
		{
			ServerNoticeManager.instance.Release();
		}
		foreach (HeroProspectQuest quest in m_prospectQuests.Values)
		{
			quest.Release();
		}
		m_bReleased = true;
	}

	public void AddAccount(Account account)
	{
		if (account == null)
		{
			throw new ArgumentNullException("account");
		}
		m_accounts.Add(account.id, account);
	}

	public void RemoveAccount(Guid id)
	{
		m_accounts.Remove(id);
	}

	public Account GetAccount(Guid id)
	{
		if (!m_accounts.TryGetValue(id, out var account))
		{
			return null;
		}
		return account;
	}

	public void AddHero(Hero hero)
	{
		m_heroes.Add(hero.id, hero);
	}

	public void RemoveHero(Guid id)
	{
		m_heroes.Remove(id);
	}

	public Hero GetHero(Guid id)
	{
		if (!m_heroes.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public Hero GetLoggedInHero(Guid id)
	{
		if (m_heroes.TryGetValue(id, out var value) && value.isLoggedIn)
		{
			return value;
		}
		return null;
	}

	public List<ClientPeer> GetClientPeers(Guid heroIdToExclude)
	{
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.isLoggedIn && hero.id != heroIdToExclude)
			{
				clientPeers.Add(hero.account.peer);
			}
		}
		return clientPeers;
	}

	public List<ClientPeer> GetClientPeers_ByExcludeNationIds(List<int> nationIdsToExclude)
	{
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.isLoggedIn && !nationIdsToExclude.Contains(hero.nationId))
			{
				clientPeers.Add(hero.account.peer);
			}
		}
		return clientPeers;
	}

	public void AddNationInstance(NationInstance nationInst)
	{
		if (nationInst == null)
		{
			throw new ArgumentNullException("nationInst");
		}
		m_nationInsts.Add(nationInst.nationId, nationInst);
	}

	public NationInstance GetNationInstance(int nNationId)
	{
		if (!m_nationInsts.TryGetValue(nNationId, out var value))
		{
			return null;
		}
		return value;
	}

	public int GetHeroCreationDefaultNationId()
	{
		NationInstance targetNationInst = null;
		foreach (NationInstance nationInst in m_nationInsts.Values)
		{
			if (targetNationInst == null)
			{
				targetNationInst = nationInst;
				continue;
			}
			int nNationHeroCount = nationInst.nationHeroCount;
			int nTargetNationHeroCount = targetNationInst.nationHeroCount;
			if (nNationHeroCount < nTargetNationHeroCount)
			{
				targetNationInst = nationInst;
			}
			else if (nNationHeroCount == nTargetNationHeroCount && nationInst.nationId < targetNationInst.nationId)
			{
				targetNationInst = nationInst;
			}
		}
		return targetNationInst.nationId;
	}

	public int GetHeroCreationDefaultNationIdWithLock()
	{
		lock (Global.syncObject)
		{
			return GetHeroCreationDefaultNationId();
		}
	}

	public void AddPlace(Place place)
	{
		if (place == null)
		{
			throw new ArgumentNullException("place");
		}
		m_places.Add(place.instanceId, place);
	}

	public Place GetPlace(Guid instanceId)
	{
		if (!m_places.TryGetValue(instanceId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool RemovePlace(Guid placeInstanceId)
	{
		return m_places.Remove(placeInstanceId);
	}

	private void AddDisputeContinentInstance(DisputeContinentInstance disputeContinentInst)
	{
		if (disputeContinentInst == null)
		{
			throw new ArgumentNullException("disputeContinentInst");
		}
		m_disputeContinentInsts.Add(disputeContinentInst.continent.id, disputeContinentInst);
	}

	public DisputeContinentInstance GetDisputeContinentInstance(int nContinentId)
	{
		if (!m_disputeContinentInsts.TryGetValue(nContinentId, out var value))
		{
			return null;
		}
		return value;
	}

	public FieldOfHonorHero GetFieldOfHonorHero(int nRanking)
	{
		if (nRanking < 0 || nRanking > 9999)
		{
			return null;
		}
		return m_fieldOfHonorRankers[nRanking - 1];
	}

	public FieldOfHonorHero GetFieldOfHonorHeroOfHeroId(Guid heroId)
	{
		FieldOfHonorHero[] fieldOfHonorRankers = m_fieldOfHonorRankers;
		foreach (FieldOfHonorHero ranker in fieldOfHonorRankers)
		{
			if (ranker != null && ranker.id == heroId)
			{
				return ranker;
			}
		}
		return null;
	}

	public PDFieldOfHonorHero GetPDFieldOfHonorHero(int nRanking)
	{
		return GetFieldOfHonorHero(nRanking)?.ToPDFieldOfHonorHero();
	}

	public PDFieldOfHonorRanking GetPDFieldOfHonorRanking(int nRanking)
	{
		return GetFieldOfHonorHero(nRanking)?.ToPDFieldOfHonorRanking();
	}

	public void SetFieldOfHonorRanker(FieldOfHonorHero ranker)
	{
		if (ranker == null)
		{
			throw new ArgumentNullException("ranker");
		}
		int nRanking = ranker.ranking;
		if (nRanking >= 0 && nRanking <= 9999)
		{
			m_fieldOfHonorRankers[nRanking - 1] = ranker;
			if (nRanking > m_nCurrentFieldOfHonorMaxRanking)
			{
				m_nCurrentFieldOfHonorMaxRanking = nRanking;
			}
		}
	}

	public void AddParty(Party party)
	{
		if (party == null)
		{
			throw new ArgumentNullException("party");
		}
		m_parties.Add(party.id, party);
	}

	public void RemoveParty(Guid id)
	{
		m_parties.Remove(id);
	}

	public Party GetParty(Guid id)
	{
		Party value = null;
		if (!m_parties.TryGetValue(id, out value))
		{
			return null;
		}
		return value;
	}

	public void AddPartyMember(PartyMember member)
	{
		if (member == null)
		{
			throw new ArgumentNullException("member");
		}
		m_partyMembers.Add(member.id, member);
	}

	public void RemovePartyMember(Guid id)
	{
		m_partyMembers.Remove(id);
	}

	public PartyMember GetPartyMember(Guid id)
	{
		if (!m_partyMembers.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddCartInstance(CartInstance cartInst)
	{
		if (cartInst == null)
		{
			throw new ArgumentNullException("cartInst");
		}
		m_cartInsts.Add(cartInst.instanceId, cartInst);
	}

	public CartInstance GetCartInstance(long lnInstanceId)
	{
		if (!m_cartInsts.TryGetValue(lnInstanceId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveCartInstance(CartInstance cartInst)
	{
		if (cartInst != null && m_cartInsts.Remove(cartInst.instanceId))
		{
			cartInst.Release();
		}
	}

	public void AddGuild(Guild guild)
	{
		if (guild == null)
		{
			throw new ArgumentNullException("guild");
		}
		m_guilds.Add(guild.id, guild);
		NationInstance nationInst = guild.nationInst;
		nationInst.AddGuild(guild);
	}

	public Guild GetGuild(Guid id)
	{
		if (!m_guilds.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveGuild(Guid id)
	{
		Guild guild = GetGuild(id);
		NationInstance nationInst = guild.nationInst;
		nationInst.RemoveGuild(id);
		m_guilds.Remove(id);
	}

	public void AddGuildMember(GuildMember member)
	{
		if (member == null)
		{
			throw new ArgumentNullException("member");
		}
		m_guildMembers.Add(member.id, member);
	}

	public GuildMember GetGuildMember(Guid heroId)
	{
		if (!m_guildMembers.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveGuildMember(Guid heroId)
	{
		m_guildMembers.Remove(heroId);
	}

	public void AddGuildApplication(GuildApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		m_guildApplications.Add(app.id, app);
		GetOrCreateHeroGuildApplicationCollection(app.heroId).Add(app);
	}

	public GuildApplication GetGuildApplication(Guid id)
	{
		if (!m_guildApplications.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveGuildApplication(GuildApplication app)
	{
		if (app != null)
		{
			m_guildApplications.Remove(app.id);
			HeroGuildApplicationCollection col = GetHeroGuildApplicationCollection(app.heroId);
			col.Remove(app.id);
			if (col.applications.Count == 0)
			{
				RemoveHeroGuildApplicationCollection(app.heroId);
			}
		}
	}

	public void CancelAllGuildApplicationsOfHero(Guid heroId)
	{
		GetHeroGuildApplicationCollection(heroId)?.CancelAll();
	}

	public HeroGuildApplicationCollection GetHeroGuildApplicationCollection(Guid heroId)
	{
		if (!m_heroGuildApplicationCollections.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	public HeroGuildApplicationCollection GetOrCreateHeroGuildApplicationCollection(Guid heroId)
	{
		HeroGuildApplicationCollection inst = GetHeroGuildApplicationCollection(heroId);
		if (inst == null)
		{
			inst = new HeroGuildApplicationCollection(heroId);
			m_heroGuildApplicationCollections.Add(inst.heroId, inst);
		}
		return inst;
	}

	public void RemoveHeroGuildApplicationCollection(Guid heroId)
	{
		m_heroGuildApplicationCollections.Remove(heroId);
	}

	public void OnUpdate_RefreshCreatureCardShop()
	{
		try
		{
			RefreshCreatureCardShop();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public void RefreshCreatureCardShop()
	{
		DateTime currentDate = m_currentUpdateTime.Date;
		CreatureCardShopRefreshSchedule targetSchedule = null;
		foreach (CreatureCardShopRefreshSchedule schedule in Resource.instance.creatureCardShopRefreshSchedules)
		{
			DateTimeOffset targetTime = currentDate.AddSeconds(schedule.refreshTime);
			if (m_prevUpdateTime < targetTime && m_currentUpdateTime >= targetTime)
			{
				targetSchedule = schedule;
				break;
			}
		}
		if (targetSchedule == null)
		{
			return;
		}
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.isLoggedIn)
			{
				lock (hero.syncObject)
				{
					hero.RefreshCreatureCardShopOnSchedule(currentDate, targetSchedule.id, bSendEvent: true, DateTimeUtil.currentTime);
				}
			}
		}
	}

	private void OnUpdate_StaminaRecovery()
	{
		try
		{
			RecoveryStaminaOnSchedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	public void RecoveryStaminaOnSchedule()
	{
		DateTime currentDate = m_currentUpdateTime.Date;
		StaminaRecoverySchedule targetSchedule = null;
		foreach (StaminaRecoverySchedule schedule in Resource.instance.staminaRecoverySchedules)
		{
			DateTimeOffset targetTime = currentDate.AddSeconds(schedule.recoveryTime);
			if (m_prevUpdateTime < targetTime && m_currentUpdateTime >= targetTime)
			{
				targetSchedule = schedule;
				break;
			}
		}
		if (targetSchedule == null)
		{
			return;
		}
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.isLoggedIn)
			{
				lock (hero.syncObject)
				{
					hero.RecoveryStaminaOnSchedule(currentDate, targetSchedule.id, targetSchedule.recoveryStamina, bSendEvent: true, m_currentUpdateTime);
				}
			}
		}
	}

	private void OnUpdate_RefreshProofOfValorSchedule()
	{
		try
		{
			RefreshProofOfValorSchedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void RefreshProofOfValorSchedule()
	{
		ProofOfValor proofOfValor = Resource.instance.proofOfValor;
		if (proofOfValor == null)
		{
			return;
		}
		DateTime currentDate = m_currentUpdateTime.Date;
		ProofOfValorRefreshSchedule targetSchedule = null;
		foreach (ProofOfValorRefreshSchedule schedule in proofOfValor.refreshSchedules)
		{
			DateTimeOffset targetTime = currentDate.AddSeconds(schedule.refreshTime);
			if (m_prevUpdateTime < targetTime && m_currentUpdateTime >= targetTime)
			{
				targetSchedule = schedule;
				break;
			}
		}
		if (targetSchedule == null)
		{
			return;
		}
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.isLoggedIn)
			{
				lock (hero.syncObject)
				{
					hero.RefreshProofOfValorAutoRefreshSchedule(currentDate, targetSchedule.id, bSendEvent: true, m_currentUpdateTime);
				}
			}
		}
	}

	public void SendNoticeAsync(string sContent)
	{
		Global.instance.AddWork(new SFAction<string>(SendNotice, sContent));
	}

	private void SendNotice(string sContent)
	{
		ServerEvent.SendNotice(GetClientPeers(Guid.Empty), sContent);
	}

	public void UpdateServerMaxLevel(int nLevel, bool bSendEvent)
	{
		m_nServerMaxLevel = nLevel;
		if (bSendEvent)
		{
			ServerEvent.SendServerMaxLevelUpdated(GetClientPeers(Guid.Empty), m_nServerMaxLevel);
		}
	}

	public float GetWorldLevelExpFactor(int nLevel)
	{
		if (nLevel <= 0)
		{
			throw new ArgumentOutOfRangeException("nLevel");
		}
		float fDefaultFactor = 1f;
		Resource resource = Resource.instance;
		if (nLevel < resource.worldLevelExpBuffMinHeroLevel)
		{
			return fDefaultFactor;
		}
		WorldLevelExpFactor worldLevelExpFactor = resource.GetWorldLevelExpFactor(m_nServerMaxLevel - nLevel);
		if (worldLevelExpFactor == null)
		{
			return fDefaultFactor;
		}
		return fDefaultFactor + worldLevelExpFactor.expFactor;
	}

	private void OnUpdate_CheckFieldBossStartSchedule()
	{
		try
		{
			CheckFieldBossStartSchedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void CheckFieldBossStartSchedule()
	{
		FieldBossEvent fieldBossEvent = Resource.instance.fieldBossEvent;
		if (fieldBossEvent == null)
		{
			return;
		}
		List<FieldBossEventSchedule> schedules = fieldBossEvent.schedules;
		if (schedules.Count <= 0)
		{
			return;
		}
		DateTime currentDate = m_currentUpdateTime.Date;
		FieldBossEventSchedule targetSchedule = null;
		foreach (FieldBossEventSchedule schedule in schedules)
		{
			DateTimeOffset targetTime = currentDate.AddSeconds(schedule.startTime);
			if (targetTime > m_prevUpdateTime && targetTime <= m_currentUpdateTime)
			{
				targetSchedule = schedule;
				break;
			}
		}
		if (targetSchedule == null)
		{
			return;
		}
		m_currentFieldBossEventSchedule = targetSchedule;
		foreach (FieldBoss fieldBoss in fieldBossEvent.fieldBosses.Values)
		{
			Continent continent = fieldBoss.continent;
			if (continent.isNationTerritory)
			{
				foreach (NationInstance nationInst in m_nationInsts.Values)
				{
					NationContinentInstance nationContinentInst = nationInst.GetContinentInstance(continent.id);
					if (nationContinentInst != null)
					{
						nationContinentInst.SpawnFieldBoss(fieldBoss, targetSchedule, m_currentUpdateTime);
						continue;
					}
					break;
				}
			}
			else
			{
				GetDisputeContinentInstance(continent.id)?.SpawnFieldBoss(fieldBoss, targetSchedule, m_currentUpdateTime);
			}
		}
		ServerEvent.SendFieldBossEventStarted(GetClientPeers(Guid.Empty));
	}

	private void OnUpdate_CheckFieldBossEndSchedule()
	{
		try
		{
			CheckFieldBossEndSchedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void CheckFieldBossEndSchedule()
	{
		if (m_currentFieldBossEventSchedule == null)
		{
			return;
		}
		DateTimeOffset endTime = m_currentUpdateTime.Date.AddSeconds(m_currentFieldBossEventSchedule.endTime);
		if (endTime > m_currentUpdateTime)
		{
			return;
		}
		m_currentFieldBossEventSchedule = null;
		foreach (NationInstance nationInst in m_nationInsts.Values)
		{
			foreach (NationContinentInstance nationContinentInst in nationInst.continentInsts.Values)
			{
				nationContinentInst.ClearFieldBoss();
			}
		}
		foreach (DisputeContinentInstance disputeContinentInst in m_disputeContinentInsts.Values)
		{
			disputeContinentInst.ClearFieldBoss();
		}
		ServerEvent.SendFieldBossEventEnded(GetClientPeers(Guid.Empty));
	}

	public List<int> GetSpawnedFieldBosses(int nNationId)
	{
		NationInstance nationinst = GetNationInstance(nNationId);
		if (nationinst == null)
		{
			throw new ArgumentException("nNationId");
		}
		List<int> spawnedFieldBosses = new List<int>();
		foreach (NationContinentInstance nationContinentInst in nationinst.continentInsts.Values)
		{
			foreach (FieldBossMonsterInstance monsterInst2 in nationContinentInst.fieldBossMonsterInsts.Values)
			{
				spawnedFieldBosses.Add(monsterInst2.fieldBoss.id);
			}
		}
		foreach (DisputeContinentInstance disputeContinentInst in m_disputeContinentInsts.Values)
		{
			foreach (FieldBossMonsterInstance monsterInst in disputeContinentInst.fieldBossMonsterInsts.Values)
			{
				spawnedFieldBosses.Add(monsterInst.fieldBoss.id);
			}
		}
		return spawnedFieldBosses;
	}

	public List<Hero> SearchHeroesByName(string sText, Guid heroIdToExclude)
	{
		if (string.IsNullOrEmpty(sText))
		{
			throw new Exception("검색어가 유효하지 않습니다.");
		}
		List<Hero> results = new List<Hero>();
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.isLoggedIn && !(hero.id == heroIdToExclude) && hero.name.IndexOf(sText, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				results.Add(hero);
			}
		}
		return results;
	}

	public void OnHeroLevelUp(Hero hero, int nOldLevel)
	{
		SFLogUtil.Info(GetType(), "OnHeroLevelUp : " + hero.name + ", " + nOldLevel);
		Global.instance.AddWork(new SFAction<Guid, int, int, string, int>(OnHeroLevelUpAsync_BlessingQuest, hero.id, nOldLevel, hero.level, hero.name, hero.nationId));
	}

	private void OnHeroLevelUpAsync_BlessingQuest(Guid heroId, int nOldLevel, int nNewLevel, string sName, int nNationId)
	{
		SFLogUtil.Info(GetType(), "OnHeroLevelUpAsync_BlessingQuest : " + sName + ", " + nOldLevel + ", " + nNewLevel);
		List<BlessingTargetLevel> targetLevels = new List<BlessingTargetLevel>();
		for (int nLevel = nOldLevel; nLevel <= nNewLevel; nLevel++)
		{
			BlessingTargetLevel targetLevel = Resource.instance.GetBlessingTargetLevelByLevel(nLevel);
			if (targetLevel != null)
			{
				targetLevels.Add(targetLevel);
			}
		}
		if (targetLevels.Count == 0)
		{
			return;
		}
		SFLogUtil.Info(GetType(), "targetLevels.Count = " + targetLevels.Count);
		NationInstance nationInst = GetNationInstance(nNationId);
		foreach (Hero other in nationInst.heroes.Values)
		{
			if (other.id == heroId)
			{
				continue;
			}
			lock (other.syncObject)
			{
				if (other.level >= Resource.instance.blessingQuestRequiredHeroLevel)
				{
					other.StartBlessingQuest(heroId, sName, targetLevels);
				}
			}
		}
	}

	public void AddProspectQuest(HeroProspectQuest quest)
	{
		if (quest == null)
		{
			throw new ArgumentNullException("quest");
		}
		m_prospectQuests.Add(quest.instanceId, quest);
	}

	public void RemoveProspectQuest(Guid instanceId)
	{
		m_prospectQuests.Remove(instanceId);
	}

	public HeroProspectQuest GetProspectQuest(Guid instanceId)
	{
		if (!m_prospectQuests.TryGetValue(instanceId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDNationInstance> GetPDNationInstances()
	{
		List<PDNationInstance> results = new List<PDNationInstance>();
		foreach (NationInstance nationInst in m_nationInsts.Values)
		{
			results.Add(nationInst.ToPDNationInstance());
		}
		return results;
	}

	public List<PDNationPowerRanking> GetPDNationPowerRankings()
	{
		List<PDNationPowerRanking> results = new List<PDNationPowerRanking>();
		foreach (NationInstance nationInst in m_nationInsts.Values)
		{
			NationPowerRanking ranking = nationInst.nationPowerRanking;
			if (ranking != null)
			{
				results.Add(ranking.ToPDNationPowerRanking());
			}
		}
		return results;
	}

	public List<PDNationAlliance> GetPDNationAlliances(DateTimeOffset time)
	{
		List<PDNationAlliance> results = new List<PDNationAlliance>();
		foreach (NationAlliance nationAlliance in m_nationAlliances.Values)
		{
			results.Add(nationAlliance.ToPDNationAlliance(time));
		}
		return results;
	}

	public void AddNationAllianceApplication(NationAllianceApplication nationAllianceApplication)
	{
		m_nationAllianceApplications.Add(nationAllianceApplication.id, nationAllianceApplication);
	}

	public void RemoveNationAllianceApplication(Guid id)
	{
		m_nationAllianceApplications.Remove(id);
	}

	public NationAllianceApplication GetNationAllianceApplication(Guid id)
	{
		if (!m_nationAllianceApplications.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public NationAllianceApplication GetNationAllianceApplication_ByNationIdAndTargetNationId(int nNationId, int nTargetNationId)
	{
		foreach (NationAllianceApplication nationAllianceApplication in m_nationAllianceApplications.Values)
		{
			if (nationAllianceApplication.nationId == nNationId && nationAllianceApplication.targetNationId == nTargetNationId)
			{
				return nationAllianceApplication;
			}
		}
		return null;
	}

	public List<PDNationAllianceApplication> GetPDNationAllianceApplicationsOfNation(int nNationId)
	{
		List<PDNationAllianceApplication> results = new List<PDNationAllianceApplication>();
		foreach (NationAllianceApplication nationAllianceApplication in m_nationAllianceApplications.Values)
		{
			if (nationAllianceApplication.nationId == nNationId || nationAllianceApplication.targetNationId == nNationId)
			{
				results.Add(nationAllianceApplication.ToPDNationAllianceApplication());
			}
		}
		return results;
	}

	public void CancelNationAllianceApplication(NationAllianceApplication application, int nCancelNationId, DateTimeOffset time)
	{
		if (application == null)
		{
			throw new ArgumentNullException("application");
		}
		Guid nationAllianceApplicationId = application.id;
		RemoveNationAllianceApplication(nationAllianceApplicationId);
		NationInstance nationInst = application.nationInst;
		NationInstance targetNationInst = application.targetNationInst;
		if (nCancelNationId == application.nationId)
		{
			ServerEvent.SendNationAllianceApplicationCanceled(nationInst.GetClientPeers(Guid.Empty), nationAllianceApplicationId);
			ServerEvent.SendNationAllianceApplicationCanceled(targetNationInst.GetClientPeers(Guid.Empty), nationAllianceApplicationId);
		}
		else
		{
			ServerEvent.SendNationAllianceApplicationRejected(nationInst.GetClientPeers(Guid.Empty), nationAllianceApplicationId);
			ServerEvent.SendNationAllianceApplicationRejected(targetNationInst.GetClientPeers(Guid.Empty), nationAllianceApplicationId);
		}
		nationInst.AddFund(application.fund, Guid.Empty);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateNationWork(nationInst.nationId);
		dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateNationWork(targetNationInst.nationId));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationAllianceApplication(nationAllianceApplicationId, 2, time));
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationInstance_Fund(nationInst.nationId, nationInst.fund));
		dbWork.Schedule();
	}

	public void CancelNationAllianceApplicationOfNationInstance(NationInstance nationInst, DateTimeOffset time, Guid heroIdToExclude)
	{
		if (nationInst == null)
		{
			throw new ArgumentNullException("nationInst");
		}
		int nNationId = nationInst.nationId;
		long lnReturnFund = 0L;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateNationWork(nNationId);
		NationAllianceApplication[] array = m_nationAllianceApplications.Values.ToArray();
		foreach (NationAllianceApplication nationAllianceApplication in array)
		{
			Guid nationAllianceApplicationId = nationAllianceApplication.id;
			if (nationAllianceApplication.nationId == nNationId)
			{
				lnReturnFund += nationAllianceApplication.fund;
				RemoveNationAllianceApplication(nationAllianceApplicationId);
				ServerEvent.SendNationAllianceApplicationCanceled(nationInst.GetClientPeers(heroIdToExclude), nationAllianceApplicationId);
				ServerEvent.SendNationAllianceApplicationCanceled(nationAllianceApplication.targetNationInst.GetClientPeers(Guid.Empty), nationAllianceApplicationId);
				dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateNationWork(nationAllianceApplication.targetNationId));
				dbWork.AddSqlCommand(GameDac.CSC_UpdateNationAllianceApplication(nationAllianceApplicationId, 2, time));
			}
			else if (nationAllianceApplication.targetNationId == nNationId)
			{
				nationAllianceApplication.nationInst.AddFund(nationAllianceApplication.fund, Guid.Empty);
				RemoveNationAllianceApplication(nationAllianceApplication.id);
				ServerEvent.SendNationAllianceApplicationRejected(nationAllianceApplication.nationInst.GetClientPeers(Guid.Empty), nationAllianceApplicationId);
				ServerEvent.SendNationAllianceApplicationRejected(nationInst.GetClientPeers(heroIdToExclude), nationAllianceApplicationId);
				int nOtherNationId = nationAllianceApplication.nationId;
				dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateNationWork(nOtherNationId));
				dbWork.AddSqlCommand(GameDac.CSC_UpdateNationInstance_Fund(nOtherNationId, nationAllianceApplication.nationInst.fund));
				dbWork.AddSqlCommand(GameDac.CSC_UpdateNationAllianceApplication(nationAllianceApplicationId, 2, time));
			}
		}
		nationInst.AddFund(lnReturnFund, heroIdToExclude);
		if (lnReturnFund > 0)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateNationInstance_Fund(nNationId, lnReturnFund));
		}
		dbWork.Schedule();
	}

	public void AddNationAlliance(NationAlliance nationAlliance)
	{
		if (nationAlliance == null)
		{
			throw new ArgumentNullException("nationAlliance");
		}
		m_nationAlliances.Add(nationAlliance.id, nationAlliance);
	}

	public NationAlliance GetNationAlliance(Guid nationAllianceId)
	{
		if (!m_nationAlliances.TryGetValue(nationAllianceId, out var value))
		{
			return null;
		}
		return value;
	}

	private void RemoveNationAlliance(Guid nationAllianceId)
	{
		m_nationAlliances.Remove(nationAllianceId);
	}

	public void ConcludeNationAlliance(NationAlliance nationAlliance, Guid heroIdToExclude)
	{
		if (nationAlliance == null)
		{
			throw new ArgumentNullException("alliance");
		}
		AddNationAlliance(nationAlliance);
		foreach (Place place in m_places.Values)
		{
			lock (place.syncObject)
			{
				place.AddAlliance(nationAlliance.ToPlaceAlliance());
			}
		}
		ServerEvent.SendNationAllianceConcluded(GetClientPeers(heroIdToExclude), nationAlliance.ToPDNationAlliance(nationAlliance.regTime));
	}

	public void BreakNationAlliance(NationAlliance nationAlliance, Guid heroIdToExclude)
	{
		if (nationAlliance == null)
		{
			throw new ArgumentNullException("nationAlliance");
		}
		RemoveNationAlliance(nationAlliance.id);
		foreach (Place place in m_places.Values)
		{
			lock (place.syncObject)
			{
				place.RemoveAlliance(nationAlliance.id);
			}
		}
		ServerEvent.SendNationAllianceBroken(GetClientPeers(heroIdToExclude), nationAlliance.id);
	}

	public void AddAnkouTombServerBestRecord(HeroAnkouTombBestRecord bestRecord)
	{
		if (bestRecord == null)
		{
			throw new ArgumentNullException("bestRecord");
		}
		m_ankouTombServerBestRecords.Add(bestRecord.difficulty, bestRecord);
	}

	public void RemoveAnkouTombServerBestRecord(int nDifficulty)
	{
		m_ankouTombServerBestRecords.Remove(nDifficulty);
	}

	public HeroAnkouTombBestRecord GetAnkouTombServerBestRecord(int nDifficulty)
	{
		if (!m_ankouTombServerBestRecords.TryGetValue(nDifficulty, out var value))
		{
			return null;
		}
		return value;
	}

	public void RefreshAnkouTombServerBestRecord(HeroAnkouTombPoint heroPoint)
	{
		if (heroPoint == null)
		{
			throw new ArgumentNullException("heroPoint");
		}
		int nDifficulty = heroPoint.difficulty;
		HeroAnkouTombBestRecord oldBestRecord = GetAnkouTombServerBestRecord(nDifficulty);
		if (oldBestRecord != null)
		{
			if (heroPoint.point <= oldBestRecord.point)
			{
				return;
			}
			RemoveAnkouTombServerBestRecord(nDifficulty);
		}
		HeroAnkouTombBestRecord newBestRecord = new HeroAnkouTombBestRecord();
		newBestRecord.Init(heroPoint);
		AddAnkouTombServerBestRecord(newBestRecord);
		ServerEvent.SendAnkouTombServerBestRecordUpdated(GetClientPeers(Guid.Empty), newBestRecord.ToPDHeroAnkouTombBestRecord());
	}

	public List<PDHeroAnkouTombBestRecord> GetPDAnkouTombServerBestRecords()
	{
		List<PDHeroAnkouTombBestRecord> results = new List<PDHeroAnkouTombBestRecord>();
		foreach (HeroAnkouTombBestRecord record in m_ankouTombServerBestRecords.Values)
		{
			results.Add(record.ToPDHeroAnkouTombBestRecord());
		}
		return results;
	}

	public void AddTradeShipServerBestRecord(HeroTradeShipBestRecord bestRecord)
	{
		if (bestRecord == null)
		{
			throw new ArgumentNullException("bestRecord");
		}
		m_tradeShipServerBestRecords.Add(bestRecord.difficulty, bestRecord);
	}

	public void RemoveTradeShipServerBestRecord(int nDifficulty)
	{
		m_tradeShipServerBestRecords.Remove(nDifficulty);
	}

	public HeroTradeShipBestRecord GetTradeShipServerBestRecord(int nDifficulty)
	{
		if (!m_tradeShipServerBestRecords.TryGetValue(nDifficulty, out var value))
		{
			return null;
		}
		return value;
	}

	public void RefreshTradeShipServerBestRecord(HeroTradeShipPoint heroPoint)
	{
		if (heroPoint == null)
		{
			throw new ArgumentNullException("heroPoint");
		}
		int nDifficulty = heroPoint.difficulty;
		HeroTradeShipBestRecord oldBestRecord = GetTradeShipServerBestRecord(nDifficulty);
		if (oldBestRecord != null)
		{
			if (heroPoint.point <= oldBestRecord.point)
			{
				return;
			}
			RemoveTradeShipServerBestRecord(nDifficulty);
		}
		HeroTradeShipBestRecord newBestRecord = new HeroTradeShipBestRecord();
		newBestRecord.Init(heroPoint);
		AddTradeShipServerBestRecord(newBestRecord);
		ServerEvent.SendTradeShipServerBestRecordUpdated(GetClientPeers(Guid.Empty), newBestRecord.ToPDHeroTradeShipBestRecord());
	}

	public List<PDHeroTradeShipBestRecord> GetPDTradeShipServerBestRecords()
	{
		List<PDHeroTradeShipBestRecord> results = new List<PDHeroTradeShipBestRecord>();
		foreach (HeroTradeShipBestRecord record in m_tradeShipServerBestRecords.Values)
		{
			results.Add(record.ToPDHeroTradeShipBestRecord());
		}
		return results;
	}

	private void OnUpdate_CheckTimeDesignationEvent()
	{
		try
		{
			CheckTimeDesignationEvent();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void CheckTimeDesignationEvent()
	{
		DateTimeOffset currentUpdateTime = m_currentUpdateTime;
		List<TimeDesignationEvent> timeDesignationEvents = new List<TimeDesignationEvent>();
		foreach (TimeDesignationEvent evt in Resource.instance.timeDesignationEvents.Values)
		{
			DateTimeOffset targetTime = evt.startTime;
			if (m_prevUpdateTime < targetTime && m_currentUpdateTime >= targetTime)
			{
				timeDesignationEvents.Add(evt);
			}
		}
		if (timeDesignationEvents.Count == 0)
		{
			return;
		}
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.isLoggedIn)
			{
				lock (hero.syncObject)
				{
					hero.ReceiveTimeDesignationEventRewards(timeDesignationEvents, bSendEvent: true, currentUpdateTime);
				}
			}
		}
	}
}
