using System;
using System.Collections.Generic;
using System.Data;

namespace GameServer;

public class JobChainSkill : Skill
{
	private JobSkill m_skill;

	private int m_nChainSkillId;

	private SkillHitAreaType m_hitAreaType;

	private float m_fHitAreaValue1;

	private float m_fHitAreaValue2;

	private SkillHitAreaOffsetType m_hitAreaOffsetType;

	private float m_fHitAreaOffset;

	private float m_fHitValidationRadius;

	private List<JobChainSkillHit> m_hits = new List<JobChainSkillHit>();

	public override int type => 2;

	public JobSkill skill
	{
		get
		{
			return m_skill;
		}
		set
		{
			m_skill = value;
		}
	}

	public override int skillId => m_skill.skillId;

	public override int chainSkillId => m_nChainSkillId;

	public override SkillHitAreaType hitAreaType => m_hitAreaType;

	public override float hitAreaValue1 => m_fHitAreaValue1;

	public override float hitAreaValue2 => m_fHitAreaValue2;

	public override SkillHitAreaOffsetType hitAreaOffsetType => m_hitAreaOffsetType;

	public override float hitAreaOffset => m_fHitAreaOffset;

	public override float hitValidationRadius => m_fHitValidationRadius;

	public override float ssStartDelay => 0f;

	public override float ssDuration => 0f;

	public override int hitCount => m_hits.Count;

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nChainSkillId = Convert.ToInt32(dr["chainSkillId"]);
		m_hitAreaType = (SkillHitAreaType)Convert.ToInt32(dr["hitAreaType"]);
		m_fHitAreaValue1 = Convert.ToSingle(dr["hitAreaValue1"]);
		m_fHitAreaValue2 = Convert.ToSingle(dr["hitAreaValue2"]);
		m_hitAreaOffsetType = (SkillHitAreaOffsetType)Convert.ToInt32(dr["hitAreaOffsetType"]);
		m_fHitAreaOffset = Convert.ToSingle(dr["hitAreaOffset"]);
	}

	public void SetHitValidationRadius()
	{
		float fOffset = 0f;
		if (m_hitAreaOffsetType == SkillHitAreaOffsetType.Fixed)
		{
			fOffset = m_fHitAreaOffset;
		}
		else if (m_hitAreaOffsetType == SkillHitAreaOffsetType.Dynamic)
		{
			fOffset = m_skill.hitRange;
		}
		if (m_hitAreaType == SkillHitAreaType.Circle)
		{
			m_fHitValidationRadius = Math.Abs(fOffset) + m_fHitAreaValue1;
		}
		else if (m_hitAreaType == SkillHitAreaType.Square)
		{
			m_fHitValidationRadius = Math.Max(m_fHitAreaValue1 / 0.5f, Math.Abs(fOffset) + m_fHitAreaValue2 / 0.5f);
		}
	}

	public void AddJobChainSkillHit(JobChainSkillHit hit)
	{
		if (hit == null)
		{
			throw new ArgumentNullException("hit");
		}
		if (hit.jobChainSkill != null)
		{
			throw new Exception("이미 스킬에 추가된 적중 입니다.");
		}
		m_hits.Add(hit);
		hit.jobChainSkill = this;
	}

	public JobChainSkillHit GetJobChainSkillHit(int nHitId)
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
		return GetJobChainSkillHit(nHitId);
	}
}
