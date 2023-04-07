using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class PartyApplicationRefuseCommandHandler : InGameCommandHandler<PartyApplicationRefuseCommandBody, PartyApplicationRefuseResponseBody>
{
	public const short kResult_NoPartyMember = 101;

	public const short kResult_NoAuthority = 102;

	public const short kResult_ApplicationNotExist = 103;

	private Party m_party;

	private PartyApplication m_app;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "바디가 null입니다.");
		}
		long lnApplicationNo = m_body.applicationNo;
		if (lnApplicationNo <= 0)
		{
			throw new CommandHandleException(1, "신청번호가 유효하지 않습니다. lnApplicationNo = " + lnApplicationNo);
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
		m_app = m_party.GetApplication(lnApplicationNo);
		if (m_app == null)
		{
			throw new CommandHandleException(103, "신청이 존재하지 않습니다. lnApplicationNo = " + lnApplicationNo);
		}
		lock (m_app.applicant.syncObject)
		{
			Process();
		}
	}

	private void Process()
	{
		m_party.RefuseApplication(m_app);
		SendResponseOK(null);
	}
}
