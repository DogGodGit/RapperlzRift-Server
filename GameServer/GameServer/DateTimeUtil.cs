using System;

namespace GameServer;

public static class DateTimeUtil
{
	public const int kTotalSecondsOfDay = 86400;

	public const int kDayOfWeekCount = 7;

	public const string kFormat_Date = "yyyy-MM-dd";

	public const string kFormat_Time = "HH:mm:ss";

	public const string kFormat_DateTimeShort = "yyyy-MM-dd HH:mm:ss";

	public const string kFormat_DateTime = "yyyy-MM-dd HH:mm:ss.fffffff";

	public const string kFormat_DateTimeOffset = "yyyy-MM-dd HH:mm:ss.fffffffzzz";

	public static DateTimeOffset currentTime => DateTimeOffset.Now + GameServerApp.inst.currentTimeOffset;

	public static float GetTimeSpanSeconds(DateTime from, DateTime to)
	{
		return (float)(to - from).TotalSeconds;
	}

	public static float GetTimeSpanSeconds(DateTimeOffset from, DateTimeOffset to)
	{
		return (float)(to - from).TotalSeconds;
	}

	public static int GetDifferenceDays(DateTimeOffset from, DateTimeOffset to)
	{
		return (to.Date - from.Date).Days;
	}

	public static int GetDaysFromStartDayOfWeek(DateTimeOffset dt, DayOfWeek startDayOfWeek)
	{
		int nValue = dt.DayOfWeek - startDayOfWeek;
		if (nValue < 0)
		{
			return nValue + 7;
		}
		return nValue;
	}

	public static DateTime GetWeekStartDate(DateTimeOffset dt, DayOfWeek startDayOfWeek)
	{
		return dt.Date.AddDays(-GetDaysFromStartDayOfWeek(dt, startDayOfWeek));
	}

	public static DateTime GetWeekStartDate(DateTimeOffset dt)
	{
		return GetWeekStartDate(dt, DayOfWeek.Monday);
	}

	public static int GetDaysFromStartDayOfWeek(DateTime dt, DayOfWeek startDayOfWeek)
	{
		int nValue = dt.DayOfWeek - startDayOfWeek;
		if (nValue < 0)
		{
			return nValue + 7;
		}
		return nValue;
	}

	public static DateTime GetWeekStartDate(DateTime dt, DayOfWeek startDayOfWeek)
	{
		return dt.Date.AddDays(-GetDaysFromStartDayOfWeek(dt, startDayOfWeek));
	}

	public static DateTime GetWeekStartDate(DateTime dt)
	{
		return GetWeekStartDate(dt, DayOfWeek.Monday);
	}

	public static string ToString(DateTime time)
	{
		return time.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
	}

	public static string ToString(DateTimeOffset time)
	{
		return time.ToString("yyyy-MM-dd HH:mm:ss.fffffffzzz");
	}
}
