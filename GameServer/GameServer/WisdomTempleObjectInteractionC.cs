using ClientCommon;

namespace GameServer;

public class WisdomTempleObjectInteractionCancelEventHandler : InGameEventHandler<CEBWisdomTempleObjectInteractionCancelEvnetBody>
{
	protected override void HandleInGameEvent()
	{
		if (m_myHero.currentPlace is WisdomTempleInstance)
		{
			m_myHero.CancelWisdomTempleObjectInteraction(bSendEvent: false);
		}
	}
}
