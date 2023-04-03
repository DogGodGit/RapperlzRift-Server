using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class GoldDungeonInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kStatus_Disqualification = 5;

	private GoldDungeonDifficulty m_difficulty;

	private Hero m_hero;

	private int m_nStatus;

	private int m_nStepNo;

	private int m_nWaveNo;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_waveLimitTimeTimer;

	private Timer m_waveIntervalTimer;

	private Timer m_exitDelayTimeTimer;

	private Guid m_logId = Guid.Empty;

	public override PlaceType placeType => PlaceType.GoldDungeon;

	public override Location location => m_difficulty.goldDungeon;

	public override int locationParam => m_difficulty.difficulty;

	public override Rect3D mapRect => goldDungeon.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public GoldDungeon goldDungeon => m_difficulty.goldDungeon;

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

	public int stepNo => m_nStepNo;

	public int waveNo => m_nWaveNo;

	public void Init(GoldDungeonDifficulty difficulty)
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
		Close();
	}

	public void Start(DateTimeOffset time)
	{
		m_nStatus = 1;
		m_hero.RefreshDailyGoldDungeonPlayCount(time.Date);
		DateValuePair<int> dailyGoldDungeonPlayCount = m_hero.dailyGoldDungeonPlayCount;
		dailyGoldDungeonPlayCount.value++;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateGoldDungeonPlay(m_hero.id, time.Date, dailyGoldDungeonPlayCount.value));
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			m_logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddGoldDungeonPlayLog(m_logId, m_instanceId, m_hero.id, m_difficulty.difficulty, 0, 0L, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
		int nDuration = goldDungeon.startDelayTime * 1000;
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
			int nDuration = goldDungeon.limitTime * 1000;
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
		GoldDungeonStep step = m_difficulty.GetStep(m_nStepNo);
		List<PDGoldDungeonMonsterInstance> monsterInsts = new List<PDGoldDungeonMonsterInstance>();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (GoldDungeonStepMonsterArrange arrange in step.monsterArranges)
		{
			if (!arrange.isFugitive)
			{
				for (int i = 0; i < arrange.monsterCount; i++)
				{
					GoldDungeonMonsterInstance monsterInst = CreateMonster(arrange);
					AddMonster(monsterInst);
					monsterInsts.Add((PDGoldDungeonMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
				}
			}
		}
		ServerEvent.SendGoldDungeonStepStart(m_hero.account.peer, m_nStepNo, monsterInsts.ToArray());
		StartNextWave();
	}

	private GoldDungeonMonsterInstance CreateMonster(GoldDungeonStepMonsterArrange arrange)
	{
		GoldDungeonMonsterInstance monsterInst = new GoldDungeonMonsterInstance();
		monsterInst.Init(this, arrange);
		return monsterInst;
	}

	private void StartNextWave()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		m_nWaveNo++;
		GoldDungeonStepWave wave = m_difficulty.GetStep(m_nStepNo).GetWave(m_nWaveNo);
		int nWaveLimitTime = wave.limitTime * 1000;
		if (nWaveLimitTime > 0)
		{
			m_waveLimitTimeTimer = new Timer(OnWaveLimitTimeTimerTick, wave, -1, -1);
			m_waveLimitTimeTimer.Change(nWaveLimitTime, -1);
		}
		foreach (MonsterInstance monsterInst in m_monsterInsts.Values)
		{
			if (monsterInst.visibilityRange <= 0f)
			{
				continue;
			}
			foreach (Sector sector in GetInterestSectors(monsterInst.sector))
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
		ServerEvent.SendGoldDungeonWaveStart(m_hero.account.peer, m_nWaveNo);
	}

	private void OnWaveLimitTimeTimerTick(object state)
	{
		AddWork(new SFAction<GoldDungeonStepWave>(WaveLimitTimeout, (GoldDungeonStepWave)state), bGlobalLockRequired: false);
	}

	private void WaveLimitTimeout(GoldDungeonStepWave wave)
	{
		if (m_nStatus != 2)
		{
			return;
		}
		int nStepNo = wave.step.no;
		if (m_nStepNo != nStepNo)
		{
			return;
		}
		int nWaveNo = wave.no;
		if (m_nWaveNo != nWaveNo)
		{
			return;
		}
		bool bRemoveMonster = false;
		foreach (MonsterInstance monsterInst2 in m_monsterInsts.Values)
		{
			GoldDungeonMonsterInstance goldDungeonMonsterInst = (GoldDungeonMonsterInstance)monsterInst2;
			if (goldDungeonMonsterInst.stepNo == nStepNo && goldDungeonMonsterInst.activationWaveNo == nWaveNo)
			{
				bRemoveMonster = true;
				break;
			}
		}
		if (!bRemoveMonster)
		{
			return;
		}
		foreach (MonsterInstance monsterInst in m_monsterInsts.Values.ToList())
		{
			RemoveMonster(monsterInst, bSendEvent: false);
		}
		ServerEvent.SendGoldDungeonWaveTimeout(m_hero.account.peer);
		CheckIntersection();
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus != 2 || !monsterInst.isDead)
		{
			return;
		}
		bool bIsWaveClear = true;
		foreach (MonsterInstance monsterInstance in m_monsterInsts.Values)
		{
			GoldDungeonMonsterInstance goldDungeonMonsterInst = (GoldDungeonMonsterInstance)monsterInstance;
			if (goldDungeonMonsterInst.stepNo == m_nStepNo && goldDungeonMonsterInst.activationWaveNo == m_nWaveNo)
			{
				bIsWaveClear = false;
				break;
			}
		}
		if (bIsWaveClear)
		{
			ServerEvent.SendGoldDungeonWaveCompleted(m_hero.account.peer);
			CheckIntersection();
		}
	}

	private void CheckIntersection()
	{
		GoldDungeonStep step = m_difficulty.GetStep(m_nStepNo);
		if (m_nWaveNo >= step.waveCount)
		{
			GoldReward goldReward = step.goldReward;
			long lnRewardGold = 0L;
			if (goldReward != null)
			{
				lnRewardGold = goldReward.value;
				m_hero.AddGold(lnRewardGold);
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
				dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_hero.id, m_hero.gold));
				dbWork.Schedule();
			}
			ServerEvent.SendGoldDungeonStepCompleted(m_hero.account.peer, lnRewardGold, m_hero.gold, m_hero.maxGold);
			if (m_nStepNo >= m_difficulty.stepCount)
			{
				Clear();
				return;
			}
			int nDuration2 = step.GetWave(m_nWaveNo).nextWaveIntervalTime * 1000;
			m_waveIntervalTimer = new Timer(OnWaveIntervalTimerTick_Step);
			m_waveIntervalTimer.Change(nDuration2, -1);
		}
		else
		{
			int nDuration = step.GetWave(m_nWaveNo).nextWaveIntervalTime * 1000;
			m_waveIntervalTimer = new Timer(OnWaveIntervalTimerTick_Wave);
			m_waveIntervalTimer.Change(nDuration, -1);
		}
	}

	private void OnWaveIntervalTimerTick_Step(object state)
	{
		AddWork(new SFAction(StartNextStep), bGlobalLockRequired: false);
	}

	private void OnWaveIntervalTimerTick_Wave(object state)
	{
		AddWork(new SFAction(StartNextWave), bGlobalLockRequired: false);
	}

	private void Clear()
	{
		if (m_nStatus == 2)
		{
			Finish(3);
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			int nDifficulty = m_difficulty.difficulty;
			if (!m_hero.IsClearedGoldDungeonDifficulty(nDifficulty))
			{
				m_hero.AddGoldDungeonClearDifficulty(nDifficulty);
				dbWork.AddSqlCommand(GameDac.CSC_AddGoldDungeonClear(m_hero.id, nDifficulty));
			}
			long lnRewardGold = m_difficulty.goldReward.value;
			m_hero.AddGold(lnRewardGold);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_Gold(m_hero.id, m_hero.gold));
			dbWork.Schedule();
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				logWork.AddSqlCommand(GameLogDac.CSC_UpdateGoldDungeonPlayLog(m_logId, 1, lnRewardGold));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
			}
			ServerEvent.SendGoldDungeonClear(m_hero.account.peer, lnRewardGold, m_hero.gold, m_hero.maxGold);
			int nDuration = goldDungeon.exitDelayTime * 1000;
			m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
			m_exitDelayTimeTimer.Change(nDuration, -1);
		}
	}

	private void Fail()
	{
		if (m_nStatus == 2)
		{
			Finish(4);
			ServerEvent.SendGoldDungeonFail(m_hero.account.peer);
			int nDuration = goldDungeon.exitDelayTime * 1000;
			m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
			m_exitDelayTimeTimer.Change(nDuration, -1);
		}
	}

	private void OnExitDelayTimeTimerTick(object state)
	{
		AddWork(new SFAction(DungeonBanished), bGlobalLockRequired: true);
	}

	private void DungeonBanished()
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
			ServerEvent.SendGoldDungeonBanished(m_hero.account.peer, m_hero.previousContinentId, m_hero.previousNationId, m_hero.hp);
		}
	}

	public void Finish(int nStatus)
	{
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeWaveLimitTimeTimer();
		DisposeWaveIntervalTimer();
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
		DisposeWaveLimitTimeTimer();
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

	private void DisposeWaveLimitTimeTimer()
	{
		if (m_waveLimitTimeTimer != null)
		{
			m_waveLimitTimeTimer.Dispose();
			m_waveLimitTimeTimer = null;
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
