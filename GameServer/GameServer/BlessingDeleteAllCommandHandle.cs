using ClientCommon;

namespace GameServer;

public class BlessingDeleteAllCommandHandler : InGameCommandHandler<BlessingDeleteAllCommandBody, BlessingDeleteAllResponseBody>
{
	protected override void HandleInGameCommand()
	{
		m_myHero.RemoveAllReceivedBlessings();
		SendResponseOK(null);
	}
}
