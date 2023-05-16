namespace ServerFramework;

public static class SFStringUtil
{
	public static string NullTo(string sValue, string sReplacementValue)
	{
		if (sValue != null)
		{
			return sValue;
		}
		return sReplacementValue;
	}

	public static string NullToEmpty(string sValue)
	{
		return NullTo(sValue, string.Empty);
	}

	public static string NullToNullLiteral(string sValue)
	{
		return NullTo(sValue, "null");
	}
}
