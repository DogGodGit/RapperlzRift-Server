using System;

namespace GameServer;

public static class MathUtil
{
	public const int kPermissionDecimalpoint = 10000;

	public static float Sqr(float fValue)
	{
		return fValue * fValue;
	}

	public static float DegreeToRadian(float fValue)
	{
		return (float)(Math.PI * (double)fValue / 180.0);
	}

	public static float RadianToDegree(float fValue)
	{
		return (float)((double)(180f * fValue) / Math.PI);
	}

	public static float DistanceSqr(float fX1, float fY1, float fX2, float fY2)
	{
		return Sqr(fX1 - fX2) + Sqr(fY1 - fY2);
	}

	public static float DistanceSqr(Vector3 a, Vector3 b)
	{
		return DistanceSqr(a.x, a.z, b.x, b.z);
	}

	public static float DistanceSqr_3D(float fX1, float fY1, float fZ1, float fX2, float fY2, float fZ2)
	{
		return Sqr(fX1 - fX2) + Sqr(fY1 - fY2) + Sqr(fZ1 - fZ2);
	}

	public static float DistanceSqr_3D(Vector3 a, Vector3 b)
	{
		return DistanceSqr_3D(a.x, a.y, a.z, b.x, b.y, b.z);
	}

	public static float Distance_3D(Vector3 a, Vector3 b)
	{
		return (float)Math.Sqrt(DistanceSqr_3D(a, b));
	}

	public static bool CircleContains(float fCircleCenterX, float fCircleCenterY, float fCircleRadius, float fTargetX, float fTargetY)
	{
		return DistanceSqr(fCircleCenterX, fCircleCenterY, fTargetX, fTargetY) <= Sqr(fCircleRadius);
	}

	public static bool CircleContains(Vector3 circleCenter, float fCircleRadius, Vector3 target)
	{
		return CircleContains(circleCenter.x, circleCenter.z, fCircleRadius, target.x, target.z);
	}

	public static bool SphereContains(float fCircleCenterX, float fCircleCenterY, float fCircleCenterZ, float fCircleRadius, float fTargetX, float fTargetY, float fTargetZ)
	{
		return DistanceSqr_3D(fCircleCenterX, fCircleCenterY, fCircleCenterZ, fTargetX, fTargetY, fTargetZ) <= Sqr(fCircleRadius);
	}

	public static bool SphereContains(Vector3 circleCenter, float fCircleRadius, Vector3 target)
	{
		return SphereContains(circleCenter.x, circleCenter.y, circleCenter.z, fCircleRadius, target.x, target.y, target.z);
	}

	public static bool FanContainsR(float fFanOriginX, float fFanOriginY, float fFanRadius, float fFanAngle, float fFanDirectionAngle, float fTargetX, float fTargetY)
	{
		if (DistanceSqr(fFanOriginX, fFanOriginY, fTargetX, fTargetY) > Sqr(fFanRadius))
		{
			return false;
		}
		float fHalfFanAngle = fFanAngle / 2f;
		double dDiff = Math.Abs((double)fFanDirectionAngle - Math.Atan2(fTargetY, fTargetX)) % (Math.PI * 2.0);
		if (dDiff > Math.PI)
		{
			dDiff = Math.PI * 2.0 - dDiff;
		}
		return dDiff <= (double)fHalfFanAngle;
	}

	public static bool FanContainsR(Vector3 fanOrigin, float fFanRadius, float fFanAngle, float fFanDirectionAngle, Vector3 target)
	{
		return FanContainsR(fanOrigin.x, fanOrigin.z, fFanRadius, fFanAngle, fFanDirectionAngle, target.x, target.z);
	}

	public static bool FanContainsD(float fFanX, float fFanY, float fFanRadius, float fFanAngle, float fFanDirectionAngle, float fTargetX, float fTargetY)
	{
		return FanContainsR(fFanX, fFanY, fFanRadius, DegreeToRadian(fFanAngle), DegreeToRadian(fFanDirectionAngle), fTargetX, fTargetY);
	}

	public static bool FanContainsD(Vector3 fanOrigin, float fFanRadius, float fFanAngle, float fFanDirectionAngle, Vector3 target)
	{
		return FanContainsD(fanOrigin.x, fanOrigin.z, fFanRadius, fFanAngle, fFanDirectionAngle, target.x, target.z);
	}

	public static bool SquareContains(float standardPointX, float standardPointZ, float fWidth, float fHeight, float fTargetX, float fTargetZ)
	{
		float fHalfWidth = fWidth / 2f;
		if (standardPointX - fHalfWidth > fTargetX)
		{
			return false;
		}
		if (standardPointX + fHalfWidth < fTargetX)
		{
			return false;
		}
		if (standardPointZ > fTargetZ)
		{
			return false;
		}
		if (standardPointZ + fHeight < fTargetZ)
		{
			return false;
		}
		return true;
	}

	public static bool SquareContains(Vector3 standardPoint, float fWidth, float fHeight, Vector3 target)
	{
		return SquareContains(standardPoint.x, standardPoint.z, fWidth, fHeight, target.x, target.z);
	}

	public static Vector3 PositionRotation(float fTargetX, float fTargetY, float fTargetZ, float fRotationValue)
	{
		Vector3 value = default(Vector3);
		value.y = fTargetY;
		value.x = (float)Math.Truncate(((double)fTargetX * Math.Cos(DegreeToRadian(fRotationValue)) - (double)fTargetZ * Math.Sin(DegreeToRadian(fRotationValue))) * 10000.0) / 10000f;
		value.z = (float)Math.Truncate(((double)fTargetX * Math.Sin(DegreeToRadian(fRotationValue)) + (double)fTargetZ * Math.Cos(DegreeToRadian(fRotationValue))) * 10000.0) / 10000f;
		return value;
	}

	public static Vector3 PositionRotation(Vector3 target, float fRotationValue)
	{
		return PositionRotation(target.x, target.y, target.z, fRotationValue);
	}

	public static float IncludedAngle(float fX1, float fZ1, float fX2, float fZ2)
	{
		float fResult = 0f;
		float fNumerator = fX1 * fX2 + fZ1 * fZ2;
		float fDenominator = (float)(Math.Sqrt(Sqr(fX1) + Sqr(fZ1)) * Math.Sqrt(Sqr(fX2) + Sqr(fZ2)));
		fResult = ((fX1 * fZ2 - fX2 * fZ1 > 0f) ? (0f - (float)Math.Acos(fNumerator / fDenominator)) : ((float)Math.Acos(fNumerator / fDenominator)));
		fResult = RadianToDegree(fResult);
		if (fResult < 0f)
		{
			fResult = 360f + fResult;
		}
		return fResult;
	}

	public static float IncludedAngle(Vector3 Origin, Vector3 target)
	{
		return IncludedAngle(Origin.x, Origin.z, target.x, target.z);
	}
}
