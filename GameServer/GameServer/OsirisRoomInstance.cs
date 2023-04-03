using System;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class OsirisRoomInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kStatus_Disqualification = 5;

	private OsirisRoom m_osirisRoom;

	private OsirisRoomDifficulty m_difficulty;

	private Hero m_hero;

	private int m_nStatus;

	private int m_nWaveNo;

	private int m_nSpawnMonsterArrangeNo;

	private long m_lnTotalRewardGold;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_monsterSpawnIntervalTimer;

	private Timer m_waveIntervalTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.OsirisRoom;

	public override Location location => m_osirisRoom;

	public override int locationParam => m_difficulty.difficulty;

	public override Rect3D mapRect => m_osirisRoom.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public OsirisRoom osirisRoom => m_osirisRoom;

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

	public int waveNo => waveNo;

	public void Init(OsirisRoomDifficulty difficulty)
	{
		if (difficulty == null)
		{
			throw new ArgumentNullException("difficulty");
		}
		m_osirisRoom = difficulty.osirisRoom;
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
		hero.StopOsirisRoomMoneyBuff(bSendEvent: true);
		Disqualification();
		Close();
	}

	public void Start(DateTimeOffset time)
	{
		m_nStatus = 1;
		m_hero.RefreshDailyOsirisRoomPlayCount(time.Date);
		DateValuePair<int> dailyOsirisRoomPlayCount = m_hero.dailyOsirisRoomPlayCount;
		dailyOsirisRoomPlayCount.value++;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_OsirisRoomPlay(m_hero.id, dailyOsirisRoomPlayCount.date, dailyOsirisRoomPlayCount.value));
		dbWork.Schedule();
		int nDuration = m_osirisRoom.startDelayTime * 1000;
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
			int nDuration = m_osirisRoom.limitTime * 1000;
			m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
			m_limitTimeTimer.Change(nDuration, -1);
			StartNextWave();
		}
	}

	private void OnLimitTimeTimerTick(object state)
	{
		AddWork(new SFAction(Fail), bGlobalLockRequired: false);
	}

	private void StartNextWave()
	{
		if (m_nStatus == 2)
		{
			m_nWaveNo++;
			m_nSpawnMonsterArrangeNo = 0;
			OsirisRoomDifficultyWave wave = m_difficulty.GetWave(m_nWaveNo);
			ServerEvent.SendOsirisRoomWaveStart(m_hero.account.peer, m_nWaveNo, wave.monsterArrangeCount);
			SpawnMonster();
		}
	}

	private void SpawnMonster()
	{
		m_nSpawnMonsterArrangeNo++;
		OsirisRoomDifficultyWave wave = m_difficulty.GetWave(m_nWaveNo);
		OsirisRoomMonsterArrange arrange = wave.GetMonsterArrange(m_nSpawnMonsterArrangeNo);
		OsirisRoomMonsterInstance monsterInst = new OsirisRoomMonsterInstance();
		monsterInst.Init(this, arrange);
		AddMonster(monsterInst);
		ServerEvent.SendOsirisRoomMonsterSpawn(m_hero.account.peer, (PDOsirisRoomMonsterInstance)monsterInst.ToPDMonsterInstance(DateTimeUtil.currentTime));
		if (m_nSpawnMonsterArrangeNo < wave.monsterArrangeCount)
		{
			int nDuration2 = m_osirisRoom.monsterSpawnInterval * 1000;
			if (m_monsterSpawnIntervalTimer == null)
			{
				m_monsterSpawnIntervalTimer = new Timer(OnMonsterSpawnIntervalTimerTick);
			}
			m_monsterSpawnIntervalTimer.Change(nDuration2, -1);
		}
		else if (m_nWaveNo < m_difficulty.waveCount)
		{
			int nDuration = m_osirisRoom.waveInterval * 1000;
			if (m_waveIntervalTimer == null)
			{
				m_waveIntervalTimer = new Timer(OnWaveIntervalTimerTick);
			}
			m_waveIntervalTimer.Change(nDuration, -1);
		}
	}

	private void OnMonsterSpawnIntervalTimerTick(object state)
	{
		AddWork(new SFAction(SpawnMonster), bGlobalLockRequired: false);
	}

	private void OnWaveIntervalTimerTick(object state)
	{
		AddWork(new SFAction(StartNextWave), bGlobalLockRequired: false);
	}

	protected override void OnMonsterPositionChanged(MonsterInstance monsterInst, SectorChangeInfo info, DateTimeOffset currentTime)
	{
		base.OnMonsterPositionChanged(monsterInst, info, currentTime);
		if (m_osirisRoom.ContainsTargetPosition(monsterInst.position))
		{
			RemoveMonster(monsterInst, bSendEvent: true);
		}
	}

	protected override void OnMonsterDead(MonsterInstance monsterInst)
	{
		base.OnMonsterDead(monsterInst);
		if (m_nStatus == 2)
		{
			OsirisRoomMonsterInstance osirisRoomMonsterInst = (OsirisRoomMonsterInstance)monsterInst;
			long lnAcquisitionGold = osirisRoomMonsterInst.arrange.killGoldReward?.value ?? 0;
			if (lnAcquisitionGold > 0)
			{
				m_lnTotalRewardGold += lnAcquisitionGold;
				m_hero.AddGold(lnAcquisitionGold);
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
				dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(m_hero));
				dbWork.Schedule();
				ServerEvent.SendOsirisRoomRewardGoldAcquisition(m_hero.account.peer, m_hero.gold, m_hero.maxGold);
			}
		}
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus == 2 && m_nWaveNo >= m_difficulty.waveCount)
		{
			OsirisRoomDifficultyWave wave = m_difficulty.GetWave(m_nWaveNo);
			if (m_nSpawnMonsterArrangeNo >= wave.monsterArrangeCount && m_monsterInsts.Count <= 0)
			{
				Clear();
			}
		}
	}

	private void Clear()
	{
		if (m_nStatus == 2)
		{
			Finish(3);
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroOsirisRoomRewardLog(Guid.NewGuid(), m_hero.id, m_difficulty.difficulty, m_nPlayTime, m_lnTotalRewardGold, m_endTime));
			logWork.Schedule();
			ServerEvent.SendOsirisRoomClear(m_hero.account.peer);
			int nDuration = m_osirisRoom.exitDelayTime * 1000;
			m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
			m_exitDelayTimeTimer.Change(nDuration, -1);
		}
	}

	private void Fail()
	{
		if (m_nStatus == 2)
		{
			Finish(4);
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroOsirisRoomRewardLog(Guid.NewGuid(), m_hero.id, m_difficulty.difficulty, m_nPlayTime, m_lnTotalRewardGold, m_endTime));
			logWork.Schedule();
			ServerEvent.SendOsirisRoomFail(m_hero.account.peer);
			int nDuration = m_osirisRoom.exitDelayTime * 1000;
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
			ServerEvent.SendOsirisRoomBanished(m_hero.account.peer, m_hero.previousContinentId, m_hero.previousNationId, m_hero.hp);
		}
	}

	private void Disqualification()
	{
		if (!isFinished)
		{
			Finish(5);
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroOsirisRoomRewardLog(Guid.NewGuid(), m_hero.id, m_difficulty.difficulty, m_nPlayTime, m_lnTotalRewardGold, m_endTime));
			logWork.Schedule();
		}
	}

	private void Finish(int nStatus)
	{
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeMonsterSpawnIntervalTimer();
		DisposeWaveIntervalTimer();
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
		DisposeMonsterSpawnIntervalTimer();
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

	private void DisposeMonsterSpawnIntervalTimer()
	{
		if (m_monsterSpawnIntervalTimer != null)
		{
			m_monsterSpawnIntervalTimer.Dispose();
			m_monsterSpawnIntervalTimer = null;
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
