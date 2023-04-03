using System;
using System.Collections.Generic;
using System.Linq;
using ClientCommon;

namespace GameServer;

public class HitResult
{
	private OffenseHit m_offenseHit;

	private DateTimeOffset m_time = DateTimeOffset.MinValue;

	private bool m_bIsCritical;

	private bool m_bIsPenetration;

	private bool m_bIsBlocked;

	private bool m_bIsImmortal;

	private int m_nHP;

	private int m_nDamage;

	private int m_nHPDamage;

	private List<PDAbnormalStateEffectDamageAbsorbShield> m_changedAbnormalStateEffectDamageAbsorbShields = new List<PDAbnormalStateEffectDamageAbsorbShield>();

	private int m_nMoveSpeed;

	private HashSet<long> m_removedAbnormalStateEffects = new HashSet<long>();

	public OffenseHit offenseHit
	{
		get
		{
			return m_offenseHit;
		}
		set
		{
			m_offenseHit = value;
		}
	}

	public DateTimeOffset time
	{
		get
		{
			return m_time;
		}
		set
		{
			m_time = value;
		}
	}

	public bool isCritical
	{
		get
		{
			return m_bIsCritical;
		}
		set
		{
			m_bIsCritical = value;
		}
	}

	public bool isPenetration
	{
		get
		{
			return m_bIsPenetration;
		}
		set
		{
			m_bIsPenetration = value;
		}
	}

	public bool isBlocked
	{
		get
		{
			return m_bIsBlocked;
		}
		set
		{
			m_bIsBlocked = value;
		}
	}

	public bool isImmortal
	{
		get
		{
			return m_bIsImmortal;
		}
		set
		{
			m_bIsImmortal = value;
		}
	}

	public int hp
	{
		get
		{
			return m_nHP;
		}
		set
		{
			m_nHP = value;
		}
	}

	public int damage
	{
		get
		{
			return m_nDamage;
		}
		set
		{
			m_nDamage = value;
		}
	}

	public int hpDamage
	{
		get
		{
			return m_nHPDamage;
		}
		set
		{
			m_nHPDamage = value;
		}
	}

	public List<PDAbnormalStateEffectDamageAbsorbShield> changedAbnormalStateEffectDamageAbsorbShields => m_changedAbnormalStateEffectDamageAbsorbShields;

	public int moveSpeed
	{
		get
		{
			return m_nMoveSpeed;
		}
		set
		{
			m_nMoveSpeed = value;
		}
	}

	public void AddRemovedAbnormalStateEffect(long lnInstanceId)
	{
		m_removedAbnormalStateEffects.Add(lnInstanceId);
	}

	public void AddRemovedAbnormalStateEffects(IEnumerable<long> instanceIds)
	{
		foreach (long lnInstanceId in instanceIds)
		{
			AddRemovedAbnormalStateEffect(lnInstanceId);
		}
	}

	public PDHitResult ToPDHitResult()
	{
		Offense offense = m_offenseHit.offense;
		PDHitResult inst = new PDHitResult();
		Skill skill = offense.skill;
		inst.attacker = offense.attacker.ToPDAttacker();
		inst.skillId = skill.skillId;
		inst.chainSkillId = skill.chainSkillId;
		inst.hitId = m_offenseHit.hitId;
		inst.isCritical = m_bIsCritical;
		inst.isPenetration = m_bIsPenetration;
		inst.isBlocked = m_bIsBlocked;
		inst.isImmortal = m_bIsImmortal;
		inst.hp = m_nHP;
		inst.damage = m_nDamage;
		inst.hpDamage = m_nHPDamage;
		inst.changedAbnormalStateEffectDamageAbsorbShields = m_changedAbnormalStateEffectDamageAbsorbShields.ToArray();
		inst.removedAbnormalStateEffects = m_removedAbnormalStateEffects.ToArray();
		return inst;
	}
}
