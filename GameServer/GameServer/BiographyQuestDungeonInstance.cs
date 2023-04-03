using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class BiographyQuestDungeonInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kStatus_Disqualification = 5;

	private BiographyQuestDungeon m_biographyQuestDungeon;

	private Hero m_hero;

	private int m_nStatus;

	private int m_nWaveNo;

	private int m_nClearWaveNo;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private int m_nTargetArrangeMonsterCount;

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.BiographyQuestDungeon;

	public override Location location => m_biographyQuestDungeon;

	public override int locationParam => 0;

	public override Rect3D mapRect => m_biographyQuestDungeon.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public BiographyQuestDungeon biographyQuestDungeon => m_biographyQuestDungeon;

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

	public void Init(BiographyQuestDungeon dungeon)
	{
		if (dungeon == null)
		{
			throw new ArgumentNullException("dungeon");
		}
		m_biographyQuestDungeon = dungeon;
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
		int nDuration = m_biographyQuestDungeon.startDelayTime * 1000;
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
			int nLimitTime = m_biographyQuestDungeon.limitTime * 1000;
			m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
			m_limitTimeTimer.Change(nLimitTime, -1);
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
		BiographyQuestDungeonWave wave = m_biographyQuestDungeon.GetWave(m_nWaveNo);
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		List<PDBiographyQuestDungeonMonsterInstance> monsterInsts = new List<PDBiographyQuestDungeonMonsterInstance>();
		foreach (BiographyQuestMonsterArrange arrange in wave.monsterArranges.Values)
		{
			for (int i = 0; i < arrange.monsterCount; i++)
			{
				BiographyQuestDungeonMonsterInstance monsterInst = CreateMonster(arrange);
				AddMonster(monsterInst);
				monsterInsts.Add((PDBiographyQuestDungeonMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
			}
			if (wave.targetType == BiographyQuestDungeonWaveTargetType.TargetMonster && arrange.key == wave.targetArrangeKey)
			{
				m_nTargetArrangeMonsterCount = arrange.monsterCount;
			}
		}
		ServerEvent.SendBiographyQuestDungeonWaveStart(m_hero.account.peer, m_nWaveNo, monsterInsts.ToArray());
	}

	private BiographyQuestDungeonMonsterInstance CreateMonster(BiographyQuestMonsterArrange arrange)
	{
		BiographyQuestDungeonMonsterInstance monsterInst = new BiographyQuestDungeonMonsterInstance();
		monsterInst.Init(this, arrange);
		return monsterInst;
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus != 2 || m_nWaveNo == m_nClearWaveNo)
		{
			return;
		}
		BiographyQuestDungeonWave wave = m_biographyQuestDungeon.GetWave(m_nWaveNo);
		if (wave.targetType == BiographyQuestDungeonWaveTargetType.AllMonster)
		{
			if (base.monsterInsts.Count == 0)
			{
				CompleteWave(wave);
			}
			return;
		}
		_ = wave.targetArrangeKey;
		BiographyQuestDungeonMonsterInstance biographyQuestDungeonMosnterInst = (BiographyQuestDungeonMonsterInstance)monsterInst;
		if (m_nWaveNo == biographyQuestDungeonMosnterInst.arrange.wave.no)
		{
			if (wave.targetArrangeKey == biographyQuestDungeonMosnterInst.arrange.key)
			{
				m_nTargetArrangeMonsterCount--;
			}
			if (m_nTargetArrangeMonsterCount <= 0)
			{
				CompleteWave(wave);
			}
		}
	}

	private void CompleteWave(BiographyQuestDungeonWave wave)
	{
		m_nClearWaveNo = wave.no;
		MonsterInstance[] array = m_monsterInsts.Values.ToArray();
		foreach (MonsterInstance monsterInst in array)
		{
			RemoveMonster(monsterInst, bSendEvent: true);
		}
		ServerEvent.SendBiographyQuestDungeonWaveCompleted(m_hero.account.peer, m_nWaveNo);
		if (m_nWaveNo >= m_biographyQuestDungeon.lastWaveNo)
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
		if (m_nStatus == 2)
		{
			Finish(3);
			m_hero.ProcessBiographyQuestsForDungeon(m_biographyQuestDungeon.id);
			ServerEvent.SendBiographyQuestDungeonClear(m_hero.account.peer);
			int nDuration = m_biographyQuestDungeon.exitDelayTime * 1000;
			m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
			m_exitDelayTimeTimer.Change(nDuration, -1);
		}
	}

	private void Fail()
	{
		if (m_nStatus == 2)
		{
			Finish(4);
			ServerEvent.SendBiographyQuestDungeonFail(m_hero.account.peer);
			int nDuration = m_biographyQuestDungeon.exitDelayTime * 1000;
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
			ServerEvent.SendBiographyQuestDungeonBanished(m_hero.account.peer, m_hero.previousContinentId, m_hero.previousNationId, m_hero.hp);
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
}
