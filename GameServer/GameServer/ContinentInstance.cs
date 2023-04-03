using System;
using System.Collections.Generic;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public abstract class ContinentInstance : Place
{
	public class MonsterRegenTimerState
	{
		public Timer timer;

		public ContinentMonsterArrange arrange;

		public DateTimeOffset regenTime;
	}

	public class ContinentObjectRegenTimerState
	{
		public Timer timer;

		public ContinentObjectArrange arrange;
	}

	protected Continent m_continent;

	protected HashSet<MonsterRegenTimerState> m_monsterRegenTimerStates = new HashSet<MonsterRegenTimerState>();

	protected Dictionary<long, ContinentObjectInstance> m_objectInsts = new Dictionary<long, ContinentObjectInstance>();

	protected HashSet<ContinentObjectRegenTimerState> m_objectRegenTimerStates = new HashSet<ContinentObjectRegenTimerState>();

	protected Dictionary<long, CartInstance> m_cartInsts = new Dictionary<long, CartInstance>();

	protected Dictionary<int, FieldBossMonsterInstance> m_fieldBossMonsterInsts = new Dictionary<int, FieldBossMonsterInstance>();

	public override Location location => m_continent;

	public bool isNationTerritory => m_continent.isNationTerritory;

	public abstract int nationId { get; }

	public override Rect3D mapRect => m_continent.mapRect;

	public override bool interestManaged => true;

	public override bool ownershipManaged => true;

	public override bool battleEnabled => true;

	public Continent continent => m_continent;

	public Dictionary<int, FieldBossMonsterInstance> fieldBossMonsterInsts => m_fieldBossMonsterInsts;

	protected void InitContinent(Continent continent)
	{
		m_continent = continent;
		InitPlace();
		foreach (ContinentMonsterArrange arrange2 in m_continent.monsterArranges.Values)
		{
			InitMonster(arrange2);
		}
		foreach (ContinentObjectArrange arrange in m_continent.objectArranges.Values)
		{
			InitObject(arrange);
		}
	}

	private void InitMonster(ContinentMonsterArrange arrange)
	{
		for (int i = 0; i < arrange.monsterCount; i++)
		{
			AddMonster(CreateMonster(arrange));
		}
	}

	private ContinentMonsterInstance CreateMonster(ContinentMonsterArrange arrange)
	{
		ContinentMonsterInstance monsterInst = new ContinentMonsterInstance();
		monsterInst.Init(this, arrange);
		return monsterInst;
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		switch (monsterInst.monsterInstanceType)
		{
		case MonsterInstanceType.ContinentMonster:
			OnMonsterRemoved_ContinentMonster((ContinentMonsterInstance)monsterInst);
			break;
		case MonsterInstanceType.TreatOfFarmQuestMonster:
			OnMonsterRemoved_TreatOfFarmQuestMonster((TreatOfFarmQuestMonsterInstance)monsterInst);
			break;
		case MonsterInstanceType.GuildMissionQuestMonster:
			OnMonsterRemoved_GuildMissionMonster((GuildMissionMonsterInstance)monsterInst);
			break;
		case MonsterInstanceType.CreatureFarmQuestMissionMonster:
			OnMonsterRemoved_CreatureFarmQuestMissionMonster((CreatureFarmQuestMissionMonsterInstance)monsterInst);
			break;
		}
	}

	protected override void OnMonsterDead(MonsterInstance monsterInst)
	{
		base.OnMonsterDead(monsterInst);
	}

	private void OnMonsterRemoved_ContinentMonster(ContinentMonsterInstance monsterInst)
	{
		DateTimeOffset monsterDeadTime = monsterInst.lastDamageTime;
		ContinentMonsterArrange arrange = monsterInst.arrange;
		int nRegenDelayTime = arrange.regenTime * 1000;
		MonsterRegenTimerState state = new MonsterRegenTimerState();
		state.arrange = arrange;
		state.regenTime = monsterDeadTime.AddSeconds(arrange.regenTime);
		state.timer = new Timer(OnMonsterRegenTimerTick, state, -1, -1);
		state.timer.Change(nRegenDelayTime, -1);
		m_monsterRegenTimerStates.Add(state);
	}

	private void OnMonsterRemoved_TreatOfFarmQuestMonster(TreatOfFarmQuestMonsterInstance monsterInst)
	{
	}

	private void OnMonsterRemoved_GuildMissionMonster(GuildMissionMonsterInstance monsterInst)
	{
	}

	private void OnMonsterRemoved_CreatureFarmQuestMissionMonster(CreatureFarmQuestMissionMonsterInstance monsterInst)
	{
	}

	private void OnMonsterRegenTimerTick(object state)
	{
		AddWork(new SFAction<MonsterRegenTimerState>(RegenMonster, (MonsterRegenTimerState)state), bGlobalLockRequired: false);
	}

	private void RegenMonster(MonsterRegenTimerState state)
	{
		Timer timer = state.timer;
		timer.Dispose();
		m_monsterRegenTimerStates.Remove(state);
		ContinentMonsterInstance monsterInst = CreateMonster(state.arrange);
		SpawnMonster(monsterInst, DateTimeUtil.currentTime);
	}

	protected override void OnHeroEntering(Hero hero, DateTimeOffset time)
	{
		base.OnHeroEntering(hero, time);
		NationWarInstance nationWarInst = hero.allianceNationWarInst;
		if (!m_continent.isNationWarTarget || (nationWarInst != null && nationWarInst.defenseNation.id != nationId))
		{
			hero.ClearAllianceNationWar();
		}
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		hero.CancelTrueHeroQuestStepWaiting(isLogOut);
	}

	private void InitObject(ContinentObjectArrange arrange)
	{
		AddObject(CreateObject(arrange));
	}

	private void AddObject(ContinentObjectInstance objectInst)
	{
		if (objectInst == null)
		{
			throw new ArgumentNullException("objectInst");
		}
		m_objectInsts.Add(objectInst.instanceId, objectInst);
	}

	private ContinentObjectInstance CreateObject(ContinentObjectArrange arrange)
	{
		ContinentObjectInstance objectInst = new ContinentObjectInstance();
		objectInst.Init(this, arrange);
		return objectInst;
	}

	public ContinentObjectInstance GetObject(long lnInstanceId)
	{
		if (!m_objectInsts.TryGetValue(lnInstanceId, out var value))
		{
			return null;
		}
		return value;
	}

	public void OnContinentObjectInteractionFinished(ContinentObjectInstance continentObjectInst)
	{
		if (continentObjectInst != null && continentObjectInst.isPublic)
		{
			RemoveObject(continentObjectInst);
			ContinentObject continentObject = continentObjectInst.obj;
			int nRegenDelayTime = continentObject.regenTime * 1000;
			if (nRegenDelayTime < 0)
			{
				nRegenDelayTime = 0;
			}
			ContinentObjectRegenTimerState objectRegenTimerState = new ContinentObjectRegenTimerState();
			objectRegenTimerState.arrange = continentObjectInst.arrange;
			objectRegenTimerState.timer = new Timer(OnContinentObjectRegenTimerTick, objectRegenTimerState, -1, -1);
			objectRegenTimerState.timer.Change(nRegenDelayTime, -1);
			m_objectRegenTimerStates.Add(objectRegenTimerState);
		}
	}

	public void OnContinentObjectRegenTimerTick(object state)
	{
		AddWork(new SFAction<ContinentObjectRegenTimerState>(RegenObject, (ContinentObjectRegenTimerState)state), bGlobalLockRequired: false);
	}

	public void RegenObject(ContinentObjectRegenTimerState state)
	{
		Timer timer = state.timer;
		timer.Dispose();
		m_objectRegenTimerStates.Remove(state);
		ContinentObjectInstance objectInst = CreateObject(state.arrange);
		AddObject(objectInst);
		ServerEvent.SendContinentObjectCreated(GetInterestClientPeers(objectInst.sector, Guid.Empty), objectInst.instanceId, objectInst.arrange.no);
	}

	private void RemoveObject(ContinentObjectInstance objectInst)
	{
		if (objectInst == null)
		{
			throw new ArgumentNullException("objectInst");
		}
		if (m_objectInsts.Remove(objectInst.instanceId))
		{
			objectInst.RemoveFromSector();
			objectInst.Release();
		}
	}

	protected override void ReleaseInternal()
	{
		base.ReleaseInternal();
		m_objectInsts.Clear();
		foreach (MonsterRegenTimerState state2 in m_monsterRegenTimerStates)
		{
			state2.timer.Dispose();
		}
		m_monsterRegenTimerStates.Clear();
		foreach (ContinentObjectRegenTimerState state in m_objectRegenTimerStates)
		{
			state.timer.Dispose();
		}
		m_objectRegenTimerStates.Clear();
	}

	public void EnterCart(CartInstance cartInst, DateTimeOffset time, bool bSendEvent)
	{
		if (cartInst == null)
		{
			throw new ArgumentException("cartInst");
		}
		cartInst.EndMove();
		if (!Contains(cartInst.position))
		{
			throw new Exception("카트의 위치가 유효하지 않습니다.");
		}
		m_cartInsts.Add(cartInst.instanceId, cartInst);
		cartInst.AddToSector(GetSectorOfPosition(cartInst.position));
		cartInst.SetCurrentPlace(this);
		if (bSendEvent)
		{
			ServerEvent.SendCartEnter(GetDynamicClientPeers(cartInst.sector, cartInst.owner.id), cartInst.ToPDCartInstance(time));
		}
	}

	public void ExitCart(CartInstance cartInst, bool bSendEvent, bool bResetPlaceReferenceOfCartInst)
	{
		if (cartInst == null)
		{
			throw new ArgumentNullException("cartInst");
		}
		if (m_cartInsts.Remove(cartInst.instanceId))
		{
			cartInst.EndMove();
			Sector sector = cartInst.sector;
			cartInst.RemoveFromSector(bResetPlaceReferenceOfCartInst);
			if (bResetPlaceReferenceOfCartInst)
			{
				cartInst.SetCurrentPlace(null);
			}
			if (bSendEvent)
			{
				ServerEvent.SendCartExit(GetDynamicClientPeers(sector, cartInst.owner.id), cartInst.instanceId);
			}
		}
	}

	public CartInstance GetCartInstance(long lnInstanceId)
	{
		if (!m_cartInsts.TryGetValue(lnInstanceId, out var value))
		{
			return null;
		}
		return value;
	}

	protected SectorChangeInfo ChangeCartPositionAndRotation(CartInstance cartInst, Vector3 position, float fRotationY, DateTimeOffset currentTime)
	{
		Sector oldSector = cartInst.sector;
		cartInst.SetPositionAndRotation(position, fRotationY);
		cartInst.ChangeSector(GetSectorOfPosition(position));
		Sector newSector = cartInst.sector;
		SectorChangeInfo info = GetSectorChangeInfo(oldSector, newSector);
		OnCartPositionChanged(cartInst, info, currentTime);
		return info;
	}

	protected virtual void OnCartPositionChanged(CartInstance cartInst, SectorChangeInfo info, DateTimeOffset currentTime)
	{
		HashSet<Sector> addedSectors = info.addedSectors;
		HashSet<Sector> removedSectors = info.removedSectors;
		Hero owner = cartInst.owner;
		if (addedSectors.Count > 0)
		{
			ServerEvent.SendCartInterestAreaEnter(Sector.GetClientPeers(addedSectors, owner.id), cartInst.ToPDCartInstance(currentTime));
		}
		if (removedSectors.Count > 0)
		{
			ServerEvent.SendCartInterestAreaExit(Sector.GetClientPeers(removedSectors, owner.id), cartInst.instanceId);
		}
		owner.SetPositionAndRotation(cartInst.position, cartInst.rotationY);
		owner.ChangeSector(GetSectorOfPosition(cartInst.position));
		OnHeroPositionChanged(owner, info, bSendInterestTargetChangeEvent: true, currentTime);
	}

	public void MoveCart(CartInstance cartInst, Vector3 position, float fRotationY, DateTimeOffset currentTime)
	{
		Vector3 previousPosition = cartInst.position;
		SectorChangeInfo info = ChangeCartPositionAndRotation(cartInst, position, fRotationY, currentTime);
		List<ClientPeer> peers = new List<ClientPeer>();
		Sector.GetClientPeers(info.notChangedSectors, cartInst.owner.id, peers);
		if (peers.Count > 0)
		{
			ServerEvent.SendCartMove(peers, cartInst.instanceId, cartInst.position, cartInst.rotationY);
		}
		cartInst.OnMove(previousPosition);
	}

	protected override void OnHeroDead(Hero hero)
	{
		base.OnHeroDead(hero);
		OnHeroDead_PvP(hero);
		OnHeroDead_DeadRecord(hero);
	}

	private void OnHeroDead_PvP(Hero hero)
	{
		Guid lastAttackerId = ((hero.lastAttacker is Hero lastAttacker) ? lastAttacker.id : Guid.Empty);
		DateTimeOffset deadTime = hero.lastDamageTime;
		_ = deadTime.Date;
		foreach (Guid attackerId in hero.receivedDamages.Keys)
		{
			Hero attacker = GetHero(attackerId);
			if (attacker == null)
			{
				continue;
			}
			lock (attacker.syncObject)
			{
				int nLevelGap = hero.level - attacker.level;
				if (attacker.id == lastAttackerId)
				{
					attacker.IncreasePvpKillCount(nLevelGap, deadTime);
				}
				else
				{
					attacker.IncreasePvpAssistCount(nLevelGap, deadTime);
				}
				attacker.ProcessHolyWarQuest(deadTime);
			}
		}
	}

	private void OnHeroDead_DeadRecord(Hero hero)
	{
		if (!(hero.lastAttacker is Hero killer))
		{
			return;
		}
		lock (killer.syncObject)
		{
			hero.AddDeadRecord(killer, DateTimeUtil.currentTime);
		}
	}

	public abstract bool IsSame(int nTargetContinentId, int nTargetNationId);

	public override void OnUnitDead(Unit unit)
	{
		base.OnUnitDead(unit);
		UnitType type = unit.type;
		if (type == UnitType.CartInstance)
		{
			OnCartDead((CartInstance)unit);
		}
	}

	protected virtual void OnCartDead(CartInstance cartInst)
	{
		ExitCart(cartInst, bSendEvent: false, bResetPlaceReferenceOfCartInst: false);
		Cache.instance.RemoveCartInstance(cartInst);
	}

	public void SpawnFieldBoss(FieldBoss fieldBoss, FieldBossEventSchedule schedule, DateTimeOffset time)
	{
		if (fieldBoss == null)
		{
			throw new ArgumentNullException("fieldBoss");
		}
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		if (GetFieldBoss(fieldBoss.id) == null)
		{
			FieldBossMonsterInstance monsterInst = new FieldBossMonsterInstance();
			monsterInst.Init(this, fieldBoss, schedule);
			SpawnMonster(monsterInst, time);
			AddFieldBoss(monsterInst);
		}
	}

	private void AddFieldBoss(FieldBossMonsterInstance fieldBossMonsterInst)
	{
		m_fieldBossMonsterInsts.Add(fieldBossMonsterInst.fieldBoss.id, fieldBossMonsterInst);
	}

	public FieldBossMonsterInstance GetFieldBoss(int nFieldBossId)
	{
		if (!m_fieldBossMonsterInsts.TryGetValue(nFieldBossId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveFieldBoss(int nFieldBossId)
	{
		m_fieldBossMonsterInsts.Remove(nFieldBossId);
	}

	public void ClearFieldBoss()
	{
		foreach (FieldBossMonsterInstance monsterInst in m_fieldBossMonsterInsts.Values)
		{
			if (!monsterInst.isDead)
			{
				RemoveMonster(monsterInst, bSendEvent: true);
			}
		}
		m_fieldBossMonsterInsts.Clear();
	}
}
