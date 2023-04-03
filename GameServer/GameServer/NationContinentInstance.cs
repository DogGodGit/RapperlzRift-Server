using System;
using System.Collections.Generic;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class NationContinentInstance : ContinentInstance
{
	public class EliteMonsterSpawnTimerState
	{
		public Timer timer;

		public int currentSpawnScheduleIndex;
	}

	private Nation m_nation;

	protected Dictionary<int, ContinentEliteMonsterInstance> m_eliteMonsterInsts = new Dictionary<int, ContinentEliteMonsterInstance>();

	protected Dictionary<int, EliteMonsterSpawnTimerState> m_eliteMonsterSpawnTimerStates = new Dictionary<int, EliteMonsterSpawnTimerState>();

	public override PlaceType placeType => PlaceType.NationContinent;

	public override int locationParam => m_nation.id;

	public override int nationId => m_nation.id;

	public Nation nation => m_nation;

	public override bool pvpEnabled => true;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => true;

	public override bool isExpScrollBuffEnabled => true;

	public override bool isExpLevelPenaltyEnabled => true;

	public override bool isWorldLevelExpBuffEnabled => true;

	public Dictionary<int, ContinentEliteMonsterInstance> eliteMonsterInsts => m_eliteMonsterInsts;

	public void InitNationContinent(NationInstance nationInst, Continent continent)
	{
		if (nationInst == null)
		{
			throw new ArgumentNullException("nationInst");
		}
		if (continent == null)
		{
			throw new ArgumentNullException("continent");
		}
		m_nation = nationInst.nation;
		InitContinent(continent);
		EliteMonsterCategory eliteMonsterCategory = m_continent.eliteMonsterCategory;
		if (eliteMonsterCategory == null)
		{
			return;
		}
		foreach (EliteMonsterMaster master in eliteMonsterCategory.masters.Values)
		{
			EliteMonsterSpawnTimerState state = new EliteMonsterSpawnTimerState();
			state.currentSpawnScheduleIndex = -1;
			m_eliteMonsterSpawnTimerStates.Add(master.id, state);
			StartNextEliteMonsterSpawnTimer(master, (float)DateTimeUtil.currentTime.TimeOfDay.TotalSeconds);
		}
	}

	public override bool IsSame(int nTargetContinentId, int nTargetNationId)
	{
		if (m_continent.id == nTargetContinentId)
		{
			return m_nation.id == nTargetNationId;
		}
		return false;
	}

	protected override void OnHeroEnter(Hero hero, DateTimeOffset time)
	{
		base.OnHeroEnter(hero, time);
		NationWarInstance nationWarInst = Cache.instance.GetNationInstance(m_nation.id).nationWarInst;
		if (nationWarInst != null && m_nation.id == nationWarInst.defenseNation.id && m_continent.isNationWarTarget && !nationWarInst.ContainsHero(hero) && hero.level >= Resource.instance.pvpMinHeroLevel)
		{
			if (hero.isDistorting)
			{
				hero.CancelDistortion(bSendEventToMyself: true);
			}
			nationWarInst.AddHero(hero, time);
		}
	}

	protected override void OnHeroDead(Hero hero)
	{
		base.OnHeroDead(hero);
		AddWork(new SFAction<Hero>(OnHeroDeadAsync, hero), bGlobalLockRequired: true);
	}

	private void OnHeroDeadAsync(Hero hero)
	{
		lock (hero.syncObject)
		{
			ProcessNationWarHeroDead(hero);
		}
	}

	private void ProcessNationWarHeroDead(Hero hero)
	{
		NationWarInstance nationWarInst = Cache.instance.GetNationInstance(m_nation.id).nationWarInst;
		if (nationWarInst == null || m_nation.id != nationWarInst.defenseNation.id || !m_continent.isNationWarTarget || !nationWarInst.ContainsHero(hero))
		{
			return;
		}
		Hero killer = hero.lastAttacker as Hero;
		if (nationWarInst.ContainsRealJoinNationHero(hero))
		{
			hero.NationWarIncreaseDeadCount();
		}
		if (hero.nationNoblesse != null && killer != null)
		{
			nationWarInst.OnNoblesseHeroDead(killer, hero);
		}
		if (killer != null)
		{
			Hero heroKiller = GetHero(killer.id);
			if (heroKiller != null)
			{
				lock (heroKiller.syncObject)
				{
					if (nationWarInst.ContainsRealJoinNationHero(heroKiller))
					{
						heroKiller.NationWarIncreaseKillCount(hero.lastDamageTime);
					}
					else
					{
						heroKiller.IncreaseAllianceNationWarKillCount();
					}
				}
			}
		}
		foreach (Guid heroId in hero.receivedDamages.Keys)
		{
			Hero assistHero = GetHero(heroId);
			if (assistHero != null && nationWarInst.ContainsRealJoinNationHero(assistHero) && (killer == null || !(killer.id == heroId)))
			{
				lock (assistHero.syncObject)
				{
					assistHero.NationWarIncreaseAssistCount();
				}
			}
		}
	}

	private void StartNextEliteMonsterSpawnTimer(EliteMonsterMaster master, float fTime)
	{
		if (master.spawnSchedules.Count == 0)
		{
			return;
		}
		EliteMonsterSpawnSchedule schedule = null;
		EliteMonsterSpawnTimerState state = GetEliteMonsterSpawnTimerState(master.id);
		do
		{
			state.currentSpawnScheduleIndex++;
			if (state.currentSpawnScheduleIndex >= master.spawnSchedules.Count)
			{
				state.currentSpawnScheduleIndex = 0;
				fTime -= 86400f;
			}
			schedule = master.GetSpawnScheduleAt(state.currentSpawnScheduleIndex);
		}
		while (fTime > (float)schedule.spawnTime);
		int nDelayTime = (int)Math.Floor(((float)schedule.spawnTime - fTime) * 1000f);
		if (state.timer == null)
		{
			state.timer = new Timer(OnEliteMonsterSpawnTimerTick, master, -1, -1);
		}
		state.timer.Change(nDelayTime, -1);
	}

	private void OnEliteMonsterSpawnTimerTick(object state)
	{
		AddWork(new SFAction<EliteMonsterMaster>(SpawnEliteMonster, (EliteMonsterMaster)state), bGlobalLockRequired: true);
	}

	private void SpawnEliteMonster(EliteMonsterMaster eliteMonsterMaster)
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		ContinentEliteMonsterInstance monsterInst = GetEliteMonsterByMasterId(eliteMonsterMaster.id);
		if (monsterInst == null)
		{
			monsterInst = new ContinentEliteMonsterInstance();
			monsterInst.Init(this, eliteMonsterMaster, currentTime);
			AddEliteMonster(monsterInst);
			SpawnMonster(monsterInst, currentTime);
			NationInstance nationInst = Cache.instance.GetNationInstance(nationId);
			ServerEvent.SendEliteMonsterSpawn(nationInst.GetClientPeers(Guid.Empty), monsterInst.eliteMonsterId);
		}
		StartNextEliteMonsterSpawnTimer(eliteMonsterMaster, (float)currentTime.TimeOfDay.TotalSeconds);
	}

	public void AddEliteMonster(ContinentEliteMonsterInstance eliteMonsterInst)
	{
		if (eliteMonsterInst == null)
		{
			throw new ArgumentNullException("eliteMonsterInst");
		}
		m_eliteMonsterInsts.Add(eliteMonsterInst.eliteMonsterId, eliteMonsterInst);
	}

	public void RemoveEliteMonster(int nEliteMonsterId)
	{
		m_eliteMonsterInsts.Remove(nEliteMonsterId);
	}

	private EliteMonsterSpawnTimerState GetEliteMonsterSpawnTimerState(int nMasterId)
	{
		if (!m_eliteMonsterSpawnTimerStates.TryGetValue(nMasterId, out var value))
		{
			return null;
		}
		return value;
	}

	public ContinentEliteMonsterInstance GetEliteMonsterByMasterId(int nMasterId)
	{
		foreach (ContinentEliteMonsterInstance monsterInst in m_eliteMonsterInsts.Values)
		{
			if (monsterInst.eliteMonsterMasterId == nMasterId)
			{
				return monsterInst;
			}
		}
		return null;
	}

	public override void RemoveMonster(MonsterInstance monsterInst, bool bSendEvent)
	{
		base.RemoveMonster(monsterInst, bSendEvent);
		if (monsterInst is ContinentEliteMonsterInstance eliteMonsterinst)
		{
			AddWork(new SFAction<int>(OnRemoveEliteMonster, eliteMonsterinst.eliteMonsterId), bGlobalLockRequired: true);
		}
	}

	private void OnRemoveEliteMonster(int nEliteMonsterId)
	{
		RemoveEliteMonster(nEliteMonsterId);
		NationInstance nationInst = Cache.instance.GetNationInstance(nationId);
		ServerEvent.SendEliteMonsterRemoved(nationInst.GetClientPeers(Guid.Empty), nEliteMonsterId);
	}

	protected override void ReleaseInternal()
	{
		base.ReleaseInternal();
		foreach (EliteMonsterSpawnTimerState state in m_eliteMonsterSpawnTimerStates.Values)
		{
			state.timer.Dispose();
		}
		m_eliteMonsterSpawnTimerStates.Clear();
	}
}
