using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class AnkouTombInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kLogStatus_Clear = 1;

	public const int kLogStatus_Fail = 2;

	private AnkouTomb m_ankouTomb;

	private AnkouTombDifficulty m_difficulty;

	private AnkouTombSchedule m_schedule;

	private int m_nStatus;

	private int m_nWaveNo;

	private DateTimeOffset m_creationTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private Dictionary<Guid, HeroAnkouTombPoint> m_heroPoints = new Dictionary<Guid, HeroAnkouTombPoint>();

	private Dictionary<Guid, long> m_receivedRewardExps = new Dictionary<Guid, long>();

	private HashSet<Guid> m_additionalRewardReceivedHeroes = new HashSet<Guid>();

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.AnkouTomb;

	public override Location location => m_ankouTomb;

	public override int locationParam => m_difficulty.difficulty;

	public override Rect3D mapRect => m_ankouTomb.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public AnkouTomb ankouTomb => m_ankouTomb;

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

	public void Init(AnkouTombDifficulty difficulty, AnkouTombSchedule schedule, DateTimeOffset time)
	{
		if (difficulty == null)
		{
			throw new ArgumentNullException("difficulty");
		}
		if (schedule == null)
		{
			throw new ArgumentNullException("schedule");
		}
		m_ankouTomb = difficulty.ankouTomb;
		m_difficulty = difficulty;
		m_schedule = schedule;
		m_creationTime = time;
		InitPlace();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddAnkouTombCreationLog(m_instanceId, m_difficulty.difficulty, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		m_nStatus = 1;
		int nDuration = m_ankouTomb.startDelayTime * 1000;
		m_playWaitingTimer = new Timer(OnPlayWaitingTimerTick);
		m_playWaitingTimer.Change(nDuration, -1);
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		hero.StopAnkouTombMoneyBuff(bSendEvent: true);
		Disqualification(hero);
	}

	public float GetRemainingStartTime(DateTimeOffset time)
	{
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fStartTime = (float)m_creationTime.TimeOfDay.TotalSeconds + (float)m_ankouTomb.startDelayTime;
		return Math.Max(fStartTime - fTime, 0f);
	}

	public float GetRemainingLimitTime(DateTimeOffset time)
	{
		if (m_nStatus == 1)
		{
			return m_ankouTomb.limitTime;
		}
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fEndTime = (float)m_startTime.TimeOfDay.TotalSeconds + (float)m_ankouTomb.limitTime;
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
			int nDuration = m_ankouTomb.limitTime * 1000;
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
		AnkouTombWave wave = m_difficulty.GetWave(m_nWaveNo);
		List<PDAnkouTombMonsterInstance> monsterInsts = new List<PDAnkouTombMonsterInstance>();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		Vector3 monsterSpawnPosition = m_ankouTomb.SelectMonsterSpawnPosition();
		foreach (AnkouTombMonsterArrange monsterArrange in wave.monsterArranges)
		{
			if (monsterArrange.monsterType != 2 || Util.DrawLots(m_ankouTomb.bossMonsterSpawnRate))
			{
				for (int i = 0; i < monsterArrange.monsterCount; i++)
				{
					AnkouTombMonsterInstance monsterInst = CreateMonster(monsterArrange, monsterSpawnPosition);
					AddMonster(monsterInst);
					monsterInsts.Add((PDAnkouTombMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
				}
			}
		}
		ServerEvent.SendAnkouTombWaveStart(GetClientPeers(), m_nWaveNo, monsterInsts.ToArray());
	}

	private AnkouTombMonsterInstance CreateMonster(AnkouTombMonsterArrange monsterArrange, Vector3 position)
	{
		AnkouTombMonsterInstance inst = new AnkouTombMonsterInstance();
		inst.Init(this, monsterArrange, position);
		return inst;
	}

	protected override void OnMonsterDead(MonsterInstance monsterInst)
	{
		base.OnMonsterDead(monsterInst);
		if (m_nStatus != 2)
		{
			return;
		}
		AnkouTombMonsterInstance ankouTombMonsterInst = (AnkouTombMonsterInstance)monsterInst;
		if (!(monsterInst.lastAttacker is Hero killer))
		{
			return;
		}
		Hero heroKiller = GetHero(killer.id);
		if (heroKiller != null)
		{
			HeroAnkouTombPoint heroPoint = GetOrCreateHeroPoint(heroKiller);
			if (ankouTombMonsterInst.arrange.monsterType == 1)
			{
				heroPoint.AddPoint(m_ankouTomb.monsterPoint, ankouTombMonsterInst.lastDamageTime);
			}
			else
			{
				heroPoint.AddPoint(m_ankouTomb.bossMonsterPoint, ankouTombMonsterInst.lastDamageTime);
			}
			ServerEvent.SendAnkouTombPointAcquisition(heroKiller.account.peer, heroPoint.point);
		}
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus == 2 && m_monsterInsts.Count <= 0)
		{
			if (m_nWaveNo >= m_ankouTomb.waveCount)
			{
				Clear();
			}
			else
			{
				StartNextWave();
			}
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
			logWork2.AddSqlCommand(GameLogDac.CSC_AddAnkouTombCompletionLog(m_instanceId, 1, m_endTime));
			logWork2.Schedule();
		}
		catch (Exception ex2)
		{
			SFLogUtil.Error(GetType(), null, ex2);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_AnkouTomb);
		int nClearPoint = m_ankouTomb.clearPoint;
		HeroAnkouTombPoint bestHeroPoint = null;
		foreach (Hero hero in m_heroes.Values)
		{
			HeroAnkouTombPoint heroPoint = GetOrCreateHeroPoint(hero);
			heroPoint.AddPoint(nClearPoint, m_endTime);
			bool bMyBestRecordChanged = hero.RefreshAnkouTombBestRecord(heroPoint);
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
					mail = Mail.Create("MAIL_REWARD_N_34", "MAIL_REWARD_D_34", m_endTime);
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
				dbWork.AddSqlCommand(GameDac.CSC_AddorUpdateHeroAnkouTombBestRecord(hero.id, heroPoint.difficulty, heroPoint.point, heroPoint.updateTime));
			}
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				logWork.AddSqlCommand(GameLogDac.CSC_AddAnkouTombCompletionMemberRewardLog(m_instanceId, hero.id, lnAcquisitionGold, lnAcquisitionExp, itemBooty?.id ?? 0, itemBooty?.owned ?? false, itemBooty?.count ?? 0));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex);
			}
			ServerEvent.SendAnkouTombClear(hero.account.peer, heroPoint.point, lnAcquisitionExp, hero.level, hero.exp, hero.realMaxHP, hero.hp, hero.gold, hero.maxGold, itemBooty, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
		}
		Cache.instance.RefreshAnkouTombServerBestRecord(bestHeroPoint);
		dbWork.Schedule();
		int nDuration = m_ankouTomb.exitDelayTime * 1000;
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
			logWork2.AddSqlCommand(GameLogDac.CSC_AddAnkouTombCompletionLog(m_instanceId, 2, m_endTime));
			logWork2.Schedule();
		}
		catch (Exception ex2)
		{
			SFLogUtil.Error(GetType(), null, ex2);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_AnkouTomb);
		foreach (Hero hero in m_heroes.Values)
		{
			HeroAnkouTombPoint heroPoint = GetOrCreateHeroPoint(hero);
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
				logWork.AddSqlCommand(GameLogDac.CSC_AddAnkouTombCompletionMemberRewardLog(m_instanceId, hero.id, lnAcquisitionGold, lnAcquisitionExp, 0, bRewardItemOwned: false, 0));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex);
			}
			ServerEvent.SendAnkouTombFail(hero.account.peer, lnAcquisitionExp, hero.level, hero.exp, hero.realMaxHP, hero.hp, hero.gold, hero.maxGold);
		}
		dbWork.Schedule();
		int nDuration = m_ankouTomb.exitDelayTime * 1000;
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
				ServerEvent.SendAnkouTombBanished(hero.account.peer, hero.previousContinentId, hero.previousNationId, hero.hp);
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
			logWork.AddSqlCommand(GameLogDac.CSC_AddAnkouTombMemberDisqualificationLog(m_instanceId, hero.id, DateTimeUtil.currentTime));
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

	private HeroAnkouTombPoint GetHeroPoint(Guid heroId)
	{
		if (!m_heroPoints.TryGetValue(heroId, out var value))
		{
			return null;
		}
		return value;
	}

	public HeroAnkouTombPoint GetOrCreateHeroPoint(Hero hero)
	{
		HeroAnkouTombPoint heroPoint = GetHeroPoint(hero.id);
		if (heroPoint == null)
		{
			heroPoint = new HeroAnkouTombPoint();
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
