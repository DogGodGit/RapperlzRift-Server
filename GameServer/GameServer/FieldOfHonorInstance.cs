using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class FieldOfHonorInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	private FieldOfHonor m_fieldOfHonor;

	private Hero m_challenger;

	private Hero m_targetRanker;

	private int m_nTargetRanking;

	private int m_nStatus;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.FieldOfHonor;

	public override Location location => m_fieldOfHonor;

	public override int locationParam => 0;

	public override Rect3D mapRect => m_fieldOfHonor.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public override bool distortionScrollUseEnabled => false;

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

	public Hero targetRanker => m_targetRanker;

	public void Init(Hero challenger, Hero targetRanker, int nTargetRanking, DateTimeOffset time)
	{
		if (challenger == null)
		{
			throw new ArgumentNullException("challenger");
		}
		if (targetRanker == null)
		{
			throw new ArgumentNullException("targetRanker");
		}
		m_fieldOfHonor = Resource.instance.fieldOfHonor;
		InitPlace();
		m_challenger = challenger;
		m_targetRanker = targetRanker;
		m_nTargetRanking = nTargetRanking;
		m_targetRanker.SetPositionAndRotation(m_fieldOfHonor.targetPosition, m_fieldOfHonor.targetYRotation);
		Enter(m_targetRanker, time, bIsRevivalEnter: false);
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		if (hero.id == m_challenger.id)
		{
			if (!isFinished)
			{
				OnFailAsync(bForcedExit: true);
			}
			else
			{
				Close();
			}
		}
	}

	public void Start(DateTimeOffset time)
	{
		m_nStatus = 1;
		m_challenger.RefreshDailyFieldOfHonorPlayCount(time.Date);
		DateValuePair<int> dailyFieldOfHonorPlayCount = m_challenger.dailyFieldOfHonorPlayCount;
		dailyFieldOfHonorPlayCount.value++;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_challenger.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateFieldOfHonorPlay(m_challenger.id, dailyFieldOfHonorPlayCount.date, dailyFieldOfHonorPlayCount.value));
		dbWork.Schedule();
		int nDuration = m_fieldOfHonor.startDelayTime * 1000;
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
			int nDuration = m_fieldOfHonor.limitTime * 1000;
			m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
			m_limitTimeTimer.Change(nDuration, -1);
			ServerEvent.SendFieldOfHonorStart(m_challenger.account.peer);
		}
	}

	private void OnLimitTimeTimerTick(object state)
	{
		AddWork(new SFAction<bool>(Fail, arg: false), bGlobalLockRequired: true);
	}

	public override bool ProcessHeroSkillHit_Hero(OffenseHit offenseHit, int nAttackerLevel, Hero target)
	{
		if (offenseHit == null)
		{
			throw new ArgumentNullException("offenseHit");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		Hero attacker = (Hero)offenseHit.offense.attacker;
		if (attacker.id == target.id)
		{
			return false;
		}
		return target.Hit(offenseHit);
	}

	public override bool IsHeroRankActiveSkillCast_OtherHero(Hero source, Hero target)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (source.id == target.id)
		{
			return false;
		}
		return true;
	}

	protected override void OnHeroDead(Hero hero)
	{
		base.OnHeroDead(hero);
		if (hero.id == m_targetRanker.id)
		{
			OnClearAsync();
		}
		else
		{
			OnFailAsync(bForcedExit: false);
		}
	}

	private void OnClearAsync()
	{
		AddWork(new SFAction(Clear), bGlobalLockRequired: true);
	}

	private void Clear()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		Finish(3);
		lock (m_challenger.syncObject)
		{
			int nHonorPoint = m_fieldOfHonor.winnerHonorPointReward.value;
			m_challenger.AddHonorPoint(nHonorPoint);
			FieldOfHonorLevelReward fieldOfHonorReward = m_fieldOfHonor.GetLevelReward(m_challenger.level);
			long lnAcquisitionExp = 0L;
			if (fieldOfHonorReward != null)
			{
				lnAcquisitionExp = fieldOfHonorReward.expReward.value;
				if (lnAcquisitionExp > 0)
				{
					lnAcquisitionExp = (long)Math.Floor((float)lnAcquisitionExp * Cache.instance.GetWorldLevelExpFactor(m_challenger.level));
					m_challenger.AddExp(lnAcquisitionExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
				}
			}
			m_challenger.fieldOfHonorSuccessiveCount++;
			int nChallengerOldRanking = m_challenger.fieldOfHonorRanking;
			FieldOfHonorHero fieldOfHonorChallenger = m_challenger.ToFieldOfHonorHero();
			FieldOfHonorHero fieldOfHonorTargetHero = Cache.instance.GetFieldOfHonorHero(m_nTargetRanking);
			Cache cache = Cache.instance;
			int nChallengerNewRanking = nChallengerOldRanking;
			int nTargetRankerNewRanking = m_nTargetRanking;
			if (m_nTargetRanking < nChallengerOldRanking)
			{
				nChallengerNewRanking = m_nTargetRanking;
				nTargetRankerNewRanking = nChallengerOldRanking;
			}
			FieldOfHonorHistory challengerHistory = new FieldOfHonorHistory(m_challenger, Guid.NewGuid());
			challengerHistory.type = 1;
			challengerHistory.targetHeroId = m_targetRanker.id;
			challengerHistory.targetName = m_targetRanker.name;
			challengerHistory.oldRanking = nChallengerOldRanking;
			challengerHistory.ranking = nChallengerNewRanking;
			challengerHistory.isWin = true;
			challengerHistory.regTime = m_endTime;
			fieldOfHonorChallenger.ranking = nChallengerNewRanking;
			cache.SetFieldOfHonorRanker(fieldOfHonorChallenger);
			m_challenger.fieldOfHonorRanking = fieldOfHonorChallenger.ranking;
			m_challenger.fieldOfHonorTargets.Clear();
			m_challenger.AddFieldOfHonorHistory(challengerHistory);
			Hero targetHero = Cache.instance.GetHero(fieldOfHonorTargetHero.id);
			if (targetHero == null)
			{
				targetHero = fieldOfHonorTargetHero.ToHero(m_challenger, m_endTime);
			}
			FieldOfHonorHistory targetRankerHistory = new FieldOfHonorHistory(targetHero, Guid.NewGuid());
			targetRankerHistory.type = 2;
			targetRankerHistory.targetHeroId = m_challenger.id;
			targetRankerHistory.targetName = m_challenger.name;
			targetRankerHistory.oldRanking = m_nTargetRanking;
			targetRankerHistory.ranking = nTargetRankerNewRanking;
			targetRankerHistory.isWin = false;
			targetRankerHistory.regTime = m_endTime;
			fieldOfHonorTargetHero.ranking = nTargetRankerNewRanking;
			cache.SetFieldOfHonorRanker(fieldOfHonorTargetHero);
			if (targetHero.isReal)
			{
				lock (targetHero.syncObject)
				{
					targetHero.fieldOfHonorRanking = nTargetRankerNewRanking;
					targetHero.fieldOfHonorTargets.Clear();
					targetHero.AddFieldOfHonorHistory(targetRankerHistory);
				}
			}
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_FieldOfHonor);
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_challenger.id));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_HonorPoint(m_challenger.id, m_challenger.honorPoint));
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_challenger));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_FieldOfHonorSuccessiveCount(m_challenger.id, m_challenger.fieldOfHonorSuccessiveCount));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_FieldOfHonorRanking(m_challenger.id, m_challenger.fieldOfHonorRanking));
			dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorTargets(m_challenger.id));
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHistory(challengerHistory));
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(fieldOfHonorTargetHero.id));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_FieldOfHonorRanking(fieldOfHonorTargetHero.id, fieldOfHonorTargetHero.ranking));
			dbWork.AddSqlCommand(GameDac.CSC_DeleteFieldOfHonorTargets(fieldOfHonorTargetHero.id));
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHistory(targetRankerHistory));
			dbWork.Schedule();
			ServerEvent.SendFieldOfHonorClear(m_challenger.account.peer, m_challenger.fieldOfHonorRanking, m_challenger.fieldOfHonorSuccessiveCount, m_challenger.honorPoint, lnAcquisitionExp, m_challenger.level, m_challenger.exp, m_challenger.realMaxHP, m_challenger.hp);
		}
		int nDuration = m_fieldOfHonor.exitDelayTime * 1000;
		m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
		m_exitDelayTimeTimer.Change(nDuration, -1);
	}

	private void OnFailAsync(bool bForcedExit)
	{
		AddWork(new SFAction<bool>(Fail, bForcedExit), bGlobalLockRequired: true);
	}

	public void Fail(bool bForcedExit)
	{
		if (isFinished)
		{
			return;
		}
		Finish(4);
		lock (m_challenger.syncObject)
		{
			int nHonorPoint = m_fieldOfHonor.loserHonorPointReward.value;
			m_challenger.AddHonorPoint(nHonorPoint);
			FieldOfHonorLevelReward fieldOfHonorReward = m_fieldOfHonor.GetLevelReward(m_challenger.level);
			long lnAcquisitionExp = 0L;
			if (fieldOfHonorReward != null)
			{
				lnAcquisitionExp = fieldOfHonorReward.expReward.value;
				if (lnAcquisitionExp > 0)
				{
					lnAcquisitionExp = (long)Math.Floor((float)lnAcquisitionExp * Cache.instance.GetWorldLevelExpFactor(m_challenger.level));
					m_challenger.AddExp(lnAcquisitionExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
				}
			}
			m_challenger.fieldOfHonorSuccessiveCount = 0;
			FieldOfHonorHistory challengerHistory = new FieldOfHonorHistory(m_challenger, Guid.NewGuid());
			challengerHistory.type = 1;
			challengerHistory.targetHeroId = m_targetRanker.id;
			challengerHistory.targetName = m_targetRanker.name;
			challengerHistory.oldRanking = m_challenger.fieldOfHonorRanking;
			challengerHistory.ranking = m_challenger.fieldOfHonorRanking;
			challengerHistory.isWin = false;
			challengerHistory.regTime = m_endTime;
			m_challenger.AddFieldOfHonorHistory(challengerHistory);
			FieldOfHonorHero fieldOfHonorTargetHero = Cache.instance.GetFieldOfHonorHero(m_nTargetRanking);
			Hero targetHero = Cache.instance.GetHero(fieldOfHonorTargetHero.id);
			if (targetHero == null)
			{
				targetHero = fieldOfHonorTargetHero.ToHero(m_challenger, m_endTime);
			}
			FieldOfHonorHistory targetRankerHistory = new FieldOfHonorHistory(targetHero, Guid.NewGuid());
			targetRankerHistory.type = 2;
			targetRankerHistory.targetHeroId = m_challenger.id;
			targetRankerHistory.targetName = m_challenger.name;
			targetRankerHistory.oldRanking = m_nTargetRanking;
			targetRankerHistory.ranking = m_nTargetRanking;
			targetRankerHistory.isWin = true;
			targetRankerHistory.regTime = m_endTime;
			if (targetHero.isReal && targetHero.currentPlace != null)
			{
				lock (targetHero.syncObject)
				{
					targetHero.AddFieldOfHonorHistory(targetRankerHistory);
				}
			}
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_FieldOfHonor);
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(m_challenger.id));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_HonorPoint(m_challenger.id, m_challenger.honorPoint));
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_challenger));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_FieldOfHonorSuccessiveCount(m_challenger.id, m_challenger.fieldOfHonorSuccessiveCount));
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHistory(challengerHistory));
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(fieldOfHonorTargetHero.id));
			dbWork.AddSqlCommand(GameDacEx.CSC_AddFieldOfHonorHistory(targetRankerHistory));
			dbWork.Schedule();
			if (bForcedExit)
			{
				Close();
				return;
			}
			ServerEvent.SendFieldOfHonorFail(m_challenger.account.peer, m_challenger.fieldOfHonorSuccessiveCount, m_challenger.honorPoint, lnAcquisitionExp, m_challenger.level, m_challenger.exp, m_challenger.realMaxHP, m_challenger.hp);
		}
		int nDuration = m_fieldOfHonor.exitDelayTime * 1000;
		m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
		m_exitDelayTimeTimer.Change(nDuration, -1);
	}

	private void OnExitDelayTimeTimerTick(object statei)
	{
		AddWork(new SFAction(DungeonBanish), bGlobalLockRequired: true);
	}

	private void DungeonBanish()
	{
		if (!isFinished)
		{
			return;
		}
		lock (m_challenger.syncObject)
		{
			if (m_challenger.isDead)
			{
				m_challenger.Revive(bSendEvent: false);
			}
			else
			{
				m_challenger.RestoreHP(m_challenger.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
			}
			Exit(m_challenger, isLogOut: false, null);
			ServerEvent.SendFieldOfHonorBanished(m_challenger.account.peer, m_challenger.previousContinentId, m_challenger.previousNationId, m_challenger.hp);
		}
	}

	private void Finish(int nStatus)
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
}
