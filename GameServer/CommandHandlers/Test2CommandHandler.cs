using ClientCommon;

namespace GameServer.CommandHandlers;

public class Test2CommandHandler : InGameCommandHandler<Test2CommandBody, Test2ResponseBody>
{
    protected override void HandleInGameCommand()
    {
        SendResponseOK(null);
    }
}
