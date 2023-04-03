using System;
using ClientCommon;

namespace GameServer;

public class PartyCallCommandHandler : InGameCommandHandler<PartyCallCommandBody, PartyCallResponseBody>
{
	public const short kResult_NoPartyMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_CoolTimeNotElapsed = 103;

	public const short kResult_NotAllowedPlace = 104;

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
		DateTimeOffset currentTime = DateTimeUtil.currentTime;
		if (!party.IsCallCoolTimeElapsed(currentTime))
		{
			throw new CommandHandleException(103, "쿨타임이 경과되지 않았습니다.");
		}
		if (!(m_myHero.currentPlace is ContinentInstance))
		{
			throw new CommandHandleException(104, "현재 장소에서 사용할 수 없습니다.");
		}
		party.Call(currentTime);
		SendResponseOK(null);
	}
}
