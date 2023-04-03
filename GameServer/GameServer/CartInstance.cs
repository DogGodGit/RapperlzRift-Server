using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public abstract class CartInstance : Unit
{
	public const int kUpdateInterval = 500;

	public const float kAccelCoolTimeFactor = 0.9f;

	protected long m_lnInstanceId;

	protected Cart m_cart;

	protected Hero m_owner;

	private Timer m_updateTimer;

	protected Dictionary<Guid, CartReceivedDamage> m_receivedDamages = new Dictionary<Guid, CartReceivedDamage>();

	protected int m_nLevel;

	protected bool m_bIsRiding;

	protected bool m_bMoving;

	protected int m_nMoveSpeed;

	private DateTimeOffset m_moveVerificationStartTime = DateTimeOffset.MinValue;

	private DateTimeOffset m_moveVerificationEndTime = DateTimeOffset.MinValue;

	private float m_fMoveVerificationDistance;

	private int m_nAbnormalMoveSpeedCount;

	protected bool m_bIsHighSpeed;

	protected DateTimeOffset m_highSpeedStartTime = DateTimeOffset.MinValue;

	protected int m_nHighSpeedDuration;

	protected DateTimeOffset m_lastAccelTime = DateTimeOffset.MinValue;

	protected PlaceEntranceParam m_placeEntranceParam;

	public static readonly SFSynchronizedLongFactory idFactory = new SFSynchronizedLongFactory();

	public object syncObject => this;

	public override UnitType type => UnitType.CartInstance;

	public abstract CartInstanceType cartInstanceType { get; }

	public long instanceId => m_lnInstanceId;

	public Cart cart => m_cart;

	public Hero owner => m_owner;

	public override bool increaseDamageEnabledByProbability => false;

	public override bool blockEnabled => false;

	public int level => m_nLevel;

	public bool isRiding
	{
		get
		{
			return m_bIsRiding;
		}
		set
		{
			m_bIsRiding = value;
		}
	}

	public bool moving => m_bMoving;

	public override int moveSpeed => m_nMoveSpeed;

	public bool isHighSpeed => m_bIsHighSpeed;

	public int highSpeedDuration => m_nHighSpeedDuration;

	public PlaceEntranceParam placeEntranceParam
	{
		get
		{
			return m_placeEntranceParam;
		}
		set
		{
			m_placeEntranceParam = value;
		}
	}

	public float radius => m_cart.radius;

	public CartInstance()
	{
		m_lnInstanceId = idFactory.NewValue();
	}

	protected void InitCartInstance(Cart cart, Hero owner, DateTimeOffset currentTime)
	{
		if (cart == null)
		{
			throw new ArgumentNullException("cart");
		}
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		m_cart = cart;
		m_owner = owner;
		m_nLevel = m_owner.level;
		InitUnit(currentTime);
		RefreshRealValues(bSendMaxHpChangedToOthers: false);
		RefreshMoveSpeed();
		m_nHP = m_nRealMaxHP;
		m_updateTimer = new Timer(OnUpdateTimerTick);
		m_updateTimer.Change(500, 500);
	}

	protected override void RefreshRealValues_Sum()
	{
		base.RefreshRealValues_Sum();
		JobLevelMaster levelMaster = Resource.instance.GetJobLevelMaster(m_nLevel);
		m_nRealMaxHP += levelMaster.cartMaxHp;
	}

	protected override void InvokeRunWorkInternal(ISFRunnable work)
	{
		CartSynchronizer.Exec(this, new SFAction<ISFRunnable>(base.RunWorkInternal, work));
	}

	private void DisposeUpdateTimer()
	{
		if (m_updateTimer != null)
		{
			m_updateTimer.Dispose();
			m_updateTimer = null;
		}
	}

	private void OnUpdateTimerTick(object state)
	{
		AddWork(new SFAction(base.OnUpdate), bGlobalLockRequired: false);
	}

	protected override void ReleaseInternal()
	{
		DisposeUpdateTimer();
		base.ReleaseInternal();
	}

	public void SetCurrentPlace(Place place)
	{
		m_currentPlace = place;
	}

	public void GetOn(DateTimeOffset time)
	{
		if (!m_bIsRiding)
		{
			m_bIsRiding = true;
			m_owner.ridingCartInst = this;
			Sector oldSector = m_sector;
			SetPositionAndRotation(m_owner.position, m_owner.rotationY);
			ChangeSector(m_currentPlace.GetSectorOfPosition(m_position));
			OnGetOn();
			ServerEvent.SendCartGetOn(m_currentPlace.GetDynamicClientPeers(oldSector, m_owner.sector, m_owner.id), ToPDCartInstance(time), m_owner.id);
		}
	}

	protected virtual void OnGetOn()
	{
		m_owner.EndMove();
		m_owner.CancelAllExclusiveActions();
		m_owner.GetOffMount(bSendEventToMyself: false);
		m_owner.CancelMatching(2);
		foreach (MonsterInstance monsterInst2 in owner.ownMonsterInsts)
		{
			if (monsterInst2.ownerType == MonsterOwnerType.Target)
			{
				monsterInst2.SetOwnership(owner, MonsterOwnerType.Controller);
			}
		}
		foreach (MonsterInstance monsterInst in m_owner.aggroMonsterInsts)
		{
			monsterInst.RemoveAggro(owner.id);
		}
		m_owner.ClearAggroMonsters();
		m_owner.EndAutoHunt();
	}

	public void GetOff(DateTimeOffset time, bool bSendEvent)
	{
		if (m_bIsRiding)
		{
			m_bIsRiding = false;
			m_owner.ridingCartInst = null;
			OnGetOff();
			if (bSendEvent)
			{
				ServerEvent.SendCartGetOff(m_currentPlace.GetDynamicClientPeers(m_sector, m_owner.id), m_lnInstanceId, m_owner.ToPDHero(time));
			}
		}
	}

	protected virtual void OnGetOff()
	{
		EndMove();
	}

	public void StartMove()
	{
		if (!m_bMoving)
		{
			m_bMoving = true;
			StartMoveVerification(DateTimeUtil.currentTime);
		}
	}

	public void EndMove()
	{
		if (m_bMoving)
		{
			m_bMoving = false;
			VerifyMoveSpeed(DateTimeUtil.currentTime);
		}
	}

	private void StartMoveVerification(DateTimeOffset time)
	{
		m_moveVerificationStartTime = time;
		m_fMoveVerificationDistance = 0f;
	}

	private void VerifyMoveSpeed(DateTimeOffset time)
	{
		m_moveVerificationEndTime = time;
		float fTime = (float)(m_moveVerificationEndTime - m_moveVerificationStartTime).TotalSeconds;
		if (!(fTime >= 1f))
		{
			return;
		}
		float fMoveSpeed = m_fMoveVerificationDistance / fTime;
		if (fMoveSpeed > (float)m_nMoveSpeed * 2f)
		{
			m_nAbnormalMoveSpeedCount++;
			if (m_nAbnormalMoveSpeedCount > 2)
			{
				SFLogUtil.Warn(GetType(), string.Concat("[CartAbnormalMoveSpeedLog] ownerId = ", m_owner.id, ", moveSpeed = ", fMoveSpeed));
				m_owner.AddWork(new SFAction(m_owner.LogoutByAbnormalMoveSpeed), bGlobalLockRequired: true);
			}
		}
	}

	public void OnMove(Vector3 previousPosition)
	{
		if (m_bMoving)
		{
			float fDistance = MathUtil.Distance_3D(previousPosition, m_position);
			m_fMoveVerificationDistance += fDistance;
		}
	}

	public float GetRemainingAccelCoolTime(DateTimeOffset time)
	{
		return (float)Math.Max((double)Resource.instance.cartAccelCoolTime - (time - m_lastAccelTime).TotalSeconds, 0.0);
	}

	public bool IsAccelCoolTimeElapsed(DateTimeOffset time)
	{
		return (time - m_lastAccelTime).TotalSeconds >= (double)((float)Resource.instance.cartAccelCoolTime * 0.9f);
	}

	public bool Accelerate(DateTimeOffset time)
	{
		m_lastAccelTime = time;
		if (!Util.DrawLots(Resource.instance.cartAccelSuccessRate))
		{
			return false;
		}
		if (!m_bIsHighSpeed)
		{
			m_bIsHighSpeed = true;
			m_highSpeedStartTime = time;
			m_nHighSpeedDuration = Resource.instance.cartHighSpeedDuration;
			RefreshMoveSpeed();
			if (m_currentPlace != null)
			{
				ServerEvent.SendCartHighSpeedStart(m_currentPlace.GetDynamicClientPeers(m_sector, m_owner.id), m_lnInstanceId);
			}
		}
		else
		{
			m_nHighSpeedDuration += Resource.instance.cartHighSpeedDurationExtension;
		}
		return true;
	}

	private void EndHighSpeed()
	{
		if (!m_bIsHighSpeed)
		{
			return;
		}
		m_bIsHighSpeed = false;
		RefreshMoveSpeed();
		if (isRiding)
		{
			ServerEvent.SendMyCartHighSpeedEnd(m_owner.account.peer);
			if (m_currentPlace != null)
			{
				ServerEvent.SendCartHighSpeedEnd(m_currentPlace.GetDynamicClientPeers(m_sector, m_owner.id), m_lnInstanceId);
			}
		}
	}

	public bool IsRidablePosition(Vector3 position, float fRadius)
	{
		return MathUtil.CircleContains(m_position, m_cart.ridableRange * 1.1f + fRadius * 2f, position);
	}

	protected override void RefreshMoveSpeed()
	{
		int nOldMoveSpeed = m_nMoveSpeed;
		m_nMoveSpeed = (m_bIsHighSpeed ? Resource.instance.cartHighSpeed : Resource.instance.cartNormalSpeed);
		foreach (AbnormalStateEffect effect in m_abnormalStateEffects.Values)
		{
			if (effect.abnormalState.id == 11)
			{
				m_nMoveSpeed = (int)Math.Floor((float)m_nMoveSpeed * ((float)effect.abnormalStateLevel.value1 / 10000f));
			}
		}
		if (nOldMoveSpeed > 0 && nOldMoveSpeed != m_nMoveSpeed)
		{
			DateTimeOffset currentTime = DateTimeUtil.currentTime;
			VerifyMoveSpeed(currentTime);
			StartMoveVerification(currentTime);
		}
	}

	protected override void OnUpdateInternal()
	{
		base.OnUpdateInternal();
		OnUpdate_ManageHighSpeed();
	}

	private void OnUpdate_ManageHighSpeed()
	{
		try
		{
			ManageHighSpeed();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex, bStackTrace: true);
		}
	}

	private void ManageHighSpeed()
	{
		if (m_bIsHighSpeed && !((DateTimeUtil.currentTime - m_highSpeedStartTime).TotalSeconds < (double)m_nHighSpeedDuration))
		{
			EndHighSpeed();
		}
	}

	protected override void OnDamage()
	{
		base.OnDamage();
		Hero attacker = (Hero)m_lastAttacker;
		AddReceivedDamage(attacker, m_nLastDamage, m_lastDamageTime);
	}

	private void AddReceivedDamage(Hero attacker, long lnDamage, DateTimeOffset time)
	{
		CartReceivedDamage damage = GetOrCreateReceivedDamage(attacker.id, attacker.name);
		damage.AddDamage(lnDamage, time);
	}

	public CartReceivedDamage GetReceivedDamage(Guid attackerId)
	{
		if (!m_receivedDamages.TryGetValue(attackerId, out var value))
		{
			return null;
		}
		return value;
	}

	public CartReceivedDamage GetOrCreateReceivedDamage(Guid attackerId, string sAttackerName)
	{
		CartReceivedDamage damage = GetReceivedDamage(attackerId);
		if (damage == null)
		{
			damage = new CartReceivedDamage(attackerId, sAttackerName);
			m_receivedDamages.Add(damage.attackerId, damage);
		}
		return damage;
	}

	protected override void SendMaxHPChangedEventToOthers()
	{
	}

	protected override void SendHitEvent(HitResult hitResult)
	{
		PDHitResult pdHitResult = hitResult.ToPDHitResult();
		ServerEvent.SendCartHit(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, pdHitResult);
	}

	protected override void SendAbnormalStateEffectStart(AbnormalStateEffect effect, int nDamageAbsorbShieldRemainingAbsorbAmount, IEnumerable<long> removedAbnormalStateEffects)
	{
		ServerEvent.SendCartAbnormalStateEffectStart(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, effect.id, effect.abnormalState.id, effect.sourceJobId, effect.abnormalStateLevel.level, effect.duration, nDamageAbsorbShieldRemainingAbsorbAmount, removedAbnormalStateEffects.ToArray());
	}

	protected override void SendAbnormalStateEffectHit(AbnormalStateEffect effect, IEnumerable<long> removedAbnormalStateEffects)
	{
		PDAttacker pdAttacker = m_lastAttacker.ToPDAttacker();
		ServerEvent.SendCartAbnormalStateEffectHit(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, m_nHP, effect.id, m_nLastDamage, m_nLastHPDamage, removedAbnormalStateEffects.ToArray(), pdAttacker);
	}

	protected override void SendAbnormalStateEffectFinishedEvent(AbnormalStateEffect effect)
	{
		ServerEvent.SendCartAbnormalStateEffectFinished(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, effect.id);
	}

	protected override void OnDead()
	{
		base.OnDead();
		if (m_bIsRiding)
		{
			lock (m_owner.syncObject)
			{
				GetOff(m_lastDamageTime, bSendEvent: false);
				if (m_currentPlace != null)
				{
					ServerEvent.SendHeroEnter(m_currentPlace.GetDynamicClientPeers(m_sector, m_owner.id), m_owner.ToPDHero(m_lastDamageTime), bIsRevivalEnter: false);
				}
			}
		}
		m_currentPlace.AddWork(new SFAction<DateTimeOffset>(OnDeadAsync, m_lastDamageTime), bGlobalLockRequired: true);
	}

	protected virtual void OnDeadAsync(DateTimeOffset time)
	{
	}

	public override PDAttacker ToPDAttacker()
	{
		throw new NotSupportedException();
	}

	protected abstract PDCartInstance CreatePDCartInstance();

	public PDCartInstance ToPDCartInstance(DateTimeOffset time)
	{
		PDCartInstance inst = CreatePDCartInstance();
		inst.instanceId = m_lnInstanceId;
		inst.cartId = m_cart.id;
		inst.level = m_nLevel;
		inst.maxHP = m_nRealMaxHP;
		inst.hp = m_nHP;
		inst.isHighSpeed = m_bIsHighSpeed;
		inst.remainingAccelCoolTime = GetRemainingAccelCoolTime(time);
		inst.position = m_position;
		inst.rotationY = m_fRotationY;
		inst.ownerId = (Guid)m_owner.id;
		inst.ownerName = m_owner.name;
		inst.ownerNationId = m_owner.nationId;
		inst.rider = (m_bIsRiding ? m_owner.ToPDHeroWithLock(time) : null);
		return inst;
	}

	public PDCartInstance ToPDCartInstanceWithLock(DateTimeOffset time)
	{
		lock (syncObject)
		{
			return ToPDCartInstance(time);
		}
	}
}
