using ClientCommon;

namespace GameServer.CommandHandlers;

public class TestCommandHandler : InGameCommandHandler<TestCommandBody, TestResponseBody>
{
    protected override void HandleInGameCommand()
    {
        SendResponseOK(null);
    }
}
