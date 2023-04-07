using System;
using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class PartyMemberBanishCommandHandler : InGameCommandHandler<PartyMemberBanishCommandBody, PartyMemberBanishResponseBody>
{
	public const short kResult_NoPartyMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_TargetNotExist = 103;

	private Party m_party;

	private PartyMember m_targetMember;

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
			throw new CommandHandleException(1, "자신은 추방할 수 없습니다.");
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
		m_targetMember = m_party.GetMember(targetMemberId);
		if (m_targetMember == null)
		{
			throw new CommandHandleException(103, "대상멤버가 존재하지 않습니다.");
		}
		if (m_targetMember.hero != null)
		{
			lock (m_targetMember.hero.syncObject)
			{
				Process();
				return;
			}
		}
		Process();
	}

	private void Process()
	{
		m_party.Banish(m_targetMember);
		SendResponseOK(null);
	}
}
