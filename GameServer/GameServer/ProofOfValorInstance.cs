using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ProofOfValorInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kStatus_Disqualification = 5;

	private ProofOfValor m_proofOfValor;

	private HeroProofOfValorInstance m_heroProofOfValorInst;

	private Hero m_hero;

	private int m_nStatus;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private Dictionary<long, ProofOfValorBuffBoxInstance> m_buffBoxInsts = new Dictionary<long, ProofOfValorBuffBoxInstance>();

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_exitDelayTimeTimer;

	private Timer m_buffBoxCreationTimer;

	private Timer m_buffBoxLifetimeTimer;

	public override PlaceType placeType => PlaceType.ProofOfValor;

	public override Location location => m_proofOfValor;

	public override int locationParam => 0;

	public override Rect3D mapRect => m_proofOfValor.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => m_nStatus == 2;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public ProofOfValor proofOfValor => m_proofOfValor;

	public HeroProofOfValorInstance heroProofOfValorInst => m_heroProofOfValorInst;

	public int status => m_nStatus;

	public bool isFinished
	{
		get
		{
			if (m_nStatus != 3 && m_nStatus != 4)
			{
				return m_nStatus == 5;
			}
			return true;
		}
	}

	public DateTimeOffset startTime => m_startTime;

	public DateTimeOffset endTime => m_endTime;

	public int playTime => m_nPlayTime;

	public Dictionary<long, ProofOfValorBuffBoxInstance> buffBoxInsts => m_buffBoxInsts;

	public void Init(HeroProofOfValorInstance heroProofOfValorInst, DateTimeOffset time)
	{
		if (heroProofOfValorInst == null)
		{
			throw new ArgumentNullException("heroProofOfValorInst");
		}
		m_proofOfValor = Resource.instance.proofOfValor;
		m_heroProofOfValorInst = heroProofOfValorInst;
		InitPlace();
		ProofOfValorBossMonsterArrange bossMonsterArrange = heroProofOfValorInst.bossMonsterArrange;
		ProofOfValorBossMonsterInstance bossMonsterInst = CreateBossMonster(bossMonsterArrange);
		AddMonster(bossMonsterInst);
		foreach (ProofOfValorNormalMonsterArrange normalMonsterArrange in bossMonsterArrange.normalMonsterArranges)
		{
			ProofOfValorNormalMonsterInstance normalMonsterInst = CreateNormalMonster(normalMonsterArrange);
			AddMonster(normalMonsterInst);
		}
	}

	private ProofOfValorBossMonsterInstance CreateBossMonster(ProofOfValorBossMonsterArrange arrange)
	{
		ProofOfValorBossMonsterInstance monsterInst = new ProofOfValorBossMonsterInstance();
		monsterInst.Init(this, arrange);
		return monsterInst;
	}

	private ProofOfValorNormalMonsterInstance CreateNormalMonster(ProofOfValorNormalMonsterArrange arrange)
	{
		ProofOfValorNormalMonsterInstance monsterInst = new ProofOfValorNormalMonsterInstance();
		monsterInst.Init(this, arrange);
		return monsterInst;
	}

	protected override void OnHeroEnter(Hero hero, DateTimeOffset time)
	{
		base.OnHeroEnter(hero, time);
		m_hero = hero;
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		if (isLogOut)
		{
			Disqualification();
		}
		hero.StopProofOfValorBuff();
		Close();
	}

	public void Start(DateTimeOffset time)
	{
		m_nStatus = 1;
		m_hero.dailyProofOfValorPlayCount.value++;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroProofOfValorInstance(m_heroProofOfValorInst.id, 1, m_heroProofOfValorInst.level, 0, time));
		dbWork.Schedule();
		int nDuration = m_proofOfValor.startDelayTime * 1000;
		m_playWaitingTimer = new Timer(OnPlayWaitingTimerTick);
		m_playWaitingTimer.Change(nDuration, -1);
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
			StartBuffBoxStartTimer();
			int nLimitTime = m_proofOfValor.limitTime * 1000;
			m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
			m_limitTimeTimer.Change(nLimitTime, -1);
			ServerEvent.SendProofOfValorStart(m_hero.account.peer);
		}
	}

	public void StartBuffBoxStartTimer()
	{
		int nBuffBoxCreationTime = m_proofOfValor.buffBoxCreationTime * 1000;
		int nBuffBoxCreationInterval = m_proofOfValor.buffBoxCreationInterval * 1000;
		m_buffBoxCreationTimer = new Timer(OnBuffBoxCreationTimerTick);
		m_buffBoxCreationTimer.Change(nBuffBoxCreationTime, nBuffBoxCreationInterval);
	}

	private void OnBuffBoxCreationTimerTick(object state)
	{
		AddWork(new SFAction(CreateBuffBox), bGlobalLockRequired: false);
	}

	private void CreateBuffBox()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		List<PDProofOfValorBuffBoxInstance> buffBoxInsts = new List<PDProofOfValorBuffBoxInstance>();
		foreach (ProofOfValorBuffBoxArrange arrange in m_proofOfValor.buffBoxArranges)
		{
			ProofOfValorBuffBoxInstance buffBoxInst = new ProofOfValorBuffBoxInstance();
			buffBoxInst.Init(this, arrange, currentTime);
			AddBuffBox(buffBoxInst);
			buffBoxInsts.Add(buffBoxInst.ToPDProofOfValorBuffBoxInstance());
		}
		ServerEvent.SendProofOfValorBuffBoxCreated(m_hero.account.peer, buffBoxInsts.ToArray());
		StartBuffBoxLifetimeTimer();
	}

	private void AddBuffBox(ProofOfValorBuffBoxInstance buffBoxInst)
	{
		m_buffBoxInsts.Add(buffBoxInst.instanceId, buffBoxInst);
	}

	private void StartBuffBoxLifetimeTimer()
	{
		int nLifeTime = m_proofOfValor.buffBoxLifeTime * 1000;
		if (m_buffBoxLifetimeTimer == null)
		{
			m_buffBoxLifetimeTimer = new Timer(OnBuffBoxLifetimeTimerTick);
		}
		m_buffBoxLifetimeTimer.Change(nLifeTime, -1);
	}

	private void OnBuffBoxLifetimeTimerTick(object state)
	{
		AddWork(new SFAction(EndBuffBoxLifetime), bGlobalLockRequired: false);
	}

	private void EndBuffBoxLifetime()
	{
		RemoveAllBuffBox();
		ServerEvent.SendProofOfValorBuffBoxLifetimeEnded(m_hero.account.peer);
	}

	public ProofOfValorBuffBoxInstance GetBuffBoxInstance(long lnInstanceId)
	{
		if (!m_buffBoxInsts.TryGetValue(lnInstanceId, out var value))
		{
			return null;
		}
		return value;
	}

	public void RemoveAllBuffBox()
	{
		m_buffBoxInsts.Clear();
	}

	private void OnLimitTimeTimerTick(object state)
	{
		AddWork(new SFAction(Fail), bGlobalLockRequired: false);
	}

	protected override void OnHeroDead(Hero hero)
	{
		base.OnHeroDead(hero);
		Fail();
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus == 2 && monsterInst is ProofOfValorBossMonsterInstance)
		{
			Clear();
		}
	}

	private void Clear()
	{
		if (m_nStatus == 2)
		{
			MonsterInstance[] array = m_monsterInsts.Values.ToArray();
			foreach (MonsterInstance monsterInst in array)
			{
				RemoveMonster(monsterInst, bSendEvent: true);
			}
			Finish(3);
			m_heroProofOfValorInst.status = 2;
			m_heroProofOfValorInst.level = m_hero.level;
			HeroCreatureCard heroCreatureCard = m_hero.IncreaseCreatureCardCount(m_heroProofOfValorInst.creatureCard);
			ProofOfValorReward reward = m_proofOfValor.GetReward(m_hero.level);
			long lnRewardExp = 0L;
			if (reward != null)
			{
				lnRewardExp = reward.successExpRewardValue;
				lnRewardExp = (long)Math.Floor((float)lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_hero.level));
				m_hero.AddExp(lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
			}
			ProofOfValorBossMonsterArrange bossMonsterArrange = m_heroProofOfValorInst.bossMonsterArrange;
			int nRewardSoulPowder = bossMonsterArrange.rewardSoulPowder;
			int nSpecialRewardSoulPowder = 0;
			if (bossMonsterArrange.isSpecial)
			{
				nSpecialRewardSoulPowder = bossMonsterArrange.specialRewardSoulPowder;
			}
			m_hero.AddSoulPowder(nRewardSoulPowder + nSpecialRewardSoulPowder);
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroProofOfValorInstance(m_heroProofOfValorInst.id, m_heroProofOfValorInst.status, m_heroProofOfValorInst.level, m_nPlayTime, m_endTime));
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateHeroCreatureCard(heroCreatureCard));
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_hero));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SoulPowder(m_hero.id, m_hero.soulPowder));
			dbWork.Schedule();
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroProofOfValorRewardLog(Guid.NewGuid(), m_hero.id, m_heroProofOfValorInst.id, 1, heroCreatureCard.card.id, nRewardSoulPowder, lnRewardExp, nSpecialRewardSoulPowder, m_endTime));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
			}
			m_hero.CreateHeroProofOfValorInstance(m_endTime, bIsRefreshPaidCount: true);
			int nRemainingTime = Math.Max(m_proofOfValor.limitTime - m_nPlayTime, 0);
			ServerEvent.SendProofOfValorClear(m_hero.account.peer, m_proofOfValor.GetClearGrade(nRemainingTime).clearGrade, lnRewardExp, m_hero.level, m_hero.exp, m_hero.realMaxHP, m_hero.hp, m_hero.soulPowder, heroCreatureCard.ToPDHeroCreatureCard(), m_hero.heroProofOfValorInst.ToPDHeroProofOfValorInstance(), m_hero.proofOfValorPaidRefreshCount);
			int nDuration = m_proofOfValor.exitDelayTime * 1000;
			m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
			m_exitDelayTimeTimer.Change(nDuration, -1);
		}
	}

	private void Fail()
	{
		if (m_nStatus == 2)
		{
			Finish(4);
			m_heroProofOfValorInst.status = 3;
			m_heroProofOfValorInst.level = m_hero.level;
			ProofOfValorReward reward = m_proofOfValor.GetReward(m_hero.level);
			long lnRewardExp = 0L;
			if (reward != null)
			{
				lnRewardExp = reward.failureExpRewardValue;
				lnRewardExp = (long)Math.Floor((float)lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_hero.level));
				m_hero.AddExp(lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
			}
			int nRewardSoulPowder = m_proofOfValor.failureRewardSoulPowder;
			m_hero.AddSoulPowder(nRewardSoulPowder);
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroProofOfValorInstance(m_heroProofOfValorInst.id, m_heroProofOfValorInst.status, m_heroProofOfValorInst.level, m_nPlayTime, m_endTime));
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_hero));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SoulPowder(m_hero.id, m_hero.soulPowder));
			dbWork.Schedule();
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroProofOfValorRewardLog(Guid.NewGuid(), m_hero.id, m_heroProofOfValorInst.id, 2, 0, nRewardSoulPowder, lnRewardExp, 0, m_endTime));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
			}
			m_hero.CreateHeroProofOfValorInstance(m_endTime, bIsRefreshPaidCount: true);
			ServerEvent.SendProofOfValorFail(m_hero.account.peer, lnRewardExp, m_hero.level, m_hero.exp, m_hero.realMaxHP, m_hero.hp, m_hero.soulPowder, m_heroProofOfValorInst.ToPDHeroProofOfValorInstance(), m_hero.proofOfValorPaidRefreshCount);
			int nDuration = m_proofOfValor.exitDelayTime * 1000;
			m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
			m_exitDelayTimeTimer.Change(nDuration, -1);
		}
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
		lock (m_hero.syncObject)
		{
			if (m_hero.isDead)
			{
				m_hero.Revive(bSendEvent: false);
			}
			else
			{
				m_hero.RestoreHP(m_hero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
			}
			Exit(m_hero, isLogOut: false, null);
			ServerEvent.SendProofOfValorBanished(m_hero.account.peer, m_hero.previousContinentId, m_hero.previousNationId, m_hero.hp);
		}
	}

	private void Disqualification()
	{
		if (!isFinished)
		{
			Finish(5);
			m_heroProofOfValorInst.status = 4;
			m_heroProofOfValorInst.level = m_hero.level;
			ProofOfValorReward reward = m_proofOfValor.GetReward(m_hero.level);
			long lnRewardExp = 0L;
			if (reward != null)
			{
				lnRewardExp = reward.failureExpRewardValue;
				lnRewardExp = (long)Math.Floor((float)lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_hero.level));
				m_hero.AddExp(lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
			}
			int nRewardSoulPowder = m_proofOfValor.failureRewardSoulPowder;
			m_hero.AddSoulPowder(nRewardSoulPowder);
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroProofOfValorInstance(m_heroProofOfValorInst.id, m_heroProofOfValorInst.status, m_heroProofOfValorInst.level, m_nPlayTime, m_endTime));
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_hero));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_SoulPowder(m_hero.id, m_hero.soulPowder));
			dbWork.Schedule();
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroProofOfValorRewardLog(Guid.NewGuid(), m_hero.id, m_heroProofOfValorInst.id, 3, 0, nRewardSoulPowder, lnRewardExp, 0, m_endTime));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
			}
			m_hero.CreateHeroProofOfValorInstance(m_endTime, bIsRefreshPaidCount: true);
		}
	}

	public void Finish(int nStatus)
	{
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeBuffBoxCreationTimer();
		m_endTime = DateTimeUtil.currentTime;
		if (m_nStatus == 2)
		{
			m_nPlayTime = (int)Math.Floor((m_endTime - m_startTime).TotalSeconds);
		}
		m_nStatus = nStatus;
	}

	protected override void ReleaseInternal()
	{
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeExitDelayTimeTimer();
		DisposeBuffBoxCreationTimer();
		DisposeBuffBoxLifetimeTimer();
		RemoveAllBuffBox();
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

	private void DisposeExitDelayTimeTimer()
	{
		if (m_exitDelayTimeTimer != null)
		{
			m_exitDelayTimeTimer.Dispose();
			m_exitDelayTimeTimer = null;
		}
	}

	private void DisposeBuffBoxCreationTimer()
	{
		if (m_buffBoxCreationTimer != null)
		{
			m_buffBoxCreationTimer.Dispose();
			m_buffBoxCreationTimer = null;
		}
	}

	private void DisposeBuffBoxLifetimeTimer()
	{
		if (m_buffBoxLifetimeTimer != null)
		{
			m_buffBoxLifetimeTimer.Dispose();
			m_buffBoxLifetimeTimer = null;
		}
	}
}
