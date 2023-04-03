using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public abstract class Place
{
	public const int kUpdateInterval = 500;

	protected Guid m_instanceId = Guid.Empty;

	protected bool m_bClosed;

	protected bool m_bReleased;

	private SFDynamicWorker m_worker = new SFDynamicWorker();

	protected Sector[,] m_sectors;

	protected Dictionary<Guid, Hero> m_heroes = new Dictionary<Guid, Hero>();

	protected Dictionary<long, MonsterInstance> m_monsterInsts = new Dictionary<long, MonsterInstance>();

	private Timer m_updateTimer;

	protected DateTimeOffset m_prevUpdateTime = DateTimeOffset.MinValue;

	protected DateTimeOffset m_currentUpdateTime = DateTimeOffset.MinValue;

	protected HashSet<SkillEffect> m_skillEffects = new HashSet<SkillEffect>();

	protected Dictionary<Guid, PlaceAlliance> m_alliances = new Dictionary<Guid, PlaceAlliance>();

	public Guid instanceId => m_instanceId;

	public abstract PlaceType placeType { get; }

	public abstract Location location { get; }

	public abstract int locationParam { get; }

	public object syncObject => this;

	public abstract Rect3D mapRect { get; }

	public abstract bool interestManaged { get; }

	public abstract bool ownershipManaged { get; }

	public abstract bool battleEnabled { get; }

	public abstract bool pvpEnabled { get; }

	public abstract bool distortionScrollUseEnabled { get; }

	public bool mountRidingEnabled => location.mountRidingEnabled;

	public bool hpPotionUseEnabled => location.hpPotionUseEnabled;

	public bool returnScrollUseEnabled => location.returnScrollUseEnabled;

	public bool evasionCastEnabled => location.evasionCastEnabled;

	public abstract bool isPartyExpBuffEnabled { get; }

	public abstract bool isExpScrollBuffEnabled { get; }

	public abstract bool isExpLevelPenaltyEnabled { get; }

	public abstract bool isWorldLevelExpBuffEnabled { get; }

	public Dictionary<Guid, Hero> heroes => m_heroes;

	public Dictionary<long, MonsterInstance> monsterInsts => m_monsterInsts;

	public Place()
	{
		m_instanceId = Guid.NewGuid();
	}

	protected virtual void InitPlace()
	{
		Rect3D mapRect = this.mapRect;
		int nSectorSize = Resource.instance.sectorSize;
		int nRowCount = (int)Math.Floor(mapRect.sizeZ / (float)nSectorSize) + 1;
		int nColumnCount = (int)Math.Floor(mapRect.sizeX / (float)nSectorSize) + 1;
		m_sectors = new Sector[nRowCount, nColumnCount];
		for (int nRow = 0; nRow < nRowCount; nRow++)
		{
			for (int nColumn = 0; nColumn < nColumnCount; nColumn++)
			{
				m_sectors[nRow, nColumn] = new Sector(this, nRow, nColumn);
			}
		}
		m_worker.isAsyncErrorLogging = true;
		m_worker.Start();
		m_prevUpdateTime = DateTimeUtil.currentTime;
		m_currentUpdateTime = m_prevUpdateTime;
		m_updateTimer = new Timer(OnUpdateTimerTick);
		m_updateTimer.Change(500, 500);
		InitAlliances();
	}

	public Sector GetSector(int nRow, int nColumn)
	{
		if (nRow < 0 || nRow >= m_sectors.GetLength(0) || nColumn < 0 || nColumn >= m_sectors.GetLength(1))
		{
			return null;
		}
		return m_sectors[nRow, nColumn];
	}

	public Sector GetSector(MatrixPoint pt)
	{
		return GetSector(pt.row, pt.column);
	}

	public Sector GetSectorOfPosition(Vector3 position)
	{
		return GetSector(GetSectorPoint(position));
	}

	public void GetInterestSectors(int nTargetRow, int nTargetColumn, ICollection<Sector> buffer)
	{
		Sector sector = null;
		for (int nRow = nTargetRow - 1; nRow <= nTargetRow + 1; nRow++)
		{
			for (int nColumn = nTargetColumn - 1; nColumn <= nTargetColumn + 1; nColumn++)
			{
				sector = GetSector(nRow, nColumn);
				if (sector != null)
				{
					buffer.Add(sector);
				}
			}
		}
	}

	public List<Sector> GetInterestSectors(int nTargetRow, int nTargetColumn)
	{
		List<Sector> sectors = new List<Sector>();
		GetInterestSectors(nTargetRow, nTargetColumn, sectors);
		return sectors;
	}

	public void GetInterestSectors(Sector target, ICollection<Sector> buffer)
	{
		GetInterestSectors(target.row, target.column, buffer);
	}

	public List<Sector> GetInterestSectors(Sector target)
	{
		return GetInterestSectors(target.row, target.column);
	}

	public List<Sector> GetInterestSectorsOfPosition(Vector3 position)
	{
		MatrixPoint pt = GetSectorPoint(position);
		return GetInterestSectors(pt.row, pt.column);
	}

	public MatrixPoint GetSectorPoint(Vector3 position)
	{
		int nSectorSize = Resource.instance.sectorSize;
		Rect3D mapRect = this.mapRect;
		MatrixPoint pt = default(MatrixPoint);
		pt.row = (int)Math.Floor((position.z - mapRect.z) / (float)nSectorSize);
		pt.column = (int)Math.Floor((position.x - mapRect.x) / (float)nSectorSize);
		return pt;
	}

	protected SectorChangeInfo GetSectorChangeInfo(Sector oldSector, Sector newSector)
	{
		HashSet<Sector> addedSectors = new HashSet<Sector>();
		HashSet<Sector> removedSectors = new HashSet<Sector>();
		HashSet<Sector> notChangedSectors = new HashSet<Sector>();
		if (oldSector == newSector)
		{
			GetInterestSectors(oldSector, notChangedSectors);
		}
		else
		{
			GetInterestSectors(newSector, addedSectors);
			GetInterestSectors(oldSector, removedSectors);
			Sector[] array = addedSectors.ToArray();
			foreach (Sector sector in array)
			{
				if (removedSectors.Remove(sector))
				{
					addedSectors.Remove(sector);
					notChangedSectors.Add(sector);
				}
			}
		}
		return new SectorChangeInfo(addedSectors, removedSectors, notChangedSectors);
	}

	public bool Contains(float fX, float fY, float fZ)
	{
		return mapRect.Contains(fX, fY, fZ);
	}

	public bool Contains(Vector3 position)
	{
		return Contains(position.x, position.y, position.z);
	}

	public void AddWork(ISFRunnable work, bool bGlobalLockRequired)
	{
		m_worker.EnqueueWork(new SFAction<ISFRunnable, bool>(RunWork, work, bGlobalLockRequired));
	}

	private void RunWork(ISFRunnable work, bool bGlobalLockRequired)
	{
		if (bGlobalLockRequired)
		{
			lock (Global.syncObject)
			{
				RunWorkInternal(work);
				return;
			}
		}
		RunWorkInternal(work);
	}

	private void RunWorkInternal(ISFRunnable work)
	{
		lock (syncObject)
		{
			if (!m_bReleased)
			{
				work.Run();
			}
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
		AddWork(new SFAction(OnUpdate), bGlobalLockRequired: false);
	}

	private void OnUpdate()
	{
		m_prevUpdateTime = m_currentUpdateTime;
		m_currentUpdateTime = DateTimeUtil.currentTime;
		OnUpdateInternal();
	}

	protected virtual void OnUpdateInternal()
	{
		MonsterInstance[] array = m_monsterInsts.Values.ToArray();
		foreach (MonsterInstance monsterInst in array)
		{
			monsterInst.OnUpdate();
		}
	}

	public virtual void OnUnitDead(Unit unit)
	{
		switch (unit.type)
		{
		case UnitType.Hero:
			OnHeroDead((Hero)unit);
			break;
		case UnitType.MonsterInstance:
		{
			MonsterInstance monsterInst = (MonsterInstance)unit;
			OnMonsterDead(monsterInst);
			RemoveMonster(monsterInst, bSendEvent: false);
			break;
		}
		}
	}

	public virtual bool OnUnitDamage(Unit unit)
	{
		return false;
	}

	public void Close()
	{
		if (!m_bClosed)
		{
			m_bClosed = true;
			Release();
			Cache.instance.RemovePlace(m_instanceId);
		}
	}

	public void Release()
	{
		if (!m_bReleased)
		{
			ReleaseInternal();
			m_bReleased = true;
		}
	}

	protected virtual void ReleaseInternal()
	{
		DisposeUpdateTimer();
		m_worker.Stop(bClearQueue: true);
		foreach (MonsterInstance monsterInst in m_monsterInsts.Values)
		{
			monsterInst.Release();
		}
		m_monsterInsts.Clear();
		foreach (SkillEffect effect in m_skillEffects)
		{
			effect.Stop();
		}
		m_skillEffects.Clear();
	}

	public void AddSkillEffect(SkillEffect effect)
	{
		if (effect == null)
		{
			throw new ArgumentNullException("effect");
		}
		m_skillEffects.Add(effect);
	}

	protected void RemoveSkillEffect(SkillEffect effect)
	{
		m_skillEffects.Remove(effect);
	}

	private void InitAlliances()
	{
		foreach (NationAlliance nationAlliance in Cache.instance.nationAlliances.Values)
		{
			AddAlliance(nationAlliance.ToPlaceAlliance());
		}
	}

	public void AddAlliance(PlaceAlliance alliance)
	{
		if (alliance == null)
		{
			throw new ArgumentNullException("alliance");
		}
		m_alliances.Add(alliance.id, alliance);
	}

	public void RemoveAlliance(Guid allianceId)
	{
		m_alliances.Remove(allianceId);
	}

	public bool IsAlliance(int nNationId1, int nNationId2)
	{
		foreach (PlaceAlliance alliance in m_alliances.Values)
		{
			if (alliance.IsAlliance(nNationId1, nNationId2))
			{
				return true;
			}
		}
		return false;
	}

	public Hero GetHero(Guid heroId)
	{
		if (!m_heroes.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	protected virtual void OnHeroDead(Hero hero)
	{
	}

	protected virtual void OnHeroMove(Hero hero)
	{
	}

	public List<PDHero> GetPDHeroes(Guid heroIdToExclude, DateTimeOffset currentTime)
	{
		List<PDHero> results = new List<PDHero>();
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.id != heroIdToExclude)
			{
				lock (hero.syncObject)
				{
					results.Add(hero.ToPDHero(currentTime));
				}
			}
		}
		return results;
	}

	public List<PDHero> GetPDHeroes(DateTimeOffset currentTime)
	{
		return GetPDHeroes(Guid.Empty, currentTime);
	}

	public void Enter(Hero hero, DateTimeOffset time, bool bIsRevivalEnter)
	{
		if (hero == null)
		{
			throw new ArgumentException("hero");
		}
		OnHeroEntering(hero, time);
		if (!Contains(hero.position))
		{
			throw new Exception("영웅의 위치가 유효하지 않습니다.");
		}
		m_heroes.Add(hero.id, hero);
		hero.AddToSector(GetSectorOfPosition(hero.position));
		hero.SetCurrentPlace(this);
		OnHeroAdded(hero, time);
		if (!hero.isRidingCart)
		{
			ServerEvent.SendHeroEnter(GetDynamicClientPeers(hero.sector, hero.id), hero.ToPDHero(time), bIsRevivalEnter);
		}
		OnHeroEnter(hero, time);
	}

	protected virtual void OnHeroEntering(Hero hero, DateTimeOffset time)
	{
		hero.EndMove();
		if (!mountRidingEnabled)
		{
			hero.GetOffMount(bSendEventToMyself: true);
		}
		if (location.locationType != 0)
		{
			hero.CancelMatching(6);
			hero.ClearAllianceNationWar();
		}
	}

	protected virtual void OnHeroAdded(Hero hero, DateTimeOffset time)
	{
	}

	protected virtual void OnHeroEnter(Hero hero, DateTimeOffset time)
	{
		if (!distortionScrollUseEnabled)
		{
			hero.CancelDistortion(bSendEventToMyself: true);
		}
	}

	public void Exit(Hero hero, bool isLogOut, PlaceEntranceParam placeEntranceParam)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		if (m_heroes.Remove(hero.id))
		{
			if (!isLogOut)
			{
				hero.BackupLocationInfo();
			}
			if (!isLogOut)
			{
				hero.CancelAllExclusiveActions();
			}
			Sector sector = hero.sector;
			hero.RemoveFromSector(bResetReference: true);
			hero.SetCurrentPlace(null);
			hero.placeEntranceParam = placeEntranceParam;
			OnHeroRemoved(hero);
			if (!hero.isRidingCart)
			{
				ServerEvent.SendHeroExit(GetDynamicClientPeers(sector, hero.id), hero.id);
			}
			OnHeroExit(hero, isLogOut);
		}
	}

	protected virtual void OnHeroRemoved(Hero hero)
	{
	}

	protected virtual void OnHeroExit(Hero hero, bool isLogOut)
	{
		foreach (MonsterInstance monsterInst2 in hero.ownMonsterInsts)
		{
			monsterInst2.RemoveOwnership(bRemoveFromHero: false);
		}
		hero.ClearOwnMonsters();
		foreach (MonsterInstance monsterInst in hero.aggroMonsterInsts)
		{
			monsterInst.RemoveAggro(hero.id);
		}
		hero.ClearAggroMonsters();
		hero.CancelMainQuestTransformationMonsterEffect(bResetHP: true, bSendEventMyself: true, bSendEventToOthers: false, new List<long>());
		hero.EndAutoHunt();
	}

	protected virtual void OnHeroPositionChanged(Hero hero, SectorChangeInfo info, bool bSendInterestTargetChangeEvent, DateTimeOffset currentTime)
	{
		HashSet<Sector> addedSectors = info.addedSectors;
		HashSet<Sector> removedSectors = info.removedSectors;
		HashSet<Sector> notChangedSectors = info.notChangedSectors;
		if (interestManaged)
		{
			if ((addedSectors.Count > 0 || removedSectors.Count > 0) && bSendInterestTargetChangeEvent)
			{
				ServerEvent.SendInterestTargetChange(hero.account.peer, Sector.GetPDHeroes(addedSectors, hero.id, currentTime).ToArray(), Sector.GetPDMonsterInstances<PDMonsterInstance>(addedSectors, currentTime).ToArray(), Sector.GetPDContinentObjectInstances(addedSectors).ToArray(), Sector.GetPDCartInstances(addedSectors, 0L, currentTime).ToArray(), Sector.GetHeroIds(removedSectors, hero.id).ToArray(), Sector.GetMonsterInstanceIds(removedSectors).ToArray(), Sector.GetContinentObjectInstanceIds(removedSectors).ToArray(), Sector.GetCartInstanceIds(removedSectors, 0L).ToArray());
			}
			if (addedSectors.Count > 0 && !hero.isRidingCart)
			{
				ServerEvent.SendHeroInterestAreaEnter(Sector.GetClientPeers(addedSectors, hero.id), hero.ToPDHero(currentTime));
			}
			if (removedSectors.Count > 0 && !hero.isRidingCart)
			{
				ServerEvent.SendHeroInterestAreaExit(Sector.GetClientPeers(removedSectors, hero.id), hero.id);
			}
			foreach (Sector sector4 in removedSectors)
			{
				foreach (MonsterInstance monsterInst4 in sector4.monsterInsts.Values)
				{
					if (monsterInst4.ownerId == hero.id)
					{
						monsterInst4.RemoveOwnership(bRemoveFromHero: true);
					}
				}
			}
		}
		foreach (Sector sector3 in removedSectors)
		{
			foreach (MonsterInstance monsterInst3 in sector3.monsterInsts.Values)
			{
				if (monsterInst3.RemoveAggro(hero.id))
				{
					hero.RemoveAggroMonster(monsterInst3);
				}
			}
		}
		foreach (Sector sector2 in notChangedSectors)
		{
			foreach (MonsterInstance monsterInst2 in sector2.monsterInsts.Values)
			{
				if (!(monsterInst2.visibilityRange <= 0f) && MathUtil.CircleContains(monsterInst2.position, monsterInst2.visibilityRange, hero.position) && !monsterInst2.ContainsAggro(hero.id))
				{
					monsterInst2.AddAggro(hero, 1L);
				}
			}
		}
		foreach (Sector sector in addedSectors)
		{
			foreach (MonsterInstance monsterInst in sector.monsterInsts.Values)
			{
				if (!(monsterInst.visibilityRange <= 0f) && MathUtil.CircleContains(monsterInst.position, monsterInst.visibilityRange, hero.position) && !monsterInst.ContainsAggro(hero.id))
				{
					monsterInst.AddAggro(hero, 1L);
				}
			}
		}
	}

	public void GetClientPeers(Guid heroIdToExclude, ICollection<ClientPeer> buffer)
	{
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.isReal && hero.id != heroIdToExclude)
			{
				buffer.Add(hero.account.peer);
			}
		}
	}

	public List<ClientPeer> GetClientPeers(Guid heroIdToExclude)
	{
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		GetClientPeers(heroIdToExclude, clientPeers);
		return clientPeers;
	}

	public List<ClientPeer> GetClientPeers()
	{
		return GetClientPeers(Guid.Empty);
	}

	public void GetInterestClientPeers(int nTargetSectorRow, int nTargetSectorColumn, Guid heroIdToExclude, ICollection<ClientPeer> buffer)
	{
		Sector sector = null;
		for (int nRow = nTargetSectorRow - 1; nRow <= nTargetSectorRow + 1; nRow++)
		{
			for (int nColumn = nTargetSectorColumn - 1; nColumn <= nTargetSectorColumn + 1; nColumn++)
			{
				GetSector(nRow, nColumn)?.GetClientPeers(buffer, heroIdToExclude);
			}
		}
	}

	public List<ClientPeer> GetInterestClientPeers(int nTargetSectorRow, int nTargetSectorColumn, Guid heroIdToExclude)
	{
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		GetInterestClientPeers(nTargetSectorRow, nTargetSectorColumn, heroIdToExclude, clientPeers);
		return clientPeers;
	}

	public void GetInterestClientPeers(Sector targetSector, Guid heroIdToExclude, ICollection<ClientPeer> buffer)
	{
		GetInterestClientPeers(targetSector.row, targetSector.column, heroIdToExclude, buffer);
	}

	public List<ClientPeer> GetInterestClientPeers(Sector targetSector, Guid heroIdToExclude)
	{
		return GetInterestClientPeers(targetSector.row, targetSector.column, heroIdToExclude);
	}

	public void GetDynamicClientPeers(Sector targetSector, Guid heroIdToExclude, ICollection<ClientPeer> buffer)
	{
		if (interestManaged)
		{
			GetInterestClientPeers(targetSector, heroIdToExclude, buffer);
		}
		else
		{
			GetClientPeers(heroIdToExclude, buffer);
		}
	}

	public List<ClientPeer> GetDynamicClientPeers(Sector targetSector, Guid heroIdToExclude)
	{
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		GetDynamicClientPeers(targetSector, heroIdToExclude, clientPeers);
		return clientPeers;
	}

	public void GetInterestClientPeers(int nTargetSectorRow1, int nTargetSectorColumn1, int nTargetSectorRow2, int nTargetSectorColumn2, Guid heroIdToExclude, ICollection<ClientPeer> buffer)
	{
		HashSet<Sector> sectors = new HashSet<Sector>();
		GetInterestSectors(nTargetSectorRow1, nTargetSectorColumn1, sectors);
		GetInterestSectors(nTargetSectorRow2, nTargetSectorColumn2, sectors);
		Sector.GetClientPeers(sectors, heroIdToExclude, buffer);
	}

	public List<ClientPeer> GetInterestClientPeers(int nTargetSectorRow1, int nTargetSectorColumn1, int nTargetSectorRow2, int nTargetSectorColumn2, Guid heroIdToExclude)
	{
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		GetInterestClientPeers(nTargetSectorRow1, nTargetSectorColumn1, nTargetSectorRow2, nTargetSectorColumn2, heroIdToExclude, clientPeers);
		return clientPeers;
	}

	public void GetInterestClientPeers(Sector targetSector1, Sector targetSector2, Guid heroIdToExclude, ICollection<ClientPeer> buffer)
	{
		GetInterestClientPeers(targetSector1.row, targetSector1.column, targetSector2.row, targetSector2.column, heroIdToExclude, buffer);
	}

	public List<ClientPeer> GetInterestClientPeers(Sector targetSector1, Sector targetSector2, Guid heroIdToExclude)
	{
		return GetInterestClientPeers(targetSector1.row, targetSector1.column, targetSector2.row, targetSector2.column, heroIdToExclude);
	}

	public void GetDynamicClientPeers(Sector targetSector1, Sector targetSector2, Guid heroIdToExclude, ICollection<ClientPeer> buffer)
	{
		if (interestManaged)
		{
			GetInterestClientPeers(targetSector1, targetSector2, heroIdToExclude, buffer);
		}
		else
		{
			GetClientPeers(heroIdToExclude, buffer);
		}
	}

	public List<ClientPeer> GetDynamicClientPeers(Sector targetSector1, Sector targetSector2, Guid heroIdToExclude)
	{
		List<ClientPeer> clientPeers = new List<ClientPeer>();
		GetDynamicClientPeers(targetSector1, targetSector2, heroIdToExclude, clientPeers);
		return clientPeers;
	}

	protected void AddMonster(MonsterInstance monsterInst)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		m_monsterInsts.Add(monsterInst.instanceId, monsterInst);
	}

	public virtual void RemoveMonster(MonsterInstance monsterInst, bool bSendEvent)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		if (m_monsterInsts.Remove(monsterInst.instanceId))
		{
			monsterInst.RemoveFromSector(bResetReference: false);
			OnMonsterRemoved(monsterInst);
			monsterInst.Release();
			if (bSendEvent)
			{
				ServerEvent.SendMonsterRemoved(GetDynamicClientPeers(monsterInst.sector, Guid.Empty), monsterInst.instanceId);
			}
		}
	}

	protected virtual void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		monsterInst.RemoveOwnership(bRemoveFromHero: true);
		foreach (Aggro aggro in monsterInst.aggroes.Values)
		{
			Hero target = aggro.target;
			lock (target.syncObject)
			{
				target.RemoveAggroMonster(monsterInst);
			}
		}
		monsterInst.ClearAggroes();
		monsterInst.targetTamer?.CancelGroggyMonsterItemSteal(bSendEventToMyself: true, bSendEventToOthers: true);
	}

	public void SpawnMonster(MonsterInstance monsterInst, DateTimeOffset currentTime)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		AddMonster(monsterInst);
		ServerEvent.SendMonsterSpawn(GetDynamicClientPeers(monsterInst.sector, Guid.Empty), monsterInst.ToPDMonsterInstance(currentTime));
	}

	public MonsterInstance GetMonster(long lnInstanceId)
	{
		if (!m_monsterInsts.TryGetValue(lnInstanceId, out var value))
		{
			return null;
		}
		return value;
	}

	protected virtual void OnMonsterDead(MonsterInstance monsterInst)
	{
	}

	protected virtual void OnMonsterPositionChanged(MonsterInstance monsterInst, SectorChangeInfo info, DateTimeOffset currentTime)
	{
		HashSet<Sector> addedSectors = info.addedSectors;
		HashSet<Sector> removedSectors = info.removedSectors;
		HashSet<Sector> notChangedSectors = info.notChangedSectors;
		if (interestManaged)
		{
			if (addedSectors.Count > 0)
			{
				ServerEvent.SendMonsterInterestAreaEnter(Sector.GetClientPeers(addedSectors, Guid.Empty), monsterInst.ToPDMonsterInstance(currentTime));
			}
			if (removedSectors.Count > 0)
			{
				ServerEvent.SendMonsterInterestAreaExit(Sector.GetClientPeers(removedSectors, Guid.Empty), monsterInst.instanceId);
			}
			Hero owner = monsterInst.owner;
			if (owner != null)
			{
				lock (owner.syncObject)
				{
					if (!monsterInst.IsInterestSector(owner.sector.row, owner.sector.column))
					{
						monsterInst.RemoveOwnership(bRemoveFromHero: true);
					}
				}
			}
		}
		foreach (Sector sector3 in removedSectors)
		{
			foreach (Hero hero3 in sector3.heroes.Values)
			{
				lock (hero3.syncObject)
				{
					if (monsterInst.RemoveAggro(hero3.id))
					{
						hero3.RemoveAggroMonster(monsterInst);
					}
				}
			}
		}
		if (!(monsterInst.visibilityRange > 0f))
		{
			return;
		}
		foreach (Sector sector2 in notChangedSectors)
		{
			foreach (Hero hero2 in sector2.heroes.Values)
			{
				if (MathUtil.CircleContains(monsterInst.position, monsterInst.visibilityRange, hero2.position) && !monsterInst.ContainsAggro(hero2.id))
				{
					monsterInst.AddAggro(hero2, 1L);
				}
			}
		}
		foreach (Sector sector in addedSectors)
		{
			foreach (Hero hero in sector.heroes.Values)
			{
				if (MathUtil.CircleContains(monsterInst.position, monsterInst.visibilityRange, hero.position) && !monsterInst.ContainsAggro(hero.id))
				{
					monsterInst.AddAggro(hero, 1L);
				}
			}
		}
	}

	public PDMonsterInstance GetPDMonsterInstance(DateTimeOffset currentTime)
	{
		return m_monsterInsts.Values.FirstOrDefault()?.ToPDMonsterInstance(currentTime);
	}

	public List<PDMonsterInstance> GetPDMonsterInstances(DateTimeOffset currentTime)
	{
		return GetPDMonsterInstances<PDMonsterInstance>(currentTime);
	}

	public List<T> GetPDMonsterInstances<T>(DateTimeOffset currentTime) where T : PDMonsterInstance
	{
		List<T> results = new List<T>();
		foreach (MonsterInstance monsterInst in m_monsterInsts.Values)
		{
			results.Add((T)monsterInst.ToPDMonsterInstance(currentTime));
		}
		return results;
	}

	public SectorChangeInfo ChangeHeroPositionAndRotation(Hero targetHero, Vector3 position, float fRotationY, bool bSendInterestTargetChangeEvent, DateTimeOffset currentTime)
	{
		Sector oldSector = targetHero.sector;
		targetHero.SetPositionAndRotation(position, fRotationY);
		targetHero.ChangeSector(GetSectorOfPosition(position));
		Sector newSector = targetHero.sector;
		SectorChangeInfo info = GetSectorChangeInfo(oldSector, newSector);
		OnHeroPositionChanged(targetHero, info, bSendInterestTargetChangeEvent, currentTime);
		return info;
	}

	public void MoveHero(Hero hero, Vector3 position, float fRotationY, DateTimeOffset currentTime, bool bSendEvent)
	{
		if (hero == null)
		{
			throw new ArgumentNullException("hero");
		}
		Vector3 previousPosition = hero.position;
		SectorChangeInfo info = ChangeHeroPositionAndRotation(hero, position, fRotationY, bSendInterestTargetChangeEvent: true, currentTime);
		if (bSendEvent)
		{
			IEnumerable<ClientPeer> peers = null;
			if (interestManaged)
			{
				HashSet<Sector> notChangedSectors = info.notChangedSectors;
				if (notChangedSectors.Count > 0)
				{
					peers = Sector.GetClientPeers(notChangedSectors, hero.id);
				}
			}
			else
			{
				peers = GetClientPeers(hero.id);
			}
			if (peers != null)
			{
				ServerEvent.SendHeroMove(peers, hero.id, hero.position, hero.rotationY);
			}
		}
		OnHeroMove(hero);
		hero.OnMove(previousPosition);
	}

	protected SectorChangeInfo ChangeMonsterPositionAndRotation(MonsterInstance monsterInst, Vector3 position, float fRotationY, DateTimeOffset currentTime)
	{
		Sector oldSector = monsterInst.sector;
		monsterInst.SetPositionAndRotation(position, fRotationY);
		monsterInst.ChangeSector(GetSectorOfPosition(position));
		Sector newSector = monsterInst.sector;
		SectorChangeInfo info = GetSectorChangeInfo(oldSector, newSector);
		OnMonsterPositionChanged(monsterInst, info, currentTime);
		return info;
	}

	public void MoveMonster(MonsterInstance monsterInst, Vector3 position, float fRotationY, DateTimeOffset currentTime)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		SectorChangeInfo info = ChangeMonsterPositionAndRotation(monsterInst, position, fRotationY, currentTime);
		IEnumerable<ClientPeer> peers = null;
		if (interestManaged)
		{
			HashSet<Sector> notChangedSectors = info.notChangedSectors;
			if (notChangedSectors.Count > 0)
			{
				peers = Sector.GetClientPeers(notChangedSectors, monsterInst.ownerId);
			}
		}
		else
		{
			peers = GetClientPeers(monsterInst.ownerId);
		}
		if (peers != null)
		{
			ServerEvent.SendMonsterMove(peers, monsterInst.instanceId, monsterInst.position, monsterInst.rotationY);
		}
	}

	public virtual bool ProcessHeroSkillHit_Hero(OffenseHit offenseHit, int nAttackerLevel, Hero target)
	{
		if (offenseHit == null)
		{
			throw new ArgumentNullException("offenseHit");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		Hero attacker = (Hero)offenseHit.offense.attacker;
		if (!pvpEnabled)
		{
			return false;
		}
		if (target.id == attacker.id)
		{
			return false;
		}
		int nAttackerNationId = attacker.nationId;
		int nTargetNationId = target.nationId;
		if (nTargetNationId == nAttackerNationId)
		{
			return false;
		}
		if (IsAlliance(nTargetNationId, nAttackerNationId))
		{
			return false;
		}
		int nPvpMinHeroLevel = Resource.instance.pvpMinHeroLevel;
		if (nAttackerLevel < nPvpMinHeroLevel)
		{
			return false;
		}
		if (target.level < nPvpMinHeroLevel)
		{
			return false;
		}
		if (target.isRidingCart)
		{
			return false;
		}
		if (target.isSafeMode)
		{
			return false;
		}
		if (target.isDistorting)
		{
			return false;
		}
		return target.Hit(offenseHit);
	}

	public bool ProcessHeroSkillHit_Monster(OffenseHit offenseHit, MonsterInstance target)
	{
		if (offenseHit == null)
		{
			throw new ArgumentNullException("offenseHit");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		Hero attacker = (Hero)offenseHit.offense.attacker;
		if (target.isExclusive && target.exclusiveHeroId != attacker.id)
		{
			return false;
		}
		int nAttackerNationId = attacker.nationId;
		int nTargetNationId = target.nationId;
		if (nTargetNationId == nAttackerNationId)
		{
			return false;
		}
		if (IsAlliance(nTargetNationId, nAttackerNationId))
		{
			return false;
		}
		return target.Hit(offenseHit);
	}

	public bool ProcessHeroSkillHit_Cart(OffenseHit offenseHit, CartInstance target)
	{
		if (offenseHit == null)
		{
			throw new ArgumentNullException("offenseHit");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		Hero attacker = (Hero)offenseHit.offense.attacker;
		int nAttackerNationId = attacker.nationId;
		int nTargetOwnerNationId = target.owner.nationId;
		if (nTargetOwnerNationId == nAttackerNationId)
		{
			return false;
		}
		if (IsAlliance(nTargetOwnerNationId, nAttackerNationId))
		{
			return false;
		}
		return target.Hit(offenseHit);
	}

	public virtual bool IsHeroRankActiveSkillCast_OtherHero(Hero source, Hero target)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (!pvpEnabled)
		{
			return false;
		}
		if (source.id == target.id)
		{
			return false;
		}
		if (source.isDistorting || target.isDistorting)
		{
			return false;
		}
		int nSourceNationId = source.nationId;
		int nTargetNationId = target.nationId;
		if (nTargetNationId == nSourceNationId)
		{
			return false;
		}
		if (IsAlliance(nTargetNationId, nSourceNationId))
		{
			return false;
		}
		int nPvpMinHeroLevel = Resource.instance.pvpMinHeroLevel;
		if (source.level < nPvpMinHeroLevel || target.level < nPvpMinHeroLevel)
		{
			return false;
		}
		return true;
	}

	public void ProcessMonsterSkillHit(MonsterInstanceSkill monsterInstanceSkill, Vector3 skillTargetPositioin)
	{
		MonsterInstance monsterInst = monsterInstanceSkill.monsterInst;
		MonsterSkill monsterSkill = monsterInstanceSkill.monsterSkill;
		Cache.instance.GetNationInstance(monsterInst.nationId);
		if (monsterSkill.skillType == 2)
		{
			SkillEffect effect = new SkillEffect();
			effect.Init(this, monsterInstanceSkill.MakeOffense(), skillTargetPositioin, Guid.Empty, monsterInst.level);
			AddSkillEffect(effect);
			return;
		}
		Offense offense = monsterInstanceSkill.MakeOffense();
		float fHitRange = monsterSkill.hitRange;
		foreach (MonsterSkillHit hit in monsterSkill.hits)
		{
			foreach (Sector sector in GetInterestSectors(GetSectorOfPosition(skillTargetPositioin)))
			{
				foreach (Hero hero in sector.heroes.Values)
				{
					lock (hero.syncObject)
					{
						if (monsterInst.isDead)
						{
							break;
						}
						if (!hero.isDead && BattleUtil.IsHit(monsterInst.position, fHitRange, hero.position, hero.radius) && monsterInstanceSkill.monsterSkill.ValidationAreaContains(skillTargetPositioin, monsterInst.position, monsterInst.rotationY, hero.position))
						{
							ProcessMonsterSkillHit_Hero(new OffenseHit(offense, hit.id), hero);
						}
						continue;
					}
				}
			}
		}
	}

	protected void ProcessMonsterSkillHit_Hero(OffenseHit offenseHit, Hero target)
	{
		MonsterInstance attacker = (MonsterInstance)offenseHit.offense.attacker;
		int nAttackerNationId = attacker.nationId;
		int nTargetNationId = target.nationId;
		if (nTargetNationId != nAttackerNationId && !IsAlliance(nTargetNationId, nAttackerNationId))
		{
			target.Hit(offenseHit);
		}
	}

	public void ProcessTamingMonsterSkillHit(OffenseHit offenseHit, MonsterInstance target)
	{
		target.Hit(offenseHit);
	}

	public void ProcessSkillEffectHitTick(SkillEffect effect)
	{
		Offense offense = effect.offense;
		Unit attacker = offense.attacker;
		SkillHit hit = offense.skill.GetHit(effect.currentHitId);
		if (hit.damageFactor != 0f)
		{
			switch (attacker.type)
			{
			case UnitType.Hero:
				ProcessSkillEffectHitTick_HeroSkill(effect);
				break;
			case UnitType.MonsterInstance:
				ProcessSkillEffectHitTick_MonsterSkill(effect);
				break;
			}
		}
	}

	private void ProcessSkillEffectHitTick_HeroSkill(SkillEffect effect)
	{
		Offense offense = effect.offense;
		int nHitId = effect.currentHitId;
		JobSkill jobSkill = (JobSkill)offense.skill;
		bool bIsAcquiredLak = false;
		foreach (Sector sector in GetInterestSectors(GetSectorOfPosition(effect.position)))
		{
			if (effect.isHeroHitEnabled && jobSkill.heroHitType != 0)
			{
				foreach (Hero hero in sector.heroes.Values)
				{
					lock (hero.syncObject)
					{
						if ((jobSkill.heroHitType != 1 || !(effect.targetId != hero.id)) && effect.Contains(hero.position) && ProcessHeroSkillHit_Hero(new OffenseHit(offense, nHitId), effect.attackerLevel, hero))
						{
							bIsAcquiredLak = true;
						}
					}
				}
			}
			MonsterInstance[] array = sector.monsterInsts.Values.ToArray();
			foreach (MonsterInstance monsterInst in array)
			{
				if (effect.Contains(monsterInst.position) && ProcessHeroSkillHit_Monster(new OffenseHit(offense, nHitId), monsterInst))
				{
					bIsAcquiredLak = true;
				}
			}
			CartInstance[] array2 = sector.cartInsts.Values.ToArray();
			foreach (CartInstance cartInst in array2)
			{
				if (effect.Contains(cartInst.position) && ProcessHeroSkillHit_Cart(new OffenseHit(offense, nHitId), cartInst))
				{
					bIsAcquiredLak = true;
				}
			}
		}
		if (bIsAcquiredLak)
		{
			SkillHit hit = jobSkill.GetHit(nHitId);
			if (hit != null)
			{
				Hero attacker = (Hero)offense.attacker;
				attacker.AddLak(hit.acquireLak);
			}
		}
	}

	private void ProcessSkillEffectHitTick_MonsterSkill(SkillEffect effect)
	{
		Offense offense = effect.offense;
		int nHitId = effect.currentHitId;
		foreach (Sector sector in GetInterestSectors(GetSectorOfPosition(effect.position)))
		{
			if (!effect.isHeroHitEnabled)
			{
				continue;
			}
			foreach (Hero hero in sector.heroes.Values)
			{
				lock (hero.syncObject)
				{
					if (!hero.isDead && effect.Contains(hero.position))
					{
						ProcessMonsterSkillHit_Hero(new OffenseHit(offense, nHitId), hero);
					}
				}
			}
		}
	}

	public void ProcessSkillEffectFinished(SkillEffect effect)
	{
		RemoveSkillEffect(effect);
	}

	public void ProcessHeroJobCommonSkillHit_Monster(Hero attacker, JobCommonSkill skill, MonsterInstance target)
	{
		if (target.monster.tamingEnabled)
		{
			target.HitMentalStrength(attacker, skill.mentalStrengthDamage, skill.skillId);
		}
	}
}
