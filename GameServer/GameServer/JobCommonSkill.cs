using System;
using System.Data;

namespace GameServer;

public class JobCommonSkill : Skill
{
	public const int kFormType_ChainSkill = 1;

	public const int kFormType_NormalSkill = 2;

	public const int kFormType_BuffSkill = 3;

	public const int kEvasionSkillId = 1;

	private int m_nId;

	private int m_nOpenRequiredMainQuestNo;

	private int m_nSkillType;

	private int m_nFormType;

	private bool m_bIsRequireTarget;

	private float m_fCastRange;

	private float m_fHitRange;

	private float m_fCoolTime;

	private SkillHitAreaType m_hitAreaType;

	private float m_fHitAreaValue1;

	private float m_fHitAreaValue2;

	private SkillHitAreaOffsetType m_hitAreaOffsetType;

	private float m_fHitAreaOffset;

	private float m_fHitValidationRadius;

	private int m_nMentalStrengthDamage;

	private AbnormalState m_buffAbnormalState;

	public override int type => 4;

	public override int skillId => m_nId;

	public override int chainSkillId => 0;

	public int openRequiredMainQuestNo => m_nOpenRequiredMainQuestNo;

	public int skillType => m_nSkillType;

	public int formType => m_nFormType;

	public bool isRequireTarget => m_bIsRequireTarget;

	public float castRange => m_fCastRange;

	public float hitRange => m_fHitRange;

	public float coolTime => m_fCoolTime;

	public override SkillHitAreaType hitAreaType => m_hitAreaType;

	public override float hitAreaValue1 => m_fHitAreaValue1;

	public override float hitAreaValue2 => m_fHitAreaValue2;

	public override SkillHitAreaOffsetType hitAreaOffsetType => m_hitAreaOffsetType;

	public override float hitAreaOffset => m_fHitAreaOffset;

	public override float hitValidationRadius => m_fHitValidationRadius;

	public override float ssStartDelay => 0f;

	public override float ssDuration => 0f;

	public int mentalStrengthDamage => m_nMentalStrengthDamage;

	public AbnormalState buffAbnormalState => m_buffAbnormalState;

	public override int hitCount
	{
		get
		{
			if (m_nFormType != 3)
			{
				return 1;
			}
			return 0;
		}
	}

	public void Set(DataRow dr)
	{
		if (dr == null)
		{
			throw new ArgumentNullException("dr");
		}
		m_nId = Convert.ToInt32(dr["skillId"]);
		m_nOpenRequiredMainQuestNo = Convert.ToInt32(dr["openRequiredMainQuestNo"]);
		m_nSkillType = Convert.ToInt32(dr["type"]);
		m_nFormType = Convert.ToInt32(dr["formType"]);
		m_bIsRequireTarget = Convert.ToBoolean(dr["isRequireTarget"]);
		m_fCastRange = Convert.ToSingle(dr["castRange"]);
		m_fHitRange = Convert.ToSingle(dr["hitRange"]);
		m_fCoolTime = Convert.ToSingle(dr["coolTime"]);
		m_hitAreaType = (SkillHitAreaType)Convert.ToInt32(dr["hitAreaType"]);
		m_fHitAreaValue1 = Convert.ToSingle(dr["hitAreaValue1"]);
		m_fHitAreaValue2 = Convert.ToSingle(dr["hitAreaValue2"]);
		m_hitAreaOffsetType = (SkillHitAreaOffsetType)Convert.ToInt32(dr["hitAreaOffsetType"]);
		m_fHitAreaOffset = Convert.ToSingle(dr["hitAreaOffset"]);
		SetHitValidationRadius();
		m_nMentalStrengthDamage = Convert.ToInt32(dr["mentalStrengthDamage"]);
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

	public override SkillHit GetHit(int nHitId)
	{
		return null;
	}
}
