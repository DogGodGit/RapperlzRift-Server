using System;
using System.Collections.Generic;
using System.Linq;
using ServerFramework;

namespace GameServer;

public static class Util
{
	public static bool DrawLots(int nTargetPoint, int nMaxPoint)
	{
		if (nMaxPoint <= 0)
		{
			throw new ArgumentOutOfRangeException("nMaxPoint");
		}
		if (nTargetPoint <= 0)
		{
			return false;
		}
		return SFRandom.Next(nMaxPoint) < nTargetPoint;
	}

	public static bool DrawLots(int nTargetPoint)
	{
		return DrawLots(nTargetPoint, 10000);
	}

	public static bool DrawLots(float fChance)
	{
		return DrawLots((int)Math.Floor(fChance * 10000f));
	}

	public static Vector3 SelectPoint(Vector3 basePoint, float fRadius)
	{
		Vector3 pos = default(Vector3);
		pos.x = basePoint.x + SFRandom.NextFloat(0f - fRadius, fRadius);
		pos.y = basePoint.y;
		pos.z = basePoint.z + SFRandom.NextFloat(0f - fRadius, fRadius);
		return pos;
	}

	public static int SelectAngle()
	{
		return SFRandom.Next(360);
	}

	public static T SelectPickEntry<T>(IEnumerable<T> entries, int nTotalPoint) where T : class, IPickEntry
	{
		if (nTotalPoint <= 0)
		{
			return null;
		}
		int nTargetPoint = SFRandom.Next(nTotalPoint) + 1;
		int nPoint = 0;
		foreach (T entry in entries)
		{
			nPoint += entry.point;
			if (nTargetPoint <= nPoint)
			{
				return (T)entry;
			}
		}
		return null;
	}

	public static T SelectPickEntry<T>(IEnumerable<T> entries) where T : class, IPickEntry
	{
		int nTotalPoint = 0;
		foreach (T entry in entries)
		{
			nTotalPoint += entry.point;
		}
		return SelectPickEntry(entries, nTotalPoint);
	}

	public static T SelectAndRemovePickEntry<T>(IList<T> entries, int nTotalPoint) where T : class, IPickEntry
	{
		if (nTotalPoint <= 0)
		{
			return null;
		}
		int nTargetPoint = SFRandom.Next(nTotalPoint) + 1;
		int nPoint = 0;
		for (int i = 0; i < entries.Count; i++)
		{
			IPickEntry entry = entries[i];
			nPoint += entry.point;
			if (nTargetPoint <= nPoint)
			{
				entries.RemoveAt(i);
				return (T)entry;
			}
		}
		return null;
	}

	public static T SelectAndRemovePickEntry<T>(IList<T> entries) where T : class, IPickEntry
	{
		int nTotalPoint = 0;
		foreach (T entry in entries)
		{
			nTotalPoint += entry.point;
		}
		return SelectAndRemovePickEntry(entries, nTotalPoint);
	}

	public static List<T> SelectPickEntries<T>(IEnumerable<T> entries, int nTotalPoint, int nCount, bool bDuplicated) where T : class, IPickEntry
	{
		List<T> results = new List<T>();
		if (nTotalPoint <= 0 || nCount <= 0)
		{
			return results;
		}
		if (bDuplicated)
		{
			for (int j = 0; j < nCount; j++)
			{
				T entry2 = SelectPickEntry(entries, nTotalPoint);
				if (entry2 != null)
				{
					results.Add(entry2);
				}
			}
		}
		else
		{
			List<T> clonedEntries = entries.ToList();
			T entry = SelectAndRemovePickEntry(clonedEntries, nTotalPoint);
			nCount--;
			if (entry != null)
			{
				results.Add(entry);
			}
			for (int i = 0; i < nCount; i++)
			{
				entry = SelectAndRemovePickEntry(clonedEntries);
				if (entry != null)
				{
					results.Add(entry);
				}
			}
		}
		return results;
	}

	public static List<T> SelectPickEntries<T>(IEnumerable<T> entries, int nCount, bool bDuplicated) where T : class, IPickEntry
	{
		int nTotalPoint = 0;
		foreach (T entry in entries)
		{
			nTotalPoint += entry.point;
		}
		return SelectPickEntries(entries, nTotalPoint, nCount, bDuplicated);
	}

	public static long CalcBattlePower(IEnumerable<IAttrValuePair> attrValuePairs)
	{
		long lnBattlePower = 0L;
		foreach (AttrValuePair attrValuePair in attrValuePairs)
		{
			lnBattlePower += attrValuePair.value * Resource.instance.GetAttr(attrValuePair.id).battlePowerFactor;
		}
		return lnBattlePower;
	}
}
