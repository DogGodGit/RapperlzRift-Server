using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class AncientRelicInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kLogStatus_Start = 0;

	public const int kLogStatus_Clear = 1;

	public const int kLogStatus_Fail = 2;

	private AncientRelic m_ancientRelic;

	private int m_nAverageHeroLevel;

	private int m_nRouteId;

	private int m_nStatus;

	private int m_nStepNo;

	private int m_nWaveNo;

	private DateTimeOffset m_creationTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private int m_nMonsterPoint;

	private List<AncientRelicTrapInstance> m_trapInsts = new List<AncientRelicTrapInstance>();

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_waveIntervalTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.AncientRelic;

	public override Location location => m_ancientRelic;

	public override int locationParam => 0;

	public override Rect3D mapRect => m_ancientRelic.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public AncientRelic ancientRelic => m_ancientRelic;

	public int status => m_nStatus;

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

	public int stepNo => m_nStepNo;

	public int waveNo => m_nWaveNo;

	public int averageHeroLevel => m_nAverageHeroLevel;

	public int routeId => m_nRouteId;

	public void Init(AncientRelic ancientRelic, DateTimeOffset time, int nAverageHeroLevel)
	{
		if (ancientRelic == null)
		{
			throw new ArgumentNullException("ancientRelic");
		}
		m_ancientRelic = ancientRelic;
		m_nAverageHeroLevel = nAverageHeroLevel;
		m_nRouteId = ancientRelic.SelectRouteId();
		m_creationTime = time;
		InitPlace();
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_AncientRelic);
		dbWork.AddSqlCommand(GameDac.CSC_AddAncientRelicInstance(m_instanceId, nAverageHeroLevel, 0, 0, time));
		dbWork.Schedule();
		m_nStatus = 1;
		foreach (AncientRelicTrap trap in m_ancientRelic.traps)
		{
			AncientRelicTrapInstance trapInst = CreateTrap(trap);
			m_trapInsts.Add(trapInst);
		}
		int nDuration = ancientRelic.startDelayTime * 1000;
		m_playWaitingTimer = new Timer(OnPlayWaitingTimerTick);
		m_playWaitingTimer.Change(nDuration, -1);
	}

	public float GetRemainingStartTime(DateTimeOffset time)
	{
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fStartTime = (float)m_creationTime.TimeOfDay.TotalSeconds + (float)m_ancientRelic.startDelayTime;
		return Math.Max(fStartTime - fTime, 0f);
	}

	public float GetRemainingLimitTime(DateTimeOffset time)
	{
		if (m_nStatus == 1)
		{
			return m_ancientRelic.limitTime;
		}
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fEndTime = (float)m_startTime.TimeOfDay.TotalSeconds + (float)m_ancientRelic.limitTime;
		return Math.Max(fEndTime - fTime, 0f);
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		hero.StopAncientRelicTrapAbnormalStateEffect();
		Disqualification(hero);
	}

	private AncientRelicTrapInstance CreateTrap(AncientRelicTrap trap)
	{
		AncientRelicTrapInstance inst = new AncientRelicTrapInstance();
		inst.Init(trap, this, m_ancientRelic.GetMonsterAttrFactor(m_nAverageHeroLevel).trapDamageFactor);
		return inst;
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
			int nDuration = m_ancientRelic.limitTime * 1000;
			m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
			m_limitTimeTimer.Change(nDuration, -1);
			StartNextStep();
		}
	}

	private void OnLimitTimeTimerTick(object state)
	{
		AddWork(new SFAction(Fail), bGlobalLockRequired: false);
	}

	private void StartNextStep()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		m_nStepNo++;
		m_nWaveNo = 0;
		m_nMonsterPoint = 0;
		if (m_ancientRelic.trapActivateStartStep == m_nStepNo)
		{
			DateTimeOffset currentTime = DateTimeUtil.currentTime;
			foreach (AncientRelicTrapInstance trapInst in m_trapInsts)
			{
				trapInst.Start(currentTime);
			}
		}
		AncientRelicStepRoute stepRoute = m_ancientRelic.GetStep(m_nStepNo).GetRoute(m_nRouteId);
		ServerEvent.SendAncientRelicStepStart(GetClientPeers(), m_nStepNo, stepRoute.targetPosition, stepRoute.targetRadius, stepRoute.removeObstacleId);
		StartNextWave();
	}

	private void StartNextWave()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		m_nWaveNo++;
		AncientRelicStepWave wave = m_ancientRelic.GetStep(m_nStepNo).GetWave(m_nWaveNo);
		List<PDAncientRelicMonsterInstance> monsterInsts = new List<PDAncientRelicMonsterInstance>();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (AncientRelicMonsterArrange arrange in wave.arranges)
		{
			int nArrangeRouteId = arrange.routeId;
			if (nArrangeRouteId == 0 || nArrangeRouteId == m_nRouteId)
			{
				for (int i = 0; i < arrange.monsterCount; i++)
				{
					AncientRelicMonsterInstance monsterInst = CreateMonster(arrange);
					AddMonster(monsterInst);
					monsterInsts.Add((PDAncientRelicMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
				}
			}
		}
		ServerEvent.SendAncientRelicWaveStart(GetClientPeers(), m_nWaveNo, monsterInsts.ToArray());
	}

	private AncientRelicMonsterInstance CreateMonster(AncientRelicMonsterArrange arrange)
	{
		AncientRelicMonsterInstance monsterInst = null;
		monsterInst = ((arrange.type != 1) ? ((AncientRelicMonsterInstance)new AncientRelicBossMonsterInstance()) : ((AncientRelicMonsterInstance)new AncientRelicNormalMonsterinstance()));
		monsterInst.Init(this, arrange);
		return monsterInst;
	}

	public List<Guid> GetTrapEffectHeroes()
	{
		List<Guid> results = new List<Guid>();
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.isAncientRelicTrapEffect)
			{
				results.Add(hero.id);
			}
		}
		return results;
	}

	protected override void OnHeroPositionChanged(Hero hero, SectorChangeInfo info, bool bSendInterestTargetChangeEvent, DateTimeOffset currentTime)
	{
		base.OnHeroPositionChanged(hero, info, bSendInterestTargetChangeEvent, currentTime);
		if (m_ancientRelic.trapActivateStartStep < m_nStepNo)
		{
			return;
		}
		foreach (AncientRelicTrapInstance trapInst in m_trapInsts)
		{
			if (trapInst.status == 2 && trapInst.InTrapHitPosition(hero.position))
			{
				hero.HitAncientRelicTrap(trapInst.trapDamage, trapInst.trapPenaltyMoveSpeed, trapInst.duration, currentTime);
			}
		}
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus != 2 || !monsterInst.isDead)
		{
			return;
		}
		AncientRelicStep step = m_ancientRelic.GetStep(m_nStepNo);
		if (step.type == 1)
		{
			if (m_monsterInsts.Count == 0)
			{
				CompleteWave();
			}
			return;
		}
		AncientRelicMonsterInstance ancientRelicMonsterInst = (AncientRelicMonsterInstance)monsterInst;
		int nAcquisitionPoint = ancientRelicMonsterInst.point;
		m_nMonsterPoint += nAcquisitionPoint;
		if (nAcquisitionPoint > 0)
		{
			ServerEvent.SendAncientRelicPointUpdated(GetClientPeers(), m_nMonsterPoint);
		}
		if (m_nMonsterPoint >= step.targetPoint)
		{
			CompleteStep();
		}
		else if (m_monsterInsts.Count == 0)
		{
			CompleteWave();
		}
	}

	private void CompleteWave()
	{
		AncientRelicStep step = m_ancientRelic.GetStep(m_nStepNo);
		if (m_nWaveNo >= step.waveCount)
		{
			CompleteStep();
			return;
		}
		int nDuration = m_ancientRelic.waveIntervalTime * 1000;
		m_waveIntervalTimer = new Timer(OnWaveIntervalTimerTick_Wave);
		m_waveIntervalTimer.Change(nDuration, -1);
	}

	private void OnWaveIntervalTimerTick_Wave(object state)
	{
		AddWork(new SFAction(StartNextWave), bGlobalLockRequired: false);
	}

	private void CompleteStep()
	{
		AncientRelicStep step = m_ancientRelic.GetStep(m_nStepNo);
		foreach (MonsterInstance monsterInst in m_monsterInsts.Values.ToList())
		{
			RemoveMonster(monsterInst, bSendEvent: true);
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_AncientRelic);
		foreach (Hero hero in m_heroes.Values)
		{
			lock (hero.syncObject)
			{
				List<PDItemBooty> booties = new List<PDItemBooty>();
				List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
				Mail mail = null;
				AncientRelicStepRewardPoolCollection collection = step.GetRewardPoolCollection(hero.level);
				if (collection != null)
				{
					for (int i = 0; i < collection.poolCount; i++)
					{
						ItemReward itemReward = collection.GetPool(i + 1).SelectItemReward();
						if (itemReward == null || itemReward.count <= 0)
						{
							continue;
						}
						Item item = itemReward.item;
						int nCount = itemReward.count;
						bool bOwned = itemReward.owned;
						int nRewardItemRemainCount = hero.AddItem(item, bOwned, nCount, changedInventorySlots);
						PDItemBooty itemBooty = new PDItemBooty();
						itemBooty.id = item.id;
						itemBooty.count = nCount;
						itemBooty.owned = bOwned;
						booties.Add(itemBooty);
						if (nRewardItemRemainCount > 0)
						{
							if (mail == null)
							{
								mail = Mail.Create("MAIL_REWARD_N_4", "MAIL_REWARD_D_4", m_endTime);
							}
							mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainCount, bOwned));
						}
					}
				}
				dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(hero.id));
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
					Guid logId = Guid.NewGuid();
					logWork.AddSqlCommand(GameLogDac.CSC_AddAncientRelicRewardLog(logId, hero.id, m_instanceId, m_nStepNo, currentTime));
					foreach (PDItemBooty booty in booties)
					{
						logWork.AddSqlCommand(GameLogDac.CSC_AddAncientRelicRwardDetailLog(Guid.NewGuid(), logId, booty.id, booty.count, booty.owned));
					}
					logWork.Schedule();
				}
				catch (Exception ex)
				{
					SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
				}
				ServerEvent.SendAncientRelicStepCompleted(hero.account.peer, booties.ToArray(), InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
			}
		}
		dbWork.Schedule();
		if (m_nStepNo >= m_ancientRelic.stepCount)
		{
			Clear();
			return;
		}
		int nDuration = m_ancientRelic.waveIntervalTime * 1000;
		m_waveIntervalTimer = new Timer(OnWaveIntervalTimerTick_Step);
		m_waveIntervalTimer.Change(nDuration, -1);
	}

	private void OnWaveIntervalTimerTick_Step(object state)
	{
		AddWork(new SFAction(StartNextStep), bGlobalLockRequired: false);
	}

	private void Clear()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		Finish(3);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_AncientRelic);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateAncientRelicInstance(m_instanceId, 1, m_nPlayTime, m_endTime));
		foreach (Hero hero in m_heroes.Values)
		{
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(hero.id));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateAncientRelicInstanceMember(m_instanceId, hero.id, 1, m_endTime));
		}
		dbWork.Schedule();
		ServerEvent.SendAncientRelicClear(GetClientPeers());
		int nDuration = m_ancientRelic.exitDelayTime * 1000;
		m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
		m_exitDelayTimeTimer.Change(nDuration, -1);
	}

	private void Fail()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		Finish(4);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_AncientRelic);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateAncientRelicInstance(m_instanceId, 2, m_nPlayTime, m_endTime));
		foreach (Hero hero in m_heroes.Values)
		{
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(hero.id));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateAncientRelicInstanceMember(m_instanceId, hero.id, 2, m_endTime));
		}
		dbWork.Schedule();
		ServerEvent.SendAncientRelicFail(GetClientPeers());
		int nDuration = m_ancientRelic.exitDelayTime * 1000;
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
		foreach (Hero hero in m_heroes.Values.ToList())
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
				ServerEvent.SendAncientRelicBanished(hero.account.peer, hero.previousContinentId, hero.previousNationId, hero.hp);
			}
		}
		Close();
	}

	public void Disqualification(Hero hero)
	{
		if (!isFinished)
		{
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_AncientRelic);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateAncientRelicInstanceMember(m_instanceId, hero.id, 3, DateTimeUtil.currentTime));
			dbWork.Schedule();
		}
	}

	private void Finish(int nStatus)
	{
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeWaveIntervalTimer();
		DisposeExitDelayTimeTimer();
		m_endTime = DateTimeUtil.currentTime;
		if (m_nStatus == 2)
		{
			m_nPlayTime = (int)Math.Floor((m_endTime - m_startTime).TotalSeconds);
		}
		m_nStatus = nStatus;
		foreach (AncientRelicTrapInstance trapInst in m_trapInsts)
		{
			trapInst.Release();
		}
		m_trapInsts.Clear();
	}

	protected override void ReleaseInternal()
	{
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeWaveIntervalTimer();
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

	private void DisposeWaveIntervalTimer()
	{
		if (m_waveIntervalTimer != null)
		{
			m_waveIntervalTimer.Dispose();
			m_waveIntervalTimer = null;
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
