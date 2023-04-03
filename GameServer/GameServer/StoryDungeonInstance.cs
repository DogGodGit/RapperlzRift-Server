using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class StoryDungeonInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kStatus_Disqualification = 5;

	private StoryDungeonDifficulty m_difficulty;

	private Hero m_hero;

	private int m_nStatus;

	private int m_nStepNo;

	private int m_nClearStepNo;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private HashSet<Timer> m_trapCastingTimers = new HashSet<Timer>();

	private HashSet<StoryDungeonTrapEffect> m_trapEffects = new HashSet<StoryDungeonTrapEffect>();

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_exitDelayTimeTimer;

	private Guid m_logId = Guid.Empty;

	public override PlaceType placeType => PlaceType.StoryDungeon;

	public override Location location => m_difficulty.storyDungeon;

	public override int locationParam => m_difficulty.difficulty;

	public override Rect3D mapRect => storyDungeon.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public StoryDungeon storyDungeon => m_difficulty.storyDungeon;

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

	public Guid logId => m_logId;

	public void Init(StoryDungeonDifficulty difficulty)
	{
		if (difficulty == null)
		{
			throw new ArgumentNullException("difficulty");
		}
		m_difficulty = difficulty;
		InitPlace();
	}

	protected override void OnHeroEnter(Hero hero, DateTimeOffset time)
	{
		base.OnHeroEnter(hero, time);
		m_hero = hero;
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		hero.StopMonsterTame();
		Close();
	}

	public void Start(DateTimeOffset time)
	{
		m_nStatus = 1;
		StoryDungeonPlay play = m_hero.GetOrCreateStoryDungeonPlay(storyDungeon.no, time.Date);
		play.enterCount++;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateStoryDungeonPlay(m_hero.id, time.Date, storyDungeon.no, play.enterCount));
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			m_logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddStoryDungeonPlayLog(m_logId, m_instanceId, m_hero.id, storyDungeon.no, m_difficulty.difficulty, 0, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
		int nDuration = storyDungeon.startDelayTime * 1000;
		m_playWaitingTimer = new Timer(OnPlayWaitingTimerTick);
		m_playWaitingTimer.Change(nDuration, -1);
	}

	private void OnPlayWaitingTimerTick(object state)
	{
		AddWork(new SFAction(StartPlay), bGlobalLockRequired: false);
	}

	private void StartPlay()
	{
		if (m_nStatus != 1)
		{
			return;
		}
		m_nStatus = 2;
		m_startTime = DateTimeUtil.currentTime;
		int nLimitTime = storyDungeon.limitTime * 1000;
		m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
		m_limitTimeTimer.Change(nLimitTime, -1);
		foreach (StoryDungeonTrap trap in m_difficulty.traps)
		{
			int nStartDelay = (int)Math.Floor(trap.startDelay * 1000f);
			int nCastingTerm = (int)Math.Floor(trap.castingTerm * 1000f);
			Timer trapCastingTimer = new Timer(OnTrapCastingTimerTick, trap, -1, -1);
			trapCastingTimer.Change(nStartDelay, nCastingTerm);
			m_trapCastingTimers.Add(trapCastingTimer);
		}
		StartNextStep();
	}

	private void OnLimitTimeTimerTick(object state)
	{
		AddWork(new SFAction(Fail), bGlobalLockRequired: false);
	}

	private void OnTrapCastingTimerTick(object state)
	{
		AddWork(new SFAction<StoryDungeonTrap>(CastTrap, (StoryDungeonTrap)state), bGlobalLockRequired: false);
	}

	private void CastTrap(StoryDungeonTrap trap)
	{
		StoryDungeonTrapEffect effect = new StoryDungeonTrapEffect();
		effect.Init(this, trap);
		m_trapEffects.Add(effect);
		ServerEvent.SendStoryDungeonTrapCast(m_hero.account.peer, trap.id);
	}

	private void RemoveTrapEffect(StoryDungeonTrapEffect effect)
	{
		m_trapEffects.Remove(effect);
	}

	private void StartNextStep()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		m_nStepNo++;
		StoryDungeonStep step = m_difficulty.GetStep(m_nStepNo);
		List<PDStoryDungeonMonsterInstance> monsterInsts = new List<PDStoryDungeonMonsterInstance>();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (StoryDungeonMonsterArrange arrange in step.monsterArranges)
		{
			for (int i = 0; i < arrange.monsterCount; i++)
			{
				StoryDungeonMonsterInstance monsterInst = CreateMonster(arrange);
				AddMonster(monsterInst);
				monsterInsts.Add((PDStoryDungeonMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
			}
		}
		ServerEvent.SendStoryDungeonStepStart(m_hero.account.peer, m_nStepNo, monsterInsts.ToArray());
	}

	private StoryDungeonMonsterInstance CreateMonster(StoryDungeonMonsterArrange arrange)
	{
		StoryDungeonMonsterInstance monsterInst = new StoryDungeonMonsterInstance();
		monsterInst.Init(this, arrange);
		return monsterInst;
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus != 2 || m_nStepNo == m_nClearStepNo)
		{
			return;
		}
		StoryDungeonStep step = m_difficulty.GetStep(m_nStepNo);
		if (step.type == 2)
		{
			if (m_monsterInsts.Count == 0)
			{
				CompleteStep();
			}
		}
		else if (step.type == 3)
		{
			StoryDungeonMonsterInstance storyDungeonMonsterInst = (StoryDungeonMonsterInstance)monsterInst;
			if (storyDungeonMonsterInst.monsterType == 2)
			{
				CompleteStep();
			}
		}
	}

	protected override void OnHeroPositionChanged(Hero hero, SectorChangeInfo info, bool bSendInterestTargetChangeEvent, DateTimeOffset currentTime)
	{
		base.OnHeroPositionChanged(hero, info, bSendInterestTargetChangeEvent, currentTime);
		if (m_nStatus == 2)
		{
			StoryDungeonStep step = m_difficulty.GetStep(m_nStepNo);
			if (step.type == 1 && step.IsInTargetPosition(hero.position))
			{
				CompleteStep();
			}
		}
	}

	public void OnTameMonster(StoryDungeonMonsterInstance monsterInst)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		RemoveMonster(monsterInst, bSendEvent: false);
	}

	private void CompleteStep()
	{
		m_nClearStepNo = m_nStepNo;
		StoryDungeonStep step = m_difficulty.GetStep(m_nStepNo);
		if (step.isCompletionRemoveTaming)
		{
			m_hero.StopMonsterTame();
		}
		MonsterInstance[] array = m_monsterInsts.Values.ToArray();
		foreach (MonsterInstance monsterInst in array)
		{
			RemoveMonster(monsterInst, bSendEvent: true);
		}
		if (m_nStepNo >= m_difficulty.stepCount)
		{
			Clear();
		}
		else
		{
			StartNextStep();
		}
	}

	private void Clear()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		Finish(3);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		int nDungeonNo = storyDungeon.no;
		int nDifficulty = m_difficulty.difficulty;
		if (nDifficulty > m_hero.GetStoryDungeonClearMaxDifficulty(nDungeonNo))
		{
			m_hero.SetStoryDungeonClearMaxDifficulty(nDungeonNo, nDifficulty);
			dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateStoryDungeonClear(m_hero.id, nDungeonNo, nDifficulty));
		}
		List<StoryDungeonReward> rewards = m_difficulty.rewards;
		List<PDItemBooty> pdItemBooties = new List<PDItemBooty>();
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		Mail mail = null;
		foreach (StoryDungeonReward reward2 in rewards)
		{
			Item rewardItem = reward2.itemReward.item;
			int nRewardCount = reward2.itemReward.count;
			bool bRewardOwned = reward2.itemReward.owned;
			int nRewardItemRemainCount = m_hero.AddItem(rewardItem, bRewardOwned, nRewardCount, changedInventorySlots);
			PDItemBooty pdItemBooty = new PDItemBooty();
			pdItemBooty.id = rewardItem.id;
			pdItemBooty.count = nRewardCount;
			pdItemBooty.owned = bRewardOwned;
			if (nRewardCount > 0)
			{
				pdItemBooties.Add(pdItemBooty);
			}
			if (nRewardItemRemainCount > 0)
			{
				if (mail == null)
				{
					mail = Mail.Create("MAIL_REWARD_N_5", "MAIL_REWARD_D_5", m_endTime);
				}
				mail.AddAttachmentWithNo(new MailAttachment(rewardItem, nRewardItemRemainCount, bRewardOwned));
			}
		}
		if (mail != null)
		{
			m_hero.AddMail(mail, bSendEvent: true);
		}
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
			logWork.AddSqlCommand(GameLogDac.CSC_UpdateStoryDungeonPlayLog(m_logId, 1));
			int nNo = 1;
			foreach (StoryDungeonReward reward in rewards)
			{
				ItemReward itemReward = reward.itemReward;
				logWork.AddSqlCommand(GameLogDac.CSC_AddStoryDungeonPlayRewardLog(m_logId, nNo++, itemReward.item.id, itemReward.count, itemReward.owned));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
		List<PDInventorySlot> pdChangedInventorySlots = InventorySlot.ToPDInventorySlots(changedInventorySlots);
		ServerEvent.SendStoryDungeonClear(m_hero.account.peer, pdItemBooties.ToArray(), pdChangedInventorySlots.ToArray());
		int nDuration = storyDungeon.exitDelayTime * 1000;
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
				logWork.AddSqlCommand(GameLogDac.CSC_UpdateStoryDungeonPlayLog(m_logId, 2));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
			}
			ServerEvent.SendStoryDungeonFail(m_hero.account.peer);
			int nDuration = storyDungeon.exitDelayTime * 1000;
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
			ServerEvent.SendStoryDungeonBanished(m_hero.account.peer, m_hero.previousContinentId, m_hero.previousNationId, m_hero.hp);
		}
	}

	public void Finish(int nStatus)
	{
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeExitDelayTimeTimer();
		DisposeTrapCastingTimer();
		m_endTime = DateTimeUtil.currentTime;
		if (m_nStatus == 2)
		{
			m_nPlayTime = (int)Math.Floor((m_endTime - m_startTime).TotalSeconds);
		}
		m_nStatus = nStatus;
		foreach (StoryDungeonTrapEffect effect in m_trapEffects)
		{
			effect.Stop();
		}
	}

	public void ProcessTrapEffectHitTick(StoryDungeonTrapEffect effect)
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (Sector sector in GetInterestSectors(GetSectorOfPosition(effect.position)))
		{
			foreach (Hero hero in sector.heroes.Values)
			{
				lock (hero.syncObject)
				{
					if (effect.Contains(hero.position))
					{
						hero.StoryDungeonTrapHit(effect.damage, currentTime);
					}
				}
			}
		}
	}

	public void ProcessTrapEffectFinished(StoryDungeonTrapEffect effect)
	{
		RemoveTrapEffect(effect);
	}

	protected override void ReleaseInternal()
	{
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeExitDelayTimeTimer();
		DisposeTrapCastingTimer();
		m_trapEffects.Clear();
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

	private void DisposeTrapCastingTimer()
	{
		foreach (Timer timer in m_trapCastingTimers)
		{
			timer.Dispose();
		}
		m_trapCastingTimers.Clear();
	}
}
