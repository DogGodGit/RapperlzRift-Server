using ClientCommon;

namespace GameServer.CommandHandlers;

public class HeroLogoutCommandHandler : InGameCommandHandler<HeroLogoutCommandBody, HeroLogoutResponseBody>
{
    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        m_myHero.LogOut();
        SendResponseOK(null);
    }
}
