using System;
using ClientCommon;

namespace GameServer;

public class PartyInvitationAcceptCommandHandler : InGameCommandHandler<PartyInvitationAcceptCommandBody, PartyInvitationAcceptResponseBody>
{
	public const short kResult_AlreadyPartyMember = 101;

	public const short kResult_InvitationNotExist = 102;

	public const short kResult_PartyMemberFull = 103;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "바디가 null입니다.");
		}
		long lnInvitationNo = m_body.invitationNo;
		if (lnInvitationNo <= 0)
		{
			throw new CommandHandleException(1, "초대번호가 유효하지 않습니다. lnInvitationNo = " + lnInvitationNo);
		}
		if (m_myHero.partyMember != null)
		{
			throw new CommandHandleException(101, "이미 파티에 가입되어 있습니다.");
		}
		PartyInvitation invitation = m_myHero.GetPartyInvitation(lnInvitationNo);
		if (invitation == null)
		{
			throw new CommandHandleException(102, "초대가 존재하지 않습니다. lnInvitationNo = " + lnInvitationNo);
		}
		Party party = invitation.party;
		if (party.isMemberFull)
		{
			throw new CommandHandleException(103, "파티의 멤버가 모두 찼습니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		m_myHero.AcceptPartyInvitation(invitation);
		PartyInvitationAcceptResponseBody resBody = new PartyInvitationAcceptResponseBody();
		resBody.party = party.ToPDParty(currentTime);
		SendResponseOK(resBody);
	}
}
