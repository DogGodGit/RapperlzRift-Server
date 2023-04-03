using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class NationWarInstance
{
	public class MonsterRegenTimerState
	{
		public Timer timer;

		public NationWarMonsterArrange arrange;

		public NationContinentInstance continentInst;

		public DateTimeOffset regenTime;
	}

	public const int kStatus_Init = 0;

	public const int kStatus_Start = 1;

	public const int kStatus_Finished = 2;

	private NationWar m_nationWar;

	private NationWarDeclaration m_declaration;

	private NationInstance m_offenseNationInst;

	private HashSet<Guid> m_offenseNationHeroes = new HashSet<Guid>();

	private Dictionary<Guid, Hero> m_offenseNationAllianceHeroes = new Dictionary<Guid, Hero>();

	private NationInstance m_defenseNationInst;

	private HashSet<Guid> m_defenseNationHeroes = new HashSet<Guid>();

	private Dictionary<Guid, Hero> m_defenseNationAllianceHeroes = new Dictionary<Guid, Hero>();

	private Dictionary<int, NationWarMonsterInstance> m_monsterInsts = new Dictionary<int, NationWarMonsterInstance>();

	private int m_nStatus;

	private DateTimeOffset m_regTime = DateTimeOffset.MinValue;

	private bool m_bIsReleased;

	private HashSet<MonsterRegenTimerState> m_monsterRegenTimerStates = new HashSet<MonsterRegenTimerState>();

	private int m_nActivationNpcId;

	public NationWarDeclaration declaration => m_declaration;

	public NationInstance OffenseNationInst => m_offenseNationInst;

	public Nation offenseNation => m_offenseNationInst.nation;

	public NationInstance defenseNationInst => m_defenseNationInst;

	public Nation defenseNation => m_defenseNationInst.nation;

	public int status => m_nStatus;

	public bool isReleased => m_bIsReleased;

	public int activationNpcId => m_nActivationNpcId;

	public void Init(NationWarDeclaration declaration, DateTimeOffset time)
	{
		if (declaration == null)
		{
			throw new ArgumentNullException("declaration");
		}
		m_nationWar = Resource.instance.nationWar;
		m_declaration = declaration;
		Cache cache = Cache.instance;
		m_offenseNationInst = cache.GetNationInstance(declaration.nationId);
		m_defenseNationInst = cache.GetNationInstance(declaration.targetNationId);
		m_regTime = time;
	}

	public void Start(DateTimeOffset time)
	{
		m_nStatus = 1;
		m_declaration.StartNationWar(this);
		foreach (NationWarMonsterArrange monsterArrange in m_nationWar.monsterArranges.Values)
		{
			NationContinentInstance continentInst = m_defenseNationInst.GetContinentInstance(monsterArrange.continentId);
			if (continentInst == null)
			{
				throw new Exception("monsterArrangeContinentId");
			}
			NationWarMonsterInstance monsterInst = CreateMonster(continentInst, monsterArrange, m_defenseNationInst.nation);
			AddMonster(monsterInst);
			lock (continentInst.syncObject)
			{
				continentInst.SpawnMonster(monsterInst, time);
				ServerEvent.SendNationWarMonsterSpawn(GetClientPeers(Guid.Empty), monsterInst.arrangeId, monsterInst.nationId);
			}
		}
	}

	private NationWarMonsterInstance CreateMonster(NationContinentInstance continentInst, NationWarMonsterArrange arrange, Nation nation)
	{
		NationWarMonsterInstance inst = new NationWarMonsterInstance();
		inst.Init(continentInst, this, arrange, nation);
		return inst;
	}

	private void AddMonster(NationWarMonsterInstance monsterInst)
	{
		m_monsterInsts.Add(monsterInst.arrangeId, monsterInst);
	}

	public NationWarMonsterInstance GetMonster(int nArrangeId)
	{
		if (!m_monsterInsts.TryGetValue(nArrangeId, out var value))
		{
			return null;
		}
		return value;
	}

	private void RemoveMonster(int nArrangeId)
	{
		m_monsterInsts.Remove(nArrangeId);
	}

	public void OnMonsterDead(NationWarMonsterInstance deadMonsterInst)
	{
		if (deadMonsterInst == null)
		{
			throw new ArgumentNullException("deadMonsterInst");
		}
		if (m_nStatus != 1)
		{
			return;
		}
		RemoveMonster(deadMonsterInst.arrangeId);
		ServerEvent.SendNationWarMonsterDead(GetClientPeers(Guid.Empty), deadMonsterInst.arrangeId);
		_ = deadMonsterInst.lastDamageTime;
		switch (deadMonsterInst.arrangeType)
		{
		case 1:
			Finish(offenseNation.id, deadMonsterInst.lastDamageTime);
			break;
		case 2:
		{
			NationContinentInstance monsterContinentInst = (NationContinentInstance)deadMonsterInst.currentPlace;
			bool bIsDecreaseFinalDamagePenalty = false;
			NationWarMonsterInstance createMonster = null;
			if (deadMonsterInst.nationId == defenseNation.id)
			{
				bIsDecreaseFinalDamagePenalty = true;
				createMonster = CreateMonster(monsterContinentInst, deadMonsterInst.arrange, offenseNation);
			}
			else
			{
				bIsDecreaseFinalDamagePenalty = false;
				createMonster = CreateMonster(monsterContinentInst, deadMonsterInst.arrange, defenseNation);
			}
			createMonster.SetPositionAndRotation(deadMonsterInst.position, deadMonsterInst.rotationY);
			createMonster.EnableMonsterHit();
			AddMonster(createMonster);
			lock (monsterContinentInst.syncObject)
			{
				monsterContinentInst.SpawnMonster(createMonster, DateTimeUtil.currentTime);
				ServerEvent.SendNationWarMonsterSpawn(GetClientPeers(Guid.Empty), createMonster.arrangeId, createMonster.nationId);
			}
			{
				foreach (NationWarMonsterInstance monsterInst2 in m_monsterInsts.Values)
				{
					if (monsterInst2.arrangeType != 1)
					{
						continue;
					}
					lock (monsterInst2.currentPlace.syncObject)
					{
						if (bIsDecreaseFinalDamagePenalty)
						{
							monsterInst2.DecreaseFinalDamagePenaltyRate();
						}
						else
						{
							monsterInst2.IncreaseFinalDamagePenaltyRate();
						}
					}
				}
				break;
			}
		}
		case 3:
		{
			foreach (NationWarMonsterInstance monsterInst in m_monsterInsts.Values)
			{
				if (monsterInst.arrangeType == 2)
				{
					lock (monsterInst.currentPlace.syncObject)
					{
						monsterInst.EnableMonsterHit();
					}
				}
			}
			break;
		}
		case 5:
			OnMonsterDead_Rock(deadMonsterInst);
			break;
		case 4:
			break;
		}
	}

	private void OnMonsterDead_Rock(NationWarMonsterInstance monsterInst)
	{
		m_nActivationNpcId = monsterInst.arrange.nationWarNpcId;
		DateTimeOffset monsterDeadTime = monsterInst.lastDamageTime;
		NationWarMonsterArrange arrange = monsterInst.arrange;
		int nRegenDelayTime = arrange.regenTime * 1000;
		MonsterRegenTimerState state = new MonsterRegenTimerState();
		state.arrange = arrange;
		state.continentInst = (NationContinentInstance)monsterInst.currentPlace;
		state.regenTime = monsterDeadTime.AddSeconds(arrange.regenTime);
		state.timer = new Timer(OnMonsterRegenTimerTick, state, -1, -1);
		state.timer.Change(nRegenDelayTime, -1);
		m_monsterRegenTimerStates.Add(state);
	}

	private void OnMonsterRegenTimerTick(object state)
	{
		MonsterRegenTimerState monsterRegenTimerState = (MonsterRegenTimerState)state;
		monsterRegenTimerState.continentInst.AddWork(new SFAction<MonsterRegenTimerState>(RegenMonster, monsterRegenTimerState), bGlobalLockRequired: false);
	}

	private void RegenMonster(MonsterRegenTimerState state)
	{
		if (m_nStatus == 1)
		{
			Timer timer = state.timer;
			timer.Dispose();
			m_monsterRegenTimerStates.Remove(state);
			m_nActivationNpcId = 0;
			NationContinentInstance continentInst = state.continentInst;
			NationWarMonsterInstance monsterInst = CreateMonster(state.continentInst, state.arrange, defenseNation);
			AddMonster(monsterInst);
			continentInst.SpawnMonster(monsterInst, DateTimeUtil.currentTime);
			ServerEvent.SendNationWarMonsterSpawn(GetClientPeers(Guid.Empty), monsterInst.arrangeId, monsterInst.nationId);
		}
	}

	public void Finish(int nWinNationId, DateTimeOffset time)
	{
		if (m_nStatus != 1)
		{
			return;
		}
		m_nStatus = 2;
		List<NationWarMember> offenseNationMembers = SetRanking(m_offenseNationHeroes);
		List<NationWarMember> defenseNationMembers = SetRanking(m_defenseNationHeroes);
		NationWar nationWar = Resource.instance.nationWar;
		ItemReward offenseNationItemReward1 = null;
		ItemReward offenseNationItemReward2 = null;
		ExploitPointReward offenseNationExploitPointReward = null;
		ItemReward offenseAllianceNationReward = null;
		int nOffenseNationNationWarPointReward = 0;
		ItemReward defenseNationItemReward1 = null;
		ItemReward defenseNationItemReward2 = null;
		ExploitPointReward defenseNationExploitPointReward = null;
		ItemReward defenseAllianceNationReward = null;
		int nDefenseNationNationWarPointReward = 0;
		bool bIsOffenseNationWin = false;
		int nOffenseNationNationPowerRanking = m_offenseNationInst.nationPowerRanking.ranking;
		int nDefenseNationNationPowerRanking = m_defenseNationInst.nationPowerRanking.ranking;
		int nOffenseNationRatingDifference = nOffenseNationNationPowerRanking - nDefenseNationNationPowerRanking;
		int nDefenseNationRatingDifference = nDefenseNationNationPowerRanking - nOffenseNationNationPowerRanking;
		if (nWinNationId == m_offenseNationInst.nationId)
		{
			bIsOffenseNationWin = true;
			offenseNationItemReward1 = nationWar.winNationItemReward1;
			offenseNationItemReward2 = nationWar.winNationItemReward2;
			offenseNationExploitPointReward = nationWar.winNationExploitPointReward;
			offenseAllianceNationReward = nationWar.winNationAllianceItemReward;
			nOffenseNationNationWarPointReward = m_nationWar.GetPointReward(nOffenseNationRatingDifference).winNationWarPoint;
			defenseNationItemReward1 = nationWar.loseNationItemReward1;
			defenseNationItemReward2 = nationWar.loseNationItemReward2;
			defenseNationExploitPointReward = nationWar.loseNationExploitPointReward;
			defenseAllianceNationReward = nationWar.loseNationAllianceItemReward;
			nDefenseNationNationWarPointReward = m_nationWar.GetPointReward(nDefenseNationRatingDifference).loseNationWarPoint;
		}
		else
		{
			bIsOffenseNationWin = false;
			defenseNationItemReward1 = nationWar.winNationItemReward1;
			defenseNationItemReward2 = nationWar.winNationItemReward2;
			defenseNationExploitPointReward = nationWar.winNationExploitPointReward;
			defenseAllianceNationReward = nationWar.winNationAllianceItemReward;
			nOffenseNationNationWarPointReward = m_nationWar.GetPointReward(nOffenseNationRatingDifference).loseNationWarPoint;
			offenseNationItemReward1 = nationWar.loseNationItemReward1;
			offenseNationItemReward2 = nationWar.loseNationItemReward2;
			offenseNationExploitPointReward = nationWar.loseNationExploitPointReward;
			offenseAllianceNationReward = nationWar.loseNationAllianceItemReward;
			nDefenseNationNationWarPointReward = m_nationWar.GetPointReward(nDefenseNationRatingDifference).winNationWarPoint;
		}
		NationReward(m_offenseNationInst, nOffenseNationNationWarPointReward, time, bIsOffenseNationWin);
		NationReward(m_defenseNationInst, nDefenseNationNationWarPointReward, time, !bIsOffenseNationWin);
		Reward(time, offenseNationItemReward1, offenseNationItemReward2, offenseNationExploitPointReward, m_offenseNationHeroes, bIsOffenseNationWin);
		Reward(time, defenseNationItemReward1, defenseNationItemReward2, defenseNationExploitPointReward, m_defenseNationHeroes, !bIsOffenseNationWin);
		_ = Cache.instance;
		if (offenseAllianceNationReward != null)
		{
			Hero[] array = m_offenseNationAllianceHeroes.Values.ToArray();
			foreach (Hero hero2 in array)
			{
				HeroSynchronizer.Exec(hero2, new SFAction<Hero, DateTimeOffset, ItemReward, bool>(ProcessAllianceNationHeroReward, hero2, time, offenseAllianceNationReward, bIsOffenseNationWin));
			}
		}
		if (defenseAllianceNationReward != null)
		{
			Hero[] array2 = m_defenseNationAllianceHeroes.Values.ToArray();
			foreach (Hero hero in array2)
			{
				HeroSynchronizer.Exec(hero, new SFAction<Hero, DateTimeOffset, ItemReward, bool>(ProcessAllianceNationHeroReward, hero, time, defenseAllianceNationReward, !bIsOffenseNationWin));
			}
		}
		m_offenseNationInst.OnNationWarFinish();
		m_defenseNationInst.OnNationWarFinish();
		foreach (NationWarMonsterInstance monsterInst in m_monsterInsts.Values)
		{
			Place currentPlace = monsterInst.currentPlace;
			lock (currentPlace.syncObject)
			{
				currentPlace.RemoveMonster(monsterInst, bSendEvent: true);
			}
		}
		m_monsterInsts.Clear();
		NationWarResult result = new NationWarResult();
		result.Init(m_declaration, nWinNationId, m_offenseNationInst.nationId, offenseNationMembers, m_defenseNationInst.nationId, defenseNationMembers, time);
		Cache.instance.nationWarManager.OnNationWarInstanceFinish(result);
		Close();
	}

	private void NationReward(NationInstance nationInst, int nPointReward, DateTimeOffset time, bool bIsWin)
	{
		nationInst.nationWarPoint += nPointReward;
		int nNationId = nationInst.nationId;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateNationWork(nNationId);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationInstance_NationWarPoint(nNationId, nationInst.nationWarPoint));
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddNationWarNationPowerReward(Guid.NewGuid(), m_declaration.id, nNationId, bIsWin, nPointReward, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void Reward(DateTimeOffset time, ItemReward itemReward1, ItemReward itemReward2, ExploitPointReward exploitReward, HashSet<Guid> rewardHeroIds, bool bIsWin)
	{
		Cache cache = Cache.instance;
		_ = Resource.instance.nationWar;
		foreach (Guid rewardHeroId in rewardHeroIds)
		{
			Hero loginHero = cache.GetHero(rewardHeroId);
			if (loginHero != null)
			{
				HeroSynchronizer.Exec(loginHero, new SFAction<Hero, DateTimeOffset, ItemReward, ItemReward, ExploitPointReward, bool>(ProcessReward, loginHero, time, itemReward1, itemReward2, exploitReward, bIsWin));
			}
		}
	}

	private void ProcessReward(Hero hero, DateTimeOffset time, ItemReward itemReward1, ItemReward itemReward2, ExploitPointReward exploitReward, bool bIsWin)
	{
		List<Mail> mails = new List<Mail>();
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(hero.id);
		List<PDItemBooty> joinBooties = new List<PDItemBooty>();
		long lnJoinAcquiredExp = 0L;
		int nJoinAcquiredExploitPoint = 0;
		Mail defaultRewardMail = null;
		if (itemReward1 != null)
		{
			Item item4 = itemReward1.item;
			bool bOwned4 = itemReward1.owned;
			int nCount4 = itemReward1.count;
			int nRewardItemRemainCount4 = hero.AddItem(item4, bOwned4, nCount4, changedInventorySlots);
			PDItemBooty booty3 = new PDItemBooty();
			booty3.id = item4.id;
			booty3.owned = bOwned4;
			booty3.count = nCount4;
			joinBooties.Add(booty3);
			if (nRewardItemRemainCount4 > 0)
			{
				defaultRewardMail = Mail.Create("MAIL_NAME_00014", "MAIL_DESC_00014", time);
				defaultRewardMail.AddAttachmentWithNo(new MailAttachment(item4, nRewardItemRemainCount4, bOwned4));
			}
		}
		if (itemReward2 != null)
		{
			Item item3 = itemReward2.item;
			bool bOwned3 = itemReward2.owned;
			int nCount3 = itemReward2.count;
			int nRewardItemRemainCount3 = hero.AddItem(item3, bOwned3, nCount3, changedInventorySlots);
			PDItemBooty booty2 = new PDItemBooty();
			booty2.id = item3.id;
			booty2.owned = bOwned3;
			booty2.count = nCount3;
			joinBooties.Add(booty2);
			if (nRewardItemRemainCount3 > 0)
			{
				if (defaultRewardMail == null)
				{
					defaultRewardMail = Mail.Create("MAIL_NAME_00014", "MAIL_DESC_00014", time);
				}
				defaultRewardMail.AddAttachmentWithNo(new MailAttachment(item3, nRewardItemRemainCount3, bOwned3));
			}
		}
		if (defaultRewardMail != null)
		{
			mails.Add(defaultRewardMail);
		}
		int nJoinRewardExploitPoint = 0;
		if (exploitReward != null)
		{
			nJoinRewardExploitPoint = exploitReward.value;
			nJoinAcquiredExploitPoint = hero.AddExploitPoint(nJoinRewardExploitPoint, time, bSaveToDB: false);
		}
		NationWarExpReward nationWarExpReward = m_nationWar.GetExpReward(hero.level);
		if (nationWarExpReward != null)
		{
			ExpReward expReward = nationWarExpReward.expReward;
			if (expReward != null)
			{
				lnJoinAcquiredExp = expReward.value;
				lnJoinAcquiredExp = (long)Math.Floor((float)lnJoinAcquiredExp * Cache.instance.GetWorldLevelExpFactor(hero.level));
				hero.AddExp(lnJoinAcquiredExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
			}
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(hero));
		}
		int nObjectiveAchievementOwnDia = 0;
		int nObjectiveAchievementAcquiredExploitPoint = 0;
		int nObjectiveAchievementRewardExploitPoint = 0;
		NationWarMember nationWarMember = hero.nationWarMember;
		foreach (NationWarHeroObjectiveEntry entry in m_nationWar.heroObjectiveEntries)
		{
			int nRewardOwnDia = 0;
			ExploitPointReward exploitPointReward = null;
			switch (entry.type)
			{
			case 1:
				if (bIsWin)
				{
					if (entry.rewardType == 1)
					{
						exploitPointReward = entry.exploitPointReward;
					}
					else
					{
						nRewardOwnDia = entry.ownDiaRewardValue;
					}
				}
				break;
			case 2:
				if (!bIsWin)
				{
					if (entry.rewardType == 1)
					{
						exploitPointReward = entry.exploitPointReward;
					}
					else
					{
						nRewardOwnDia = entry.ownDiaRewardValue;
					}
				}
				break;
			case 3:
				if (nationWarMember.killCount >= entry.objectiveCount)
				{
					if (entry.rewardType == 1)
					{
						exploitPointReward = entry.exploitPointReward;
					}
					else
					{
						nRewardOwnDia = entry.ownDiaRewardValue;
					}
				}
				break;
			case 4:
				if (nationWarMember.immediateRevivalCount >= entry.objectiveCount)
				{
					if (entry.rewardType == 1)
					{
						exploitPointReward = entry.exploitPointReward;
					}
					else
					{
						nRewardOwnDia = entry.ownDiaRewardValue;
					}
				}
				break;
			}
			if (nRewardOwnDia > 0)
			{
				hero.AddOwnDia(nRewardOwnDia);
				nObjectiveAchievementOwnDia += nRewardOwnDia;
				dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_OwnDia(hero));
			}
			else if (exploitPointReward != null)
			{
				int nExploitPoint = exploitPointReward.value;
				nObjectiveAchievementRewardExploitPoint += nExploitPoint;
				nObjectiveAchievementAcquiredExploitPoint += hero.AddExploitPoint(nExploitPoint, time, bSaveToDB: false);
			}
		}
		PDItemBooty rankingBooty = null;
		NationWarRankingReward rankingReward = m_nationWar.GetRankingReward(nationWarMember.ranking);
		if (rankingReward != null)
		{
			ItemReward itemReward3 = rankingReward.itemReward;
			if (itemReward3 != null)
			{
				Item item2 = itemReward3.item;
				bool bOwned2 = itemReward3.owned;
				int nCount2 = itemReward3.count;
				int nRewardItemRemainCount2 = hero.AddItem(item2, bOwned2, nCount2, changedInventorySlots);
				rankingBooty = new PDItemBooty();
				rankingBooty.id = item2.id;
				rankingBooty.owned = bOwned2;
				rankingBooty.count = nCount2;
				if (nRewardItemRemainCount2 > 0)
				{
					Mail mail3 = Mail.Create("MAIL_NAME_00015", "MAIL_DESC_00015", time);
					mail3.AddAttachmentWithNo(new MailAttachment(item2, nRewardItemRemainCount2, bOwned2));
					hero.AddMail(mail3, bSendEvent: true);
					mails.Add(mail3);
				}
			}
		}
		PDItemBooty luckyBooty = null;
		if (nationWarMember.luckyReward)
		{
			ItemReward luckyItemReward = m_nationWar.luckyItemReward;
			if (luckyItemReward != null)
			{
				Item item = luckyItemReward.item;
				bool bOwned = luckyItemReward.owned;
				int nCount = luckyItemReward.count;
				int nRewardItemRemainCount = hero.AddItem(item, bOwned, nCount, changedInventorySlots);
				luckyBooty = new PDItemBooty();
				luckyBooty.id = item.id;
				luckyBooty.owned = bOwned;
				luckyBooty.count = nCount;
				if (nRewardItemRemainCount > 0)
				{
					Mail mail2 = Mail.Create("MAIL_NAME_00016", "MAIL_DESC_00016", time);
					mail2.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainCount, bOwned));
					hero.AddMail(mail2, bSendEvent: true);
					mails.Add(mail2);
				}
			}
		}
		if (bIsWin)
		{
			hero.accNationWarWinCount++;
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_NationWarWinCount(hero.id, hero.accNationWarWinCount));
		}
		nationWarMember.rewarded = true;
		nationWarMember.rewardedExp = lnJoinAcquiredExp;
		if (nJoinAcquiredExploitPoint + nObjectiveAchievementAcquiredExploitPoint > 0)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Exploit(hero));
		}
		foreach (InventorySlot slot in changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		foreach (Mail mail in mails)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(mail));
		}
		dbWork.AddSqlCommand(GameDac.CSC_UpdateNationWarMember_Rewarded(nationWarMember.declaration.id, nationWarMember.heroId, nationWarMember.rewarded));
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			Guid logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddNationWarRerwardLog(logId, m_declaration.id, hero.id, bIsWin, nJoinRewardExploitPoint, nJoinAcquiredExploitPoint, lnJoinAcquiredExp, time));
			foreach (PDItemBooty booty in joinBooties)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddNationWarRewardDetailLog(Guid.NewGuid(), logId, booty.id, booty.count, booty.owned));
			}
			logWork.AddSqlCommand(GameLogDac.CSC_AddNationWarHeroObjectiveRewardLog(Guid.NewGuid(), m_declaration.id, hero.id, bIsWin, nationWarMember.killCount, nationWarMember.immediateRevivalCount, nObjectiveAchievementOwnDia, nObjectiveAchievementRewardExploitPoint, nObjectiveAchievementAcquiredExploitPoint, time));
			if (rankingBooty != null)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddNationWarRankingRewardLog(Guid.NewGuid(), hero.id, 1, nationWarMember.ranking, rankingBooty.id, rankingBooty.count, rankingBooty.owned, time));
			}
			if (luckyBooty != null)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddNationWarRankingRewardLog(Guid.NewGuid(), hero.id, 2, nationWarMember.ranking, luckyBooty.id, luckyBooty.count, luckyBooty.owned, time));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
		if (bIsWin)
		{
			ServerEvent.SendNationWarWin(hero.account.peer, joinBooties.ToArray(), lnJoinAcquiredExp, nJoinAcquiredExploitPoint, nObjectiveAchievementOwnDia, nObjectiveAchievementAcquiredExploitPoint, rankingBooty, luckyBooty, hero.level, hero.exp, hero.exploitPoint, hero.realMaxHP, hero.hp, hero.ownDia, hero.dailyExploitPoint.date, hero.dailyExploitPoint.value, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray(), hero.accNationWarWinCount);
		}
		else
		{
			ServerEvent.SendNationWarLose(hero.account.peer, joinBooties.ToArray(), lnJoinAcquiredExp, nJoinAcquiredExploitPoint, nObjectiveAchievementOwnDia, nObjectiveAchievementAcquiredExploitPoint, rankingBooty, luckyBooty, hero.level, hero.exp, hero.exploitPoint, hero.realMaxHP, hero.hp, hero.ownDia, hero.dailyExploitPoint.date, hero.dailyExploitPoint.value, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
		}
		hero.ProcessOrdealQuestMissions(OrdealQuestMissionType.NationWar, 1, time);
	}

	private void ProcessAllianceNationHeroReward(Hero hero, DateTimeOffset time, ItemReward itemReward, bool bIsWin)
	{
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		Mail mail = null;
		Item item = itemReward.item;
		bool bOwned = itemReward.owned;
		int nCount = itemReward.count;
		int nRewardItemRemainCount = hero.AddItem(item, bOwned, nCount, changedInventorySlots);
		PDItemBooty booty = new PDItemBooty();
		booty.id = item.id;
		booty.owned = bOwned;
		booty.count = nCount;
		if (nRewardItemRemainCount > 0)
		{
			mail = Mail.Create("MAIL_NAME_00014", "MAIL_DESC_00014", time);
			mail.AddAttachment(new MailAttachment(item, nRewardItemRemainCount, bOwned));
			hero.AddMail(mail, bSendEvent: true);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(hero.id);
		foreach (InventorySlot slot in changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(mail));
		}
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddNationWarAllianceRewardLog(Guid.NewGuid(), m_declaration.id, hero.id, bIsWin, item.id, bOwned, nCount, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		ServerEvent.SendNationWarAllianceNationReward(hero.account.peer, bIsWin, booty, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
		hero.ClearAllianceNationWar();
	}

	private static List<NationWarMember> SetRanking(IEnumerable<Guid> heroIds)
	{
		List<NationWarMember> results = new List<NationWarMember>();
		Cache cache = Cache.instance;
		foreach (Guid heroId in heroIds)
		{
			Hero loginHero = cache.GetLoggedInHero(heroId);
			if (loginHero != null)
			{
				lock (loginHero.syncObject)
				{
					results.Add(loginHero.nationWarMember);
				}
			}
		}
		results.Sort(NationWarMember.Compare);
		results.Reverse();
		NationWar nationWar = Resource.instance.nationWar;
		List<int> luckyRewardRankings = new List<int>();
		int nResultCount = results.Count;
		int nLuckyRewardHighRanking = nationWar.luckyRewardHighRanking;
		int nLuckyRewardLowRanking = Math.Min(nationWar.luckyRewardLowRanking, nResultCount);
		int nLuckyRewardHeroCount = Math.Min(nationWar.luckyRewardHeroCount, Math.Max(nLuckyRewardLowRanking - nLuckyRewardHighRanking + 1, 0));
		if (nLuckyRewardHighRanking <= nLuckyRewardLowRanking && nResultCount >= nLuckyRewardHighRanking)
		{
			luckyRewardRankings = SelectRandomRankings(nLuckyRewardHighRanking, nLuckyRewardLowRanking, nLuckyRewardHeroCount);
		}
		int nRanking = 0;
		foreach (NationWarMember member in results)
		{
			nRanking = (member.ranking = nRanking + 1);
			GuildMember guildMember = Cache.instance.GetGuildMember(member.heroId);
			if (guildMember != null)
			{
				member.guildId = guildMember.guild.id;
				member.guildName = guildMember.guild.name;
			}
			if (luckyRewardRankings.Contains(nRanking))
			{
				member.luckyReward = true;
			}
		}
		return results;
	}

	private static List<int> SelectRandomRankings(int nHighRanking, int nLowRanking, int nCount)
	{
		List<int> rankings = new List<int>();
		for (int j = nHighRanking; j <= nLowRanking; j++)
		{
			rankings.Add(j);
		}
		List<int> results = new List<int>();
		for (int i = 0; i < nCount; i++)
		{
			int nNumber = rankings[SFRandom.Next(0, rankings.Count)];
			results.Add(nNumber);
			rankings.Remove(nNumber);
		}
		return results;
	}

	public void AddHero(Hero hero, DateTimeOffset time)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		NationWarMember member = null;
		int nHeroNationId = hero.nationId;
		if (nHeroNationId == offenseNation.id)
		{
			m_offenseNationHeroes.Add(hero.id);
			member = (hero.nationWarMember = new NationWarMember(m_declaration, hero.id, hero.name, time));
		}
		else if (nHeroNationId == m_offenseNationInst.allianceNationId)
		{
			m_offenseNationAllianceHeroes.Add(hero.id, hero);
			hero.JoinAllianceNationWar(this);
		}
		else if (nHeroNationId == defenseNation.id)
		{
			m_defenseNationHeroes.Add(hero.id);
			member = (hero.nationWarMember = new NationWarMember(m_declaration, hero.id, hero.name, time));
		}
		else if (nHeroNationId == m_defenseNationInst.allianceNationId)
		{
			m_defenseNationAllianceHeroes.Add(hero.id, hero);
			hero.JoinAllianceNationWar(this);
		}
		if (member != null)
		{
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(hero.id);
			dbWork.AddSqlCommand(GameDacEx.CSC_AddNationWarMember(member));
			dbWork.Schedule();
		}
	}

	public void RemoveHero(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		int nHeroNationId = hero.nationId;
		Guid heroId = hero.id;
		if (nHeroNationId == offenseNation.id)
		{
			m_offenseNationHeroes.Remove(heroId);
		}
		else if (nHeroNationId == m_offenseNationInst.allianceNationId)
		{
			m_offenseNationAllianceHeroes.Remove(heroId);
		}
		else if (nHeroNationId == defenseNation.id)
		{
			m_defenseNationHeroes.Remove(heroId);
		}
		else if (nHeroNationId == m_defenseNationInst.allianceNationId)
		{
			m_defenseNationAllianceHeroes.Remove(heroId);
		}
	}

	public bool ContainsRealJoinNationHero(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		int nNationId = hero.nationId;
		if (nNationId == offenseNation.id)
		{
			return m_offenseNationHeroes.Contains(hero.id);
		}
		if (nNationId == defenseNation.id)
		{
			return m_defenseNationHeroes.Contains(hero.id);
		}
		return false;
	}

	public bool ContainsHero(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		int nNationId = hero.nationId;
		if (nNationId == offenseNation.id)
		{
			return m_offenseNationHeroes.Contains(hero.id);
		}
		if (nNationId == m_offenseNationInst.allianceNationId)
		{
			return m_offenseNationAllianceHeroes.ContainsKey(hero.id);
		}
		if (nNationId == defenseNation.id)
		{
			return m_defenseNationHeroes.Contains(hero.id);
		}
		if (nNationId == m_defenseNationInst.allianceNationId)
		{
			return m_defenseNationAllianceHeroes.ContainsKey(hero.id);
		}
		return false;
	}

	public bool IsNationWarJoinEnabled(Hero hero)
	{
		int nNationId = hero.nationId;
		if (nNationId == offenseNation.id || nNationId == m_offenseNationInst.allianceNationId || nNationId == defenseNation.id || nNationId == m_defenseNationInst.allianceNationId)
		{
			return true;
		}
		return false;
	}

	public bool IsEnabledNpcTransmission(int nNpcId)
	{
		bool bIsEnabled = true;
		foreach (NationWarMonsterInstance monsterInst in m_monsterInsts.Values)
		{
			if (monsterInst.arrangeType == 5 && !monsterInst.IsEnableNpc(nNpcId))
			{
				return false;
			}
		}
		return bIsEnabled;
	}

	public List<ClientPeer> GetClientPeers(Guid heroIdToExclude)
	{
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		foreach (Hero offenseHero in m_offenseNationInst.heroes.Values)
		{
			if (heroIdToExclude != offenseHero.id)
			{
				clientPeers.Add(offenseHero.account.peer);
			}
		}
		foreach (Hero offenseAllianceNationHero in m_offenseNationAllianceHeroes.Values)
		{
			if (heroIdToExclude != offenseAllianceNationHero.id)
			{
				clientPeers.Add(offenseAllianceNationHero.account.peer);
			}
		}
		foreach (Hero defenseHero in m_defenseNationInst.heroes.Values)
		{
			if (heroIdToExclude != defenseHero.id)
			{
				clientPeers.Add(defenseHero.account.peer);
			}
		}
		foreach (Hero defenseAllianceNationHeroes in m_defenseNationAllianceHeroes.Values)
		{
			if (heroIdToExclude != defenseAllianceNationHeroes.id)
			{
				clientPeers.Add(defenseAllianceNationHeroes.account.peer);
			}
		}
		return clientPeers;
	}

	public void OnUpdate(DateTimeOffset time)
	{
		OnUpdate_LifeTime(time);
		OnUpdate_NationInstance(time);
	}

	private void OnUpdate_LifeTime(DateTimeOffset time)
	{
		if (m_regTime.Date != time.Date)
		{
			Finish(m_defenseNationInst.nationId, time);
			return;
		}
		float fTotalSeconds = (float)time.TimeOfDay.TotalSeconds;
		if (!(fTotalSeconds < (float)Resource.instance.nationWar.endTime))
		{
			Finish(m_defenseNationInst.nationId, time);
		}
	}

	private void OnUpdate_NationInstance(DateTimeOffset time)
	{
		if (m_nStatus == 1)
		{
			m_offenseNationInst.OnNationWarInstanceUpdate(time);
			m_defenseNationInst.OnNationWarInstanceUpdate(time);
		}
	}

	public void OnMonsterBattleModeStart(NationWarMonsterInstance monsterInst)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		if (m_nStatus == 1)
		{
			ServerEvent.SendNationWarMonsterBattleModeStart(GetClientPeers(Guid.Empty), monsterInst.arrangeId);
		}
	}

	public void OnMonsterBattleModeEnd(NationWarMonsterInstance monsterInst)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		if (m_nStatus == 1)
		{
			ServerEvent.SendNationWarMonsterBattleModeEnd(GetClientPeers(Guid.Empty), monsterInst.arrangeId);
		}
	}

	public void OnMonsterEmergency(NationWarMonsterInstance monsterInst)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		if (m_nStatus == 1)
		{
			ServerEvent.SendNationWarMonsterEmergency(GetClientPeers(Guid.Empty), monsterInst.arrangeId);
		}
	}

	public void OnHeroMultiKill(Hero hero)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		if (m_nStatus == 1)
		{
			ServerEvent.SendNationWarMultiKill(GetClientPeers(Guid.Empty), hero.id, hero.name, hero.nationId, hero.nationWarMember.killCount);
		}
	}

	public void OnNoblesseHeroDead(Hero killerHero, Hero deadNoblesseHero)
	{
		if (killerHero == null)
		{
			throw new ArgumentNullException("killerHero");
		}
		if (deadNoblesseHero == null)
		{
			throw new ArgumentNullException("deadNoblesseHero");
		}
		if (m_nStatus == 1 && deadNoblesseHero.nationNoblesse != null)
		{
			ServerEvent.SendNationWarNoblesseKill(GetClientPeers(Guid.Empty), killerHero.id, killerHero.name, killerHero.nationId, deadNoblesseHero.id, deadNoblesseHero.name, deadNoblesseHero.nationNoblesseId, deadNoblesseHero.nationId);
		}
	}

	public void Close()
	{
		Release();
		Cache.instance.nationWarManager.RemoveNationWarInstance(m_declaration.id);
	}

	public void Release()
	{
		if (m_bIsReleased)
		{
			return;
		}
		foreach (MonsterRegenTimerState state in m_monsterRegenTimerStates)
		{
			state.timer.Dispose();
		}
		m_monsterRegenTimerStates.Clear();
		m_bIsReleased = true;
	}

	public List<PDSimpleNationWarMonsterInstance> GetPDSimpleNationWarMonsterInstances()
	{
		List<PDSimpleNationWarMonsterInstance> results = new List<PDSimpleNationWarMonsterInstance>();
		foreach (NationWarMonsterInstance monsterInst in m_monsterInsts.Values)
		{
			results.Add(monsterInst.ToPDSimpleNationWarMonsterInstance());
		}
		return results;
	}
}
