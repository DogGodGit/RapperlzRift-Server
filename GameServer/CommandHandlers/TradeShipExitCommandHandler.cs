using ClientCommon;

namespace GameServer.CommandHandlers;

public class TradeShipExitCommandHandler : InGameCommandHandler<TradeShipExitCommandBody, TradeShipExitResponseBody>
{
    public const short kResult_NotStatusFinished = 101;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        if (!(m_myHero.currentPlace is TradeShipInstance currentPlace))
        {
            throw new CommandHandleException(1, "현재 장소에서 사용할 수 없는 명령입니다.");
        }
        if (!currentPlace.isFinished)
        {
            throw new CommandHandleException(101, "현재 상태에서 사용할 수 없는 명령입니다.");
        }
        if (m_myHero.isDead)
        {
            m_myHero.Revive(bSendEvent: false);
        }
        else
        {
            m_myHero.RestoreHP(m_myHero.realMaxHP, bSendEventToMyself: false, bSendEventToOthers: false);
        }
        currentPlace.Exit(m_myHero, isLogOut: false, null);
        TradeShipExitResponseBody resBody = new TradeShipExitResponseBody();
        resBody.previousContinentId = m_myHero.previousContinentId;
        resBody.previousNationId = m_myHero.previousNationId;
        resBody.hp = m_myHero.hp;
        SendResponseOK(resBody);
    }
}
