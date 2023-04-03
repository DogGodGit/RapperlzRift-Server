using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class NationInstance
{
	private Nation m_nation;

	private long m_lnFund;

	private DateValuePair<int> m_dailyNationWarCallCount = new DateValuePair<int>();

	private DateValuePair<int> m_dailyConvergingAttackCount = new DateValuePair<int>();

	private int m_nNationWarPoint;

	private NationAlliance m_alliance;

	private Dictionary<int, NationContinentInstance> m_continentInsts = new Dictionary<int, NationContinentInstance>();

	private Dictionary<Guid, Hero> m_heroes = new Dictionary<Guid, Hero>();

	private int m_nNationHeroCount;

	private NationInstance m_secretLetterQuestTargetNationInst;

	private List<Ranking> m_battlePowerRankings = new List<Ranking>();

	private Dictionary<Guid, Ranking> m_battlePowerRankingsByHero = new Dictionary<Guid, Ranking>();

	private List<Ranking> m_exploitPointRankings = new List<Ranking>();

	private Dictionary<Guid, Ranking> m_exploitPointRankingsByHero = new Dictionary<Guid, Ranking>();

	private List<PresentPopularityPointRanking> m_weeklyPresentPopularityPointRankings = new List<PresentPopularityPointRanking>();

	private Dictionary<Guid, PresentPopularityPointRanking> m_weeklyPresentPopularityPointRankingsByHero = new Dictionary<Guid, PresentPopularityPointRanking>();

	private List<PresentContributionPointRanking> m_weeklyPresentContributionPointRankings = new List<PresentContributionPointRanking>();

	private Dictionary<Guid, PresentContributionPointRanking> m_weeklyPresentContributionPointRankingsByHero = new Dictionary<Guid, PresentContributionPointRanking>();

	private Dictionary<int, UndergroundMazeInstance> m_undergroundMazeInsts = new Dictionary<int, UndergroundMazeInstance>();

	private Dictionary<Guid, Guild> m_guilds = new Dictionary<Guid, Guild>();

	private List<GuildRanking> m_guildRankings = new List<GuildRanking>();

	private Dictionary<Guid, GuildRanking> m_guildRankingsByGuild = new Dictionary<Guid, GuildRanking>();

	private Dictionary<int, NationNoblesseInstance> m_noblesseInsts = new Dictionary<int, NationNoblesseInstance>();

	private DateValuePair<int> m_weeklyNationWarDeclarationCount = new DateValuePair<int>();

	private NationWarInstance m_nationWarInst;

	private NationWarCall m_nationWarCall;

	private DateTimeOffset m_nationWarCallLastStartTime = DateTimeOffset.MinValue;

	private NationWarConvergingAttack m_nationWarConvergingAttack;

	private DateTimeOffset m_nationWarConvergingAttackLastStartTime = DateTimeOffset.MinValue;

	private Dictionary<long, NationCall> m_nationCalls = new Dictionary<long, NationCall>();

	private NationPowerRanking m_nationPowerRanking;

	public Nation nation => m_nation;

	public int nationId => m_nation.id;

	public long fund => m_lnFund;

	public DateValuePair<int> dailyNationWarCallCount => m_dailyNationWarCallCount;

	public DateValuePair<int> dailyConvergingAttackCount => m_dailyConvergingAttackCount;

	public int nationWarPoint
	{
		get
		{
			return m_nNationWarPoint;
		}
		set
		{
			m_nNationWarPoint = value;
		}
	}

	public NationAlliance alliance => m_alliance;

	public int allianceNationId
	{
		get
		{
			if (m_alliance == null)
			{
				return 0;
			}
			return m_alliance.GetAllianceNationId(nationId);
		}
	}

	public Dictionary<int, NationContinentInstance> continentInsts => m_continentInsts;

	public Dictionary<Guid, Hero> heroes => m_heroes;

	public int heroCount => m_heroes.Count;

	public int nationHeroCount
	{
		get
		{
			return m_nNationHeroCount;
		}
		set
		{
			m_nNationHeroCount = value;
		}
	}

	public NationInstance secretLetterQuestTargetNationInst => m_secretLetterQuestTargetNationInst;

	public int secretLetterQuestTargetNationId => m_secretLetterQuestTargetNationInst.nationId;

	public Dictionary<int, NationNoblesseInstance> noblesseInsts => m_noblesseInsts;

	public DateValuePair<int> weeklyNationWarDeclarationCount => m_weeklyNationWarDeclarationCount;

	public NationWarInstance nationWarInst => m_nationWarInst;

	public NationWarCall nationWarCall => m_nationWarCall;

	public DateTimeOffset nationWarCallLastStartTime => m_nationWarCallLastStartTime;

	public NationWarConvergingAttack nationWarConvergingAttack => m_nationWarConvergingAttack;

	public DateTimeOffset nationWarConvergingAttackLastStartTime => m_nationWarConvergingAttackLastStartTime;

	public NationPowerRanking nationPowerRanking => m_nationPowerRanking;

	public NationInstance(Nation nation)
	{
		m_nation = nation;
	}

	public void Init(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_lnFund = Convert.ToInt64(dr["fund"]);
		m_dailyNationWarCallCount.date = SFDBUtil.ToDateTime(dr["nationWarCallDate"], DateTime.MinValue);
		m_dailyNationWarCallCount.value = Convert.ToInt32(dr["nationWarCallCount"]);
		m_dailyConvergingAttackCount.date = SFDBUtil.ToDateTime(dr["convergingAttackDate"], DateTime.MinValue);
		m_dailyConvergingAttackCount.value = Convert.ToInt32(dr["convergingAttackCount"]);
		m_nNationWarPoint = Convert.ToInt32(dr["nationWarPoint"]);
		foreach (NationNoblesse noblesse in Resource.instance.nationNoblesses.Values)
		{
			NationNoblesseInstance noblesseInst = new NationNoblesseInstance(this, noblesse.id);
			AddNoblesseInstance(noblesseInst);
		}
	}

	public void SetAlliance(NationAlliance alliance)
	{
		if (alliance == null)
		{
			throw new ArgumentNullException("nationAlliance");
		}
		m_alliance = alliance;
	}

	public void ClearAlliance()
	{
		m_alliance = null;
	}

	public void AddContinentInstance(NationContinentInstance continentInst)
	{
		if (continentInst == null)
		{
			throw new ArgumentException("continentInst");
		}
		m_continentInsts.Add(continentInst.continent.id, continentInst);
	}

	public NationContinentInstance GetContinentInstance(int nContinentId)
	{
		if (!m_continentInsts.TryGetValue(nContinentId, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddHero(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		m_heroes.Add(hero.id, hero);
	}

	public void RemoveHero(Guid heroId)
	{
		m_heroes.Remove(heroId);
	}

	public Hero GetHero(Guid heroId)
	{
		if (!m_heroes.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	public Hero GetHero(string sName)
	{
		foreach (Hero hero in m_heroes.Values)
		{
			if (string.Compare(hero.name, sName, ignoreCase: true) == 0)
			{
				return hero;
			}
		}
		return null;
	}

	public List<ClientPeer> GetClientPeers(Guid heroIdToExclude)
	{
		List<ClientPeer> results = new List<ClientPeer>();
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.id != heroIdToExclude)
			{
				results.Add(hero.account.peer);
			}
		}
		return results;
	}

	private NationInstance SelectSecretLetterQuestTargetNationInstance()
	{
		List<NationInstance> poolEntries = new List<NationInstance>();
		foreach (NationInstance nationInst in Cache.instance.nationInsts.Values)
		{
			if (nationInst.nationId != nationId)
			{
				poolEntries.Add(nationInst);
			}
		}
		return poolEntries[SFRandom.Next(poolEntries.Count)];
	}

	public void OnDateChanged()
	{
		OnDateChanged_RefreshSecretLetterQuestTargetNation();
	}

	private void OnDateChanged_RefreshSecretLetterQuestTargetNation()
	{
		try
		{
			RefreshSecretLetterQuestTargetNation(bSendEvent: true);
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	public void RefreshSecretLetterQuestTargetNation(bool bSendEvent)
	{
		m_secretLetterQuestTargetNationInst = SelectSecretLetterQuestTargetNationInstance();
		if (bSendEvent)
		{
			ServerEvent.SendSecretLetterQuestTargetNationChanged(GetClientPeers(Guid.Empty), m_secretLetterQuestTargetNationInst.nationId);
		}
	}

	public void ClearBattlePowerRankings()
	{
		m_battlePowerRankings.Clear();
		m_battlePowerRankingsByHero.Clear();
	}

	public void AddBattlePowerRanking(Ranking ranking)
	{
		m_battlePowerRankings.Add(ranking);
		m_battlePowerRankingsByHero.Add(ranking.heroId, ranking);
	}

	public Ranking GetBattlePowerRankingOfHero(Guid heroId)
	{
		if (!m_battlePowerRankingsByHero.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDRanking> GetBattlePowerPDRankings(int nCount)
	{
		List<PDRanking> insts = new List<PDRanking>();
		int nLoopCount = Math.Min(nCount, m_battlePowerRankings.Count);
		for (int i = 0; i < nLoopCount; i++)
		{
			insts.Add(m_battlePowerRankings[i].ToPDRanking());
		}
		return insts;
	}

	public void ClearExploitPointRankings()
	{
		m_exploitPointRankings.Clear();
		m_exploitPointRankingsByHero.Clear();
	}

	public void AddExploitPointRanking(Ranking ranking)
	{
		m_exploitPointRankings.Add(ranking);
		m_exploitPointRankingsByHero.Add(ranking.heroId, ranking);
	}

	public Ranking GetExploitPointRankingOfHero(Guid heroId)
	{
		if (!m_exploitPointRankingsByHero.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDRanking> GetExploitPointPDRankings(int nCount)
	{
		List<PDRanking> insts = new List<PDRanking>();
		int nLoopCount = Math.Min(nCount, m_exploitPointRankings.Count);
		for (int i = 0; i < nLoopCount; i++)
		{
			insts.Add(m_exploitPointRankings[i].ToPDRanking());
		}
		return insts;
	}

	public void ClearWeeklyPresentPopularityPointRankings()
	{
		m_weeklyPresentPopularityPointRankings.Clear();
		m_weeklyPresentPopularityPointRankingsByHero.Clear();
	}

	public void AddWeeklyPresentPopularityPointRanking(PresentPopularityPointRanking ranking)
	{
		m_weeklyPresentPopularityPointRankings.Add(ranking);
		m_weeklyPresentPopularityPointRankingsByHero.Add(ranking.heroId, ranking);
	}

	public PresentPopularityPointRanking GetWeeklyPresentPopularityPointRankingOfHero(Guid heroId)
	{
		if (!m_weeklyPresentPopularityPointRankingsByHero.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	public int GetWeeklyPresentPopularityPointRankingValueOfHero(Guid heroId)
	{
		return GetWeeklyPresentPopularityPointRankingOfHero(heroId)?.ranking ?? 0;
	}

	public List<PDPresentPopularityPointRanking> GetWeeklyPDPresentPopularityPointRankings(int nCount)
	{
		List<PDPresentPopularityPointRanking> insts = new List<PDPresentPopularityPointRanking>();
		int nLoopCount = Math.Min(nCount, m_weeklyPresentPopularityPointRankings.Count);
		for (int i = 0; i < nLoopCount; i++)
		{
			insts.Add(m_weeklyPresentPopularityPointRankings[i].ToPDRanking());
		}
		return insts;
	}

	public void ClearWeeklyPresentContributionPointRankings()
	{
		m_weeklyPresentContributionPointRankings.Clear();
		m_weeklyPresentContributionPointRankingsByHero.Clear();
	}

	public void AddWeeklyPresentContributionPointRanking(PresentContributionPointRanking ranking)
	{
		m_weeklyPresentContributionPointRankings.Add(ranking);
		m_weeklyPresentContributionPointRankingsByHero.Add(ranking.heroId, ranking);
	}

	public PresentContributionPointRanking GetWeeklyPresentContributionPointRankingOfHero(Guid heroId)
	{
		if (!m_weeklyPresentContributionPointRankingsByHero.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	public int GetWeeklyPresentContributionPointRankingValueOfHero(Guid heroId)
	{
		return GetWeeklyPresentContributionPointRankingOfHero(heroId)?.ranking ?? 0;
	}

	public List<PDPresentContributionPointRanking> GetWeeklyPDPresentContributionPointRankings(int nCount)
	{
		List<PDPresentContributionPointRanking> insts = new List<PDPresentContributionPointRanking>();
		int nLoopCount = Math.Min(nCount, m_weeklyPresentContributionPointRankings.Count);
		for (int i = 0; i < nLoopCount; i++)
		{
			insts.Add(m_weeklyPresentContributionPointRankings[i].ToPDRanking());
		}
		return insts;
	}

	public void AddUndergroundMazeInstance(UndergroundMazeInstance undergroundMazeInst)
	{
		if (undergroundMazeInst == null)
		{
			throw new ArgumentNullException("undergroundMazeInst");
		}
		m_undergroundMazeInsts.Add(undergroundMazeInst.floor.floor, undergroundMazeInst);
	}

	public UndergroundMazeInstance GetUndergroundMazeInstance(int nFloor)
	{
		if (!m_undergroundMazeInsts.TryGetValue(nFloor, out var value))
		{
			return null;
		}
		return value;
	}

	public void AddGuild(Guild guild)
	{
		if (guild == null)
		{
			throw new ArgumentNullException("guild");
		}
		m_guilds.Add(guild.id, guild);
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
		m_guilds.Remove(id);
	}

	public List<PDSimpleGuild> GetPDSimpleGuilds()
	{
		List<PDSimpleGuild> insts = new List<PDSimpleGuild>();
		foreach (Guild guild in m_guilds.Values)
		{
			insts.Add(guild.ToPDSimpleGuild());
		}
		return insts;
	}

	public void ClearGuildRankings()
	{
		m_guildRankings.Clear();
		m_guildRankingsByGuild.Clear();
	}

	public void AddGuildRanking(GuildRanking ranking)
	{
		m_guildRankings.Add(ranking);
		m_guildRankingsByGuild.Add(ranking.guildId, ranking);
	}

	public GuildRanking GetGuildRankingOfGuild(Guid guildId)
	{
		if (!m_guildRankingsByGuild.TryGetValue(guildId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDGuildRanking> GetPDGuildRankings(int nCount)
	{
		List<PDGuildRanking> insts = new List<PDGuildRanking>();
		int nLoopCount = Math.Min(nCount, m_guildRankings.Count);
		for (int i = 0; i < nLoopCount; i++)
		{
			insts.Add(m_guildRankings[i].ToPDGuildRanking());
		}
		return insts;
	}

	public void UpdateKing()
	{
		Guild guild = null;
		foreach (GuildRanking ranking in m_guildRankings)
		{
			guild = GetGuild(ranking.guildId);
			if (guild != null)
			{
				break;
			}
		}
		if (guild == null)
		{
			return;
		}
		NationNoblesseInstance noblesseInst = GetNoblesseInstanceByNoblesseId(1);
		bool bIsDismissed = false;
		if (noblesseInst.heroId != Guid.Empty)
		{
			Hero oldKing = Cache.instance.GetLoggedInHero(noblesseInst.heroId);
			if (oldKing != null)
			{
				HeroSynchronizer.Exec(oldKing, new SFAction<NationNoblesse>(oldKing.SetNationNoblesse, null));
			}
			DismissNoblesse(1);
			bIsDismissed = true;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		GuildMember targetGuildMaster = guild.master;
		NationNoblesseInstance preveNoblesseOfTargetGuildMaster = GetNoblesseInstanceByHeroId(targetGuildMaster.id);
		if (preveNoblesseOfTargetGuildMaster != null)
		{
			DismissNoblesse(preveNoblesseOfTargetGuildMaster.id);
		}
		Hero newKing = targetGuildMaster.hero;
		if (newKing != null)
		{
			HeroSynchronizer.Exec(newKing, new SFAction<NationNoblesse>(newKing.SetNationNoblesse, Resource.instance.GetNationNoblesse(1)));
		}
		AppointNoblesse(1, targetGuildMaster.id, targetGuildMaster.name, targetGuildMaster.jobId, currentTime.Date);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateNationWork(nationId);
		if (preveNoblesseOfTargetGuildMaster != null)
		{
			dbWork.AddSqlCommand(GameDac.CSC_DeleteNationIncumbentNoblesse(m_nation.id, preveNoblesseOfTargetGuildMaster.id));
		}
		if (bIsDismissed)
		{
			dbWork.AddSqlCommand(GameDac.CSC_DeleteNationIncumbentNoblesse(m_nation.id, 1));
		}
		dbWork.AddSqlCommand(GameDac.CSC_AddNationIncumbentNoblesse(m_nation.id, 1, targetGuildMaster.id));
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateNationNoblesseAppointment(m_nation.id, 1, currentTime));
		dbWork.Schedule();
	}

	public void RefreshDailyNationWarCallCount(DateTime date)
	{
		if (!(m_dailyNationWarCallCount.date == date))
		{
			m_dailyNationWarCallCount.date = date;
			m_dailyNationWarCallCount.value = 0;
		}
	}

	public void RefreshDailyConvergingAttackCount(DateTime date)
	{
		if (!(m_dailyConvergingAttackCount.date == date))
		{
			m_dailyConvergingAttackCount.date = date;
			m_dailyConvergingAttackCount.value = 0;
		}
	}

	public void AddNoblesseInstance(NationNoblesseInstance noblesseInst)
	{
		if (noblesseInst == null)
		{
			throw new ArgumentNullException("noblesseInst");
		}
		m_noblesseInsts.Add(noblesseInst.id, noblesseInst);
	}

	public NationNoblesseInstance GetNoblesseInstanceByNoblesseId(int nNoblesseId)
	{
		if (!m_noblesseInsts.TryGetValue(nNoblesseId, out var value))
		{
			return null;
		}
		return value;
	}

	public NationNoblesseInstance GetNoblesseInstanceByHeroId(Guid heroId)
	{
		foreach (NationNoblesseInstance noblesseInst in m_noblesseInsts.Values)
		{
			if (heroId == noblesseInst.heroId)
			{
				return noblesseInst;
			}
		}
		return null;
	}

	public void AppointNoblesse(int nNoblesseId, Guid targetHeroId, string sName, int nJobId, DateTime date)
	{
		NationNoblesseInstance nationNoblesseInst = GetNoblesseInstanceByNoblesseId(nNoblesseId);
		nationNoblesseInst.heroId = targetHeroId;
		nationNoblesseInst.heroName = sName;
		nationNoblesseInst.heroJobId = nJobId;
		nationNoblesseInst.appointmentDate = date;
		ServerEvent.SendNationNoblesseAppointment(Cache.instance.GetClientPeers(Guid.Empty), nationId, nNoblesseId, targetHeroId, sName, nJobId, date);
	}

	public void DismissNoblesse(int nNoblesseId)
	{
		NationNoblesseInstance noblesseInst = GetNoblesseInstanceByNoblesseId(nNoblesseId);
		if (!(noblesseInst.heroId == Guid.Empty))
		{
			noblesseInst.heroId = Guid.Empty;
			noblesseInst.heroName = null;
			noblesseInst.heroJobId = 0;
			ServerEvent.SendNationNoblesseDismissal(Cache.instance.GetClientPeers(Guid.Empty), nationId, nNoblesseId);
		}
	}

	public void RefreshWeeklyNationWarDeclarationCount(DateTime date)
	{
		DateTime dateOfSunday = DateTimeUtil.GetWeekStartDate(date);
		if (!(m_weeklyNationWarDeclarationCount.date == dateOfSunday))
		{
			m_weeklyNationWarDeclarationCount.date = dateOfSunday;
			m_weeklyNationWarDeclarationCount.value = 0;
		}
	}

	public void SetWeeklyNationWarDeclarationCount(DateTime date, int nCount)
	{
		m_weeklyNationWarDeclarationCount.date = date;
		m_weeklyNationWarDeclarationCount.value = nCount;
		RefreshWeeklyNationWarDeclarationCount(date);
	}

	public void AddFund(long lnAmount, Guid memberIdToExcludeFromEventTarget)
	{
		if (lnAmount < 0)
		{
			throw new ArgumentOutOfRangeException("lnAmount");
		}
		if (lnAmount != 0)
		{
			m_lnFund += lnAmount;
			ServerEvent.SendNationFundChanged(Cache.instance.GetClientPeers(memberIdToExcludeFromEventTarget), nationId, m_lnFund);
		}
	}

	public void UseFund(long lnAmount, Guid memberIdToExcludeFromEventTarget)
	{
		if (lnAmount < 0)
		{
			throw new ArgumentOutOfRangeException("lnAmount");
		}
		if (lnAmount != 0)
		{
			if (lnAmount > m_lnFund)
			{
				throw new Exception("보유한 국고자금이 부족합니다.");
			}
			m_lnFund -= lnAmount;
			ServerEvent.SendNationFundChanged(Cache.instance.GetClientPeers(memberIdToExcludeFromEventTarget), nationId, m_lnFund);
		}
	}

	public void ReadyNationWar(NationWarInstance nationWarInst, DateTimeOffset time)
	{
		if (nationWarInst == null)
		{
			throw new ArgumentNullException("nationWarInst");
		}
		m_nationWarInst = nationWarInst;
		if (nationId != m_nationWarInst.defenseNation.id)
		{
			return;
		}
		Resource res = Resource.instance;
		int nPvpMinHeroLevel = res.pvpMinHeroLevel;
		foreach (NationContinentInstance nationContinentInst in m_continentInsts.Values)
		{
			lock (nationContinentInst.syncObject)
			{
				Hero[] array = nationContinentInst.heroes.Values.ToArray();
				foreach (Hero hero in array)
				{
					lock (hero.syncObject)
					{
						if (nationWarInst.IsNationWarJoinEnabled(hero))
						{
							if (hero.level >= nPvpMinHeroLevel && nationContinentInst.continent.isNationWarTarget)
							{
								if (hero.isDistorting)
								{
									hero.CancelDistortion(bSendEventToMyself: true);
								}
								m_nationWarInst.AddHero(hero, time);
							}
						}
						else
						{
							Continent targetContinent = res.GetContinent(res.saftyRevivalContinentId);
							int nTargetNationId = hero.nationId;
							nationContinentInst.Exit(hero, isLogOut: false, new ContinentEnterForContinentBanishedParam(targetContinent, nTargetNationId, res.SelectSaftyRevivalPosition(), res.SelectSaftyRevivalYRotation(), time));
							ServerEvent.SendContinentBanished(hero.account.peer, targetContinent.id, nTargetNationId);
						}
					}
				}
			}
		}
	}

	public void RegistNationWarCall(Hero hero, DateTimeOffset time)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		NationContinentInstance currentPlace = (NationContinentInstance)hero.currentPlace;
		NationWarCall nationWarCall = (m_nationWarCall = new NationWarCall(m_nationWarInst, hero.id, hero.name, hero.nationNoblesseId, currentPlace.continent, currentPlace.nationId, hero.position, hero.rotationY, time));
		m_nationWarCallLastStartTime = time;
		new List<ClientPeer>();
		ServerEvent.SendNationWarCall(GetClientPeers(hero.id), nationWarCall.callerId, nationWarCall.callerName, nationWarCall.callerNoblesseId, m_dailyNationWarCallCount.date, m_dailyNationWarCallCount.value, GetNationWarCallRemainingCoolTime(time));
	}

	public float GetNationWarCallRemainingCoolTime(DateTimeOffset time)
	{
		if (m_nationWarCallLastStartTime == DateTimeOffset.MinValue)
		{
			return 0f;
		}
		float fElapsedTime = DateTimeUtil.GetTimeSpanSeconds(m_nationWarCallLastStartTime, time);
		return Math.Max((float)Resource.instance.nationWar.nationCallCoolTime * 0.9f - fElapsedTime, 0f);
	}

	public void RegistNationWarConvergingAttack(Hero hero, int nTargetMonsterArrangeId, DateTimeOffset time)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		NationWarConvergingAttack nationWarConvergingAttack = (m_nationWarConvergingAttack = new NationWarConvergingAttack(m_nationWarInst, nTargetMonsterArrangeId, time));
		m_nationWarConvergingAttackLastStartTime = time;
		ServerEvent.SendNationWarConvergingAttack(GetClientPeers(hero.id), nTargetMonsterArrangeId, m_dailyConvergingAttackCount.date, m_dailyConvergingAttackCount.value, GetNationWarConvergingAttackRemainingCoolTime(time));
	}

	public void OnNationWarInstanceUpdate(DateTimeOffset time)
	{
		if (m_nationWarConvergingAttack != null)
		{
			float fElapsedTime = DateTimeUtil.GetTimeSpanSeconds(m_nationWarConvergingAttack.regTime, time);
			int nLifeTime = Resource.instance.nationWar.convergingAttackLifeTime;
			if (!(fElapsedTime < (float)nLifeTime))
			{
				m_nationWarConvergingAttack = null;
				ServerEvent.SendNationWarConvergingAttackFinished(GetClientPeers(Guid.Empty));
			}
		}
	}

	public float GetNationWarConvergingAttackRemainingCoolTime(DateTimeOffset time)
	{
		if (m_nationWarConvergingAttackLastStartTime == DateTimeOffset.MinValue)
		{
			return 0f;
		}
		float fElapsedTime = DateTimeUtil.GetTimeSpanSeconds(m_nationWarConvergingAttackLastStartTime, time);
		return Math.Max((float)Resource.instance.nationWar.convergingAttackCoolTime * 0.9f - fElapsedTime, 0f);
	}

	public void OnNationWarFinish()
	{
		m_nationWarInst = null;
		m_nationWarCall = null;
		m_nationWarCallLastStartTime = DateTimeOffset.MinValue;
		m_nationWarConvergingAttack = null;
		m_nationWarConvergingAttackLastStartTime = DateTimeOffset.MinValue;
	}

	public void AddNationCall(NationCall call)
	{
		if (call == null)
		{
			throw new ArgumentNullException("call");
		}
		m_nationCalls.Add(call.id, call);
	}

	public NationCall GetNationCall(long lnId)
	{
		if (!m_nationCalls.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveNationCall(long lnId)
	{
		m_nationCalls.Remove(lnId);
	}

	public void OnGuildCallLifetimeEnded(long lnId)
	{
		RemoveNationCall(lnId);
	}

	public void CallNationMembers(Hero caller, Continent continent, int nNationId)
	{
		if (caller == null)
		{
			throw new ArgumentNullException("caller");
		}
		if (continent == null)
		{
			throw new ArgumentNullException("continent");
		}
		NationCall call = new NationCall(caller, continent, nNationId, caller.position, caller.rotationY);
		AddNationCall(call);
		ServerEvent.SendNationCall(GetClientPeers(caller.id), call.ToPDNationCall());
	}

	public List<PDNationNoblesseInstance> GetPDNationNoblesseInstances()
	{
		List<PDNationNoblesseInstance> results = new List<PDNationNoblesseInstance>();
		foreach (NationNoblesseInstance noblesseInst in m_noblesseInsts.Values)
		{
			results.Add(noblesseInst.ToPDNationNoblesseInstance());
		}
		return results;
	}

	public List<int> GetSpawnedEliteMonsterIds()
	{
		List<int> results = new List<int>();
		foreach (NationContinentInstance continentInst in m_continentInsts.Values)
		{
			lock (continentInst.syncObject)
			{
				foreach (ContinentEliteMonsterInstance eliteMonsterInst in continentInst.eliteMonsterInsts.Values)
				{
					results.Add(eliteMonsterInst.eliteMonsterId);
				}
			}
		}
		return results;
	}

	public PDNationInstance ToPDNationInstance()
	{
		PDNationInstance inst = new PDNationInstance();
		inst.nationId = nationId;
		inst.fund = m_lnFund;
		inst.noblesseInsts = GetPDNationNoblesseInstances().ToArray();
		return inst;
	}

	public void SetNationPowerRanking(NationPowerRanking ranking)
	{
		m_nationPowerRanking = ranking;
	}
}
