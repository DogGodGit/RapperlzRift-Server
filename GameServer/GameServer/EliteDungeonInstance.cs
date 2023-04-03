using System;
using System.Threading;
using ServerFramework;

namespace GameServer;

public class EliteDungeonInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kStatus_Disqualification = 5;

	private EliteDungeon m_eliteDungeon;

	private EliteMonster m_eliteMonster;

	private Hero m_hero;

	private int m_nStatus;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.EliteDungeon;

	public override Location location => m_eliteDungeon;

	public override int locationParam => 0;

	public override Rect3D mapRect => m_eliteDungeon.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => m_nStatus == 2;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public EliteDungeon eliteDungeon => m_eliteDungeon;

	public EliteMonsterMaster master => m_eliteMonster.master;

	public EliteMonster eliteMonster => m_eliteMonster;

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

	public void Init(EliteMonsterMaster master, DateTimeOffset time)
	{
		if (master == null)
		{
			throw new ArgumentNullException("master");
		}
		m_eliteDungeon = Resource.instance.eliteDungeon;
		InitPlace();
		EliteDungeonMonsterInstance monsterInst = CreateMonster(master);
		m_eliteMonster = monsterInst.eliteMonster;
		AddMonster(monsterInst);
	}

	private EliteDungeonMonsterInstance CreateMonster(EliteMonsterMaster master)
	{
		EliteDungeonMonsterInstance monsterInst = new EliteDungeonMonsterInstance();
		monsterInst.Init(this, master);
		return monsterInst;
	}

	protected override void OnHeroEnter(Hero hero, DateTimeOffset time)
	{
		base.OnHeroEnter(hero, time);
		m_hero = hero;
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		if (isLogOut)
		{
			Disqualification();
		}
		Close();
	}

	public void Start(DateTimeOffset time)
	{
		m_nStatus = 1;
		m_hero.dailyEliteDungeonPlayCount.value++;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddHeroEliteDungeonPlay(m_instanceId, m_hero.id, m_eliteMonster.id, 0, 0, time));
		dbWork.Schedule();
		int nDuration = m_eliteDungeon.startDelayTime * 1000;
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
			int nLimitTime = m_eliteDungeon.limitTime * 1000;
			m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
			m_limitTimeTimer.Change(nLimitTime, -1);
			ServerEvent.SendEliteDungeonStart(m_hero.account.peer);
		}
	}

	private void OnLimitTimeTimerTick(object state)
	{
		AddWork(new SFAction(Fail), bGlobalLockRequired: false);
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus == 2 && base.monsterInsts.Count == 0)
		{
			Clear();
		}
	}

	private void Clear()
	{
		if (m_nStatus == 2)
		{
			Finish(3);
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroEliteDungeonPlay(m_instanceId, 1, m_nPlayTime, m_endTime));
			dbWork.Schedule();
			ServerEvent.SendEliteDungeonClear(m_hero.account.peer);
			int nDuration = m_eliteDungeon.exitDelayTime * 1000;
			m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
			m_exitDelayTimeTimer.Change(nDuration, -1);
		}
	}

	private void Fail()
	{
		if (m_nStatus == 2)
		{
			Finish(4);
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroEliteDungeonPlay(m_instanceId, 3, m_nPlayTime, m_endTime));
			dbWork.Schedule();
			ServerEvent.SendEliteDungeonFail(m_hero.account.peer);
			int nDuration = m_eliteDungeon.exitDelayTime * 1000;
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
			ServerEvent.SendEliteDungeonBanished(m_hero.account.peer, m_hero.previousContinentId, m_hero.previousNationId, m_hero.hp);
		}
	}

	public void Disqualification()
	{
		if (!isFinished)
		{
			Finish(5);
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHeroEliteDungeonPlay(m_instanceId, 2, m_nPlayTime, m_endTime));
			dbWork.Schedule();
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

	private void DisposeExitDelayTimeTimer()
	{
		if (m_exitDelayTimeTimer != null)
		{
			m_exitDelayTimeTimer.Dispose();
			m_exitDelayTimeTimer = null;
		}
	}
}
