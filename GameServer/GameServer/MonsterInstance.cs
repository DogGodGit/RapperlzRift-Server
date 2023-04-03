using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public abstract class MonsterInstance : Unit
{
	protected long m_lnInstanceId;

	protected int m_nMoveSpeed;

	protected int m_nBattleMoveSpeed;

	protected Hero m_owner;

	protected MonsterOwnerType m_ownerType;

	protected Dictionary<Guid, MonsterReceivedDamage> m_receivedDamages = new Dictionary<Guid, MonsterReceivedDamage>();

	protected bool m_bIsReceivedDamageRankingInvalid;

	protected MonsterReceivedDamage[] m_sortedReceivedDamages = new MonsterReceivedDamage[0];

	protected Dictionary<Guid, Aggro> m_aggroes = new Dictionary<Guid, Aggro>();

	protected bool m_bIsMaxAggroInvalid = true;

	protected Aggro m_maxAggro;

	protected Vector3 m_spawnedPosition = Vector3.zero;

	protected float m_fSpawnedRotationY;

	protected List<MonsterInstanceSkill> m_monsterInstanceSkills = new List<MonsterInstanceSkill>();

	protected DateTimeOffset m_lastSkillCastTime = DateTimeOffset.MinValue;

	protected float m_fSkillCastingInterval;

	protected bool m_bIsReturnMode;

	protected int m_nMaxMentalStrength;

	protected int m_nMentalStrength;

	protected Dictionary<Guid, MonsterReceivedMentalDamage> m_receivedMentalDamages = new Dictionary<Guid, MonsterReceivedMentalDamage>();

	protected bool m_bIsReceivedMentalDamageRankingInvalid;

	protected MonsterReceivedMentalDamage[] m_sortedReceivedMentalDamages = new MonsterReceivedMentalDamage[0];

	protected Timer m_groggyTimer;

	protected Hero m_targetTamer;

	public static readonly SFSynchronizedLongFactory instanceIdFactory = new SFSynchronizedLongFactory();

	public override UnitType type => UnitType.MonsterInstance;

	public abstract MonsterInstanceType monsterInstanceType { get; }

	public long instanceId => m_lnInstanceId;

	public abstract Monster monster { get; }

	public MonsterCharacter monsterCharacter => monster.monsterCharacter;

	public virtual int nationId => 0;

	public virtual bool aggroManaged => true;

	public override bool skillEnabled
	{
		get
		{
			if (base.skillEnabled && monster.attackEnabled)
			{
				return !isGroggy;
			}
			return false;
		}
	}

	public override bool moveEnabled
	{
		get
		{
			if (base.moveEnabled && monster.moveEnabled)
			{
				return !isGroggy;
			}
			return false;
		}
	}

	public override bool hitEnabled
	{
		get
		{
			if (base.hitEnabled)
			{
				return !isGroggy;
			}
			return false;
		}
	}

	public override bool abnormalStateHitEnabled
	{
		get
		{
			if (base.abnormalStateHitEnabled)
			{
				return !isGroggy;
			}
			return false;
		}
	}

	public override bool increaseDamageEnabledByProbability => false;

	public override bool blockEnabled => false;

	protected virtual float maxHpFactor => 1f;

	protected virtual float offenseFactor => 1f;

	public bool isReturnMode => m_bIsReturnMode;

	public virtual bool normalItemLootingEnabled => true;

	public virtual bool isExclusive => false;

	public virtual Guid exclusiveHeroId => Guid.Empty;

	public virtual string exclusiveHeroName => null;

	public int level => monster.level;

	public override int moveSpeed => m_nMoveSpeed;

	public int battleMoveSpeed => m_nBattleMoveSpeed;

	public Hero owner => m_owner;

	public Guid ownerId
	{
		get
		{
			if (m_owner == null)
			{
				return Guid.Empty;
			}
			return m_owner.id;
		}
	}

	public MonsterOwnerType ownerType => m_ownerType;

	public Dictionary<Guid, MonsterReceivedDamage> receivedDamages => m_receivedDamages;

	public MonsterReceivedDamage[] sortedReceivedDamages
	{
		get
		{
			RefreshReceivedDamageRanking();
			return m_sortedReceivedDamages;
		}
	}

	public Dictionary<Guid, Aggro> aggroes => m_aggroes;

	public Vector3 spawnedPosition => m_spawnedPosition;

	public float spawendRotationY => m_fSpawnedRotationY;

	public float visibilityRange => monster.visibilityRange;

	public int maxMentalStrength => m_nMaxMentalStrength;

	public int mentalStrength => m_nMentalStrength;

	public bool isGroggy
	{
		get
		{
			if (monster.tamingEnabled)
			{
				return m_nMentalStrength <= 0;
			}
			return false;
		}
	}

	public Dictionary<Guid, MonsterReceivedMentalDamage> receivedMentalDamages => m_receivedMentalDamages;

	public MonsterReceivedMentalDamage[] sortedReceivedMentalDamages
	{
		get
		{
			RefreshReceivedDamageRanking();
			return m_sortedReceivedMentalDamages;
		}
	}

	public Hero targetTamer => m_targetTamer;

	public Guid targetTamerId
	{
		get
		{
			if (m_targetTamer == null)
			{
				return Guid.Empty;
			}
			return m_targetTamer.id;
		}
	}

	public MonsterInstance()
	{
		m_lnInstanceId = instanceIdFactory.NewValue();
	}

	protected void InitMonsterInstance(Place place, Vector3 position, float fRotationY)
	{
		if (place == null)
		{
			throw new ArgumentException("place");
		}
		m_currentPlace = place;
		m_spawnedPosition = position;
		m_fSpawnedRotationY = fRotationY;
		m_position = position;
		m_fRotationY = m_fSpawnedRotationY;
		InitUnit(DateTimeUtil.currentTime);
		AddToSector(m_currentPlace.GetSectorOfPosition(m_position));
		RefreshRealValues(bSendMaxHpChangedToOthers: false);
		RefreshMoveSpeed();
		m_nHP = m_nRealMaxHP;
		if (monster.tamingEnabled)
		{
			m_nMaxMentalStrength = monster.mentalStrength;
			m_nMentalStrength = m_nMaxMentalStrength;
		}
		foreach (MonsterOwnSkill ownSkill in monster.ownSkills)
		{
			MonsterInstanceSkill monsterInstanceSkill = new MonsterInstanceSkill(this, ownSkill.skill);
			m_monsterInstanceSkills.Add(monsterInstanceSkill);
		}
		m_monsterInstanceSkills.Sort(MonsterInstanceSkill.Compare);
		m_fSkillCastingInterval = Math.Max(monster.skillCastingInterval, 0f);
	}

	protected override void InvokeRunWorkInternal(ISFRunnable work)
	{
		lock (m_currentPlace.syncObject)
		{
			RunWorkInternal(work);
		}
	}

	protected override void OnUpdateInternal()
	{
		base.OnUpdateInternal();
		OnUpdate_UpdateOwnership();
		OnUpdate_SkillCast();
	}

	protected override void RefreshMoveSpeed()
	{
		m_nMoveSpeed += monster.moveSpeed;
		m_nBattleMoveSpeed += monster.battleMoveSpeed;
		foreach (AbnormalStateEffect effect in m_abnormalStateEffects.Values)
		{
			if (effect.abnormalState.id == 11)
			{
				int nValue1 = effect.abnormalStateLevel.value1;
				m_nMoveSpeed = (int)Math.Floor((float)m_nMoveSpeed * ((float)nValue1 / 10000f));
				m_nBattleMoveSpeed = (int)Math.Floor((float)m_nBattleMoveSpeed * ((float)nValue1 / 10000f));
			}
		}
	}

	protected override void RefreshRealValues_Sum()
	{
		base.RefreshRealValues_Sum();
		m_nRealMaxHP += monster.maxHP;
		m_nRealPhysicalOffense += monster.physicalOffense;
	}

	protected override void RefreshRealValues_Multiplication()
	{
		base.RefreshRealValues_Multiplication();
		m_nRealMaxHP = (int)Math.Floor((float)m_nRealMaxHP * maxHpFactor);
		m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * offenseFactor);
	}

	protected override void RefreshRealValues_LastApply()
	{
		base.RefreshRealValues_LastApply();
		_ = m_nRealMaxHP;
		int nBasePhysicalOffense = m_nRealPhysicalOffense;
		int nBaseMagicalOffense = m_nRealMagicalOffense;
		int nBasePhysicalDefense = m_nRealPhysicalDefense;
		int nBaseMagicalDefense = m_nRealMagicalDefense;
		_ = m_nRealCritical;
		_ = m_nRealCriticalResistance;
		_ = m_nRealCriticalDamageIncRate;
		_ = m_nRealCriticalDamageDecRate;
		_ = m_nRealPenetration;
		_ = m_nRealBlock;
		int nBaseFireOffense = m_nRealFireOffense;
		int nBaseFireDefense = m_nRealFireDefense;
		int nBaseLightningOffense = m_nRealLightningOffense;
		int nBaseLightningDefense = m_nRealLightningDefense;
		int nBaseDarkOffense = m_nRealDarkOffense;
		int nBaseDarkDefense = m_nRealDarkDefense;
		int nBaseHolyOffense = m_nRealHolyOffense;
		int nBaseHolyDefense = m_nRealHolyDefense;
		_ = m_nRealDamageIncRate;
		_ = m_nRealDamageDecRate;
		_ = m_nRealStunResistance;
		_ = m_nRealSnareResistance;
		_ = m_nRealSilenceResistance;
		foreach (AbnormalStateEffect effect in m_abnormalStateEffects.Values)
		{
			AbnormalStateLevel abnormalStateLevel = effect.abnormalStateLevel;
			switch (effect.abnormalState.id)
			{
			case 5:
			{
				int nValue1 = abnormalStateLevel.value1;
				m_nRealPhysicalOffense += (int)Math.Floor((float)nBasePhysicalOffense * ((float)nValue1 / 10000f));
				m_nRealMagicalOffense += (int)Math.Floor((float)nBaseMagicalOffense * ((float)nValue1 / 10000f));
				break;
			}
			case 6:
			{
				int nValue2 = abnormalStateLevel.value1;
				m_nRealPhysicalDefense += (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue2 / 10000f));
				m_nRealMagicalDefense += (int)Math.Floor((float)nBaseMagicalDefense * ((float)nValue2 / 10000f));
				break;
			}
			case 7:
			{
				int nValue3 = abnormalStateLevel.value1;
				m_nRealFireDefense += (int)Math.Floor((float)nBaseFireDefense * ((float)nValue3 / 10000f));
				m_nRealLightningDefense += (int)Math.Floor((float)nBaseLightningDefense * ((float)nValue3 / 10000f));
				m_nRealDarkDefense += (int)Math.Floor((float)nBaseDarkDefense * ((float)nValue3 / 10000f));
				m_nRealHolyDefense += (int)Math.Floor((float)nBaseHolyDefense * ((float)nValue3 / 10000f));
				break;
			}
			case 13:
			{
				int nValue4 = abnormalStateLevel.value1;
				int nValue14 = abnormalStateLevel.value2;
				m_nRealCritical += nValue4;
				m_nRealCriticalDamageIncRate = nValue14;
				break;
			}
			case 103:
			{
				int nValue5 = abnormalStateLevel.value1;
				int nValue15 = abnormalStateLevel.value2;
				m_nRealPhysicalDefense += (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue5 / 10000f)) + nValue15;
				break;
			}
			case 104:
			{
				int nValue6 = abnormalStateLevel.value1;
				int nValue16 = abnormalStateLevel.value2;
				m_nRealPhysicalDefense += (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue6 / 10000f)) + nValue16;
				break;
			}
			case 105:
			{
				int nValue7 = abnormalStateLevel.value1;
				int nValue17 = abnormalStateLevel.value2;
				int nValue23 = abnormalStateLevel.value3;
				m_nRealPhysicalOffense = (int)Math.Floor((float)nBasePhysicalOffense * ((float)nValue7 / 10000f)) + nValue17;
				m_nRealPenetration = nValue23;
				break;
			}
			case 106:
			{
				int nValue8 = abnormalStateLevel.value1;
				int nValue18 = abnormalStateLevel.value2;
				int nValue24 = abnormalStateLevel.value3;
				m_nRealPhysicalOffense = (int)Math.Floor((float)nBasePhysicalOffense * ((float)nValue8 / 10000f)) + nValue18;
				m_nRealPenetration = nValue24;
				break;
			}
			case 107:
			{
				int nValue9 = abnormalStateLevel.value1;
				int nValue19 = abnormalStateLevel.value2;
				int nValue25 = abnormalStateLevel.value3;
				m_nRealPhysicalOffense = (int)Math.Floor((float)nBasePhysicalOffense * ((float)nValue9 / 10000f)) + nValue19;
				m_nRealCritical += nValue25;
				break;
			}
			case 108:
			{
				int nValue10 = abnormalStateLevel.value1;
				int nValue20 = abnormalStateLevel.value2;
				int nValue26 = abnormalStateLevel.value3;
				int nValue29 = abnormalStateLevel.value4;
				m_nRealMagicalOffense = (int)Math.Floor((float)nBaseMagicalOffense * ((float)nValue10 / 10000f)) + nValue20;
				m_nRealPhysicalDefense = (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue26 / 10000f)) + nValue29;
				m_nRealMagicalDefense = (int)Math.Floor((float)nBaseMagicalDefense * ((float)nValue26 / 10000f)) + nValue29;
				break;
			}
			case 109:
			{
				int nValue11 = abnormalStateLevel.value1;
				int nValue21 = abnormalStateLevel.value2;
				int nValue27 = abnormalStateLevel.value3;
				int nValue30 = abnormalStateLevel.value4;
				m_nRealMagicalOffense = (int)Math.Floor((float)nBaseMagicalOffense * ((float)nValue11 / 10000f)) + nValue21;
				m_nRealPhysicalDefense = (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue27 / 10000f)) + nValue30;
				m_nRealMagicalDefense = (int)Math.Floor((float)nBaseMagicalDefense * ((float)nValue27 / 10000f)) + nValue30;
				break;
			}
			case 110:
			{
				int nValue12 = abnormalStateLevel.value1;
				int nValue22 = abnormalStateLevel.value2;
				int nValue28 = abnormalStateLevel.value3;
				int nValue31 = abnormalStateLevel.value4;
				int nValue32 = abnormalStateLevel.value5;
				int nValue33 = abnormalStateLevel.value6;
				m_nRealMagicalOffense = (int)Math.Floor((float)nBaseMagicalOffense * ((float)nValue12 / 10000f)) + nValue22;
				m_nRealPhysicalDefense = (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue28 / 10000f)) + nValue31;
				m_nRealMagicalDefense = (int)Math.Floor((float)nBaseMagicalDefense * ((float)nValue28 / 10000f)) + nValue31;
				m_nRealFireOffense = (int)Math.Floor((float)nBaseFireOffense * ((float)nValue32 / 10000f)) + nValue33;
				m_nRealLightningOffense = (int)Math.Floor((float)nBaseLightningOffense * ((float)nValue32 / 10000f)) + nValue33;
				m_nRealDarkOffense = (int)Math.Floor((float)nBaseDarkOffense * ((float)nValue32 / 10000f)) + nValue33;
				m_nRealHolyOffense = (int)Math.Floor((float)nBaseHolyOffense * ((float)nValue32 / 10000f)) + nValue33;
				break;
			}
			case 111:
			{
				int nValue13 = abnormalStateLevel.value1;
				m_nRealPhysicalDefense = (int)Math.Floor((float)nBasePhysicalDefense * ((float)nValue13 / 10000f));
				m_nRealMagicalDefense = (int)Math.Floor((float)nBaseMagicalDefense * ((float)nValue13 / 10000f));
				break;
			}
			}
		}
	}

	private void OnUpdate_UpdateOwnership()
	{
		try
		{
			UpdateOwnership();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void UpdateOwnership()
	{
		if (!m_currentPlace.ownershipManaged)
		{
			return;
		}
		RefreshMaxAggro();
		if (m_maxAggro != null)
		{
			lock (m_maxAggro.target.syncObject)
			{
				SetOwnership(m_maxAggro.target, MonsterOwnerType.Target);
			}
		}
		else if (m_ownerType == MonsterOwnerType.Target)
		{
			RemoveOwnership(bRemoveFromHero: true);
		}
		UpdateOwnership_Controller();
	}

	private void UpdateOwnership_Controller()
	{
		if (m_ownerType != 0)
		{
			return;
		}
		Hero hero = null;
		if (m_currentPlace.interestManaged)
		{
			hero = m_sector.heroes.Values.FirstOrDefault();
			if (hero == null)
			{
				foreach (Sector sector in m_currentPlace.GetInterestSectors(m_sector))
				{
					hero = sector.heroes.Values.FirstOrDefault();
					if (hero != null)
					{
						break;
					}
				}
			}
		}
		else
		{
			hero = m_currentPlace.heroes.Values.FirstOrDefault();
		}
		if (hero == null)
		{
			return;
		}
		lock (hero.syncObject)
		{
			SetOwnership(hero, MonsterOwnerType.Controller);
		}
	}

	public virtual void SetOwnership(Hero owner, MonsterOwnerType ownerType)
	{
		if (owner == null)
		{
			throw new ArgumentNullException("owner");
		}
		if (ownerType == MonsterOwnerType.None)
		{
			throw new ArgumentException("해당 소유자타입은 허용되지 않습니다.");
		}
		if (m_owner != owner || m_ownerType != ownerType)
		{
			if ((m_owner != owner) ? true : false)
			{
				RemoveOwnership(bRemoveFromHero: true);
				m_owner = owner;
				m_owner.AddOwnMonster(this);
			}
			m_ownerType = ownerType;
			ServerEvent.SendMonsterOwnershipChange(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, m_owner.id, m_ownerType);
		}
	}

	public void RemoveOwnership(bool bRemoveFromHero)
	{
		if (m_owner == null)
		{
			return;
		}
		Hero owner = m_owner;
		m_owner = null;
		m_ownerType = MonsterOwnerType.None;
		if (bRemoveFromHero)
		{
			lock (owner.syncObject)
			{
				owner.RemoveOwnMonster(this);
			}
		}
		ServerEvent.SendMonsterOwnershipChange(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, Guid.Empty, m_ownerType);
	}

	private void OnUpdate_SkillCast()
	{
		try
		{
			SkillCast();
		}
		catch (Exception ex)
		{
			SFLogUtil.Error(GetType(), null, ex);
		}
	}

	private void SkillCast()
	{
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (m_fSkillCastingInterval <= 0f || (currentTime - m_lastSkillCastTime).TotalSeconds <= (double)m_fSkillCastingInterval || !m_currentPlace.battleEnabled || !skillEnabled || m_ownerType != MonsterOwnerType.Target)
		{
			return;
		}
		int nTopAutoPriorityGroup = 0;
		int nTotalAutoWeight = 0;
		List<MonsterInstanceSkill> castAbleSkills = new List<MonsterInstanceSkill>();
		foreach (MonsterInstanceSkill monsterInstanceSkill in m_monsterInstanceSkills)
		{
			MonsterSkill monsterSkill = monsterInstanceSkill.monsterSkill;
			if (nTopAutoPriorityGroup == 0)
			{
				if (monsterInstanceSkill.IsExpiredSkillCoolTime(currentTime))
				{
					nTopAutoPriorityGroup = monsterSkill.autoPriorityGroup;
					nTotalAutoWeight += monsterSkill.autoWeight;
					castAbleSkills.Add(monsterInstanceSkill);
				}
				continue;
			}
			if (monsterSkill.autoPriorityGroup == nTopAutoPriorityGroup)
			{
				if (monsterInstanceSkill.IsExpiredSkillCoolTime(currentTime))
				{
					nTotalAutoWeight += monsterSkill.autoWeight;
					castAbleSkills.Add(monsterInstanceSkill);
				}
				continue;
			}
			break;
		}
		if (castAbleSkills.Count == 0)
		{
			return;
		}
		MonsterInstanceSkill currentMonsterInstanceSkill = Util.SelectPickEntry(castAbleSkills, nTotalAutoWeight);
		if (currentMonsterInstanceSkill == null)
		{
			return;
		}
		MonsterSkill currentMonsterSkill = currentMonsterInstanceSkill.monsterSkill;
		Vector3 skillTargetPosition = Vector3.zero;
		if (currentMonsterSkill.formType == 2)
		{
			return;
		}
		float fCastRange = currentMonsterSkill.castRange;
		lock (m_owner.syncObject)
		{
			skillTargetPosition = m_owner.position;
			if (!MathUtil.SphereContains(m_position, fCastRange, skillTargetPosition))
			{
				return;
			}
		}
		currentMonsterInstanceSkill.castTime = currentTime;
		m_lastSkillCastTime = currentTime;
		float fRotationY = MathUtil.IncludedAngle(Vector3.forward, skillTargetPosition - m_position);
		if (float.IsNaN(fRotationY))
		{
			fRotationY = m_fRotationY;
		}
		m_fRotationY = fRotationY;
		ServerEvent.SendMonsterSkillCast(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, currentMonsterSkill.skillId, skillTargetPosition);
		m_currentPlace.ProcessMonsterSkillHit(currentMonsterInstanceSkill, skillTargetPosition);
	}

	protected override void OnDamage()
	{
		base.OnDamage();
		if (m_nLastHPDamage > 0)
		{
			Hero attacker = (Hero)m_lastAttacker;
			AddReceivedDamage(attacker, m_nLastHPDamage, m_lastDamageTime);
			if (base.isDead)
			{
				RefreshReceivedDamageRanking();
			}
		}
	}

	private void AddReceivedDamage(Hero attacker, long lnDamage, DateTimeOffset time)
	{
		MonsterReceivedDamage damage = GetOrCreateReceivedDamage(attacker.id, attacker.name);
		damage.AddDamage(lnDamage, time);
		m_bIsReceivedDamageRankingInvalid = true;
		AddAggro(attacker, lnDamage);
	}

	public MonsterReceivedDamage GetReceivedDamage(Guid attackerId)
	{
		if (!m_receivedDamages.TryGetValue(attackerId, out var value))
		{
			return null;
		}
		return value;
	}

	public MonsterReceivedDamage GetOrCreateReceivedDamage(Guid attackerId, string sAttackeName)
	{
		MonsterReceivedDamage damage = GetReceivedDamage(attackerId);
		if (damage == null)
		{
			damage = new MonsterReceivedDamage(attackerId, sAttackeName);
			m_receivedDamages.Add(damage.attackerId, damage);
		}
		return damage;
	}

	private void RefreshReceivedDamageRanking()
	{
		if (m_bIsReceivedDamageRankingInvalid)
		{
			MonsterReceivedDamage[] damages = m_receivedDamages.Values.ToArray();
			Array.Sort(damages, MonsterReceivedDamage.Compare);
			Array.Reverse(damages);
			for (int i = 0; i < damages.Length; i++)
			{
				damages[i].rank = i + 1;
			}
			m_sortedReceivedDamages = damages;
			m_bIsReceivedDamageRankingInvalid = false;
		}
	}

	protected MonsterReceivedDamage GetMaxReceivedDamage()
	{
		RefreshReceivedDamageRanking();
		return m_sortedReceivedDamages.FirstOrDefault();
	}

	public void ClearReceivedDamage()
	{
		m_receivedDamages.Clear();
	}

	public void AddAggro(Hero target, long lnValue)
	{
		if (target == null || lnValue <= 0 || !aggroManaged || m_bIsReturnMode || !m_currentPlace.ownershipManaged || !target.isLoggedIn || target.isDead || !IsInterestSector(target.sector) || (isExclusive && target.id != exclusiveHeroId))
		{
			return;
		}
		int nTargetNationId = target.nationId;
		if (nTargetNationId != nationId && !m_currentPlace.IsAlliance(nTargetNationId, nationId) && !target.isRidingCart)
		{
			Aggro aggro = GetAggro(target.id);
			if (aggro == null)
			{
				aggro = new Aggro(target);
				m_aggroes.Add(aggro.target.id, aggro);
				target.AddAggroMonster(this);
			}
			aggro.AddValue(lnValue);
			m_bIsMaxAggroInvalid = true;
		}
	}

	public Aggro GetAggro(Guid targetId)
	{
		if (!m_aggroes.TryGetValue(targetId, out var value))
		{
			return null;
		}
		return value;
	}

	public bool ContainsAggro(Guid targetId)
	{
		return m_aggroes.ContainsKey(targetId);
	}

	public bool RemoveAggro(Guid targetId)
	{
		if (!m_aggroes.Remove(targetId))
		{
			return false;
		}
		m_bIsMaxAggroInvalid = true;
		return true;
	}

	public void RefreshMaxAggro()
	{
		if (!m_bIsMaxAggroInvalid)
		{
			return;
		}
		Aggro maxAggro = null;
		foreach (Aggro aggro in m_aggroes.Values)
		{
			int nCompare = aggro.CompareTo(maxAggro);
			if (nCompare > 0)
			{
				maxAggro = aggro;
			}
			else if (nCompare == 0 && MathUtil.DistanceSqr(m_position, aggro.target.position) < MathUtil.DistanceSqr(m_position, maxAggro.target.position))
			{
				maxAggro = aggro;
			}
		}
		if (m_ownerType == MonsterOwnerType.Target)
		{
			Aggro oldTargetAggro = GetAggro(m_owner.id);
			if (oldTargetAggro != null && oldTargetAggro.value >= maxAggro.value)
			{
				maxAggro = oldTargetAggro;
			}
		}
		m_maxAggro = maxAggro;
		m_bIsMaxAggroInvalid = false;
	}

	public void ClearAggroes()
	{
		m_aggroes.Clear();
		m_bIsMaxAggroInvalid = true;
	}

	protected override void SendHitEvent(HitResult hitResult)
	{
		PDHitResult pdHitResult = hitResult.ToPDHitResult();
		ServerEvent.SendMonsterHit(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, pdHitResult);
	}

	protected override void SendAbnormalStateEffectStart(AbnormalStateEffect effect, int nDamageAbsorbShieldRemainingAbsorbAmount, IEnumerable<long> removedAbnormalStateEffects)
	{
		ServerEvent.SendMonsterAbnormalStateEffectStart(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, effect.id, effect.abnormalState.id, effect.sourceJobId, effect.abnormalStateLevel.level, effect.duration, nDamageAbsorbShieldRemainingAbsorbAmount, removedAbnormalStateEffects.ToArray());
	}

	protected override void SendAbnormalStateEffectHit(AbnormalStateEffect effect, IEnumerable<long> removedAbnormalStateEffects)
	{
		PDAttacker pdAttacker = m_lastAttacker.ToPDAttacker();
		ServerEvent.SendMonsterAbnormalStateEffectHit(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, m_nHP, effect.id, m_nLastDamage, m_nLastHPDamage, removedAbnormalStateEffects.ToArray(), pdAttacker);
	}

	protected override void SendAbnormalStateEffectFinishedEvent(AbnormalStateEffect effect)
	{
		ServerEvent.SendMonsterAbnormalStateEffectFinished(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, effect.id);
	}

	protected override void OnDead()
	{
		base.OnDead();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		m_currentPlace.AddWork(new SFAction<DateTimeOffset>(OnDeadAsync, currentTime), bGlobalLockRequired: true);
		foreach (MonsterReceivedDamage damage in m_receivedDamages.Values)
		{
			Guid attackerId = damage.attackerId;
			Hero attacker = m_currentPlace.GetHero(attackerId);
			if (attacker == null)
			{
				continue;
			}
			lock (attacker.syncObject)
			{
				if (IsQuestAreaPosition(attacker.position))
				{
					ProcessQuest(attacker, currentTime);
					m_currentPlace.AddWork(new SFAction<Hero, DateTimeOffset>(ProcessQuestForParty, attacker, currentTime), bGlobalLockRequired: true);
				}
			}
		}
	}

	protected virtual void OnDeadAsync(DateTimeOffset time)
	{
		MonsterReceivedDamage damage = GetMaxReceivedDamage();
		if (damage == null)
		{
			return;
		}
		Hero looter = m_currentPlace.GetHero(damage.attackerId);
		if (looter == null)
		{
			return;
		}
		lock (looter.syncObject)
		{
			if (!looter.isDead && IsQuestAreaPosition(looter.position))
			{
				if (normalItemLootingEnabled)
				{
					OnDeadAsync_ProcessDropItemLooting(looter);
				}
				OnDeadAsync_ProcessExpLooting(looter, time);
				OnDeadAsync_ProcessAccomplishment(looter, time);
			}
		}
	}

	protected void OnDeadAsync_ProcessDropItemLooting(Hero originalLooter)
	{
		List<DropObject> dropObjects = CreateDropObjects();
		if (dropObjects.Count == 0)
		{
			return;
		}
		List<Looter> looters = new List<Looter>();
		PartyMember partyMember = originalLooter.partyMember;
		if (partyMember != null)
		{
			foreach (PartyMember member in partyMember.party.members)
			{
				Hero looterPartyMember = m_currentPlace.GetHero(member.id);
				if (looterPartyMember == null)
				{
					continue;
				}
				lock (looterPartyMember.syncObject)
				{
					if (!looterPartyMember.isDead && IsQuestAreaPosition(looterPartyMember.position))
					{
						looters.Add(new Looter(looterPartyMember));
					}
				}
			}
		}
		else
		{
			looters.Add(new Looter(originalLooter));
		}
		foreach (DropObject dropObject in dropObjects)
		{
			Looter looter2 = SelectRandomLooter(looters);
			looter2.AddDropObject(dropObject);
		}
		foreach (Looter looter in looters)
		{
			if (looter.dropObjects.Count > 0)
			{
				Hero hero = looter.hero;
				lock (hero.syncObject)
				{
					ProcessHeroDropItemLooting(hero, looter.dropObjects);
				}
			}
		}
	}

	protected void ProcessHeroDropItemLooting(Hero looter, List<DropObject> dropObjects)
	{
		List<PDDropObject> pdLootedDropObjects = new List<PDDropObject>();
		List<PDDropObject> pdNotLootedDropObjects = new List<PDDropObject>();
		List<HeroMainGear> dropHeroMainGears = new List<HeroMainGear>();
		List<InventorySlot> changedInventorySlots = new List<InventorySlot>();
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		foreach (DropObject dropObject in dropObjects)
		{
			switch (dropObject.type)
			{
			case 1:
			{
				InventorySlot emptyInventorySlot = looter.GetEmptyInventorySlot();
				MainGearDropObject mainGearDropObject = (MainGearDropObject)dropObject;
				if (emptyInventorySlot != null && looter.IsAvailableLootItemGrade(mainGearDropObject.mainGear.grade.id))
				{
					HeroMainGear heroMainGear2 = new HeroMainGear(looter);
					heroMainGear2.Init(mainGearDropObject.mainGear, mainGearDropObject.enchantLevel, mainGearDropObject.owned, currentTime);
					looter.AddMainGear(heroMainGear2, bInit: false, currentTime);
					emptyInventorySlot.Place(heroMainGear2);
					changedInventorySlots.Add(emptyInventorySlot);
					dropHeroMainGears.Add(heroMainGear2);
					pdLootedDropObjects.Add(mainGearDropObject.ToPDDropObject());
				}
				else
				{
					pdNotLootedDropObjects.Add(mainGearDropObject.ToPDDropObject());
				}
				break;
			}
			case 2:
			{
				ItemDropObject itemDropObject = (ItemDropObject)dropObject;
				int nRemainingCount = 0;
				nRemainingCount = ((!looter.IsAvailableLootItemGrade(itemDropObject.item.grade)) ? itemDropObject.count : looter.AddItem(itemDropObject.item, itemDropObject.owned, itemDropObject.count, changedInventorySlots));
				if (nRemainingCount > 0)
				{
					PDItemDropObject pdNotLootedItemDropObject = (PDItemDropObject)itemDropObject.ToPDDropObject();
					pdNotLootedItemDropObject.count = nRemainingCount;
					pdNotLootedDropObjects.Add(pdNotLootedItemDropObject);
				}
				if (nRemainingCount != itemDropObject.count)
				{
					PDItemDropObject pdLootedItemDropObject = (PDItemDropObject)itemDropObject.ToPDDropObject();
					pdLootedItemDropObject.count -= nRemainingCount;
					pdLootedDropObjects.Add(pdLootedItemDropObject);
				}
				break;
			}
			}
		}
		if (pdLootedDropObjects.Count > 0)
		{
			SFSqlQueuingWork dbWork = SqlQueuingWorkUtil.CreateHeroWork(looter.id);
			foreach (HeroMainGear heroMainGear in dropHeroMainGears)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddHeroMainGear(heroMainGear));
			}
			foreach (InventorySlot slot in changedInventorySlots)
			{
				dbWork.AddSqlCommand(GameDacEx.CSC_AddOrUpdateInventorySlot(slot));
			}
			dbWork.Schedule();
		}
		ServerEvent.SendDropObjectLooted(looter.account.peer, HeroMainGear.ToPDFullHeroMainGears(dropHeroMainGears).ToArray(), looter.maxAcquisitionMainGearGrade, InventorySlot.ToPDInventorySlots(changedInventorySlots).ToArray(), pdLootedDropObjects.ToArray(), pdNotLootedDropObjects.ToArray());
	}

	private void OnDeadAsync_ProcessExpLooting(Hero originalLooter, DateTimeOffset time)
	{
		long lnExp = monster.exp;
		if (lnExp <= 0)
		{
			return;
		}
		Resource res = Resource.instance;
		if (m_currentPlace.isPartyExpBuffEnabled && originalLooter.partyMember != null)
		{
			Party party = originalLooter.partyMember.party;
			List<Hero> looters = new List<Hero>();
			foreach (PartyMember member in party.members)
			{
				if (member.id == originalLooter.id)
				{
					continue;
				}
				Hero looterPartyMember = m_currentPlace.GetHero(member.id);
				if (looterPartyMember == null)
				{
					continue;
				}
				lock (looterPartyMember.syncObject)
				{
					if (!looterPartyMember.isDead && IsQuestAreaPosition(looterPartyMember.position))
					{
						looters.Add(looterPartyMember);
					}
				}
			}
			lnExp = (long)Math.Floor((float)lnExp * res.GetPartyExpFactorValue(looters.Count + 1));
			foreach (Hero looter in looters)
			{
				lock (looter.syncObject)
				{
					OnDeadAsync_ProcessExpLootingPartyMemberInternal(looter, lnExp, time);
				}
			}
		}
		if (m_currentPlace.isExpLevelPenaltyEnabled)
		{
			lnExp = (long)Math.Floor((float)lnExp * res.GetMonsterKillExpFactorValue(originalLooter.level - level));
		}
		if (m_currentPlace.isExpScrollBuffEnabled && originalLooter.IsExpScrollActiveTime(time))
		{
			lnExp = (long)Math.Floor((float)(lnExp * originalLooter.expScrollItem.value2) / 10000f);
		}
		if (m_currentPlace.isWorldLevelExpBuffEnabled)
		{
			lnExp = (long)Math.Floor((float)lnExp * Cache.instance.GetWorldLevelExpFactor(originalLooter.level));
		}
		if (m_currentPlace.placeType == PlaceType.ExpDungeon)
		{
			VipLevel vipLevel = originalLooter.vipLevel;
			if (vipLevel != null)
			{
				lnExp = (long)Math.Floor((float)lnExp * vipLevel.expDungeonAdditionalExpRewardFactor);
			}
		}
		originalLooter.AddExp(lnExp, bSendExpAcquisitionEvent: true, bSaveToDBForLevelUp: true);
	}

	private void OnDeadAsync_ProcessExpLootingPartyMemberInternal(Hero looter, long lnExp, DateTimeOffset time)
	{
		long lnAcquisitionExp = lnExp;
		Resource res = Resource.instance;
		if (m_currentPlace.isExpLevelPenaltyEnabled)
		{
			lnAcquisitionExp = (long)Math.Floor((float)lnAcquisitionExp * res.GetMonsterKillExpFactorValue(looter.level - level));
		}
		if (m_currentPlace.isExpScrollBuffEnabled && looter.IsExpScrollActiveTime(time))
		{
			lnAcquisitionExp = (long)Math.Floor((float)(lnAcquisitionExp * looter.expScrollItem.value2) / 10000f);
		}
		if (m_currentPlace.isWorldLevelExpBuffEnabled)
		{
			lnAcquisitionExp = (long)Math.Floor((float)lnAcquisitionExp * Cache.instance.GetWorldLevelExpFactor(looter.level));
		}
		looter.AddExp(lnAcquisitionExp, bSendExpAcquisitionEvent: true, bSaveToDBForLevelUp: true);
	}

	private void OnDeadAsync_ProcessAccomplishment(Hero looter, DateTimeOffset time)
	{
		looter.ProcessAccomplishment_MonsterKill();
		if (looter.partyMember == null)
		{
			return;
		}
		Party party = looter.partyMember.party;
		foreach (PartyMember member in party.members)
		{
			if (member.id == looter.id)
			{
				continue;
			}
			Hero looterPartyMember = m_currentPlace.GetHero(member.id);
			if (looterPartyMember == null)
			{
				continue;
			}
			lock (looterPartyMember.syncObject)
			{
				if (!looterPartyMember.isDead && IsQuestAreaPosition(looterPartyMember.position))
				{
					looterPartyMember.ProcessAccomplishment_MonsterKill();
				}
			}
		}
	}

	protected virtual void ProcessQuest(Hero attacker, DateTimeOffset currentTime)
	{
		attacker.ProcessMainQuestForHunt(this);
		attacker.ProcessMainQuestForAcquisition(this);
		attacker.ProcessDailyQuestForHunt(this, currentTime);
		attacker.ProcessWeeklyQuestRoundForHunt(this, currentTime);
		attacker.ProcessSubQuestForHunt(this);
		attacker.ProcessSubQuestForAcquisition(this);
		attacker.ProcessOrdealQuestMissionsForHunt(this, currentTime);
		attacker.ProcessBiographyQuestsForHunt(this);
	}

	private void ProcessQuestForParty(Hero hero, DateTimeOffset currentTime)
	{
		lock (hero.syncObject)
		{
			if (!hero.isLoggedIn)
			{
				return;
			}
			PartyMember partyMember = hero.partyMember;
			if (partyMember == null)
			{
				return;
			}
			foreach (PartyMember member in partyMember.party.members)
			{
				Hero partyMemberHero = m_currentPlace.GetHero(member.id);
				if (partyMemberHero == null || partyMemberHero.id == hero.id)
				{
					continue;
				}
				lock (partyMemberHero.syncObject)
				{
					if (IsQuestAreaPosition(partyMemberHero.position))
					{
						ProcessQuestForParty_Member(partyMemberHero, currentTime);
					}
				}
			}
		}
	}

	protected virtual void ProcessQuestForParty_Member(Hero member, DateTimeOffset currentTime)
	{
		member.ProcessMainQuestForHunt(this);
		member.ProcessMainQuestForAcquisition(this);
		member.ProcessDailyQuestForHunt(this, currentTime);
		member.ProcessWeeklyQuestRoundForHunt(this, currentTime);
		member.ProcessSubQuestForHunt(this);
		member.ProcessSubQuestForAcquisition(this);
		member.ProcessOrdealQuestMissionsForHunt(this, currentTime);
		member.ProcessBiographyQuestsForHunt(this);
	}

	public override void OnHPRestored(bool bSendEventToMyself, bool bSendEventToOthers)
	{
		base.OnHPRestored(bSendEventToMyself, bSendEventToOthers);
		if (bSendEventToOthers)
		{
			ServerEvent.SendMonsterHpRestored(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, m_nHP);
		}
	}

	protected List<DropObject> CreateDropObjects()
	{
		List<DropObject> dropObjects = new List<DropObject>();
		DropObjectPool dropObjectPool = monster.dropObjectPool;
		if (dropObjectPool == null)
		{
			return dropObjects;
		}
		foreach (DropObjectPoolEntry entry2 in dropObjectPool.fixedEntries)
		{
			DropObject dropObject2 = DropObject.Create(entry2);
			if (dropObject2 != null)
			{
				dropObject2.source = this;
				dropObjects.Add(dropObject2);
			}
		}
		DropCountPool dropCountPool = monster.dropCountPool;
		if (dropCountPool == null)
		{
			return dropObjects;
		}
		for (int nSelectCount = dropCountPool.SelectDropCount(); nSelectCount > 0; nSelectCount--)
		{
			DropObjectPoolEntry entry = dropObjectPool.SelectEntry();
			if (entry != null)
			{
				DropObject dropObject = DropObject.Create(entry);
				if (dropObject != null)
				{
					dropObject.source = this;
					dropObjects.Add(dropObject);
				}
			}
		}
		return dropObjects;
	}

	private Looter SelectRandomLooter(List<Looter> looters)
	{
		int nLootersCount = looters.Count;
		if (nLootersCount == 1)
		{
			return looters.FirstOrDefault();
		}
		return looters[SFRandom.Next(nLootersCount)];
	}

	public bool IsQuestAreaPosition(Vector3 position)
	{
		return MathUtil.CircleContains(m_position, monster.questAreaRadius, position);
	}

	public void UpdateReturnMode()
	{
		if (m_bIsReturnMode)
		{
			if (!MathUtil.CircleContains(m_spawnedPosition, monster.returnCompletionRadius, m_position))
			{
				return;
			}
			m_bIsReturnMode = false;
		}
		else
		{
			if (MathUtil.CircleContains(m_spawnedPosition, monster.activeAreaRadius, m_position))
			{
				return;
			}
			m_bIsReturnMode = true;
			if (m_owner != null)
			{
				SetOwnership(m_owner, MonsterOwnerType.Controller);
			}
			foreach (Aggro aggro in m_aggroes.Values)
			{
				Hero target = aggro.target;
				lock (target.syncObject)
				{
					target.RemoveAggroMonster(this);
				}
			}
			ClearAggroes();
			ClearReceivedDamage();
		}
		ServerEvent.SendMonsterReturnModeChanged(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, m_bIsReturnMode);
	}

	protected override void SendMaxHPChangedEventToOthers()
	{
	}

	public void HitMentalStrength(Hero attacker, int nDamage, int nSkillId)
	{
		if (!hitEnabled)
		{
			return;
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		List<long> removedAbnormalStateEffects = new List<long>();
		int nMentalStrengthDamage = ((nDamage <= m_nMentalStrength) ? nDamage : m_nMentalStrength);
		m_nMentalStrength -= nMentalStrengthDamage;
		AddReceivedMentalDamage(attacker, nDamage, currentTime);
		AddReceivedDamage(attacker, nDamage, currentTime);
		if (isGroggy)
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
			OnGroggy();
		}
		ServerEvent.SendMonsterMentalHit(m_currentPlace.GetDynamicClientPeers(m_sector, Guid.Empty), m_lnInstanceId, attacker.id, targetTamerId, nSkillId, m_nMentalStrength, nDamage, nMentalStrengthDamage, removedAbnormalStateEffects.ToArray());
	}

	protected void OnGroggy()
	{
		MonsterReceivedMentalDamage damage = GetMaxReceivedMentalDamage();
		if (damage != null)
		{
			m_targetTamer = m_currentPlace.GetHero(damage.attackerId);
		}
		int nDelayTime = Resource.instance.monsterGroggyDuration * 1000;
		m_groggyTimer = new Timer(OnGroggyTimerTick);
		m_groggyTimer.Change(nDelayTime, -1);
	}

	private void OnGroggyTimerTick(object state)
	{
		AddWork(new SFAction<MonsterInstance, bool>(m_currentPlace.RemoveMonster, this, arg2: true), bGlobalLockRequired: false);
	}

	private void DisposeGroggyTimer()
	{
		if (m_groggyTimer != null)
		{
			m_groggyTimer.Dispose();
			m_groggyTimer = null;
		}
	}

	private void AddReceivedMentalDamage(Hero attacker, int nDamage, DateTimeOffset time)
	{
		MonsterReceivedMentalDamage damage = GetOrCreateReceivedMentalDamage(attacker.id, attacker.name);
		damage.AddDamage(nDamage, time);
		m_bIsReceivedMentalDamageRankingInvalid = true;
	}

	public MonsterReceivedMentalDamage GetReceivedMentalDamage(Guid attackerId)
	{
		if (!m_receivedMentalDamages.TryGetValue(attackerId, out var value))
		{
			return null;
		}
		return value;
	}

	public MonsterReceivedMentalDamage GetOrCreateReceivedMentalDamage(Guid attackerId, string sAttackeName)
	{
		MonsterReceivedMentalDamage damage = GetReceivedMentalDamage(attackerId);
		if (damage == null)
		{
			damage = new MonsterReceivedMentalDamage(attackerId, sAttackeName);
			m_receivedMentalDamages.Add(damage.attackerId, damage);
		}
		return damage;
	}

	private void RefreshReceivedMentalDamageRanking()
	{
		if (m_bIsReceivedMentalDamageRankingInvalid)
		{
			MonsterReceivedMentalDamage[] damages = m_receivedMentalDamages.Values.ToArray();
			Array.Sort(damages, MonsterReceivedMentalDamage.Compare);
			Array.Reverse(damages);
			for (int i = 0; i < damages.Length; i++)
			{
				damages[i].rank = i + 1;
			}
			m_sortedReceivedMentalDamages = damages;
			m_bIsReceivedMentalDamageRankingInvalid = false;
		}
	}

	protected MonsterReceivedMentalDamage GetMaxReceivedMentalDamage()
	{
		RefreshReceivedMentalDamageRanking();
		return m_sortedReceivedMentalDamages.FirstOrDefault();
	}

	public void ClearReceivedMentalDamage()
	{
		m_receivedMentalDamages.Clear();
	}

	public bool IsStealEnabledPosition(Vector3 position)
	{
		return MathUtil.CircleContains(m_position, monster.stealRadius * 1.1f, position);
	}

	protected override void ReleaseInternal()
	{
		DisposeGroggyTimer();
		base.ReleaseInternal();
	}

	protected abstract PDMonsterInstance CreatePDMonsterInstance();

	public virtual PDMonsterInstance ToPDMonsterInstance(DateTimeOffset currentTime)
	{
		PDMonsterInstance inst = CreatePDMonsterInstance();
		inst.instanceId = m_lnInstanceId;
		inst.monsterId = monster.id;
		inst.maxHP = m_nRealMaxHP;
		inst.hp = m_nHP;
		inst.maxMentalStrength = m_nMaxMentalStrength;
		inst.mentalStrength = m_nMentalStrength;
		List<PDAbnormalStateEffect> abnormalStateEffects = new List<PDAbnormalStateEffect>();
		foreach (AbnormalStateEffect effect in m_abnormalStateEffects.Values)
		{
			abnormalStateEffects.Add(effect.ToPDAbnormalStateEffect(currentTime));
		}
		inst.abnormalStateEffects = abnormalStateEffects.ToArray();
		inst.spawnedPosition = m_spawnedPosition;
		inst.spawnedRotationY = m_fSpawnedRotationY;
		inst.position = m_position;
		inst.rotationY = m_fRotationY;
		inst.ownerId = (Guid)ownerId;
		inst.ownerType = m_ownerType;
		inst.isExclusive = isExclusive;
		inst.exclusiveHeroId = (Guid)exclusiveHeroId;
		inst.exclusiveHeroName = exclusiveHeroName;
		inst.nationId = nationId;
		inst.isReturnMode = m_bIsReturnMode;
		inst.tamerId = (Guid)targetTamerId;
		return inst;
	}

	public override PDAttacker ToPDAttacker()
	{
		PDMonsterAttacker inst = new PDMonsterAttacker();
		inst.monsterInstanceId = m_lnInstanceId;
		inst.monsterId = monster.id;
		return inst;
	}
}
