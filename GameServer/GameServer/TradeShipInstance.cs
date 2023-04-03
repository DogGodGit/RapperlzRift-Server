using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class TradeShipInstance : Place
{
	public class TradeShipMonsterRegenTimerState
	{
		public Timer timer;

		public TradeShipMonsterArrange arrange;
	}

	public class TradeShipAdditionalMonsterRegenTimerState
	{
		public Timer timer;

		public TradeShipAdditionalMonsterArrange arrange;
	}

	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kLogStatus_Clear = 1;

	public const int kLogStatus_Fail = 2;

	private TradeShip m_tradeShip;

	private TradeShipDifficulty m_difficulty;

	private TradeShipSchedule m_schedule;

	private int m_nStatus;

	private int m_nStepNo;

	private DateTimeOffset m_creationTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private HashSet<TradeShipObjectInstance> m_objectInsts = new HashSet<TradeShipObjectInstance>();

	private int m_nCurrentStepMonsterKillCount;

	private HashSet<TradeShipMonsterRegenTimerState> m_tradeShipMonsterRegenTimerStates = new HashSet<TradeShipMonsterRegenTimerState>();

	private HashSet<TradeShipAdditionalMonsterRegenTimerState> m_tradeShipAdditionalMonsterRegenTimerStates = new HashSet<TradeShipAdditionalMonsterRegenTimerState>();

	private Dictionary<Guid, HeroTradeShipPoint> m_heroPoints = new Dictionary<Guid, HeroTradeShipPoint>();

	private Dictionary<Guid, long> m_receivedRewardExps = new Dictionary<Guid, long>();

	private HashSet<Guid> m_additionalRewardReceivedHeroes = new HashSet<Guid>();

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.TradeShip;

	public override Location location => m_tradeShip;

	public override int locationParam => m_difficulty.difficulty;

	public override Rect3D mapRect => m_tradeShip.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => false;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public TradeShip tradeShip => m_tradeShip;

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

	public void Init(TradeShipDifficulty difficulty, TradeShipSchedule schedule, DateTimeOffset time)
	{
		if (difficulty == null)
		{
			throw new ArgumentNullException("difficulty");
		}
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_tradeShip = difficulty.tradeShip;
		m_difficulty = difficulty;
		m_schedule = schedule;
		m_creationTime = time;
		InitPlace();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddTradeShipCreationLog(m_instanceId, m_difficulty.difficulty, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		m_nStatus = 1;
		int nDuration = m_tradeShip.startDelayTime * 1000;
		m_playWaitingTimer = new Timer(OnPlayWaitingTimerTick);
		m_playWaitingTimer.Change(nDuration, -1);
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		hero.StopTradeShipMoneyBuff(bSendEvent: true);
		Disqualification(hero);
	}

	public float GetRemainingStartTime(DateTimeOffset time)
	{
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fStartTime = (float)m_creationTime.TimeOfDay.TotalSeconds + (float)m_tradeShip.startDelayTime;
		return Math.Max(fStartTime - fTime, 0f);
	}

	public float GetRemainingLimitTime(DateTimeOffset time)
	{
		if (m_nStatus == 1)
		{
			return m_tradeShip.limitTime;
		}
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fEndTime = (float)m_startTime.TimeOfDay.TotalSeconds + (float)m_tradeShip.limitTime;
		return Math.Max(fEndTime - fTime, 0f);
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
			int nDuration = m_tradeShip.limitTime * 1000;
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
		m_nCurrentStepMonsterKillCount = 0;
		TradeShipDifficultyStep difficultyStep = m_difficulty.GetStep(m_nStepNo);
		List<PDTradeShipMonsterInstance> monsterInsts = new List<PDTradeShipMonsterInstance>();
		List<PDTradeShipAdditionalMonsterInstance> additionalMonsterInsts = new List<PDTradeShipAdditionalMonsterInstance>();
		List<PDTradeShipObjectInstance> objectInsts = new List<PDTradeShipObjectInstance>();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (TradeShipMonsterArrange monsterArrange in difficultyStep.monsterArranges)
		{
			TradeShipMonsterInstance monsterInst = CreateMonster(monsterArrange);
			AddMonster(monsterInst);
			monsterInsts.Add((PDTradeShipMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
		}
		TradeShipAdditionalMonsterArrangePoolEntry additionalMonsterArrangePoolEntry = difficultyStep.SelectAdditionalMonsterArrangePoolEntry();
		if (additionalMonsterArrangePoolEntry != null)
		{
			foreach (TradeShipAdditionalMonsterArrange additionalMonsterArrange in additionalMonsterArrangePoolEntry.monsterArranges)
			{
				TradeShipAdditionalMonsterInstance additionalMonsterInst = CreateAdditionalMonster(additionalMonsterArrange);
				AddMonster(additionalMonsterInst);
				additionalMonsterInsts.Add((PDTradeShipAdditionalMonsterInstance)additionalMonsterInst.ToPDMonsterInstance(currentTime));
			}
		}
		foreach (TradeShipObject obj in m_difficulty.objs.Values)
		{
			if (obj.activationStepNo == m_nStepNo)
			{
				TradeShipObjectInstance objInst = CreateObject(obj);
				AddMonster(objInst);
				m_objectInsts.Add(objInst);
				objectInsts.Add((PDTradeShipObjectInstance)objInst.ToPDMonsterInstance(currentTime));
			}
		}
		ServerEvent.SendTradeShipStepStart(GetClientPeers(), m_nStepNo, monsterInsts.ToArray(), additionalMonsterInsts.ToArray(), objectInsts.ToArray());
	}

	private TradeShipMonsterInstance CreateMonster(TradeShipMonsterArrange monsterArrange)
	{
		TradeShipMonsterInstance inst = new TradeShipMonsterInstance();
		inst.Init(this, monsterArrange);
		return inst;
	}

	private TradeShipAdditionalMonsterInstance CreateAdditionalMonster(TradeShipAdditionalMonsterArrange additionalMonsterArrange)
	{
		TradeShipAdditionalMonsterInstance inst = new TradeShipAdditionalMonsterInstance();
		inst.Init(this, additionalMonsterArrange);
		return inst;
	}

	private TradeShipObjectInstance CreateObject(TradeShipObject obj)
	{
		TradeShipObjectInstance inst = new TradeShipObjectInstance();
		inst.Init(this, obj);
		return inst;
	}

	protected override void OnMonsterDead(MonsterInstance monsterInst)
	{
		base.OnMonsterDead(monsterInst);
		if (m_nStatus != 2)
		{
			return;
		}
		MonsterInstanceType monsterInstanceType = monsterInst.monsterInstanceType;
		DateTimeOffset lastDamageTime = monsterInst.lastDamageTime;
		switch (monsterInstanceType)
		{
		case MonsterInstanceType.TradeShipMonster:
		case MonsterInstanceType.TradeShipAdditionalMonster:
		{
			if (!(monsterInst.lastAttacker is Hero killer))
			{
				break;
			}
			Hero heroKiller = GetHero(killer.id);
			if (heroKiller != null)
			{
				HeroTradeShipPoint heroPoint = GetOrCreateHeroPoint(heroKiller);
				if (monsterInstanceType == MonsterInstanceType.TradeShipMonster)
				{
					heroPoint.AddPoint(((TradeShipMonsterInstance)monsterInst).arrange.point, lastDamageTime);
				}
				else
				{
					heroPoint.AddPoint(((TradeShipAdditionalMonsterInstance)monsterInst).arrange.point, lastDamageTime);
				}
				ServerEvent.SendTradeShipPointAcquisition(heroKiller.account.peer, heroPoint.point);
			}
			break;
		}
		case MonsterInstanceType.TradeShipObject:
		{
			MonsterReceivedDamage[] sortedReceivedDamages = monsterInst.sortedReceivedDamages;
			foreach (MonsterReceivedDamage damage in sortedReceivedDamages)
			{
				Hero hero = GetHero(damage.attackerId);
				if (hero == null)
				{
					continue;
				}
				TradeShipObjectInstance objInst = (TradeShipObjectInstance)monsterInst;
				lock (hero.syncObject)
				{
					TradeShipObject obj = objInst.obj;
					HeroTradeShipPoint heroPoint2 = GetOrCreateHeroPoint(hero);
					heroPoint2.AddPoint(obj.point, lastDamageTime);
					List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
					PDItemBooty itemBooty = null;
					Mail mail = null;
					ItemReward itemReward = obj.SelectDestroyerReward()?.itemReward;
					if (itemReward == null)
					{
						continue;
					}
					if (itemReward.count > 0)
					{
						Item item = itemReward.item;
						int nCount = itemReward.count;
						bool bOwned = itemReward.owned;
						int nRewardItemRemainCount = hero.AddItem(item, bOwned, nCount, changedInventorySlots);
						itemBooty = new PDItemBooty();
						itemBooty.id = item.id;
						itemBooty.count = nCount;
						itemBooty.owned = bOwned;
						if (nRewardItemRemainCount > 0)
						{
							mail = Mail.Create("MAIL_REWARD_N_35", "MAIL_REWARD_D_35", m_endTime);
							mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainCount, bOwned));
							hero.AddMail(mail, bSendEvent: true);
						}
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
					ServerEvent.SendTradeShipObjectDestructionReward(hero.account.peer, heroPoint2.point, itemBooty, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
				}
			}
			break;
		}
		}
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus != 2)
		{
			return;
		}
		switch (monsterInst.monsterInstanceType)
		{
		case MonsterInstanceType.TradeShipMonster:
		{
			TradeShipMonsterInstance tradeShipMonsterInst = (TradeShipMonsterInstance)monsterInst;
			OnMonsterRemove_TradeShipMonster(tradeShipMonsterInst);
			if (tradeShipMonsterInst.arrange.step.no == m_nStepNo)
			{
				m_nCurrentStepMonsterKillCount++;
			}
			break;
		}
		case MonsterInstanceType.TradeShipAdditionalMonster:
		{
			TradeShipAdditionalMonsterInstance tradeShipAdditionalMonsterInst = (TradeShipAdditionalMonsterInstance)monsterInst;
			OnMonsterRemove_TradeShipAdditionalMonster(tradeShipAdditionalMonsterInst);
			if (tradeShipAdditionalMonsterInst.arrange.entry.step.no == m_nStepNo)
			{
				m_nCurrentStepMonsterKillCount++;
			}
			break;
		}
		case MonsterInstanceType.TradeShipObject:
			m_objectInsts.Remove((TradeShipObjectInstance)monsterInst);
			break;
		}
		TradeShipStep step = m_tradeShip.GetStep(m_nStepNo);
		if (m_nCurrentStepMonsterKillCount >= step.targetMonsterKillCount && m_objectInsts.Count == 0)
		{
			if (m_nStepNo >= m_tradeShip.stepCount)
			{
				Clear();
			}
			else
			{
				StartNextStep();
			}
		}
	}

	private void OnMonsterRemove_TradeShipMonster(TradeShipMonsterInstance monsterInst)
	{
		TradeShipMonsterArrange arrange = monsterInst.arrange;
		int nRegenDelayTime = m_tradeShip.monsterRegenTime * 1000;
		TradeShipMonsterRegenTimerState state = new TradeShipMonsterRegenTimerState();
		state.arrange = arrange;
		state.timer = new Timer(OnTradeShipMonsterRegenTimerTick, state, -1, -1);
		state.timer.Change(nRegenDelayTime, -1);
		m_tradeShipMonsterRegenTimerStates.Add(state);
	}

	private void OnTradeShipMonsterRegenTimerTick(object state)
	{
		AddWork(new SFAction<TradeShipMonsterRegenTimerState>(RegenTradeShipMonster, (TradeShipMonsterRegenTimerState)state), bGlobalLockRequired: false);
	}

	private void RegenTradeShipMonster(TradeShipMonsterRegenTimerState state)
	{
		Timer timer = state.timer;
		timer.Dispose();
		m_tradeShipMonsterRegenTimerStates.Remove(state);
		TradeShipMonsterInstance monsterInst = CreateMonster(state.arrange);
		SpawnMonster(monsterInst, DateTimeUtil.currentTime);
	}

	private void OnMonsterRemove_TradeShipAdditionalMonster(TradeShipAdditionalMonsterInstance monsterInst)
	{
		TradeShipAdditionalMonsterArrange arrange = monsterInst.arrange;
		int nRegenDelayTime = m_tradeShip.monsterRegenTime * 1000;
		TradeShipAdditionalMonsterRegenTimerState state = new TradeShipAdditionalMonsterRegenTimerState();
		state.arrange = arrange;
		state.timer = new Timer(OnTradeShipAdditionalMonsterRegenTimerTick, state, -1, -1);
		state.timer.Change(nRegenDelayTime, -1);
		m_tradeShipAdditionalMonsterRegenTimerStates.Add(state);
	}

	private void OnTradeShipAdditionalMonsterRegenTimerTick(object state)
	{
		AddWork(new SFAction<TradeShipAdditionalMonsterRegenTimerState>(RegenTradeShipAdditionalMonster, (TradeShipAdditionalMonsterRegenTimerState)state), bGlobalLockRequired: false);
	}

	private void RegenTradeShipAdditionalMonster(TradeShipAdditionalMonsterRegenTimerState state)
	{
		Timer timer = state.timer;
		timer.Dispose();
		m_tradeShipAdditionalMonsterRegenTimerStates.Remove(state);
		TradeShipAdditionalMonsterInstance monsterInst = CreateAdditionalMonster(state.arrange);
		SpawnMonster(monsterInst, DateTimeUtil.currentTime);
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
			logWork2.AddSqlCommand(GameLogDac.CSC_AddTradeShipCompletionLog(m_instanceId, 1, m_endTime));
			logWork2.Schedule();
		}
		catch (Exception ex2)
		{
			SFLogUtil.Error(GetType(), null, ex2);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_TradeShip);
		int nRemainingTime = m_tradeShip.limitTime - (int)DateTimeUtil.GetTimeSpanSeconds(m_startTime, m_endTime);
		int nClearPoint = nRemainingTime * m_tradeShip.clearPointPerRemainTime;
		HeroTradeShipPoint bestHeroPoint = null;
		foreach (Hero hero in m_heroes.Values)
		{
			HeroTradeShipPoint heroPoint = GetOrCreateHeroPoint(hero);
			heroPoint.AddPoint(nClearPoint, m_endTime);
			bool bMyBestRecordChanged = hero.RefreshTradeShipBestRecord(heroPoint);
			if (heroPoint.CompareTo(bestHeroPoint) > 0)
			{
				bestHeroPoint = heroPoint;
			}
			List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
			PDItemBooty itemBooty = null;
			Mail mail = null;
			ItemReward itemReward = m_difficulty.SelectRewardPoolEntry()?.itemReward;
			if (itemReward != null && itemReward.count > 0)
			{
				Item item = itemReward.item;
				int nCount = itemReward.count;
				bool bOwned = itemReward.owned;
				int nRewardItemRemainCount = hero.AddItem(item, bOwned, nCount, changedInventorySlots);
				itemBooty = new PDItemBooty();
				itemBooty.id = item.id;
				itemBooty.count = nCount;
				itemBooty.owned = bOwned;
				if (nRewardItemRemainCount > 0)
				{
					mail = Mail.Create("MAIL_REWARD_N_35", "MAIL_REWARD_D_35", m_endTime);
					mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainCount, bOwned));
					hero.AddMail(mail, bSendEvent: true);
				}
			}
			long lnAcquisitionExp = m_difficulty.expReward?.value ?? 0;
			ExpReward pointExpReward = m_difficulty.pointExpReward;
			long lnPointExpReward = Math.Min((pointExpReward != null) ? (pointExpReward.value * heroPoint.point) : 0, m_difficulty.maxAdditionalExp);
			lnAcquisitionExp += lnPointExpReward;
			m_receivedRewardExps.Add(hero.id, lnAcquisitionExp);
			if (lnAcquisitionExp > 0)
			{
				lnAcquisitionExp = (long)Math.Floor((float)lnAcquisitionExp * Cache.instance.GetWorldLevelExpFactor(hero.level));
				hero.AddExp(lnAcquisitionExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
			}
			long lnAcquisitionGold = m_difficulty.goldReward?.value ?? 0;
			GoldReward pointGoldReward = m_difficulty.pointGoldReward;
			lnAcquisitionGold += ((pointGoldReward != null) ? (pointGoldReward.value * heroPoint.point) : 0);
			hero.AddGold(lnAcquisitionGold);
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(hero.id));
			foreach (InventorySlot slot in changedInventorySlots)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
			}
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(hero));
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(hero));
			if (mail != null)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(mail));
			}
			if (bMyBestRecordChanged)
			{
				dbWork.AddSqlCommand(GameDac.CSC_AddorUpdateHeroTradeShipBestRecord(hero.id, heroPoint.difficulty, heroPoint.point, heroPoint.updateTime));
			}
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				logWork.AddSqlCommand(GameLogDac.CSC_AddTradeShipCompletionMemberRewardLog(m_instanceId, hero.id, lnAcquisitionGold, lnAcquisitionExp, itemBooty?.id ?? 0, itemBooty?.owned ?? false, itemBooty?.count ?? 0));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex);
			}
			ServerEvent.SendTradeShipClear(hero.account.peer, heroPoint.point, lnAcquisitionExp, hero.level, hero.exp, hero.realMaxHP, hero.hp, hero.gold, hero.maxGold, itemBooty, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
		}
		Cache.instance.RefreshTradeShipServerBestRecord(bestHeroPoint);
		dbWork.Schedule();
		int nDuration = m_tradeShip.exitDelayTime * 1000;
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
		try
		{
			SFSqlStandaloneWork logWork2 = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork2.AddSqlCommand(GameLogDac.CSC_AddTradeShipCompletionLog(m_instanceId, 2, m_endTime));
			logWork2.Schedule();
		}
		catch (Exception ex2)
		{
			SFLogUtil.Error(GetType(), null, ex2);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_TradeShip);
		foreach (Hero hero in m_heroes.Values)
		{
			HeroTradeShipPoint heroPoint = GetOrCreateHeroPoint(hero);
			long lnAcquisitionExp = m_difficulty.expReward?.value ?? 0;
			ExpReward pointExpReward = m_difficulty.pointExpReward;
			long lnPointExpReward = Math.Min((pointExpReward != null) ? (pointExpReward.value * heroPoint.point) : 0, m_difficulty.maxAdditionalExp);
			lnAcquisitionExp += lnPointExpReward;
			m_receivedRewardExps.Add(hero.id, lnAcquisitionExp);
			if (lnAcquisitionExp > 0)
			{
				lnAcquisitionExp = (long)Math.Floor((float)lnAcquisitionExp * Cache.instance.GetWorldLevelExpFactor(hero.level));
				hero.AddExp(lnAcquisitionExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
			}
			long lnAcquisitionGold = m_difficulty.goldReward?.value ?? 0;
			GoldReward pointGoldReward = m_difficulty.pointGoldReward;
			lnAcquisitionGold += ((pointGoldReward != null) ? (pointGoldReward.value * heroPoint.point) : 0);
			hero.AddGold(lnAcquisitionGold);
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(hero.id));
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(hero));
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(hero));
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				logWork.AddSqlCommand(GameLogDac.CSC_AddTradeShipCompletionMemberRewardLog(m_instanceId, hero.id, lnAcquisitionGold, lnAcquisitionExp, 0, bRewardItemOwned: false, 0));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex);
			}
			ServerEvent.SendTradeShipFail(hero.account.peer, lnAcquisitionExp, hero.level, hero.exp, hero.realMaxHP, hero.hp, hero.gold, hero.maxGold);
		}
		dbWork.Schedule();
		int nDuration = m_tradeShip.exitDelayTime * 1000;
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
				ServerEvent.SendTradeShipBanished(hero.account.peer, hero.previousContinentId, hero.previousNationId, hero.hp);
			}
		}
		Close();
	}

	private void Disqualification(Hero hero)
	{
		if (isFinished)
		{
			return;
		}
		RemoveHeroPoint(hero.id);
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddTradeShipMemberDisqualificationLog(m_instanceId, hero.id, DateTimeUtil.currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void Finish(int nStatus)
	{
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		foreach (TradeShipMonsterRegenTimerState state2 in m_tradeShipMonsterRegenTimerStates)
		{
			state2.timer.Dispose();
		}
		m_tradeShipMonsterRegenTimerStates.Clear();
		foreach (TradeShipAdditionalMonsterRegenTimerState state in m_tradeShipAdditionalMonsterRegenTimerStates)
		{
			state.timer.Dispose();
		}
		m_tradeShipAdditionalMonsterRegenTimerStates.Clear();
		m_endTime = DateTimeUtil.currentTime;
		if (m_nStatus == 2)
		{
			m_nPlayTime = (int)Math.Floor((m_endTime - m_startTime).TotalSeconds);
		}
		m_nStatus = nStatus;
	}

	private HeroTradeShipPoint GetHeroPoint(Guid heroId)
	{
		if (!m_heroPoints.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	public HeroTradeShipPoint GetOrCreateHeroPoint(Hero hero)
	{
		HeroTradeShipPoint heroPoint = GetHeroPoint(hero.id);
		if (heroPoint == null)
		{
			heroPoint = new HeroTradeShipPoint();
			heroPoint.Init(hero, m_difficulty);
			m_heroPoints.Add(hero.id, heroPoint);
		}
		return heroPoint;
	}

	private void RemoveHeroPoint(Guid heroId)
	{
		m_heroPoints.Remove(heroId);
	}

	public long GetReceivedRewardExp(Guid heroId)
	{
		if (!m_receivedRewardExps.TryGetValue(heroId, out var value))
		{
			return 0L;
		}
		return value;
	}

	public void AddAdditionalRewardReceivedHero(Guid heroId)
	{
		m_additionalRewardReceivedHeroes.Add(heroId);
	}

	public bool ContainsAdditionalRewardReceivedHero(Guid heroId)
	{
		return m_additionalRewardReceivedHeroes.Contains(heroId);
	}

	protected override void ReleaseInternal()
	{
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
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

	public void DisposeExitDelayTimeTimer()
	{
		if (m_exitDelayTimeTimer != null)
		{
			m_exitDelayTimeTimer.Dispose();
			m_exitDelayTimeTimer = null;
		}
	}
}
