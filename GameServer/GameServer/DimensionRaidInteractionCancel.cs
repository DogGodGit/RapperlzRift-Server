using ClientCommon;

namespace GameServer;

public class DimensionRaidInteractionCancelEventHandler : InGameEventHandler<CEBDimensionRaidInteractionCancelEventBody>
{
	protected override void HandleInGameEvent()
	{
		if (m_myHero.currentPlace is ContinentInstance)
		{
			m_myHero.CancelDimensionRaidInteraction(bSendEventToMyself: false, bSendEventToOthers: true);
		}
	}
}
