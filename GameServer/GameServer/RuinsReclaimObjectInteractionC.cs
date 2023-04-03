using ClientCommon;

namespace GameServer;

public class RuinsReclaimObjectInteractionCancelEventHandler : InGameEventHandler<CEBRuinsReclaimObjectInteractionCancelEventBody>
{
	protected override void HandleInGameEvent()
	{
		if (m_myHero.currentPlace is RuinsReclaimInstance)
		{
			m_myHero.CancelRuinsReclaimObjectInteraction(bSendEventToMyself: false, bSendEventToOthers: true);
		}
	}
}
