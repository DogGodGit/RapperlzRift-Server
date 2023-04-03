using System.Configuration;

namespace GameServer;

public static class AppConfigUtil
{
	public const string kConnectionStringKey_UserDB = "RappelzRift_User";

	public const string kSettingKey_HashKey = "HashKey";

	public static string hashKey => GetSetting("HashKey");

	public static string userDBConnectionString => GetConnectionString("RappelzRift_User");

	public static string GetSetting(string sKey)
	{
		return ConfigurationManager.AppSettings[sKey];
	}

	public static string GetConnectionString(string sKey)
	{
		return ConfigurationManager.ConnectionStrings[sKey].ConnectionString;
	}
}
