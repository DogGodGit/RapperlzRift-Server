using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClientCommon;
using ServerFramework;

namespace GameServer;

public class RuinsReclaimMonsterInstance : MonsterInstance
{
	private RuinsReclaimMonsterArrange m_arrange;

	private RuinsReclaimMonsterAttrFactor m_attrFactor;

	private Dictionary<Guid, MonsterReceivedDamage> m_monsterReceivedDamages = new Dictionary<Guid, MonsterReceivedDamage>();

	private bool m_bSummonMonster;

	private float m_fHPRecoveryFactor;

	private Timer m_hpRecoveryTimer;

	public override MonsterInstanceType monsterInstanceType => MonsterInstanceType.RuinsReclaimMonster;

	public override Monster monster => m_arrange.monsterArrange.monster;

	public RuinsReclaimMonsterArrange arrange => m_arrange;

	public float hpRecoveryFactor
	{
		get
		{
			return m_fHPRecoveryFactor;
		}
		set
		{
			m_fHPRecoveryFactor = value;
		}
	}

	public void Init(RuinsReclaimInstance ruinsReclaimInst, RuinsReclaimMonsterArrange arrange, RuinsReclaimMonsterAttrFactor attrFactor)
	{
		if (ruinsReclaimInst == null)
		{
			throw new ArgumentNullException("ruinsReclaimInst");
		}
		if (arrange == null)
		{
			throw new ArgumentNullException("arrange");
		}
		if (attrFactor == null)
		{
			throw new ArgumentNullException("attrFactor");
		}
		m_arrange = arrange;
		m_attrFactor = attrFactor;
		InitMonsterInstance(ruinsReclaimInst, arrange.SelectPosition(), arrange.SelectRotationY());
	}

	protected override void RefreshRealValues_Multiplication()
	{
		base.RefreshRealValues_Multiplication();
		m_nRealMaxHP = (int)Math.Floor((float)m_nRealMaxHP * m_attrFactor.offenseFactor);
		m_nRealPhysicalOffense = (int)Math.Floor((float)m_nRealPhysicalOffense * m_attrFactor.offenseFactor);
	}

	protected override void OnDamage()
	{
		base.OnDamage();
		RuinsReclaimInstance ruinsReclaimInst = (RuinsReclaimInstance)m_currentPlace;
		if (ruinsReclaimInst.isFinished)
		{
			return;
		}
		if (m_arrange.type == 2 && m_arrange.key == m_arrange.ruinsReclaim.lastBossArrangeKey)
		{
			Hero attacker = (Hero)m_lastAttacker;
			Hero heroAttacker = m_currentPlace.GetHero(attacker.id);
			if (heroAttacker != null)
			{
				MonsterReceivedDamage damage = GetOrCreateMonsterReceivedDamage(heroAttacker.id, heroAttacker.name);
				damage.AddDamage(m_nLastHPDamage, m_lastDamageTime);
			}
		}
		float fSummonMinHpFactor = m_arrange.summonMinHpFactor;
		if (fSummonMinHpFactor > 0f && !m_bSummonMonster && !base.isDead)
		{
			int nSummonHp = (int)Math.Floor((float)m_nRealMaxHP * fSummonMinHpFactor);
			if (m_nHP < nSummonHp)
			{
				m_bSummonMonster = true;
				ruinsReclaimInst.SummonMonster(this);
				StartHPRecoveryTimer();
			}
		}
	}

	private void StartHPRecoveryTimer()
	{
		int nInterval = m_arrange.ruinsReclaim.summonMonsterHpRecoveryInterval * 1000;
		m_hpRecoveryTimer = new Timer(OnHPRecoveryTimerTick);
		m_hpRecoveryTimer.Change(nInterval, nInterval);
	}

	private void OnHPRecoveryTimerTick(object state)
	{
		m_currentPlace.AddWork(new SFAction(RecoveryHP), bGlobalLockRequired: false);
	}

	private void RecoveryHP()
	{
		if (!m_bReleased && !(m_fHPRecoveryFactor <= 0f))
		{
			int nAmount = (int)Math.Floor((float)m_nRealMaxHP * m_fHPRecoveryFactor);
			RestoreHP(nAmount, bSendEventToMyself: true, bSendEventToOthers: true);
		}
	}

	public MonsterReceivedDamage GetMonsterReceivedDamage(Guid attackerId)
	{
		if (!m_monsterReceivedDamages.TryGetValue(attackerId, out var value))
		{
			return null;
		}
		return value;
	}

	private MonsterReceivedDamage GetOrCreateMonsterReceivedDamage(Guid attackerId, string sAttackeName)
	{
		MonsterReceivedDamage damage = GetMonsterReceivedDamage(attackerId);
		if (damage == null)
		{
			damage = new MonsterReceivedDamage(attackerId, sAttackeName);
			m_monsterReceivedDamages.Add(damage.attackerId, damage);
		}
		return damage;
	}

	public MonsterReceivedDamage GetMaxMonsterReceivedDamage()
	{
		_ = (RuinsReclaimInstance)m_currentPlace;
		MonsterReceivedDamage[] damages = m_monsterReceivedDamages.Values.ToArray();
		Array.Sort(damages, MonsterReceivedDamage.Compare);
		Array.Reverse(damages);
		for (int i = 0; i < damages.Length; i++)
		{
			damages[i].rank = i + 1;
		}
		return damages.FirstOrDefault();
	}

	public void RemoveMonsterReceivedDamage(Guid heroId)
	{
		m_monsterReceivedDamages.Remove(heroId);
	}

	private void DisposeHPRecoveryTimer()
	{
		if (m_hpRecoveryTimer != null)
		{
			m_hpRecoveryTimer.Dispose();
			m_hpRecoveryTimer = null;
		}
	}

	protected override void ReleaseInternal()
	{
		DisposeHPRecoveryTimer();
		base.ReleaseInternal();
	}

	protected override PDMonsterInstance CreatePDMonsterInstance()
	{
		PDRuinsReclaimMonsterInstance inst = new PDRuinsReclaimMonsterInstance();
		inst.monsterType = m_arrange.type;
		return inst;
	}
}
