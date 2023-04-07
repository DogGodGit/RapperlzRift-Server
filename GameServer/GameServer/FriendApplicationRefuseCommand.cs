using ClientCommon;
using GameServer.CommandHandlers;

namespace GameServer;

public class FriendApplicationRefuseCommandHandler : InGameCommandHandler<FriendApplicationRefuseCommandBody, FriendApplicationRefuseResponseBody>
{
	public const short kResult_ApplicationNotExist = 101;

	private FriendApplication m_app;

	protected override bool globalLockRequired => true;

	protected override void HandleInGameCommand()
	{
		if (m_body == null)
		{
			throw new CommandHandleException(1, "body가 null입니다.");
		}
		long lnApplicationNo = m_body.applicationNo;
		if (lnApplicationNo <= 0)
		{
			throw new CommandHandleException(1, "신청번호가 유효하지 않습니다. lnApplicationNo = " + lnApplicationNo);
		}
		m_app = m_myHero.GetReceivedFriendApplication(lnApplicationNo);
		if (m_app == null)
		{
			throw new CommandHandleException(101, "신청이 존재하지 않습니다. lnApplicationNo = " + lnApplicationNo);
		}
		lock (m_app.applicant.syncObject)
		{
			Process();
		}
	}

	private void Process()
	{
		m_myHero.RefuseFriendApplication(m_app);
		SendResponseOK(null);
	}
}
