using ClientCommon;

namespace GameServer;

public class PartyDisbandCommandHandler : InGameCommandHandler<PartyDisbandCommandBody, PartyDisbandResponseBody>
{
	public const short kResult_NoPartyMember = 101;

	public const short kResult_NoAuthority = 102;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
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
		party.Disband();
		SendResponseOK(null);
	}
}
