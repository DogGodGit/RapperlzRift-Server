using ClientCommon;

namespace GameServer;

public class DistortionCancelCommandHandler : InGameCommandHandler<DistortionCancelCommandBody, DistortionCancelResponseBody>
{
	protected override void HandleInGameCommand()
	{
		m_myHero.CancelDistortion(bSendEventToMyself: false);
		SendResponseOK(null);
	}
}
