using ClientCommon;

namespace GameServer;

public class ContinentObjectInteractionCancelEventHandler : InGameEventHandler<CEBContinentObjectInteractionCancelEventBody>
{
	protected override void HandleInGameEvent()
	{
		if (m_myHero.currentPlace is ContinentInstance)
		{
			m_myHero.CancelContinentObjectInteraction(bSendEventToMyself: false, bSendEventToOthers: true);
		}
	}
}
