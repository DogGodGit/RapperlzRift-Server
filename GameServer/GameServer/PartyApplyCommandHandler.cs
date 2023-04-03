using System;
using ClientCommon;

namespace GameServer;

public class PartyApplyCommandHandler : InGameCommandHandler<PartyApplyCommandBody, PartyApplyResponseBody>
{
	public const short kResult_AlreadyPartyMember = 101;

	public const short kResult_PartyApplicationExist = 102;

	public const short kResult_PartyNotExist = 103;

	public const short kResult_DifferentNationParty = 104;

	public const short kResult_PartyMemberFull = 105;

	public const short kResult_MasterNotLoggedIn = 106;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "바디가 null입니다.");
		}
		Guid partyId = (Guid)m_body.partyId;
		if (partyId == Guid.Empty)
		{
			throw new CommandHandleException(1, "파티ID가 유효하지 않습니다.");
		}
		if (m_myHero.partyMember != null)
		{
			throw new CommandHandleException(101, "이미 파티에 가입되어 있습니다.");
		}
		if (m_myHero.ContainsPartyApplicationForParty(partyId))
		{
			throw new CommandHandleException(102, "대상 파티에 대한 신청이 존재합니다. partyId = " + partyId);
		}
		Party party = Cache.instance.GetParty(partyId);
		if (party == null)
		{
			throw new CommandHandleException(103, "파티가 존재하지 않습니다. partyId = " + partyId);
		}
		if (party.nationInst.nationId != m_myHero.nationId)
		{
			throw new CommandHandleException(104, "다른 국가의 파티입니다. partyId = " + partyId);
		}
		if (party.isMemberFull)
		{
			throw new CommandHandleException(105, "파티의 멤버가 모두 찼습니다. partyId = " + partyId);
		}
		if (!party.master.isLoggedIn)
		{
			throw new CommandHandleException(106, "파티장이 로그인중이 아닙니다. partyId = " + partyId);
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		PartyApplication app = m_myHero.ApplyToParty(party, currentTime);
		PartyApplyResponseBody resBody = new PartyApplyResponseBody();
		resBody.app = app.ToPDPartyApplication();
		SendResponseOK(resBody);
	}
}
