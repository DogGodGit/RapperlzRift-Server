using System;
using ClientCommon;

namespace GameServer.CommandHandlers;

public class PresentReplyCommandHandler : InGameCommandHandler<PresentReplyCommandBody, PresentReplyResponseBody>
{
    public const short kResult_TargetHeroNotLoggedIn = 101;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "body가 null입니다.");
        }
        Guid targetHeroId = m_body.targetHeroid;
        if (targetHeroId == Guid.Empty)
        {
            throw new CommandHandleException(1, "대상영웅ID가 유효하지 않습니다.");
        }
        Hero targetHero = Cache.instance.GetLoggedInHero(targetHeroId);
        if (targetHero == null)
        {
            throw new CommandHandleException(101, "대상영웅이 로그인하지 않았습니다. targetHeroId = " + targetHeroId);
        }
        ServerEvent.SendPresentReplyReceived(targetHero.account.peer, m_myHero.id, m_myHero.name, m_myHero.nationId);
        SendResponseOK(null);
    }
}
