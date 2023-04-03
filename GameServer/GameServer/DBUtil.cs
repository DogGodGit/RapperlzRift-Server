using System.Data.SqlClient;
using ServerFramework;

namespace GameServer;

public static class DBUtil
{
	public static string userDBConnectionString => AppConfigUtil.userDBConnectionString;

	public static SqlConnection OpenUserDBConnection()
	{
		return SFDBUtil.OpenConnection(userDBConnectionString);
	}

	public static string GetGameDBConnectionString(int nGameServerId)
	{
		return SystemResource.instance.GetGameServer(nGameServerId)?.gameDBConnectionString;
	}

	public static SqlConnection OpenGameDBConnection(int nGameServerId)
	{
		return SFDBUtil.OpenConnection(GetGameDBConnectionString(nGameServerId));
	}

	public static SqlConnection OpenGameDBConnection()
	{
		return OpenGameDBConnection(GameServerApp.inst.serverId);
	}

	public static string GetGameLogDBConnectionString(int nGameServerId)
	{
		return SystemResource.instance.GetGameServer(nGameServerId)?.gameLogDBConnectionString;
	}

	public static SqlConnection OpenGameLogDBConnection(int nGameServerId)
	{
		return SFDBUtil.OpenConnection(GetGameLogDBConnectionString(nGameServerId));
	}

	public static SqlConnection OpenGameLogDBConnection()
	{
		return OpenGameLogDBConnection(GameServerApp.inst.serverId);
	}
}
