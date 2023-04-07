using ClientCommon;

namespace GameServer.CommandHandlers;

public class WarMemoryAbandonCommandHandler : InGameCommandHandler<WarMemoryAbandonCommandBody, WarMemoryAbandonResponseBody>
{
    public const short kResult_NotStatusPlayWaitingOrPlaying = 101;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        if (!(m_myHero.currentPlace is WarMemoryInstance currentPlace))
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        if (currentPlace.status != 2 && currentPlace.status != 1)
        {
            throw new CommandHandleException(101, "현재 상태에서 상용할 수 없는 명령입니다.");
        }
        currentPlace.Disqualification(m_myHero);
        if (m_myHero.isDead)
        {
            m_myHero.Revive(bSendEvent: false);
        }
        else
        {
            m_myHero.RestoreHP(hero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
        }
        currentPlace.Exit(m_myHero, isLogOut: false, null);
        WarMemoryAbandonResponseBody resBody = new WarMemoryAbandonResponseBody();
        resBody.previousContinentId = m_myHero.previousContinentId;
        resBody.previousNationId = m_myHero.previousNationId;
        resBody.hp = m_myHero.hp;
        SendResponseOK(resBody);
    }
}
