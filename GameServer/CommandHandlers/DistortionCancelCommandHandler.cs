using ClientCommon;

namespace GameServer.CommandHandlers;

public class DistortionCancelCommandHandler : InGameCommandHandler<DistortionCancelCommandBody, DistortionCancelResponseBody>
{
    protected override void HandleInGameCommand()
    {
        m_myHero.CancelDistortion(bSendEventToMyself: false);
        SendResponseOK(null);
    }
}
