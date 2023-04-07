using ClientCommon;

namespace GameServer.CommandHandlers;

public class PartyExitCommandHandler : InGameCommandHandler<PartyExitCommandBody, PartyExitResponseBody>
{
    public const short kResult_NoPartyMember = 101;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        PartyMember partyMember = m_myHero.partyMember;
        if (partyMember == null)
        {
            throw new CommandHandleException(101, "파티에 가입되어있지 않습니다.");
        }
        Party party = partyMember.party;
        party.Exit(partyMember);
        SendResponseOK(null);
    }
}
