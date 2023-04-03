namespace GameServer;

public static class BattleUtil
{
	public static bool IsHit(Vector3 attackerPosition, float fAttackerRange, Vector3 targetPosition, float fTargetRadius)
	{
		return MathUtil.DistanceSqr_3D(attackerPosition, targetPosition) <= MathUtil.Sqr(fAttackerRange + fTargetRadius);
	}
}
