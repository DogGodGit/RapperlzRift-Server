using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class WarMemoryInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	private WarMemory m_warMemory;

	private WarMemorySchedule m_schedule;

	private int m_nAverageHeroLevel;

	private int m_nStatus;

	private int m_nWaveNo;

	private int m_nHeroEnterNo;

	private DateTimeOffset m_creationTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private int m_nTargetArrangeMonsterCount;

	private Dictionary<long, WarMemoryTransformationObjectInstance> m_transformationObjectInsts = new Dictionary<long, WarMemoryTransformationObjectInstance>();

	private Dictionary<Guid, HeroWarMemoryPoint> m_heroPoints = new Dictionary<Guid, HeroWarMemoryPoint>();

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_waveStartDelayTimeTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.WarMemory;

	public override Location location => m_warMemory;

	public override int locationParam => 0;

	public override Rect3D mapRect => m_warMemory.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => false;

	public WarMemory warMemory => m_warMemory;

	public int averageHeroLevel => m_nAverageHeroLevel;

	public int status => m_nStatus;

	public int waveNo => m_nWaveNo;

	public int heroEnterNo => m_nHeroEnterNo;

	public bool isFinished
	{
		get
		{
			if (m_nStatus != 3)
			{
				return m_nStatus == 4;
			}
			return true;
		}
	}

	public void Init(WarMemorySchedule schedule, DateTimeOffset time, int nAverageHeroLevel)
	{
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_warMemory = schedule.warMemory;
		m_schedule = schedule;
		m_nAverageHeroLevel = nAverageHeroLevel;
		m_creationTime = time;
		m_nHeroEnterNo = 1;
		InitPlace();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddWarMemoryCreationLog(m_instanceId, m_schedule.id, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		m_nStatus = 1;
		int nDuration = m_warMemory.startDelayTime * 1000;
		m_playWaitingTimer = new Timer(OnPlayWaitingTimerTick);
		m_playWaitingTimer.Change(nDuration, -1);
	}

	public float GetRemainingStartTime(DateTimeOffset time)
	{
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fStartTime = (float)m_creationTime.TimeOfDay.TotalSeconds + (float)m_warMemory.startDelayTime;
		return Math.Max(fStartTime - fTime, 0f);
	}

	public float GetRemainingLimitTime(DateTimeOffset time)
	{
		if (m_nStatus == 1)
		{
			return m_warMemory.limitTime;
		}
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fEndTime = (float)m_startTime.TimeOfDay.TotalSeconds + (float)m_warMemory.limitTime;
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
		hero.warMemoryStartPositionIndex = -1;
		hero.CancelWarMemoryTransformationMonsterEffect(bResetHP: true, bSendEventToOthers: false);
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
			int nDuration = m_warMemory.limitTime * 1000;
			m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
			m_limitTimeTimer.Change(nDuration, -1);
			StartWaveStartDelayTimeTimer();
		}
	}

	private void OnLimitTimeTimerTick(object state)
	{
		AddWork(new SFAction(Fail), bGlobalLockRequired: false);
	}

	private void StartWaveStartDelayTimeTimer()
	{
		WarMemoryWave nextWave = m_warMemory.GetWave(m_nWaveNo + 1);
		int nDuration = nextWave.startDelayTime * 1000;
		if (m_waveStartDelayTimeTimer == null)
		{
			m_waveStartDelayTimeTimer = new Timer(OnWaveStartDelayTimeTimerTick);
		}
		m_waveStartDelayTimeTimer.Change(nDuration, -1);
	}

	private void OnWaveStartDelayTimeTimerTick(object state)
	{
		AddWork(new SFAction(StartNextWave), bGlobalLockRequired: false);
	}

	private void StartNextWave()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		m_nWaveNo++;
		WarMemoryWave wave = m_warMemory.GetWave(m_nWaveNo);
		List<PDWarMemoryMonsterInstance> monsterInsts = new List<PDWarMemoryMonsterInstance>();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		WarMemoryMonsterAttrFactor monsterAttrFactor = m_warMemory.GetMonsterAttrFactor(m_nAverageHeroLevel);
		foreach (WarMemoryMonsterArrange arrange in wave.monsterArranges)
		{
			int nMonsterCount = arrange.monsterCount;
			for (int i = 0; i < nMonsterCount; i++)
			{
				WarMemoryMonsterInstance monsterInst = CreateMonster(arrange, monsterAttrFactor);
				AddMonster(monsterInst);
				monsterInsts.Add((PDWarMemoryMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
			}
			if (wave.targetType == 2 && wave.targetArrangeKey == arrange.key)
			{
				m_nTargetArrangeMonsterCount = nMonsterCount;
			}
		}
		List<PDWarMemoryTransformationObjectInstance> objectInsts = new List<PDWarMemoryTransformationObjectInstance>();
		foreach (WarMemoryTransformationObject tranformationObject in wave.transformationObjects)
		{
			WarMemoryTransformationObjectInstance objectInst = new WarMemoryTransformationObjectInstance();
			objectInst.Init(this, tranformationObject);
			AddTransformationObject(objectInst);
			objectInsts.Add(objectInst.ToPDWarMemoryTransformationObjectInstance());
		}
		ServerEvent.SendWarMemoryWaveStart(GetClientPeers(), m_nWaveNo, monsterInsts.ToArray(), objectInsts.ToArray());
	}

	private WarMemoryMonsterInstance CreateMonster(WarMemoryMonsterArrange arrange, WarMemoryMonsterAttrFactor monsterAttrFactor)
	{
		WarMemoryMonsterInstance inst = new WarMemoryMonsterInstance();
		inst.Init(this, arrange, monsterAttrFactor);
		return inst;
	}

	public void SummonMonster(WarMemoryMonsterInstance monsterInst)
	{
		if (monsterInst == null)
		{
			return;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		List<PDWarMemorySummonMonsterInstance> monsterInsts = new List<PDWarMemorySummonMonsterInstance>();
		WarMemoryMonsterAttrFactor attrFactor = m_warMemory.GetMonsterAttrFactor(m_nAverageHeroLevel);
		foreach (WarMemorySummonMonsterArrange arrange in monsterInst.arrange.summonMonsterArranges)
		{
			WarMemorySummonMonsterInstance summonMonsterInst = CreateSummonMonster(arrange, attrFactor, monsterInst);
			AddMonster(summonMonsterInst);
			monsterInsts.Add((PDWarMemorySummonMonsterInstance)summonMonsterInst.ToPDMonsterInstance(currentTime));
		}
		ServerEvent.SendWarMemoryMonsterSummon(GetClientPeers(), monsterInsts.ToArray());
	}

	private WarMemorySummonMonsterInstance CreateSummonMonster(WarMemorySummonMonsterArrange arrange, WarMemoryMonsterAttrFactor attrFactor, WarMemoryMonsterInstance parentMonsterInst)
	{
		WarMemorySummonMonsterInstance inst = new WarMemorySummonMonsterInstance();
		inst.Init(this, arrange, attrFactor, parentMonsterInst);
		return inst;
	}

	private WarMemoryTransformationObjectInstance CreateTransformObject(WarMemoryTransformationObject transformationObject)
	{
		WarMemoryTransformationObjectInstance inst = new WarMemoryTransformationObjectInstance();
		inst.Init(this, transformationObject);
		return inst;
	}

	private void AddTransformationObject(WarMemoryTransformationObjectInstance objectInst)
	{
		m_transformationObjectInsts.Add(objectInst.instanceId, objectInst);
	}

	public WarMemoryTransformationObjectInstance GetTransformationObject(long lnInstanceId)
	{
		if (!m_transformationObjectInsts.TryGetValue(lnInstanceId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDWarMemoryTransformationObjectInstance> GetPDTransformationObjectInstances()
	{
		List<PDWarMemoryTransformationObjectInstance> results = new List<PDWarMemoryTransformationObjectInstance>();
		foreach (WarMemoryTransformationObjectInstance objectInst in m_transformationObjectInsts.Values)
		{
			results.Add(objectInst.ToPDWarMemoryTransformationObjectInstance());
		}
		return results;
	}

	public void RemoveTransformationObject(WarMemoryTransformationObjectInstance objectInst)
	{
		if (objectInst == null)
		{
			throw new ArgumentNullException("objectInst");
		}
		objectInst.Release();
		m_transformationObjectInsts.Remove(objectInst.instanceId);
	}

	public void ClearTransformationObject()
	{
		WarMemoryTransformationObjectInstance[] array = m_transformationObjectInsts.Values.ToArray();
		foreach (WarMemoryTransformationObjectInstance transformationObjectInst in array)
		{
			Hero interactionHero = transformationObjectInst.interactionHero;
			if (interactionHero != null)
			{
				lock (interactionHero.syncObject)
				{
					interactionHero.CancelWarMemoryTransformationObjectInteraction(bSendEventToMyself: true, bSendEventToOthers: true);
				}
			}
			transformationObjectInst.Release();
			m_transformationObjectInsts.Remove(transformationObjectInst.instanceId);
		}
		m_transformationObjectInsts.Clear();
	}

	private HeroWarMemoryPoint GetHeroPoint(Guid heroId)
	{
		if (!m_heroPoints.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	public HeroWarMemoryPoint GetOrCreateHeroPoint(Hero hero)
	{
		HeroWarMemoryPoint heroPoint = GetHeroPoint(hero.id);
		if (heroPoint == null)
		{
			heroPoint = new HeroWarMemoryPoint(hero);
			m_heroPoints.Add(hero.id, heroPoint);
		}
		return heroPoint;
	}

	public List<PDWarMemoryPoint> GetPDWarMemoryPoints()
	{
		List<PDWarMemoryPoint> results = new List<PDWarMemoryPoint>();
		foreach (HeroWarMemoryPoint point in m_heroPoints.Values)
		{
			results.Add(point.ToPDWarMemoryPoint());
		}
		return results;
	}

	private void RemoveHeroPoint(Guid heroId)
	{
		m_heroPoints.Remove(heroId);
	}

	public void OnExpireTransformationObjectLifetime(WarMemoryTransformationObjectInstance objectInst)
	{
		if (objectInst == null)
		{
			throw new ArgumentNullException("objectInst");
		}
		Hero interactionHero = objectInst.interactionHero;
		if (interactionHero != null)
		{
			lock (interactionHero.syncObject)
			{
				interactionHero.CancelWarMemoryTransformationObjectInteraction(bSendEventToMyself: true, bSendEventToOthers: true);
			}
		}
		RemoveTransformationObject(objectInst);
		ServerEvent.SendWarMemoryTransformationObjectLifetimeEnded(GetClientPeers(), objectInst.instanceId);
	}

	public void OnTransformationObjectInteractionFinish(WarMemoryTransformationObjectInstance objectInst)
	{
		if (objectInst == null)
		{
			throw new ArgumentNullException("objectInst");
		}
		RemoveTransformationObject(objectInst);
		ClearTransformationObject();
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus != 2)
		{
			return;
		}
		WarMemoryWave wave = m_warMemory.GetWave(m_nWaveNo);
		if (wave.targetType == 1)
		{
			if (m_monsterInsts.Count == 0)
			{
				CompleteWave();
			}
		}
		else if (monsterInst is WarMemoryMonsterInstance warMemoryMonsterInst && warMemoryMonsterInst.arrange.key == wave.targetArrangeKey)
		{
			m_nTargetArrangeMonsterCount--;
			if (m_nTargetArrangeMonsterCount <= 0)
			{
				CompleteWave();
			}
		}
	}

	private void CompleteWave()
	{
		MonsterInstance[] array = m_monsterInsts.Values.ToArray();
		foreach (MonsterInstance monsterInst in array)
		{
			RemoveMonster(monsterInst, bSendEvent: true);
		}
		ClearTransformationObject();
		WarMemoryWave wave = m_warMemory.GetWave(m_nWaveNo);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		int nClearPoint = wave.clearPoint;
		foreach (Hero hero in m_heroes.Values)
		{
			HeroWarMemoryPoint heroPoint = GetOrCreateHeroPoint(hero);
			heroPoint.AddPoint(currentTime, nClearPoint);
		}
		ServerEvent.SendWarMemoryWaveCompleted(GetClientPeers(), GetPDWarMemoryPoints().ToArray());
		if (m_nWaveNo >= m_warMemory.waveCount)
		{
			Clear();
		}
		else
		{
			StartWaveStartDelayTimeTimer();
		}
	}

	private void Clear()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		Finish(3);
		try
		{
			SFSqlStandaloneWork logWork2 = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork2.AddSqlCommand(GameLogDac.CSC_AddWarMemoryCompletionLog(m_instanceId, 1, m_nPlayTime, m_endTime));
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
		List<PDWarMemoryRanking> rankings = new List<PDWarMemoryRanking>();
		HeroWarMemoryPoint[] heroPoints = m_heroPoints.Values.ToArray();
		Array.Sort(heroPoints, HeroWarMemoryPoint.Compare);
		Array.Reverse(heroPoints);
		for (int i = 0; i < heroPoints.Length; i++)
		{
			HeroWarMemoryPoint heroPoint = heroPoints[i];
			if (heroPoint.point > 0)
			{
				heroPoint.rank = i + 1;
			}
			rankings.Add(heroPoint.ToPDWarMemoryRanking());
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_WarMemory);
		foreach (Hero hero in m_heroes.Values)
		{
			lock (hero.syncObject)
			{
				dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(hero.id));
				WarMemoryReward reward = m_warMemory.GetReward(hero.level);
				long lnAcquisitionExp = reward.expReward?.value ?? 0;
				if (lnAcquisitionExp > 0)
				{
					lnAcquisitionExp = (long)Math.Floor((float)lnAcquisitionExp * Cache.instance.GetWorldLevelExpFactor(hero.level));
					hero.AddExp(lnAcquisitionExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
				}
				List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
				List<PDItemBooty> rankingBooties = new List<PDItemBooty>();
				Mail mail = null;
				HeroWarMemoryPoint heroPoint2 = GetHeroPoint(hero.id);
				if (heroPoint2 != null)
				{
					foreach (WarMemoryRankingReward rankingReward in m_warMemory.GetRankingRewards(heroPoint2.rank))
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
				dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(hero));
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
					logWork.AddSqlCommand(GameLogDac.CSC_AddWarMemoryCompletionMemberLog(m_instanceId, hero.id, lnAcquisitionExp, heroPoint2.point, heroPoint2.lastPointAcquisitionTime));
					foreach (PDItemBooty booty in rankingBooties)
					{
						logWork.AddSqlCommand(GameLogDac.CSC_AddWarMemoryCompletionRewardDetailLog(Guid.NewGuid(), m_instanceId, hero.id, booty.id, booty.owned, booty.count));
					}
					logWork.Schedule();
				}
				catch (Exception ex)
				{
					SFLogUtil.Error(GetType(), null, ex);
				}
				ServerEvent.SendWarMemoryClear(hero.account.peer, rankings.ToArray(), lnAcquisitionExp, hero.level, hero.exp, hero.realMaxHP, hero.hp, rankingBooties.ToArray(), InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
			}
		}
		dbWork.Schedule();
		int nDuration = m_warMemory.exitDelayTime * 1000;
		m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
		m_exitDelayTimeTimer.Change(nDuration, -1);
	}

	private void Fail()
	{
		if (m_nStatus == 2)
		{
			Finish(4);
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				logWork.AddSqlCommand(GameLogDac.CSC_AddWarMemoryCompletionLog(m_instanceId, 2, m_nPlayTime, m_endTime));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex);
			}
			ServerEvent.SendWarMemoryFail(GetClientPeers());
			int nDuration = m_warMemory.exitDelayTime * 1000;
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
				ServerEvent.SendWarMemoryBanished(hero.account.peer, hero.previousContinentId, hero.previousNationId, hero.hp);
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
			logWork.AddSqlCommand(GameLogDac.CSC_AddWarMemoryDisqualificationLog(m_instanceId, hero.id, DateTimeUtil.currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void Finish(int nStatus)
	{
		ClearTransformationObject();
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeWaveStartDelayTimeTimer();
		m_endTime = DateTimeUtil.currentTime;
		if (m_nStatus == 2)
		{
			m_nPlayTime = (int)Math.Floor((m_endTime - m_startTime).TotalSeconds);
		}
		m_nStatus = nStatus;
	}

	protected override void ReleaseInternal()
	{
		ClearTransformationObject();
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeWaveStartDelayTimeTimer();
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

	private void DisposeWaveStartDelayTimeTimer()
	{
		if (m_waveStartDelayTimeTimer != null)
		{
			m_waveStartDelayTimeTimer.Dispose();
			m_waveStartDelayTimeTimer = null;
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
