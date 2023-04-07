using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class PartyInvitationRefuseCommandHandler : InGameCommandHandler<PartyInvitationRefuseCommandBody, PartyInvitationRefuseResponseBody>
{
	public const short kResult_InvitationNotExist = 101;

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
		PartyInvitation invitation = m_myHero.GetPartyInvitation(lnInvitationNo);
		if (invitation == null)
		{
			throw new CommandHandleException(101, "초대가 존재하지 않습니다. lnInvitationNo = " + lnInvitationNo);
		}
		m_myHero.RefusePartyInvitation(invitation);
		SendResponseOK(null);
	}
}
