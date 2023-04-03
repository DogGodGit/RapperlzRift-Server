using ServerFramework;

namespace GameServer;

public static class SqlStandaloneWorkUtil
{
	public static SFSqlStandaloneWork CreateUserDBWork()
	{
		return new SFSqlStandaloneWork(DBUtil.userDBConnectionString);
	}

	public static SFSqlStandaloneWork CreateGameLogDBWork()
	{
		return new SFSqlStandaloneWork(DBUtil.GetGameLogDBConnectionString(GameServerApp.inst.serverId));
	}
}
