using System.Collections.Generic;

namespace ServerFramework;

public class SFQueuingWorkEqualityComparer : EqualityComparer<SFQueuingWork>
{
	public static readonly SFQueuingWorkEqualityComparer defaultComparer = new SFQueuingWorkEqualityComparer();

	public override bool Equals(SFQueuingWork x, SFQueuingWork y)
	{
		if (x.targetType == y.targetType)
		{
			return x.targetId.Equals(y.targetId);
		}
		return false;
	}

	public override int GetHashCode(SFQueuingWork obj)
	{
		return obj.targetType.GetHashCode() ^ obj.targetId.GetHashCode();
	}
}
