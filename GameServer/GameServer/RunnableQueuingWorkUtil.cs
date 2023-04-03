using System;
using ServerFramework;

namespace GameServer;

public static class RunnableQueuingWorkUtil
{
	public static SFRunnableQueuingWork CreateUserWork(Guid userId, ISFRunnable runnable)
	{
		return new SFRunnableQueuingWork(1, userId, runnable);
	}

	public static SFRunnableQueuingWork CreateUserWork(Guid userId)
	{
		return CreateUserWork(userId, null);
	}

	public static SFRunnableQueuingWork CreateAccountWork(Guid accountId, ISFRunnable runnable)
	{
		return new SFRunnableQueuingWork(2, accountId, runnable);
	}

	public static SFRunnableQueuingWork CreateAccountWork(Guid accountId)
	{
		return CreateAccountWork(accountId, null);
	}

	public static SFRunnableQueuingWork CreateHeroWork(Guid heroId, ISFRunnable runnable)
	{
		return new SFRunnableQueuingWork(3, heroId, runnable);
	}

	public static SFRunnableQueuingWork CreateHeroWork(Guid heroId)
	{
		return CreateHeroWork(heroId, null);
	}

	public static SFRunnableQueuingWork CreateContentWork(QueuingWorkContentId contentId)
	{
		return CreateContentWork(contentId, null);
	}

	public static SFRunnableQueuingWork CreateContentWork(QueuingWorkContentId contentId, ISFRunnable runnable)
	{
		return new SFRunnableQueuingWork(4, (int)contentId, runnable);
	}

	public static SFRunnableQueuingWork CreateGuildWork(Guid guildId, ISFRunnable runnable)
	{
		return new SFRunnableQueuingWork(5, guildId, runnable);
	}

	public static SFRunnableQueuingWork CreateGuildWork(Guid guildId)
	{
		return CreateGuildWork(guildId, null);
	}
}
