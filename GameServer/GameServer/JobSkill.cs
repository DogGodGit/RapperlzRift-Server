using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ServerFramework;

namespace GameServer;

public class JobSkill : Skill
{
	public const int kFormType_ChainSkill = 1;

	public const int kFormType_NormalSkill = 2;

	public const int kFormType_BuffSkill = 3;

	public const int kHeroHitType_None = 0;

	public const int kHeroHitType_Single = 1;

	public const int kHeroHitType_Multi = 2;

	public const int kCastingMoveType_None = 0;

	public const int kCastingMoveType_Fixed = 1;

	public const int kCastingMoveType_Control = 2;

	private Job m_job;

	private JobSkillMaster m_skillMaster;

	private string m_sNameKey;

	private string m_sDescriptionKey;

	private int m_nSkillType;

	private int m_nFormType;

	private bool m_bIsRequireTarget;

	private float m_fCastRange;

	private float m_fHitRange;

	private float m_fCoolTime;

	private int m_nHeroHitType;

	private SkillHitAreaType m_hitAreaType;

	private float m_fHitAreaValue1;

	private float m_fHitAreaValue2;

	private SkillHitAreaOffsetType m_hitAreaOffsetType;

	private float m_fHitAreaOffset;

	private float m_fHitValidationRadius;

	private float m_fSSStartDelay;

	private float m_fSSDuration;

	private int m_nCastingMoveType;

	private int m_nCastingMoveValue1;

	private int m_nCastingMoveValue2;

	private AbnormalState m_buffAbnormalState;

	private List<JobSkillLevel> m_levels = new List<JobSkillLevel>();

	private List<JobSkillHit> m_hits = new List<JobSkillHit>();

	private List<JobChainSkill> m_chainSkills = new List<JobChainSkill>();

	public override int type => 1;

	public Job job
	{
		get
		{
			return m_job;
		}
		set
		{
			m_job = value;
		}
	}

	public JobSkillMaster skillMaster => m_skillMaster;

	public override int skillId => m_skillMaster.skillId;

	public override int chainSkillId => 0;

	public string nameKey => m_sNameKey;

	public string descriptionKey => m_sDescriptionKey;

	public int skillType => m_nSkillType;

	public int formType => m_nFormType;

	public bool isRequireTarget => m_bIsRequireTarget;

	public float castRange => m_fCastRange;

	public float hitRange => m_fHitRange;

	public float coolTime => m_fCoolTime;

	public int heroHitType => m_nHeroHitType;

	public override SkillHitAreaType hitAreaType => m_hitAreaType;

	public override float hitAreaValue1 => m_fHitAreaValue1;

	public override float hitAreaValue2 => m_fHitAreaValue2;

	public override SkillHitAreaOffsetType hitAreaOffsetType => m_hitAreaOffsetType;

	public override float hitAreaOffset => m_fHitAreaOffset;

	public override float hitValidationRadius => m_fHitValidationRadius;

	public override float ssStartDelay => m_fSSStartDelay;

	public override float ssDuration => m_fSSDuration;

	public int castingMoveType => m_nCastingMoveType;

	public int castingMoveValue1 => m_nCastingMoveValue1;

	public int castingMoveValue2 => m_nCastingMoveValue2;

	public AbnormalState buffAbnormalState => m_buffAbnormalState;

	public override int hitCount => m_hits.Count;

	public bool isSpecialSkill => skillId == Resource.instance.specialSkillId;

	public JobSkillLevel maxSkillLevel => m_levels.LastOrDefault();

	public JobSkill(JobSkillMaster skillMaster)
	{
		m_skillMaster = skillMaster;
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_sNameKey = Convert.ToString(dr["nameKey"]);
		m_sDescriptionKey = Convert.ToString(dr["descriptionKey"]);
		m_nSkillType = Convert.ToInt32(dr["type"]);
		m_nFormType = Convert.ToInt32(dr["formType"]);
		m_bIsRequireTarget = Convert.ToBoolean(dr["isRequireTarget"]);
		m_fCastRange = Convert.ToSingle(dr["castRange"]);
		m_fHitRange = Convert.ToSingle(dr["hitRange"]);
		m_fCoolTime = Convert.ToSingle(dr["coolTime"]);
		m_nHeroHitType = Convert.ToInt32(dr["heroHitType"]);
		m_hitAreaType = (SkillHitAreaType)Convert.ToInt32(dr["hitAreaType"]);
		m_fHitAreaValue1 = Convert.ToSingle(dr["hitAreaValue1"]);
		m_fHitAreaValue2 = Convert.ToSingle(dr["hitAreaValue2"]);
		m_hitAreaOffsetType = (SkillHitAreaOffsetType)Convert.ToInt32(dr["hitAreaOffsetType"]);
		m_fHitAreaOffset = Convert.ToSingle(dr["hitAreaOffset"]);
		SetHitValidationRadius();
		m_fSSStartDelay = Convert.ToSingle(dr["ssStartDelay"]);
		m_fSSDuration = Convert.ToSingle(dr["ssDuration"]);
		if (m_nSkillType == 2 && m_fSSDuration <= 0f)
		{
			SFLogUtil.Warn(GetType(), "독립스킬지속시간이 유효하지 않습니다. skillId = " + skillId + ", m_nSSDuration = " + m_fSSDuration);
		}
		m_nCastingMoveType = Convert.ToInt32(dr["castingMoveType"]);
		m_nCastingMoveValue1 = Convert.ToInt32(dr["castingMoveValue1"]);
		m_nCastingMoveValue2 = Convert.ToInt32(dr["castingMoveValue2"]);
		int nBuffAbnormalStateId = Convert.ToInt32(dr["buffAbnormalStateId"]);
		m_buffAbnormalState = Resource.instance.GetAbnormalState(nBuffAbnormalStateId);
	}

	private void SetHitValidationRadius()
	{
		float fOffset = 0f;
		if (m_hitAreaOffsetType == SkillHitAreaOffsetType.Fixed)
		{
			fOffset = m_fHitAreaOffset;
		}
		else if (m_hitAreaOffsetType == SkillHitAreaOffsetType.Dynamic)
		{
			fOffset = m_fHitRange;
		}
		if (m_hitAreaType == SkillHitAreaType.Circle)
		{
			m_fHitValidationRadius = Math.Abs(fOffset) + m_fHitAreaValue1;
		}
		else if (m_hitAreaType == SkillHitAreaType.Square)
		{
			m_fHitValidationRadius = Math.Max(m_fHitAreaValue1 * 0.5f, Math.Abs(fOffset) + m_fHitAreaValue2 / 0.5f);
		}
	}

	public void AddLevel(JobSkillLevel level)
	{
		if (level == null)
		{
			throw new ArgumentNullException("level");
		}
		if (level.skill != null)
		{
			throw new Exception("이미 스킬에 추가된 레벨 입니다.");
		}
		m_levels.Add(level);
		level.skill = this;
	}

	public JobSkillLevel GetLevel(int nLevel)
	{
		int nIndex = nLevel - 1;
		if (nIndex < 0 || nIndex >= m_levels.Count)
		{
			return null;
		}
		return m_levels[nIndex];
	}

	public void AddJobSkillHit(JobSkillHit hit)
	{
		if (hit == null)
		{
			throw new ArgumentNullException("hit");
		}
		if (hit.jobSkill != null)
		{
			throw new Exception("이미 스킬에 추가된 적중입니다.");
		}
		m_hits.Add(hit);
		hit.jobSkill = this;
	}

	public JobSkillHit GetJobSkillHit(int nHitId)
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
		return GetJobSkillHit(nHitId);
	}

	public void AddChainSkill(JobChainSkill chainSkill)
	{
		if (chainSkill == null)
		{
			throw new ArgumentNullException("chainSkill");
		}
		if (chainSkill.skill != null)
		{
			throw new Exception("이미 스킬에 추가된 연계스킬 입니다.");
		}
		m_chainSkills.Add(chainSkill);
		chainSkill.skill = this;
		chainSkill.SetHitValidationRadius();
	}

	public JobChainSkill GetChainSkill(int nChainSkillId)
	{
		int nIndex = nChainSkillId - 1;
		if (nIndex < 0 || nIndex >= m_chainSkills.Count)
		{
			return null;
		}
		return m_chainSkills[nIndex];
	}
}
