using System;
using ClientCommon;

namespace GameServer;

public class GuildInvitationRefuseCommandHandler : InGameCommandHandler<GuildInvitationRefuseCommandBody, GuildInvitationRefuseResponseBody>
{
	public const short kResult_InvitationNotExist = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		Guid invitationId = (Guid)m_body.invitationId;
		if (invitationId == Guid.Empty)
		{
			throw new CommandHandleException(1, "초대ID가 유효하지 않습니다. invitationId = " + invitationId);
		}
		GuildInvitation invitation = m_myHero.GetGuildInvitation(invitationId);
		if (invitation == null)
		{
			throw new CommandHandleException(101, "초대가 존재하지 않습니다. invitationId = " + invitationId);
		}
		m_myHero.RefuseGuildInvitation(invitation);
		SendResponseOK(null);
	}
}
