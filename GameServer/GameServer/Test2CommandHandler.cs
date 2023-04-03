using ClientCommon;

namespace GameServer;

public class Test2CommandHandler : InGameCommandHandler<Test2CommandBody, Test2ResponseBody>
{
	protected override void HandleInGameCommand()
	{
		SendResponseOK(null);
	}
}
