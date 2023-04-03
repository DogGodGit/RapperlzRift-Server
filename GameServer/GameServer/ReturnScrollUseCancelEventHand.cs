using ClientCommon;

namespace GameServer;

public class ReturnScrollUseCancelEventHandler : InGameEventHandler<CEBReturnScrollUseCancelEventBody>
{
	protected override void HandleInGameEvent()
	{
		Place currentPlace = m_myHero.currentPlace;
		if (currentPlace != null)
		{
			m_myHero.CancelReturnScrollUse(bSendEventToMyself: false, bSendEventToOthers: true);
		}
	}
}
