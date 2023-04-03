using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ExpDungeonInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Disqualification = 4;

	private ExpDungeonDifficulty m_difficulty;

	private Hero m_hero;

	private int m_nStatus;

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

	public override PlaceType placeType => PlaceType.ExpDungeon;

	public override Location location => m_difficulty.expDungeon;

	public override int locationParam => m_difficulty.difficulty;

	public override Rect3D mapRect => expDungeon.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public ExpDungeon expDungeon => m_difficulty.expDungeon;

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

	public void Init(ExpDungeonDifficulty difficulty)
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
		m_hero.RefreshDailyExpDungeonPlayCount(time.Date);
		DateValuePair<int> dailyExpDungeonPlayCount = m_hero.dailyExpDungeonPlayCount;
		dailyExpDungeonPlayCount.value++;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateExpDungeonPlay(m_hero.id, time.Date, dailyExpDungeonPlayCount.value));
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			m_logId = Guid.NewGuid();
			logWork.AddSqlCommand(GameLogDac.CSC_AddExpDungeonPlayLog(m_logId, m_instanceId, m_hero.id, m_difficulty.difficulty, 0, 0L, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
		int nDuration = expDungeon.startDelayTime * 1000;
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
			int nDuration = expDungeon.limitTime * 1000;
			m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
			m_limitTimeTimer.Change(nDuration, -1);
			StartNextWave();
		}
	}

	private void OnLimitTimeTimerTick(object state)
	{
		AddWork(new SFAction(Clear), bGlobalLockRequired: false);
	}

	private void StartNextWave()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		m_nWaveNo++;
		ExpDungeonDifficultyWave wave = m_difficulty.GetWave(m_nWaveNo);
		int nWaveLimitTime = wave.waveLimitTime * 1000;
		m_waveLimitTimeTimer = new Timer(OnWaveLimitTimeTimerTick, m_nWaveNo, -1, -1);
		m_waveLimitTimeTimer.Change(nWaveLimitTime, -1);
		List<PDExpDungeonMonsterInstance> monsterInsts = new List<PDExpDungeonMonsterInstance>();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (ExpDungeonMonsterArrange arrange in wave.monsterArranges)
		{
			for (int i = 0; i < arrange.monsterCount; i++)
			{
				ExpDungeonMonsterInstance monsterInst2 = CreateMonster(arrange);
				AddMonster(monsterInst2);
				monsterInsts.Add((PDExpDungeonMonsterInstance)monsterInst2.ToPDMonsterInstance(currentTime));
			}
		}
		PDExpDungeonLakChargeMonsterInstance lakChargeMonsterInst = null;
		if (Util.DrawLots(wave.lakChargeMonsterRate))
		{
			ExpDungeonLakChargeMonsterInstance monsterInst = CreateLakChargeMonster(wave);
			AddMonster(monsterInst);
			lakChargeMonsterInst = (PDExpDungeonLakChargeMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime);
		}
		ServerEvent.SendExpDungeonWaveStart(m_hero.account.peer, m_nWaveNo, monsterInsts.ToArray(), lakChargeMonsterInst);
	}

	private ExpDungeonMonsterInstance CreateMonster(ExpDungeonMonsterArrange monsterArrange)
	{
		ExpDungeonMonsterInstance monsterInst = new ExpDungeonMonsterInstance();
		monsterInst.Init(this, monsterArrange);
		return monsterInst;
	}

	private ExpDungeonLakChargeMonsterInstance CreateLakChargeMonster(ExpDungeonDifficultyWave wave)
	{
		ExpDungeonLakChargeMonsterInstance monsterInst = new ExpDungeonLakChargeMonsterInstance();
		monsterInst.Init(this, wave);
		return monsterInst;
	}

	private void OnWaveLimitTimeTimerTick(object state)
	{
		AddWork(new SFAction<int>(WaveLimitTimeout, (int)state), bGlobalLockRequired: false);
	}

	private void WaveLimitTimeout(int nWaveNo)
	{
		if (m_nStatus != 2 || m_nWaveNo != nWaveNo || m_monsterInsts.Count == 0)
		{
			return;
		}
		foreach (MonsterInstance monsterInst in m_monsterInsts.Values.ToList())
		{
			RemoveMonster(monsterInst, bSendEvent: false);
		}
		ServerEvent.SendExpDungeonWaveTimeout(m_hero.account.peer);
		if (nWaveNo >= m_difficulty.waveCount)
		{
			Clear();
			return;
		}
		int nWaveInterval = expDungeon.waveIntervalTime * 1000;
		m_waveIntervalTimer = new Timer(OnWaveIntervalTimerTick);
		m_waveIntervalTimer.Change(nWaveInterval, -1);
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus == 2 && monsterInst.isDead && m_monsterInsts.Count == 0)
		{
			CompleteWave();
		}
	}

	private void CompleteWave()
	{
		ServerEvent.SendExpDungeonWaveCompleted(m_hero.account.peer);
		if (m_nWaveNo >= m_difficulty.waveCount)
		{
			Clear();
			return;
		}
		int nWaveInterval = expDungeon.waveIntervalTime * 1000;
		m_waveIntervalTimer = new Timer(OnWaveIntervalTimerTick);
		m_waveIntervalTimer.Change(nWaveInterval, -1);
	}

	private void OnWaveIntervalTimerTick(object state)
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
			if (!m_hero.IsClearedExpDungeonDifficulty(nDifficulty))
			{
				m_hero.AddExpDungeonClearDifficulty(nDifficulty);
				dbWork.AddSqlCommand(GameDac.CSC_AddExpDungeonClear(m_hero.id, nDifficulty));
			}
			ExpReward expReward = m_difficulty.expReward;
			long lnBaseRewardExp = expReward.value;
			long lnExp = lnBaseRewardExp;
			lnExp = (long)Math.Floor((float)lnExp * Cache.instance.GetWorldLevelExpFactor(m_hero.level));
			m_hero.AddExp(lnExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_hero));
			dbWork.Schedule();
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				logWork.AddSqlCommand(GameLogDac.CSC_UpdateExpDungeonPlayLog(m_logId, 1, lnExp));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
			}
			ServerEvent.SendExpDungeonClear(m_hero.account.peer, lnExp, m_hero.level, m_hero.exp, m_hero.realMaxHP, m_hero.hp);
			int nDuration = expDungeon.exitDelayTime * 1000;
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
			ServerEvent.SendExpDungeonBanished(m_hero.account.peer, m_hero.previousContinentId, m_hero.previousNationId, m_hero.hp);
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
