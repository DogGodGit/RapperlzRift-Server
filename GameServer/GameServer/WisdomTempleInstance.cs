using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class WisdomTempleInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kStatus_Disqualification = 5;

	public const int kRowDimension = 0;

	public const int kColDimension = 1;

	private WisdomTemple m_wisdomTemple;

	private Hero m_hero;

	private int m_nHeroLevel;

	private int m_nStatus;

	private int m_nStepNo;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private int m_nCurrentPuzzleId;

	private int m_nColorMatchingPoint;

	private WisdomTempleColorMatchingObjectInstance[,] m_colorMatchingObjectInsts;

	private Dictionary<long, WisdomTemplePuzzleRewardObjectInstance> m_puzzleRewardObjectInsts = new Dictionary<long, WisdomTemplePuzzleRewardObjectInstance>();

	private long m_lnRealTreasureBoxInstanceId;

	private int m_nFindTreasureBoxKillCount;

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_stepStartDelayTimeTimer;

	private Timer m_colorMatchingMonsterCreationTimer;

	private Timer m_bossMonsterCreationTimer;

	private Timer m_exitDelayTimeTimer;

	private int m_nLastLogRegistrationStepNo;

	private Guid m_logId = Guid.Empty;

	public override PlaceType placeType => PlaceType.WisdomTemple;

	public override Location location => m_wisdomTemple;

	public override int locationParam => 0;

	public override Rect3D mapRect => m_wisdomTemple.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => false;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public WisdomTemple wisdomTemple => m_wisdomTemple;

	public int heroLevel => m_nHeroLevel;

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

	public int stepNo => m_nStepNo;

	public int currentPuzzleId => m_nCurrentPuzzleId;

	public int colorMatchingPoint => m_nColorMatchingPoint;

	public Guid logId => m_logId;

	public void Init()
	{
		m_wisdomTemple = Resource.instance.wisdomTemple;
		m_colorMatchingObjectInsts = new WisdomTempleColorMatchingObjectInstance[m_wisdomTemple.arrangePositionMaxRow + 1, m_wisdomTemple.arrangePositionMaxCol + 1];
		InitPlace();
	}

	protected override void OnHeroEnter(Hero hero, DateTimeOffset time)
	{
		base.OnHeroEnter(hero, time);
		m_hero = hero;
		m_nHeroLevel = hero.level;
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		if (!isFinished)
		{
			Finish(5);
		}
		Close();
	}

	public void Start(DateTimeOffset time)
	{
		m_nStatus = 1;
		DateTime date = time.Date;
		m_hero.RefreshDailyWisdomTemplePlayCount(date);
		DateValuePair<int> dailyWisdomTemplePlayCount = m_hero.dailyWisdomTemplePlayCount;
		dailyWisdomTemplePlayCount.value++;
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		dbWork.AddSqlCommand(GameDac.CSC_AddOrUpdateHeroWisdomTemplePlay(m_hero.id, date, dailyWisdomTemplePlayCount.value));
		dbWork.Schedule();
		int nDuration = m_wisdomTemple.startDelayTime * 1000;
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
			int nDuration = m_wisdomTemple.limitTime * 1000;
			m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
			m_limitTimeTimer.Change(nDuration, -1);
			StartStepStartDelayTimeTimer();
		}
	}

	private void OnLimitTimeTimerTick(object state)
	{
		AddWork(new SFAction(Fail), bGlobalLockRequired: false);
	}

	private void StartStepStartDelayTimeTimer()
	{
		WisdomTempleStep step = m_wisdomTemple.GetStep(m_nStepNo + 1);
		int nDuration = step.startDelayTime * 1000;
		if (m_stepStartDelayTimeTimer == null)
		{
			m_stepStartDelayTimeTimer = new Timer(OnStepStartDelayTimeTimerTick);
		}
		m_stepStartDelayTimeTimer.Change(nDuration, -1);
	}

	private void OnStepStartDelayTimeTimerTick(object state)
	{
		AddWork(new SFAction(StartNextStep), bGlobalLockRequired: false);
	}

	private void StartNextStep()
	{
		if (m_nStatus == 2)
		{
			m_nStepNo++;
			m_nColorMatchingPoint = 0;
			WisdomTempleStep step = m_wisdomTemple.GetStep(m_nStepNo);
			int nType = step.type;
			switch (nType)
			{
			case 1:
				StartPuzzle();
				break;
			case 2:
				StartQuiz();
				break;
			default:
				throw new Exception("단계타입이 유효하지 않습니다. nType = " + nType);
			}
		}
	}

	private void StartPuzzle()
	{
		m_nCurrentPuzzleId = m_wisdomTemple.SelectPuzzle().id;
		switch (m_nCurrentPuzzleId)
		{
		case 1:
			StartPuzzle_ColorMatching();
			break;
		case 2:
			StartPuzzle_FindTreasureBox();
			break;
		default:
			throw new Exception("퍼즐ID가 유효하지 않습니다. m_nCurrentPuzzleId = " + m_nCurrentPuzzleId);
		}
	}

	private void StartPuzzle_ColorMatching()
	{
		int nMaxRow = m_wisdomTemple.arrangePositionMaxRow;
		int nMaxCol = m_wisdomTemple.arrangePositionMaxCol;
		List<PDWisdomTempleColorMatchingObjectInstance> colorMatchingObjectInsts = new List<PDWisdomTempleColorMatchingObjectInstance>();
		for (int nRow = 0; nRow <= nMaxRow; nRow++)
		{
			for (int nCol = 0; nCol <= nMaxCol; nCol++)
			{
				HashSet<int> excludeIds = new HashSet<int>();
				WisdomTempleColorMatchingObjectInstance prevRowObject = GetColorMatchingObjectInstance_ByRowAndCol(nRow - 1, nCol);
				if (prevRowObject != null)
				{
					excludeIds.Add(prevRowObject.obj.id);
				}
				WisdomTempleColorMatchingObjectInstance prevColObject = GetColorMatchingObjectInstance_ByRowAndCol(nRow, nCol - 1);
				if (prevColObject != null)
				{
					excludeIds.Add(prevColObject.obj.id);
				}
				WisdomTempleColorMatchingObjectInstance objectInst = null;
				objectInst = ((excludeIds.Count <= 0) ? CreateColorMatchingObject_ColorMatchingStart(m_wisdomTemple.SelectRandomCreateColorMatchingObject(), m_wisdomTemple.GetArrangePosition(nRow, nCol)) : CreateColorMatchingObject_ColorMatchingStart(m_wisdomTemple.SelectRandomCreateColorMatchingObject_ToExclude(excludeIds), m_wisdomTemple.GetArrangePosition(nRow, nCol)));
				AddColorMatchingObjectInstance(objectInst);
				colorMatchingObjectInsts.Add(objectInst.ToPDWisdomTempleColorMatchingObjectInstance());
			}
		}
		ServerEvent.SendWisdomTempleStepStart(m_hero.account.peer, m_nStepNo, m_nCurrentPuzzleId, 0, new List<PDWisdomTempleMonsterInstance>().ToArray(), colorMatchingObjectInsts.ToArray());
		StartColorMatchingMonsterCreationTimer();
	}

	private void StartColorMatchingMonsterCreationTimer()
	{
		int nDuration = m_wisdomTemple.colorMatchingMonsterSpawnInterval * 1000;
		m_colorMatchingMonsterCreationTimer = new Timer(OnColorMatchingMonsterCreationTimerTick);
		m_colorMatchingMonsterCreationTimer.Change(nDuration, -1);
	}

	private void OnColorMatchingMonsterCreationTimerTick(object state)
	{
		AddWork(new SFAction(CreateColorMatchingMonster), bGlobalLockRequired: false);
	}

	private void CreateColorMatchingMonster()
	{
		if (m_nStatus == 2 && m_nCurrentPuzzleId == 1)
		{
			WisdomTempleMonsterAttrFactor monsterAttrFactor = m_wisdomTemple.GetMonsterAttrFactor(m_nHeroLevel);
			WisdomTempleColorMatchingMonsterInstance monsterInst = new WisdomTempleColorMatchingMonsterInstance();
			monsterInst.Init(this, monsterAttrFactor, m_wisdomTemple.colorMatchingMonsterArrange);
			AddMonster(monsterInst);
			ServerEvent.SendWisdomTempleColorMatchingMonsterCreated(m_hero.account.peer, (PDWisdomTempleColorMatchingMonsterInstance)monsterInst.ToPDMonsterInstance(DateTimeUtil.currentTime));
		}
	}

	public void AddColorMatchingObjectInstance(WisdomTempleColorMatchingObjectInstance colorMatchingObjectInstance)
	{
		int nRow = colorMatchingObjectInstance.arrangePosition.row;
		int nCol = colorMatchingObjectInstance.arrangePosition.col;
		m_colorMatchingObjectInsts[nRow, nCol] = colorMatchingObjectInstance;
	}

	public WisdomTempleColorMatchingObjectInstance GetColorMatchingObjectInstance_ByRowAndCol(int nRow, int nCol)
	{
		if (nRow < 0 || nRow >= m_colorMatchingObjectInsts.GetLength(0))
		{
			return null;
		}
		if (nCol < 0 || nCol >= m_colorMatchingObjectInsts.GetLength(1))
		{
			return null;
		}
		return m_colorMatchingObjectInsts[nRow, nCol];
	}

	public WisdomTempleColorMatchingObjectInstance GetColorMatchingObjectInstance_ByInstanceId(long lnInstanceId)
	{
		WisdomTempleColorMatchingObjectInstance[,] colorMatchingObjectInsts = m_colorMatchingObjectInsts;
		foreach (WisdomTempleColorMatchingObjectInstance inst in colorMatchingObjectInsts)
		{
			if (inst != null && inst.instanceId == lnInstanceId)
			{
				return inst;
			}
		}
		return null;
	}

	private WisdomTempleColorMatchingObjectInstance CreateColorMatchingObject_ColorMatchingStart(WisdomTempleColorMatchingObject colorMatchingObject, WisdomTempleArrangePosition arrangePosition)
	{
		WisdomTempleColorMatchingObjectInstance inst = new WisdomTempleColorMatchingObjectInstance();
		inst.Init(this, colorMatchingObject, arrangePosition);
		return inst;
	}

	private WisdomTempleColorMatchingObjectInstance CreateColorMatchingObject(WisdomTempleArrangePosition arrangePosition)
	{
		WisdomTempleColorMatchingObjectInstance inst = new WisdomTempleColorMatchingObjectInstance();
		inst.Init(this, m_wisdomTemple.SelectRandomCreateColorMatchingObject(), arrangePosition);
		return inst;
	}

	private void ClearColorMatchingObject()
	{
		for (int nRow = 0; nRow <= m_wisdomTemple.arrangePositionMaxRow; nRow++)
		{
			for (int nCol = 0; nCol <= m_wisdomTemple.arrangePositionMaxCol; nCol++)
			{
				WisdomTempleColorMatchingObjectInstance objectInst = GetColorMatchingObjectInstance_ByRowAndCol(nRow, nCol);
				if (objectInst != null)
				{
					objectInst.interactionHero?.CancelWisdomTempleObjectInteraction(bSendEvent: true);
					objectInst.Release();
				}
				m_colorMatchingObjectInsts[nRow, nCol] = null;
			}
		}
	}

	private void ClearPuzzleRewardObject()
	{
		foreach (WisdomTemplePuzzleRewardObjectInstance objectInst in m_puzzleRewardObjectInsts.Values)
		{
			objectInst.interactionHero?.CancelWisdomTempleObjectInteraction(bSendEvent: true);
			objectInst.Release();
		}
		m_puzzleRewardObjectInsts.Clear();
	}

	public void CheckColorMatchingObject(ICollection<PDWisdomTempleColorMatchingObjectInstance> createdColorMatchingObjectInsts)
	{
		int nMaxRow = m_wisdomTemple.arrangePositionMaxRow;
		int nMaxCol = m_wisdomTemple.arrangePositionMaxCol;
		HashSet<WisdomTempleColorMatchingObjectInstance> changedColorMatchingObjectInstances = new HashSet<WisdomTempleColorMatchingObjectInstance>();
		for (int nRow3 = 0; nRow3 <= nMaxRow; nRow3++)
		{
			bool bIsSame = true;
			int nBeforeObjectId = 0;
			for (int nCol2 = 0; nCol2 <= nMaxCol; nCol2++)
			{
				if (!bIsSame)
				{
					break;
				}
				WisdomTempleColorMatchingObjectInstance inst = GetColorMatchingObjectInstance_ByRowAndCol(nRow3, nCol2);
				if (nBeforeObjectId == 0)
				{
					nBeforeObjectId = inst.obj.id;
				}
				else if (nBeforeObjectId != inst.obj.id)
				{
					bIsSame = false;
				}
			}
			if (bIsSame)
			{
				for (int nCol = 0; nCol <= nMaxCol; nCol++)
				{
					changedColorMatchingObjectInstances.Add(GetColorMatchingObjectInstance_ByRowAndCol(nRow3, nCol));
				}
				m_nColorMatchingPoint += m_wisdomTemple.colorMatchingPoint;
			}
		}
		for (int nCol3 = 0; nCol3 <= nMaxCol; nCol3++)
		{
			bool bIsSame2 = true;
			int nBeforeObjectId2 = 0;
			for (int nRow2 = 0; nRow2 <= nMaxRow; nRow2++)
			{
				if (!bIsSame2)
				{
					break;
				}
				WisdomTempleColorMatchingObjectInstance inst2 = GetColorMatchingObjectInstance_ByRowAndCol(nRow2, nCol3);
				if (nBeforeObjectId2 == 0)
				{
					nBeforeObjectId2 = inst2.obj.id;
				}
				else if (nBeforeObjectId2 != inst2.obj.id)
				{
					bIsSame2 = false;
				}
			}
			if (bIsSame2)
			{
				for (int nRow = 0; nRow <= nMaxRow; nRow++)
				{
					changedColorMatchingObjectInstances.Add(GetColorMatchingObjectInstance_ByRowAndCol(nRow, nCol3));
				}
				m_nColorMatchingPoint += m_wisdomTemple.colorMatchingPoint;
			}
		}
		foreach (WisdomTempleColorMatchingObjectInstance colorMatchingObjectInst in changedColorMatchingObjectInstances)
		{
			WisdomTempleColorMatchingObjectInstance inst3 = CreateColorMatchingObject(colorMatchingObjectInst.arrangePosition);
			colorMatchingObjectInst.Release();
			AddColorMatchingObjectInstance(inst3);
			createdColorMatchingObjectInsts.Add(inst3.ToPDWisdomTempleColorMatchingObjectInstance());
		}
	}

	public void CheckColorMatchingPoint()
	{
		if (m_nColorMatchingPoint >= m_wisdomTemple.colorMatchingObjectivePoint)
		{
			ClearColorMatchingObject();
			int nObjectCreateRow = m_wisdomTemple.arrangePositionMaxRow / 2;
			int nObjectCreateCol = m_wisdomTemple.arrangePositionMaxCol / 2;
			WisdomTempleArrangePosition rewardObjectCreateArrangePosition = m_wisdomTemple.GetArrangePosition(nObjectCreateRow, nObjectCreateCol);
			CompletePuzzle(rewardObjectCreateArrangePosition);
		}
	}

	private void StartPuzzle_FindTreasureBox()
	{
		int nSuccessRow = SFRandom.NextInt(0.0, m_wisdomTemple.arrangePositionMaxRow);
		int nSuccessCol = SFRandom.NextInt(0.0, m_wisdomTemple.arrangePositionMaxCol);
		List<PDWisdomTempleMonsterInstance> monsterInsts = new List<PDWisdomTempleMonsterInstance>();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		WisdomTempleMonsterAttrFactor monsterAttrFactor = m_wisdomTemple.GetMonsterAttrFactor(m_nHeroLevel);
		foreach (WisdomTempleArrangePosition arrangePosition in m_wisdomTemple.arrangePositions)
		{
			WisdomTempleTreasureBoxMonsterInstance monsterInst = CreateTreasureBoxMonster(monsterAttrFactor, arrangePosition);
			AddMonster(monsterInst);
			monsterInsts.Add((PDWisdomTempleMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
			if (monsterInst.arrangePosition.row == nSuccessRow && monsterInst.arrangePosition.col == nSuccessCol)
			{
				m_lnRealTreasureBoxInstanceId = monsterInst.instanceId;
			}
		}
		ServerEvent.SendWisdomTempleStepStart(m_hero.account.peer, m_nStepNo, m_nCurrentPuzzleId, 0, monsterInsts.ToArray(), new List<PDWisdomTempleColorMatchingObjectInstance>().ToArray());
	}

	private WisdomTempleTreasureBoxMonsterInstance CreateTreasureBoxMonster(WisdomTempleMonsterAttrFactor monsterAttrFactor, WisdomTempleArrangePosition arrangePosition)
	{
		WisdomTempleTreasureBoxMonsterInstance inst = new WisdomTempleTreasureBoxMonsterInstance();
		inst.Init(this, monsterAttrFactor, m_wisdomTemple.findTreasureBoxMonsterArrange, arrangePosition);
		return inst;
	}

	private void CompletePuzzle(WisdomTempleArrangePosition rewardObjectCreateArrangePosition)
	{
		MonsterInstance[] array = m_monsterInsts.Values.ToArray();
		foreach (MonsterInstance monsterInst in array)
		{
			RemoveMonster(monsterInst, bSendEvent: true);
		}
		long lnAcquisitionExp = m_wisdomTemple.GetPuzzleReward(m_hero.level).expReward?.value ?? 0;
		if (lnAcquisitionExp > 0)
		{
			lnAcquisitionExp = (long)Math.Floor((float)lnAcquisitionExp * Cache.instance.GetWorldLevelExpFactor(m_hero.level));
			m_hero.AddExp(lnAcquisitionExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		}
		if (lnAcquisitionExp > 0)
		{
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_hero));
			dbWork.Schedule();
			try
			{
				SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
				if (RefreshLogId())
				{
					logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWisdomTempleRewardLog(m_logId, m_hero.id, m_instanceId, DateTimeUtil.currentTime));
				}
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWisdomTempleRewardDetailLog(Guid.NewGuid(), m_logId, m_nStepNo, lnAcquisitionExp, 0, bRewardItemOwned: false, 0));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
			}
		}
		List<PDWisdomTemplePuzzleRewardObjectInstance> puzzleRewardObjectInsts = new List<PDWisdomTemplePuzzleRewardObjectInstance>();
		int nRewardObjectCount = 0;
		nRewardObjectCount = ((m_nCurrentPuzzleId == 1) ? 1 : m_wisdomTemple.GetFindTreasureBoxCount(m_nFindTreasureBoxKillCount).rewardCount);
		foreach (WisdomTemplePuzzleRewardObjectOffset puzzleRewardObjectOffset in m_wisdomTemple.GetPuzzleRewardObjectOffsets(nRewardObjectCount))
		{
			WisdomTemplePuzzleRewardObjectInstance puzzleRewardObjectInst = CreatePuzzleRewardObject(rewardObjectCreateArrangePosition, puzzleRewardObjectOffset);
			AddPuzzleRewardObject(puzzleRewardObjectInst);
			puzzleRewardObjectInsts.Add(puzzleRewardObjectInst.ToPDWisdomTemplePuzzleRewardObjectInstance());
		}
		m_nCurrentPuzzleId = 0;
		m_lnRealTreasureBoxInstanceId = 0L;
		m_nFindTreasureBoxKillCount = 0;
		ServerEvent.SendWisdomTemplePuzzleCompleted(m_hero.account.peer, lnAcquisitionExp, m_hero.level, m_hero.exp, m_hero.realMaxHP, m_hero.hp, puzzleRewardObjectInsts.ToArray());
		if (puzzleRewardObjectInsts.Count <= 0)
		{
			CompleteStep();
		}
	}

	private WisdomTemplePuzzleRewardObjectInstance CreatePuzzleRewardObject(WisdomTempleArrangePosition baseArrangePosition, WisdomTemplePuzzleRewardObjectOffset puzzleRewardObjectOffset)
	{
		WisdomTemplePuzzleRewardObjectInstance inst = new WisdomTemplePuzzleRewardObjectInstance();
		inst.Init(this, baseArrangePosition.position + puzzleRewardObjectOffset.offset);
		return inst;
	}

	private void AddPuzzleRewardObject(WisdomTemplePuzzleRewardObjectInstance puzzleRewardObjectInst)
	{
		m_puzzleRewardObjectInsts.Add(puzzleRewardObjectInst.instanceId, puzzleRewardObjectInst);
	}

	private void RemovePuzzleRewardObject(WisdomTemplePuzzleRewardObjectInstance puzzleRewardObjectInst)
	{
		if (puzzleRewardObjectInst == null)
		{
			throw new ArgumentNullException("puzzleRewardObjectInst");
		}
		puzzleRewardObjectInst.Release();
		m_puzzleRewardObjectInsts.Remove(puzzleRewardObjectInst.instanceId);
	}

	public WisdomTemplePuzzleRewardObjectInstance GetPuzzleRewardObject(long lnInstanceId)
	{
		if (!m_puzzleRewardObjectInsts.TryGetValue(lnInstanceId, out var value))
		{
			return null;
		}
		return value;
	}

	private void CheckAroundTreasureBox(WisdomTempleArrangePosition arrangePosition)
	{
		bool bExistAroundRealTreasureBox = false;
		int nBaseRow = arrangePosition.row;
		int nBaseCol = arrangePosition.col;
		WisdomTempleArrangePosition realTreasureBoxArrangePosition = ((WisdomTempleTreasureBoxMonsterInstance)GetMonster(m_lnRealTreasureBoxInstanceId)).arrangePosition;
		int nRealTreasureBoxRow = realTreasureBoxArrangePosition.row;
		int nRealTreasureBoxCol = realTreasureBoxArrangePosition.col;
		if (nRealTreasureBoxRow == nBaseRow)
		{
			if (nRealTreasureBoxCol >= nBaseCol - 1 && nRealTreasureBoxCol <= nBaseCol + 1)
			{
				bExistAroundRealTreasureBox = true;
			}
		}
		else if (nRealTreasureBoxCol == nBaseCol && nRealTreasureBoxRow >= nBaseRow - 1 && nRealTreasureBoxRow <= nBaseRow + 1)
		{
			bExistAroundRealTreasureBox = true;
		}
		ServerEvent.SendWisdomTempleFakeTreasureBoxKill(m_hero.account.peer, nBaseRow, nBaseCol, bExistAroundRealTreasureBox);
	}

	public void OnColorMatchingObjectInteractionComplete(WisdomTempleColorMatchingObjectInstance colorMatchingObjectInst)
	{
		if (colorMatchingObjectInst == null)
		{
			throw new ArgumentNullException("colorMatchingObjectInst");
		}
		WisdomTempleColorMatchingObjectInstance newColorMatchingObjectInst = new WisdomTempleColorMatchingObjectInstance();
		newColorMatchingObjectInst.Init(this, m_wisdomTemple.SelectRandomCreateColorMatchingObject_ToExclude(new List<int> { colorMatchingObjectInst.obj.id }), colorMatchingObjectInst.arrangePosition);
		colorMatchingObjectInst.Release();
		AddColorMatchingObjectInstance(newColorMatchingObjectInst);
		ServerEvent.SendWisdomTempleColorMatchingObjectInteractionFinished(m_hero.account.peer, newColorMatchingObjectInst.ToPDWisdomTempleColorMatchingObjectInstance());
	}

	public void OnPuzzleRewardObjectInteractionComplete(WisdomTemplePuzzleRewardObjectInstance puzzleRewardObjectInst)
	{
		if (puzzleRewardObjectInst == null)
		{
			throw new ArgumentNullException("puzzleRewardObjectInst");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		PDItemBooty itemBooty = null;
		Mail mail = null;
		ItemReward itemReward = m_wisdomTemple.SelectPuzzleReward();
		if (itemReward != null)
		{
			Item item = itemReward.item;
			int nCount = itemReward.count;
			bool bOwned = itemReward.owned;
			int nRewardItemRemainCount = m_hero.AddItem(item, bOwned, nCount, changedInventorySlots);
			itemBooty = new PDItemBooty();
			itemBooty.id = item.id;
			itemBooty.count = nCount;
			itemBooty.owned = bOwned;
			if (nRewardItemRemainCount > 0)
			{
				mail = Mail.Create("MAIL_REWARD_N_17", "MAIL_REWARD_D_17", currentTime);
				mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainCount, bOwned));
				m_hero.AddMail(mail, bSendEvent: true);
			}
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
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
				if (RefreshLogId())
				{
					logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWisdomTempleRewardLog(m_logId, m_hero.id, m_instanceId, currentTime));
				}
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWisdomTempleRewardDetailLog(Guid.NewGuid(), m_logId, m_nStepNo, 0L, itemBooty.id, itemBooty.owned, itemBooty.count));
				logWork.Schedule();
			}
			catch (Exception ex)
			{
				SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
			}
		}
		RemovePuzzleRewardObject(puzzleRewardObjectInst);
		ServerEvent.SendWisdomTemplePuzzleRewardObjectInteractionFinished(m_hero.account.peer, itemBooty, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
		if (m_puzzleRewardObjectInsts.Count <= 0)
		{
			CompleteStep();
		}
	}

	private void StartQuiz()
	{
		WisdomTempleStep step = m_wisdomTemple.GetStep(m_nStepNo);
		WisdomTempleQuizPoolEntry quizPoolEntry = step.SelectQuizPoolEntry();
		List<WisdomTempleQuizMonsterPosition> monsterPositions = step.quizMonsterPositions;
		WisdomTempleQuizRightAnswerPoolEntry quizRightAnswerPoolEntry = quizPoolEntry.SelectRightAnswerPoolEntry();
		List<WisdomTempleQuizWrongAnswerPoolEntry> quizWrongAnserPoolEntries = quizPoolEntry.SelectWrongAnswerPoolEntries(monsterPositions.Count - 1);
		int nRightAnswerMonsterCreationNo = SFRandom.NextInt(0.0, monsterPositions.Count - 1);
		int nCreationNo = 0;
		List<PDWisdomTempleQuizMonsterInstance> monsterInsts = new List<PDWisdomTempleQuizMonsterInstance>();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		WisdomTempleMonsterAttrFactor monsterAttrFactor = m_wisdomTemple.GetMonsterAttrFactor(m_nHeroLevel);
		foreach (WisdomTempleQuizMonsterPosition monsterPosition in monsterPositions)
		{
			WisdomTempleQuizMonsterInstance monsterInst = null;
			if (nCreationNo == nRightAnswerMonsterCreationNo)
			{
				monsterInst = CreateQuizRightAnswerMonster(quizRightAnswerPoolEntry, monsterAttrFactor, monsterPosition.arrangePosition);
			}
			else
			{
				WisdomTempleQuizWrongAnswerPoolEntry wrongPolEntry = quizWrongAnserPoolEntries.FirstOrDefault();
				monsterInst = CreateQuizWrongAnswerMonster(wrongPolEntry, monsterAttrFactor, monsterPosition.arrangePosition);
				quizWrongAnserPoolEntries.Remove(wrongPolEntry);
			}
			AddMonster(monsterInst);
			monsterInsts.Add((PDWisdomTempleQuizMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
			nCreationNo++;
		}
		ServerEvent.SendWisdomTempleStepStart(m_hero.account.peer, m_nStepNo, 0, quizPoolEntry.no, monsterInsts.ToArray(), new List<PDWisdomTempleColorMatchingObjectInstance>().ToArray());
	}

	private WisdomTempleQuizRightAnswerMonsterInstance CreateQuizRightAnswerMonster(WisdomTempleQuizRightAnswerPoolEntry entry, WisdomTempleMonsterAttrFactor monsterAttrFactor, WisdomTempleArrangePosition arrangePosition)
	{
		WisdomTempleQuizRightAnswerMonsterInstance inst = new WisdomTempleQuizRightAnswerMonsterInstance();
		inst.Init(this, entry, monsterAttrFactor, arrangePosition);
		return inst;
	}

	private WisdomTempleQuizWrongAnswerMonsterInstance CreateQuizWrongAnswerMonster(WisdomTempleQuizWrongAnswerPoolEntry entry, WisdomTempleMonsterAttrFactor monsterAttrFactor, WisdomTempleArrangePosition arrangePosition)
	{
		WisdomTempleQuizWrongAnswerMonsterInstance inst = new WisdomTempleQuizWrongAnswerMonsterInstance();
		inst.Init(this, entry, monsterAttrFactor, arrangePosition);
		return inst;
	}

	private void CompleteStep()
	{
		MonsterInstance[] array = m_monsterInsts.Values.ToArray();
		foreach (MonsterInstance monsterInst in array)
		{
			RemoveMonster(monsterInst, bSendEvent: true);
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		long lnAcquisitionExp = m_wisdomTemple.GetStepReward(m_hero.level).expReward?.value ?? 0;
		if (lnAcquisitionExp > 0)
		{
			lnAcquisitionExp = (long)Math.Floor((float)lnAcquisitionExp * Cache.instance.GetWorldLevelExpFactor(m_hero.level));
			m_hero.AddExp(lnAcquisitionExp, bSendExpAcquisitionEvent: false, bSaveToDBForLevelUp: false);
		}
		WisdomTempleStep step = m_wisdomTemple.GetStep(m_nStepNo);
		ItemReward itemReward = step.itemReward;
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		PDItemBooty itemBooty = null;
		Mail mail = null;
		if (itemReward != null)
		{
			Item item = itemReward.item;
			int nCount = itemReward.count;
			bool bOwned = itemReward.owned;
			int nRewardItemRemainCount = m_hero.AddItem(item, bOwned, nCount, changedInventorySlots);
			itemBooty = new PDItemBooty();
			itemBooty.id = item.id;
			itemBooty.count = nCount;
			itemBooty.owned = bOwned;
			if (nRewardItemRemainCount > 0)
			{
				mail = Mail.Create("MAIL_REWARD_N_17", "MAIL_REWARD_D_17", currentTime);
				mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainCount, bOwned));
				m_hero.AddMail(mail, bSendEvent: true);
			}
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
		if (lnAcquisitionExp > 0)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Level(m_hero));
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
			if (RefreshLogId())
			{
				logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWisdomTempleRewardLog(m_logId, m_hero.id, m_instanceId, currentTime));
			}
			logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWisdomTempleRewardDetailLog(Guid.NewGuid(), m_logId, m_nStepNo, lnAcquisitionExp, itemBooty?.id ?? 0, itemBooty?.owned ?? false, itemBooty?.count ?? 0));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
		ServerEvent.SendWisdomTempleStepCompleted(m_hero.account.peer, lnAcquisitionExp, m_hero.level, m_hero.exp, m_hero.realMaxHP, m_hero.hp, itemBooty, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
		if (m_nStepNo >= m_wisdomTemple.stepCount)
		{
			Clear();
		}
		else
		{
			StartStepStartDelayTimeTimer();
		}
	}

	protected override void OnHeroDead(Hero hero)
	{
		base.OnHeroDead(hero);
		Fail();
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus != 2 || !monsterInst.isDead)
		{
			return;
		}
		WisdomTempleStep step = m_wisdomTemple.GetStep(m_nStepNo);
		if (monsterInst.monsterInstanceType == MonsterInstanceType.WisdomTempleColorMatchingMonster)
		{
			if (step.type != 1 || m_nCurrentPuzzleId != 1)
			{
				return;
			}
			List<PDWisdomTempleColorMatchingObjectInstance> createdColorMatchingObjectInsts = new List<PDWisdomTempleColorMatchingObjectInstance>();
			m_nColorMatchingPoint += m_wisdomTemple.colorMatchingMonsterKillPoint;
			int nColorMatchingObjectivePoint = m_wisdomTemple.colorMatchingObjectivePoint;
			WisdomTempleColorMatchingObjectInstance[,] colorMatchingObjectInsts = m_colorMatchingObjectInsts;
			foreach (WisdomTempleColorMatchingObjectInstance objectInst in colorMatchingObjectInsts)
			{
				WisdomTempleColorMatchingObjectInstance colorMatchingObjectInst = CreateColorMatchingObject(objectInst.arrangePosition);
				objectInst.Release();
				AddColorMatchingObjectInstance(colorMatchingObjectInst);
				createdColorMatchingObjectInsts.Add(colorMatchingObjectInst.ToPDWisdomTempleColorMatchingObjectInstance());
			}
			ServerEvent.SendWisdomTempleColorMatchingMonsterkill(m_hero.account.peer, m_nColorMatchingPoint, createdColorMatchingObjectInsts.ToArray());
			if (m_nColorMatchingPoint >= nColorMatchingObjectivePoint)
			{
				int nObjectCreateRow = m_wisdomTemple.arrangePositionMaxRow / 2;
				int nObjectCreateCol = m_wisdomTemple.arrangePositionMaxCol / 2;
				WisdomTempleArrangePosition rewardObjectCreateArrangePosition = m_wisdomTemple.GetArrangePosition(nObjectCreateRow, nObjectCreateCol);
				CompletePuzzle(rewardObjectCreateArrangePosition);
			}
		}
		else if (monsterInst.monsterInstanceType == MonsterInstanceType.WisdomTempleTreasureBoxMonster)
		{
			if (step.type == 1 && m_nCurrentPuzzleId == 2)
			{
				m_nFindTreasureBoxKillCount++;
				WisdomTempleTreasureBoxMonsterInstance findTreasureBoxMonsterInst = (WisdomTempleTreasureBoxMonsterInstance)monsterInst;
				if (m_lnRealTreasureBoxInstanceId == monsterInst.instanceId)
				{
					CompletePuzzle(findTreasureBoxMonsterInst.arrangePosition);
				}
				else
				{
					CheckAroundTreasureBox(findTreasureBoxMonsterInst.arrangePosition);
				}
			}
		}
		else if (monsterInst.monsterInstanceType == MonsterInstanceType.WisdomTempleQuizMonster)
		{
			if (step.type == 2)
			{
				WisdomTempleQuizMonsterInstance quizAnswerMonsterInst = (WisdomTempleQuizMonsterInstance)monsterInst;
				if (quizAnswerMonsterInst.isRightMonster)
				{
					CompleteStep();
				}
				else
				{
					FailQuiz();
				}
			}
		}
		else
		{
			if (monsterInst.monsterInstanceType != MonsterInstanceType.WisdomTempleBossMonster)
			{
				return;
			}
			DateTimeOffset currentTime = DateTimeUtil.currentTime;
			List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
			PDItemBooty itemBooty = null;
			Mail mail = null;
			ItemReward itemReward = m_wisdomTemple.bossMonsterKillItemReward;
			if (itemReward != null)
			{
				Item item = itemReward.item;
				int nCount = itemReward.count;
				bool bOwned = itemReward.owned;
				int nRewardItemRemainCount = m_hero.AddItem(item, bOwned, nCount, changedInventorySlots);
				itemBooty = new PDItemBooty();
				itemBooty.id = item.id;
				itemBooty.count = nCount;
				itemBooty.owned = bOwned;
				if (nRewardItemRemainCount > 0)
				{
					mail = Mail.Create("MAIL_REWARD_N_17", "MAIL_REWARD_D_17", currentTime);
					mail.AddAttachmentWithNo(new MailAttachment(item, nRewardItemRemainCount, bOwned));
					m_hero.AddMail(mail, bSendEvent: true);
				}
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
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
					if (RefreshLogId())
					{
						logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWisdomTempleRewardLog(m_logId, m_hero.id, m_instanceId, currentTime));
					}
					logWork.AddSqlCommand(GameLogDac.CSC_AddHeroWisdomTempleRewardDetailLog(Guid.NewGuid(), m_logId, m_nStepNo, 0L, itemBooty.id, itemBooty.owned, itemBooty.count));
					logWork.Schedule();
				}
				catch (Exception ex)
				{
					SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
				}
			}
			ServerEvent.SendWisdomTempleBossMonsterKill(m_hero.account.peer, itemBooty, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
			Clear();
		}
	}

	private void FailQuiz()
	{
		MonsterInstance[] array = m_monsterInsts.Values.ToArray();
		foreach (MonsterInstance monsterInst in array)
		{
			RemoveMonster(monsterInst, bSendEvent: true);
		}
		ServerEvent.SendWisdomTempleQuizFail(m_hero.account.peer);
		int nDuration = m_wisdomTemple.bossMonsterSpawnDelay * 1000;
		m_bossMonsterCreationTimer = new Timer(OnBossMonsterCreationTimerTick);
		m_bossMonsterCreationTimer.Change(nDuration, -1);
	}

	private void OnBossMonsterCreationTimerTick(object state)
	{
		AddWork(new SFAction(CreateBossMonster), bGlobalLockRequired: false);
	}

	private void CreateBossMonster()
	{
		if (m_nStatus == 2)
		{
			m_nStepNo = 0;
			WisdomTempleMonsterAttrFactor monsterAttrFactor = m_wisdomTemple.GetMonsterAttrFactor(m_nHeroLevel);
			WisdomTempleBossMonsterInstance monsterInst = new WisdomTempleBossMonsterInstance();
			monsterInst.Init(this, monsterAttrFactor, m_wisdomTemple.bossMonsterArrange);
			AddMonster(monsterInst);
			ServerEvent.SendWisdomTempleBossMonsterCreated(m_hero.account.peer, (PDWisdomTempleBossMonsterInstance)monsterInst.ToPDMonsterInstance(DateTimeUtil.currentTime));
		}
	}

	private void Clear()
	{
		if (m_nStatus == 2)
		{
			Finish(3);
			if (!m_hero.wisdomTempleCleared)
			{
				m_hero.wisdomTempleCleared = true;
				SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(m_hero.id);
				dbWork.AddSqlCommand(GameDac.CSC_UpdateHero_WisdomTempleCleared(m_hero.id, bWisdomTempleCleared: true));
				dbWork.Schedule();
			}
			ServerEvent.SendWisdomTempleClear(m_hero.account.peer);
			int nDuration = m_wisdomTemple.exitDelayTime * 1000;
			m_exitDelayTimeTimer = new Timer(OnExitDelayTimeTimerTick);
			m_exitDelayTimeTimer.Change(nDuration, -1);
		}
	}

	private void Fail()
	{
		if (m_nStatus == 2)
		{
			Finish(4);
			ServerEvent.SendWisdomTempleFail(m_hero.account.peer);
			int nDuration = m_wisdomTemple.exitDelayTime * 1000;
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
			ServerEvent.SendWisdomTempleBanished(m_hero.account.peer, m_hero.previousContinentId, m_hero.previousNationId, m_hero.hp);
		}
	}

	public bool RefreshLogId()
	{
		if (m_nLastLogRegistrationStepNo == m_nStepNo)
		{
			return false;
		}
		m_nLastLogRegistrationStepNo = m_nStepNo;
		m_logId = Guid.NewGuid();
		return true;
	}

	public void Finish(int nStatus)
	{
		ClearColorMatchingObject();
		ClearPuzzleRewardObject();
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeStepStartDelayTimeTimer();
		DisposeColorMatchingMonsterCreationTimer();
		DisposeBossMonsterCreationTimer();
		m_endTime = DateTimeUtil.currentTime;
		if (m_nStatus == 2)
		{
			m_nPlayTime = (int)Math.Floor((m_endTime - m_startTime).TotalSeconds);
		}
		m_nStatus = nStatus;
	}

	protected override void ReleaseInternal()
	{
		ClearColorMatchingObject();
		ClearPuzzleRewardObject();
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		DisposeStepStartDelayTimeTimer();
		DisposeColorMatchingMonsterCreationTimer();
		DisposeBossMonsterCreationTimer();
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

	private void DisposeColorMatchingMonsterCreationTimer()
	{
		if (m_colorMatchingMonsterCreationTimer != null)
		{
			m_colorMatchingMonsterCreationTimer.Dispose();
			m_colorMatchingMonsterCreationTimer = null;
		}
	}

	private void DisposeBossMonsterCreationTimer()
	{
		if (m_bossMonsterCreationTimer != null)
		{
			m_bossMonsterCreationTimer.Dispose();
			m_bossMonsterCreationTimer = null;
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
