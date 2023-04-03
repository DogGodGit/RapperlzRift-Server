using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimInstance : Place
{
	public const int kStatus_Init = 0;

	public const int kStatus_PlayWaiting = 1;

	public const int kStatus_Playing = 2;

	public const int kStatus_Clear = 3;

	public const int kStatus_Fail = 4;

	public const int kArrangeDirectionCount = 4;

	public const int kArrange_Up = 1;

	public const int kArrange_Left = 2;

	public const int kArrange_Down = 3;

	public const int kArrange_Right = 4;

	public const float kArrangeInterval = 2.5f;

	public const int kArrangeRotationMaxCount = 2;

	private RuinsReclaim m_ruinsReclaim;

	private RuinsReclaimOpenSchedule m_openSchedule;

	private int m_nAverageHeroLevel;

	private int m_nStatus;

	private int m_nStepNo;

	private int m_nWaveNo;

	private DateTimeOffset m_creationTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_startTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_endTime = DateTimeOffset.MinValue;

	private int m_nPlayTime;

	private Dictionary<int, RuinsReclaimPortal> m_activationPortals = new Dictionary<int, RuinsReclaimPortal>();

	private RuinsReclaimDebuffArea m_debuffArea;

	private HashSet<RuinsReclaimTrapInstance> m_trapInsts = new HashSet<RuinsReclaimTrapInstance>();

	private Dictionary<long, RuinsReclaimRewardObjectInstance> m_rewardObjectInsts = new Dictionary<long, RuinsReclaimRewardObjectInstance>();

	private Dictionary<long, RuinsReclaimMonsterTransformationCancelObjectInstance> m_monsterTransformationCancelObjectInsts = new Dictionary<long, RuinsReclaimMonsterTransformationCancelObjectInstance>();

	private RuinsReclaimStepWaveSkillEffect m_waveSkillEffect;

	private RuinsReclaimMonsterInstance m_lastBossMonsterInst;

	private Dictionary<Guid, MonsterReceivedDamage> m_LastSummonMonsterReceivedDamages = new Dictionary<Guid, MonsterReceivedDamage>();

	private Timer m_playWaitingTimer;

	private Timer m_limitTimeTimer;

	private Timer m_exitDelayTimeTimer;

	public override PlaceType placeType => PlaceType.RuinsReclaim;

	public override Location location => m_ruinsReclaim;

	public override int locationParam => 0;

	public override Rect3D mapRect => m_ruinsReclaim.mapRect;

	public override bool interestManaged => false;

	public override bool ownershipManaged => !isFinished;

	public override bool battleEnabled => m_nStatus == 2;

	public override bool pvpEnabled => false;

	public override bool distortionScrollUseEnabled => true;

	public override bool isPartyExpBuffEnabled => false;

	public override bool isExpScrollBuffEnabled => false;

	public override bool isExpLevelPenaltyEnabled => false;

	public override bool isWorldLevelExpBuffEnabled => true;

	public RuinsReclaim ruinsReclaim => m_ruinsReclaim;

	public int status => m_nStatus;

	public int stepNo => m_nStepNo;

	public int waveNo => m_nWaveNo;

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

	public void Init(RuinsReclaimOpenSchedule openSchedule, DateTimeOffset time, int nAverageHeroLevel)
	{
		if (openSchedule == null)
		{
			throw new ArgumentNullException("openSchedule");
		}
		if (nAverageHeroLevel < 0)
		{
			throw new ArgumentOutOfRangeException("nAverageHeroLevel");
		}
		m_ruinsReclaim = openSchedule.ruinsReclaim;
		m_openSchedule = openSchedule;
		m_nAverageHeroLevel = nAverageHeroLevel;
		m_creationTime = time;
		InitPlace();
		try
		{
			SFSqlStandaloneWork logWork = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimCreationLog(m_instanceId, m_openSchedule.id, time));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
		m_nStatus = 1;
		foreach (RuinsReclaimTrap trap in m_ruinsReclaim.traps)
		{
			RuinsReclaimTrapInstance trapInst = CreateTrap(trap);
			m_trapInsts.Add(trapInst);
		}
		int nDuration = m_ruinsReclaim.startDelayTime * 1000;
		m_playWaitingTimer = new Timer(OnPlayWaitingTimerTick);
		m_playWaitingTimer.Change(nDuration, -1);
	}

	public float GetRemainingStartTime(DateTimeOffset time)
	{
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fStartTime = (float)m_creationTime.TimeOfDay.TotalSeconds + (float)m_ruinsReclaim.startDelayTime;
		return Math.Max(fStartTime - fTime, 0f);
	}

	public float GetRemainingLimitTime(DateTimeOffset time)
	{
		if (m_nStatus == 1)
		{
			return m_ruinsReclaim.limitTime;
		}
		float fTime = (float)time.TimeOfDay.TotalSeconds;
		float fEndTime = (float)m_startTime.TimeOfDay.TotalSeconds + (float)m_ruinsReclaim.limitTime;
		return Math.Max(fEndTime - fTime, 0f);
	}

	protected override void OnHeroExit(Hero hero, bool isLogOut)
	{
		base.OnHeroExit(hero, isLogOut);
		if (isLogOut)
		{
			Disqualification(hero);
		}
		_ = m_heroes.Count;
		if (!isFinished)
		{
			if (m_lastBossMonsterInst != null)
			{
				m_lastBossMonsterInst.RemoveMonsterReceivedDamage(hero.id);
			}
			RemoveSummonMonsterReceivedDamage(hero.id);
		}
	}

	private RuinsReclaimTrapInstance CreateTrap(RuinsReclaimTrap trap)
	{
		RuinsReclaimTrapInstance inst = new RuinsReclaimTrapInstance();
		inst.Init(trap, this);
		return inst;
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
		int nDuration = m_ruinsReclaim.limitTime * 1000;
		m_limitTimeTimer = new Timer(OnLimitTimeTimerTick);
		m_limitTimeTimer.Change(nDuration, -1);
		foreach (RuinsReclaimTrapInstance trapInst in m_trapInsts)
		{
			trapInst.Start();
		}
		StartNextStep();
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
		m_nWaveNo = 0;
		RuinsReclaimStep step = m_ruinsReclaim.GetStep(m_nStepNo);
		if (m_nStepNo == m_ruinsReclaim.debuffAreaActivationStepNo)
		{
			RuinsReclaimDebuffArea debuffArea = new RuinsReclaimDebuffArea();
			debuffArea.Init(this);
			m_debuffArea = debuffArea;
		}
		else if (m_nStepNo == m_ruinsReclaim.debuffAreaDeactivationStepNo)
		{
			m_debuffArea = null;
			foreach (Hero hero in m_heroes.Values)
			{
				lock (hero.syncObject)
				{
					if (hero.ruinsReclaimDebuffEffect)
					{
						hero.StopRuinsReclaimDebuffEffect(bSendEvent: true);
					}
				}
			}
		}
		RuinsReclaimPortal activationPortal = step.activationPortal;
		if (activationPortal != null)
		{
			AddActivationPortal(activationPortal);
		}
		int nDeactivationPortalId = step.deactivationPortalId;
		if (nDeactivationPortalId > 0)
		{
			RemoveActivationPortal(nDeactivationPortalId);
		}
		int nsStepType = step.type;
		switch (nsStepType)
		{
		case 1:
			StartMoveStep();
			break;
		case 2:
			StartInteractionStep();
			break;
		case 3:
			StartWaveStep();
			break;
		default:
			throw new Exception("단계타입이 유효하지 않습니다. nsStepType = " + nsStepType);
		}
	}

	private void AddActivationPortal(RuinsReclaimPortal portal)
	{
		m_activationPortals.Add(portal.id, portal);
	}

	public RuinsReclaimPortal GetActivationPortal(int nId)
	{
		if (!m_activationPortals.TryGetValue(nId, out var value))
		{
			return null;
		}
		return value;
	}

	private void RemoveActivationPortal(int nId)
	{
		m_activationPortals.Remove(nId);
	}

	private void StartMoveStep()
	{
		ServerEvent.SendRuinsReclaimStepStart(GetClientPeers(), m_nStepNo, new List<PDRuinsReclaimRewardObjectInstance>().ToArray());
	}

	protected override void OnHeroPositionChanged(Hero hero, SectorChangeInfo info, bool bSendInterestTargetChangeEvent, DateTimeOffset currentTime)
	{
		base.OnHeroPositionChanged(hero, info, bSendInterestTargetChangeEvent, currentTime);
		if (m_nStatus != 2)
		{
			return;
		}
		if (m_debuffArea != null)
		{
			if (hero.ruinsReclaimDebuffEffect)
			{
				if (!m_debuffArea.Contains(hero.position))
				{
					hero.StopRuinsReclaimDebuffEffect(bSendEvent: true);
				}
			}
			else if (m_debuffArea.Contains(hero.position))
			{
				hero.StartRuinsReclaimDebuffEffect();
			}
		}
		RuinsReclaimStep step = m_ruinsReclaim.GetStep(m_nStepNo);
		if (step.type == 1 && step.ContainsTargetPosition(hero.position))
		{
			CompleteStep();
		}
	}

	private void StartInteractionStep()
	{
		RuinsReclaimStep step = m_ruinsReclaim.GetStep(m_nStepNo);
		List<PDRuinsReclaimRewardObjectInstance> objectInsts = new List<PDRuinsReclaimRewardObjectInstance>();
		Vector3 beforCreationPosition = Vector3.zero;
		int nArrangeDirection = 0;
		int nDirectionCreationMaxCount = 1;
		int nCurrentDirectionCreationCount = 0;
		int nArrangeRotation = 0;
		foreach (RuinsReclaimObjectArrange objectArrange in step.objectArranges)
		{
			for (int i = 0; i < m_heroes.Count; i++)
			{
				if (nCurrentDirectionCreationCount >= nDirectionCreationMaxCount)
				{
					nCurrentDirectionCreationCount = 0;
					nArrangeRotation++;
					if (nArrangeRotation >= 2)
					{
						nArrangeRotation = 0;
						nDirectionCreationMaxCount++;
					}
					nArrangeDirection++;
					if (nArrangeDirection > 4)
					{
						nArrangeDirection = 1;
					}
				}
				Vector3 creationPosition = Vector3.zero;
				if (i != 0)
				{
					switch (nArrangeDirection)
					{
					case 1:
						creationPosition = beforCreationPosition + new Vector3(0f, 0f, -2.5f);
						break;
					case 2:
						creationPosition = beforCreationPosition + new Vector3(-2.5f, 0f, 0f);
						break;
					case 3:
						creationPosition = beforCreationPosition + new Vector3(0f, 0f, 2.5f);
						break;
					case 4:
						creationPosition = beforCreationPosition + new Vector3(2.5f, 0f, 0f);
						break;
					}
				}
				else
				{
					creationPosition = objectArrange.position;
				}
				RuinsReclaimRewardObjectInstance objectInst = CreateRewardObject(objectArrange, creationPosition);
				AddRewardObject(objectInst);
				objectInsts.Add(objectInst.ToPDRuinsReclaimRewardObjectInstance());
				nCurrentDirectionCreationCount++;
				beforCreationPosition = creationPosition;
			}
		}
		ServerEvent.SendRuinsReclaimStepStart(GetClientPeers(), m_nStepNo, objectInsts.ToArray());
	}

	private RuinsReclaimRewardObjectInstance CreateRewardObject(RuinsReclaimObjectArrange arrange, Vector3 position)
	{
		RuinsReclaimRewardObjectInstance inst = new RuinsReclaimRewardObjectInstance();
		inst.Init(this, arrange, position);
		return inst;
	}

	private void AddRewardObject(RuinsReclaimRewardObjectInstance objectInst)
	{
		m_rewardObjectInsts.Add(objectInst.instanceId, objectInst);
	}

	public RuinsReclaimRewardObjectInstance GetRewardObject(long lnInstanceId)
	{
		if (!m_rewardObjectInsts.TryGetValue(lnInstanceId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDRuinsReclaimRewardObjectInstance> GetPDRewardObjectInstnaces()
	{
		List<PDRuinsReclaimRewardObjectInstance> results = new List<PDRuinsReclaimRewardObjectInstance>();
		foreach (RuinsReclaimRewardObjectInstance inst in m_rewardObjectInsts.Values)
		{
			results.Add(inst.ToPDRuinsReclaimRewardObjectInstance());
		}
		return results;
	}

	public void RemoveRewardObject(RuinsReclaimRewardObjectInstance objectInst)
	{
		if (objectInst == null)
		{
			throw new ArgumentNullException("objectInst");
		}
		objectInst.Release();
		m_rewardObjectInsts.Remove(objectInst.instanceId);
	}

	private void ClearRewardObject()
	{
		foreach (RuinsReclaimRewardObjectInstance inst in m_rewardObjectInsts.Values)
		{
			Hero hero = inst.interactionHero;
			if (hero != null)
			{
				lock (hero.syncObject)
				{
					hero.CancelRuinsReclaimObjectInteraction(bSendEventToMyself: true, bSendEventToOthers: true);
				}
			}
			inst.Release();
		}
		m_rewardObjectInsts.Clear();
	}

	public void CheckRewardObject()
	{
		if (m_rewardObjectInsts.Count <= 0)
		{
			CompleteStep();
		}
	}

	public void OnRewardObjectInteractionFinish(RuinsReclaimRewardObjectInstance objectInst)
	{
		if (objectInst == null)
		{
			throw new ArgumentNullException("objectInst");
		}
		Hero hero = objectInst.interactionHero;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		RuinsReclaimObjectArrange arrange = objectInst.arrange;
		long lnAcquisitionGold = 0L;
		PDItemBooty booty = null;
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		Mail mail = null;
		lnAcquisitionGold = arrange.goldReward?.value ?? 0;
		if (lnAcquisitionGold > 0)
		{
			hero.AddGold(lnAcquisitionGold);
		}
		ItemReward itemReward = arrange.itemReward;
		if (itemReward != null)
		{
			Item item = itemReward.item;
			bool bOwned = itemReward.owned;
			int nCount = itemReward.count;
			booty = new PDItemBooty();
			booty.id = item.id;
			booty.owned = bOwned;
			booty.count = nCount;
			int nRemainingItemCount = hero.AddItem(item, bOwned, nCount, changedInventorySlots);
			if (nRemainingItemCount > 0)
			{
				mail = Mail.Create("MAIL_REWARD_N_20", "MAIL_REWARD_D_20", currentTime);
				mail.AddAttachment(new MailAttachment(item, nRemainingItemCount, bOwned));
				hero.AddMail(mail, bSendEvent: true);
			}
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(hero.id);
		if (lnAcquisitionGold > 0)
		{
			dbWork.AddSqlCommand(GameDacEx.CSC_UpdateHero_Gold(hero));
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
			logWork.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimObjectRewardLog(Guid.NewGuid(), m_instanceId, hero.id, m_nStepNo, lnAcquisitionGold, booty?.id ?? 0, booty?.owned ?? false, booty?.count ?? 0, currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		RemoveRewardObject(objectInst);
		ServerEvent.SendRuinsReclaimRewardObjectInteractionFinished(hero.account.peer, hero.gold, hero.maxGold, booty, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
		ServerEvent.SendHeroRuinsReclaimRewardObjectInteractionFinished(GetDynamicClientPeers(hero.sector, hero.id), hero.id, objectInst.instanceId);
		if (m_rewardObjectInsts.Count <= 0)
		{
			CompleteStep();
		}
	}

	private void StartWaveStep()
	{
		ServerEvent.SendRuinsReclaimStepStart(GetClientPeers(), m_nStepNo, new List<PDRuinsReclaimRewardObjectInstance>().ToArray());
		StartNextWave();
	}

	private void StartNextWave()
	{
		m_nWaveNo++;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		RuinsReclaimStep step = m_ruinsReclaim.GetStep(m_nStepNo);
		RuinsReclaimStepWave wave = step.GetWave(m_nWaveNo);
		List<PDRuinsReclaimMonsterInstance> monsterInsts = new List<PDRuinsReclaimMonsterInstance>();
		RuinsReclaimMonsterAttrFactor attrFactor = m_ruinsReclaim.GetMonsterAttrFactor(m_nAverageHeroLevel);
		foreach (RuinsReclaimMonsterArrange arrange in wave.monsterArrages)
		{
			for (int i = 0; i < arrange.monsterCount; i++)
			{
				RuinsReclaimMonsterInstance monsterInst = CreateMonster(arrange, attrFactor);
				AddMonster(monsterInst);
				monsterInsts.Add((PDRuinsReclaimMonsterInstance)monsterInst.ToPDMonsterInstance(currentTime));
				if (arrange.key == m_ruinsReclaim.lastBossArrangeKey)
				{
					m_lastBossMonsterInst = monsterInst;
				}
			}
		}
		ServerEvent.SendRuinsReclaimWaveStart(GetClientPeers(), m_nWaveNo, monsterInsts.ToArray());
		RuinsReclaimStepWaveSkill skill = wave.skill;
		if (skill != null)
		{
			RuinsReclaimStepWaveSkillEffect skillEffect = new RuinsReclaimStepWaveSkillEffect();
			skillEffect.Init(skill, this);
			m_waveSkillEffect = skillEffect;
		}
	}

	private RuinsReclaimMonsterInstance CreateMonster(RuinsReclaimMonsterArrange arrange, RuinsReclaimMonsterAttrFactor attrFactor)
	{
		RuinsReclaimMonsterInstance inst = new RuinsReclaimMonsterInstance();
		inst.Init(this, arrange, attrFactor);
		return inst;
	}

	public void SummonMonster(RuinsReclaimMonsterInstance monsterInst)
	{
		if (monsterInst == null)
		{
			return;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		List<PDRuinsReclaimSummonMonsterInstance> monsterInsts = new List<PDRuinsReclaimSummonMonsterInstance>();
		RuinsReclaimMonsterAttrFactor attrFactor = m_ruinsReclaim.GetMonsterAttrFactor(m_nAverageHeroLevel);
		foreach (RuinsReclaimSummonMonsterArrange arrange in monsterInst.arrange.summonMonsterArranges)
		{
			RuinsReclaimSummonMonsterInstance summonMonsterInst = CreateSummonMonster(arrange, attrFactor, monsterInst);
			AddMonster(summonMonsterInst);
			monsterInsts.Add((PDRuinsReclaimSummonMonsterInstance)summonMonsterInst.ToPDMonsterInstance(currentTime));
			monsterInst.hpRecoveryFactor += arrange.bossMonsterHpRecoveryFactor;
		}
		ServerEvent.SendRuinsReclaimMonsterSummon(GetClientPeers(), monsterInsts.ToArray());
	}

	private RuinsReclaimSummonMonsterInstance CreateSummonMonster(RuinsReclaimSummonMonsterArrange arrange, RuinsReclaimMonsterAttrFactor attrFactor, RuinsReclaimMonsterInstance parentMonsterInst)
	{
		RuinsReclaimSummonMonsterInstance inst = new RuinsReclaimSummonMonsterInstance();
		inst.Init(this, arrange, attrFactor, parentMonsterInst);
		return inst;
	}

	public void OnCastWaveSkill(RuinsReclaimStepWaveSkillEffect effect)
	{
		if (effect == null)
		{
			return;
		}
		RuinsReclaimStepWaveSkill skill = effect.skill;
		List<PDRuinsReclaimMonsterTransformationCancelObjectInstance> createdObjectInsts = new List<PDRuinsReclaimMonsterTransformationCancelObjectInstance>();
		Vector3 beforCreationPosition = Vector3.zero;
		int nArrangeDirection = 0;
		int nDirectionCreationMaxCount = 1;
		int nCurrentDirectionCreationCount = 0;
		int nArrangeRotation = 0;
		for (int i = 0; i < m_heroes.Count; i++)
		{
			if (nCurrentDirectionCreationCount >= nDirectionCreationMaxCount)
			{
				nCurrentDirectionCreationCount = 0;
				nArrangeRotation++;
				if (nArrangeRotation >= 2)
				{
					nArrangeRotation = 0;
					nDirectionCreationMaxCount++;
				}
				nArrangeDirection++;
				if (nArrangeDirection > 4)
				{
					nArrangeDirection = 1;
				}
			}
			Vector3 creationPosition = Vector3.zero;
			if (i != 0)
			{
				switch (nArrangeDirection)
				{
				case 1:
					creationPosition = beforCreationPosition + new Vector3(0f, 0f, -2.5f);
					break;
				case 2:
					creationPosition = beforCreationPosition + new Vector3(-2.5f, 0f, 0f);
					break;
				case 3:
					creationPosition = beforCreationPosition + new Vector3(0f, 0f, 2.5f);
					break;
				case 4:
					creationPosition = beforCreationPosition + new Vector3(2.5f, 0f, 0f);
					break;
				}
			}
			else
			{
				creationPosition = skill.position;
			}
			RuinsReclaimMonsterTransformationCancelObjectInstance objectInst = CreateMonsterTransformtaionCancelObject(skill, creationPosition);
			AddMonsterTransformationCancelObject(objectInst);
			createdObjectInsts.Add(objectInst.ToPDRuinsReclaimMonsterTransformationCancelObjectInstnace());
			nCurrentDirectionCreationCount++;
			beforCreationPosition = creationPosition;
		}
		ServerEvent.SendRuinsReclaimStepWaveSkillCast(GetClientPeers(), skill.position, createdObjectInsts.ToArray());
		foreach (Sector sector in GetInterestSectorsOfPosition(effect.position))
		{
			foreach (Hero hero in sector.heroes.Values)
			{
				lock (hero.syncObject)
				{
					if (effect.Contains(hero.position))
					{
						hero.TransformRuinsReclaimMonster(effect.skill);
					}
				}
			}
		}
	}

	public List<Guid> GetMonsterTransformationHeroes()
	{
		List<Guid> results = new List<Guid>();
		foreach (Hero hero in m_heroes.Values)
		{
			if (hero.isTransformRuinsReclaimMonster)
			{
				results.Add(hero.id);
			}
		}
		return results;
	}

	private RuinsReclaimMonsterTransformationCancelObjectInstance CreateMonsterTransformtaionCancelObject(RuinsReclaimStepWaveSkill skill, Vector3 position)
	{
		RuinsReclaimMonsterTransformationCancelObjectInstance inst = new RuinsReclaimMonsterTransformationCancelObjectInstance();
		inst.Init(this, skill, position);
		return inst;
	}

	private void AddMonsterTransformationCancelObject(RuinsReclaimMonsterTransformationCancelObjectInstance objectInst)
	{
		m_monsterTransformationCancelObjectInsts.Add(objectInst.instanceId, objectInst);
	}

	public RuinsReclaimMonsterTransformationCancelObjectInstance GetMonsterTransformationCancelObject(long lnInstanceId)
	{
		if (!m_monsterTransformationCancelObjectInsts.TryGetValue(lnInstanceId, out var value))
		{
			return null;
		}
		return value;
	}

	public List<PDRuinsReclaimMonsterTransformationCancelObjectInstance> GetPDMonsterTransformationCancelObjectInstances()
	{
		List<PDRuinsReclaimMonsterTransformationCancelObjectInstance> results = new List<PDRuinsReclaimMonsterTransformationCancelObjectInstance>();
		foreach (RuinsReclaimMonsterTransformationCancelObjectInstance inst in m_monsterTransformationCancelObjectInsts.Values)
		{
			results.Add(inst.ToPDRuinsReclaimMonsterTransformationCancelObjectInstnace());
		}
		return results;
	}

	public void RemoveMonsterTransformationCancelObject(RuinsReclaimMonsterTransformationCancelObjectInstance objectInst)
	{
		if (objectInst == null)
		{
			throw new ArgumentNullException("objectInst");
		}
		objectInst.Release();
		m_monsterTransformationCancelObjectInsts.Remove(objectInst.instanceId);
	}

	private void ClearMonsterTransformationCancelObject()
	{
		foreach (RuinsReclaimMonsterTransformationCancelObjectInstance inst in m_monsterTransformationCancelObjectInsts.Values)
		{
			Hero hero = inst.interactionHero;
			if (hero != null)
			{
				lock (hero.syncObject)
				{
					hero.CancelRuinsReclaimObjectInteraction(bSendEventToMyself: true, bSendEventToOthers: true);
				}
			}
			inst.Release();
		}
		m_monsterTransformationCancelObjectInsts.Clear();
	}

	public void OnExpireMonsterTransformationCancelObjectLifetime(RuinsReclaimMonsterTransformationCancelObjectInstance objectInst)
	{
		if (objectInst == null)
		{
			throw new ArgumentNullException("objectInst");
		}
		Hero interactionHero = objectInst.interactionHero;
		if (interactionHero != null)
		{
			lock (interactionHero.syncObject)
			{
				interactionHero.CancelRuinsReclaimObjectInteraction(bSendEventToMyself: true, bSendEventToOthers: true);
			}
		}
		RemoveMonsterTransformationCancelObject(objectInst);
		ServerEvent.SendRuinsReclaimMonsterTransformationCancelObjectLifetimeEnded(GetClientPeers(), objectInst.instanceId);
	}

	protected override void OnMonsterRemoved(MonsterInstance monsterInst)
	{
		base.OnMonsterRemoved(monsterInst);
		if (m_nStatus != 2 || !monsterInst.isDead)
		{
			return;
		}
		RuinsReclaimStep step = m_ruinsReclaim.GetStep(m_nStepNo);
		if (step.type != 3)
		{
			return;
		}
		if (monsterInst is RuinsReclaimSummonMonsterInstance summonMonsterInst)
		{
			RuinsReclaimMonsterInstance parentMonsterInst = summonMonsterInst.parentMonsterInst;
			if (parentMonsterInst != null && !parentMonsterInst.released)
			{
				parentMonsterInst.hpRecoveryFactor -= summonMonsterInst.arrange.bossMonsterHpRecoveryFactor;
			}
		}
		RuinsReclaimStepWave wave = step.GetWave(m_nWaveNo);
		if (wave.targetType == 1)
		{
			if (m_monsterInsts.Count == 0)
			{
				CompleteWave();
			}
		}
		else if (monsterInst is RuinsReclaimMonsterInstance ruinsReclaimMonsterInst && ruinsReclaimMonsterInst.arrange.key == wave.TargetArrangeKey)
		{
			CompleteWave();
		}
	}

	public void OnMonsterTransformationCancelObjectInteractionFinish(RuinsReclaimMonsterTransformationCancelObjectInstance objectInst)
	{
		if (objectInst == null)
		{
			throw new ArgumentNullException("objectInst");
		}
		Hero hero = objectInst.interactionHero;
		hero.CancelRuinsReclaimMonsterTransformation();
		RemoveMonsterTransformationCancelObject(objectInst);
		ServerEvent.SendRuinsReclaimMonsterTransformationCancelObjectInteractionFinished(hero.account.peer);
		ServerEvent.SendHeroRuinsReclaimMonsterTransformationCancelObjectInteractionFinished(GetDynamicClientPeers(hero.sector, hero.id), hero.id, objectInst.instanceId);
	}

	private void CompleteWave()
	{
		if (m_waveSkillEffect != null)
		{
			m_waveSkillEffect.Stop();
		}
		ClearMonsterTransformationCancelObject();
		foreach (Hero hero in m_heroes.Values)
		{
			hero.CancelRuinsReclaimMonsterTransformation();
		}
		ServerEvent.SendRuinsReclaimWaveComplete(GetClientPeers());
		RuinsReclaimStep step = m_ruinsReclaim.GetStep(m_nStepNo);
		step.GetWave(m_nWaveNo);
		if (m_nWaveNo >= step.waveCount)
		{
			CompleteStep();
		}
		else
		{
			StartNextWave();
		}
	}

	private void CompleteStep()
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		RuinsReclaimStep step = m_ruinsReclaim.GetStep(m_nStepNo);
		try
		{
			SFSqlStandaloneWork logWork2 = SqlStandaloneWorkUtil.CreateGameLogDBWork();
			logWork2.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimStepCompletionLog(m_instanceId, m_nStepNo, currentTime));
			logWork2.Schedule();
		}
		catch (Exception ex2)
		{
			SFLogUtil.Error(GetType(), null, ex2);
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_RuinsReclaim);
		foreach (Hero hero in m_heroes.Values)
		{
			lock (hero.syncObject)
			{
				dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(hero.id));
				List<PDItemBooty> booties = new List<PDItemBooty>();
				List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
				Mail mail = null;
				foreach (RuinsReclaimStepReward stepReward in step.rewards)
				{
					ItemReward itemReward = stepReward.itemReward;
					if (itemReward == null)
					{
						continue;
					}
					Item item = itemReward.item;
					bool bOwned = itemReward.owned;
					int nCount = itemReward.count;
					PDItemBooty booty2 = new PDItemBooty();
					booty2.id = item.id;
					booty2.owned = bOwned;
					booty2.count = nCount;
					booties.Add(booty2);
					int nRemainingItemCount = hero.AddItem(item, bOwned, nCount, changedInventorySlots);
					if (nRemainingItemCount > 0)
					{
						if (mail == null)
						{
							mail = Mail.Create("MAIL_REWARD_N_20", "MAIL_REWARD_D_20", currentTime);
						}
						mail.AddAttachment(new MailAttachment(item, nRemainingItemCount, bOwned));
					}
				}
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
						logWork.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimStepCompletionRewardLog(Guid.NewGuid(), m_instanceId, m_nStepNo, hero.id, booty.id, booty.owned, booty.count));
					}
					logWork.Schedule();
				}
				catch (Exception ex)
				{
					SFLogUtil.Error(GetType(), null, ex);
				}
				ServerEvent.SendRuinsReclaimStepCompleted(hero.account.peer, booties.ToArray(), InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
			}
		}
		dbWork.Schedule();
		if (m_nStepNo < m_ruinsReclaim.stepCount)
		{
			StartNextStep();
		}
		else
		{
			Clear();
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
			logWork2.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimCompletionLog(m_instanceId, 1, m_nPlayTime, m_endTime));
			logWork2.Schedule();
		}
		catch (Exception ex2)
		{
			SFLogUtil.Error(GetType(), null, ex2);
		}
		Hero bossKiller = null;
		MonsterReceivedDamage topBossMonsterDamage = null;
		if (m_lastBossMonsterInst != null)
		{
			Hero killer = m_lastBossMonsterInst.lastAttacker as Hero;
			bossKiller = GetHero(killer.id);
			topBossMonsterDamage = m_lastBossMonsterInst.GetMaxMonsterReceivedDamage();
		}
		MonsterReceivedDamage topSummonMonsterDamage = GetMaxSummonMonsterReceivedDamage();
		PDItemBooty monsterTerminatorBooty = null;
		ItemReward monsterTermainatorItemReward = m_ruinsReclaim.SelectMonsterTerminatorRewardPoolEntry()?.itemReward;
		if (monsterTermainatorItemReward != null)
		{
			monsterTerminatorBooty = new PDItemBooty();
			monsterTerminatorBooty.id = monsterTermainatorItemReward.item.id;
			monsterTerminatorBooty.owned = monsterTermainatorItemReward.owned;
			monsterTerminatorBooty.count = monsterTermainatorItemReward.count;
		}
		PDItemBooty ultimateAttackKingBooty = null;
		ItemReward ultimateAttackKingReward = m_ruinsReclaim.SelectUltimateAttackKingRewardPoolEntry()?.itemReward;
		if (ultimateAttackKingReward != null)
		{
			ultimateAttackKingBooty = new PDItemBooty();
			ultimateAttackKingBooty.id = ultimateAttackKingReward.item.id;
			ultimateAttackKingBooty.owned = ultimateAttackKingReward.owned;
			ultimateAttackKingBooty.count = ultimateAttackKingReward.count;
		}
		PDItemBooty partyVolunteerBooty = null;
		ItemReward partyVolunteerReward = m_ruinsReclaim.SelectPartyVolunteerRewardPoolEntry()?.itemReward;
		if (partyVolunteerReward != null)
		{
			partyVolunteerBooty = new PDItemBooty();
			partyVolunteerBooty.id = partyVolunteerReward.item.id;
			partyVolunteerBooty.owned = partyVolunteerReward.owned;
			partyVolunteerBooty.count = partyVolunteerReward.count;
		}
		SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateGameDBContentWork(QueuingWorkContentId.GameDB_RuinsReclaim);
		foreach (Hero hero in m_heroes.Values)
		{
			lock (hero.syncObject)
			{
				dbWork.AddSyncWork(SyncQueuingWorkUtil.CreateHeroWork(hero.id));
				List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
				Mail mail = null;
				PDItemBooty randomBooty = null;
				ItemReward randomReward = m_ruinsReclaim.SelectRandomRewardPoolEntry()?.itemReward;
				if (randomReward != null)
				{
					Item item5 = randomReward.item;
					bool bOwned5 = randomReward.owned;
					int nCount5 = randomReward.count;
					int nRemainingItemCount5 = hero.AddItem(item5, bOwned5, nCount5, changedInventorySlots);
					randomBooty = new PDItemBooty();
					randomBooty.id = item5.id;
					randomBooty.owned = bOwned5;
					randomBooty.count = nCount5;
					if (nRemainingItemCount5 > 0)
					{
						mail = Mail.Create("MAIL_REWARD_N_20", "MAIL_REWARD_D_20", m_endTime);
						mail.AddAttachment(new MailAttachment(item5, nRemainingItemCount5, bOwned5));
					}
				}
				List<PDItemBooty> booties = new List<PDItemBooty>();
				foreach (RuinsReclaimReward reward in m_ruinsReclaim.rewards)
				{
					ItemReward itemReward = reward.itemReward;
					if (itemReward == null)
					{
						continue;
					}
					Item item4 = itemReward.item;
					bool bOwned4 = itemReward.owned;
					int nCount4 = itemReward.count;
					int nRemainingItemCount4 = hero.AddItem(item4, bOwned4, nCount4, changedInventorySlots);
					PDItemBooty booty2 = new PDItemBooty();
					booty2.id = item4.id;
					booty2.owned = bOwned4;
					booty2.count = nCount4;
					booties.Add(booty2);
					if (nRemainingItemCount4 > 0)
					{
						if (mail == null)
						{
							mail = Mail.Create("MAIL_REWARD_N_20", "MAIL_REWARD_D_20", m_endTime);
						}
						mail.AddAttachment(new MailAttachment(item4, nRemainingItemCount4, bOwned4));
					}
				}
				if (bossKiller != null && bossKiller.id == hero.id && monsterTermainatorItemReward != null)
				{
					Item item3 = monsterTermainatorItemReward.item;
					bool bOwned3 = monsterTermainatorItemReward.owned;
					int nCount3 = monsterTermainatorItemReward.count;
					int nRemainingItemCount3 = hero.AddItem(item3, bOwned3, nCount3, changedInventorySlots);
					if (nRemainingItemCount3 > 0)
					{
						if (mail == null)
						{
							mail = Mail.Create("MAIL_REWARD_N_20", "MAIL_REWARD_D_20", m_endTime);
						}
						mail.AddAttachment(new MailAttachment(item3, nRemainingItemCount3, bOwned3));
					}
				}
				if (topBossMonsterDamage != null && topBossMonsterDamage.attackerId == hero.id && ultimateAttackKingReward != null)
				{
					Item item2 = ultimateAttackKingReward.item;
					bool bOwned2 = ultimateAttackKingReward.owned;
					int nCount2 = ultimateAttackKingReward.count;
					int nRemainingItemCount2 = hero.AddItem(item2, bOwned2, nCount2, changedInventorySlots);
					if (nRemainingItemCount2 > 0)
					{
						if (mail == null)
						{
							mail = Mail.Create("MAIL_REWARD_N_20", "MAIL_REWARD_D_20", m_endTime);
						}
						mail.AddAttachment(new MailAttachment(item2, nRemainingItemCount2, bOwned2));
					}
				}
				if (topSummonMonsterDamage != null && topSummonMonsterDamage.attackerId == hero.id && partyVolunteerReward != null)
				{
					Item item = partyVolunteerReward.item;
					bool bOwned = partyVolunteerReward.owned;
					int nCount = partyVolunteerReward.count;
					int nRemainingItemCount = hero.AddItem(item, bOwned, nCount, changedInventorySlots);
					if (nRemainingItemCount > 0)
					{
						if (mail == null)
						{
							mail = Mail.Create("MAIL_REWARD_N_20", "MAIL_REWARD_D_20", m_endTime);
						}
						mail.AddAttachment(new MailAttachment(item, nRemainingItemCount, bOwned));
					}
				}
				if (mail != null)
				{
					hero.AddMail(mail, bSendEvent: true);
				}
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
					int nBossDamage = 0;
					if (m_lastBossMonsterInst != null)
					{
						MonsterReceivedDamage damage = m_lastBossMonsterInst.GetMonsterReceivedDamage(hero.id);
						nBossDamage = (int)((damage != null) ? damage.damage : 0);
					}
					int nSummonMonsterDamage = 0;
					MonsterReceivedDamage summonMonsterDamage = GetSummonMonsterReceivedDamage(hero.id);
					if (summonMonsterDamage != null)
					{
						nSummonMonsterDamage = (int)summonMonsterDamage.damage;
					}
					logWork.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimCompletionMemberLog(m_instanceId, hero.id, nBossDamage, nSummonMonsterDamage));
					foreach (PDItemBooty booty in booties)
					{
						logWork.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimCompletionMemberRewardLog(Guid.NewGuid(), m_instanceId, hero.id, 1, booty.id, booty.owned, booty.count));
					}
					if (randomBooty != null)
					{
						logWork.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimCompletionMemberRewardLog(Guid.NewGuid(), m_instanceId, hero.id, 2, randomBooty.id, randomBooty.owned, randomBooty.count));
					}
					if (bossKiller != null && bossKiller.id == hero.id && monsterTerminatorBooty != null)
					{
						logWork.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimCompletionMemberRewardLog(Guid.NewGuid(), m_instanceId, hero.id, 3, monsterTerminatorBooty.id, monsterTerminatorBooty.owned, monsterTerminatorBooty.count));
					}
					if (topBossMonsterDamage != null && topBossMonsterDamage.attackerId == hero.id && ultimateAttackKingBooty != null)
					{
						logWork.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimCompletionMemberRewardLog(Guid.NewGuid(), m_instanceId, hero.id, 4, ultimateAttackKingBooty.id, ultimateAttackKingBooty.owned, ultimateAttackKingBooty.count));
					}
					if (summonMonsterDamage != null && summonMonsterDamage.attackerId == hero.id && partyVolunteerBooty != null)
					{
						logWork.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimCompletionMemberRewardLog(Guid.NewGuid(), m_instanceId, hero.id, 5, partyVolunteerBooty.id, partyVolunteerBooty.owned, partyVolunteerBooty.count));
					}
					logWork.Schedule();
				}
				catch (Exception ex)
				{
					SFLogUtil.Error(GetType(), null, ex);
				}
				string sMonsterTerminatorHeroName = bossKiller?.name;
				string sUltimateAttackKingHeroName = topBossMonsterDamage?.attackerName;
				string sPartyVolunteerHeroName = topSummonMonsterDamage?.attackerName;
				ServerEvent.SendRuinsReclaimClear(hero.account.peer, bossKiller.id, sMonsterTerminatorHeroName, monsterTerminatorBooty, topBossMonsterDamage.attackerId, sUltimateAttackKingHeroName, ultimateAttackKingBooty, topSummonMonsterDamage.attackerId, sPartyVolunteerHeroName, partyVolunteerBooty, randomBooty, booties.ToArray(), InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray());
			}
		}
		dbWork.Schedule();
		int nDuration = m_ruinsReclaim.exitDelayTime * 1000;
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
			logWork.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimCompletionLog(m_instanceId, 2, m_nPlayTime, m_endTime));
			foreach (Hero hero in m_heroes.Values)
			{
				int nBossDamage = 0;
				if (m_lastBossMonsterInst != null)
				{
					MonsterReceivedDamage damage = m_lastBossMonsterInst.GetMonsterReceivedDamage(hero.id);
					nBossDamage = (int)((damage != null) ? damage.damage : 0);
				}
				int nSummonMonsterDamage = 0;
				MonsterReceivedDamage summonMonsterDamage = GetSummonMonsterReceivedDamage(hero.id);
				if (summonMonsterDamage != null)
				{
					nSummonMonsterDamage = (int)summonMonsterDamage.damage;
				}
				logWork.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimCompletionMemberLog(m_instanceId, hero.id, nBossDamage, nSummonMonsterDamage));
			}
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
		ServerEvent.SendRuinsReclaimFail(GetClientPeers());
		int nDuration = m_ruinsReclaim.exitDelayTime * 1000;
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
				hero.CancelRuinsReclaimMonsterTransformation();
				hero.StopRuinsReclaimDebuffEffect(bSendEvent: false);
				if (hero.isDead)
				{
					hero.Revive(bSendEvent: false);
				}
				else
				{
					hero.RestoreHP(hero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
				}
				Exit(hero, isLogOut: false, null);
				ServerEvent.SendRuinsReclaimBanished(hero.account.peer, hero.previousContinentId, hero.previousNationId, hero.hp);
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
			logWork.AddSqlCommand(GameLogDac.CSC_AddRuinsReclaimDisqualificationLog(m_instanceId, hero.id, DateTimeUtil.currentTime));
			logWork.Schedule();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void Finish(int nStatus)
	{
		if (m_waveSkillEffect != null)
		{
			m_waveSkillEffect.Stop();
		}
		ClearRewardObject();
		ClearMonsterTransformationCancelObject();
		foreach (RuinsReclaimTrapInstance trapInst in m_trapInsts)
		{
			trapInst.Release();
		}
		m_trapInsts.Clear();
		DisposePlayWaitingTimer();
		DisposeLimitTimeTimer();
		m_endTime = DateTimeUtil.currentTime;
		if (m_nStatus == 2)
		{
			m_nPlayTime = (int)Math.Floor((m_endTime - m_startTime).TotalSeconds);
		}
		m_nStatus = nStatus;
	}

	public void OnSummonMonsterDamage(Hero attacker, int nDamage, DateTimeOffset damageTime)
	{
		MonsterReceivedDamage damage = GetOrCreateSummonMonsterReceivedDamage(attacker.id, attacker.name);
		damage.AddDamage(nDamage, damageTime);
	}

	private MonsterReceivedDamage GetSummonMonsterReceivedDamage(Guid attackerId)
	{
		if (!m_LastSummonMonsterReceivedDamages.TryGetValue(attackerId, out var value))
		{
			return null;
		}
		return value;
	}

	private MonsterReceivedDamage GetOrCreateSummonMonsterReceivedDamage(Guid attackerId, string sAttackeName)
	{
		MonsterReceivedDamage damage = GetSummonMonsterReceivedDamage(attackerId);
		if (damage == null)
		{
			damage = new MonsterReceivedDamage(attackerId, sAttackeName);
			m_LastSummonMonsterReceivedDamages.Add(damage.attackerId, damage);
		}
		return damage;
	}

	private MonsterReceivedDamage GetMaxSummonMonsterReceivedDamage()
	{
		MonsterReceivedDamage[] damages = m_LastSummonMonsterReceivedDamages.Values.ToArray();
		Array.Sort(damages, MonsterReceivedDamage.Compare);
		Array.Reverse(damages);
		for (int i = 0; i < damages.Length; i++)
		{
			damages[i].rank = i + 1;
		}
		return damages.FirstOrDefault();
	}

	private void RemoveSummonMonsterReceivedDamage(Guid heroId)
	{
		m_LastSummonMonsterReceivedDamages.Remove(heroId);
	}

	protected override void ReleaseInternal()
	{
		if (m_waveSkillEffect != null)
		{
			m_waveSkillEffect.Stop();
		}
		ClearRewardObject();
		ClearMonsterTransformationCancelObject();
		foreach (RuinsReclaimTrapInstance trapInst in m_trapInsts)
		{
			trapInst.Release();
		}
		m_trapInsts.Clear();
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
