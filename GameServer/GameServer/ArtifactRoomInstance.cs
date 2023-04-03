using System;
using System.Collections.Generic;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class ArtifactRoomInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kStatus_Disqualification = 5;

	private ArtifactRoomFloor m_floor;

	private Hero m_hero;

	private int m_nStatus;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_challengeWaitingTimeTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.ArtifactRoom;

	public override Location location => artifactRoom;

	public override int locationParam => m_floor.floor;

	public override Rect3D mapRect => artifactRoom.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public ArtifactRoom artifactRoom => m_floor.artifactRoom;

	public ArtifactRoomFloor floor => m_floor;

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

	public void Init(ArtifactRoomFloor floor)
	{
		if (floor == null)
		{
			throw new ArgumentNullException("floor");
		}
		m_floor = floor;
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
		int nDuration = artifactRoom.startDelayTime * 1000;
		m_playWaitingTimer = new Timer(OnPlayWaitingTimerTick);
		m_playWaitingTimer.Change(nDuration, -1);
	}

	private void OnPlayWaitingTimerTick(object state)
	{
		AddWork(new SFAction(StartPlay), bGlobalLockRequired: false);
	}

	private void StartPlay()
	{
		if (m_nStatus != 1)
		{
			return;
		}
		m_nStatus = 2;
		m_startTime = DateTimeUtil.currentTime;
		int nLimitTime = artifactRoom.limitTime * 1000;
		m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
		m_limitTimeTimer.Change(nLimitTime, -1);
		List<PDArtifactRoomMonsterInstance> monsterInsts = new List<PDArtifactRoomMonsterInstance>();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (ArtifactRoomMonsterArrange arrange in m_floor.arranges)
		{
			for (int i = 0; i < arrange.monsterCount; i++)
			{
				ArtifactRoomMonsterInstance monsterInst = CreateMonster(arrange);
				AddMonster(monsterInst);
				monsterInsts.Add((PDArtifactRoomMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
			}
		}
		ServerEvent.SendArtifactRoomStart(m_hero.account.peer, monsterInsts.ToArray());
	}

	private ArtifactRoomMonsterInstance CreateMonster(ArtifactRoomMonsterArrange arrange)
	{
		ArtifactRoomMonsterInstance monsterInst = new ArtifactRoomMonsterInstance();
		monsterInst.Init(this, arrange);
		return monsterInst;
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus == 2 && base.monsterInsts.Count == 0)
		{
			Clear();
		}
	}

	private void OnLimitTimeTimerTick(object state)
	{
		AddWork(new SFAction(Fail), bGlobalLockRequired: false);
	}

	private void Clear()
	{
		if (m_nStatus != 2)
		{
			return;
		}
		Finish(3);
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		int nOldBestFloor = m_hero.artifactRoomBestFloor;
		m_hero.SetArtifactRoomCurrentFloor(m_floor.floor + 1);
		dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ArtifactRoomCurrentFloor(m_hero.id, m_hero.artifactRoomCurrentFloor));
		if (nOldBestFloor < m_hero.artifactRoomBestFloor)
		{
			dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_ArtifactRoomBestFloor(m_hero.id, m_hero.artifactRoomBestFloor));
		}
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		ItemReward itemReward = m_floor.itemReward;
		Item item = itemReward.item;
		int nCount = itemReward.count;
		bool bOwned = itemReward.owned;
		int nRewardItemRemainCount = m_hero.AddItem(item, bOwned, nCount, changedInventorySlots);
		PDItemBooty itemBooty = new PDItemBooty();
		itemBooty.id = item.id;
		itemBooty.count = nCount;
		itemBooty.owned = bOwned;
		Mail mail = null;
		if (nRewardItemRemainCount > 0)
		{
			mail = Mail.Create("MAIL_REWARD_N_3", "MAIL_REWARD_D_3", m_endTime);
			mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainCount, bOwned));
			m_hero.AddMail(mail, bSendEvent: true);
		}
		foreach (InventorySlot slot in changedInventorySlots)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
		}
		if (mail != null)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_AddMail(mail));
		}
		dbWork.Schedule();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddArtifactRoomPlayLog(Guid.NewGuid(), m_hero.id, 1, m_floor.floor, m_hero.artifactRoomCurrentFloor, m_hero.artifactRoomBestFloor, 0, 0, m_endTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
		ServerEvent.SendArtifactRoomClear(m_hero.account.peer, itemBooty, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
		int nCurrentFloor = m_floor.floor;
		ArtifactRoomFloor nextFloor = artifactRoom.GetFloor(nCurrentFloor + 1);
		if (nCurrentFloor >= artifactRoom.topFloor || (nextFloor != null && m_hero.level < nextFloor.requiredHeroLevel))
		{
			int nDuration = artifactRoom.exitDelayTime * 1000;
			m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
			m_exitDelayTimeTimer.Change(nDuration, -1);
		}
		else
		{
			int nDuration2 = artifactRoom.continuationChallengeWaitingTime * 1000;
			m_challengeWaitingTimeTimer = new Timer(OnChallengeWaitingTimeTimerTick);
			m_challengeWaitingTimeTimer.Change(nDuration2, -1);
		}
	}

	private void OnChallengeWaitingTimeTimerTick(object state)
	{
		AddWork(new SFAction(ChallengeNextFloor), bGlobalLockRequired: true);
	}

	private void ChallengeNextFloor()
	{
		if (!isFinished)
		{
			return;
		}
		lock (m_hero.syncObject)
		{
			Exit(m_hero, isLogOut: false, new ArtifactRoomEnterParam(DateTimeUtil.currentTime));
			ServerEvent.SendArtifactRoomBanishedForNextFloorChallenge(m_hero.account.peer);
		}
	}

	protected override void OnHeroDead(Hero hero)
	{
		base.OnHeroDead(hero);
		Fail();
	}

	private void Fail()
	{
		if (m_nStatus == 2)
		{
			Finish(4);
			ServerEvent.SendArtifactRoomFail(m_hero.account.peer);
			int nDuration = artifactRoom.exitDelayTime * 1000;
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
			ServerEvent.SendArtifactRoomBanished(m_hero.account.peer, m_hero.previousContinentId, m_hero.previousNationId, m_hero.hp);
		}
	}

	public void Finish(int nStatus)
	{
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeChallengeWaitingTimeTimer();
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
		DisposeChallengeWaitingTimeTimer();
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

	private void DisposeChallengeWaitingTimeTimer()
	{
		if (m_challengeWaitingTimeTimer != null)
		{
			m_challengeWaitingTimeTimer.Dispose();
			m_challengeWaitingTimeTimer = null;
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
