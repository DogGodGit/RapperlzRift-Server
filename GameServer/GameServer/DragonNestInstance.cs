using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class DragonNestInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	private DragonNest m_dragonNest;

	private int m_nAverageHeroLevel;

	private int m_nStatus;

	private int m_nStepNo;

	private DateTimeOffset m_creationTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private List<DragonNestTrapInstance> m_trapInsts = new List<DragonNestTrapInstance>();

	private bool m_bAdditionalStepOpen;

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_stepStartDelayTimeTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.DragonNest;

	public override Location location => m_dragonNest;

	public override int locationParam => 0;

	public override Rect3D mapRect => m_dragonNest.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public DragonNest dragonNest => m_dragonNest;

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

	public int averageHeroLevel => m_nAverageHeroLevel;

	public void Init(DragonNest dragonNest, DateTimeOffset time, int nAverageHeroLevel)
	{
		if (dragonNest == null)
		{
			throw new ArgumentNullException("dragonNest");
		}
		m_dragonNest = dragonNest;
		m_nAverageHeroLevel = nAverageHeroLevel;
		m_creationTime = time;
		InitPlace();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddDragonNestCreationLog(m_instanceId, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		m_nStatus = 1;
		int nDuration = m_dragonNest.startDelayTime * 1000;
		m_playWaitingTimer = new Timer(OnPlayWaitingTimerTick);
		m_playWaitingTimer.Change(nDuration, -1);
	}

	public float GetRemainingStartTime(DateTimeOffset time)
	{
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fStartTime = (float)m_creationTime.TimeOfDay.TotalSeconds + (float)m_dragonNest.startDelayTime;
		return Math.Max(fStartTime - fTime, 0f);
	}

	public float GetRemainingLimitTime(DateTimeOffset time)
	{
		if (m_nStatus == 1)
		{
			return m_dragonNest.limitTime;
		}
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fEndTime = (float)m_startTime.TimeOfDay.TotalSeconds + (float)m_dragonNest.limitTime;
		return Math.Max(fEndTime - fTime, 0f);
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		hero.StopDragonNestTrapEffect();
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
			int nDuration = m_dragonNest.limitTime * 1000;
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
		DragonNestMonsterAttrFactor attrFactor = m_dragonNest.GetMonsterAttrFactor(m_nAverageHeroLevel);
		foreach (DragonNestTrap trap in m_dragonNest.traps)
		{
			if (trap.activationStepNo == m_nStepNo)
			{
				DragonNestTrapInstance trapInst = CreateTrap(trap, attrFactor);
				m_trapInsts.Add(trapInst);
			}
		}
		DateTimeOffset currentTIme = DateTimeUtil.currentTime;
		DragonNestStep step = m_dragonNest.GetStep(m_nStepNo);
		List<PDDragonNestMonsterInstance> monsterInsts = new List<PDDragonNestMonsterInstance>();
		foreach (DragonNestMonsterArrange monsterArrange in step.monsterArranges)
		{
			for (int i = 0; i < monsterArrange.monsterCount; i++)
			{
				DragonNestMonsterInstance monsterInst = CreateMonster(monsterArrange, attrFactor);
				AddMonster(monsterInst);
				monsterInsts.Add((PDDragonNestMonsterInstance)monsterInst.ToPDMonsterInstance(currentTIme));
			}
		}
		ServerEvent.SendDragonNestStepStart(GetClientPeers(), m_nStepNo, monsterInsts.ToArray());
	}

	private DragonNestTrapInstance CreateTrap(DragonNestTrap trap, DragonNestMonsterAttrFactor attrFactor)
	{
		DragonNestTrapInstance inst = new DragonNestTrapInstance();
		inst.Init(this, trap, attrFactor);
		return inst;
	}

	private DragonNestMonsterInstance CreateMonster(DragonNestMonsterArrange monsterArrange, DragonNestMonsterAttrFactor attrFactor)
	{
		DragonNestMonsterInstance inst = new DragonNestMonsterInstance();
		inst.Init(this, monsterArrange, attrFactor);
		return inst;
	}

	public List<Guid> GetTrapEffectHeroes()
	{
		List<Guid> results = new List<Guid>();
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.isDragonNestTrapEffect)
			{
				results.Add(hero.id);
			}
		}
		return results;
	}

	protected override void OnHeroPositionChanged(Hero hero, SectorChangeInfo info, bool bSendInterestTargetChangeEvent, DateTimeOffset currentTime)
	{
		base.OnHeroPositionChanged(hero, info, bSendInterestTargetChangeEvent, currentTime);
		if (m_nStatus != 2)
		{
			return;
		}
		DragonNestStep step = m_dragonNest.GetStep(m_nStepNo);
		if (step.type == 1 && step.ContainsTargetPosition(hero.position))
		{
			CompleteStep();
		}
		foreach (DragonNestTrapInstance trapInst in m_trapInsts)
		{
			if (trapInst.trap.ContainsPosition(hero.position))
			{
				hero.HitDragonNestTrap(trapInst, currentTime);
			}
		}
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus == 2 && m_monsterInsts.Count <= 0)
		{
			CompleteStep();
		}
	}

	private void CompleteStep()
	{
		DragonNestStep step = m_dragonNest.GetStep(m_nStepNo);
		DragonNestTrapInstance[] array = m_trapInsts.ToArray();
		foreach (DragonNestTrapInstance trapInst in array)
		{
			if (trapInst.trap.deactivationStepNo == m_nStepNo)
			{
				trapInst.Release();
				m_trapInsts.Remove(trapInst);
			}
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_DragonNest);
		foreach (Hero hero in m_heroes.Values)
		{
			List<PDItemBooty> booties = new List<PDItemBooty>();
			List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
			Mail mail = null;
			foreach (DragonNestStepReward reward in step.rewards)
			{
				ItemReward itemReward = reward.itemReward;
				if (itemReward == null || itemReward.count <= 0)
				{
					continue;
				}
				Item item = itemReward.item;
				int nCount = itemReward.count;
				bool bOwned = itemReward.owned;
				int nRewardItemRemainCount = hero.AddItem(item, bOwned, nCount, changedInventorySlots);
				PDItemBooty itemBooty = new PDItemBooty();
				itemBooty.id = item.id;
				itemBooty.count = nCount;
				itemBooty.owned = bOwned;
				booties.Add(itemBooty);
				if (nRewardItemRemainCount > 0)
				{
					if (mail == null)
					{
						mail = Mail.Create("MAIL_REWARD_N_26", "MAIL_REWARD_D_26", m_endTime);
					}
					mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainCount, bOwned));
				}
			}
			if (mail != null)
			{
				hero.AddMail(mail, bSendEvent: true);
			}
			dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(hero.id));
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
				foreach (PDItemBooty booty in booties)
				{
					logWork.AddSqlCommand(GameLogDac.CSC_AddDragonNestCompletionMemberRewardLog(Guid.NewGuid(), m_instanceId, hero.id, m_nStepNo, booty.id, booty.owned, booty.count));
				}
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex);
			}
			ServerEvent.SendDragonNestStepCompleted(hero.account.peer, booties.ToArray(), InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
		}
		dbWork.Schedule();
		if (m_nStepNo >= m_dragonNest.stepCount)
		{
			Clear();
		}
		else if (m_nStepNo == m_dragonNest.baseMaxStep)
		{
			if (Util.DrawLots(m_dragonNest.additionalStepOpenRate))
			{
				m_bAdditionalStepOpen = true;
				int nDuration2 = step.startDelayTime * 1000;
				if (m_stepStartDelayTimeTimer == null)
				{
					m_stepStartDelayTimeTimer = new Timer(OnStepStartDelayTimeTimerTick);
				}
				m_stepStartDelayTimeTimer.Change(nDuration2, -1);
			}
			else
			{
				Clear();
			}
		}
		else
		{
			int nDuration = step.startDelayTime * 1000;
			if (m_stepStartDelayTimeTimer == null)
			{
				m_stepStartDelayTimeTimer = new Timer(OnStepStartDelayTimeTimerTick);
			}
			m_stepStartDelayTimeTimer.Change(nDuration, -1);
		}
	}

	private void OnStepStartDelayTimeTimerTick(object state)
	{
		AddWork(new SFAction(StartNextStep), bGlobalLockRequired: false);
	}

	private void Clear()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		Finish(3);
		List<PDSimpleHero> clearedHeroes = new List<PDSimpleHero>();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddDragonNestCompletionLog(m_instanceId, 1, m_bAdditionalStepOpen, m_endTime));
			foreach (Hero hero in m_heroes.Values)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddDragonNestCompletionMemberLog(m_instanceId, hero.id));
				clearedHeroes.Add(hero.ToPDSimpleHero());
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		ServerEvent.SendDragonNestClear(GetClientPeers(), clearedHeroes.ToArray());
		int nDuration = m_dragonNest.exitDelayTime * 1000;
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
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddDragonNestCompletionLog(m_instanceId, 2, m_bAdditionalStepOpen, m_endTime));
			foreach (Hero hero in m_heroes.Values)
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddDragonNestCompletionMemberLog(m_instanceId, hero.id));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		ServerEvent.SendDragonNestFail(GetClientPeers());
		int nDuration = m_dragonNest.exitDelayTime * 1000;
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
		foreach (Hero hero in m_heroes.Values.ToList())
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
				ServerEvent.SendDragonNestBanished(hero.account.peer, hero.previousContinentId, hero.previousNationId, hero.hp);
			}
		}
		Close();
	}

	public void Disqualification(Hero hero)
	{
		if (isFinished)
		{
			return;
		}
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddDragonNestMemberDisqualificationLog(m_instanceId, hero.id, DateTimeUtil.currentTime));
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
		DisposeStepStartDelayTimeTimer();
		m_endTime = DateTimeUtil.currentTime;
		if (m_nStatus == 2)
		{
			m_nPlayTime = (int)Math.Floor((m_endTime - m_startTime).TotalSeconds);
		}
		m_nStatus = nStatus;
		foreach (DragonNestTrapInstance trapInst in m_trapInsts)
		{
			trapInst.Release();
		}
		m_trapInsts.Clear();
	}

	protected override void ReleaseInternal()
	{
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeStepStartDelayTimeTimer();
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

	private void DisposeStepStartDelayTimeTimer()
	{
		if (m_stepStartDelayTimeTimer != null)
		{
			m_stepStartDelayTimeTimer.Dispose();
			m_stepStartDelayTimeTimer = null;
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
