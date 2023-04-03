using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class Guild
{
	private Guid m_id = Guid.Empty;

	private bool m_bReleased;

	private string m_sName;

	private NationInstance m_nationInst;

	private string m_sNotice;

	private int m_nBuildingPoint;

	private long m_lnFund;

	private DateValuePair<int> m_dailyBanishmentCount = new DateValuePair<int>();

	private GuildMember m_master;

	private Dictionary<Guid, GuildMember> m_members = new Dictionary<Guid, GuildMember>();

	private Dictionary<Guid, GuildApplication> m_applications = new Dictionary<Guid, GuildApplication>();

	private Dictionary<Guid, GuildInvitation> m_invitations = new Dictionary<Guid, GuildInvitation>();

	private GuildTerritoryInstance m_territoryInst;

	private GuildBuildingInstance m_lobby;

	private GuildBuildingInstance m_laboratory;

	private GuildBuildingInstance m_shop;

	private GuildBuildingInstance m_tankFactory;

	private Dictionary<int, GuildBuildingInstance> m_buildingInsts = new Dictionary<int, GuildBuildingInstance>();

	private int m_nFoodWarehouseLevel = 1;

	private int m_nFoodWarehouseExp;

	private Guid m_foodWarehouseCollectionId = Guid.Empty;

	private DateValuePair<int> m_moralPoint = new DateValuePair<int>();

	private Dictionary<long, GuildCall> m_guildCalls = new Dictionary<long, GuildCall>();

	private GuildSupplySupportQuestPlay m_guildSupplySupportQuestPlay;

	private DateValuePair<int> m_dailyGuildSupplySupportQuestStartCount = new DateValuePair<int>();

	private DateValuePair<int> m_dailyHuntingDonationCount = new DateValuePair<int>();

	private DateTime m_dailyObjectiveDate = DateTime.MinValue.Date;

	private int m_nDailyObjectiveContentId;

	private Dictionary<Guid, GuildDailyObjectiveCompletionMember> m_dailyObjectiveCompletionMembers = new Dictionary<Guid, GuildDailyObjectiveCompletionMember>();

	private DateTime m_weeklyObjectiveDate = DateTime.MinValue.Date;

	private int m_nWeeklyObjectiveId;

	private int m_nWeeklyObjectiveCompletionMemberCount;

	private DateTimeOffset m_blessingBuffStartTime = DateTimeOffset.MinValue;

	private GuildBlessingBuff m_blessingBuff;

	private Timer m_blessingBuffTimer;

	public Guid id => m_id;

	public string name => m_sName;

	public int level => m_lobby.level.level;

	public NationInstance nationInst => m_nationInst;

	public string notice
	{
		get
		{
			return m_sNotice;
		}
		set
		{
			m_sNotice = value;
		}
	}

	public int buildingPoint => m_nBuildingPoint;

	public long fund => m_lnFund;

	public DateValuePair<int> dailyBanishmentCount => m_dailyBanishmentCount;

	public GuildMember master
	{
		get
		{
			return m_master;
		}
		set
		{
			m_master = value;
		}
	}

	public Dictionary<Guid, GuildMember> members => m_members;

	public bool isMemberFull => m_members.Count >= Resource.instance.GetGuildLevel(m_lobby.level.level).maxMemberCount;

	public int viceMasterCount
	{
		get
		{
			int nCount = 0;
			foreach (GuildMember member in m_members.Values)
			{
				if (member.isViceMaster)
				{
					nCount++;
				}
			}
			return nCount;
		}
	}

	public bool isViceMasterFull => viceMasterCount >= Resource.instance.guildViceMasterCount;

	public int lordCount
	{
		get
		{
			int nCount = 0;
			foreach (GuildMember member in m_members.Values)
			{
				if (member.isLord)
				{
					nCount++;
				}
			}
			return nCount;
		}
	}

	public bool isLordFull => lordCount >= Resource.instance.guildLordCount;

	public Dictionary<Guid, GuildApplication> applications => m_applications;

	public GuildTerritoryInstance territoryInst => m_territoryInst;

	public GuildBuildingInstance lobby => m_lobby;

	public GuildBuildingInstance laboratory => m_laboratory;

	public GuildBuildingInstance shop => m_shop;

	public GuildBuildingInstance tankFactory => m_tankFactory;

	public Dictionary<int, GuildBuildingInstance> buildingInsts => m_buildingInsts;

	public int foodWarehouseLevel => m_nFoodWarehouseLevel;

	public bool isFoodWarehouseLevelMax => m_nFoodWarehouseLevel >= Resource.instance.guildFoodWarehouse.lastLevel.level;

	public int foodWarehouseExp => m_nFoodWarehouseExp;

	public Guid foodWarehouseCollectionId => m_foodWarehouseCollectionId;

	public DateTime moralPointDate => m_moralPoint.date;

	public int moralPoint => m_moralPoint.value;

	public bool isMoralPointMax => m_moralPoint.value >= Resource.instance.guildAltar.dailyGuildMaxMoralPoint;

	public GuildSupplySupportQuestPlay guildSupplySupportQuestPlay => m_guildSupplySupportQuestPlay;

	public DateValuePair<int> dailyGuildSupplySupportQuestStartCount => m_dailyGuildSupplySupportQuestStartCount;

	public DateValuePair<int> dailyHuntingDonationCount => m_dailyHuntingDonationCount;

	public DateTime dailyObjectiveDate => m_dailyObjectiveDate;

	public int dailyObjectiveContentId => m_nDailyObjectiveContentId;

	public Dictionary<Guid, GuildDailyObjectiveCompletionMember> dailyObjectiveCompletionMembers => m_dailyObjectiveCompletionMembers;

	public DateTime weeklyObjectiveDate
	{
		get
		{
			return m_weeklyObjectiveDate;
		}
		set
		{
			m_weeklyObjectiveDate = value;
		}
	}

	public int weeklyObjectiveId
	{
		get
		{
			return m_nWeeklyObjectiveId;
		}
		set
		{
			m_nWeeklyObjectiveId = value;
		}
	}

	public int weeklyObjectiveCompletionMemberCount
	{
		get
		{
			return m_nWeeklyObjectiveCompletionMemberCount;
		}
		set
		{
			m_nWeeklyObjectiveCompletionMemberCount = value;
		}
	}

	public DateTimeOffset blessingBuffStartTime => m_blessingBuffStartTime;

	public GuildBlessingBuff blessingBuff => m_blessingBuff;

	public bool isBlessingBuffRunning => m_blessingBuffTimer != null;

	public Guild()
		: this(null)
	{
	}

	public Guild(string sName)
	{
		m_sName = sName;
	}

	public void Init(DataRow dr, DateTimeOffset time)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_id = (Guid)dr["guildId"];
		m_sName = Convert.ToString(dr["name"]);
		int nNationId = Convert.ToInt32(dr["nationId"]);
		m_nationInst = Cache.instance.GetNationInstance(nNationId);
		if (m_nationInst == null)
		{
			throw new Exception(string.Concat("국가가 존재하지 않습니다. m_id = ", m_id, ", nationId = ", nNationId));
		}
		m_sNotice = Convert.ToString(dr["notice"]);
		m_nBuildingPoint = Convert.ToInt32(dr["buildingPoint"]);
		m_lnFund = Convert.ToInt64(dr["fund"]);
		DateTime memberBanishmentDate = SFDBUtil.ToDateTime(dr["memberBanishmentDate"], DateTime.MinValue.Date);
		int nMemberBanishmentCount = Convert.ToInt32(dr["memberBanishmentCount"]);
		m_dailyBanishmentCount.date = memberBanishmentDate;
		m_dailyBanishmentCount.value = nMemberBanishmentCount;
		RefreshDailyBanishmentCount(DateTimeUtil.currentTime.Date);
		m_nFoodWarehouseLevel = Convert.ToInt32(dr["foodWarehouseLevel"]);
		m_nFoodWarehouseExp = Convert.ToInt32(dr["foodWarehouseExp"]);
		m_foodWarehouseCollectionId = (Guid)dr["foodWarehouseCollectionId"];
		m_moralPoint.date = SFDBUtil.ToDateTime(dr["moralPointDate"], DateTime.MinValue.Date);
		m_moralPoint.value = Convert.ToInt32(dr["moralPoint"]);
		m_dailyHuntingDonationCount.date = SFDBUtil.ToDateTime(dr["huntingDonationDate"], DateTime.MinValue.Date);
		m_dailyHuntingDonationCount.value = Convert.ToInt32(dr["huntingDonationCount"]);
		m_dailyObjectiveDate = SFDBUtil.ToDateTime(dr["dailyObjectiveDate"], DateTime.MinValue.Date);
		m_nDailyObjectiveContentId = Convert.ToInt32(dr["dailyObjectiveContentId"]);
		RefreshDailyObjective(time, bSendEvent: false, bSaveToDB: true);
		m_weeklyObjectiveDate = SFDBUtil.ToDateTime(dr["weeklyObjectiveDate"], DateTime.MinValue.Date);
		m_nWeeklyObjectiveId = Convert.ToInt32(dr["weeklyObjectiveId"]);
		RefreshWeeklyObjective(time, bSendEvent: false, bSaveToDB: true);
		m_blessingBuffStartTime = SFDBUtil.ToDateTimeOffset(dr["blessingBuffStartTime"], DateTimeOffset.MinValue);
		int nBlessingBuffId = Convert.ToInt32(dr["blessingBuffId"]);
		m_blessingBuff = Resource.instance.GetGuildBlessingBuff(nBlessingBuffId);
		int nBlessingBuffTimerDuration = (int)(GetRemainingBlessingBuffTime(time) * 1000f);
		if (nBlessingBuffTimerDuration > 0)
		{
			StartBlessingBuffTimer(nBlessingBuffTimerDuration);
		}
		InitTerritoryInstance();
	}

	public void Create(Hero creator, DateTimeOffset time)
	{
		if (creator == null)
		{
			throw new ArgumentNullException("creator");
		}
		m_id = Guid.NewGuid();
		m_nationInst = creator.nationInst;
		GuildMember member = new GuildMember(this);
		member.Init(1, creator);
		AddMember(member);
		m_master = member;
		InitBuildingInstances();
		creator.RefreshGuildSkillRealLevels();
		creator.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		InitTerritoryInstance();
		Cache.instance.AddGuild(this);
		Cache.instance.CancelAllGuildApplicationsOfHero(member.id);
		if (creator.currentPlace != null)
		{
			ServerEvent.SendHeroGuildInfoUpdated(creator.currentPlace.GetDynamicClientPeers(creator.sector, creator.id), creator.id, m_id, m_sName, member.grade.id);
		}
		RefreshDailyObjective(time, bSendEvent: false, bSaveToDB: false);
		RefreshWeeklyObjective(time, bSendEvent: false, bSaveToDB: false);
	}

	public void OnUpdate(DateTimeOffset time)
	{
		if (m_guildSupplySupportQuestPlay != null)
		{
			m_guildSupplySupportQuestPlay.OnUpdate(time);
		}
	}

	private void InitTerritoryInstance()
	{
		GuildTerritoryInstance territoryInstance = new GuildTerritoryInstance();
		territoryInstance.Init(this);
		m_territoryInst = territoryInstance;
		Cache.instance.AddPlace(territoryInstance);
	}

	public void AddMember(GuildMember member)
	{
		if (member == null)
		{
			throw new ArgumentNullException("member");
		}
		m_members.Add(member.id, member);
		Cache.instance.AddGuildMember(member);
	}

	public GuildMember GetMember(Guid id)
	{
		if (!m_members.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveMember(Guid id)
	{
		m_members.Remove(id);
		Cache.instance.RemoveGuildMember(id);
	}

	public void AddApplication(GuildApplication app)
	{
		if (app == null)
		{
			throw new ArgumentNullException("app");
		}
		m_applications.Add(app.id, app);
		Cache.instance.AddGuildApplication(app);
		ServerEvent.SendGuildApplicationCountUpdated(GetClientPeers(Guid.Empty), m_applications.Count);
	}

	public GuildApplication GetApplication(Guid id)
	{
		if (!m_applications.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveApplication(GuildApplication app)
	{
		m_applications.Remove(app.id);
		Cache.instance.RemoveGuildApplication(app);
		ServerEvent.SendGuildApplicationCountUpdated(GetClientPeers(Guid.Empty), m_applications.Count);
	}

	public void Enter(GuildMember member)
	{
		if (member == null)
		{
			throw new ArgumentNullException("member");
		}
		AddMember(member);
		Cache.instance.CancelAllGuildApplicationsOfHero(member.id);
		Hero hero = member.hero;
		hero?.RefuseAllGuildInvitations();
		if (hero != null)
		{
			hero.RefreshGuildSkillRealLevels();
			hero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		ServerEvent.SendGuildMemberEnter(GetClientPeers(member.id), member.id, member.name);
		if (hero != null && hero.currentPlace != null)
		{
			ServerEvent.SendHeroGuildInfoUpdated(hero.currentPlace.GetDynamicClientPeers(hero.sector, hero.id), hero.id, m_id, m_sName, member.grade.id);
		}
	}

	public void Exit(GuildMember member, bool bIsBanished, DateTimeOffset currentTime)
	{
		if (member == null)
		{
			throw new ArgumentNullException("member");
		}
		RemoveMember(member.id);
		ProcessGuildFarmQuestOnExit(member);
		ProcessGuildAltarSpellInjectionMissionOnExit(member);
		ProcessGuildAltarDefenseMissionOnExit(member);
		ProcessGuildMissionQuestOnExit(member, currentTime);
		ProcessGuildHuntingQuestOnExit(member, currentTime);
		ProcessJobChangeQuestOnExit(member, currentTime);
		ExitFromTerritory(member);
		Hero hero = member.hero;
		if (hero != null)
		{
			hero.guildMember = null;
			hero.guildWithdrawalTime = currentTime;
			member.RemoveHero();
			hero.RefreshGuildSkillRealLevels();
			hero.RefreshRealValues(bSendMaxHpChangedToOthers: true);
		}
		ServerEvent.SendGuildMemberExit(GetClientPeers(Guid.Empty), member.id, member.name, bIsBanished);
		if (hero != null && hero.currentPlace != null)
		{
			ServerEvent.SendHeroGuildInfoUpdated(hero.currentPlace.GetDynamicClientPeers(hero.sector, hero.id), hero.id, Guid.Empty, null, 0);
		}
	}

	private void ProcessGuildFarmQuestOnExit(GuildMember member)
	{
		member.hero?.RemoveGuildFarmQuest();
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(member.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildFarmQuest_PerformingQuestFail(member.id, m_id, DateTimeUtil.currentTime));
		dbWork.Schedule();
	}

	private void ProcessGuildAltarSpellInjectionMissionOnExit(GuildMember member)
	{
		member.hero?.RemoveGuildAltarSpellInjectionMission();
	}

	private void ProcessGuildAltarDefenseMissionOnExit(GuildMember member)
	{
		Hero hero = member.hero;
		if (hero != null && hero.guildAltarDefenseMission != null)
		{
			hero.guildAltarDefenseMission.Fail(bSendEventToMySelf: false);
		}
	}

	private void ProcessGuildMissionQuestOnExit(GuildMember member, DateTimeOffset currentTime)
	{
		Hero hero = member.hero;
		if (hero != null && hero.guildMissionQuest != null)
		{
			HeroGuildMissionQuest heroGuildMissionQuest = hero.guildMissionQuest;
			if (heroGuildMissionQuest.currentMission != null)
			{
				heroGuildMissionQuest.FailCurrentMission(currentTime, bSendEvent: false);
			}
		}
	}

	private void ProcessGuildHuntingQuestOnExit(GuildMember member, DateTimeOffset currentTime)
	{
		Hero hero = member.hero;
		if (hero != null)
		{
			hero.FailGuildHuntingQuest(currentTime);
			return;
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroGuildHuntingQuest_PerformingQuestFail(member.id, m_id, currentTime));
		dbWork.Schedule();
	}

	private void ProcessJobChangeQuestOnExit(GuildMember member, DateTimeOffset currentTime)
	{
		Hero hero = member.hero;
		if (hero != null)
		{
			HeroJobChangeQuest heroJobChangeQuest = hero.jobChangeQuest;
			if (heroJobChangeQuest != null)
			{
				JobChangeQuestDifficulty jobChangeQuestDifficulty = heroJobChangeQuest.quest.GetDifficulty(heroJobChangeQuest.difficulty);
				if (jobChangeQuestDifficulty != null && jobChangeQuestDifficulty.isTargetPlaceGuildTerrtory)
				{
					heroJobChangeQuest.RemoveExclusiveMonster();
					heroJobChangeQuest.Fail(bSendEvent: true, currentTime);
				}
			}
			return;
		}
		MonsterInstance targetMonsterInst = null;
		foreach (MonsterInstance monsterInstance in m_territoryInst.monsterInsts.Values)
		{
			if (monsterInstance is JobChangeQuestMonsterInstance)
			{
				JobChangeQuestMonsterInstance jobChangeQuetMonsterInstance = (JobChangeQuestMonsterInstance)monsterInstance;
				if (jobChangeQuetMonsterInstance.exclusiveHeroId == member.id)
				{
					targetMonsterInst = jobChangeQuetMonsterInstance;
					break;
				}
			}
		}
		if (targetMonsterInst != null)
		{
			m_territoryInst.RemoveMonster(targetMonsterInst, bSendEvent: true);
		}
	}

	private void ExitFromTerritory(GuildMember member)
	{
		Hero hero = member.hero;
		if (hero != null && hero.currentPlace is GuildTerritoryInstance territoryInst)
		{
			territoryInst.Exit(hero, isLogOut: false, null);
		}
	}

	public void AddInvitation(GuildInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		m_invitations.Add(invitation.id, invitation);
	}

	public void RemoveInvitation(GuildInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		m_invitations.Remove(invitation.id);
		invitation.Release();
	}

	public GuildInvitation GetInvitation(Guid id)
	{
		if (!m_invitations.TryGetValue(id, out var value))
		{
			return null;
		}
		return value;
	}

	public bool ContainsInvitationForHero(Guid heroId)
	{
		foreach (GuildInvitation invitation in m_invitations.Values)
		{
			if (heroId == invitation.target.id)
			{
				return true;
			}
		}
		return false;
	}

	public void OnInvitationLifetimeEnded(GuildInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		RemoveInvitation(invitation);
		GuildMember member = GetMember(invitation.inviterId);
		if (member != null && member.grade.invitationEnabled && member.isLoggedIn)
		{
			ServerEvent.SendGuildInvitationLifetimeEnded(member.hero.account.peer, invitation.id, invitation.target.id, invitation.target.name);
		}
	}

	public GuildInvitation Invite(Hero target, GuildMember inviter, DateTimeOffset currentTime)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		GuildInvitation invitation = new GuildInvitation(this, target, inviter.id, inviter.name, currentTime);
		AddInvitation(invitation);
		target.OnGuildInvited(invitation);
		return invitation;
	}

	public void OnInvitationRefused(GuildInvitation invitation)
	{
		if (invitation == null)
		{
			throw new ArgumentNullException("invitation");
		}
		RemoveInvitation(invitation);
		GuildMember member = GetMember(invitation.inviterId);
		if (member != null && member.grade.invitationEnabled && member.isLoggedIn)
		{
			ServerEvent.SendGuildInvitationRefused(member.hero.account.peer, invitation.target.id, invitation.target.name);
		}
	}

	public void CancelAllInvitation()
	{
		GuildInvitation[] array = m_invitations.Values.ToArray();
		foreach (GuildInvitation invitation in array)
		{
			invitation.target.OnGuildInvitationCanceled(invitation.id);
			RemoveInvitation(invitation);
		}
	}

	public List<ClientPeer> GetClientPeers(Guid memberIdToExclude)
	{
		List<ClientPeer> peers = new List<ClientPeer>();
		foreach (GuildMember member in m_members.Values)
		{
			if (member.isLoggedIn && !(memberIdToExclude == member.id))
			{
				peers.Add(member.hero.account.peer);
			}
		}
		return peers;
	}

	public void RefreshDailyBanishmentCount(DateTime date)
	{
		if (!(date == m_dailyBanishmentCount.date))
		{
			m_dailyBanishmentCount.date = date;
			m_dailyBanishmentCount.value = 0;
		}
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
			ServerEvent.SendGuildFundChanged(GetClientPeers(memberIdToExcludeFromEventTarget), m_lnFund);
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
				throw new Exception("보유한 자금이 부족합니다.");
			}
			m_lnFund -= lnAmount;
			ServerEvent.SendGuildFundChanged(GetClientPeers(memberIdToExcludeFromEventTarget), m_lnFund);
		}
	}

	public void AddBuildingPoint(int nAmount, Guid memberIdToExcludeFromEventTarget)
	{
		if (nAmount < 0)
		{
			throw new ArgumentNullException("nAmount");
		}
		if (nAmount != 0)
		{
			m_nBuildingPoint += nAmount;
			ServerEvent.SendGuildBuildingPointChanged(GetClientPeers(memberIdToExcludeFromEventTarget), m_nBuildingPoint);
		}
	}

	public void AddBuildingInstance(GuildBuildingInstance buildingInst)
	{
		if (buildingInst == null)
		{
			throw new ArgumentNullException("buildingInst");
		}
		m_buildingInsts.Add(buildingInst.building.id, buildingInst);
		switch (buildingInst.building.id)
		{
		case 1:
			m_lobby = buildingInst;
			break;
		case 2:
			m_laboratory = buildingInst;
			break;
		case 3:
			m_shop = buildingInst;
			break;
		case 4:
			m_tankFactory = buildingInst;
			break;
		default:
			throw new Exception("유효하지 않는 길드건물ID 입니다. buildingId = " + buildingInst.building.id);
		}
	}

	public GuildBuildingInstance GetBuildingInstance(int nId)
	{
		if (!m_buildingInsts.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	public void InitBuildingInstances()
	{
		foreach (GuildBuilding building in Resource.instance.guildBuildings.Values)
		{
			GuildBuildingInstance buildingInstance = new GuildBuildingInstance();
			buildingInstance.Create(this, building.firstLevel);
			AddBuildingInstance(buildingInstance);
		}
	}

	public void AddFoodWarehouseExp(int nAmount)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount == 0)
		{
			return;
		}
		m_nFoodWarehouseExp += nAmount;
		GuildFoodWarehouse warehouse = Resource.instance.guildFoodWarehouse;
		GuildFoodWarehouseLevel level = warehouse.GetLevel(m_nFoodWarehouseLevel);
		if (level.isMax)
		{
			return;
		}
		int nNextLevelUpRequiredExp = 0;
		do
		{
			nNextLevelUpRequiredExp = level.nextLevelUpRequiredExp;
			if (m_nFoodWarehouseExp < nNextLevelUpRequiredExp)
			{
				break;
			}
			m_nFoodWarehouseExp -= nNextLevelUpRequiredExp;
			m_nFoodWarehouseLevel++;
			level = warehouse.GetLevel(m_nFoodWarehouseLevel);
		}
		while (!level.isMax);
	}

	public void CollectFoodWarehouse()
	{
		m_nFoodWarehouseLevel = 1;
		m_nFoodWarehouseExp = 0;
		m_foodWarehouseCollectionId = Guid.NewGuid();
	}

	public void RefreshMoralPoint(DateTime date)
	{
		if (!(m_moralPoint.date == date))
		{
			m_moralPoint.Set(date, 0);
		}
	}

	public void AddMoralPoint(DateTime date, int nAmount, Guid memberIdToExcludeFromEventTarget)
	{
		if (nAmount < 0)
		{
			throw new ArgumentOutOfRangeException("nAmount");
		}
		if (nAmount != 0)
		{
			RefreshMoralPoint(date);
			m_moralPoint.value += nAmount;
			ServerEvent.SendGuildMoralPointChanged(GetClientPeers(memberIdToExcludeFromEventTarget), m_moralPoint.date, m_moralPoint.value);
		}
	}

	public void AddGuildCall(GuildCall call)
	{
		if (call == null)
		{
			throw new ArgumentNullException("call");
		}
		m_guildCalls.Add(call.id, call);
	}

	public GuildCall GetGuildCall(long lnId)
	{
		if (!m_guildCalls.TryGetValue(lnId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveGuildCall(long lnId)
	{
		m_guildCalls.Remove(lnId);
	}

	public void OnGuildCallLifetimeEnded(long lnId)
	{
		RemoveGuildCall(lnId);
	}

	public void CallGuildMembers(Hero caller, Continent continent, int nNationId)
	{
		if (caller == null)
		{
			throw new ArgumentNullException("caller");
		}
		if (continent == null)
		{
			throw new ArgumentNullException("continent");
		}
		GuildCall guildCall = new GuildCall(caller, continent, nNationId, caller.position, caller.rotationY);
		AddGuildCall(guildCall);
		List<ClientPeer> peers = new List<ClientPeer>();
		foreach (GuildMember member in m_members.Values)
		{
			Hero hero = member.hero;
			if (hero != null && !(hero.id == caller.id))
			{
				peers.Add(hero.account.peer);
			}
		}
		ServerEvent.SendGuildCall(peers, guildCall.ToPDGuildCall());
	}

	public void SetGuildSupplySupportQuest(GuildSupplySupportQuestPlay play, Hero hero)
	{
		if (play == null)
		{
			throw new ArgumentNullException("play");
		}
		m_guildSupplySupportQuestPlay = play;
		if (hero != null)
		{
			lock (hero.syncObject)
			{
				hero.guildSupplySupportQuestPlay = play;
			}
		}
	}

	public void RemoveGuildSupplySupportQuest()
	{
		Hero hero = Cache.instance.GetLoggedInHero(m_guildSupplySupportQuestPlay.heroId);
		if (hero != null)
		{
			lock (hero.syncObject)
			{
				hero.guildSupplySupportQuestPlay = null;
			}
		}
		m_guildSupplySupportQuestPlay = null;
	}

	public void RefreshDailyGuildSupplySupportQuestStartCount(DateTime date)
	{
		if (!(m_dailyGuildSupplySupportQuestStartCount.date == date))
		{
			m_dailyGuildSupplySupportQuestStartCount.date = date;
			m_dailyGuildSupplySupportQuestStartCount.value = 0;
		}
	}

	public int GetDailyGuildSupplySupportQuestCount(DateTime date)
	{
		RefreshDailyGuildSupplySupportQuestStartCount(date);
		if (m_guildSupplySupportQuestPlay == null)
		{
			return m_dailyGuildSupplySupportQuestStartCount.value;
		}
		if (m_dailyGuildSupplySupportQuestStartCount.date == date)
		{
			return m_dailyGuildSupplySupportQuestStartCount.value - 1;
		}
		return m_dailyGuildSupplySupportQuestStartCount.value;
	}

	public void RefreshDailyHuntingDonationCount(DateTime date)
	{
		if (!(date == m_dailyHuntingDonationCount.date))
		{
			m_dailyHuntingDonationCount.date = date;
			m_dailyHuntingDonationCount.value = 0;
		}
	}

	public void AddDailyObjectiveCompletionMember(GuildDailyObjectiveCompletionMember member)
	{
		if (member == null)
		{
			throw new ArgumentNullException("member");
		}
		m_dailyObjectiveCompletionMembers.Add(member.id, member);
	}

	public List<PDGuildDailyObjectiveCompletionMember> GetPDGuildDailyObjectiveCompletionMembers()
	{
		List<PDGuildDailyObjectiveCompletionMember> insts = new List<PDGuildDailyObjectiveCompletionMember>();
		foreach (GuildDailyObjectiveCompletionMember member in m_dailyObjectiveCompletionMembers.Values)
		{
			insts.Add(member.ToPDGuildDailyObjectiveCompletionMember());
		}
		return insts;
	}

	public void RefreshDailyObjective(DateTimeOffset time, bool bSendEvent, bool bSaveToDB)
	{
		DateTime date = time.Date;
		if (date == m_dailyObjectiveDate)
		{
			return;
		}
		m_dailyObjectiveCompletionMembers.Clear();
		m_dailyObjectiveDate = date;
		m_nDailyObjectiveContentId = Resource.instance.SelectGuildContent().id;
		if (bSendEvent)
		{
			ServerEvent.SendGuildDailyObjectiveSet(GetClientPeers(Guid.Empty), date, m_nDailyObjectiveContentId);
		}
		if (bSaveToDB)
		{
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_DailyObjective(m_id, m_dailyObjectiveDate, m_nDailyObjectiveContentId));
			dbWork.Schedule();
		}
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGuildDailyObjectiveLog(Guid.NewGuid(), m_id, m_dailyObjectiveDate, m_nDailyObjectiveContentId, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void RefreshWeeklyObjective(DateTimeOffset currentTime, bool bSendEvent, bool bSaveToDB)
	{
		DateTime currentDate = currentTime.Date;
		DateTime currentDateOfMonday = DateTimeUtil.GetWeekStartDate(currentTime);
		if (currentDate != currentDateOfMonday)
		{
			if (!(m_weeklyObjectiveDate == currentDateOfMonday) || m_nWeeklyObjectiveId <= 0)
			{
				m_weeklyObjectiveDate = currentDateOfMonday;
				m_nWeeklyObjectiveId = Resource.instance.defaultGuildWeeklyObjectiveId;
				if (bSendEvent)
				{
					ServerEvent.SendGuildWeeklyObjectiveSet(GetClientPeers(Guid.Empty), m_weeklyObjectiveDate, m_nWeeklyObjectiveId);
				}
				if (bSaveToDB)
				{
					SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_id);
					dbWork.AddSqlCommand(GameDac.CSC_UpdateGuild_WeeklyObjective(m_id, m_weeklyObjectiveDate, m_nWeeklyObjectiveId));
					dbWork.Schedule();
				}
				try
				{
					SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
					logWork.AddSqlCommand(GameLogDac.CSC_AddGuildWeeklyObjectiveLog(Guid.NewGuid(), m_id, 1, Guid.Empty, m_nWeeklyObjectiveId, currentTime));
					logWork.Schedule();
				}
				catch (Exception ex)
				{
					SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
				}
			}
		}
		else if (!(m_weeklyObjectiveDate == currentDateOfMonday))
		{
			m_weeklyObjectiveDate = currentDateOfMonday;
			m_nWeeklyObjectiveId = 0;
			m_nWeeklyObjectiveCompletionMemberCount = 0;
		}
	}

	public void CompleteGuildDailyObjective(DateTime date, int nContentId, GuildMember member)
	{
		if (member == null)
		{
			throw new ArgumentNullException("member");
		}
		if (!(date != m_dailyObjectiveDate) && nContentId == m_nDailyObjectiveContentId && !m_dailyObjectiveCompletionMembers.ContainsKey(member.id))
		{
			AddDailyObjectiveCompletionMember(new GuildDailyObjectiveCompletionMember(this, member.id, member.name));
			ServerEvent.SendGuildDailyObjectiveCompletionMemberCountUpdated(GetClientPeers(Guid.Empty), m_dailyObjectiveCompletionMembers.Count);
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGuildWork(m_id);
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(member.id));
			dbWork.AddSqlCommand(GameDac.CSC_AddGuildDailyObjectiveCompletion(m_id, date, member.id));
			dbWork.Schedule();
			CompleteGuildDailyObjective_WeeklyObjective();
		}
	}

	private void CompleteGuildDailyObjective_WeeklyObjective()
	{
		m_nWeeklyObjectiveCompletionMemberCount++;
		ServerEvent.SendGuildWeeklyObjectiveCompletionMemberCountUpdated(GetClientPeers(Guid.Empty), m_nWeeklyObjectiveCompletionMemberCount);
	}

	public void OnDateChanged(DateTimeOffset time)
	{
		OnDateChanged_RefreshDailyObjective(time);
		OnDateChanged_RefreshWeeklyObjective(time);
	}

	private void OnDateChanged_RefreshDailyObjective(DateTimeOffset time)
	{
		try
		{
			RefreshDailyObjective(time, bSendEvent: true, bSaveToDB: true);
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void OnDateChanged_RefreshWeeklyObjective(DateTimeOffset time)
	{
		try
		{
			RefreshWeeklyObjective(time, bSendEvent: true, bSaveToDB: true);
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	public void StartBlessingBuff(GuildBlessingBuff buff, DateTimeOffset startTime)
	{
		if (buff == null)
		{
			throw new ArgumentNullException("buff");
		}
		if (m_blessingBuffTimer != null)
		{
			throw new Exception("이미 버프가 존재합니다.");
		}
		m_blessingBuff = buff;
		m_blessingBuffStartTime = startTime;
		ServerEvent.SendGuildBlessingBuffStarted(GetClientPeers(Guid.Empty), startTime.Date);
		StartBlessingBuffTimer(buff.duration * 1000);
	}

	public float GetRemainingBlessingBuffTime(DateTimeOffset currentTime)
	{
		if (m_blessingBuff == null)
		{
			return 0f;
		}
		float fElaspsedTime = (float)(currentTime - m_blessingBuffStartTime).TotalSeconds;
		return Math.Max((float)m_blessingBuff.duration - fElaspsedTime, 0f);
	}

	private void StartBlessingBuffTimer(int nDuration)
	{
		if (nDuration > 0)
		{
			m_blessingBuffTimer = new Timer(OnBlessingBuffTimerTick);
			m_blessingBuffTimer.Change(nDuration, -1);
		}
	}

	private void OnBlessingBuffTimerTick(object state)
	{
		Global.instance.AddWork(new SFAction(ProcessBlessingBuffTimerTickTick));
	}

	private void ProcessBlessingBuffTimerTickTick()
	{
		if (m_blessingBuffTimer != null)
		{
			DisposeBlessingBuffTimer();
			ServerEvent.SendGuildBlessingBuffEnded(GetClientPeers(Guid.Empty));
		}
	}

	private void DisposeBlessingBuffTimer()
	{
		if (m_blessingBuffTimer != null)
		{
			m_blessingBuffTimer.Dispose();
			m_blessingBuffTimer = null;
		}
	}

	public void Release()
	{
		if (m_bReleased)
		{
			return;
		}
		foreach (GuildInvitation invitation in m_invitations.Values)
		{
			invitation.Release();
		}
		m_invitations.Clear();
		foreach (GuildCall call in m_guildCalls.Values)
		{
			call.Release();
		}
		m_guildCalls.Clear();
		DisposeBlessingBuffTimer();
		m_bReleased = true;
	}

	public PDSimpleGuild ToPDSimpleGuild()
	{
		PDSimpleGuild inst = new PDSimpleGuild();
		inst.id = (Guid)m_id;
		inst.name = m_sName;
		inst.notice = m_sNotice;
		inst.level = m_lobby.level.level;
		inst.masterId = (Guid)m_master.id;
		inst.masterName = m_master.name;
		inst.memberCount = m_members.Count;
		return inst;
	}

	public PDGuild ToPDGuild(DateTimeOffset time)
	{
		DateTime date = time.Date;
		RefreshMoralPoint(date);
		RefreshDailyGuildSupplySupportQuestStartCount(date);
		RefreshDailyHuntingDonationCount(date);
		PDGuild inst = new PDGuild();
		inst.id = (Guid)m_id;
		inst.name = m_sName;
		inst.notice = m_sNotice;
		inst.buildingPoint = m_nBuildingPoint;
		inst.fund = m_lnFund;
		inst.applicationCount = m_applications.Count;
		inst.buildingInsts = GuildBuildingInstance.ToPDGuildBuildingInstances(m_buildingInsts.Values).ToArray();
		inst.foodWarehouseCollectionId = (Guid)m_foodWarehouseCollectionId;
		inst.moralPointDate = (DateTime)m_moralPoint.date;
		inst.moralPoint = m_moralPoint.value;
		inst.dailyGuildSupplySupportQuestStartDate = (DateTime)m_dailyGuildSupplySupportQuestStartCount.date;
		inst.dailyGuildSupplySupportQuestStartCount = m_dailyGuildSupplySupportQuestStartCount.value;
		inst.dailyHuntingDonationDate = (DateTime)m_dailyHuntingDonationCount.date;
		inst.dailyHuntingDonationCount = m_dailyHuntingDonationCount.value;
		inst.dailyObjectiveDate = (DateTime)m_dailyObjectiveDate;
		inst.dailyObjectiveContentId = m_nDailyObjectiveContentId;
		inst.dailyObjectiveCompletionMemberCount = m_dailyObjectiveCompletionMembers.Count;
		inst.weeklyObjectiveDate = (DateTime)m_weeklyObjectiveDate;
		inst.weeklyObjectiveId = m_nWeeklyObjectiveId;
		inst.weeklyObjectiveCompletionMemberCount = m_nWeeklyObjectiveCompletionMemberCount;
		inst.lastBlessingBuffStartDate = (DateTime)m_blessingBuffStartTime.Date;
		inst.isBlessingBuffRunning = isBlessingBuffRunning;
		return inst;
	}
}
