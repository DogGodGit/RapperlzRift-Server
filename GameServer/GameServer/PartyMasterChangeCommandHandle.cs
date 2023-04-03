using System;
using ClientCommon;

namespace GameServer;

public class PartyMasterChangeCommandHandler : InGameCommandHandler<PartyMasterChangeCommandBody, PartyMasterChangeResponseBody>
{
	public const short kResult_NoPartyMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_TargetNotExist = 103;

	public const short kResult_TargetNotLoggedIn = 104;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "바디가 null입니다.");
		}
		Guid targetMemberId = (Guid)m_body.targetMemberId;
		if (targetMemberId == Guid.Empty)
		{
			throw new CommandHandleException(1, "대상멤버ID가 유효하지 않습니다.");
		}
		if (targetMemberId == m_myHero.id)
		{
			throw new CommandHandleException(1, "자신을 파티장으로 변경할 수 없습니다.");
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
		Party party = partyMember.party;
		PartyMember targetMember = party.GetMember(targetMemberId);
		if (targetMember == null)
		{
			throw new CommandHandleException(103, "대상멤버가 존재하지 않습니다.");
		}
		if (!targetMember.isLoggedIn)
		{
			throw new CommandHandleException(104, "대상멤버가 로그인 상태가 아닙니다.");
		}
		party.ChangeMaster(targetMember, m_myHero.id);
		SendResponseOK(null);
	}
}
