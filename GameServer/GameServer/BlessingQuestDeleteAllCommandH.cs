using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class BlessingQuestDeleteAllCommandHandler : InGameCommandHandler<BlessingQuestDeleteAllCommandBody, BlessingQuestDeleteAllResponseBody>
{
	protected override void HandleInGameCommand()
	{
		m_myHero.RemoveAllBlessingQuests();
		SendResponseOK(null);
	}
}
