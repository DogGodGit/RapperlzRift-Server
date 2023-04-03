using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class InfiniteWarInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	private InfiniteWar m_infiniteWar;

	private InfiniteWarOpenSchedule m_openSchedule;

	private int m_nAverageHeroLevel;

	private int m_nStatus;

	private int m_nHeroEnterNo;

	private DateTimeOffset m_creationTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private Dictionary<long, InfiniteWarBuffBoxInstance> m_buffBoxInsts = new Dictionary<long, InfiniteWarBuffBoxInstance>();

	private Dictionary<Guid, HeroInfiniteWarPoint> m_heroPoints = new Dictionary<Guid, HeroInfiniteWarPoint>();

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_monsterSpawnTimer;

	private Timer m_buffBoxCreationTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.InfiniteWar;

	public override Location location => m_infiniteWar;

	public override int locationParam => 0;

	public override Rect3D mapRect => m_infiniteWar.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => true;

	public override bool distortionScrollUseEnabled => false;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public InfiniteWar infiniteWar => m_infiniteWar;

	public int status => m_nStatus;

	public int heroEnterNo => m_nHeroEnterNo;

	public bool isFinished => m_nStatus == 3;

	public int averageHeroLevel => m_nAverageHeroLevel;

	public void Init(InfiniteWarOpenSchedule openSchedule, DateTimeOffset time, int nAverageHeroLevel)
	{
		if (openSchedule == null)
		{
			throw new ArgumentNullException("openSchedule");
		}
		m_infiniteWar = openSchedule.infiniteWar;
		m_openSchedule = openSchedule;
		m_nAverageHeroLevel = nAverageHeroLevel;
		m_creationTime = time;
		m_nHeroEnterNo = 1;
		InitPlace();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddInfiniteWarCreationLog(m_instanceId, m_openSchedule.id, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		m_nStatus = 1;
		int nDuration = m_infiniteWar.startDelayTime * 1000;
		m_playWaitingTimer = new Timer(OnPlayWaitingTimerTick);
		m_playWaitingTimer.Change(nDuration, -1);
	}

	public float GetRemainingStartTime(DateTimeOffset time)
	{
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fStartTime = (float)m_creationTime.TimeOfDay.TotalSeconds + (float)m_infiniteWar.startDelayTime;
		return Math.Max(fStartTime - fTime, 0f);
	}

	public float GetRemainingLimitTime(DateTimeOffset time)
	{
		if (m_nStatus == 1)
		{
			return m_infiniteWar.limitTime;
		}
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fEndTime = (float)m_startTime.TimeOfDay.TotalSeconds + (float)m_infiniteWar.limitTime;
		return Math.Max(fEndTime - fTime, 0f);
	}

	protected override void OnHeroEnter(Hero hero, DateTimeOffset time)
	{
		base.OnHeroEnter(hero, time);
		m_nHeroEnterNo++;
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		if (isLogOut)
		{
			Disqualification(hero);
		}
	}

	private void OnPlayWaitingTimerTick(object state)
	{
		AddWork(new SFAction(StartPlay), bGlobalLockRequired: false);
	}

	private void StartPlay()
	{
		if (m_nStatus == 1)
		{
			m_nStatus = 2;
			m_startTime = DateTimeUtil.currentTime;
			int nDuration = m_infiniteWar.limitTime * 1000;
			m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
			m_limitTimeTimer.Change(nDuration, -1);
			Start();
		}
	}

	private void OnLimitTimeTimerTick(object state)
	{
		AddWork(new SFAction(DungeonTimeout), bGlobalLockRequired: false);
	}

	private void Start()
	{
		if (m_nStatus == 2)
		{
			ServerEvent.SendInfiniteWarStart(GetClientPeers());
			int nMonsterSpawnDelayTime = m_infiniteWar.monsterSpawnDelayTime * 1000;
			m_monsterSpawnTimer = new Timer(OnMonsterSpawnTimerTick);
			m_monsterSpawnTimer.Change(nMonsterSpawnDelayTime, -1);
			int nBuffBoxCreationInterval = m_infiniteWar.buffBoxCreationInterval * 1000;
			m_buffBoxCreationTimer = new Timer(OnBuffBoxCreationInterval);
			m_buffBoxCreationTimer.Change(nBuffBoxCreationInterval, nBuffBoxCreationInterval);
		}
	}

	private void OnMonsterSpawnTimerTick(object state)
	{
		AddWork(new SFAction(ProcessMonsterSpawn), bGlobalLockRequired: false);
	}

	private void ProcessMonsterSpawn()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		List<PDInfiniteWarMonsterInstance> monsterInsts = new List<PDInfiniteWarMonsterInstance>();
		InfiniteWarMonsterAttrFactor attrFactor = m_infiniteWar.GetMonsterAttrFactor(m_nAverageHeroLevel);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (InfiniteWarMonsterArrange arrange in m_infiniteWar.monsterArranges)
		{
			InfiniteWarMonsterInstance monsterInst = CreateMonster(arrange, attrFactor);
			AddMonster(monsterInst);
			monsterInsts.Add((PDInfiniteWarMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
		}
		ServerEvent.SendInfiniteWarMonsterSpawn(GetClientPeers(), monsterInsts.ToArray());
	}

	private InfiniteWarMonsterInstance CreateMonster(InfiniteWarMonsterArrange arrange, InfiniteWarMonsterAttrFactor attrFactor)
	{
		InfiniteWarMonsterInstance inst = new InfiniteWarMonsterInstance();
		inst.Init(this, arrange, attrFactor);
		return inst;
	}

	private void OnBuffBoxCreationInterval(object state)
	{
		AddWork(new SFAction(ProcessBuffBoxCreation), bGlobalLockRequired: false);
	}

	private void ProcessBuffBoxCreation()
	{
		if (m_nStatus == 2)
		{
			List<PDInfiniteWarBuffBoxInstance> buffBoxInsts = new List<PDInfiniteWarBuffBoxInstance>();
			for (int i = 0; i < m_infiniteWar.buffBoxCreationCount; i++)
			{
				InfiniteWarBuffBoxInstance buffBoxInst = CreateBuffBox();
				AddBuffBox(buffBoxInst);
				buffBoxInsts.Add(buffBoxInst.ToPDInfiniteWarBuffBoxInstance());
			}
			ServerEvent.SendInfiniteWarBuffBoxCreated(GetClientPeers(), buffBoxInsts.ToArray());
		}
	}

	public InfiniteWarBuffBoxInstance CreateBuffBox()
	{
		InfiniteWarBuffBoxInstance inst = new InfiniteWarBuffBoxInstance();
		inst.Init(this, m_infiniteWar.SelectBuffBox(), m_infiniteWar.SelectBuffBoxPosition());
		return inst;
	}

	private void AddBuffBox(InfiniteWarBuffBoxInstance buffBoxInst)
	{
		m_buffBoxInsts.Add(buffBoxInst.instanceId, buffBoxInst);
	}

	public InfiniteWarBuffBoxInstance GetBuffBox(long lnInstanceId)
	{
		if (!m_buffBoxInsts.TryGetValue(lnInstanceId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDInfiniteWarBuffBoxInstance> GetPDBuffBoxInstances()
	{
		List<PDInfiniteWarBuffBoxInstance> results = new List<PDInfiniteWarBuffBoxInstance>();
		foreach (InfiniteWarBuffBoxInstance inst in m_buffBoxInsts.Values)
		{
			results.Add(inst.ToPDInfiniteWarBuffBoxInstance());
		}
		return results;
	}

	public void RemoveBuffBox(InfiniteWarBuffBoxInstance buffBoxInst)
	{
		if (buffBoxInst == null)
		{
			throw new ArgumentNullException("buffBoxInst");
		}
		buffBoxInst.Release();
		m_buffBoxInsts.Remove(buffBoxInst.instanceId);
	}

	private void ClearBuffBox()
	{
		foreach (InfiniteWarBuffBoxInstance inst in m_buffBoxInsts.Values)
		{
			inst.Release();
		}
		m_buffBoxInsts.Clear();
	}

	public void OnExpireBuffBoxLifetime(InfiniteWarBuffBoxInstance buffBoxInst)
	{
		if (buffBoxInst == null)
		{
			throw new ArgumentNullException("buffBoxInst");
		}
		RemoveBuffBox(buffBoxInst);
		ServerEvent.SendInfiniteWarBuffBoxLifetimeEnded(GetClientPeers(), buffBoxInst.instanceId);
	}

	private HeroInfiniteWarPoint GetHeroPoint(Guid heroId)
	{
		if (!m_heroPoints.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	private HeroInfiniteWarPoint GetOrCreateHeroPoint(Hero hero)
	{
		HeroInfiniteWarPoint heroPoint = GetHeroPoint(hero.id);
		if (heroPoint == null)
		{
			heroPoint = new HeroInfiniteWarPoint(hero);
			m_heroPoints.Add(heroPoint.hero.id, heroPoint);
		}
		return heroPoint;
	}

	public List<PDInfiniteWarPoint> GetPDInfiniteWarPoints()
	{
		List<PDInfiniteWarPoint> results = new List<PDInfiniteWarPoint>();
		foreach (HeroInfiniteWarPoint point in m_heroPoints.Values)
		{
			results.Add(point.ToPDInfiniteWarPoint());
		}
		return results;
	}

	private void RemoveHeroPoint(Guid heroId)
	{
		m_heroPoints.Remove(heroId);
	}

	protected override void OnHeroDead(Hero hero)
	{
		base.OnHeroDead(hero);
		if (m_nStatus == 2 && hero.lastAttacker is Hero killer)
		{
			Hero heroKiller = GetHero(killer.id);
			if (heroKiller != null)
			{
				HeroInfiniteWarPoint heroPoint = GetOrCreateHeroPoint(heroKiller);
				heroPoint.AddPoint(hero.lastDamageTime, m_infiniteWar.heroKillPoint);
				ServerEvent.SendInfiniteWarPointAcquisition(heroKiller.account.peer, heroPoint.point, heroPoint.lastPointAcquisitionTime.Ticks);
				ServerEvent.SendHeroInfiniteWarPointAcquisition(GetClientPeers(heroKiller.id), heroKiller.id, heroPoint.point, heroPoint.lastPointAcquisitionTime.Ticks);
			}
		}
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus == 2)
		{
			Hero killer = (Hero)monsterInst.lastAttacker;
			Hero heroKiller = GetHero(killer.id);
			if (heroKiller != null)
			{
				InfiniteWarMonsterInstance infiniteWarMonsterInst = (InfiniteWarMonsterInstance)monsterInst;
				HeroInfiniteWarPoint heroPoint = GetOrCreateHeroPoint(heroKiller);
				heroPoint.AddPoint(monsterInst.lastDamageTime, infiniteWarMonsterInst.arrange.point);
				ServerEvent.SendInfiniteWarPointAcquisition(heroKiller.account.peer, heroPoint.point, heroPoint.lastPointAcquisitionTime.Ticks);
				ServerEvent.SendHeroInfiniteWarPointAcquisition(GetClientPeers(heroKiller.id), heroKiller.id, heroPoint.point, heroPoint.lastPointAcquisitionTime.Ticks);
			}
		}
	}

	private void DungeonTimeout()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		Finish(3);
		try
		{
			SFSqlStandaloneWork logWork2 = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork2.AddSqlCommand(GameLogDac.CSC_AddInfiniteWarCompletionLog(m_instanceId, m_endTime));
			logWork2.Schedule();
		}
		catch (Exception ex2)
		{
			SFLogUtil.Error(GetType(), null, ex2);
		}
		foreach (Hero hero2 in m_heroes.Values)
		{
			if (GetHeroPoint(hero2.id) == null)
			{
				GetOrCreateHeroPoint(hero2);
			}
		}
		List<PDInfiniteWarRanking> rankins = new List<PDInfiniteWarRanking>();
		HeroInfiniteWarPoint[] heroPoints = m_heroPoints.Values.ToArray();
		Array.Sort(heroPoints, HeroInfiniteWarPoint.Compare);
		Array.Reverse(heroPoints);
		for (int i = 0; i < heroPoints.Length; i++)
		{
			HeroInfiniteWarPoint heroPoint = heroPoints[i];
			if (heroPoint.point > 0)
			{
				heroPoint.rank = i + 1;
			}
			rankins.Add(heroPoint.ToPDInfiniteWarRanking());
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_InfiniteWar);
		foreach (Hero hero in m_heroes.Values)
		{
			lock (hero.syncObject)
			{
				dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(hero.id));
				List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
				List<PDItemBooty> booties = new List<PDItemBooty>();
				List<PDItemBooty> rankingBooties = new List<PDItemBooty>();
				Mail mail = null;
				foreach (InfiniteWarReward reward in m_infiniteWar.rewards)
				{
					ItemReward itemReward2 = reward.itemReward;
					if (itemReward2 == null)
					{
						continue;
					}
					Item item2 = itemReward2.item;
					bool bOwned2 = itemReward2.owned;
					int nCount2 = itemReward2.count;
					int nRemainingItemCount2 = hero.AddItem(item2, bOwned2, nCount2, changedInventorySlots);
					PDItemBooty booty3 = new PDItemBooty();
					booty3.id = item2.id;
					booty3.owned = bOwned2;
					booty3.count = nCount2;
					booties.Add(booty3);
					if (nRemainingItemCount2 > 0)
					{
						if (mail == null)
						{
							mail = Mail.Create("MAIL_REWARD_N_21", "MAIL_REWARD_D_21", m_endTime);
						}
						mail.AddAttachment(new MailAttachment(item2, nRemainingItemCount2, bOwned2));
					}
				}
				HeroInfiniteWarPoint heroPoint2 = GetHeroPoint(hero.id);
				if (heroPoint2 != null)
				{
					List<InfiniteWarRankingReward> rankingRewards = m_infiniteWar.GetRankingRewards(heroPoint2.rank);
					if (rankingRewards != null)
					{
						foreach (InfiniteWarRankingReward rankingReward in rankingRewards)
						{
							ItemReward itemReward = rankingReward.itemReward;
							if (itemReward == null)
							{
								continue;
							}
							Item item = itemReward.item;
							bool bOwned = itemReward.owned;
							int nCount = itemReward.count;
							int nRemainingItemCount = hero.AddItem(item, bOwned, nCount, changedInventorySlots);
							PDItemBooty booty2 = new PDItemBooty();
							booty2.id = item.id;
							booty2.owned = bOwned;
							booty2.count = nCount;
							rankingBooties.Add(booty2);
							if (nRemainingItemCount > 0)
							{
								if (mail == null)
								{
									mail = Mail.Create("MAIL_REWARD_N_21", "MAIL_REWARD_D_21", m_endTime);
								}
								mail.AddAttachment(new MailAttachment(item, nRemainingItemCount, bOwned));
							}
						}
					}
				}
				foreach (InventorySlot slot in changedInventorySlots)
				{
					dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
				}
				if (mail != null)
				{
					dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(mail));
				}
				try
				{
					SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
					logWork.AddSqlCommand(GameLogDac.CSC_AddInfiniteWarCompletionMemberLog(m_instanceId, hero.id, heroPoint2?.point ?? 0, heroPoint2?.lastPointAcquisitionTime ?? DateTimeOffset.MinValue));
					foreach (PDItemBooty booty in booties)
					{
						logWork.AddSqlCommand(GameLogDac.CSC_AddInfiniteWarCompletionRewardDetailLog(Guid.NewGuid(), m_instanceId, hero.id, booty.id, booty.owned, booty.count));
					}
					foreach (PDItemBooty rankingBooty in rankingBooties)
					{
						logWork.AddSqlCommand(GameLogDac.CSC_AddInfiniteWarCompletionRewardDetailLog(Guid.NewGuid(), m_instanceId, hero.id, rankingBooty.id, rankingBooty.owned, rankingBooty.count));
					}
					logWork.Schedule();
				}
				catch (Exception ex)
				{
					SFLogUtil.Error(GetType(), null, ex);
				}
				ServerEvent.SendInfiniteWarClear(hero.account.peer, rankins.ToArray(), booties.ToArray(), rankingBooties.ToArray(), InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
			}
		}
		dbWork.Schedule();
		int nDuration = m_infiniteWar.exitDelayTime * 1000;
		m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
		m_exitDelayTimeTimer.Change(nDuration, -1);
	}

	private void OnExitDelayTimeTimerTick(object state)
	{
		AddWork(new SFAction(DungeonBanish), bGlobalLockRequired: true);
	}

	private void DungeonBanish()
	{
		if (!isFinished)
		{
			return;
		}
		Hero[] array = m_heroes.Values.ToArray();
		foreach (Hero hero in array)
		{
			lock (hero.syncObject)
			{
				if (hero.isDead)
				{
					hero.Revive(bSendEvent: false);
				}
				else
				{
					hero.RestoreHP(hero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
				}
				Exit(hero, isLogOut: false, null);
				ServerEvent.SendInfiniteWarBanished(hero.account.peer, hero.previousContinentId, hero.previousNationId, hero.hp);
			}
		}
		Close();
	}

	public void Disqualification(Hero hero)
	{
		if (isFinished)
		{
			return;
		}
		RemoveHeroPoint(hero.id);
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddInfiniteWarDisqualificationLog(m_instanceId, hero.id, DateTimeUtil.currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void Finish(int nStatus)
	{
		ClearBuffBox();
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeMonsterSpawnTimer();
		DisposeBuffBoxCreationTimer();
		DisposeExitDelayTimeTimer();
		m_endTime = DateTimeUtil.currentTime;
		if (m_nStatus == 2)
		{
			m_nPlayTime = (int)Math.Floor((m_endTime - m_startTime).TotalSeconds);
		}
		m_nStatus = nStatus;
	}

	public override bool ProcessHeroSkillHit_Hero(OffenseHit offenseHit, int nAttackerLevel, Hero target)
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
		if (attacker.id == target.id)
		{
			return false;
		}
		return target.Hit(offenseHit);
	}

	public override bool IsHeroRankActiveSkillCast_OtherHero(Hero source, Hero target)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (source.id == target.id)
		{
			return false;
		}
		return true;
	}

	protected override void ReleaseInternal()
	{
		ClearBuffBox();
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeMonsterSpawnTimer();
		DisposeBuffBoxCreationTimer();
		DisposeExitDelayTimeTimer();
		base.ReleaseInternal();
	}

	private void DisposePlayWaitingTimer()
	{
		if (m_playWaitingTimer != null)
		{
			m_playWaitingTimer.Dispose();
			m_playWaitingTimer = null;
		}
	}

	private void DisposeLimitTimeTimer()
	{
		if (m_limitTimeTimer != null)
		{
			m_limitTimeTimer.Dispose();
			m_limitTimeTimer = null;
		}
	}

	private void DisposeMonsterSpawnTimer()
	{
		if (m_monsterSpawnTimer != null)
		{
			m_monsterSpawnTimer.Dispose();
			m_monsterSpawnTimer = null;
		}
	}

	private void DisposeBuffBoxCreationTimer()
	{
		if (m_monsterSpawnTimer != null)
		{
			m_buffBoxCreationTimer.Dispose();
			m_buffBoxCreationTimer = null;
		}
	}

	private void DisposeExitDelayTimeTimer()
	{
		if (m_exitDelayTimeTimer != null)
		{
			m_exitDelayTimeTimer.Dispose();
			m_exitDelayTimeTimer = null;
		}
	}
}
