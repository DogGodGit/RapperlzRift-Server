using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class SoulCoveterInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kLogStatus_Start = 1;

	public const int kLogStatus_Clear = 2;

	public const int kLogStatus_Fail = 3;

	private SoulCoveter m_soulCoveter;

	private SoulCoveterDifficulty m_difficulty;

	private int m_nStatus;

	private int m_nWaveNo;

	private DateTimeOffset m_creationTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_waveIntervalTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.SoulCoveter;

	public override Location location => m_soulCoveter;

	public override int locationParam => m_difficulty.difficulty;

	public override Rect3D mapRect => m_soulCoveter.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public SoulCoveter soulCoveter => m_soulCoveter;

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

	public int waveNo => m_nWaveNo;

	public void Init(SoulCoveterDifficulty difficulty, DateTimeOffset time)
	{
		if (difficulty == null)
		{
			throw new ArgumentNullException("difficulty");
		}
		m_soulCoveter = difficulty.soulCoveter;
		m_difficulty = difficulty;
		m_creationTime = time;
		InitPlace();
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_SoulCoveter);
		dbWork.AddSqlCommand(GameDac.CSC_AddSoulCoveterInstance(m_instanceId, m_difficulty.difficulty, 1, 0, time));
		dbWork.Schedule();
		m_nStatus = 1;
		int nDuration = m_soulCoveter.startDelayTime * 1000;
		m_playWaitingTimer = new Timer(OnPlayWaitingTimerTick);
		m_playWaitingTimer.Change(nDuration, -1);
	}

	public float GetRemainingStartTime(DateTimeOffset time)
	{
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fStartTime = (float)m_creationTime.TimeOfDay.TotalSeconds + (float)m_soulCoveter.startDelayTime;
		return Math.Max(fStartTime - fTime, 0f);
	}

	public float GetRemainingLimitTime(DateTimeOffset time)
	{
		if (m_nStatus == 1)
		{
			return m_soulCoveter.limitTime;
		}
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fEndTime = (float)m_startTime.TimeOfDay.TotalSeconds + (float)m_soulCoveter.limitTime;
		return Math.Max(fEndTime - fTime, 0f);
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
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
			int nDuration = m_soulCoveter.limitTime * 1000;
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
		SoulCoveterDifficultyWave wave = m_difficulty.GetWave(m_nWaveNo);
		List<PDSoulCoveterMonsterInstance> monsterInsts = new List<PDSoulCoveterMonsterInstance>();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (SoulCoveterMonsterArrange arrange in wave.monsterArranges)
		{
			for (int i = 0; i < arrange.monsterCount; i++)
			{
				SoulCoveterMonsterInstance monsterInst = CreateMonster(arrange);
				AddMonster(monsterInst);
				monsterInsts.Add((PDSoulCoveterMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
			}
		}
		ServerEvent.SendSoulCoveterWaveStart(GetClientPeers(), m_nWaveNo, monsterInsts.ToArray());
	}

	private SoulCoveterMonsterInstance CreateMonster(SoulCoveterMonsterArrange arrange)
	{
		SoulCoveterMonsterInstance monsterInst = new SoulCoveterMonsterInstance();
		monsterInst.Init(this, arrange);
		return monsterInst;
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus != 2 || !monsterInst.isDead)
		{
			return;
		}
		SoulCoveterDifficultyWave wave = m_difficulty.GetWave(m_nWaveNo);
		int nTargetArrangeNo = wave.targetArrangeNo;
		if (nTargetArrangeNo == 0)
		{
			if (m_monsterInsts.Count == 0)
			{
				CompleteWave();
			}
			return;
		}
		SoulCoveterMonsterInstance soulCoveterMonsterInst = (SoulCoveterMonsterInstance)monsterInst;
		if (soulCoveterMonsterInst.arrange.no == nTargetArrangeNo)
		{
			CompleteWave();
		}
	}

	private void CompleteWave()
	{
		MonsterInstance[] array = m_monsterInsts.Values.ToArray();
		foreach (MonsterInstance monsterInst in array)
		{
			RemoveMonster(monsterInst, bSendEvent: true);
		}
		if (m_nWaveNo >= m_difficulty.waveCount)
		{
			Clear();
			return;
		}
		int nDuration = m_soulCoveter.waveIntervalTime * 1000;
		m_waveIntervalTimer = new Timer(OnWaveIntervalTimerTick);
		m_waveIntervalTimer.Change(nDuration, -1);
	}

	private void OnWaveIntervalTimerTick(object state)
	{
		AddWork(new SFAction(StartNextWave), bGlobalLockRequired: false);
	}

	private void Clear()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		Finish(3);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_SoulCoveter);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateSoulCoveterInstance(m_instanceId, 2, m_nPlayTime, m_endTime));
		List<SoulCoveterDifficultyReward> rewards = m_difficulty.rewards;
		foreach (Hero hero in m_heroes.Values)
		{
			lock (hero.syncObject)
			{
				List<PDItemBooty> booties = new List<PDItemBooty>();
				List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
				Mail mail = null;
				foreach (SoulCoveterDifficultyReward reward in rewards)
				{
					ItemReward itemReward = reward.itemReward;
					if (itemReward == null)
					{
						continue;
					}
					Item item = itemReward.item;
					int nCount = itemReward.count;
					bool bOwned = itemReward.owned;
					int nRewardItemRemainingCount = hero.AddItem(item, bOwned, nCount, changedInventorySlots);
					PDItemBooty itemBooty = new PDItemBooty();
					itemBooty.id = item.id;
					itemBooty.count = nCount;
					itemBooty.owned = bOwned;
					booties.Add(itemBooty);
					if (nRewardItemRemainingCount > 0)
					{
						if (mail == null)
						{
							mail = Mail.Create("MAIL_REWARD_N_4", "MAIL_REWARD_D_4", m_endTime);
						}
						mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainingCount, bOwned));
					}
				}
				if (mail != null)
				{
					hero.AddMail(mail, bSendEvent: true);
				}
				dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(hero.id));
				dbWork.AddSqlCommand(GameDac.CSC_UpdateSoulCoveterInstanceMember(m_instanceId, hero.id, 1, m_endTime));
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
					logWork.AddSqlCommand(GameLogDac.CSC_AddSoulCoveterRewardLog(logId, hero.id, m_instanceId, m_difficulty.difficulty, m_endTime));
					foreach (PDItemBooty booty in booties)
					{
						logWork.AddSqlCommand(GameLogDac.CSC_AddSoulCoveterRewardDetailLog(Guid.NewGuid(), logId, booty.id, booty.count, booty.owned));
					}
					logWork.Schedule();
				}
				catch (Exception ex)
				{
					SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
				}
				ServerEvent.SendSoulCoveterClear(hero.account.peer, booties.ToArray(), InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
			}
		}
		dbWork.Schedule();
		int nDuration = m_soulCoveter.exitDelayTime * 1000;
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
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_SoulCoveter);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateSoulCoveterInstance(m_instanceId, 3, m_nPlayTime, m_endTime));
		foreach (Hero hero in m_heroes.Values)
		{
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(hero.id));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateSoulCoveterInstanceMember(m_instanceId, hero.id, 2, m_endTime));
		}
		dbWork.Schedule();
		ServerEvent.SendSoulCoveterFail(GetClientPeers());
		int nDuration = m_soulCoveter.exitDelayTime * 1000;
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
				ServerEvent.SendSoulCoveterBanished(hero.account.peer, hero.previousContinentId, hero.previousNationId, hero.hp);
			}
		}
		Close();
	}

	public void Disqualification(Hero hero)
	{
		if (!isFinished)
		{
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_SoulCoveter);
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(hero.id));
			dbWork.AddSqlCommand(GameDac.CSC_UpdateSoulCoveterInstanceMember(m_instanceId, hero.id, 3, DateTimeUtil.currentTime));
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
