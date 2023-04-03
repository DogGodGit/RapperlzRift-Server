using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class FearAltarInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kLogStatus_Clear = 1;

	public const int kLogStatus_Fail = 2;

	private FearAltar m_fearAltar;

	private FearAltarStage m_stage;

	private int m_nAverageHeroLevel;

	private int m_nStatus;

	private int m_nWaveNo;

	private DateTimeOffset m_creationTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private int m_nTargetArrangeMonsterCount;

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.FearAltar;

	public override Location location => m_stage;

	public override int locationParam => m_stage.id;

	public override Rect3D mapRect => m_stage.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public FearAltar fearAltar => m_fearAltar;

	public FearAltarStage stage => m_stage;

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

	public DateTimeOffset creationTime => m_creationTime;

	public int waveNo => m_nWaveNo;

	public void Init(FearAltarStage stage, DateTimeOffset time, int nAverageHeroLevel)
	{
		if (stage == null)
		{
			throw new ArgumentNullException("stage");
		}
		m_fearAltar = stage.fearAltar;
		m_stage = stage;
		m_nAverageHeroLevel = nAverageHeroLevel;
		m_creationTime = time;
		InitPlace();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddFearAltarCreationLog(m_instanceId, m_stage.id, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		m_nStatus = 1;
		int nDuration = m_fearAltar.startDelayTime * 1000;
		m_playWaitingTimer = new Timer(OnPlayWaitingTimerTick);
		m_playWaitingTimer.Change(nDuration, -1);
	}

	public float GetRemainingStartTime(DateTimeOffset time)
	{
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fStartTime = (float)m_creationTime.TimeOfDay.TotalSeconds + (float)m_fearAltar.startDelayTime;
		return Math.Max(fStartTime - fTime, 0f);
	}

	public float GetRemainingLimitTime(DateTimeOffset time)
	{
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fEndTime = (float)m_startTime.TimeOfDay.TotalSeconds + (float)m_fearAltar.limitTime;
		return Math.Max(fEndTime - fTime, 0f);
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		Disqualification(hero);
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
			int nDuration = m_fearAltar.limitTime * 1000;
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
		if (m_nStatus != 2)
		{
			return;
		}
		m_nWaveNo++;
		FearAltarStageWave wave = m_stage.GetWave(m_nWaveNo);
		List<PDFearAltarMonsterInstance> monsterInsts = new List<PDFearAltarMonsterInstance>();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		FearAltarMonsterAttrFactor monsterAttrFactor = m_fearAltar.GetMonsterAttrFactor(m_nAverageHeroLevel);
		foreach (FearAltarStageWaveMonsterArrange arrange in wave.monsterArranges)
		{
			int nMonsterCount = arrange.monsterCount;
			for (int i = 0; i < nMonsterCount; i++)
			{
				FearAltarMonsterInstance monsterInst2 = CreateMonster(arrange, monsterAttrFactor);
				AddMonster(monsterInst2);
				monsterInsts.Add((PDFearAltarMonsterInstance)monsterInst2.ToPDMonsterInstance(currentTime));
			}
			if (wave.type == 2 && wave.targetArrangeKey == arrange.key)
			{
				m_nTargetArrangeMonsterCount = nMonsterCount;
			}
		}
		PDFearAltarHalidomMonsterInstance halidomMonsterInst = null;
		if (Util.DrawLots(wave.halidomMonsterSpawnRate))
		{
			FearAltarHalidomMonsterInstance monsterInst = CreateHalidomMonster(m_fearAltar.SelectHalidom(), wave, monsterAttrFactor);
			AddMonster(monsterInst);
			halidomMonsterInst = (PDFearAltarHalidomMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime);
		}
		ServerEvent.SendFearAltarWaveStart(GetClientPeers(), m_nWaveNo, monsterInsts.ToArray(), halidomMonsterInst);
	}

	private FearAltarMonsterInstance CreateMonster(FearAltarStageWaveMonsterArrange arrange, FearAltarMonsterAttrFactor attrFactor)
	{
		FearAltarMonsterInstance inst = new FearAltarMonsterInstance();
		inst.Init(this, arrange, attrFactor);
		return inst;
	}

	private FearAltarHalidomMonsterInstance CreateHalidomMonster(FearAltarHalidom halidom, FearAltarStageWave wave, FearAltarMonsterAttrFactor attrFactor)
	{
		FearAltarHalidomMonsterInstance inst = new FearAltarHalidomMonsterInstance();
		inst.Init(this, halidom, wave, attrFactor);
		return inst;
	}

	public void OnExpireHalidomMonsterLifetime(FearAltarHalidomMonsterInstance monsterInst)
	{
		if (monsterInst == null)
		{
			throw new ArgumentNullException("monsterInst");
		}
		RemoveMonster(monsterInst, bSendEvent: true);
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus != 2)
		{
			return;
		}
		FearAltarStageWave wave = m_stage.GetWave(m_nWaveNo);
		if (wave.type == 1)
		{
			if (m_monsterInsts.Count > 0)
			{
				return;
			}
		}
		else
		{
			if (!(monsterInst is FearAltarMonsterInstance fearAltarMonsterInst) || fearAltarMonsterInst.arrange.key != wave.targetArrangeKey)
			{
				return;
			}
			m_nTargetArrangeMonsterCount--;
			if (m_nTargetArrangeMonsterCount > 0)
			{
				return;
			}
		}
		CompleteWave();
	}

	private void CompleteWave()
	{
		MonsterInstance[] array = m_monsterInsts.Values.ToArray();
		foreach (MonsterInstance monsterInst in array)
		{
			RemoveMonster(monsterInst, bSendEvent: true);
		}
		if (m_nWaveNo >= m_stage.waveCount)
		{
			Clear();
		}
		else
		{
			StartNextWave();
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
			logWork2.AddSqlCommand(GameLogDac.CSC_AddFearAltarCompletionLog(m_instanceId, 1, m_endTime));
			logWork2.Schedule();
		}
		catch (Exception ex2)
		{
			SFLogUtil.Error(GetType(), null, ex2);
		}
		List<PDFearAltarHero> clearedHeroes = new List<PDFearAltarHero>();
		foreach (Hero hero2 in m_heroes.Values)
		{
			clearedHeroes.Add(hero2.ToPDFearAltarHero());
		}
		foreach (Hero hero in m_heroes.Values)
		{
			lock (hero.syncObject)
			{
				long lnAcquisitionExp = (m_fearAltar.GetReward(hero.level)?.expReward)?.value ?? 0;
				if (lnAcquisitionExp > 0)
				{
					lnAcquisitionExp = (long)Math.Floor((float)lnAcquisitionExp * Cache.instance.GetWorldLevelExpFactor(hero.level));
					hero.AddExp(lnAcquisitionExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
				}
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(hero.id);
				dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(hero));
				dbWork.Schedule();
				try
				{
					SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
					logWork.AddSqlCommand(GameLogDac.CSC_AddFearAltarCompletionMemberLog(m_instanceId, hero.id, lnAcquisitionExp));
					logWork.Schedule();
				}
				catch (Exception ex)
				{
					SFLogUtil.Error(GetType(), null, ex);
				}
				ServerEvent.SendFearAltarClear(hero.account.peer, clearedHeroes.ToArray(), lnAcquisitionExp, hero.level, hero.exp, hero.realMaxHP, hero.hp);
			}
		}
		int nDuration = m_fearAltar.exitDelayTime * 1000;
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
				logWork.AddSqlCommand(GameLogDac.CSC_AddFearAltarCompletionLog(m_instanceId, 2, m_endTime));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex);
			}
			ServerEvent.SendFearAltarFail(GetClientPeers());
			int nDuration = m_fearAltar.exitDelayTime * 1000;
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
				ServerEvent.SendFearAltarBanished(hero.account.peer, hero.previousContinentId, hero.previousNationId, hero.hp);
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
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddFearAltarDisqualificationLog(m_instanceId, hero.id, DateTimeUtil.currentTime));
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

	public void DisposeExitDelayTimeTimer()
	{
		if (m_exitDelayTimeTimer != null)
		{
			m_exitDelayTimeTimer.Dispose();
			m_exitDelayTimeTimer = null;
		}
	}
}
