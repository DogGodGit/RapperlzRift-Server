using System;
using ClientCommon;

namespace GameServer;

public class PartyCreateCommandHandler : InGameCommandHandler<PartyCreateCommandBody, PartyCreateResponseBody>
{
	public const short kResult_AlreadyPartyMember = 101;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_myHero.partyMember != null)
		{
			throw new CommandHandleException(101, "이미 파티에 가입되어 있습니다.");
		}
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		Party party = new Party();
		party.Init(m_myHero);
		PartyCreateResponseBody resBody = new PartyCreateResponseBody();
		resBody.party = party.ToPDParty(currentTime);
		SendResponseOK(resBody);
	}
}
