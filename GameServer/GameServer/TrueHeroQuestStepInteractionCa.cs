using ClientCommon;

namespace GameServer;

public class TrueHeroQuestStepInteractionCancelEventHandler : InGameEventHandler<CEBTrueHeroQuestStepInteractionCancelEventBody>
{
	protected override void HandleInGameEvent()
	{
		if (m_myHero.currentPlace is ContinentInstance)
		{
			m_myHero.CancelTrueHeroQuestInteraction(bSendEventToMyself: false, bSendEventToOthers: true);
		}
	}
}
