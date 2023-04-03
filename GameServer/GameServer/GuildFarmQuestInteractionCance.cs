using ClientCommon;

namespace GameServer;

public class GuildFarmQuestInteractionCancelEventHandler : InGameEventHandler<CEBGuildFarmQuestInteractionCancelEventBody>
{
	protected override void HandleInGameEvent()
	{
		if (m_myHero.currentPlace is GuildTerritoryInstance)
		{
			m_myHero.CancelGuildFarmQuestInteraction(bSendEventToMyself: false, bSendEventToOthers: true);
		}
	}
}
