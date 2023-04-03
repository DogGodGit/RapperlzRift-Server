using ClientCommon;

namespace GameServer;

public class WarMemoryObjectInteractionCancelEventHandler : InGameEventHandler<CEBWarMemoryObjectInteractionCancelEventBody>
{
	protected override void HandleInGameEvent()
	{
		if (m_myHero.currentPlace is WarMemoryInstance)
		{
			m_myHero.CancelWarMemoryTransformationObjectInteraction(bSendEventToMyself: false, bSendEventToOthers: true);
		}
	}
}
