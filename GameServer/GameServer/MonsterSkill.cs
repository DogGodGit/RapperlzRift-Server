using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class MonsterSkill : Skill
{
	public const int kFormType_NormalSkill = 1;

	public const int kFormType_BuffSkill = 2;

	public const float kFanRightAngle = 90f;

	private int m_nId;

	private OffenseType m_baseDamageType;

	private int m_nSkillType;

	private int m_nElementalId;

	private int m_nFormType;

	private bool m_bIsRequiredTarget;

	private float m_fCastRange;

	private float m_fHitRange;

	private float m_fCoolTime;

	private SkillHitAreaType m_hitAreaType;

	private float m_fHitAreaValue1;

	private float m_fHitAreaValue2;

	private SkillHitAreaOffsetType m_hitAreaOffsetType;

	private float m_fHitAreaOffset;

	private float m_fSSStartDelay;

	private float m_fSSDuration;

	private int m_nAutoPriorityGroup;

	private int m_nAutoWeight;

	private List<MonsterSkillHit> m_hits = new List<MonsterSkillHit>();

	public override int type => 3;

	public override int skillId => m_nId;

	public override int chainSkillId => 0;

	public OffenseType baseDamageType => m_baseDamageType;

	public int skillType => m_nSkillType;

	public int elementalId => m_nElementalId;

	public int formType => m_nFormType;

	public bool isRequiredTarget => m_bIsRequiredTarget;

	public float castRange => m_fCastRange;

	public float hitRange => m_fHitRange;

	public float coolTime => m_fCoolTime;

	public override SkillHitAreaType hitAreaType => m_hitAreaType;

	public override float hitAreaValue1 => m_fHitAreaValue1;

	public override float hitAreaValue2 => m_fHitAreaValue2;

	public override SkillHitAreaOffsetType hitAreaOffsetType => m_hitAreaOffsetType;

	public override float hitAreaOffset => m_fHitAreaOffset;

	public override float hitValidationRadius => 0f;

	public override float ssStartDelay => m_fSSStartDelay;

	public override float ssDuration => m_fSSDuration;

	public int autoPriorityGroup => m_nAutoPriorityGroup;

	public int autoWeight => m_nAutoWeight;

	public List<MonsterSkillHit> hits => m_hits;

	public override int hitCount => m_hits.Count;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["skillId"]);
		m_baseDamageType = (OffenseType)Convert.ToInt32(dr["baseDamageType"]);
		m_nSkillType = Convert.ToInt32(dr["type"]);
		m_nElementalId = Convert.ToInt32(dr["elementalId"]);
		m_nFormType = Convert.ToInt32(dr["formType"]);
		m_bIsRequiredTarget = Convert.ToBoolean(dr["isRequiredTarget"]);
		m_fCastRange = Convert.ToSingle(dr["castRange"]);
		m_fHitRange = Convert.ToSingle(dr["hitRange"]);
		m_fCoolTime = Convert.ToSingle(dr["coolTime"]);
		m_hitAreaType = (SkillHitAreaType)Convert.ToInt32(dr["hitAreaType"]);
		m_fHitAreaValue1 = Convert.ToSingle(dr["hitAreaValue1"]);
		m_fHitAreaValue2 = Convert.ToSingle(dr["hitAreaValue2"]);
		m_hitAreaOffsetType = (SkillHitAreaOffsetType)Convert.ToSingle(dr["hitAreaOffsetType"]);
		m_fHitAreaOffset = Convert.ToSingle(dr["hitAreaOffset"]);
		m_fSSStartDelay = Convert.ToSingle(dr["ssStartDelay"]);
		m_fSSDuration = Convert.ToSingle(dr["ssDuration"]);
		m_nAutoPriorityGroup = Convert.ToInt32(dr["autoPriorityGroup"]);
		m_nAutoWeight = Convert.ToInt32(dr["autoWeight"]);
	}

	public bool ValidationAreaContains(Vector3 skillTargetPosition, Vector3 castPosition, float fRotationY, Vector3 targetPosition)
	{
		Vector3 revisionTargetPosition = Vector3.zero;
		if (m_hitAreaOffsetType == SkillHitAreaOffsetType.Fixed)
		{
			revisionTargetPosition = MathUtil.PositionRotation(targetPosition - castPosition, fRotationY);
			revisionTargetPosition.z -= m_fHitAreaOffset;
			if (m_hitAreaType == SkillHitAreaType.Circle)
			{
				if (MathUtil.FanContainsD(Vector3.zero, m_fHitAreaValue1, m_fHitAreaValue2, 90f, revisionTargetPosition))
				{
					return true;
				}
			}
			else if (m_hitAreaType == SkillHitAreaType.Square && MathUtil.SquareContains(Vector3.zero, m_fHitAreaValue1, m_fHitAreaValue2, revisionTargetPosition))
			{
				return true;
			}
		}
		else if (m_hitAreaOffsetType == SkillHitAreaOffsetType.Dynamic)
		{
			revisionTargetPosition = MathUtil.PositionRotation(targetPosition - skillTargetPosition, fRotationY);
			if (m_hitAreaType == SkillHitAreaType.Circle)
			{
				if (MathUtil.FanContainsD(Vector3.zero, m_fHitAreaValue1, m_fHitAreaValue2, 90f, revisionTargetPosition))
				{
					return true;
				}
			}
			else if (m_hitAreaType == SkillHitAreaType.Square && MathUtil.SquareContains(Vector3.zero, m_fHitAreaValue1, m_fHitAreaValue2, revisionTargetPosition))
			{
				return true;
			}
		}
		return false;
	}

	public void AddMonsterSkillHit(MonsterSkillHit hit)
	{
		if (hit == null)
		{
			throw new ArgumentNullException("hit");
		}
		if (hit.monsterSkill != null)
		{
			throw new Exception("이미 몬스터스킬에 추가된 몬스터스킬적중 입니다.");
		}
		m_hits.Add(hit);
		hit.monsterSkill = this;
	}

	public MonsterSkillHit GetMonsterSkillHit(int nHitId)
	{
		int nIndex = nHitId - 1;
		if (nIndex < 0 || nIndex >= hitCount)
		{
			return null;
		}
		return m_hits[nIndex];
	}

	public override SkillHit GetHit(int nHitId)
	{
		return GetMonsterSkillHit(nHitId);
	}
}
