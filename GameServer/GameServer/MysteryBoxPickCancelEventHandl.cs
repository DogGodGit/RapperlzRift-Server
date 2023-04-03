using ClientCommon;

namespace GameServer;

public class MysteryBoxPickCancelEventHandler : InGameEventHandler<CEBMysteryBoxPickCancelEventBody>
{
	protected override void HandleInGameEvent()
	{
		if (m_myHero.currentPlace is ContinentInstance)
		{
			m_myHero.CancelMysteryBoxPicking(bSendEventToMyself: false, bSendEventToOthers: true);
		}
	}
}
