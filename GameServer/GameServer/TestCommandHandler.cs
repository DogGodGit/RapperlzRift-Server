using ClientCommon;

namespace GameServer;

public class TestCommandHandler : InGameCommandHandler<TestCommandBody, TestResponseBody>
{
	protected override void HandleInGameCommand()
	{
		SendResponseOK(null);
	}
}
