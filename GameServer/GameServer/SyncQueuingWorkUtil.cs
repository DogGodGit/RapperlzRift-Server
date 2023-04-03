using System;
using ServerFramework;

namespace GameServer;

public static class SyncQueuingWorkUtil
{
	public static SFSyncQueuingWork CreateUserWork(Guid userId)
	{
		return new SFSyncQueuingWork(1, userId);
	}

	public static SFSyncQueuingWork CreateAccountWork(Guid accountId)
	{
		return new SFSyncQueuingWork(2, accountId);
	}

	public static SFSyncQueuingWork CreateHeroWork(Guid heroId)
	{
		return new SFSyncQueuingWork(3, heroId);
	}

	public static SFSyncQueuingWork CreateContentWork(QueuingWorkContentId contentId)
	{
		return new SFSyncQueuingWork(4, contentId);
	}

	public static SFSyncQueuingWork CreateGuildWork(Guid guildId)
	{
		return new SFSyncQueuingWork(5, guildId);
	}

	public static SFSyncQueuingWork CreateNationWork(int nNationId)
	{
		return new SFSyncQueuingWork(6, nNationId);
	}
}
