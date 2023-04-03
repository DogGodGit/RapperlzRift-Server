namespace GameServer;

public abstract class Skill
{
	public const int kType_JobSkill = 1;

	public const int kType_JobChainSkill = 2;

	public const int kType_MonsterSkill = 3;

	public const int kType_JobCommonSkill = 4;

	public const int kSkillType_CharacterDependent = 1;

	public const int kSkillType_CharacterStandalone = 2;

	public const float kHitValidationRadiusFactor = 1.1f;

	public const float kSkillCoolTimeFactor = 0.9f;

	public const float kSkillCastRangeFactor = 1.1f;

	public const float kSkillMoveValueFactor = 1.1f;

	public abstract int type { get; }

	public abstract int skillId { get; }

	public abstract int chainSkillId { get; }

	public abstract SkillHitAreaType hitAreaType { get; }

	public abstract float hitAreaValue1 { get; }

	public abstract float hitAreaValue2 { get; }

	public abstract SkillHitAreaOffsetType hitAreaOffsetType { get; }

	public abstract float hitAreaOffset { get; }

	public abstract float hitValidationRadius { get; }

	public abstract float ssStartDelay { get; }

	public abstract float ssDuration { get; }

	public abstract int hitCount { get; }

	public abstract SkillHit GetHit(int nHitId);
}
