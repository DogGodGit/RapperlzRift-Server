using System.Collections.Generic;

namespace GameServer;

public abstract class SkillHit
{
	public const int kType_JobSkillHit = 1;

	public const int kType_JobChainSkillHit = 2;

	public const int kType_MonsterSkillHit = 3;

	public abstract int type { get; }

	public abstract Skill skill { get; }

	public abstract int id { get; }

	public abstract float damageFactor { get; }

	public abstract int acquireLak { get; }

	public abstract List<JobSkillHitAbnormalState> abnormalStates { get; }
}
