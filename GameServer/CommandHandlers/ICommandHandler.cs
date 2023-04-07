using ServerFramework;

namespace GameServer.CommandHandlers;

public interface ICommandHandler : ISFCommandHandler, ISFHandler, ISFRunnable
{
}
