using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class MainQuestDungeonInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kStatus_Disqualification = 5;

	private MainQuestDungeon m_mainQuestDungeon;

	private Hero m_hero;

	private int m_nStatus;

	private int m_nStepNo;

	private int m_nClearStepNo;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_directionTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.MainQuestDungeon;

	public override Location location => m_mainQuestDungeon;

	public override int locationParam => 0;

	public override Rect3D mapRect => m_mainQuestDungeon.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public MainQuestDungeon mainQuestDungeon => m_mainQuestDungeon;

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

	public void Init(MainQuestDungeon mainQuestDungeon)
	{
		if (mainQuestDungeon == null)
		{
			throw new ArgumentNullException("mainQuestDungeon");
		}
		m_mainQuestDungeon = mainQuestDungeon;
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
		Close();
	}

	public void Start()
	{
		m_nStatus = 1;
		int nDuration = m_mainQuestDungeon.startDelayTime * 1000;
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
			int nLimitTime = m_mainQuestDungeon.limitTime * 1000;
			m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
			m_limitTimeTimer.Change(nLimitTime, -1);
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
		MainQuestDungeonStep step = m_mainQuestDungeon.GetStep(m_nStepNo);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		List<PDMainQuestDungeonMonsterInstance> monsterInsts = new List<PDMainQuestDungeonMonsterInstance>();
		if (step.type == 3)
		{
			ChangeHeroPositionAndRotation(m_hero, step.SelectPosition(), step.directingStartYRotation, bSendInterestTargetChangeEvent: true, currentTime);
			int nDuration = step.directingDuration * 1000;
			m_directionTimer = new Timer(OnDirectionTimerTick);
			m_directionTimer.Change(nDuration, -1);
		}
		else
		{
			foreach (MainQuestDungeonMonsterArrange arrange in step.monsterArranges)
			{
				for (int i = 0; i < arrange.monsterCount; i++)
				{
					MainQuestDungeonMonsterInstance monsterInst = CreateMonster(arrange);
					AddMonster(monsterInst);
					monsterInsts.Add((PDMainQuestDungeonMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
				}
			}
		}
		ServerEvent.SendMainQuestDungeonStepStart(m_hero.account.peer, m_nStepNo, monsterInsts.ToArray(), m_hero.position, m_hero.rotationY);
	}

	private MainQuestDungeonMonsterInstance CreateMonster(MainQuestDungeonMonsterArrange arrange)
	{
		MainQuestDungeonMonsterInstance monsterInst = new MainQuestDungeonMonsterInstance();
		monsterInst.Init(this, arrange);
		return monsterInst;
	}

	private void OnDirectionTimerTick(object state)
	{
		AddWork(new SFAction(CompleteDirection), bGlobalLockRequired: false);
	}

	private void CompleteDirection()
	{
		if (m_nStatus == 2)
		{
			MainQuestDungeonStep step = m_mainQuestDungeon.GetStep(m_nStepNo);
			CompleteStep(step, DateTimeUtil.currentTime);
		}
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus != 2 || m_nStepNo == m_nClearStepNo)
		{
			return;
		}
		MainQuestDungeonStep step = m_mainQuestDungeon.GetStep(m_nStepNo);
		if (step.type != 2)
		{
			return;
		}
		if (step.targetMonsterArrangeNo == 0)
		{
			if (base.monsterInsts.Count == 0)
			{
				CompleteStep(step, monsterInst.lastDamageTime);
			}
		}
		else if (monsterInst is MainQuestDungeonMonsterInstance mainQuestDungeonMosnterInst && step.targetMonsterArrangeNo == mainQuestDungeonMosnterInst.arrange.no)
		{
			CompleteStep(step, monsterInst.lastDamageTime);
		}
	}

	protected override void OnHeroPositionChanged(Hero hero, SectorChangeInfo info, bool bSendInterestTargetChangeEvent, DateTimeOffset currentTime)
	{
		base.OnHeroPositionChanged(hero, info, bSendInterestTargetChangeEvent, currentTime);
		if (m_nStatus == 2)
		{
			MainQuestDungeonStep step = m_mainQuestDungeon.GetStep(m_nStepNo);
			if (step.type == 1 && step.IsInTargetPosition(hero.position))
			{
				CompleteStep(step, currentTime);
			}
		}
	}

	private void CompleteStep(MainQuestDungeonStep step, DateTimeOffset time)
	{
		m_nClearStepNo = step.no;
		MonsterInstance[] array = m_monsterInsts.Values.ToArray();
		foreach (MonsterInstance monsterInst in array)
		{
			RemoveMonster(monsterInst, bSendEvent: true);
		}
		int nDungeonId = m_mainQuestDungeon.id;
		HeroMainQuestDungeonReward reward = m_hero.GetHeroMainQuestDungeonReward(nDungeonId, m_nStepNo);
		long lnRewardExp = 0L;
		long lnGold = 0L;
		if (reward == null)
		{
			ExpReward expReward = step.expReward;
			GoldReward goldReward = step.goldReward;
			if (expReward != null)
			{
				lnRewardExp = expReward.value;
				lnRewardExp = (long)Math.Floor((float)lnRewardExp * Cache.instance.GetWorldLevelExpFactor(m_hero.level));
				m_hero.AddExp(lnRewardExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
			}
			if (goldReward != null)
			{
				lnGold = goldReward.value;
				m_hero.AddGold(lnGold);
			}
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_hero));
			if (lnGold > 0)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_hero));
			}
			dbWork.AddSqlCommand(GameDac.CSC_AddHeroMainQuestDungeonReward(m_hero.id, m_mainQuestDungeon.id, m_nStepNo, time));
			dbWork.Schedule();
			reward = new HeroMainQuestDungeonReward();
			reward.Init(nDungeonId, m_nStepNo);
			m_hero.AddHeroMainQuestDungeonReward(reward);
		}
		ServerEvent.SendMainQuestDungeonStepCompleted(m_hero.account.peer, m_nStepNo, m_hero.level, m_hero.exp, m_hero.realMaxHP, m_hero.hp, m_hero.gold, m_hero.maxGold, lnRewardExp);
		if (m_nStepNo >= m_mainQuestDungeon.stepCount)
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
		if (m_nStatus == 2)
		{
			Finish(3);
			HeroMainQuest heroMainQuest = m_hero.currentHeroMainQuest;
			heroMainQuest.IncreaseProgressCount();
			ServerEvent.SendMainQuestDungeonClear(m_hero.account.peer);
			int nDuration = m_mainQuestDungeon.exitDelayTime * 1000;
			m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
			m_exitDelayTimeTimer.Change(nDuration, -1);
		}
	}

	private void Fail()
	{
		if (m_nStatus == 2)
		{
			Finish(4);
			ServerEvent.SendMainQuestDungeonFail(m_hero.account.peer);
			int nDuration = m_mainQuestDungeon.exitDelayTime * 1000;
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
			if (m_nStatus == 3 && m_mainQuestDungeon.completionExitPositionEnabled)
			{
				m_hero.SetPreviousPositionAndRotation(m_mainQuestDungeon.completionExitPosition, m_mainQuestDungeon.completionExitYRotation);
			}
			Exit(m_hero, isLogOut: false, null);
			ServerEvent.SendMainQuestDungeonBanished(m_hero.account.peer, m_hero.previousContinentId, m_hero.previousNationId, m_hero.hp);
		}
	}

	public void Finish(int nStatus)
	{
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeExitDelayTimeTimer();
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

	public void SummonBySummonerMonster(MainQuestDungeonMonsterInstance summoner, DateTimeOffset time)
	{
		List<PDMainQuestDungeonSummonMonsterInstance> monsterInsts = new List<PDMainQuestDungeonSummonMonsterInstance>();
		foreach (MainQuestDungeonSummon summon in summoner.arrange.summons)
		{
			for (int i = 0; i < summon.monsterCount; i++)
			{
				MainQuestDungeonSummonMonsterInstance summonMonsterInst = SummonMonster(summon);
				AddMonster(summonMonsterInst);
				monsterInsts.Add((PDMainQuestDungeonSummonMonsterInstance)summonMonsterInst.ToPDMonsterInstance(time));
			}
		}
		ServerEvent.SendMainQuestDungeonMonsterSummon(m_hero.account.peer, summoner.instanceId, monsterInsts.ToArray());
	}

	private MainQuestDungeonSummonMonsterInstance SummonMonster(MainQuestDungeonSummon summon)
	{
		MainQuestDungeonSummonMonsterInstance monsterInst = new MainQuestDungeonSummonMonsterInstance();
		monsterInst.Init(this, summon);
		return monsterInst;
	}
}
