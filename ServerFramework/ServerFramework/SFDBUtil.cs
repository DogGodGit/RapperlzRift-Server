using System;
using System.Data.SqlClient;

namespace ServerFramework;

public static class SFDBUtil
{
	public static SqlConnection OpenConnection(string sConnectionString)
	{
		SqlConnection sqlConnection = new SqlConnection(sConnectionString);
		sqlConnection.Open();
		return sqlConnection;
	}

	public static void Close(ref SqlConnection conn)
	{
		if (conn != null)
		{
			conn.Close();
			conn = null;
		}
	}

	public static void Commit(ref SqlTransaction trans)
	{
		if (trans != null)
		{
			trans.Commit();
			trans = null;
		}
	}

	public static void Rollback(ref SqlTransaction trans)
	{
		if (trans != null)
		{
			trans.Rollback();
			trans = null;
		}
	}

	public static string EscapeSingleQuote(string sValue)
	{
		return sValue?.Replace("'", "''");
	}

	public static object NullToDBNull(object value)
	{
		if (value != null)
		{
			return value;
		}
		return DBNull.Value;
	}

	public static byte ToByte(object value, byte valueForDBNull)
	{
		if (value == DBNull.Value)
		{
			return valueForDBNull;
		}
		return Convert.ToByte(value);
	}

	public static byte ToByte(object value)
	{
		return ToByte(value, 0);
	}

	public static byte? ToNullableByte(object value)
	{
		if (value == DBNull.Value)
		{
			return null;
		}
		return Convert.ToByte(value);
	}

	public static sbyte ToSByte(object value, sbyte valueForDBNull)
	{
		if (value == DBNull.Value)
		{
			return valueForDBNull;
		}
		return Convert.ToSByte(value);
	}

	public static sbyte ToSByte(object value)
	{
		return ToSByte(value, 0);
	}

	public static sbyte? ToNullableSByte(object value)
	{
		if (value == DBNull.Value)
		{
			return null;
		}
		return (sbyte)Convert.ToByte(value);
	}

	public static short ToInt16(object value, short valueForDBNull)
	{
		if (value == DBNull.Value)
		{
			return valueForDBNull;
		}
		return Convert.ToInt16(value);
	}

	public static short ToInt16(object value)
	{
		return ToInt16(value, 0);
	}

	public static short? ToNullableInt16(object value)
	{
		if (value == DBNull.Value)
		{
			return null;
		}
		return Convert.ToInt16(value);
	}

	public static int ToInt32(object value, int valueForDBNull)
	{
		if (value == DBNull.Value)
		{
			return valueForDBNull;
		}
		return Convert.ToInt32(value);
	}

	public static int ToInt32(object value)
	{
		return ToInt32(value, 0);
	}

	public static int? ToNullableInt32(object value)
	{
		if (value == DBNull.Value)
		{
			return null;
		}
		return Convert.ToInt32(value);
	}

	public static long ToInt64(object value, long valueForDBNull)
	{
		if (value == DBNull.Value)
		{
			return valueForDBNull;
		}
		return Convert.ToInt64(value);
	}

	public static long ToInt64(object value)
	{
		return ToInt64(value, 0L);
	}

	public static long? ToNullableInt64(object value)
	{
		if (value == DBNull.Value)
		{
			return null;
		}
		return Convert.ToInt64(value);
	}

	public static float ToSingle(object value, float valueForDBNull)
	{
		if (value == DBNull.Value)
		{
			return valueForDBNull;
		}
		return Convert.ToSingle(value);
	}

	public static float ToSingle(object value)
	{
		return ToSingle(value, 0f);
	}

	public static float? ToNullableSingle(object value)
	{
		if (value == DBNull.Value)
		{
			return null;
		}
		return Convert.ToSingle(value);
	}

	public static double ToDouble(object value, double valueForDBNull)
	{
		if (value == DBNull.Value)
		{
			return valueForDBNull;
		}
		return Convert.ToDouble(value);
	}

	public static double ToDouble(object value)
	{
		return ToDouble(value, 0.0);
	}

	public static double? ToNullableDouble(object value)
	{
		if (value == DBNull.Value)
		{
			return null;
		}
		return Convert.ToDouble(value);
	}

	public static string ToString(object value, string valueForDBNull)
	{
		if (value == DBNull.Value)
		{
			return valueForDBNull;
		}
		return Convert.ToString(value);
	}

	public static string ToString(object value)
	{
		return ToString(value, null);
	}

	public static Guid ToGuid(object value, Guid valueForDBNull)
	{
		if (value == DBNull.Value)
		{
			return valueForDBNull;
		}
		return (Guid)value;
	}

	public static Guid ToGuid(object value)
	{
		return ToGuid(value, Guid.Empty);
	}

	public static Guid? ToNullableGuid(object value)
	{
		if (value == DBNull.Value)
		{
			return null;
		}
		return (Guid?)value;
	}

	public static DateTime ToDateTime(object value, DateTime valueForDBNull)
	{
		if (value == DBNull.Value)
		{
			return valueForDBNull;
		}
		return Convert.ToDateTime(value);
	}

	public static DateTime ToDateTime(object value)
	{
		return ToDateTime(value, DateTime.MinValue);
	}

	public static DateTime? ToNullableDateTime(object value)
	{
		if (value == DBNull.Value)
		{
			return null;
		}
		return Convert.ToDateTime(value);
	}

	public static DateTimeOffset ToDateTimeOffset(object value, DateTimeOffset valueForDBNull)
	{
		if (value == DBNull.Value)
		{
			return valueForDBNull;
		}
		return (DateTimeOffset)value;
	}

	public static DateTimeOffset ToDateTimeOffset(object value)
	{
		return ToDateTimeOffset(value, DateTimeOffset.MinValue);
	}

	public static DateTimeOffset? ToNullableDateTimeOffset(object value)
	{
		if (value == DBNull.Value)
		{
			return null;
		}
		return (DateTimeOffset?)value;
	}

    public static bool ToBoolean(object value, bool valueForDBNull)
    {
        if (value == DBNull.Value)
        {
            return valueForDBNull;
        }
        return Convert.ToBoolean(value);
    }

    public static bool ToBoolean(object value)
    {
        return ToBoolean(value, false);
    }

    public static bool? ToNullableBoolean(object value)
    {
        if (value == DBNull.Value)
        {
            return null;
        }
        return Convert.ToBoolean(value);
    }
}
