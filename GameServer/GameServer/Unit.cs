using System;
using System.Collections.Generic;
using System.Linq;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public abstract class Unit
{
	public const int kBaseRate = 500;

	public const int kMaxRate = 10000;

	public const int kCriticalDivideRate = 6000;

	public const int kCriticalResistanceDivideRate = 6000;

	public const float kCriticalFactor = 1.5f;

	public const int kPenetrationDivideRate = 3000;

	public const float kPenetrationFactor = 1.2f;

	public const int kBlockDivideRate = 3000;

	public const float kBlockFactor = 0.5f;

	public const float kMainElementalFactor = 1.5f;

	public const float kSubElementalFactor = 0.8f;

	protected bool m_bReleased;

	private SFDynamicWorker m_worker = new SFDynamicWorker();

	protected DateTimeOffset m_prevUpdateTime = DateTimeOffset.MinValue;

	protected DateTimeOffset m_currentUpdateTime = DateTimeOffset.MinValue;

	protected int m_nHP;

	protected int m_nRealMaxHP;

	protected int m_nRealPhysicalOffense;

	protected int m_nRealMagicalOffense;

	protected int m_nRealPhysicalDefense;

	protected int m_nRealMagicalDefense;

	protected int m_nRealCritical;

	protected int m_nRealCriticalResistance;

	protected int m_nRealCriticalDamageIncRate;

	protected int m_nRealCriticalDamageDecRate;

	protected int m_nRealPenetration;

	protected int m_nRealBlock;

	protected int m_nRealFireOffense;

	protected int m_nRealFireDefense;

	protected int m_nRealLightningOffense;

	protected int m_nRealLightningDefense;

	protected int m_nRealDarkOffense;

	protected int m_nRealDarkDefense;

	protected int m_nRealHolyOffense;

	protected int m_nRealHolyDefense;

	protected int m_nRealDamageIncRate;

	protected int m_nRealDamageDecRate;

	protected int m_nRealStunResistance;

	protected int m_nRealSnareResistance;

	protected int m_nRealSilenceResistance;

	protected int m_nRealBaseMaxHPIncRate;

	protected int m_nRealBaseOffenseIncRate;

	protected int m_nRealBasePhysicalDefenseIncRate;

	protected int m_nRealBaseMagicalDefenseIncRate;

	protected int m_nRealOffense;

	protected Place m_currentPlace;

	protected Vector3 m_position = Vector3.zero;

	protected float m_fRotationY;

	protected Sector m_sector;

	protected Unit m_lastAttacker;

	protected int m_nLastDamage;

	protected int m_nLastHPDamage;

	protected DateTimeOffset m_lastDamageTime = DateTimeOffset.MinValue;

	protected Dictionary<long, AbnormalStateEffect> m_abnormalStateEffects = new Dictionary<long, AbnormalStateEffect>();

	public abstract UnitType type { get; }

	public bool released => m_bReleased;

	public virtual bool moveEnabled
	{
		get
		{
			if (!isDead && !ContainsAbnormalStateEffectOfAbnormalState(1) && !ContainsAbnormalStateEffectOfAbnormalState(2))
			{
				return !ContainsAbnormalStateEffectOfAbnormalState(111);
			}
			return false;
		}
	}

	public virtual bool skillEnabled
	{
		get
		{
			if (!isDead && !ContainsAbnormalStateEffectOfAbnormalState(1))
			{
				return !ContainsAbnormalStateEffectOfAbnormalState(111);
			}
			return false;
		}
	}

	public virtual bool hitEnabled
	{
		get
		{
			if (m_currentPlace != null && m_currentPlace.battleEnabled && !isDead)
			{
				return !ContainsAbnormalStateEffectOfAbnormalState(9);
			}
			return false;
		}
	}

	public virtual bool abnormalStateHitEnabled
	{
		get
		{
			if (m_currentPlace != null && m_currentPlace.battleEnabled && !isDead)
			{
				return !ContainsAbnormalStateEffectOfAbnormalState(9);
			}
			return false;
		}
	}

	public virtual bool abnormalStateDamageEnabled
	{
		get
		{
			if (m_currentPlace != null && m_currentPlace.battleEnabled && !isDead)
			{
				return !ContainsAbnormalStateEffectOfAbnormalState(9);
			}
			return false;
		}
	}

	public bool isAbnormalStateImmune
	{
		get
		{
			if (!ContainsAbnormalStateEffectOfAbnormalState(101) && !ContainsAbnormalStateEffectOfAbnormalState(9))
			{
				return ContainsAbnormalStateEffectOfAbnormalState(12);
			}
			return true;
		}
	}

	protected virtual int finalDamagePenaltyRate => 0;

	public abstract bool increaseDamageEnabledByProbability { get; }

	public abstract bool blockEnabled { get; }

	public int realMaxHP => m_nRealMaxHP;

	public int realPhysicalOffense => m_nRealPhysicalOffense;

	public int realMagicalOffense => m_nRealMagicalOffense;

	public int realPhysicalDefense => m_nRealPhysicalDefense;

	public int realMagicalDefense => m_nRealMagicalDefense;

	public int realCritical => m_nRealCritical;

	public int realCriticalResistance => m_nRealCriticalResistance;

	public int realCriticalDamageIncRate => m_nRealCriticalDamageIncRate;

	public int realCriticalDamageDecRate => m_nRealCriticalDamageDecRate;

	public int realPenetration => m_nRealPenetration;

	public int realBlock => m_nRealBlock;

	public int realFireOffense => m_nRealFireOffense;

	public int realFireDefense => m_nRealFireDefense;

	public int realLightningOffense => m_nRealLightningOffense;

	public int realLightningDefense => m_nRealLightningDefense;

	public int realDarkOffense => m_nRealDarkOffense;

	public int realDarkDefense => m_nRealDarkDefense;

	public int realHolyOffense => m_nRealHolyOffense;

	public int realHolyDefense => m_nRealHolyDefense;

	public int realDamageIncRate => m_nRealDamageIncRate;

	public int realDamageDecRate => m_nRealDamageDecRate;

	public int realStunResistance => m_nRealStunResistance;

	public int realSnareResistance => m_nRealSnareResistance;

	public int realSilenceResistance => m_nRealSilenceResistance;

	public int realBaseMaxHPIncRate => m_nRealBaseMaxHPIncRate;

	public int realBaseOffenseIncRate => m_nRealBaseOffenseIncRate;

	public int realBasePhysicalDenfenseIncRate => m_nRealBasePhysicalDefenseIncRate;

	public int realBaseMagicalDefenseIncRate => m_nRealBaseMagicalDefenseIncRate;

	public int realOffense => m_nRealOffense;

	public abstract int moveSpeed { get; }

	public int hp => m_nHP;

	public bool isDead => m_nHP <= 0;

	public bool isFullHp => m_nHP >= m_nRealMaxHP;

	public Place currentPlace => m_currentPlace;

	public Vector3 position => m_position;

	public float rotationY => m_fRotationY;

	public Sector sector => m_sector;

	public Unit lastAttacker => m_lastAttacker;

	public int lastDamage => m_nLastDamage;

	public int lastHPDamage => m_nLastHPDamage;

	public DateTimeOffset lastDamageTime => m_lastDamageTime;

	public Dictionary<long, AbnormalStateEffect> abnormalStateEffects => m_abnormalStateEffects;

	protected void InitUnit(DateTimeOffset currentTime)
	{
		m_worker.isAsyncErrorLogging = true;
		m_worker.Start();
		m_prevUpdateTime = currentTime;
		m_currentUpdateTime = currentTime;
	}

	public void SetPositionAndRotation(Vector3 position, float fRotationY)
	{
		m_position = position;
		m_fRotationY = fRotationY;
	}

	public void AddWork(ISFRunnable work, bool bGlobalLockRequired)
	{
		m_worker.EnqueueWork(new SFAction<ISFRunnable, bool>(RunWork, work, bGlobalLockRequired));
	}

	private void RunWork(ISFRunnable work, bool bGlobalLockRequired)
	{
		if (bGlobalLockRequired)
		{
			lock (Global.syncObject)
			{
				InvokeRunWorkInternal(work);
				return;
			}
		}
		InvokeRunWorkInternal(work);
	}

	protected abstract void InvokeRunWorkInternal(ISFRunnable work);

	protected void RunWorkInternal(ISFRunnable work)
	{
		if (!m_bReleased)
		{
			work.Run();
		}
	}

	public void OnUpdate()
	{
		m_prevUpdateTime = m_currentUpdateTime;
		m_currentUpdateTime = DateTimeUtil.currentTime;
		OnUpdateInternal();
	}

	protected virtual void OnUpdateInternal()
	{
		if (m_prevUpdateTime.Date != m_currentUpdateTime.Date)
		{
			OnDateChanged();
		}
	}

	protected virtual void OnDateChanged()
	{
	}

	public void Release()
	{
		if (!m_bReleased)
		{
			ReleaseInternal();
			m_bReleased = true;
		}
	}

	protected virtual void ReleaseInternal()
	{
		ClearAllAbnormalStateEffects();
		m_worker.Stop(bClearQueue: true);
	}

	public void AddToSector(Sector sector)
	{
		if (sector == null)
		{
			throw new ArgumentException("sector");
		}
		if (m_sector != null)
		{
			throw new Exception("이미 섹터에 추가되어 있습니다.");
		}
		sector.AddUnit(this);
		m_sector = sector;
	}

	public void RemoveFromSector(bool bResetReference)
	{
		if (m_sector != null)
		{
			m_sector.RemoveUnit(this);
			if (bResetReference)
			{
				m_sector = null;
			}
		}
	}

	public void ChangeSector(Sector sector)
	{
		if (sector == null)
		{
			throw new ArgumentException("sector");
		}
		if (m_sector != sector)
		{
			RemoveFromSector(bResetReference: true);
			AddToSector(sector);
		}
	}

	public bool IsInterestSector(int nSectorRow, int nSectorColumn)
	{
		return m_sector.IsInterestSector(nSectorRow, nSectorColumn);
	}

	public bool IsInterestSector(Sector sector)
	{
		if (sector == null)
		{
			return false;
		}
		if (sector.place != m_currentPlace)
		{
			return false;
		}
		return IsInterestSector(sector.row, sector.column);
	}

	public bool Hit(OffenseHit offenseHit)
	{
		if (!hitEnabled)
		{
			return false;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		Offense offense = offenseHit.offense;
		Unit attacker = offense.attacker;
		Skill skill = offense.skill;
		HitResult hitResult = new HitResult();
		hitResult.offenseHit = offenseHit;
		hitResult.time = currentTime;
		int nDamage = 0;
		int nAttackerOffense = offense.power;
		int nDefenderDefense = ((offense.offenseType == OffenseType.Physical) ? m_nRealPhysicalDefense : m_nRealMagicalDefense);
		float fSkillOffenseFactor = (float)offense.offenseAmp / 10000f;
		int nSkillPoint = offense.offensePoint;
		nDamage = (int)Math.Floor((float)Math.Max(1, nAttackerOffense - nDefenderDefense) * fSkillOffenseFactor) + nSkillPoint;
		if (attacker.increaseDamageEnabledByProbability)
		{
			int nCritical = offense.critical;
			int nCriticalRate = Math.Min(10000, (int)Math.Floor((float)nCritical / (float)(nCritical + 6000) * 10000f) + 500);
			int nCriticalResistanceRate = Math.Min(10000, (int)Math.Floor((float)m_nRealCriticalResistance / (float)(m_nRealCriticalResistance + 6000) * 10000f) + 500);
			nCriticalRate = Math.Max(0, nCriticalRate - nCriticalResistanceRate);
			int nPenetration = offense.penetration;
			int nPenetrationRate = Math.Min(10000, (int)Math.Floor((float)nPenetration / (float)(nPenetration + 3000) * 10000f) + 500);
			int nNormalAttackRate = Math.Max(0, 10000 - nCriticalRate - nPenetrationRate);
			int nTotalRate = nCriticalRate + nPenetrationRate + nNormalAttackRate;
			int nSelectPoint = SFRandom.NextInt(1.0, nTotalRate);
			if (nSelectPoint <= nCriticalRate)
			{
				float fCriticalDamageIncFactor = (float)offense.criticalDamageIncRate / 10000f;
				float fCriticalDamageDecFactor = (float)m_nRealCriticalDamageDecRate / 10000f;
				float fCriticalFactor = Math.Max(0f, 1.5f + fCriticalDamageIncFactor - fCriticalDamageDecFactor);
				nDamage = (int)Math.Floor((float)nDamage * fCriticalFactor);
				hitResult.isCritical = true;
			}
			else if (nSelectPoint <= nCriticalRate + nPenetrationRate)
			{
				nDamage = (int)Math.Floor((float)nDamage * 1.2f);
				hitResult.isPenetration = true;
			}
		}
		if (blockEnabled)
		{
			int nBlockRate = Math.Min(10000, (int)Math.Floor((float)m_nRealBlock / (float)(m_nRealBlock + 3000) * 10000f) + 500);
			if (Util.DrawLots(nBlockRate))
			{
				nDamage = (int)Math.Floor((float)nDamage * 0.5f);
				hitResult.isBlocked = true;
			}
		}
		int nFireOffense = 0;
		int nLightningOffense = 0;
		int nDarkOffense = 0;
		int nHolyOffense = 0;
		switch (offense.elementalId)
		{
		case 1:
			nFireOffense = (int)Math.Floor((float)offense.fireOffense * 1.5f);
			nLightningOffense = (int)Math.Floor((float)offense.lightningOffense * 0.8f);
			nDarkOffense = (int)Math.Floor((float)offense.darkOffense * 0.8f);
			nHolyOffense = (int)Math.Floor((float)offense.holyOffense * 0.8f);
			break;
		case 2:
			nFireOffense = (int)Math.Floor((float)offense.fireOffense * 0.8f);
			nLightningOffense = (int)Math.Floor((float)offense.lightningOffense * 1.5f);
			nDarkOffense = (int)Math.Floor((float)offense.darkOffense * 0.8f);
			nHolyOffense = (int)Math.Floor((float)offense.holyOffense * 0.8f);
			break;
		case 4:
			nFireOffense = (int)Math.Floor((float)offense.fireOffense * 0.8f);
			nLightningOffense = (int)Math.Floor((float)offense.lightningOffense * 0.8f);
			nDarkOffense = (int)Math.Floor((float)offense.darkOffense * 1.5f);
			nHolyOffense = (int)Math.Floor((float)offense.holyOffense * 0.8f);
			break;
		case 3:
			nFireOffense = (int)Math.Floor((float)offense.fireOffense * 0.8f);
			nLightningOffense = (int)Math.Floor((float)offense.lightningOffense * 0.8f);
			nDarkOffense = (int)Math.Floor((float)offense.darkOffense * 0.8f);
			nHolyOffense = (int)Math.Floor((float)offense.holyOffense * 1.5f);
			break;
		default:
			nFireOffense = (int)Math.Floor((float)offense.fireOffense * 0.8f);
			nLightningOffense = (int)Math.Floor((float)offense.lightningOffense * 0.8f);
			nDarkOffense = (int)Math.Floor((float)offense.darkOffense * 0.8f);
			nHolyOffense = (int)Math.Floor((float)offense.holyOffense * 0.8f);
			break;
		}
		int nElementalDamage = Math.Max(0, nFireOffense - m_nRealFireDefense) + Math.Max(0, nLightningOffense - m_nRealLightningDefense) + Math.Max(0, nDarkOffense - m_nRealDarkDefense) + Math.Max(0, nHolyOffense - m_nRealHolyDefense);
		float fDamageIncFactor = (float)offense.damageIncRate / 10000f;
		float fDamageDecFactor = (float)m_nRealDamageDecRate / 10000f;
		nDamage = (int)Math.Floor((float)(nDamage + nElementalDamage) * Math.Max(0f, 1f + fDamageIncFactor - fDamageDecFactor));
		SkillHit hit = skill.GetHit(offenseHit.hitId);
		nDamage = (int)Math.Floor((float)nDamage * hit.damageFactor);
		List<AbnormalStateEffect> absorbShieldEffects = new List<AbnormalStateEffect>();
		AbnormalStateEffect immortalityEffect = null;
		foreach (AbnormalStateEffect effect in m_abnormalStateEffects.Values)
		{
			switch (effect.abnormalState.id)
			{
			case 8:
				nDamage = (int)Math.Floor((float)nDamage * ((float)effect.abnormalStateLevel.value1 / 10000f));
				break;
			case 10:
			case 102:
			case 109:
				absorbShieldEffects.Add(effect);
				break;
			case 106:
				if (immortalityEffect == null)
				{
					immortalityEffect = effect;
				}
				break;
			}
		}
		HashSet<long> removedAbnormalStateEffects = new HashSet<long>();
		if (nDamage <= 0)
		{
			nDamage = 1;
		}
		if (finalDamagePenaltyRate > 0)
		{
			nDamage -= (int)Math.Floor((float)nDamage * ((float)finalDamagePenaltyRate / 10000f));
		}
		absorbShieldEffects.Sort(AbnormalStateEffect.Compare);
		foreach (AbnormalStateEffect absorbShieldEffect in absorbShieldEffects)
		{
			if (nDamage > 0)
			{
				nDamage = absorbShieldEffect.AbsorbDamage(nDamage);
				int nRemainingDamageAbsorbShield = absorbShieldEffect.damageAbsorbShieldRemainingAmount;
				if (nRemainingDamageAbsorbShield <= 0 && (absorbShieldEffect.abnormalState.id == 102 || absorbShieldEffect.abnormalState.id == 10))
				{
					absorbShieldEffect.Stop();
					RemoveAbnormalStateEffect(absorbShieldEffect.id);
					removedAbnormalStateEffects.Add(absorbShieldEffect.id);
				}
				PDAbnormalStateEffectDamageAbsorbShield shield = new PDAbnormalStateEffectDamageAbsorbShield();
				shield.abnormalStateEffectInstanceId = absorbShieldEffect.id;
				shield.remainingAbsorbShieldAmount = nRemainingDamageAbsorbShield;
				hitResult.changedAbnormalStateEffectDamageAbsorbShields.Add(shield);
				continue;
			}
			break;
		}
		bool bIsImmortalityEffect = false;
		if (nDamage >= m_nHP && immortalityEffect != null && immortalityEffect.UseImmortalityEffect())
		{
			bIsImmortalityEffect = true;
			hitResult.isImmortal = true;
		}
		Damage(offense.attacker, nDamage, currentTime, bIsImmortalityEffect, removedAbnormalStateEffects);
		hitResult.hp = m_nHP;
		hitResult.damage = m_nLastDamage;
		hitResult.hpDamage = m_nLastHPDamage;
		hitResult.moveSpeed = moveSpeed;
		hitResult.AddRemovedAbnormalStateEffects(removedAbnormalStateEffects);
		SendHitEvent(hitResult);
		ProcessHitAbnormalState(attacker, skill.GetHit(offenseHit.hitId).abnormalStates, offense.skillLevel, currentTime);
		OnHit(hitResult);
		if (isDead)
		{
			OnDead();
			m_currentPlace.OnUnitDead(this);
		}
		return true;
	}

	private void ProcessHitAbnormalState(Unit source, ICollection<JobSkillHitAbnormalState> skillHitAbnormalStates, int nSkillLevel, DateTimeOffset currentTime)
	{
		if (type == UnitType.Hero)
		{
			if (source.type == UnitType.Hero)
			{
				AddWork(new SFAction<Hero, ICollection<JobSkillHitAbnormalState>, int, DateTimeOffset>(AsyncProcessHitAbnormalState, (Hero)source, skillHitAbnormalStates, nSkillLevel, currentTime), bGlobalLockRequired: true);
			}
		}
		else
		{
			ProcessHitAbnormalStateInternal(source, skillHitAbnormalStates, nSkillLevel, currentTime);
		}
	}

	private void AsyncProcessHitAbnormalState(Hero source, ICollection<JobSkillHitAbnormalState> skillHitAbnormalStates, int nSkillLevel, DateTimeOffset currentTime)
	{
		HeroSynchronizer.Exec(source, new SFAction<Unit, ICollection<JobSkillHitAbnormalState>, int, DateTimeOffset>(ProcessHitAbnormalStateInternal, source, skillHitAbnormalStates, nSkillLevel, currentTime));
	}

	private void ProcessHitAbnormalStateInternal(Unit source, ICollection<JobSkillHitAbnormalState> skillHitAbnormalStates, int nSkillLevel, DateTimeOffset currentTime)
	{
		if (skillHitAbnormalStates == null || skillHitAbnormalStates.Count == 0)
		{
			return;
		}
		foreach (JobSkillHitAbnormalState skillHitAbnormalState in skillHitAbnormalStates)
		{
			if (!Util.DrawLots(skillHitAbnormalState.hitRate))
			{
				break;
			}
			AbnormalState abnormalState = skillHitAbnormalState.abnormalState;
			AbnormalStateLevel abnormalStateLevel = null;
			if (source.type == UnitType.Hero)
			{
				Hero hero = (Hero)source;
				JobAbnormalState jobAbnormalState = abnormalState.GetJobAbnormalState(hero.jobId);
				if (jobAbnormalState == null)
				{
					continue;
				}
				abnormalStateLevel = jobAbnormalState.GetAbnormalStateJobSkillLevel(nSkillLevel);
			}
			if (abnormalStateLevel != null)
			{
				ProcessAbnormalState(source, abnormalStateLevel, currentTime);
			}
		}
	}

	protected virtual void OnHit(HitResult hitResult)
	{
	}

	protected abstract void SendHitEvent(HitResult hitResult);

	protected virtual void OnDead()
	{
	}

	protected void Damage(Unit attacker, int nDamage, DateTimeOffset time, bool bIsImmortalityEffect, ICollection<long> removedAbnormalStateEffects)
	{
		if (nDamage < 0)
		{
			return;
		}
		int nHPDamage = ((nDamage <= m_nHP) ? nDamage : m_nHP);
		if (bIsImmortalityEffect)
		{
			nHPDamage--;
		}
		m_nHP -= nHPDamage;
		m_lastAttacker = attacker;
		m_nLastDamage = nDamage;
		m_nLastHPDamage = nHPDamage;
		m_lastDamageTime = time;
		if (isDead)
		{
			foreach (AbnormalStateEffect effect in m_abnormalStateEffects.Values)
			{
				removedAbnormalStateEffects.Add(effect.id);
			}
			if (m_abnormalStateEffects.Count > 0)
			{
				ClearAllAbnormalStateEffects();
				RefreshRealValues(bSendMaxHpChangedToOthers: false);
			}
		}
		OnDamage();
		if (m_currentPlace != null)
		{
			m_currentPlace.OnUnitDamage(this);
		}
	}

	protected virtual void OnDamage()
	{
	}

	public void RestoreHP(int nAmount, bool bSendEventToMyself, bool bSendEventToOthers)
	{
		if (!isDead && nAmount > 0)
		{
			m_nHP = Math.Min(m_nHP + nAmount, m_nRealMaxHP);
			OnHPRestored(bSendEventToMyself, bSendEventToOthers);
		}
	}

	public virtual void OnHPRestored(bool bSendEventToMyself, bool bSendEventToOthers)
	{
	}

	protected virtual void ClearRealValues()
	{
		m_nRealMaxHP = 0;
		m_nRealPhysicalOffense = 0;
		m_nRealMagicalOffense = 0;
		m_nRealPhysicalDefense = 0;
		m_nRealMagicalDefense = 0;
		m_nRealCritical = 0;
		m_nRealCriticalResistance = 0;
		m_nRealCriticalDamageIncRate = 0;
		m_nRealCriticalDamageDecRate = 0;
		m_nRealPenetration = 0;
		m_nRealBlock = 0;
		m_nRealFireOffense = 0;
		m_nRealFireDefense = 0;
		m_nRealLightningOffense = 0;
		m_nRealLightningDefense = 0;
		m_nRealDarkOffense = 0;
		m_nRealDarkDefense = 0;
		m_nRealHolyOffense = 0;
		m_nRealHolyDefense = 0;
		m_nRealDamageIncRate = 0;
		m_nRealDamageDecRate = 0;
		m_nRealStunResistance = 0;
		m_nRealSnareResistance = 0;
		m_nRealSilenceResistance = 0;
		m_nRealBaseMaxHPIncRate = 0;
		m_nRealBaseOffenseIncRate = 0;
		m_nRealBasePhysicalDefenseIncRate = 0;
		m_nRealBaseMagicalDefenseIncRate = 0;
		m_nRealOffense = 0;
	}

	protected void AddAttrRealValue(int nAttrId, int nValue)
	{
		switch (nAttrId)
		{
		case 1:
			m_nRealMaxHP += nValue;
			break;
		case 2:
			m_nRealPhysicalOffense += nValue;
			break;
		case 3:
			m_nRealMagicalOffense += nValue;
			break;
		case 4:
			m_nRealPhysicalDefense += nValue;
			break;
		case 5:
			m_nRealMagicalDefense += nValue;
			break;
		case 6:
			m_nRealCritical += nValue;
			break;
		case 7:
			m_nRealCriticalResistance += nValue;
			break;
		case 8:
			m_nRealCriticalDamageIncRate += nValue;
			break;
		case 9:
			m_nRealCriticalDamageDecRate += nValue;
			break;
		case 10:
			m_nRealPenetration += nValue;
			break;
		case 11:
			m_nRealBlock += nValue;
			break;
		case 12:
			m_nRealFireOffense += nValue;
			break;
		case 13:
			m_nRealFireDefense += nValue;
			break;
		case 14:
			m_nRealLightningOffense += nValue;
			break;
		case 15:
			m_nRealLightningDefense += nValue;
			break;
		case 16:
			m_nRealDarkOffense += nValue;
			break;
		case 17:
			m_nRealDarkDefense += nValue;
			break;
		case 18:
			m_nRealHolyOffense += nValue;
			break;
		case 19:
			m_nRealHolyDefense += nValue;
			break;
		case 20:
			m_nRealDamageIncRate += nValue;
			break;
		case 21:
			m_nRealDamageDecRate += nValue;
			break;
		case 22:
			m_nRealStunResistance += nValue;
			break;
		case 23:
			m_nRealSnareResistance += nValue;
			break;
		case 24:
			m_nRealSilenceResistance += nValue;
			break;
		case 25:
			m_nRealBaseMaxHPIncRate += nValue;
			break;
		case 26:
			m_nRealBaseOffenseIncRate += nValue;
			break;
		case 27:
			m_nRealBasePhysicalDefenseIncRate += nValue;
			break;
		case 28:
			m_nRealBaseMagicalDefenseIncRate += nValue;
			break;
		case 29:
			m_nRealOffense += nValue;
			break;
		}
	}

	protected void MultiplyRealValue(int nAttrId, float fFactor)
	{
		switch (nAttrId)
		{
		case 1:
			m_nRealMaxHP = (int)Math.Floor((float)m_nRealMaxHP * fFactor);
			break;
		case 2:
			m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * fFactor);
			break;
		case 3:
			m_nRealMagicalOffense = (int)Math.Floor((float)m_nRealMagicalOffense * fFactor);
			break;
		case 4:
			m_nRealPhysicalDefense = (int)Math.Floor((float)m_nRealPhysicalDefense * fFactor);
			break;
		case 5:
			m_nRealMagicalDefense = (int)Math.Floor((float)m_nRealMagicalDefense * fFactor);
			break;
		case 6:
			m_nRealCritical = (int)Math.Floor((float)m_nRealCritical * fFactor);
			break;
		case 7:
			m_nRealCriticalResistance = (int)Math.Floor((float)m_nRealCriticalResistance * fFactor);
			break;
		case 8:
			m_nRealCriticalDamageIncRate = (int)Math.Floor((float)m_nRealCriticalDamageIncRate * fFactor);
			break;
		case 9:
			m_nRealCriticalDamageDecRate = (int)Math.Floor((float)m_nRealCriticalDamageDecRate * fFactor);
			break;
		case 10:
			m_nRealPenetration = (int)Math.Floor((float)m_nRealPenetration * fFactor);
			break;
		case 11:
			m_nRealBlock = (int)Math.Floor((float)m_nRealBlock * fFactor);
			break;
		case 12:
			m_nRealFireOffense = (int)Math.Floor((float)m_nRealFireOffense * fFactor);
			break;
		case 13:
			m_nRealFireDefense = (int)Math.Floor((float)m_nRealFireDefense * fFactor);
			break;
		case 14:
			m_nRealLightningOffense = (int)Math.Floor((float)m_nRealLightningOffense * fFactor);
			break;
		case 15:
			m_nRealLightningDefense = (int)Math.Floor((float)m_nRealLightningDefense * fFactor);
			break;
		case 16:
			m_nRealDarkOffense = (int)Math.Floor((float)m_nRealDarkOffense * fFactor);
			break;
		case 17:
			m_nRealDarkDefense = (int)Math.Floor((float)m_nRealDarkDefense * fFactor);
			break;
		case 18:
			m_nRealHolyOffense = (int)Math.Floor((float)m_nRealHolyOffense * fFactor);
			break;
		case 19:
			m_nRealHolyDefense = (int)Math.Floor((float)m_nRealHolyDefense * fFactor);
			break;
		case 20:
			m_nRealDamageIncRate = (int)Math.Floor((float)m_nRealDamageIncRate * fFactor);
			break;
		case 21:
			m_nRealDamageDecRate = (int)Math.Floor((float)m_nRealDamageDecRate * fFactor);
			break;
		case 22:
			m_nRealStunResistance = (int)Math.Floor((float)m_nRealStunResistance * fFactor);
			break;
		case 23:
			m_nRealSnareResistance = (int)Math.Floor((float)m_nRealSnareResistance * fFactor);
			break;
		case 24:
			m_nRealSilenceResistance = (int)Math.Floor((float)m_nRealSilenceResistance * fFactor);
			break;
		case 25:
			m_nRealBaseMaxHPIncRate = (int)Math.Floor((float)m_nRealBaseMaxHPIncRate * fFactor);
			break;
		case 26:
			m_nRealBaseOffenseIncRate = (int)Math.Floor((float)m_nRealBaseOffenseIncRate * fFactor);
			break;
		case 27:
			m_nRealBasePhysicalDefenseIncRate = (int)Math.Floor((float)m_nRealBasePhysicalDefenseIncRate * fFactor);
			break;
		case 28:
			m_nRealBaseMagicalDefenseIncRate = (int)Math.Floor((float)m_nRealBaseMagicalDefenseIncRate * fFactor);
			break;
		case 29:
			m_nRealOffense = (int)Math.Floor((float)m_nRealOffense * fFactor);
			break;
		}
	}

	public virtual void RefreshRealValues(bool bSendMaxHpChangedToOthers)
	{
		int nOldRealMaxHP = m_nRealMaxHP;
		ClearRealValues();
		RefreshRealValues_Sum();
		RefreshRealValues_Multiplication();
		RefreshRealValues_LastApply();
		if (m_nHP > m_nRealMaxHP)
		{
			m_nHP = m_nRealMaxHP;
		}
		if (bSendMaxHpChangedToOthers && nOldRealMaxHP != m_nRealMaxHP)
		{
			SendMaxHPChangedEventToOthers();
		}
	}

	protected virtual void RefreshRealValues_Sum()
	{
	}

	protected virtual void RefreshRealValues_Multiplication()
	{
	}

	protected virtual void RefreshRealValues_LastApply()
	{
	}

	protected virtual void RefreshMoveSpeed()
	{
	}

	protected abstract void SendMaxHPChangedEventToOthers();

	public void ProcessAbnormalState(Unit source, AbnormalStateLevel abnormalStateLevel, DateTimeOffset currentTime)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (abnormalStateLevel == null)
		{
			throw new ArgumentNullException("abnormalStateLevel");
		}
		if (isDead)
		{
			return;
		}
		AbnormalState abnormalState = abnormalStateLevel.abnormalState;
		if (!IsAbnormalStateHitEnabled(source, abnormalState) || (isAbnormalStateImmune && abnormalState.type == 1) || abnormalStateLevel.duration <= 0)
		{
			return;
		}
		bool bIsOverlab = abnormalState.isOverlap;
		List<long> removedAbnormalStateEffects = new List<long>();
		AbnormalStateEffect[] array = m_abnormalStateEffects.Values.ToArray();
		foreach (AbnormalStateEffect effect in array)
		{
			if ((abnormalState.id == 101 && effect.abnormalState.type == 1) || (!bIsOverlab && effect.abnormalState.id == abnormalState.id))
			{
				effect.Stop();
				RemoveAbnormalStateEffect(effect.id);
				removedAbnormalStateEffects.Add(effect.id);
			}
		}
		AbnormalStateEffect newEffect = new AbnormalStateEffect();
		newEffect.Init(source, this, abnormalStateLevel, currentTime);
		AddAbnormalStateEffect(newEffect);
		RefreshRealValues(bSendMaxHpChangedToOthers: true);
		if (abnormalState.id == 11)
		{
			RefreshMoveSpeed();
		}
		SendAbnormalStateEffectStart(newEffect, newEffect.damageAbsorbShieldRemainingAmount, removedAbnormalStateEffects);
	}

	protected virtual bool IsAbnormalStateHitEnabled(Unit source, AbnormalState abnormalState)
	{
		return abnormalStateHitEnabled;
	}

	protected void AddAbnormalStateEffect(AbnormalStateEffect effect)
	{
		m_abnormalStateEffects.Add(effect.id, effect);
		OnAbnormalStateEffectAdded(effect);
	}

	protected virtual void OnAbnormalStateEffectAdded(AbnormalStateEffect effect)
	{
	}

	protected abstract void SendAbnormalStateEffectStart(AbnormalStateEffect effect, int nDamageAbsorbShield, IEnumerable<long> removedAbnormalStateEffects);

	protected void RemoveAbnormalStateEffect(long lnId)
	{
		m_abnormalStateEffects.Remove(lnId);
	}

	protected void ClearAllAbnormalStateEffects()
	{
		foreach (AbnormalStateEffect effect in m_abnormalStateEffects.Values)
		{
			effect.Stop();
		}
		m_abnormalStateEffects.Clear();
	}

	protected bool ContainsAbnormalStateEffectOfAbnormalState(int nAbnormalStateId)
	{
		AbnormalStateEffect effect = GetAbnormalStateEffect(nAbnormalStateId);
		if (effect == null)
		{
			return false;
		}
		return true;
	}

	private AbnormalStateEffect GetAbnormalStateEffect(int nAbnormalStateId)
	{
		foreach (AbnormalStateEffect effect in m_abnormalStateEffects.Values)
		{
			if (effect.abnormalState.id == nAbnormalStateId)
			{
				return effect;
			}
		}
		return null;
	}

	public void OnAbnormalStateEffectTick(AbnormalStateEffect effect)
	{
		Unit attacker = effect.source;
		AbnormalState abnormalState = effect.abnormalState;
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (!abnormalState.isTickPerSecondAbnormalState)
		{
			return;
		}
		if (effect.abnormalState.id == 11)
		{
			RestoreHP(effect.hpRestoreAmount, bSendEventToMyself: true, bSendEventToOthers: true);
		}
		else if (IsAbnormalStateDamageHitEnabled(effect))
		{
			int nDamage = effect.sourceAbnormalStateOffense;
			if (nDamage <= 0)
			{
				nDamage = 1;
			}
			if (finalDamagePenaltyRate > 0)
			{
				nDamage -= (int)Math.Floor((float)nDamage * ((float)finalDamagePenaltyRate / 10000f));
			}
			List<long> removedAbnormalStateEffects = new List<long>();
			Damage(attacker, nDamage, currentTime, bIsImmortalityEffect: false, removedAbnormalStateEffects);
			SendAbnormalStateEffectHit(effect, removedAbnormalStateEffects);
			OnAbnormalStateEffectHit();
			if (isDead)
			{
				OnDead();
				m_currentPlace.OnUnitDead(this);
			}
		}
	}

	protected abstract void SendAbnormalStateEffectHit(AbnormalStateEffect effect, IEnumerable<long> removedAbnormalStateEffects);

	protected virtual void OnAbnormalStateEffectHit()
	{
	}

	public void ProcessAbnormalStateEffectFinished(AbnormalStateEffect effect)
	{
		RemoveAbnormalStateEffect(effect.id);
		RefreshRealValues(bSendMaxHpChangedToOthers: true);
		SendAbnormalStateEffectFinishedEvent(effect);
	}

	protected abstract void SendAbnormalStateEffectFinishedEvent(AbnormalStateEffect effect);

	protected virtual bool IsAbnormalStateDamageHitEnabled(AbnormalStateEffect effect)
	{
		return abnormalStateDamageEnabled;
	}

	public abstract PDAttacker ToPDAttacker();
}
