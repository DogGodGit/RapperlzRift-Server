using ClientCommon;

namespace GameServer;

public class SecretLetterPickCancelEventHandler : InGameEventHandler<CEBSecretLetterPickCancelEventBody>
{
	protected override void HandleInGameEvent()
	{
		if (m_myHero.currentPlace is ContinentInstance)
		{
			m_myHero.CancelSecretLetterPicking(bSendEventToMyself: false, bSendEventToOthers: true);
		}
	}
}
