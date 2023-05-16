using System;

namespace ServerFramework;

public static class SFRandom
{
	private static Random s_instance = new Random();

	public static int Next()
	{
		lock (s_instance)
		{
			return s_instance.Next();
		}
	}

	public static int Next(int nMaxValue)
	{
		lock (s_instance)
		{
			return s_instance.Next(nMaxValue);
		}
	}

	public static int Next(int nMinValue, int nMaxValue)
	{
		lock (s_instance)
		{
			return s_instance.Next(nMinValue, nMaxValue);
		}
	}

	public static double NextDouble(double minValue, double maxValue)
	{
		if (minValue > maxValue)
		{
			throw new ArgumentOutOfRangeException("minValue가 maxValue보다 큽니다.");
		}
		if (minValue == maxValue)
		{
			return minValue;
		}
		lock (s_instance)
		{
			return s_instance.NextDouble() * (maxValue - minValue) + minValue;
		}
	}

	public static float NextFloat()
	{
		lock (s_instance)
		{
			return (float)s_instance.NextDouble();
		}
	}

	public static float NextFloat(float fValue)
	{
		lock (s_instance)
		{
			return NextFloat(0f, fValue);
		}
	}

	public static float NextFloat(float minValue, float maxValue)
	{
		if (minValue > maxValue)
		{
			throw new ArgumentOutOfRangeException("minValue가 maxValue보다 큽니다.");
		}
		if (minValue == maxValue)
		{
			return minValue;
		}
		lock (s_instance)
		{
			return NextFloat() * (maxValue - minValue) + minValue;
		}
	}

	public static int NextInt(double minValue, double maxValue)
	{
		lock (s_instance)
		{
			return s_instance.Next((int)Math.Ceiling(minValue), (int)Math.Floor(maxValue) + 1);
		}
	}
}
