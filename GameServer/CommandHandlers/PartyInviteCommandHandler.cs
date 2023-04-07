using System;
using ClientCommon;

namespace GameServer.CommandHandlers;

public class PartyInviteCommandHandler : InGameCommandHandler<PartyInviteCommandBody, PartyInviteResponseBody>
{
    public const short kResult_NoPartyMember = 101;

    public const short kResult_NoAuthority = 102;

    public const short kResult_TargetNotExistOrDifferentNation = 103;

    public const short kResult_TargetAlreadyPartyMember = 104;

    public const short kResulT_TargetAlreadyInvited = 105;

    private Party m_party;

    private Hero m_targetHero;

    protected override bool globalLockRequired => true;

    protected override void HandleInGameCommand()
    {
        if (m_body == null)
        {
            throw new CommandHandleException(1, "바디가 null입니다.");
        }
        Guid targetHeroId = m_body.targetHeroId;
        if (targetHeroId == Guid.Empty)
        {
            throw new CommandHandleException(1, "대상영웅ID가 유효하지 않습니다.");
        }
        if (targetHeroId == m_myHero.id)
        {
            throw new CommandHandleException(1, "자신은 초대할 수 없습니다.");
        }
        PartyMember partyMember = m_myHero.partyMember;
        if (partyMember == null)
        {
            throw new CommandHandleException(101, "파티에 가입되어있지 않습니다.");
        }
        if (!partyMember.isMaster)
        {
            throw new CommandHandleException(102, "권한이 없습니다.");
        }
        m_party = partyMember.party;
        m_targetHero = m_myHero.nationInst.GetHero(targetHeroId);
        if (m_targetHero == null)
        {
            throw new CommandHandleException(103, "대상영웅이 존재하지 않거나 다른 국가 영웅입니다. targetHeroId = " + targetHeroId);
        }
        lock (m_targetHero.syncObject)
        {
            Process();
        }
    }

    private void Process()
    {
        DateTimeOffset currentTime = DateTimeUtil.currentTime;
        if (m_targetHero.partyMember != null)
        {
            throw new CommandHandleException(104, "대상영웅은 이미 파티멤버입니다.");
        }
        if (m_party.ContainsInvitationForHero(m_targetHero.id))
        {
            throw new CommandHandleException(105, "대상영웅은 이미 초대되었습니다.");
        }
        PartyInvitation invitation = m_party.Invite(m_targetHero, currentTime);
        PartyInviteResponseBody resBody = new PartyInviteResponseBody();
        resBody.invitation = invitation.ToPDPartyInvitation();
        SendResponseOK(resBody);
    }
}
