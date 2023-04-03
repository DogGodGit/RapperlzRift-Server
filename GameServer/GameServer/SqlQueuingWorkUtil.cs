using System;
using ServerFramework;

namespace GameServer;

public static class SqlQueuingWorkUtil
{
	public static SFSqlQueuingWork CreateUserWork(Guid userId)
	{
		return new SFSqlQueuingWork(1, userId, DBUtil.userDBConnectionString);
	}

	public static SFSqlQueuingWork CreateAccountWork(Guid accountId)
	{
		return new SFSqlQueuingWork(2, accountId, DBUtil.GetGameDBConnectionString(GameServerApp.inst.serverId));
	}

	public static SFSqlQueuingWork CreateHeroWork(Guid heroId)
	{
		return new SFSqlQueuingWork(3, heroId, DBUtil.GetGameDBConnectionString(GameServerApp.inst.serverId));
	}

	public static SFSqlQueuingWork CreateContentWork(QueuingWorkContentId contentId, string sConnectionString)
	{
		return new SFSqlQueuingWork(4, (int)contentId, sConnectionString);
	}

	public static SFSqlQueuingWork CreateUserDBContentWork(QueuingWorkContentId contentId)
	{
		return CreateContentWork(contentId, DBUtil.userDBConnectionString);
	}

	public static SFSqlQueuingWork CreateGameDBContentWork(QueuingWorkContentId contentId)
	{
		return CreateContentWork(contentId, DBUtil.GetGameDBConnectionString(GameServerApp.inst.serverId));
	}

	public static SFSqlQueuingWork CreateGameLogDBContentWork(QueuingWorkContentId contentId)
	{
		return CreateContentWork(contentId, DBUtil.GetGameLogDBConnectionString(GameServerApp.inst.serverId));
	}

	public static SFSqlQueuingWork CreateGuildWork(Guid guildId)
	{
		return new SFSqlQueuingWork(5, guildId, DBUtil.GetGameDBConnectionString(GameServerApp.inst.serverId));
	}

	public static SFSqlQueuingWork CreateNationWork(int nNationId)
	{
		return new SFSqlQueuingWork(6, nNationId, DBUtil.GetGameDBConnectionString(GameServerApp.inst.serverId));
	}
}
