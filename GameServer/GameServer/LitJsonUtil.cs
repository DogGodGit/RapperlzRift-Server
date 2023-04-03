using System;
using System.Collections;
using LitJson;

namespace GameServer;

public static class LitJsonUtil
{
	public static JsonData Create(JsonType type)
	{
		JsonData jo = new JsonData();
		jo.SetJsonType(type);
		return jo;
	}

	public static JsonData CreateArray()
	{
		return Create(JsonType.Array);
	}

	public static JsonData CreateObject()
	{
		return Create(JsonType.Object);
	}

	public static bool GetBooleanValue(JsonData jo)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		return (bool)jo;
	}

	public static int GetIntValue(JsonData jo)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		int value = 0;
		return jo.GetJsonType() switch
		{
			JsonType.Double => Convert.ToInt32((double)jo), 
			JsonType.Int => Convert.ToInt32((int)jo), 
			JsonType.Long => Convert.ToInt32((long)jo), 
			_ => throw new InvalidOperationException("JsonData 인스턴스가 숫자형식이 아닙니다."), 
		};
	}

	public static long GetLongValue(JsonData jo)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		long value = 0L;
		return jo.GetJsonType() switch
		{
			JsonType.Double => Convert.ToInt64((double)jo), 
			JsonType.Int => Convert.ToInt64((int)jo), 
			JsonType.Long => Convert.ToInt64((long)jo), 
			_ => throw new InvalidOperationException("JsonData 인스턴스가 숫자형식이 아닙니다."), 
		};
	}

	public static double GetDoubleValue(JsonData jo)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		double value = 0.0;
		return jo.GetJsonType() switch
		{
			JsonType.Double => Convert.ToDouble((double)jo), 
			JsonType.Int => Convert.ToDouble((int)jo), 
			JsonType.Long => Convert.ToDouble((long)jo), 
			_ => throw new InvalidOperationException("JsonData 인스턴스가 숫자형식이 아닙니다."), 
		};
	}

	public static string GetStringValue(JsonData jo)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		return (string)jo;
	}

	public static bool TryGetBooleanValue(JsonData jo, out bool value)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		value = false;
		if (!jo.IsBoolean)
		{
			return false;
		}
		value = (bool)jo;
		return true;
	}

	public static bool TryGetIntValue(JsonData jo, out int value)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		value = 0;
		try
		{
			switch (jo.GetJsonType())
			{
			case JsonType.Double:
				value = Convert.ToInt32((double)jo);
				break;
			case JsonType.Int:
				value = Convert.ToInt32((int)jo);
				break;
			case JsonType.Long:
				value = Convert.ToInt32((long)jo);
				break;
			default:
				return false;
			}
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static bool TryGetLongValue(JsonData jo, out long value)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		value = 0L;
		try
		{
			switch (jo.GetJsonType())
			{
			case JsonType.Double:
				value = Convert.ToInt64((double)jo);
				break;
			case JsonType.Int:
				value = Convert.ToInt64((int)jo);
				break;
			case JsonType.Long:
				value = Convert.ToInt64((long)jo);
				break;
			default:
				return false;
			}
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static bool TryGetDoubleValue(JsonData jo, out double value)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		value = 0.0;
		try
		{
			switch (jo.GetJsonType())
			{
			case JsonType.Double:
				value = Convert.ToDouble((double)jo);
				break;
			case JsonType.Int:
				value = Convert.ToDouble((int)jo);
				break;
			case JsonType.Long:
				value = Convert.ToDouble((long)jo);
				break;
			default:
				return false;
			}
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static bool TryGetStringValue(JsonData jo, out string value)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		value = null;
		if (!jo.IsString)
		{
			return false;
		}
		value = (string)jo;
		return true;
	}

	public static JsonData GetArrayProperty(JsonData jo, string sName)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		if (sName == null)
		{
			throw new ArgumentNullException("sName");
		}
		JsonData joProperty = jo[sName];
		if (joProperty != null && !joProperty.IsArray)
		{
			throw new InvalidOperationException("JsonData 인스턴스가 Array형식이 아닙니다.");
		}
		return joProperty;
	}

	public static JsonData GetObjectProperty(JsonData jo, string sName)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		if (sName == null)
		{
			throw new ArgumentNullException("sName");
		}
		JsonData joProperty = jo[sName];
		if (joProperty != null && !joProperty.IsObject)
		{
			throw new InvalidOperationException("JsonData 인스턴스가 Object형식이 아닙니다.");
		}
		return joProperty;
	}

	public static bool GetBooleanProperty(JsonData jo, string sName)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		if (sName == null)
		{
			throw new ArgumentNullException("sName");
		}
		return GetBooleanValue(jo[sName]);
	}

	public static int GetIntProperty(JsonData jo, string sName)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		if (sName == null)
		{
			throw new ArgumentNullException("sName");
		}
		return GetIntValue(jo[sName]);
	}

	public static long GetLongProperty(JsonData jo, string sName)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		if (sName == null)
		{
			throw new ArgumentNullException("sName");
		}
		return GetLongValue(jo[sName]);
	}

	public static double GetDoubleProperty(JsonData jo, string sName)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		if (sName == null)
		{
			throw new ArgumentNullException("sName");
		}
		return GetDoubleValue(jo[sName]);
	}

	public static string GetStringProperty(JsonData jo, string sName)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		if (sName == null)
		{
			throw new ArgumentNullException("sName");
		}
		JsonData joProperty = jo[sName];
		if (joProperty != null)
		{
			return (string)joProperty;
		}
		return null;
	}

	public static bool TryGetProperty(JsonData jo, string sName, out JsonData value)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		if (sName == null)
		{
			throw new ArgumentNullException("sName");
		}
		value = null;
		if (!((IDictionary)jo).Contains((object)sName))
		{
			return false;
		}
		value = jo[sName];
		return true;
	}

	public static bool TryGetArrayProperty(JsonData jo, string sName, out JsonData value)
	{
		value = null;
		if (!TryGetProperty(jo, sName, out var joProperty))
		{
			return false;
		}
		if (joProperty == null)
		{
			return true;
		}
		if (!joProperty.IsArray)
		{
			return false;
		}
		value = joProperty;
		return true;
	}

	public static bool TryGetObjectProperty(JsonData jo, string sName, out JsonData value)
	{
		value = null;
		if (!TryGetProperty(jo, sName, out var joProperty))
		{
			return false;
		}
		if (joProperty == null)
		{
			return true;
		}
		if (!joProperty.IsObject)
		{
			return false;
		}
		value = joProperty;
		return true;
	}

	public static bool TryGetBooleanProperty(JsonData jo, string sName, out bool value)
	{
		value = false;
		if (!TryGetProperty(jo, sName, out var joProperty) || joProperty == null)
		{
			return false;
		}
		if (!joProperty.IsBoolean)
		{
			return false;
		}
		value = (bool)joProperty;
		return true;
	}

	public static bool TryGetIntProperty(JsonData jo, string sName, out int value)
	{
		value = 0;
		if (!TryGetProperty(jo, sName, out var joProperty) || joProperty == null)
		{
			return false;
		}
		try
		{
			switch (joProperty.GetJsonType())
			{
			case JsonType.Double:
				value = Convert.ToInt32((double)joProperty);
				break;
			case JsonType.Int:
				value = Convert.ToInt32((int)joProperty);
				break;
			case JsonType.Long:
				value = Convert.ToInt32((long)joProperty);
				break;
			default:
				return false;
			}
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static bool TryGetLongProperty(JsonData jo, string sName, out long value)
	{
		value = 0L;
		if (!TryGetProperty(jo, sName, out var joProperty) || joProperty == null)
		{
			return false;
		}
		try
		{
			switch (joProperty.GetJsonType())
			{
			case JsonType.Double:
				value = Convert.ToInt64((double)joProperty);
				break;
			case JsonType.Int:
				value = Convert.ToInt64((int)joProperty);
				break;
			case JsonType.Long:
				value = Convert.ToInt64((long)joProperty);
				break;
			default:
				return false;
			}
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static bool TryGetDoubleProperty(JsonData jo, string sName, out double value)
	{
		value = 0.0;
		if (!TryGetProperty(jo, sName, out var joProperty) || joProperty == null)
		{
			return false;
		}
		try
		{
			switch (joProperty.GetJsonType())
			{
			case JsonType.Double:
				value = Convert.ToDouble((double)joProperty);
				break;
			case JsonType.Int:
				value = Convert.ToDouble((int)joProperty);
				break;
			case JsonType.Long:
				value = Convert.ToDouble((long)joProperty);
				break;
			default:
				return false;
			}
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	public static bool TryGetStringProperty(JsonData jo, string sName, out string value)
	{
		value = null;
		if (!TryGetProperty(jo, sName, out var joProperty))
		{
			return false;
		}
		if (joProperty == null)
		{
			return true;
		}
		if (!joProperty.IsString)
		{
			return false;
		}
		value = (string)joProperty;
		return true;
	}

	public static bool Contains(JsonData jo, string sName)
	{
		if (jo == null)
		{
			throw new ArgumentNullException("jo");
		}
		if (sName == null)
		{
			throw new ArgumentNullException("sName");
		}
		return ((IDictionary)jo).Contains((object)sName);
	}
}
